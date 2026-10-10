using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteractionPivot : VrInteraction
{

    private GameObject m_RootGameObject;
    private ContentPivotMetaData m_ContentPivotMetaData;

    private Vector3 m_ModelPivotRotation = Vector3.zero;



    protected override void Awake()
    {
        base.Awake();
        m_RootGameObject = this.gameObject;
    }

    public void Initialise(ContentPivotMetaData data, GameObject objAttachedTo)
    {
        m_ContentPivotMetaData = data;

        try
        {
            for (int i = 0; i < data.m_PivotData.Count; i++)
            {
                data.m_PivotData[i].m_ModelPivotGameObject = this.transform.Search(data.m_PivotData[i].m_ModelPivotName).gameObject;
                data.m_PivotData[i].m_ModelPivotCollider = data.m_PivotData[i].m_ModelPivotGameObject.GetComponentInChildren<Collider>();
#if HouseBuilder
                if (data.m_PivotData[i].m_ModelPivotCollider == null)
                {
                    var inter = m_RootGameObject.GetComponent<Interaction>();
                    UnityEngine.Debug.LogError($"Pivot data is incorrect on {inter.CatalogueEntryRef.ShortName}, missing colliders on pivot items: {i}");
                }
#endif
                data.m_PivotData[i].m_ModelPivotRotation = data.m_PivotData[i].m_ModelPivotGameObject.transform.localEulerAngles;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in ContentPivotMetaData   {objAttachedTo.name}    , {e.Message}");
        }
    }

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {

        var hitItem = m_ContentPivotMetaData.m_PivotData.Find(e => e.m_ModelPivotCollider == interaction.Main.HitCollider);

        if (hitItem != null)
        {
            if (true == hitItem.m_PivotInteractionsAllowed)
            {
                hitItem.m_PivotInteractionsAllowed = false;
                if (hitItem.m_PivotState == ContentPivotMetaData.StringItemData.PivotState.ToOpen)
                {
                    ToOpen(hitItem);
                }
                else
                {
                    ToClose(hitItem);
                }
            }
        }
        else
        {
#if HouseBuilder
            var inter = interaction.Main.HitCollider.gameObject.GetComponentInParent<Interaction>();
            UnityEngine.Debug.LogError($"Pivot data on {inter.CatalogueEntryRef.ShortName}, is invalid, check catalogue item");
#endif
        }
    }

    public override void ResetToOriginalState()
    {
        for (int i = 0; i < m_ContentPivotMetaData.m_PivotData.Count; i++)
        {
            m_ContentPivotMetaData.m_PivotData[i].m_ModelPivotGameObject.transform.localEulerAngles = m_ContentPivotMetaData.m_PivotData[i].m_ModelPivotRotation;
            m_ContentPivotMetaData.m_PivotData[i].m_PivotState = ContentPivotMetaData.StringItemData.PivotState.ToOpen;
            m_ContentPivotMetaData.m_PivotData[i].m_PivotInteractionsAllowed = true;
        }

    }

    private void ToOpen(ContentPivotMetaData.StringItemData data)
    {
        this.Create<ValueTween>(data.m_Time, EaseType.SineInOut, () =>
        {
            data.m_PivotState = ContentPivotMetaData.StringItemData.PivotState.ToClose;
            data.m_PivotInteractionsAllowed = true;
        }).Initialise(0f, 1f, (f) =>
        {
            data.m_ModelPivotGameObject.transform.localEulerAngles = Lerp(m_ModelPivotRotation, data.m_Min.WrapTo180(), data.m_Max.WrapTo180(), data.m_Axis, f);
        });
    }

    private void ToClose(ContentPivotMetaData.StringItemData data)
    {
        this.Create<ValueTween>(data.m_Time, EaseType.SineInOut, () =>
        {
            data.m_PivotState = ContentPivotMetaData.StringItemData.PivotState.ToOpen;
            data.m_PivotInteractionsAllowed = true;
        }).Initialise(0f, 1f, (f) =>
        {
            data.m_ModelPivotGameObject.transform.localEulerAngles = Lerp(m_ModelPivotRotation, data.m_Max.WrapTo180(), data.m_Min.WrapTo180(), data.m_Axis, f);
        });
    }

    private Vector3 Lerp(Vector3 normal, float start, float end, Axis axis, float time)
    {
        float lerpAmount = Mathf.LerpAngle(start, end, time);
        switch (axis)
        {
            case Axis.X:
                return new Vector3(lerpAmount, normal.y, normal.z);

            case Axis.Y:
                return new Vector3(normal.x, lerpAmount, normal.z);

            case Axis.Z:
                return new Vector3(normal.x, normal.y, lerpAmount);

            default:
                break;
        }
        return new Vector3(lerpAmount, 0, 0);
    }



}
