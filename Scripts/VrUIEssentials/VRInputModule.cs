using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class VRInputModule : PointerInputModule
{
    public static VRInputModule Instance;
    public List<VRUIPointer> Pointers = new List<VRUIPointer>();

    private Dictionary<VRUIPointer, UIReticleData> m_DictOfPointerStates = new Dictionary<VRUIPointer, UIReticleData>();

    private List<RaycastResult> m_HoverVibrateLastResults = new List<RaycastResult>();
    private GameObject m_HoverVibrateLastGameObject = null;
    private List<GameObject> m_LastHovered = new List<GameObject>();
    private Selectable m_HoverSelectable;
    private InputSystemUIInputModule m_InputSystemUIInputModule;

    public GlobalConsts.ControllerType ControlerTypeRef = GlobalConsts.ControllerType.LaserPointer;


    public UIReticleData GetPointerState(VRUIPointer pointer)
    {
        if (m_DictOfPointerStates.ContainsKey(pointer))
        {
            return m_DictOfPointerStates[pointer];
        }

        return null;
    }

    protected float MIN_DRAG_DISTANCE = 0.0005f;

    [SerializeField]
    protected float MAX_DRAG_DISTANCE = 0.2f;

    public void AddVRPointer(VRUIPointer pointer)
    {
        Pointers.Add(pointer);
        m_DictOfPointerStates[pointer] = new UIReticleData()
        {
            State = UIReticle.ReticleState.Normal,
            ScrollValue = 0f
        };
    }

    public void RemoveVRPointer(VRUIPointer pointer)
    {
        Pointers.Remove(pointer);
        m_DictOfPointerStates.Remove(pointer);
    }

    protected override void Awake()
    {
        Instance = this;
        EnvironmentLoader.OnEnvironmentsMenuOpened += OnEnvironmentWindowOpened;
        var inputModule = this.GetComponent<StandaloneInputModule>();
        if(inputModule != null)
        {
            Debug.LogError($"StandaloneInputModule is old, it needs to be replaced, use the button on GetComponent to do this");
        }

        //this.transform.SetAsLastSibling(); // this is to stop weird issues with using the wrong Event System
        m_InputSystemUIInputModule = this.GetComponent<InputSystemUIInputModule>();
    }

    public void SetToVR(bool toVr)
    {
        m_InputSystemUIInputModule.enabled = !toVr;
        this.enabled = toVr;
    }

    public bool IsVr() => this.enabled;

    public void OnEnvironmentWindowOpened(bool bShowing)
    {
        this.enabled = !bShowing;
        if(bShowing == true)
        {
            m_InputSystemUIInputModule.enabled = true;
            Debug.Log("force m_InputSystemUIInputModule to be on so you can pick a program to run");
        }
    }

    public override void Process()
    {
#if VR_INTERACTION
        if (null != InputManagerVR.Instance && true == InputManagerVR.Instance.IsEnabled)
        {

            for (int i = 0; i < Pointers.Count; i++)
            {
                VRUIPointer pointer = Pointers[i];
                ////string pointerName = pointer.transform.parent.parent.parent.name;
                if (true == pointer.IsLocked)
                    continue;

                List<RaycastResult> results = new List<RaycastResult>();
                results = CheckRaycasts(pointer);

                if(pointer.IsSelectionButtonPressed()== false)
                {
                    m_AttemptClickPressed = null;
                }

                pointer.IsHoveringUI = (true == (results.Count > 0));


                // if comment out hover and ReticleLaser UI not move slowly
                Hover(pointer, results);
                Click(pointer, results);
                Drag(pointer, results);
                Scroll(pointer, results);
                ReticleLaser(pointer, results);
            }
        }
#else
        this.enabled = false;
#endif
    }

    private void ReticleLaser(VRUIPointer pointer, List<RaycastResult> results)
    {
        UIReticle.ReticleState state = UIReticle.ReticleState.Normal;
        for (int i = 0; i < results.Count; i++)
        {
            RaycastResult result = results[i];
            var btn = result.gameObject.GetComponent<Button>();
            if (null != btn &&
                true == btn.interactable)
            {
                state = UIReticle.ReticleState.Hover;
            }
        }

        if (true == PointerDragging(pointer) &&
            true == pointer.pointerEventData.BlockClick)
        {
            //Show drag visual
            state = UIReticle.ReticleState.Scroll;
        }

        float scroll = pointer.pointerEventData.GetDragDirection(MIN_DRAG_DISTANCE, MAX_DRAG_DISTANCE);

        if (true == m_DictOfPointerStates.ContainsKey(pointer))
        {
            m_DictOfPointerStates[pointer].State = state;
            m_DictOfPointerStates[pointer].ScrollValue = scroll;
        }

    }

    protected virtual List<RaycastResult> CheckRaycasts(VRUIPointer pointer)
    {
        RaycastResult raycastResult = new RaycastResult();
        raycastResult.worldPosition = pointer.GetOriginPosition();
        raycastResult.worldNormal = pointer.GetOriginForward();

        pointer.pointerEventData.pointerCurrentRaycast = raycastResult;

        List<RaycastResult> raycasts = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer.pointerEventData, raycasts);
        return raycasts;
    }

    protected virtual void CheckPointerHoverClick(VRUIPointer pointer, List<RaycastResult> results)
    {
        if (pointer.m_fHoverDurationTimer > 0f)
        {
            pointer.m_fHoverDurationTimer -= Time.deltaTime;
        }

        if (pointer.m_bCanClickOnHover && pointer.m_fHoverDurationTimer <= 0f)
        {
            pointer.m_bCanClickOnHover = false;
            ClickOnDownLaser(pointer, results, true);
        }
    }


    protected virtual void Hover(VRUIPointer pointer, List<RaycastResult> results)
    {
        //get background position
        if (results.Count > 0)
        {
            RaycastResult result = results[results.Count - 1];
            pointer.pointerEventData.WorldHoverPosition = result.worldPosition + (result.worldNormal * result.distance);
        }

        pointer.pointerEventData.hovered.Clear();

        for (int i = 0; i < results.Count; i++)
        {
            if (false == ValidElement(results[i].gameObject))
            {
                continue;
            }
            pointer.pointerEventData.hovered.Add(results[i].gameObject);
            pointer.pointerEventData.pointerEnter = results[i].gameObject;
            if (false == m_LastHovered.Remove(results[i].gameObject))
            {
                // this will mean single hit 
                if (ControlerTypeRef == GlobalConsts.ControllerType.LaserPointer)
                { 
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerEnter, pointer.pointerEventData, ExecuteEvents.pointerEnterHandler);// only highlight when laserline
                }
            }
        }

        // only items left in the list , are no longer highlighted
        for (int i = 0; i < m_LastHovered.Count; i++)
        {
            if (ControlerTypeRef == GlobalConsts.ControllerType.LaserPointer)
            {
                ExecuteEvents.ExecuteHierarchy(m_LastHovered[i], pointer.pointerEventData, ExecuteEvents.pointerExitHandler); // only un highlight when laserline
            }
        }


        // re get lists
        m_LastHovered.Clear();
        for (int i = 0; i < results.Count; i++)
        {
            RaycastResult result = results[i];
            if (true == ValidElement(result.gameObject))
            {
                m_LastHovered.Add(result.gameObject);
            }
        }

        GameObject lastGameObject = FindLastItem(results);
        if (null != lastGameObject)
        {
            Selectable selectable = lastGameObject.GetComponentInParent<Selectable>();
            if (null != selectable)
            {
                Navigation noNavigation = new Navigation();
                noNavigation.mode = Navigation.Mode.None;
                selectable.navigation = noNavigation;
                if (m_HoverSelectable != selectable)
                {
                    m_HoverSelectable = selectable;
                    if (ControlerTypeRef == GlobalConsts.ControllerType.LaserPointer)
                    {
                        pointer.Vibrate(VibrateType.Buzz); // only vibrate when laserline
                    }
                }
            }
        }
    }


    private GameObject FindLastItem(List<RaycastResult> results)
    {
        GameObject lastObject = null;
        int lengthMax = int.MinValue;
        for (int i = 0; i < results.Count; i++)
        {
            int count = results[i].gameObject.GetGameObjectParentCount();
            if (count > lengthMax)
            {
                lengthMax = count;
                lastObject = results[i].gameObject;
            }
        }
        return lastObject;
    }

    protected virtual void Click(VRUIPointer pointer, List<RaycastResult> results)
    {
        switch (pointer.m_ClickMethod)
        {
            case VRUIPointer.ClickMethods.ClickOnButtonUp:
                ClickOnUpLaser(pointer, results);
                break;
            case VRUIPointer.ClickMethods.ClickOnButtonDown:
                ClickOnDownLaser(pointer, results);
                break;
            default:
                break;
        }
    }

    private bool PointerDragging(VRUIPointer pointer)
    {
        return pointer.IsSelectionButtonPressed() &&
            true == pointer.pointerEventData.IsDragging(MIN_DRAG_DISTANCE);
    }

    protected virtual void Drag(VRUIPointer pointer, List<RaycastResult> results)
    {
        pointer.pointerEventData.dragging = pointer.IsSelectionButtonPressed(); // cant use PointerDragging as pointer.pointerEventData.IsDragging causes issues when it updates

        if (true == pointer.pointerEventData.dragging)
        {
            //Set drag to true to block click if user returns to none drag position
            pointer.pointerEventData.BlockClick = true;
        }

        if (pointer.pointerEventData.pointerDrag)
        {
            if (!ValidElement(pointer.pointerEventData.pointerDrag))
            {
                pointer.pointerEventData.pointerDrag = null;
                return;
            }

            if (pointer.pointerEventData.dragging)
            {
                if (IsHovering(pointer))
                {
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.dragHandler);
                }
            }
            else
            {
                ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.dragHandler);
                ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.endDragHandler);
                for (int i = 0; i < results.Count; i++)
                {
                    ExecuteEvents.ExecuteHierarchy(results[i].gameObject, pointer.pointerEventData, ExecuteEvents.dropHandler);
                }
                pointer.pointerEventData.pointerDrag = null;
            }
        }
        else if (pointer.pointerEventData.dragging)
        {
            for (int i = 0; i < results.Count; i++)
            {
                RaycastResult result = results[i];
                if (!ValidElement(result.gameObject))
                {
                    continue;
                }

                ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.initializePotentialDrag);
                ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.beginDragHandler);
                GameObject target = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.dragHandler);
                if (target != null)
                {
                    pointer.pointerEventData.pointerDrag = target;
                    break;
                }
            }
        }
    }

    protected virtual void Scroll(VRUIPointer pointer, List<RaycastResult> results)
    {
        pointer.pointerEventData.scrollDelta = pointer.GetTouchpadAxis();
        for (int i = 0; i < results.Count; i++)
        {
            if (pointer.pointerEventData.scrollDelta != Vector2.zero)
            {
                GameObject target = ExecuteEvents.ExecuteHierarchy(results[i].gameObject, pointer.pointerEventData, ExecuteEvents.scrollHandler);
            }
        }
    }

    protected virtual void ClickOnDownLaser(VRUIPointer pointer, List<RaycastResult> results, bool forceClick = false)
    {
        pointer.pointerEventData.eligibleForClick = (forceClick ? true : pointer.ValidClick(true));
        if (IsEligibleClickLaser(pointer, results))
        {
            //pointer.pointerEventData.eligibleForClick = false;
            AttemptClick(pointer);
        }
    }


    protected virtual void ClickOnUpLaser(VRUIPointer pointer, List<RaycastResult> results)
    {
        pointer.pointerEventData.eligibleForClick = pointer.ValidClick(false);
        if (!AttemptClick(pointer))
        {
            IsEligibleClickLaser(pointer, results);
        }
    }


    private GameObject m_AttemptClickPressed = null;
    protected virtual bool AttemptClick(VRUIPointer pointer)
    {
        if(m_AttemptClickPressed != null)
        {
            return false;
        }
        if (pointer.pointerEventData.pointerPress)
        {
            m_AttemptClickPressed = pointer.pointerEventData.pointerPress;

            if (!ValidElement(pointer.pointerEventData.pointerPress))
            {
                pointer.pointerEventData.pointerPress = null;
                return true;
            }

            //TODO, this was originally true ==, which made zero sense
            // was changed to false, and tested
            if (false == pointer.pointerEventData.eligibleForClick)
            {
                if (!IsHovering(pointer))
                {
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerUpHandler);
                    pointer.pointerEventData.pointerPress = null;
                }
            }
            else
            {
                // pointer.OnUIPointerElementClick(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, pointer.pointerEventData.pointerPress));
                ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerUpHandler);
                pointer.pointerEventData.pointerPress = null;
            }
            return true;
        }
        return false;
    }

    protected virtual bool IsHovering(VRUIPointer pointer)
    {
        for (int i = 0; i < pointer.pointerEventData.hovered.Count; i++)
        {
            GameObject hoveredObject = pointer.pointerEventData.hovered[i];
            if (pointer.pointerEventData.pointerEnter != null && hoveredObject != null && CheckTransformTree(hoveredObject.transform, pointer.pointerEventData.pointerEnter.transform))
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsEligibleClickLaser(VRUIPointer pointer, List<RaycastResult> results)
    {
        if (pointer.pointerEventData.eligibleForClick &&
    /*false == pointer.pointerEventData.IsDragging(MIN_DRAG_DISTANCE) && */
    false == pointer.pointerEventData.BlockClick)
        {
            for (int i = 0; i < results.Count; i++)
            {
                RaycastResult result = results[i];
                if (!ValidElement(result.gameObject))
                {
                    continue;
                }

                GameObject target = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.pointerDownHandler);
                if (target != null)
                {
                    pointer.pointerEventData.pressPosition = pointer.pointerEventData.position;
                    pointer.pointerEventData.pointerPressRaycast = result;
                    pointer.pointerEventData.pointerPress = target;
                    return true;
                }
            }
        }

        return false;
    }


    private bool CompairList(List<RaycastResult> list1, List<RaycastResult> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }
        for (int i = 0; i < list1.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < list2.Count; j++)
            {
                if (list1[i].gameObject.GetInstanceID() == list2[j].gameObject.GetInstanceID())
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                return false;
            }
        }
        return true;
    }


    protected bool ValidElement(GameObject obj)
    {
        return null != obj;
    }

    protected virtual bool NoValidCollision(VRUIPointer pointer, List<RaycastResult> results)
    {
        return (results.Count == 0 || !CheckTransformTree(results[0].gameObject.transform, pointer.pointerEventData.pointerEnter.transform));
    }

    protected virtual bool CheckTransformTree(Transform target, Transform source)
    {
        if (target == null)
        {
            return false;
        }

        if (target == source)
        {
            return true;
        }

        return CheckTransformTree(target.transform.parent, source);
    }
}
