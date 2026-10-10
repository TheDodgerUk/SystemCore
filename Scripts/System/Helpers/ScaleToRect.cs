using UnityEngine;

[ExecuteInEditMode]
public class ScaleToRect : MonoBehaviour
{
    [SerializeField]
    private Transform m_Transform;
    [SerializeField]
    private RectTransform m_Target;

    [SerializeField]
    private bool m_SwitchAxes = false;

    private void Awake()
    {
        m_Transform = transform;
        m_Target = m_Transform.parent.ToRect();
    }

    private void Update() => ManualUpdate();

    public void ManualUpdate()
    {
        var s = m_Target.rect.size;
        if (m_SwitchAxes == true)
        {
            SetScale(s.y, s.x);
        }
        else
        {
            SetScale(s.x, s.y);
        }
    }

    private void SetScale(float x, float y)
    {
        m_Transform.SetScale(x.Abs(), y.Abs(), 1);
    }
}
