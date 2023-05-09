using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LeftRightClick : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onRightClick = new UnityEvent();
    public UnityEvent onLeftClick = new UnityEvent();

    public void OnPointerDown(PointerEventData data)
    {
        if(data.button == PointerEventData.InputButton.Right)
        {
            onRightClick?.Invoke();
        }
        else
        {
            onLeftClick?.Invoke();
        }
    }

}
