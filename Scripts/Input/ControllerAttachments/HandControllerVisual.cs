using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerVisual : ControllerVisual
{
    public static readonly Color m_MainHandColor = new Color(0.35f, 0.85f, 1f, 0.4f);
    public static readonly Color m_OffHandColor = new Color(1f, 1f, 1f, 0.15f);

    private Animator m_HandAnimator;
    private string m_Grip = "Grip";
    private string m_Trigger = "Trigger";
    private Material m_HandMaterial;
    private BeamVisual m_LaserBeam;
    private Transform m_WristTransform;
    private Color m_ColourState;

    private bool m_bisExtended = false;
    private bool m_bShowUI = false;

    private UIBezier m_HandUI;
    private float m_fBeamScale;

    public void SetExtendedHand(bool IsExtended)
    {
        m_bisExtended = IsExtended;
        //Turn off UI immediately
        DisplayControllerHints();
    }

    public override void Init(ControllerGraphics gfx)
    {
        base.Init(gfx);

        m_HandAnimator = m_BaseController.GetComponent<Animator>();
        Transform Hand = m_BaseController.Search("RHand");
        m_HandMaterial = GetMaterial(Hand);

        m_HandUI = gameObject.GetComponentInChildren<UIBezier>();

        m_WristTransform = transform.Search("Wrist");

        m_LaserBeam = GetComponentInChildren<BeamVisual>();

        m_HandUI?.SetAllVisible(false);
    }

    public Transform GetBeamAttachPoint()
    {
        return m_WristTransform;
    }

    public override void Visible(bool bShow)
    {
        base.Visible(bShow);

        if (m_HandMaterial != null)
        {
            if (true == bShow)
            {
                m_HandMaterial.SetFloat(FADE_IN, 1.0f);
                m_HandMaterial.SetFloat(DISSOLVE, 1.0f);
            }
            else
            {
                m_HandMaterial.SetFloat(FADE_IN, 0.0f);
                m_HandMaterial.SetFloat(DISSOLVE, 0.0f);
            }
        }
        else
        {
            Debug.LogError("Cannot find materail");
        }

    }

    public override void OnGrabBegin(ControllerData controller)
    {
        m_bShowUI = true;
        DisplayControllerHints();
        m_HandUI?.ChangeState(2, true);
        SetAnimatorState(m_Grip, controller.Inputs.BtnGrab.Value);
    }

    public override void OnGrabUpdate(ControllerData controller)
    {
        SetAnimatorState(m_Grip, controller.Inputs.BtnGrab.Value);
    }

    public override void OnGrabEnd(ControllerData controller)
    {
        m_bShowUI = false;
        DisplayControllerHints();
        m_HandUI?.ChangeState(2, false);
        SetAnimatorState(m_Grip, 0f);
    }

    public override void OnInteractBegin(ControllerData controller)
    {
        m_bShowUI = true;
        DisplayControllerHints();
        m_HandUI?.ChangeState(1, true);
        SetAnimatorState(m_Trigger, controller.Inputs.Interact.Value);
    }

    public override void OnInteractUpdate(ControllerData controller)
    {
        SetAnimatorState(m_Trigger, controller.Inputs.Interact.Value);
    }

    public override void OnInteractEnd(ControllerData controller)
    {
        m_bShowUI = false;
        DisplayControllerHints();
        m_HandUI?.ChangeState(1, false);
        SetAnimatorState(m_Trigger, 0f);
    }

    public override void UpdateControllerState(ControllerData controller)
    {
        if (true == m_BaseController.gameObject.activeSelf)
        {
            bool OnObject = controller.CurrentCollider != null;
            if (m_HandAnimator.isActiveAndEnabled)
            {
                m_HandAnimator.SetBool("HoverObject", OnObject);
            }
        }
    }

    public override void OnTouchstickUpdate(ControllerData controller)
    {
        Vector2 axis = controller.Inputs.Stick.Value2;
        if (null != m_LaserBeam && true == m_LaserBeam.IsActive())
        {            
            float fValue = (int)axis.y.RoundToNearest(1f);
            m_LaserBeam.SetBeamDirection(fValue);
        }
        if (m_HandAnimator.isActiveAndEnabled)
        {
            m_HandAnimator.SetBool("ThumbstickActive", axis.magnitude > 0f);

            //Update animator
            SetAnimatorState("ThumbstickX", axis.x);
            SetAnimatorState("ThumbstickY", axis.y);
        }
        m_HandUI?.ChangeState(3, true);
        m_bShowUI = true;
        DisplayControllerHints();
    }

    public override void OnTouchstickEnd(ControllerData controller)
    {
        if (null != m_LaserBeam && true == m_LaserBeam.IsActive())
        {
            m_LaserBeam.SetBeamDirection(0f);
        }
        if (m_HandAnimator.isActiveAndEnabled)
        {
            m_HandAnimator.SetBool("ThumbstickActive", false);
            SetAnimatorState("ThumbstickX", 0f);
            SetAnimatorState("ThumbstickY", 0f);
        }
        m_HandUI?.ChangeState(3, false);
        m_bShowUI = false;
        DisplayControllerHints();
    }

    private void DisplayControllerHints()
    {
        if (false == m_bisExtended)
        {
            m_HandUI?.SetAllVisible(m_bShowUI);
        }
        else
        {
            m_HandUI?.SetAllVisible(false);
        }
    }

    private void SetAnimatorState(string name, float value)
    {
        if (m_HandAnimator.isActiveAndEnabled)
        {
            m_HandAnimator.SetFloat(name, value);
        }
    }

    public void ActivateBeam(bool bBeam, Transform target)
    {
        if(null != m_LaserBeam)
        {
           // SetColor(m_OffHandColor);
            m_LaserBeam.Init(target);
            m_LaserBeam.Visible(bBeam);
            m_LaserBeam.SetBeamColor(m_OffHandColor, m_MainHandColor);
        }
    }

    public void SetColor(Color color)
    {
        m_ColourState = color;
        m_HandMaterial.SetColor("_ColorA", m_ColourState);
    }

    public Color GetColor()
    {
        return m_ColourState;
    }

    public void UpdateBeam()
    {
        if (null != m_LaserBeam)
        {
            m_LaserBeam.ManualUpdate();
        }
    }

    public void UpdateAllControllerStates(ControllerData controller)
    {
        OnTouchstickUpdate(controller);
        OnGrabUpdate(controller);
        OnInteractUpdate(controller);
        UpdateControllerState(controller);
    }
}