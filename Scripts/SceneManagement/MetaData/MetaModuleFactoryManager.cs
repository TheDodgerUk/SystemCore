using System;
using System.Collections.Generic;
using UnityEngine;


//https://www.generacodice.com/en/articolo/1667911/reflection-method-getmethod-does-not-return-the-static-method-of-a-class-on-an-iphone-but-does-on-simulator
[UnityEngine.Scripting.Preserve]
public static class MetaModuleFactoryManager
{
    private static readonly List<IMetaAttributeModuleFactory> AttributeFactories = new List<IMetaAttributeModuleFactory>
    {
        new MetaAttributeModuleFactory<MoveableModule>(MetaAttribute.Moveable),
    };

    private static readonly List<IMetaModuleFactory> DataFactories = new List<IMetaModuleFactory>
    {
        new MetaDataModuleFactory<RenderModule, RenderMetaData>(GlobalConsts.RootModel),
    };

    private static readonly Dictionary<MetaAttribute, IMetaAttributeModuleFactory> AttributeFactoriesByType;
    private static readonly Dictionary<Type, IMetaModuleFactory> DataFactoriesByType;

    static MetaModuleFactoryManager()
    {
        AttributeFactoriesByType = AttributeFactories.ExtractAsValues(f => f.Attribute);
        DataFactoriesByType = DataFactories.ExtractAsValues(f => f.GetDataType());
    }

    public static IMetaAttributeModule CreateModule(ModuleObject moduleObject, MetaAttribute attribute)
    {
        return CreateModuleWithFactory(AttributeFactoriesByType, attribute, moduleObject) as IMetaAttributeModule;
    }

    public static IMetaModule CreateModule(ModuleObject moduleObject, MetaData metaData)
    {
        return CreateModuleWithFactory(DataFactoriesByType, metaData.GetType(), moduleObject);
    }

    public static void InitialiseModule(ModuleObject moduleObject, IMetaModule module, MetaData metaData, Action callback)
    {
        InvokeGenericMethod(metaData, nameof(Initialise), moduleObject, module, metaData, callback);
    }

    [UnityEngine.Scripting.Preserve]
    private static IMetaModule CreateModuleWithFactory<TKey, TFactory>(Dictionary<TKey, TFactory> factories, TKey key, ModuleObject moduleObject) where TFactory : IMetaModuleFactory
    {
        if (factories.ContainsKey(key) == true)
        {
            return factories[key].Create(moduleObject);
        }
        else
        {
            Debug.LogWarning($"Could not find Factory for MetaModule from ({key})\n");
            return null;
        }
    }

    [UnityEngine.Scripting.Preserve]
    private static void Initialise<T>(ModuleObject moduleObject, IMetaDataModule<T> module, T metaData, Action callback)
    {
        module.Initialise(moduleObject, metaData, callback);
    }

    [UnityEngine.Scripting.Preserve]
    private static object InvokeGenericMethod(MetaData metaData, string methodName, params object[] args)
    {
        var method = typeof(MetaModuleFactoryManager).GetMethod(methodName, Utils.Reflection.AnyFlags());
        return method.MakeGenericMethod(metaData.GetType()).Invoke(null, args);
    }
}
