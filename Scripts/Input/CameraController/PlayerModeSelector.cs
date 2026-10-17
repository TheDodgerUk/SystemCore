using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class PlayerModeSelector : MonoBehaviour
{
    [Header("VR Player")]
    public GameObject vrPlayer;

    [Header("Desktop Camera")]
    public Vector3 desktopPosition = new Vector3(0, 1.6f, -2f);
    public Vector3 desktopRotation = new Vector3(15, 0, 0);

    private GameObject desktopPlayer;

    public void SetupPlayer(GameObject vrPlayerItem)
    {
        vrPlayer = vrPlayerItem;
        StartCoroutine(SetupPlayerCoroutine());
    }

    private IEnumerator SetupPlayerCoroutine()
    {
        // Give XR time to initialise
        yield return null;

        bool vrActive = XRSettings.isDeviceActive;

        Debug.LogError("VR Active: " + vrActive);

        if (vrActive)
        {
            vrPlayer.SetActive(true);
        }
        else
        {
            vrPlayer.SetActive(false);
            CreateDesktopCamera();
        }
    }

    private void CreateDesktopCamera()
    {
        desktopPlayer = new GameObject("DesktopPlayer");

        Camera cam = desktopPlayer.AddComponent<Camera>();

        cam.tag = "MainCamera";
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.cullingMask = ~0;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 1000f;

        desktopPlayer.transform.position = desktopPosition;
        desktopPlayer.transform.eulerAngles = desktopRotation;

        desktopPlayer.AddComponent<AudioListener>();

        Debug.Log("Desktop camera created");
    }
}
