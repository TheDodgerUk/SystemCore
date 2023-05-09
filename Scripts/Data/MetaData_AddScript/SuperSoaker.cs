using Autohand;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SuperSoaker : MonoBehaviour
{

    public enum Messages
    {
        Start, 
        Stop,
    }


    public AudioClip m_ShootSound;
    public float m_ShootVolume = 1f;

    private Autohand.Grabbable m_Grabbable;

    private Transform m_GunBarrel;
    private VrInteraction m_VrInteraction;

    private VisualEffect m_VisualEffect;
    private float m_StartSpeed = 5;
    private float m_StartRateOverTime = 5;

    private bool m_IsFiring = false;


    private const float START_POWER = 4;
    private float m_Power = START_POWER;

    private readonly int SPAWN_RATE = Shader.PropertyToID("SpawnRate");
    private readonly int VFX_RANGE = Shader.PropertyToID("Range");


    private void Start()
    {
        m_ShootSound = this.GetComponent<AudioClip>();
        m_Grabbable = this.GetComponent<Autohand.Grabbable>();

        m_VrInteraction = this.GetComponent<VrInteraction>();

        m_Grabbable.onSqueeze.AddListener((data1, data2) => StartFiring(true));
        m_Grabbable.onUnsqueeze.AddListener((data1, data2) => StopFiring(true));


        Core.PhotonGenericRef.CollectGenericVrInterationIntMessage(m_VrInteraction, (int)Messages.Start, () =>
        {
            StartFiring(false);
        });

        Core.PhotonGenericRef.CollectGenericVrInterationIntMessage(m_VrInteraction, (int)Messages.Stop, () =>
        {
            StopFiring(false);
        });

        Core.AssetsLocalRef.VisualEffectLocalRef.GetItemInstantiated("VFX_Water Gun", (vfx) =>
        {
            if (vfx != null)
            {
                var fire= this.gameObject.SearchComponent<Transform>("Particle System");
                var data = fire.gameObject.transform.GetTransformData();
                m_VisualEffect = vfx.GetComponent<VisualEffect>();
                vfx.gameObject.ApplyTransformData(data, TransformDataEnum.AllLocal);
                m_VisualEffect.SetFloat(SPAWN_RATE, 0f);
                m_VisualEffect.SetFloat(VFX_RANGE, 0f);
            }
        });
    }



    [EditorButton]
    private void  DEBUG_START()
    {
        StartFiring(true);
    }

    [EditorButton]
    private void DEBUG_STOP()
    {
        StopFiring(true);
    }

    private void Update()
    {
        if(m_IsFiring == true)
        {
            m_Power -= (Time.deltaTime / 3f);
            m_Power = MathF.Max(0, m_Power);

            ////var main = m_ParticleSystem.main;
            ////main.startSpeed = m_StartSpeed * percentage;

            ////var emission = m_ParticleSystem.emission;
            ////emission.rateOverTime = m_StartRateOverTime *  percentage;
            m_VisualEffect.SetFloat(VFX_RANGE, m_Power);
            m_VisualEffect.SetFloat(SPAWN_RATE, 1f);

        }
    }

    private void StartFiring(bool sendNetwork)
    {
        if (m_IsFiring == false)
        {
            m_IsFiring = true;
            //m_ParticleSystem.Play();
            if (sendNetwork == true)
            {
                Core.PhotonGenericRef.SendGenericVrInterationIntMessage(m_VrInteraction, (int)Messages.Start);
            }
        }
    }

    private  void StopFiring(bool sendNetwork)
    {
        if (m_IsFiring == true)
        {
            m_Power = START_POWER;
            m_IsFiring = false;

            m_VisualEffect.SetFloat(VFX_RANGE, m_Power);
            m_VisualEffect.SetFloat(SPAWN_RATE, 0f);
            //m_ParticleSystem.Stop();
            if (sendNetwork == true)
            {
                Core.PhotonGenericRef.SendGenericVrInterationIntMessage(m_VrInteraction, (int)Messages.Stop);
            }
        }
    }

}
