using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EnvironmentConsoleScreen : EnvironmentEffect
{
    private const float SqrVisibleDistance = VisibleDistance * VisibleDistance;
    private const float VisibleDistance = 2f;

    private List<VideoClipStreamingAssetInformation> m_Videos;

    private VideoPlayer m_VideoPlayer;
    private RenderTexture m_Output;
    private Renderer m_Renderer;
    private Material m_Material;
    private Transform m_ScreenRoot;

    private void Awake()
    {
        // create render target
        Vector2 res = new Vector2(1920f, 1080f);
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

        // create material
        m_Material = Resources.Load<Material>("Materials/ConsoleScreen");
        m_Material.mainTexture = m_Output;



        // add video player component
        m_VideoPlayer = gameObject.AddComponent<VideoPlayer>();
        m_VideoPlayer.prepareCompleted += OnVideoPrepared;
        m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        m_VideoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        m_VideoPlayer.source = VideoSource.Url;
        m_VideoPlayer.targetTexture = m_Output;
        m_VideoPlayer.playOnAwake = false;
        m_VideoPlayer.isLooping = true;

        m_ScreenRoot = transform.Find("ScreenRoot");
        m_Renderer = m_ScreenRoot.GetComponentInChildren<Renderer>();
        m_Renderer.material = m_Material;
    }

    private void Update()
    {
        if (m_VideoPlayer.isPrepared == true)
        {
            bool isPlaying = m_VideoPlayer.isPlaying;
            bool isVisible = m_Renderer.IsVisible(m_ScreenRoot, SqrVisibleDistance);
            if (isVisible == true && isPlaying == false)
            {
                Play();
            }
            else if (isVisible == false && isPlaying == true)
            {
                Pause();
            }
        }
    }

    public override void OnEnvironmentEffectsLoaded()
    {
        base.OnEnvironmentEffectsLoaded();

        string objectName = GetObjectName();
        if (string.IsNullOrEmpty(objectName) == false)
        {
            // load idle video

            m_Videos = new List<VideoClipStreamingAssetInformation>();
            Core.AssetBundlesRef.VideoClipStreamingAssetRef.GetItemList((list) =>
            {
                foreach(var itemName in list)
                {
                    Core.AssetBundlesRef.VideoClipStreamingAssetRef.GetItem(this, itemName, (item) =>
                    {
                        m_Videos.Add(item);
                    });
                }
            });
        }
    }

    public override void OnSplashReset()
    {
        m_VideoPlayer.Stop();
    }

    public override void OnSplashComplete()
    {
        m_VideoPlayer.url = m_Videos[0].URL;
        m_VideoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        m_VideoPlayer.Play();
    }

    private void OnInfoPanelOpened(int panelIndex)
    {
        m_VideoPlayer.url = m_Videos[panelIndex + 1].URL;
        m_VideoPlayer.Prepare();
    }

    private void OnInfoPanelClosed(int panelIndex)
    {
        OnSplashComplete();
    }

    [InspectorButton]
    private void Pause() => m_VideoPlayer.Pause();

    [InspectorButton]
    private void Play() => m_VideoPlayer.Play();

}