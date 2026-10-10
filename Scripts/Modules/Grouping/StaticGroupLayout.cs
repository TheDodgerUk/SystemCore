using System.Collections.Generic;

[System.Serializable]
public class StaticGroupLayout : ObjectGroupLayout
{
    public override ObjectLayoutType Type => ObjectLayoutType.None;

    public override void Position(List<ModuleObject> objects) { }
}
