using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : EffectController
{
    private ParticleSystem m_ParticleSystem;
    public ParticleSystem GetParticleSystem => m_ParticleSystem;

    protected override void OnEffectCreated(GameObject effect)
    {
        m_ParticleSystem = effect.GetComponentInChildren<ParticleSystem>();
    }

    public override void OnDeactivated()
    {
        if (null != m_ParticleSystem)
        {
            m_ParticleSystem.Stop();
            this.WaitFor(m_ParticleSystem.main.duration, () =>
            {
                base.OnDeactivated();
            });
        }
    }

    public override void OnEffectStart(Transform obj)
    {
        m_Transform.SetPositionAndRotation(obj.position, obj.rotation);

        if (null != m_ParticleSystem)
            m_ParticleSystem.Play();
    }

    public override void OnEffectUpdate(Transform obj)
    {
        m_Transform.SetPositionAndRotation(obj.position, obj.rotation);
    }

    public override void OnEffectEnd(Transform obj)
    {
        if (null != m_ParticleSystem)
            m_ParticleSystem.Stop();
    }

    public override void OnEffectStart(Vector3 pos, Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        m_EffectInstance.transform.SetPositionAndRotation(pos, rotation);

        if (null != m_ParticleSystem)
            m_ParticleSystem.Play();
    }

    public override void OnEffectUpdate(Vector3 pos, Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        m_EffectInstance.transform.SetPositionAndRotation(pos, rotation);
    }

    public override void OnEffectEnd(Vector3 pos, Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        m_EffectInstance.transform.SetPositionAndRotation(pos, rotation);

        if (null != m_ParticleSystem)
            m_ParticleSystem.Stop();
    }
}
