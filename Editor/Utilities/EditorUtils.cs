using FilePathHelper;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Utils
{
    public static class Editor
    {
        public static SceneView GetSceneView()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                return SceneView.lastActiveSceneView;
            }
            if (SceneView.sceneViews.Count > 0)
            {
                return SceneView.sceneViews[0] as SceneView;
            }
            return EditorWindow.GetWindow<SceneView>();
        }

        public static void CopyFiles(string sourceFolder, string destinationFolder, bool cleanCopy, params string[] extensionsToCopy)
        {
            if (IO.FolderExists(sourceFolder) == true)
            {
                if (cleanCopy == true && IO.FolderExists(destinationFolder) == true)
                {
                    IO.DestroyFolder(destinationFolder);
                }

                var exts = extensionsToCopy.ToList().Extract(e => $"*.{e}");
                var sourceFiles = exts.Extract(e => GetFiles(sourceFolder, e)).Flatten();

                using (var progressBar = new ProgressBar("Copying Files", sourceFiles.Count))
                {
                    for (int i = 0; i < sourceFiles.Count; ++i)
                    {
                        progressBar.UpdateIncrement(sourceFiles[i]);

                        var sourcePath = FilePath.FromAbsolute(sourceFiles[i]);
                        var destinationPath = sourcePath.ChangeRoot(sourceFolder, destinationFolder);

                        IO.ForceFolder(destinationPath.GetFolder().Absolute);
                        string destination = destinationPath.Absolute.DesanitiseSlashes();
                        string source = sourcePath.Absolute.DesanitiseSlashes();
                        File.Copy(source, destination, true);
                    }
                }
            }
        }

        private static List<string> GetFiles(string folder, string searchPattern)
        {
            return Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories).ToList();
        }
    }
}
