using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CatalogueEntry
{
    public class SubItems
    {
        [System.NonSerialized]
        public UnityEngine.GameObject m_GameObject;

        [UnityEngine.SerializeField]
        public string m_ObjectName;

        [UnityEngine.SerializeField]
        public string m_GUID;
    }

    public string ShortName => GetLocalisation("short");
    public string FullName => GetLocalisation("full");

    public readonly string Guid;
    public List<MetaAttribute> Attributes;
    [Newtonsoft.Json.JsonConverter(typeof(MetaDataConverter))]
    public Dictionary<MetaDataType, MetaData> MetaData;


    public List<string> CatalogueFilters = new List<string>();

    public List<SubItems> m_SubItems = new List<SubItems>();

    public int Version = -1;

    public string ReferenceNumber;

    private Dictionary<Type, MetaData> m_LoadedDataByType = new Dictionary<Type, MetaData>();

    public override string ToString() => $"{Guid} - {FullName}";

    public T GetMetaData<T>() where T : MetaData => m_LoadedDataByType.Get(typeof(T)) as T;
    public Dictionary<MetaDataType, MetaData> GetMetaData() => MetaData;
    public List<MetaData> GetAllMetaData() => MetaData.Values.ToList();
    public List<MetaData> GetLoadedMetaData() => m_LoadedDataByType.Values.ToList();

    private void FilterAttributeData(List<string> attributesToKeep)
    {
        List<MetaAttribute> allowedAttributes = FilterEnum<MetaAttribute>(attributesToKeep);

        for(int i = 0; i < Attributes.Count; i++)
        {
            //Cant find this attribute in our allowed list... remove it
            //so spawned catalogue items dont have this attribute
            if(false == allowedAttributes.Contains(Attributes[i]))
            {
                Attributes.RemoveAt(i);
                i--;
            }
        }
    }

    private List<T> FilterEnum<T>(List<string> strings) where T : Enum
    {
        List<T> list = new List<T>();

        for(int i = 0; i < strings.Count; i++)
        {
            T type = (T)Enum.Parse(typeof(T), strings[i]);
            if(true == Enum.IsDefined(typeof(T), type))
            {
                list.Add(type);
            }
        }

        return list;
    }

    public void LoadMetaData(MonoBehaviour host, Action callback)
    {
        var attributesAllowed = SceneDataManager.Instance.AttributesLoaded;

        // cache meta data by type
        var allMetaData = GetAllMetaData();
        m_LoadedDataByType = allMetaData.ExtractAsValues(d => d.GetType());

        FilterAttributeData(attributesAllowed);

        // initialise internal meta data
        SequentialAction.List(allMetaData, (metaData, onTick) =>
        {
            metaData.Initialise(host, this, onTick);
        }, callback);
    }

    public void LoadMainPrefab(MonoBehaviour host, Action<GameObject> callback)
    {
        var renderMetaData = GetMetaData<RenderMetaData>();
        if (renderMetaData != null)
        {
            renderMetaData.LoadMainPrefab(host, callback);
        }
        else
        {
            callback(null);
        }
    }

    private string GetLocalisation(string suffix) => Localisation.Instance.Get(GetLocalisationKey(Guid, suffix));
    public static string GetLocalisationKey(string uuid, string suffix) => $"catalogue_entry_{uuid}_{suffix}";

    public MetaData AddMetaData(MetaDataType enumType)
    {
        var type = enumType.GetDataType();
        if (type != null)
        {
            var data = Activator.CreateInstance(type) as MetaData;
            //////// data.OnPostLoad(this);
            if(MetaData == null)
            {
                MetaData = new Dictionary<MetaDataType, MetaData>();
            }
            MetaData.Add(enumType, data);

            var keys = MetaData.Keys.ToList();
            keys.Sort();

            var clone = MetaData.Clone();
            MetaData.Clear();

            foreach (var key in keys)
            {
                MetaData.Add(key, clone[key]);
            }
            return data;
        }
        else
        {
            UnityEngine.Debug.LogError($"Could not add MetaData of type \"{enumType}\". There is no matching C# type.\n");
            return null;
        }
    }

    public MetaData AddMetaData(MetaDataType enumType, MetaData data)
    {
        var type = enumType.GetDataType();
        if (type != null)
        {
            //////// data.OnPostLoad(this);
            if (MetaData == null)
            {
                MetaData = new Dictionary<MetaDataType, MetaData>();
            }
            MetaData.Add(enumType, data);

            var keys = MetaData.Keys.ToList();
            keys.Sort();

            var clone = MetaData.Clone();
            MetaData.Clear();

            foreach (var key in keys)
            {
                MetaData.Add(key, clone[key]);
            }
            return data;
        }
        else
        {
            UnityEngine.Debug.LogError($"Could not add MetaData of type \"{enumType}\". There is no matching C# type.\n");
            return null;
        }
    }
}