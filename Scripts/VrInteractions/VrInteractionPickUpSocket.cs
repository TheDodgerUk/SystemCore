using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionPickUpSocket : VrInteractionPickUp
{
    private Action<bool> m_OnSocketed;
    public bool IsSocketed { get;  set; }

    private ContentPickUpSocketMetaData.SocketData m_MetaData;


    private bool m_CheckDistance;
    private float m_CheckDistanceAmount;
    private Collider m_GenericSocketCollider;

    public void OnSocketed(Action<bool> onSocketed)
    {
        m_OnSocketed = onSocketed;
    }


    public void Initialise(GameObject root, ContentPickUpSocketMetaData.SocketData data)
    {
        m_MetaData = data;

        m_MetaData.m_GameObjectPickUpRigidbody = data.m_GameObjectPickup.ForceComponent<Rigidbody>();
        m_MetaData.m_GameObjectPickUpRigidbody.mass = data.m_Weight;
        IsSocketed = data.IsSocketed;
        var trigger = data.m_GameObjectPickUpSocket.ForceComponent<TriggerCallback>();
        if (m_MetaData.m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
        {
            Collider col = data.m_GameObjectSocket.GetComponents<Collider>().ToList().Find(e => e.isTrigger == true);
            trigger.OnCallback(col, OnTriggerEnter, OnTriggerExit);
        }
        else
        {
            trigger.OnCallback(data.m_GameObjectSocketName, OnTriggerEnter, OnTriggerExit);
        }      
    }


    protected override void InternalGameStarted()
    {
        base.InternalGameStarted();
        m_MetaData.m_GameObjectPickUpRigidbody.useGravity = !IsSocketed;
        m_MetaData.m_GameObjectPickUpRigidbody.isKinematic = IsSocketed;
    }



    private void OnTriggerEnter(Collider col)
    {
        if ((HasGameStarted == true) && (m_MetaData != null) && (m_MetaData.m_GameObjectSocket != null) && (col.gameObject == m_MetaData.m_GameObjectSocket))
        {
            if (m_MetaData.m_GameObjectPickUpRigidbody != null)
            {
                if (IsSocketed == false)
                {
                    m_CheckDistance = false;
#if VR_INTERACTION
                    var hand = CameraControllerVR.Instance.HandsRef.Find(e => (e.holdingObj != null) && (e.holdingObj.gameObject == m_MetaData.m_GameObjectPickup));
                    if (hand != null)
                    {
                        m_MetaData.m_GameObjectPickUpRigidbody.isKinematic = true;
                        m_MetaData.m_GameObjectPickup.ApplyTransformData(m_MetaData.m_OriginalData, TransformDataEnum.AllLocal);
                        IsSocketed = true;
                        m_OnSocketed?.Invoke(IsSocketed);


                        // DO not do this, becasause the hand will close and break stuff //hand.OpenHand();
                        this.GrabbableRef.HandRelease(hand);
                        hand.ForceReleaseGrab();
                    }
#endif
   
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if ((HasGameStarted == true) && (m_MetaData != null) && (m_MetaData.m_GameObjectSocket != null) && (col.gameObject == m_MetaData.m_GameObjectSocket))
        {
            if (m_MetaData.m_GameObjectPickUpRigidbody != null)
            {
                if (IsSocketed == true && m_CheckDistance == false)
                {
                    m_CheckDistance = true;
                    m_GenericSocketCollider = col;
                    m_CheckDistanceAmount = Vector3.Distance(m_MetaData.m_GameObjectPickup.transform.position, FixedSocketPosition());
                    m_CheckDistanceAmount *= 2;
                }
            }
        }
    }



    protected virtual void Update()
    {
        //Exit
        if (IsSocketed == true && m_CheckDistance == true)
        {
            float amount = Vector3.Distance(m_MetaData.m_GameObjectPickup.transform.position, FixedSocketPosition());

            if(amount > m_CheckDistanceAmount)
            {
                IsSocketed = false;
                m_CheckDistance = false;
                m_OnSocketed?.Invoke(IsSocketed);
            }
        }
    }


    public void ForceDisonnect()
    {
        if(IsSocketed == true && m_MetaData.m_GameObjectPickUpRigidbody.isKinematic == true)
        {
            IsSocketed = false;
            m_MetaData.m_GameObjectPickUpRigidbody.useGravity = true;
            m_MetaData.m_GameObjectPickUpRigidbody.isKinematic = false;
            m_MetaData.m_GameObjectPickUpRigidbody.AddForce(m_MetaData.m_GameObjectPickup.transform.forward * 5, ForceMode.Force);

#if VR_INTERACTION
            var pickUp = m_MetaData.m_GameObjectPickUpRigidbody.GetComponent<VrInteractionPickUp>();
            pickUp.Grab();
            Core.Mono.WaitFor(2, () => { pickUp.Release(); });
#endif
        }
    }

    protected Vector3 FixedSocketPosition()
    {
        if (m_MetaData.m_GameObjectSocket != null)
        {
            return m_MetaData.m_GameObjectSocket.transform.position;
        }
        else
        {
            return m_GenericSocketCollider.transform.position;
        }
    }
}
