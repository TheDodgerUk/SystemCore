using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Bundles
{
    public class BundleBuilderWindow : EditorWindow, ISerializationCallbackReceiver
    {
        [MenuItem("Window/Custom/Asset Bundle Builder", false, 2)]
        public static void ShowWindow() => GetWindow<BundleBuilderWindow>("Bundle Builder");

        private BuildTarget m_Platform = BuildTarget.StandaloneWindows64;

        private BrowseFolder m_MainProjectFolder;
        private BrowseFolder m_NetworkFolder;

        private bool m_AllPlatforms = false;
        private bool m_CleanCopy = false;

        private bool m_Serialised = false;

        private void Awake()
        {
            m_Platform = EditorUserBuildSettings.activeBuildTarget;
            m_MainProjectFolder = new MainProjectFolder();
            m_NetworkFolder = new NetworkFolder();
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => m_Serialised = true;

        private void OnGUI()
        {
            if (m_Serialised == true)
            {
                Awake();
                m_Serialised = false;
            }

            GUILayout.Label("Asset Bundle Builder", EditorStyles.boldLabel);

            var label = new GUIContent("Target Platform");
            var platform = EditorUserBuildSettings.activeBuildTarget;
            EditorHelper.EnumPopup(label, platform, platform.List());

            EditorGUILayout.Space();
            m_NetworkFolder.DrawFolderPath();
            EditorGUILayout.Space();

            // build / copy options
            m_CleanCopy = EditorGUILayout.Toggle("Clean Copy", m_CleanCopy);

            if (m_NetworkFolder.DriveAvailable == false && GUILayout.Button("Rescan for Network Drive") == true)
            {
                const string AreYouSure = "Are you sure?\n\nThis can be slow if the network drive does not exist";
                if (EditorUtility.DisplayDialog("Rescan for Network Drive.", AreYouSure, "Yes", "No") == true)
                {
                    m_NetworkFolder.CheckDriveExists();
                }
            }

            EditorGUILayout.Space();
            DrawMainProjectControls();

            EditorGUILayout.Space();
            DrawNetworkControls();
        }

        private void DrawMainProjectControls()
        {
            m_MainProjectFolder.DrawControls(true, btnWidth =>
            {
                return false;
            });
        }

        private void DrawNetworkControls()
        {
            using (new GuiEnabledScope(m_NetworkFolder.DriveAvailable))
            {
                bool networkExists = m_NetworkFolder.DriveAvailable && m_NetworkFolder.FoldersExist();
                m_NetworkFolder.DrawControls(networkExists, btnWidth =>
                {
                    using (new GuiEnabledScope(networkExists))
                    {
                        if (GUILayout.Button("Pull from", btnWidth) == true)
                        {
                            CopyNetworkBundles();
                            return true;
                        }
                    }
                    return false;
                });
            }
        }

        private List<BuildTarget> GetPlatforms()
        {
            if (m_AllPlatforms == true)
            {
                return Utils.Code.GetEnumValues<BuildTarget>();
            }
            return m_Platform.List();
        }

        private void CopyNetworkBundles()
        {
            TransferOverNetwork((localPath, networkPath, extensions) =>
            {
                Utils.Editor.CopyFiles(networkPath, localPath, m_CleanCopy, extensions);
            });
        }

        private delegate void TransferHandler(string localPath, string networkPath, params string[] extensions);
        private void TransferOverNetwork(TransferHandler transferHandler)
        {
            var projectPath = m_MainProjectFolder.GetCurrentPath();
            var networkPath = m_NetworkFolder.GetCurrentPath();

            string networkDataPath = networkPath.Append("data").Absolute;
            string projectDataPath = projectPath.Append("data").Absolute;
            transferHandler(projectDataPath, networkDataPath, "json");

            string platform = EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();
            string networkBundlePath = networkPath.Append("bundles").Append(platform).Absolute;
            string projectBundlePath = projectPath.Append("bundles").Absolute;
            transferHandler(projectBundlePath, networkBundlePath, "json", AssetBundleBuilder.BundleExtension);
        }
    }
}
