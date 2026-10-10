using System;
using UnityEngine;

public class VirtualInput : InputSystem
{
    public override void Initialise(Action callback)
    {
        Debug.Log("VirtualInput");
        callback?.Invoke();
    }

    public void AddVirtualController()
    {
        var currentControllers = GetControllers();
        var go = new GameObject("VirtualController" + currentControllers.Count);

        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        AddController(new VirtualControllerData(go.transform, go.transform, "VirtualController" + currentControllers.Count, null)
        {
            m_bOffsetOnAction = false
        });
    }

    public VirtualControllerData GetUnlockedController()
    {
        foreach (var controller in GetControllers())
        {
            var virtualController = controller as VirtualControllerData;
            if (virtualController != null)
            {
                if (false == virtualController.IsBusy && false == controller.IsLocked)
                {
                    return virtualController;
                }
            }
        }

        return null;
    }
}
