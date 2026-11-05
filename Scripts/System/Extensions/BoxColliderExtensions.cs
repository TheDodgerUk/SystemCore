using UnityEngine;

public static class BoxColliderExtensions
{
    // Four corners of the collider's local +Y face, expressed in world space.
    // TransformPoint includes position, rotation, scale and all parent transforms.
    public static Vector3[] GetWorldBoxCorners(this BoxCollider box)
    {
        Transform t = box.transform;
        Vector3 c = box.center;
        Vector3 h = box.size * 0.5f;
        return new Vector3[]
        {
            t.TransformPoint(c + new Vector3(-h.x, h.y, -h.z)),
            t.TransformPoint(c + new Vector3( h.x, h.y, -h.z)),
            t.TransformPoint(c + new Vector3( h.x, h.y,  h.z)),
            t.TransformPoint(c + new Vector3(-h.x, h.y,  h.z))
        };
    }

    // Axis-aligned min/max of the entire box, including all 3 local axes.
    public static void GetWorldBoxMinMax(this BoxCollider box, out Vector3 min, out Vector3 max)
    {
        Transform t = box.transform;
        Vector3 center = t.TransformPoint(box.center);
        Vector3 half = box.size * 0.5f;
        Vector3 x = t.TransformVector(new Vector3(half.x, 0f, 0f));
        Vector3 y = t.TransformVector(new Vector3(0f, half.y, 0f));
        Vector3 z = t.TransformVector(new Vector3(0f, 0f, half.z));
        Vector3 extent = new Vector3(
            Mathf.Abs(x.x) + Mathf.Abs(y.x) + Mathf.Abs(z.x),
            Mathf.Abs(x.y) + Mathf.Abs(y.y) + Mathf.Abs(z.y),
            Mathf.Abs(x.z) + Mathf.Abs(y.z) + Mathf.Abs(z.z));
        min = center - extent;
        max = center + extent;
    }

    // Uniform random point on the collider's local +Y face, respecting rotation.
    // edgePadding is in world units and is measured along each face edge.
    public static Vector3 GetRandomPointOnBox(this BoxCollider box, float edgePadding = 0f)
    {
        Vector3[] corners = box.GetWorldBoxCorners();
        Vector3 edgeX = corners[1] - corners[0];
        Vector3 edgeZ = corners[3] - corners[0];
        float width = edgeX.magnitude;
        float depth = edgeZ.magnitude;
        edgePadding = Mathf.Max(0f, edgePadding);

        if (width <= edgePadding * 2f || depth <= edgePadding * 2f)
        {
            Debug.LogWarning($"Edge padding {edgePadding} is too large for {box.name} ({width} x {depth}).");
            return (corners[0] + corners[2]) * 0.5f;
        }

        float u = UnityEngine.Random.Range(edgePadding / width, 1f - edgePadding / width);
        float v = UnityEngine.Random.Range(edgePadding / depth, 1f - edgePadding / depth);
        return corners[0] + edgeX * u + edgeZ * v;
    }

}
