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

    [AddComponentMenu("Photon Networking/PhotonVrInteractionButton")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonVrInteractionButton: PhotonVrInteraction
    {
        private VrInteractionButtonLatched m_VrInteractionButtonLatched;
        private void Start()
        {
            m_VrInteractionButtonLatched = this.GetComponent<VrInteractionButtonLatched>();
        }
        protected new byte EventID => (byte)EventIDEnum.Button;


        public void SendPhotonBegin()
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InternalSendData(options, null);
        }
        private void InternalSendData(SendOptions sendoptions, RaiseEventOptions raiseEventOptions = null)
        {
            if (Core.PhotonMultiplayerRef.PhotonViewOwnerRef != null)
            {
                object[] fullData = new object[] { m_VrInteractionButtonLatched.GuidRef };
                PhotonNetwork.RaiseEvent(EventID, fullData, raiseEventOptions, sendoptions);
            }
            else
            {
                Debug.Log($"Item does not have PhotonViewRef");
            }
        }

        public override void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventID) return;
            object[] objectData = (object[])photonEvent.CustomData;

            string createdGuid = (string)objectData[0];

            if (m_VrInteractionButtonLatched.GuidRef.Equals(createdGuid) == true)
            {
                m_VrInteractionButtonLatched.PhontonCallback_Begin();
            }
        }

    }
}
#endif