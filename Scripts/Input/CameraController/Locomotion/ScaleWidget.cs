using UnityEngine;

public class ScaleWidget : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_ScalePivot;

    private RectTransform m_Root;

    private float m_OriginalScale;

    private void Awake()
    {
        m_Root = transform.ToRect();
        m_OriginalScale = m_Root.localScale.x;
    }

    public void Toggle(bool state) => m_Root.SetActive(state);

    public void UpdateWidget(Vector3 a, Vector3 b, float scaleFactor)
    {
        float t = Easing.Ease(EaseType.QuadOut, scaleFactor);
        float width = (a - b).magnitude / m_Root.localScale.x;

        m_Root.position = (a + b) * 0.5f;
        m_Root.rotation = GetRotation(a, b);
        m_Root.SetWidth(width);
        m_Root.localScale = Vector3.one * m_OriginalScale * CameraControllerVR.Instance.Scale;
        m_ScalePivot.pivot = new Vector2(t, m_ScalePivot.pivot.y);
    }

    private static Quaternion GetRotation(Vector3 a, Vector3 b)
    {
        var up = CameraControllerVR.Instance.CameraTransform.up;
        var direction = Vector3.Cross(up, (a - b).normalized);
        if (direction.sqrMagnitude == 0)
        {
            return Quaternion.identity;
        }
        return Quaternion.LookRotation(direction);
    }
}
