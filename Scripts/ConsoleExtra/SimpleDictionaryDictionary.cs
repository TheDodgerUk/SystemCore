using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using System.Diagnostics;
using System.IO;



public class SimpleDictionaryDictionary<TKey1, TKey2, TValue>
{
    Dictionary<TKey1, Dictionary<TKey2, TValue>> m_Dictionary = new Dictionary<TKey1, Dictionary<TKey2, TValue>>();
    public SimpleDictionaryDictionary()
    {
        m_Dictionary.Clear();
    }


    public List<TKey1> MainKeys => m_Dictionary.Keys.ToList();

    public List<TValue> Values()
    {
        List<TValue> newList = new List<TValue>();
        foreach (TKey1 lKey1 in m_Dictionary.Keys)
        {
            foreach (TKey2 lKey2 in m_Dictionary[lKey1].Keys)
            {
                newList.Add(m_Dictionary[lKey1][lKey2]);
            }
        }
        return newList;
    }


    public Dictionary<TKey2, TValue> GetMainDictionary(TKey1 lTKey)
    {
        m_CheckForKeyMain(lTKey);
        return m_Dictionary[lTKey];
    }



    public int CountMain(TKey1 lKey)
    {
        m_CheckForKeyMain(lKey);
        return m_Dictionary[lKey].Count;
    }


    /// <summary>
    /// 
    /// </summary>
    public void ClearAll()
    {
        foreach (TKey1 lKey1 in m_Dictionary.Keys)
        {
            m_Dictionary[lKey1].Clear();
        }
    }
    /// <summary>
    /// 
    /// <param name="lEDebugType"></param>
    public void ClearDictonarySecond(TKey1 lTKey)
    {
        m_CheckForKeyMain(lTKey);
        m_Dictionary[lTKey].Clear();
    }

    public TValue GetValue(TKey1 lTKey1, TKey2 Key2)
    {
        m_CheckForKeyMain(lTKey1);

        if (true == m_Dictionary[lTKey1].ContainsKey(Key2))
        {
            return m_Dictionary[lTKey1][Key2];
        }
        return default(TValue);
    }

    public Dictionary<TKey2, TValue> GetValue(TKey1 lTKey1)
    {
        if (true == m_Dictionary.ContainsKey(lTKey1))
        {
            return m_Dictionary[lTKey1];
        }
        return new Dictionary<TKey2, TValue>();
    }



    public void AddValue(TKey1 lTKey1, TKey2 lTKey2, TValue lTValue)
    {
        m_CheckForKeyMain(lTKey1);
        if (false == m_Dictionary[lTKey1].ContainsKey(lTKey2))
        {
            m_Dictionary[lTKey1].Add(lTKey2, lTValue);
        }
    }

    private void m_CheckForKeyMain(TKey1 lTKey)
    {
        if (m_Dictionary.ContainsKey(lTKey) == false)
        {
            m_Dictionary.Add(lTKey, new Dictionary<TKey2, TValue>());
        }
    }

}

