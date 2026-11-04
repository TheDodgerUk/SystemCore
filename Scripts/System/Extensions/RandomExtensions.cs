using UnityEngine;

public static class RandomExtensions
{
    public static Vector3 RandomPoint(BoxCollider box, float edgePadding = 0f)
    {
        Transform t = box.transform;

        Vector3 center = box.center;
        Vector3 half = box.size * 0.5f;

        float width = t.TransformVector(Vector3.right * box.size.x).magnitude;
        float depth = t.TransformVector(Vector3.forward * box.size.z).magnitude;

        if (width <= edgePadding * 2f ||
            depth <= edgePadding * 2f)
        {
            Debug.LogWarning("Edge padding is too large.");
            return t.TransformPoint(center);
        }

        float x = UnityEngine.Random.Range(
            -half.x + edgePadding * box.size.x / width,
             half.x - edgePadding * box.size.x / width
        );

        float z = UnityEngine.Random.Range(
            -half.z + edgePadding * box.size.z / depth,
             half.z - edgePadding * box.size.z / depth
        );

        return t.TransformPoint(center + new Vector3(x, half.y, z));
    }
}