using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenericLoading
{
    private GenericLoadingMono m_GenericLoadingMono;

    public GenericLoading(GenericLoadingMono genericLoadingMono) => m_GenericLoadingMono = genericLoadingMono;
    public void SetActive(bool active) => m_GenericLoadingMono.m_Canvas.SetActive(active);
    public void OnMessage(string message) => m_GenericLoadingMono.m_Message.text = message;
    public void OnProgress(float progress) => m_GenericLoadingMono.m_Slider.value = progress;
}



public class GenericLoadingMono: MonoBehaviour
{
    [HideInInspector]
    public TextMeshProUGUI m_Message;

    [HideInInspector]
    public Slider m_Slider;

    [HideInInspector]
    public Canvas m_Canvas;

    void Awake()
    {
        m_Message = this.transform.Search("Message").GetComponent<TextMeshProUGUI>();
        m_Canvas = this.transform.Search("Canvas").GetComponent<Canvas>();
        m_Slider = this.GetComponentInChildren<Slider>(true);

        m_Canvas.SetActive(false);
    }

}
