using UnityEngine;
using System;

public abstract class TutorialStep : MonoBehaviour
{
    public Action OnActivateCallback;
    public Action OnCompleteCallback;
    [SerializeField]
    protected bool m_bActionTriggered;
    [SerializeField]
    protected bool m_bStepActive;
    
    public virtual void ActivateStep()
    {
        this.enabled = true;
        m_bStepActive = true;
        m_bActionTriggered = false;
        OnActivateCallback?.Invoke();
    }

    protected void TutorialStepComplete()
    {
        if (false == m_bActionTriggered)
        {
            m_bStepActive = false;
            m_bActionTriggered = true;
            OnCompleteCallback?.Invoke();
            this.enabled = false;
        }
    }

    [InspectorButton]
    public void CompleteStep() => TutorialStepComplete();
}