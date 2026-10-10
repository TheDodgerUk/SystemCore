using System;

public class TaskAction
{
    private Action m_Callback;
    private Action m_EachCallback;
    private int m_Current;
    private int m_Count;
    public int GetCount => m_Current;

    private bool m_IsComplete = false;

    public TaskAction(int count, Action callback)
    {
        if (count == 0)
        {
            callback?.Invoke();
        }
        else
        {
            m_Callback = callback;
            m_Count = count;
        }
    }

    public bool IsComplete => m_IsComplete;

    public TaskAction(int count, Action callback, Action eachCallBack)
    {
        if (count == 0)
        {
            callback?.Invoke();
            eachCallBack?.Invoke();
            m_IsComplete = true;
        }
        else
        {
            m_Callback = callback;
            m_EachCallback = eachCallBack;
            m_Count = count;
        }
    }



    public void Increment()
    {
        m_Current++;
        m_EachCallback?.Invoke();

        if (m_Current == m_Count)
        {
            m_Callback?.Invoke();
            m_IsComplete = true;
        }
    }
}
