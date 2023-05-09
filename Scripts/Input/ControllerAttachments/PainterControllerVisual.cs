using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainterControllerVisual : ControllerVisual
{
    private Transform m_Grip;
    private Transform m_Trigger;
    private Transform m_Hologram;

    private Material m_GripMaterial;
    private Material m_InteractMaterial;

    private ValueTween m_InteractTween;
    private ValueTween m_HologramTween;
    private float m_fMaxHologramSize = 0.07f;
    private Vector3 m_HologramTargetScale = Vector3.zero;

    private Color m_ActiveColor = Color.green * 1.5f;
    private Color m_InactiveColor = Color.black;
    private Vector3 m_GripStartPosition;
    private Vector3 m_GripEndPosition;
    private Vector3 m_GridGrabPositionOffset = new Vector3(0.004f, 0f, 0f);
    private Quaternion m_TriggerStartRotation;
    private Quaternion m_TriggerEndRotation;
    private Quaternion m_TriggerRotationOffset = Quaternion.Euler(5f, 0f, 0f);

    public override void Init(ControllerGraphics gfx)
    {
        base.Init(gfx);

        //Find components we need
        m_Hologram = m_Transform.Search("Hologram");
        m_Grip = m_Transform.Search(GRIP_BUTTON);
        m_GripMaterial = GetMaterial(m_Grip);
        m_GripMaterial.EnableKeyword(EMISSION_KEYWORD);

        m_Trigger = m_Transform.Search(TRIGGER_BUTTON);
        m_InteractMaterial = GetMaterial(m_Trigger);
        m_InteractMaterial.EnableKeyword(EMISSION_KEYWORD);

        m_GripStartPosition = m_Grip.localPosition;
        float dir = Mathf.Abs(m_GripStartPosition.x) / m_GripStartPosition.x;
        m_GripEndPosition = m_GripStartPosition + (m_GridGrabPositionOffset * dir);

        m_TriggerStartRotation = m_Trigger.localRotation;
        m_TriggerEndRotation = m_TriggerStartRotation * m_TriggerRotationOffset;
    }

    public override void Visible(bool bShow)
    {
        base.Visible(bShow);
        //Set default size
        m_Hologram.localScale = Vector3.zero;
    }

    public override void OnGrabBegin(ControllerData controller)
    {
        //Show hologram
        m_HologramTween = m_Hologram.Create<ValueTween>(0.25f, EaseType.ExpoIn);
        m_Hologram.localScale = Vector3.zero;
        m_HologramTargetScale = Vector3.one * m_fMaxHologramSize;

        Color originalCol = m_GripMaterial.GetColor(EMISSION_COLOR);
        m_HologramTween.Initialise(0f, 1f, (e) =>
        {
            UpdateTransitionPosition(e, m_Grip, m_GripEndPosition);
            UpdateMaterialColor(e, m_GripMaterial, EMISSION_COLOR, originalCol, m_ActiveColor);
            UpdateTransitionScale(e, m_Hologram, m_HologramTargetScale);
        });
    }

    public override void OnGrabEnd(ControllerData controller)
    {
        //Show hologram
        m_HologramTween = m_Hologram.Create<ValueTween>(0.25f, EaseType.ExpoIn);
        m_HologramTargetScale = Vector3.zero;

        Color originalCol = m_GripMaterial.GetColor(EMISSION_COLOR);
        m_HologramTween.Initialise(0f, 1f, (e) =>
        {
            UpdateTransitionPosition(e, m_Grip, m_GripStartPosition);
            UpdateMaterialColor(e, m_GripMaterial, EMISSION_COLOR, originalCol, m_InactiveColor);
            UpdateTransitionScale(e, m_Hologram, m_HologramTargetScale);
        });
    }

    public override void OnInteractBegin(ControllerData controller)
    {
        m_InteractTween = m_Trigger.Create<ValueTween>(0.25f, EaseType.ExpoIn);
        Color originalCol = m_InteractMaterial.GetColor(EMISSION_COLOR);
        m_InteractTween.Initialise(0f, 1f, (e) =>
        {
            UpdateTransitionRotate(e, m_Trigger, m_TriggerEndRotation);
            UpdateMaterialColor(e, m_InteractMaterial, EMISSION_COLOR, originalCol, m_ActiveColor);
        });
    }

    public override void OnInteractEnd(ControllerData controller)
    {
        m_InteractTween = m_Trigger.Create<ValueTween>(0.25f, EaseType.ExpoIn);
        Color originalCol = m_InteractMaterial.GetColor(EMISSION_COLOR);
        m_InteractTween.Initialise(0f, 1f, (e) =>
        {
            UpdateTransitionRotate(e, m_Trigger, m_TriggerStartRotation);
            UpdateMaterialColor(e, m_InteractMaterial, EMISSION_COLOR, originalCol, m_InactiveColor);
        });
    }
}
