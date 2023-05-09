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
    using UnityEngine;
    using static Photon.Pun.PhotonOculusQuestControllersView;

    [AddComponentMenu("Photon Networking/PhotonOculusQuestFingersView")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonOculusQuestFingersView : MonoBehaviourPun, IPunObservable
    {

        public Hand m_Hand = Hand.Left;
        private bool m_HasBeenSetup = false;
        private bool m_HasFingers = false;

        private SkinnedMeshRenderer m_SendFingerSkinnedMeshRenderer;
        private SkinnedMeshRenderer m_ReadFingerSkinnedMeshRenderer;

        private Transform m_SendLeftHandFingerTransform;
        private Transform m_SendRightHandFingerTransform;


        public void Awake()
        {
#if VR_INTERACTION
            m_ReadFingerSkinnedMeshRenderer = this.transform.parent.SearchComponent<Transform>("OVRHandPrefab").GetComponent<SkinnedMeshRenderer>();
            m_ReadFingerSkinnedMeshRenderer.enabled = false;

            StartCoroutine(GetFingers());
#endif
        }



        //////private void Update()
        //////{
        //////    if (m_HasBeenSetup == false)
        //////    {
        //////        return;
        //////    }



        //////    // Testing only 
        //////    if (m_ReadFingerSkinnedMeshRenderer != null && m_SendFingerSkinnedMeshRenderer != null)
        //////    {

        //////        if (m_Hand == Hand.Left)
        //////        {
        //////            Debug.LogError(m_SendLeftHandFingerTransform.transform.position);
        //////            m_ReadFingerSkinnedMeshRenderer.transform.position = m_SendLeftHandFingerTransform.transform.position + new Vector3(-0.1f, 0, 0);
        //////            m_ReadFingerSkinnedMeshRenderer.transform.rotation = m_SendLeftHandFingerTransform.transform.rotation;
        //////        }
        //////        else
        //////        {
        //////            m_ReadFingerSkinnedMeshRenderer.transform.position = m_SendRightHandFingerTransform.transform.position + new Vector3(0.1f, 0, 0);
        //////            m_ReadFingerSkinnedMeshRenderer.transform.rotation = m_SendRightHandFingerTransform.transform.rotation;
        //////        }

        //////        for (int i = 0; i < m_ReadFingerSkinnedMeshRenderer.bones.Length; i++)
        //////        {
        //////            m_ReadFingerSkinnedMeshRenderer.bones[i].localRotation = m_SendFingerSkinnedMeshRenderer.bones[i].localRotation;
        //////        }
        //////    }
        //////}

        private IEnumerator GetFingers()
        {
            yield return new WaitForEndOfFrame();

#if VR_INTERACTION
            while (true)
            {
                yield return new WaitForSeconds(3f);

                Transform rootBar = CameraControllerVR.Instance.RigRoot;


                Transform sendRoot = rootBar.SearchComponent<Transform>("OurAvatarHandSkeletonLeft");
                if (m_Hand == Hand.Right)
                {
                    sendRoot = rootBar.SearchComponent<Transform>("OurAvatarHandSkeletonRight");
                }

                if (m_Hand == Hand.Right)
                {
                    m_SendRightHandFingerTransform = sendRoot;
                }
                else
                {
                    m_SendLeftHandFingerTransform = sendRoot;
                }

                if (sendRoot != null)
                {
                    m_SendFingerSkinnedMeshRenderer = sendRoot.transform.GetComponentInChildren<SkinnedMeshRenderer>();
                    

                    yield return new WaitForSeconds(1f);
                    m_HasFingers = true;
                    Debug.LogError("m_HasFingers");
                    Debug.LogError("OVRHandPrefab on to fake hand, disable top one");
                    Debug.LogError("m_FingerSkinnedMeshRenderer.enable = true");
#if UNITY_ANDROID
                    m_ReadFingerSkinnedMeshRenderer.GetComponent<OVRHand>().enabled = false;
                    m_ReadFingerSkinnedMeshRenderer.GetComponent<OVRSkeleton>().enabled = false;
                    m_ReadFingerSkinnedMeshRenderer.GetComponent<OVRSkeletonRenderer>().enabled = false;
                    m_ReadFingerSkinnedMeshRenderer.GetComponent<OVRMesh>().enabled = false;
                    m_ReadFingerSkinnedMeshRenderer.GetComponent<OVRMeshRenderer>().enabled = false;
#endif
                    yield break;
                }
            }
#endif
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
#if VR_INTERACTION
            if (m_HasBeenSetup == false)
            {
                return;
            }

            if (m_HasFingers == false)
            {
                return;
            }

            if (stream.IsWriting)
            {
                stream.SendNext(m_SendFingerSkinnedMeshRenderer.enabled);
                for (int i = 0; i < m_ReadFingerSkinnedMeshRenderer.bones.Length; i++)
                {
                    stream.SendNext(m_SendFingerSkinnedMeshRenderer.bones[i].localRotation);
                }
            }
            // Read
            else
            {
                m_SendFingerSkinnedMeshRenderer.enabled = (bool)stream.ReceiveNext();
                for (int i = 0; i < m_ReadFingerSkinnedMeshRenderer.bones.Length; i++)
                {
                    m_ReadFingerSkinnedMeshRenderer.bones[i].localRotation = (Quaternion)stream.ReceiveNext();
                }
            }
#endif
        }
    }
}
#endif