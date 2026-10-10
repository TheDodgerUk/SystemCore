using UnityEngine;

[RequireComponent(typeof(SoundEffect))]
public class DetachedAudioSource : MonoBehaviour
{
    [SerializeField]
    private bool m_AutoDetach = false;

    private SoundEffect m_SoundEffect;
    private Transform m_Transform;
    private Transform m_Target;

    private void Awake()
    {
        m_Transform = transform;
        m_Target = m_Transform.parent;

        m_SoundEffect = GetComponent<SoundEffect>();
    }

    private void Start()
    {
        if (m_AutoDetach == true)
        {
            Detach();
        }
    }

    private void Update()
    {
        m_Transform.position = m_Target.position;
    }

    public void Detach() => m_Transform.SetParent(m_Target.parent);

    public void FadeIn(float duration) => m_SoundEffect.FadeIn(duration, 0f);

    public void FadeOut(float duration) => m_SoundEffect.FadeOut(duration);

    public void SetTargetPitch(float pitch) => m_SoundEffect.SetTargetPitch(pitch);
}
