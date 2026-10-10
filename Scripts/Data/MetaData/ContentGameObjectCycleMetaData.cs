using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentGameObjectCycle)]
public class ContentGameObjectCycleMetaData : MetaData
{
    [System.Serializable]
    public class CycleData
    {
        [System.NonSerialized]
        public GameObject m_GameObject;

        [SerializeField]
        public string m_GameObjectName;
    }


    public List<CycleData> m_CycleData = new List<CycleData>();
}