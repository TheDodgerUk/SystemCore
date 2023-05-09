using System;
using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy;
using UnityEngine;
using UnityEngine.UI;

public class VisualLogger
{
    public enum InformationType
    {
        Avatars,
        Other,
    }

    public enum Information
    {
        Information_1,
        Information_2,
        Information_3,
        Information_4,
        Information_5,
        Information_6,
        Information_7,
        Information_8,
        Information_9,
    }

    public class TextData
    {
        public GameObject m_Group;
        public List<Text> m_TextList = new List<Text>();
    }
    public Text m_Header;

    public InformationType m_CurrentType = InformationType.Avatars;
    public Dictionary<InformationType, TextData> m_DisplayLinks = new Dictionary<InformationType, TextData>();

    private VisualLoggerMono m_VisualLoggerMono;
    public float CameraDistance { get; set; } = 2f;

    public VisualLogger(VisualLoggerMono visualLoggerMono)
    {
        m_VisualLoggerMono = visualLoggerMono;

        var mainGroup = visualLoggerMono.transform.SearchComponent<Transform>("Grouped");

        foreach (InformationType m_InformationType in Enum.GetValues(typeof(InformationType)))
        {
            var init = GameObject.Instantiate(mainGroup, m_VisualLoggerMono.transform);

            m_Header = init.transform.SearchComponent<Text>("InformationType");
            m_Header.text = $"{m_InformationType.ToString()},    Application.version : {Application.version}: LoopDisplayType to change it";

            var values = Enum.GetValues(typeof(Information));

            m_DisplayLinks.Add(m_InformationType, new TextData());
            foreach (Information m_Information in Enum.GetValues(typeof(Information)))
            {
                var text = init.transform.SearchComponent<Text>(m_Information.ToString());
                if (text != null)
                {
                    m_DisplayLinks[m_InformationType].m_TextList.Add(text);
                    text.text = "";
                }
            }
            m_DisplayLinks[m_InformationType].m_Group = init.gameObject;
        }
        mainGroup.transform.SetActive(false);
        Display(m_CurrentType);
    }

    public void Show(bool enable)
    {
        m_VisualLoggerMono.SetActive(enable);
        m_VisualLoggerMono.gameObject.PlaceInfrontMainCamera(CameraDistance);
    }


    public bool IsActive => m_VisualLoggerMono.isActiveAndEnabled;
    public void Toggle()
    {
        m_VisualLoggerMono.SetActive(!m_VisualLoggerMono.isActiveAndEnabled);
        m_VisualLoggerMono.gameObject.PlaceInfrontMainCamera(CameraDistance);
    }

    public void SetInformation(InformationType type, Information information, string info)
    {
        int number = (int)information;
        number++; // make it start from 1

        m_DisplayLinks[type].m_TextList[(int)information].text = $"{number}: {info}";
    }
    public void Display(InformationType type)
    {
        m_CurrentType = type;
        foreach (InformationType foo in Enum.GetValues(typeof(InformationType)))
        {
            m_DisplayLinks[foo].m_Group.gameObject.SetActive(false);
        }
        m_DisplayLinks[type].m_Group.gameObject.SetActive(true);
    }
    public void LoopDisplayType()
    {
        int current = (int)m_CurrentType;
        current++;
        int count = Enum.GetValues(typeof(InformationType)).Length;
        if(current > (count-1))
        {
            current = 0;
        }
        Display((InformationType)current);
    }

}
