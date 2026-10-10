#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;



[UnityEditor.InitializeOnLoad]
public static class BuildOverload 
{



    [MenuItem("BuildSetting/BuildSize/Windows VR_Interaction")]
    private static void WindowsVR_Interaction()
    {
        bool unHide = false;
        MoveToHide("Oculus", unHide);
        MoveToHide("SteamVR", unHide);
        MoveToHide("SteamVR_Input", unHide);
        MoveToHide("SteamVR_Resources", unHide);
        MoveToHide("Scripts/Input/InputSystems/SteamVr", unHide);
        MoveToHide("Scripts/Input/InputSystems/Oculus", unHide);
        MoveToHide("Artwork/Models/Controllers/Teleporter/Tool", unHide);
        MoveToHide("Artwork/Models/Resources", unHide);
        MoveToHide("Artwork/Models/Hands/Art", unHide);

        Application.OpenURL("https://dev.azure.com/MusicTribe/MI_R00398/_wiki/wikis/MI_R00398/916/Reduce-Size");
        EditorUtility.DisplayDialog("Windows VR_Interaction", $"Set VR_INTERCTION link to wiki  ", "ok");
    }


    [MenuItem("BuildSetting/BuildSize/Android VR_Interaction")]
    private static void AndroidNon_VR_Interaction()
    {
        bool unHide = false;
        bool steamHide = true;

        MoveToHide("Oculus", unHide);
        MoveToHide("SteamVR", steamHide);
        MoveToHide("SteamVR_Input", steamHide);
        MoveToHide("SteamVR_Resources", steamHide);
        MoveToHide("Scripts/Input/InputSystems/SteamVr", steamHide);
        MoveToHide("Scripts/Input/InputSystems/Oculus", unHide);
        MoveToHide("Artwork/Models/Controllers/Teleporter/Tool", unHide);
        MoveToHide("Artwork/Models/Resources", unHide);
        MoveToHide("Artwork/Models/Hands/Art", unHide);

        Application.OpenURL("https://dev.azure.com/MusicTribe/MI_R00398/_wiki/wikis/MI_R00398/916/Reduce-Size");
        EditorUtility.DisplayDialog("Android VR_Interaction", $"Set VR_INTERCTION link to wiki ", "ok");
    }


    [MenuItem("BuildSetting/BuildSize/Non_VR_Interaction")]
    private static void Non_VR_Interaction()
    {
        bool hide = true;

        MoveToHide("Oculus", hide);
        MoveToHide("SteamVR", hide);
        MoveToHide("SteamVR_Input", hide);
        MoveToHide("SteamVR_Resources", hide);
        MoveToHide("Scripts/Input/InputSystems/SteamVr", hide);
        MoveToHide("Scripts/Input/InputSystems/Oculus", hide);
        MoveToHide("Artwork/Models/Controllers/Teleporter/Tool", hide);
        MoveToHide("Artwork/Models/Resources", hide);
        MoveToHide("Artwork/Models/Hands/Art", hide);

        Application.OpenURL("https://dev.azure.com/MusicTribe/MI_R00398/_wiki/wikis/MI_R00398/916/Reduce-Size");
        EditorUtility.DisplayDialog("Non_VR_Interaction", $"Set Non VR_INTERCTION link to wiki  ", "ok");
    }




    static BuildOverload()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);
    }

    private static void OnBuild(BuildPlayerOptions options)
    {
        StreamingAssetsContents.WriteData();
        var timer = Stopwatch.Diagnostics();
        timer.Reset();

#if UNITY_ANDROID || UNITY_IOS
        List<string> environment = Json.JsonNet.ReadFromFile<List<string>>($"{GlobalConsts.BUNDLE_STREAMING_ENVIRONMENT_DATA}/index");
        if (environment.Count != 1)
        {
            EditorApplication.Beep();
            EditorApplication.Beep();
            EditorApplication.Beep();
            EditorApplication.Beep();
            EditorUtility.DisplayDialog($"ERROR", $"Cannot build a non variant build as it will not compile", "OK");
            return;
        }
#endif

#if UNITY_WEBGL
        if (PlayerSettings.colorSpace == ColorSpace.Linear)
        {
            EditorApplication.Beep();
            EditorApplication.Beep();
            EditorApplication.Beep();
            EditorApplication.Beep();
            EditorUtility.DisplayDialog($"ERROR", $"Linear colourspace is not completely supported in WebGL, UI/camera draw order have issues.", "OK");
            UnityEngine.Debug.LogError("Linear colourspace is not completely supported in WebGL, UI/camera draw order have issues.");
            return;
        }
#endif



#if UNITY_ANDROID && VR_INTERACTION
        string uploadFileNameString = "";
        if (options.locationPathName.CaseInsensitiveContains("MonitorMayhem"))
        {
            if(EditorUtility.DisplayDialog($"Upload", $"Do you want to upload MonitorMayhem build", "Upload build", "Nothing") == true)
            {
                uploadFileNameString = "UploadMonitorMayhem.bat";
            }
        }

        if (options.locationPathName.CaseInsensitiveContains("Museum"))
        {
            if (EditorUtility.DisplayDialog($"Upload", $"Do you want to upload Museum build", "Upload build", "Nothing") == true)
            {
                uploadFileNameString = "UploadMusicTribeMuseum.bat";
            }
        }
#endif

        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(options);

        timer.Stop();
        if(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
#if UNITY_ANDROID && VR_INTERACTION
            if (string.IsNullOrEmpty(uploadFileNameString) == false)
            {
                var proc1 = new ProcessStartInfo();
                proc1.UseShellExecute = true;
                string UploadingDir = $"{Application.dataPath}/Uploading";

                proc1.WorkingDirectory = UploadingDir;

                proc1.FileName = uploadFileNameString;

                proc1.WindowStyle = ProcessWindowStyle.Normal;
                Process.Start(proc1);
            }
#endif
            EditorApplication.Beep();
            EditorUtility.DisplayDialog($"Completed", $"Build Completed", "OK");

        }
        else
        {
            EditorApplication.Beep();
            EditorUtility.DisplayDialog($"Completed", $"Build FAILED", "OK");
        }




    }

    private static void MoveToHide(string folder, bool hide)
    {
        var root = Application.dataPath;
        var fromFolder = $"{root}/{folder}";
        var toFolder = $"{root}/{folder}";
        if (hide == true)
        {
            toFolder = $"{toFolder}~";
        }
        else
        {
            fromFolder = $"{fromFolder}~";

        }

        if (Directory.Exists($"{fromFolder}") == true && Directory.GetFiles($"{fromFolder}").Length > 0)
        {
            if (Directory.Exists($"{toFolder}") == true)
            {
                Directory.Delete($"{toFolder}");
            }

            Directory.Move($"{fromFolder}", $"{toFolder}");

            if (Directory.Exists($"{fromFolder}") == true)
            {
                Directory.Delete($"{fromFolder}");
            }
        }
    }

}

#endif
