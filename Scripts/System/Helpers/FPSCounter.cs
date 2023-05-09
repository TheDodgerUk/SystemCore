using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private CanvasScaler m_CanvasScaler;
    private Text m_TxtFrameRate;

    private float m_UpdateRate = 4f;  // 4 updates per sec.

    private float m_SmoothedFps = 0f;
    private float m_FrameCount = 0f;
    private float m_DeltaTime = 0f;

    private void Awake()
    {
        m_TxtFrameRate = gameObject.GetComponentInChildren<Text>();
        m_CanvasScaler = GetComponent<CanvasScaler>();
        m_CanvasScaler.enabled = false;
    }

    private void Update()
    {
        float FPS = 1.0f / Time.deltaTime;

        m_FrameCount++;
        m_DeltaTime += Time.deltaTime;
        if (m_DeltaTime > 1.0f / m_UpdateRate)
        {
            m_SmoothedFps = m_FrameCount / m_DeltaTime;
            m_FrameCount = 0f;
            m_DeltaTime -= 1.0f / m_UpdateRate;
        }

        if (null != m_TxtFrameRate)
        {
            bool displayFps = Input.GetKey(KeyCode.F);
            m_TxtFrameRate.SetActive(displayFps);
            m_CanvasScaler.enabled = displayFps;

            if (displayFps == true)
            {
                m_TxtFrameRate.text = $"FPS: {FPS.ToString("0")}\nSmoothed: {m_SmoothedFps.ToString("0")}";
            }
        }
    }
}
