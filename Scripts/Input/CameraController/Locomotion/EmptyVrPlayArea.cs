using System;
using UnityEngine;

public class EmptyVrPlayArea : PlayArea
{
    protected override void RequestPlayAreaSize(Action<Vector3> callback)
    {
        callback(Vector3.one);
    }
}