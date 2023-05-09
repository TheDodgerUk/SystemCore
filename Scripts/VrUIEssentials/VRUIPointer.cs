using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRUIPointer : MonoBehaviour
{
    public static Camera EventCamera;

    public const float PointerLength = 10f;
    public VRPointerEventData pointerEventData;

    [HideInInspector]
    public float m_fHoverDurationTimer = 0f;
    [HideInInspector]
    public bool m_bCanClickOnHover = false;

    private bool m_IsHoveringUI;
    public bool IsHoveringUI
    {
        get { return m_IsHoveringUI; }
        set { m_IsHoveringUI = value; }
    }

    private bool m_bIsControllerLocked = false;
    public bool IsLocked
    {
        get { return m_bIsControllerLocked; }
        set { m_bIsControllerLocked = value; }
    }

    [Tooltip("Determines when the UI Click event action should happen.")]
    public ClickMethods m_ClickMethod = ClickMethods.ClickOnButtonDown  ;

    private ControllerInput m_ControllerData;
    private bool m_LastPointerClickState = false;
    [SerializeField]
    private bool m_bControllerSelectionPressed = false;
    private Vector2 m_TouchpadAxis = Vector2.zero;
    private TrackedControllerData m_TrackedControllerData;

    public void FingerPress(bool press, GameObject currentTouchObject)
    {
        if (VRInputModule.Instance.ControlerTypeRef == GlobalConsts.ControllerType.PhysicsHand)
        {
            m_bControllerSelectionPressed = press;
        }
    }

    public enum ClickMethods
    {
        /// <summary>
        /// Consider a UI Click action has happened when the UI Click alias button is released.
        /// </summary>
        ClickOnButtonUp,
        /// <summary>
        /// Consider a UI Click action has happened when the UI Click alias button is pressed.
        /// </summary>
        ClickOnButtonDown
    }

    public Vector3 GetOriginPosition()
    {
        return transform.position;
    }

    public Vector3 GetOriginForward()
    {
        return transform.forward;
    }

    public Vector2 GetTouchpadAxis()
    {
        return m_TouchpadAxis;
    }

    public float GetUIDistance()
    {
        return Vector3.Distance(transform.position, pointerEventData.pointerCurrentRaycast.worldPosition);
    }

    public void Vibrate(VibrateType type)
    {
        m_TrackedControllerData.Vibrate(type);
    }

    private void IsValidClick()
    {
        if (true == m_IsHoveringUI && true == m_bControllerSelectionPressed)
        {
            var parentName = this.transform.parent.parent.parent.gameObject.name;
            pointerEventData.ClickTriggered();
        }
        else
        {
            pointerEventData.ClickReleased();
        }
    }

    // Start is called before the first frame update
    public void Init(TrackedControllerData parent, ControllerInput controller)
    {
        m_TrackedControllerData = parent;
        VRInputModule inputModule = VRInputModule.Instance;
        var cachedEventSystem = inputModule.gameObject.GetComponent<EventSystem>();
        if (null != cachedEventSystem)
        {
            pointerEventData = new VRPointerEventData(cachedEventSystem)
            {
                useDragThreshold = true
            };
        }

        m_ControllerData = controller;

        m_LastPointerClickState = false;
        EventCamera = SetupCamera();
        this.transform.localPosition = Vector3.zero; // for some reson its not set correctly 
        this.transform.localRotation = Quaternion.identity;
    }

    private Camera SetupCamera()
    {
        return null;
#if UNITY_ANDROID
        return null;
#endif
        Camera camera = gameObject.ForceComponent<Camera>();

        camera.clearFlags = CameraClearFlags.Nothing;
        camera.cullingMask = LayerMask.NameToLayer("Nothing");
        camera.renderingPath = RenderingPath.Forward;
        camera.useOcclusionCulling = false;
        camera.allowHDR = false;
        camera.allowMSAA = false;
        camera.stereoTargetEye = StereoTargetEyeMask.None;

        camera.depth = -10f;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 10f;
        camera.fieldOfView = 5f;

        return camera;
    }

    private void OnEnable()
    {
        if (VRInputModule.Instance != null)
        {
            VRInputModule.Instance.AddVRPointer(this);
        }
    }

    private void OnDisable()
    {
        if (VRInputModule.Instance != null)
        {
            VRInputModule.Instance.RemoveVRPointer(this);
        }
    }

    public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)
    {
        if (VRInputModule.Instance.ControlerTypeRef == GlobalConsts.ControllerType.LaserPointer)
        {
            m_bControllerSelectionPressed = (m_ControllerData.Interact.State == TriggerState.Held);
        }
        return InternalValidClick(checkLastClick, lastClickState);
    }

    public virtual bool ValidClickFinger(bool checkLastClick, List<RaycastResult> results, bool lastClickState = false)
    {
        return InternalValidClick(checkLastClick, lastClickState);
    }

    public virtual bool InternalValidClick(bool checkLastClick, bool lastClickState)
    {
        bool result = (checkLastClick ? m_bControllerSelectionPressed && m_LastPointerClickState == lastClickState : m_bControllerSelectionPressed);

        //Has click state changed?
        if (m_LastPointerClickState != m_bControllerSelectionPressed)
        {
            IsValidClick();
        }
        m_LastPointerClickState = m_bControllerSelectionPressed;
        return result;
    }

    public virtual bool IsSelectionButtonPressed()
    {
        return m_bControllerSelectionPressed;
    }
}
