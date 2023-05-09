using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentAnimation)]
public class ContentAnimationMetaData : MetaData
{
    [System.Serializable]
    public class StringItemData
    {
        [System.NonSerialized]
        public GameObject AnimationGameObject;

        [SerializeField]
        public string AnimationGameObjectName;

        [System.NonSerialized]
        public Animator AnimationAnimator;

        [System.NonSerialized]
        public Collider AnimationCollider;


        [SerializeField]
        public List<string> AnimationName = new List<string>();
    }

    [System.Serializable]
    public enum AnimationType
    {
        Trigger,
        Animation,
    }


    [SerializeField]

    public List<StringItemData> m_AnimationData;
    public AnimationType m_AnimationType = AnimationType.Animation;

    public ContentAnimationMetaData()
    {
        m_AnimationData = new List<StringItemData>();
    }

    public override void CollectAllData(UnityEngine.GameObject root)
    {
        for (int i = 0; i < m_AnimationData.Count; i++)
        {
            m_AnimationData[i].AnimationGameObject = StringToGameObject(root, m_AnimationData[i].AnimationGameObjectName);
            m_AnimationData[i].AnimationAnimator = m_AnimationData[i].AnimationGameObject.GetComponent<Animator>();
            m_AnimationData[i].AnimationCollider = m_AnimationData[i].AnimationGameObject.GetComponentInChildren<Collider>();
        }
    }
}