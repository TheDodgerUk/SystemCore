using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionPickUpCable : VrInteractionPickUpSocket
{
    public ContentPickUpCableMetaData.CableData m_MetaData { get; private set; }

    private const float SNAP_POWER = 0.9f;
    private float m_SnapTimer = 0;
    private const float MAX_SNAP_TIMER = 1f;


    public void Initialise(GameObject root, ContentPickUpCableMetaData.CableData data)
    {
        base.Initialise(root, data);
        m_MetaData = data;

        var rope = root.AddComponent<RopeControllerSimple>();
        rope.SetRopeLength(data.m_MaxDistance, data.m_RopeWidth);
        rope.whatTheRopeIsConnectedTo = data.m_GameObjectPickUpCableStart.transform;
        rope.whatIsHangingFromTheRope = data.m_GameObjectFixedCableEnd.transform;

        GrabbableRef = this.gameObject.ForceComponent<Autohand.Grabbable>();
        this.GrabbableRef.onGrab.AddListener(Grabbed);

    }

    private void Grabbed(Autohand.Hand arg0, Autohand.Grabbable arg1)
    {
        m_Rigidbody.useGravity = true;
        m_Rigidbody.isKinematic = false;
    }

    protected override void Update()
    {
        base.Update();

        // snapping when cable to long
        var distance = Vector3.Distance(m_MetaData.m_GameObjectFixedCableEnd.transform.position, m_MetaData.m_GameObjectPickup.transform.position);
        if((distance > m_MetaData.m_MaxDistance) && (m_SnapTimer == 0))
        {
#if VR_INTERACTION
            Debug.LogError("ForceReleaseGrab");
            var hand = CameraControllerVR.Instance.HandsRef.Find(e => e.holdingObj.gameObject == m_MetaData.m_GameObjectPickup);
            if (hand != null)
            {
                // DO not do this, becasause the hand will close and break stuff //hand.OpenHand();
                this.GrabbableRef.HandRelease(hand);
                hand.ForceReleaseGrab();
            }
#endif
            m_SnapTimer = MAX_SNAP_TIMER;
            m_MetaData.m_GameObjectPickUpRigidbody.velocity = -m_MetaData.m_GameObjectPickUpRigidbody.velocity * SNAP_POWER;
            if(Vector3.Magnitude(m_MetaData.m_GameObjectPickUpRigidbody.velocity) < 0.01f)
            {
                var direction = (m_MetaData.m_GameObjectFixedCableEnd.transform.position - m_MetaData.m_GameObjectPickup.transform.position).normalized;
                m_MetaData.m_GameObjectPickUpRigidbody.velocity = (direction * 0.1f);
            }
        }
        m_SnapTimer -= Time.deltaTime;
        m_SnapTimer = Mathf.Clamp(m_SnapTimer, 0f, MAX_SNAP_TIMER);
     }


}
