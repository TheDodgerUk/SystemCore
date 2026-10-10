using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionSlider : VrInteractionBaseSlider
{
    private Vector3 m_Start;
    private Vector3 m_End;
    private float m_TotalDistance;
    private float m_CurrentPositionPercentage = 0f;
    private float m_PreviousPositionPercentage = 0f;


    private Vector3 m_GrabOffset;
    private Quaternion m_Quaternion;


    private Coroutine m_MovementCoroutine = null;

    private float m_fResetTime = 1.0f;


    protected override void Awake()
    {
        base.Awake();

#if AR_INTERACTION
        m_fResetTime = ARViewer.ProductSetup.RESET_TIME;
#endif

#if UNITY_ANDROID && VR_INTERACTION
        var grab = this.gameObject.ForceComponent<OVRGrabbable>();
        //grab.OnGrabStart += this.BeginPhysicsGrab;
        //grab.OnGrabUpdate += this.UpdatePhysicsGrab;
        //grab.OnGrabEnd += this.EndPhysicsGrab;
#endif
    }



    public void Initialise(GameObject root, ContentSliderMetaData.ItemData data,  Action<float> callback)
    {
        m_PercentageCallback = callback;
        m_Start = data.m_ModelGameObject.transform.localPosition;

        CollectData(root, ref  m_End, data);
        m_TotalDistance = Vector3.Distance(m_Start, m_End);
        m_Quaternion = this.transform.localRotation;

        // Get details for instruction mode or VR tooltips.
        FriendlyName = data.m_sTitle;
        ItemDescription = data.m_sDescription;
    }


    public static void CollectData(GameObject rootObj, ref Vector3 localEnd, ContentSliderMetaData.ItemData  data)
    {
        switch (data.m_MoveType)
        {
            case ContentSliderMetaData.ItemData.MoveType.GameObject:
                localEnd = data.m_ModelEndLocalPositionGameObject.transform.localPosition;
                break;
            case ContentSliderMetaData.ItemData.MoveType.Amount:
                localEnd = rootObj.transform.localPosition;
                localEnd += new Vector3(0f, 0f, data.m_ModelEndLocalPositionfloat);
                ////////   this came up weird , but zero idea why localEnd += rootObj.transform.forward * data.m_ModelEndLocalPositionfloat;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Resets the object's position, returns whether or not a reset was needed.
    /// </summary>
    public bool ResetPosition()
    {
        if (m_CurrentPositionPercentage != 0f)
        {
            if (null != m_MovementCoroutine)
            {
                StopCoroutine(m_MovementCoroutine);
            }
            // Only set the new previous rotation if it fully reset last time.
            else
            {
                m_PreviousPositionPercentage = m_CurrentPositionPercentage;
            }

            m_MovementCoroutine = StartCoroutine(MoveToPosition(m_CurrentPositionPercentage, 0f));

            return true;
        }

        m_PreviousPositionPercentage = 0f;

        return false;
    }


    /// <summary>
    /// Goes back to the previous position.
    /// </summary>
    public void RestorePreviousPosition()
    {
        if (null != m_MovementCoroutine)
        {
            StopCoroutine(m_MovementCoroutine);
        }

        if (m_CurrentPositionPercentage != m_PreviousPositionPercentage)
        {
            m_MovementCoroutine = StartCoroutine(MoveToPosition(m_CurrentPositionPercentage, m_PreviousPositionPercentage));
        }
    }


    /// <summary>
    /// Move this slider from "fFromPercentage" to "fToPercentage" over ARViewer.ProductSetup.RESET_TIME.
    /// </summary>
    private IEnumerator MoveToPosition(float fFromPercentage, float fToPercentage)
    {
        float fTime = 0.0f;
        while (fTime < m_fResetTime)
        {
            fTime += Time.deltaTime / m_fResetTime;
            m_CurrentPositionPercentage = Mathf.Lerp(fFromPercentage, fToPercentage, fTime);
            gameObject.transform.localPosition = Vector3.Lerp(m_Start, m_End, m_CurrentPositionPercentage);
            yield return new WaitForEndOfFrame();
        }

        m_MovementCoroutine = null;
    }


    public override void SetPercentageCallbackAmount(float percentage)
    {
        m_CurrentPositionPercentage = percentage;
        gameObject.transform.localPosition = Vector3.Lerp(m_Start, m_End, m_CurrentPositionPercentage);
    }



    public void PhontonCallback_Begin()
    {
        BeginLaser(new ControllerStateInteraction(),false);
    }


    public void PhontonCallback_Update(Vector3 localPosition)
    {
        this.gameObject.transform.localPosition = localPosition;
        float currentDistance = Vector3.Distance(m_Start, this.gameObject.transform.localPosition);
        m_PercentageCallback?.Invoke(currentDistance / m_TotalDistance);
    }

    public void PhontonCallback_End()
    {
        EndLaser(new ControllerStateInteraction(), false);
    }


    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (null != m_MovementCoroutine)
        {
            StopCoroutine(m_MovementCoroutine);
        }

        if (interaction.Main != null)
        {
            interaction.Main.LockToObject(this);
        }
#if Photon
        if (m_PhotonVrInteractionSlider != null && m_PhotonVrInteractionSlider.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionSlider.SendPhotonBegin();
        }
#endif
    }

    public override void OnUpdateLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (true == InputUtility.LinePlaneIntersectionDualDirection(out Vector3 pos, interaction.Main.RaycastTransform.position, interaction.Main.RaycastTransform.forward, this.gameObject.transform.up, this.gameObject.transform.position))
        {
            this.gameObject.transform.localPosition = m_Start;
            Vector3 start = this.gameObject.transform.position;

            this.gameObject.transform.localPosition = m_End;
            Vector3 end = this.gameObject.transform.position;

            this.gameObject.transform.position = CoreUtils.ClosestPointOnLine(start, end, pos);
        }

        float currentDistance = Vector3.Distance(m_Start, this.gameObject.transform.localPosition);
#if Photon
        if(m_PhotonVrInteractionSlider != null && m_PhotonVrInteractionSlider.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionSlider.SendPhotonUpdate(this.gameObject.transform.localPosition);
        }
#endif
        m_CurrentPositionPercentage = currentDistance / m_TotalDistance;
        SliderPercentageValue = m_CurrentPositionPercentage;
        m_PercentageCallback?.Invoke(m_CurrentPositionPercentage);
    }

    public override void EndLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main != null)
        {
            interaction.Main.LockToObject(null);
        }
#if Photon
        if (m_PhotonVrInteractionSlider != null && m_PhotonVrInteractionSlider.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionSlider.SendPhotonEnd();
        }
#endif
        float currentDistance = Vector3.Distance(m_Start, this.gameObject.transform.localPosition);
        m_CurrentPositionPercentage = currentDistance / m_TotalDistance;
        SliderPercentageValue = m_CurrentPositionPercentage;
        m_PercentageCallback?.Invoke(m_CurrentPositionPercentage);
    }


#if VR_INTERACTION

    public override void BeginFingerTouch(GameObject fingerTransform)
    {
        // this is just here for example, we want to grab not push with finger

        this.EnableOutlineCanBeUsed = true;
        this.SetOutLine(true);
    }


    public override void OnUpdateFingerTouch(GameObject fingerTransform)
    {
        //this is just here for example, we want to grab not push with finger

        this.SetOutLine(true);

        this.gameObject.transform.localPosition = m_Start;
        Vector3 start = this.gameObject.transform.position;

        this.gameObject.transform.localPosition = m_End;
        Vector3 end = this.gameObject.transform.position;

        this.gameObject.transform.position = CoreUtils.ClosestPointOnLine(start, end, fingerTransform.transform.position);

        float currentDistance = Vector3.Distance(m_Start, this.gameObject.transform.localPosition);

        SliderPercentageValue = m_CurrentPositionPercentage;
        m_PercentageCallback?.Invoke(currentDistance / m_TotalDistance);
    }

    public override void EndFingerTouch(GameObject fingerTransform)
    {
        // this is just here for example, we want to grab not push with finger

        this.SetOutLine(false);
        float currentDistance = Vector3.Distance(m_Start, this.gameObject.transform.localPosition);
        float percentage = currentDistance / m_TotalDistance;
        SliderPercentageValue = percentage;
        m_PercentageCallback?.Invoke(percentage);
    }



#endif

}

