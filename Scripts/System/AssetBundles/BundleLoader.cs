using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class BundleLoader
{
    public class DependencyData
    {
        public string PrefabName;
        public string BundleName;
        public string FullName;
        public string GUID;
        public int Version;
        public float SizeMB;
    }

    private static string DependencyPath;// = Device.StreamingPath + $"/bundles/{GetPlatformBundleFolder(Application.platform)}/master";
    public static string BundlePath; //= Device.StreamingPath + $"/bundles/{GetPlatformBundleFolder(Application.platform)}/{0}";
    public static string BundlePathShort;
    public const string BundleExtension = ".unity3d";
    public const string BLANK_HASH_CODE = "NONE";


    private Dictionary<string, List<Action<AssetBundle>>> m_PendingCallbacks;
    private Dictionary<string, List<DependencyData>> m_BundleDependencies;
    private Dictionary<string, AssetBundleRef> m_LoadedBundles;

    private Dictionary<string, List<Action<AssetBundle>>> m_GenericAssetPendingCallbacks;

    private const string ANDROID_START = "jar:file:";

    public BundleLoader()
    {
        DependencyPath = Device.StreamingPath + $"/bundles/{GlobalConsts.CurrentPlatformName()}/master";
        BundlePath = Device.StreamingPath + $"/bundles/{GlobalConsts.CurrentPlatformName()}/";
        BundlePathShort = $"/bundles/{GlobalConsts.CurrentPlatformName()}/";
    }

    public static string CreateVersionFileName(string fullPath)
    {
        string noExtention = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));
        noExtention += "_Version";
        return noExtention.SanitiseSlashes();
    }


    public void LoadAssetDatabase(MonoBehaviour mono, Action callback)
    {
        m_PendingCallbacks = new Dictionary<string, List<Action<AssetBundle>>>();
        m_LoadedBundles = new Dictionary<string, AssetBundleRef>();

        m_GenericAssetPendingCallbacks = new Dictionary<string, List<Action<AssetBundle>>>();


        Json.AndroidNet.ReadFromFileAsync<Dictionary<string, List<DependencyData>>>(mono, showError: false, DependencyPath, JsonLibraryType.JsonNet, (dependencies) =>
        {
            if (dependencies == null)
            {
                Core.Network.LoadAssetJsonFile<Dictionary<string, List<DependencyData>>>(DependencyPath, true, (dependenciesNetwork) =>
                {
                    m_BundleDependencies = dependenciesNetwork;
                    callback();
                });
            }
            else
            {
                m_BundleDependencies = dependencies;
                callback();
            }

        });

    }

    public void Unload(string path, int version, bool allObjects)
    {
        string key = GetBundleKey(path);
        if (m_LoadedBundles.ContainsKey(key) == true)
        {
            if (m_LoadedBundles[key].Bundle != null)
            {
                m_LoadedBundles[key].Bundle.Unload(allObjects);
                m_LoadedBundles.Remove(key);
            }
        }
    }

    public bool Contains(string path, int version)
    {
        string key = GetBundleKey(path);
        return (m_LoadedBundles.ContainsKey(key) == true);
    }


    public void UnloadAll()
    {
        foreach (var kvp in m_LoadedBundles)
        {
            kvp.Value.Bundle.Unload(true);
        }

        m_LoadedBundles.Clear();
    }

    public void GetBundle(string bundleName, Action<AssetBundle> callback, MonoBehaviour host, int version)
    {
        RetrieveBundle(AppendExtension(bundleName), callback, host, version);
    }

    private void RetrieveBundle(string bundleName, Action<AssetBundle> callback, MonoBehaviour host, int version)
    {
        // check if bundle is already loaded
        string key = GetBundleKey(bundleName);
        if (m_LoadedBundles.ContainsKey(key) == false)
        {
            LoadBundle(bundleName, version, callback, host);
        }
        else
        {
            callback(m_LoadedBundles[key].Bundle);
        }
    }

    public bool IsBundleLoaded(string bundle)
    {
        var key = AppendExtension(bundle);
        bool loaded =  m_LoadedBundles.ContainsKey(key) && (m_LoadedBundles[key] != null) && (m_LoadedBundles[key].Bundle != null);
        return loaded;
    }



    private void LoadBundle(string bundleName, int version, Action<AssetBundle> callback, MonoBehaviour host)
    {
#if UNITY_EDITOR
        if (UnityEngine.Windows.Directory.Exists(BundlePath) == false)
        {
            UnityEditor.EditorUtility.DisplayDialog("Error in LoadBundle.cs", $"Cannot find path :{BundlePath}, check that the correct platform for asset bundles have been copied over", "OK");
        }
#endif

        if (GlobalConsts.IsGenericAsset(bundleName) == false)
        {
            // register callback, continue if unique
            if (AddCallback(bundleName, callback) == false)
            {
                return;
            }
        }
        else
        {
            // register callback, continue if unique
            if (AddAssetCallback(bundleName, callback) == false)
            {
                return;
            }
        }


        // grab dependencies that need loading
        List<DependencyData> dependencies = GetDependencies(bundleName);
        TaskAction task = new TaskAction(dependencies.Count + 1, () =>
        {
            InvokeCallback(bundleName, version);
        });

        for (int i = 0; i < dependencies.Count; ++i)
        {
            // load each one 
            RetrieveBundle(dependencies[i].BundleName, bundle => task.Increment(), host, version);
        }


        // load the bundle
        string path = $"{BundlePath}{bundleName}";
        var stored = Core.Contents.FileExist(path, "private void LoadBundle", false);
        if(stored == StreamingAssetsContents.ContentsEnum.None)
        {
            path = CreatePersistentDataPath(bundleName);
            stored = Core.Contents.FileExist(path, "private void LoadBundle", false);           
        }
        switch (stored)
        {
            case StreamingAssetsContents.ContentsEnum.InBuild:
            case StreamingAssetsContents.ContentsEnum.ServerAndStored:
                if (stored == StreamingAssetsContents.ContentsEnum.ServerAndStored)
                {
                    path = CreatePersistentDataPath(bundleName);
                }

#if PLATFORM_WEBGL
                UnityWebRequest bundleWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(path);

                host.WaitForOp(bundleWebRequest.SendWebRequest(), request =>
                {
                    if (bundleWebRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Failed to download asset bundle: {path} Error: {bundleWebRequest.error}");
                    }
                    else
                    {
                        AddToLoadedBundle(DownloadHandlerAssetBundle.GetContent(bundleWebRequest), bundleName, version);
                        if (GlobalConsts.IsGenericAsset(bundleName) == false)
                        {
                            task.Increment();
                        }
                        else
                        {
                            InvokeGenericAssetCallback(bundleName, version);
                        }
                    }
                });
#else
                host.WaitForOp(AssetBundle.LoadFromFileAsync(path), request =>
                {
                    if (request.assetBundle != null)
                    {
                        AddToLoadedBundle(request.assetBundle, bundleName, version);
                        if (GlobalConsts.IsGenericAsset(bundleName) == false)
                        {
                            task.Increment();
                        }
                        else
                        {
                            InvokeGenericAssetCallback(bundleName, version);
                        }
                    }
                });
#endif
                break;
            case StreamingAssetsContents.ContentsEnum.None:
                DownloadSingleAssetCatalogueFromServerAndSaveLocally(bundleName, version, null, () =>
                {
                    if (GlobalConsts.IsGenericAsset(bundleName) == false)
                    {
                        task.Increment();
                    }
                    else
                    {
                        InvokeGenericAssetCallback(bundleName, version);
                    }
                });
                break;
        }
        
    }


    public static string CreatePersistentDataPath(string bundle)
    {
        bundle = AssetLoader.RendererFullPath(bundle);
        bundle = bundle.ToLower();
        bundle = bundle.SanitiseSlashes();
        if (bundle.StartsWith(Application.streamingAssetsPath.ToLower()) == true)
        {
            bundle = bundle.Substring(Application.streamingAssetsPath.Length);
            bundle = bundle.Remove(0, 1);// remove the /
        }

        string persistentDataPath = $"{Application.persistentDataPath}/{bundle}";

        return persistentDataPath;
    }


    public static string RemovePersistentDataPath(string bundle)
    {
        bundle = CreatePersistentDataPath(bundle);
        bundle = bundle.ToLower();
        if (bundle.StartsWith(Application.persistentDataPath.ToLower()) == true)
        {
            bundle = bundle.Substring(Application.persistentDataPath.Length);
            bundle = bundle.Remove(0, 1); // remove the /
        }

        string stringToRemove = "streamingassets/";
        if (bundle.StartsWith(stringToRemove) == true)
        {
            bundle = bundle.Substring(stringToRemove.Length);
        }

        //Debug.LogError($"fullPath : {fullPath} ");
        return bundle;
    }


    public void DownloadSingleAssetCatalogueFromServerAndSaveLocally(string bundleName, int version, Action<float> progress, Action callback)
    {
        CatalogueItemFromServerAndSaveLocally(bundleName, version, progress, callback);
    }

    private void CatalogueItemFromServerAndSaveLocally(string bundleName, int version, Action<float> progress, Action callback)
    {
        string serverPath = RemovePersistentDataPath(bundleName);
 
        Unload(bundleName, 0, true); // need to remove it first, other wise cant add to bundle

        Core.Network.GetAssetBundle(serverPath, (progressCallback) => progress?.Invoke(progressCallback), (data) =>
        {
            if ((data != null) && (data.m_AssetBundle != null))
            {
                AddToLoadedBundle(data.m_AssetBundle, bundleName, version);
#if !PLATFORM_WEBGL // netwoking.cs, WEBGL cannot save the raw data, so nothing to save
                SaveCatalogueItemLocally(data.m_Data, bundleName, version);
#endif
            }
            else
            {
                Debug.LogError("Problem getting GetAssetBundle");
            }
            callback?.Invoke();
        });
    }


    private void AddToLoadedBundle(AssetBundle assetBundle, string bundleName, int version)
    {
        var bundleRef = new AssetBundleRef(assetBundle, bundleName, version);
        string key = GetBundleKey(bundleName);
        if (assetBundle != null)
        {
            if (m_LoadedBundles.ContainsKey(key) == false)
            {
                m_LoadedBundles.Add(key, bundleRef);
            }
            else
            {
                Debug.LogError($"key {key} already exists");
            }
        }
        else
        {
            Debug.LogError($"key {key} assetBundle == null");
        }
    }

    private void LoadFromFile(string bundleName, int version, Action callback)
    {
        if (IsItemInCatalogue(bundleName) == false)
        {
            string path = $"{BundlePath}{bundleName}";
            Core.Mono.WaitForOp(AssetBundle.LoadFromFileAsync(path), request =>
            {
                if (request != null && request.assetBundle != null)
                {
                    AddToLoadedBundle(request.assetBundle, bundleName, version);
                    callback?.Invoke();
                }
                else
                {
                    string persistentDataPath = CreatePersistentDataPath(bundleName);
                    Core.Mono.WaitForOp(AssetBundle.LoadFromFileAsync(persistentDataPath), request2 =>
                    {
                        AddToLoadedBundle(request2.assetBundle, bundleName, version);
                        callback?.Invoke();
                    });
                }
            });
        }
        else
        {
            string persistentDataPath = CreatePersistentDataPath(bundleName);
            Core.Mono.WaitForOp(AssetBundle.LoadFromFileAsync(persistentDataPath), request =>
            {
                AddToLoadedBundle(request.assetBundle, bundleName, version);
                callback?.Invoke();
            });
        }
    }


    void SaveCatalogueItemLocally(byte[] data, string bundleName, int version)
    {
        string persistentDataPath = CreatePersistentDataPath(bundleName);
        //Create the Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(persistentDataPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(persistentDataPath));
        }

        // write file like behringer_deepmind12.unity3d245
        try
        {
            File.WriteAllBytes(persistentDataPath, data);
            Debug.Log("Saved Data to: " + persistentDataPath.SanitiseSlashes());

            string versionFile = CreateVersionFileName(persistentDataPath);
            Json.FullSerialiser.WriteToFile<string>(version.ToString(), versionFile, true);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed To Save Data to: " + persistentDataPath.SanitiseSlashes());
            Debug.LogError("Error: " + e.Message);
        }
    }


    private bool AddCallback(string bundleName, Action<AssetBundle> callback)
    {
        if (m_PendingCallbacks.ContainsKey(bundleName) == false)
        {
            m_PendingCallbacks.Add(bundleName, new[] { callback }.ToList());
            return true;
        }
        else
        {
            m_PendingCallbacks[bundleName].Add(callback);
            return false;
        }
    }

    private bool AddAssetCallback(string bundleName, Action<AssetBundle> callback)
    {
        if (m_GenericAssetPendingCallbacks.ContainsKey(bundleName) == false)
        {
            m_GenericAssetPendingCallbacks.Add(bundleName, new[] { callback }.ToList());
            return true;
        }
        else
        {
            m_GenericAssetPendingCallbacks[bundleName].Add(callback);
            return false;
        }
    }

    private void InvokeCallback(string bundleName, int version)
    {
        if (m_PendingCallbacks.ContainsKey(bundleName) == true)
        {
            string key = GetBundleKey(bundleName);
            if (m_LoadedBundles.ContainsKey(key) == true)
            {
                var bundle = m_LoadedBundles[key].Bundle;
                m_PendingCallbacks[bundleName].ForEach(c => c(bundle));
                m_PendingCallbacks.Remove(bundleName);

                string allKeys = $"{System.Environment.NewLine}";
                foreach (var currentKey in m_PendingCallbacks.Keys)
                {
                    allKeys += $"   {currentKey} {System.Environment.NewLine}";
                }
                ConsoleExtra.Log($"m_PendingCallbacks count: {m_PendingCallbacks.Count},   bundleName: {bundleName},  key: {key},  allKeys:{allKeys}", null, ConsoleExtraEnum.EDebugType.LoadScene);
            }
            else
            {
                Debug.LogError($"private void InvokeCallback could not find key:{key} , from bundleName: {bundleName}");
                m_PendingCallbacks[bundleName].ForEach(c => c(null));
                m_PendingCallbacks.Remove(bundleName);
            }
        }
        else
        {
            Debug.LogError($"InvokeCallback , {bundleName}");
        }
    }

    private void InvokeGenericAssetCallback(string bundleName, int version)
    {
        if (m_GenericAssetPendingCallbacks.ContainsKey(bundleName) == true)
        {
            if (m_GenericAssetPendingCallbacks[bundleName].Count > 0)
            {
                string key = GetBundleKey(bundleName);
                if (m_LoadedBundles.ContainsKey(key) == true)
                {
                    var bundle = m_LoadedBundles[key].Bundle;
                    m_GenericAssetPendingCallbacks[bundleName].ForEach(c => c(bundle));
                    m_GenericAssetPendingCallbacks.Remove(bundleName);
                }
                else
                {
                    Debug.LogError($"m_LoadedBundles {bundleName}, key not found {key}");
                    m_GenericAssetPendingCallbacks[bundleName].ForEach(c => c(null));
                    m_GenericAssetPendingCallbacks.Remove(bundleName);
                }
            }
        }
        else
        {
            Debug.LogError($"InvokeGenericAssetCallback , {bundleName}");
        }
    }


    public List<DependencyData> GetDependencies(string bundleName)
    {
        return m_BundleDependencies.Get(bundleName) ?? new List<DependencyData>();
    }

    private static string GetBundleKey(string path) => string.Concat(path);

    private static string AppendExtension(string bundleName)
    {
        if (bundleName.EndsWith(BundleExtension) == false)
        {
            return $"{bundleName}{BundleExtension}";
        }
        return bundleName;
    }

    private class AssetBundleRef
    {
        public AssetBundle Bundle { get; private set; }
        public int Version { get; private set; }
        public string Url { get; private set; }

        public AssetBundleRef(AssetBundle bundle, string url, int version)
        {
            Bundle = bundle;
            Version = version;
            Url = url;
        }
    };

    private bool IsItemInCatalogue(string bundleName)
    {
        var cat = Core.Catalogue.GetCatalogue;
        List<RenderMetaData> collected = new List<RenderMetaData>();
        foreach (var item in cat)
        {
            var renderMetaData = item.GetMetaData<RenderMetaData>();
            collected.Add(renderMetaData);
        }
        var found = collected.Find(e => e.Main.Bundle == bundleName);
        return (found != null);
    }
}
