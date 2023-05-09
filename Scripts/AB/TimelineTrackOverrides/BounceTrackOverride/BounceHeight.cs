using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BounceHeight : MonoBehaviour
{
    [SerializeField]
    private float m_HeightOffset = 1f;
    public float m_BounceSpeed = 1.0f;
    [SerializeField]
    private float m_BounceHeight = 0.15f;
    public float m_BounceStrength = 0f;

    [SerializeField]
    private float m_Time = 0.0f;

    public void Init(float bounceHeight)
    {
        m_Time = 0f;
        m_BounceHeight = bounceHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (true == Application.isPlaying)
        {
            m_Time += Time.deltaTime * m_BounceSpeed;
        }
        else
        {
            m_Time += 0.15f * m_BounceSpeed;
        }

        float direction = Mathf.Sin(m_Time);

        Vector3 position = (Vector3.up * direction) * m_BounceHeight * m_BounceStrength;
        position += (Vector3.up * m_HeightOffset);
        transform.localPosition = position;
    }

    public void SetBounce(float Speed, float Strength)
    {
        m_BounceSpeed = Speed;
        m_BounceStrength = Strength;
    }
}
