using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField]
    public ExplodeProduct m_ExplodeScript;

    [SerializeField]
    public XrayProduct m_XrayScript;

    [EditorButton]
    public void ExplodeOn()
    {
        m_ExplodeScript?.TriggerExplodeProduct();
    }

    [EditorButton]
    public void ExplodeOff()
    {
        m_ExplodeScript?.TriggerResetProduct();
    }

    [EditorButton]
    public void XrayOn()
    {
        m_XrayScript?.XrayOn();
    }

    [EditorButton]
    public void XrayOff()
    {
        m_XrayScript?.XrayOff();
    }
}
