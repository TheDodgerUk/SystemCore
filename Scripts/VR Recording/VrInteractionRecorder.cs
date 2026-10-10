using System;
using UnityEngine;

public class VrInteractionRecorder : VrInteraction
{
    private GameObject m_RootGameObject;
    public Action RecordAction;
    private float m_MinDistance = 0.1f;
    private bool m_BeingGrabbed = false;
    private float m_CurrentTouchRotate = 0f;
    private float m_RotationSpeed = 2.5f;
    private float m_PushPullSpeed = 1.0f;
    private float m_CurrentPushDistance;
    private float m_MinSwipeDistance = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }

    public override void OnUpdateClickStick(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.Hand == Handedness.Left)
        {
            return;
        }

        if (true == m_BeingGrabbed)
        {
            Vector2 direction = interaction.Main.Inputs.Stick.Value2;
            if (Vector2.Distance(direction, Vector2.zero) < m_MinSwipeDistance * 2f)
            {
                float YAxis = interaction.Main.Inputs.Stick.Value2.y;
                UpdatePushPullDistance(YAxis);
                m_CurrentTouchRotate = interaction.Main.Inputs.Stick.Value2.y;

                m_RootGameObject.transform.Rotate(Vector3.up, (m_CurrentTouchRotate * m_RotationSpeed));
            }
        }
    }

    private void UpdatePushPullDistance(float distance)
    {
        if (Mathf.Abs(distance) > m_MinSwipeDistance)
        {
            m_CurrentPushDistance += ((distance > 0f) ? m_PushPullSpeed : -m_PushPullSpeed) * Time.deltaTime;

            //Maintain min distance when pushing and pulling
            m_CurrentPushDistance = Mathf.Max(m_MinDistance, m_CurrentPushDistance);
        }
    }

    public override void EndClickStick(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.Hand == Handedness.Left)
        {
            return;
        }
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.Hand == Handedness.Left)
        {
            if (m_RootGameObject.GetComponent<EnvironmentVideoCameraScreen>())
            {
                RecordAction?.Invoke();
            }
        }
        else
        {
            m_BeingGrabbed = true;
            interaction.Main.m_UseOverrideBeamColour = true;
            interaction.Main.m_OverrideBeamColour = Color.green;
            interaction.Main.LockToObject(this);
            m_RootGameObject.transform.SetParent(interaction.Main.WristTransform);
        }
    }

    public override void OnUpdateLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.Hand == Handedness.Left)
        {
            return;
        }

        interaction.Main.m_OverrideBeamColour = Color.green;
        m_RootGameObject.transform.position = interaction.Main.RaycastTransform.position + (interaction.Main.RaycastTransform.forward * m_CurrentPushDistance) - (transform.rotation * Vector3.zero);
    }

    public override void EndLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.Hand == Handedness.Left)
        {
            return;
        }
        m_BeingGrabbed = false;
        interaction.Main.m_UseOverrideBeamColour = false;
        interaction.Main.LockToObject(null);
        m_RootGameObject.transform.SetParent(null);
    }
}

//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class VideoCamera : EnvironmentEffect
//{
//    private VideoCameraScreen m_Screen;
//    private Light m_ScreenGlow;

//    private RenderTexture m_Output;
//    private Camera m_Camera;

//    private float m_FrameRate = 23.98f;
//    private float m_RenderInterval = 0;
//    private float m_LastRenderTime = 0;
//    private GameObject m_Cube;
//    private bool m_IsRecording = false;
//    private void Awake()
//    {
//        m_ScreenGlow = transform.FindComponent<Light>("ScreenGlow");

//        var res = Configs.System.VideoCameraResolution;
//        m_Output = new RenderTexture((int)res.x, (int)res.y, 16, RenderTextureFormat.ARGB32)
//        {
//            filterMode = FilterMode.Trilinear,
//            wrapMode = TextureWrapMode.Clamp,
//            autoGenerateMips = true,
//            useMipMap = true,
//            anisoLevel = 8,
//            name = gameObject.name
//        };
//        m_Output.Create();

//        m_Camera = transform.Find("CameraDummy").AddComponent<Camera>();
//        m_Cube = transform.Find("Cube").gameObject;
//        m_Camera.stereoTargetEye = StereoTargetEyeMask.None;
//        m_Camera.cullingMask = Layers.RuntimeMask;
//        m_Camera.targetTexture = m_Output;
//        m_Camera.allowMSAA = false;
//        m_Camera.allowHDR = false;
//        m_Camera.fieldOfView = 38;
//        m_Camera.enabled = false;

//        m_Camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Tutorial"));
//    }

//    private void Extra()
//    {
//        m_Camera.enabled = true;
//        m_Camera.fieldOfView = 60;
//        m_Camera.tag = "VideoCamera";
//        m_Camera.transform.SetParent(m_Cube.transform);
//        m_Camera.backgroundColor = Color.black;
//        m_Cube.AddComponent<VrInteractionDrag>();
//        var interaction = m_Screen.AddComponent<VrInteractionDrag>();
//        interaction.RecordAction = ToggleRecord;

//        m_Screen.ToggleRedDot(false);
//        m_Screen.gameObject.SetLayerRecursively(21);
//        m_Screen.GetComponentInChildren<Collider>().gameObject.SetLayerRecursively(8);
//        Scene sc = this.gameObject.scene;
//        var list = sc.GetRootGameObjects().ToList();
//        GameObject head = list.FindLast(e => e.name == "[HeadModel]");
//        head.transform.SetParent(CameraControllerVR.Instance.CameraTransform, false);
//        head.transform.localPosition = new Vector3(0, 0, -0.1f);
//        head.transform.localEulerAngles = Vector3.zero;
//        head.transform.SetScale(2.5f, 2.5f, 2.5f);
//    }

//    private void Update()
//    {
//        m_RenderInterval = 1f / m_FrameRate;
//        if (m_LastRenderTime + m_RenderInterval < Time.time)
//        {
//            if (m_Screen != null)
//            {
//                m_Camera.Render();
//            }
//            while (m_LastRenderTime < Time.time)
//            {
//                m_LastRenderTime += m_RenderInterval;
//            }
//        }
//    }

//    private void ToggleRecord()
//    {
//        m_IsRecording = !m_IsRecording;

//        if (m_IsRecording)
//        {
//            m_Camera.AddComponent<ScreenRecorder>();

//            m_Screen.ToggleRedDot(true);
//        }
//        else
//        {
//            Destroy(m_Camera.GetComponent<ScreenRecorder>());
//            m_Screen.ToggleRedDot(false);
//        }
//        //if (recorderWindow.IsRecording())
//        //{
//        //    recorderWindow.StopRecording();
//        //    m_Screen.ToggleRedDot(false);
//        //}
//        //else
//        //{
//        //    recorderWindow.StartRecording();
//        //    m_Screen.ToggleRedDot(true);
//        //}
//    }


//    [InspectorButton]
//    public override void OnEnvironmentEffectsLoaded()
//    {
//        base.OnEnvironmentEffectsLoaded();

//        GameObject screenObject = transform.Find("VideoCameraScreen").gameObject;

//        m_Screen = screenObject.AddComponent<VideoCameraScreen>();
//        m_Screen.SetActive(Configs.System.VideoCameraEnabled);
//        m_Screen.Initialise(m_Output);

//        foreach (var lightingConsole in Core.Environment.GetFx<LightingConsole>())
//        {
//            lightingConsole.LinkVideoCamera(OnLightingSetupChanged);
//        }

//        this.SetActive(Configs.System.VideoCameraEnabled);
//        Extra();
//    }

//    public override void OnSplashReset()
//    {
//        this.enabled = false;
//    }

//    public override void OnSplashComplete()
//    {
//        this.enabled = true;
//    }

//    private void OnLightingSetupChanged(LightingConsole.LightingSetup setup, int index)
//    {
//        m_ScreenGlow.color = setup.colorA;

//        //foreach (var screen in m_CameraScreens)
//        //{
//        //    screen.UpdatePreset(index);
//        //}
//    }
//}