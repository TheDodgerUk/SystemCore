using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VrInteractionDrink: VrInteractionPickUp
{
    private ContentDrinkMetaData m_Data;
    private LiquidWobble m_LiquidWobble;
    private VisualEffect m_VisualEffect;

    public void Initialise(ContentDrinkMetaData data)
    {
        m_Data = data;
        if (m_Data.m_LiquidWobbleGameObject != null)
        {
            m_LiquidWobble = m_Data.m_LiquidWobbleGameObject.GetComponent<LiquidWobble>();
            m_VisualEffect = m_Data.m_LiquidWobbleGameObject.GetComponent<VisualEffect>();
        }
        // close to head, and tilted
        // take percentage out of the shader of something
    }

}
