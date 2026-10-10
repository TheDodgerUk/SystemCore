using System;
using UnityEngine;
using UnityEngine.XR;

public static class Device
{
    public static string StreamingPath => Application.streamingAssetsPath;
    public static VRDevices VrType { get; private set; }

    public static void ResetVrType() => VrType = VRDevices.None;

    public static PlatformType GetPlatformType()
    {

#if AR_INTERACTION && (UNITY_IOS || UNITY_ANDROID)
#if UNITY_EDITOR
        return PlatformType.Desktop;
#else
        return PlatformType.Mobile;
#endif
#endif

#if UNITY_WEBGL
        return PlatformType.Mobile;
#endif

#if UNITY_STANDALONE && VR_INTERACTION
        return PlatformType.VirtualOpenXR;
#endif
#if UNITY_STANDALONE
        if (VrType == VRDevices.None)
        {
            return PlatformType.Desktop;
        }
        else
        {
            if (VrType == VRDevices.OpenVR)
            {
                return PlatformType.VirtualOpenVR;
                return PlatformType.VirtualOpenXR;
                ///return PlatformType.VirtualOpenVR;
            }
            else
            {
                return PlatformType.VirtualOpenXR;
            }
        }
#elif UNITY_IOS
        return PlatformType.Mobile;
#elif UNITY_ANDROID
#if VR_INTERACTION
        return PlatformType.Quest;
#else
        return PlatformType.Mobile;
#endif
#else
        return PlatformType.VirtualOpenXR;
#endif
    }

    public static void EnableVr(MonoBehaviour host, Action callback)
    {
#if VR_INTERACTION
        XRSettings.enabled = true;
        // what vr systems do we support
        var devices = XRSettings.supportedDevices.ToList();
        devices.Remove(VRDevices.None.ToString());


        string listOfDevices = "";
        foreach(var item in devices)
        {
            listOfDevices += $"{item}, ";
        }

        ConsoleExtra.Log($"EnableVr : {listOfDevices}", null, ConsoleExtraEnum.EDebugType.StartUp);
        Debug.Log($"EnableVr : listOfDevices: {listOfDevices}");
        SequentialAction.List(devices, (device, onTick) =>
        {          
            if (VrType == VRDevices.None)
            {
                // attempt to load and wait a frame
                XRSettings.LoadDeviceByName(device);
                host.WaitForFrames(3, () => //the documentation says wait a single frame, but i want to be on the safe side
                {
                    if (XRSettings.loadedDeviceName == device)
                    {
                        // we have succeeded to load this system
                        if (device.Contains("OpenVR") == true)// it can now come in as OpenVR Display
                        {
                            VrType = VRDevices.OpenVR;
                        }
                        else if (device.Contains("oculus") == true)// it can now come in as OpenVR Display
                        {
                            VrType = VRDevices.Oculus;
                        }
                        else if(device.Contains("OpenXR") == true)// it can now come in as OpenXR Display
                        {
                            VrType = VRDevices.OpenXR;
                        }
                        else
                        {
                            VrType = VRDevices.OpenVR;
                        }
                        ConsoleExtra.Log($"VR :{device}  which is set to : {VrType}", null, ConsoleExtraEnum.EDebugType.StartUp);
                        Debug.Log($"VR :{device}  which is set to : {VrType}");
                        XRSettings.enabled = true;
                        callback();
                    }
                    else
                    {
                        onTick();
                    }

                });
            }
        }, callback);
#else
        callback?.Invoke();
#endif
    }
}

public enum VRDevices
{
    None,
    Oculus,
    OpenVR,
    OpenXR,
}

public enum PlatformType
{
    None,
    Desktop,
    Mobile,
    VirtualOpenVR,
    VirtualOpenXR,
    Virtual,
    Quest,
}
