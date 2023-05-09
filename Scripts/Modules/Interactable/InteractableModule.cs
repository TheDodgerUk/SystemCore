using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableModule : MetaAttributeModule
{
#if UNITY_EDITOR
    public InteractionSubscriptionMap Subscriptions => m_Subscriptions;
#endif

    public List<Collider> Colliders => m_Colliders.Clone();
    public ModuleObject ModuleObject => m_ModuleObject;
    public bool IsEnabled
    {
        get { return m_IsEnabled; }
        set { m_IsEnabled = value; }
    }

    [SerializeField]
    private bool m_IsEnabled = true;

    private InteractionSubscriptionMap m_Subscriptions;
    private List<Collider> m_Colliders;
    private ModuleObject m_ModuleObject;

    private void OnDestroy()
    {
        InputManagerVR.Instance?.RemoveModule(m_Colliders);
    }

    public void InitialiseWithColliders(List<Collider> colliders)
    {
        Initialise(null);
        SetColliders(colliders);
        PostSetup(null);
    }

    public override void Initialise(ModuleObject moduleObject, Action callback)
    {
        m_ModuleObject = moduleObject;
        m_Subscriptions = new InteractionSubscriptionMap();
        m_Colliders = new List<Collider>();
        callback();
    }

    public override void OnModulesInitialised(ModuleObject moduleObject)
    {
        base.OnModulesInitialised(moduleObject);

        RegisterInteractable();
    }

    public bool ContainsCollider(Collider collider) => m_Colliders.Contains(collider);

    public void SetColliders(List<Collider> colliders)
    {
        m_Colliders = colliders.Clone();
    }

    public void Toggle(bool state)
    {
        m_Colliders.ForEach(c => c.SetActive(state));
        ToggleEnabled(state);
    }

    public void ToggleEnabled(bool state)
    {
        m_IsEnabled = state;
    }

    public void Subscribe(MonoBehaviour module, Action<InteractionSubscription> handler) => Subscribe(module, m_Colliders, handler);

    public void Subscribe(MonoBehaviour module, List<Collider> colliders, Action<InteractionSubscription> handler)
    {
        var subscription = new InteractionSubscription();
        foreach (var collider in colliders)
        {
            AssignSubscription(module, collider, subscription);
        }
        handler(subscription);
    }

    public void Subscribe(MonoBehaviour module, Collider collider, Action<InteractionSubscription> handler)
    {
        GameObject obj = collider.gameObject;
        var subscription = new InteractionSubscription();
        AssignSubscription(module, collider, subscription);
        handler(subscription);
    }

    public void GetSubscription(MonoBehaviour module, Action<InteractionSubscription> modifier)
    {
        m_Subscriptions.Get(module, modifier);
    }

    public void RegisterInteractable() => InputManagerVR.Instance.Register(m_Colliders, this);

    public void AbsorbInteractions(InteractableModule interactable, Action<MonoBehaviour, InteractionSubscription> modifier)
    {
        // add collider to self
        m_Colliders.AddRange(interactable.m_Colliders);

        // clone the subscriptions from target module
        m_Subscriptions.Append(interactable.m_Subscriptions, modifier);

        // register with input manager using new colliders
        InputManagerVR.Instance.Register(interactable.m_Colliders, this);
    }

    public void RestoreInteractions(InteractableModule interactable)
    {
        interactable.m_Colliders.ForEach(c => m_Colliders.Remove(c));
        m_Subscriptions.Remove(interactable.m_Subscriptions);
        InputManagerVR.Instance.RemoveModule(interactable.m_Colliders);
    }

    public void ClearGrabSubScriptions()
    {
        m_Subscriptions.ClearGrabs(m_Subscriptions);
    }

    public void OnInteraction(List<ControllerData> controllers, Func<ControllerData, Collider> getCollider = null)
    {
        m_Subscriptions.OnInteraction(controllers, getCollider);
    }

    private void AssignSubscription(MonoBehaviour module, Collider collider, InteractionSubscription subscription)
    {
        if(null == collider)
        {
            Debug.LogError($"Collider is null, from {module}", this);
        }
        m_Subscriptions.AssignSubscription(collider, module, subscription);
        InputManagerVR.Instance.Register(collider, this);
    }

    public void RemoveSubscription(MonoBehaviour module, List<Collider> colliders, InteractionSubscription subscription)
    {
        if (null == colliders)
        {
            Debug.LogError($"Collider is null, from {module}", this);
        }
        m_Subscriptions = new InteractionSubscriptionMap();
        InputManagerVR.Instance.RemoveModule(colliders);
    }

}