#if VR_INTERACTION
using System;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.XR.OpenXR.Samples.ControllerSample;


public class OpenXRinput : InputSystem
{
    private Transform m_LeftHandAnchor;
    private Transform m_RightHandAnchor;

    public override string PHOTON_NETWORK_PLAYER => "NetworkPlayer_AutoHand_Gloves";
    protected override string PLAYER_PREFAB => "System/Player_AutoHand_Gloves_XR";

    public override void Initialise(Action callback)
    {
        Debug.Log("OpenXRinput");
#if VR_INTERACTION
        this.WaitForClone(PLAYER_PREFAB, cameraRig =>
        {
            this.WaitFor(1f, () =>
            {
                cameraRig.transform.ClearLocals();
                SetupXR(cameraRig.transform);
                callback();
            });
        });
#else
        callback();
#endif
    }

    private ControllerType m_ControllerType = ControllerType.XR;

    private void SetupXR(Transform cameraRig)
    {
        CameraControllerVR.Instance.SetCamera(Camera.main, cameraRig);
        Camera.main.transform.ClearLocals();

        {
            {
                string handName = "RobotHand (L)";
                if (m_ControllerType == ControllerType.OVR)
                {
                    handName = "RobotHand (L) (OVR)";
                }
                Transform leftHandAnchor = cameraRig.gameObject.SearchComponent<Transform>(handName);
                var indexL = leftHandAnchor.SearchComponent<Transform>("Index2");
                Transform leftHandIndex = indexL.SearchComponent<Transform>("Tip");
                var leftAnchor = CreateRaycastAnchorPoint(leftHandIndex.transform);
                leftAnchor.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                AddController(new OpenXRControllerData(leftAnchor, leftHandIndex, OVRInput.Controller.LTouch, "Left"));
                var pokeLeft = leftAnchor.GetComponentInChildren<VRUIPointer>().AddComponent<FingerPokeUICanvas>();
                pokeLeft.SetIndexPoint(indexL);
            }
            //////////////////
            {
                string handName = "RobotHand (R)";
                if (m_ControllerType == ControllerType.OVR)
                {
                    handName = "RobotHand (R) (OVR)";
                }
                Transform rightHandAnchor = cameraRig.gameObject.SearchComponent<Transform>(handName);
                var indexR = rightHandAnchor.SearchComponent<Transform>("Index2");
                Transform rightHandIndex = indexR.SearchComponent<Transform>("Tip");
                var rightAnchor = CreateRaycastAnchorPoint(rightHandIndex.transform);
                rightAnchor.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

                AddController(new OpenXRControllerData(rightAnchor, rightHandIndex, OVRInput.Controller.RTouch, "Right"));
                var pokeRight = rightAnchor.GetComponentInChildren<VRUIPointer>().AddComponent<FingerPokeUICanvas>();
                pokeRight.SetIndexPoint(indexR);
            }

            Core.Environment.OnEnvironmentLoadingComplete += OnEnvironmentLoading;
        }

        Core.Mono.WaitUntil(1, () => CameraControllerVR.Instance != null && CameraControllerVR.Instance.AutoHandPlayerRef != null && CameraControllerVR.Instance.AutoHandPlayerRef.body != null, () =>
        {
            CameraControllerVR.Instance.CollectAutohandData();
            SafetyChecks();
            CameraControllerVR.Instance.AutoHandPlayerRef.body.isKinematic = true;
        });
        Core.Environment.OnEnvironmentLoadingComplete += OnEnvironmentLoading;
    }

    private void SafetyChecks()
    {
        VrInteractionFood.AttachFoodCollider(Camera.main.gameObject);
        RemoveItems(CameraControllerVR.Instance.HandLeftRef.gameObject);
        RemoveItems(CameraControllerVR.Instance.HandRightRef.gameObject);

        foreach (var item in CameraControllerVR.Instance.HandLeftRef.fingers)
        {
            item.enabled = true;
        }

        foreach (var item in CameraControllerVR.Instance.HandRightRef.fingers)
        {
            item.enabled = true;
        }

        if (CameraControllerVR.Instance.HandLeftRef.follow == null)
        {
            Debug.LogError("CameraControllerVR.Instance.HandLeftRef.follow needs to be set to TrackedPoseDriverLeft");
            Debug.LogError("Setting it vai code");
            var input = CameraControllerVR.Instance.GetComponentInChildren<AutoHandTransformTrackingInputManager>(true);
            CameraControllerVR.Instance.HandLeftRef.follow = input.leftController;
        }

        if (CameraControllerVR.Instance.HandRightRef.follow == null)
        {
            Debug.LogError("CameraControllerVR.Instance.HandLeftRef.follow needs to be set to TrackedPoseDriverRight");
            Debug.LogError("Setting it vai code");
            var input = CameraControllerVR.Instance.GetComponentInChildren<AutoHandTransformTrackingInputManager>(true);
            CameraControllerVR.Instance.HandRightRef.follow = input.rightController;
        }
    }

    private void RemoveItems(GameObject hand)
    {
        var view = hand.GetComponent<Photon.Pun.PhotonTransformView>();
        if (view != null)
        {
            Destroy(view);
        }

        var fingers = hand.GetComponentInChildren<NetworkAutoHandFingers>();
        if (fingers != null)
        {
            Destroy(fingers);
        }

        var photonFingers = hand.GetComponentInChildren<Photon.Pun.PhotonTransformFingers>();
        if (photonFingers != null)
        {
            Destroy(photonFingers);
        }
    }

    private void OnEnvironmentLoading()
    {
    }



}
#endif