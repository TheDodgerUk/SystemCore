#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds the given define symbols to PlayerSettings define symbols.
/// Just add your own define symbols to the Symbols property at the below.
/// </summary>
[InitializeOnLoad]
public class SetDefineSymbols : MonoBehaviour
{
    public static readonly string[] Symbols = new string[]
    {
        "UNITY_POST_PROCESSING_STACK_V2",
        "CT_FB",
        "MONITOR_TRAINER"
     };


    [InspectorButton]
    public static void Set()
    {
        List<string> allDefines = new List<string>();
        allDefines.AddRange(Symbols);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));
    }

}
#endif