using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDEffect
{
    protected float m_fMaxTimeChange = 0.1f;
    public virtual void Init() { }
    public virtual void UpdateEffect(LEDPanel ledPanel) { }
}
