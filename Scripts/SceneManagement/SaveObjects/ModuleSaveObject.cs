using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSaveObject : SceneSaveObject
{
    public readonly List<string> Modules;

    public ModuleSaveObject(string runtimeId, Transform transform, List<ISaveable> children) : base(runtimeId, transform, children)
    {
        Modules = children.Extract(s => s.GetType().AssemblyQualifiedName);
    }

    public List<Type> GetModulesTypes()
    {
        return Modules.Extract(m => Type.GetType(m, false)).FindAll(t => t != null);
    }
}
