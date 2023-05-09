using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionDial : VrInteraction
{
    private float m_MaxRotation = 0f; 
    private Axis m_Axis;
    private float m_CurrentGameObjectRotation;
    private float m_CurrentControllerRotation;
    private Action<float> m_CallBack;

    private Vector3 m_StartingPosition;
    private Quaternion m_StartingRotation;

#if Photon
    private PhotonVrInteractionDial m_PhotonVrInteractionDial;
#endif

    // Used for touch screen and mouse dial rotation.
    private float m_fRotationOffset = 0.0f;
    protected readonly Vector3 ROTATION_AXIS = new Vector3(0.0f, 1.0f, 0.0f); // Only works with the Y axis for now.
    private float m_fDistance = 0f;
    private float m_fPreviousRotation = 0f;
    private Coroutine m_RotationCoroutine = null;

    private float m_fResetTime = 1.0f;



    protected override void Awake()
    {
        base.Awake();

#if AR_INTERACTION
        m_fResetTime = ARViewer.ProductSetup.RESET_TIME;
#endif

#if UNITY_ANDROID && VR_INTERACTION
        /////////////////////////var grab = this.gameObject.ForceComponent<OVRGrabbable>();

        //grab.OnGrabStart += this.BeginPhysicsGrab;
        //grab.OnGrabUpdate += this.UpdatePhysicsGrab;
        //grab.OnGrabEnd += this.EndPhysicsGrab;
#endif

#if Photon
        m_PhotonVrInteractionDial = this.gameObject.ForceComponent<PhotonVrInteractionDial>();
#endif
    }

    public void Initialise(GameObject root, ContentDialMetaData.StringItemData data, Action<float> callback)
    {
        m_Axis = Axis.Y;
        m_MaxRotation = data.m_RotationAmount;
        m_CallBack = callback;
        m_CurrentGameObjectRotation = 0f;
        m_StartingPosition = this.transform.localPosition;
        m_StartingRotation = this.transform.localRotation;

        // Get details for instruction mode or VR tooltips.
        FriendlyName = data.m_sTitle;
        ItemDescription = data.m_sDescription;
    }


    public void Initialise(Axis axis, float maxRotation, Action<float> callback)
    {
        m_Axis = axis;
        m_MaxRotation = maxRotation;
        m_CallBack = callback;
        m_CurrentGameObjectRotation = 0f;
        m_StartingPosition = this.transform.localPosition;
        m_StartingRotation = this.transform.localRotation;
        Debug.LogError(m_StartingPosition.ToAccurateString());
    }


    /// <summary>
    /// Resets the object's rotation, returns whether or not a reset was needed.
    /// </summary>
    public bool ResetRotation()
    {
        if (m_CurrentGameObjectRotation != 0f)
        {
            if (null != m_RotationCoroutine)
            {
                StopCoroutine(m_RotationCoroutine);
            }
            // Only set the new previous rotation if it fully reset last time.
            else
            {
                m_fPreviousRotation = m_CurrentGameObjectRotation;
            }

            m_RotationCoroutine = StartCoroutine(RotateToAngle(m_CurrentGameObjectRotation, 0f));

            return true;
        }

        m_fPreviousRotation = 0f;

        return false;
    }


    /// <summary>
    /// Goes back to the previous rotation.
    /// </summary>
    public void RestorePreviousRotation()
    {
        if (null != m_RotationCoroutine)
        {
            StopCoroutine(m_RotationCoroutine);
        }

        if (m_CurrentGameObjectRotation != m_fPreviousRotation)
        {
            m_RotationCoroutine = StartCoroutine(RotateToAngle(m_CurrentGameObjectRotation, m_fPreviousRotation));
        }
    }


    /// <summary>
    /// Rotate this dial from "fFromAngle" to "fToAngle" over ARViewer.ProductSetup.RESET_TIME.
    /// </summary>
    private IEnumerator RotateToAngle(float fFromAngle, float fToAngle)
    {
        float fTime = 0.0f;
        while (fTime < m_fResetTime)
        {
            fTime += Time.deltaTime / m_fResetTime;
            m_CurrentGameObjectRotation = Mathf.Lerp(fFromAngle, fToAngle, fTime);
            transform.localRotation = m_StartingRotation * Quaternion.AngleAxis(m_CurrentGameObjectRotation, ROTATION_AXIS);
            yield return new WaitForEndOfFrame();
        }

        m_RotationCoroutine = null;
    }


    /// <summary>
    /// Instantly set the rotation to a specific angle.
    /// </summary>
    public void SetRotation(float fAngle)
    {
        m_CurrentGameObjectRotation = fAngle;

        // Clamp the lerp angle between min and max.
        m_CurrentGameObjectRotation = Mathf.Clamp(m_CurrentGameObjectRotation, 0f, m_MaxRotation);

        // Rotate the object on it's Y axis, by the lerp angle, always from the starting rotation.
        transform.localRotation = m_StartingRotation * Quaternion.AngleAxis(m_CurrentGameObjectRotation, ROTATION_AXIS);
    }


    public void PhontonCallback_Begin()
    {
        /// not needed Begin(new ControllerStateInteraction(), false);
    }

    public void PhontonCallback_Update(Vector3 localRotation)
    {
        this.gameObject.transform.localEulerAngles = localRotation;
        float percentage = this.gameObject.transform.localEulerAngles.GetAxis(m_Axis) / m_MaxRotation;
        m_CallBack?.Invoke(percentage);
    }


    public void PhontonCallback_End()
    {
        EndLaser(new ControllerStateInteraction(), false);
    }


    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
#if VR_INTERACTION
        Begin_VR(interaction, sendPhotonMessage);
#else
        Begin_NONVR(interaction, sendPhotonMessage);
#endif
    }
    private void Begin_VR(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        interaction.Main.LockToObject(this);
#if Photon
        if(m_PhotonVrInteractionDial != null && m_PhotonVrInteractionDial.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionDial.SendPhotonBegin();
        }
#endif
        m_CurrentGameObjectRotation = this.gameObject.transform.localEulerAngles.GetAxis(m_Axis);
        m_CurrentControllerRotation = -ControllerAxis(interaction);
    }


    private void Begin_NONVR(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (null != m_RotationCoroutine)
        {
            StopCoroutine(m_RotationCoroutine);
        }

        interaction.Main.LockToObject(this);

        // Figure out the direction vector in relation to this transform's local axis, then normalise it.
        Vector3 normalisedDirectionVector = transform.InverseTransformDirection(interaction.Main.Raycaster.Hit.Point - transform.position).normalized;

        // Work out the distance to the raycast so that it can be used in the rest of the interaction frames.
        m_fDistance = Vector3.Distance(interaction.Main.RaycastTransform.position, interaction.Main.Raycaster.Hit.Point);

        float fAngle = GetAngle(normalisedDirectionVector);

        // The initial angle is the rotational offset.
        m_fRotationOffset = fAngle;
    }


    public override void OnUpdateLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
#if VR_INTERACTION
        OnUpdate_VR(interaction, sendPhotonMessage);
#else
        OnUpdate_NONVR(interaction, sendPhotonMessage);
#endif
    }





    private void OnUpdate_VR(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        float amount = m_CurrentControllerRotation + ControllerAxis(interaction) + m_CurrentGameObjectRotation;
        this.gameObject.transform.localEulerAngles = this.gameObject.transform.localEulerAngles.SetAxis(m_Axis, amount);
        float percentage = this.gameObject.transform.localEulerAngles.GetAxis(m_Axis) / m_MaxRotation;
#if Photon
        if (m_PhotonVrInteractionDial != null && m_PhotonVrInteractionDial.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionDial.SendPhotonUpdate(this.gameObject.transform.localEulerAngles);
        }
#endif
        m_CallBack?.Invoke(percentage);
    }


    public override void EndLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        interaction.Main.LockToObject(null);
#if Photon
        if (m_PhotonVrInteractionDial != null && m_PhotonVrInteractionDial.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionDial.SendPhotonEnd();
        }
#endif
    }

    private void OnUpdate_NONVR(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        // Figure out the direction vector in relation to this transform's local axis, then normalise it.
        // Use GetPoint with the initial interaction distance to allow movement on the dail even if you aren't touching a collider.
        Vector3 normalisedDirectionVector = transform.InverseTransformDirection(interaction.Main.Raycaster.Ray.GetPoint(m_fDistance) - transform.position).normalized;

        float fAngle = GetAngle(normalisedDirectionVector);

        // Always take away the initial rotation, this is the offset between where you grabbed, and where it thinks you pointed.
        fAngle -= m_fRotationOffset;

        // If the angle is over 180 now, it probably needs a negative direction to get where it's going.
        if (fAngle > 180.0f)
        {
            fAngle = -(360.0f - fAngle);
        }

        // The angle we get is the difference between where we are and where we need to point, add it to our lerp angle.
        m_CurrentGameObjectRotation += fAngle;

        // Clamp the lerp angle between min and max.
        m_CurrentGameObjectRotation = Mathf.Clamp(m_CurrentGameObjectRotation, 0f, m_MaxRotation);

        // Rotate the object on it's Y axis, by the lerp angle, always from the starting rotation.
        transform.localRotation = m_StartingRotation * Quaternion.AngleAxis(m_CurrentGameObjectRotation, ROTATION_AXIS);
    }


    private float GetAngle(Vector3 normalisedDirectionVector)
    {
        // Work out the Y rotation by using the (x,z) direction vector.
        // More info:
        // https://www.dummies.com/education/science/physics/how-to-find-the-angle-and-magnitude-of-a-vector/
        // https://www.khanacademy.org/math/precalculus/x9e81a4f98389efdf:vectors/x9e81a4f98389efdf:component-form/a/vector-magnitude-and-direction-review
        float fAngle = (Vector3.zero == normalisedDirectionVector) ? (0.0f) : (Mathf.Rad2Deg * (Mathf.Atan(normalisedDirectionVector.x / normalisedDirectionVector.z)));

        // As we are doing fAngle=tan-1(xz) in our case, as we are working in a 2D plane of (x,z), if the Z is negative, we need to add 180.
        // This is because arctangent (Mathf.Atan) gives a range of -90 - 90, so in our case, if the Z is negative, we're in the 90 - 270 range and need to add 180.
        // This would usually be the X axis as the standard formula is fAngle=tan-1(yx). As this maths is for 2D planes (x,y).
        // More info:
        // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Principal_values
        if (0.0f > normalisedDirectionVector.z)
        {
            fAngle += 180.0f;
        }

        // If the angle is negative now, we're in the -90 - 0 area and should add it to 360 to get the correct positive rotation.
        if (0 > fAngle)
        {
            fAngle += 360.0f;
        }

        return fAngle;
    }



    private float ControllerAxis(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        return interaction.Main.WristTransform.localEulerAngles.GetAxis(Axis.Z);
    }


#if VR_INTERACTION
    private float ControllerAxis(OVRGrabber grab)
    {
        return grab.transform.localEulerAngles.GetAxis(Axis.Z);
    }

#endif

}

