using System.Collections.Generic;

[System.Serializable]
public class CatalogueCategory
{
    public string Name => Localisation.Instance.Get($"catalogue_category_{Guid}");

    public readonly string Guid;
    public readonly List<CatalogueCategory> SubCategories;
    public readonly List<CatalogueFilter> Filters;

    private List<CatalogueEntry> m_Entries;

    public void PopulateEntries(List<CatalogueEntry> allEntries)
    {
        m_Entries = new List<CatalogueEntry>();
        foreach (var entry in allEntries)
        {
            var metaData = entry.GetMetaData<CatalogueMetaData>();
            if (metaData != null)
            {
                if (metaData.Categories.Exists(c => c.CategoryId == Guid) == true)
                {
                    m_Entries.Add(entry);
                }
            }
        }

        foreach (var subCategory in SubCategories)
        {
            subCategory.PopulateEntries(allEntries);
            m_Entries.AddRange(subCategory.m_Entries);
        }
    }

    public List<CatalogueEntry> GetCatalogueEntries() => m_Entries.Clone();
}
