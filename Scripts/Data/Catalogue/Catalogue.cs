using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Catalogue : MonoSingleton<Catalogue>
{
    //must be same in CatalogueProgram and main program 
    private static readonly string DataFolder = $"{GlobalConsts.BUNDLE_STREAMING}";
    private static readonly string CatalogueFolder = $"{DataFolder}/CatalogueData";
    private static readonly string EntryCombinedPath = $"{CatalogueFolder}/entries/Combined";
    private static readonly string DescriptionPath = $"{CatalogueFolder}/description";
    private static readonly string ProfilesFolder = $"{CatalogueFolder}/profiles";
    private static readonly string EntriesFolder = $"{CatalogueFolder}/entries";
    private static readonly string PresetsFolder = $"{CatalogueFolder}/presets";
    private static readonly string EntriesIndexPath = $"{EntriesFolder}/index";
    private static readonly string PresetsIndexPath = $"{PresetsFolder}/index";

    public static readonly string CatalogueFiltersFolder = $"{CatalogueFolder}/filters";

    public CatalogueDescription Description => m_Description;
    public ProductInfoManager ProductInfoManagerRef;


    private Dictionary<string, CataloguePreset> m_PresetsById = new Dictionary<string, CataloguePreset>();
    private Dictionary<string, CatalogueEntry> m_EntriesById = new Dictionary<string, CatalogueEntry>();
    private List<CataloguePreset> m_CataloguePresets = new List<CataloguePreset>();
    private List<CatalogueEntry> m_CatalogueEntries = new List<CatalogueEntry>();
    private CatalogueDescription m_Description;

    private HashSet<string> m_AllGuids = new HashSet<string>();

    public List<CatalogueEntry> GetCatalogue => m_CatalogueEntries.Clone();

    public string m_sSingleProductCode;

    private void Start()
    {
        ProductInfoManagerRef = new ProductInfoManager();
        // load catalogue description
#if UNITY_ANDROID || UNITY_IOS
        Debug.Log($"DescriptionPath {DescriptionPath}");
        Json.AndroidNet.ReadFromFileAsync<CatalogueDescription>(this, showError:true, DescriptionPath, JsonLibraryType.JsonNet, (description) =>
        {
            m_Description = description;
            if (m_Description == null)
            {
                Core.Network.LoadAssetJsonFile<CatalogueDescription>(DescriptionPath, false, (descriptionNetwork) =>
                {
                    m_Description = descriptionNetwork;
                });
            }
        });
#else
        m_Description = Json.JsonNet.ReadFromFile<CatalogueDescription>(DescriptionPath);

        if (m_Description == null)
        {
            Core.Network.LoadAssetJsonFile<CatalogueDescription>(DescriptionPath, false, (description) =>
            {
                m_Description = description;
            });
        }
#endif

    }

    public void LoadFromFile(string sEnvironment, Action callback, Action<float> onProgress)
    {
        SceneDataManager.Instance.LoadDataForEnvironment(() =>
        {
            Json.SetLibrary(JsonLibraryType.JsonNet);
            //Load these catalogue items
            if (SceneDataManager.Instance.LoadedCatalogueIndexes == null)
            {
                Debug.LogError($"SceneDataManager.Instance.LoadedCatalogueIndexes == null,    so not getting anything from cat side");
                OnCatalogueLoaded(this, callback);
            }
            else
            {

                //Single items   PC 6.35 seconds
                //Combined items PC 7.11 seconds


                //Single items   Quest 14.694 seconds
                //Combined items Quest 11.949 seconds



                //In build on PC unity version 2020.3.5
                //Single 4.07 seconds
                //Combined 3.33 seconds

                //In Editor on PC
                //Single 14.07 seconds
                //Combined 31.0 seconds

                // problems with new server stuff
                //////#if UNITY_ANDROID
                //////                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                //////                LoadFromIndexOnePath<List<CatalogueEntry>>(EntryCombinedPath, (fullData) =>
                //////                {
                //////                    m_CatalogueEntries = new List<CatalogueEntry>();
                //////                    foreach (var item in SceneDataManager.Instance.LoadedCatalogueIndexes)
                //////                    {
                //////                        var newItem = fullData.FindLast(e => e.Guid == item);
                //////                        m_CatalogueEntries.Add(newItem);
                //////                    }
                //////                    OnCatalogueLoaded(this, callback);
                //////                    Json.ResetLibrary();
                //////#if DEVELOPMENT_BUILD || UNITY_EDITOR
                //////                    DebugUIQuest.Instance.LogMessageSpecial(DebugUIQuest.SpecialEnum.CatalogueCount, $"m_CatalogueEntries count: {m_CatalogueEntries.Count}");
                //////#endif
                //////                    timer.Stop();
                //////                    Debug.Log($"List<CatalogueEntry>  {timer.ElapsedMilliseconds / 1000f}");
                //////                });
                //////#else

                // This will only work for WEBGL right now.
                if (Core.Environment.Environments[0].SingleCatalogueItemOnly)
                {
                    // Grab the URL from the web browser to figure out the product we need to download.
                    m_sSingleProductCode = Application.absoluteURL;

                    // No URL in editor so we'll fake one here.
#if (UNITY_EDITOR)
                    m_sSingleProductCode = "https://resehmibackup.z33.web.core.windows.net/Android/index.html?P0E3H"; // Afterglow P0CAQ // Tannoy Reveal P0C48 // Go XLR Mini P0DI7
#endif

                    // If there's a reference number at the end of the URL.
                    if (true == m_sSingleProductCode.Contains("?"))
                    {
                        // Grab the reference number from the URL.
                        m_sSingleProductCode = m_sSingleProductCode.Split('?')[1];

                        Debug.Log($"Catalogue Parsed Reference Number from URL: {m_sSingleProductCode}");

                        SceneDataManager.Instance.LoadedCatalogueIndexes.RemoveAll(e => e.ReferenceNumber != m_sSingleProductCode);
                        // remove all but the one needed from SceneDataManager.Instance.LoadedCatalogueIndexes
                        if (SceneDataManager.Instance.LoadedCatalogueIndexes.Count != 1)
                        {
                            Debug.LogError("Catalogue has not successfully found product from code in URL", this);
                        }
                    }
                    else
                    {
                        Debug.LogError($"No product code found in URL", this);
                    }
                }

                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                var splitLists = SceneDataManager.Instance.LoadedCatalogueIndexes.SplitList(GlobalConsts.MAX_SEQUENTAILACTION_SIZE);
                m_CatalogueEntries = new List<CatalogueEntry>();
                TaskAction task = new TaskAction(splitLists.Count, () =>
                {
                    OnCatalogueLoaded(this, callback);
                    Json.ResetLibrary();
                    timer.Stop();
                    Debug.LogError($"Catalogue List single took {timer.Elapsed.ToReadableString()} seconds to load");
                });
                foreach (var item in splitLists)
                {
                    LoadFromIndex<CatalogueEntry>(this, JsonLibraryType.JsonNet, item, SceneDataManager.Instance.DownloadCatFromServer, GetEntryPath, p =>
                    {
                        onProgress?.Invoke(p * GlobalConsts.LOADINGBAR_PERCENTAGE);

                    }, entries =>
                    {                       
                        m_CatalogueEntries.AddRange(entries);
                        task.Increment();
                    });
                }
            }
        });
    }



    private static void LoadFromIndexOnePath<T>(string path, Action<T> callback) where T : class
    {
        if (Core.Contents.FileExist(path, "", true) == StreamingAssetsContents.ContentsEnum.InBuild)
        {
            Json.Library.ReadFromFileAsyncNoErrorMessage<T>(path, (item) =>
            {
                callback?.Invoke(item);
            });
        }
        else
        {
            Core.Network.LoadAssetJsonFile<T>(path, true, (itemOther) =>
            {
                callback?.Invoke(itemOther);
            });
        }
    }

    private static void LoadFromIndex<T>(MonoBehaviour host, JsonLibraryType jsonType, List<GlobalConsts.CatalogueData> indexes, bool fromServer, Func<string, string> getPath, Action<float> onProgress, Action<List<T>> callback) where T : class
    {
        bool shoulbBeFromServer = false;
        if (Core.Environment.Environments.Count == 1 && Core.Environment.CurrentEnvironment.IncludeCataloguePrefabs == false)
        {
            shoulbBeFromServer = true;
        }

        Debug.Log($"catalogue indexes.Count  {indexes.Count}");
        float progress = 0;
        float progressIncrement = 1f / indexes.Count;
        SequentialAction.WithResult(indexes, (guid, onTick) =>
        {
            onProgress?.Invoke(progress += progressIncrement);
            string path = getPath(guid.GUID);  
            string persistentPath = BundleLoader.CreatePersistentDataPath($"catalogue/entries/{guid.GUID}/{guid.GUID}");
            if (fromServer == false)
            {
                string localpath = path;
                if(shoulbBeFromServer == true)
                {
                    localpath = persistentPath;
                }
                Json.AndroidNet.ReadFromFileAsync<T>(host, showError: false, localpath, jsonType, (item) =>
                {
                    if (item != null)
                    {
                        onTick(item);                       
                    }
                    else
                    {
                        Core.Network.LoadAssetJsonFile<T>(path, false, (itemOther) =>
                        {
                            Json.JsonNet.WriteToFile<T>(itemOther, persistentPath, true);
                            if (itemOther == null)
                            {
                                Debug.LogError($"Cannot find {path}");
                            }
                            onTick(itemOther);
                        });
                    }
                });
            }
            else
            {
                Core.Network.LoadAssetJsonFile<T>(path, false, (itemOther) =>
                {
                    Json.JsonNet.WriteToFile<T>(itemOther, persistentPath, true);
                    if (itemOther == null)
                    {
                        Debug.LogError($"Cannot find {path}");
                    }

                    onTick(itemOther);
                });
            }

        }, callback);
    }

    private void OnCatalogueLoaded(MonoBehaviour host, Action callback)
    {
        // remove any broken entries
        m_CataloguePresets.RemoveAll(e => e == null);
        m_CatalogueEntries.RemoveAll(e => e == null);

        // Remove duplicates
        m_CataloguePresets = m_CataloguePresets.GroupBy(x => x.Guid).Where(x => x.Count() == 1).Select(x => x.First()).ToList();
        m_CatalogueEntries = m_CatalogueEntries.GroupBy(x => x.Guid).Where(x => x.Count() == 1).Select(x => x.First()).ToList();

        // cache entries by id
        m_PresetsById = m_CataloguePresets.ExtractAsValues(e => e.Guid);
        m_EntriesById = m_CatalogueEntries.ExtractAsValues(e => e.Guid);

        // cache a hash of all ids
        m_AllGuids = new HashSet<string>();
        m_CataloguePresets.ForEach(p => m_AllGuids.Add(p.Guid));
        m_CatalogueEntries.ForEach(p => m_AllGuids.Add(p.Guid));

        var splits = m_CatalogueEntries.SplitList(GlobalConsts.MAX_SEQUENTAILACTION_SIZE);
        if(splits.Count != 1)
        {
            Debug.LogError($"m_CatalogueEntries has been split up parts {splits.Count},  had weird errors if it proccessed more than 318 items, splitting it fixes this");
        }

        TaskAction task = new TaskAction(splits.Count, () =>
        {
            // populate categories
            if (m_Description != null)
            {
                m_Description.RootCategory.PopulateEntries(m_CatalogueEntries);
            }

            // populate mlf manager
            callback();
        });
        foreach (var item in splits)
        {
            SequentialAction.List(item, (entry, onTick) =>
            {
                entry.LoadMetaData(host, onTick);
            }, () =>
            {
                task.Increment();
            });
        }

    }

    public int Count() => m_AllGuids.Count;
    public bool Exists(string catalogueId) => m_AllGuids.Contains(catalogueId);
    public CataloguePreset GetPreset(string id) => m_PresetsById.Get(id);
    public CatalogueEntry GetEntry(string id) => m_EntriesById.Get(id);

    public static List<string> LoadEntryIndex() => LoadIndex(EntriesIndexPath);
    private static List<string> LoadIndex(string path) => Json.JsonNet.ReadFromFile<List<string>>(path);

    public static string GetExternalMetaDataPath(string guid, ExternalMetaDataType type)
    {
        return GetEntryPath(guid, type.ToString().ToLower());
    }

    public static string GetProfilePath<T>(string name) => $"{ProfilesFolder}/{typeof(T).Name.ToLowerUnderscored()}/{name}";
    public static string GetEntryPath(string guid, string suffix) => $"{GetEntryPath(guid)}_{suffix}";
    public static string GetEntryPath(string guid) => $"{EntriesFolder}/{guid}/{guid}";
    public static string GetPresetPath(string guid) => $"{PresetsFolder}/{guid}/{guid}";

    public void GetLocalAssetVersion(CatalogueEntry entry, Action<int> callBack)
    {
        Core.Assets.GetLocalVersionNumber(entry, callBack);        
    }
}
