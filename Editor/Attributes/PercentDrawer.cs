using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PercentAttribute))]
public class PercentDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var percentRect = new Rect(position) { width = 16 };
        position.width -= percentRect.width;
        percentRect.x = position.xMax;

        float percent = property.floatValue * 100f;
        percent = EditorGUI.Slider(position, label, percent, 0, 100);
        property.floatValue = percent / 100f;

        EditorGUI.LabelField(percentRect, "%");
    }
}
