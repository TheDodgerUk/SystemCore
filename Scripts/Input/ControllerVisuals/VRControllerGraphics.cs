using System;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerAttachment
{
    Teleporter,
}

public class VRControllerGraphics : ControllerGraphics
{
    private static readonly Dictionary<ControllerAttachment, Type> m_ControllerFactory = new Dictionary<ControllerAttachment, Type>()
    {
    };
    private static readonly Dictionary<Type, ControllerAttachment> m_TypeToAttachment = m_ControllerFactory.Inverse();

    private Dictionary<ControllerAttachment, ControllerVisual> m_Attachments = new Dictionary<ControllerAttachment, ControllerVisual>();
    private ControllerAttachment m_CurrentAttachment;

    private ControllerVisual m_ActiveControllerVisual;

    private Vector3 m_SphereOffset = new Vector3(0f, -0.003f, 0.1f);
    private float m_fSphereScale = 0.005f;

    public override void Initialise(Handedness handedness)
    {
        if(m_Transform == null)
        {
            BaseInitilise();
        }
        m_Transform.localPosition = GetHandPosition(handedness);
        m_Transform.localRotation = GetHandRotation(handedness);


        RaycastRoot = Utils.Unity.Clone("ControllerSphere", m_Transform).transform;
        base.Initialise(handedness);
    }


    public override void SplashReset()
    {
        base.SplashReset();

        m_ActiveControllerVisual.SplashReset();
    }

    public override void SetLayer(int layer)
    {
        base.SetLayer(layer);

        m_ActiveControllerVisual.SetLayer(layer);
    }

    public override void OnNewPoses()
    {
        base.OnNewPoses();

        m_ActiveControllerVisual.OnNewPoses();
    }

    public override void UpdateState(ControllerData controller)
    {
        if (controller.Inputs.BtnGrab.State == TriggerState.Down)
        {
            ControllerBeginGrab(controller);
        }
        else if (controller.Inputs.BtnGrab.State == TriggerState.Held)
        {
            ControllerUpdateGrab(controller);
        }
        else if (controller.Inputs.BtnGrab.State == TriggerState.Up)
        {
            ControllerEndGrab(controller);
        }

        if (controller.Inputs.Interact.State == TriggerState.Down)
        {
            OnInteractBegin(controller);
        }
        else if (controller.Inputs.Interact.State == TriggerState.Held)
        {
            OnInteractUpdate(controller);
        }
        else if (controller.Inputs.Interact.State == TriggerState.Up)
        {
            OnInteractEnd(controller);
        }

        if (controller.Inputs.Stick.State == TriggerState.Held)
        {
            OnTouchStickUpdate(controller);
        }
        else if (controller.Inputs.Stick.State == TriggerState.Up)
        {
            OnTouchStickEnd(controller);
        }

        UpdateControllerState(controller);
    }

    public T GetVisual<T>() where T : ControllerVisual
    {
        var attachment = m_TypeToAttachment[typeof(T)];
        return m_Attachments.Get(attachment) as T;
    }

    protected virtual Vector3 GetHandPosition(Handedness handedness) => Vector3.zero;
    protected virtual Quaternion GetHandRotation(Handedness handedness) => Quaternion.identity;


    private void UpdateControllerState(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].UpdateControllerState(controller);
        }
    }

    private void ControllerBeginGrab(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnGrabBegin(controller);
        }
    }

    private void ControllerUpdateGrab(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnGrabUpdate(controller);
        }
    }

    private void ControllerEndGrab(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnGrabEnd(controller);
        }
    }

    private void OnInteractBegin(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnInteractBegin(controller);
        }
    }

    private void OnInteractUpdate(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnInteractUpdate(controller);
        }
    }

    private void OnInteractEnd(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnInteractEnd(controller);
        }
    }

    private void OnTouchStickUpdate(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnTouchstickUpdate(controller);
        }
    }

    private void OnTouchStickEnd(ControllerData controller)
    {
        if (m_Attachments.ContainsKey(m_CurrentAttachment))
        {
            m_Attachments[m_CurrentAttachment].OnTouchstickEnd(controller);
        }
    }
}