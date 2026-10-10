using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private const float VolumeDuration = 0.5f;
    private const float PitchDuration = 0.2f;

    [SerializeField]
    private float m_MaxVolume = 1f;

    private AudioSource m_AudioSource;
    private ValueLerp m_Volume;
    private ValueLerp m_Pitch;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.transform.SetParentAndClearLocals(this.transform);

        m_MaxVolume = m_AudioSource.volume;

        m_Volume = new ValueLerp(m_AudioSource.volume, OnVolumeChanged);
        m_Pitch = new ValueLerp(m_AudioSource.pitch, OnPitchChanged);
    }

    private void Update()
    {
        m_Volume.ManualUpdate();
        m_Pitch.ManualUpdate();
    }

    public void Toggle(bool state)
    {
        if (state == true)
        {
            if (m_Volume.TargetValue != m_MaxVolume)
            {
                FadeIn(0.1f);
            }
        }
        else if (state == false)
        {
            if (m_Volume.TargetValue != 0 && m_AudioSource.isPlaying == true)
            {
                FadeOut(0.25f);
            }
        }
    }

    public void SetTargetPitch(float pitch)
    {
        m_Pitch.SetTarget(pitch, PitchDuration);
        m_Pitch.ManualUpdate();
    }
    public void SetTargetVolume(float volume, float duration = VolumeDuration, float? start = null)
    {
        m_AudioSource.SetActive(true);
        m_Volume.SetTarget(volume, duration, start);
        m_Volume.ManualUpdate();
    }

    public void FadeIn(float duration, float? start = null)
    {
        SetTargetVolume(m_MaxVolume, duration, start);

        // if it's not playing or is a one shot, start playback
        if (m_AudioSource.isPlaying == false || m_AudioSource.loop == false)
        {
            if (m_AudioSource.isActiveAndEnabled == true)
            {
                m_AudioSource.Play();
            }
        }
    }

    public void FadeOut(float duration)
    {
        SetTargetVolume(0, duration);
    }

    private void OnVolumeChanged(float volume)
    {
        if (m_AudioSource.volume != volume)
        {
            if (volume == 0)
            {
                m_AudioSource.SetActive(false);
            }
        }
        m_AudioSource.volume = volume;
    }

    private void OnPitchChanged(float pitch)
    {
        m_AudioSource.pitch = pitch;
    }

    private class ValueLerp
    {
        public System.Action<float> NewValue = null;
        public bool IsLerping => m_StartTime > 0f && m_EndTime > 0f;
        public float TargetValue => m_EndValue;

        private System.Action<float> m_SetValue;

        private float m_StartTime;
        private float m_EndTime;

        private float m_StartValue;
        private float m_EndValue;

        private float m_Value;

        public ValueLerp(float currentValue, System.Action<float> setValue)
        {
            m_SetValue = setValue;
            m_Value = currentValue;

            Reset();
        }

        public void ManualUpdate()
        {
            if (IsLerping == true)
            {
                float t = Time.time.InverseLerp(m_StartTime, m_EndTime);
                m_Value = Mathf.Lerp(m_StartValue, m_EndValue, t);
                m_SetValue(m_Value);

                if (t >= 1f)
                {
                    Reset();
                }
            }
        }

        public void SetTarget(float target, float duration, float? start = null)
        {
            m_StartTime = Time.time;
            m_EndTime = m_StartTime + duration;

            m_StartValue = start ?? m_Value;
            m_EndValue = target;
        }

        private void Reset()
        {
            m_StartValue = -1f;
            m_EndValue = -1f;

            m_StartTime = 0;
            m_EndTime = 0;
        }
    }
}
