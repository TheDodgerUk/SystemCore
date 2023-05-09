
public class CataloguePreset
{
    public string ShortName => GetLocalisation("short");
    public string FullName => GetLocalisation("full");

    public readonly string Guid;
    public readonly SaveScene Contents;

    private string GetLocalisation(string suffix) => Localisation.Instance.Get($"catalogue_preset_{Guid}_{suffix}");
}

