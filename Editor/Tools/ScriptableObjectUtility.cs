using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    // [MenuItem("Assets/Create/Drum Sound Bank")]
    // public static void DrumSoundBank() { CreateAsset<DrumSoundBank>(); }

    [MenuItem("Assets/Create/Gradient")]
    public static void Gradient() { CreateAsset<GradientData>(); }

    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path) == true)
        {
            path = "Assets";
        }
        else if (string.IsNullOrEmpty(Path.GetExtension(path)) == false)
        {
            path = path.Strip(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)));
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}