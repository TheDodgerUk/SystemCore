using EnvironmentHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VrInterationVideoPlayer : VrInteraction
{
    public static readonly int TILING_ID_SECONDARY = Shader.PropertyToID("_DetailAlbedoMap");
    private GameObject m_RootGameObject;
    private ContentVideoPlayerMetaData m_ContentVideoPlayerMetaData;

    private VideoPlayer m_VideoPlayer;
    private RenderTexture m_Output;
    private Material m_Material;
    private Transform m_ScreenRoot;
    private VideoClip m_VideoClip;
    private Renderer m_Renderer;

    private Bounds m_OriginalBounds;

    private string m_VideoName = "";
    private GameObject m_VideoObject;


    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }


    public void Initialise(ContentVideoPlayerMetaData data, GameObject objectAttachedTo)
    {
        m_ContentVideoPlayerMetaData = data;

        string gameObjectName = m_ContentVideoPlayerMetaData.m_ScreenGameObjectName;

        if (string.IsNullOrEmpty(gameObjectName) == true)
        {
            Debug.LogError($"m_ContentVideoPlayerMetaData.m_ScreenName is null");
            gameObjectName = "Screen";
        }
        m_VideoObject = this.transform.Search(gameObjectName).gameObject;
        m_OriginalBounds = m_VideoObject.GetEncapsulatingBounds();

        if (m_ContentVideoPlayerMetaData.m_DisplayType == ContentVideoPlayerMetaData.DisplayType.LED)
        {
            var script = objectAttachedTo.AddComponent<LEDPanelVideoManager>();
            script.Initialise();
            m_Renderer = m_VideoObject.GetComponentInChildren<Renderer>(true);
            m_VideoPlayer = m_VideoObject.GetComponentInChildren<VideoPlayer>(true);
        }
        else
        {
            m_Renderer = m_VideoObject.GetComponent<Renderer>();
            m_VideoPlayer = m_VideoObject.ForceComponent<VideoPlayer>();
            SetMaterial();
        }     
    }

    private void SetMaterial()
    {

        if (m_Material == null)
        {
            // create material
            m_Material = Resources.Load<Material>("Materials/ConsoleScreen");
            m_Material.mainTexture = m_Output;
            m_Material.SetTexture(TILING_ID_SECONDARY, null);
        }
    }

    private void SetVideo(string video)
    {
        m_VideoName = video;
        Core.AssetBundlesRef.VideoClipAssetBundleRef.GetItem(this, video, (clip) =>
        {
            m_VideoClip = clip;
            if (m_ContentVideoPlayerMetaData.m_DisplayType != ContentVideoPlayerMetaData.DisplayType.LED)
            {
                CreateOutPut();
            }
            SetVideo();
        });
    }

    private void SetRandomMaterialAndVideo(Action callback)
    {
        TaskAction task = new TaskAction(1, () => { callback.Invoke(); });

        Core.AssetBundlesRef.VideoClipAssetBundleRef.GetItemList(Core.Mono, (items) =>
        {
            int index = items.GetRandomIndex();
            m_VideoName = items[index];
            Core.AssetBundlesRef.VideoClipAssetBundleRef.GetItem(this, items[index], (clip) =>
            {
                m_VideoClip = clip;
                if (m_ContentVideoPlayerMetaData.m_DisplayType != ContentVideoPlayerMetaData.DisplayType.LED)
                {
                    CreateOutPut();
                }
                SetVideo();
                task.Increment();
            });
        });
    }

    private void CreateOutPut()
    {
        if (m_VideoClip != null)
        {
            Debug.Log($"Video clip {m_VideoClip.name} {m_VideoClip.width} , {m_VideoClip.height}");
        }

        var res = new Vector2(1024, 1024);
        m_Output = new RenderTexture((int)res.x, (int)res.y, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            autoGenerateMips = true,
            useMipMap = true,
            anisoLevel = 8,
            name = gameObject.name
        };
        m_Output.Create();
        m_Material.mainTexture = m_Output;
    }

    private void SetVideo()
    {
        if (m_ContentVideoPlayerMetaData.m_DisplayType != ContentVideoPlayerMetaData.DisplayType.LED)
        {

            if (m_Renderer != null)
            {
                m_Renderer.material = m_Material;
            }

            m_VideoPlayer.prepareCompleted += OnVideoPrepared;
            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_VideoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            m_VideoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            m_VideoPlayer.source = VideoSource.VideoClip;
            m_VideoPlayer.clip = m_VideoClip;
            m_VideoPlayer.targetTexture = m_Output;
            m_VideoPlayer.playOnAwake = true;
            m_VideoPlayer.isLooping = true;
            m_VideoPlayer.Prepare();
        }
        else
        {
            if(m_VideoPlayer == null)
            {
                m_VideoPlayer = m_RootGameObject.GetComponentInChildren<VideoPlayer>(true);
            }
            m_VideoPlayer.prepareCompleted += OnVideoPrepared;
            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            m_VideoPlayer.aspectRatio = VideoAspectRatio.Stretch;
            m_VideoPlayer.source = VideoSource.Url;
            m_VideoPlayer.clip = m_VideoClip;
            m_VideoPlayer.playOnAwake = true;
            m_VideoPlayer.isLooping = true;
            m_VideoPlayer.Prepare();
        }

        ////float Y = ((float)m_VideoPlayer.clip.height) / ((float)m_VideoPlayer.clip.width);

        ////float dd = m_OriginalBounds.Value.size.y / m_OriginalBounds.Value.size.x;
        ////videoObject.transform.localScale = new Vector3(1, Y/dd, 1);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        m_VideoPlayer.Play();
    }


    private new void OnEnable()
    {
        if (m_VideoPlayer != null && m_VideoPlayer.isPrepared)
        {
            m_VideoPlayer.Play();
        }
    }

    [InspectorButton]
    private void Play() => m_VideoPlayer.Play();


    public string VideoName
    {
        get
        {
            return m_VideoName;
        }
        set
        {

            SetVideo(value);
        }
    }


    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if(m_VideoPlayer.isPlaying)
        {
            m_VideoPlayer.Pause();
        }
        else
        {
            m_VideoPlayer.Play();
        }
    }

}

