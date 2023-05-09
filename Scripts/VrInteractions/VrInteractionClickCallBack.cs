using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionClickCallBack : VrInteraction
{
    private Action m_CallBack;
    private Animation m_Animation;

    public InstructionData InstructionDataRef { get; private set; }
#if Photon
    private PhotonVrInteractionClick m_PhotonVrInteractionClick;
#endif

    protected override void Awake()
    {
        base.Awake();
        m_Animation = gameObject.GetComponent<Animation>();
        EnableOutlineCanBeUsed = true;

#if Photon
        m_PhotonVrInteractionClick = this.gameObject.ForceComponent<PhotonVrInteractionClick>();
#endif
    }


    public void Initialise(GameObject root, InstructionData data, Action<float> callback)
    {
        InstructionDataRef = data;
        m_PercentageCallback = callback;

        // Get details for instruction mode or VR tooltips.
        FriendlyName = data.m_sTitle;
        ItemDescription = data.m_sDescription;
    }

    


    
    public void PlayAnimation() => m_Animation?.Play();

    public override void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        m_CallBack?.Invoke();
        m_PercentageCallback?.Invoke(1f);
#if Photon
        if(m_PhotonVrInteractionClick != null && m_PhotonVrInteractionClick.CanSendMessage() && sendPhotonMessage == true)
        {
            m_PhotonVrInteractionClick.SendPhotonBegin();
        }
#endif
    }


    public void PhontonCallback_Begin()
    {
        BeginLaser(new ControllerStateInteraction(), false);
    }
}
