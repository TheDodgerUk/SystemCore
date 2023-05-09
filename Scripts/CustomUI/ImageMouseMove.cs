using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ImageMouseMove : ImageMouseBase
{
    protected Vector3 m_MouseOffset;

    protected override void DoubleTap()
    {
        base.DoubleTap();
    }

    protected override void RightClick()
    {
        base.RightClick();
    }

    protected override void Tap()
    {      
        base.Tap();
    }

    protected override void End()
    {
        base.End();
    }

    protected override void Begin()
    {       
        m_MouseOffset = Input.mousePosition - this.transform.position;
        base.Begin();
    }

    protected override void Held()
    {       
        this.transform.position = Input.mousePosition;
        this.transform.position -= m_MouseOffset;
        base.Held();
    }



}
