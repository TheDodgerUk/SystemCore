using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSubscriptionMap
{
#if UNITY_EDITOR
    public Dictionary<ColliderModulePair, InteractionSubscription> SubscriptionsByPair => m_SubscriptionsByPair;
    public Dictionary<Collider, List<MonoBehaviour>> ModulesByCollider => m_ModulesByCollider;
#endif

    private readonly Dictionary<ColliderModulePair, InteractionSubscription> m_SubscriptionsByPair;
    private readonly Dictionary<Collider, List<MonoBehaviour>> m_ModulesByCollider;

    public InteractionSubscriptionMap()
    {
        m_SubscriptionsByPair = new Dictionary<ColliderModulePair, InteractionSubscription>();
        m_ModulesByCollider = new Dictionary<Collider, List<MonoBehaviour>>();
    }

    public void AssignSubscription(Collider collider, MonoBehaviour module, InteractionSubscription subscription)
    {
        if (collider == null) throw new ArgumentNullException(nameof(collider));
        if (module == null) throw new ArgumentNullException(nameof(collider));

        // do we already have this pair registered?
        var pair = new ColliderModulePair(module, collider);



        // remove items which should not be in there 
        var allKeys = m_SubscriptionsByPair.Keys;
        foreach (var key in allKeys)
        {
            if(key.Collider == collider && key.Module != module)
            {
                Debug.Log("this collider is a attched to wrong item, at there is a closer in hierarchy to a different module");
                m_SubscriptionsByPair.Remove(key);
            }
        }



        if (m_SubscriptionsByPair.ContainsKey(pair) == false)
        {
            // create new list if we have not registered this collider, else update list
            if (m_ModulesByCollider.ContainsKey(collider) == false)
            {
                m_ModulesByCollider[collider] = new List<MonoBehaviour> { pair.Module };
            }
            else
            {
                m_ModulesByCollider[collider].Add(pair.Module);
            }
        }

        // update subscription
        m_SubscriptionsByPair[pair] = subscription;
    }

    public void Append(InteractionSubscriptionMap subscriptions, Action<MonoBehaviour, InteractionSubscription> modifier)
    {
        // clone the subscriptions from target module
        foreach (var kvp in subscriptions.m_SubscriptionsByPair)
        {
            var subscription = kvp.Value.Clone();
            AssignSubscription(kvp.Key.Collider, kvp.Key.Module, subscription);
            modifier(kvp.Key.Module, subscription);
        }
    }

    public void Remove(InteractionSubscriptionMap subscriptions)
    {
        // remove all subscriptons that match the specified subscriptions
        foreach (var pair in subscriptions.m_SubscriptionsByPair.Keys)
        {
            m_SubscriptionsByPair.Remove(pair);
        }
        foreach (var collider in subscriptions.m_ModulesByCollider.Keys)
        {
            m_ModulesByCollider.Remove(collider);
        }
    }

    public void ClearGrabs(InteractionSubscriptionMap subscriptions)
    {
        foreach (var pair in subscriptions.m_SubscriptionsByPair.Keys)
        {
            m_SubscriptionsByPair[pair].Grab.Clear();
        }
    }

    public void Get(MonoBehaviour module, Action<InteractionSubscription> modifier)
    {
        foreach (var kvp in m_SubscriptionsByPair)
        {
            if (kvp.Key.Module == module)
            {
                modifier(kvp.Value);
            }
        }
    }

    public void OnInteraction(List<ControllerData> controllers, Func<ControllerData, Collider> getCollider = null)
    {
        // gathers up interactions per module
        var interactions = new Dictionary<MonoBehaviour, ControllerInteraction>();
        foreach (var controller in controllers)
        {
            // get the collider from the controller (using action if specified)
            var collider = getCollider?.Invoke(controller) ?? controller.CurrentCollider;
            var modules = m_ModulesByCollider.Get(collider);
            if (null == modules)
            {
                continue;
            }

            foreach (var module in modules)
            {
                // get the subscription corresponding to this pair
                var pair = new ColliderModulePair(module, collider);
                var subscription = m_SubscriptionsByPair.Get(pair);
                if (subscription != null)
                {
                    // add controller + subscription to interaction
                    if (interactions.ContainsKey(module) == false)
                    {
                        interactions.Add(module, new ControllerInteraction(controller, subscription));
                    }
                    else
                    {
                        interactions[module].Subscriptions.Add(subscription);
                        interactions[module].Controllers.Add(controller);
                    }
                }
            }
        }

        // fire off events
        foreach (var interaction in interactions.Values)
        {
            interaction.Invoke();
        }
    }
}

public struct ColliderModulePair
{
    public readonly MonoBehaviour Module;
    public readonly Collider Collider;

    public ColliderModulePair(MonoBehaviour module, Collider collider)
    {
        Collider = collider;
        Module = module;
    }

    public override int GetHashCode()
    {
        return Module.GetHashCode() ^ Collider.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is ColliderModulePair)
        {
            var pair = (ColliderModulePair)obj;
            return pair.Module == Module && pair.Collider == Collider;
        }
        return base.Equals(obj);
    }
}
