using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@12.1/manual/containers/create-custom-renderer-feature-1.html

//https://forum.unity.com/threads/graphics-drawmeshinstanced-not-working-on-urp.807915/
public class DrawInstancedMeshs : MonoBehaviour // need to be MonoBehaviour for the update
{
    private List<InstancedGroup> m_Instances = new List<InstancedGroup>();
    private Material m_Material;
    private Mesh[] m_Mesh;
    private const int MAX_MESH = 1023;

    public void Init(GameObject obj)
    {
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        m_Mesh = new Mesh[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            m_Mesh[i] = meshFilters[i].sharedMesh;
        }

        m_Material = obj.GetComponentInChildren<Renderer>().material;

        foreach (Transform child in transform)
        {
            m_Instances.Add(new InstancedGroup(child));
        }
    }
    public void ClearAll() => m_Instances.Clear();
    public void Add(Transform trans) => m_Instances.Add(new InstancedGroup(trans));

    public void Add(List<Transform> trans)
    {
        var transList = trans.SplitList(MAX_MESH);
        for (int i = 0; i < transList.Count; i++)
        {
            m_Instances.Add(new InstancedGroup(transList[i]));
        }
    }

    public void Add(List<Vector3> pos, List<Quaternion> rot)
    {
        var positions = pos.SplitList(MAX_MESH);
        var rotations = rot.SplitList(MAX_MESH);
        for(int i = 0; i < positions.Count; i++)
        {
            m_Instances.Add(new InstancedGroup(positions[i], rotations[i]));
        }
    }

    public void Add(List<Vector3> pos, List<Quaternion> rot, Vector3 scale)
    {
        var positions = pos.SplitList(MAX_MESH);
        var rotations = rot.SplitList(MAX_MESH);
        for (int i = 0; i < positions.Count; i++)
        {
            m_Instances.Add(new InstancedGroup(positions[i], rotations[i], scale));
        }
    }

    private void Update()
    {
        foreach(var instance in m_Instances)
        {
            var meshToDraw = m_Mesh[0];
            if (meshToDraw != null)
            {
                Graphics.DrawMeshInstanced(meshToDraw, 0, m_Material, instance.Matrices, instance.Matrices.Length);
            }
        }
    }

    private class InstancedGroup
    {
        public Matrix4x4[] Matrices { get; private set; }

        public InstancedGroup(Transform root)
        {
            var children = root.GetDirectChildren();
            Matrices = new Matrix4x4[children.Count];

            for (int i = 0; i < children.Count; ++i)
            {
                var pos = children[i].position;
                var rot = children[i].rotation;

                Matrices[i] = Matrix4x4.TRS(pos, rot, Vector3.one);
            }
        }

        public InstancedGroup(List<Transform> trans)
        {
            Matrices = new Matrix4x4[trans.Count];

            for (int i = 0; i < trans.Count; ++i)
            {
                var pos = trans[i].position;
                var rot = trans[i].rotation;

                Matrices[i] = Matrix4x4.TRS(pos, rot, Vector3.one);
            }
        }

        public InstancedGroup(List<Vector3> position, List<Quaternion> rotation)
        {
            Matrices = new Matrix4x4[position.Count];

            for (int i = 0; i < position.Count; ++i)
            {
                var pos = position[i];
                var rot = rotation[i];
                Matrices[i] = Matrix4x4.TRS(pos, rot, Vector3.one);
            }
        }

        public InstancedGroup(List<Vector3> position, List<Quaternion> rotation, Vector3 scale)
        {
            Matrices = new Matrix4x4[position.Count];

            for (int i = 0; i < position.Count; ++i)
            {
                var pos = position[i];
                var rot = rotation[i];
                Matrices[i] = Matrix4x4.TRS(pos, rot, scale);
            }
        }

    }
}