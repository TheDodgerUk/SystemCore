using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[MetaData(MetaDataType.Render)]
public class RenderMetaData : MetaData
{
    public string RuntimeDirectory = null;
    public readonly RenderItemData Main;
    public readonly List<RenderItemData> Parts;
    public readonly List<RenderItemData> Variants;
    public readonly int Version;
    public readonly float[] SizeOfFileMB = new float[GlobalConsts.PlatformNamesLength];

    public RenderMetaData()
    {
        Variants = new List<RenderItemData>();
        Parts = new List<RenderItemData>();
        Main = new RenderItemData();
    }

    public void LoadMainPrefab(MonoBehaviour host, Action<GameObject> callback)
    {
        Main.LoadPrefab(host, Version, callback);
    }

    public void LoadPartPrefab(MonoBehaviour host, int partNumber, Action<GameObject> callback)
    {
        if (Parts.Count > partNumber)
        {
            Parts[partNumber].LoadPrefab(host, Version, callback);
        }
        else
        {
            callback?.Invoke(null);
        }
    }
}

[System.Serializable]
public class RenderItemData
{
    public readonly string Bundle;
    public readonly string Prefab;

    public void LoadPrefab(MonoBehaviour host, int version, Action<GameObject> callback)
    {
        Core.Assets.LoadAsset(host, Prefab, Bundle, callback, version);
    }
}
