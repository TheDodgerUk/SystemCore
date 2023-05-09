using System.Collections.Generic;
using UnityEngine;

public class EnvironmentVideoCamera : EnvironmentEffect
{
    private List<EnvironmentVideoCameraScreen> m_CameraScreens;
    private Light m_ScreenGlow;

    private RenderTexture m_Output;
    private Camera m_Camera;

    private float m_FrameRate = 23.98f;
    private float m_RenderInterval = 0;
    private float m_LastRenderTime = 0;

    private void Awake()
    {
        m_CameraScreens = new List<EnvironmentVideoCameraScreen>();
        m_ScreenGlow = transform.FindComponent<Light>("ScreenGlow");

        Vector2 res = new Vector2(1920f, 1080f);
        m_Output = new RenderTexture((int)res.x, (int)res.y, 16, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            autoGenerateMips = true,
            useMipMap = true,
            anisoLevel = 8,
            name = gameObject.name
        };
        m_Output.Create();

        m_Camera = transform.Find("CameraDummy").AddComponent<Camera>();
        m_Camera.stereoTargetEye = StereoTargetEyeMask.None;
        m_Camera.cullingMask = Layers.CameraRuntimeMask;
        m_Camera.targetTexture = m_Output;
        m_Camera.allowMSAA = false;
        m_Camera.allowHDR = false;
        m_Camera.fieldOfView = 38;
        m_Camera.enabled = false;
    }

    private void Update()
    {
        m_RenderInterval = 1f / m_FrameRate;
        if (m_LastRenderTime + m_RenderInterval < Time.time)
        {
            if (m_CameraScreens.Exists(s => s.IsVisible()))
            {
                m_Camera.Render();
            }
            while (m_LastRenderTime < Time.time)
            {
                m_LastRenderTime += m_RenderInterval;
            }
        }
    }

    [InspectorButton]
    public override void OnEnvironmentEffectsLoaded()
    {
        base.OnEnvironmentEffectsLoaded();

        //////var cameraScreens = Core.Environment.GetTaggedObjectByTag(Layers.VideoCameraTag);
        //////foreach (var screenObject in cameraScreens ?? new List<GameObject>())
        //////{
        //////    var screen = screenObject.AddComponent<EnvironmentVideoCameraScreen>();
        //////    screen.SetActive(Configs.System.VideoCameraEnabled);
        //////    screen.Initialise(m_Output);
        //////    m_CameraScreens.Add(screen);
        //////}
    }

    public override void OnSplashReset()
    {
        this.enabled = false;
    }

    public override void OnSplashComplete()
    {
        this.enabled = true;
    }

}