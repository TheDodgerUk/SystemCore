using UnityEngine;

public class TouchControllerData : CameraControllerData
{
    public TouchPhase Phase => m_Touch.phase;
    public readonly int Id;

    private readonly Touch m_Touch;

    public TouchControllerData(Touch touch) : base(touch.fingerId.ToString())
    {
        Inputs.Interact.SetUpdateHandler(GetTouchState, touch);

        Id = touch.fingerId;
        m_Touch = touch;
    }

    public override bool HasLaser() => false;

    public override void Vibrate(float intensity, float duration) { }

    protected override Vector2 GetCameraSpacePosition() => m_Touch.position;

    private TriggerState GetTouchState(Touch touch)
    {
        return InputState.GetState(
            () => touch.phase == TouchPhase.Began,
            () => touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary,
            () => touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);
    }
}
