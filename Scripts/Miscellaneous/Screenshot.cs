using UnityEngine;

public class Screenshot : MonoBehaviour {

    private int iCaptureCount = 0;
    [SerializeField]
    private int iSuperSize = 1;

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshot_" + iCaptureCount, iSuperSize);
            iCaptureCount++;
        }
    }
}
