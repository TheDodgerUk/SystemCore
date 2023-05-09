using System;
using UnityEditor;
using UnityEngine;

namespace BuildSystem
{
    public class BambooBuilder
    {
        public static int ReturnCode = 0;

        public static void BuildApplication() => ExecuteBuildJob(BuildApplicationJob.BuildApplication);

        private static void ExecuteBuildJob(Action job)
        {
            ReturnCode = 0;
            var logger = new ExternalLogger();
            try
            {
                job();
            }
            catch (Exception ex)
            {
                if (ReturnCode == 0)
                {
                    ReturnCode = 100;
                }
                Debug.Log(ex.ToString());
            }
            finally
            {
                Debug.Log($"Exiting Unity Editor. Return Code is {ReturnCode}");
                logger.Dispose();
            }
            EditorApplication.Exit(ReturnCode);
        }
    }
}
