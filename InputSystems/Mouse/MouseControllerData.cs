using UnityEngine;

public class MouseControllerData : CameraControllerData
{
    public MouseControllerData() : base("mouse")
    {
        Debug.Log("MouseControllerData");
        Inputs.BtnPrimary.SetUpdateHandler(GetKeyState, KeyCode.Space);

        Inputs.Interact.SetUpdateHandler(GetMouseState, 0);
        Inputs.BtnGrab.SetUpdateHandler(GetMouseState, 1);
    }

    public override bool IsConnected() => Camera.main != null;

    public override bool HasLaser() => true;

    public override void Vibrate(float intensity, float duration) { }


    protected override void UpdateRay()
    {
        m_Raycaster.transform.position = GetPosition();
        m_Raycaster.Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        m_Raycaster.transform.forward = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
    }

    protected override Vector3 GetPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    protected override Quaternion GetRotation() => m_Raycaster.transform.rotation;

    protected override Vector2 GetCameraSpacePosition() => Input.mousePosition;

    private static TriggerState GetMouseState(int buttonId)
    {
        return InputState.GetState(
            () => Input.GetMouseButtonDown(buttonId),
            () => Input.GetMouseButton(buttonId),
            () => Input.GetMouseButtonUp(buttonId));
    }

    private static TriggerState GetKeyState(KeyCode key)
    {
        return InputState.GetState(
            () => Input.GetKeyDown(key),
            () => Input.GetKey(key),
            () => Input.GetKeyUp(key));
    }
}
