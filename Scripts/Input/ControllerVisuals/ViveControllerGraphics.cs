using UnityEngine;

public class ViveControllerGraphics : VRControllerGraphics
{
    protected override Vector3 GetHandPosition(Handedness handedness) => new Vector3(0, 0, 0);
    protected override Quaternion GetHandRotation(Handedness handedness) => Quaternion.Euler(0, 0, 0);
}
