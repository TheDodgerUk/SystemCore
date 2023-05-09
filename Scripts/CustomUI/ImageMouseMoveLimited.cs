using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ImageMouseInputBase))]
public class ImageMouseMoveLimited : ImageMouseMove
{
    private Vector3 m_Min;
    private Vector3 m_Max;

    public void SetLocalValidArea(Vector3 min, Vector3 max)
    {
        m_Min = min;
        m_Max = max;
    }

    protected override void RightClick()
    {
        OnRightClick?.Invoke();
    }

    protected override void Held()
    {
        this.transform.position = Input.mousePosition;
        this.transform.position -= m_MouseOffset;

        Vector3 localPos = this.transform.localPosition;
        localPos = localPos.Clamp(m_Min, m_Max);
        this.transform.localPosition = localPos;
        OnPointerHeld?.Invoke();
    }



}
