using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionButtonToggle : VrInteractionButtonLatched
{

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (m_InteractionsAllowed == true)
        {
            Down();
        }
    }

    public override void SetInstantStateWithoutCallback(ButtonStateEnum buttonState)
    {
        m_ButtonState = buttonState;

    }

    private void Down()
    {
        m_InteractionsAllowed = false;
        bool useEmmisive = m_UseEmmisive;

        this.Create<ValueTween>(m_AnimationTime, EaseType.SineInOut, () =>
        {
            StateChange();
            Up();
        }).Initialise(0, 1, (f) =>
        {         
            gameObject.transform.localPosition = Vector3.Lerp(m_StartLocalPosition, m_EndLocalPosition, f);
        });
    }

    private void Up()
    {
        bool useEmmisive = m_UseEmmisive;
        this.Create<ValueTween>(m_AnimationTime, EaseType.SineInOut, () =>
        {
            m_InteractionsAllowed = true;
        }).Initialise(0, 1, (f) =>
        {
            gameObject.transform.localPosition = Vector3.Lerp(m_EndLocalPosition, m_StartLocalPosition, f);
        });
    }


}

