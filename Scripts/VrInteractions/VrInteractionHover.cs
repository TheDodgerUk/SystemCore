using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionHover: VrInteraction
{
    private Action m_CallBackStart;
    private Action m_CallBackEnd;

    protected override void Awake()
    {
        base.Awake();
        EnableOutlineCanBeUsed = true;
    }



    public void Initialise(Action callBackStart, Action callBackEnd)
    {
        m_CallBackStart = callBackStart;
        m_CallBackEnd = callBackEnd;
    }


    public override void OnHoverBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_CallBackStart?.Invoke();
    }

    public override void OnHoverEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_CallBackEnd?.Invoke();
    }
}
