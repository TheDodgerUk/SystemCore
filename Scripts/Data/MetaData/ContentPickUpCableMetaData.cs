using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentPickUpCable)]
public class ContentPickUpCableMetaData : ContentPickUpSocketMetaData
{

    public class CableData : SocketData
    {
        [SerializeField]
        public float m_RopeWidth = 0.01f;


        [System.NonSerialized]
        public GameObject m_GameObjectPickUpCableStart;
        [SerializeField]
        public string m_GameObjectPickUpCableStartName;

        [System.NonSerialized]
        public GameObject m_GameObjectFixedCableEnd;
        [SerializeField]
        public string m_GameObjectFixedCableEndName;

        [SerializeField]
        public float m_MaxDistance = 1;

    }

    public new List<CableData> m_Data = new List<CableData>();


    public override void CollectAllData(GameObject root)
    {
        base.CollectAllData(root);
        for (int i = 0; i < m_Data.Count; i++)
        {
            m_Data[i].m_GameObjectPickup = StringToGameObject(root, m_Data[i].m_GameObjectPickUpName);
            m_Data[i].m_GameObjectPickUpSocket = StringToGameObject(root, m_Data[i].m_GameObjectPickUpSocketName);
            if (m_Data[i].m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
            {
                m_Data[i].m_GameObjectSocket = StringToGameObject(root, m_Data[i].m_GameObjectSocketName, false);
            }

            m_Data[i].m_GameObjectPickUpCableStart = StringToGameObject(root, m_Data[i].m_GameObjectPickUpCableStartName);
            m_Data[i].m_GameObjectFixedCableEnd = StringToGameObject(root, m_Data[i].m_GameObjectFixedCableEndName);

#if !CATALOG_PROGRAM
            m_Data[i].m_OriginalData = m_Data[i].m_GameObjectPickup.GetTransformData();
#endif

        }
    }

}