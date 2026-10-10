using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class ToolsMenu
{
    [MenuItem("Tools/Find All Non One Scaled")]
    public static void NonOneScaled()
    {
        var transforms = GameObject.FindObjectsOfType<Transform>().ToList();
        var nonOne = transforms.FindAll(t =>
        {
            var s = t.localScale;
            return s.sqrMagnitude != 3 || s.x < 0 || s.y < 0 || s.z < 0;
        });
        Debug.Log(nonOne.Stringify(t => t.GetScenePath(), "\n"));
    }

    [MenuItem("Tools/Clean Empty Folders", false, 200)]
    public static void CleanEmptyFolders()
    {
        FolderCleaner.CleanEmptyFolderInProject();
    }

    [MenuItem("Tools/Clear Progress Bar", false, 202)]
    public static void ClearProgressBar()
    {
        ProgressBar.Clear();
    }

    [MenuItem("Tools/Clear Player Prefs", false, 204)]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Tools/Apply Selected Prefabs", false, 206)]
    public static void ApplySelectedPrefabs()
    {
        using (var progressBar = new ProgressBar("Applying Prefabs", Selection.gameObjects.Length))
        {
            foreach (var selection in Selection.gameObjects)
            {
                progressBar.UpdateIncrement(selection.name);

                var root = PrefabUtility.FindPrefabRoot(selection);

                var originalHideFlags = root.hideFlags;
                if (originalHideFlags != HideFlags.None)
                {
                    Debug.LogWarning($"Had to reset hideFlags on \"{root.name}\". Was to set to [{originalHideFlags}]\n", root);
                    root.hideFlags = HideFlags.None;
                }
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(root);
                if (prefab != null)
                {
                    var options = ReplacePrefabOptions.ConnectToPrefab;
                    PrefabUtility.ReplacePrefab(root, prefab, options);
                }
                root.hideFlags = originalHideFlags;
            }
        }
    }

    [MenuItem("Tools/Switch Child with Parent", false, 208)]
    public static void MakeChildParent()
    {
        var transforms = Selection.transforms;
        using (var progressBar = new ProgressBar("Switching Children", transforms.Length))
        {
            foreach (var child in transforms)
            {
                string undoName = child.name;
                var parent = child.parent;
                if (parent != null)
                {
                    int siblingIndex = parent.GetSiblingIndex();
                    var grandChildren = child.GetDirectChildren();
                    var siblings = parent.GetDirectChildren();
                    siblings.Remove(child);

                    var grandParent = parent.parent;
                    Undo.SetTransformParent(child, grandParent, undoName);
                    Undo.SetTransformParent(parent, child, undoName);
                    child.SetSiblingIndex(siblingIndex);

                    foreach (var sibling in siblings)
                    {
                        Undo.SetTransformParent(sibling, child, undoName);
                    }
                    foreach (var grandChild in grandChildren)
                    {
                        Undo.SetTransformParent(grandChild, parent, undoName);
                    }

                    string parentName = parent.name;
                    Undo.RegisterCompleteObjectUndo(parent, undoName);
                    parent.name = child.name;
                    Undo.RegisterCompleteObjectUndo(child, undoName);
                    child.name = parentName;
                }
                progressBar.UpdateIncrement(child.name);
            }
        }
    }

    [MenuItem("Tools/Spawn Audio Clips", false, 210)]
    public static void SpawnAudioClips() => SpawnAudioClipWindow.ShowWindow();

    [MenuItem("Tools/Spawn Audio Clips", true, 210)]
    public static bool SpawnAudioClips_Validation()
    {
        return Selection.GetFiltered<AudioClip>(SelectionMode.DeepAssets).Length > 0;
    }

    [MenuItem("Tools/Bamboo Builder/Build Application", false, 500)]
    public static void BuildApplication()
    {
        BuildSystem.BambooBuilder.BuildApplication();
    }
}
