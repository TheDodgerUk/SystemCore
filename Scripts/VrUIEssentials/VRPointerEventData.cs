using UnityEngine.EventSystems;
using UnityEngine;
using System;

[Serializable]
public class VRPointerEventData : PointerEventData
{
    public Vector3 ClickStartPosition;
    public Vector3 WorldHoverPosition;

    [SerializeField]
    private bool bDragging;

    [SerializeField]
    private float m_fDragCatchupModifier = 0.5f;

    public bool BlockClick = false;

    public void ClickTriggered()
    {
        ClickStartPosition = WorldHoverPosition;
    }
    
    public void ClickReleased()
    {
        BlockClick = false;
    }

    public bool IsDragging(float distance)
    {
        bDragging = (Mathf.Abs(ClickStartPosition.y - WorldHoverPosition.y) > distance) || 
            (Mathf.Abs(ClickStartPosition.x - WorldHoverPosition.x) > distance);
        return bDragging;
    }

    public float GetDragDirection(float distance, float MaxDistance)
    {
        if (true == IsDragging(distance))
        {
            float dist = (WorldHoverPosition.y - ClickStartPosition.y);
            //Get direction
            float direction = Mathf.Abs(dist) / dist;
            dist = Mathf.Abs(dist);

            float value = Mathf.InverseLerp(distance, MaxDistance, dist);

            ClickStartPosition = Vector3.Lerp(ClickStartPosition, WorldHoverPosition, 
                Time.deltaTime * m_fDragCatchupModifier);
            return value * direction;
        }

        return 0f;
    }

    public VRPointerEventData(EventSystem eventSystem) : base(eventSystem)
    {
    }
}