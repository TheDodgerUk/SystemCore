using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AssetLoader
{
    private BundleLoader m_BundleLoader;
    private MonoBehaviour m_Mono;


    public const int ASSET_NOT_FOUND = -1; 

    public static string RendererFullPath(string bundleName) => "StreamingAssets" + BundleLoader.BundlePathShort + bundleName;
    public static string RendererFullPath(RenderMetaData data) => RendererFullPath(data.Main.Bundle);


    public class DownloadDepencys
    {
        public int CurrentIndex { get; private set; }
        public int TotalCount { get; private set; }
        public float TotalMeg { get; private set; }
        public float MegLeft { get; private set; }

        public void UpdateData(int currentIndex,int totalCount, float totalMeg, float megLeft)
        {
            CurrentIndex = currentIndex; 
            TotalCount = totalCount;
            TotalMeg = totalMeg;
            MegLeft = megLeft;
        }
    }

    public DownloadDepencys DownloadDepencysRef = new DownloadDepencys();

    public void Initialise(MonoBehaviour mono, Action callback)
    {
        m_Mono = mono;
        m_BundleLoader = new BundleLoader();
        m_BundleLoader.LoadAssetDatabase(m_Mono, callback);
    }

    public void Shutdown()
    {
        m_BundleLoader?.UnloadAll();
        m_BundleLoader = null;
    }

    public bool IsBundleLoaded(string bundleName)
    {
        if (bundleName.EndsWith(BundleLoader.BundleExtension) == false)
        {
            bundleName += BundleLoader.BundleExtension;
        }
        return m_BundleLoader.IsBundleLoaded(bundleName);
    }


    public void LoadBundle(MonoBehaviour host, string bundleName, Action<AssetBundle> callback, int version)
    {
        // load or retrieve bundle from loader
        m_BundleLoader.GetBundle(bundleName, callback, host, version);
    }


    public void UpdateLocalAssetsFromServer(Action<string> onMessage, Action<float> onProgress, Action<float> callBack)
    {
        m_Mono.StartCoroutine(CheckWhatsLocallyStoredThenDownload(onMessage, onProgress, callBack));
    }

    private IEnumerator CheckWhatsLocallyStoredThenDownload(Action<string> onMessage, Action<float> onProgress, Action<float> callBack)
    {
#if UNITY_EDITOR
        bool debugMessageDisplayed = false;
#endif
        var cat = Core.Catalogue.GetCatalogue;

        if (Core.Environment.CurrentEnvironment.CataloguePrefabsFromServer == true)
        {
            yield return new WaitForSeconds(3f); // time to read "Calculating..."
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        int notFoundCount = 0;
        int outOfDate = 0;
        Dictionary<string, BundleLoader.DependencyData> dependencies = new Dictionary<string, BundleLoader.DependencyData>();
        Dictionary<string, BundleLoader.DependencyData> missingFromLocal = new Dictionary<string, BundleLoader.DependencyData>();

        var ExtrasFromServer = GlobalConsts.GetExtraFromServerInMask(Core.Environment.CurrentEnvironment.ExtraBundlesFromServer);


        TaskAction taskGenetic = new TaskAction(ExtrasFromServer.Count, () =>
        {
            if (Core.Environment.CurrentEnvironment.CataloguePrefabsFromServer == true)
            {
                TaskAction task = new TaskAction(cat.Count, () =>
                {
                    if(notFoundCount != 0)
                    {
                        Debug.LogError($"notFoundCount : {notFoundCount}");
                    }
                    if (outOfDate != 0)
                    {
                        Debug.LogError($"outOfDate : {outOfDate}");
                    }

                    Core.Mono.StartCoroutine(DownloadMissingCatalogueItemsFromServer(cat, dependencies, missingFromLocal, onMessage, onProgress, callBack));
                });

                for (int i = 0; i < cat.Count; i++)
                {
                    var catEntry = cat[i];
                    var rend = cat[i].GetMetaData<RenderMetaData>();
                    string fullpath = RendererFullPath(rend);

                    string versionFile = BundleLoader.CreatePersistentDataPath(rend.Main.Bundle);
                    versionFile = BundleLoader.CreateVersionFileName(versionFile);


                    GlobalConsts.ConvertToAndroidPath(ref versionFile);
                    // this checks local saved items, NOT server
                    Json.AndroidNet.ReadFromFileAsync<string>(Core.Mono, false, versionFile, JsonLibraryType.FullSerializer, (data) =>
                    {
                        string dateDebug = data;
                        if (string.IsNullOrEmpty(data) == true)
                        {
                            ConsoleExtra.Log($"Item not found, add to items to download", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
                            DebugOnce.LogError($"Item not found, add to items to download");
                            AddToItemsToDownload(catEntry, dependencies, missingFromLocal, rend);
                            notFoundCount++;
                            task.Increment();
                        }
                        else
                        {
                            var renderVersion = catEntry.Version;
                            if (data != renderVersion.ToString())
                            {
                                ConsoleExtra.Log($"outOfDate", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
                                DebugOnce.LogError($"Item not found, add to items to download");
                                AddToItemsToDownload(catEntry, dependencies, missingFromLocal, rend);
                                outOfDate++;
                                task.Increment();
                            }
                            else
                            {
                                task.Increment();
                            }
                        }
                    });
                }
            }
            else
            {
                callBack?.Invoke(0);
            }
        });


        foreach (var extraFromServerItem in ExtrasFromServer)
        {
            switch (extraFromServerItem)
            {
                case GlobalConsts.ExtraFromServer.AudioClipAssetBundle:
                    Core.AssetBundlesRef.AudioClipAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.GameObjectAssetBundle:
                    Core.AssetBundlesRef.GameObjectAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.MaterialAssetBundle:
                    Core.AssetBundlesRef.MaterialAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.Texture2DAssetBundle:
                    Core.AssetBundlesRef.Texture2DAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.VideoClipAssetBundle:
                    Core.AssetBundlesRef.VideoClipAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.TextAssetBundle:
                    Core.AssetBundlesRef.TextAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.CatalogueThumbnail:
                    Core.AssetBundlesRef.CatalogueThumbnailAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.CatalogueThumbnailGroup:
                    Core.AssetBundlesRef.CatalogueThumbnailGroupAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                default:
                    break;
            }
        }            
    }

    public void AreThereItemsToDownload(Action<bool> itemsToDownload)
    {
        m_Mono.StartCoroutine(CheckWhatsLocallyStored(itemsToDownload));
    }

    private IEnumerator CheckWhatsLocallyStored(Action<bool> itemsToDownload)
    {
#if UNITY_EDITOR
        bool debugMessageDisplayed = false;
#endif
        var cat = Core.Catalogue.GetCatalogue;


       yield return new WaitForEndOfFrame();


        int notFoundCount = 0;
        int outOfDate = 0;
        Dictionary<string, BundleLoader.DependencyData> dependencies = new Dictionary<string, BundleLoader.DependencyData>();
        Dictionary<string, BundleLoader.DependencyData> missingFromLocal = new Dictionary<string, BundleLoader.DependencyData>();

        var ExtrasFromServer = GlobalConsts.GetExtraFromServerInMask(Core.Environment.CurrentEnvironment.ExtraBundlesFromServer);


        TaskAction taskGenetic = new TaskAction(ExtrasFromServer.Count, () =>
        {
            if (Core.Environment.CurrentEnvironment.CataloguePrefabsFromServer == true)
            {
                TaskAction task = new TaskAction(cat.Count, () =>
                {
                    itemsToDownload?.Invoke(dependencies.Count != 0 || missingFromLocal.Count != 0);
                });

                for (int i = 0; i < cat.Count; i++)
                {
                    var catEntry = cat[i];
                    var rend = cat[i].GetMetaData<RenderMetaData>();
                    string fullpath = RendererFullPath(rend);

                    string versionFile = BundleLoader.CreatePersistentDataPath(rend.Main.Bundle);
                    versionFile = BundleLoader.CreateVersionFileName(versionFile);


                    GlobalConsts.ConvertToAndroidPath(ref versionFile);
                    // this checks local saved items, NOT server
                    Json.AndroidNet.ReadFromFileAsync<string>(Core.Mono, false, versionFile, JsonLibraryType.FullSerializer, (data) =>
                    {
                        string dateDebug = data;
                        if (string.IsNullOrEmpty(data) == true)
                        {
                            AddToItemsToDownload(catEntry, dependencies, missingFromLocal, rend);
                            notFoundCount++;
                            task.Increment();
                        }
                        else
                        {
                            var renderVersion = catEntry.Version;
                            if (data != renderVersion.ToString())
                            {
                                AddToItemsToDownload(catEntry, dependencies, missingFromLocal, rend);
                                outOfDate++;
                                task.Increment();
                            }
                            else
                            {
                                task.Increment();
                            }
                        }
                    });
                }
            }
            else
            {
                itemsToDownload?.Invoke(false);
            }
        });


        foreach (var extraFromServerItem in ExtrasFromServer)
        {
            switch (extraFromServerItem)
            {
                case GlobalConsts.ExtraFromServer.AudioClipAssetBundle:
                    Core.AssetBundlesRef.AudioClipAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.GameObjectAssetBundle:
                    Core.AssetBundlesRef.GameObjectAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.MaterialAssetBundle:
                    Core.AssetBundlesRef.MaterialAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.Texture2DAssetBundle:
                    Core.AssetBundlesRef.Texture2DAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.VideoClipAssetBundle:
                    Core.AssetBundlesRef.VideoClipAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.TextAssetBundle:
                    Core.AssetBundlesRef.TextAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.CatalogueThumbnail:
                    Core.AssetBundlesRef.CatalogueThumbnailAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                case GlobalConsts.ExtraFromServer.CatalogueThumbnailGroup:
                    Core.AssetBundlesRef.CatalogueThumbnailGroupAssetBundleRef.DeleteOldData(missingFromLocal, taskGenetic.Increment);
                    break;
                default:
                    break;
            }
        }
    }



    public void GetLocalVersionNumber(CatalogueEntry cat, Action<int> callback)
    {
        var rend = cat.GetMetaData<RenderMetaData>();
        string fullpath = RendererFullPath(rend);

        string versionFile = BundleLoader.CreatePersistentDataPath(rend.Main.Bundle);
        versionFile = BundleLoader.CreateVersionFileName(versionFile);


        GlobalConsts.ConvertToAndroidPath(ref versionFile);
        // this checks local saved items, NOT server
        Json.AndroidNet.ReadFromFileAsync<string>(Core.Mono, false, versionFile, JsonLibraryType.FullSerializer, (data) =>
        {
            string dateDebug = data;
            if (string.IsNullOrEmpty(data) == true)
            {
                callback?.Invoke(ASSET_NOT_FOUND);
            }
            else
            {
                if(int.TryParse(data, out int result) == true)
                {
                    callback?.Invoke(result);
                }
                else
                {
                    callback?.Invoke(ASSET_NOT_FOUND);
                }
            }
        });
    }



    private IEnumerator DownloadMissingCatalogueItemsFromServer(List<CatalogueEntry> cat , Dictionary<string, BundleLoader.DependencyData> dependencies, Dictionary<string, BundleLoader.DependencyData> missingFromLocal, Action<string> onMessage, Action<float> onProgress, Action<float> callBack)
    {
        float totalSize = 0;
        foreach (var item in dependencies)
        {
            var itemInCatalogue = cat.Find(e => e.Guid == item.Value.GUID);
            if(itemInCatalogue == null)
            {
                AddUniqueToList(missingFromLocal, item.Value);
            }
        }

        foreach (var item in missingFromLocal)
        {
            totalSize += item.Value.SizeMB;
        }


        float currentSize = 0;
        int count = 0;

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        bool contains = true;
        BundleLoader.DependencyData data = null;
        string mbPerSecond = "";

        foreach (var item in missingFromLocal)
        {
            if(data != null && contains == false)
            {
                m_BundleLoader.Unload(data.BundleName, data.Version, true);
            }
            data = item.Value;

            contains = m_BundleLoader.Contains(data.BundleName, data.Version);
            bool loaded = false;

            float amountLeft = totalSize - currentSize;
            string amountLeftString = ByteSizeLib.ByteSize.FromMegaBytes(amountLeft).ToString();
            DownloadDepencysRef.UpdateData(count, missingFromLocal.Count, totalSize, amountLeft);
            string message = "Item " + count + " of " + missingFromLocal.Count + ", " + amountLeftString + " remaining.";
            count++;
            if (onMessage != null)
            {
                Debug.Log($"{message}  {data.BundleName}");
                onMessage.Invoke(message);
            }
            else
            {
                Debug.LogError(message);
            }

            if (string.IsNullOrEmpty(item.Value.BundleName) == false)
            {
                DownloadSingleAssetCatalogueFromServerAndSaveLocally(data.BundleName, data.Version, (progress) =>
                // Make the progress report the total for the amount of items being downloaded.
                onProgress.Invoke(((1f / missingFromLocal.Count) * (count - 1)) + (progress / missingFromLocal.Count)), () =>
                {
                    loaded = true;
                });
            }
            else
            {
                loaded = true;
            }

            yield return new WaitUntil(() => loaded == true);

            currentSize += item.Value.SizeMB;

            float secondsSoFar = ((float)stopwatch.ElapsedMilliseconds * 0.001f);
            mbPerSecond = (currentSize / secondsSoFar).ToString();

        }

        stopwatch.Stop();
        yield return new WaitForSeconds(1.5f); // time to see it completed
        onProgress?.Invoke(1f); // force fill bar before end
        onMessage?.Invoke($"Update completed. seconds: {TimeFromMilliseconds(stopwatch.ElapsedMilliseconds)}, MB:{currentSize.ToString("0.00")}");
        if (missingFromLocal.Count != 0)
        {
            Debug.Log($"Time taken to upload {stopwatch.Elapsed.TotalSeconds}");
        }


        callBack?.Invoke(1.5f); // time to read the finished message
    }



    public void DownloadAndOverwriteSingleAssetIfOutOfDateAndDependencies(CatalogueEntry entry, Action<string> onMessage, Action<float> onProgress, Action callback)
    {
        int gotLocalversion = 0; 
        int gotNetworkAssetVersion = 0;
        TaskAction task = new TaskAction(2, () =>
        {
            if (gotLocalversion == ASSET_NOT_FOUND || gotLocalversion != gotNetworkAssetVersion)
            {
                Dictionary<string, BundleLoader.DependencyData> dependencies = new Dictionary<string, BundleLoader.DependencyData>();
                Dictionary<string, BundleLoader.DependencyData> missingFromLocal = new Dictionary<string, BundleLoader.DependencyData>();
                var rend = entry.GetMetaData<RenderMetaData>();
                AddToItemsToDownload(entry, dependencies, missingFromLocal, rend);


                List<CatalogueEntry> cat = new List<CatalogueEntry>() { entry };
                Core.Mono.StartCoroutine(DownloadMissingCatalogueItemsFromServer(cat, dependencies, missingFromLocal, onMessage, onProgress, (delayTime) => callback.Invoke()));
            }
            else
            {
                callback?.Invoke();
            }
        });


        Core.Catalogue.GetLocalAssetVersion(entry, (localversion) =>
        {
            gotLocalversion = localversion;
            task.Increment();
        });

        Core.Network.GetAssetVersionNumber(entry, (networkAssetVersion) =>
        {
            gotNetworkAssetVersion = networkAssetVersion;
            task.Increment();
        });
    }

    public void DownloadAndOverwriteSingleAsset(CatalogueEntry entry, Action<float> progress, Action callback)
    {
        var rend = entry.GetMetaData<RenderMetaData>();
        string fullpath = RendererFullPath(rend);

        string main = BundleLoader.CreatePersistentDataPath(rend.Main.Bundle);
        string versionFile = BundleLoader.CreateVersionFileName(main);

        try
        {
            File.Delete(main);
            File.Delete(versionFile);
        }
        catch (Exception e)
        {
            Debug.LogError($"using Exception as not sure how android will cope with file.exists: {e}");
        }
        DownloadSingleAssetCatalogueFromServerAndSaveLocally(rend.Main.Bundle, 0, progress, callback);
    }


    public void DownloadAndOverwriteSingleAsset(string Bundle, Action<float> progress ,Action callback)
    {
        string main = BundleLoader.CreatePersistentDataPath(Bundle);
        string versionFile = BundleLoader.CreateVersionFileName(main);

        try
        {
            File.Delete(main);
            File.Delete(versionFile);
        }
        catch (Exception e)
        {
            Debug.LogError("using Exception as not sure how android will cope with file.exists");
        }
        DownloadSingleAssetCatalogueFromServerAndSaveLocally(Bundle, 0, progress, callback);
    }




    private string TimeFromMilliseconds(double mil)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(mil);
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
               (int)t.TotalHours,
               t.Minutes,
               t.Seconds);
    }


    private void AddToItemsToDownload(CatalogueEntry catEntry, Dictionary<string, BundleLoader.DependencyData> dependencies, Dictionary<string, BundleLoader.DependencyData> missingFromLocal, RenderMetaData rend)
    {
        BundleLoader.DependencyData newData = new BundleLoader.DependencyData();

        newData.PrefabName = rend.Main.Prefab;
        newData.BundleName = rend.Main.Bundle;
        newData.FullName = RendererFullPath(rend);
        newData.GUID = catEntry.Guid;
        newData.Version = rend.Version;
        newData.SizeMB = rend.SizeOfFileMB[GlobalConsts.CurrentPlatformIndex()];
        AddUniqueToList(missingFromLocal, newData);

        var dep = m_BundleLoader.GetDependencies(rend.Main.Bundle);
        for (int depIndex = 0; depIndex < dep.Count; depIndex++)
        {
            var depItem = dep[depIndex];
            depItem.FullName = RendererFullPath(depItem.BundleName);
            AddUniqueToList(dependencies, depItem);
        }
    }



    private bool AddUniqueToList(Dictionary<string, BundleLoader.DependencyData> list, BundleLoader.DependencyData item)
    {
        var searchName = item.BundleName;
        if(searchName == null)
        {
            Debug.LogError($"searchName cannot be null, {item.PrefabName}");
        }
        if(GlobalConsts.IsExternalSourceAsset(searchName, withSlash: true) == true)
        {
            return false;
        }

        if (list.ContainsKey(item.BundleName) == false)
        {
            list.Add(item.BundleName, item);
            return true;
        }
        else
        {
            return false;
        }

    }

    public void DownloadSingleAssetCatalogueFromServerAndSaveLocally(string bundle, int version, Action<float> progress, Action callback)
    {
        m_BundleLoader.DownloadSingleAssetCatalogueFromServerAndSaveLocally(bundle, version, progress, callback);
    }


    public void WhatsInAssetBundle(MonoBehaviour host, string bundleName, Action<List<string>> callBackNames)
    {
        LoadBundle(host, bundleName, bundle =>
        {
            if (bundle != null)
            {
                List<string> without = new List<string>();
                foreach(var item in bundle.GetAllAssetNames())
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(item);
                    without.Add(fileName);

                }
                callBackNames?.Invoke(without);
            }
            else
            {
                callBackNames?.Invoke(new List<string>());
            }
        }, 0);
    }

    public void WhatsInAssetBundleWithPath(MonoBehaviour host, string bundleName, Action<List<string>> callBackNames)
    {
        LoadBundle(host, bundleName, bundle =>
        {
            if (bundle != null)
            {
                List<string> without = new List<string>();
                foreach (var item in bundle.GetAllAssetNames())
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item));
                    fileName = fileName.SanitiseSlashes();
                    var clean = bundleName.SanitiseSlashes();
                    clean = clean.Replace("environment/", "");
                    
                    int index = fileName.IndexOf(clean);
                    fileName = fileName.Substring(index + clean.Length +1);
                    without.Add(fileName);

                }
                callBackNames?.Invoke(without);
            }
            else
            {
                callBackNames?.Invoke(new List<string>());
            }
        }, 0);
    }

    public void LoadAsset<T>(MonoBehaviour host, string assetName, string bundleName, Action<T> callback, int version) where T : UnityEngine.Object
    {
        // load from resources or bundles?
        LoadBundle(host, bundleName, (bundle) =>
        {
            //AssetBundle debug = bundle;
            if (bundle != null)
            {
                host.WaitForOp(bundle.LoadAssetAsync<T>(assetName), op =>
                {
                    callback(op.asset as T);
                });
            }
            else
            {
                callback(null);
            }
        }, version);
    }

    public void LoadAssetWithFullName<T>(MonoBehaviour host, string assetName, string bundleName, Action<T> callback, int version) where T : UnityEngine.Object
    {
        // load from resources or bundles?
        LoadBundle(host, bundleName, (bundle) =>
        {
            AssetBundle debug = bundle;
            if (bundle != null)
            {
                var names = bundle.GetAllAssetNames().ToList();
                var foundAssetName = names.FirstOrDefault(e => e.CaseInsensitiveContains(assetName));
                // load asset from bundle
                if (string.IsNullOrEmpty(foundAssetName) == false)
                {
                    host.WaitForOp(bundle.LoadAssetAsync<T>(foundAssetName), op =>
                    {
                        callback(op.asset as T);
                    });
                }
                else
                {
                    Debug.LogError($"foundAssetName is null, assetName: {assetName} , bundleName : {bundleName}");
                    callback(null);
                }
            }
            else
            {
                Debug.LogError($"bundle is null, assetName: {assetName} , bundleName : {bundleName}");
                callback(null);
            }
        }, version);
    }
}
