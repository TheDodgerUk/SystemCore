using UnityEngine;

public abstract class ObjectTracker : MonoBehaviour
{
    [SerializeField]
    protected Transform m_Target;

    protected Transform m_Transform;

    protected virtual void Awake()
    {
        m_Transform = transform;
    }

    private void LateUpdate()
    {
        if (m_Target == null)
        {
            m_Target = Camera.main?.transform;
        }
        if (m_Target != null)
        {
            OnUpdate();
        }
    }

    protected abstract void OnUpdate();

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }
}
