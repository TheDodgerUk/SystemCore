using System;

public class TaskAction
{
    private Action m_Callback;
    private Action m_EachCallback;
    private int m_Current;
    private int m_TargetCount;
    public int GetCount => m_Current;
    public int TargetCount => m_TargetCount;

    private string m_Name = "NotSet";
    public string Name => m_Name;



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
            m_TargetCount = count;
        }
    }

    public TaskAction(int count, string name, Action callback)
    {
        if (count == 0)
        {
            callback?.Invoke();
        }
        else
        {
            m_Callback = callback;
            m_TargetCount = count;
            m_Name = name;
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
            m_TargetCount = count;
        }
    }



    public void Increment()
    {
        m_Current++;
        m_EachCallback?.Invoke();

        if (m_Current == m_TargetCount)
        {
            m_Callback?.Invoke();
            m_IsComplete = true;
        }
    }
}
