using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ImageMouseInputBase))]
public class ImageMouseBase : MonoBehaviour
{

    public UnityEvent OnPointerTap = new UnityEvent();
    public UnityEvent OnPointerBegin = new UnityEvent();
    public UnityEvent OnPointerHeld = new UnityEvent();
    public UnityEvent OnPointerEnd = new UnityEvent();
    public UnityEvent OnRightClick = new UnityEvent();
    public UnityEvent OnPointerDoubleTap = new UnityEvent();

    private ImageMouseInputBase m_ImageMouseInputBase;
    public void EndWhenLeaveObject(bool onLeaveObject) => m_ImageMouseInputBase.OnEndWhenLeaveObject = onLeaveObject;

    private void Awake()
    {
        m_ImageMouseInputBase = GetComponent<ImageMouseInputBase>();
        m_ImageMouseInputBase.OnPointerTap.AddListener(Tap);
        m_ImageMouseInputBase.OnPointerDoubleTap.AddListener(DoubleTap);
        m_ImageMouseInputBase.OnPointerBegin.AddListener(Begin);
        m_ImageMouseInputBase.OnPointerHeld.AddListener(Held);
        m_ImageMouseInputBase.OnPointerEnd.AddListener(End);
        m_ImageMouseInputBase.OnRightClick.AddListener(RightClick);
    }

    protected virtual void DoubleTap()
    {
        OnPointerDoubleTap?.Invoke();
    }

    protected virtual void RightClick()
    {
        OnRightClick?.Invoke();
    }

    protected virtual void Tap()
    {
        OnPointerTap?.Invoke();
    }


    protected virtual void End()
    {
        OnPointerEnd?.Invoke();
    }

    protected virtual void Begin()
    {
        OnPointerBegin?.Invoke();
    }

    protected virtual void Held()
    {
        OnPointerHeld?.Invoke();
    }



}
