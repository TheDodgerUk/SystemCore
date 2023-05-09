
using System;
using System.Collections;
using UnityEngine;

#if VR_INTERACTION && !UNITY_ANDROID
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;
#endif


public class OpenXRControl : MonoBehaviour
{

#if VR_INTERACTION && !UNITY_ANDROID
    [Tooltip("Action Reference that represents the control")]
    [SerializeField] public InputActionReference _actionReference = null;

    private TriggerState m_CurrentTriggerState = TriggerState.Up;
    private TriggerState m_UpdateTriggerState = TriggerState.Up;

    public TriggerState TriggerStateRef => m_CurrentTriggerState;

    public void UpdateTriggerState()
    {

        // this is needed as down gets called and the instantly held gets called , 
        // so when system is checking the TriggerState Down has been overwriten before it can be used in the system
        if (m_CurrentTriggerState == TriggerState.Up && m_UpdateTriggerState == TriggerState.Held)
        {
            m_CurrentTriggerState = TriggerState.Down;
        }
        else
        {
            m_CurrentTriggerState = m_UpdateTriggerState;
        }
    }

    private void InternalUpdateTrigger(TriggerState newState)
    {
        m_UpdateTriggerState = newState;
    }


    protected virtual void OnEnable()
    {
        if (_actionReference == null || _actionReference.action == null)
            return;

        _actionReference.action.started += OnActionStarted;
        _actionReference.action.performed += OnActionPerformed;
        _actionReference.action.canceled += OnActionCanceled;

        _actionReference.action.started += OnTriggerStateStarted;
        _actionReference.action.performed += OnTriggerStatePerformed;
        _actionReference.action.canceled += OnTriggerStateCanceled;

        StartCoroutine(UpdateBinding());
    }

    protected virtual void OnDisable()
    {
        if (_actionReference == null || _actionReference.action == null)
            return;

        _actionReference.action.started -= OnActionStarted;
        _actionReference.action.performed -= OnActionPerformed;
        _actionReference.action.canceled -= OnActionCanceled;

        _actionReference.action.started -= OnTriggerStateStarted;
        _actionReference.action.performed -= OnTriggerStatePerformed;
        _actionReference.action.canceled -= OnTriggerStateCanceled;
    }

    private IEnumerator UpdateBinding()
    {


        while (isActiveAndEnabled)
        {
            if (_actionReference.action != null &&
                _actionReference.action.controls.Count > 0 &&
                _actionReference.action.controls[0].device != null &&
                OpenXRInput.TryGetInputSourceName(_actionReference.action, 0, out var actionName, OpenXRInput.InputSourceNameFlags.Component, _actionReference.action.controls[0].device))
            {

                OnActionBound();
                break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }


    public virtual void OnBegin(Action callback)
    {

    }

    private void OnTriggerStateStarted(InputAction.CallbackContext ctx) { InternalUpdateTrigger(TriggerState.Down); }

    private void OnTriggerStatePerformed(InputAction.CallbackContext ctx) { InternalUpdateTrigger(TriggerState.Held); }

    private void OnTriggerStateCanceled(InputAction.CallbackContext ctx) { InternalUpdateTrigger(TriggerState.Up);}

    protected virtual void OnActionStarted(InputAction.CallbackContext ctx) {  }

    protected virtual void OnActionPerformed(InputAction.CallbackContext ctx) { }
    protected virtual void OnActionCanceled(InputAction.CallbackContext ctx) { }

    public virtual void OnActionBound()
    {
    }
#endif
}

