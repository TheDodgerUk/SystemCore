using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSystem : MonoBehaviour
{

#if VR_INTERACTION_AVATAR
    protected virtual string PLAYER_PREFAB => "overwrited";
#else
    protected virtual string PLAYER_PREFAB => "overwrited";
#endif


#if VR_INTERACTION_AVATAR
    public virtual string PHOTON_NETWORK_PLAYER => "overwrited";
#else
    public virtual string PHOTON_NETWORK_PLAYER => "overwrited";
#endif


    public const string ANCHOR = "Anchor";

    public event Action<ControllerData> ValidateHoverState = null;
    public event Action<ControllerData> ControllerAdded = null;

    protected List<ControllerData> m_Controllers;

    protected virtual void Awake()
    {
        m_Controllers = new List<ControllerData>();
    }

    public abstract void Initialise(Action callback);

    public List<ControllerData> GetControllers() => m_Controllers.Clone();

    public void ToggleSystem(bool state)
    {
        m_Controllers.ForEach(c => c.Gfx?.ToggleGraphics(state));
    }

    public void ToggleRaycastGraphics(Handedness hand,  bool state)
    {
        foreach (var item in m_Controllers)
        {
            if(item.Hand == hand)
            {
                item.Gfx?.ToggleGraphics(state);
            }
        }
        
    }

    public void UpdateSystem(List<ControllerData> changedControllers)
    {
        PreUpdate();
        foreach (var controller in m_Controllers)
        {
            controller.ManualUpdate();
//#if !VR_INTERACTION
            if (controller.HasChanged() == true && true == controller.NotHoveringUI)
            {
                changedControllers.Add(controller);
            }
//#endif
        }
        PostUpdate();
    }

    protected virtual void PreUpdate() { }

    protected virtual void PostUpdate() { }

    protected void AddController(ControllerData controller)
    {
        controller.ValidateHoverState += OnValidateHoverState;
        m_Controllers.Add(controller);
        OnControllerAdded(controller);
    }

    protected void RemoveController(ControllerData controller)
    {
        m_Controllers.Remove(controller);
        OnControllerRemoved(controller);
    }

    protected virtual void OnControllerAdded(ControllerData controller)
    {
        ControllerAdded?.Invoke(controller);
    }

    protected virtual void OnControllerRemoved(ControllerData controller)
    {
    }

    protected Transform CreateRaycastAnchorPoint(Transform root)
    {
        Transform anchor = root.Find(ANCHOR);

        if (null == anchor)
        {
            anchor = new GameObject(ANCHOR).transform;
            anchor.SetParent(root);
            anchor.Reset();
        }
        return anchor;
    }

    private void OnValidateHoverState(ControllerData controller)
    {
        ValidateHoverState?.Invoke(controller);
    }
}
