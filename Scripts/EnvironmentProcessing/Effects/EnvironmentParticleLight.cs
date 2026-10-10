using UnityEngine;

[RequireComponent(typeof(Light))]
public class EnvironmentParticleLight : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_ParticleSystem;
    [SerializeField]
    private int m_MaxParticles = 250;

    private float m_MaxIntensity = 1f;
    private Light m_Light;

    private void Awake()
    {
        m_Light = GetComponent<Light>();
        m_MaxIntensity = m_Light.intensity;
    }

    private void Update()
    {
        float t = m_ParticleSystem.particleCount / (float)m_MaxParticles;
        m_Light.enabled = m_ParticleSystem.particleCount > 0;
        m_Light.intensity = t * m_MaxIntensity;
    }
}
