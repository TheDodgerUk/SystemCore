using UnityEngine;

public class SonarPulse : MonoBehaviour
{
    [SerializeField]
    private bool m_Loop = false;

    private AnimatorPlayer m_Animation;
    private AudioSource m_Sfx;

    private Transform m_Transform;
    private Transform m_Parent;

    private void Awake()
    {
        m_Transform = transform;
        m_Parent = m_Transform.parent;
        m_Animation = m_Transform.GetComponent<AnimatorPlayer>();
        m_Sfx = m_Transform.GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        ResetPulse();
    }

    public void Trigger(ControllerData controller)
    {
        controller.Vibrate(VibrateType.Tick);

        Trigger();
    }

    [InspectorButton]
    public void Trigger()
    {
        Core.Scene.ReturnToScene(m_Transform);
        m_Transform.localScale = Vector3.one;
        m_Animation.Play("Reset");
        m_Animation.Play("Play", () =>
        {
            if (m_Loop == false)
            {
                m_Animation.Play("Reset");
                m_Transform.SetParent(m_Parent);
                m_Transform.Reset();
            }
            else
            {
                m_Transform.OrientTo(m_Parent, false);
                Trigger();
            }
        });
        m_Sfx.Play();
    }

    public void ResetPulse()
    {
        if (m_Animation) m_Animation.Play("Reset");
        if (m_Sfx) m_Sfx.Stop();
    }
}
