using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionCollision : VrInteraction
{
#if Photon
    protected PhotonVrInteractionSlider m_PhotonVrInteractionSlider;
#endif

    private ContentCollisionMetaData.CollisionData m_Data;

    private Action<bool, VrInteraction> m_EnterCallbackBaseGuid;
    private Action<bool, VrInteraction> m_ExitCallbackBaseGuid;

    private Action<bool, VrInteraction> m_EnterCallbackBaseVrInteraction;
    private Action<bool, VrInteraction> m_ExitCallbackBaseVrInteraction;


    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialise(GameObject root, ContentCollisionMetaData.CollisionData data)
    {
        m_Data = data;
    }

    public void SetEnterCallbackBaseGuid(Action<bool, VrInteraction> callback) => m_EnterCallbackBaseGuid = callback;
    public void SetExitCallbackBaseGuid(Action<bool, VrInteraction> callback) => m_ExitCallbackBaseGuid = callback;


    private void OnCollisionEnter(Collision collision)
    {
        var vr = collision.gameObject.GetComponentInParent<VrInteraction>();
        var found = m_Data.m_ValidGuids.Find(e => e == vr.CatalogueEntryRef.Guid); 
        m_EnterCallbackBaseGuid?.Invoke(found != null, vr);
    }

    private void OnCollisionExit(Collision collision)
    {
        var vr = collision.gameObject.GetComponentInParent<VrInteraction>();
        var found = m_Data.m_ValidGuids.Find(e => e == vr.CatalogueEntryRef.Guid);
        m_ExitCallbackBaseGuid?.Invoke(found != null, vr);
    }

    private void OnTriggerEnter(Collider collision)
    {
        var vr = collision.gameObject.GetComponentInParent<VrInteraction>();
        var found = m_Data.m_ValidGuids.Find(e => e == vr.CatalogueEntryRef.Guid);
        m_EnterCallbackBaseGuid?.Invoke(found != null, vr);
    }

    private void OnTriggerExit(Collider collision)
    {
        var vr = collision.gameObject.GetComponentInParent<VrInteraction>();
        var found = m_Data.m_ValidGuids.Find(e => e == vr.CatalogueEntryRef.Guid);
        m_ExitCallbackBaseGuid?.Invoke(found != null, vr);
    }
}
