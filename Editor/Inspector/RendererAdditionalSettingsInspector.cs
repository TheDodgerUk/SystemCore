using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RendererAdditionalSettings))]
public class RendererAdditionalSettingsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var renderer = ((RendererAdditionalSettings)target).GetRenderer();

        var id = serializedObject.FindProperty("m_SortingLayerId");
        var order = serializedObject.FindProperty("m_SortingOrder");

        serializedObject.Update();

        var layers = SortingLayer.layers.ToList();
        var layer = layers.Find(l => l.id == id.intValue);

        EditorGUI.BeginChangeCheck();
        layer = EditorHelper.Popup("Sorting Layer", layer, layers, l => l.name);
        if (EditorGUI.EndChangeCheck() == true)
        {
            renderer.sortingLayerName = layer.name;
            renderer.sortingLayerID = layer.id;
            id.intValue = layer.id;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(order);
        if (EditorGUI.EndChangeCheck() == true)
        {
            renderer.sortingOrder = order.intValue;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
