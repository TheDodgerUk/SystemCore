using JsonPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAssetBundle : GenericAssetBundle<TextAsset>
{
    protected override string ASSET_TYPE => nameof(TextAssetBundle);

    public override void GetItemList(MonoBehaviour host, Action<List<string>> callbackList)
    {
        Core.Assets.WhatsInAssetBundleWithPath(host, AssetBundleLocation, callbackList);
    }

    public void GetItem<T>(MonoBehaviour host, string name,  Action<T> data)
    {
        GetItem(host, name, (raw) =>
        {
            if (raw != null)
            {
                T converted = Json.JsonNet.ReadFromText<T>(raw.text);
                data?.Invoke(converted);
            }
            else
            {
                Debug.LogError($"Cannot find: {name}");
                data?.Invoke(default(T));
            }
        });
    }

    public override void GetItem(MonoBehaviour host, string name, Action<TextAsset> callback)
    {
        MonoBehaviour mono = host;
        if (mono == null)
        {
            mono = m_Mono;
        }
        if (true == string.IsNullOrEmpty(name))
        {
            callback?.Invoke(default(TextAsset));
        }
        else
        {
            Core.Assets.LoadAssetWithFullName<TextAsset>(mono, name, AssetBundleLocation, (item) =>
            {
                if (item != null)
                {
                    if (m_Stored.ContainsKey(name) == false)
                    {
                        m_Stored.Add(name, item);
                    }
                    callback?.Invoke(item);
                }
                else
                {
                    callback?.Invoke(default(TextAsset));
                }
            }, 0);
        }
    }

}

