
public class RenderModuleSaveData : SaveData
{
    public readonly bool HideModel;

    public RenderModuleSaveData(string runtimeId, bool hideModel) : base(runtimeId)
    {
        HideModel = hideModel;
    }
}
