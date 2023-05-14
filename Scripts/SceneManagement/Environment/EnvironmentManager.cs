using EnvironmentHelpers;
#if HouseBuilder
using HouseBuilder;
#endif
#if VR_INTERACTION
using MonitorTrainer;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using static GlobalConsts;

public class EnvironmentManager /*: MonoBehaviour*/
{
    public enum ChunkType
    {
        Original,
        Optional,
    }

    public event Action OnPreLoadEnvironment;

    public event Action OnEnvironmentUnloaded;

    public event Action OnEnvironmentChanged;

    public event Action OnEnvironmentLoadingComplete;           // Once all the CurrentEnvironment scenes have been loaded

    public event Action OnEnvironmentOptionalLoadingComplete;

    public event Action OnEnvironmentOptionalLoadingCleaned;

    public event Action OnEnvironmentPreEffectsLoaded;

    private static string RootFolder => $"{GlobalConsts.BUNDLE_STREAMING_ENVIRONMENT_DATA}/";
    private static string IndexPath => RootFolder + "index";

#if VR_INTERACTION
    private const string LoadingSceneName = "LoadingVR";
#else
#if AR_INTERACTION || PLATFORM_WEBGL
    private const string LoadingSceneName = "LoadingMobile";
#else
    private const string LoadingSceneName = "LoadingNonVR";
#endif

#endif

    public bool HasOnEnvironmentLoadingComplete { get; private set; } = false;

    public List<EnvironmentData> Environments => m_Environments.Clone();
    public EnvironmentData CurrentEnvironment { get; private set; }
    public LoadingScreenBase LoadingScreen { get; private set; }

    private Dictionary<string, List<GameObject>> m_ObjectsByTag;
    private Dictionary<string, GameObject> m_TaggedObjects;
    private List<EnvironmentEffect> m_EnvironmentEffects;

    private SimpleDictionaryDictionary<Scene, string, List<GameObject>> m_ObjectsByTagOptional;
    private SimpleDictionaryDictionary<Scene, string, GameObject> m_TaggedObjectsOptional;
    private SimpleDictionaryList<Scene, EnvironmentEffect> m_EnvironmentEffectsOptional;

    private List<EnvironmentData> m_Environments;

    private List<Scene> m_CurrentLoadedScenes = new List<Scene>();
    private List<Scene> m_CurrentLoadedOptionalScenes = new List<Scene>();

    public static string LoadedArenaName = string.Empty;
    public static string LoadedVariantName = string.Empty;
    public static string LoadedEnvironmentName = string.Empty;

    public void RemoveEnvironmentEffectSelf(EnvironmentEffect self) => m_EnvironmentEffects.Remove(self);

    public bool IsCurrentEnvironmentValid => !(CurrentEnvironment == null || CurrentEnvironment.VariantName == null);

    public EnvironmentManager()
    {
        m_ObjectsByTag = new Dictionary<string, List<GameObject>>();
        m_TaggedObjects = new Dictionary<string, GameObject>();
        m_EnvironmentEffects = new List<EnvironmentEffect>();

        m_ObjectsByTagOptional = new SimpleDictionaryDictionary<Scene, string, List<GameObject>>();
        m_TaggedObjectsOptional = new SimpleDictionaryDictionary<Scene, string, GameObject>();
        m_EnvironmentEffectsOptional = new SimpleDictionaryList<Scene, EnvironmentEffect>();
    }

    public List<Scene> GetLoadedScenes()
    {
        return m_CurrentLoadedScenes;
    }

    public GameObject FindGameObjectInLoadedScenes(string name)
    {
        List<Scene> loadedScenes = GetLoadedScenes();

        for (int i = 0; i < loadedScenes.Count; i++)
        {
            GameObject[] rootObjs = loadedScenes[i].GetRootGameObjects();

            for (int x = 0; x < rootObjs.Length; x++)
            {
                if (rootObjs[x].name == name)
                {
                    return rootObjs[x];
                }
            }
        }

        return null;
    }

    public List<T> GetFx<T>() where T : EnvironmentEffect => m_EnvironmentEffects.OfType<T>().ToList();

    public List<GameObject> GetTaggedObjectByTag(string name) => m_ObjectsByTag.Get(name);

    public GameObject GetTaggedObjectByName(string name) => m_TaggedObjects.Get(name);

    public void LoadEnvironmentData(Action callback, Action<float> onProgress)
    {
        // load index file
        ConsoleExtra.Log($"Environment Json now loading file:  {IndexPath}", null, ConsoleExtraEnum.EDebugType.StartUp);

        Json.AndroidNet.ReadFromFileAsync<string[]>(Core.Mono, showError: false, IndexPath, JsonLibraryType.JsonNet, (indexFile) =>
        {
            // create empty list to fill with environment data (in order)
            m_Environments = Utils.Code.EmptyList<EnvironmentData>(indexFile.Length);

            TaskAction task = new TaskAction(indexFile.Length, () =>
            {
                callback.Invoke();
            });
            float progressIncrement = 1f / indexFile.Length;
            float progress = 0;

            for (int i = 0; i < indexFile.Length; ++i)
            {
                int index = i;
                onProgress?.Invoke(progress += progressIncrement);

                // load environment data file and add to list
                string entryPath = RootFolder + indexFile[index];
                Json.AndroidNet.ReadFromFileAsync<EnvironmentData>(Core.Mono, showError: false, entryPath, JsonLibraryType.JsonNet, (data) =>
                {
                    m_Environments[index] = data;
                    task.Increment();
                });
            }
        });
    }

    public void LoadConstructScene(Action callback)
    {
        // are we already in the loading scene?
        if (GetScene(LoadingSceneName).IsValid() == false)
        {
            // blink on
#if VR_INTERACTION
            CameraControllerVR.Instance.MainCamera.SetActive(true);
            // load loading scene
            LoadScene(LoadingSceneName, scene =>
            {
                // grab the loading screen
                var roots = scene.GetRootGameObjects().ToList();
                LoadingScreen = roots.FindComponentInChildren<LoadingScreenBase>();
                Core.Network.SetLoadingScreen(LoadingScreen);
                // unblink and only view loading objects
                SceneManager.SetActiveScene(scene);
                CameraControllerVR.Instance.SetCameraLoadingSetup(true);
                callback?.Invoke();
            });
#else
            LoadScene(LoadingSceneName, scene =>
            {
                // grab the loading screen
                var roots = scene.GetRootGameObjects().ToList();
                LoadingScreen = roots.FindComponentInChildren<LoadingScreenBase>();

                // unblink and only view loading objects
                SceneManager.SetActiveScene(scene);
                callback?.Invoke();
            });
#endif
        }
        else
        {
            callback?.Invoke();
        }
    }

    public void LoadEnvironment(EnvironmentData data, Action callback, Action<float> onProgress, Action onLoadEnviroment)
    {
#if VR_INTERACTION
        Core.Keyboard.DestroyKeyboard();
#endif

        Core.Environment.LoadingScreen.InitialiseLoadingScene(data.EnvironmentName + "_" + data.VariantName, true);
        UnloadAllLoadedScenes(() =>
        {
            LoadedArenaName = data.OverloadEnvironmentName();//.Replace(" ", string.Empty); 
            LoadedVariantName = data.OverloadVariantName();//.Replace(" ", string.Empty);
            LoadedEnvironmentName = $"{LoadedArenaName}/{LoadedVariantName}";
            Debug.Log($"Loading environment: {LoadedArenaName} - {data.VariantName}, naming {LoadedEnvironmentName}\n");
            Core.Environment.OnEnvironmentUnloaded?.Invoke();
            CurrentEnvironment = data;
            OnEnvironmentChanged?.Invoke();
            //Load catalogue data for this environment
            Core.Catalogue.LoadFromFile(LoadedEnvironmentName, () =>
            {
                onLoadEnviroment?.Invoke();
                m_EnvironmentEffects.Clear();
                // load loading scene
                LoadConstructScene(() =>
                {
                    // load each chunk of environment
                    List<string> originalItems = GetOnlyOriginalScenes(data);
                    LoadEnvironmentChunks(data, originalItems, ChunkType.Original, (chunks) =>
                    {
                        m_CurrentLoadedScenes.AddRange(chunks);
                        OnEnvironmentLoaded(chunks, ChunkType.Original, callback);
                    }, onProgress);
                });
            }, onProgress);
        });
    }

    public void LoadEnvironmentOptionalAndRemoveOld(List<string> optionalChoices, Action callback)
    {
        UnloadAllOptionalLoadedScenes(() =>
        {
            LoadEnvironmentOptional(optionalChoices, callback);
        });
    }

    public void LoadEnvironmentOptional(List<string> optionalChoices, Action callback)
    {
        RemoveLoadedScenesFromList(ref optionalChoices);
        List<string> optionalList = GetOnlyOptionalScenes(CurrentEnvironment, optionalChoices);

        LoadEnvironmentOptional(CurrentEnvironment, optionalList, () =>
        {
            Debug.Log($"Num scenes loaded {m_CurrentLoadedScenes.Count}");
            // post process environment
            Core.Environment.OnEnvironmentOptionalLoadingComplete?.Invoke();
            Core.Mono.WaitForEndOfFrame(() =>
            {
                callback?.Invoke();
            });

        }, null);
    }

    private void RemoveLoadedScenesFromList(ref List<string> optionalList)
    {
        List<Scene> sceneList = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            sceneList.Add(SceneManager.GetSceneAt(i));
        }

        for (int i = optionalList.Count - 1; i >= 0; i--)
        {
            for (int sceneIndex = 0; sceneIndex < sceneList.Count; sceneIndex++)
            {
                if (true == sceneList[sceneIndex].name.EndsWith(optionalList[i]))
                {
                    optionalList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void UnLoadEnvironmentOptional(List<string> optionalChoices, Action callback)
    {
        //collect scenes
        List<Scene> sceneList = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var name in optionalChoices)
            {
                if (true == SceneManager.GetSceneAt(i).name.EndsWith(name))
                {
                    sceneList.Add(SceneManager.GetSceneAt(i));
                }
            }
        }

        UnloadAllOptionalLoadedScenes(sceneList, () =>
        {
            callback?.Invoke();
        });
    }

    private void LoadEnvironmentOptional(EnvironmentData data, List<string> optionalChoices, Action callback, Action<float> onProgress)
    {
        LoadEnvironmentChunks(data, optionalChoices, ChunkType.Optional, (chunks) =>
        {
            m_CurrentLoadedOptionalScenes.AddRange(chunks);
            Debug.Log($"Num scenes loaded {m_CurrentLoadedOptionalScenes.Count}");
            // post process environment
            OnEnvironmentLoaded(chunks, ChunkType.Optional, () =>
            {
                Core.Scene.SafetyCheckForAndroid(); 
                callback?.Invoke();
            });
        }, onProgress);
    }

    public void UnloadLoadingScene(Action callback)
    {
        CameraControllerVR.Instance.SetLayerMask(Layers.CameraRuntimeMask);
        // reset any loading screen changes
        LoadingScreen.OnSceneUnloading();

        Core.Scene.SafetyCheckForAndroid();
        // unload loading screen
        Core.Mono.WaitForOp(SceneManager.UnloadSceneAsync(LoadingSceneName), op =>
        {
            Core.Mono.WaitForFrame(() =>
            {
#if VR_INTERACTION
                Core.SceneLoader.SetInputSystemToVR(Core.Environment.CurrentEnvironment.InteractionType == InteractionType.Physics || Core.Environment.CurrentEnvironment.InteractionType == InteractionType.ControllerVR);
#else
                Core.SceneLoader.SetInputSystemToVR(false);
#endif
                if (Core.Environment.CurrentEnvironment.InteractionType == InteractionType.Physics)
                {
                    VRInputModule.Instance.ControlerTypeRef = GlobalConsts.ControllerType.PhysicsHand;
                }
                else
                {
                    VRInputModule.Instance.ControlerTypeRef = GlobalConsts.ControllerType.LaserPointer;
                }

                // set environment visible and unblink
                CameraControllerVR.Instance.SetLayerMask(Layers.CameraRuntimeMask);  // in case the camera changes 
#if VR_INTERACTION
                CameraControllerVR.Instance.SetCameraLoadingSetup(false);
                CameraControllerVR.Instance.ToggleBlink(false, () =>
                {                  
                    callback?.Invoke();
                    HasOnEnvironmentLoadingComplete = true;

                    VRInputModule.Instance.ControlerTypeRef = GlobalConsts.ControllerType.PhysicsHand;
                    InputManagerVR.Instance.ToggleHandRaycastGraphics(Handedness.Left, false);
                    InputManagerVR.Instance.ToggleHandRaycastGraphics(Handedness.Right, false);
                    CameraControllerVR.Instance.DistanceGrabHighLightInvalidTarget = false;
#if UNITY_ANDROID && VR_INTERACTION
                    OVRManager.SetSpaceWarp(true);
                    Debug.LogError($"SetSpaceWarp {OVRManager.GetSpaceWarp()}");
#endif
                    OnEnvironmentLoadingComplete?.Invoke();
                    Core.Mono.WaitFor(5, () =>
                    {
                        if(CameraControllerVR.Instance.IsFirstTime() == true)
                        {
                            DebugBeep.LogError("Fail safe TeleportAvatar", DebugBeep.MessageLevel.High);
                            CameraControllerVR.Instance.TeleportAvatar(Core.Mono.gameObject.scene, Vector3.zero, null);
                        }

                    });
                });

#else
                callback?.Invoke();
                Debug.LogError("OnEnvironmentLoadingComplete");
                HasOnEnvironmentLoadingComplete = true;
                OnEnvironmentLoadingComplete?.Invoke();
#endif
            });
        });
    }

    private void UnloadAllLoadedScenes(Action callback)
    {
        if (m_CurrentLoadedScenes.Count == 0)
        {
            callback?.Invoke();
            return;
        }

        TaskAction task = new TaskAction(m_CurrentLoadedScenes.Count + m_CurrentLoadedOptionalScenes.Count, () =>
        {
            Debug.LogError("UnloadAllLoadedScenes task");
            Debug.Log($"Num scenes removed {m_CurrentLoadedScenes.Count}");
            m_CurrentLoadedScenes.Clear();
            m_CurrentLoadedOptionalScenes.Clear();
            OnPreLoadEnvironment?.Invoke();
            callback?.Invoke();
        });

        Debug.LogError("UnloadAllLoadedScenes 1");
        //Unload all cached objects
        m_TaggedObjects.Clear();
        m_ObjectsByTag.Clear();
        m_EnvironmentEffects.Clear();
        m_TaggedObjectsOptional.ClearAll();
        m_ObjectsByTagOptional.ClearAll();
        m_EnvironmentEffectsOptional.ClearAll();

        foreach (var scene in m_CurrentLoadedScenes)
        {
            Debug.LogError($"UnloadAllLoadedScenes m_CurrentLoadedScenes : {scene.name}");
            Core.Mono.WaitForOp(SceneManager.UnloadSceneAsync(scene), (e) =>
            {
                task.Increment();
            });
        }

        foreach (var scene in m_CurrentLoadedOptionalScenes)
        {
            Debug.LogError($"UnloadAllLoadedScenes m_CurrentLoadedScenes : {scene.name}");
            Core.Mono.WaitForOp(SceneManager.UnloadSceneAsync(scene), (e) =>
            {
                task.Increment();
            });
        }
    }

    private void UnloadAllOptionalLoadedScenes(Action callback)
    {
        if (m_CurrentLoadedOptionalScenes.Count == 0)
        {
            Core.Environment.OnEnvironmentOptionalLoadingCleaned?.Invoke();
            callback?.Invoke();
            return;
        }

        TaskAction task = new TaskAction(m_CurrentLoadedOptionalScenes.Count, () =>
        {
            m_CurrentLoadedOptionalScenes.Clear();
            Core.Environment.OnEnvironmentOptionalLoadingCleaned?.Invoke();
            callback?.Invoke();
        });

        foreach (var sceneKey in m_TaggedObjectsOptional.MainKeys)
        {
            foreach (var key in m_TaggedObjectsOptional.GetValue(sceneKey).Keys)
            {
                m_TaggedObjects.Remove(key);
            }
        }

        foreach (var sceneKey in m_ObjectsByTagOptional.MainKeys)
        {
            foreach (var key in m_ObjectsByTagOptional.GetValue(sceneKey).Keys)
            {
                m_ObjectsByTag.Remove(key);
            }
        }

        foreach (var scene in m_EnvironmentEffectsOptional.GetKeys())
        {
            m_EnvironmentEffectsOptional.ClearList(scene);
        }

        m_TaggedObjectsOptional.ClearAll();
        m_ObjectsByTagOptional.ClearAll();
        m_EnvironmentEffectsOptional.ClearAll();
        CachedObjects.Instance.RemoveAllOptional();
        //TODO need to do the above as well
        foreach (var scene in m_CurrentLoadedOptionalScenes)
        {
            Debug.Log($"Removing {scene.name}");
            Core.Mono.WaitForOp(SceneManager.UnloadSceneAsync(scene), (e) =>
            {
                task.Increment();
            });
        }
    }

    private void UnloadAllOptionalLoadedScenes(List<Scene> scenes, Action callback)
    {
        if (scenes.Count == 0)
        {
            Core.Environment.OnEnvironmentOptionalLoadingCleaned?.Invoke();
            callback?.Invoke();
            return;
        }

        TaskAction task = new TaskAction(scenes.Count, () =>
        {
            foreach (var scene in scenes)
            {
                m_CurrentLoadedOptionalScenes.Remove(scene);
            }
            Core.Environment.OnEnvironmentOptionalLoadingCleaned?.Invoke();
            callback?.Invoke();
        });

        foreach (var sceneKey in scenes)
        {
            foreach (var key in m_TaggedObjectsOptional.GetValue(sceneKey).Keys)
            {
                m_TaggedObjects.Remove(key);
            }
        }
        foreach (var sceneKey in scenes)
        {
            foreach (var key in m_ObjectsByTagOptional.GetValue(sceneKey).Keys)
            {
                m_ObjectsByTag.Remove(key);
            }
        }

        foreach (var sceneKey in scenes)
        {
            foreach (var key in m_EnvironmentEffectsOptional[sceneKey])
            {
                m_EnvironmentEffects.Remove(key);
            }
        }

        foreach (var sceneKey in scenes)
        {
            m_TaggedObjectsOptional.GetValue(sceneKey).Clear();
            m_ObjectsByTagOptional.GetValue(sceneKey).Clear();
        }

        foreach (var scene in scenes)
        {
            m_EnvironmentEffectsOptional.ClearList(scene);
        }

        CachedObjects.Instance.RemoveAllOptional();
        //TODO need to do the above as well
        foreach (var scene in scenes)
        {
            Debug.Log($"Removing {scene.name}");
            Core.Mono.WaitForOp(SceneManager.UnloadSceneAsync(scene), (e) =>
            {
                task.Increment();
            });
        }
    }

    private void LoadEnvironmentChunks(EnvironmentData data, List<string> chunkList, ChunkType chunkType, Action<List<Scene>> callback, Action<float> onProgress)
    {

        float progress = GlobalConsts.LOADINGBAR_PERCENTAGE; // starts at 50%
        float progressIncrement = (1f / chunkList.Count) * GlobalConsts.LOADINGBAR_PERCENTAGE; // the  0.5f makes so it goes from 0.5f - 1f

        // load environment bundle
        string bundlePath = $"environments/{data.VariantGuid}";
        Core.Assets.LoadBundle(Core.Mono, bundlePath, (bundle) =>
         {
             SequentialAction.WithResult(chunkList, (chunkName, onTick) =>
             {
                 if (bundle == null)
                 {
                     // message so can see what happening
                     Debug.LogError($"************** {data.EnvironmentName}/{data.VariantName}, has not been built, build this to get rid off error Above**************");
                 }
                 else
                 {
                     // load each chunk from bundle
                     onProgress?.Invoke(progress += progressIncrement);
                     LoadScene(chunkName, chunk =>
                     {
                         // post process chunk
                         ProcessChunk(chunk, chunkType, () => onTick(chunk));
                     });
                 }
             }, callback);
         }, 0);
    }

    private static List<string> GetOnlyOriginalScenes(EnvironmentData data)
    {
        if (null != data.OptionalChunks)
        {
            List<string> validItems = data.Chunks.Except(data.OptionalChunks).ToList();
            return validItems;
        }

        return data.Chunks;
    }

    private static List<string> GetOnlyOptionalScenes(EnvironmentData data, List<string> optionalList)
    {
        List<string> validItems = new List<string>();
        for (int i = 0; i < optionalList.Count; i++)
        {
            string item = data.OptionalChunks.Find(e => e.EndsWith(optionalList[i]));
            if (false == string.IsNullOrEmpty(item))
            {
                validItems.Add(item);
            }
        }
        return validItems;
    }

    private void OnEnvironmentLoaded(List<Scene> chunks, ChunkType chunkType, Action callback)
    {
        List<CatalogueReplaceInScene> totalItemsToReplace = new List<CatalogueReplaceInScene>();
        var catalogue = Core.Catalogue.GetCatalogue;

        // remove all scenes that failed to load
        for (int i = 0; i < chunks.Count; ++i)
        {
            if (chunks[i].IsValid() == false)
            {
                Debug.LogError($"Failed to load scene {chunks[i].name}\n");
                chunks.RemoveAt(i);
                --i;
            }
        }

        // process each loaded chunk sequentially
        SequentialAction.List(chunks, (chunk, chunkTick) =>
        {

            var rootObjects = chunk.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                obj.transform.SetStaticRecursivelyStartsWithStatic();
            }
#if UNITY_ANDROID
            VFXReplace(chunk);
#endif

            var itemsToAddJson = SceneDataManager.Instance.CatalogueReplaceInSceneLoaded.FindAll(e => chunk.name.EndsWith(e.Scene));
            TaskAction task = new TaskAction(itemsToAddJson.Count, () =>
            {
                // process each root object sequentially
                SequentialAction.List(chunk.GetRootGameObjects(), (root, rootTick) =>
                    {
                        // if there is a process for the object, invoke and wait a frame

                        var cam = root.GetComponent<Camera>();
                        if(cam != null)
                        {
                            root.SetActive(false);
                            DebugBeep.LogError($"Has a camera when should not", DebugBeep.MessageLevel.High);

                        }

                        if (root != null)
                        {
                            var processor = GetEnvironmentProcessor(chunk, root, chunkType);
                            if (processor != null)
                            {
                                processor.Invoke(chunk, root, chunkType);
                                Core.Mono.WaitForFrame(rootTick);
                            }
                            else
                            {
                                rootTick();
                            }
                        }
                        else
                        {
                            rootTick();
                        }
                    }, chunkTick);
            });


            // Apply Json to a catalogue item in the scene
            for (int itemIndex = 0; itemIndex < itemsToAddJson.Count; ++itemIndex)
            {
                CatalogueReplaceInScene itemToReplace = itemsToAddJson[itemIndex];

                if (totalItemsToReplace.Contains(itemToReplace) == false)
                {
                    totalItemsToReplace.Add(itemToReplace);
                }
                else
                {
                    Debug.LogError("itemToReplace is a duplicate again");
                }


                var allRoots = chunk.GetRootGameObjects();
                bool foundItem = false;
                for (int rootIndex = 0; rootIndex < allRoots.Length; rootIndex++)
                {
                    GameObject rootGameObject = allRoots[rootIndex];
                    if (rootGameObject.name == itemToReplace.HierarchyPathRoot && foundItem == false)
                    {
                        foundItem = true;
                        // strips the first part of the path off
                        Transform transformFound = null;
                        string toFind = "";
                        if (itemToReplace.HierarchyPathRoot == itemToReplace.HierarchyPath)
                        {
                            transformFound = rootGameObject.transform;
                        }
                        else
                        {
                            toFind = itemToReplace.HierarchyPath.Remove(0, itemToReplace.HierarchyPathRoot.Length + 1);
                            transformFound = rootGameObject.transform.Search(toFind);
                        }

                        CatalogueEntry catalogueEntry = catalogue.Find(e => e.Guid == itemToReplace.CatalogueGUID);

                        if (transformFound != null && catalogueEntry != null)
                        {
                            ConsoleExtra.Log($"ApplyJsonToRoot in scene :{chunk.name},  {itemToReplace.HierarchyPath},  {itemToReplace.CatalogueGUID}", null, ConsoleExtraEnum.EDebugType.Generic);
                            Core.Scene.ApplyJsonToRoot(transformFound.gameObject, itemToReplace.CatalogueGUID, (itemCallBack) =>
                            {
                                var rig = itemCallBack.GetComponent<Rigidbody>();
                                if(rig != null)
                                {
                                    rig.isKinematic = true;
                                    itemCallBack.transform.position = itemToReplace.Position;
                                    itemCallBack.transform.rotation = itemToReplace.Rotation;
                                }
                                rig = transformFound.GetComponent<Rigidbody>();
                                if (rig != null)
                                {
                                    rig.isKinematic = true;
                                    transformFound.transform.position = itemToReplace.Position;
                                    transformFound.transform.rotation = itemToReplace.Rotation;
                                }

                                var vr = itemCallBack.GetComponent<VrInteraction>();
                                if (vr != null)
                                {
                                    itemCallBack.transform.position = itemToReplace.Position;
                                    itemCallBack.transform.rotation = itemToReplace.Rotation;
                                    vr.m_TransformData = itemCallBack.gameObject.GetTransformData();
                                }
                                vr = transformFound.GetComponent<VrInteraction>();
                                if (vr != null)
                                {
                                    transformFound.transform.position = itemToReplace.Position;
                                    transformFound.transform.rotation = itemToReplace.Rotation;
                                    vr.m_TransformData = transformFound.gameObject.GetTransformData();
                                }

                                task.Increment();
                            });
                        }
                        else
                        {
                            if (transformFound == null)
                            {
                                DebugBeep.LogError($"ApplyJsonToRoot Error, Cannot find the {itemToReplace.HierarchyPath}, in catalogue item {itemToReplace.CatalogueGUID}, toFind {toFind}", DebugBeep.MessageLevel.High);
                            }
                            if (catalogueEntry == null)
                            {
                                DebugBeep.LogError($"ApplyJsonToRoot Error, Guid:\"{itemToReplace.CatalogueGUID}\"  {itemToReplace.RealGUID} {itemToReplace.Filepath} is not in the catalogue, check if the Environment has been added to the Catalogue data for this item", DebugBeep.MessageLevel.High);

                            }
                            task.Increment();
                        }
                    }
                }
                if (foundItem == false)
                {
                    Debug.LogError($"Cannot find Item  in scene: {chunk.name}, {itemToReplace.HierarchyPath}, in catalogue item {itemToReplace.CatalogueGUID}");
                    task.Increment();
                }
            }

        }, () =>
        {
            GC.Collect();
            OnEnvironmentPreEffectsLoaded?.Invoke();
            // post process environment processors
            SequentialAction.List(m_EnvironmentEffects, (fx, onTick) =>
            {
                fx.OnEnvironmentEffectsLoaded();
                Core.Mono.WaitForFrame(onTick);
            },
            () =>
            {

                var clonedAll = SceneDataManager.Instance.CatalogueReplaceInSceneLoaded.Clone();
                foreach (var used in totalItemsToReplace)
                {
                    clonedAll.Remove(used);
                }
                if (clonedAll.Count != 0)
                {
                    Debug.LogError($"Not all Jsons used, WHY ???  count : {clonedAll.Count}");
                }
                DynamicGI.UpdateEnvironment();
                callback?.Invoke();
            });
        });
    }

    private void VFXReplace(Scene chunk)
    {
        if(SceneDataManager.Instance.VFXReplaceInSceneLoaded == null)
        {
            Debug.LogError("SceneDataManager.Instance.VFXReplaceInSceneLoaded == null");
            return;
        }

        var itemsToAddJson = SceneDataManager.Instance.VFXReplaceInSceneLoaded.FindAll(e => chunk.name.EndsWith(e.Scene));
        foreach (var item in itemsToAddJson)
        {
            var localItem = item;
            var allRoots = chunk.GetRootGameObjects();
            bool foundItem = false;
            for (int rootIndex = 0; rootIndex < allRoots.Length; rootIndex++)
            {
                var itemFound = allRoots[rootIndex].transform.Find(localItem.HierarchyPath);
                if (itemFound == null)
                {
                    var toFind = localItem.HierarchyPath.Remove(0, localItem.HierarchyPathRoot.Length + 1);
                    itemFound = allRoots[rootIndex].transform.Search(toFind);
                }

                if (itemFound != null && itemFound.CompareTag(Layers.PC_Only) == false)
                {
                    Core.AssetsLocalRef.VisualEffectLocalRef.GetItem(localItem.VfxName, (createItem) =>
                    {
                        if (createItem != null)
                        {
                            try
                            {
                                var eff = createItem.GetComponent<VisualEffect>();
                                var newEff = itemFound.gameObject.ForceComponent<VisualEffect>();
                                newEff.visualEffectAsset = eff.visualEffectAsset;
                                newEff.SetActive(false);
                                Core.Mono.WaitFor(2, () =>
                                {
                                    newEff.SetActive(true);
                                });
                            }
                            catch(Exception e)
                            {
                                Debug.LogError($"Something went wrong {localItem.VfxName} , {e.Message}");
                            }
                        }
                        else
                        {
                            DebugBeep.LogError($"Could not find '{localItem.VfxName}'  , item {itemFound.gameObject.GetGameObjectPath()}", DebugBeep.MessageLevel.High);
                            Core.AssetsLocalRef.VisualEffectLocalRef.GetItemList((items) =>
                            {
                                var all = items;
                                int ff = 0;
                            });                           
                        }
                    });
                }
            }
        }
    }

    private void LoadScene(string sceneName, Action<Scene> callback)
    {
        ConsoleExtra.Log($"LoadScene {sceneName} ", null,  ConsoleExtraEnum.EDebugType.LoadScene);
        Core.Mono.WaitForOp(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive), op =>
        {
            GC.Collect();
            Core.Mono.WaitForFrame(() => callback(GetScene(sceneName)));
        });
    }

    private void ProcessChunk(Scene chunk, ChunkType chunkType, Action callback)
    {
        var roots = chunk.GetRootGameObjects().ToList();
        //var renderers = roots.Extract(r => r.GetComponentsInChildren<MeshRenderer>().ToList()).Flatten();

        //const int PerFrameCount = 50;
        //int count = PerFrameCount;
        //SequentialAction.List(renderers, (renderer, onTick) =>
        //{
        //    ProcessRenderer(chunk, renderer);

        //    count--;
        //    if (count == 0)
        //    {
        //        this.WaitForFrame(onTick);
        //        count = PerFrameCount;
        //    }
        //    else
        //    {
        //        onTick();
        //    }
        //}, () =>
        //{
        foreach (var root in roots)
        {
            foreach (var obj in root.GetComponentsInChildren<Transform>())
            {
                ProcessSceneObject(chunk, obj.gameObject, chunkType);
            }
        }
        callback();
        //});
    }

    //private void ProcessRenderer(Scene chunk, MeshRenderer renderer)
    //{
    //    var material = renderer.sharedMaterial;
    //    if (null == material)
    //    {
    //        if (Application.isEditor == false && renderer.materials.Length > 0)
    //        {
    //            string path = chunk.name + "/" + Utils.Unity.GetScenePath(renderer.transform);
    //            Debug.LogWarning($"Null material: {path}\n", renderer);
    //        }
    //    }
    //    else if (material.shader.name != "Standard")
    //    {
    //        material.shader = Shader.Find(material.shader.name);
    //    }
    //    //renderer.gameObject.layer = Layers.EnvironmentLayer;
    //    //renderer.gameObject.isStatic = true;
    //}

    private void ProcessSceneObject(Scene scene, GameObject go, ChunkType chunkType)
    {
        if (go.CompareTag("Untagged") == false)
        {
            string tag = go.tag;
            if (false == m_ObjectsByTag.ContainsKey(tag))
            {
                m_ObjectsByTag.Add(tag, new List<GameObject>());
            }
            m_ObjectsByTag[tag].Add(go);

            m_TaggedObjects[go.name] = go;

            if (chunkType == ChunkType.Optional)
            {
                var item = m_ObjectsByTagOptional.GetValue(scene, tag);
                if (item == null || item.Count == 0)
                {
                    m_ObjectsByTagOptional.AddValue(scene, tag, new List<GameObject>());
                }
                item = m_ObjectsByTagOptional.GetValue(scene, tag);
                item.Add(go);

                m_TaggedObjectsOptional.AddValue(scene, go.name, go);
            }
        }
    }

    private static Scene GetScene(string sceneName) => SceneManager.GetSceneByName(sceneName);

    private Action<Scene, GameObject, ChunkType> GetEnvironmentProcessor(Scene scene, GameObject go, ChunkType chunkType)
    {
        string taggedName = "";
        if (false == GetNamedTagged(go, ref taggedName))
        {
            return null;
        }

        switch (taggedName)
        {
            case "[SeatingDummies]": return ProcessSeatingBlock;

            case "[ScreenDummies]": return ProcessVideoScreens;
            case "[LEDPanelDummies]": return ProcessLEDPanel;
            case Video360Manager.TAG: return Process360Video;

            case "[ConfettiCannonDummies]": return ProcessConfettiCannons;
            case "[PyroDummies]": return ProcessPyroCannons;
            case "[CryoDummies]": return ProcessCryoCannons;
            //case "[StrobeLightDummies]": return ProcessStrobeLights;
            case "[SmokeMachineDummies]": return ProcessSmokeMachines;

            case "[VideoCameraDummy]": return ProcessVideoCamera;
            case "[HDScreen]": return ProcessHDScreen;


            case "[VRIntroduction]": return ProcessVRIntro;

            case "[AB_Tutorial]": return ProcessVRTutorial;

            case "[ParticleEffects]": return ProcessSceneEffects;

            case "[Paths]": return ProcessPaths;


            case StreamTextures.TAG: return ProcessTextureStreaming;
            case "[StreamVideos]": return ProcessVideoStreaming;
            case CachedObjects.CachedObjectsMono.TAG: return ProcessCachedObjects;
            case "[CatalogueOnly]": return ProcessCatalogueOnly;
            case EnvironmentPosterManager.TAG: return ProcessPosters;
            case "[HouseBuilder]": return ProcessHouseBuilder;

#if VR_INTERACTION
            case "[SpaceHulk]" : return ProcessSpaceHulk;
            case MonitorTrainerConsts.TAG: return ProcessMonitorTrainer;
            case "[Museum]": return ProcessMuseum;
            case "[TimeLine]": return ProcessTimeLine;
#endif
            case "[VRRecordCamera]": return ProcessVRRecordCamera;
            case "[NetworkSpeedTest]": return ProcessNetworkSpeedTest;
            case "[OscController]": return ProcessOscController;
            case "[ARViewer]": return ProcessArViewer;
            case "[WiFiAudioTest]": return ProcessWiFiAudioTest;
            case "[CatalogeOnly]": return null;
            case "[MobileWebAR]": return ProcessMobileWebAR;
            case "[DesertStrike]": return ProcessDesertStrike;
            default:
                Debug.LogError($"Cannot find :{taggedName}");
                return null;
        }
    }

    private void ProcessCatalogueOnly(Scene scene, GameObject obj, ChunkType chunkType)
    {
        Debug.Log("REMOVED [CatalogueOnly]");
        UnityEngine.Object.Destroy(obj);
    }

    private void ProcessTextureStreaming(Scene scene, GameObject obj, ChunkType chunkType)
    {
        StreamTextures textureStreamer = obj.ForceComponent<StreamTextures>();
        textureStreamer.Init();
    }

    private void ProcessVideoStreaming(Scene scene, GameObject obj, ChunkType chunkType)
    {
        StreamVideos videoStreamer = obj.ForceComponent<StreamVideos>();
        videoStreamer.Init();
    }

    private void ProcessCachedObjects(Scene scene, GameObject obj, ChunkType chunkType)
    {
        CachedObjects.Instance.AddObjectsToCache(scene, obj.transform, chunkType);
    }

    private void ProcessMuseum(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if VR_INTERACTION
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(Museum.MuseumRoot):
                Museum.MuseumRoot root = obj.ForceComponent<Museum.MuseumRoot>();
                root.Initialise();
                break;
            default:
                break;
        }
#endif
    }

    private void ProcessTimeLine(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if VR_INTERACTION
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(TimeLine.TimeLineRoot):
                TimeLine.TimeLineRoot root = obj.ForceComponent<TimeLine.TimeLineRoot>();
                root.Initialise();
                break;
            default:
                break;
        }
#endif
    }
    
    private void ProcessArViewer(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if AR_INTERACTION
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(ARViewer.ARViewerRoot):
                ARViewer.ARViewerRoot arViewerRef = obj.ForceComponent<ARViewer.ARViewerRoot>();
                arViewerRef.Initialise();
                break;
            default:
                break;
        }
#endif
    }


    private void ProcessWiFiAudioTest(Scene scene, GameObject obj, ChunkType chunkType)
    {
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(WiFiAudioTest.WiFiAudioTestRoot):
                WiFiAudioTest.WiFiAudioTestRoot test = obj.ForceComponent<WiFiAudioTest.WiFiAudioTestRoot>();
                test.Initialise();
                break;
            default:
                break;
        }
    }

    private void ProcessNetworkSpeedTest(Scene scene, GameObject obj, ChunkType chunkType)
    {
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(NetworkSpeedTest.SpeedTestRoot):
                NetworkSpeedTest.SpeedTestRoot speedRef = obj.ForceComponent<NetworkSpeedTest.SpeedTestRoot>();
                speedRef.Initialise();
                break;
        }
    }


    private void ProcessOscController(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if HouseBuilder
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(OscController.OscControllerRoot):
                OscController.OscControllerRoot rootRef = obj.ForceComponent<OscController.OscControllerRoot>();
                rootRef.Initialise();
                break;
        }
#endif
    }

    private void ProcessHouseBuilder(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if HouseBuilder
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(HouseBuilder.HouseBuilderRoot):
                HouseBuilder.HouseBuilderRoot houseBuilderRef = obj.ForceComponent<HouseBuilder.HouseBuilderRoot>();
                houseBuilderRef.Initialise();
                break;

            case "GUIROOT":
                HouseBuilder.HBGUIManager guiManager = obj.ForceComponent<HouseBuilder.HBGUIManager>();
                guiManager.Initialise();
                break;

            case nameof(FloorAndSkyboxManager):
                FloorAndSkyboxManager interactionFloorRef = obj.ForceComponent<FloorAndSkyboxManager>();
                break;

            case nameof(HouseBuilder.GuiCentre):
                HouseBuilder.GuiCentre guiRGuiGenericRef = obj.ForceComponent<HouseBuilder.GuiCentre>();
                guiRGuiGenericRef.Initialise();
                break;

            case nameof(HouseBuilder.GuiCentreWorldSpace):
                HouseBuilder.GuiCentreWorldSpace guiCentreWorldSpace = obj.ForceComponent<HouseBuilder.GuiCentreWorldSpace>();
                guiCentreWorldSpace.Initialise();
                break;

            case nameof(HouseViewer.HouseViewerRoot): //This should be in ProcessHouseViewer
                HouseViewer.HouseViewerRoot houseViewerRef = obj.ForceComponent<HouseViewer.HouseViewerRoot>();
                houseViewerRef.Initialise();
                break;

            case nameof(HouseViewer.FileLoaderMenu): //This should be in ProcessHouseViewer
                HouseViewer.FileLoaderMenu fileLoaderMenu = obj.ForceComponent<HouseViewer.FileLoaderMenu>();
                fileLoaderMenu.Initialise();
                break;

            case nameof(HouseViewer.SoundLightMenu): //This should be in ProcessHouseViewer
                HouseViewer.SoundLightMenu soundLightMenu = obj.ForceComponent<HouseViewer.SoundLightMenu>();
                soundLightMenu.Initialise();
                break;

            case nameof(HouseViewer.UserSelectionMenu): //This should be in ProcessHouseViewer
                HouseViewer.UserSelectionMenu userSelectionMenu = obj.ForceComponent<HouseViewer.UserSelectionMenu>();
                userSelectionMenu.Initialise();
                break;

            case nameof(OscController.OscControllerRoot):
                OscController.OscControllerRoot rootRef = obj.ForceComponent<OscController.OscControllerRoot>();
                rootRef.Initialise();
                break;

            case nameof(HouseBuilder.AssetSyncMenu):
                HouseBuilder.AssetSyncMenu menuRef = obj.ForceComponent<HouseBuilder.AssetSyncMenu>();
                menuRef.Initialise();
                break;
        }
#endif
    }

    private void ProcessMonitorTrainer(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if VR_INTERACTION
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case nameof(MonitorTrainer.MonitorTrainerRoot):
                MonitorTrainer.MonitorTrainerRoot monitorTrainerRootRef = obj.ForceComponent<MonitorTrainer.MonitorTrainerRoot>();
                monitorTrainerRootRef.Initialise();

                MonitorTrainer.PhysicalAmp physicalAmpRef = obj.ForceComponent<MonitorTrainer.PhysicalAmp>();
                physicalAmpRef.Initialise();
                break;

            case nameof(MonitorTrainer.PhysicalConsole):
                MonitorTrainer.PhysicalConsole physicalConsoleRef = obj.ForceComponent<MonitorTrainer.PhysicalConsole>();
                physicalConsoleRef.Initialise();
                break;

            case nameof(MonitorTrainer.OppositeSide):
                MonitorTrainer.OppositeSide oppositeSide = obj.ForceComponent<MonitorTrainer.OppositeSide>();
                oppositeSide.Initialise();
                break;


            case nameof(MonitorTrainer.CrowdAndGenericRockSoundManager):
                MonitorTrainer.CrowdAndGenericRockSoundManager crowdSoundRef = obj.ForceComponent<MonitorTrainer.CrowdAndGenericRockSoundManager>();
                crowdSoundRef.Initialise();
                break;

            case nameof(MonitorTrainer.MusicSoundManager):
                MonitorTrainer.MusicSoundManager musicSoundRef = obj.ForceComponent<MonitorTrainer.MusicSoundManager>();
                musicSoundRef.Initialise();
                break;

            case nameof(MonitorTrainer.BalloonManager):
                MonitorTrainer.BalloonManager balloonRef = obj.ForceComponent<MonitorTrainer.BalloonManager>();
                balloonRef.Initialise();
                break;

            case nameof(MonitorTrainer.PhoneManager):
                MonitorTrainer.PhoneManager phoneRef = obj.ForceComponent<MonitorTrainer.PhoneManager>();
                phoneRef.Initialise();
                break;


            case nameof(MonitorTrainer.BandManager):
                MonitorTrainer.BandManager bandManagerRef = obj.ForceComponent<MonitorTrainer.BandManager>();
                bandManagerRef.Initialise();
                break;

            case nameof(MonitorTrainer.MenuManager):
                MonitorTrainer.MenuManager menu = obj.ForceComponent<MonitorTrainer.MenuManager>();
                menu.Initialise();
                break;

            default:
                Debug.LogError($"could not find class {className}");
                break;
        }
#endif
    }


    private void ProcessSpaceHulk(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if VR_INTERACTION
        string className = EnvironmentEffect.GetObjectName(obj);
        switch (className)
        {
            case "SpaceHulkRoot":
                SpaceHulk.SpaceHulkRoot spaceHulkRoot = obj.ForceComponent<SpaceHulk.SpaceHulkRoot>();
                spaceHulkRoot.Initialise();
                break;

            case "Demeo":
                Demeo.DemeoRoot demeoRoot = obj.ForceComponent<Demeo.DemeoRoot>();
                demeoRoot.Initialise();
                break;

            case nameof(MonitorTrainer.PhysicalConsole):
                MonitorTrainer.PhysicalConsole physicalConsoleRef = obj.ForceComponent<MonitorTrainer.PhysicalConsole>();
                physicalConsoleRef.Initialise();
                break;

            default:
                Debug.LogError($"could not find class {className}");
                break;
        }
#endif
    }




    private void AddEnvironmentEffect<T>(Scene scene, GameObject obj, ChunkType chunkType) where T : EnvironmentEffect
    {
        T item = obj.AddComponent<T>();
        m_EnvironmentEffects.Add(item);
        item.Initialise();
        if (chunkType == ChunkType.Optional)
        {
            m_EnvironmentEffectsOptional.AddToList(scene, item);
        }
    }

    private static void ProcessSeatingBlock(Scene scene, GameObject obj, ChunkType chunkType)
    {
        GameObject model = Resources.Load<GameObject>("MainArena_ArenaSeating_LOD0");
        Material material = Resources.Load<Material>("GPUInstanceSeat");
        obj.AddComponent<DrawMeshSeating>().Init(model, material);
    }

    private void ProcessVideoScreens(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var script = obj.AddComponent<ProjectorScreenVideoManager>();
        if (true == script.Initialise())
        {
            m_EnvironmentEffects.Add(script);
            if (chunkType == ChunkType.Optional)
            {
                m_EnvironmentEffectsOptional.AddToList(scene, script);
            }
        }
    }


    private void ProcessLEDPanel(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var script = obj.AddComponent<LEDPanelVideoManager>();
        if (true == script.Initialise())
        {
            m_EnvironmentEffects.Add(script);
            if (chunkType == ChunkType.Optional)
            {
                m_EnvironmentEffectsOptional.AddToList(scene, script);
            }
        }
    }

    private void Process360Video(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var script = obj.AddComponent<Video360Manager>();
        if (true == script.Initialise())
        {
            m_EnvironmentEffects.Add(script);
            if (chunkType == ChunkType.Optional)
            {
                m_EnvironmentEffectsOptional.AddToList(scene, script);
            }
        }
    }




    private static void ProcessSplPlanes<T>(Scene scene, GameObject obj, ChunkType chunkType) where T : Component
    {
        var parent = new GameObject(obj.name).transform;
        Core.Scene.ReturnToScene(parent);

        foreach (var dummy in obj.transform.GetDirectChildren())
        {
            if (dummy.GetComponent<MeshRenderer>().enabled == true)
            {
                dummy.SetParent(parent, true);
                dummy.AddComponent<T>();
                dummy.SetActive(true);
            }
        }
    }




    private void ProcessPosters(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentPosterManager>(scene, obj, chunkType);


    private void ProcessConfettiCannons(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentConfettiCannonManager>(scene, obj, chunkType);

    private void ProcessPyroCannons(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentPyroEffectManager>(scene, obj, chunkType);

    private void ProcessCryoCannons(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentCryoEffectManager>(scene, obj, chunkType);

    private void ProcessSmokeMachines(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentSmokeMachineManager>(scene, obj, chunkType);

    private void ProcessVideoCamera(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentVideoCamera>(scene, obj, chunkType);

    private void ProcessHDScreen(Scene scene, GameObject obj, ChunkType chunkType) => AddEnvironmentEffect<EnvironmentConsoleScreen>(scene, obj, chunkType);


    private void ProcessVRRecordCamera(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var controller = obj.ForceComponent<VRRecorder>();
        controller.Initialise();
    }

    private void ProcessVRIntro(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var tutorial = obj.ForceComponent<VRTutorial>();
        tutorial.Initialise();
    }


    private void ProcessPaths(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var followers = obj.ForceComponent<PathFollowers>();
        followers.Initialise();
    }

    private void ProcessVRTutorial(Scene scene, GameObject obj, ChunkType chunkType)
    {
        string taggedName = "";
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);

            if (false == GetNamedTagged(child.gameObject, ref taggedName))
            {
                continue;
            }

            switch (taggedName)
            {
                case "[Robot]":
                    var ab = child.gameObject.ForceComponent<BuildAB>();
                    ab.Initialise();
                    break;

                case "[DialogueBox]":
                    var db = child.gameObject.ForceComponent<BuildDialogueBox>();
                    db.Initialise();
                    break;

                case "[ChargeStation]":
                    var cs = child.gameObject.ForceComponent<BuildChargeStation>();
                    cs.Initialise();
                    break;

                case "[Chaperone]":
                    var ch = child.gameObject.ForceComponent<Chaperone>();
                    ch.Initialise();
                    break;

                default:
                    break;
            }
        }
    }

    private void ProcessSceneEffects(Scene scene, GameObject obj, ChunkType chunkType)
    {
        var effectSystem = obj.ForceComponent<EnvironmentEffects>();
        effectSystem.Init();
    }


    private void ProcessMobileWebAR(Scene scene, GameObject obj, ChunkType chunkType)
    {
#if PLATFORM_WEBGL
        MobileWebAR webAR = obj.ForceComponent<MobileWebAR>();
        webAR.Init();
#endif
    }
    private void ProcessDesertStrike(Scene scene, GameObject obj, ChunkType chunkType)
    {
        DesertStrike.DesertStrikeRoot root = obj.ForceComponent<DesertStrike.DesertStrikeRoot>();
        root.Initialise();
    }

    

    public bool IsNamedTagged(GameObject objectRef)
    {
        string namedTag = "";
        return GetNamedTagged(objectRef, ref namedTag);
    }

    public bool GetNamedTagged(GameObject objectRef, ref string taggedName)
    {
        var match = Regex.Match(objectRef.name, @"\[(\w+)\]");
        taggedName = match.Value;
        return match.Success;
    }
}