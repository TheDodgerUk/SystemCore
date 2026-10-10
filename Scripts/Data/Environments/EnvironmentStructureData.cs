using System.Collections.Generic;
using UnityEngine;
using System;

public class EnvironmentStructurePoint
{
    public Vector3 point;
    public SupportBar bar;

    public EnvironmentStructurePoint(Vector3 point, SupportBar support)
    {
        this.point = point;
        bar = support;
    }
}

[System.Serializable]
public class EnvironmentStructureData
{
    public string SlingModelGUID;
    public string SlingModelPrefab;
    public string SlingModelBundle;
    public readonly List<SupportBar> Bars;
    public readonly List<Vector3> Points;

    public void LoadPrefab(MonoBehaviour host, Action<GameObject> callback)
    {
        Core.Assets.LoadAsset(host, SlingModelPrefab, SlingModelBundle, callback, 0);
    }

    public List<EnvironmentStructurePoint> GetClosestSupportPoint(Vector3 position)
    {
        SupportBar primary = null;
        Vector3 closestPoint = ClosestPoint(position, false, Vector3.forward, ref primary);

        //Get alternate point in oposite direction
        SupportBar secondary = null;
        Vector3 direction = position - closestPoint;
        Vector3 altPoint = ClosestPoint(position, true, direction, ref secondary);

        return new List<EnvironmentStructurePoint>() { new EnvironmentStructurePoint(closestPoint, null), new EnvironmentStructurePoint(altPoint, null) };
    }

    private Vector3 ClosestPoint(Vector3 position, bool useDirection, Vector3 direction, ref SupportBar support)
    {
        Vector3 closestPoint = Vector3.zero;
        float dist = Mathf.Infinity;
        for (int i = 0; i < Bars.Count; i++)
        {
            SupportBar bar = Bars[i];
            Vector3 point = ProjectPointOnLineSegment(bar.Start, bar.End, position);
            if (dist > (position - point).magnitude)
            {
                Vector3 dir = position - point;
                if (false == useDirection ||
                    true == useDirection && Vector3.Dot(dir, direction) < 0f)
                {
                    closestPoint = point;
                    support = bar;
                    dist = (position - point).magnitude;
                }
            }
        }

        return closestPoint;
    }

    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {
        //get vector from point on line to point in space
        Vector3 linePointToPoint = point - linePoint;
        float t = Vector3.Dot(linePointToPoint, lineVec);
        return linePoint + lineVec * t;
    }

    public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {
        Vector3 vector = linePoint2 - linePoint1;
        Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);
        int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);
        //The projected point is on the line segment
        if (side == 0)
        {
            return projectedPoint;
        }

        if (side == 1)
        {
            return linePoint1;
        }

        if (side == 2)
        {
            return linePoint2;
        }

        //output is invalid
        return Vector3.zero;
    }

    public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {
        Vector3 lineVec = linePoint2 - linePoint1;
        Vector3 pointVec = point - linePoint1;
        float dot = Vector3.Dot(pointVec, lineVec);
        //point is on side of linePoint2, compared to linePoint1
        if (dot > 0)
        {
            //point is on the line segment
            if (pointVec.magnitude <= lineVec.magnitude)
            {
                return 0;
            }
            //point is not on the line segment and it is on the side of linePoint2
            else
            {
                return 2;
            }
        }

        //Point is not on side of linePoint2, compared to linePoint1.
        //Point is not on the line segment and it is on the side of linePoint1.
        else
        {
            return 1;
        }
    }
}

[System.Serializable]
public class SupportBar
{
    public readonly Vector3 Start;
    public readonly Vector3 End;
}
