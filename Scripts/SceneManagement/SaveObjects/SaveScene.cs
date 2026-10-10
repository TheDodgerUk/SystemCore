using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveScene
{
    public readonly string CompatibleEnvironment;
    public readonly List<SceneSaveObject> Objects;

    public SaveScene(List<ModuleObject> moduleObjects)
    {
        Objects = moduleObjects.Extract(m => m.SaveAsType());
    }

    public void ValidateCatalogueEntries(Catalogue catalogue)
    {
        if(catalogue.Count() == 0)
        {
            Debug.LogWarning("Catalogue count is Zero while trying to load from catalogue");
        }

        var missing = Objects.OfType<CatalogueSaveObject>().Where(d => !catalogue.Exists(d.CatalogueId)).ToList();
        if (missing.Count > 0)
        {
            Debug.LogWarning($"Could not find Catalogue entries for GUIDs:\n{missing.Stringify(s => s.CatalogueId)}");
            missing.ForEach(m => Objects.Remove(m));
        }
    }
}
