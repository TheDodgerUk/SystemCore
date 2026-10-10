using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentSlider)]
public class ContentSliderMetaData : MetaData
{
    [System.Serializable]
    public class ItemData : InstructionData
    {
        [SerializeField]
        public enum MoveType
        {
            GameObject,
            Amount,
        }

        [SerializeField]
        public MoveType m_MoveType = MoveType.GameObject;

        [System.NonSerialized]
        public GameObject m_ModelEndLocalPositionGameObject;

        [SerializeField]
        public string m_ModelEndLocalPositionGameObjectName;

        [SerializeField]
        public float m_ModelEndLocalPositionfloat;


        public Vector3 m_StartLocalPosition;
    }

    [SerializeField]
    public List<ItemData> m_ItemData = new List<ItemData>();

    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            foreach (var item in m_ItemData)
            {
                item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
                if (item.m_ModelGameObject != null)
                {
                    if (string.IsNullOrEmpty(item.m_ModelEndLocalPositionGameObjectName) == false)
                    {
                        item.m_ModelEndLocalPositionGameObject = StringToGameObject(root, item.m_ModelEndLocalPositionGameObjectName);
                    }
                    item.m_StartLocalPosition = item.m_ModelGameObject.transform.localPosition;
                }
                else
                {
                    Debug.LogError($"Slider error, {root.name} , cannot finf {item.m_ModelGameObjectName}", root);
                }
            }
        }
    }


}