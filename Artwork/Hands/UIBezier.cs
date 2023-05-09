using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBezier : MonoBehaviour
{
    public List<UIToolTip> m_ToolTips = new List<UIToolTip>();
    private Color m_ActiveColor = new Color(1.0f, 0.6f, 0f, 0.25f);
    private Color m_InActiveColor = new Color(0.5f, 0.3f, 0f, 0.1f);

    private void Start()
    {
        List<Transform> children = transform.GetDirectChildren();

        children.ForEach((e) =>
        {
            UIToolTip tool = e.gameObject.GetComponentInChildren<UIToolTip>();

            if(null != tool)
            {
                tool.Init();
                tool.SetButtonColor(m_InActiveColor);
                m_ToolTips.Add(tool);
            }
        });

        SetAllVisible(false);
    }

    public void SetAllVisible(bool bVisible)
    {
        m_ToolTips.ForEach((e) => e.Visible(bVisible));
    }

    private void Update()
    {
        m_ToolTips.ForEach((e) => e.ManualUpdate());
    }

    public void ChangeState(int ID, bool bActive)
    {
        m_ToolTips.ForEach((e) =>
        {
            if (e.m_iControllerID == ID)
            {
                e.SetButtonColor((true == bActive) ? m_ActiveColor : m_InActiveColor);
            }
        });
    }
}