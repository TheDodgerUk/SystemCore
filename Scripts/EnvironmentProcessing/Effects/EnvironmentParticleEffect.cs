using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentParticleEffect : MonoBehaviour
{
    protected abstract string[] AudioNames { get; }
    protected abstract string PrefabName { get; }

    public bool Trigger = false;
    private bool m_LastTrigger = false;

    [SerializeField]
    protected float m_FireDuration = 1.0f;

    private List<ParticleSystem> m_ParticleSystems;
    private List<float> m_EmissionRates;

    private Dictionary<string, SoundEffect> m_SfxByName;
    private List<SoundEffect> m_Sfxs;

    protected virtual void Awake()
    {
        // clone sfx name
        m_Sfxs = new List<SoundEffect>();
        foreach (var audioName in AudioNames)
        {
            TaskAction task = new TaskAction(AudioNames.Length, () =>
            {
                m_SfxByName = m_Sfxs.ExtractAsValues(s => s.name);
            });


            Core.AssetBundlesRef.GameObjectAssetBundleRef.GetItem(this, audioName, (sfx) =>
            {
                sfx.transform.SetParent(this.transform);
                if (sfx != null)
                {
                    m_Sfxs.Add(sfx.AddComponent<SoundEffect>());
                }
                task.Increment();
            });
        }

        // grab colliders from children
        var colliders = transform.parent.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.gameObject.layer = Layers.DefaultLayer;
        }

        // setup module object
        var module = gameObject.AddComponent<ModuleObject>();
        module.Interactable.SetColliders(colliders.ToList());
        module.Interactable.Subscribe(this, subscription =>
        {
            subscription.Interact.Begin += OnInteractionBegan;
            subscription.Interact.End += OnInteractionEnd;
        });

        // load particles prefab
        var prefab = Resources.Load<GameObject>(PrefabName);
        CreateParticleSystem(prefab, transform);

        // reset object name
        gameObject.name = PrefabName;
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
        while(m_SfxByName == null)
        {
            yield return null;
        }
        callback?.Invoke(m_SfxByName.Get(name));
    }


    private void CreateParticleSystem(GameObject prefab, Transform dummy)
    {
        var clone = Utils.Unity.Clone(prefab);
        clone.transform.SetParent(dummy);
        clone.transform.Reset(false);

        m_ParticleSystems = clone.GetComponentsInChildren<ParticleSystem>().ToList();
        m_EmissionRates = m_ParticleSystems.Extract(p => p.emission.rateOverTime.constant);

        for (int i = 0; i < m_ParticleSystems.Count; ++i)
        {
            if (m_EmissionRates[i] == 0)
            {
                m_FireDuration = m_FireDuration.Max(m_ParticleSystems[i].main.duration);
            }
        }

        ToggleParticleSystems(false);
    }

    private void ToggleParticleSystems(bool state)
    {
        for (int i = 0; i < m_ParticleSystems.Count; i++)
        {
            float emissionRate = m_EmissionRates[i];
            if (emissionRate == 0)
            {
                m_ParticleSystems[i].Stop();

                if (state == true)
                {
                    m_ParticleSystems[i].Play();
                }
            }
            else
            {
                m_ParticleSystems[i].SetEmissionRate(state ? emissionRate : 0f);
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