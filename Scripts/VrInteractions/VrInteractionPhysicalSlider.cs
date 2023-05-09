using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VrInteractionButtonLatched;

public class VrInteractionPhysicalSlider : VrInteractionBaseSlider
{

    [SerializeField]
    private float m_CurrentPercentage = 0f;

    [ReadOnlyAttribute, SerializeField]
    private bool m_AllowUpdate = false;

    private ContentSliderMetaData.ItemData m_Data;
    private Vector3 m_EndPosition;

    private GameObject m_StartGameObject;
    private GameObject m_EndGameObject;

    private Vector3 m_LocalPosition;


    public override void SetPercentageCallbackAmount(float percentage)
    {
        ///igidbody.position 
        this.transform.position = Vector3.Lerp(m_StartGameObject.transform.position, m_EndGameObject.transform.position, percentage);
        UpdateCurrentPercentage(percentage, allowCallback: true);
    }

    public override void SetPercentageNoCallbackAmount(float percentage)
    {
        ///igidbody.position 
        this.transform.position = Vector3.Lerp(m_StartGameObject.transform.position, m_EndGameObject.transform.position, percentage);
        UpdateCurrentPercentage(percentage, allowCallback: false);
    }

    public void Initialise(GameObject root, ContentSliderMetaData.ItemData data, Action<float> callBack)
    {
        m_Data = data;
        m_PercentageCallback = callBack;


        m_StartGameObject = new GameObject("m_StartGameObject");
        m_StartGameObject.transform.SetParent(this.transform.parent.parent);
        m_StartGameObject.transform.ClearLocals();

        VrInteractionSlider.CollectData(m_StartGameObject, ref m_EndPosition, data);
        m_EndGameObject = new GameObject("m_EndGameObject");
        m_EndGameObject.transform.SetParent(this.transform.parent.parent);
        m_EndGameObject.transform.ClearLocals();
        m_EndGameObject.transform.localPosition = m_EndPosition;

        SetPercentageCallbackAmount(0f);
        this.WaitForFrames(2, () =>
        {
            m_AllowUpdate = true;
        });


        var render = this.GetComponent<Renderer>();
        if (render != null)
        {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        render = m_Data.m_ModelGameObject.GetComponent<Renderer>();
        if (render != null)
        {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void Update()
    {
        if (m_LocalPosition == this.transform.localPosition)
        {
            // optimization
            return;
        }
        m_LocalPosition = this.transform.localPosition;


        if (m_PercentageCallback != null)
        {
            if (m_AllowUpdate == true)
            {
                float springValue = GetNormalisedPressed();
                if (m_CurrentPercentage != springValue)
                {
                    UpdateCurrentPercentage(springValue, allowCallback:true);
                }
            }
        }
    }

    private void UpdateCurrentPercentage(float amount, bool allowCallback)
    {
        if (allowCallback == true)
        {
            var tt = GetNormalisedPressed();
            m_CurrentPercentage = amount;
            SliderPercentageValue = m_CurrentPercentage;
            m_PercentageCallback?.Invoke(m_CurrentPercentage);
            if (m_CurrentPercentage >= 0.9020173f)
            {
                Debug.LogError($"m_CurrentPercentage {m_CurrentPercentage}");
            }
        }
    }

    private float GetNormalisedPressed()
    {
        float maxDistance = Vector3.Distance(m_StartGameObject.transform.position, m_EndGameObject.transform.position);
        float currentDistance = Vector3.Distance(m_StartGameObject.transform.position, this.transform.position);


        float percentage = currentDistance / maxDistance;

        return percentage;
    }
}

