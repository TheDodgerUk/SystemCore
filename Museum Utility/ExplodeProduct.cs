using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExplodeProduct : MonoBehaviour
{
    // Used for the explosion.
    private Animator[] m_ExplosionAnimators = null;
    private int SPEED_HASH = Animator.StringToHash("Speed");
    private int EXPLODE_HASH = Animator.StringToHash("Explode");
    private int LAYER = 0;
    private bool m_bExploded = false;


    private void Start()
    {
        // Figure out if this model has an explosion animator.
        m_ExplosionAnimators = this.gameObject.GetComponentsInChildren<Animator>();

        for (int i = 0; i < m_ExplosionAnimators.Length; i++)
        {
            m_ExplosionAnimators[i].enabled = false;
        }
    }


    [EditorButton]
    public void ToggleExplodeProduct()
    {
        if (false == m_bExploded)
        {
            m_bExploded = true;
            ExplodeAnimation();
        }
        else
        {
            m_bExploded = false;
            ExplodeAnimation();
        }
    }

    [EditorButton]
    public void TriggerExplodeProduct()
    {
        if (false == m_bExploded)
        {
            m_bExploded = true;
            ExplodeAnimation();
        }
    }

    [EditorButton]
    public void TriggerResetProduct()
    {
        if (true == m_bExploded)
        {
            m_bExploded = false;
            ExplodeAnimation();
        }
    }


    private void ExplodeAnimation()
    {
        for (int i = 0; i < m_ExplosionAnimators.Length; i++)
        {
            // Enable the animator, it's disabled so that interaction allows things to move.
            m_ExplosionAnimators[i].enabled = true;

            // Annoyingly you still have to clamp the normalised animation time as it isn't actually normalised. #UnityProblems

            if (true == m_bExploded)
            {
                m_ExplosionAnimators[i].SetFloat(SPEED_HASH, 1.0f);
                m_ExplosionAnimators[i].Play(EXPLODE_HASH, LAYER, Mathf.Clamp01(m_ExplosionAnimators[i].GetCurrentAnimatorStateInfo(LAYER).normalizedTime));
            }
            else
            {
                m_ExplosionAnimators[i].SetFloat(SPEED_HASH, -1.0f);
                m_ExplosionAnimators[i].Play(EXPLODE_HASH, LAYER, Mathf.Clamp01(m_ExplosionAnimators[i].GetCurrentAnimatorStateInfo(LAYER).normalizedTime));
            }
        }
    }
}
