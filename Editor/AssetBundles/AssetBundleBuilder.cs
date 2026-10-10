using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Bundles
{
    public class AssetBundleBuilder
    {
        public const string BundleNameStart = "Environment";
        public const string BundleExtension = "unity3d";
        public static readonly List<BuildTarget> ValidPlatforms = new List<BuildTarget> { BuildTarget.StandaloneWindows64 };
        public static readonly FilePath StagingFolder = FilePath.FromRelative("Assets/staged_bundles");

        public static bool BuildBundles(BuildTarget platform, bool forceRebuild)
        {
            if (ValidPlatforms.Contains(platform) == true)
            {
                var folderPath = GetStagingPath(platform);
                Utils.IO.ForceFolder(folderPath.Absolute);
                EditorUtility.DisplayProgressBar("Building Bundles", "", 0);

                var options = GetOptions(forceRebuild);
                var manifest = BuildPipeline.BuildAssetBundles(folderPath.Relative, options, platform);
                if (manifest != null)
                {
                    CreateDependencyMap(folderPath, manifest);
                    return true;
                }
            }
            return false;
        }

        private static void CreateDependencyMap(FilePath folderPath, AssetBundleManifest manifest)
        {
            var map = new Dictionary<string, List<string>>();
            var bundles = manifest.GetAllAssetBundles();
            using (var progressBar = new ProgressBar("Building Dependencies", bundles.Length + 1))
            {
                for (int i = 0; i < bundles.Length; ++i)
                {
                    progressBar.UpdateIncrement($"Creating dependencies for {bundles[i]}");
                    map.Add(bundles[i], manifest.GetDirectDependencies(bundles[i]).ToList());
                }

                progressBar.UpdateIncrement("Writing JSON");
                string path = folderPath.Append("master").Absolute;
                Json.JsonNet.WriteToFile(map, path, true);
            }
        }

        public static void CopyToDestination(BuildTarget platform, string destinationFolder, bool cleanCopy)
        {
            Utils.Editor.CopyFiles(GetStagingPath(platform).Absolute, destinationFolder, cleanCopy, BundleExtension, "json");
        }

        public static void CopyFromSource(BuildTarget platform, string sourceFolder, bool cleanCopy)
        {
            Utils.Editor.CopyFiles(sourceFolder, GetStagingPath(platform).Absolute, cleanCopy, BundleExtension, "json");
        }

        public static void CleanAssetBundles(List<BuildTarget> targets)
        {
            for (int i = 0; i < targets.Count; ++i)
            {
                Utils.IO.DestroyFolder(GetStagingPath(targets[i]).Absolute);
            }
            FolderCleaner.CleanEmptyFoldersIn(StagingFolder.Absolute, true);
        }

        public static bool IsPlatformBuilt(BuildTarget platform) => Utils.IO.FolderExists(GetStagingPath(platform).Absolute);
        public static bool IsAnyPlatformBuilt() => Utils.IO.FolderExists(StagingFolder.Absolute);

        private static FilePath GetStagingPath(BuildTarget platform) => StagingFolder.Append(platform.ToString().ToLower());
        private static BuildAssetBundleOptions GetOptions(bool forceRebuild)
        {
            var options = BuildAssetBundleOptions.ChunkBasedCompression;
            if (forceRebuild == true)
            {
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }
            return options;
        }
    }
}
