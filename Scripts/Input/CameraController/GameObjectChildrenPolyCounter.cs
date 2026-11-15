
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class GameObjectChildrenPolyCounter : MonoBehaviour
{
    [Header("Scan Settings")]
    public bool includeInactive = true;
    public bool refreshOnStart = true;

    [Header("GameObject Statistics")]
    [SerializeField] private long gameObjectCount;
    [SerializeField] private long activeGameObjectCount;

    [Header("Mesh Statistics")]
    [SerializeField] private long meshCount;
    [SerializeField] private long uniqueMeshCount;
    [SerializeField] private long vertexCount;
    [SerializeField] private long triangleCount;
    [SerializeField] private long polygonCount;
    [SerializeField] private long subMeshCount;

    [Header("Material Statistics")]
    [SerializeField] private long materialSlotCount;
    [SerializeField] private long uniqueMaterialCount;

    [Header("Additional Statistics")]
    [SerializeField] private long meshRendererCount;
    [SerializeField] private long skinnedMeshRendererCount;
    [SerializeField] private long colliderCount;
    [SerializeField] private long lightCount;
    [SerializeField] private long cameraCount;

    private void Start()
    {
        if (Application.isPlaying && refreshOnStart)
            RefreshStatistics();
    }

    // -----------------------------------------
    // REFRESH
    // -----------------------------------------

    [ContextMenu("Refresh Children Statistics")]
    public void RefreshStatistics()
    {
        ResetStatistics();

        HashSet<Mesh> uniqueMeshes =
            new HashSet<Mesh>();

        HashSet<Material> uniqueMaterials =
            new HashSet<Material>();

        // Scan children only, excluding this GameObject.
        for (int i = 0; i < transform.childCount; i++)
        {
            ScanHierarchy(
                transform.GetChild(i),
                uniqueMeshes,
                uniqueMaterials);
        }

        uniqueMeshCount = uniqueMeshes.Count;
        uniqueMaterialCount = uniqueMaterials.Count;

        // Triangle-equivalent polygon count.
        polygonCount = triangleCount;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorUtility.SetDirty(this);
#endif
    }

    // -----------------------------------------
    // SCAN HIERARCHY
    // -----------------------------------------

    private void ScanHierarchy(
        Transform current,
        HashSet<Mesh> uniqueMeshes,
        HashSet<Material> uniqueMaterials)
    {
        if (current == null)
            return;

        GameObject obj = current.gameObject;

        if (!includeInactive && !obj.activeInHierarchy)
            return;

        gameObjectCount++;

        if (obj.activeInHierarchy)
            activeGameObjectCount++;

        // Mesh Filters
        MeshFilter[] filters =
            obj.GetComponents<MeshFilter>();

        foreach (MeshFilter filter in filters)
        {
            if (filter.sharedMesh != null)
                AddMesh(filter.sharedMesh, uniqueMeshes);
        }

        // Mesh Renderers
        MeshRenderer[] renderers =
            obj.GetComponents<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            meshRendererCount++;

            AddMaterials(
                renderer,
                uniqueMaterials);
        }

        // Skinned Mesh Renderers
        SkinnedMeshRenderer[] skinned =
            obj.GetComponents<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer renderer in skinned)
        {
            skinnedMeshRendererCount++;

            if (renderer.sharedMesh != null)
                AddMesh(renderer.sharedMesh, uniqueMeshes);

            AddMaterials(
                renderer,
                uniqueMaterials);
        }

        colliderCount +=
            obj.GetComponents<Collider>().Length;

        lightCount +=
            obj.GetComponents<Light>().Length;

        cameraCount +=
            obj.GetComponents<Camera>().Length;

        // Recursively scan all descendants.
        for (int i = 0; i < current.childCount; i++)
        {
            ScanHierarchy(
                current.GetChild(i),
                uniqueMeshes,
                uniqueMaterials);
        }
    }

    // -----------------------------------------
    // MESH STATISTICS
    // -----------------------------------------

    private void AddMesh(
        Mesh mesh,
        HashSet<Mesh> uniqueMeshes)
    {
        meshCount++;
        uniqueMeshes.Add(mesh);

        vertexCount += mesh.vertexCount;
        subMeshCount += mesh.subMeshCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            long indices = (long)mesh.GetIndexCount(i);

            switch (mesh.GetTopology(i))
            {
                case MeshTopology.Triangles:
                    triangleCount += indices / 3;
                    break;

                case MeshTopology.Quads:
                    triangleCount +=
                        (indices / 4) * 2;
                    break;
            }
        }
    }

    // -----------------------------------------
    // MATERIAL STATISTICS
    // -----------------------------------------

    private void AddMaterials(
        Renderer renderer,
        HashSet<Material> uniqueMaterials)
    {
        Material[] materials =
            renderer.sharedMaterials;

        materialSlotCount += materials.Length;

        foreach (Material material in materials)
        {
            if (material != null)
                uniqueMaterials.Add(material);
        }
    }

    // -----------------------------------------
    // RESET
    // -----------------------------------------

    private void ResetStatistics()
    {
        gameObjectCount = 0;
        activeGameObjectCount = 0;

        meshCount = 0;
        uniqueMeshCount = 0;
        vertexCount = 0;
        triangleCount = 0;
        polygonCount = 0;
        subMeshCount = 0;

        materialSlotCount = 0;
        uniqueMaterialCount = 0;

        meshRendererCount = 0;
        skinnedMeshRendererCount = 0;
        colliderCount = 0;
        lightCount = 0;
        cameraCount = 0;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(GameObjectChildrenPolyCounter))]
public class GameObjectChildrenPolyCounterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(
            "Scan Settings",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("includeInactive"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("refreshOnStart"));

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);

        if (GUILayout.Button(
            "REFRESH CHILDREN STATISTICS",
            GUILayout.Height(45)))
        {
            foreach (Object obj in targets)
            {
                GameObjectChildrenPolyCounter counter =
                    (GameObjectChildrenPolyCounter)obj;

                counter.RefreshStatistics();
            }

            serializedObject.Update();
            Repaint();
        }

        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField(
            "GameObject Statistics",
            EditorStyles.boldLabel);

        DrawReadOnlyNumber(
            "GameObjects",
            "gameObjectCount");

        DrawReadOnlyNumber(
            "Active GameObjects",
            "activeGameObjectCount");

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Mesh Statistics",
            EditorStyles.boldLabel);

        DrawReadOnlyNumber(
            "Mesh Instances",
            "meshCount");

        DrawReadOnlyNumber(
            "Unique Meshes",
            "uniqueMeshCount");

        DrawReadOnlyNumber(
            "Vertices",
            "vertexCount");

        DrawReadOnlyNumber(
            "Triangles",
            "triangleCount");

        DrawReadOnlyNumber(
            "Polygons",
            "polygonCount");

        DrawReadOnlyNumber(
            "Submeshes",
            "subMeshCount");

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Material Statistics",
            EditorStyles.boldLabel);

        DrawReadOnlyNumber(
            "Material Slots",
            "materialSlotCount");

        DrawReadOnlyNumber(
            "Unique Materials",
            "uniqueMaterialCount");

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Additional Statistics",
            EditorStyles.boldLabel);

        DrawReadOnlyNumber(
            "Mesh Renderers",
            "meshRendererCount");

        DrawReadOnlyNumber(
            "Skinned Mesh Renderers",
            "skinnedMeshRendererCount");

        DrawReadOnlyNumber(
            "Colliders",
            "colliderCount");

        DrawReadOnlyNumber(
            "Lights",
            "lightCount");

        DrawReadOnlyNumber(
            "Cameras",
            "cameraCount");

        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Counts children and all descendants only.\n" +
            "Parent GameObject is excluded.\n" +
            "Statistics are read-only.\n" +
            "Numbers include thousands separators.\n" +
            "Works in Edit Mode and Play Mode.",
            MessageType.Info);
    }

    // -----------------------------------------
    // READ-ONLY FORMATTED NUMBER
    // -----------------------------------------

    private void DrawReadOnlyNumber(
        string label,
        string propertyName)
    {
        SerializedProperty property =
            serializedObject.FindProperty(propertyName);

        if (property == null)
            return;

        long value = property.longValue;

        string formatted = value.ToString(
            "N0",
            CultureInfo.GetCultureInfo("en-GB"));

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField(
                label,
                formatted);
        }
    }
}

#endif
