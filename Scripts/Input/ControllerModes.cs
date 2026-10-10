using UnityEngine;

public class ControllerModes : MonoBehaviour
{
    //public enum ExtensionMode
    //{
    //    Linear,
    //    Acceleration
    //}
    //private Teleporter m_Teleporter;
    //private Locomotion m_Locomotion;

    //Activate hand after distance
    //[SerializeField]
    //private float m_fLerpSmoothing = 6.0f;
    //private float m_fSkinThickness = 0.1f;

    //[SerializeField]
    //private float m_fPositionAnchorSmoothing = 30.0f;
    //[SerializeField]
    //private float m_fRotationAnchorSmoothing = 30.0f;

    //[SerializeField]
    //private ExtensionMode m_ExtensionMode = ExtensionMode.Acceleration;

    //private Dictionary<ControllerData, HandControllerVisual> m_StretchedControllers = new Dictionary<ControllerData, HandControllerVisual>();

    private void Awake()
    {
        //InputManager.Instance.AnySubscription.TouchStick.Begin += OnThumbstickTouchDown;
        //InputManager.Instance.AnySubscription.TouchStick.Update += OnThumbstickTouch;
        //InputManager.Instance.AnySubscription.TouchStick.End += OnThumbstickTouchUp;

        //InputManager.Instance.AnySubscription.ClickStick.End += OnThumbstickClickUp;

        //InputManager.Instance.VoidSubscription.Interact.Begin += OnInteractBegin;
        //InputManager.Instance.VoidSubscription.Interact.Begin += OnInteractUpdate;
        //InputManager.Instance.VoidSubscription.Interact.End += OnInteractEnd;

        //m_Teleporter = GetComponent<Teleporter>();
        //m_Locomotion = GetComponent<Locomotion>();
    }

    //private void OnInteractBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    if (interaction.Main.IsLocked == false)
    //    {
    //        if (CameraController.Instance.TeleportingAllowed == true)
    //        {
    //            interaction.Main.LockToObject(this);
    //            m_Teleporter.ToggleVisuals(interaction.Main, true);
    //        }
    //    }
    //}

    //private void OnInteractUpdate(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    if (interaction.Main.LockedObject == this)
    //    {
    //        m_Teleporter.UpdateLaser(interaction.Main);
    //    }
    //}

    //private void OnInteractEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    if (interaction.Main.LockedObject == this)
    //    {
    //        interaction.Main.UnlockFromObject();

    //        if (CameraController.Instance.TeleportingAllowed == true)
    //        {
    //            CameraController.Instance.TeleportingAllowed = false;
    //            bool success = m_Teleporter.Teleport(interaction.Main, () =>
    //            {
    //                m_Teleporter.ToggleVisuals(interaction.Main, false);
    //            }, () =>
    //            {
    //                CameraController.Instance.TeleportingAllowed = true;
    //            });

    //            if (success == false)
    //            {
    //                m_Teleporter.ToggleVisuals(interaction.Main, false);
    //                CameraController.Instance.TeleportingAllowed = true;
    //            }
    //        }
    //    }
    //}

    //private void OnThumbstickTouchDown(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    if (interaction.Main.IsLocked == false)
    //    {
    //        interaction.Main.LockToObject(this);
    //    }
    //}

    //private void OnThumbstickTouch(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    //For each controller
    //    for (int i = 0; i < interaction.Active.Count; i++)
    //    {
    //        ControllerData controller = interaction.Active[i];

    //        //Get direction of thumbstick
    //        Vector2 axis = controller.Inputs.TouchStick.MaxAxis;

    //        //Is vertical movement?
    //        if(Mathf.Abs(axis.y) > Mathf.Abs(axis.x))
    //        {
    //            switch (controller.ControllerMode)
    //            {
    //                case ControllerMode.Teleporter:
    //                    float fValue = axis.y.RoundToNearest(1f);
    //                    controller.Raycaster.SetTeleportArc(fValue);
    //                    break;

    //                case ControllerMode.Selector:
    //                    ProcessExtendableHand(controller, axis);

    //                    if (controller.StretchDistance > Configs.Controller.ExtendHandMinDistance)
    //                    {
    //                        ExtendController(controller);
    //                    }
    //                    else
    //                    {
    //                        RetractController(controller);
    //                    }
    //                    break;
    //            }                
    //        }
    //    }
    //}

    //private void OnThumbstickTouchUp(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    Vector2 axis = interaction.Main.Inputs.TouchStick.MaxAxis;

    //    ControllerData controller = interaction.Main;
    //    if (controller.LockedObject == this)
    //    {
    //        //release controller
    //        controller.LockToObject(null);

    //        if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
    //        {
    //            SwapControllerModes(controller, axis);
    //        }
    //    }

    //    controller.CurrentAcceleration = 0f;
    //    //Push or pull
    //    if (Mathf.Abs(axis.y) > Mathf.Abs(axis.x))
    //    {
    //        //Pass across our controller data to the created controller visuals
    //        if (m_StretchedControllers.ContainsKey(controller))
    //        {
    //            m_StretchedControllers[controller].OnTouchstickEnd(controller);
    //        }
    //    }
    //}

    //private void SwapControllerModes(ControllerData controller, Vector2 axis)
    //{        
    //    int iValue = (int)axis.x.RoundToNearest(1f);
    //    //Actual change?
    //    if (iValue != 0)
    //    {
    //        controller.ControllerMode = controller.ControllerMode.Wrap(iValue);

    //        switch (controller.ControllerMode)
    //        {
    //            case ControllerMode.Teleporter:
    //                OnTeleporterModeActive(controller);
    //                break;

    //            case ControllerMode.Selector:
    //                OnSelectorModeActive(controller);
    //                break;
    //        }
    //    }
    //}

    //private void OnTeleporterModeActive(ControllerData controller)
    //{
    //    VRControllerGraphics gfx = (VRControllerGraphics)controller.Gfx;
    //    gfx?.SetAttachment(ControllerAttachment.Teleporter);
    //    m_Teleporter.TeleporterActive(true, controller);

    //    //If controller is active as extended hand... hide visuals
    //    if(true == m_StretchedControllers.ContainsKey(controller))
    //    {
    //        //Hide additional parts
    //        HandControllerVisual handVis = m_StretchedControllers[controller];

    //        handVis.Visible(false);
    //        handVis.ActivateBeam(false, null);

    //        //Set controller to original position
    //        controller.Root.SetParent(controller.Origin);
    //        controller.Root.localPosition = Vector3.zero;
    //        controller.Root.localRotation = Quaternion.identity;
    //    }
    //}

    //private void OnSelectorModeActive(ControllerData controller)
    //{
    //    VRControllerGraphics gfx = (VRControllerGraphics)controller.Gfx;
    //    gfx?.SetAttachment(ControllerAttachment.Hand);
    //    m_Teleporter.TeleporterActive(false, controller);

    //    if(true == m_StretchedControllers.ContainsKey(controller))
    //    {
    //        controller.Root.SetParent(null);
    //        HandControllerVisual handVis = m_StretchedControllers[controller];
    //        handVis.Visible(true);
    //        HandControllerVisual controllerVis = gfx?.GetVisualOfType(ControllerAttachment.Hand) as HandControllerVisual;
    //        Transform attachPoint = controllerVis.GetBeamAttachPoint();
    //        handVis.ActivateBeam(true, attachPoint);
    //        UpdateSelectorController(controller, handVis, true);
    //    }
    //}

    //private void ProcessExtendableHand(ControllerData controller, Vector2 axis)
    //{
    //    if (Mathf.Abs(controller.StretchDistance - controller.CurrentDistance) > 0.1f)
    //    {
    //        controller.StretchDistance = controller.CurrentDistance;
    //    }

    //    float value = 0f;
    //    if (m_ExtensionMode == ExtensionMode.Linear)
    //    {
    //        value = axis.y * Configs.Controller.LinearPushSpeed * CameraController.Instance.Scale * Time.deltaTime;
    //    }
    //    else if(m_ExtensionMode == ExtensionMode.Acceleration)
    //    {
    //        controller.CurrentAcceleration += Configs.Controller.AccelerationPushSpeed * CameraController.Instance.Scale * Time.deltaTime;
    //        value = axis.y * controller.CurrentAcceleration * Time.deltaTime;
    //    }
    //    float stretchDistance = controller.StretchDistance + value;
    //    controller.StretchDistance = Mathf.Clamp(stretchDistance, 0f, Configs.Controller.ExtendHandMaxDistance);
    //    controller.CurrentDistance = controller.StretchDistance;
    //}

    //private void OnThumbstickClickUp(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    //{
    //    ControllerData controller = interaction.Main;

    //    switch (controller.ControllerMode)
    //    {
    //        case ControllerMode.Teleporter:
    //            break;

    //        case ControllerMode.Selector:
    //            //Retract
    //            RetractController(interaction.Main);
    //            break;
    //    }
    //}

    //private void ExtendController(ControllerData controller)
    //{
    //    if (false == m_StretchedControllers.ContainsKey(controller))
    //    {
    //        m_StretchedControllers.Add(controller, CreateHand(controller));
    //        VRControllerGraphics vrgfx = controller.Gfx as VRControllerGraphics;
    //        HandControllerVisual hand = vrgfx.GetVisualOfType(ControllerAttachment.Hand) as HandControllerVisual;
    //        hand?.SetExtendedHand(true);
    //        controller.Root.SetParent(null);
    //    }
    //}

    //private void RetractController(ControllerData controller)
    //{
    //    if (m_StretchedControllers.ContainsKey(controller))
    //    {
    //        GameObject obj = m_StretchedControllers[controller].gameObject;
    //        GameObject.Destroy(obj);
    //        m_StretchedControllers.Remove(controller);
    //        controller.StretchDistance = 0f;
    //        controller.CurrentDistance = 0f;
    //        controller.CurrentAcceleration = 0f;
    //        controller.Root.SetParent(controller.Origin);
    //        controller.Root.localPosition = Vector3.zero;
    //        controller.Root.localRotation = Quaternion.identity;

    //        VRControllerGraphics vrgfx = controller.Gfx as VRControllerGraphics;
    //        HandControllerVisual hand = vrgfx.GetVisualOfType(ControllerAttachment.Hand) as HandControllerVisual;
    //        hand?.SetExtendedHand(false);
    //    }
    //}

    //private HandControllerVisual CreateHand(ControllerData controller)
    //{
    //    HandControllerVisual visual = null;
    //    string side = controller.Hand.ToString();
    //    GameObject hand = Resources.Load<GameObject>(side + "Hand") as GameObject;

    //    if(null != hand)
    //    {
    //        hand = GameObject.Instantiate(hand) as GameObject;

    //        hand.transform.localScale = Vector3.one * CameraController.Instance.Locomotion.Scale;
    //        visual = hand.AddComponent<HandControllerVisual>();
    //        visual.Init(hand);
    //        visual.Visible(true);

    //        Transform attachPoint = controller.Reticle;
    //        VRControllerGraphics vrGFX = controller.Gfx as VRControllerGraphics;
    //        if(null != vrGFX)
    //        {
    //            HandControllerVisual handVis = vrGFX.GetVisualOfType(ControllerAttachment.Hand) as HandControllerVisual;
    //            attachPoint = handVis?.GetBeamAttachPoint();
    //        }
    //        visual.ActivateBeam(true, attachPoint);
    //    }

    //    return visual;
    //}

    //private void Update()
    //{
    //    foreach(KeyValuePair<ControllerData, HandControllerVisual> kvp in m_StretchedControllers)
    //    {
    //        ControllerData controller = kvp.Key;

    //        //Only update if we are in the correct state
    //        if(controller.ControllerMode == ControllerMode.Selector)
    //        {
    //            if (controller.Inputs.Grab.State == TriggerState.Held && 
    //                CameraController.Instance.Locomotion == controller.LockedObject)
    //            {
    //                UpdateAnchor(controller, kvp.Value);
    //            }
    //            else
    //            {
    //                UpdateSelectorController(controller, kvp.Value);
    //            }
    //            HandControllerVisual handVis = kvp.Value;
    //            //Pass across our controller data to the created controller visuals
    //            handVis.UpdateAllControllerStates(controller);

    //            VRControllerGraphics gfx = kvp.Key.Gfx as VRControllerGraphics;
    //            HandControllerVisual visual = gfx?.GetVisualOfType(ControllerAttachment.Hand) as HandControllerVisual;
    //            handVis.SetColor(visual.GetColor());
    //        }
    //    }
    //}

    //public void PreRescaleUser()
    //{
    //    foreach (KeyValuePair<ControllerData, HandControllerVisual> kvp in m_StretchedControllers)
    //    {
    //        ControllerData controller = kvp.Key;
    //        kvp.Value.transform.SetParent(controller.Origin);
    //        controller.Root.SetParent(controller.Origin);
    //    }
    //}

    //public void PostRescaleUser()
    //{
    //    foreach (KeyValuePair<ControllerData, HandControllerVisual> kvp in m_StretchedControllers)
    //    {
    //        ControllerData controller = kvp.Key;

    //        kvp.Value.transform.SetParent(null);
    //        controller.Root.SetParent(null);

    //        UpdateSelectorController(controller, kvp.Value, true);
    //    }
    //}

    //private void UpdateSelectorController(ControllerData controller, HandControllerVisual handVis, bool bInstant = false)
    //{
    //    float stretchDist = controller.StretchDistance;// * CameraController.Instance.Scale;
    //    //Raycast into scene from origin position
    //    Ray ray = new Ray(controller.Origin.position, controller.Origin.forward);
    //    RaycastHit info;
    //    if (true == Physics.Raycast(ray, out info, stretchDist, Layers.ControllerMask))
    //    {
    //        //Minus skin distance
    //        stretchDist = info.distance - m_fSkinThickness;
    //        Vector3 position = controller.Origin.position + (controller.Origin.forward * stretchDist);
    //        controller.Root.position = position;
    //        controller.Root.rotation = controller.Origin.rotation;
    //    }
    //    else
    //    {
    //        Vector3 position = controller.Origin.position + (controller.Origin.forward * stretchDist);
    //        if (false == bInstant)
    //        {
    //            controller.Root.position = Vector3.Lerp(controller.Root.position, position, Time.deltaTime * m_fLerpSmoothing);
    //            controller.Root.rotation = Quaternion.Lerp(controller.Root.rotation, controller.Origin.rotation, Time.deltaTime * m_fLerpSmoothing);
    //        }
    //        else
    //        {
    //            //Dont lerp
    //            controller.Root.position = position;
    //            controller.Root.rotation = controller.Origin.rotation;
    //        }
    //        Debug.DrawLine(controller.Origin.position, controller.Root.position, Color.green);
    //    }

    //    controller.CurrentDistance = stretchDist;

    //    handVis.transform.position = controller.Origin.position;
    //    handVis.transform.rotation = controller.Origin.rotation;

    //    handVis.UpdateBeam();
    //}

    //private void UpdateAnchor(ControllerData controller, HandControllerVisual handVis)
    //{
    //    float stretchDist = controller.StretchDistance;
    //    controller.CurrentDistance = stretchDist;

    //    Vector3 position = controller.Origin.position + (controller.Origin.forward * stretchDist);
    //    controller.Root.position = Vector3.Lerp(controller.Root.position, position, Time.deltaTime * m_fPositionAnchorSmoothing);
    //    controller.Root.rotation = Quaternion.Lerp(controller.Root.rotation, controller.Origin.rotation, Time.deltaTime * m_fRotationAnchorSmoothing);

    //    handVis.transform.position = controller.Origin.position;
    //    handVis.transform.rotation = controller.Origin.rotation;

    //    handVis.UpdateBeam();
    //}
}