using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDWaveEffect : LEDEffect
{
    private Gradient m_Gradient;
    private float m_fGradientShiftTime = 0.1f;

    public override void Init()
    {
        GradientData data = Resources.Load<GradientData>("LightPanel07Gradient");
        m_Gradient = data.gradient;
    }

    public override void UpdateEffect(LEDPanel ledPanel)
    {
        ledPanel.CurrentChangeTime += Time.deltaTime;

        if (ledPanel.CurrentChangeTime > m_fMaxTimeChange)
        {
            ledPanel.CurrentChangeTime = 0f;
            //Update this material
            ledPanel.m_fLastValue = ledPanel.m_fCurrentValue;
            ledPanel.m_fCurrentValue = Mathf.Lerp(-0.00001f, 0.008f, UnityEngine.Random.Range(0f, 1f));
        }

        float colorLerp = ((Mathf.Sin(Time.time * m_fGradientShiftTime)) * 0.5f) + 0.5f;
        Color color = m_Gradient.Evaluate(colorLerp);
        float lerp = Mathf.InverseLerp(0f, m_fMaxTimeChange, ledPanel.CurrentChangeTime);
        float audioValue = Mathf.Lerp(ledPanel.m_fLastValue, ledPanel.m_fCurrentValue, lerp);
        float lineOffset = Mathf.Lerp(1f, 10f, colorLerp);
        ledPanel.CurrentMaterial.SetVector("_Spectrum", new Vector4(0f, audioValue, 0f, 0f));
        ledPanel.CurrentMaterial.SetColor("_Color", color);
        ledPanel.CurrentMaterial.SetFloat("_Offset", lineOffset);
        ledPanel.CurrentMaterial.SetFloat(LEDPanel.EMISSIVE, 10.0f);
    }
}
