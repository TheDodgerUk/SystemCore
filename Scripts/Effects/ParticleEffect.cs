using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleEffect : MonoBehaviour
{

    public bool Trigger = false;
    private bool m_LastTrigger = false;

    [SerializeField]
    protected float m_FireDuration = 1.0f;

    private List<ParticleSystem> m_ParticleSystems;
    private List<ParticleSystem.MinMaxCurve> m_StartLifeTime;
    private List<float> m_EmissionRate;
    private float m_Multiplier;

    private Dictionary<string, SoundEffect> m_SfxByName;
    private List<SoundEffect> m_Sfxs;

    protected virtual void Awake()
    {
        // clone sfx name
        m_Sfxs = new List<SoundEffect>();
        var items = this.GetComponentsInChildren<AudioSource>();
        foreach(var item in items)
        {
            var soundEffect = item.gameObject.ForceComponent<SoundEffect>();
            m_Sfxs.Add(soundEffect);
        }

        //////// grab colliders from children
        //////var colliders = transform.parent.GetComponentsInChildren<Collider>();
        //////foreach (var collider in colliders)
        //////{
        //////    collider.gameObject.layer = Layers.SelectableLayer;
        //////}

        // setup module object
        ////var module = gameObject.AddComponent<ModuleObject>();
        ////module.Interactable.SetColliders(colliders.ToList());
        ////module.Interactable.Subscribe(this, subscription =>
        ////{
        ////    subscription.Interact.Begin += OnInteractionBegan;
        ////    subscription.Interact.End += OnInteractionEnd;
        ////});

        // load particles prefab
        CreateParticleSystem();

        // reset object name
        this.enabled = false;
    }

    public void ClearParticles()
    {
        ToggleParticleSystems(false);

        foreach (var particleSystem in m_ParticleSystems)
        {
            particleSystem.Clear();
        }
    }

    private void Update()
    {
        if (m_LastTrigger != Trigger)
        {
            Toggle(Trigger);

            m_LastTrigger = Trigger;
        }
    }

    public void SetLayer(int layer)
    {
        foreach (var particleSystem in m_ParticleSystems)
        {
            particleSystem.gameObject.layer = layer;
        }
    }

    [InspectorButton]
    public void Fire() => Fire(m_FireDuration);

    public void Fire(float duration)
    {
        Toggle(true);

        this.WaitFor(duration, () => Toggle(false));
    }

    public void Toggle(bool state)
    {
        this.StopAllCoroutines();

        ToggleParticleSystems(state);
        foreach (var sfx in m_Sfxs)
        {
            sfx.Toggle(state);
        }

        OnToggled(state);
    }


    protected void GetSfx(string name, Action<SoundEffect> callback)
    {
        StartCoroutine(InternalGetSfx(name, callback));
    }

    private IEnumerator InternalGetSfx(string name, Action<SoundEffect> callback)
    {
        while (m_SfxByName == null)
        {
            yield return null;
        }
        callback?.Invoke(m_SfxByName.Get(name));
    }


    private void CreateParticleSystem()
    {
        m_ParticleSystems = this.GetComponentsInChildren<ParticleSystem>().ToList();
        m_StartLifeTime = m_ParticleSystems.Extract(p => p.main.startLifetime);
        m_EmissionRate = m_ParticleSystems.Extract(p => p.emission.rateOverTimeMultiplier);
        var data = Json.FullSerialiser.WriteToText<List<ParticleSystem.MinMaxCurve>>(m_StartLifeTime, true);
        m_StartLifeTime = Json.FullSerialiser.ReadFromText<List<ParticleSystem.MinMaxCurve>>(data);
        m_Multiplier = 1;

        ToggleParticleSystems(false);
    }


    //[Tooltip("between 0 and 1")]
    public void SetLifeTimePercentageNormalized(float amount)
    {
        for (int i = 0; i < m_ParticleSystems.Count; i++)
        {
            if (amount == 0)
            {
                m_ParticleSystems[i].Stop();
            }
            else
            {
                m_ParticleSystems[i].Play();
            }


            var main = m_ParticleSystems[i].main;
            m_Multiplier = amount;
            if (m_StartLifeTime[i].curve != null)
            {
                main.startLifetime = new MinMaxCurve(amount, m_StartLifeTime[i].curve);
            }
            else
            {
                main.startLifetime = new MinMaxCurve( m_StartLifeTime[i].constantMin * amount, m_StartLifeTime[i].constantMax * amount);
            }
        }
    }

    public void SetEmissionRateNormalized(float amount)
    {
        for (int i = 0; i < m_ParticleSystems.Count; i++)
        {
            m_ParticleSystems[i].SetEmissionRate(m_EmissionRate[i] * amount);
        }
    }


    private void ToggleParticleSystems(bool state)
    {
        for (int i = 0; i < m_ParticleSystems.Count; i++)
        {
            float emissionRate = m_Multiplier;
            if (emissionRate == 0f)
            {
                m_ParticleSystems[i].Stop();

                if (state == true)
                {
                    m_ParticleSystems[i].Play();
                }
            }
            else
            {
                m_ParticleSystems[i].SetEmissionRate(state ? m_EmissionRate[i] : 0f);
            }
        }
    }

    private void OnInteractionBegan(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.IsLocked == false)
        {
            interaction.Main.LockToObject(this);

            Toggle(true);
        }
    }

    private void OnInteractionEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.LockedObject == this)
        {
            interaction.Main.UnlockFromObject();

            float duration = interaction.Main.Inputs.Interact.Time.Total;
            if (duration < m_FireDuration)
            {
                this.WaitFor(m_FireDuration - duration, () =>
                {
                    Toggle(false);
                });
            }
            else
            {
                Toggle(false);
            }
        }
    }

    protected virtual void OnToggled(bool state) { }
}