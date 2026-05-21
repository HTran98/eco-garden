using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EcoGarden.Editor
{
    public static class EcoGardenAndroidBuildVerification
    {
        private const string ScenePath = "Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity";
        private const string OutputPath = "Builds/Android/EcoGarden_Level15_VerticalSlice.apk";

        public static void BuildLevel15Android()
        {
            if (!File.Exists(ScenePath))
            {
                throw new FileNotFoundException("Level 15 scene is missing.", ScenePath);
            }

            string outputDirectory = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.Development
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            Debug.Log("Eco Garden Android build result: " + summary.result);
            Debug.Log("Eco Garden Android build output: " + summary.outputPath);
            Debug.Log("Eco Garden Android build size bytes: " + summary.totalSize);
            Debug.Log("Eco Garden Android build total time: " + summary.totalTime);

            if (summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException("Android build failed with result: " + summary.result);
            }
        }
    }
}
