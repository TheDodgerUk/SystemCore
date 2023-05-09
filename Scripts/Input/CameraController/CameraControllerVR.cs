
using System;
using UnityEngine;
#if VR_INTERACTION
using UnityEngine.SpatialTracking;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CameraControllerVR : MonoSingleton<CameraControllerVR>
{
    public float MinimumCameraDistance { get; private set; }
    public Transform CameraTransform => m_CameraTransform;
    public Transform RigRoot => m_CameraRigRoot;
    public Camera MainCamera { get { return m_MainCamera; } set { m_MainCamera = value; m_CameraTransform = m_MainCamera.transform; } }

#if VR_INTERACTION
    #region Teleporting
    public TeleportData TeleporterRight { get; private set; }

    private Autohand.HandDistanceGrabber[] m_Grabbers;
    #endregion

    public Autohand.AutoHandPlayer AutoHandPlayerRef { get; private set; }
    public Autohand.Hand HandLeftRef { get; private set; }
    public Autohand.Hand HandRightRef { get; private set; }

    public List<Autohand.Hand> HandsRef { get; private set; } = new List<Autohand.Hand>();
#endif

#if Photon && VR_INTERACTION
    public Photon.Pun.PhotonAvatarLoading NetworkPlayerRef { get; set; }
#endif

    public float Scale => 1f;
    public ControllerSelection Selection => m_ControllerSelection;
    //public ControllerModes ControllerModes => m_ControllerModes;

    private Dictionary<string, Camera> m_DictOfCreatedCameras = new Dictionary<string, Camera>();


    private Transform m_CameraTransform;
    private Transform m_CameraRigRoot;

    private Camera m_MainCamera;

    private CameraOverlay m_Overlay;
    // private CameraFade m_Fade;

    //private ControllerModes m_ControllerModes;
    private ControllerSelection m_ControllerSelection;

    private bool m_IsFirstTime = true;

    protected override void Awake()
    {
        base.Awake();

        m_CameraRigRoot = new GameObject("CameraRigRoot").transform;
        m_CameraRigRoot.SetParent(transform, false);
        m_CameraRigRoot.AddComponent<DebugSafetyVR>();

        if (m_MainCamera == null && Camera.main != null)
        {
            SetCamera(Camera.main, null);
        }

    }

    public void Initialise()
    {
        Core.Environment.OnPreLoadEnvironment += OnStartEnvironmentChange;
        CollectAutohandData();
    }

    private void OnStartEnvironmentChange()
    {
        foreach (var cam in m_DictOfCreatedCameras.Values)
        {
            GameObject.Destroy(cam.gameObject);
        }

        m_DictOfCreatedCameras.Clear();
        this.WaitFor(10, () =>
        {
            if (m_IsFirstTime == true)
            {
                for (int i = 0; i < 10; i++)
                {
                    Debug.LogError("Need to teleport before isKinematic = false, its now isKinematic == true");
                }
            }
        });
    }




    public void CollectAutohandData()
    {
#if VR_INTERACTION
        if (AutoHandPlayerRef == null)
        {
            AutoHandPlayerRef = this.GetComponentInChildren<Autohand.AutoHandPlayer>();
            var col = AutoHandPlayerRef.GetComponentsInChildren<Collider>();
            foreach (var item in col)
            {
                m_OriginalColliderState.Add(item, item.isTrigger);
            }
            TeleporterRight = new TeleportData(this.gameObject);


            HandsRef = this.GetComponentsInChildren<Autohand.Hand>().ToList();
            HandLeftRef = HandsRef.Find(e => e.left == true);
            HandRightRef = HandsRef.Find(e => e.left == false);
        }
#endif
    }

#if VR_INTERACTION
    public bool DistanceGrabHighLightInvalidTarget
    {
        get
        {
            if (m_Grabbers == null)
            {
                m_Grabbers = this.GetComponentsInChildren<Autohand.HandDistanceGrabber>(true);
            }

            return (m_Grabbers[0].enabled);
        }

        set
        {
            if (m_Grabbers == null)
            {
                m_Grabbers = this.GetComponentsInChildren<Autohand.HandDistanceGrabber>(true);
            }
            foreach (var item in m_Grabbers)
            {
                item.enabled = value;
            }
        }
    }

    public bool DistanceGrabber
    {
        get
        {
            if (m_Grabbers == null)
            {
                m_Grabbers = this.GetComponentsInChildren<Autohand.HandDistanceGrabber>(true);
            }

            return m_Grabbers[0].isActiveAndEnabled;
        }

        set
        {
            if (m_Grabbers == null)
            {
                m_Grabbers = this.GetComponentsInChildren<Autohand.HandDistanceGrabber>(true);
            }
            foreach (var item in m_Grabbers)
            {
                item.enabled = value;
            }
        }
    }

#endif

    #region teleport
#if VR_INTERACTION
    private Dictionary<Collider, bool> m_OriginalColliderState = new Dictionary<Collider, bool>();

    /// <summary>
    /// when teleporting need to change the colliders on player to trigger
    /// so they not hit phyisics objects
    /// 
    /// and turned back to correct state after
    /// </summary>
    /// <param name="teleporting"></param>
    private void TeleportingColliders(bool teleporting)
    {
        if(teleporting == true)
        {
            foreach (var item in m_OriginalColliderState)
            {
                item.Key.isTrigger = true;
            }
        }
        else
        {
            foreach (var item in m_OriginalColliderState)
            {
                item.Key.isTrigger = item.Value;
            }
        }
    }

    private void TeleportFirstTime()
    {
        if (Core.Environment.HasOnEnvironmentLoadingComplete == true)
        {
            if (m_IsFirstTime == true)
            {
                m_IsFirstTime = false;
                CameraControllerVR.Instance.AutoHandPlayerRef.body.isKinematic = false;
            }
        }
    }

    public void TeleportAvatar(Scene targetScene, Vector3 target, Action onFinish)
    {
        CollectAutohandData();
        Core.PhotonMultiplayerRef.OwnersAvatarSendData(false);

        CameraControllerVR.Instance.ToggleBlink(true, () =>
        {
            SnapTeleport(target);
            CameraControllerVR.Instance.ToggleBlink(false, () =>
            {
                Core.PhotonMultiplayerRef.OwnersAvatarSendData(true);
                SceneManager.SetActiveScene(targetScene);
                onFinish?.Invoke();
                TeleportFirstTime();
            });
        });
    }

    public void TeleportAvatar(Scene targetScene, Vector3 target, Quaternion rot, Action onFinish)
    {
        CollectAutohandData();
        Core.PhotonMultiplayerRef.OwnersAvatarSendData(false);
        CameraControllerVR.Instance.ToggleBlink(true, () =>
        {
            SnapTeleport(target, rot);
            CameraControllerVR.Instance.ToggleBlink(false, () =>
            {
                Core.PhotonMultiplayerRef.OwnersAvatarSendData(true);
                SceneManager.SetActiveScene(targetScene);
                onFinish?.Invoke();
                TeleportFirstTime();
            });
        });
    }

    public void TeleportAvatar(Scene targetScene, Transform target, Action onFinish)
    {
        CollectAutohandData();
        Core.PhotonMultiplayerRef.OwnersAvatarSendData(false);
        CameraControllerVR.Instance.ToggleBlink(true, () =>
        {
            SnapTeleport(target.position, target.rotation);
            CameraControllerVR.Instance.ToggleBlink(false, () =>
            {
                Core.PhotonMultiplayerRef.OwnersAvatarSendData(true);
                SceneManager.SetActiveScene(targetScene);
                onFinish?.Invoke();
                TeleportFirstTime();
            });
        });
    }

    public void TeleportAvatar(Scene targetScene, GameObject target, Action onFinish)
    {
        CollectAutohandData();
        Core.PhotonMultiplayerRef.OwnersAvatarSendData(false);
        CameraControllerVR.Instance.ToggleBlink(true, () =>
        {
            SnapTeleport(target.transform.position, target.transform.rotation);
            CameraControllerVR.Instance.ToggleBlink(false, () =>
            {
                Core.PhotonMultiplayerRef.OwnersAvatarSendData(true);
                SceneManager.SetActiveScene(targetScene);
                onFinish?.Invoke();
                TeleportFirstTime();
            });
        });
    }


    public void SnapTeleport(Vector3 pos)
    {
        TeleportingColliders(teleporting: true);
        AutoHandPlayerRef.SetPosition(pos);
        TeleportingColliders(teleporting: false);
        TeleportFirstTime();
    }

    public void SnapTeleport(Vector3 pos, Quaternion rot)
    {
        TeleportingColliders(teleporting: true);
        AutoHandPlayerRef.SetPosition(pos, rot);
        TeleportingColliders(teleporting: false);
        TeleportFirstTime();
    }
#endif
    #endregion

    public void SetCamera(Camera camera, Transform rig)
    {
        //m_ControllerModes.DestroyObject();

        if (rig != null)
        {
            //m_ControllerModes = m_CameraRigRoot.AddComponent<ControllerModes>();
            m_ControllerSelection = m_CameraRigRoot.AddComponent<ControllerSelection>();
            m_CameraRigRoot.position = Vector3.zero;
            m_CameraRigRoot.rotation = Quaternion.identity;

            //Set the incoming camera rig to be a child of our rig
            rig.SetParent(m_CameraRigRoot, false);
        }


        // clear existing camera tag
        if (m_MainCamera != null)
        {
            m_MainCamera.tag = Layers.MainCameraTag;
        }

        // store the camera and its transform
        m_CameraTransform = camera.transform;
        m_MainCamera = camera;
        m_MainCamera.clearFlags = CameraClearFlags.SolidColor;
        m_MainCamera.backgroundColor = Color.black;

        // set the layer and tag of the camera
        m_MainCamera.tag = Layers.MainCameraTag;
        m_MainCamera.eventMask = 0;
        SetLayerMask(Layers.CameraLoadingMask);

        // pass on depth buffer to other cameras
        m_MainCamera.clearStencilAfterLightingPass = true;
        //m_SplashCamera.clearStencilAfterLightingPass = true;
        m_MainCamera.nearClipPlane = 0.07f;
        m_MainCamera.farClipPlane = 1000f;
        // add collider & rigidbody for fast travel triggers
        var rigidbody = m_MainCamera.gameObject.ForceComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        var sphere = m_MainCamera.transform.CreateChild<SphereCollider>();
        //sphere.gameObject.layer = Layers.TeleportLayer;
        sphere.isTrigger = true;
        sphere.radius = 0.01f;

        // enable depth rendering and add a camera overlay
        m_Overlay = m_MainCamera.gameObject.ForceComponent<CameraOverlay>();
        //m_Fade = m_UiCamera.gameObject.ForceComponent<CameraFade>();

        // initialise camera fader
        MinimumCameraDistance = GetFurthestCorner(m_MainCamera) + 0.4f;
        //m_Fade.Initialise(m_MainCamera, m_CameraRigRoot, MinimumCameraDistance);
    }


    public Vector3 GetHeadDisplacement(Vector3 target)
    {
        float height = m_CameraTransform.position.y - m_CameraRigRoot.position.y;
        var displacement = target - m_CameraTransform.position;
        displacement.y += height;
        return displacement;
    }

    public bool IsInCeiling() => m_CameraRigRoot.position.y > 28f;

    public void SetLayerMask(int layerMask)
    {
        if (null != m_MainCamera)
        {
            m_MainCamera.cullingMask = layerMask;
        }
        else
        {
            Debug.LogError("SetLayerMask: m_MainCamera is null.");
        }
    }

    public void ToggleBlink(bool state, Action callback = null)
    {
        DebugBeep.Log("ToggleBlink", DebugBeep.MessageLevel.Low);
        if (OVRScreenFade.instance != null)
        {
            if (state == true)
            {

                OVRScreenFade.instance.FadeOut(callback);
            }
            else
            {
                OVRScreenFade.instance.FadeIn(callback);
            }
        }
        else
        {
            Core.Mono.WaitFor(0.3f, () =>
            {
                callback?.Invoke();
            });
        }

    }

    public void SetCurrentBlink(BlinkType blinkType) => m_Overlay.SetCurrentBlink(blinkType);
    public void SetBlinkDuration(float duration) => m_Overlay.SetBlinkDuration(duration);



    private static float GetFurthestCorner(Camera camera)
    {
        var corners = new Vector3[4];
        float left = GetCornerLength(camera, Camera.MonoOrStereoscopicEye.Left, corners);
        float right = GetCornerLength(camera, Camera.MonoOrStereoscopicEye.Right, corners);
        return left.Max(right);
    }

    private static float GetCornerLength(Camera camera, Camera.MonoOrStereoscopicEye eye, Vector3[] corners)
    {
        camera.CalculateFrustumCorners(camera.rect, camera.nearClipPlane, eye, corners);
        return corners.ToList().Maximum(v => v.magnitude);
    }

    public void SetCameraLoadingSetup(bool bLoading)
    {
        if (true == bLoading)
        {
            SetLayerMask(Layers.CameraLoadingMask);
            //m_UiCamera.cullingMask = Layers.LoadingMask;
        }
        else
        {
            SetLayerMask(Layers.CameraRuntimeMask);
            // m_UiCamera.cullingMask = Layers.UiMask;
        }
    }
}
