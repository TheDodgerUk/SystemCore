using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDBarEffect : LEDAudioEffect
{
    private Gradient m_Gradient;
    private float m_fGradientShiftTime = 0.1f;

    public override void Init()
    {
        base.Init();

        GradientData data = Resources.Load<GradientData>("LightPanel07Gradient");
        m_Gradient = data.gradient;
    }

    public override void UpdateEffect(LEDPanel ledPanel)
    {
        UpdateAudioTexture(ledPanel);

        float colorLerp = ((Mathf.Sin(Time.time * m_fGradientShiftTime)) * 0.5f) + 0.5f;
        Color color = m_Gradient.Evaluate(colorLerp) * 0.05f;
        ledPanel.CurrentMaterial.SetColor("_Color", color);
        ledPanel.CurrentMaterial.SetTexture("_MainTex", m_AudioTexture);
    }
}