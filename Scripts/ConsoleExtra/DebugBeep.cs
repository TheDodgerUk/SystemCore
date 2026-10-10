using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DebugBeep
{
    public enum MessageLevel
    {
        High,
        Medium,
        Low
    }

    private static List<string> m_ErrorSent = new List<string>();


    public static void Log(string message, MessageLevel level, GameObject obj = null)
    {
        Debug.LogError(StringColor(message, level), obj);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.Beep();
#endif
    }



    public static void LogError(string message, MessageLevel level, GameObject obj = null)
    {
        Debug.LogError(StringColor(message, level), obj);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.Beep();
#endif
    }




    private static string StringColor(string message, MessageLevel level)
    {
        string prefix = "";
        switch (level)
        {
            case MessageLevel.High:
                prefix = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>";
                break;
            case MessageLevel.Medium:
                prefix = $"<color=#FFA500>"; // orange
                break;
            case MessageLevel.Low:
                prefix = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.green)}>";
                break;
            default:
                break;
        }

        string suffix = "</color>";
        return $"{prefix} { message} {suffix}";
    }

}

