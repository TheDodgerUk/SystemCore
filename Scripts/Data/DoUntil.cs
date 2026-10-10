using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoUntil
{
    public DoUntil(Func<bool> doUntil, Action doStuff)
    {
        DebugErrorCheck error = new DebugErrorCheck();
        do
        {
            error.Increment();
            if(error.IsError() == true)
            {
                break;
            }
            doStuff?.Invoke();
        } while (doUntil() == false);
    }

}
