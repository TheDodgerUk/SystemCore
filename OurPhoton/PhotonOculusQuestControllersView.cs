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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TriLibCore.Fbx;
    using UnityEngine;

    [AddComponentMenu("Photon Networking/PhotonOculusQuestControllersView")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonOculusQuestControllersView : MonoBehaviourPun, IPunObservable
    {
        //OVRPlugin
        public enum BoneId
        {
            Invalid = -1,

            // hand bones
            Hand_Start = 0,
            Hand_World = Hand_Start + 0, // root frame of the hand, where the wrist is located
            Hand_Hand = Hand_Start + 1, // frame for user's forearm
            Hand_Thumb0 = Hand_Start + 2, // thumb trapezium bone
            Hand_Thumb1 = Hand_Start + 3, // thumb metacarpal bone
            Hand_Thumb2 = Hand_Start + 4, // thumb proximal phalange bone
            Hand_Thumb3 = Hand_Start + 5, // thumb distal phalange bone
            Hand_Index1 = Hand_Start + 6, // index proximal phalange bone
            Hand_Index2 = Hand_Start + 7, // index intermediate phalange bone
            Hand_Index3 = Hand_Start + 8, // index distal phalange bone
            Hand_Middle1 = Hand_Start + 9, // middle proximal phalange bone
            Hand_Middle2 = Hand_Start + 10, // middle intermediate phalange bone
            Hand_Middle3 = Hand_Start + 11, // middle distal phalange bone
            Hand_Ring1 = Hand_Start + 12, // ring proximal phalange bone
            Hand_Ring2 = Hand_Start + 13, // ring intermediate phalange bone
            Hand_Ring3 = Hand_Start + 14, // ring distal phalange bone
            Hand_Pinky0 = Hand_Start + 15, // pinky metacarpal bone
            Hand_Pinky1 = Hand_Start + 16, // pinky proximal phalange bone
            Hand_Pinky2 = Hand_Start + 17, // pinky intermediate phalange bone
            Hand_Pinky3 = Hand_Start + 18, // pinky distal phalange bone
            Hand_MaxSkinnable = Hand_Start + 19,
            // Bone tips are position only. They are not used for skinning but are useful for hit-testing.
            // NOTE: Hand_ThumbTip == Hand_MaxSkinnable since the extended tips need to be contiguous
            Hand_ThumbTip = Hand_MaxSkinnable + 0, // tip of the thumb
            Hand_IndexTip = Hand_MaxSkinnable + 1, // tip of the index finger
            Hand_MiddleTip = Hand_MaxSkinnable + 2, // tip of the middle finger
            Hand_RingTip = Hand_MaxSkinnable + 3, // tip of the ring finger
            Hand_PinkyTip = Hand_MaxSkinnable + 4, // tip of the pinky
            Hand_Grip = Hand_MaxSkinnable + 5,


            // add new bones here

            Max = ((int)Hand_Grip > 50) ? (int)Hand_Grip : 50,
        }

        public enum Hand
        {
            Right,
            Left,
        }

        public Hand m_Hand = Hand.Left;
        private bool m_HasBeenSetup = false;



        private OrderedDictionary<BoneId, Transform> m_SendController = new OrderedDictionary<BoneId, Transform>();
        private OrderedDictionary<BoneId, Transform> m_ReadController = new OrderedDictionary<BoneId, Transform>();



        ////////private void Update()
        ////////{
        ////////    if (m_HasBeenSetup == false)
        ////////    {
        ////////        return;
        ////////    }


        ////////    // Testing only 
        ////////    foreach (BoneId boneKey in Enum.GetValues(typeof(BoneId)))
        ////////    {

        ////////        if (m_SendController.ContainsKey(boneKey) == true)
        ////////        {
        ////////            m_ReadController[boneKey].localRotation = m_SendController[boneKey].localRotation;
        ////////        }
        ////////    }
        ////////}


        public void Awake()
        {
            Transform rootBar = Camera.main.transform.parent.parent;

            Transform sendRoot = rootBar.SearchComponent<Transform>("hand_left");
            if (m_Hand == Hand.Right)
            {
                sendRoot = rootBar.SearchComponent<Transform>("hand_right");
            }

            if (m_Hand == Hand.Right)
            {
                this.transform.localPosition = new Vector3(0.015f, -0.08f, -0.04f);
            }
            else
            {
                this.transform.localPosition = new Vector3(-0.015f, -0.08f, -0.04f);
            }


            if (sendRoot != null) // on mac this will be null
            {
                if (m_Hand == Hand.Left)
                {
                    m_SendController[BoneId.Hand_World] = sendRoot.transform.SearchComponent<Transform>("hands:l_hand_world");
                    m_SendController[BoneId.Hand_Hand] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_hand");
                    m_SendController[BoneId.Hand_Grip] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_grip");


                    m_SendController[BoneId.Hand_Thumb0] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_thumb1");
                    m_SendController[BoneId.Hand_Thumb1] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_thumb2");
                    m_SendController[BoneId.Hand_Thumb2] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_thumb3");

                    m_SendController[BoneId.Hand_Index1] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_index1");
                    m_SendController[BoneId.Hand_Index2] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_index2");
                    m_SendController[BoneId.Hand_Index3] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_index3");

                    m_SendController[BoneId.Hand_Middle1] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_middle1");
                    m_SendController[BoneId.Hand_Middle2] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_middle2");
                    m_SendController[BoneId.Hand_Middle3] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_middle3");

                    m_SendController[BoneId.Hand_Ring1] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_ring1");
                    m_SendController[BoneId.Hand_Ring2] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_ring2");
                    m_SendController[BoneId.Hand_Ring3] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_ring3");


                    m_SendController[BoneId.Hand_Pinky0] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_pinky0");
                    m_SendController[BoneId.Hand_Pinky1] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_pinky1");
                    m_SendController[BoneId.Hand_Pinky2] = sendRoot.transform.SearchComponent<Transform>("hands:b_l_pinky2");
                    // m_Read[BoneId.Hand_Pinky3] = this.transform.SearchComponent<Transform>("hands:b_l_pinky3");
                }
                else
                {
                    m_SendController[BoneId.Hand_World] = sendRoot.transform.SearchComponent<Transform>("hands:r_hand_world");
                    m_SendController[BoneId.Hand_Hand] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_hand");
                    m_SendController[BoneId.Hand_Grip] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_grip");

                    m_SendController[BoneId.Hand_Thumb0] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_thumb1");
                    m_SendController[BoneId.Hand_Thumb1] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_thumb2");
                    m_SendController[BoneId.Hand_Thumb2] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_thumb3");

                    m_SendController[BoneId.Hand_Index1] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_index1");
                    m_SendController[BoneId.Hand_Index2] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_index2");
                    m_SendController[BoneId.Hand_Index3] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_index3");

                    m_SendController[BoneId.Hand_Middle1] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_middle1");
                    m_SendController[BoneId.Hand_Middle2] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_middle2");
                    m_SendController[BoneId.Hand_Middle3] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_middle3");

                    m_SendController[BoneId.Hand_Ring1] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_ring1");
                    m_SendController[BoneId.Hand_Ring2] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_ring2");
                    m_SendController[BoneId.Hand_Ring3] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_ring3");


                    m_SendController[BoneId.Hand_Pinky0] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_pinky0");
                    m_SendController[BoneId.Hand_Pinky1] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_pinky1");
                    m_SendController[BoneId.Hand_Pinky2] = sendRoot.transform.SearchComponent<Transform>("hands:b_r_pinky2");
                    //m_Read[BoneId.Hand_Pinky3] = this.transform.SearchComponent<Transform>("hands:b_r_pinky3");
                }
            }

            ////////////////////////////////////////////////////////////////////////////


            if (m_Hand == Hand.Left)
            {
                m_ReadController[BoneId.Hand_World] = this.transform.SearchComponent<Transform>("LeftHand:hands:l_hand_world");
                m_ReadController[BoneId.Hand_Hand] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_hand");
                m_ReadController[BoneId.Hand_Grip] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_grip");

                m_ReadController[BoneId.Hand_Thumb0] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_thumb1");
                m_ReadController[BoneId.Hand_Thumb1] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_thumb2");
                m_ReadController[BoneId.Hand_Thumb2] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_thumb3");

                m_ReadController[BoneId.Hand_Index1] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_index1");
                m_ReadController[BoneId.Hand_Index2] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_index2");
                m_ReadController[BoneId.Hand_Index3] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_index3");

                m_ReadController[BoneId.Hand_Middle1] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_middle1");
                m_ReadController[BoneId.Hand_Middle2] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_middle2");
                m_ReadController[BoneId.Hand_Middle3] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_middle3");

                m_ReadController[BoneId.Hand_Ring1] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_ring1");
                m_ReadController[BoneId.Hand_Ring2] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_ring2");
                m_ReadController[BoneId.Hand_Ring3] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_ring3");


                m_ReadController[BoneId.Hand_Pinky0] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_pinky0");
                m_ReadController[BoneId.Hand_Pinky1] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_pinky1");
                m_ReadController[BoneId.Hand_Pinky2] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_pinky2");
                //m_Write[BoneId.Hand_Pinky3] = this.transform.SearchComponent<Transform>("LeftHand:hands:b_l_pinky3");
            }
            else
            {
                m_ReadController[BoneId.Hand_World] = this.transform.SearchComponent<Transform>("RightHand:hands:r_hand_world");
                m_ReadController[BoneId.Hand_Hand] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_hand");
                m_ReadController[BoneId.Hand_Grip] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_grip");


                m_ReadController[BoneId.Hand_Thumb0] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_thumb1");
                m_ReadController[BoneId.Hand_Thumb1] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_thumb2");
                m_ReadController[BoneId.Hand_Thumb2] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_thumb3");

                m_ReadController[BoneId.Hand_Index1] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_index1");
                m_ReadController[BoneId.Hand_Index2] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_index2");
                m_ReadController[BoneId.Hand_Index3] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_index3");

                m_ReadController[BoneId.Hand_Middle1] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_middle1");
                m_ReadController[BoneId.Hand_Middle2] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_middle2");
                m_ReadController[BoneId.Hand_Middle3] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_middle3");

                m_ReadController[BoneId.Hand_Ring1] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_ring1");
                m_ReadController[BoneId.Hand_Ring2] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_ring2");
                m_ReadController[BoneId.Hand_Ring3] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_ring3");


                m_ReadController[BoneId.Hand_Pinky0] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_pinky0");
                m_ReadController[BoneId.Hand_Pinky1] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_pinky1");
                m_ReadController[BoneId.Hand_Pinky2] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_pinky2");
                // m_Write[BoneId.Hand_Pinky3] = this.transform.SearchComponent<Transform>("RightHand:hands:b_r_pinky3");
            }


            m_HasBeenSetup = true;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_HasBeenSetup == false)
            {
                return;
            }
            // Write
            if (stream.IsWriting)
            {
                if (m_SendController.Count != 0)
                {
                    stream.SendNext(m_SendController[BoneId.Hand_World].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Hand].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Grip].localRotation);

                    stream.SendNext(m_SendController[BoneId.Hand_Thumb0].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Thumb1].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Thumb2].localRotation);

                    stream.SendNext(m_SendController[BoneId.Hand_Index1].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Index2].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Index3].localRotation);

                    stream.SendNext(m_SendController[BoneId.Hand_Middle1].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Middle2].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Middle3].localRotation);

                    stream.SendNext(m_SendController[BoneId.Hand_Ring1].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Ring2].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Ring3].localRotation);

                    stream.SendNext(m_SendController[BoneId.Hand_Pinky0].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Pinky1].localRotation);
                    stream.SendNext(m_SendController[BoneId.Hand_Pinky2].localRotation);
                }
                else
                {
                    // no vr, like mac
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);

                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);

                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);

                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);

                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);

                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                    stream.SendNext(Quaternion.identity);
                }


            }
            // Read
            else
            {
                if (m_ReadController.Count != 0)
                {
                    m_ReadController[BoneId.Hand_World].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Hand].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Grip].localRotation = (Quaternion)stream.ReceiveNext();

                    m_ReadController[BoneId.Hand_Thumb0].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Thumb1].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Thumb2].localRotation = (Quaternion)stream.ReceiveNext();

                    m_ReadController[BoneId.Hand_Index1].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Index2].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Index3].localRotation = (Quaternion)stream.ReceiveNext();

                    m_ReadController[BoneId.Hand_Middle1].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Middle2].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Middle3].localRotation = (Quaternion)stream.ReceiveNext();

                    m_ReadController[BoneId.Hand_Ring1].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Ring2].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Ring3].localRotation = (Quaternion)stream.ReceiveNext();

                    m_ReadController[BoneId.Hand_Pinky0].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Pinky1].localRotation = (Quaternion)stream.ReceiveNext();
                    m_ReadController[BoneId.Hand_Pinky2].localRotation = (Quaternion)stream.ReceiveNext();


                }
                else
                {
                    // no vr, like mac
                    Quaternion dummy;
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();

                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();

                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();

                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();

                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();

                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();
                    dummy = (Quaternion)stream.ReceiveNext();

                }

            }
        }
    }
}
#endif