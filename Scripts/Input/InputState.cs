using System;
using UnityEngine;

public enum TriggerState { None, Down, Held, Up }

public class InputState
{
    public readonly DeltaQuaternion Rotation = new DeltaQuaternion();
    public readonly DeltaVector3 Position = new DeltaVector3();
    public readonly DeltaFloat Time = new DeltaFloat();
    public bool AutoUpdate = true;

    public TriggerState State
    {
        get { return m_CurrentState; }
        set
        {
            m_OriginalState = value;
            m_CurrentState = value;
        }
    }
    public float Value { get; private set; }
    public float PrevValue { get; private set; }


    private TriggerState m_OriginalState = TriggerState.None;
    private TriggerState m_CurrentState = TriggerState.None;

    private Func<TriggerState> m_GetState = null;
    private Func<float> m_GetValue = null;

    public void SetUpdateHandler<T>(Func<T, TriggerState> func, T arg)
    {
        m_GetState = () => func(arg);
    }

    public void SetValueHandler<T>(Func<T, float> func, T arg)
    {
        m_GetValue = () => func(arg);
    }

    public void SetTempState(TriggerState state) => m_CurrentState = state;
    public void RestoreFromTempState() => m_CurrentState = m_OriginalState;

    public void Reset()
    {
        SetStart(Vector3.zero, Quaternion.identity);
        State = TriggerState.None;
        Value = 0;
    }

    public virtual void UpdateState(Vector3 position, Quaternion rotation)
    {
        if (AutoUpdate == true)
        {
            if (m_GetValue != null)
            {
                PrevValue = Value;
                Value = m_GetValue.Invoke();
            }
            if (m_GetState != null)
            {
                State = m_GetState.Invoke();

                if (m_GetValue == null)
                {
                    bool down = (State == TriggerState.Down || State == TriggerState.Held);
                    Value = down ? 1f : 0f;
                }
            }
        }

        if (m_CurrentState == TriggerState.Down)
        {
            SetStart(position, rotation);
        }
        else if (m_CurrentState != TriggerState.None)
        {
            SetCurrent(position, rotation);
        }
    }

    private void SetStart(Vector3 position, Quaternion rotation)
    {
        Time.SetStart(UnityEngine.Time.time);
        Position.SetStart(position);
        Rotation.SetStart(rotation);
    }

    private void SetCurrent(Vector3 position, Quaternion rotation)
    {
        Time.SetCurrent(UnityEngine.Time.time);
        Position.SetCurrent(position);
        Rotation.SetCurrent(rotation);
    }

    public static TriggerState GetState(Func<bool> down, Func<bool> held, Func<bool> up)
    {
        if (down() == true)
        {
            return TriggerState.Down;
        }
        if (held() == true)
        {
            return TriggerState.Held;
        }
        if (up() == true)
        {
            return TriggerState.Up;
        }

        return TriggerState.None;
    }
}