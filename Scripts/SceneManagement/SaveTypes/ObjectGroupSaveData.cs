using System.Collections.Generic;

public class ObjectGroupSaveData : SaveData
{
    public readonly List<SceneSaveObject> Objects;
    public readonly ObjectGroupLayout Layout;

    public ObjectGroupSaveData(string runtimeId, List<ModuleObject> moduleObjects, ObjectGroupLayout layout) : base(runtimeId)
    {
        Objects = moduleObjects.Extract(o => o.SaveAsType());
        Layout = layout;
    }
}
