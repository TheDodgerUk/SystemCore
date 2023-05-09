using Autohand;
using MonitorTrainer;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireExtinguisher : MonoBehaviour
{

    public enum Messages
    {
        Start, 
        Stop,
    }


    public AudioSource m_ShootSound;
    public float m_ShootVolume = 1f;

    private Autohand.Grabbable m_Grabbable;


    public float m_BulletForce = 20f;
    private Transform m_GunBarrel;
    private VrInteraction m_VrInteraction;

    private VisualEffect m_VisualEffect;

    private Transform m_HoseEnd;
    private bool m_IsFiring = false;

    private void Start()
    {
        m_Grabbable = this.GetComponent<Autohand.Grabbable>();

        m_VrInteraction = this.GetComponent<VrInteraction>();

        Core.AssetsLocalRef.VisualEffectLocalRef.GetItemInstantiated("VFX_StylisedSmoke", (item) =>
        {
            var vfx = item;
            m_HoseEnd = this.transform.SearchComponent<Transform>("HoseEnd");
            m_ShootSound = m_HoseEnd.gameObject.GetComponent<AudioSource>();
            m_ShootSound.loop = true;
            vfx.transform.SetParent(m_HoseEnd);
            vfx.ClearLocals();

            vfx.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            m_VisualEffect = vfx.GetComponent<VisualEffect>();



            // weird issue has to be on off on,
            // with time delay other wise it not work
            m_VisualEffect.SetActive(false);
            Core.Mono.WaitForFrames(2, () =>
            {
                m_VisualEffect.SetActive(true);
                Core.Mono.WaitForFrames(2, () =>
                {
                    m_VisualEffect.SetActive(false);
                    Core.Mono.WaitForFrames(2, () =>
                    {
                        m_VisualEffect.SetActive(true);
                        m_IsFiring = true;
                        StopFiring(false);
                    });
                });
            });


        });


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


    private void StartFiring(bool sendNetwork)
    {
        if (m_IsFiring == false)
        {
            m_IsFiring = true;
            m_VisualEffect.Reinit();
            m_ShootSound.Play();
            if (sendNetwork == true)
            {
                Core.PhotonGenericRef.SendGenericVrInterationIntMessage(m_VrInteraction, (int)Messages.Start);
            }
        }
    }

    private void Update()
    {
        if (m_IsFiring == true)
        {
#if Photon && VR_INTERACTION
            var allHits = Physics.RaycastAll(m_HoseEnd.position, m_HoseEnd.forward, 4f, Layers.IgnoreVrInteractionMask);
            foreach (var hit in allHits)
            {
                var hitBox = SpecialTasksHitBox.Instance.GetRayCastHit(typeof(FireExtinguisher));
                if (hitBox != null)
                {
                    if (hit.collider.gameObject == hitBox.gameObject)
                    {
                        hitBox.RayCastHit(typeof(FireExtinguisher), m_VrInteraction.ActorNickNameTouched);
                        break;
                    }
                }
            }
#endif
        }
    }

    private  void StopFiring(bool sendNetwork)
    {
        if (m_IsFiring == true)
        {
            m_IsFiring = false;
            m_VisualEffect.Stop();
            m_ShootSound.Stop();
            if (sendNetwork == true)
            {
                Core.PhotonGenericRef.SendGenericVrInterationIntMessage(m_VrInteraction, (int)Messages.Stop);
            }
        }
    }

}
