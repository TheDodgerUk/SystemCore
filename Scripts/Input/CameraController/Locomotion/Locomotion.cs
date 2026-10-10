using System;
using UnityEngine;

public class Locomotion : MonoBehaviour
{


    public float Scale => m_Transform.localScale.x;





    [SerializeField]
    private float m_MaxScale = 40f;

    private Transform m_Transform;

    private void Awake()
    {
        m_Transform = transform;
    }

    private void Start()
    {
        CreatePlayArea();
    }

    public void SetToyBoxScale(bool state)
    {
        float scale = state ? m_MaxScale : 1f;
        m_Transform.localScale = Vector3.one * scale;
    }


    private PlayArea CreatePlayArea()
    {
        var playArea = Utils.Unity.Clone("Prefab/Input/Locomotion/PlayArea", m_Transform, false);
        switch (Device.VrType)
        {
            #if UNITY_ANDROID
            case VRDevices.Oculus: return playArea.AddComponent<OculusPlayArea>();
#endif
            case VRDevices.OpenVR: return playArea.AddComponent<SteamVrPlayArea>();
            default: return playArea.AddComponent<EmptyVrPlayArea>();
        }
    }
}