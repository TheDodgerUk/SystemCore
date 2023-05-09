using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoverControllerVisual : ControllerVisual
{
    private Transform m_Grip;
    private Material m_GripMaterial;
    private Vector3 m_GripStartPosition;
    private Vector3 m_GripEndPosition;
    private Vector3 m_GridGrabPositionOffset = new Vector3(0.004f, 0f, 0f);
    private ValueTween m_GripTween;
    private Color m_GripActiveColor = new Color(0.8f, 0.3f, 0.06f) * 1.5f;

    public override void Init(ControllerGraphics gfx)
    {
        base.Init(gfx);

        //Get Grip model and material
        m_Grip = m_Transform.Search(GRIP_BUTTON);
        m_GripStartPosition = m_Grip.localPosition;
        float dir = Mathf.Abs(m_GripStartPosition.x) / m_GripStartPosition.x;
        m_GripEndPosition = m_GripStartPosition + (m_GridGrabPositionOffset * dir);

        m_GripMaterial = GetMaterial(m_Grip);
        m_GripMaterial.EnableKeyword(EMISSION_KEYWORD);
    }

    public override void Visible(bool bShow)
    {
        base.Visible(bShow);

        if (true == bShow)
        {
            m_GripMaterial.SetFloat(FADE_IN, 1.0f);
            m_GripMaterial.SetFloat(DISSOLVE, 0.0f);
        }
        else
        {
            m_GripMaterial.SetFloat(FADE_IN, 0.0f);
        }
    }

    protected override void OnTransitionUpdate(float value)
    {
        base.OnTransitionUpdate(value);

        m_GripMaterial.SetFloat(DISSOLVE, value);
    }

    public override void OnGrabBegin(ControllerData controller)
    {
        m_GripTween = m_Grip.Create<ValueTween>(0.25f, EaseType.ExpoIn);
        Color originalColor = m_GripMaterial.GetColor(EMISSION_COLOR);
        m_GripTween.Initialise(0f, 1f, (e) =>
        {
            UpdateTransitionPosition(e, m_Grip, m_GripEndPosition);
            UpdateMaterialColor(e, m_GripMaterial, EMISSION_COLOR, originalColor, m_GripActiveColor);
        });
    }

    public override void OnGrabEnd(ControllerData controller)
    {
        m_GripTween = m_Grip.Create<ValueTween>(0.25f, EaseType.ExpoIn);
        Color originalColor = m_GripMaterial.GetColor(EMISSION_COLOR);
        m_GripTween.Initialise(0f, 1f, (e) =>
        {
            UpdateTransitionPosition(e, m_Grip, m_GripStartPosition);
            UpdateMaterialColor(e, m_GripMaterial, EMISSION_COLOR, originalColor, Color.black);
        });
    }
}
