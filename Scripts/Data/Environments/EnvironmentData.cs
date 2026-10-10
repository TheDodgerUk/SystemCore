using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentData
{
    private const string EnvironmentKey = "environment";
    private const string VariantKey = "environment_variant";

    public string EnvironmentName => Localisation.Instance.Get($"environment_{EnvironmentGuid}_name");
    public string VariantName => Localisation.Instance.Get($"environment_variant_{VariantGuid}_name");

    public string OverloadEnvironmentName()
    {
        if (UseOverloadData == true)
        {
            return Localisation.Instance.Get($"{EnvironmentKey}_{OverloadEnvironmentGuid}_name");
        }
        else
        {
            return Localisation.Instance.Get($"{EnvironmentKey}_{EnvironmentGuid}_name");
        }
    }

    public string NonOverloadEnvironmentName()
    {
        return Localisation.Instance.Get($"{EnvironmentKey}_{EnvironmentGuid}_name");
    }

    public string OverloadVariantName()
    {
        if (UseOverloadData == true)
        {
            return Localisation.Instance.Get($"{VariantKey}_{OverloadVariantGuid}_name");
        }
        else
        {
            return Localisation.Instance.Get($"{VariantKey}_{VariantGuid}_name");
        }
    }

    public string NonOverloadVariantName()
    {
        return Localisation.Instance.Get($"{VariantKey}_{VariantGuid}_name");
    }



    public List<string> GetExtraBundlesInclude()
    {
        var extraList = GlobalConsts.GetExternalSourceAssetBundlesInMask(ExtraBundlesToInclude);
        return extraList;
    }

    public readonly string EnvironmentGuid;
    public readonly string VariantGuid;
    public readonly List<string> Chunks;
    public readonly List<string> OptionalChunks;
    public readonly bool Combined;

    public readonly bool SkipIntro;
    public readonly Vector3 StartPosition;

    public GlobalConsts.InteractionType InteractionType;
    public bool UseOverloadData;
    public string OverloadEnvironmentGuid;
    public string OverloadVariantGuid;
    public bool IncludeCataloguePrefabs;
    public bool IncludeGenericAssetBundles;
    public bool SingleCatalogueItemOnly;
    public int ExtraBundlesToInclude;
    public int ExtraBundlesFromServer;
    public bool CataloguePrefabsFromServer;
    public bool CatalogueListFromServer;

    public readonly EnvironmentStructureData Structure;
}