using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteractionFood : VrInteractionPickUp
{
    private static string HeadFoodInteractionCollider = "HeadFoodInteractionCollider";

    public class EatData
    {
        public Vector3 Position;
    }

    public enum HeadState
    {
        WithinHead,
        OutsideOfHead,
        JustEaten,
        NetworkEat,
    }

    [SerializeField]
    private ContentFoodMetaData m_Data;
    private int m_CurrentIndex = 0;

    private HeadState m_HeadState = HeadState.OutsideOfHead;

    public override void ResetToOriginalState()
    {
        base.ResetToOriginalState();
        m_CurrentIndex = 0;
        TurnOnNewPart();
    }

    public static void AttachFoodCollider(GameObject rootObj)
    {
        var child = rootObj.transform.Find(HeadFoodInteractionCollider);
        if (child == null)
        {
            GameObject headInteractionCollider = new GameObject(HeadFoodInteractionCollider);
            SphereCollider m_HeadFoodEnterSphereCollider = headInteractionCollider.AddComponent<SphereCollider>();
            m_HeadFoodEnterSphereCollider.isTrigger = true;
            m_HeadFoodEnterSphereCollider.radius = 0.2f;
            headInteractionCollider.transform.SetParent(rootObj.transform);
            headInteractionCollider.transform.ClearLocals();
        }
        else
        {
            SphereCollider m_HeadFoodEnterSphereCollider = child.GetComponent<SphereCollider>();
        }
    }



    public void Initialise(ContentFoodMetaData data)
    {
        m_Data = data;
        TurnOnNewPart();

        Core.PhotonGenericRef.CollectGenericVrInterationMessage<EatData>(this, (eatData) =>
        {
            var foodPart = m_Data.m_FoodParts[m_CurrentIndex];
            if (foodPart.m_AudioSource != null && foodPart.m_AudioSource.clip != null)
            {
                AudioSource.PlayClipAtPoint(foodPart.m_AudioSource.clip, eatData.Position);
            }

            m_CurrentIndex++;
            TurnOnNewPart();
        });


        // loop though all parts and only allow the one that stays the longest, to be able to grab
        // not the last one
        for (int i = 0; i < m_Data.m_FoodParts.Count-1; i++)
        {
            foreach (var collider in m_Data.m_FoodParts[i].m_Collider)
            {
                GrabbableRef.heldIgnoreColliders.Add(collider);
            }  
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (HeadFoodInteractionCollider == other.gameObject.name)
        {
            if (m_HeadState == HeadState.JustEaten)
            {
                m_HeadState = HeadState.OutsideOfHead;
                var foodPart = m_Data.m_FoodParts[m_CurrentIndex];
                foodPart.m_Collider.ForEach(e => e.enabled = false);

                this.WaitFor(0.3f, () =>
                {
                    NextFoodPart();
                });
            }
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (HeadFoodInteractionCollider == obj.gameObject.name)
        {
            if (m_HeadState == HeadState.OutsideOfHead)
            {
                m_HeadState = HeadState.WithinHead;
                var foodPart = m_Data.m_FoodParts[m_CurrentIndex];

                if (foodPart.m_AudioSource != null && foodPart.m_AudioSource.clip != null)
                {
                    foodPart.m_AudioSource.Play();
                }
                m_HeadState = HeadState.JustEaten;
                SwitchRenderers();
            }
        }
    }

    private void SwitchRenderers()
    {
        var foodPart = m_Data.m_FoodParts[m_CurrentIndex];
        foodPart.m_Renderer.ForEach(e => e.enabled = false);
        
        if (m_Data.m_FoodParts.ContainsIndex(m_CurrentIndex+1) == true)
        {
            var foodPartNext = m_Data.m_FoodParts[m_CurrentIndex+1];
            foodPartNext.m_Renderer.ForEach(e => e.enabled = true);
        }

        EatData eatData = new EatData();
        eatData.Position = CameraControllerVR.Instance.CameraTransform.position;
        Core.PhotonGenericRef.SendGenericVrInterationMessage<EatData>(this, eatData);
    }

    private void NextFoodPart()
    {
#if VR_INTERACTION
        m_CurrentIndex++;
        if (m_Data.m_FoodParts.ContainsIndex(m_CurrentIndex) == false)
        {
            var hand = CameraControllerVR.Instance.HandsRef.Find(e => e.holdingObj == this);
            if (hand != null)
            {
                hand.ForceReleaseGrab();
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            TurnOnNewPart();
        }
#endif
    }

    private void TurnOnNewPart()
    {
        m_Data.m_FoodParts.ForEach(e=> e.m_Renderer.ForEach(e => e.enabled = false));
        m_Data.m_FoodParts.ForEach(e => e.m_Collider.ForEach(e => e.enabled = false));

        m_Data.m_FoodParts[m_CurrentIndex].m_Renderer.ForEach(e => e.enabled = true);
        m_Data.m_FoodParts[m_CurrentIndex].m_Collider.ForEach(e => e.enabled = true);
    }


}


