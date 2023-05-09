using System;
using UnityEngine;

public class RotateTween : Tween
{
    private Action<Quaternion> m_Handler;
    private Quaternion m_Target;
    private Quaternion m_Start;


    protected override void OnStep(float t)
    {
        if (m_Handler != null)
        {
            m_Handler(Quaternion.SlerpUnclamped(m_Start, m_Target, t));
        }
        else
        {
            Debug.LogError("Handler is somehow null, on " + name, this);
        }
    }


    public void Initialise(Quaternion startRotationAmount, Quaternion endRotationAmount, Action<Quaternion> handler)
    {
        m_Handler = handler;
        m_Start = startRotationAmount;
        m_Target = endRotationAmount;

        if (m_Start == m_Target)
        {
            End();
        }
    }


    public void Initialise(Quaternion startRotationAmount, Vector3 endRotationAmount, Action<Quaternion> handler)
    {
        m_Handler = handler;
        m_Start = startRotationAmount;
        m_Target = Quaternion.Euler(endRotationAmount);

        if (m_Start == m_Target)
        {
            End();
        }
    }

}
