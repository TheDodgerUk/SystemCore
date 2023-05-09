using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using System.Diagnostics;
using System.IO;

public class SimpleDictionaryList<TKey, TValue>
{
    public class KeyVal<TTKey, TTVal>
    {
        public TTKey Key { get; set; }
        public TTVal Value { get; set; }

        public KeyVal() { }

        public KeyVal(TTKey key, TTVal val)
        {
            this.Key = key;
            this.Value = val;
        }
    }



    List<KeyVal<TKey, List<TValue>>> m_Dictionary = new List<KeyVal<TKey, List<TValue>>>();
    public SimpleDictionaryList()
    {
        m_Dictionary.Clear();
    }

    /// <summary>
    /// 
    /// <returns></returns>
    public List<TValue> GetList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
        return m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value;
    }


    public List<TValue> GetListAll()
    {
        List<TValue> returnList = new List<TValue>();
        foreach (var item in m_Dictionary)
        {
            returnList.AddRange(item.Value);
        }
        return returnList;
    }


    public TValue GetRandomFromList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
        int index = UnityEngine.Random.Range(0, m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value.Count);
        return m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value[index];
    }


    public List<TKey> GetKeys()
    {
        List<TKey> lList = new List<TKey>();

        foreach (var item in m_Dictionary)
        {
            lList.Add(item.Key);
        }
        return lList;
    }

    public TKey GetKey(int index)
    {
        return m_Dictionary[index].Key;
    }

    public List<TValue> GetValues(int index)
    {
        return m_Dictionary[index].Value;
    }


    public void GetKey(string lKeyString, ref TKey lTKey)
    {
        List<TKey> lList = new List<TKey>();
        foreach (var lKey in lList)
        {
            if (lKey.ToString().Contains(lKeyString) == true)
            {
                lTKey = lKey;
            }
        }
    }

    public bool KeyExists(TKey lTKey)
    {
        return m_Dictionary.Find(e => e.Key.Equals(lTKey)) != null;
    }

    public int Count(TKey lKey)
    {
        m_CheckForKey(lKey);
        return m_Dictionary.Find(e => e.Key.Equals(lKey)).Value.Count;
    }

    public int CountAll()
    {
        int count = 0;

        foreach (var item in m_Dictionary)
        {
            count += item.Value.Count;
        }
        return count;
    }

    public List<TValue> this[TKey lTKey]
    {
        get
        {
            m_CheckForKey(lTKey);
            return m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value;
        }
        set
        {
            m_CheckForKey(lTKey);
            m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value = value;
        }
    }

    public TKey GetKey(TValue lTValue)
    {
        foreach (var item in m_Dictionary)
        {
            if (item.Value.Contains(lTValue) == true)
            {
                return item.Key;
            }
        }
        return default(TKey);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ClearAll()
    {

        foreach (var item in m_Dictionary)
        {
            m_CheckForKey(item.Key);
            item.Value.Clear();
        }
        m_Dictionary.Clear();
    }
    /// <summary>
    /// 
    /// <param name="lEDebugType"></param>
    public void ClearList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
        m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value.Clear();
    }

    public void GetValue(TKey lTKey, string lName, ref TValue lRefItem)
    {
        List<TValue> lList = GetList(lTKey);
        foreach (var lItem in lList)
        {
            if (lItem.ToString() == lName)
            {
                lRefItem = lItem;
            }
        }
    }

    public List<TValue> GetValuesAll()
    {
        List<TValue> lList = new List<TValue>();

        foreach (var item in m_Dictionary)
        {
            lList.AddRange(item.Value);
        }
        return lList;
    }


    public void GetValue(string KeyName, string lName, ref TValue lRefItem)
    {
        List<TKey> lListKeys = GetKeys();
        foreach (var lKey in lListKeys)
        {
            if (lKey.ToString().Contains(KeyName))
            {
                List<TValue> lValues = GetList(lKey);
                foreach (var lItem in lValues)
                {
                    if (lItem.ToString().Contains(lName))
                    {
                        lRefItem = lItem;
                    }
                }
            }
        }
    }


    public List<TValue> GetValuesList(TKey lTKey)
    {
        return GetList(lTKey);
    }

    public void AddToList(TKey lTKey, TValue lTValue)
    {
        m_CheckForKey(lTKey);
        m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value.Add(lTValue);
    }

    public void AddToList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
    }


    public void AddToList(TKey lTKey, List<TValue> lTValue)
    {
        m_CheckForKey(lTKey);
        foreach (var item in lTValue)
        {
            m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value.Add(item);
        }
    }

    public void RemoveFromList(TKey lTKey, TValue lTValue)
    {
        m_CheckForKey(lTKey);
        m_Dictionary.Find(e => e.Key.Equals(lTKey)).Value.Remove(lTValue);
    }

    private void m_CheckForKey(TKey lTKey)
    {
        if (m_Dictionary.Count == 0 || m_Dictionary.Find(e => e.Key.Equals(lTKey)) == null)
        {
            KeyVal<TKey, List<TValue>> newitem = new KeyVal<TKey, List<TValue>>(lTKey, new List<TValue>());
            m_Dictionary.Add(newitem);
        }
    }

}

