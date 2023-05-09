using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTutorialStep : TutorialStep
{
    private float m_fTriggerDistance;
    private Transform m_OriginPoint;
    private Transform m_TriggerObject;

	public void Initialise(Transform origin, Transform target, float distance)
    {
        m_fTriggerDistance = distance;
        m_OriginPoint = origin;
        m_TriggerObject = target;
    }

    private void Update()
    {
        if (true == m_bStepActive && 
            false == m_bActionTriggered)
        {
            Vector3 positionA = m_OriginPoint.position;
            Vector3 positionB = m_TriggerObject.position;

            positionA.y = 0f;
            positionB.y = 0f;

            if (Vector3.Distance(positionA, positionB) < m_fTriggerDistance)
            {
                TutorialStepComplete();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(true == m_bStepActive)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(m_OriginPoint.position, m_fTriggerDistance);
        }
    }
}