using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSafetyVR : MonoBehaviour
{


#if UNITY_EDITOR
    void Update()
    {
        if(this.transform.localEulerAngles != Vector3.zero)
        {
            Debug.LogError("At present, you cannot change this, as rotation breaks VR hands, this needs looking at for a better solution");
        }
        if (this.transform.eulerAngles != Vector3.zero)
        {
            Debug.LogError("At present, you cannot change this, as rotation breaks VR hands, this needs looking at for a better solution");
        }

        if (this.transform.parent.localEulerAngles != Vector3.zero)
        {
            Debug.LogError("At present, you cannot change this, rotation in  parent as it breaks VR hands, this needs looking at for a better solution");
        }
        if (this.transform.parent.eulerAngles != Vector3.zero)
        {
            Debug.LogError("At present, you cannot change this, rotation in parent as it breaks VR hands, this needs looking at for a better solution");
        }
    }
#endif
}
