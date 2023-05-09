using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstimateTimeLeftForLoops
{
    private int m_TotalLoopCount;
    private DateTime m_StartTime;
    public EstimateTimeLeftForLoops(int totalLoopCount)
    {
        m_TotalLoopCount = totalLoopCount;
        m_StartTime = DateTime.Now;
    }

    public TimeSpan RemainingTime(int index)
    {
        return TimeSpan.FromTicks(DateTime.Now.Subtract(m_StartTime).Ticks * ((m_TotalLoopCount) - (index + 1)) / (index + 1));
    }

    public string RemainingTimeReadable(int index)
    {
        if (index == 0)
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

