using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOptions : MonoBehaviour {

    public bool m_Clear = true;

	void Update () {
        GetComponent<Camera>().clearStencilAfterLightingPass = m_Clear;
	}
}
