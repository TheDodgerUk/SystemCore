using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(SaveableScene), true)]
public class SaveableSceneInspector : Editor
{
    private Dictionary<string, string> m_Names;

    private List<string> m_Uuids;

    private void Awake()
    {
        m_Names = Localisation.LoadLanguage(Localisation.Language.English);
        m_Uuids = Catalogue.LoadEntryIndex();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (m_Names == null)
        {
            Awake();
        }

        var scene = target as SaveableScene;
        if (scene == null)
        {
            return;
        }

        var sceneFiles = Utils.IO.GetFiles(SaveableScene.RootFolder, "*.json");
        var sceneNames = sceneFiles.Extract(f => FilePath.FromAbsolute(f).RemoveExt().FileName);

        var defaultSceneProperty = serializedObject.FindProperty("m_DefaultScene");
        string currentScene = defaultSceneProperty.stringValue;

        string selectedScene = EditorHelper.Popup("Default Scene", currentScene, sceneNames);
        if (selectedScene != currentScene)
        {
            defaultSceneProperty.stringValue = selectedScene;
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Open Scene") == true)
        {
            var path = FilePath.FromAbsolute($"{SaveableScene.GetFilePath(currentScene)}.json");
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path.Relative);
            EditorGUIUtility.PingObject(asset);

            Utils.Code.Run(path.Absolute);
        }

        EditorGUILayout.Space();

        DrawSpawnObjects();

        EditorGUILayout.Space();

        DrawModuleObjects(scene.ModuleObjects);
        DrawSaveables(scene.Saveables);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSpawnObjects()
    {
        var entries = serializedObject.FindProperty("m_SpawnObjects");
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PrefixLabel("Entries to Spawn");
            EditorGUILayout.LabelField($"Count ({entries.arraySize})");
            GUILayout.FlexibleSpace();

            if (Utils.Gui.BtnUtility("+", true) == true)
            {
                entries.InsertArrayElementAtIndex(entries.arraySize);
            }
        }

        for (int i = 0; i < entries.arraySize; ++i)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var property = entries.GetArrayElementAtIndex(i);
                string uuid = property.stringValue;

                if (string.IsNullOrEmpty(uuid) == true)
                {
                    uuid = m_Uuids.First();
                    property.stringValue = uuid;
                }

                string selectedUuid = EditorHelper.Popup(uuid, m_Uuids, id =>
                {
                    return m_Names[CatalogueEntry.GetLocalisationKey(id, "full")];
                });

                if (selectedUuid != uuid)
                {
                    property.stringValue = selectedUuid;
                }

                if (Utils.Gui.BtnRemove(true) == true)
                {
                    entries.DeleteArrayElementAtIndex(i);
                    --i;
                }
            }
        }
    }

    private void DrawModuleObjects(List<ModuleObject> moduleObjects)
    {
        if (moduleObjects != null && moduleObjects.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Modules", EditorStyles.boldLabel);
            using (new GuiIndentScope())
            {
                foreach (var moduleObject in moduleObjects)
                {
                    EditorHelper.ObjectField(moduleObject.RuntimeId, moduleObject);
                }
            }
        }
    }

    private void DrawSaveables(Dictionary<string, ISaveable> saveables)
    {
        if (saveables != null && saveables.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Saveables", EditorStyles.boldLabel);
            using (new GuiIndentScope())
            {
                foreach (var pair in saveables)
                {
                    var mono = pair.Value as Object;
                    EditorHelper.ObjectField(pair.Key, mono);
                }
            }
        }
    }
}
