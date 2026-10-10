using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaperone : MonoBehaviour
{
  //  public static Chaperone Instance;
    [SerializeField]
    private float size = 0.05f;
    private const int ARRAY_SIZE = 100;

    private List<Transform> m_TrackedObjects = new List<Transform>();
    [SerializeField]
    private List<Vector4> Positions = new List<Vector4>(ARRAY_SIZE);
    private string m_sGlobalParamName = "_ChaperoneInfluencers";
    private Material m_Material;

    private bool bInitialised;

    public void Initialise()
    {
        //Instance = this;
        Renderer renderer = GetComponentInChildren<Renderer>();
        m_Material = new Material(renderer.material);
        renderer.material = m_Material;


        for(int i =0; i < ARRAY_SIZE; i++)
        {
            Positions.Add(Vector4.zero);
        }
        SetMaterials();
        bInitialised = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(false == bInitialised)
        {
            return;
        }

        CheckSizes();
        UpdatePositions();

        if (Positions.Count > 0)
        {
            SetMaterials();
        }
    }

    private void SetMaterials()
    {
        m_Material.SetFloat("_ChaperoneArrayLength", Positions.Count);
        m_Material.SetVectorArray(m_sGlobalParamName, Positions);
    }

    private void CheckSizes()
    {
        if(Positions.Count != m_TrackedObjects.Count)
        {
            int Diff = m_TrackedObjects.Count - Positions.Count;
            //Add or remove list to make same size
            if (Diff > 0)
            {
                for (int i = 0; i < Diff; i++)
                {
                    Positions.Add(Vector4.zero);
                }
            }
            else
            {
                Diff = Mathf.Abs(Diff);
                if (Diff >= Positions.Count)
                {
                    Positions.Clear();
                }
                else
                {
                    for (int i = 0; i < Diff; i++)
                    {
                        Positions.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void UpdatePositions()
    {
        for(int i = 0; i < m_TrackedObjects.Count; i++)
        {
            Vector3 position = m_TrackedObjects[i].position;
            Vector4 pos = Positions[i];
            pos.x = position.x;
            pos.y = position.y;
            pos.z = position.z;
            Positions[i] = pos;
        }
    }

    public void TrackObject(Transform obj)
    {
        if (false == m_TrackedObjects.Contains(obj))
        {
            m_TrackedObjects.Add(obj);
        }
    }

    public void RemoveObject(Transform obj)
    {
        m_TrackedObjects.Remove(obj);
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < Positions.Count; i++)
    //    {
    //        Gizmos.DrawWireCube(Positions[i], Vector3.one * size);
    //    }
    //}
}
