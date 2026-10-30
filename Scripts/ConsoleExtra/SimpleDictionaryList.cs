
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimpleDictionaryList<TKey, TValue>
{
    [Serializable]
    public class ValueList<T>
    {
        [SerializeField]
        private List<T> values = new List<T>();

        public List<T> Values
        {
            get => values;
            set => values = value ?? new List<T>();
        }

        public ValueList()
        {
        }

        public ValueList(List<T> values)
        {
            Values = values;
        }
    }

    [Serializable]
    public class KeyVal<TTKey, TTVal>
    {
        [SerializeField]
        private TTKey key;

        [SerializeField]
        private ValueList<TTVal> value = new ValueList<TTVal>();

        public TTKey Key
        {
            get => key;
            set => key = value;
        }

        public List<TTVal> Value
        {
            get => value.Values;
            set
            {
                if (this.value == null)
                    this.value = new ValueList<TTVal>();

                this.value.Values = value;
            }
        }

        public KeyVal()
        {
        }

        public KeyVal(TTKey key, List<TTVal> values)
        {
            this.key = key;
            this.value = new ValueList<TTVal>(values);
        }
    }

    [SerializeField]
    private List<KeyVal<TKey, TValue>> m_Dictionary =
        new List<KeyVal<TKey, TValue>>();

    public SimpleDictionaryList()
    {
    }

    public List<TValue> GetList(TKey lTKey)
    {
        m_CheckForKey(lTKey);

        return m_Dictionary.Find(
            e => EqualityComparer<TKey>.Default.Equals(e.Key, lTKey)
        ).Value;
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
        List<TValue> list = GetList(lTKey);

        if (list.Count == 0)
            return default(TValue);

        int index = UnityEngine.Random.Range(0, list.Count);

        return list[index];
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
        foreach (var key in GetKeys())
        {
            if (key != null &&
                key.ToString().Contains(lKeyString))
            {
                lTKey = key;
                return;
            }
        }
    }

    public bool KeyExists(TKey lTKey)
    {
        return m_Dictionary.Exists(
            e => EqualityComparer<TKey>.Default.Equals(e.Key, lTKey)
        );
    }

    public int Count(TKey lKey)
    {
        return GetList(lKey).Count;
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
            return GetList(lTKey);
        }
        set
        {
            m_CheckForKey(lTKey);

            var entry = m_Dictionary.Find(
                e => EqualityComparer<TKey>.Default.Equals(e.Key, lTKey)
            );

            entry.Value = value;
        }
    }

    public TKey GetKey(TValue lTValue)
    {
        foreach (var item in m_Dictionary)
        {
            if (item.Value.Contains(lTValue))
                return item.Key;
        }

        return default(TKey);
    }

    public void ClearAll()
    {
        m_Dictionary.Clear();
    }

    public void ClearList(TKey lTKey)
    {
        GetList(lTKey).Clear();
    }

    public void GetValue(
        TKey lTKey,
        string lName,
        ref TValue lRefItem)
    {
        List<TValue> lList = GetList(lTKey);

        foreach (var lItem in lList)
        {
            if (lItem != null &&
                lItem.ToString() == lName)
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

    public void GetValue(
        string KeyName,
        string lName,
        ref TValue lRefItem)
    {
        List<TKey> lListKeys = GetKeys();

        foreach (var lKey in lListKeys)
        {
            if (lKey != null &&
                lKey.ToString().Contains(KeyName))
            {
                List<TValue> lValues = GetList(lKey);

                foreach (var lItem in lValues)
                {
                    if (lItem != null &&
                        lItem.ToString().Contains(lName))
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
        GetList(lTKey).Add(lTValue);
    }

    public void AddToList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
    }

    public void AddToList(TKey lTKey, List<TValue> lTValue)
    {
        if (lTValue == null)
            return;

        GetList(lTKey).AddRange(lTValue);
    }

    public void RemoveFromList(TKey lTKey, TValue lTValue)
    {
        GetList(lTKey).Remove(lTValue);
    }

    private void m_CheckForKey(TKey lTKey)
    {
        if (!KeyExists(lTKey))
        {
            var newItem =
                new KeyVal<TKey, TValue>(
                    lTKey,
                    new List<TValue>()
                );

            m_Dictionary.Add(newItem);
        }
    }
}
