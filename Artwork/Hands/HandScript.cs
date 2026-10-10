using UnityEngine;

public class HandScript : MonoBehaviour
{
    [SerializeField, Range(0, 1)]
    private float m_Flex;
    [SerializeField, Range(0, 1)]
    private float m_Pinch;
    [SerializeField, Range(0, 3)]
    private int m_Pose;

    private Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        m_Animator.SetFloat("Flex", m_Flex);
        m_Animator.SetFloat("Pinch", m_Pinch);
        m_Animator.SetInteger("Pose", m_Pose);
    }
}
