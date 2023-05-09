using UnityEngine;

public abstract class ForceAwakeMono : MonoBehaviour
{
    protected bool m_IsAwakened = false;

    public void ForceAwake() => Awake();

    private void Awake()
    {
        if (m_IsAwakened == false)
        {
            Awaken();
        }
        m_IsAwakened = true;
    }

    protected abstract void Awaken();
}
