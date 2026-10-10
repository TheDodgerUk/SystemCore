using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental;
using System;
using System.IO;
using static GlobalConsts;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneDataManager : MonoSingleton<SceneDataManager>
{
    private static string DataRoot => Core.Environment.CurrentEnvironment == null ? "" : $"{GlobalConsts.LocalBundleStreamingAssetsDirectoryData}/";
    private static string DataRootNonOverload => Core.Environment.CurrentEnvironment == null ? "" : $"{GlobalConsts.LocalBundleStreamingAssetsDirectoryDataNonOverload}/";

    private const string CatalogueData = "CatalogueData";
    private const string ModuleData = "ModuleData";
    private const string AttributeData = "AttributeData";
    private const string LightingData = "LightingData";
    private const string CatalogueReplaceInScene = "CatalogueReplaceInScene";
    private const string VFXReplaceInScene = "VFXReplaceInScene";

    public List<GlobalConsts.CatalogueData> LoadedCatalogueIndexes => m_LoadedCatalogueIndexes;
    [SerializeField]
    private List<GlobalConsts.CatalogueData> m_LoadedCatalogueIndexes = new List<GlobalConsts.CatalogueData>();


    public List<CatalogueReplaceInScene> CatalogueReplaceInSceneLoaded => m_CatalogueReplaceInSceneLoaded;
    [SerializeField]
    private List<CatalogueReplaceInScene> m_CatalogueReplaceInSceneLoaded = new List<CatalogueReplaceInScene>();

    public List<VFXReplaceInScene> VFXReplaceInSceneLoaded => m_VFXReplaceInSceneLoaded;
    [SerializeField]
    private List<VFXReplaceInScene> m_VFXReplaceInSceneLoaded = new List<VFXReplaceInScene>();

    public List<string> AttributesLoaded => m_AttributesLoaded;
    [SerializeField]
    private List<string> m_AttributesLoaded = new List<string>();

    private const string ERROR_MESSAGE = "SDM: ";
    private string m_ErrorMessage = ERROR_MESSAGE;

    private const int NUMBER_OF_TASKS = 4;

    public bool DownloadCatFromServer { get; private set; }


    public void LoadDataForEnvironment(Action callback)
    {
        TaskAction task = new TaskAction(NUMBER_OF_TASKS, () =>
        {
            callback?.Invoke();

            if (m_ErrorMessage.Length == ERROR_MESSAGE.Length)
            {
                m_ErrorMessage += "Passed";
            }
        });

        // NUMBER_OF_TASKS the following items
        LoadCatalogueData(task);
        LoadAttributesData(task);
        LoadCatalogueReplaceInScene(task);
        LoadVFXReplaceInScene(task);

        //Load any video Data
        VideoLoader.Instance?.LoadVideoData();
    }

    private void LoadCatalogueData(TaskAction task)
    {
        DownloadCatFromServer = false;
        m_LoadedCatalogueIndexes.Clear();
        string sFolder = $"{DataRoot}";

        string file = sFolder + CatalogueData;
        string fileVersion = file+"_Version";
        ConsoleExtra.Log($"LoadCatalogueData {file}", null, ConsoleExtraEnum.EDebugType.StartUp);
        string saveFile = BundleLoader.CreatePersistentDataPath(CatalogueData);

        if ((Core.Environment.Environments.Count == 1) && (Core.Environment.CurrentEnvironment.IncludeCataloguePrefabs == false && Core.Environment.CurrentEnvironment.CatalogueListFromServer == true))
        {
            Core.Network.LoadAssetJsonFile<GlobalConsts.AssetBundleVersion>(fileVersion, false, (networkData) =>
            {
                int networkVersion = networkData.Version;

                var localPathVersion = BundleLoader.CreatePersistentDataPath(CatalogueData + "_Version");
                Json.AndroidNet.ReadFromFileAsync<GlobalConsts.AssetBundleVersion>(this, showError: false, localPathVersion, JsonLibraryType.JsonNet, (localVersionData) =>
                {
                    Json.JsonNet.WriteToFile<GlobalConsts.AssetBundleVersion>(networkData, localPathVersion, true);
                    if (localVersionData == null || localVersionData.Version != networkVersion)
                    {
                        DownloadCatData(file, saveFile, task);
                    }
                    else
                    {
                        Json.AndroidNet.ReadFromFileAsync<List<GlobalConsts.CatalogueData>>(this, showError: false, saveFile, JsonLibraryType.JsonNet, (localData) =>
                        {
                            m_LoadedCatalogueIndexes = localData;
                            if(m_LoadedCatalogueIndexes != null)
                            {
                                task.Increment();
                            }
                            else
                            {
                                DownloadCatData(file, saveFile, task);
                            }
                            
                        });
                    }
                });

            });
        }
        else
        {
            Json.AndroidNet.ReadFromFileAsync<List<GlobalConsts.CatalogueData>>(this, showError: true, file, JsonLibraryType.JsonNet, (data) =>
            {
                m_LoadedCatalogueIndexes = data;
                if(m_LoadedCatalogueIndexes == null)
                {
                    Debug.LogError($"local m_LoadedCatalogueIndexes is null {file}");
                }
                task.Increment();
            });
        }
    }


    private void DownloadCatData(string file, string saveFile, TaskAction task)
    {
        Core.Network.LoadAssetJsonFile<List<GlobalConsts.CatalogueData>>(file, false, (dataNetwork) =>
        {
            m_LoadedCatalogueIndexes = dataNetwork;
            if (m_LoadedCatalogueIndexes == null)
            {
                Core.GenericMessageRef.DisplayError($"Cannot find {file} on network", 2, null);
            }
            Json.JsonNet.WriteToFile<List<GlobalConsts.CatalogueData>>(dataNetwork, saveFile, true);
            DownloadCatFromServer = true;
            task.Increment();
        });
    }

    private void LoadAttributesData(TaskAction task)
    {
        string sFolder = $"{DataRoot}";
        string file = sFolder + AttributeData;
        ConsoleExtra.Log($"LoadAttributesData {file}", null, ConsoleExtraEnum.EDebugType.StartUp);

        Json.AndroidNet.ReadFromFileAsync<List<string>>(this, showError: false, file, JsonLibraryType.JsonNet, (data) =>
        {
            m_AttributesLoaded = data;
            if (m_AttributesLoaded == null)
            {
                Core.Network.LoadAssetJsonFile<List<string>>(file, false, (dataNetwork) =>
                {
                    m_AttributesLoaded = dataNetwork;
                    if (m_AttributesLoaded == null)
                    {
                        Debug.LogError("m_AttributesLoaded is null , check networking , neeeeeeed a big generic message for screen here");
                    }
                    task.Increment();
                });
            }
            else
            {
                task.Increment();
            }
        });

    }

    private void LoadCatalogueReplaceInScene(TaskAction task)
    {
        string sFolder = $"{DataRootNonOverload}";
        string file = sFolder + CatalogueReplaceInScene;
        ConsoleExtra.Log($"LoadCatalogueReplaceInScene {file}", null, ConsoleExtraEnum.EDebugType.StartUp);

        Json.AndroidNet.ReadFromFileAsync<List<CatalogueReplaceInScene>>(this, showError: false, file, JsonLibraryType.JsonNet, (data) =>
        {
            m_CatalogueReplaceInSceneLoaded = data;
            if (m_CatalogueReplaceInSceneLoaded == null)
            {
                Core.Network.LoadAssetJsonFile<List<CatalogueReplaceInScene>>(file, false, (dataNetwork) =>
                {
                    m_CatalogueReplaceInSceneLoaded = dataNetwork;
                    if (m_CatalogueReplaceInSceneLoaded == null)
                    {
                        Debug.LogError("m_CatalogueReplaceInSceneLoaded is null , check networking , neeeeeeed a big generic message for screen here");
                        m_CatalogueReplaceInSceneLoaded = new List<CatalogueReplaceInScene>();
                    }
                    task.Increment();
                });
            }
            else
            {
                task.Increment();
            }
        });

    }


    private void LoadVFXReplaceInScene(TaskAction task)
    {
        string sFolder = $"{DataRootNonOverload}";
        string file = sFolder + VFXReplaceInScene;
        ConsoleExtra.Log($"VFXReplaceInScene {file}", null, ConsoleExtraEnum.EDebugType.StartUp);

        Json.AndroidNet.ReadFromFileAsync<List<VFXReplaceInScene>>(this, showError: false, file, JsonLibraryType.JsonNet, (data) =>
        {
            m_VFXReplaceInSceneLoaded = data;
            if (m_CatalogueReplaceInSceneLoaded == null)
            {
                Core.Network.LoadAssetJsonFile<List<VFXReplaceInScene>>(file, false, (dataNetwork) =>
                {
                    m_VFXReplaceInSceneLoaded = dataNetwork;
                    if (m_CatalogueReplaceInSceneLoaded == null)
                    {
                        Debug.LogError("VFXReplaceInScene is null , check networking , neeeeeeed a big generic message for screen here");
                        m_VFXReplaceInSceneLoaded = new List<VFXReplaceInScene>();
                    }
                    task.Increment();
                });
            }
            else
            {
                task.Increment();
            }
        });

    }
    
}