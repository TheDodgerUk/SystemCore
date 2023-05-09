﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BezierCable : MonoBehaviour
{
    public Transform m_PointA;
    public Transform m_AnchorA;
    public Transform m_PointB;
    public Transform m_AnchorB;

    public Transform[] m_Parts;

    [SerializeField]
    private Vector3 rotationalOffset;

    [SerializeField]
    private float m_GizmoScale = 0.05f;

    public void Initialise(Transform pointA, Transform pointB, Transform anchorA, Transform anchorB,
        Transform[] parts)
    {
        m_PointA = pointA;
        m_PointB = pointB;
        m_AnchorA = anchorA;
        m_AnchorB = anchorB;

        m_Parts = parts;
    }

    // Update is called once per frame
    void Update()
    {
        if (null != m_Parts)
        {
            for (int i = 0; i < m_Parts.Length; i++)
            {
                float normalisedValue = Mathf.InverseLerp(0f, m_Parts.Length - 1, i);
                Vector3 position = Bezier.GetCubicPoint(normalisedValue, m_PointA.position, m_AnchorA.position, m_AnchorB.position,
                    m_PointB.position);

                m_Parts[i].position = position;
            }

            Quaternion lastRotation = Quaternion.identity;
            for(int i =0; i < m_Parts.Length-1; i++)
            {
                Vector3 dir = m_Parts[i + 1].position - m_Parts[i].position;

                lastRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(rotationalOffset);
                m_Parts[i].rotation = lastRotation;
            }

            m_Parts[m_Parts.Length-1].rotation = lastRotation;
        }
    }

    private void OnDrawGizmos()
    {
        if (null != m_PointA && null != m_PointB &&
            null != m_AnchorA && null != m_AnchorB)
        {
            //Draw Anchors
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_AnchorA.position, m_GizmoScale);
            Gizmos.DrawWireSphere(m_AnchorB.position, m_GizmoScale);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(m_PointA.position, m_GizmoScale);
            Gizmos.DrawWireSphere(m_PointB.position, m_GizmoScale);
        }

        if(null != m_Parts && m_Parts.Length > 1)
        {
            for(int i = 0; i < m_Parts.Length-1; i++)
            {
                float normalisedValueA = Mathf.InverseLerp(0f, m_Parts.Length - 1, i);
                float normalisedValueB = Mathf.InverseLerp(0f, m_Parts.Length - 1, i+1);
                Vector3 positionA = Bezier.GetCubicPoint(normalisedValueA, m_PointA.position, m_AnchorA.position, m_AnchorB.position, m_PointB.position);
                Vector3 positionB = Bezier.GetCubicPoint(normalisedValueB, m_PointA.position, m_AnchorA.position, m_AnchorB.position, m_PointB.position);

                Gizmos.DrawLine(positionA, positionB);
            }
        }
    }

}


