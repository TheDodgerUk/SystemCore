using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteractionRotation : VrInteraction
{
    private const float ROTATION_MULIPLYER = 1.5f;
    private GameObject m_RootGameObject;
    private Vector3 m_ObjectStartRotation;
    private Vector3 m_ControllerStartRotation;

    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        interaction.Main.m_UseOverrideBeamColour = true;
        interaction.Main.m_OverrideBeamColour = Color.green;
        interaction.Main.LockToObject(this);

        m_ControllerStartRotation = interaction.Main.WristTransform.localEulerAngles;
        m_ObjectStartRotation = m_RootGameObject.transform.localEulerAngles;
    }

    public override void OnUpdateLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        interaction.Main.m_OverrideBeamColour = Color.green;
        float amount = Helpers.Generic.GetCorrectRotateAmount(m_ControllerStartRotation.y, interaction.Main.WristTransform.localEulerAngles.y);
        amount *= ROTATION_MULIPLYER;
        m_RootGameObject.transform.localRotation = Quaternion.Euler(0, m_ObjectStartRotation.y + amount, 0);
    }


    public override void EndLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        interaction.Main.m_UseOverrideBeamColour = false;
        interaction.Main.LockToObject(null);
    }

}
