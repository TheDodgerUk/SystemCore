using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualLoggerMono : MonoBehaviour
{
    public static VisualLoggerMono s_Instance;

    [InspectorButton]
    private void Toggle() => Core.VisualLoggerRef.Toggle();

    [InspectorButton]
    private void LoopDisplayType() => Core.VisualLoggerRef.LoopDisplayType();


    private void Awake()
    {
        s_Instance = this;
    }
}
