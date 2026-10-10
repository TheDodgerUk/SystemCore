using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_Speed = Vector3.zero;
    [SerializeField]
    private Space m_RelativeTo = Space.World;

    private Vector3 m_StartPosition;
    private Transform m_Transform;

    private void Awake()
    {
        m_Transform = transform;
        m_StartPosition = m_Transform.position;
    }

    private void OnEnable()
    {
        ResetPosition();
    }

    private void Update()
    {
        m_Transform.Translate(m_Speed * Time.deltaTime, m_RelativeTo);
    }

    public void ResetPosition() => m_Transform.position = m_StartPosition;

    public void SetSpeed(float x, float y, float z) => m_Speed = new Vector3(x, y, z);
}
