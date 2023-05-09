using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityImpact : MonoBehaviour
{

    private Action<float> m_CallbackMagnitude;

    public void ClearCallbackMagnitude() => m_CallbackMagnitude = null;
    public void CallbackMagnitude(Action<float> callback) => m_CallbackMagnitude = callback;
    void OnCollisionEnter(Collision collision)
    {
        m_CallbackMagnitude?.Invoke(collision.relativeVelocity.magnitude);
    }
}
