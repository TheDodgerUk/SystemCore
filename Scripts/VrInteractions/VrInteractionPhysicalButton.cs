using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteractionPhysicalButton : VrInteractionBaseButton
{
    private float m_Threshold = 0.1f;
    private float m_DeadZone = 0.025f;

    private Vector3 m_StartPosition;
    private ConfigurableJoint m_Joint;

    private bool m_AllowPress = true;

    private float m_LimitMultiplyer = 0;
    private Vector3 m_LocalPosition;

    public void Initialise(ContentButtonMetaData.ButtonItemData data, Action<ButtonStateEnum> callBack)
    {
        m_ButtonItemData = data; 
        m_ButtonStateEnumCallBack = callBack;

        m_StartPosition = this.transform.localPosition;
        m_Joint = this.gameObject.ForceComponent<ConfigurableJoint>();

        m_UseEmmisive = data.m_UseEmmision;
        m_EmmisiveDown = data.m_Down;
        Emmisive = EmmisiveEnum.Down;

        bool allow = AllowCallBack;
        AllowCallBack = false;

        m_Renderer.material.SetColor(EMISSION_COLOR_ID, Color.black);
        m_Renderer.material.DisableKeyword(ENABLE_EMISSION);

        ForceButtonState(m_ButtonState);
        AllowCallBack = allow;


        var render = this.GetComponent<Renderer>();
        if(render != null)
        {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        render = m_ButtonItemData.m_ModelGameObject.GetComponent<Renderer>();
        if (render != null)
        {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        m_LimitMultiplyer = 1f / m_Joint.linearLimit.limit;
    }


    private void Update()
    {
        if(m_LocalPosition == this.transform.localPosition)
        {
            // optimization
            return;
        }
        m_LocalPosition = this.transform.localPosition;


        float springValue = GetNormalisedPressed();
        if((m_AllowPress == true) && (springValue + (m_Threshold * m_Joint.linearLimit.limit) >= 1f)) // button down 
        {
            m_AllowPress = false;
            if (ButtonState == ButtonStateEnum.Up)
            {
                ButtonState = ButtonStateEnum.Down;
            }
            else
            {
                ButtonState = ButtonStateEnum.Up;
            }
        }

        if ((springValue - (m_Threshold * m_Joint.linearLimit.limit) <= 0f)) // button up
        {
            m_AllowPress = true;
        }

        // limits the upwards movement
        if (this.transform.localPosition.y > 0f)
        {
            this.transform.localPosition = Vector3.zero;
        }
    }

    private float GetNormalisedPressed()
    {
        var value = Vector3.Distance(m_StartPosition, this.transform.localPosition) * m_LimitMultiplyer;
        if(Math.Abs(value) < (m_Joint.linearLimit.limit * m_DeadZone))
        {
            value = 0f;
        }
        return Mathf.Clamp(value, -1f, 1f);
    }

    [EditorButton]
    private void DEBUG_PressButton()
    {
        if(ButtonState == ButtonStateEnum.Down)
        {
            ButtonState = ButtonStateEnum.Up;
        }
        else
        {
            ButtonState = ButtonStateEnum.Down;
        }
    }
}

