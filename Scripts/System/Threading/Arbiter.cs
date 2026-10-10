using System;
using System.Collections.Generic;
using UnityEngine;

public class Arbiter : MonoBehaviour
{
    private static Arbiter m_Instance;

#if ARBITER_DEBUG
    [SerializeField]
    private bool m_DebugMode = false;
#pragma warning disable 0414
    [SerializeField]
    private List<string> m_JobIds = new List<string>();
#pragma warning restore 0414
#endif

    private List<WorkerJob> m_JobQueue = new List<WorkerJob>();

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            this.DestroyObject();
        }
    }

    private void OnDestroy()
    {
        if (m_Instance == this)
        {
            m_Instance = null;
        }

        // cleanup all jobs 
        for (int i = 0; i < m_JobQueue.Count; ++i)
        {
            m_JobQueue[i].Shutdown();
        }
        m_JobQueue.Clear();
    }

    private void Update()
    {
        for (int i = 0; i < m_JobQueue.Count; ++i)
        {
            if (m_JobQueue[i].IsComplete() == true)
            {
                m_JobQueue[i].InvokeCompleteHandler();
                m_JobQueue.RemoveAt(i);
                --i;
            }
        }

#if ARBITER_DEBUG
        if (m_DebugMode == true)
        {
            m_JobIds = m_JobQueue.Extract(b => b.Id);
        }
#endif
    }

    public static void CreateJob(Action work, Action onSuccess, string id = null)
    {
        if (m_Instance != null)
        {
            m_Instance.m_JobQueue.Add(new WorkerJob(id, work, onSuccess));
        }
        else
        {
            work();
            onSuccess();
        }
    }

    public static void CreateJobWithResult<T>(Func<T> work, Action<T> onSuccess)
    {
        var obj = default(T);
        CreateJob(() => obj = work(), () => onSuccess?.Invoke(obj));
    }
}
