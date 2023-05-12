using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveableScene : MonoBehaviour
{

#if UNITY_EDITOR
    public static string RootFolder => GlobalConsts.JSON_SAVEABLE_SCENE_FILE_BASE;
    public Dictionary<string, ISaveable> Saveables => m_SaveablesById;
    public List<ModuleObject> ModuleObjects => m_ModuleObjects;
#endif

    [SerializeField, HideInInspector]
    private string m_DefaultScene = "default";
    [SerializeField, HideInInspector]
    private List<string> m_SpawnObjects;

    public Transform GetSceneTransform => m_Transform;
    private Transform m_Transform;

    private Dictionary<string, ISaveable> m_SaveablesById;
    private List<ModuleObject> m_ModuleObjects;
    private SceneObjectFactory m_Factory;
    private bool m_IsLoading = false;
    private static MonoBehaviour m_Mono;
    public AddCatalogueInteractions m_AddCatalogueInteractions = new AddCatalogueInteractions();
    private List<VrInteraction> m_SpawnedRootVrInteractions;
    private int m_NetworkCreation = 0;

    private void Awake()
    {
        m_Mono = this;
        m_Transform = transform;

        m_Factory = new SceneObjectFactory(this, m_Transform);
        m_SaveablesById = new Dictionary<string, ISaveable>();
        m_ModuleObjects = new List<ModuleObject>();
        m_SpawnedRootVrInteractions = new List<VrInteraction>();
    }

    public void Initialise()
    {
        Core.Environment.OnPreLoadEnvironment += OnPreLoadEnvironment;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4) == true)
        {
            SaveDefaultScene();
        }
        if (Input.GetKeyDown(KeyCode.F5) == true)
        {
            LoadDefaultScene();
        }
    }

    private void OnPreLoadEnvironment()
    {
        Debug.Log("OnPreLoadEnvironment");
        //On changing environment, clean up anything loaded from current environment
        while(m_ModuleObjects.Count > 0)
        {
            //Calling shutdown will immediately de-activate the attached module and prep it for 
            //destroying. It will also return to RemoveModuleObject (see this class) and that will remove
            //the item from the list naturally. (Collection gets modified each loop!)
            m_ModuleObjects[0].Shutdown();            
        }

        //Get all children and destory, Any scripts on children of Scene should handle their own destruction
        //properly unsubscribing to anything they access
        List<Transform> children = m_Transform.GetDirectChildren();
        children.ForEach((c) => Destroy(c.gameObject));
    }        

    public void ReturnToScene(Transform obj) => obj.SetParent(m_Transform, true);
    public ISaveable GetObjectById(string id) => m_SaveablesById.Get(id);

    public List<ModuleObject> GetModulesByCatalogueId(string catalogueId)
    {
        var mobj = m_ModuleObjects.FindAll(m => m.Entry?.Guid == catalogueId);
        return mobj;
    }

    public void AddModuleObject(ModuleObject moduleObject)
    {
        m_ModuleObjects.Add(moduleObject);
        m_SaveablesById.Add(moduleObject.RuntimeId, moduleObject);
        foreach (var saveable in moduleObject.Saveables)
        {
            if (m_SaveablesById.ContainsKey(saveable.RuntimeId) == false)
            {
                m_SaveablesById.Add(saveable.RuntimeId, saveable);
            }
            else
            {
                Debug.LogError($"Cannot add [{saveable.GetType().Name}] to system, the ID {saveable.RuntimeId} already exists\n", saveable as UnityEngine.Object);
            }
        }
    }

    public void RemoveModuleObject(ModuleObject moduleObject)
    {
        m_ModuleObjects.Remove(moduleObject);
        m_SaveablesById.Remove(moduleObject.RuntimeId);
        foreach (var saveable in moduleObject.Saveables)
        {
            m_SaveablesById.Remove(saveable.RuntimeId);
        }
    }

    public ModuleObject CreateModuleObject(params Type[] types)
    {
        var moduleObject = m_Factory.CreateModuleObject(types);
        AddModuleObject(moduleObject);
        return moduleObject;
    }

    public void SpawnPreset(string presetId, Action<List<ModuleObject>> callback)
    {
        var preset = Core.Catalogue.GetPreset(presetId);
        LoadObjects(preset.Contents.Objects, moduleObjects =>
        {
            OnScenePopulated(moduleObjects, preset.Contents, () =>
            {
                callback(moduleObjects);
            });
        });
    }

    public VrInteraction GetSpawnedVrInteraction(string originalName)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        VrInteraction returnData = m_SpawnedRootVrInteractions.Find(e => e.OriginalName.ToLower() == originalName.ToLower());
        if(returnData == null)
        {
            Debug.LogError($"GetSpawnedVrInteraction cannot find : {originalName}");

            string items = "";
            foreach(var item in m_SpawnedRootVrInteractions)
            {
                items += $"\n{item.OriginalName}";
            }
            Debug.LogError($"SpawnedVrInteraction {items}");
        }
        return returnData;
    }

    public VrInteraction GetSpawnedVrInteraction(string originalName, string alsoContains)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        VrInteraction returnData = m_SpawnedRootVrInteractions.Find(e => (e.OriginalName.ToLower() == originalName.ToLower()) && (e.OriginalFullPath.CaseInsensitiveContains(alsoContains)));
        if (returnData == null)
        {
            Debug.LogError($"GetSpawnedVrInteraction cannot find : {originalName}");

            string items = "";
            foreach (var item in m_SpawnedRootVrInteractions)
            {
                items += $"\n{item.OriginalName}";
            }
            Debug.LogError($"SpawnedVrInteraction {items}");
        }
        return returnData;
    }


    public List<VrInteraction> GetSpawnedVrInteractionGUID(string guid)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        List<VrInteraction> returnData = m_SpawnedRootVrInteractions.FindAll(e => e.CatalogueEntryRef.Guid.ToLower() == guid.ToLower());
        return returnData;
    }

    public VrInteraction GetSpawnedVrInteractionScene(string originalName, string sceneName)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        VrInteraction returnData = m_SpawnedRootVrInteractions.Find(e => (e.OriginalName.ToLower() == originalName.ToLower()) && (e.gameObject.scene.name.CaseInsensitiveContains(sceneName)));
        if (returnData == null)
        {
            Debug.LogError($"GetSpawnedVrInteraction cannot find : {originalName}");

            string items = "";
            foreach (var item in m_SpawnedRootVrInteractions)
            {
                items += $"\n{item.OriginalName}";
            }
            Debug.LogError($"SpawnedVrInteraction {items}");
        }
        return returnData;
    }

    public List<VrInteraction> GetSpawnedVrInteractionsConatainingScene(string containing, string sceneName)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        List<VrInteraction> returnData = m_SpawnedRootVrInteractions.FindAll(e => (e.OriginalFullPath.CaseInsensitiveContains(containing)) && (e.gameObject.scene.name.CaseInsensitiveContains(sceneName)));
        return returnData;
    }


    public VrInteraction GetSpawnedVrInteractionScene(string originalName, string alsoContains, string sceneName)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        VrInteraction returnData = m_SpawnedRootVrInteractions.Find(e => (e.OriginalName.ToLower() == originalName.ToLower()) && (e.gameObject.scene.name.CaseInsensitiveContains(sceneName)) && (e.OriginalFullPath.CaseInsensitiveContains(alsoContains)));
        {
            Debug.LogError($"GetSpawnedVrInteraction cannot find : {originalName}");

            string items = "";
            foreach (var item in m_SpawnedRootVrInteractions)
            {
                items += $"\n{item.OriginalName}";
            }
            Debug.LogError($"SpawnedVrInteraction {items}");
        }
        return returnData;
    }



    public List<VrInteraction> GetAllSpawnedVrInteraction()
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        List<VrInteraction> returnData = m_SpawnedRootVrInteractions;
        return returnData;
    }

    public List<VrInteraction> GetAllSpawnedVrInteractionScene(string scene)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        List<VrInteraction> returnData = m_SpawnedRootVrInteractions.FindAll(e => (e.gameObject.scene.name.CaseInsensitiveContains(scene)));
        return returnData;
    }

    public List<VrInteraction> GetAllSpawnedVrInteractionScene(UnityEngine.SceneManagement.Scene scene)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        List<VrInteraction> returnData = m_SpawnedRootVrInteractions.FindAll(e => (e.gameObject.scene == scene));
        return returnData;
    }

    public List<VrInteraction> GetAllSpawnedVrInteraction(string containingPath)
    {
        m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
        List<VrInteraction> returnData = m_SpawnedRootVrInteractions.FindAll(e => e.OriginalFullPath.Contains(containingPath, StringComparison.CurrentCultureIgnoreCase));
        return returnData;
    }


    public void SpawnObject(CatalogueEntry entry, Action<string> onMessage, Action<float> downLoadProgress, Action<ModuleObject> callback)
    {
        Core.Assets.DownloadAndOverwriteSingleAssetIfOutOfDateAndDependencies(entry, (message) =>
        {
            onMessage?.Invoke(message);
        },(realDownLoadProgress) =>
        {
            downLoadProgress?.Invoke(realDownLoadProgress);
        }, () =>
        {
            SpawnObject(entry, callback);
        });
    }

    public void SpawnObjectReplace(GameObject original, string catalogueId, Action<ModuleObject> callback)
    {
        SpawnObjectReplace(original, Core.Catalogue.GetEntry(catalogueId), callback);
    }

    public void SpawnObjectReplace(GameObject original, CatalogueEntry entry, Action<ModuleObject> callback)
    {
        if(original == null)
        {
            Debug.LogError("original is null");
            callback?.Invoke(null);
            return;
        }
        var pos = original.transform.localPosition;
        var rot = original.transform.localRotation;
        var parent = original.transform.parent;

        Destroy(original);
        SpawnObject(entry, (item) =>
        {
            item.transform.parent = parent;
            item.transform.localPosition = pos;
            item.transform.localRotation = rot;

            callback?.Invoke(item);
        });
    }

    public void ApplyJsonToRoot(GameObject root, string catalogueId, Action<ModuleObject> callback)
    {
        CatalogueEntry entry = Core.Catalogue.GetEntry(catalogueId);
        ApplyJsonToRoot(root, entry, callback);
    }

    public void ApplyJsonToRoot(GameObject root, CatalogueEntry entry, Action<ModuleObject> callback)
    {
        if (entry != null)
        {
            var moduleObject = root.AddComponent<ModuleObject>();
            moduleObject.Initialise(entry);
            InternalApplyJsonToRoot(moduleObject, entry, isInEnvironment: true, callback);
        }
        else
        {
            Debug.LogError($"Entry is null  {gameObject.GetGameObjectPath()}", gameObject);
            callback?.Invoke(null);
        }
    }

    private void InternalApplyJsonToRoot(ModuleObject moduleObject, CatalogueEntry entry, bool isInEnvironment, Action<ModuleObject> callback)
    {
        ModuleObject callbackIntem = moduleObject;
        if (callbackIntem.IsValid == true)
        {
            callbackIntem.RootObject.tag = GlobalConsts.RootCatalogueItemTag;
            AddModuleObject(moduleObject);

            SimpleDictionaryList<MetaDataType, VrInteraction> newInteraction = null;
            SimpleDictionaryList<MetaDataType, MetaData> metaData = null;
            m_AddCatalogueInteractions.AddInteractions(callbackIntem.RootObject, entry, ref newInteraction, ref metaData);
            VrInteraction rootVrInteraction = callbackIntem.RootObject.GetComponent<VrInteraction>();
            if (rootVrInteraction == null)
            {
                rootVrInteraction = callbackIntem.RootObject.AddComponent<VrInteraction>();
            }
            callbackIntem.VrInteractionRootRef = rootVrInteraction;
            // is just spawned after enviroment 
            if (isInEnvironment == false)
            {
                callbackIntem.RootObject.ReAssignChildrenRenderShaders();
                rootVrInteraction.GameStarted();

                var allItems = rootVrInteraction.BaseVrInteractions.GetListAll();
                foreach (VrInteraction item in allItems)
                {
                    item.GameStarted();
                }
            }

            m_SpawnedRootVrInteractions.RemoveAll(e => e == null);
            m_SpawnedRootVrInteractions.Add(rootVrInteraction);
            // this to make sure the awakes are done 
            Core.Mono.WaitForFrames(2, () =>
            {
                var allInteractions = newInteraction.GetListAll();
                foreach (var item in allInteractions)
                {
                    item.ReInitiliseOutline();
                }
                rootVrInteraction.BaseVrInteractions = newInteraction;
                rootVrInteraction.BaseVrMetaDatas = metaData;

                callback?.Invoke(callbackIntem);
            });
        }
    }

    public void SpawnObject(CatalogueEntry entry, Action<ModuleObject> callback)
    {
        if(entry == null)
        {
            Debug.LogError("CatalogueEntry is null");
            callback?.Invoke(null);
            return;
        }

        m_Factory.SpawnObject(entry, moduleObject =>
        {
            ModuleObject callbackIntem = moduleObject;
            if (callbackIntem.IsValid == true)
            {
                InternalApplyJsonToRoot(callbackIntem, entry, isInEnvironment: false, callback);
            }
            else
            {
                GameObject.Destroy(moduleObject);
                callback?.Invoke(null);
            }
        });
    }




    public void SpawnObjectNetwork(string catalogueId, string newName, float syncTime, Vector3 position, Quaternion rot, Action<ModuleObject> callback)
    {
#if Photon
        if (Core.PhotonMultiplayerRef.CurrentRoom != null)
        {
            PhotonGeneric.VrCreationData newData = new PhotonGeneric.VrCreationData();
            newData.CatGuid = catalogueId;
            newData.ActorNumber = Core.PhotonMultiplayerRef.MySelf.ActorNumber;
            newData.BaseName = newName;
            newData.NewName = $"{newName}_{m_NetworkCreation}_{Core.PhotonMultiplayerRef.MySelf.ActorNumber}";
            newData.SyncTime = syncTime;
            newData.Position = position;
            newData.Rotation = rot;
            m_NetworkCreation++;
            Core.PhotonGenericRef.SendVrCreation(newData);
        }

        SpawnObject(Core.Catalogue.GetEntry(catalogueId), (item) =>
        {
            item.gameObject.name = newName;
            var vr = item.GetComponent<VrInteraction>();
            vr.ForceCreateGuid();

#if VR_INTERACTION
            if (vr is VrInteractionPickUp)
            {
                VrInteractionPickUp pick = (VrInteractionPickUp)vr;
                Core.Mono.WaitForFrames(20, () =>
                {
                    pick.Grab();
                    Core.Mono.WaitFor(0.5f, () => { pick.Grab(); });
                    if (syncTime >= 1.5f)
                    {
                        Core.Mono.WaitFor(1f, () => { pick.Grab(); });
                    }
                    if (syncTime >= 2.5f)
                    {
                        Core.Mono.WaitFor(2f, () => { pick.Grab(); });
                    }
                    Core.Mono.WaitFor(syncTime/2f, () => { pick.Grab(); });
                    Core.Mono.WaitFor(syncTime, () => { pick.Release(); });
                });
            }
#endif
            callback?.Invoke(item);
        });

#else
            SpawnObject(catalogueId, callback);
#endif
    }

    public void SpawnObject(string catalogueId, Action<ModuleObject> callback)
    {
        SpawnObject(Core.Catalogue.GetEntry(catalogueId), callback);
    }

    public void DestroyObject(ModuleObject obj)
    {
        if (null != obj)
        {
            obj.Shutdown();
        }
    }

    public void CloneObject(ModuleObject source, Action<ModuleObject> callback = null)
    {
        CloneObject(source, source.RootTransform.position, source.RootTransform.rotation, callback);
    }

    public void CloneObject(ModuleObject source, Vector3 position, Quaternion rotation, Action<ModuleObject> callback = null)
    {
        m_Factory.CloneObject(source, position, rotation, moduleObject =>
        {
            AddModuleObject(moduleObject);
            callback?.Invoke(moduleObject);
        });
    }

    public void LoadObjects(List<SceneSaveObject> saveObjects, Action<List<ModuleObject>> callback, Action<float> onProgress = null)
    {
        m_Factory.LoadObjects(saveObjects, callback, onProgress);
    }

    [InspectorButton]
    public void SaveDefaultScene() => Save(m_DefaultScene);
    [InspectorButton]
    public void LoadDefaultScene() => LoadDefaultScene(() => { });
    public void LoadDefaultScene(Action callback, Action<float> onProgress = null)
    {
        Load(m_DefaultScene, callback, onProgress);
    }

    public void Save(string sceneName)
    {
        Json.FullSerialiser.WriteToFile(new SaveScene(m_ModuleObjects), GetFilePath(sceneName), true);
        Debug.Log($"Saved scene: {sceneName}\n");
    }

    public void Load(string sceneName, Action callback, Action<float> onProgress)
    {
        if (m_IsLoading == true)
        {
            return;
        }
        m_IsLoading = true;


        // this will delete items you created, we no longer need this 
        // it was to change programs without restarting the program
        // clear current scene
        //////////m_ModuleObjects.ForEach(m => m.RootObject.DestroyObject());
        //////////m_ModuleObjects.Clear();
        //////////m_SaveablesById.Clear();

        string sceneToLoad = EnvironmentManager.LoadedEnvironmentName.Replace("/", "-");
        // load save scene
        LoadSaveScene(sceneToLoad, saveScene =>
        {
            //Scene could not be loaded
            if(null != saveScene && 
                saveScene.CompatibleEnvironment == EnvironmentManager.LoadedEnvironmentName)
            {
                Debug.Log($"Loading scene data {saveScene.CompatibleEnvironment}");
                // spawn objects + apply data
                LoadObjects(saveScene.Objects, (moduleObjects) =>
                {
                    OnScenePopulated(moduleObjects, saveScene, () =>
                    {
                        Debug.Log("Scene loaded\n");
                        m_IsLoading = false;

                        var task = new TaskAction(m_SpawnObjects.Count, callback);
                        foreach (var uuid in m_SpawnObjects)
                        {
                            SpawnObject(uuid, moduleObject =>
                            {
                                task.Increment();
                            });
                        }
                    });
                }, onProgress);
            }
            else
            {               
                //Loaded scene is not compatible with the loaded environment
                Debug.Log($"Loaded scene {EnvironmentManager.LoadedArenaName} is not compatible with any scene data");
                m_IsLoading = false;
                callback?.Invoke();
            }
        });
    }

    private void OnScenePopulated(List<ModuleObject> moduleObjects, SaveScene saveScene, Action callback)
    {
        // add to id dictionary
        foreach (var moduleObject in moduleObjects)
        {
            AddModuleObject(moduleObject);
        }

        // load id-sensitive data onto objects
        foreach (var saveObject in saveScene.Objects)
        {
            m_SaveablesById[saveObject.RuntimeId].OnPostLoad(saveObject);
        }

        // add to id dictionary
        foreach (var moduleObject in moduleObjects)
        {
            GenerateNewGuids(moduleObject);
        }

        SequentialAction.List(m_ModuleObjects, (m, onTick) =>
        {
            m.OnModulesLoaded();
            this.WaitForFrame(onTick);
        }, () =>
        {
            for (int i = 0; i < moduleObjects.Count; ++i)
            {
                moduleObjects[i].OnSceneLoaded();
            }
            callback?.Invoke();
        });
    }

    public void GenerateNewGuids(ModuleObject moduleObject)
    {
        foreach (var saveable in moduleObject.Saveables)
        {
            GenerateNewGuid(saveable);
        }
        GenerateNewGuid(moduleObject);
    }

    private void GenerateNewGuid(ISaveable saveable)
    {
        bool removed = m_SaveablesById.Remove(saveable.RuntimeId);
        saveable.GenerateNewGuid();

        if (removed == true)
        {
            m_SaveablesById.Add(saveable.RuntimeId, saveable);
        }
    }

    private static void LoadSaveScene(string sceneName, Action<SaveScene> callback)
    {
        // load scene from file
        Debug.Log($"Loading scene: {sceneName}\n");
        string path = GetFilePath(sceneName);

        if (Utils.IO.FileExists($"{path}.json"))
        {
#if UNITY_ANDROID
            Json.AndroidNet.ReadFromFileAsync<SaveScene>(m_Mono, showError: false, path, JsonLibraryType.FullSerializer, (saveScene) =>
#else
            Json.FullSerialiser.ReadFromFileAsync<SaveScene>(path, saveScene =>
#endif
            {
                if (saveScene == null)
                {
                    saveScene = new SaveScene(new List<ModuleObject>());
                }
                else
                {
                    saveScene.ValidateCatalogueEntries(Core.Catalogue);
                }
                callback?.Invoke(saveScene);
            });
        }
        else
        {
            //Still perform callback if file not found
            callback?.Invoke(null);
        }
    }

    public static string GetFilePath(string sceneName) => GlobalConsts.JSON_SAVEABLE_SCENE_FILE_BASE + sceneName;



    public void SafetyCheckForAndroid()
    {
        Debug.LogError("ReAssignChildrenTextMeshProShaders  UNITY_ANDROID && UNITY_EDITOR");


        EnvironmentData data = Core.Environment.CurrentEnvironment;
        // FIX_SHADERS
        //#if UNITY_ANDROID || UNITY_WEBGL || UNITY_EDITOR
        // weird android issues, esp the Oculus Quest
        // this has to be done in the EDITOR, other wise all shaders are pink which come from the asset bundles
        // this has to be done in the BUILD, other wise lightmap baked data will not show up 

        ReAssignChildrenLayers_ANDROID(data);
        ReAssignChildrenRenderShaders_ANDROID(data);
        ReAssignChildrenTextMeshProShaders_ANDROID(data);
        ReAssignChildrenImageShaders_ANDROID(data);
        ReAssignChildrenDecals_ANDROID(data);
        ReAssignChildrenOutlinable_ANDROID(data);
        ReAssignChildrenLightBeam_ANDROID(data);
    }

    private void ReAssignChildrenLayers_ANDROID(EnvironmentData data)
    {
        var transforms = GameObject.FindObjectsOfType<Transform>(true).ToList();
        for (int i = 0; i < transforms.Count; i++)
        {

            if(transforms[i].gameObject.IsValidLayer() == false)
            {
               //// Debug.LogError($"invalid layer on {transforms[i].gameObject.name}", transforms[i].gameObject);
            }
        }
    }


    private void ReAssignChildrenRenderShaders_ANDROID(EnvironmentData data)
    {
        string URP = "Universal Render Pipeline/Lit";
        var renderers = GameObject.FindObjectsOfType<Renderer>(true).ToList();
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderers[i].allowOcclusionWhenDynamic = true;
            foreach (var mat in renderers[i].materials)
            {
                var shader = Shader.Find(mat.shader.name);
                if (shader != null)
                {
                    mat.shader = shader;

                    if (shader.name == URP)
                    {
                        mat.SetFloat("_ReceiveShadows", 0);
                    }
                    if (mat.shader.name.CaseInsensitiveContains("Forge") == true)
                    {
                        Debug.LogError($"Forge shader, game object disabled, {renderers[i].gameObject.GetGameObjectPath()}", renderers[i].gameObject);
                    }
                }
            }
        }
    }

    private void ReAssignChildrenOutlinable_ANDROID(EnvironmentData data)
    {
        var outlinables = GameObject.FindObjectsOfType<EPOOutline.Outlinable>(true).ToList();
        for (int i = 0; i < outlinables.Count; i++)
        {
            outlinables[i].enabled = false;
        }

        var probes = GameObject.FindObjectsOfType<ReflectionProbe>(true).ToList();
        for (int i = 0; i < probes.Count; i++)
        {
            probes[i].enabled = false;
        }
    }
    private void ReAssignChildrenLightBeam_ANDROID(EnvironmentData data)
    {
        //GameObject.FindObjectsOfType<VolumetricLightBeam>();
        ////Debug.LogError("UnityEngine.Rendering.ShadowCastingMode.Off  UNITY_ANDROID && UNITY_EDITOR");
        ////var outlinables = GameObject.FindObjectsOfType<VLB.VolumetricLightBeam>(true).ToList();
        ////for (int i = 0; i < outlinables.Count; i++)
        ////{
        ////    outlinables[i].enabled = false;
        ////}

        ////var probes = GameObject.FindObjectsOfType<ReflectionProbe>(true).ToList();
        ////for (int i = 0; i < probes.Count; i++)
        ////{
        ////    probes[i].enabled = false;
        ////}
    }

    
    private void ReAssignChildrenDecals_ANDROID(EnvironmentData data)
    {
        var decals = GameObject.FindObjectsOfType<UnityEngine.Rendering.Universal.DecalProjector>(true).ToList();
        Debug.LogError($"ReAssignChildrenDecals {decals.Count}");
        for (int i = 0; i < decals.Count; i++)
        {
            var shader = Shader.Find(decals[i].material.shader.name);
            if (shader != null)
            {
                decals[i].material.shader = shader;
            }
        }

        for (int i = 0; i < decals.Count; i++)
        {
            // need local ref
            var decal = decals[i];

            Core.AssetsLocalRef.DecalsLocalRef.GetItem(decal.material.name, (item) =>
            {
                if (item != null)
                {
                    decal.material = item;
                    decal.material.shader = Shader.Find(decal.material.shader.name);
                }
                else
                {
                    Debug.LogError($"Decal not found {decal.material.name}, on: {decal.gameObject.GetGameObjectPath()} ");
                }
            });
        }
    }


    private void ReAssignChildrenTextMeshProShaders_ANDROID(EnvironmentData data)
    {
        var renderers = GameObject.FindObjectsOfType<TMPro.TextMeshProUGUI>(true).ToList();
        for (int i = 0; i < renderers.Count; i++)
        {
            var mat = renderers[i].materialForRendering;
            if (mat != null)
            {
                var shader = Shader.Find(mat.shader.name);
                if (shader != null)
                {
                    mat.shader = shader;
                    if (mat.shader.name.CaseInsensitiveContains("Forge") == true)
                    {
                        Debug.LogError("Forge shader, game object disabled");
                    }
                }
                else
                {
                    Debug.LogError($"Cannot find shader match on : {renderers[i].gameObject.GetGameObjectPath()}");
                }
            }
        }
    }


    private void ReAssignChildrenImageShaders_ANDROID(EnvironmentData data)
    {
        var renderers = GameObject.FindObjectsOfType<Image>(true).ToList();
        for (int i = 0; i < renderers.Count; i++)
        {
            var mat = renderers[i].materialForRendering;
            if (mat != null)
            {
                var shader = Shader.Find(mat.shader.name);
                if (shader != null)
                {
                    mat.shader = shader;
                    if (mat.shader.name.CaseInsensitiveContains("Forge") == true)
                    {
                        Debug.LogError("Forge shader, game object disabled");
                    }
                }
                else
                {
                    Debug.LogError($"Cannot find shader match on : {renderers[i].gameObject.GetGameObjectPath()}");
                }
            }
        }
    }
}
