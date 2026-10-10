using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteractionGameObjectCycle : VrInteraction
{

    private GameObject m_RootGameObject;
    private ContentGameObjectCycleMetaData m_ContentGameObjectCycleMetaData;
    int m_CurrentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }

    public void Initialise(ContentGameObjectCycleMetaData data, GameObject objAttachedTo)
    {
        m_ContentGameObjectCycleMetaData = data;
        try
        {
            for (int i = 0; i < data.m_CycleData.Count; i++)
            {
                data.m_CycleData[i].m_GameObject = this.transform.Search(data.m_CycleData[i].m_GameObjectName).gameObject;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in ContentGameObjectCycleMetaData   {objAttachedTo.name}    , {e.Message}");
        }
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_CurrentIndex = m_CurrentIndex.WrapIncrement(m_ContentGameObjectCycleMetaData.m_CycleData);
        SetCorrectGameObjects();
    }

    public override void ResetToOriginalState()
    {
        m_CurrentIndex = 0;
        SetCorrectGameObjects();
    }

    private void SetCorrectGameObjects()
    {
        m_ContentGameObjectCycleMetaData.m_CycleData.ForEach(e => e.m_GameObject.SetActive(false));
        m_ContentGameObjectCycleMetaData.m_CycleData[m_CurrentIndex].m_GameObject.SetActive(true);
    }


}
