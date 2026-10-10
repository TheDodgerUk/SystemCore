using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristWatch : MonoBehaviour
{
    private Transform m_Transform;

	// Use this for initialization
	public void Init(ControllerData controller)
    {
        m_Transform = transform;
        m_Transform.SetParent(controller.WristTransform);

        //Setup everything else
        Setup();
    }

    private void Setup()
    {
        //Setup screen

        //Load additional graphics

    }
}
