using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DeskFan : MonoBehaviour
{
    // audio 
    private AudioSource m_AudioSource;
    private float m_MaxAudioVolume = 0f;
    private float m_MaxAudioPitch = 0f;

    // base
    private float m_BaseSpeed;
    private GameObject m_Base;

    // blades
    private float m_MaxBladeSpeed = 1000f;
    private GameObject m_Blades;

    // speed change
    private float m_SpeedIncrementAmount = 0.5f;
    private float m_CurrentPercentage = 0f;
    private bool m_SpeedUp = false;


    // buttons
    private VrInteraction m_RootVrInteraction;

    // animations
    private List<PlayableDirector> m_TimeLines = new List<PlayableDirector>();


    [InspectorButton]
    private void SpeedUp() => m_SpeedUp = true;

    [InspectorButton]
    private void SpeedDown() => m_SpeedUp = false;


    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>(); 
        if (m_AudioSource != null)
        {
            m_MaxAudioVolume = m_AudioSource.volume;
            m_MaxAudioPitch = m_AudioSource.pitch;
        }

    }

    void Update()
    {
        if (m_RootVrInteraction == null)
        {
            m_RootVrInteraction = this.GetComponent<VrInteraction>();
            if (m_RootVrInteraction != null)
            {
                this.WaitForFrames(3, () =>
                {
                    Setup();
                });
            }
        }
        else
        {
            UpdateSpeeds();
        }
    }

    private void Setup()
    {
        m_TimeLines = this.GetComponentsInChildren<PlayableDirector>().ToList();

        var offButton = m_RootVrInteraction.GetVrInteractionFromRoot(MetaDataType.ContentButton,"Button_Off");
        if (offButton != null)
        {
            ((VrInteractionBaseButton)offButton).ClearAddCallback((pressed) =>
            {
                if (pressed == VrInteractionBaseButton.ButtonStateEnum.Down)
                {
                    Debug.LogError("DeskFan OFF");
                    m_SpeedUp = false;
                }
            });
        }

        var onButton = m_RootVrInteraction.GetVrInteractionFromRoot(MetaDataType.ContentButton, "Button_On");
        if (offButton != null)
        {
            ((VrInteractionBaseButton)offButton).ClearAddCallback((pressed) =>
            {
                if (pressed == VrInteractionBaseButton.ButtonStateEnum.Down)
                {
                    /////Debug.LogError("DeskFan Button_On");
                    m_SpeedUp = true;
                }
            });
        };

        var onOffButton = m_RootVrInteraction.GetVrInteractionFromRoot(MetaDataType.ContentButton, "Button_OnOff");
        if (onOffButton != null)
        {
            ((VrInteractionBaseButton)offButton).ClearAddCallback((pressed) =>
            {
                if (pressed == VrInteractionBaseButton.ButtonStateEnum.Down)
                {
                    Debug.LogError("DeskFan Button_OnOff");
                    m_SpeedUp = !m_SpeedUp;
                }
            });
        };

        m_Blades = this.transform.SearchComponent<Transform>("Blades").gameObject;
    }

    private void UpdateSpeeds()
    {
        UpdateCurrentPercentage();

        if (m_Blades != null)
        {
            float bladeSpeed = Mathf.Lerp(0f, m_MaxBladeSpeed, m_CurrentPercentage);
            m_Blades.transform.Rotate(Vector3.forward, bladeSpeed * Time.deltaTime, Space.Self);
        }

        if(m_AudioSource != null)
        {
            m_AudioSource.volume = m_MaxAudioVolume * m_CurrentPercentage;
            m_AudioSource.pitch = m_MaxAudioPitch * m_CurrentPercentage;
        }

        foreach (var item in m_TimeLines)
        {
            item.playableGraph.GetRootPlayable(0).SetSpeed(m_CurrentPercentage);
        }
    }

    private void UpdateCurrentPercentage()
    {
        if (m_SpeedUp == false)
        {
            m_CurrentPercentage -= m_SpeedIncrementAmount * Time.deltaTime;
        }
        else
        {
            m_CurrentPercentage += m_SpeedIncrementAmount * Time.deltaTime;
        }

        m_CurrentPercentage = Mathf.Clamp01(m_CurrentPercentage);
    }
}
