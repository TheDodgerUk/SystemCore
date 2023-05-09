using UnityEngine;
using System;

public class AnalogueClock : MonoBehaviour
{

    private Transform m_HoursTransform;
    private Transform m_MinutesTransform;
    private Transform m_SecondsTransform;

    private const float DEGREES_PER_HOUR = 30f;
    private const float DEGREES_PER_MIN = 6f;
    private const float DEGREES_PER_SEC = 6f;

    public bool continuous = false;

    private void Awake()
    {
        m_HoursTransform = this.transform.Search("Hour");
        m_MinutesTransform = this.transform.Search("Min");
        m_SecondsTransform = this.transform.Search("Sec");
    }

    void Update()
    {
        if (continuous)
        {
            UpdateContinuous();
        }
        else
        {
            UpdateDiscrete();
        }
    }


    void UpdateDiscrete()
    {
        DateTime time = DateTime.Now;
        m_HoursTransform.localRotation = Quaternion.Euler(0f, 0, (float)time.Hour * DEGREES_PER_HOUR);
        m_MinutesTransform.localRotation = Quaternion.Euler(0f, 0, (float)time.Minute * DEGREES_PER_MIN);
        if (m_SecondsTransform != null)
        {
            m_SecondsTransform.localRotation = Quaternion.Euler(0f, 0, (float)time.Second * DEGREES_PER_SEC);
        }
    }

    void UpdateContinuous()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        m_HoursTransform.localRotation = Quaternion.Euler(0f, 0, (float)time.TotalHours * DEGREES_PER_HOUR);
        m_MinutesTransform.localRotation = Quaternion.Euler(0f, 0, (float)time.TotalMinutes * DEGREES_PER_MIN);
        if (m_SecondsTransform != null)
        {
            m_SecondsTransform.localRotation = Quaternion.Euler(0f, 0, (float)time.TotalSeconds * DEGREES_PER_SEC);
        }
    }
}
