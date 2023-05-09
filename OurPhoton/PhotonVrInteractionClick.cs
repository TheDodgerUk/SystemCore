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

    [AddComponentMenu("Photon Networking/PhotonVrInteractionClick")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonVrInteractionClick : PhotonVrInteraction
    {
        private VrInteractionClickCallBack m_VrInteractionClickCallBack;
        private void Start()
        {
            m_VrInteractionClickCallBack = this.GetComponent<VrInteractionClickCallBack>();
        }

        protected new byte EventID => (byte)EventIDEnum.Click;


        public void SendPhotonBegin()
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InteranlSendBegin(State.Begin, options, null);
        }


        private void InteranlSendBegin(State state, SendOptions sendoptions, RaiseEventOptions raiseEventOptions = null)
        {
            object[] fullData = new object[] { m_VrInteractionClickCallBack.GuidRef, state };
            PhotonNetwork.RaiseEvent(EventID, fullData, raiseEventOptions, sendoptions);
        }


        public override void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventID) return;

            object[] objectData = (object[])photonEvent.CustomData;


            string createdGuid = (string)objectData[0];
            State state = (State)objectData[1];

            if (m_VrInteractionClickCallBack.GuidRef.Equals(createdGuid) == true)
            {
                switch (state)
                {
                    case State.Begin:
                        m_VrInteractionClickCallBack.PhontonCallback_Begin();
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
#endif