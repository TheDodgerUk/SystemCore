using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CurveLineRenderer : MonoBehaviour
{
    public class SplineData
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 FacingVector = Vector3.forward;
        public float Time = 1.0f;
        public float Wait = 0.0f;
    }

    public bool m_RunUpdate = true; 
    public List<SplineData> m_RawControlPoints;
    public int SegmentCount => 50;


    private LineRenderer m_LinerendererPositions;
    private LineRenderer m_LinerendererRotations;

    
    public enum BezierType
    {
        Normal,
        CatmullRom,
    }

    public BezierType m_BezierType = CurveLineRenderer.BezierType.Normal;


    void Start()
    {
        Material mat = Resources.Load<Material>("Materials/black");
        m_LinerendererPositions = this.gameObject.ForceComponent<LineRenderer>();
        m_LinerendererPositions.material = mat;
        m_LinerendererPositions.material = mat;
        m_LinerendererPositions.sortingLayerID = 0;
        m_LinerendererPositions.startWidth = 0.1f;
        m_LinerendererPositions.endWidth = 0.1f;


        GameObject holder = new GameObject("Holder");
        holder.transform.SetParent(m_LinerendererPositions.transform);
        m_LinerendererRotations = holder.gameObject.AddComponent<LineRenderer>();
    }

    void Update()
    {
        if(m_LinerendererPositions != null && m_RunUpdate == true)
        {
            DrawCurve();
        }
    }

    private void OnEnable()
    {
        if (m_LinerendererPositions != null)
        {
            m_LinerendererPositions.enabled = true;
            m_LinerendererRotations.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (m_LinerendererPositions != null)
        {
            m_LinerendererPositions.enabled = false;
            m_LinerendererRotations.enabled = false;
        }
    }

    public List<SplineData> RawData() => m_RawControlPoints;
    public List<Vector3> GetPositionPoints()
    {
        Vector3[] points = new Vector3[m_LinerendererPositions.positionCount];
        m_LinerendererPositions.GetPositions(points);
        return points.ToList();
    }

    public List<Vector3> GetFacingVectorPoints()
    {
        Vector3[] points = new Vector3[m_LinerendererPositions.positionCount];
        m_LinerendererRotations.GetPositions(points);
        return points.ToList();
    }

    void DrawCurve()
    {
        m_LinerendererPositions.enabled = (m_RawControlPoints.Count >= 2);
        m_LinerendererRotations.enabled = false;

        if (m_RawControlPoints.Count > 0)
        {
            
            List<Vector3> positions = m_RawControlPoints.Extract(e => e.Position);
            var newPositions = MakeSmoothCurve(positions.ToArray(), SegmentCount);
            m_LinerendererPositions.positionCount = newPositions.Count;
            m_LinerendererPositions.SetPositions(newPositions.ToArray());

            List<Vector3> facing = m_RawControlPoints.Extract(e => e.FacingVector);
            var newFacing = MakeSmoothCurve(facing.ToArray(), SegmentCount);
            m_LinerendererRotations.positionCount = newPositions.Count;
            m_LinerendererRotations.SetPositions(newFacing.ToArray());
        }
    }

    /// <summary>
    /// Below was found on the internet
    /// </summary>
    private List<Vector3> MakeSmoothCurve(Vector3[] points, int smooth)
    {
        if (points.Length == 1) return points.Select(x => x).ToList();

        int length = points.Length-1;
        points = PathControlPointGenerator(points);
        List<Vector3> result = new List<Vector3>();
        int SmoothAmount = length * smooth;
        for (int i = 1; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(points, pm);
            result.Add(currPt);
        }
        return result;
    }

    private  Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        //create and store path points:
        suppliedPath = path;

        //populate calculate path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (vector3s[1] == vector3s[vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }

        return (vector3s);
    }

    private  Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

}
