using UnityEngine;
using System.Collections.Generic;

public enum ProcMeshType
{
    Tube,
    GraphUp,
    GraphDown,
    Custom
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    public int SegmentCount { get { return m_iSegmentCount; } set { m_iSegmentCount = value; } }
    public int TubeSegments { get { return m_iTubeSegments; } set { m_iTubeSegments = value; } }
    public float Radius { get { return m_fRadius; } set { m_fRadius = value; } }

    [SerializeField]
    private bool m_bDrawGizmos = false;

    [SerializeField]
    private ProcMeshType m_MeshBuildType = ProcMeshType.Custom;

    [SerializeField]
    private int m_iSegmentCount = 8;
    [SerializeField]
    private int m_iTubeSegments = 5;
    [SerializeField]
    private float m_fRadius = 0.5f;
    [SerializeField]
    private Quaternion m_Rotation = Quaternion.identity;

    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector3> m_Normals = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();

    private List<Vector2> m_UVs = new List<Vector2>();
    private List<Color> m_Colors = new List<Color>();

    private MeshRenderer m_MeshRenderer;
    private MeshFilter m_MeshFilter;
    private Mesh m_Mesh = null;

    private Vector3[] m_Positions;
    private float m_fMaxHeight;
    private bool m_bUVsSet = false;

    // Use this for initialization
    public void Init(Vector3[] positions, Material material, float fMaxHeight, ProcMeshType meshType = ProcMeshType.Tube)
    {
        m_MeshRenderer = gameObject.ForceComponent<MeshRenderer>();

        m_MeshRenderer.receiveShadows = false;
        m_MeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        m_MeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        m_MeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        m_MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        m_MeshFilter = gameObject.ForceComponent<MeshFilter>();

        m_Mesh = new Mesh();

        m_Positions = positions;
        m_MeshRenderer.sharedMaterial = material;

        m_Mesh.MarkDynamic();
        m_MeshFilter.sharedMesh = m_Mesh;

        m_fMaxHeight = fMaxHeight;

        m_MeshBuildType = meshType;
    }

    public void SetColors(Color[] colors)
    {
        m_Colors = colors.ToList();
    }

    public void SetUVs(Vector2[] uvs)
    {
        m_UVs = uvs.ToList();
        m_bUVsSet = true;
    }

    public void UpdateMesh()
    {
        m_Mesh.SetVertices(m_Vertices);
        m_Mesh.SetTriangles(m_Triangles, 0);
        m_Mesh.SetUVs(0, m_UVs);
        m_Mesh.SetNormals(m_Normals);

        if (m_Colors.Count > 0)
        {
            m_Mesh.SetColors(m_Colors);
        }
    }

    public void RecalculateMesh()
    {
        m_Vertices.Clear();
        m_Triangles.Clear();
        if (m_bUVsSet == false)
        {
            m_UVs.Clear();
        }
        m_Normals.Clear();

        int iCount = m_Positions.Length;

        switch (m_MeshBuildType)
        {
            case ProcMeshType.Tube:
                for (int i = 0; i < iCount; i++)
                {
                    float v = i / 2f;
                    BuildRing(m_Positions[i], m_iSegmentCount, v, i > 0, m_Rotation);
                }
                break;

            case ProcMeshType.GraphUp:
                float angle = 0f;
                for (int i = 0; i < iCount; i++)
                {
                    BuildQuad(m_Positions[i], m_fRadius, i > 0, angle);
                }
                break;

            case ProcMeshType.GraphDown:
                break;

            case ProcMeshType.Custom:
                for (int i = 0; i < iCount; i++)
                {
                    bool first = (i % 72 == 0);
                    BuildCustomQuad(m_Positions[i], 72, i > 72, first);
                }
                break;
        }
    }

    private void BuildQuad(Vector3 centre, float fWidth, bool bBuildTriangles, float rotation)
    {
        //Add current position
        m_Vertices.Add(centre + new Vector3(0f, 0f, fWidth));
        m_Normals.Add(Vector3.up);
        m_UVs.Add(new Vector2(0.5f, 1.0f));
        AddTrianglesForQuad(0, bBuildTriangles);

        //Add position behind to make 3D
        m_Vertices.Add(centre);
        m_Normals.Add(Vector3.forward);
        m_UVs.Add(new Vector2(0.5f, .95f));
        AddTrianglesForQuad(1, bBuildTriangles);

        //end of white stripe
        float minusHeight = Mathf.Min(centre.y, m_fMaxHeight * 0.05f);
        m_Vertices.Add(centre + new Vector3(0f, -minusHeight, 0f));
        m_Normals.Add(Vector3.forward);
        m_UVs.Add(new Vector2(0.5f, .9f));
        AddTrianglesForQuad(2, bBuildTriangles);

        //Add front bottom
        m_Vertices.Add(new Vector3(centre.x, 0f, 0f));
        m_Normals.Add(Vector3.forward);
        AddTrianglesForQuad(3, bBuildTriangles);
        m_UVs.Add(new Vector2(0.5f, .1f));

        //Add back bottom
        m_Vertices.Add(new Vector3(centre.x, 0f, fWidth));
        m_Normals.Add(Vector3.down);
        m_UVs.Add(new Vector2(0.5f, 0f));
        AddTrianglesForQuad(4, bBuildTriangles);
    }

    private void AddTrianglesForQuad(int iIndex, bool bBuildTriangles)
    {
        int baseIndex = m_Vertices.Count - 1;
        int vertsPerRow = 5;

        if (true == bBuildTriangles && iIndex > 0)
        {
            int index0 = baseIndex;
            int index1 = baseIndex - 1;
            int index2 = baseIndex - vertsPerRow;
            int index3 = baseIndex - vertsPerRow - 1;

            m_Triangles.Add(index0);
            m_Triangles.Add(index2);
            m_Triangles.Add(index1);

            m_Triangles.Add(index2);
            m_Triangles.Add(index3);
            m_Triangles.Add(index1);
        }
    }

    private void BuildCustomQuad(Vector3 position, int iIndexOffset, bool bBuildTriangles, bool isFirst)
    {
        m_Vertices.Add(position);

        int baseIndex = m_Vertices.Count - 1;
        int vertsPerRow = iIndexOffset;

        int index0 = baseIndex;
        int index1 = baseIndex - 1;
        int index2 = baseIndex - vertsPerRow;
        int index3 = baseIndex - vertsPerRow - 1;

        if (isFirst)
        {
            index3 = index1;
            index1 += vertsPerRow;
        }
        if (true == bBuildTriangles)
        {
            m_Triangles.Add(index0);
            m_Triangles.Add(index2);
            m_Triangles.Add(index1);

            m_Triangles.Add(index2);
            m_Triangles.Add(index3);
            m_Triangles.Add(index1);
        }
    }

    private void BuildRing(Vector3 centre, int iSegmentCount, float v, bool bBuildTriangles, Quaternion rotation)
    {
        float radius = m_fRadius;
        float angleInc = (Mathf.PI * 2.0f) / iSegmentCount;
        for (int i = 0; i <= iSegmentCount; i++)
        {
            float angle = angleInc * i;

            Vector3 unitPosition = Vector3.zero;
            unitPosition.z = Mathf.Cos(angle);
            unitPosition.y = Mathf.Sin(angle);

            unitPosition = rotation * unitPosition;

            m_Vertices.Add(centre + unitPosition * radius);
            m_UVs.Add(new Vector2((float)i / iSegmentCount, v));
            m_Normals.Add(unitPosition);

            if (i > 0 && true == bBuildTriangles)
            {
                int baseIndex = m_Vertices.Count - 1;
                int vertsPerRow = iSegmentCount + 1;

                int index0 = baseIndex;
                int index1 = baseIndex - 1;
                int index2 = baseIndex - vertsPerRow;
                int index3 = baseIndex - vertsPerRow - 1;

                m_Triangles.Add(index0);
                m_Triangles.Add(index2);
                m_Triangles.Add(index1);

                m_Triangles.Add(index2);
                m_Triangles.Add(index3);
                m_Triangles.Add(index1);
            }
        }
    }

    private void AddTriangle(int index0, int index1, int index2)
    {
        m_Triangles.Add(index0);
        m_Triangles.Add(index1);
        m_Triangles.Add(index2);
    }

    private void AddQuadVertex(Vector3 m_Position, float fWidth, float fHeight)
    {
        m_Vertices.Add(new Vector3(0f, 0f, 0f) + m_Position);
        m_Vertices.Add(new Vector3(0f, 0f, fHeight) + m_Position);
        m_Vertices.Add(new Vector3(fWidth, 0f, fHeight) + m_Position);
        m_Vertices.Add(new Vector3(fWidth, 0f, 0f) + m_Position);
    }

    private void AddQuadTriangles(int iTriangleIndex)
    {
        m_Triangles.Add(iTriangleIndex - 4);
        m_Triangles.Add(iTriangleIndex - 3);
        m_Triangles.Add(iTriangleIndex - 2);

        m_Triangles.Add(iTriangleIndex - 2);
        m_Triangles.Add(iTriangleIndex - 1);
        m_Triangles.Add(iTriangleIndex - 4);
    }

    private void AddQuadUVs()
    {
        m_UVs.Add(Vector2.zero);
        m_UVs.Add(new Vector2(0f, 1f));
        m_UVs.Add(new Vector2(1f, 1f));
        m_UVs.Add(new Vector2(1f, 0f));
    }

    private void OnDrawGizmos()
    {
        if (false == m_bDrawGizmos)
        {
            return;
        }

        Gizmos.color = Color.blue;

        for (int i = 0; i < m_Vertices.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position + m_Vertices[i], 0.1f);
        }
    }
}
