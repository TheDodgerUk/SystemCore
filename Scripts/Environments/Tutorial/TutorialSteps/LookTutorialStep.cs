using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTutorialStep : TutorialStep
{
    [SerializeField]
    private Transform m_LookTarget;

    [SerializeField]
    private float angle;

    public void Initialise(Transform LookTarget)
    {
        m_LookTarget = LookTarget;
    }

    private void Update()
    {
        //If looking at target
        if(null != CameraControllerVR.Instance.MainCamera && 
            null != m_LookTarget)
        {
            Transform camTransform = CameraControllerVR.Instance.MainCamera.transform;
            Vector3 vecA = camTransform.forward;
            Vector3 vecB = m_LookTarget.position - camTransform.position;

            angle = Vector3.SignedAngle(vecB, vecA, Vector3.up);

            if(Mathf.Abs(angle) < 15f)
            {
                //Looking at target
                TutorialStepComplete();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (true == m_bStepActive)
        {
            if (null != CameraControllerVR.Instance.MainCamera &&
                null != m_LookTarget)
            {
                Transform camTransform = CameraControllerVR.Instance.MainCamera.transform;
                Vector3 vecA = camTransform.forward;
                Vector3 vecB = m_LookTarget.position - camTransform.position;

                Debug.DrawRay(camTransform.position, vecA, Color.blue);
                Debug.DrawRay(camTransform.position, vecB, Color.green);
            }
        }
    }
}