using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoneLookAt : MonoBehaviour
{
    public Transform m_Bone;
    [SerializeField]
    private float NeckOffsetHeight = 0.2f;
    public Vector3 m_Offset = new Vector3(-90f, 90f, 0f);
    public Vector3 m_LookTarget;

    private Vector3 vector;
    private float angleX;
    private float angleY;

    Quaternion lastRotation;
    [SerializeField]
    private float Smoothing = 2.5f;

    // Start is called before the first frame update
    public void Init(Transform bone, Vector3 offset)
    {
        m_Bone = bone;
        m_Offset = offset;
        lastRotation = m_Bone.rotation;
    }

    public void SetLookTarget(Vector3 target)
    {
        m_LookTarget = target;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Quaternion finalRotation = Quaternion.identity * Quaternion.Euler(m_Offset);
        if (null != m_Bone)
        {
            Vector3 boneOrigin = m_Bone.position + (Vector3.up * NeckOffsetHeight);
            vector = (m_LookTarget - boneOrigin).normalized;
            Debug.DrawRay(boneOrigin, vector, Color.blue);

            Vector3 angle = transform.InverseTransformDirection(vector);
            if (angle.z > 0f)
            {
                angleY = angle.x * Mathf.Rad2Deg;
                angleX = angle.y * Mathf.Rad2Deg;

                Quaternion lookRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(-angleX, angleY, 0f);
                Debug.DrawRay(boneOrigin, lookRotation * Vector3.forward, Color.green);

                finalRotation = lookRotation * Quaternion.Euler(m_Offset);   
            }
        }

        m_Bone.rotation = Quaternion.Lerp(lastRotation, finalRotation, Time.deltaTime * Smoothing);
        lastRotation = m_Bone.rotation;
    }
}