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

    [AddComponentMenu("Photon Networking/PhotonVrInteractionDial")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonVrInteractionDial : PhotonVrInteraction
    {
        private VrInteractionDial m_VrInteractionDial;
        private void Start()
        {
            m_VrInteractionDial = this.GetComponent<VrInteractionDial>();
        }
        protected new byte EventID => (byte)EventIDEnum.Dial;

        public void SendPhotonBegin()
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InteranlSendBegin(State.Begin, options, null);
        }

        public void SendPhotonEnd()
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InteranlSendBegin(State.End, options, null);
        }

        public void SendPhotonUpdate(Vector3 localEulerAngles)
        {
            SendOptions options = new SendOptions();
            options.DeliveryMode = DeliveryMode.Reliable;
            InternalSendUpdate(localEulerAngles, options, null);
        }

        private void InteranlSendBegin(State state, SendOptions sendoptions, RaiseEventOptions raiseEventOptions = null)
        {
            object[] fullData = new object[] { m_VrInteractionDial.GuidRef, state };
            PhotonNetwork.RaiseEvent(EventID, fullData, raiseEventOptions, sendoptions);
        }


        private void InternalSendUpdate(Vector3 localPosition, SendOptions sendoptions, RaiseEventOptions raiseEventOptions = null)
        {
            object[] fullData = new object[] { m_VrInteractionDial.GuidRef, State.Update, localPosition };
            PhotonNetwork.RaiseEvent(EventID, fullData, raiseEventOptions, sendoptions);
        }

        public override void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventID) return;

            object[] objectData = (object[])photonEvent.CustomData;

            string createdGuid = (string)objectData[0];
            State state = (State)objectData[1];

            if (m_VrInteractionDial.GuidRef.Equals(createdGuid) == true)
            {
                switch (state)
                {
                    case State.Begin:
                        SendMessage("PhontonCallback_Begin");
                        break;
                    case State.Update:
                        SendMessage("PhontonCallback_OnUpdate", (Vector3)objectData[2]);
                        break;
                    case State.End:
                        SendMessage("PhontonCallback_End");
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
#endif