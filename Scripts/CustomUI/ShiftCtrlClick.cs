using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShiftCtrlClick : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onShiftClick = new UnityEvent();
    public UnityEvent onCtrlClick = new UnityEvent();
    public UnityEvent onNonClick = new UnityEvent();

    public void OnPointerDown(PointerEventData data)
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            onShiftClick?.Invoke();
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            onCtrlClick?.Invoke();
        }
        else
        {
            onNonClick?.Invoke();
        }
    }

}
