using System.Collections.Generic;
using UnityEngine;

public class EnvironmentVideoCameraScreen : MonoBehaviour
{
    private const float SqrVisibleDistance = VisibleDistance * VisibleDistance;
    private const float VisibleDistance = 2.5f;

    private List<Texture2D> m_Overlays;
    private Transform m_Transform;
    private Material m_Material;
    private Renderer m_Renderer;
    private GameObject m_RecordingMarker;

    public void Initialise(RenderTexture display)
    {
        m_Transform = transform;
        m_RecordingMarker = transform.Find("Sphere").gameObject;
        // create material 
        var displayAspect = GetDisplayAspect(display.width, display.height);
        var mainAspect = GetScreenAspect(displayAspect);
        m_Material = new Material(Resources.Load<Material>("VideoCamera/VideoScreen"))
        {
            mainTextureOffset = GetScreenOffset(mainAspect),
            mainTextureScale = mainAspect,
            mainTexture = display
        };

        // load + assign primary overlay
        string overlayName = GetOverlayName();
        var overlay = Resources.Load<Texture2D>(overlayName);
        m_Material.SetTexture("_PrimaryOverlay", overlay);

        // load secondary overlays
        m_Overlays = new List<Texture2D>();
        for (int i = 0; i >= 0; ++i)
        {
            var tex = Resources.Load<Texture2D>($"{overlayName}{i}");
            if (tex != null)
            {
                m_Overlays.Add(tex);
            }
            else
            {
                break;
            }
        }

        // assign material
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.material = m_Material;
        ToggleRedDot(false);
    }

    public void ToggleRedDot(bool state)
    {
        m_RecordingMarker.SetActive(state);
    }

    public bool IsVisible() => m_Renderer.IsVisible(m_Transform, SqrVisibleDistance);

    public void UpdatePreset(int index)
    {
        m_Material.SetTexture("_SecondaryOverlay", m_Overlays.Get(index, ListGet.Nothing));
    }

    private void OnDrawGizmosSelected()
    {
        var camera = CameraControllerVR.Instance?.CameraTransform ?? Camera.main.transform;
        if (camera != null && m_Transform != null)
        {
            bool isInRange = false;
            bool isFacingDirection = false;
            var dir = camera.position - m_Transform.position;
            if (dir.sqrMagnitude < SqrVisibleDistance == true)
            {
                // is the renderer facing towards the camera?
                float dot = dir.Dot(m_Transform.forward);
                isFacingDirection = dot > 0;
                isInRange = true;
            }

            Gizmos.color = isInRange ? Color.green : Color.yellow;
            Gizmos.DrawLine(camera.position, m_Transform.position);

            Gizmos.color = isFacingDirection ? Color.green : Color.blue;
            Gizmos.DrawRay(m_Transform.position, m_Transform.forward);

            Gizmos.color = isInRange ? Color.cyan : Color.magenta;
            Gizmos.DrawWireSphere(m_Transform.position, VisibleDistance);
        }
    }

    private string GetOverlayName() => $"VideoCamera/Overlay{name}";

    private Vector2 GetDisplayAspect(int width, int height)
    {
        if (width < height)
        {
            return new Vector2(height / (float)width, 1f);
        }
        else
        {
            return new Vector2(1f, width / (float)height);
        }
    }

    private Vector2 GetScreenAspect(Vector2 displayAspect)
    {
        var size = transform.localScale.Multiply(displayAspect);
        if (size.x < size.y)
        {
            return new Vector2(size.x / size.y, 1f);
        }
        else
        {
            return new Vector2(1f, size.y / size.x);
        }
    }

    private static Vector2 GetScreenOffset(Vector2 aspect)
    {
        return new Vector2(GetScreenOffset(aspect.x), GetScreenOffset(aspect.y));
    }

    private static float GetScreenOffset(float f) => (f - 1) * -0.5f;
}