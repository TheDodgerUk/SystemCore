using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyVector2Event : UnityEvent<Vector2>
{
}

public class ImageMouseDifference : ImageMouseBase
{
    public MyVector2Event OnDifference = new MyVector2Event();
    public MyVector2Event OnDifferenceEnd = new MyVector2Event();
    protected Vector3 m_MouseStartPosition;

    private void Start()
    {
        EndWhenLeaveObject(false);
    }
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
        OnDifference?.Invoke(Input.mousePosition - m_MouseStartPosition);
        OnDifferenceEnd?.Invoke(Input.mousePosition - m_MouseStartPosition);
    }

    protected override void Begin()
    {
        m_MouseStartPosition = Input.mousePosition;
        OnDifference?.Invoke(Input.mousePosition - m_MouseStartPosition);
    }

    protected override void Held()
    {       
        OnDifference?.Invoke(Input.mousePosition - m_MouseStartPosition);
    }



}
