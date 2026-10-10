using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveableModule : MetaAttributeModule
{
    public event Action<MoveableModule> Thrown = null;
    public event Action OnValidTargetPosition;
    public event Action OnInvalidTargetPosition;
    public event Action OnInvalidPosition;
    public event Action<ControllerData> OnMoveBegan;
    public event Action<ControllerData> OnMoveUpdate;
    public event Action<ControllerData> OnMoveEnd;

    #region Variables
    private ModuleObject m_ModuleObject;
    private RenderModule m_RenderModule;


    [SerializeField]
    private Vector3 m_TargetPosition;                   //Current target position of this object
    [SerializeField]
    private Quaternion m_TargetRotation;
    private Vector3 m_GrabPositionOffset;
    private Quaternion m_GrabRotationOffset;

    //Can be used if current position is determined to be completely invalid. i.e output of the world!
    private Vector3 m_LastPosition;
    private Quaternion m_LastRotation;

    public bool m_bDebugSnap = false;
    public bool m_bDrawGizmos = false;

    private Transform m_MoveTransform;

    [SerializeField]
    private List<Collider> m_CurrentTriggerEnters = new List<Collider>();

    [SerializeField]
    private bool m_bIsValidPosition = false;
    #endregion

    #region Getters and setters
    public bool IsValidPosition()
    {
        //if (m_iNumFixedUpdates > m_iCyclesTillPhysicsUpdates)
        //{
        //    m_bIsValidPosition = !TriggeredCollision() && true == m_bOverEnvironment;
        //    return m_bIsValidPosition;
        //}

        //m_bIsValidPosition = false;
        return m_bIsValidPosition;
    }

    /// <summary>
    /// Allows an external component which processes the position in more detail to inform this class if the calculated position
    /// became valid. Positions are invalid until processed to determine valid end point
    /// </summary>
    public void SetPositionValidity(bool bValidity)
    {
        //TODO: Have this tell the validation module position is valid
        //m_bValidTargetPosition = bValidity;
    }

    public Vector3 TargetPosition
    {
        get { return m_TargetPosition; }
        set { m_TargetPosition = value; }
    }

    public Quaternion TargetRotation
    {
        get { return m_TargetRotation; }
        set { m_TargetRotation = value; }
    }

    public Transform GetMoveTransform()
    {
        return m_MoveTransform;
    }

    public void ResampleLastPositionAndRotation()
    {
        m_LastPosition = m_MoveTransform.position;
        m_LastRotation = m_MoveTransform.rotation;
    }

    public void OverridePositionAndRotation(Vector3 position, Quaternion rotation)
    {
        //Only allowed to override an invalid position... i.e. user spawns something inside another
        //Should display invalid state but allow updates to continue for user to resolve issue
        if (false == IsValidPosition())
        {
            m_LastPosition = position;
            m_LastRotation = rotation;
        }
    }

    public void Disable()
    {
        m_ModuleObject.Interactable.GetSubscription(this, subscription =>
        {
            subscription.Grab.Clear();
        });
    }

    private bool TriggeredCollision()
    {
        //Quick clean up as another object that hit us could have been destroyed so ours is null but taking up a place.
        int iCount = 0;
        while (iCount < m_CurrentTriggerEnters.Count)
        {
            if (null == m_CurrentTriggerEnters[iCount])
            {
                m_CurrentTriggerEnters.RemoveAt(iCount);
            }
            else { iCount++; }
        }

        return m_CurrentTriggerEnters.Count > 0;
    }
    #endregion

    #region Initialisation
    public override void Initialise(ModuleObject moduleObject, Action callback)
    {
        this.enabled = false;
        m_ModuleObject = moduleObject;
        m_MoveTransform = moduleObject.RootTransform;

        m_TargetPosition = m_MoveTransform.position;
        m_TargetRotation = m_MoveTransform.rotation;

        m_LastPosition = m_TargetPosition;
        m_LastRotation = m_TargetRotation;


        callback?.Invoke();
    }

    public override void OnModulesInitialised(ModuleObject moduleObject)
    {
        base.OnModulesInitialised(moduleObject);

        m_RenderModule = moduleObject.GetModule<RenderModule>();
        Signup(moduleObject.Interactable);
    }

    public override void OnModulesLoaded(ModuleObject moduleObject)
    {
        base.OnModulesLoaded(moduleObject);
        this.enabled = true;
    }

    public void Signup() => Signup(m_ModuleObject.Interactable);

    public void Signup(InteractableModule interactable)
    {
        interactable.Subscribe(this, subscription =>
        {
            subscription.Grab.Begin = OnGrabBegin;
            subscription.Grab.Update = OnGrabUpdate;
            subscription.Grab.End = OnGrabEnd;
        });
    }
    #endregion

    #region Events
    public void OnGrabBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        //Lock this object so we are linked to the controller until grab is triggered again
        interaction.Main.LockToObject(this);
        //Debug.Log("Grab began\n");

        if (Mathf.Abs(interaction.Main.StretchDistance - interaction.Main.CurrentDistance) > 0.1f)
        {
            interaction.Main.StretchDistance = interaction.Main.CurrentDistance;
        }

        m_RenderModule?.ToggleCollisionLayer(false);

        if (null != m_MoveTransform)
        {
            BeginHandGrab(interaction.Main);

            OnMoveBegan?.Invoke(interaction.Main);
        }
    }

    public void OnGrabUpdate(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (false == interaction.Main.IsLocked)
        {
            //If not locked... lock object. This can happen if user holds grip then rolls over an object.
            return;
        }

        if (null == m_MoveTransform)
        {
            Debug.LogError("NULL MoveTransform In update: " + gameObject.name);
            return;
        }

        //Process hologram colours based on calculated validity
        UpdateHandGrab(interaction.Main);

        bool bValidPosition = true;
        bool bLogicValid = true;
        OnMoveUpdate?.Invoke(interaction.Main);

        //Totally invalid position?
        if (true == bValidPosition)
        {
            //Set position and rotation
            m_MoveTransform.position = m_TargetPosition;
            m_MoveTransform.rotation = m_TargetRotation;

            m_LastPosition = m_TargetPosition;
            m_LastRotation = m_TargetRotation;

            m_bIsValidPosition = true;
            //Valid placement but logically valid
            if (true == bLogicValid)
            {
                SetObjectState(ObjectState.Confirmed, interaction.Main);
                // ValidTargetPosition();
            }
            else
            {
                SetObjectState(ObjectState.Valid, interaction.Main);
                //  InvalidTargetPosition();
            }
        }
        else
        {
            m_bIsValidPosition = false;
            m_MoveTransform.position = m_LastPosition;
            m_MoveTransform.rotation = m_LastRotation;
            //Completely invalid position
            SetObjectState(ObjectState.Invalid, interaction.Main);
            //InvalidPosition();
        }
    }

    public void OnGrabEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (false == interaction.Main.IsLocked)
        {
            //If not locked... lock object. This can happen if user holds grip then rolls over an object.
            return;
        }

        interaction.Main.UnlockFromObject();
        //Debug.Log("Grab Ended\n");

        m_RenderModule?.ToggleCollisionLayer(true);

        //If not valid position reset to last good position and rotation
        if (false == IsValidPosition())
        {
            m_TargetPosition = m_LastPosition;
            m_TargetRotation = m_LastRotation;
        }

        if (true == IsValidPosition())
        {
            //Should only be able to process end of movement if position is valid
            OnMoveEnd?.Invoke(interaction.Main);
        }

        EndHandGrab(interaction.Main);
    }
    #endregion

    #region Methods
    #region Hand Grab controls
    /// <summary>
    /// Object is 'attached' to controller and mimics controller position and rotation
    /// </summary>
    private void BeginHandGrab(ControllerData data)
    {
        m_TargetPosition = m_MoveTransform.position;
        m_TargetRotation = m_MoveTransform.rotation;

        //Set offsets for rotation and position
        m_GrabPositionOffset = data.WristTransform.InverseTransformPoint(m_TargetPosition) * CameraControllerVR.Instance.Scale;
        m_GrabRotationOffset = SubtractRotation(m_TargetRotation, data.WristTransform.rotation);
    }

    private void UpdateHandGrab(ControllerData data)
    {
        if (true == data.m_bOffsetOnAction)
        {
            m_TargetRotation = data.WristTransform.rotation * m_GrabRotationOffset;
            Vector3 relativeOffset = data.WristTransform.rotation * m_GrabPositionOffset;
            m_TargetPosition = data.WristTransform.position + relativeOffset;
        }
        else
        {
            m_TargetRotation = data.WristTransform.rotation;
            m_TargetPosition = data.WristTransform.position;
        }
    }

    private void EndHandGrab(ControllerData data)
    {
        var grabState = data.Inputs.BtnGrab;
        var framePos = grabState.Position.Delta;

        SetObjectState(ObjectState.None, data);
        if (framePos.sqrMagnitude - data.CurrentAcceleration > 1.0f)
        {
            var interactable = m_ModuleObject.Interactable;
            interactable.Toggle(false);

            float timeScale = (1f / Time.deltaTime);
            var torque = grabState.Rotation.Delta.eulerAngles * timeScale;
            var force = framePos * timeScale;


            Thrown?.Invoke(this);

            this.WaitFor(2f, m_ModuleObject.Shutdown);
        }
    }

    #endregion

    private void SetObjectState(ObjectState state, ControllerData controller)
    {
        m_ModuleObject.SetObjectState(state);

        var gfx = controller.GetGfx<VRControllerGraphics>();
        var hands = gfx?.GetVisual<HandControllerVisual>();

        switch (state)
        {
            case ObjectState.Invalid:
                OnInvalidPosition?.Invoke();
                break;
            case ObjectState.Valid:
                OnInvalidTargetPosition?.Invoke();
                break;
            case ObjectState.Confirmed:
                OnValidTargetPosition?.Invoke();
                break;
        }
    }

    //caclulate the rotational difference from A to B
    private Quaternion SubtractRotation(Quaternion B, Quaternion A)
    {
        Quaternion C = Quaternion.Inverse(A) * B;
        return C;
    }
    #endregion
}
