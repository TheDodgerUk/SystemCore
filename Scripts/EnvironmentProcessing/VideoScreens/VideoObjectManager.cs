using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

public abstract class VideoObjectManager : EnvironmentEffect
{
    private static Dictionary<string, VideoObjectManager> m_DictOfVideoManagers = new Dictionary<string, VideoObjectManager>();

    public static VideoObjectManager GetScreenManager(string sUID)
    {
        if(true == m_DictOfVideoManagers.ContainsKey(sUID))
        {
            return m_DictOfVideoManagers[sUID];
        }

        return null;
    }
    private static readonly Vector2 MirroredTextureTiling = new Vector2(-1f, 1f);
    private static readonly Vector2 MirroredTextureOffset = new Vector2(1f, 0f);

    protected abstract string MaterialName { get; }

    [Range(0, 1)]
    public float Emission = 1f;
    private float m_LastEmissionValue;

    public string ScreenName => m_ScreenName;

    protected RenderTexture m_VideoTexture;
    protected VideoPlayer m_VideoPlayer;
    protected Material m_VideoMaterial;
    protected Texture m_VideoTextureBlack;

    private VideoObjectManager m_RemappedScreen;
    private VideoSequenceData m_SequenceData;
    private VideoObjectData m_ScreenData;
    private VideoClip m_SplashClip;
    private string m_ScreenName;

    private int m_iMaxVideoCount = 2;
    private int m_iVideoCount = 2;

    private List<Action> m_OnPlay = new List<Action>();
    private List<Action> m_OnStop = new List<Action>();

    public override bool Initialise()
    {
        Video360Manager Video360ManagerRef = GetComponent<Video360Manager>();
        m_ScreenName = GetObjectName();
        if (true == string.IsNullOrEmpty(m_ScreenName))
        {
            return false;
        }
        Debug.Log($"VideoObjectManager looking for m_ScreenName: {m_ScreenName}");

        m_iVideoCount = m_iMaxVideoCount;
        //Load the video data for this screen, this determines which videos it will play and if it will remap to another
        //screen after the intro sequence has finished playing
        m_SequenceData = VideoLoader.Instance.GetVideoSequenceData(m_ScreenName);
        m_ScreenData = VideoLoader.Instance.GetScreenData(m_ScreenName);
        if (null == Video360ManagerRef)
        {
            m_VideoMaterial = new Material(Resources.Load<Material>(MaterialName));
        }
        else
        {
            m_VideoMaterial = new Material(Shader.Find("Custom/360VideoRenderer"));
        }
        m_VideoTextureBlack = Resources.Load<Texture2D>("Black");
        if (null == m_ScreenData)
        {
            //We cannot run this screen without data for it, disable screen
            //m_VideoMaterial.mainTexture = m_VideoTextureBlack;

            Debug.Log($"Could not find SequenceData {VideoLoader.Instance.CurrentSequenceDataFolder}");
            Debug.Log($"Could not find CurrentData  {VideoLoader.Instance.CurrentDataFolder}");
            Debug.Log($"m_ScreenData is null, look for VideoData.json, the screen name needs to be in it:  {m_ScreenName}", this);
            //return false;
        }

        if (string.IsNullOrEmpty(m_ScreenName) == false)
        {
            m_DictOfVideoManagers[m_ScreenName] = this;
        }

        var res = m_ScreenData?.Resolution ?? new Vector2(512, 512);
        
        m_VideoTexture = new RenderTexture((int)res.x, (int)res.y, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            autoGenerateMips = true,
            useMipMap = true,
            name = gameObject.name,
        };
        m_VideoTexture.Create();

        SetVideoTexture(m_VideoTexture, false);

        SetupVideoPlayer();

         m_LastEmissionValue = -1f;

        //this.enabled = false;
        return true;
    }


    private void OnDestroy()
    {
        for(int i = 0; i < m_OnPlay.Count; i++)
        {
            m_OnPlay[i] = null;
        }
        m_OnPlay.Clear();

        for (int i = 0; i < m_OnStop.Count; i++)
        {
            m_OnStop[i] = null;
        }
        m_OnStop.Clear();
    }

    private void SetupVideoPlayer()
    {
        m_VideoPlayer = gameObject.GetComponentInChildren<VideoPlayer>();
        if (null == m_VideoPlayer)
        {
            m_VideoPlayer = gameObject.ForceComponent<VideoPlayer>();
        }
        m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        m_VideoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        m_VideoPlayer.source = VideoSource.Url;

        m_VideoPlayer.prepareCompleted += OnVideoPrepared;
        m_VideoPlayer.loopPointReached += OnVideoEnded;
        m_VideoPlayer.targetTexture = m_VideoTexture;
    }

    private void Update()
    {
        if (Mathf.Abs(m_LastEmissionValue - Emission) > Mathf.Epsilon)
        {
            m_LastEmissionValue = Emission;
            ApplyEmission(Emission);
        }
    }

    public void SetSplashVideos(List<VideoClip> splashVideos)
    {
        splashVideos.RemoveAll(item => item == null);
        if (splashVideos.Count != 0)
        {
            if (string.IsNullOrEmpty(m_ScreenData.StartVideo) == false)
            {
                m_SplashClip = splashVideos.Find(v => v.name == m_ScreenData.StartVideo);
            }
        }
    }

    public override void OnEnvironmentEffectsLoaded()
    {
        if (string.IsNullOrEmpty(m_ScreenData.RemappedScreen) == false)
        {
            string remapName = m_ScreenData.RemappedScreen;
            var videoManagers = Core.Environment.GetFx<VideoObjectManager>();
            m_RemappedScreen = videoManagers.Find(v => v.m_ScreenName == remapName);
        }
    }

    public override void OnSplashReset()
    {
        if (m_SplashClip != null)
        {
            m_VideoPlayer.clip = m_SplashClip;
            m_VideoPlayer.source = VideoSource.VideoClip;
            SetVideoTexture(m_VideoTexture, false);

            m_VideoPlayer.prepareCompleted -= OnVideoPrepared;
            m_VideoPlayer.loopPointReached -= OnVideoEnded;

            m_VideoTexture.Clear();
            m_VideoPlayer.Stop();
            m_VideoPlayer.Prepare();
        }

    }

    public override void OnSplashComplete()
    {
        m_VideoPlayer.Stop();
        m_VideoTexture.Clear();

        bool mirrored = m_ScreenData?.MirrorImage ?? false;

        //Should this screen be remapped to another screen?
        if (m_RemappedScreen != null)
        {
            SetVideoTexture(m_RemappedScreen.m_VideoTexture, mirrored);
            m_RemappedScreen.m_OnPlay.Add(Play);
            m_RemappedScreen.m_OnStop.Add(Stop);
            m_VideoPlayer.enabled = false;
        }
        else
        {
            //Video is not being mapped to another video manager... allow it to run normally
            m_VideoPlayer.prepareCompleted += OnVideoPrepared;
            m_VideoPlayer.loopPointReached += OnVideoEnded;
            SetVideoTexture(m_VideoTexture, mirrored);
            if (true == string.IsNullOrEmpty(m_ScreenData.StartVideo))
            {
                if (true == m_ScreenData.ChangeVideoOnCompleted)
                {
                    SetRandomVideo();
                }
            }
            else
            {
                Play(m_ScreenData.StartVideo);
            }
        }

    }

    public void Play()
    {
        m_VideoPlayer.enabled = true;
        this.enabled = true;
        Emission = 1.0f;
    }

    public void GetURLViaGenericVideoLoader(string name , Action<string> callBackUrl)
    {
        Core.AssetBundlesRef.VideoClipStreamingAssetRef.GetItem(this, name, (source) =>
        {
            callBackUrl?.Invoke(source.URL);
        });
    }


    public void PlayViaGenericVideoLoader(string name )
    {
        Core.AssetBundlesRef.VideoClipStreamingAssetRef.GetItem(this,name, (source) =>
        {
            PlayDirectFromURL(source.URL);
            Debug.Log($"fakeDictonaryItemRef {source}");
        });
    }


    public void PlayDirectFromURL(string sURL)
    {
        SetVideoClip(sURL);
        Play();
    }

    public void Play(string sFileName)
    {
        SetVideoClip(VideoLoader.Instance.GetVideo(sFileName));
        Play();
        foreach(var item in m_OnPlay)
        {
            item?.Invoke();
        }
    }

    public void Stop()
    {
        Emission = 0.0f;
        this.enabled = false;
        foreach (var item in m_OnStop)
        {
            item?.Invoke();
        }
    }

    protected abstract void ApplyEmission(float emission);

    private void SetRandomVideo()
    {
        if (m_ScreenData != null && m_ScreenData.VideoNames != null && m_ScreenData.VideoNames.Count > 0)
        {
            string videoName = m_ScreenData.VideoNames.GetRandom();
            SetVideoClip(VideoLoader.Instance.GetVideo(videoName));
        }
    }

    private void SetVideoClip(string url)
    {
        if (false == string.IsNullOrEmpty(url))
        {
            if(m_VideoPlayer == null)
            {
                Debug.LogError($"m_VideoPlayer is null, {this.gameObject.GetGameObjectPath()}", this);
            }
            m_VideoPlayer.url = url;
            m_VideoPlayer.source = VideoSource.Url;
            m_VideoPlayer.prepareCompleted -= OnVideoPrepared;
            m_VideoPlayer.prepareCompleted += OnVideoPrepared;
            m_VideoPlayer.Prepare();
        }
    }

    private void SetVideoTexture(RenderTexture texture, bool mirrored)
    {
        m_VideoMaterial.mainTextureOffset = mirrored ? MirroredTextureOffset : Vector2.zero;
        m_VideoMaterial.mainTextureScale = mirrored ? MirroredTextureTiling : Vector2.one;
        m_VideoMaterial.mainTexture = texture;
    }

    [InspectorButton]
    private void Pause() => m_VideoPlayer.Pause();
    [InspectorButton]
    private void Resume() => m_VideoPlayer.Play();

    protected virtual void OnVideoPrepared(VideoPlayer player)
    {
        m_VideoPlayer.Play();
    }

    protected virtual void OnVideoEnded(VideoPlayer player)
    {
        if (null != m_ScreenData)
        {
            if (true == m_ScreenData.ChangeVideoOnCompleted)
            {
                if (null != m_SequenceData)
                {
                    m_iVideoCount--;
                    if (m_iVideoCount <= 0)
                    {
                        m_iVideoCount = m_iMaxVideoCount;
                        VideoSequence sequence = m_SequenceData.SequenceList.GetRandom();

                        sequence.Sequence.ForEach((e) =>
                        {
                            if (true == m_DictOfVideoManagers.ContainsKey(e.TargetScreen))
                            {
                                string url = VideoLoader.Instance.GetVideo(e.VideoName);
                                m_DictOfVideoManagers[e.TargetScreen].SetVideoClip(url);
                            }
                        });
                    }
                }
            }        
            //If we should not change video on complete, return without changing video
            else
            {
                //Skip to playing video
                m_VideoPlayer.Play();
                return;
            }
        }

        SetRandomVideo();
    }
}