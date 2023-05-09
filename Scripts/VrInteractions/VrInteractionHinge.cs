using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInteractionHinge : VrInteraction
{
    private ContentHingeMetaData.HingeData m_HingeData;

    private Rigidbody m_Rigidbody;
    private HingeJoint m_HingeJoint;
    private Autohand.Grabbable m_Grabbable;

    private float m_CurrentPercentage = 0;
    public void Initialise(GameObject root, ContentHingeMetaData.HingeData data)
    {
        m_HingeData = data;
        m_Rigidbody = this.gameObject.ForceComponent<Rigidbody>();
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;

        m_HingeJoint = this.gameObject.ForceComponent<HingeJoint>();
        m_Grabbable = m_HingeJoint.gameObject.ForceComponent<Autohand.Grabbable>();
        m_Grabbable.grabType = HandGrabType.GrabbableToHand;
        m_Grabbable.ignoreWeight = true;
        m_Grabbable.jointBreakForce = float.PositiveInfinity;
        m_Grabbable.singleHandOnly = true;
        m_Grabbable.parentOnGrab = false; // needed otherwise it goes weird

        m_HingeJoint.axis = data.m_Axis;
        m_HingeJoint.useSpring = data.m_UseSpring;



        var spring = m_HingeJoint.spring;
        spring.spring = data.m_SpringData.m_Spring;
        spring.damper = data.m_SpringData.m_Damper;
        spring.targetPosition = data.m_SpringData.m_TargetPosition;
        m_HingeJoint.spring = spring;


        m_HingeJoint.useLimits = true;
        if (data.m_IsConnectedToRoot == true)
        {
            m_HingeJoint.connectedBody = root.ForceComponent<Rigidbody>();
        }
        else
        {
            m_HingeJoint.connectedBody = data.m_ConnectedGameObject.ForceComponent<Rigidbody>();
            if(m_HingeJoint.connectedBody != null)
            {
                m_HingeJoint.connectedBody.isKinematic = true;
                m_HingeJoint.connectedBody.useGravity = false;
            }
        }

        JointLimits limits = m_HingeJoint.limits;
        limits.min = data.m_AngleMin;
        limits.max = data.m_AngleMax;
        m_HingeJoint.limits = limits;

        m_HingeJoint.anchor = Vector3.zero;
    }

    private void Update()
    {
        float springValue = Quaternion.Angle(Quaternion.identity, transform.localRotation);
        float angleValue = springValue / m_HingeJoint.limits.max;
        
        if (m_CurrentPercentage != angleValue)
        {
            m_CurrentPercentage = angleValue;
            m_PercentageCallback?.Invoke(m_CurrentPercentage);
        }
    }
}
