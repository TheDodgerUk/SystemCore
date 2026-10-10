using UnityEngine;

[System.Serializable]
public class VirtualControllerData : ControllerData
{
    public bool m_bGrab;
    public bool m_bInteract;
    public bool m_bHover;
    private bool m_bLastGrab;
    private bool m_bLastInteract;
    private bool m_bLastHover;

    public GameObject m_Model;
    public bool IsBusy;

    public VirtualControllerData(Transform trackingSpace, Transform origin, string id, System.Type graphicsType) :
        base(id.ToString(), trackingSpace, origin, null)
    {
        IsBusy = false;
    }

    public override bool HasLaser() => true;

    public override bool IsConnected() => true;

    public override void Vibrate(float intensity, float duration) { }

    protected override void UpdateInput()
    {
        Inputs.BtnGrab.State = EvalState(m_bLastGrab, m_bGrab);
        Inputs.Interact.State = EvalState(m_bLastInteract, m_bInteract);
        Inputs.Hover.State = EvalState(m_bLastHover, m_bHover);

        m_bLastGrab = m_bGrab;
        m_bLastInteract = m_bInteract;
        m_bLastHover = m_bHover;
        base.UpdateInput();
    }

    private TriggerState EvalState(bool bLast, bool bCurrent)
    {
        if (true == bCurrent && false == bLast)
        {
            return TriggerState.Down;
        }
        else if (false == bCurrent && true == bLast)
        {
            return TriggerState.Up;
        }
        else
        {
            return (true == bCurrent) ? TriggerState.Held : TriggerState.None;
        }
    }

    public void SetCollider(Collider collider)
    {
        CurrentCollider = collider;
    }

    public void SetRootTransform(Vector3 position, Quaternion rotation)
    {
        if (null != RaycastRoot)
        {
            RaycastRoot.position = position;
            RaycastRoot.rotation = rotation;
        }
    }
}