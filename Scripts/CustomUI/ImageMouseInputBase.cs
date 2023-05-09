using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class ImageMouseInputBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float HOLD_TIMER = 0.1f;

    private float m_HeldTimer;
    private float m_DoubleTapTimer = 0;
    private bool IsPointerDown = false;

    public UnityEvent OnPointerTap = new UnityEvent();
    public UnityEvent OnPointerDoubleTap = new UnityEvent();
    public UnityEvent OnRightClick = new UnityEvent();

    public UnityEvent OnPointerBegin = new UnityEvent();
    public UnityEvent OnPointerHeld = new UnityEvent();
    public UnityEvent OnPointerEnd = new UnityEvent();

    public PointerEventData.InputButton m_Button = PointerEventData.InputButton.Middle;

    public bool OnEndWhenLeaveObject = true; 

    public void OnPointerDown(PointerEventData data)
    {
        m_Button = data.button;
        if (data.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke();
            return;
        }
        OnPointerBegin?.Invoke();
        m_DoubleTapTimer = 0;
        if(m_DoubleTapTimer != 0  && m_DoubleTapTimer < HOLD_TIMER)
        {
            OnPointerDoubleTap?.Invoke();
            m_DoubleTapTimer = 100;
        }
        IsPointerDown = true;
    }


    public void OnPointerUp(PointerEventData data)
    {
       
        if (OnEndWhenLeaveObject == true)
        {
            End();
        }
        else
        {
            if (Input.GetMouseButton((int)m_Button) == false)
            {
                End();
            }
        }
    }

    private void End()
    {
        if (m_HeldTimer <= HOLD_TIMER)
        {
            OnPointerTap?.Invoke();
        }
        OnPointerEnd?.Invoke();
        IsPointerDown = false;
        m_HeldTimer = 0;
    }


    void Update()
    {
        m_DoubleTapTimer += Time.deltaTime;
        if (IsPointerDown == true)
        {
            m_HeldTimer += Time.deltaTime;
            
            if (m_HeldTimer > HOLD_TIMER)
            {
                m_DoubleTapTimer = 100;
                OnPointerHeld?.Invoke();
            }

            if (OnEndWhenLeaveObject == false && Input.GetMouseButton((int)m_Button) == false)
            {
                End();
            }
        }
    }
}
