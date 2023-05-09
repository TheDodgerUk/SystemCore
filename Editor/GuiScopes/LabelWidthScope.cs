using UnityEditor;
using UnityEngine;

public class LabelWidthScope : GUI.Scope
{
    private readonly float m_Width;

    public LabelWidthScope(float newWidth)
    {
        m_Width = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = newWidth;
    }

    protected override void CloseScope()
    {
        EditorGUIUtility.labelWidth = m_Width;
    }
}