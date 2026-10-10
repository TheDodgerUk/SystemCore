using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionButtonLatched : VrInteractionBaseButton
{

    protected override void Awake()
    {
        base.Awake();
        m_Renderer = gameObject.GetComponent<Renderer>();
    }


    public void Initialise(float animationTime, float movementAmount, Action<ButtonStateEnum> callBack)
    {
        m_ButtonStateEnumCallBack = callBack;
        m_AnimationTime = animationTime;
        m_StartLocalPosition = gameObject.transform.localPosition;
        m_EndLocalPosition = gameObject.transform.localPosition + new Vector3(0f, movementAmount, 0f);
    }



    public void EnableEmissive(bool enable)
    {
        m_EnableEmissive = enable;
        ChangeEmission();
    }


    public override void SetInstantStateWithoutCallback(ButtonStateEnum buttonState)
    {
        bool tempAllow = AllowCallBack;
        AllowCallBack = false;
        ButtonState = buttonState;
        AllowCallBack = tempAllow;

        if (m_ButtonState == ButtonStateEnum.Down)
        {
            gameObject.transform.localPosition = m_EndLocalPosition;
        }
        else
        {
            gameObject.transform.localPosition = m_StartLocalPosition;
        }
    }

    public ButtonStateEnum GetState()
    {
        return m_ButtonState;
    }
    public void SetState(ButtonStateEnum state)
    {
        if(state != m_ButtonState)
        {
            BeginLaser(new ControllerStateInteraction());
        }
    }


    public override void BeginFingerTouch(GameObject fingerTransform)
    {
        BeginLaser(new ControllerStateInteraction());
    }



    public void PhontonCallback_Begin()
    {
        BeginLaser(new ControllerStateInteraction(), false);
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {      
        if (m_InteractionsAllowed == true)
        {
#if Photon
            if(m_PhotonVrInteractionButton != null && m_PhotonVrInteractionButton.CanSendMessage() && sendPhotonMessage == true)
            {
                m_PhotonVrInteractionButton.SendPhotonBegin();
            }
#endif

            bool useEmmisive = m_UseEmmisive;
            if (m_ButtonState == ButtonStateEnum.Up)
            {
                useEmmisive = ((Emmisive == EmmisiveEnum.Down || Emmisive == EmmisiveEnum.Both) && true == useEmmisive);
                this.Create<ValueTween>(m_AnimationTime, EaseType.SineInOut, () =>
                {
                    ButtonState = ButtonStateEnum.Down;
                    m_InteractionsAllowed = true;
                }).Initialise(0, 1, (f) =>
                {
                    if ((true == useEmmisive) && (f > 0.5f))
                    {
                        useEmmisive = false;
                        m_Renderer.material.SetColor(EMISSION_COLOR_ID, m_EmmisiveDown);
                    }
                    m_InteractionsAllowed = false;

                    gameObject.transform.localPosition = Vector3.Lerp(m_StartLocalPosition, m_EndLocalPosition, f);
                });
            }
            if (m_ButtonState == ButtonStateEnum.Down)
            {
                useEmmisive = ((Emmisive == EmmisiveEnum.Up || Emmisive == EmmisiveEnum.Both) && true == useEmmisive);
                this.Create<ValueTween>(m_AnimationTime, EaseType.SineInOut, () =>
                {
                    ButtonState = ButtonStateEnum.Up;
                    m_InteractionsAllowed = true;
                }).Initialise(0, 1, (f) =>
                {
                    if ((true == useEmmisive) && (f > 0.5f))
                    {
                        useEmmisive = false;
                        m_Renderer.material.SetColor(EMISSION_COLOR_ID, m_EmmisiveUp);
                    }
                    m_InteractionsAllowed = false;

                    gameObject.transform.localPosition = Vector3.Lerp(m_EndLocalPosition, m_StartLocalPosition, f);
                });
            }
        }
    }



    protected void StateChange()
    {
        if (ButtonState == ButtonStateEnum.Down)
        {
            ButtonState = ButtonStateEnum.Up;
        }
        else
        {
            ButtonState = ButtonStateEnum.Down;
        }
    }

}

