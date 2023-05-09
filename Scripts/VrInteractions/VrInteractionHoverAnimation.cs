using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionHoverAnimation : VrInteraction
{

    private Animation m_Start;
    private Animation m_End;

    protected override void Awake()
    {
        base.Awake();
        EnableOutlineCanBeUsed = true;
    }


    public void Initialise(Animation start, Animation end)
    {
        m_Start = start;
        m_End = end;
    }

    public override void OnHoverBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_Start.Play();
    }

    public override void OnHoverEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_End.Play();
    }
}
