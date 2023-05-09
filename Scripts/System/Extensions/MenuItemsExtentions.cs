#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class MenuItemsExtentions
{
    [MenuItem("GameObject/PrintFullPath", false, 0)]
    private static void PrintFullPath()
    {
        var selected = Selection.activeGameObject;
        string name = selected.scene.path.Split('/').Last();
        Debug.LogError($"{name.Replace(".unity", "")} ->  {selected.GetGameObjectPath()}");
    }

    [MenuItem("GameObject/ConvertURPToStandard", false, 0)]
    private static void ConvertAllToStandard()
    {
        Debug.Log("*** FINDING ASSETS BY TYPE ***");

        string STANDARD = "Standard";
        string URP = "Universal Render Pipeline/Lit";
        string URP_UNLIT = "Universal Render Pipeline/Unlit";

        var standard = Shader.Find(STANDARD);
        var lit = Shader.Find(URP);
        var unLit = Shader.Find(URP_UNLIT);

        if (standard == null)
        {
            Debug.LogError("standard is null");
            return;
        }

        if (lit == null)
        {
            Debug.LogError("lit is null");
            return;
        }

        if (unLit == null)
        {
            Debug.LogError("unLit is null");
            return;
        }

        string[] guids;

        guids = AssetDatabase.FindAssets("t:material");
        int count = 0;
        int index = 0;

#if CATALOG_PROGRAM
        string FileName = "D:/FilesChecked_CATALOG";
#else
        string FileName = "D:/FilesChecked_MAIN";
#endif

        List<string> list = Json.FullSerialiser.ReadFromFile<List<string>>(FileName);
        if (list == null)
        {
            list = new List<string>();

        }
        Debug.LogError($"guids {guids.Length}");
        ///return;
        foreach (string guid in guids)
        {
            if (list.Contains(guid) == false)
            {
                Debug.Log("Material: " + AssetDatabase.GUIDToAssetPath(guid));
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Material asset = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (asset.shader.name == URP || asset.shader.name == URP_UNLIT)
                {
                    asset.shader = standard;
                    Debug.LogError("asset: " + asset.name);
                    EditorUtility.SetDirty(asset);
                    count++;
                }
                list.Add(guid);
                index++;
                if (count > 100)
                {
                    AssetDatabase.SaveAssets();
                    break;
                }
            }
        }


        Json.FullSerialiser.WriteToFile(list, FileName, true);
        Debug.LogError($"number of URP changed :{count}  , length {guids.Length}   got to {guids.Length - index}");
    }
}
#endif