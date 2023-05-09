using System;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerMode
{
    Teleporter,
    Selector
}

public class ControllerDataTest : ControllerData
{
    public ControllerDataTest(Collider collider) : base(collider)
    {

    }
    public override bool HasLaser()
    {
        return true;
    }

    public override bool IsConnected()
    {
        return true;
    }

    public override void Vibrate(float intensity, float duration)
    {
    }
}
public abstract class ControllerData
{
    public event Action<ControllerData> ValidateHoverState = null;

    public ControllerMode ControllerMode = ControllerMode.Selector;
    public bool m_bOffsetOnAction = true;

    public Collider PreviousCollider { get; protected set; }
    public Collider CurrentCollider { get; protected set; }
    public Collider HitCollider { get; protected set; }

    public Transform RaycastTransform => m_Raycaster.transform;
    public ControllerRaycaster Raycaster => m_Raycaster;
    public Transform Reticle => Gfx?.RaycastRoot ?? RaycastRoot;

    public MonoBehaviour LockedObject => m_LockObject;
    public bool IsLocked { get; private set; }

    public readonly string UniqueId;
    public readonly Handedness Hand;

    public float StretchDistance = 0f;
    public float CurrentDistance = 0f;
    public float CurrentAcceleration = 0f;

    public readonly ControllerInput Inputs;
    public readonly ControllerGraphics Gfx;
    public readonly Transform WristTransform;
    public readonly Transform RaycastRoot;
    public readonly Transform RaycastRootAnchor;
    private bool m_bAllowRaycasts = true;

    protected ControllerRaycaster m_Raycaster;
    protected VRUIPointer m_UIPointer;
    public bool NotHoveringUI => NotOverUI();
    public bool m_UseOverrideBeamColour;
    public Color m_OverrideBeamColour;

    protected bool NotOverUI()
    {
        return (null == m_UIPointer || (null != m_UIPointer && false == m_UIPointer.IsHoveringUI));
    }

    private MonoBehaviour m_LockObject;

    public void SetRaycastable(bool bRaycastable)
    {
        m_bAllowRaycasts = bRaycastable;
        Gfx?.ToggleGraphics(bRaycastable);
    }

    public ControllerData(string uniqueId, Transform raycast, Transform wrist, Type graphicsType, Handedness handedness) :
        this(uniqueId, raycast, wrist, AddGraphics(raycast, (((int)handedness) < 0) ? null : graphicsType))
    {
        Gfx?.Initialise(handedness);
        Hand = handedness;
    }

    public ControllerData(Collider collider)
    {
        HitCollider = collider;
    }
    public ControllerData(string uniqueId, Transform raycast, Transform wrist, ControllerGraphics gfx)
    {
        var raycastTransform = raycast.transform.Find("RaycastTransform");
        if(null != raycastTransform)
        {
            RaycastRoot = raycastTransform;
        }
        else
        {
            RaycastRoot = new GameObject("RaycastTransform").transform;
        }
        RaycastRootAnchor = raycast;
        RaycastRoot.SetParent(raycast);
        RaycastRoot.gameObject.name = $"RaycastTransform_{uniqueId}";

        Inputs = new ControllerInput();

        m_Raycaster = RaycastRoot.gameObject.ForceComponent<ControllerRaycaster>();
#if UNITY_ANDROID
        var camera = m_Raycaster.GetComponent<Camera>();
        if(camera != null)
        {
            camera.enabled = false;
        }
#endif

        Hand = Handedness.Left;
#if VR_INTERACTION && UNITY_ANDROID
        if (uniqueId == OVRInput.Controller.LTouch.ToString())
        {
            Hand = Handedness.Left;
        }
        else if (uniqueId == OVRInput.Controller.RTouch.ToString())
        {
            Hand = Handedness.Right;
        }
#endif

        UniqueId = uniqueId;
        WristTransform = wrist;
        Gfx = gfx;
        Debug.Log($"Root", RaycastRoot);
        Debug.Log($"Origin", WristTransform);
    }

    public void AddModelAttachmentToRaycastRoot(GameObject attachment, Vector3 localPosition, Quaternion localRotation)
    {
        attachment.transform.SetParent(RaycastRoot);
        attachment.transform.localPosition = localPosition;
        attachment.transform.localRotation = localRotation;
    }

    public void AddModelAttachmentToGrip(GameObject attachment, Vector3 localPosition, Quaternion localRotation)
    {
        attachment.transform.SetParent(WristTransform);
        attachment.transform.localPosition = localPosition;
        attachment.transform.localRotation = localRotation;
    }

    public void Shutdown() => Gfx.DestroyGameObject();


    public abstract bool IsConnected();

    public abstract bool HasLaser();

    public virtual bool HasChanged()
    {
        if (HasColliderChanged() == true)
        {
            return true;
        }
        foreach (var input in Inputs.All)
        {
            if (input.State != TriggerState.None)
            {
                return true;
            }
        }
        return false;
    }

    public T GetGfx<T>() where T : ControllerGraphics => (Gfx as T);

    public bool HasColliderChanged() => CurrentCollider != PreviousCollider;

    public void LockToObject(MonoBehaviour lockObject)
    {
        m_LockObject = lockObject;
        if (null != m_UIPointer)
        {
            m_UIPointer.IsLocked = (null != lockObject);
        }
        IsLocked = (lockObject != null);
    }

    public void UnlockFromObject() => LockToObject(null);

    public bool Raycast(int layerMask, RaycastType type = RaycastType.Straight) => m_Raycaster.Raycast(layerMask, type);
    public bool Spherecast(int layerMask) => m_Raycaster.Spherecast(layerMask);

    public abstract void Vibrate(float intensity, float duration);
    public void Vibrate(VibrateType type)
    {
        var setting = VibrationSetting.Get(type);
        Vibrate(setting.Intensity, setting.Duration);
    }

    public void SetCurrentCollider(Collider collider, bool andPrevious = true)
    {
        if (andPrevious == true)
        {
            PreviousCollider = collider;
        }
        CurrentCollider = collider;
    }  

    public virtual void ManualUpdate(bool bRunClicks = true)
    {
        if (true == bRunClicks)
        {
            if (IsConnected() == true)
            {
                if (HasLaser() == true)
                {
                    if (true == m_bAllowRaycasts)
                    {
                        UpdateRay();
                        RaycastSelectables();
                    }
                    UpdateInput();
                    Gfx?.UpdateState(this);
                }
            }
            else
            {
                Inputs.All.ForEach(i => i.Reset());
            }
        }
        else
        {
            UpdateInput();
        }
    }

    protected virtual void UpdateRay()
    {
        m_Raycaster.Ray.direction = Reticle.forward;
        m_Raycaster.Ray.origin = Reticle.position;
    }

    protected virtual void UpdateInput()
    {
        var position = GetPosition();
        var rotation = GetRotation();
        foreach (var state in Inputs.All)
        {
            state.UpdateState(position, rotation);
        }
    }

    protected virtual Quaternion GetRotation() => Reticle.rotation;
    protected virtual Vector3 GetPosition() => Reticle.position;

    private void RaycastSelectables()
    {
        if (ControllerMode == ControllerMode.Teleporter)
        {
            return;
        }

        // cast for objects
        CastForColliders(InputManagerVR.RaycastMask);

        //Check if raycasted on UI

        // assign colliders
        HitCollider = m_Raycaster.Hit.Collider;
        PreviousCollider = CurrentCollider;

        GameObject parent = null;
        InteractableModule interactableModule = null;
        if (m_Raycaster.Hit.Collider != null && m_Raycaster.Hit.Collider.gameObject.transform.parent != null)
        {
            parent = m_Raycaster.Hit.Collider.gameObject.transform.parent.gameObject;
            interactableModule = parent.GetComponent<InteractableModule>();
        }

        
        if (IsLocked == false)
        {
            if (HitCollider.IsEnvironment() == false)
            {
                CurrentCollider = HitCollider;
            }
            else
            {
                CurrentCollider = null;
            }
        }

        // update hover state
        Inputs.Hover.State = GetHoverState();
        ValidateHoverState?.Invoke(this);

        if (interactableModule != null && interactableModule.IsEnabled == true)
        {
            // vibrate on hovering
            if (Inputs.Hover.State == TriggerState.Down)
            {
                Vibrate(VibrateType.Buzz);
            }
            else if (Inputs.Hover.State == TriggerState.Up)
            {
                Vibrate(VibrateType.Pulse);
            }
        }
    }

    private void CastForColliders(int layerMask)
    {
        if (Raycast(layerMask) == false)
        {
            Spherecast(layerMask);
        }
    }

    private TriggerState GetHoverState()
    {
        if (HasColliderChanged() == true)
        {
            if (CurrentCollider == null)
            {
                return TriggerState.Up;
            }
            else
            {
                return TriggerState.Down;
            }
        }
        else
        {
            if (CurrentCollider == null)
            {
                return TriggerState.None;
            }
            else
            {
                return TriggerState.Held;
            }
        }
    }

    protected static ControllerGraphics AddGraphics(Transform root, Type graphicsType)
    {
        if (graphicsType == null)
        {
            return null;
        }
        var child = root.CreateChild("Graphics").gameObject;
        return child.AddComponent(graphicsType) as ControllerGraphics;
    }
}

public class ControllerInput
{
    public readonly List<InputState> All;
    public readonly InputState Hover = new InputState();
    public readonly InputState BtnGrab = new InputState();

    public readonly InputState BtnStart = new InputState();
    public readonly InputState Interact = new InputState();
    public readonly InputState BtnPrimary = new InputState();
    public readonly InputState BtnSecondary = new InputState();
    public readonly InputState BtnStick = new InputState();

    public readonly Vector1State PullTrigger = new Vector1State();
    public readonly Vector1State PullGrip = new Vector1State();
    public readonly Vector2State Stick = new Vector2State();
    
    public readonly InputState TchTrigger = new InputState();
    public readonly InputState TchGrab = new InputState();   
    public readonly InputState TchPrimary = new InputState();
    public readonly InputState TchSecondary = new InputState();
    public readonly InputState TchStick = new InputState();



    public ControllerInput()
    {
        Hover = new InputState();
        BtnGrab = new InputState();
        BtnStart = new InputState();
        Interact = new InputState();
        BtnPrimary = new InputState();
        BtnSecondary = new InputState();
        PullTrigger = new Vector1State();
        PullGrip = new Vector1State();
        Stick = new Vector2State();
        BtnStick = new InputState();

        TchTrigger = new InputState();
        TchGrab = new InputState();
        TchPrimary = new InputState();
        TchSecondary = new InputState();
        TchStick = new InputState();



        All = new[] { Hover, BtnGrab, BtnStart, Interact, BtnPrimary, BtnSecondary, PullGrip, PullTrigger, Stick, BtnStick, TchGrab, TchTrigger, TchPrimary, TchSecondary, TchStick }.ToList();
    }

    public enum InputStateEnums
    {
        Hover, BtnGrab, BtnStart, Interact, BtnPrimary, BtnSecondary, PullGrip, PullTrigger, Stick, ClickStick,
    }
}