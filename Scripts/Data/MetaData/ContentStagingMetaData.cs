using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentStaging)]
public class ContentStagingMetaData : MetaData
{
    public enum ConectionType
    {
        Ladder = 2,// 2 * 180 is full rotation
        Triangle = 3,// 3 * 120 is full rotation
        Square = 4, // 4 * 90 is full rotation
        Rectangle = 12, // i wnt this to be 2, but it leeds to issues, so i made it 12 and get it back to 2 in WrapIncrementRotationIndex
    }
    public class StagingData : InstructionData
    {
        [SerializeField]
        public int m_OwnIndex = 0;


        [SerializeField]
        public int m_RotationIndex = 0;

        [System.NonSerialized]
        public Transform m_OriginalParent;
        [System.NonSerialized]
        public Quaternion m_OriginalQuaternion;
        [System.NonSerialized]
        public Vector3 m_OriginalPosition;
        [System.NonSerialized]
        public StagingData m_ConnectedTo;
    }

    [SerializeField]
    public int m_CurrentPoint = 0;


    [SerializeField]
    public List<StagingData> m_StagingData = new List<StagingData>();

    [SerializeField]
    public ConectionType m_ConectionType = ConectionType.Triangle;

    public override void CollectAllData(GameObject root)
    {
        for (int i = 0; i < m_StagingData.Count; i++)
        {
            StagingData item = m_StagingData[i];
            item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
            if (item.m_ModelGameObject != null)
            {
                item.m_OriginalParent = item.m_ModelGameObject.transform.parent;

                item.m_OriginalQuaternion = item.m_ModelGameObject.transform.localRotation;
                item.m_OriginalPosition = item.m_ModelGameObject.transform.localPosition;
            }
            item.m_OwnIndex = i;
        }
    }

    public StagingData CurrentData => m_StagingData[m_CurrentPoint];
    public int StagingDataCount => m_StagingData.Count;
    public int CurrentPointIndex => m_CurrentPoint;


    public void WrapIncrementCurrentPoint()
    {
        m_CurrentPoint = m_CurrentPoint.WrapIncrement(m_StagingData);
    }

    public void WrapDecrementCurrentPoint()
    {
        m_CurrentPoint = m_CurrentPoint.WrapDecrement(m_StagingData);
    }

    public void WrapIncrementRotationIndex()
    {
        // to cope with Rectangle = 12, 
        int number = (int)m_ConectionType;
        int lastDigit = number % (10);
        CurrentData.m_RotationIndex = CurrentData.m_RotationIndex.WrapIncrement(lastDigit);
    }

    public void WrapDecrementRotationIndex()
    {
        int number = (int)m_ConectionType;
        int lastDigit = number % (10);
        CurrentData.m_RotationIndex = CurrentData.m_RotationIndex.WrapDecrement(lastDigit);
    }

}