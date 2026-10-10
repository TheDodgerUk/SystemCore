using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenericMessage
{
    private GenericMessageMono m_GenericErrorMono;
    public GenericMessage(GenericMessageMono genericErrorMono)
    {
        m_GenericErrorMono = genericErrorMono;
    }

    public void DisplayError(string message, float time, Action callback)
    {
        m_GenericErrorMono.DisplayMessageTimer(message, time, callback);
    }

    public void DisplayMessage(string message, string message1, Action callback)
    {
        m_GenericErrorMono.DisplayMessage(message, message1, callback);
    }

    public void DisplayMessage(string message, string message1, string message2, Action callback1, Action callback2)
    {
        m_GenericErrorMono.DisplayMessage(message, message1, message2, callback1, callback2);
    }
}



public class GenericMessageMono : MonoBehaviour
{

    private TextMeshProUGUI m_HeaderMessage;
    private Canvas m_Canvas;
    private Button m_Button1;
    private Button m_Button2;
    private TextMeshProUGUI m_Message1;
    private TextMeshProUGUI m_Message2;

    void Awake()
    {
        m_HeaderMessage = this.transform.Search("HeaderMessage").GetComponent<TextMeshProUGUI>();
        m_Button1 = this.transform.Search("Button1").GetComponent<Button>();
        m_Button2 = this.transform.Search("Button2").GetComponent<Button>();
        m_Canvas = this.transform.Search("Canvas").GetComponent<Canvas>();

        m_Message1 = m_Button1.GetComponentInChildren<TextMeshProUGUI>();
        m_Message2 = m_Button2.GetComponentInChildren<TextMeshProUGUI>();
        m_Canvas.SetActive(false);
    }


    public void DisplayMessageTimer(string message, float time, Action callback)
    {
        m_HeaderMessage.text = message;
        m_Canvas.SetActive(true);
        m_Button1.SetActive(false);
        m_Button2.SetActive(false);
        this.WaitFor(time, () =>
        {
            m_Canvas.SetActive(false);
            callback?.Invoke();
        });
    }

    public void DisplayMessage(string message, string button1, Action callbackButton1)
    {
        m_HeaderMessage.text = message;
        m_Canvas.SetActive(true);
        m_Button1.SetActive(true);
        m_Button2.SetActive(false);

        m_Message1.text = button1;
        m_Button1.onClick.RemoveAllListeners();
        m_Button1.onClick.AddListener(() =>
        {
            m_Canvas.SetActive(false);
            callbackButton1?.Invoke();
        });

    }


    public void DisplayMessage(string message, string button1, string button2, Action callbackButton1, Action callbackButton2)
    {
        m_HeaderMessage.text = message;
        m_Canvas.SetActive(true);
        m_Button1.SetActive(true);
        m_Button2.SetActive(true);

        m_Message1.text = button1;
        m_Button1.onClick.RemoveAllListeners();
        m_Button1.onClick.AddListener(() =>
        {
            m_Canvas.SetActive(false);
            callbackButton1?.Invoke();
        });

        m_Message2.text = button2;
        m_Button2.onClick.RemoveAllListeners();
        m_Button2.onClick.AddListener(() =>
        {
            m_Canvas.SetActive(false);
            callbackButton2?.Invoke();
        });
    }

}
