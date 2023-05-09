using System;
using UnityEngine;

#if UNITY_ANDROID
public class OculusPlayArea : PlayArea
{
    protected override void RequestPlayAreaSize(Action<Vector3> callback)
    {
#if VR_INTERACTION
        callback(OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea));
#endif
    }
}
#endif