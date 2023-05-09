using UnityEngine;

public class BeamVisual : MonoBehaviour
{
    public const string DIRECTION = "_Invert";
    public const string START_COLOR = "_StartColor";
    public const string END_COLOR = "_EndColor";
    public const string ARROW_TEXTURE = "_MainTex";
    public const string START_FADE_POSITION = "_StartFadeInPosition";
    public const string END_FADE_POSITION = "_EndFadeInPosition";

    private Transform m_Transform;

    private LineRenderer m_LineRenderer;
    [SerializeField]
    private bool m_bVisible;
    [SerializeField]
    private Vector3 TargetPosition;

    private float m_fPortionBezier = 0.23f;
    [SerializeField]
    private float m_fRepeatsPerMetre = 25f;

    private Vector2 m_WrapCount = new Vector2(1f, 2f);
    //private Vector2 m_TextureOffset = new Vector2(0f, -0.5f);

    [SerializeField]
    private Transform m_Target;
    private Vector3[] m_Positions = null;
    private Material m_Material;
    private Vector4 m_FadeInPosition = Vector4.zero;
    private Vector4 m_FadeOutPosition = Vector4.zero;

    private bool m_bFadeEnds = true;

    private const int m_iSegments = 24;

    public void SetPortionBezier(float portion)
    {
        m_fPortionBezier = portion;
    }

    public bool IsActive()
    {
        return m_bVisible;
    }

    public void Init(Transform target, Transform root = null)
    {
        m_Transform = root;
        if (null == m_Transform)
        {
            m_Transform = transform;
        }

        m_Target = target;
        if (null == m_LineRenderer)
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.numCornerVertices = 2;
            m_LineRenderer.numCapVertices = 0;
        }

        if (null == m_Positions || 0 == m_Positions.Length)
        {
            m_Positions = new Vector3[m_iSegments];
            m_LineRenderer.SetPositions(m_Positions);
            m_Material = new Material(m_LineRenderer.sharedMaterial);
            //m_Material.SetTextureOffset(ARROW_TEXTURE, m_TextureOffset);
            m_LineRenderer.material = m_Material;
        }
        Visible(false);
        this.transform.Reset();

        // this is weird and i not understand it but it needs to be flipped
        this.transform.localRotation = GlobalConsts.LASER_LINE_OFFSET_ROTATION;
    }

    public Material GetMaterial()
    {
        return m_Material;
    }

    public void UseWorldSpace(bool bWorld)
    {
        if (null != m_LineRenderer)
        {
            m_LineRenderer.useWorldSpace = bWorld;
        }
    }

    public void SetBeamWidth(float start, float end)
    {
        if (null != m_LineRenderer)
        {
            m_LineRenderer.startWidth = start;
            m_LineRenderer.endWidth = end;
        }
    }

    public void SetBeamColor(Color start, Color end)
    {
        if (null != m_LineRenderer)
        {
            m_Material.SetColor(START_COLOR, start);
            m_Material.SetColor(END_COLOR, end);
            m_Material.color = start;
        }
    }

    public void SetBeamDirection(float Direction)
    {
        m_Material.SetFloat(DIRECTION, Direction);
    }

    public void Visible(bool bVisible)
    {
        if (m_bVisible != bVisible)
        {
            m_bVisible = bVisible;

            if (false == m_bVisible)
            {
                for (int i = 0; i < m_Positions.Length; i++)
                {
                    m_Positions[i] = Vector3.zero;
                }

                m_LineRenderer.SetPositions(m_Positions);
            }
            m_LineRenderer.enabled = m_bVisible;
        }
    }

    // Update is called once per frame
    public void ManualUpdate()
    {
        if (false == m_bVisible)
        {
            return;
        }

        if (null != m_Target)
        {
            TargetPosition = m_Target.position;
        }

        CreatePointsFromBezier(TargetPosition);
    }

    public void SetPoints(Vector3[] points)
    {
        m_Positions = points;
        m_LineRenderer.positionCount = m_Positions.Length;
        m_LineRenderer.SetPositions(m_Positions);

        if (true == m_bFadeEnds)
        {
            m_FadeInPosition = m_Transform.position;
            m_Material.SetVector(START_FADE_POSITION, m_FadeInPosition);
            m_FadeOutPosition = m_Positions[m_Positions.Length - 1];
            m_Material.SetVector(END_FADE_POSITION, m_FadeOutPosition);
        }
    }

    public void CreatePointsFromBezier(Vector3 TargetPosition)
    {
        float fullDistance = Vector3.Distance(m_Transform.position, TargetPosition);
        float quarterDistance = fullDistance * m_fPortionBezier;
        Vector3 p1 = m_Transform.position + (m_Transform.forward * quarterDistance);
        Vector3 p2 = TargetPosition + (-m_Target.forward * quarterDistance);

        float fCount = (float)m_Positions.Length - 1f;
        for (int i = 0; i < m_Positions.Length - 1; i++)
        {
            float dist = Mathf.InverseLerp(0f, fCount, (float)i);
            Vector3 position = Bezier.GetCubicPoint(dist, m_Transform.position, p1, p2, TargetPosition);
            if (false == m_LineRenderer.useWorldSpace)
            {
                position = m_Transform.InverseTransformPoint(position);
            }
            m_Positions[i] = position;
        }

        if (false == m_LineRenderer.useWorldSpace)
        {
            TargetPosition = m_Transform.InverseTransformPoint(TargetPosition);
        }

        m_Positions[m_Positions.Length - 1] = TargetPosition;
        SetPoints(m_Positions);

        WrapSecondaryTexture();
    }

    private void WrapSecondaryTexture()
    {
        float wrapCount = GetLength(m_Positions) * m_fRepeatsPerMetre;
        m_WrapCount.x = wrapCount;
        m_Material.SetTextureScale(ARROW_TEXTURE, m_WrapCount);
    }

    private float GetLength(Vector3[] positions)
    {
        float distance = 0f;

        for (int i = 1; i < positions.Length; i++)
        {
            distance += Vector3.Distance(positions[i - 1], positions[i]);
        }
        return distance;
    }

    private static float QuadInOut(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
        {
            return end * 0.5f * value * value + start;
        }
        value--;
        return -end * 0.5f * (value * (value - 2) - 1) + start;
    }
}