using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PressTutorialStep : TutorialStep
{
    public enum ButtonType
    {
        Trigger,
        Grip,
        ClickStick,
        TouchStick,
        Menu
    }

    public Action<ControllerData> OnDown;
    public Action<ControllerData> OnHeld;
    public Action<ControllerData> OnUp;

    [SerializeField]
    private TriggerState m_ActionCompleteOnState;

    [SerializeField]
    private ButtonType m_ButtonType;

    public void Initialise(TriggerState completeOnState, ButtonType buttonType)
    {
        m_ActionCompleteOnState = completeOnState;
        m_ButtonType = buttonType;
    }

    public override void ActivateStep()
    {
        base.ActivateStep();

        switch (m_ButtonType)
        {
            case ButtonType.Trigger:
                InputManagerVR.Instance.AnySubscription.Interact.Begin += OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.Interact.Update += OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.Interact.End += OnControllerEnd;
                break;
            case ButtonType.Grip:
                InputManagerVR.Instance.AnySubscription.Grab.Begin += OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.Grab.Update += OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.Grab.End += OnControllerEnd;
                break;
            case ButtonType.ClickStick:
                InputManagerVR.Instance.AnySubscription.ClickStick.Begin += OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.ClickStick.Update += OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.ClickStick.End += OnControllerEnd;
                break;
            case ButtonType.Menu:
                InputManagerVR.Instance.AnySubscription.BtnSecondary.Begin += OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.BtnSecondary.Update += OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.BtnSecondary.End += OnControllerEnd;
                break;
        }
    }

    public void DeInitialise()
    {
        switch (m_ButtonType)
        {
            case ButtonType.Trigger:
                InputManagerVR.Instance.AnySubscription.Interact.Begin -= OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.Interact.Update -= OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.Interact.End -= OnControllerEnd;
                break;
            case ButtonType.Grip:
                InputManagerVR.Instance.AnySubscription.Grab.Begin -= OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.Grab.Update -= OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.Grab.End -= OnControllerEnd;
                break;
            case ButtonType.ClickStick:
                InputManagerVR.Instance.AnySubscription.ClickStick.Begin -= OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.ClickStick.Update -= OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.ClickStick.End -= OnControllerEnd;
                break;
            case ButtonType.Menu:
                InputManagerVR.Instance.AnySubscription.BtnSecondary.Begin -= OnControllerBegin;
                InputManagerVR.Instance.AnySubscription.BtnSecondary.Update -= OnControllerUpdate;
                InputManagerVR.Instance.AnySubscription.BtnSecondary.End -= OnControllerEnd;
                break;
        }
    }

    private void OnControllerBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if(m_ActionCompleteOnState == TriggerState.Down)
        {
            TutorialStepComplete();
        }

        OnDown?.Invoke(interaction.Main);
    }

    private void OnControllerUpdate(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (m_ActionCompleteOnState == TriggerState.Held)
        {
            TutorialStepComplete();
        }

        OnHeld?.Invoke(interaction.Main);
    }

    private void OnControllerEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (m_ActionCompleteOnState == TriggerState.Up)
        {
            TutorialStepComplete();
        }

        OnUp?.Invoke(interaction.Main);
    }
}