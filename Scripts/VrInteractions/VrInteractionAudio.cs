using EnvironmentHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if HouseBuilder
using static HouseBuilder.HouseBuilderConstants.AudioData;
#endif

public class VrInteractionAudio : VrInteraction
{
    private GameObject m_RootGameObject;
    private ContentAudioMetaData m_ContentAudioMetaData;


    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }

    public void Initialise(ContentAudioMetaData data, GameObject objAttachedTo)
    {
        m_ContentAudioMetaData = data;

        try
        {
            for (int i = 0; i < data.m_AudioDatas.Count; i++)
            {
                data.m_AudioDatas[i].m_BaseSoundGameObject = this.transform.Search(data.m_AudioDatas[i].m_BaseSoundGameObjectName).gameObject;

                data.m_AudioDatas[i].m_SoundCollider = StringToComponent<Collider>(data.m_AudioDatas[i].m_SoundColliderName, this.gameObject);
                if (data.m_AudioDatas[i].m_SoundCollider == null)
                {
                    Debug.LogError("Collider has not been set up correctly");
                }

                var group = data.m_AudioDatas[i];
                for (int x = 0; x < group.m_AudioSubGroupData.Count; x++)
                {
                    var individualData = group.m_AudioSubGroupData[x];
                    individualData.m_AudioSource = StringToComponent<AudioSource>(individualData.m_AudioSourceName, this.gameObject);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Error in ContentAudioMetaData   {objAttachedTo.name}    , {e.Message}");
        }
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        var hitItem = m_ContentAudioMetaData.m_AudioDatas.Find(e => e.m_SoundCollider == interaction.Main.HitCollider);
        var audioRandom = hitItem.m_AudioSubGroupData.GetRandom();
        if (audioRandom != null && audioRandom.m_AudioSource != null)
        {
            audioRandom.m_AudioSource.Play();
        }
        else
        {
            Debug.LogError("audioRandom was null", this);
        }
    }
}

