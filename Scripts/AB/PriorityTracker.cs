using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class PriorityDetails
{
    [SerializeField]
    protected float m_fDistance;

    [SerializeField]
    protected float m_fCurrentValueSize;

    public bool bValid;
    protected float m_fMinDist;
    protected float m_fMaxDist;
    protected float m_fMinValue;
    protected float m_fMaxValue;

    public float GetDistance => m_fDistance;
    public float GetValueSize => m_fCurrentValueSize;

    protected void Init(float minValue, float maxValue, 
        float minDist, float maxDist)
    {
        m_fMinDist = minDist;
        m_fMaxDist = maxDist;
        m_fMinValue = minValue;
        m_fMaxValue = maxValue;
    }

    public float ProcessDistance(Vector3 ourPosition, Vector3 position)
    {
        float dist = Vector3.Distance(ourPosition, position);

        //Minus off our value based on our distance from the object
        float normalisedDist = Mathf.InverseLerp(m_fMinDist, m_fMaxDist, dist);
        m_fCurrentValueSize = Mathf.Lerp(m_fMinValue, m_fMaxValue, normalisedDist);

        m_fDistance = dist - m_fCurrentValueSize;
        return m_fDistance;
    }
}

public class TransformPriorityDetails : PriorityDetails
{
    public Transform m_Transform;

    public void Init(Transform transform,
        float minValue, float maxValue,
        float minDist, float maxDist)
    {
        m_Transform = transform;
        Init(minValue, maxValue, minDist, maxDist);
    }

    public float ProcessDistance(Transform target)
    {
        return ProcessDistance(m_Transform.position, target.position);
    }
}

public class ParticlePriorityDetails : PriorityDetails
{
    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.Particle[] m_Particles;

    public void Init(ParticleSystem ps,
        float minValue, float maxValue,
        float minDist, float maxDist)
    {
        m_ParticleSystem = ps;
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
        Init(minValue, maxValue, minDist, maxDist);
    }

    public float ProcessDistance(Transform target, float angle, ref Vector3 closestPosition)
    {
        int iNumAliveParticles = m_ParticleSystem.GetParticles(m_Particles);

        float closestDistance = -1f;
        bValid = false;
        for (int i = 0; i < iNumAliveParticles; i++)
        {
            Vector3 particlePosition = m_Particles[i].position;

            Vector3 vec = particlePosition - target.position;

            float signedAngle = Vector3.Angle(vec, target.forward);
            if (angle < signedAngle)
            {
                //Skip particle as outside of viewing angle
                continue;
            }

            //Check distance based importance
            float dist = ProcessDistance(particlePosition, target.position);

            //Check this particle is valid
            if(m_fCurrentValueSize > 0f)
            {
                //Store the distance for sending back
                closestDistance = dist;
                closestPosition = particlePosition;
            }
        }

        if(closestDistance > 0f)
        {
            bValid = true;
        }

        return closestDistance;
    }
}

public class PriorityTracker : MonoBehaviour
{
    public Action<Vector3> OnTargetChange;
    public List<TransformPriorityDetails> TrackedItems = new List<TransformPriorityDetails>();
    public List<ParticlePriorityDetails> TrackedParticleSystems = new List<ParticlePriorityDetails>();

    private Transform m_Transform;
    [SerializeField]
    private Transform highestPriorityTarget = null;

    [SerializeField]
    private float angle = 45f;
    
    // Start is called before the first frame update
    public void Init(Transform headTransform)
    {
        m_Transform = headTransform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 ourPosition = m_Transform.position;
        float distance = Mathf.Infinity;
        Vector3 closestPosition = Vector3.zero;

        bool hasTarget = GetClosestInTrackedList(m_Transform, TrackedItems, ref distance, ref closestPosition);
        bool hasParticle = GetClosestInParticleList(m_Transform, TrackedParticleSystems, ref distance, ref closestPosition);

        if (true == hasTarget || true == hasParticle)
        {
            OnTargetChange?.Invoke(closestPosition);
        }
    }

    private bool GetClosestInTrackedList(Transform target, List<TransformPriorityDetails> list, ref float distance, ref Vector3 closestPosition)
    {
        bool hasClosest = false;
        for (int i = 0; i < list.Count; i++)
        {
            var tracked = TrackedItems[i];
            Vector3 vec = tracked.m_Transform.position - target.position;
            
            float signedAngle = Vector3.Angle(vec, m_Transform.forward);
            if (angle < signedAngle)
            {
                tracked.bValid = false;
                continue;
            }

            float dist = tracked.ProcessDistance(target);

            if (tracked.GetValueSize > 0f)
            {
                tracked.bValid = true;

                //This object is a closer target
                if (dist < distance)
                {
                    hasClosest = true;
                    closestPosition = tracked.m_Transform.position;
                    distance = dist;
                }
            }
        }

        return hasClosest;
    }

    private bool GetClosestInParticleList(Transform target, List<ParticlePriorityDetails> list, ref float distance, ref Vector3 closestPosition)
    {
        bool hasClosest = false;
        //For each particle system
        for(int i = 0; i < list.Count; i++)
        {
            var tracked = list[i];

            float dist = tracked.ProcessDistance(target, angle, ref closestPosition);

            if(dist < distance)
            {
                hasClosest = true;
                distance = dist;
            }
        }

        return hasClosest;
    }

    public void AddTrackedObject(Transform obj, 
        float minValue, float maxValue,
        float minDist, float maxDist)
    {
        var tracked = new TransformPriorityDetails();
        tracked.Init(obj, minValue, maxValue, minDist, maxDist);
        TrackedItems.Add(tracked);
    }

    public void AddTrackedParticleSystem(ParticleSystem ps, 
        float minValue, float maxValue,
        float minDist, float maxDist)
    {
        var tracked = new ParticlePriorityDetails();
        tracked.Init(ps, minValue, maxValue, minDist, maxDist);
        TrackedParticleSystems.Add(tracked);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < TrackedItems.Count; i++)
        {
            var item = TrackedItems[i];
            if (true == item.bValid)
            {
                Gizmos.color = (highestPriorityTarget == item.m_Transform) ? Color.green : Color.blue;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(item.m_Transform.position, item.GetValueSize);
        }

        for(int i  = 0; i < TrackedParticleSystems.Count; i++)
        {

        }
    }
}