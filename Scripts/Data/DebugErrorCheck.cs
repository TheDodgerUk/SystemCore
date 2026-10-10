using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugErrorCheck
{
    public const string ERROR_MESSAGE = "DebugErrorCheck Error";
    private int m_Counter = 0;
    public void Increment() => m_Counter++;
    public bool IsError()
    {
        bool error = m_Counter > 100;
        if(error == true)
        {
            Debug.LogError(ERROR_MESSAGE);
        }

        bool errorHalf = m_Counter == 50;
        if (errorHalf == true)
        {
            Debug.LogError($"{ERROR_MESSAGE} half error, pause");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Beep();
#endif
        }
        return error;
    }

}
