using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineRenderEffectController : EffectController
{
    private const string MAIN_TEX = "_MainTex";
    private LineRenderer m_LineRenderer;
    private Material m_Material;
    private int m_NumSegments = 2;

    [SerializeField]
    private float m_fScrollSpeed = -5.0f;
    private Vector2 m_TextureOffset = Vector2.zero;
    private Vector2 m_TextureScale = new Vector2(5f, 1f);

    [SerializeField]
    private float m_fLineWidth = 0.05f;

    protected override void OnEffectCreated(GameObject effect)
    {
        m_LineRenderer = effect.GetComponentInChildren<LineRenderer>();

        m_LineRenderer.startWidth = m_fLineWidth;
        m_LineRenderer.endWidth = m_fLineWidth;

        m_Material = new Material(m_LineRenderer.material);
        m_Material.SetTextureScale(MAIN_TEX, m_TextureScale);
        m_LineRenderer.material = m_Material;
        CheckValid();
    }

    public override void OnActivated()
    {
        base.OnActivated();
        m_LineRenderer.SetActive(true);
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();
        m_LineRenderer.SetActive(false);
    }

    private void CheckValid()
    {
        if (m_LineRenderer.positionCount != m_NumSegments)
        {
            m_LineRenderer.positionCount = m_NumSegments;
        }
    }

    public override void OnEffectStart(ControllerData obj)
    {
        CheckValid();
        m_LineRenderer.SetActive(true);
    }

    public override void OnEffectUpdate(ControllerData obj)
    {
        CheckValid();

        Quaternion direction = Quaternion.LookRotation(obj.Raycaster.Ray.direction);
        m_Transform.SetPositionAndRotation(obj.Raycaster.Ray.origin, direction);

        Vector3 start = m_Transform.InverseTransformPoint(obj.Raycaster.GetPoint(0));
        Vector3 end = m_Transform.InverseTransformPoint(obj.Raycaster.GetEndPoint());

        //Over the distance create random points offset... pinching in at the ends
        for (int i = 0; i < m_NumSegments; i++)
        {
            float normalisedPos = Mathf.InverseLerp(0, m_NumSegments - 1, i);
            Vector3 position = Vector3.Lerp(start, end, normalisedPos);
            m_LineRenderer.SetPosition(i, position);
        }

        //Animate effect
        if (null != m_Material)
        {
            m_TextureOffset.x += Time.deltaTime * m_fScrollSpeed;
            m_Material.SetTextureOffset(MAIN_TEX, m_TextureOffset);
        }
    }

    public override void OnEffectEnd(ControllerData obj)
    {
        CheckValid();
        m_LineRenderer.SetActive(false);
    }
}
