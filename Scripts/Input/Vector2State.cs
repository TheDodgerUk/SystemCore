using System;
using UnityEngine;

public class Vector2State : InputState
{
    public Vector2 Value2 { get; private set; }
    public Vector2 PrevValue2 { get; private set; }
    public Vector2 Diffence { get; private set; }

    private Func<Vector2> m_GetThumbstickState = null;
    private Vector2 m_LastAxis;
    private Vector2 m_LargestVector;

    public void SetAxisHandler<T>(Func<T, Vector2> func, T arg)
    {
        m_GetThumbstickState = () => func(arg);
    }

    public override void UpdateState(Vector3 position, Quaternion rotation)
    {
        base.UpdateState(position, rotation);

        var currentAxis = Vector2.zero;
        if (AutoUpdate == true)
        {
            if (m_GetThumbstickState != null)
            {
                currentAxis = m_GetThumbstickState.Invoke();
            }
        }

        PrevValue2 = Value2;
        Value2 = currentAxis;

        Diffence = (currentAxis - m_LastAxis).Clamp(-1f, 1f);
    }

    private TriggerState GetThumbstickState(Vector2 current)
    {
        //Moved?
        if(current.magnitude > 0.01f)
        {
            if(State == TriggerState.None)
            {
                return TriggerState.Down;
            }
            else if(State == TriggerState.Down)
            {
                return TriggerState.Held;
            }
        }
        else if(State == TriggerState.Held)
        {
            return TriggerState.Up;
        }

        return TriggerState.None;
    }

    private float GetLargestValue(float value, float original)
    {
        //Current value is larger
        if(Mathf.Abs(value) > Mathf.Abs(original) ||
            Mathf.Abs(value) - Mathf.Abs(original) < 0.01f)
        {
            //Current value is larger or same so take newest value
            return value;
        }

        return original;
    }
}
