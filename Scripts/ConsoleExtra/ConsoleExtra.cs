/* Added by Anthony Brown
 * This is the public interface of ConsoleExtra
 * this is more debug information 
 * Helpers -> ConsoleExtra
 * 
 * Usage example
 * ConsoleExtra.Log("Initialise data", this.gameobject, ConsoleExtraEnum.EDebugType.Init);
 */


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConsoleExtra : MonoBehaviour
{
#if UNITY_EDITOR
    class Data
    {
        public int m_CurrentCount;
        public int m_MaxCount;

    }
    private static Dictionary<string, Data> m_ErrorCount = new Dictionary<string, Data>();

    public static void Log(string messageString, GameObject gameObjectRef, ConsoleExtraEnum.EDebugType debugType, int count = 0)
    {
        if (UnityEditor.EditorWindow.HasOpenInstances<Logging>() == false)
        {
            return;
        }


        if (false == m_ErrorCount.ContainsKey(messageString))
        {

            Data newData = new Data();
            newData.m_CurrentCount = count + 1;
            newData.m_MaxCount = count;

            m_ErrorCount.Add(messageString, newData);
        }
        Data data = m_ErrorCount[messageString];
        data.m_CurrentCount++;
        if (data.m_CurrentCount >= data.m_MaxCount)
        {
            LoggingExtra.Log(messageString, gameObjectRef, debugType);
            data.m_CurrentCount = 0;
        }
    }

#else
    public static void Log(string sMessageString, GameObject lGameObject, ConsoleExtraEnum.EDebugType debugType, int lCount = 0)
    {
        if(debugType == ConsoleExtraEnum.EDebugType.DebugLogging)
        {
            Debug.LogError(sMessageString);
        }
        ////List<string> lNames = Enum.GetNames(typeof(ConsoleExtraEnum.LogError)).ToList();
        ////if(lNames.Contains(lEDebugType.ToString()) == true)
        ////{
        ////    Debug.Log(lEDebugType.ToString() + " **************** " + lGameObject.name + "  " + lMessageString);
        ////}
    }
#endif
}
