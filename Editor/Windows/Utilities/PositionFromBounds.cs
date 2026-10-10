using UnityEditor;
using UnityEngine;

public class PositionFromBounds : EditorWindow
{
    [MenuItem("Window/Utility/Position from Bounds")]
    public static void ShowWindow() => GetWindow<PositionFromBounds>("Bounds Position");

    private Transform m_Transform;
    private MeshFilter m_MeshFilter;
    private Renderer m_Renderer;
    private Vector3 m_Anchor;
    private bool m_IsEnabled;

    private void Awake() => FindObject();

    private void OnDestroy() => SceneView.onSceneGUIDelegate -= OnSceneGUI;

    private void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Position from Bounds", EditorStyles.boldLabel);

        EditorHelper.ObjectField("Transform", m_Transform);
        m_Renderer = EditorHelper.ObjectField("Renderer", m_Renderer);
        m_IsEnabled = EditorGUILayout.Toggle("Enabled", m_IsEnabled);

        var names = new[] { "x", "y", "z" };
        for (int i = 0; i < names.Length; ++i)
        {
            m_Anchor[i] = EditorGUILayout.Slider(names[i], m_Anchor[i], 0, 1);
        }

        if (m_IsEnabled == true && m_Transform != null && m_Renderer != null)
        {
            Undo.RecordObject(m_Transform, "Move Transform on Bounds");
            m_Transform.localPosition = GetAnchoredSize();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (m_Transform != null && m_Renderer != null && m_MeshFilter != null)
        {
            var bounds = m_Renderer.bounds;
            var pos = bounds.center + GetAnchoredSize();
            float maxSize = bounds.size.MinComponent() * 0.25f;
            Handles.DrawWireCube(bounds.center, bounds.size);
            Handles.SphereHandleCap(0, pos, Quaternion.identity, HandleSize(pos, 0.5f, maxSize), EventType.Repaint);

            var colours = new[] { Color.red, Color.green, Color.blue };
            for (int i = 0; i < colours.Length; ++i)
            {
                var colour = 0.5f * colours[i] + Color.white * 0.5f;
                DrawBoundsAxis(pos, bounds, i, maxSize, colour);
            }
        }
    }

    private void OnSelectionChange()
    {
        FindObject();
        Repaint();
    }

    private void FindObject()
    {
        m_IsEnabled = false;
        m_Anchor = new Vector3(0.5f, 0.5f, 0.5f);
        m_Transform = Selection.activeTransform;
        m_Renderer = m_Transform?.GetComponentInChildren<Renderer>();
        m_MeshFilter = m_Renderer?.GetComponent<MeshFilter>();
    }

    private Vector3 GetAnchoredSize() => m_Renderer.bounds.size.Multiply(m_Anchor - (Vector3.one * 0.5f));

    private Vector3 GetSceneGUIAnchor() => new Vector3(1f - m_Anchor.z, m_Anchor.y, 1f - m_Anchor.x);
    private Vector3 GetLocalAnchor() => new Vector3(1f - m_Anchor.x, 1f - m_Anchor.y, m_Anchor.z);

    private static void DrawBoundsAxis(Vector3 pos, Bounds bounds, int index, float maxSize, Color colour)
    {
        var normal = Vector3.zero;
        var min = pos;
        var max = pos;

        min[index] = bounds.min[index];
        max[index] = bounds.max[index];
        normal[index] = 1f;

        colour.a = 1f;
        Handles.color = colour;
        Handles.DrawLine(min, max);
        Handles.ArrowHandleCap(0, pos, Quaternion.LookRotation(normal), HandleSize(pos), EventType.Repaint);
        Handles.DrawWireDisc(min, normal, HandleSize(min, 0.2f, maxSize));
        Handles.DrawWireDisc(max, -normal, HandleSize(max, 0.2f, maxSize));
        Handles.color = Color.white;
    }

    private static float HandleSize(Vector3 position, float scale = 1f, float maxSize = Mathf.Infinity)
    {
        return (HandleUtility.GetHandleSize(position) * scale).Min(maxSize);
    }
}
