using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildSystem
{
    public class BuildApplicationJob
    {
        public static void BuildApplication()
        {
            // parse command line args
            var args = new BuildParameters();

            // build application
            var activeScenes = EditorBuildSettings.scenes.ToList();
            var options = new BuildPlayerOptions()
            {
                scenes = activeScenes.Extract(s => s.path).ToArray(),
                locationPathName = args.TargetPath,
                target = args.BuildTarget,
            };

            Debug.Log($"Building ({activeScenes.Count}) scenes:\n{activeScenes.Stringify(s => $" - {s.path}", "\n")}");

            var repeatingLog = new RepeatingLogger();

            repeatingLog.Start("Building application");
            var report = BuildPipeline.BuildPlayer(options);
            repeatingLog.Stop();

            Debug.Log($"Build took {report.summary.totalTime.TotalMinutes.ToString("N2")} minutes");
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build completed successfully.");
            }
            else
            {
                Debug.Log("Build failed, steps completed:");
                foreach (var step in report.steps)
                {
                    Debug.Log(step.name);

                    foreach (var message in step.messages)
                    {
                        Debug.Log($"[{message.type}] {message.content}");
                    }
                }
                BambooBuilder.ReturnCode = 1;
            }
        }

        private class BuildParameters : CommandLineParameters
        {
            public BuildTarget BuildTarget = BuildTarget.NoTarget;
            public string TargetPath = string.Empty;
            public bool CleanBuild = false;

            public BuildParameters() : base()
            {
                BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                if (string.IsNullOrEmpty(TargetPath) == false)
                {
                    TargetPath += GetBuildExtension();
                }
            }

            protected override void ParseArgument(string argument, string[] args, int i)
            {
                switch (argument)
                {
                    case "targetPath":
                        TargetPath = args[i + 1];
                        break;
                    case "cleanBuild":
                        CleanBuild = true;
                        break;
                    default:
                        break;
                }
            }

            private string GetBuildExtension()
            {
                switch (BuildTarget)
                {
                    case BuildTarget.StandaloneOSX:
                        return string.Empty;

                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.StandaloneWindows:
                        return ".exe";

                    default:
                        return string.Empty;
                }
            }
        }
    }
}
