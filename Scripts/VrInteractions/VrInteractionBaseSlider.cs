using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public class VrInteractionBaseSlider : VrInteraction
{
#if Photon
    protected PhotonVrInteractionSlider m_PhotonVrInteractionSlider;
#endif

    public float SliderPercentageValue {get; protected set;}

    protected override void Awake()
    {
        base.Awake();

#if Photon
        m_PhotonVrInteractionSlider = this.gameObject.ForceComponent<PhotonVrInteractionSlider>();
#endif
    }
}
