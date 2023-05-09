using System;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Animator))]
public class AnimationPlayer : MonoBehaviour
{
    [SerializeField, Range(0, 1)]
    private float m_FadeLength = 0.2f;

    private Dictionary<int, string> m_HashToNames;
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_HashToNames = new Dictionary<int, string>();
        foreach (var clip in m_Animator.runtimeAnimatorController.animationClips)
        {
            m_HashToNames.Add(Animator.StringToHash(clip.name), clip.name);
        }
    }

    public void Play(TransitionType transition, Action callback = null) => Play(transition.ToString(), callback);

    public void Play(string animationName, Action callback = null)
    {
        int hash = Animator.StringToHash(animationName);
        if (m_Animator.HasState(0, hash) == false)
        {
            Debug.LogWarning($"No clip called {animationName} found on {name}\n");
            return;
        }

        m_Animator.CrossFade(hash, m_FadeLength);

        var state = m_Animator.GetCurrentAnimatorStateInfo(0);

        m_Animator.Update(0f);

        var deltaState = m_Animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log($"State is: {m_HashToNames[state.shortNameHash]}");
        Debug.Log($"Delta state is: {m_HashToNames[deltaState.shortNameHash]}");

        //this.StopAllCoroutines();
        //this.WaitFor(deltaState.length, () =>
        //{
        //    m_Clip = null;
        //    this.enabled = false;
        //    m_Animation.GetClip
        //    //m_Animation.enabled = false;
        //    callback?.Invoke();
        //});
    }
}
