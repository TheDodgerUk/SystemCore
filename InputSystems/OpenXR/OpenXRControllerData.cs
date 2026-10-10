#if VR_INTERACTION
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR.OpenXR.Samples.ControllerSample;

public enum LocalButtonId
{
    k_EButton_System,
    k_EButton_ApplicationMenu,
    k_EButton_Grip,
    k_EButton_DPad_Left,
    k_EButton_DPad_Up,
    k_EButton_DPad_Right,
    k_EButton_DPad_Down,
    k_EButton_A,
    k_EButton_ProximitySensor,
    k_EButton_Axis0,
    k_EButton_Axis1,
    k_EButton_Axis2,
    k_EButton_Axis3,
    k_EButton_Axis4,
    k_EButton_SteamVR_Touchpad,
    k_EButton_SteamVR_Trigger,
    k_EButton_Dashboard_Back,
    k_EButton_IndexController_A,
    k_EButton_IndexController_B,
    k_EButton_IndexController_JoyStick,
    k_EButton_IndexController_JoyStick_Click,

    k_EButton_IndexController_Tch_Trigger,
    k_EButton_IndexController_Tch_Grip,
    k_EButton_IndexController_Tch_Primary,
    k_EButton_IndexController_Tch_Secondary,
    k_EButton_IndexController_Tch_Stick,

    k_EButton_Max,
};


[System.Serializable]
public class OpenXRControllerData : TrackedControllerData
{
    private readonly OVRInput.Controller m_Id;
    private CoroutineRunner m_VibrateHost;

    private Dictionary<LocalButtonId, OpenXRButton> m_DataButton = new Dictionary<LocalButtonId, OpenXRButton>();
    private Dictionary<LocalButtonId, OpenXRVector1> m_DataVector1 = new Dictionary<LocalButtonId, OpenXRVector1>();
    private Dictionary<LocalButtonId, OpenXRVector2> m_DataVector2 = new Dictionary<LocalButtonId, OpenXRVector2>();

    public OpenXRControllerData(Transform raypoint, Transform wrist, OVRInput.Controller id, string side) :
        base(id.ToString(), raypoint, wrist, typeof(OpenXRControllerGraphics), GetHandedness(id))
    {
        Inputs.BtnStart.SetUpdateHandler(GetButtonState, OVRInput.Button.Start);
        Inputs.Interact.SetUpdateHandler(GetButtonState, OVRInput.Button.PrimaryIndexTrigger);
        Inputs.BtnPrimary.SetUpdateHandler(GetButtonState, OVRInput.Button.One);
        Inputs.BtnSecondary.SetUpdateHandler(GetButtonState, OVRInput.Button.Two);
        Inputs.BtnGrab.SetUpdateHandler(GetButtonState, OVRInput.Button.PrimaryHandTrigger);
        Inputs.BtnStick.SetUpdateHandler(GetButtonState, OVRInput.Button.PrimaryThumbstick);



        Inputs.PullTrigger.SetValueHandler(GetVector1State, OVRInput.Axis1D.PrimaryIndexTrigger);
        Inputs.PullGrip.SetValueHandler(GetVector1State, OVRInput.Axis1D.PrimaryHandTrigger);
        Inputs.Stick.SetAxisHandler(GetThumbstickState, OVRInput.Axis2D.PrimaryThumbstick);

        Inputs.TchTrigger.SetUpdateHandler((e) => GetTouchState(e, Inputs.Stick.State), OVRInput.Touch.SecondaryIndexTrigger);
        //////Inputs.TchGrab.SetUpdateHandler((e) => GetTouchState(e, Inputs.Stick.State), OVRInput.Axis2D.PrimaryHandTrigger);
        Inputs.TchPrimary.SetUpdateHandler((e) => GetTouchState(e, Inputs.Stick.State), OVRInput.Touch.PrimaryTouchpad);
        Inputs.TchSecondary.SetUpdateHandler((e) => GetTouchState(e, Inputs.Stick.State), OVRInput.Touch.SecondaryTouchpad);
        Inputs.TchStick.SetUpdateHandler((e) => GetTouchState(e, Inputs.Stick.State), OVRInput.Touch.PrimaryThumbstick);

        m_VibrateHost = raypoint.CreateChild<CoroutineRunner>();

        m_Id = id;

        // this is an overide to make sure its in the right place for the Oculus
        RaycastRoot.transform.Reset();
    }


    public override bool HasLaser() => true;

    public override bool IsConnected() => true;////// OVRInput.IsControllerConnected(m_Id); // this does not work with hands

    public override void Vibrate(float intensity, float duration)
    {
        TriggerVibration(0.5f, intensity);
        m_VibrateHost.enabled = true;
        m_VibrateHost.StopAllCoroutines();

        m_VibrateHost.WaitFor(duration, () =>
        {
            TriggerVibration(0f, 0f);
        });
    }

    private void TriggerVibration(float frequency, float intensity)
    {
        OVRInput.SetControllerVibration(frequency, intensity, m_Id);
    }

    private Vector2 GetThumbstickState(OVRInput.Axis2D btnId)
    {
        return OVRInput.Get(btnId, m_Id);
    }

    private float GetVector1State(OVRInput.Axis1D btnID)
    {
        return OVRInput.Get(btnID, m_Id);
    }

    private TriggerState GetTouchState(OVRInput.Touch btnId, TriggerState prevState)
    {
        bool current = OVRInput.Get(btnId);

        if (current == true)
        {
            if (prevState == TriggerState.Up)
            {
                return TriggerState.Down;
            }
            else
            {
                return TriggerState.Held;
            }
        }
        else
        {
            return TriggerState.Up;
        }
    }

    private TriggerState GetButtonState(OVRInput.Button btnId)
    {
        return InputState.GetState(
            () => OVRInput.GetDown(btnId, m_Id),
            () => OVRInput.Get(btnId, m_Id),
            () => OVRInput.GetUp(btnId, m_Id));
    }


    private static Handedness GetHandedness(OVRInput.Controller id)
    {
        if (id == OVRInput.Controller.LTouch)
        {
            return Handedness.Left;
        }
        else if (id == OVRInput.Controller.RTouch)
        {
            return Handedness.Right;
        }
        Debug.LogWarning($"Could not determine handedness for {id}\n");
        return (Handedness)(-1);
    }
}
#endif