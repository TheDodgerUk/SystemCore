using UnityEngine;

public class CameraFade : MonoBehaviour
{
    private static int LayerMask => Layers.AllMask;

    [SerializeField]
    private float m_FadeDistance = 0.1f;
    [SerializeField]
    private float m_MaxFadeAt = 0.8f;

    [SerializeField]
    private EaseType m_EaseType = EaseType.CircOut;

    private Transform m_CameraTransform;
    private Transform m_CameraRigRoot;
    private Vector3 m_LastValidPos;
    private float m_ClipDistance;

    [SerializeField]
    private Material m_Material;
    [SerializeField]
    private Color m_Colour;
    private float m_Alpha;

    private void Awake()
    {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things. In this case, we just want to use
        // a blend mode that inverts destination colors.
        m_Material = Utils.Unity.CloneMaterial("Materials/CameraFade");
        m_Material.name = $"{typeof(CameraFade).Name}_{gameObject.name}";
        m_Material.hideFlags = HideFlags.HideAndDontSave;

        m_Colour = Color.black;
        m_Alpha = m_Colour.a;
    }

    private void OnPostRender()
    {
        if (m_Alpha > 0)
        {
            GL.PushMatrix();
            GL.LoadOrtho();

            m_Material.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.TexCoord(new Vector3(0, 0, 0));
            GL.Vertex3(0, 0, 0);
            GL.TexCoord(new Vector3(1, 0, 0));
            GL.Vertex3(1, 0, 0);
            GL.TexCoord(new Vector3(1, 1, 0));
            GL.Vertex3(1, 1, 0);
            GL.TexCoord(new Vector3(0, 1, 0));
            GL.Vertex3(0, 1, 0);
            GL.End();

            GL.PopMatrix();
        }
    }

    public void SetAlpha(float alpha)
    {
        m_Alpha = alpha;
        m_Colour.a = alpha.InverseLerp(0, m_MaxFadeAt);
        m_Material.color = m_Colour;
    }

    public void Initialise(Camera camera, Transform rigRoot, float minCameraDistance)
    {
        m_CameraTransform = camera.transform;
        m_ClipDistance = minCameraDistance;
        m_CameraRigRoot = rigRoot;

        ResetValidPosition();
    }

    public void ResetValidPosition()
    {
        m_LastValidPos = GetCameraClipPosition();
        SetAlpha(0f);
    }

    private void LateUpdate()
    {
        /*
         *  Compute the number of intersections from CurrentCameraPos to LastValidPos in both directions
         *      If I am NOT inside a wall and RaycastCountFromLastValidPos > RaycastCountToLastValidPos then I transitioned to inside a wall.
         *      Else if I am inside a wall and RaycastCountFromLastValidPos <= RaycastCountToLastValidPos then I transitioned to outside the wall
         *  If I am not inside wall I compute a set of points at a distance from the camera (in my case 2.5cm) and cast rays. I get the closest intersection
         *  if there is any and use it to compute the fade factor [0-1] depending on the distance.
         *      I update LastValidPos with the current camera position if a Physics.CheckSphere with the current camera position doesn't find any intersection.
         *  If I am inside a wall the fade factor is 1
         */

        var position = GetCameraClipPosition();
        var toLastPos = m_LastValidPos - position;
        var toCurrentPos = -toLastPos;

        // check that last pos and this pos aren't the same
        if (toLastPos.sqrMagnitude > 0.00001f)
        {
            Debug.DrawLine(m_LastValidPos, position, Color.blue);
            float distance = toLastPos.magnitude;
            int toCurrentPosCount = GetRaycastCount(m_LastValidPos, toCurrentPos, distance);
            int toLastPosCount = GetRaycastCount(position, toLastPos, distance);
            if (toCurrentPosCount != toLastPosCount)
            {
                SetAlpha(1f);
                toLastPos.y = 0;
                m_CameraRigRoot.Translate(toLastPos, Space.World);
            }
            else
            {
                SetAlpha(GetFadeValue(position));
                m_LastValidPos = position;
            }
        }
    }

    private float GetFadeValue(Vector3 position)
    {
        // is anything in range?
        float distance = float.MaxValue;
        var sphereHits = Physics.OverlapSphere(position, m_FadeDistance, LayerMask);
        foreach (var collider in sphereHits)
        {
            if (IsFadeableCollider(collider) == true)
            {
                var closestPoint = collider.ClosestPoint(position);
                float d = Vector3.Distance(position, closestPoint);
                distance = distance.Min(d);
            }
        }
        return m_EaseType.Ease(1f - (distance / m_FadeDistance));
    }

    private Vector3 GetCameraClipPosition()
    {
        return m_CameraTransform.position + m_CameraTransform.forward * m_ClipDistance;
    }

    private static int GetRaycastCount(Vector3 origin, Vector3 direction, float distance)
    {
        return Physics.RaycastAll(origin, direction, distance, LayerMask).Length;
    }

    private static bool IsFadeableCollider(Collider collider)
    {
        var mesh = collider as MeshCollider;
        if (mesh != null && mesh.convex == false)
        {
            return false;
        }
        return true;
    }
}
