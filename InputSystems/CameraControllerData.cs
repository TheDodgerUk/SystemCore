using System;
using UnityEngine;

public abstract class CameraControllerData : ControllerData
{
    public CameraControllerData(string uniqueId) : base(uniqueId, GetCameraTransform(), GetCameraTransform(), null) { }

    public override bool IsConnected() => (GetCamera() != null);

    protected override void UpdateRay()
    {
        m_Raycaster.Ray = GetCamera().ScreenPointToRay(GetCameraSpacePosition());
    }

    protected override Vector3 GetPosition() => m_Raycaster.Ray.origin;

    protected abstract Vector2 GetCameraSpacePosition();

    private static Transform GetCameraTransform()
    {
     
        if (CameraControllerVR.Instance.CameraTransform != null)
        {
            return CameraControllerVR.Instance.CameraTransform;
        }
        else
        {
            Debug.LogError($"GetCameraTransform is null  Environment.StackTrace : {Environment.StackTrace}");
            return null;
        }
    }
    private static Camera GetCamera()
    {
        if(CameraControllerVR.Instance.MainCamera != null)
        {
            return CameraControllerVR.Instance.MainCamera;
        }
        else
        {
            Debug.LogError($"GetCamera is null  Environment.StackTrace : {Environment.StackTrace}");
            return null;
        }

    }
}
