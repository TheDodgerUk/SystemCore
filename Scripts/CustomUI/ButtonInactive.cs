using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonInactive : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnClick = new UnityEvent();
    //Detect if a click occurs
    private Button m_Button;
    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(m_Button.interactable == false)
        {
            OnClick?.Invoke();
        }
    }
}
