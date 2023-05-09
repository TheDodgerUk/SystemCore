using EditorTools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectGroupModule), true)]
public class ObjectGroupModuleInspector : Editor
{
    private static Dictionary<ObjectLayoutType, Type> m_LayoutTypes;

    private bool m_AutoUpdate = false;

    private void OnEnable()
    {
        if (m_LayoutTypes == null)
        {
            var types = Utils.Reflection.GetConcreteChildren<ObjectGroupLayout>();
            m_LayoutTypes = types.ExtractAsValues(GetLayoutTypeFromType);
        }
    }

    public override void OnInspectorGUI()
    {
        var property = serializedObject.FindProperty("m_ObjectLayout");
        var layout = property.objectReferenceValue as ObjectGroupLayout;
        if (layout == null)
        {
            layout = CreateInstance<StaticGroupLayout>();
            property.objectReferenceValue = layout;
        }

        var type = EditorHelper.EnumPopup("Layout Type", layout.Type);
        if (type != layout.Type)
        {
            property.objectReferenceValue = CreateInstance(m_LayoutTypes[type]);
        }
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(property);
        serializedObject.ApplyModifiedProperties();

        using (new GuiEnabledScope(EditorApplication.isPlaying))
        {
            if (EditorGUI.EndChangeCheck() == true)
            {
                if (m_AutoUpdate == true)
                {
                    UpdatePositions();
                }
            }

            m_AutoUpdate = EditorGUILayout.Toggle("Auto Update", m_AutoUpdate);
            using (new GuiEnabledScope(GUI.enabled && !m_AutoUpdate))
            {
                var style = GUI.skin.button.Clone();
                if (m_AutoUpdate == true)
                {
                    style.normal = style.active;
                }

                if (GUILayout.Button("Update", style) == true)
                {
                    UpdatePositions();
                }
            }
        }
    }

    private void UpdatePositions() => (target as ObjectGroupModule).UpdatePosition();

    private static ObjectLayoutType GetLayoutTypeFromType(Type t)
    {
        return ((ObjectGroupLayout)CreateInstance(t)).Type;
    }
}
