using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenUI : MonoBehaviour
{
    protected RenderTexture m_RenderTexture;
    [SerializeField]
    private float m_fCameraOrthoSize = 0.1f;
    protected Camera m_UICamera;
    protected int m_LayerMask;

    [SerializeField]
    private string m_CameraLocation;
    protected Transform m_CameraTransform;

    public string UILayer = "UI";

    // Use this for initialization
    public void Construct(string cameraTransform, string uiLayer, float cameraSize)
    {
        m_CameraLocation = cameraTransform;
        UILayer = uiLayer;
        m_fCameraOrthoSize = cameraSize;

        m_LayerMask = LayerMask.NameToLayer(UILayer);
        SetupCamera();
        SetupRenderTexture();
    }

    public virtual void Init(Material mat)
    { }

    private void SetupCamera()
    {
        m_CameraTransform = transform.Find(m_CameraLocation);
        Camera camera = m_CameraTransform.GetComponent<Camera>();
        camera.transform.localPosition = Vector3.zero;

        camera.backgroundColor = Color.black;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.cullingMask = 1 << m_LayerMask;

        camera.orthographic = true;
        camera.depth = -10;
        camera.orthographicSize = m_fCameraOrthoSize;

        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 0.5f;

        m_UICamera = camera;
    }

    private void SetupRenderTexture()
    {
        m_RenderTexture = new RenderTexture(512, 512, 0);
        m_RenderTexture.wrapMode = TextureWrapMode.Clamp;
        m_UICamera.forceIntoRenderTexture = true;
        m_UICamera.targetTexture = m_RenderTexture;
    }

    public void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}