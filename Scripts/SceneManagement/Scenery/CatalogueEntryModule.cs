using System;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueEntryModule : MonoBehaviour
{
    public ModuleObject ModuleObject { get; private set; }

    [SerializeField]
    public CatalogueEntry m_Entry;

    [SerializeField]
    private string m_ShortName;

    public void Initialise(CatalogueEntry entry, Action<ModuleObject> callback)
    {
        m_Entry = entry;
        m_ShortName = m_Entry.ShortName;
        ModuleObject = gameObject.AddComponent<ModuleObject>();
        ModuleObject.Initialise(m_Entry);

        OnMetaDataLoaded(() => callback(ModuleObject));
    }

    private void OnMetaDataLoaded(Action callback)
    {
        var metaData = m_Entry.GetMetaData();


        CreateModules(ModuleObject, m_Entry.GetAllMetaData(), m_Entry.Attributes, () =>
        {
            ModuleObject.OnModulesInitialised();
            callback();
        });
    }

    private static void CreateModules(ModuleObject moduleObject, List<MetaData> metaData, List<MetaAttribute> attributes, Action callback)
    {
        var dataModules = metaData.Extract(m => new { Module = MetaModuleFactoryManager.CreateModule(moduleObject, m), MetaData = m });
        var attributeModules = attributes.Extract(m => MetaModuleFactoryManager.CreateModule(moduleObject, m));

        dataModules.RemoveAll(m => m.Module == null);
        attributeModules.RemoveAll(m => m == null);

        var allModules = dataModules.Extract(p => p.Module);
        attributeModules.ForEach(m => allModules.Add(m));

        allModules.ForEach(moduleObject.AddChild);

        // initialise meta data modules
        SequentialAction.List(dataModules, (pair, onTick) =>
        {
            MetaModuleFactoryManager.InitialiseModule(moduleObject, pair.Module, pair.MetaData, () =>
            {
                onTick();
                //moduleObject.WaitForFrame(onTick);
            });
        }, () =>
        {
            // initialise attribute modules
            SequentialAction.List(attributeModules, (module, onTick) =>
            {
                module.Initialise(moduleObject, () =>
                {
                    onTick();
                    //moduleObject.WaitForFrame(onTick);
                });
            }, callback);
        });
    }
}
