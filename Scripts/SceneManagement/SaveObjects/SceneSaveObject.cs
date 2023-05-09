using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneSaveObject : SaveData
{
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;

    public readonly List<SaveData> Children;

    public SceneSaveObject(string runtimeId, Transform transform, List<ISaveable> children) : base(runtimeId)
    {
        Position = transform.position;
        Rotation = transform.rotation;
        Children = children.Extract(c => c.Save());
        Children.RemoveAll(c => c == null);
    }

    public SceneSaveObject(string runtimeId, Vector3 position, Quaternion rotation) : base(runtimeId)
    {
        Position = position;
        Rotation = rotation;
        Children = new List<SaveData>();
    }
}
