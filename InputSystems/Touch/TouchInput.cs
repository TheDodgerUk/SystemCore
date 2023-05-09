using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : InputSystem
{
    private HashSet<int> m_TouchIds = new HashSet<int>();

    public override void Initialise(Action callback) { callback(); }

    protected override void PreUpdate()
    {
        base.PreUpdate();

        int touchCount = Input.touchCount;
        for (int i = 0; i < touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            var id = touch.fingerId;

            if (m_TouchIds.Add(id) == true)
            {
                AddController(new TouchControllerData(touch));
            }
        }
    }

    protected override void PostUpdate()
    {
        base.PostUpdate();

        var endedTouches = new List<TouchControllerData>();
        foreach (TouchControllerData touch in GetControllers())
        {
            if (touch.Phase == TouchPhase.Ended || touch.Phase == TouchPhase.Canceled)
            {
                endedTouches.Add(touch);
                m_TouchIds.Remove(touch.Id);
            }
        }
        endedTouches.ForEach(RemoveController);
    }
}