
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FixVRStereoCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float initialDelay = 0.1f;
    [SerializeField] private float refreshDelay = 0.1f;

    [Header("XR Restart")]
    [SerializeField] private bool restartXRSubsystems = true;
    [SerializeField] private float xrRestartDelay = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool useEditorSerializedProperty = true;

    [SerializeField] private StereoTargetEyeMask currentTargetEye;

    private Camera vrCamera;
    private Coroutine fixCoroutine;

    private void Awake()
    {
        Debug.LogError("Fixing weird issue with VR camera");

        vrCamera = GetComponent<Camera>();

        if (vrCamera == null)
        {
            Debug.LogError("VR Camera not found!");
            return;
        }

        Debug.LogError(
            $"VR Camera found: {vrCamera.gameObject.name}");

        Core.Environment.OnEnvironmentLoadingComplete +=
            Environment_OnEnvironmentLoadingComplete;
    }

    private void Update()
    {
        if (vrCamera != null)
            currentTargetEye = vrCamera.stereoTargetEye;
    }

    private void Environment_OnEnvironmentLoadingComplete()
    {
        if (vrCamera == null || !isActiveAndEnabled)
            return;

        if (fixCoroutine != null)
            StopCoroutine(fixCoroutine);

        fixCoroutine = StartCoroutine(FixStereoCamera());
    }

    private IEnumerator FixStereoCamera()
    {
        yield return new WaitForSecondsRealtime(initialDelay);

        Debug.LogError(
            $"BEFORE: {vrCamera.name} | " +
            $"Target Eye: {vrCamera.stereoTargetEye}");

        // STEP 1: Disable stereo rendering
        SetTargetEye(StereoTargetEyeMask.None);

        yield return new WaitForEndOfFrame();

        // STEP 2: Disable camera
        vrCamera.enabled = false;

        Debug.LogError("VR Camera disabled");

        yield return new WaitForSecondsRealtime(refreshDelay);

        // STEP 3: Restart XR subsystems
        if (restartXRSubsystems)
        {
            yield return StartCoroutine(RestartXR());
        }

        // STEP 4: Enable camera
        vrCamera.enabled = true;

        Debug.LogError("VR Camera enabled");

        yield return new WaitForEndOfFrame();

        // STEP 5: Restore stereo rendering
        SetTargetEye(StereoTargetEyeMask.Both);

        yield return new WaitForEndOfFrame();

        Debug.LogError(
            $"FINAL: {vrCamera.name} | " +
            $"Target Eye: {vrCamera.stereoTargetEye} | " +
            $"Enabled: {vrCamera.enabled}");

        fixCoroutine = null;
    }

    private IEnumerator RestartXR()
    {
        XRGeneralSettings settings = XRGeneralSettings.Instance;

        if (settings == null || settings.Manager == null)
        {
            Debug.LogError("XR Manager not found!");
            yield break;
        }

        XRManagerSettings manager = settings.Manager;

        if (manager.activeLoader == null)
        {
            Debug.LogError("XR loader is not active!");
            yield break;
        }

        Debug.LogError("Stopping XR subsystems...");

        manager.StopSubsystems();

        yield return new WaitForSecondsRealtime(xrRestartDelay);

        Debug.LogError("Starting XR subsystems...");

        manager.StartSubsystems();

        yield return new WaitForEndOfFrame();

        Debug.LogError("XR subsystems restarted");
    }

    private void SetTargetEye(StereoTargetEyeMask target)
    {
        if (vrCamera == null)
            return;

        vrCamera.stereoTargetEye = target;

#if UNITY_EDITOR
        if (useEditorSerializedProperty)
        {
            SerializedObject serializedCamera =
                new SerializedObject(vrCamera);

            SerializedProperty property =
                serializedCamera.FindProperty("m_TargetEye");

            if (property != null)
            {
                property.intValue = (int)target;
                serializedCamera.ApplyModifiedProperties();

                Debug.LogError(
                    $"Serialized Target Eye set to: {target}");
            }
            else
            {
                Debug.LogWarning(
                    "Could not find serialized Target Eye property.");
            }
        }
#endif

        currentTargetEye = vrCamera.stereoTargetEye;

        Debug.LogError(
            $"Target Eye requested: {target} | " +
            $"Actual: {vrCamera.stereoTargetEye}");
    }

    private void OnDestroy()
    {
        Core.Environment.OnEnvironmentLoadingComplete -=
            Environment_OnEnvironmentLoadingComplete;

        if (fixCoroutine != null)
        {
            StopCoroutine(fixCoroutine);
            fixCoroutine = null;
        }
    }
}
