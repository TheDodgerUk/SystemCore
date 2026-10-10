using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnDisable : MonoBehaviour
{

    private void OnEnable()
    {
        Debug.LogError($"OnEnable {this.name}", this);
    }

    private void OnDisable()
    {
        Debug.LogError($"OnDisable {this.name}", this);
    }
}
