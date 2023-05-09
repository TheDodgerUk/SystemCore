using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class EffectController : MonoBehaviour
{
    protected GameObject m_EffectInstance;
    protected Transform m_Transform;

    public void Init(string sEffectName)
    {
        if (null != EnvironmentEffects.Instance)
        {
            var effect = EnvironmentEffects.Instance.GetEffect(sEffectName);

            if (null != effect)
            {
                m_EffectInstance = GameObject.Instantiate(effect.gameObject, transform);
                m_Transform = m_EffectInstance.transform;
                OnEffectCreated(m_EffectInstance);
            }
        }

        OnDeactivated();
    }

    protected abstract void OnEffectCreated(GameObject effect);

    public virtual void OnActivated()
    {
        m_EffectInstance.SetActive(true);
    }

    public virtual void OnDeactivated()
    {
        m_EffectInstance.SetActive(false);
    }

    public virtual void OnEffectStart(Transform obj) { }
    public virtual void OnEffectUpdate(Transform obj) { }
    public virtual void OnEffectEnd(Transform obj) { }

    public virtual void OnEffectStart(Vector3 pos, Vector3 direction) { }
    public virtual void OnEffectUpdate(Vector3 pos, Vector3 direction) { }
    public virtual void OnEffectEnd(Vector3 pos, Vector3 direction) { }

    public virtual void OnEffectStart(ControllerData obj) { }
    public virtual void OnEffectUpdate(ControllerData obj) { }
    public virtual void OnEffectEnd(ControllerData obj) { }
}