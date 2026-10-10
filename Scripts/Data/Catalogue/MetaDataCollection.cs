using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MetaDataCollection
{
    public List<MetaData> LoadedData => m_LoadedMetaData.Clone();

    public readonly List<MetaAttribute> Attributes;
    public readonly List<MetaDataType> ExternalTypes;
    [Newtonsoft.Json.JsonConverter(typeof(MetaDataConverter))]
    public readonly Dictionary<MetaDataType, MetaData> InternalData;

    private Dictionary<Type, MetaData> m_LoadedDataByType;
    private List<MetaData> m_LoadedMetaData;

    public T Get<T>() where T : MetaData => m_LoadedDataByType.Get(typeof(T)) as T;

    public void LoadMetaData(MonoBehaviour host, CatalogueEntry entry, Action callback)
    {
        if (m_LoadedMetaData != null)
        {
            callback();
            return;
        }

        // add internal meta data to list
        m_LoadedMetaData = InternalData.Values.ToList();

        // initialise internal meta data
        SequentialAction.List(InternalData.Values.ToList(), (metaData, onTick) =>
        {
            //metaData.Initialise(host, onTick);
        }, () =>
        {
            // begin loading external types
            SequentialAction.List(ExternalTypes, (enumType, onTick) =>
            {
                LoadExternalMetaData(host, entry, enumType, onTick);
            }, () =>
            {
                m_LoadedDataByType = m_LoadedMetaData.ExtractAsValues(d => d.GetType());
                callback();
            });
        });
    }

    private void LoadExternalMetaData(MonoBehaviour host, CatalogueEntry entry, MetaDataType enumType, Action onTick)
    {
        // can convert this type? 
        //var dataType = enumType.GetDataType();
        //if (dataType != null)
        //{
        //    // load file into data type
        //    string filePath = Catalogue.GetExternalMetaDataPath(entry.Guid, enumType);
        //    var loadedData = Json.JsonNet.ReadFromFile(filePath, dataType);

        //    var metaData = loadedData as MetaData;
        //    if (metaData != null)
        //    {
        //        metaData.Initialise(host, () =>
        //        {
        //            OnMetaDataInitialised(host, enumType, dataType, loadedData, onTick);
        //        });
        //    }
        //    else
        //    {
        //        OnMetaDataInitialised(host, enumType, dataType, loadedData, onTick);
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning($"Could not find a data type to match to enum {enumType}\n", host);
        //    onTick();
        //}
    }

    private void OnMetaDataInitialised(MonoBehaviour host, MetaDataType enumType, Type dataType, object loadedData, Action callback)
    {
        //// does this data type have a proxy type?
        //var proxyType = enumType.GetProxyType();
        //if (proxyType != dataType)
        //{
        //    // create and initialise proxy type
        //    var proxyData = Activator.CreateInstance(proxyType) as IProxyMetaData;
        //    proxyData.InitialiseProxy(host, loadedData, () => host.WaitForFrame(callback));
        //    m_LoadedMetaData.Add((MetaData)proxyData);
        //}
        //else
        //{
        //    m_LoadedMetaData.Add((MetaData)loadedData);
        //}
    }
}
