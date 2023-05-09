using System.Collections.Generic;

public enum FilterType { Value, Range };

[System.Serializable]
public class CatalogueFilter
{
    public string Name => Localisation.Instance.Get($"catalogue_filter_{Guid}");

    public readonly string Guid;
    public readonly List<CatalogueFilterValue> Values;

    public FilterType GetFilterType()
    {
        bool isValue = Values.Exists(v => v.Max == null || v.Min == null);
        return (isValue ? FilterType.Value : FilterType.Range);
    }
    public CatalogueFilter() { }
}

[System.Serializable]
public class CatalogueFilterValue
{
    public readonly string Value;
    public readonly float? Min;
    public readonly float? Max;
    public CatalogueFilterValue() {}
}
