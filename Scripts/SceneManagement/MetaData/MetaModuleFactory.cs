using System;
using UnityEngine;

public interface IMetaModuleFactory
{
    Type GetDataType();
    IMetaModule Create(ModuleObject moduleObject);
}

public interface IMetaDataModuleFactory<TMetaData> : IMetaModuleFactory
{
    void Initialise(ModuleObject moduleObject, IMetaDataModule<TMetaData> module, TMetaData metaData, Action callback);
}

public interface IMetaAttributeModuleFactory : IMetaModuleFactory
{
    MetaAttribute Attribute { get; }
}

public abstract class MetaModuleFactory<TModule> : IMetaModuleFactory where TModule : MonoBehaviour, IMetaModule
{
    private readonly string m_ParentObjectName;

    public MetaModuleFactory(string parentObjectName)
    {
        m_ParentObjectName = parentObjectName;
    }

    public IMetaModule Create(ModuleObject moduleObject)
    {
        var transform = moduleObject.RootTransform;
        if (string.IsNullOrEmpty(m_ParentObjectName) == false)
        {
            var parent = transform.Find(m_ParentObjectName);
            if (parent == null)
            {
                return transform.CreateChild<TModule>(m_ParentObjectName);
            }
            else
            {
                return parent.AddComponent<TModule>();
            }
        }
        return transform.CreateChild<TModule>();
    }

    public abstract Type GetDataType();
}

public class MetaAttributeModuleFactory<TModule> : MetaModuleFactory<TModule>, IMetaAttributeModuleFactory
    where TModule : MonoBehaviour, IMetaModule
{
    public MetaAttribute Attribute { get; }

    public MetaAttributeModuleFactory(MetaAttribute attribute) : this(attribute, null) { }
    public MetaAttributeModuleFactory(MetaAttribute attribute, string parentObjectName) : base(parentObjectName)
    {
        Attribute = attribute;
    }

    public override Type GetDataType() => typeof(MetaAttribute);
}

public class MetaDataModuleFactory<TModule, TMetaData> : MetaModuleFactory<TModule>, IMetaDataModuleFactory<TMetaData>
    where TModule : MonoBehaviour, IMetaDataModule<TMetaData>
{
    public MetaDataModuleFactory() : base(null) { }
    public MetaDataModuleFactory(string parentObjectName) : base(parentObjectName) { }

    public void Initialise(ModuleObject moduleObject, IMetaDataModule<TMetaData> module, TMetaData metaData, Action callback)
    {
        module.Initialise(moduleObject, metaData, callback);
    }

    public override Type GetDataType() => typeof(TMetaData);
}


