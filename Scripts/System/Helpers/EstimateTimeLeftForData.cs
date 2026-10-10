using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstimateTimeLeftForData
{
    private const int FLOAT_TO_INT = 1000;
    private int m_TotalLoopCount;
    private DateTime m_StartTime;
    public EstimateTimeLeftForData(float totalMeg)
    {
        m_TotalLoopCount = ((int)(totalMeg * FLOAT_TO_INT));
        m_StartTime = DateTime.Now;
    }

    public TimeSpan RemainingTime(float currentMg)
    {
        int index = ((int)(currentMg * FLOAT_TO_INT));
        return TimeSpan.FromTicks(DateTime.Now.Subtract(m_StartTime).Ticks * ((m_TotalLoopCount) - (index + 1)) / (index + 1));
    }

    public string RemainingTimeReadable(float currentMg)
    {
        int index = ((int)(currentMg * FLOAT_TO_INT));
        if (index == FLOAT_TO_INT)
        {
            string a = "\u221E";
            return a;
        }
        else
        {
            TimeSpan amount = TimeSpan.FromTicks(DateTime.Now.Subtract(m_StartTime).Ticks * ((m_TotalLoopCount) - (index + 1)) / (index + 1));
            return amount.ToReadableString();
        }

    }
}

