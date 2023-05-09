using System.Collections.Generic;
using UnityEngine;

public enum ObjectLayoutType { None, Linear }

[System.Serializable]
public abstract class ObjectGroupLayout : ScriptableObject
{
    public abstract ObjectLayoutType Type { get; }

    public abstract void Position(List<ModuleObject> objects);
}
