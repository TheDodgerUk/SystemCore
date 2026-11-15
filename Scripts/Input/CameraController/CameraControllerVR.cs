using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Autohand;
#if VR_INTERACTION
using UnityEngine.SpatialTracking;
#endif

public class CameraControllerVR : MonoSingleton<CameraControllerVR>
{
    #region Private Fields

    private Dictionary<string, Camera> m_DictOfCreatedCameras = new Dictionary<string, Camera>();
    private Transform m_CameraTransform;
    private Transform m_WholeRootForAvatar;
    private Camera m_MainCamera;
    private CameraOverlay m_Overlay;
    private ControllerSelection m_ControllerSelection;
    private bool m_IsFirstTime = true;
    private VRCameraOfflineFix vRCameraOfflineFix;

#if VR_INTERACTION
    private Dictionary<Collider, bool> m_OriginalColliderState = new Dictionary<Collider, bool>();
#endif

    #endregion

    #region Public Fields and Properties

    public float MinimumCameraDistance { get; private set; }
    public Transform CameraTransform => m_CameraTransform;
    public Transform RigRoot => m_WholeRootForAvatar;
    public Camera MainCamera { get { return m_MainCamera; } set { m_MainCamera = value; m_CameraTransform = m_MainCamera.transform; } }
    public float Scale => 1f;
    public ControllerSelection Selection => m_ControllerSelection;
    public ExtraControllerInteractions ExtraControllerInteractionsRef;

#if VR_INTERACTION
    public Autohand.AutoHandPlayer AutoHandPlayerRef { get; private set; }
#endif

    #endregion

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        m_WholeRootForAvatar = new GameObject("WholeRootForAvatar").transform;
        m_WholeRootForAvatar.SetParent(transform, false);
        m_WholeRootForAvatar.AddComponent<DebugSafetyVR>();

        if (m_MainCamera == null && Camera.main != null)
        {
            SetCamera(Camera.main, null);
        }

        vRCameraOfflineFix = this.AddComponent<VRCameraOfflineFix>();

    }

    #endregion

    #region Initialisation

    public void Initialise()
    {
        Core.Environment.OnPreLoadEnvironment += OnStartEnvironmentChange;
        CollectAutohandData();
        ExtraControllerInteractionsRef = this.AddComponent<ExtraControllerInteractions>();
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



            var toDisable = this.GetComponentsInChildren<Autohand.HandDistanceGrabber>();
            foreach (var item in toDisable)
            {
                item.gameObject.SetActive(false);
                Debug.Log($"Diabling HandDistanceGrabber");
            }
        }
#endif
    }

    #endregion

    #region Camera

    public void SetCamera(Camera camera, Transform rig)
    {
        //m_ControllerModes.DestroyObject();

        if (rig != null)
        {
            //m_ControllerModes = m_CameraRigRoot.AddComponent<ControllerModes>();
            m_ControllerSelection = m_WholeRootForAvatar.AddComponent<ControllerSelection>();
            m_WholeRootForAvatar.position = Vector3.zero;
            m_WholeRootForAvatar.rotation = Quaternion.identity;

            //Set the incoming camera rig to be a child of our rig
            rig.SetParent(m_WholeRootForAvatar, false);
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
        float height = m_CameraTransform.position.y - m_WholeRootForAvatar.position.y;
        var displacement = target - m_CameraTransform.position;
        displacement.y += height;
        return displacement;
    }

    public bool IsInCeiling() => m_WholeRootForAvatar.position.y > 28f;

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

    #endregion

    #region Teleporting

#if VR_INTERACTION
    public void SnapTeleport(Vector3 pos, Scene targetScene)
    {
        SnapTeleport(pos);
        SceneManager.SetActiveScene(targetScene);
    }

    public void SnapTeleport(Vector3 pos)
    {
        var pos1 = AutoHandPlayerRef.GetPosition();
        var posHead1 = AutoHandPlayerRef.headCamera.transform.position;
        TeleportingColliders(teleporting: true);
        AutoHandPlayerRef.SetPosition(pos);
        TeleportingColliders(teleporting: false);

        var pos2 = AutoHandPlayerRef.GetPosition();
        var posHead2 = AutoHandPlayerRef.headCamera.transform.position;
        Debug.Log($"SnapTeleport  BEfore pos1 {pos1}  After pos2 {pos2}");
        Debug.Log($"SnapTeleport maincamera {posHead1}  maincamera {posHead2}");
    }

    public void SnapTeleport(Vector3 pos, Quaternion rot)
    {
        TeleportingColliders(teleporting: true);
        AutoHandPlayerRef.SetPosition(pos, rot);
        TeleportingColliders(teleporting: false);
    }
#endif

    #endregion

    #region Scaling



    #endregion

    #region Blink

    public void ToggleBlink(bool state, Action callback = null)
    {
#if VR_INTERACTION
        if (OVRScreenFade.instance != null)
        {
            if (state == true)
            {

                OVRScreenFade.instance.FadeOut();
                this.WaitFor(1, callback);
            }
            else
            {
                OVRScreenFade.instance.FadeIn();
                this.WaitFor(1, callback);
            }
        }
        else
        {
            Core.Mono.WaitFor(0.3f, () =>
            {
                callback?.Invoke();
            });
        }
#endif

    }

    public void SetCurrentBlink(BlinkType blinkType) => m_Overlay.SetCurrentBlink(blinkType);
    public void SetBlinkDuration(float duration) => m_Overlay.SetBlinkDuration(duration);

    #endregion

    #region Private Helpers

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

    #endregion

    #region Teleport Collider Helpers

#if VR_INTERACTION
    /// <summary>
    /// when teleporting need to change the colliders on player to trigger
    /// so they not hit phyisics objects
    /// 
    /// and turned back to correct state after
    /// </summary>
    /// <param name="teleporting"></param>
    private void TeleportingColliders(bool teleporting)
    {
        if (teleporting == true)
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
#endif

    #endregion
}
