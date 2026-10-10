using FilePathHelper;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    [Serializable]
    public class BrowseFolder
    {
        public bool DriveAvailable { get; private set; }

        private readonly string m_DefaultFolderName;
        private readonly List<string> m_SubFolders;
        private readonly string m_FolderType;
        private readonly string m_Extension;
        private readonly string m_Key;

        private string m_Path;

        public BrowseFolder(string folderType, string key, string defaultPath) : this(folderType, key, defaultPath, null) { }
        public BrowseFolder(string folderType, string key, string defaultPath, string[] subFolders) : this(folderType, key, defaultPath, null, string.Empty, subFolders) { }
        public BrowseFolder(string folderType, string key, string defaultPath, string extension, string folderName, string[] subFolders)
        {
            m_Path = PlayerPrefs.GetString(key, defaultPath);
            m_DefaultFolderName = folderName;
            m_SubFolders = subFolders.ToList();
            m_FolderType = folderType;
            m_Extension = extension;
            m_Key = key;

            CheckDriveExists();
        }

        public void UpdateSavedPath() => PlayerPrefs.SetString(m_Key, m_Path);

        public void CheckDriveExists() => DriveAvailable = Utils.IO.DriveExists(m_Path);
        public bool FoldersExist()
        {
            return GetAllPaths().TrueForAll(p => Utils.IO.FolderExists(p.Absolute));
        }

        public List<FilePath> GetAllPaths()
        {
            var path = GetCurrentPath();
            if (m_SubFolders == null)
            {
                return path.List();
            }

            return m_SubFolders.Extract(path.Append);
        }

        public FilePath GetCurrentPath()
        {
            var fullPath = FilePath.FromAbsolute(m_Path);
            if (string.IsNullOrEmpty(m_Extension) == false)
            {
                fullPath = fullPath.Append(m_Extension);
            }
            return fullPath;
        }

        public void DrawFolderPath()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                m_Path = EditorGUILayout.TextField($"{m_FolderType} Folder", m_Path);
                if (string.IsNullOrEmpty(m_Extension) == false)
                {
                    EditorGUILayout.LabelField("/" + m_Extension);
                }
                m_Path = BtnBrowse(m_FolderType, m_Path, m_DefaultFolderName);
            }
        }

        public bool DrawControls(bool isEnabled, Func<GUILayoutOption, bool> additionalControls)
        {
            DrawFolderPathLabels();
            return DrawButtons(isEnabled, additionalControls);
        }

        private void DrawFolderPathLabels()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var width = GUILayout.Width(EditorGUIUtility.labelWidth);
                EditorGUILayout.LabelField($"{m_FolderType} Folder", EditorStyles.boldLabel, width);
                using (new EditorGUILayout.VerticalScope())
                {
                    var paths = GetAllPaths();
                    foreach (var p in paths)
                    {
                        EditorGUILayout.LabelField(p.Absolute);
                    }
                }
            }
        }

        private bool DrawButtons(bool isEnabled, Func<GUILayoutOption, bool> additionalControls)
        {
            const float Width = 120;
            var btnWidth = GUILayout.Width(Width);

            var paths = GetAllPaths();
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GuiEnabledScope(isEnabled && FoldersExist()))
                {
                    if (GUILayout.Button("Show Folder", btnWidth) == true)
                    {
                        paths.ForEach(OnShowFolder);
                    }
                    if (GUILayout.Button("Clean Folder", btnWidth) == true)
                    {
                        paths.ForEach(OnCleanFolder);
                    }
                }

                bool triggered = additionalControls(btnWidth);
                if (triggered == true)
                {
                    UpdateSavedPath();
                }
                return triggered;
            }
        }

        protected virtual void OnShowFolder(FilePath path)
        {
            Utils.IO.OpenFolder(path.Absolute);
        }

        protected virtual void OnCleanFolder(FilePath path)
        {
            Utils.IO.DestroyFolder(path.Absolute);
        }

        private static string BtnBrowse(string type, string currentFolder, string defaultFolderName)
        {
            if (Utils.Gui.Btn("...", true, GUILayout.Width(24)) == true)
            {
                string parent = currentFolder;
                if (string.IsNullOrEmpty(defaultFolderName) == false)
                {
                    parent = FilePath.FromAbsolute(currentFolder).GetFolder().Absolute;
                }

                string title = $"Find your {type} folder";
                string result = EditorUtility.OpenFolderPanel(title, parent, defaultFolderName);
                if (string.IsNullOrEmpty(result) == false)
                {
                    EditorHelper.UnfocusControls();
                    return result;
                }
            }
            return currentFolder;
        }
    }
}
