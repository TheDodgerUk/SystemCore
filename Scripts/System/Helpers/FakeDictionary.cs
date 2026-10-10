using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FakeDictionaryStrings : FakeDictionary<string, string>
{
    public FakeDictionaryItem GetItem(string name)
    {
        FakeDictionaryItem item = m_Container.Find(x => x.m_Key == name);
        if (null == item)
        {
            item = m_Container.Find(x => x.m_Value == name);
        }
        return item;
    }
};


public class FakeDictionary<T_Key, T_Value>
{
    [SerializeField]
    public List<FakeDictionaryItem> m_Container = new List<FakeDictionaryItem>(); // had to change it to allow it to be read to written and read via Json

#if CATALOG_PROGRAM
    public List<FakeDictonaryItem> GetList()
    {
        return m_Container;
    }
#endif

    public void Clear()
    {
        m_Container.Clear();
    }

    public FakeDictionaryItem GetItemFromKey(T_Key key)
    {
        for (int i = 0; i < m_Container.Count; i++)
        {
            if (ReferenceEquals(key, m_Container[i].m_Key))
            {
                return m_Container[i];
            }
        }
        return new FakeDictionaryItem();
    }

    public FakeDictionaryItem GetItemFromValue(T_Value value)
    {
        for (int i = 0; i < m_Container.Count; i++)
        {
            if (ReferenceEquals(value, m_Container[i].m_Value))
            {
                return m_Container[i];
            }
        }
        return new FakeDictionaryItem();
    }


    public FakeDictionaryItem this[int i]
    {
        get
        {
            if (i < m_Container.Count)
            {
                return m_Container[i];
            }
            else
            {
                Debug.LogError($"container does not have an index at {i}");
                return new FakeDictionaryItem();
            }
        }
        set { m_Container[i] = value; }
    }

    public void Add(T_Key key, T_Value value)
    {
        FakeDictionaryItem lnew = new FakeDictionaryItem()
        {
            m_Key = key,
            m_Value = value
        };
        Add(lnew);
    }

    public void Add(FakeDictionaryItem item)
    {
        bool found = false;
        for (int i = 0; i < m_Container.Count; i++)
        {
            if (ReferenceEquals(item.m_Key, m_Container[i].m_Key))
            {
                found = true;
            }
        }

        if (false == found)
        {
            m_Container.Add(item);
        }
    }

    public int Count => m_Container.Count;
    public List<T_Key> Keys => m_Container.ConvertAll(x => x.m_Key);
    public List<T_Value> Values => m_Container.ConvertAll(x => x.m_Value);

    [System.Serializable]
    public class FakeDictionaryItem
    {
        [SerializeField]
        public T_Key m_Key;

        [SerializeField]
        public T_Value m_Value;

        public FakeDictionaryItem(T_Key key, T_Value value)
        {
            m_Key = key;
            m_Value = value;
        }

        public FakeDictionaryItem()
        {
            m_Key = default(T_Key);
            m_Value = default(T_Value);
        }
    }
}
