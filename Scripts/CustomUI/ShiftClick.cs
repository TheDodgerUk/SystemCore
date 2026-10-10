using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShiftClick : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onShiftClick = new UnityEvent();
    public UnityEvent onNonShiftClick = new UnityEvent();

    public void OnPointerDown(PointerEventData data)
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            onShiftClick?.Invoke();
        }
        else
        {
            onNonShiftClick?.Invoke();
        }
    }

}
