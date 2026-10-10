using System;
using UnityEngine;

public enum BlinkType { Blink, Vignette, Flat }

public class CameraOverlay : MonoBehaviour
{
    private const float BlinkDistance = 0.1f;

    private const float  ORIGINAL_BLINK_DURATION = 0.3f;

    [SerializeField]
    private float m_BlinkDuration = ORIGINAL_BLINK_DURATION;


    private Material[] m_BlinkMaterials;
    private Material m_BlinkMaterial;
    private Renderer m_Blink;


    private float m_CurrentBlink;


    private void Awake()
    {
        // add fog component and set settings from config


        // get blink size
        var camera = CameraControllerVR.Instance.MainCamera;
        var size = camera.GetFrustumSizeAtDistance(BlinkDistance);

        // spawn blink prefab

        m_Blink = Utils.Unity.Clone<Renderer>("Prefab/Input/Blink", transform);

        m_Blink.transform.localPosition = new Vector3(0f, 0f, BlinkDistance);
        m_Blink.transform.localRotation = Quaternion.identity;
        m_Blink.transform.localScale = new Vector3(size.x * 1.5f, size.y, 1);
        m_BlinkMaterials = m_Blink.materials;

        // setup blink materials
        int blinkCount = Utils.Code.GetEnumCount<BlinkType>();
        if (m_BlinkMaterials.Length != blinkCount)
        {
            Debug.LogWarning($"Expected {blinkCount} blink materials. Found only {m_BlinkMaterials.Length}\n");
        }

        m_Blink.materials = new Material[] { m_BlinkMaterials[0] };

        SetCurrentBlink(BlinkType.Blink);
        m_Blink.SetActive(false);
    }

    public void SetBlinkDuration(float duration) => m_BlinkDuration = duration;

    public void SetCurrentBlink(BlinkType blinkType)
    {
        m_BlinkMaterial = m_BlinkMaterials[(int)blinkType];
        m_Blink.material = m_BlinkMaterial;
        OnBlinkStep(m_CurrentBlink);
    }

    public void ToggleBlink(bool state, Action callback)
    {
        m_Blink.SetActive(true);
        float end = state ? 1f : 0f;
        this.CancelAllTweens();
        this.Create<ValueTween>(m_BlinkDuration, () =>
        {
            if(state == false)
            {
                m_Blink.SetActive(false);
            }
            callback?.Invoke();
        }).Initialise(m_CurrentBlink, end, OnBlinkStep);
    }


    [InspectorButton]
    private void BlinkOn() => ToggleBlink(true, null);


    [InspectorButton]
    private void BlinkOff() => ToggleBlink(false, null);

    private void OnBlinkStep(float t)
    {
        m_BlinkMaterial.SetFloat("_Value", t);
        m_CurrentBlink = t;
    }
}
