using System;
using UnityEngine;

public abstract class TrackedControllerData : ControllerData
{
    public event Action BtnStartSelected = null;
    public TrackedControllerData(string uniqueId, Transform raycast, Transform wrist, Type graphicsType, Handedness handedness) 
        : base(uniqueId, raycast, wrist, graphicsType, handedness)
    {
        m_UIPointer = RaycastRoot.gameObject.ForceComponent<VRUIPointer>();
        m_UIPointer.Init(this, Inputs);
    }

    public override bool HasLaser()
    {
        return true;
    }

    public override void ManualUpdate(bool runClicks = true)
    {
        bool bNotOverUI = NotOverUI();
        base.ManualUpdate(bNotOverUI);

        if (null != m_UIPointer)
        {
            //Over UI
            //Change Laser type and set distance to UI
            var gfx = GetGfx<VRControllerGraphics>();

            if (null != m_UIPointer)
            {
                Debug.DrawLine(RaycastTransform.position, RaycastTransform.position + (RaycastTransform.forward * m_UIPointer.GetUIDistance()), Color.cyan, 0.1f);
            }
        }
    }
}
