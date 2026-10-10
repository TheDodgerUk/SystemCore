using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Burst;

#if !UNITY_ANDROID && !UNITY_IOS
[BurstCompile]
#endif
public class InputManagerVR : MonoSingleton<InputManagerVR>
{
    public static int RaycastMask;

    public event Action<ControllerData> ControllerAdded = null;

    public InteractionSubscription AnySubscription { get; private set; }

    private Dictionary<Collider, InteractableModule> m_RegisteredInteractables;
    private List<InputSystem> m_InputSystems;

    private Dictionary<InteractableModule, List<ControllerData>> m_GroupedControllers;
    private List<ControllerData> m_TargetlessControllers;
    private List<ControllerData> m_ChangedControllers;

    private ControllerInteraction m_ControllerInteraction;

    [SerializeField]
    private bool m_EnableInputViewer = false;

    [SerializeField]
    private bool m_IsEnabled = false;
    public bool IsEnabled => m_IsEnabled;

    public void Initialise(Action callback)
    {
        RaycastMask = Layers.RaycastMask;
        CreateInputType(Device.GetPlatformType());

        m_InputSystems = transform.GetComponentsInChildren<InputSystem>().ToList();
        m_RegisteredInteractables = new Dictionary<Collider, InteractableModule>();

        m_GroupedControllers = new Dictionary<InteractableModule, List<ControllerData>>();
        m_TargetlessControllers = new List<ControllerData>();
        m_ChangedControllers = new List<ControllerData>();

        AnySubscription = new InteractionSubscription();

        var task = new TaskAction(m_InputSystems.Count, callback);
        foreach (var inputSystem in m_InputSystems)
        {
            inputSystem.ValidateHoverState += OnValidateHoverState;
            inputSystem.ControllerAdded += OnControllerAdded;
            inputSystem.Initialise(task.Increment);
        }
    }

    public void ToggleInput(bool state)
    {
        m_InputSystems.ForEach(i => i.ToggleSystem(state));
        m_IsEnabled = state;
    }

    public void ToggleHandRaycastGraphics(Handedness hand, bool state)
    {
        m_InputSystems.ForEach(i => i.ToggleRaycastGraphics(hand, state));
    }

    public void Register(List<Collider> colliders, InteractableModule interactable)
    {
        colliders.ForEach(c => Register(c, interactable));
    }

    public void Register(Collider collider, InteractableModule interactable)
    {
        if (m_RegisteredInteractables != null)
        {
            m_RegisteredInteractables[collider] = interactable;
        }
    }

    public void IterateControllers<T>(Action<T> handler) where T : ControllerData => GetControllers<T>().ForEach(handler);
    public void IterateControllers(Action<ControllerData> handler) => IterateControllers<ControllerData>(handler);

    public List<ControllerData> GetControllers() => GetControllers<ControllerData>();

    public ControllerData GetController(Handedness hand)
    {
        var list = GetControllers<ControllerData>();

        foreach (var item in list)
        {
            if (item.Hand == hand)
            {
                return item;
            }
        }
        return null;
    }

    public ControllerData GetController(ControllerMode mode)
    {
        var list = GetControllers<ControllerData>();

        foreach (var item in list)
        {
            if (item.ControllerMode == mode)
            {
                return item;
            }
        }
        return null;
    }


    public List<T> GetControllers<T>() where T : ControllerData
    {
        return m_InputSystems.Extract(i => i.GetControllers().OfType<T>()).Flatten();
    }



    public void RemoveModule(List<Collider> colliders)
    {
        colliders?.ForEach(c => RemoveModule(c));
    }

    public void RemoveModule(Collider collider)
    {
        if (m_RegisteredInteractables != null)
        {
            m_RegisteredInteractables.Remove(collider);
        }
    }

    private void Update()
    {
        if (m_ChangedControllers == null)
        {
            return;
        }

        if (m_IsEnabled == true)
        {
            // update input systems & received changed controllers
            m_ChangedControllers.Clear();

            foreach (var input in m_InputSystems)
            {
                input.UpdateSystem(m_ChangedControllers);
            }

#if VR_INTERACTION
            InvokeHoverReleaseEvents();
#endif

            InvokeGroupedSubscriptions();
        }
    }

    public void GetColliders(ref Collider current, ref Collider prev)
    {
        var controller = GetControllerData();
        if (null != controller)
        {
            current = controller.CurrentCollider;
            prev = controller.PreviousCollider;
        }
    }

    private ControllerData GetControllerData()
    {
        foreach (var pair in m_GroupedControllers)
        {
            foreach (var controller in pair.Value)
            {
                if (controller.ControllerMode == ControllerMode.Selector)
                {
                    return controller;
                }
            }
        }
        return null;
    }

    private void InvokeSubscription(List<ControllerData> controllers, InteractionSubscription subscription)
    {
        if (controllers != null && controllers.Count != 0)
        {
            m_ControllerInteraction.UpdateControllerInteraction(controllers, subscription);
            m_ControllerInteraction.Invoke();
        }
    }

    private void InvokeHoverReleaseEvents()
    {
        m_TargetlessControllers.Clear();
        m_GroupedControllers.Clear();

        // grab hover any hover up states and group by the previous collider
        foreach (var controller in m_ChangedControllers)
        {
            if (controller.Inputs.Hover.State != TriggerState.None)
            {
                var interactable = GetModule(controller.PreviousCollider);
                if (interactable != null)
                {
                    m_GroupedControllers.AddToList(interactable, controller);
                }
            }
        }
        //var hoverUp = m_ChangedControllers.Where(c => c.Inputs.Hover.State != TriggerState.None)
        //    .GroupBy(c => GetModule(c.PreviousCollider)).Where(g => g.Key != null);

        foreach (var pair in m_GroupedControllers)
        {
            // if there are any controllers on the object that need to send a release
            if (pair.Value.Any(NeedsHoverRelease) == true)
            {
                var controllers = pair.Value;

                // clean all the state temporarily and set all the hovers to up
                foreach (var controller in controllers)
                {
                    foreach (var state in controller.Inputs.All)
                    {
                        state.SetTempState(TriggerState.None);
                    }
                    controller.Inputs.Hover.SetTempState(TriggerState.Up);
                }

                // fire events
                pair.Key.OnInteraction(controllers, GetPreviousCollider);

                // reset input states
                foreach (var controller in controllers)
                {
                    foreach (var state in controller.Inputs.All)
                    {
                        state.RestoreFromTempState();
                    }
                }
            }
        }
    }

    private bool NeedsHoverRelease(ControllerData controller)
    {
        if (controller.Inputs.Hover.State == TriggerState.Up)
        {
            return true;
        }

        if (controller.HasColliderChanged() && HasModuleChanged(controller))
        {
            return true;
        }

        return false;
    }

    private void InvokeGroupedSubscriptions()
    {
        m_TargetlessControllers.Clear();
        m_GroupedControllers.Clear();

        // group changed controllers by linked module
        foreach (var controller in m_ChangedControllers)
        {
            var interactable = GetModule(controller.CurrentCollider);
            if (interactable != null)
            {
                m_GroupedControllers.AddToList(interactable, controller);
            }
            else
            {
                m_TargetlessControllers.Add(controller);
            }
        }
        //var grouped = m_ChangedControllers.GroupBy(c => GetModule(c.CurrentCollider));
        foreach (var pair in m_GroupedControllers)
        {
            // if interactable module exists and is enabled
            var interactable = pair.Key;
            if (interactable != null)
            {
                // if the interactable is enabled
                if (interactable.IsEnabled == true)
                {
                    // send interaction
                    interactable.OnInteraction(pair.Value);
                }
                else
                {
                    // if any of the controllers are locked, unlock them
                    foreach (var controller in pair.Value)
                    {
                        if (controller.IsLocked == true)
                        {
                            controller.UnlockFromObject();
                        }
                    }
                }
            }

            InvokeSubscription(pair.Value, AnySubscription);
        }

        InvokeSubscription(m_TargetlessControllers, AnySubscription);
    }

    public ModuleObject GetModuleObject(Collider collider)
    {
        return GetModule(collider)?.ModuleObject;
    }

    private InteractableModule GetModule(Collider collider)
    {
        if (collider != null)
        {
            return m_RegisteredInteractables.Get(collider);
        }
        return null;
    }

    private void OnValidateHoverState(ControllerData controller)
    {
        if (controller.Inputs.Hover.State == TriggerState.Down)
        {
            if (controller.PreviousCollider != null)
            {
                if (HasModuleChanged(controller) == false)
                {
                    controller.Inputs.Hover.State = TriggerState.Held;
                }
            }
        }
    }

    private void OnControllerAdded(ControllerData controller)
    {
        ControllerAdded?.Invoke(controller);
    }

    private bool HasModuleChanged(ControllerData controller)
    {
        return GetModule(controller.PreviousCollider) != GetModule(controller.CurrentCollider);
    }

    private Collider GetPreviousCollider(ControllerData controller) => controller.PreviousCollider;

    private InputSystem CreateInputType(PlatformType type)
    {
        Debug.Log($"VR CreateInputType {type}");
        switch (type)
        {
            case PlatformType.Desktop: return transform.CreateChild<MouseInput>();
            case PlatformType.Mobile:
                {
                    Debug.Log("TouchInput is not updating the touch as its a struct needs looking into, forcing it to MouseInput");
                    return transform.CreateChild<MouseInput>();
                }
#if VR_INTERACTION
#if UNITY_STANDALONE_WIN
            case PlatformType.VirtualOpenXR: return transform.CreateChild<OpenXRinput>();
#endif
#if UNITY_ANDROID
            case PlatformType.Quest: return transform.CreateChild<OculusInput>();
            case PlatformType.VirtualOpenXR: return transform.CreateChild<OpenXRinput>();
#endif
#endif
            case PlatformType.Virtual: return transform.CreateChild<VirtualInput>();
            default: return null;
        }
    }

#if UNITY_EDITOR
    public event Action<List<InputSystem>> ControllerUpdate = null;

    private void SendControllersToEditor()
    {
        ControllerUpdate?.Invoke(m_InputSystems.Clone());
    }
#endif
}
