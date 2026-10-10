#if Photon
// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    using UnityEngine;

    [AddComponentMenu("Photon Networking/PhotonVrInteraction")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonVrInteraction : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public enum EventIDEnum
        {
            GenericHeader,
            GenericHeaderAndData,
            Slider,
            Button,
            Dial,
            Click,
            Pickup,
            RigidBody,
            VrInteractionMessage,
            VrInteractionInt,
            DebugInt,
            DebugIntData,
            AchievmentData,
            LeaderBoardData,
            UserData,
            VRCreation,
        }



        public enum State
        {
            Begin,
            Update,
            End,
        }



        public bool CanSendMessage() => Core.PhotonMultiplayerRef.PhotonViewOwnerRef != null;
        protected byte EventID => (byte)EventIDEnum.Button;

        public override void OnEnable()
        {

            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        

        public virtual void OnEvent(EventData photonEvent)
        {

        }

    }
}
#endif