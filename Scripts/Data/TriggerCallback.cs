using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCallback : MonoBehaviour
{
    public Collider m_OnColider;
    public String m_OnColiderName;

    private Action<Collider> m_OnEnter;
    private Action<Collider> m_OnExit;

    private ContentPickUpSocketMetaData.SocketType m_SocketType = ContentPickUpSocketMetaData.SocketType.OneOnly;

    public void OnCallback(Collider col, Action<Collider> onEnter, Action<Collider> onExit)
    {
        m_SocketType = ContentPickUpSocketMetaData.SocketType.OneOnly;
        m_OnColider = col;
        m_OnEnter = onEnter;
        m_OnExit = onExit;
    }

    public void OnCallback(string col, Action<Collider> onEnter, Action<Collider> onExit)
    {
        m_SocketType = ContentPickUpSocketMetaData.SocketType.FindByName;
        m_OnColiderName = col;
        m_OnEnter = onEnter;
        m_OnExit = onExit;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
        {
            if (other == m_OnColider)
            {
                m_OnEnter?.Invoke(other);
            }
        }
        else
        {
            if (other.name == m_OnColiderName)
            {
                m_OnEnter?.Invoke(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
        {
            if (other == m_OnColider)
            {
                m_OnExit?.Invoke(other);
            }
        }
        else
        {
            if (other.name == m_OnColiderName)
            {
                m_OnExit?.Invoke(other);
            }
        }
    }
}

