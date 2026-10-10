using System;
using UnityEngine;

public class Vector1State : InputState
{
    public float Actual { get; private set; }
    public Vector2 Axis { get; private set; }

    private Func<float> m_GetState = null;
    private Vector2 m_LastAxis;
    private Vector2 m_LargestVector;

    public void SetAxisHandler<T>(Func<T, float> func, T arg)
    {
        m_GetState = () => func(arg);
    }

    public override void UpdateState(Vector3 position, Quaternion rotation)
    {
        base.UpdateState(position, rotation);
        State = TriggerState.Down;

    }


}
