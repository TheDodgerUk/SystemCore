using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionLight : VrInteraction
{
    private Light m_Light;
    ContentLightMetaData m_ContentAnimationMetaData;



    public void Initialise(ContentLightMetaData data, GameObject objectAttachedTo)
    {
        m_ContentAnimationMetaData = data;
        for (int i = 0; i < data.m_EmissiveData.Count; i++)
        {
            if (string.IsNullOrEmpty(data.m_EmissiveData[i].m_GameObjectName) == false)
            {
                data.m_EmissiveData[i].m_GameObject = objectAttachedTo.transform.Search(data.m_EmissiveData[i].m_GameObjectName).gameObject;
            }
            else
            {
                Debug.LogError($"{gameObject.name}  m_EmissiveData gameObject is invalid");
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_Light = this.GetComponentInChildren<Light>(true);
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (null != m_Light)
        {
            m_Light.enabled = !m_Light.enabled;
            for (int i = 0; i < m_ContentAnimationMetaData.m_EmissiveData.Count; i++)
            {
                var data = m_ContentAnimationMetaData.m_EmissiveData[i];
                SetEmission(data.m_GameObject, data.m_MaterialIndex, m_Light.enabled);
            }
        }
    }


    private void SetEmission(GameObject obj, int matIndex,bool enable)
    {
        if (obj != null)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] sharedMaterialsCopy = renderer.sharedMaterials;

                Material newMat = new Material(sharedMaterialsCopy[matIndex]);

                if(enable == true)
                {
                    newMat.EnableKeyword("_EMISSION");
                }
                else
                {
                    newMat.DisableKeyword("_EMISSION");
                }
               
                sharedMaterialsCopy[matIndex] = newMat;
                renderer.sharedMaterials = sharedMaterialsCopy;
            }

        }

    }

}
