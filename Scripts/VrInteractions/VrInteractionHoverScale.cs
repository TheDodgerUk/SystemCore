using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionHoverScale : VrInteraction
{

    private float m_ScaleTargetMulipyer = 1f;
    private float m_ScaleTime = 1f;

    private Vector3 m_StartScale;

    private float m_CurrentStep = 1f;
    protected override void Awake()
    {
        base.Awake();
        EnableOutlineCanBeUsed = true;
    }

    public void Initialise(float time, float newScaleMulipyer)
    {
        m_StartScale = gameObject.transform.localScale;
        m_ScaleTime = time;
        m_ScaleTargetMulipyer = newScaleMulipyer;
    }


    public override void OnHoverBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        this.Create<ValueTween>(m_ScaleTime, EaseType.SineInOut, () =>
        {

        }).Initialise(0f, 1f, (step) => m_CurrentStep = step, (f) =>
        {
            gameObject.transform.localScale = Vector3.Lerp(m_StartScale, m_StartScale * m_ScaleTargetMulipyer, f);
        });
    }

    public override void OnHoverEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        float localStep = 1f - m_CurrentStep;
        this.StopTweens<ValueTween>();
        this.Create<ValueTween>(m_ScaleTime, EaseType.SineInOut, () =>
        {

        }).Initialise(localStep, 1f, (f) =>
        {
            gameObject.transform.localScale = Vector3.Lerp(m_StartScale * m_ScaleTargetMulipyer, m_StartScale, f);
        });
    }
}
