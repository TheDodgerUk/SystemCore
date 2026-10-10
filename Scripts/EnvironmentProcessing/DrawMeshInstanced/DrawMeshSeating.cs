using System.Collections.Generic;
using UnityEngine;

public class DrawMeshSeating : MonoBehaviour
{
    private List<InstancedGroup> m_Instances = new List<InstancedGroup>();
    private Material m_Material;
    private Mesh[] m_Mesh;

    public float m_fLODDistance = 18.0f;
    private float SqrDistance = 0f;

    private Transform m_CameraTransform;

    public void Init(GameObject obj, Material material)
    {
        SqrDistance = m_fLODDistance * m_fLODDistance;

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        m_Mesh = new Mesh[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            m_Mesh[i] = meshFilters[i].sharedMesh;
        }

        m_Material = material;

        foreach (Transform child in transform)
        {
            m_Instances.Add(new InstancedGroup(child));
        }
    }

    private void Update()
    {
        if (Camera.main != null)
        {
            m_CameraTransform = Camera.main.transform;
        }

        if (CameraControllerVR.Instance != null)
        {
            m_CameraTransform = CameraControllerVR.Instance.CameraTransform;
        }

        foreach(var instance in m_Instances)
        {
            var meshToDraw = GetMeshToRender(m_CameraTransform, instance);
            if (meshToDraw != null)
            {
                Graphics.DrawMeshInstanced(meshToDraw, 0, m_Material, instance.Matrices, instance.Matrices.Length);
            }
        }
    }

    private Mesh GetMeshToRender(Transform cameraTransform, InstancedGroup instance)
    {
        var direction = instance.Centre - cameraTransform.position;
        float sqrMag = direction.sqrMagnitude;
        if (sqrMag > SqrDistance)
        {
            if (Vector3.Dot(cameraTransform.forward, direction.normalized) > 0.25f)
            {
                if (sqrMag > SqrDistance * 2)
                {
                    return ReturnCorrectMesh(2);
                }
                else
                {
                    return ReturnCorrectMesh(1);
                }
            }
        }
        else
        {
            return ReturnCorrectMesh(0);
        }
        return null;
    }

    private Mesh ReturnCorrectMesh(int meshIndex)
    {
        if((m_Mesh.Length-1) >= meshIndex)
        {
            return m_Mesh[meshIndex];
        }
        else
        {
            return m_Mesh[m_Mesh.Length - 1];
        }
    }

    private class InstancedGroup
    {
        public Matrix4x4[] Matrices;
        public Vector3 Centre;

        public InstancedGroup(Transform root)
        {
            var children = root.GetDirectChildren();
            Matrices = new Matrix4x4[children.Count];
            Centre = Vector3.zero;

            var seatOffset = new Vector3(0f, -0.06f, 0.0f);
            for (int i = 0; i < children.Count; ++i)
            {
                var pos = children[i].position;
                var rot = children[i].rotation;

                Centre += pos;

                pos += rot * seatOffset;
                Matrices[i] = Matrix4x4.TRS(pos, rot, Vector3.one);
            }

            Centre /= children.Count;
        }
    }
}