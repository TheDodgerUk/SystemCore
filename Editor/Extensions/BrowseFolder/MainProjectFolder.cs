using FilePathHelper;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    public class MainProjectFolder : BrowseFolder
    {
        private static readonly string StreamingPathFromRepo = Application.dataPath + "/StreamingAssets";
        private static readonly string Key = (nameof(MainProjectFolder) + "_main").ShaHash();

        public MainProjectFolder() : base("Main Project", Key, StreamingPathFromRepo, new[] { "data", "bundles" }) { }

        protected override void OnShowFolder(FilePath path)
        {
            EditorHelper.SelectAndPingObject(path.Relative);
        }

        protected override void OnCleanFolder(FilePath path)
        {
            base.OnCleanFolder(path);

            AssetDatabase.Refresh();
        }
    }
}
