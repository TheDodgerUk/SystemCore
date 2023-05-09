#if Photon
// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbodyView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize rigidbodies via PUN.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using Autohand;
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static VrInteractionPickUp;

    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Photon Networking/PhotonRigidbodyTurnonOffView")]
    public class PhotonRigidbodyTurnonOffView : MonoBehaviourPun, IPunObservable
    {

        //////private const float TURN_OFF_SNYC = 4f;
        //////private const int NO_OWNER = -100;


        private float m_Distance;
        private float m_Angle;

        private Rigidbody m_Rigidbody;

        private Vector3 m_NetworkPosition;

        private Quaternion m_NetworkRotation;
        private int m_NetworkMessageActorNumber;

        [HideInInspector]
        public bool m_SynchronizeVelocity = true;
        [HideInInspector]
        public bool m_SynchronizeAngularVelocity = true;

        [HideInInspector]
        public bool m_TeleportEnabled = true;
        [HideInInspector]
        public float m_TeleportIfDistanceGreaterThan = 3.0f;


        private PickupMessage m_PickupMessage = new PickupMessage();
        private bool m_IsSyncing = false;


        private VrInteractionPickUp m_VrInteraction;


        public void Awake()
        {
            this.m_Rigidbody = GetComponent<Rigidbody>();

            this.m_NetworkPosition = new Vector3();
            this.m_NetworkRotation = new Quaternion();

            m_VrInteraction = this.GetComponent<VrInteractionPickUp>();

            Core.PhotonMultiplayerRef.OnRoomPlayersChanged((players) => PlayersChanged(players));
        }

        private void PlayersChanged(List<Player> players)
        {
            if (m_IsSyncing == true)
            {
                var player = players.Find(e => e.ActorNumber == m_PickupMessage.MessageActorNumber);
                if(player == null) // player no longer here
                {
                    m_PickupMessage.PickUp = PickUpEnum.Release;
                    PickUpData(m_PickupMessage);
                }
            }
        }


        public void PickUpData(PickupMessage pickupMessage)
        {
            m_PickupMessage = pickupMessage;

            if (m_VrInteraction != null)
            {
                if(m_VrInteraction.transform.gameObject.activeInHierarchy == false)
                {
                    // if not visable make it 
                    m_VrInteraction.transform.SetActive(true);
                }
 
                switch (m_PickupMessage.PickUp)
                {
                    case PickUpEnum.Grabbed:
                        Core.PhotonMultiplayerRef.AddItemToSync(this);
                        m_IsSyncing = true;
                        if (m_PickupMessage.MessageActorNumber != Core.PhotonMultiplayerRef.MySelf.ActorNumber)
                        {
                            m_VrInteraction.SetActive(true); // this is for other side
                            m_VrInteraction.SetPhysicsToGrabRemote();
                        }
                        break;
                    case PickUpEnum.Release:
                    case PickUpEnum.ReleaseUpdate:
                        Core.PhotonMultiplayerRef.RemoveItemToSync(this);
                        m_IsSyncing = false;
                        if (m_PickupMessage.MessageActorNumber != Core.PhotonMultiplayerRef.MySelf.ActorNumber)
                        {
                            Debug.LogError($"PickupMessage {m_PickupMessage.PickUp}   MessageActorNumber : {m_PickupMessage.MessageActorNumber}  MySelf: {Core.PhotonMultiplayerRef.MySelf.ActorNumber}");
                            m_VrInteraction.SetPhysicsToLocalControl(m_PickupMessage);
                        }

                        break;
                    default:
                        break;
                }
            }
        }


        public void FixedUpdate()
        {
            if (m_IsSyncing == true)
            {
                if ((Core.PhotonMultiplayerRef.IsOwnerInRoom == true)) // multiplayer
                {
                    if ((m_PickupMessage.MessageActorNumber != Core.PhotonMultiplayerRef.MySelf.ActorNumber)) // i the one who NOT sent it 
                    {
                        this.m_Rigidbody.position = Vector3.MoveTowards(this.m_Rigidbody.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                        this.m_Rigidbody.rotation = Quaternion.RotateTowards(this.m_Rigidbody.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                    }
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (Core.PhotonMultiplayerRef.PhotonViewOwnerRef != null)
                {
                    stream.SendNext(Core.PhotonMultiplayerRef.MySelf.ActorNumber);
                }
                else
                {
                    stream.SendNext(-1);
                }
                stream.SendNext(this.m_Rigidbody.position);
                stream.SendNext(this.m_Rigidbody.rotation);

                if (this.m_SynchronizeVelocity)
                {
                    stream.SendNext(this.m_Rigidbody.velocity);
                }

                if (this.m_SynchronizeAngularVelocity)
                {
                    stream.SendNext(this.m_Rigidbody.angularVelocity);
                }
            }
            else
            {
                if (stream.IsPeakNextValid() == false)
                {
                    return;
                }
                this.m_NetworkMessageActorNumber = (int)stream.ReceiveNext();
                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                if (this.m_TeleportEnabled)
                {
                    if (Vector3.Distance(this.m_Rigidbody.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                    {
                        this.m_Rigidbody.position = this.m_NetworkPosition;
                    }
                }

                if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    if (this.m_SynchronizeVelocity)
                    {
                        this.m_Rigidbody.velocity = (Vector3)stream.ReceiveNext();

                        this.m_NetworkPosition += this.m_Rigidbody.velocity * lag;

                        this.m_Distance = Vector3.Distance(this.m_Rigidbody.position, this.m_NetworkPosition);
                    }

                    if (this.m_SynchronizeAngularVelocity)
                    {
                        this.m_Rigidbody.angularVelocity = (Vector3)stream.ReceiveNext();

                        this.m_NetworkRotation = Quaternion.Euler(this.m_Rigidbody.angularVelocity * lag) * this.m_NetworkRotation;

                        this.m_Angle = Quaternion.Angle(this.m_Rigidbody.rotation, this.m_NetworkRotation);
                    }
                }
            }
        }
    }
}
#endif