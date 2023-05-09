#if VR_INTERACTION && UNITY_ANDROID

using UnityEngine;

[System.Serializable]
public class OculusControllerData : TrackedControllerData
{
    private readonly OVRInput.Controller m_Id;
    private CoroutineRunner m_VibrateHost;

    public OculusControllerData(Transform root, Transform origin, OVRInput.Controller id, string side) :
        base(id.ToString(), root, origin, typeof(OculusControllerGraphics), GetHandedness(id))
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

        m_VibrateHost = root.CreateChild<CoroutineRunner>();

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