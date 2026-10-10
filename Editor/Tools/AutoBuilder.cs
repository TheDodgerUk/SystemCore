#if !NETFX_CORE
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AutoBuilder
{
    private const string LogPrefix = ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ";
    private const string LogSuffix = " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<\n";

    [MenuItem("Tools/AutoBuilder/All", priority = 0)]
    private static void PerformAllPlatformBuilds()
    {
        PerformWindowsStoreBuild();
        PerformStandaloneWindows32Build();
        PerformStandaloneWindows64Build();
    }

    [MenuItem("Tools/AutoBuilder/Windows Store Application", priority = 2)]
    private static void PerformWindowsStoreBuild()
    {
        PerformBuild(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
    }

    [MenuItem("Tools/AutoBuilder/Standalone Windows (32-bit)", priority = 4)]
    private static void PerformStandaloneWindows32Build()
    {
        PerformBuild(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/AutoBuilder/Standalone Windows (64-bit)", priority = 6)]
    private static void PerformStandaloneWindows64Build()
    {
        PerformBuild(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
    }

    private static void PerformBuild(BuildTargetGroup buildGroup, BuildTarget buildTarget)
    {
        if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
        {
            Log("Switching build target to '{0}'", buildTarget);
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildGroup, buildTarget);
        }

        // process command line args
        var args = System.Environment.GetCommandLineArgs().ToList();
        string targetPath = GetTargetPath(args, buildTarget);
        bool batchMode = args.Contains("-batchMode");
        bool autoRun = args.Contains("+autoRun");

        //Log("Target path is '{0}'", targetPath);

        // create directory if does not exist
        string folder = Path.GetDirectoryName(targetPath);
        if (Directory.Exists(folder) == false)
        {
            Log("'{0}' does not exist, creating folder...", folder);
            Directory.CreateDirectory(folder);
        }

        // clear directory
        var directoryInfo = new DirectoryInfo(folder);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
        {
            dir.Delete(true);
        }

        //Log("Starting build...");

        // start build
        string[] scenePaths = GetScenePaths();
        var options = (autoRun ? BuildOptions.AutoRunPlayer : BuildOptions.ShowBuiltPlayer);
        var report = BuildPipeline.BuildPlayer(scenePaths, targetPath, buildTarget, options);

        // log result
        bool success = (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded);
        if (success == true)
        {
            Log("Build completed successfully to '{0}'.");
        }
        else
        {
            Log("Build failed, steps completed:");
            foreach (var step in report.steps)
            {
                Log(step.name);

                foreach (var message in step.messages)
                {
                    Log($"[{message.type}] {message.content}");
                }
            }
        }

        if (batchMode == true)
        {
            EditorApplication.Exit(success ? 0 : -1);
        }
    }

    private static void Log(string log, params object[] args)
    {
        log = string.Format(log, args).PadRight(128, ' ');
        Debug.Log(log + "\n");

        log = string.Concat(LogPrefix, log, LogSuffix);
        System.Console.WriteLine(log);
    }

    private static string GetTargetPath(List<string> args, BuildTarget buildTarget)
    {
        string targetPath = string.Empty;

        int pathIndex = args.IndexOf("+targetPath");
        if (pathIndex >= 0 && pathIndex < args.Count - 1)
        {
            targetPath = args[pathIndex + 1];
        }
        else
        {
            string name = Application.productName.ToLower();
            targetPath = string.Format("Builds/{0}/{1}", buildTarget, name);

            switch (buildTarget)
            {
                case BuildTarget.Android:
                    targetPath += ".apk";
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    targetPath += ".exe";
                    break;
                case BuildTarget.WSAPlayer:
                    break;
                default:
                    break;
            }
        }
        return targetPath;
    }

    private static string[] GetScenePaths()
    {
        return EditorBuildSettings.scenes.ToList().Extract(s => s.path).ToArray();
    }
}
#endif