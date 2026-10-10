using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;

[RequireComponent(typeof(PhotonVrInteractionButton))]
#endif
public class VrInteractionBaseButton : VrInteraction
{
    public enum ButtonStateEnum
    {
        Up,
        Down,
    }

    public enum EmmisiveEnum
    {
        Up,
        Down,
        Both,
    }

    protected EmmisiveEnum Emmisive;

    protected Action<ButtonStateEnum> m_ButtonStateEnumCallBack;
    [SerializeField]
    protected ButtonStateEnum m_ButtonState = ButtonStateEnum.Up;

    public bool AllowCallBack = true;

#if Photon
    protected PhotonVrInteractionButton m_PhotonVrInteractionButton;
#endif

    protected Vector3 m_StartLocalPosition;
    protected Vector3 m_EndLocalPosition;

    protected Renderer m_Renderer;
    protected Color m_EmmisiveUp = Color.white;
    protected Color m_EmmisiveDown = Color.white;
    protected bool m_UseEmmisive = true;
    protected readonly int EMISSION_COLOR_ID = Shader.PropertyToID("_EmissionColor");
    protected UnityEngine.Rendering.LocalKeyword ENABLE_EMISSION_ID;

    protected string ENABLE_EMISSION = "_EMISSION";
    protected bool m_EnableEmissive = true;

    protected ContentButtonMetaData.ButtonItemData m_ButtonItemData;
    protected float m_AnimationTime;
    private int m_PressedCount = 0;
    public bool AllowPress { get; set; } = true;

    [SerializeField]
    public ButtonStateEnum ButtonState
    {
        get { return m_ButtonState; }

        set
        {
            if(AllowPress == false)
            {
                return;
            }
            if(m_ButtonState != value)
            {
                ForceButtonState(value);
#if UNITY_EDITOR
                m_PressedCount++;
                if(m_PressedCount > 100)
                {
                    // this is a brutal way to check the the button not going on and off
                    // i found a button that did this on its own, 
                    // via bad limits and classing with another button 
                    // DO NOT REMOVE
                    Debug.Log($"Button pressed a Lot, check if bad button{this.gameObject.GetGameObjectPath()}");
                    m_PressedCount = 0;
                }
#endif
            }
        }
    }


    protected override void InternalGameStarted()
    {
        // when use this the decals turn pink
        ENABLE_EMISSION_ID = new UnityEngine.Rendering.LocalKeyword(m_Renderer.material.shader, ENABLE_EMISSION);
    }

    public void ForceButtonState(ButtonStateEnum newState)
    {
        Debug.Log($"Button Changed to {newState}, {this.gameObject.GetGameObjectPath()}");
        m_ButtonState = newState;
        ChangeEmission();

        SimpleButtonStateSetActive();
        SimpleButtonStateEmission();

        if (AllowCallBack)
        {
            m_ButtonStateEnumCallBack?.Invoke(m_ButtonState);
            m_PercentageCallback?.Invoke((float)m_ButtonState);
        }
    }

    private void SimpleButtonStateSetActive()
    {
        if (m_ButtonItemData != null)
        {
            if (m_ButtonItemData.m_SimpleButtonType == ContentButtonMetaData.SimpleButtonType.SetActive)
            {
                if (m_ButtonItemData.m_SimpleButtonPress == ContentButtonMetaData.ButtonItemData.SimpleButtonPress.Toggle)
                {
                    if (m_ButtonState == ButtonStateEnum.Down)
                    {
                        m_ButtonItemData.m_SetActive.GameObjectRef?.SetActive(true);
                    }
                    else
                    {
                        m_ButtonItemData.m_SetActive.GameObjectRef?.SetActive(false);
                    }
                }
                else
                {
                    if (m_ButtonItemData.m_SimpleButtonPress == ContentButtonMetaData.ButtonItemData.SimpleButtonPress.On)
                    {
                        m_ButtonItemData.m_SetActive.GameObjectRef?.SetActive(true);
                    }
                    else
                    {
                        m_ButtonItemData.m_SetActive.GameObjectRef?.SetActive(false);
                    }
                }
            }
        }
    }

    private void SimpleButtonStateEmission()
    {
        if (m_ButtonItemData != null)
        {
            if (m_ButtonItemData.m_SimpleButtonType == ContentButtonMetaData.SimpleButtonType.Emission)
            {
                if (m_ButtonItemData.m_SimpleButtonPress == ContentButtonMetaData.ButtonItemData.SimpleButtonPress.Toggle)
                {
                    if (m_ButtonState == ButtonStateEnum.Down)
                    {
                        m_ButtonItemData.m_SetEmmissive.RendererObj?.material.EnableKeyword(ENABLE_EMISSION);
                    }
                    else
                    {
                        m_ButtonItemData.m_SetEmmissive.RendererObj?.material.DisableKeyword(ENABLE_EMISSION);
                    }
                }
                else
                {
                    if (m_ButtonItemData.m_SimpleButtonPress == ContentButtonMetaData.ButtonItemData.SimpleButtonPress.On)
                    {
                        m_ButtonItemData.m_SetEmmissive.RendererObj?.material.EnableKeyword(ENABLE_EMISSION);
                    }
                    else
                    {
                        m_ButtonItemData.m_SetEmmissive.RendererObj?.material.DisableKeyword(ENABLE_EMISSION);
                    }
                }
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
#if Photon
        m_PhotonVrInteractionButton = this.gameObject.ForceComponent<PhotonVrInteractionButton>();
#endif
        m_Renderer = this.GetComponentInChildren<Renderer>();
        ENABLE_EMISSION_ID = new UnityEngine.Rendering.LocalKeyword(m_Renderer.material.shader, ENABLE_EMISSION);
        var keys = m_Renderer.material.shader.keywordSpace.keywords.ToList();
        bool hasEmission = false;
        foreach (var item in keys)
        {
            if (item.name.Contains(ENABLE_EMISSION) == true)
            {
                hasEmission = true;
            }
        }

        if (hasEmission == false)
        {
            Debug.LogError($"m_Renderer.material.shader {m_Renderer.material.shader.name} not contain _EMISSION", this.gameObject);
        }
    }

    public void Initialise(GameObject root, ContentButtonMetaData.ButtonItemData data, Action<ButtonStateEnum> callBack)
    {
        m_ButtonItemData = data;
        m_ButtonStateEnumCallBack = callBack;
        m_AnimationTime = data.m_ModelMoveTime;
        m_StartLocalPosition = data.m_ModelGameObject.transform.localPosition;

        switch (data.m_MoveType)
        {
            case ContentSliderMetaData.ItemData.MoveType.GameObject:
                m_EndLocalPosition = data.m_ModelEndLocalPositionGameObject.transform.localPosition;
                break;

            case ContentSliderMetaData.ItemData.MoveType.Amount:
                m_EndLocalPosition = m_StartLocalPosition;
                m_EndLocalPosition += new Vector3(0f, data.m_ModelEndLocalPositionfloat, 0f);
                break;
            default:
                break;
        }

        m_UseEmmisive = data.m_UseEmmision;
        m_EmmisiveUp = data.m_Up;
        m_EmmisiveDown = data.m_Down;
        Emmisive = EmmisiveEnum.Both;

        // Get details for instruction mode or VR tooltips.
        FriendlyName = data.m_sTitle;
        ItemDescription = data.m_sDescription;

        bool allow = AllowCallBack;
        AllowCallBack = false;
        ForceButtonState(m_ButtonState);
        AllowCallBack = allow;
        m_Renderer.material.SetColor(EMISSION_COLOR_ID, Color.black);
        m_Renderer.material.DisableKeyword(ENABLE_EMISSION);
    }


    public void ClearAddCallback(Action<ButtonStateEnum> stateCallback)
    {
        m_ButtonStateEnumCallBack = null;
        m_ButtonStateEnumCallBack += stateCallback;
    }
    public void AddCallback(Action<ButtonStateEnum> stateCallback)
    {
        m_ButtonStateEnumCallBack += stateCallback;
    }

    public virtual void SetInstantStateWithoutCallback(ButtonStateEnum buttonState)
    {
        bool tempAllow = AllowCallBack;
        AllowCallBack = false;
        ButtonState = buttonState;
        AllowCallBack = tempAllow;
    }

    public void EnableEmmisive(bool enable)
    {
        if (enable == true)
        {
            m_Renderer.material.EnableKeyword(ENABLE_EMISSION);
        }
        else
        {
            m_Renderer.material.DisableKeyword(ENABLE_EMISSION);
        }
    }

    public  void ChangeEmission()
    {
        if (true == m_UseEmmisive)
        {
            bool emmissionSet = false;
            if (ButtonState == ButtonStateEnum.Up && (Emmisive == EmmisiveEnum.Up || Emmisive == EmmisiveEnum.Both))
            {
                emmissionSet = true;
                m_Renderer.material.SetColor(EMISSION_COLOR_ID, m_EmmisiveUp);
            }

            if (ButtonState == ButtonStateEnum.Down && (Emmisive == EmmisiveEnum.Down || Emmisive == EmmisiveEnum.Both))
            {
                emmissionSet = true;
                m_Renderer.material.SetColor(EMISSION_COLOR_ID, m_EmmisiveDown);
            }

            if (false == emmissionSet)
            {
                m_Renderer.material.DisableKeyword(ENABLE_EMISSION);
            }
            else
            {
                m_Renderer.material.EnableKeyword(ENABLE_EMISSION);
            }
        }

        if (m_EnableEmissive == false)
        {
            m_Renderer.material.DisableKeyword(ENABLE_EMISSION);
        }
        DynamicGI.UpdateEnvironment();
    }
}

