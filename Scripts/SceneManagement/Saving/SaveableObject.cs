using System;
using UnityEngine;

public abstract class SaveableObject<TSaveData> : MonoBehaviour, ISaveable where TSaveData : SaveData
{
    public string RuntimeId => m_RuntimeId;

    [SerializeField, ReadOnly]
    private string m_RuntimeId;

    protected virtual void Awake()
    {
        GenerateNewGuid();
    }

    public void GenerateNewGuid()
    {
        m_RuntimeId = Utils.Code.GenerateGuid();
    }

    public void OnPostLoad(SaveData data)
    {
        var typeData = VerifySaveDataType(data);
        if (typeData != null)
        {
            OnPostLoadData(typeData);
        }
    }

    public void Load(SaveData data, Action callback)
    {
        var typeData = VerifySaveDataType(data);
        if (typeData != null)
        {
            LoadAsType(typeData, callback);
        }
    }

    public SaveData Save() => SaveAsType();
    public TSaveData SaveAsType() => SaveData();

    public void LoadAsType(TSaveData data, Action callback)
    {
        m_RuntimeId = data.RuntimeId;
        LoadData(data, callback);
    }

    protected virtual void OnPostLoadData(TSaveData data) { }
    protected abstract void LoadData(TSaveData data, Action callback);
    protected abstract TSaveData SaveData();

    private TSaveData VerifySaveDataType(SaveData data)
    {
        if (data is TSaveData)
        {
            return (data as TSaveData);
        }
        else if (data != null)
        {
            Debug.LogError($"Passed an incorrect TSaveData type to: {GetType().Name} on {name}\nExpected: {typeof(TSaveData).Name} got {data.GetType().Name}");
        }
        else
        {
            Debug.LogError($"Passed null TSaveData type to: {GetType().Name} on {name}\n");
        }
        return null;
    }
}
