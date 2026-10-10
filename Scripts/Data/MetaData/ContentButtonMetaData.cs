using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentButton)]
public class ContentButtonMetaData : MetaData
{
    public enum SimpleButtonType
    {
        None,
        SetActive,
        Animation,
        Emission,
    }

    [System.Serializable]
    public class ButtonItemData : ContentSliderMetaData.ItemData
    {
        public enum SimpleButtonPress
        {
            Toggle,
            On,
            Off
        }
        public enum ButtonType
        {
            Toggle, // Press button, it goes down, then up, state changes once (!state).
            Hold, // Press and hold button, button goes down, state changes (1), release button, button goes up, state changes (0).
            Latched, // Press button and release, button goes down, stays down, state changes (1), Press button and release, button goes up, stays up, state changes (0).
        }

        [SerializeField]
        public float m_ModelMoveTime = 0.3f;

        [SerializeField]
        public bool m_UseEmmision = true;

        [SerializeField]
        public Color m_Up = Color.white;

        [SerializeField]
        public Color m_Down = Color.white;


        [SerializeField]
        public ButtonType m_ButtonType = ButtonType.Toggle;


        [SerializeField]
        public SimpleButtonType m_SimpleButtonType = SimpleButtonType.None;
        [SerializeField]
        public SimpleButtonPress m_SimpleButtonPress = SimpleButtonPress.Toggle;

        public GameObjectData m_SetActive = new GameObjectData();
        public AnimationGameObjectData m_SetAnimation = new AnimationGameObjectData();
        public RendererGameObjectData m_SetEmmissive = new RendererGameObjectData();
    }

    [SerializeField]
    public List<ButtonItemData> m_ItemData = new List<ButtonItemData>();


    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            foreach (var item in m_ItemData)
            {
                item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
                if (string.IsNullOrEmpty(item.m_ModelEndLocalPositionGameObjectName) == false)
                {
                    item.m_ModelEndLocalPositionGameObject = StringToGameObject(root, item.m_ModelEndLocalPositionGameObjectName);
                }

                item.m_SetActive.CollectAllData(root);
                item.m_SetAnimation.CollectAllData(root);
                item.m_SetEmmissive.CollectAllData(root);
            }
        }
    }

}