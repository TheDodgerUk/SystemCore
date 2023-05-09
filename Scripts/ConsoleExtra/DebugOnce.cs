using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugOnce
{

    private static List<string> m_ErrorSent = new List<string>();
    public static void Log(string message, GameObject obj = null)
    {
        //Debug.LogError($"m_ErrorSent1  {m_ErrorSent.Count}");
        if (m_ErrorSent.Contains(message) == false)
        {
            m_ErrorSent.Add(message);
            //Debug.LogError($"m_ErrorSent2  {m_ErrorSent.Count}");
            Debug.Log(message, obj);
        }
    }

    public static void LogError(string message, GameObject obj = null)
    {
        //Debug.LogError($"m_ErrorSent1  {m_ErrorSent.Count}");
        if (m_ErrorSent.Contains(message) == false)
        {
            m_ErrorSent.Add(message);
            //Debug.LogError($"m_ErrorSent2  {m_ErrorSent.Count}");
            Debug.LogError(message, obj);
        }
    }
}
