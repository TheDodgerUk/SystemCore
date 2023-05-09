using System.Collections.Generic;

public abstract class EnvironmentParticleEffectManager : EnvironmentEffect
{
    public override void OnSplashReset()
    {
        ClearParticles();
    }
    public override void OnSplashComplete()
    {
        SetLayer(Layers.DefaultLayer);
    }

    public abstract void ClearParticles();
    public abstract void Fire();
}

public abstract class ParticleEffectManager<T> : EnvironmentParticleEffectManager where T : EnvironmentParticleEffect
{
    private List<T> m_ParticleEffects;

    private void Awake()
    {
        var dummies = transform.SearchAll("Dummy");
        m_ParticleEffects = dummies.Extract(d => d.AddComponent<T>());
    }

    public override void ClearParticles()
    {
        foreach (var fx in m_ParticleEffects)
        {
            fx.ClearParticles();
        }
    }

    public override void SetLayer(int layer)
    {
        foreach (var fx in m_ParticleEffects)
        {
            fx.SetLayer(layer);
        }
    }

    [InspectorButton]
    public override void Fire()
    {
        foreach (var fx in m_ParticleEffects)
        {
            fx.Fire();
        }
    }
}
