using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float m_Timer = 0;
    private float m_TimeAmount = 0;
    private Button m_Button;
    private UnityAction m_OnEnter;
    private UnityAction m_OnExit;

    private IEnumerator m_TimerCoroutine;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_Timer = 0;
        m_TimerCoroutine = Timer();
        StartCoroutine(m_TimerCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_TimerCoroutine != null)
        {
            StopCoroutine(m_TimerCoroutine);
        }
        m_Timer = 0;
        m_OnExit?.Invoke();
    }

    public void SetOnHover(Button button, float timeAmount, UnityAction enter, UnityAction exit)
    {
        m_Button = button;
        m_OnEnter = enter;
        m_OnExit = exit;
        m_TimeAmount = timeAmount;
    }

    private IEnumerator Timer()
    {
        while(m_Timer < m_TimeAmount)
        {
            yield return new WaitForEndOfFrame();
            m_Timer += Time.deltaTime;
        }
        m_OnEnter?.Invoke();
    }

}
