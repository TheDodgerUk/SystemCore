using System;
using System.Threading;

public class WorkerJob
{
#if ARBITER_DEBUG
    public string Id { get; private set; }
#endif

    private volatile bool m_IsComplete = false;
    private Exception m_Exception;

    private Action m_OnComplete = null;
    private Action m_Work = null;

    public WorkerJob(string id, Action work, Action onComplete)
    {
#if ARBITER_DEBUG
        Id = id ?? Utils.Code.GenerateGuid();
#endif
        m_OnComplete = onComplete;
        m_Work = work;

        ThreadPool.QueueUserWorkItem(DoWork);
    }

    public void Shutdown()
    {
        m_OnComplete = null;
        m_Work = null;
    }

    public bool IsComplete() => m_IsComplete;

    public void InvokeCompleteHandler()
    {
        if (m_Exception != null)
        {
            UnityEngine.Debug.LogException(m_Exception);
        }
        var callback = m_OnComplete;
        m_OnComplete = null;
        callback?.Invoke();
    }

    public void DoWork(object state)
    {
        try
        {
            m_Work.Invoke();
        }
        catch (Exception ex)
        {
            m_Exception = ex;
        }
        finally
        {
            m_IsComplete = true;
            m_Work = null;
        }
    }

#if ARBITER_DEBUG
    public override string ToString() => Id;
#endif
}
