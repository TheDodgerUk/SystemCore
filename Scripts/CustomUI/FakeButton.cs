using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FakeButton : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onClick = new UnityEvent();

    public void OnPointerDown(PointerEventData data)
    {
        onClick?.Invoke();
    }
}
