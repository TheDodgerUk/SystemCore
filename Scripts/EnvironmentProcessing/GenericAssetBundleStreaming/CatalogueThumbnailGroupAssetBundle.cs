using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueThumbnailGroupAssetBundle
{
    private const int CATALOGUE_ITEM_COUNT = 6;
    private string AssetBundleLocation => "catalogue/cataloguethumbnailgroup";
    private string AssetBundleLocationNetwork => $"{AssetBundleLocation}.unity3d";
    private string AssetBundleLocationNetworkVersion => $"{AssetBundleLocation}_Version";


    private readonly string NUMBER_TO_FIND = $"0{(CATALOGUE_ITEM_COUNT - 1)}";

    private Dictionary<string, List<Texture2D>> m_Stored = new Dictionary<string, List<Texture2D>>();

    protected bool ForceDownload => true;

    public void Setup()
    {
        Core.Environment.OnEnvironmentUnloaded += delegate () { m_Stored.Clear(); };
    }



    public virtual void GetItem(MonoBehaviour host, string name, Action<List<Texture2D>> callback)
    {
        List<Texture2D> newList = new List<Texture2D>();


        if (true == string.IsNullOrEmpty(name))
        {
            callback?.Invoke(newList);
        }
        else
        {
            if (m_Stored.ContainsKey(name) == false)
            {
                TaskAction task = new TaskAction(CATALOGUE_ITEM_COUNT, () =>
                {
                    m_Stored.Add(name, newList);
                    callback?.Invoke(newList);
                });

                for (int i = 0; i < CATALOGUE_ITEM_COUNT; i++)
                {
                    string nameNumber = $"{name}_{i.ToString("00")}";
                    Core.Assets.LoadAsset<Texture2D>(host, nameNumber, AssetBundleLocation, (item) =>
                    {
                        newList.Add(item);
                        task.Increment();
                    }, 0);
                }
            }
            else
            {
                callback?.Invoke(m_Stored[name]);
            }
        }
    }

    public void GetItemList(MonoBehaviour host, Action<List<string>> callbackList)
    {
        Core.Assets.WhatsInAssetBundle(host, AssetBundleLocation, callbackList);
    }

    public void GetItemList(MonoBehaviour host, string NameStartsWith, Action<List<string>> callbackList)
    {
        Core.Assets.WhatsInAssetBundle(host, AssetBundleLocation, (list) =>
        {
            List<string> contains = new List<string>();
            foreach (var item in list)
            {
                string lower = item.ToLower();
                if (true == lower.StartsWith(NameStartsWith.ToLower()) && lower.EndsWith(NUMBER_TO_FIND))
                {
                    string shortVersion = item.Substring(0, (item.Length - NUMBER_TO_FIND.Length));
                    contains.Add(shortVersion);
                }
            }
            callbackList?.Invoke(contains);
        });
    }


    // copy from GenericAssetBundle
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

    // copy from GenericAssetBundle
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

    public void DownloadAndOverwrite(Action<float> progress, Action callback)
    {
        Dictionary<string, BundleLoader.DependencyData> missingFromLocal = new Dictionary<string, BundleLoader.DependencyData>();
        DeleteOldData(missingFromLocal, () =>
        {
            Core.Assets.DownloadAndOverwriteSingleAsset(AssetBundleLocationNetwork, progress, callback);
        });
    }

}
