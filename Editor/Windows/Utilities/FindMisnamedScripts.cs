using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FindMisnamedScripts : EditorWindow
{
    [MenuItem("Window/Utility/Find Misnamed Scripts")]
    public static void ShowWindow()
    {
        GetWindow<FindMisnamedScripts>("Misnamed Scripts");
    }

    private List<MisnamedScript> m_MisnamedScripts = new List<MisnamedScript>();
    private List<string> m_MultipleFilePaths = new List<string>();

    private Vector2 m_ScrollPos;

    private void OnEnable()
    {
        var scriptGuids = AssetDatabase.FindAssets("t:MonoScript").ToList();
        var filePaths = scriptGuids.Extract(AssetDatabase.GUIDToAssetPath);

        m_MultipleFilePaths = filePaths.GetDuplicates();
        filePaths.RemoveAll(m_MultipleFilePaths.Contains);

        var fileNames = filePaths.ExtractAsValues(Path.GetFileNameWithoutExtension);

        var monoTypes = Utils.Reflection.GetConcreteChildren<MonoBehaviour>();
        m_MisnamedScripts = monoTypes.Extract(t => GetMisnamedScript(t, fileNames));
        m_MisnamedScripts.RemoveAll(t => !string.IsNullOrEmpty(t.FilePath));
    }

    private void OnGUI()
    {
        if (m_MultipleFilePaths.Count > 0)
        {
            EditorGUILayout.LabelField($"Multiple MonoBehaviours found in single files ({m_MultipleFilePaths.Count}) times", EditorStyles.boldLabel);
            foreach (var filePath in m_MultipleFilePaths)
            {
                EditorGUILayout.LabelField(filePath);
            }
            GUILayout.Space(16);
        }

        EditorGUILayout.LabelField($"Found ({m_MisnamedScripts.Count}) misnamed scripts", EditorStyles.boldLabel);
        using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
        {
            foreach (var scriptType in m_MisnamedScripts)
            {
                EditorGUILayout.LabelField(scriptType.Type.Name);
            }

            m_ScrollPos = s.scrollPosition;
        }
    }

    private MisnamedScript GetMisnamedScript(Type type, Dictionary<string, string> fileNames)
    {
        string name = type.Name.SplitList("`").First();
        return new MisnamedScript()
        {
            FilePath = fileNames.Get(name),
            FileName = name,
            Type = type,
        };
    }

    public class MisnamedScript
    {
        public string FileName;
        public string FilePath;
        public Type Type;
    }
}
