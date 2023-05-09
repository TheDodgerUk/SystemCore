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
    using System;
    using UnityEngine;

    [AddComponentMenu("Photon Networking/PhotonVrInteractionSlider")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonVrInteractionSlider : PhotonVrInteraction
    {
        private VrInteractionSlider m_VrInteractionSlider;
        private void Start()
        {
            m_VrInteractionSlider = this.GetComponent<VrInteractionSlider>();
        }


        protected new byte EventID => (byte)EventIDEnum.Slider;


        public void SendPhotonBegin()
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InternalSendBegin(State.Begin, options, null);
        }

        public void SendPhotonEnd()
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InternalSendBegin(State.End, options, null);
        }

        public void SendPhotonUpdate(Vector3 position)
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InternalSendUpdate(position, options, null);
        }

        private void InternalSendBegin(State state, SendOptions sendoptions, RaiseEventOptions raiseEventOptions = null)
        {
            object[] fullData = new object[] { m_VrInteractionSlider.GuidRef, state };
            PhotonNetwork.RaiseEvent(EventID, fullData, raiseEventOptions, sendoptions);
        }


        private void InternalSendUpdate(Vector3 localPosition, SendOptions sendoptions, RaiseEventOptions raiseEventOptions = null)
        {
            object[] fullData = new object[] { m_VrInteractionSlider.GuidRef, State.Update, localPosition };
            PhotonNetwork.RaiseEvent(EventID, fullData, raiseEventOptions, sendoptions);
        }

        public override void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventID) return;

            object[] objectData = (object[])photonEvent.CustomData;

            string createdGuid = (string)objectData[0];
            State state = (State)objectData[1];

            if (createdGuid.Equals(m_VrInteractionSlider.GuidRef)  == true)
            {
                switch (state)
                {
                    case State.Begin:
                        m_VrInteractionSlider.PhontonCallback_Begin();
                        break;
                    case State.Update:
                        var pos =(Vector3)objectData[2];
                        m_VrInteractionSlider.PhontonCallback_Update(pos);
                        break;
                    case State.End:
                        m_VrInteractionSlider.PhontonCallback_End();
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
#endif