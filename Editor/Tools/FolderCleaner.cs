using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class FolderCleaner : UnityEditor.AssetModificationProcessor
{
    private const string LastKey = "jadabd";
    public static bool Enabled = true;

    private static bool ShouldClean()
    {
        string lastStr = PlayerPrefs.GetString(LastKey, "0");
        var lastTime = new DateTime(long.Parse(lastStr));
        var nextTime = lastTime.AddHours(4);
        return (DateTime.Now > nextTime);
    }

    // UnityEditor.AssetModificationProcessor
    public static string[] OnWillSaveAssets(string[] paths)
    {
        if (Enabled == true && ShouldClean() == true)
        {
            CleanEmptyFolderInProject();
        }
        return paths;
    }

    public static void CleanEmptyFolderInProject()
    {
        CleanEmptyFoldersIn(Application.dataPath);
    }

    public static void CleanEmptyFoldersIn(string root, bool suppressLog = false)
    {
        if (Utils.IO.FolderExists(root) == false)
        {
            return;
        }

        if (root == Application.dataPath)
        {
            PlayerPrefs.SetString(LastKey, DateTime.Now.Ticks.ToString());
        }
        using (var progressBar = new ProgressBar("Cleaning Empty Folders", "Searching Project..."))
        {
            var empties = FindEmptyFolders(root);

            int count = empties.Count;
            progressBar.SetMaxIncrement(count);

            for (int i = 0; i < count; ++i)
            {
                string name = empties[i].FullName;
                progressBar.UpdateIncrement($"Removing {name}");

                if (File.Exists(name + ".meta") == true)
                {
                    string relativePath = GetRelativePathFromCd(name);
                    AssetDatabase.MoveAssetToTrash(relativePath);
                }
                else
                {
                    Directory.Delete(name, true);
                }
            }

            if (suppressLog == false && count > 0)
            {
                string removed = empties.Stringify(f => f.FullName);
                Debug.Log($"Complete Folder Cleanup. Removed ({count}) folders: \n{removed}");
            }
        }
    }

    private static string GetRelativePathFromCd(string filespec)
    {
        return GetRelativePath(filespec, Directory.GetCurrentDirectory());
    }

    private static string GetRelativePath(string filespec, string folder)
    {
        // Folders must end in a slash
        if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            folder += Path.DirectorySeparatorChar;
        }

        var folderUri = new Uri(folder).MakeRelativeUri(new Uri(filespec));
        return Uri.UnescapeDataString(folderUri.ToString().Replace('/', Path.DirectorySeparatorChar));
    }

    private static List<DirectoryInfo> FindEmptyFolders(string root)
    {
        var emptyDirs = new List<DirectoryInfo>();
        var assetDir = new DirectoryInfo(root);

        WalkDirectoryTree(assetDir, (dirInfo, areSubDirsEmpty) =>
        {
            bool isDirEmpty = areSubDirsEmpty && DirHasNoFile(dirInfo);
            if (isDirEmpty == true)
            {
                emptyDirs.Add(dirInfo);
            }
            return isDirEmpty;
        });
        return emptyDirs;
    }

    delegate bool IsEmptyDirectory(DirectoryInfo dirInfo, bool areSubDirsEmpty);

    private static bool WalkDirectoryTree(DirectoryInfo root, IsEmptyDirectory isFolderEmpty)
    {
        var children = root.GetDirectories();

        bool areChildrenEmpty = true;
        for (int i = 0; i < children.Length; ++i)
        {
            if (WalkDirectoryTree(children[i], isFolderEmpty) == false)
            {
                areChildrenEmpty = false;
            }
        }

        return isFolderEmpty(root, areChildrenEmpty);
    }

    private static bool DirHasNoFile(DirectoryInfo dirInfo)
    {
        FileInfo[] files = null;

        try
        {
            files = dirInfo.GetFiles("*.*");
            files = files.Where(x => !IsMetaFile(x.Name)).ToArray();
        }
        catch { }

        return (files == null || files.Length == 0);
    }

    private static bool IsMetaFile(string path)
    {
        return path.EndsWith(".meta");
    }
}
