using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace DroneSimulator.Editor
{
    public static class DroneSimulatorBuildPipeline
    {
        private const string ScenePath = "Assets/DroneSimulator/Scenes/MVP_TrainingGround.unity";
        private const string IosBuildPath = "Builds/iOS/DroneSimulator_v0.1";

        [MenuItem("Drone Simulator/Build iOS Xcode Project")]
        public static void BuildIosXcodeProject()
        {
            EnsureSceneExists();
            IosBuildConfigurator.ConfigureIosLandscapeBuild();

            Directory.CreateDirectory(IosBuildPath);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = IosBuildPath,
                target = BuildTarget.iOS,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException("iOS build failed: " + report.summary.result);
            }
        }

        public static void CreateSceneAndConfigureIos()
        {
            EnsureSceneExists();
            IosBuildConfigurator.ConfigureIosLandscapeBuild();
        }

        private static void EnsureSceneExists()
        {
            if (!File.Exists(ScenePath))
            {
                DroneSimulatorSceneBuilder.CreateMvpTrainingScene();
            }
        }
    }
}
