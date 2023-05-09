using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PoolManager<T> where T : class
{
    private List<T> m_PoolList = new List<T>();
    private List<T> m_PublicList = new List<T>();
    private GameObject m_Prefab;
    private GameObject m_Parent;

    private Func<GameObject, T> m_onCreateNew;
    private Func<T, T> m_onSpawn;
    private Func<T, T> m_onDeSpawn;


    public PoolManager(GameObject prefab, GameObject parent, Func<GameObject, T> onCreateNew, Func<T, T> onSpawn, Func<T, T> onDeSpawn)
    {
        m_Parent = parent;
        m_Prefab = GameObject.Instantiate(prefab);
        m_Prefab.name = prefab + $"_Prefab";
        m_Prefab.transform.SetParent(parent.transform);
        prefab.SetActive(false);
        m_Prefab.SetActive(false);
        m_PoolList.Clear();

        m_onCreateNew = onCreateNew;
        m_onSpawn = onSpawn;
        m_onDeSpawn = onDeSpawn;
    }


    public T SpawnObject()
    {
        for (int i = 0; i < m_PoolList.Count; ++i)
        {
            if(m_PublicList.Contains(m_PoolList[i]) == false)
            { 
                m_PublicList.Add(m_PoolList[i]);
                var item =  m_PoolList[i];
                m_onSpawn(item);
                return item;
            }
        }

        CreateObject();
        T obj = m_PoolList[m_PoolList.Count - 1];
        m_PublicList.Add(obj);
        m_onSpawn(obj);
        return obj;
    }


    public List<T> GetPublicList() => m_PublicList;


    public void DeSpawnObject(T spawnedObject)
    {
        m_PublicList.Remove(spawnedObject);
        m_onDeSpawn(spawnedObject);
    }

    public void DeSpawnAll()
    {
        try
        {
            for (int i = m_PublicList.Count - 1; i >= 0; i--)
            {
                DeSpawnObject(m_PublicList[i]);
            }

            for (int i = m_PoolList.Count - 1; i >= 0; i--)
            {
                DeSpawnObject(m_PoolList[i]);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

    }


    private T CreateObject()
    {
        var prefab = GameObject.Instantiate(m_Prefab);
        prefab.name = m_Prefab + $"_{m_PoolList.Count}";
        prefab.transform.SetParent(m_Parent.transform);
        var newItem = m_onCreateNew(prefab);
        m_PoolList.Add(newItem);

        DeSpawnObject(newItem);
        return newItem;
    }
}


