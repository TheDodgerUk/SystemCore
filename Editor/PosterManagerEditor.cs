#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EnvironmentPosterManager;

[CustomEditor(typeof(EnvironmentPosterManager))]
public class PosterManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnvironmentPosterManager targetItems = ((EnvironmentPosterManager)target);
        DrawDefaultInspector();

        string thisGameObjectName = EnvironmentPosterManager.GetObjectName(targetItems.gameObject);
        PosterGroup PosterGroupRef = EnvironmentPosterManager.GetPosterManager(thisGameObjectName);

        EditorGUILayout.LabelField($"Name {thisGameObjectName}");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("MaterailList List");
        for (int i = 0; i < PosterGroupRef.MaterailList.Count; i++)
        {
            EditorGUILayout.LabelField($"\t{PosterGroupRef.MaterailList[i]}");
        }

        EditorGUILayout.LabelField("RendererList List");
        for (int i = 0; i < PosterGroupRef.RendererList.Count; i++)
        {
            EditorGUILayout.LabelField($"\t{PosterGroupRef.RendererList[i]}");
        }
    }
}
#endif