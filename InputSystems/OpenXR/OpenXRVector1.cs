
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if VR_INTERACTION && !UNITY_ANDROID
using UnityEngine.InputSystem;
#endif

public class OpenXRVector1 : OpenXRControl
{
#if VR_INTERACTION && !UNITY_ANDROID
    public float Value { get; private set; }
    protected override void OnActionPerformed(InputAction.CallbackContext ctx) => UpdateValue(ctx);
    protected override void OnActionStarted(InputAction.CallbackContext ctx) => UpdateValue(ctx);
    protected override void OnActionCanceled(InputAction.CallbackContext ctx) => UpdateValue(ctx);

    private void UpdateValue(InputAction.CallbackContext ctx) => Value = ctx.ReadValue<float>();
#endif
}

