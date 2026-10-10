using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDMonitorEffect : LEDAudioEffect
{
    public override void UpdateEffect(LEDPanel ledPanel)
    {
        UpdateAudioTexture(ledPanel);
        ledPanel.CurrentMaterial.SetTexture("_MainTex", m_AudioTexture);
    }
}
