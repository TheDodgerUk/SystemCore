using System.Collections.Generic;
using UnityEngine;

public enum TriggerSfxType { Down, Success, Fail }

public class InputStateGfx : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_EulerAngles;
    [SerializeField]
    private Vector3 m_Displacement;

    private Dictionary<TriggerSfxType, AudioSource> m_Sfx;

    private Quaternion m_StartRotation;
    private Vector3 m_StartPosition;

    private Quaternion m_EndRotation;
    private Vector3 m_EndPosition;

    private GameObject m_Highlight;
    private GameObject m_Active;

    private AudioSource m_SfxDown;
    private AudioSource m_SfxUp;

    private Transform m_Transform;
    private bool m_IsHighlighted = false;

    private void Awake()
    {
        m_Transform = transform;

        m_Highlight = m_Transform.Search("highlight").gameObject;
        m_Active = m_Transform.Search("active").gameObject;

        m_StartRotation = m_Transform.localRotation;
        m_StartPosition = m_Transform.localPosition;

        m_EndRotation = m_StartRotation * Quaternion.Euler(m_EulerAngles);
        m_EndPosition = m_StartPosition + m_Displacement;

        m_Sfx = new Dictionary<TriggerSfxType, AudioSource>();
        foreach (var sfx in Utils.Code.GetEnumValues<TriggerSfxType>())
        {
            m_Sfx.Add(sfx, m_Transform.FindComponent<AudioSource>($"Sfx{sfx}"));
        }

        m_Highlight.SetActive(false);
        m_IsHighlighted = false;
    }

    public void PlaySfx(TriggerSfxType sfx) => m_Sfx.Get(sfx)?.Play();

    public void UpdateState(InputState state)
    {
        if(m_Transform == null)
        {
            Awake();
        }
        m_Transform.localRotation = Quaternion.Slerp(m_StartRotation, m_EndRotation, state.Value);
        m_Transform.localPosition = Vector3.Lerp(m_StartPosition, m_EndPosition, state.Value);
        m_Active.SetActive(state.Value > 0);
    }

    public void ToggleHighlight(bool state)
    {
        if (m_IsHighlighted != state)
        {
            m_Highlight.SetActive(true);
            m_Highlight.Create<MaterialFadeTween>(0.4f, EaseType.SineIn, () =>
            {
                m_Highlight.SetActive(state);
            }).Initialise(state ? 1f : 0f);
            m_IsHighlighted = state;
        }
    }
}
