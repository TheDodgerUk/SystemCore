using UnityEngine;

public class Billboard : ObjectTracker
{
    [SerializeField]
    private bool m_FlipFace = true;

    [SerializeField]
    private bool m_Laggy = false;
    [SerializeField]
    private float m_Speed = 1f;

    protected override void OnUpdate()
    {
        if (m_Laggy == false)
        {
            m_Transform.LookAt(m_Target, Vector3.up);

            if (m_FlipFace == true)
            {
                m_Transform.Rotate(0, 180, 0, Space.Self);
            }
        }
        else
        {
            var direction = m_Target.position - m_Transform.position;
            var target = Quaternion.LookRotation(direction, Vector3.up);
            if (m_FlipFace == true)
            {
                target = target * Quaternion.Euler(0, 180, 0);
            }

            float t = Time.deltaTime * m_Speed;
            var rotation = m_Transform.rotation;
            m_Transform.rotation = Quaternion.Slerp(rotation, target, t);
        }
    }
}
