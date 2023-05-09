using System;
using UnityEngine;

public abstract class PlayArea : MonoBehaviour
{
    private const float PlayAreaTexScale = 2;

    private Transform m_Transform;
    private RectTransform m_PlaySpace;

    private void Awake()
    {
        m_Transform = transform;
        m_PlaySpace = m_Transform.FindRect("PlaySpace");
    }

    private void Start()
    {
        m_PlaySpace.SetActive(false);

        RequestPlayAreaSize(size =>
        {
            m_PlaySpace.SetActive(true);
#if UNITY_ANDROID
            m_PlaySpace.SetActive(false);
#endif
            m_PlaySpace.SetWidth(size.x);
            m_PlaySpace.SetHeight(size.z);

            foreach (var scale in GetComponentsInChildren<ScaleToRect>())
            {
                scale.ManualUpdate();
                scale.enabled = false;
            }
        });
    }

    protected abstract void RequestPlayAreaSize(Action<Vector3> callback);
}