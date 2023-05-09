using System.Collections.Generic;
using UnityEngine;

public enum RaycastType
{
    Straight,
    Arc
}

public class ControllerRaycaster : MonoBehaviour
{
    private const float STRAIGHT_RAYCAST_LENGTH = 1000f;
    private const float SphereCastRadius = 0.01f;

    private const float HeightChange = 25f;
    private const int ArcSegments = 64;
    private const float ArcStep = 0.03f;

    public int PointCount => m_Points.Count;

    public RaycasterHit Pretouch { get; private set; }
    public RaycasterHit Hit { get; private set; }
    public Ray Ray;

    private readonly List<Vector3> m_Points;
    private readonly Collider[] m_SphereHits;
    private RaycastHit m_Hit;

    public ControllerRaycaster()
    {
        m_SphereHits = new Collider[4];
        m_Points = new List<Vector3>();
        Pretouch = new RaycasterHit();
        Hit = new RaycasterHit();
    }

    public List<Vector3> GetPoints() => m_Points;
    public void SetPoint(int index, Vector3 p) => m_Points[index] = p;
    public Vector3 GetPoint(int index) => m_Points[index];
    public Vector3 GetEndPoint()
    {
        return m_Points[m_Points.Count - 1];
    }

    public Vector3 HitNormal()
    {
        if(null != m_Hit.collider)
        {
            return m_Hit.normal;
        }
        else
        {
            //Get normal from reverse of laser effect
            if(m_Points.Count > 1)
            {
                return (m_Points[m_Points.Count - 2] - m_Points[m_Points.Count - 1]).normalized;
            }
        }

        return Vector3.forward;
    }

    public bool Raycast(int layerMask, RaycastType rayType)
    {
        ClearHitInfo();

        float scale = CameraControllerVR.Instance.Scale;
        float distance = GetRayDistance(rayType) * scale;
        //float offsetDistance = GetRayDistance(RaycastType.Straight) * scale;
        var ray = new Ray(Ray.origin, Ray.direction);

        m_Points.Clear();
        m_Points.Add(ray.origin);

        if (rayType == RaycastType.Arc)
        {
            int arcMultiplier = CameraControllerVR.Instance.IsInCeiling() ? 2 : 1;
            int segments = ArcSegments * arcMultiplier;
            for (int i = 0; i < segments; i++)
            {
                if (Raycast(ray, distance, layerMask) == true)
                {
                    return true;
                }

                ray.direction = Vector3.RotateTowards(ray.direction, Vector3.down, ArcStep, 0f);
                ray.origin = m_Points.Last();
            }
        }
        else
        {
            if (Raycast(ray, distance, layerMask) == true)
            {
                return true;
            }
        }

        return false;
    }

    private bool Raycast(Ray ray, float distance, int layerMask)
    {
        if (Physics.Raycast(ray, out m_Hit, distance, layerMask) == true)
        {
            Hit.Collider = m_Hit.collider;
            Hit.Normal = m_Hit.normal;
            Hit.Point = m_Hit.point;

            Debug.DrawLine(ray.origin, Hit.Point, Color.cyan);
            m_Points.Add(Hit.Point);
            return true;
        }
        else
        {
            var target = ray.origin + (ray.direction * distance);
            Debug.DrawLine(ray.origin, target, Color.cyan);
            m_Points.Add(target);
            return false;
        }
    }

    public bool Spherecast(int layerMask)
    {
        ClearHitInfo();

        float radius = SphereCastRadius * CameraControllerVR.Instance.Scale;
        int count = Physics.OverlapSphereNonAlloc(Ray.origin, radius, m_SphereHits, layerMask);
        if (count > 0)
        {
            Hit.Collider = m_SphereHits[0];
            Hit.Point = Hit.Collider.transform.position;
            Hit.Normal = Hit.Point - Ray.origin;
            return true;
        }
        return false;
    }

    private void ClearHitInfo()
    {
        Pretouch.Clear();
        Hit.Clear();
    }

    public static float GetRayDistance(RaycastType type)
    {
        if (type == RaycastType.Arc)
        {
            float length = 3f;
            float scale = CameraControllerVR.Instance.Scale;
            return length * scale;
        }
        return STRAIGHT_RAYCAST_LENGTH;
    }

    public class RaycasterHit
    {
        public Collider Collider;
        public Vector3 Normal;
        public Vector3 Point;

        public void Clear()
        {
            Collider = null;
            Normal = Vector3.up;
            Point = Vector3.zero;
        }

        public void CopyFrom(RaycasterHit hit)
        {
            Collider = hit.Collider;
            Normal = hit.Normal;
            Point = hit.Point;
        }
    }
}