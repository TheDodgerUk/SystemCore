using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectFactory
{
    private readonly MonoBehaviour m_Host;
    private readonly Transform m_Parent;

    public SceneObjectFactory(MonoBehaviour host, Transform parent)
    {
        m_Host = host;
        m_Parent = parent;
    }

    public ModuleObject CreateModuleObject(Type[] types)
    {
        var moduleObject = m_Parent.CreateChild<ModuleObject>();
        foreach (var type in types)
        {
            if (type.IsOfType<MonoBehaviour>() == true)
            {
                moduleObject.AddChild(moduleObject.RootTransform.CreateChild(type));
            }
        }
        moduleObject.OnModulesInitialised();
        moduleObject.OnModulesLoaded();
        return moduleObject;
    }
                                                       
    public void SpawnObject(CatalogueEntry entry, Action<ModuleObject> callback)
    {
        if (null != entry)
        {
            SpawnCatalogueObject(entry, moduleObject =>
            {
                moduleObject.OnModulesLoaded();
                callback(moduleObject);
            });
        }
        else
        {

            Debug.LogError("Tried to load null catalogue entry; Check catalogue loading for this environment in catalogue project");
            callback(null);
        }
    }



    public void CloneObject(ModuleObject source, Vector3 position, Quaternion rotation, Action<ModuleObject> callback)
    {
        SpawnCatalogueObject(source.Entry, clone =>
        {
            clone.Load(new SceneSaveObject(clone.RuntimeId, position, rotation), () =>
            {
                clone.OnModulesLoaded();
                callback?.Invoke(clone);
            });
        });
    }

    public void LoadObjects(List<SceneSaveObject> saveObjects, Action<List<ModuleObject>> callback, Action<float> onProgress)
    {
        float progress = 0f;
        float progressIncrement = 1f / saveObjects.Count;
        SequentialAction.WithResult(saveObjects, (saveObject, onTick) =>
        {
            SpawnSaveObject(saveObject, moduleObject =>
            {
                onProgress?.Invoke(progress += progressIncrement * 0.5f);
                moduleObject.Load(saveObject, () =>
                {
                    onProgress?.Invoke(progress += progressIncrement * 0.5f);
                    m_Host.WaitForFrame(() => onTick(moduleObject));
                });
            });
        }, callback);
    }

    private void SpawnCatalogueObject(string catalogueId, Action<ModuleObject> callback)
    {
        SpawnCatalogueObject(Core.Catalogue.GetEntry(catalogueId), callback);
    }

    private void SpawnCatalogueObject(CatalogueEntry entry, Action<ModuleObject> callback)
    {
        if (entry != null)
        {
            Debug.Log($"Spawning object: {entry.FullName}");
            var obj = m_Parent.CreateChild<CatalogueEntryModule>();
            obj.Initialise(entry, callback);
        }
        else
        {
            callback(null);
        }
    }

    private void SpawnSaveObject(SceneSaveObject saveObject, Action<ModuleObject> callback)
    {
        if (saveObject is ModuleSaveObject)
        {
            var moduleSave = saveObject as ModuleSaveObject;
            var types = moduleSave.GetModulesTypes();
            callback(CreateModuleObject(types.ToArray()));
        }
        else
        {
            var catalogueSave = saveObject as CatalogueSaveObject;
            SpawnCatalogueObject(catalogueSave.CatalogueId, callback);
        }
    }
}
