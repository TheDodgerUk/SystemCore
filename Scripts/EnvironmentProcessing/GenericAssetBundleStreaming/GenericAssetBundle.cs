using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAssetBundle<T> where T : UnityEngine.Object
{
    protected virtual string ASSET_TYPE => nameof(MaterialAssetBundle);

    public virtual string AssetBundleLocation => $"environment/{Core.Environment.CurrentEnvironment.OverloadEnvironmentName()}/{Core.Environment.CurrentEnvironment.OverloadVariantName()}/{ASSET_TYPE}".ToLower();

    private string AssetBundleLocationNetwork => $"{AssetBundleLocation}.unity3d";

    private string AssetBundleLocationNetworkVersion => $"{AssetBundleLocation}_Version";

    protected Dictionary<string, T> m_Stored = new Dictionary<string, T>();
    protected MonoBehaviour m_Mono;

    public void Setup(MonoBehaviour mono)
    {
        m_Mono = mono;
        Core.Environment.OnEnvironmentUnloaded += ClearAll;
    }
    protected virtual bool ForceDownload => false;

    public void DeleteOldData(Dictionary<string, BundleLoader.DependencyData> missing, Action callback)
    {
        if (Core.Environment.CurrentEnvironment.IncludeGenericAssetBundles == false || ForceDownload == true)
        {
            var path = BundleLoader.CreatePersistentDataPath(AssetBundleLocationNetwork);
            var stored = Core.Contents.FileExist(path, "private void LoadBundle", false);
            if (stored == StreamingAssetsContents.ContentsEnum.ServerAndStored)
            {
                // local version
                var pathJson = BundleLoader.CreatePersistentDataPath(AssetBundleLocation);
                string versionFile = BundleLoader.CreateVersionFileName(pathJson);
                var persistant = Json.JsonNet.ReadFromFile<string>(versionFile);

                var network = BundleLoader.RemovePersistentDataPath(AssetBundleLocation);
                string networkVersion = BundleLoader.CreateVersionFileName(network);
                Core.Network.LoadAssetJsonFile<GlobalConsts.AssetBundleVersion>(networkVersion, true, (version) =>
                {
                    bool validNumber = int.TryParse(persistant, out int number);
                    if (validNumber == false || number != version.Version)
                    {
                        System.IO.File.Delete(path);
                        System.IO.File.Delete(versionFile + ".json");
                        AddItem(missing, version);
                    }
                    callback?.Invoke();
                });
            }
            else
            {
                var network = BundleLoader.RemovePersistentDataPath(AssetBundleLocation);
                string networkVersion = BundleLoader.CreateVersionFileName(network);
                Core.Network.LoadAssetJsonFile<GlobalConsts.AssetBundleVersion>(networkVersion, true, (version) =>
                {
                    if (version.FileSizeMB != 0)
                    {
                        AddItem(missing, version);
                    }
                    callback?.Invoke();
                });
            }
        }
        else
        {
            callback?.Invoke();
        }
    }

    private void AddItem(Dictionary<string, BundleLoader.DependencyData> missing, GlobalConsts.AssetBundleVersion version)
    {
        BundleLoader.DependencyData newData = new BundleLoader.DependencyData();
        newData.BundleName = AssetBundleLocationNetwork;
        newData.Version = version.Version;
        newData.SizeMB = version.FileSizeMB;
        if (missing.ContainsKey(newData.BundleName) == false)
        {
            missing.Add(newData.BundleName, newData);
        }
    }


    private void ClearAll()
    {
        m_Stored.Clear();
    }

    private void WaitUntilLoaded(Action callback)
    {
        Core.Mono.WaitUntil(1, () => Core.Assets.IsBundleLoaded(AssetBundleLocation) == true, () =>
        {
            callback?.Invoke();
        });
    }


    public virtual void GetItem(MonoBehaviour host, string name, Action<T> callback)
    {
        MonoBehaviour mono = host;
        if (mono == null)
        {
            mono = m_Mono;
        }
        if (true == string.IsNullOrEmpty(name))
        {
            callback?.Invoke(default(T));
        }
        else
        {
            Core.Assets.LoadAsset<T>(mono, name, AssetBundleLocation, (item) =>
            {
                if (item != null)
                {   
                    if (m_Stored.ContainsKey(name) == false)
                    {
                        m_Stored.Add(name, item);
                    }
                    ExtraStepsOnCreatedItem(item);
                    callback?.Invoke(item);
                }
                else
                {
                    callback?.Invoke(default(T));
                }
            }, 0);
        }
    }

    protected virtual void ExtraStepsOnCreatedItem(T item) { }

    public virtual void GetItemList(MonoBehaviour host, Action<List<string>> callbackList)
    {
        Core.Assets.WhatsInAssetBundle(host, AssetBundleLocation, callbackList);
    }

    public void GetItemList(MonoBehaviour host, string NameStartsWith, Action<List<string>> callbackList)
    {
        Core.Assets.WhatsInAssetBundle(host, AssetBundleLocation, (list) =>
        {
            List<string> contents = new List<string>();
            foreach (var item in list)
            {
                string lower = item.ToLower();
                if (true == lower.StartsWith(NameStartsWith.ToLower()))
                {
                    contents.Add(item);
                }
            }
            callbackList?.Invoke(contents);
        });
    }

    public void GetLocalVersionNumber(Action<int> callback)
    {
        var versionFile = BundleLoader.CreatePersistentDataPath(AssetBundleLocationNetworkVersion);
        GlobalConsts.ConvertToAndroidPath(ref versionFile);

        // this checks local saved items, NOT server
        Json.AndroidNet.ReadFromFileAsync<string>(Core.Mono, false, versionFile, JsonLibraryType.FullSerializer, (data) =>
        {
            string dateDebug = data;
            if (string.IsNullOrEmpty(data) == true)
            {
                callback?.Invoke(-1);
            }
            else
            {
                if (int.TryParse(data, out int result) == true)
                {
                    callback?.Invoke(result);
                }
                else
                {
                    callback?.Invoke(-1);
                }
            }
        });
    }


    public void GetNetworkVersionNumber(Action<int> callback)
    {
        var versionFile = BundleLoader.CreatePersistentDataPath(AssetBundleLocationNetworkVersion);
        GlobalConsts.ConvertToAndroidPath(ref versionFile);

        var network = BundleLoader.RemovePersistentDataPath(AssetBundleLocation);
        string networkVersion = BundleLoader.CreateVersionFileName(network);
        Core.Network.LoadAssetJsonFile<GlobalConsts.AssetBundleVersion>(networkVersion, true, (version) =>
        {
            if(version != null)
            {
                callback?.Invoke(version.Version);
            }
            else
            {
                callback?.Invoke(-1);
            }
            
        });
    }


    public void DownloadAndOverwrite(Action<float> progress, Action callback)
    {
        Dictionary<string, BundleLoader.DependencyData> missingFromLocal = new Dictionary<string, BundleLoader.DependencyData>();
        DeleteOldData(missingFromLocal, () =>
        {
            Core.Assets.DownloadAndOverwriteSingleAsset(AssetBundleLocationNetwork, progress, callback);
        });
    }


}
