using System;
using UnityEngine;

#region MetaData Modules
/// <summary>
/// Inherit from me if you require MetaData and wish to be savable.
/// </summary>
/// <typeparam name="TMetaData">The type of MetaData to initialise with</typeparam>
/// <typeparam name="TSaveData">The type of SaveData to save</typeparam>
public abstract class MetaDataModule<TMetaData, TSaveData> : SaveableObject<TSaveData>, IMetaDataModule<TMetaData> where TSaveData : SaveData
{
    public void PostSetup(ModuleObject moduleObject)
    {
        OnModulesInitialised(moduleObject);
        OnModulesLoaded(moduleObject);
    }

    public abstract void Initialise(ModuleObject moduleObject, TMetaData metaData, Action callback);

    public virtual void OnModulesInitialised(ModuleObject moduleObject) { }

    public virtual void OnModulesLoaded(ModuleObject moduleObject) { }

    public virtual void OnSceneLoaded() { }

    public virtual void Shutdown() { }
}

/// <summary>
/// Inherit from me if you require MetaData but do not require saving
/// </summary>
/// <typeparam name="TMetaData">The type of MetaData to initialise with</typeparam>
public abstract class MetaDataModule<TMetaData> : MonoBehaviour, IMetaDataModule<TMetaData>
{
    public void PostSetup(ModuleObject moduleObject)
    {
        OnModulesInitialised(moduleObject);
        OnModulesLoaded(moduleObject);
    }

    public abstract void Initialise(ModuleObject moduleObject, TMetaData metaData, Action callback);

    public virtual void OnModulesInitialised(ModuleObject moduleObject) { }

    public virtual void OnModulesLoaded(ModuleObject moduleObject) { }

    public virtual void OnSceneLoaded() { }

    public virtual void Shutdown() { }
}
#endregion

#region MetaAttribute Modules
/// <summary>
/// Inherit from me if you need to be added based off an MetaAttribute and wish to be savable
/// </summary>
/// <typeparam name="TSaveData">The type of SaveData to save</typeparam>
public abstract class MetaAttributeModule<TSaveData> : SaveableObject<TSaveData>, IMetaAttributeModule where TSaveData : SaveData
{
    public void PostSetup(ModuleObject moduleObject)
    {
        OnModulesInitialised(moduleObject);
        OnModulesLoaded(moduleObject);
    }

    public void Initialise(ModuleObject moduleObject) => Initialise(moduleObject, () => { });

    public abstract void Initialise(ModuleObject moduleObject, Action callback);

    public virtual void OnModulesInitialised(ModuleObject moduleObject) { }

    public virtual void OnModulesLoaded(ModuleObject moduleObject) { }

    public virtual void OnSceneLoaded() { }

    public virtual void Shutdown() { }
}

/// <summary>
/// Inherit from me if you need to be added based off an MetaAttribute and wish to be savable
/// </summary>
public abstract class MetaAttributeModule : MonoBehaviour, IMetaAttributeModule
{
    public void PostSetup(ModuleObject moduleObject)
    {
        OnModulesInitialised(moduleObject);
        OnModulesLoaded(moduleObject);
    }

    public void Initialise(ModuleObject moduleObject) => Initialise(moduleObject, () => { });

    public abstract void Initialise(ModuleObject moduleObject, Action callback);

    public virtual void OnModulesInitialised(ModuleObject moduleObject) { }

    public virtual void OnModulesLoaded(ModuleObject moduleObject) { }

    public virtual void OnSceneLoaded() { }

    public virtual void Shutdown() { }
}
#endregion

#region Interfaces
public interface IMetaDataModule<TMetaData> : IMetaModule
{
    void Initialise(ModuleObject moduleObject, TMetaData metaData, Action callback);
}

public interface IMetaAttributeModule : IMetaModule
{
    void Initialise(ModuleObject moduleObject, Action callback);
}

public interface IMetaModule
{
    void OnModulesInitialised(ModuleObject moduleObject);
    void OnModulesLoaded(ModuleObject moduleObject);
    void OnSceneLoaded();
    void Shutdown();
}
#endregion
