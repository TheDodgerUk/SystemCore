using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable, MetaData(MetaDataType.ContentParticleSystem)]
public class ContentParticleSystemMetaData : MetaData
{

    [System.NonSerialized]
    public VisualEffect m_ParticleSystemVisualEffect;

    [System.NonSerialized]
    public GameObject m_ParticleSystemGameObject;

    [SerializeField]
    public string m_ParticleSystemName = "";

    public override void CollectAllData(GameObject root)
    {
        m_ParticleSystemGameObject = StringToGameObject(root, m_ParticleSystemName);
        if (string.IsNullOrEmpty(m_ParticleSystemName) == false)
        {
            m_ParticleSystemVisualEffect = m_ParticleSystemGameObject.GetComponent<VisualEffect>();
        }

    }
}