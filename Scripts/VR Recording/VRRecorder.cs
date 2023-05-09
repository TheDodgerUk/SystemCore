using UnityEngine;
using UnityEngine.SceneManagement;

/*Script package that allows you to use a camera inside of VR and record footage in a build. Footage comes out as frames in "E:/recordings/ScreenRecorder/ScreenRecorder".
 * How to use:
 * Add [VRRecordCamera] and headman prefabs from the catalog project in to any scene of your project.
 * Controls are: Right hand click moves the camera and viewfinder screen, right hand touchpad rotates and moves away and close.
 * Left hand interact on the viewfinder screen will start recording. There will be a red dot in the corner to show this.
 * Make sure to not close the unity application until the frames are done rendering even after recording is stopped.
 * Make sure to move your frames to a different location otherwise they will be overwritten when the next recording starts.
 * Total recording time is set to 10 minutes.
 */
public class VRRecorder : MonoBehaviour
{
    private EnvironmentVideoCameraScreen m_Screen;
    private RenderTexture m_Output;
    private Camera m_Camera;
    private GameObject m_Model;

    private float m_FrameRate = 30f;
    private float m_RenderInterval = 0;
    private float m_LastRenderTime = 0;

    private bool m_IsRecording = false;

    public void Initialise()
    {
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
        m_Camera.backgroundColor = Color.black;
        m_Camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Tutorial"));
        m_Camera.enabled = true;
        m_Camera.fieldOfView = 60;
        m_Camera.tag = "VideoCamera";

        GameObject screenObject = transform.Find("VideoCameraScreen").gameObject;

        m_Screen = screenObject.AddComponent<EnvironmentVideoCameraScreen>();
        m_Screen.SetActive(false);
        m_Screen.Initialise(m_Output);

        m_Screen.ToggleRedDot(false);
        m_Screen.gameObject.SetLayerRecursively(21);
        m_Screen.GetComponentInChildren<Collider>().gameObject.SetLayerRecursively(8);


        m_Model = transform.Find("CameraModel").gameObject;
        m_Camera.transform.SetParent(m_Model.transform);

        m_Model.AddComponent<VrInteractionRecorder>();
        var interaction = m_Screen.AddComponent<VrInteractionRecorder>();
        interaction.RecordAction = ToggleRecord;

        AddHeadModel();

        this.SetActive(false);
    }

    private void AddHeadModel()
    {
        Scene sc = this.gameObject.scene;
        var list = sc.GetRootGameObjects().ToList();
        GameObject head = list.FindLast(e => e.name == "[HeadModel]");
        head.transform.SetParent(CameraControllerVR.Instance.CameraTransform, false);
        head.transform.localPosition = new Vector3(0, 0, -0.1f);
        head.transform.localEulerAngles = Vector3.zero;
        head.transform.SetScale(2.5f, 2.5f, 2.5f);
    }
    private void Update()
    {
        m_RenderInterval = 1f / m_FrameRate;
        if (m_LastRenderTime + m_RenderInterval < Time.time)
        {
            if (m_Screen != null)
            {
                m_Camera.Render();
            }
            while (m_LastRenderTime < Time.time)
            {
                m_LastRenderTime += m_RenderInterval;
            }
        }
    }

    private void ToggleRecord()
    {
        m_IsRecording = !m_IsRecording;

        if (m_IsRecording)
        {
            m_Camera.AddComponent<ScreenRecorder>();

            m_Screen.ToggleRedDot(true);
        }
        else
        {
            Destroy(m_Camera.GetComponent<ScreenRecorder>());
            m_Screen.ToggleRedDot(false);
        }
    }
}