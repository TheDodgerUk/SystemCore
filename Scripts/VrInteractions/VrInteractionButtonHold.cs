using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionButtonHold : VrInteractionButtonLatched
{

    private bool m_Hold;
    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {      
        if (m_InteractionsAllowed == true)
        {
            Down();
        }
    }

    public override void OnUpdateLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_Hold = true;
    }

    public override void EndLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_Hold = false;
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
        this.WaitUntil(1, () => m_Hold == false, () =>
       {
           bool useEmmisive = m_UseEmmisive;
           this.Create<ValueTween>(m_AnimationTime, EaseType.SineInOut, () =>
           {
               StateChange();
               m_InteractionsAllowed = true;
           }).Initialise(0, 1, (f) =>
           {
               gameObject.transform.localPosition = Vector3.Lerp(m_EndLocalPosition, m_StartLocalPosition, f);
           });
       });
    }


}

