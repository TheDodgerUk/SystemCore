using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ABEmotion
{
    Normal,
    Surprised,
    Angry,
    Happy,
    Sad,
    Worried,
    Suspicious,
    TheRock
};

[ExecuteInEditMode]
public class EyeEmotion : MonoBehaviour
{
    public ABEmotion m_CurrentEyeEmotion;
    public ABEmotion m_PrevEyeEmotion;
    private Animator m_Animator;

    // Start is called before the first frame update
    public void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void SetEmotion(ABEmotion emotion)
    {
        m_CurrentEyeEmotion = emotion;
        UpdateEmotion();
    }

    private void UpdateEmotion()
    {
        if (m_PrevEyeEmotion != m_CurrentEyeEmotion)
        {
            m_Animator?.SetInteger("Emotion", (int)m_CurrentEyeEmotion);
            m_PrevEyeEmotion = m_CurrentEyeEmotion;
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (false == Application.isPlaying)
        {
            m_Animator?.Update(0.15f);
        }
    }
#endif
}
