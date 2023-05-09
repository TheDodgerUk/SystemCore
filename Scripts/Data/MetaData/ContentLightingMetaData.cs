using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentLighting)]
public class ContentLightingMetaData : MetaData
{
    [System.Serializable]
    public class GameObjectLink
    {
        [SerializeField]
        public string m_CapabilityType = "";
        [SerializeField]
        public string m_GameObjectName = "";

#if HouseBuilder
        public Fixtures.CapabilityType m_CapabilityTypeEnum;
#endif

        [System.NonSerialized]
        public GameObject m_GameObject;
    }

    [System.Serializable]
    public class GameObjectLinks
    {
        [SerializeField]
        public List<GameObjectLink> m_GameObjectLink = new List<GameObjectLink>();
    }

    [SerializeField]
    public string m_JsonFileName = "";

#if HouseBuilder
    [System.NonSerialized]
    public Fixtures.Fixture m_Fixure = new Fixtures.Fixture();
#endif

    [SerializeField]
    public List<GameObjectLinks> m_GameObjectLinks = new List<GameObjectLinks>();

    [SerializeField]
    public bool m_InvertTilt = false;

    [SerializeField]
    public bool m_InvertPan = false;

    [SerializeField]
    public float m_SpeedTilt = 400f;

    [SerializeField]
    public float m_SpeedPan = 400f;

#if !CATALOG_PROGRAM && HouseBuilder
    public void CollectAllData(InteractionGameObject inter)
    {
        if (this.m_Fixure != null || this.m_Fixure.Modes != null)
        {
            this.m_GameObjectLinks.Resize(this.m_Fixure.Modes.Count);
            for (int i = 0; i < this.m_Fixure.Modes.Count; i++)
            {
                if (this.m_GameObjectLinks[i] == null)
                {
                    this.m_GameObjectLinks[i] = new ContentLightingMetaData.GameObjectLinks();
                }

                this.m_GameObjectLinks[i].m_GameObjectLink.Resize(this.m_Fixure.Modes[i].Channels.Count);
                for (int j = 0; j < this.m_Fixure.Modes[i].Channels.Count; j++)
                {
                    if (this.m_GameObjectLinks[i].m_GameObjectLink[j] == null)
                    {
                        this.m_GameObjectLinks[i].m_GameObjectLink[j] = new ContentLightingMetaData.GameObjectLink();
                    }
                }
            }


            for (int i = 0; i < this.m_Fixure.Modes.Count; i++)
            {
                for (int j = 0; j < this.m_Fixure.Modes[i].Channels.Count; j++)
                {
                    string name = this.m_Fixure.Modes[i].Channels[j].String;
                    var data = this.m_GameObjectLinks[i].m_GameObjectLink[j];
                    data.m_GameObject = MetaData.StringToGameObject(inter.RootGameObject, data.m_GameObjectName, showError:false);
                    data.m_CapabilityTypeEnum = Fixtures.CapabilityTypeConverter.Singleton.Convert(data.m_CapabilityType);
                }
            }

        }
    }
#endif
}