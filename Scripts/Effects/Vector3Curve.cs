using System.Collections.Generic;
using UnityEngine;

public class Vector3Curve
{
    private AnimationCurve AnimationCurveX = new AnimationCurve();
    private AnimationCurve AnimationCurveY = new AnimationCurve();
    private AnimationCurve AnimationCurveZ = new AnimationCurve();
    private int m_ListAmount = 0;
    private float m_TotalTime = 0;
    private List<Vector3> m_Points = new List<Vector3>();

    public Vector3Curve()
    {
        Clear();
    }

    public void Clear()
    {
        AnimationCurveX = new AnimationCurve();
        AnimationCurveY = new AnimationCurve();
        AnimationCurveZ = new AnimationCurve();
        m_ListAmount = 0;
        m_TotalTime = 0;
        m_Points = new List<Vector3>();
    }

    public void Add(List<Vector3> items, float totalTime)
    {
        m_ListAmount += items.Count;
        for (int i = 0; i < items.Count; i++)
        {
            float key = ((float)i / (float)items.Count) * totalTime;
            key += m_TotalTime;
            AnimationCurveX.AddKey(key, items[i].x);
            AnimationCurveY.AddKey(key, items[i].y);
            AnimationCurveZ.AddKey(key, items[i].z);
        }
        m_TotalTime += totalTime;
        float distance = Vector3.Distance(items.First(), items.Last());
        m_Points.AddRange(items);
    }

    public Vector3 GetAtPercentageCount(float amount)
    {
        Vector3 vector3 = Vector3.zero;
        vector3.x = AnimationCurveX.Evaluate(amount * m_ListAmount);
        vector3.y = AnimationCurveY.Evaluate(amount * m_ListAmount);
        vector3.z = AnimationCurveZ.Evaluate(amount * m_ListAmount);
        return vector3;
    }

    public Vector3 GetAtPercentage(float amount)
    {
        Vector3 vector3 = Vector3.zero;
        vector3.x = AnimationCurveX.Evaluate(amount * m_TotalTime);
        vector3.y = AnimationCurveY.Evaluate(amount * m_TotalTime);
        vector3.z = AnimationCurveZ.Evaluate(amount * m_TotalTime);
        return vector3;
    }

    public Vector3 GetAtTime(float amount)
    {
        Vector3 vector3 = Vector3.zero;
        vector3.x = AnimationCurveX.Evaluate(amount);
        vector3.y = AnimationCurveY.Evaluate(amount);
        vector3.z = AnimationCurveZ.Evaluate(amount);
        return vector3;
    }

    public Vector3 InterpPercentage(float t)
    {
        int numSections = m_Points.Count-3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = m_Points[currPt];
        Vector3 b = m_Points[currPt + 1];
        Vector3 c = m_Points[currPt + 2];
        Vector3 d = m_Points[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }
}
