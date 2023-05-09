using System;
using System.Collections.Generic;
using UnityEngine;

public class ReuseablePool<T> where T : Component
{
    public Predicate<T> IsAvailable = null;

    private GameObject m_OriginalObj;
    private Transform m_Parent;

    private List<T> m_Active;
    private List<T> m_Pool;
    private T m_Original;

    public ReuseablePool(T original, Transform parent)
    {
        m_OriginalObj = original.gameObject;
        m_Original = original;
        m_Parent = parent;

        m_Original.SetActive(false);
        m_Active = new List<T>();
        m_Pool = new List<T>();

        IsAvailable = e => !m_Active.Contains(e);
    }

    public void ForEach(Action<T> action)
    {
        m_Pool.ForEach(action);
        action(m_Original);
    }

    public T GetNext()
    {
        var next = m_Pool.Find(IsAvailable);
        if (next == null)
        {
            next = SpawnNew();
        }
        Activate(next);
        return next;
    }

    public void Return(T item) => Deactivate(item);

    private void Activate(T item)
    {
        m_Active.Add(item);
        item.SetActive(true);
        item.name = "[active]";
    }

    private void Deactivate(T item)
    {
        m_Active.Remove(item);
        item.SetActive(false);
        item.name = "[inactive]";

        if (item.transform.parent != m_Parent)
        {
            item.transform.SetParent(m_Parent, true);
        }
    }

    private T SpawnNew()
    {
        var next = Utils.Unity.Clone<T>(m_OriginalObj, m_Parent);
        next.name = m_Pool.Count.ToString();
        m_Pool.Add(next);
        return next;
    }
}
