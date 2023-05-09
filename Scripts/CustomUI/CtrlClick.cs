using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CtrlClick : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onCtrlClick = new UnityEvent();
    public UnityEvent onNonCtrlClick = new UnityEvent();

    public void OnPointerDown(PointerEventData data)
    {
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            onCtrlClick?.Invoke();
        }
        else
        {
            onNonCtrlClick?.Invoke();
        }
    }

}
