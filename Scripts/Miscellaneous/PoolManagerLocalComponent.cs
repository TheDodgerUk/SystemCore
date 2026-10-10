using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PoolManagerLocalComponent<T> where T : UnityEngine.Component
{
    private Transform m_Parent;
    private GameObject m_Prefab;

    private List<T> m_PoolList = new List<T>();
    private List<T> m_PublicList = new List<T>();


    public PoolManagerLocalComponent(GameObject prefab, Transform parent)
    {
        m_Parent = parent;
        m_Prefab = GameObject.Instantiate(prefab);
        m_Prefab.name = prefab + $"_Prefab";
        m_Prefab.transform.SetParent(parent.transform);
        prefab.SetActive(false);
        m_Prefab.SetActive(false);
        m_PoolList.Clear();
    }


    public T SpawnObject()
    {
        m_PoolList.RemoveAll(e => e == null);
        m_PublicList.RemoveAll(e => e == null);
        for (int i = 0; i < m_PoolList.Count; ++i)
        {
            if (m_PublicList.Contains(m_PoolList[i]) == false)
            {
                m_PoolList[i].SetActive(true);
                m_PoolList[i].gameObject.transform.SetParent(m_Parent);
                m_PublicList.Add(m_PoolList[i]);
                return m_PoolList[i];
            }
        }

        CreateObject(m_Prefab);
        T obj = m_PoolList[m_PoolList.Count - 1];
        obj.SetActive(true);
        obj.gameObject.transform.SetParent(m_Parent);
        m_PublicList.Add(obj);
        return obj;
    }


    public T SpawnObject(Transform parent)
    {
        m_PoolList.RemoveAll(e => e == null);
        m_PublicList.RemoveAll(e => e == null);
        for (int i = 0; i < m_PoolList.Count; ++i)
        {
            if (m_PoolList[i].gameObject.activeSelf == false)
            {
                m_PoolList[i].SetActive(true);
                m_PoolList[i].gameObject.transform.SetParent(parent);
                m_PublicList.Add(m_PoolList[i]);
                return m_PoolList[i];
            }
        }

        CreateObject(m_Prefab);
        T obj = m_PoolList[m_PoolList.Count - 1];
        obj.SetActive(true);
        obj.gameObject.transform.SetParent(parent);
        m_PublicList.Add(obj);
        return obj;
    }

    public List<T> GetPublicList()
    {
        m_PoolList.RemoveAll(e => e == null);
        m_PublicList.RemoveAll(e => e == null);
        return m_PublicList;
    }


    public void DeSpawnObject(T spawnedObject)
    {
        m_PublicList.Remove(spawnedObject);
        spawnedObject.SetActive(false);
        spawnedObject.transform.SetParent(m_Parent);
        spawnedObject.transform.localPosition = Vector3.zero;
        m_PoolList.RemoveAll(e => e == null);
        m_PublicList.RemoveAll(e => e == null);
    }

    public void DeSpawnAll()
    {
        m_PoolList.RemoveAll(e => e == null);
        m_PublicList.RemoveAll(e => e == null);
        try
        {
            for (int i = m_PublicList.Count - 1; i >= 0; i--)
            {
                DeSpawnObject(m_PublicList[i]);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

    }

    public void DeleteAll()
    {
        for (int i = m_PoolList.Count - 1; i >= 0; --i)
        {
            GameObject.Destroy(m_PoolList[i]);
        }
        m_PoolList.Clear();
    }


    private void OnDestroy()
    {
        DeleteAll();
    }


    private GameObject CreateObject(GameObject prefab)
    {

        GameObject newItem = GameObject.Instantiate(prefab);
        newItem.name = m_Prefab + $"_{m_PoolList.Count}";

        if (m_Parent != null)
        {
            newItem.transform.SetParent(m_Parent);
            newItem.transform.ClearLocals();
        }
        m_PoolList.Add(newItem.ForceComponent<T>());
        DeSpawnObject(m_PoolList.Last());
        return newItem;

    }
}


