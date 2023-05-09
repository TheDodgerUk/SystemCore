#if Photon
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Voice.PUN;

public class NetworkPlayerGloves : MonoBehaviour
{
#if VR_INTERACTION

    private Autohand.Hand m_OwnerSkeletonLeftHand;
    private Autohand.Hand m_OwnerSkeletonRightHand;

    [SerializeField]
    private PhotonTransformView m_RemoteHead;

    [SerializeField]
    private PhotonTransformView m_RemoteLeftHand;

    [SerializeField]
    private PhotonTransformView m_RemoteRightHand;

    private NetworkAutoHandFingers m_NetworkAutoHandFingersLeft;
    private NetworkAutoHandFingers m_NetworkAutoHandFingersRight;


    private PhotonView m_PhotonView;

    private bool m_SetupCompleate = false;

    private Renderer[] m_Renderers;
    private Collider[] m_Colliders;

    [field: SerializeField, ReadOnly]
    private bool m_UpdateOwnerData = true;


#if UNITY_EDITOR
    public bool SHOW_LODS = false;
#endif


    void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        if (m_PhotonView.IsMine)
        {
            m_Colliders = this.GetComponentsInChildren<Collider>();
            foreach (var item in m_Colliders)
            {
                item.enabled = false;
            }

            m_Renderers = this.GetComponentsInChildren<Renderer>();
            foreach (var item in m_Renderers)
            {
                item.enabled = false;
            }
        }

        var hands = CameraControllerVR.Instance.gameObject.GetComponentsInChildren<Autohand.Hand>();
        if(hands[0].left == true)
        {
            m_OwnerSkeletonLeftHand = hands[0];
            m_OwnerSkeletonRightHand = hands[1];
        }
        else
        {
            m_OwnerSkeletonLeftHand = hands[1];
            m_OwnerSkeletonRightHand = hands[0];
        }

        var handsNetwork = this.gameObject.GetComponentsInChildren<NetworkAutoHandFingers>();
        if (handsNetwork[0].m_IsLeftHand == true)
        {
            m_NetworkAutoHandFingersLeft = handsNetwork[0];
            m_NetworkAutoHandFingersRight = handsNetwork[1];
        }
        else
        {
            m_NetworkAutoHandFingersLeft = handsNetwork[1];
            m_NetworkAutoHandFingersRight = handsNetwork[0];
        }

        if(m_RemoteHead == null)
        {
            string find = nameof(m_RemoteHead);
            find = find.Replace("m_Remote", "");
            m_RemoteHead = this.gameObject.transform.SearchComponent<PhotonTransformView>(find);
            if (m_RemoteHead == null)
            {
                Debug.LogError("m_RemoteHead is null, so not assigned OR added via code, check were NetworkPlayerGloves is used ");
            }
        }

        if (m_RemoteLeftHand == null)
        {
            string find = nameof(m_RemoteLeftHand);
            find = find.Replace("m_Remote", "");
            m_RemoteLeftHand = this.gameObject.transform.SearchComponent<PhotonTransformView>(find);
            if (m_RemoteLeftHand == null)
            {
                Debug.LogError("m_RemoteLeftHand is null, so not assigned OR added via code, check were NetworkPlayerGloves is used ");
            }
        }

        if (m_RemoteRightHand == null)
        {
            string find = nameof(m_RemoteRightHand);
            find = find.Replace("m_Remote", "");
            m_RemoteRightHand = this.gameObject.transform.SearchComponent<PhotonTransformView>(find);
            if (m_RemoteRightHand == null)
            {
                Debug.LogError("m_RemoteRightHand is null, so not assigned OR added via code, check were NetworkPlayerGloves is used ");
            }
        }

        m_SetupCompleate = true;
    }

    public bool SetUpdateOwnerData(bool enable) => m_UpdateOwnerData = enable; 

    void Update()
    {
        if (m_SetupCompleate == true)
        {
            if (m_PhotonView.IsMine)
            {
                if (m_UpdateOwnerData == true)
                {
                    MapPosition(m_RemoteHead.transform, CameraControllerVR.Instance.CameraTransform.gameObject);
                    MapPosition(m_RemoteLeftHand.transform, m_OwnerSkeletonLeftHand.gameObject);
                    MapPosition(m_RemoteRightHand.transform, m_OwnerSkeletonRightHand.gameObject);


                    for (int i = 0; i < m_NetworkAutoHandFingersLeft.m_fingers.Length; i++)
                    {
                        var finger = m_NetworkAutoHandFingersLeft.m_fingers[i];
                        for (int j = 0; j < finger.fingerJoints.Length; j++)
                        {
                            m_NetworkAutoHandFingersLeft.m_fingers[i].fingerJoints[j].transform.localRotation = m_OwnerSkeletonLeftHand.fingers[i].fingerJoints[j].transform.localRotation;
                            m_NetworkAutoHandFingersRight.m_fingers[i].fingerJoints[j].transform.localRotation = m_OwnerSkeletonRightHand.fingers[i].fingerJoints[j].transform.localRotation;

                        }
                    }
                }

            }
        }
    }


    private void MapPosition(Transform localTarget, GameObject realObject)
    {
        localTarget.position = realObject.transform.position;
        localTarget.rotation = realObject.transform.rotation;
    }

#endif
}
#endif
