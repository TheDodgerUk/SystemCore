using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    private static readonly List<BuildTarget> BuildTargets = new List<BuildTarget> { BuildTarget.StandaloneWindows64 };

    [MenuItem("Tools/Asset Bundles/Build for Current Platform", false, 20)]
    private static void DoCreateAssetBundles()
    {
        foreach (var target in BuildTargets)
        {
            const string StreamingAssets = "Assets/StreamingAssets";
            string rootPath = Application.streamingAssetsPath.Strip(StreamingAssets);
            string subPath = string.Concat(StreamingAssets, "/bundles/");
            string fullPath = string.Concat(rootPath, subPath);

            if (Directory.Exists(fullPath) == false)
            {
                Directory.CreateDirectory(fullPath);
            }

            var options = BuildAssetBundleOptions.ChunkBasedCompression;
            var manifest = BuildPipeline.BuildAssetBundles(subPath, options, target);
            var bundles = manifest.GetAllAssetBundles().ToList();
            Debug.Log("Built bundles for " + target + "\n" + bundles.Stringify(s => s, "\n"));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ToolsMenu.ClearProgressBar();
    }

    [MenuItem("Tools/Asset Bundles/Label Catalogue Prefabs", false, 20)]
    private static void LabelAssetBundles()
    {
        const string BundleFormat = "catalogue/prefabs/{0}.unity3d";
        const string PrefabPath = "/Artwork/Prefabs/Catalogue/";
        string fullPath = Application.dataPath + PrefabPath;
        var option = SearchOption.AllDirectories;

        bool changed = false;
        var files = Directory.GetFiles(fullPath, "*.prefab", option);
        for (int i = 0; i < files.Length; ++i)
        {
            string assetPath = files[i].Replace(Application.dataPath, "Assets");
            string name = Path.GetFileNameWithoutExtension(files[i]);
            string bundleName = string.Format(BundleFormat, name);
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer.assetBundleName != bundleName)
            {
                importer.assetBundleName = bundleName;
                changed = true;
            }
        }

        if (changed == true)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ToolsMenu.ClearProgressBar();
        }
    }

    [MenuItem("Tools/Asset Bundles/Clean Asset Bundles", false, 20)]
    private static void CleanAssetBundles()
    {
        string folder = string.Concat(Device.StreamingPath, "/bundles/");
        
        var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
        files.ToList().ForEach(File.Delete);

        FolderCleaner.CleanEmptyFoldersIn(folder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}