using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIToolTip : MonoBehaviour
{
    public int m_iControllerID = -1;
    
    private BeamVisual m_Beam;
    [SerializeField]
    private Transform m_Root;
    [SerializeField]
    private Transform m_Anchor;

    [SerializeField]
    private Transform m_MeshTransform;

    private TextMeshPro m_TMText;
    private Material m_BeamMaterial;

    private Material m_MeshMaterial;

    public float m_BezierAmount = 0.5f;
    private float m_fAngleAmount = 65f;

    private bool m_bVisible = false;
    private float ChangeTime = 0.0f;
    private float FadeInTime = 0.5f;

    public void Init()
    {
        if (null == m_Root)
        {
            m_Root = transform;
        }

        m_Beam = m_Root.gameObject.AddComponent<BeamVisual>();
        m_Beam.Init(m_Anchor, m_Root);
        m_Beam?.SetPortionBezier(m_BezierAmount);
        m_Beam?.UseWorldSpace(false);
        m_Beam?.Visible(true);

        m_TMText = transform.parent.GetComponentInChildren<TextMeshPro>();
        m_BeamMaterial = m_Beam.GetMaterial();

        if(null != m_MeshTransform)
        {
            MeshRenderer renderer = m_MeshTransform.GetComponent<MeshRenderer>();
            m_MeshMaterial = new Material(renderer.sharedMaterial);
            renderer.material = m_MeshMaterial;
        }
    }

    public void Visible(bool bVisible)
    {
        if (m_bVisible != bVisible)
        {
            m_bVisible = bVisible;
            ChangeTime = Time.time;
        }
    }

    public void SetButtonColor(Color color)
    {
        m_MeshMaterial?.SetColor("_Color", color);
    }

    public void ManualUpdate()
    {
        m_Beam?.ManualUpdate();

        float angle = 0f;
        if (null != CameraControllerVR.Instance)
        {
            Transform camera = CameraControllerVR.Instance.CameraTransform;
            if (null != camera)
            {
                //m_TMText.transform.rotation = camera.rotation;
                angle = Vector3.Angle(camera.forward, m_TMText.transform.forward);
            }
        }
        else
        {
            // m_TMText.transform.rotation = Camera.main.transform.rotation;
            angle = Vector3.Angle(Camera.main.transform.forward, m_TMText.transform.forward);
        }

        float lerpValue = Mathf.Clamp01(Mathf.InverseLerp(m_fAngleAmount, 0f, angle));
        float FadeValue = Mathf.Clamp01(Mathf.InverseLerp(ChangeTime, ChangeTime + FadeInTime, Time.time));

        if (false == m_bVisible)
        {
            FadeValue = 1.0f - FadeValue;
        }

        float value = lerpValue * FadeValue;
        
        m_TMText.color = (Color.white * value);
        m_BeamMaterial.SetFloat("_Alpha", value);
    }
}