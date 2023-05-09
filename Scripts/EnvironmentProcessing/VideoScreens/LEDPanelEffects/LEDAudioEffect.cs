using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDAudioEffect : LEDEffect
{
    public Texture2D m_AudioTexture;
    public Color[] m_AudioValues = new Color[32];
    public Color[] m_LastAudioValues = new Color[32];
    public Color[] m_NextAudioValues = new Color[32];

    public override void Init()
    {
        base.Init();

        m_AudioTexture = new Texture2D(32, 1);
    }

    protected virtual void UpdateAudioTexture(LEDPanel ledPanel)
    {
        ledPanel.CurrentChangeTime += Time.deltaTime;
        if (ledPanel.CurrentChangeTime > m_fMaxTimeChange)
        {
            ledPanel.CurrentChangeTime = 0f;
            for (int i = 0; i < 32; i++)
            {
                m_LastAudioValues[i] = m_NextAudioValues[i];
                m_NextAudioValues[i] = Color.white * UnityEngine.Random.Range(0f, 1f);
            }
        }

        float lerp = Mathf.InverseLerp(0f, m_fMaxTimeChange, ledPanel.CurrentChangeTime);
        //Lerp values 
        for (int i = 0; i < 32; i++)
        {
            m_AudioValues[i] = Color.Lerp(m_LastAudioValues[i], m_NextAudioValues[i], lerp);
        }

        m_AudioTexture.SetPixels(m_AudioValues);
        m_AudioTexture.Apply();
    }
}