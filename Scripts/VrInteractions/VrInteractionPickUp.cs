using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VR_INTERACTION
using Autohand;
#endif
#if Photon
using Photon.Pun;
#endif

public class VrInteractionPickUp : VrInteraction
{
    public enum PickUpEnum
    {
        Grabbed,
        Release,
        ReleaseUpdate,
    }

    public class PickupMessage
    {
        public int MessageActorNumber;
        public PickUpEnum PickUp;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public Vector3 Position;
        public Quaternion Rotation;
    }


    protected Rigidbody m_Rigidbody;


#if Photon
    public PhotonRigidbodyTurnonOffView m_PhotonRigidbodyTurnonOffView;
#endif

#if VR_INTERACTION
    public Autohand.Grabbable GrabbableRef { get; set; }
    public Autohand.GrabbablePose GrabbablePoseRef { get; private set; }
#endif
    private List<CollisionSound> m_CollisionSounds = new List<CollisionSound>();


    private ContentPickUpMetaData m_ContentPickUpMetaData;

    private bool m_AllowPickup = true;

    public bool AllowPickup
    {
        get
        {
            return m_AllowPickup;
        }
        set
        {
            m_AllowPickup = value;
#if VR_INTERACTION
            GrabbableRef.enabled = m_AllowPickup;
#endif
        }
    }
    public Rigidbody Body => m_Rigidbody;


    protected override void Awake()
    {
        base.Awake();
#if Photon
        m_PhotonRigidbodyTurnonOffView = this.gameObject.ForceComponent<PhotonRigidbodyTurnonOffView>();
#endif


        m_CollisionSounds = this.GetComponentsInChildren<CollisionSound>().ToList();

        m_Rigidbody = this.gameObject.ForceComponent<Rigidbody>();
        m_Rigidbody.useGravity = true;
        m_Rigidbody.isKinematic = false;
        var root = this.transform.SearchComponent<Transform>(GlobalConsts.RootModel, false);
        if (root != null)
        {
            var direct = root.GetDirectChildren();
            foreach (var item in direct)
            {
                if (item.name == GlobalConsts.CenterOfMass)
                {
                    m_Rigidbody.centerOfMass = item.localPosition;
                    break;
                }
            }
        }
        else
        {
            if (this.GetType() != typeof(VrInteractionPickUpCable) && this.GetType() != typeof(VrInteractionPickUpSocket))
            {
                Debug.LogError($"no GlobalConsts.RootModel {GlobalConsts.RootModel}  {this.gameObject.GetGameObjectPath()}", this.gameObject);
            }
        }

#if VR_INTERACTION
        GrabbableRef = this.gameObject.ForceComponent<Autohand.Grabbable>();
        GrabbableRef.instantGrab = false;
        GrabbableRef.useGentleGrab = true;



        // for demo and to make it feel better
        GrabbableRef.grabType = HandGrabType.GrabbableToHand;
        GrabbableRef.ignoreWeight = true;
        GrabbableRef.jointBreakForce = float.PositiveInfinity;
        GrabbableRef.singleHandOnly = true;

        GrabbablePoseRef = this.gameObject.GetComponent<Autohand.GrabbablePose>();
        if (GrabbablePoseRef != null)
        {
            GrabbableRef.singleHandOnly = true;
            GrabbableRef.grabType = HandGrabType.GrabbableToHand;
            if (GrabbablePoseRef.poseScriptable == null)
            {
                Debug.LogError($"GrabbablePoseRef.poseScriptable", this.gameObject);
            }
            if (GrabbablePoseRef.poseScriptable != null)
            {
                if (GrabbablePoseRef.poseScriptable.leftPose.posePositions.Length != GrabbablePoseRef.poseScriptable.rightPose.posePositions.Length)
                {
                    Debug.LogError($"Left and right not same size", this.gameObject);
                }

                // i "think" it should be 20 
                if (GrabbablePoseRef.poseScriptable.leftPose.posePositions.Length != 20 || GrabbablePoseRef.poseScriptable.rightPose.posePositions.Length != 20)
                {
                    Debug.LogError($"leftPose should be 20 :  {GrabbablePoseRef.poseScriptable.leftPose.posePositions.Length}", this.gameObject);
                    Debug.LogError($"rightPose should be 20 : {GrabbablePoseRef.poseScriptable.rightPose.posePositions.Length}", this.gameObject);
                }

            }


        }

        m_Rigidbody.useGravity = false;
        m_Rigidbody.isKinematic = true;

        GrabbableRef.onGrab = new UnityHandGrabEvent();
        GrabbableRef.onGrab.AddListener(Grabbed);

        GrabbableRef.onRelease = new UnityHandGrabEvent();
        GrabbableRef.onRelease.AddListener(Release);
#endif
#if Photon
        Core.PhotonGenericRef.CollectGenericVrInterationMessage<PickupMessage>(this, (pickupMessage) =>
        {
            m_PhotonRigidbodyTurnonOffView.PickUpData(pickupMessage);
        });
#endif
    }

#if Photon &&  VR_INTERACTION
    [EditorButton]
    private void DEBUG_GRABBED() => Grabbed(CameraControllerVR.Instance.HandRightRef, null);

    [EditorButton]
    private void DEBUG_RELEASE() => Release(CameraControllerVR.Instance.HandRightRef, null);

    public void ForceSync() => Release(null, null);    // this is so the sleep can work 

    public void Grab() => Grabbed(CameraControllerVR.Instance.HandRightRef, null);
    public void Release() => Release(CameraControllerVR.Instance.HandRightRef, null);

    private void Grabbed(Hand hand, Grabbable grab)
    {
        this.ActorNickNameTouched = Core.PhotonMultiplayerRef.MySelf.NickName;
        this.ActorNickNamePosition = hand.transform.position;
        if (Core.PhotonMultiplayerRef.CurrentRoom != null)
        {
            Core.PhotonMultiplayerRef.AddItemToSync(m_PhotonRigidbodyTurnonOffView); // duplicates to solve the release tracking issue
            PickupMessage newMessage = CreateMessage(grab, PickUpEnum.Grabbed);
            Core.PhotonGenericRef.SendGenericVrInterationMessage<PickupMessage>(this, newMessage, Photon.Realtime.ReceiverGroup.Others);
        }

    }

    private void Release(Hand hand, Grabbable grab)
    {
        if (Core.PhotonMultiplayerRef.CurrentRoom != null)
        {
            Core.PhotonMultiplayerRef.RemoveItemToSync(m_PhotonRigidbodyTurnonOffView); // duplicates to solve the release tracking issue
            PickupMessage newMessage = CreateMessage(grab, PickUpEnum.Release);
            Core.PhotonGenericRef.SendGenericVrInterationMessage<PickupMessage>(this, newMessage, Photon.Realtime.ReceiverGroup.Others);

            // need extra 
            Core.Mono.WaitForFrames(15, () =>
            {
                if (grab.IsHeld() == false)
                {
                    newMessage.PickUp = PickUpEnum.ReleaseUpdate;
                    Core.PhotonGenericRef.SendGenericVrInterationMessage<PickupMessage>(this, newMessage, Photon.Realtime.ReceiverGroup.Others); 
                }
            });

        }
    }

    private PickupMessage CreateMessage(Grabbable grab, PickUpEnum pickup)
    {
        PickupMessage newMessage = new PickupMessage();
        newMessage.MessageActorNumber = Core.PhotonMultiplayerRef.MySelf.ActorNumber;
        newMessage.PickUp = pickup;
        newMessage.velocity = grab.body.velocity;
        newMessage.angularVelocity = grab.body.angularVelocity;
        newMessage.Position = grab.body.position;
        newMessage.Rotation = grab.body.rotation;
        return newMessage;
    }

#endif

    public void SetVolumePercenatgeOfOriginal(float percentage)
    {
        foreach (var item in m_CollisionSounds)
        {
            item.SetVolumeAmp(percentage);
        }
    }

    public override void ResetToOriginalState()
    {
        try
        {
            foreach (var item in m_CollisionSounds)
            {
                item.ResetToOriginalState();
            }
            this.gameObject.ApplyTransformData(m_TransformData, TransformDataEnum.AllLocal);
            InternalGameStarted();
        }
        catch (Exception e)
        {
            Debug.LogError($"this.gameObject {this.gameObject.name}");
        }
    }

    protected override void InternalGameStarted()
    {
        base.InternalGameStarted();
        SetPhysicsToLocalControl();
    }

    public void Initialise(ContentPickUpMetaData data)
    {
        m_ContentPickUpMetaData = data;
        m_Rigidbody.mass = m_ContentPickUpMetaData.m_WeightKG;
#if VR_INTERACTION
        GrabbableRef.throwPower = m_ContentPickUpMetaData.m_ThrowPower;
        foreach (var item in m_ContentPickUpMetaData.m_IgnoreColliders)
        {
            GrabbableRef.IgnoreColliders(item.ColliderObj);
        }

#endif
    }



    public void SetPhysicsToGrabRemote()
    {
        m_Rigidbody.useGravity = false;
        m_Rigidbody.isKinematic = true;
    }


    public void SetPhysicsToLocalControl()
    {
        try
        {
            m_Rigidbody.useGravity = true;
            m_Rigidbody.isKinematic = !AllowPickup;
        }
        catch(Exception e)
        {
            DebugBeep.LogError($"not found, {this.name}", DebugBeep.MessageLevel.High, this.gameObject);
        }
    }

    public void SetPhysicsToLocalControl(PickupMessage message)
    {
        SetPhysicsToLocalControl();
        if (m_Rigidbody.isKinematic == false)
        {
#if VR_INTERACTION
            GrabbableRef.body.velocity = message.velocity;
            GrabbableRef.body.angularVelocity = message.angularVelocity;
            GrabbableRef.body.position = message.Position;
            GrabbableRef.body.rotation = message.Rotation;
#endif
        }
    }

    
}
