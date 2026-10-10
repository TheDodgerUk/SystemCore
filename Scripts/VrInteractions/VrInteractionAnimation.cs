using EnvironmentHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteractionAnimation : VrInteraction
{
    private GameObject m_RootGameObject;
    private ContentAnimationMetaData m_ContentAnimationMetaData;
    private int m_CurrentAnimationIndex;


    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }


    public void Initialise(ContentAnimationMetaData data, GameObject objectAttachedTo)
    {
        m_ContentAnimationMetaData = data;
        ResetTriggers();
    }

     
    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        var data = m_ContentAnimationMetaData.m_AnimationData.FindLast(e => e.AnimationCollider == interaction.Main.HitCollider);
        
        Debug.Log($"{m_ContentAnimationMetaData.m_AnimationType}, plays: '{m_ContentAnimationMetaData.m_AnimationData[m_CurrentAnimationIndex].AnimationName}',  outOf:{m_ContentAnimationMetaData.m_AnimationData.Count}");
        if (m_ContentAnimationMetaData.m_AnimationType == ContentAnimationMetaData.AnimationType.Trigger)
        {
            data.AnimationAnimator.SetTrigger(data.AnimationName[m_CurrentAnimationIndex]);           
        }
        else
        {
            data.AnimationAnimator.Play(data.AnimationName[m_CurrentAnimationIndex]);
        }
        m_CurrentAnimationIndex = m_CurrentAnimationIndex.WrapIncrement(m_ContentAnimationMetaData.m_AnimationData);
    }


    private void ResetTriggers()
    {
        if (m_ContentAnimationMetaData.m_AnimationType == ContentAnimationMetaData.AnimationType.Trigger)
        {
            this.WaitForFrames(1, () =>
            {
                foreach(var item in m_ContentAnimationMetaData.m_AnimationData)
                {
                    foreach (var names in item.AnimationName)
                    {
                        item.AnimationAnimator.ResetTrigger(names);
                    }
                }
            });
        }
    }

}

