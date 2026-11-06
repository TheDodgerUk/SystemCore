using System;
using System.Collections.Generic;
using UnityEngine;


public enum ControllerType
{
    XR,
    OVR,
}

#if VR_INTERACTION && UNITY_ANDROID


/// <summary>
/// /     BOTH_WRIST_POSITION
/// </summary>
public class OculusInput : InputSystem 
{
    //  the left is just minus
    public static readonly List<Vector3> FingerTipLeft_AVATAR = new List<Vector3>()
    {
            { new Vector3(-0.0075f,0,0) }, // thumb
            { new Vector3(-0.015f,0,0) },
            { new Vector3(-0.015f,0,0) },
            { new Vector3(-0.015f,0,0) },
            { new Vector3(-0.015f,0,0) },
    };

    public static readonly List<Vector3> FingerTipRight_AVATAR = new List<Vector3>()
    {
            { new Vector3(0.0075f,0,0) }, // thumb
            { new Vector3(0.015f,0,0) },
            { new Vector3(0.015f,0,0) },
            { new Vector3(0.015f,0,0) },
            { new Vector3(0.015f,0,0) },
    };




    public enum ControllerType
    {
        XR,
        OVR,
    }

    private ControllerType m_ControllerType = ControllerType.XR;



#if VR_INTERACTION_AVATAR
    private  string TELEPORTER => "System/AutoHandTeleporterPointer";
    public override string PHOTON_NETWORK_PLAYER => "NetworkPlayer_AutoHand_AVATAR"; 
    protected override string PLAYER_PREFAB => "System/Player_AutoHand_Avatar_OVR";
#else
    public override string PHOTON_NETWORK_PLAYER => "NetworkPlayer_AutoHand_Gloves";
    protected override string PLAYER_PREFAB => "System/Player_AutoHand_Gloves_XR";
#endif


    public override void Initialise(Action callback)
    {
        Debug.Log("OculusInput   setting are done in  OVRManifestPreprocessor.cs");

        Debug.LogError($"prefab  {PLAYER_PREFAB}");

        if(PLAYER_PREFAB.EndsWith("XR") == true)
        {
            m_ControllerType = ControllerType.XR;
        }
        this.WaitForClone(PLAYER_PREFAB, (cameraRig) =>
        {
            cameraRig.transform.ClearLocals();
            SetupOculus(cameraRig.transform, callback);
        });

    }

    private void SetupOculus(Transform cameraRig, Action callback) 
    {
        CameraControllerVR.Instance.SetCamera(Camera.main, cameraRig);
        Camera.main.transform.ClearLocals();

        Debug.Log("SetupOculus");
        var sample = cameraRig.GetComponentInChildren<SampleAvatarEntity>(true);
        if (sample != null)
        {
            sample.OnSkeletonLoadedEvent.AddListener((loaded) =>
            {
                Transform root = loaded.transform.parent;


                var leftWrist = root.transform.SearchComponent<Transform>("LeftHandWrist_jnt");

                var leftHandIndex = root.transform.SearchComponent<Transform>("LeftHandIndexDistal_jnt");
                var leftAnchor = CreateRaycastAnchorPoint(leftHandIndex.transform);
                leftAnchor.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                AddController(new OculusControllerData(leftAnchor, leftHandIndex.transform, OVRInput.Controller.LTouch, "Left"));
                var pokeLeft = leftAnchor.GetComponentInChildren<VRUIPointer>().AddComponent<FingerPokeUICanvas>();
                pokeLeft.SetIndexPoint(leftHandIndex);

                GameObject leftPalm = new GameObject("leftPalm");
                leftPalm.transform.SetParent(leftWrist);
                leftPalm.transform.ClearLocals();
                leftPalm.transform.localPosition = new Vector3(-0.05f, 0, 0);
                leftPalm.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                ///////////////////////////////////////////////////////////////////////

                var rightWrist = root.transform.SearchComponent<Transform>("RightHandWrist_jnt");

                var rightHandIndex = rightWrist.transform.SearchComponent<Transform>("RightHandIndexDistal_jnt");
                var rightAnchor = CreateRaycastAnchorPoint(rightHandIndex.transform);
                rightAnchor.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                AddController(new OculusControllerData(rightAnchor, rightHandIndex.transform, OVRInput.Controller.RTouch, "Right"));
                var pokeRight = rightAnchor.GetComponentInChildren<VRUIPointer>().AddComponent<FingerPokeUICanvas>();
                pokeRight.SetIndexPoint(rightHandIndex);

                GameObject rightPalm = new GameObject("rightPalm");
                rightPalm.transform.SetParent(rightWrist);
                rightPalm.transform.ClearLocals();
                rightPalm.transform.localPosition = new Vector3(0.05f, 0, 0);
                rightPalm.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);


                TaskAction teleporterAdded = new TaskAction(2, () =>
                {
                    CameraControllerVR.Instance.CollectAutohandData();
                    if (CameraControllerVR.Instance.AutoHandPlayerRef != null)
                    {
                        var lefWrist = CameraControllerVR.Instance.transform.SearchComponent<Transform>("Joint LeftHandWrist");
                        GameObject leftP = new GameObject("left");
                        leftP.transform.SetParent(lefWrist);
                        leftP.transform.ClearLocals();
                        leftP.transform.localRotation = Quaternion.Euler(0, 180f, 0f);


                        var rightWrist = CameraControllerVR.Instance.transform.SearchComponent<Transform>("Joint RightHandWrist");
                        GameObject rightP = new GameObject("right");
                        rightP.transform.SetParent(rightWrist);
                        rightP.transform.ClearLocals();
                        rightP.transform.localRotation = Quaternion.Euler(0, 180f, 0f);


                        CameraControllerVR.Instance.HandLeftRef.palmTransform = leftPalm.transform;
                        CameraControllerVR.Instance.HandRightRef.palmTransform = rightPalm.transform;


                        AddFingerTipsFor_AVATAR(CameraControllerVR.Instance.HandLeftRef, FingerTipLeft_AVATAR);
                        AddFingerTipsFor_AVATAR(CameraControllerVR.Instance.HandRightRef, FingerTipRight_AVATAR);

                        CameraControllerVR.Instance.HandLeftRef.InitilisePalm();
                        CameraControllerVR.Instance.HandRightRef.InitilisePalm();
                    }
                    SafetyChecks();


                    callback.Invoke();
                });

#if VR_INTERACTION_AVATAR
                Core.Mono.WaitForClone(TELEPORTER, (teleportLeft) =>
                {
                    teleportLeft.transform.SetParent(leftAnchor);
                    teleportLeft.transform.ClearLocals();
                    teleporterAdded.Increment();
                });


                Core.Mono.WaitForClone(TELEPORTER, (teleportRight) =>
                {
                    teleportRight.transform.SetParent(rightAnchor);
                    teleportRight.transform.ClearLocals();
                    teleporterAdded.Increment();
                });
#else
                teleporterAdded.Increment();
                teleporterAdded.Increment();
#endif

            });
        }
        else
        {
            {
                string handName = "RobotHand (L)";
                if(m_ControllerType == ControllerType.OVR)
                {
                    handName = "RobotHand (L) (OVR)";
                }
                Transform leftHandAnchor = cameraRig.gameObject.SearchComponent<Transform>(handName);
                var indexL = leftHandAnchor.SearchComponent<Transform>("Index2");
                Transform leftHandIndex = indexL.SearchComponent<Transform>("Tip");
                var leftAnchor = CreateRaycastAnchorPoint(leftHandIndex.transform);
                leftAnchor.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                AddController(new OculusControllerData(leftAnchor, leftHandIndex, OVRInput.Controller.LTouch, "Left"));
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
                
                AddController(new OculusControllerData(rightAnchor, rightHandIndex, OVRInput.Controller.RTouch, "Right"));
                var pokeRight = rightAnchor.GetComponentInChildren<VRUIPointer>().AddComponent<FingerPokeUICanvas>();
                pokeRight.SetIndexPoint(indexR);
            }
            SafetyChecks();
            callback.Invoke();
            Core.Environment.OnEnvironmentLoadingComplete += OnEnvironmentLoading;
        }

        Core.Mono.WaitUntil(1, () => CameraControllerVR.Instance != null && CameraControllerVR.Instance.AutoHandPlayerRef != null && CameraControllerVR.Instance.AutoHandPlayerRef.body != null, () =>
        {
            CameraControllerVR.Instance.CollectAutohandData();
            CameraControllerVR.Instance.AutoHandPlayerRef.body.isKinematic = true;
        });
    }


    private void AddFingerTipsFor_AVATAR(Autohand.Hand hand, List<Vector3> tipPositions )
    {
        // have to add finger tip AFTER wards like other stuff as the OvrAvatarCustomHandPose explodes if doing to before hand
        // need tip  as AutoHand will not bend OUR end of finger without it 
        // with out this the "Distal_jnt" parts do not bend

        for (int i = 0; i < hand.fingers.Length; i++)
        {
            var finger = hand.fingers[i];
            var oldTip = finger.tip;
            GameObject newTip = new GameObject("Tip");
            newTip.transform.SetParent(oldTip);
            newTip.transform.ClearLocals();
            finger.tip = newTip.transform;
 
            newTip.transform.localPosition = tipPositions[i];


            var debug = finger.transform.SearchComponent<Transform>(AutoHandFingerBenderHelper.DEBUG_CAPSULE, false);
            if(debug != null)
            {
                Debug.LogError("There is a debug Capsule on the running Hand, these need removing ");
            }
        }
    }
    private void SafetyChecks()
    {
        CameraControllerVR.Instance.CollectAutohandData();
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

        if(CameraControllerVR.Instance.HandLeftRef.follow == null)
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

        var photonFingers = hand.GetComponentInChildren< Photon.Pun.PhotonTransformFingers> ();
        if (photonFingers != null)
        {
            Destroy(photonFingers);
        }
    }


    private void OnEnvironmentLoading()
    {
        this.WaitUntil(1, () => OVRManager.instance != null, () =>
        {
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.RoomScale);
            OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
        });

    }

    private void Update()
    {
        if (OVRManager.instance == null)
        {
            // this needs to be called for OVRInput to work
            OVRInput.Update();
        }
    }
}
#endif