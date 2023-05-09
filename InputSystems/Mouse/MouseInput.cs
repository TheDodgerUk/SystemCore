using System;
using UnityEngine;

public class MouseInput : InputSystem
{
    public override void Initialise(Action callback)
    {
        Debug.Log("MouseInput");
        var cameraGo = new GameObject("MainCamera");
        var camera = cameraGo.AddComponent<Camera>();
        camera.nearClipPlane = 0.01f;
        camera.allowMSAA = false;
        camera.allowHDR = false;
        cameraGo.AddComponent<AudioListener>();

        CameraControllerVR.Instance.SetCamera(camera, cameraGo.transform);
        cameraGo.transform.localPosition = new Vector3(0, 1.8f, 0);
        cameraGo.transform.localRotation = Quaternion.Euler(5, 0, 0);

        AddController(new MouseControllerData());

        callback();
    }
}
