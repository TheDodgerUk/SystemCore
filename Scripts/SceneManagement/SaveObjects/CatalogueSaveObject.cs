using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CatalogueSaveObject : SceneSaveObject
{
    public readonly string CatalogueId;

    public CatalogueSaveObject(string runtimeId, string catalogueId, Transform transform, List<ISaveable> children) : base(runtimeId, transform, children)
    {
        CatalogueId = catalogueId;
    }
}
