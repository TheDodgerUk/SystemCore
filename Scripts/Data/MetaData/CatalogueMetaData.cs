using System.Collections.Generic;

[System.Serializable]
[MetaData(MetaDataType.Catalogue)]
public class CatalogueMetaData : MetaData
{
    public readonly List<CatalogueCategoryMetaData> Categories;
    public readonly string Brand;
}

[System.Serializable]
public class CatalogueCategoryMetaData
{
    public readonly string CategoryId;

    public readonly Dictionary<string, string> Filters;

    public CatalogueCategoryMetaData()
    {
        Filters = new Dictionary<string, string>();
    }
}
