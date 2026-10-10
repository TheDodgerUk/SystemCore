using UnityEngine;

public class DistanceScale : ObjectTracker
{
    [Space]
    [SerializeField]
    private float m_MinDistance = 0f;
    [SerializeField]
    private float m_MaxDistance = 100f;

    [Space]
    [SerializeField]
    private float m_MinSize = 1f;
    [SerializeField]
    private float m_MaxSize = 10f;

    protected override void OnUpdate()
    {
        float d = Vector3.Distance(m_Transform.position, m_Target.position);
        float t = d.InverseLerp(m_MinDistance, m_MaxDistance);
        float size = t.Lerp(m_MinSize, m_MaxSize);
        m_Transform.localScale = Vector3.one * size;
    }
}
