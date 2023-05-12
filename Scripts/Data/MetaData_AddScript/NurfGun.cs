using Autohand;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NurfGun : MonoBehaviour
{

    public class Ammo
    {
        public PhotonRigidbodyTurnonOffView m_BulletView;
        public Rigidbody m_RealBullet;
        public Transform m_FakeBullet;
    }
    private Rigidbody m_Rigidbody;

    public float m_RecoilPower = 0.3f;


    public AudioClip m_ShootSound;
    public float m_ShootVolume = 1f;

    private Autohand.Grabbable m_Grabbable;


    private List<Ammo> m_AmmoList = new List<Ammo>();

    public float m_BulletForce = 2f;
    private Transform m_GunBarrel;
    private VrInteraction m_VrInteraction;

    private void Start()
    {
        m_Rigidbody = this.gameObject.ForceComponent<Rigidbody>();
        m_Rigidbody.mass = 0.1f;
        m_ShootSound = this.GetComponent<AudioClip>();
        m_Grabbable = this.GetComponent<Autohand.Grabbable>();

        m_VrInteraction = this.GetComponent<VrInteraction>();
        int arrowindex = 0;
        while(true)
        {
            arrowindex++;
            string nameToFind = $"NurfBullet_Prefab{arrowindex}";
            string fakeNameToFind = $"FakeNurfBullet{arrowindex}";
            var realAmmo = this.transform.SearchComponent<Rigidbody>(nameToFind, false);
            var fakeAmmo = this.transform.SearchComponent<Transform>(fakeNameToFind, false);
            if (realAmmo != null && fakeAmmo != null)
            {
                Ammo newAmmo = new Ammo();
                newAmmo.m_RealBullet = realAmmo;
                newAmmo.m_FakeBullet = fakeAmmo;
                m_AmmoList.Add(newAmmo);

                newAmmo.m_FakeBullet.SetActive(true);
                newAmmo.m_RealBullet.useGravity = false;
                newAmmo.m_RealBullet.isKinematic = true;


                var grabbable = newAmmo.m_RealBullet.gameObject.ForceComponent<Autohand.Grabbable>();
                grabbable.instantGrab = false;
                grabbable.useGentleGrab = true;

                // this need to make sure it all setup
                newAmmo.m_RealBullet.gameObject.SetActive(true);
                newAmmo.m_BulletView = newAmmo.m_RealBullet.gameObject.ForceComponent<PhotonRigidbodyTurnonOffView>();
                Core.Mono.WaitForFrames(5, () =>
                {
                    newAmmo.m_RealBullet.gameObject.SetActive(false);
                });

            }
            else
            {
                break;
            }

            // safety check
            if(arrowindex > 50)
            {
                break;
            }

        }

        if (m_AmmoList.Count == 0)
        {
            Debug.LogError($"Cannot find any nurfs bullets");
        }

        m_GunBarrel = this.transform.SearchComponent<Transform>("GunBarrel");

        m_Grabbable.onSqueeze.AddListener(Shoot);
    }


    [EditorButton]
    private void DEBUG_SHOOT()
    {
        Shoot(null, null);
    }
    private void Shoot(Hand arg0, Grabbable arg1)
    {
        if (m_AmmoList.Count != 0)
        {
            var ammo = m_AmmoList.Last();
            m_AmmoList.Remove(ammo);

            // ammo will collider with the gun if firing from
            var ammoCollider = ammo.m_RealBullet.GetComponentInChildren<Collider>();
            foreach (var col in m_VrInteraction.ColliderList)
            {
                if (ammoCollider != col)
                {
                    Physics.IgnoreCollision(col, ammoCollider);
                }
            }


            ammo.m_FakeBullet.SetActive(false);
            ammo.m_RealBullet.transform.position = m_GunBarrel.transform.position;
            ammo.m_RealBullet.transform.rotation = m_GunBarrel.transform.rotation;

            ammo.m_RealBullet.gameObject.transform.SetParent(null);
            ammo.m_RealBullet.gameObject.SetActive(true);
            ammo.m_RealBullet.useGravity = true;
            ammo.m_RealBullet.isKinematic = false;
            ammo.m_RealBullet.AddForce(m_GunBarrel.forward * m_BulletForce, ForceMode.Impulse);

            if (m_ShootSound)
            {
                AudioSource.PlayClipAtPoint(m_ShootSound, m_GunBarrel.transform.position, m_ShootVolume);
            }

#if VR_INTERACTION
            var pickup = ammo.m_RealBullet.GetComponent<VrInteractionPickUp>();
            pickup.ForceSync();

            var heldItem = CameraControllerVR.Instance.HandsRef.Find(e => e.holdingObj != null);
            if (heldItem != null)
            {
                m_Rigidbody.AddForce(m_GunBarrel.transform.up * m_RecoilPower * 5, ForceMode.Impulse);
            }
#endif
        }
    }
}
