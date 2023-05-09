using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UIReticleData
{
    public UIReticle.ReticleState State;
    public float ScrollValue;
}

public class UIReticle : MonoBehaviour
{
    public enum ReticleState
    {
        Normal = 1,
        Hover = 2,
        Scroll = 3
    }

    //The speed that the visual reticle will move
    private float m_fReticleSmoothing = 10.0f;
    private Vector3 m_LastReticlePosition = Vector3.zero;

    //Was the reticle visible last frame
    private bool m_bLastVisibleState = true;
    [SerializeField]
    private RectTransform m_Transform;

    //Animator of the reticle... changes between all the visual states
    private Animator m_Animator;

    //Current and last state... used to trigger animation changes only when there is a new state
    [SerializeField]
    private UIReticleData m_ReticleData;
    private ReticleState m_LastCurrentReticleState;

    public void Init()
    {
        m_Transform = gameObject.ForceComponent<RectTransform>();
        m_Animator = gameObject.GetComponent<Animator>();

        m_ReticleData = new UIReticleData();

        gameObject.SetActive(true);
        SetReticle(false, Vector3.zero);
        m_LastCurrentReticleState = ReticleState.Normal;
    }

    private void UpdatePointerState()
    {
        if (null != VRInputModule.Instance && VRInputModule.Instance.Pointers.Count > 0)
        {
            m_ReticleData = VRInputModule.Instance.GetPointerState(VRInputModule.Instance.Pointers[0]);
            OnChangeReticleState(m_ReticleData);
        }
    }

    private void OnChangeReticleState(UIReticleData data)
    {
        if (null != m_Animator)
        {
            if (m_LastCurrentReticleState != data.State)
            {
                m_Animator.SetInteger("State", (int)data.State);
            }

            m_Animator.SetFloat("Scroll Blend", data.ScrollValue);
        }
        m_LastCurrentReticleState = data.State;
    }

    public void SetReticle(bool bVisible, Vector3 position)
    {
        if(VRInputModule.Instance.ControlerTypeRef == GlobalConsts.ControllerType.PhysicsHand)
        {
            bVisible = false;
        }
        Vector3 smoothedPosition = Vector3.Lerp(m_LastReticlePosition, position, Time.deltaTime * m_fReticleSmoothing);
        if (m_bLastVisibleState != bVisible)
        {
            gameObject.SetActive(bVisible);
            //Set position fresh
            smoothedPosition = position;
            m_bLastVisibleState = bVisible;
        }

        UpdatePointerState();
        SetReticlePosition(smoothedPosition);
        m_LastReticlePosition = smoothedPosition;
    }

    private void SetReticlePosition(Vector2 screenCoords)
    {
        m_Transform.localPosition = screenCoords;
    }

    private void SetReticlePosition(Vector3 worldSpace)
    {
        //Set position in localspace
        Vector3 localSpace = transform.parent.InverseTransformPoint(worldSpace);
        SetReticlePosition(new Vector2(localSpace.x, localSpace.y));
    }
}