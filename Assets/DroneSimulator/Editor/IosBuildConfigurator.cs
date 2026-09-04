using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DroneSimulator.Editor
{
    public static class IosBuildConfigurator
    {
        private const string ScenePath = "Assets/DroneSimulator/Scenes/MVP_TrainingGround.unity";
        private const string BundleIdentifier = "com.louis0211netizen.dronesimulator";

        [MenuItem("Drone Simulator/Configure iOS Landscape Build")]
        public static void ConfigureIosLandscapeBuild()
        {
            PlayerSettings.companyName = "louis0211-netizen";
            PlayerSettings.productName = "DroneSimulator_v0.1";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, BundleIdentifier);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            EnsureSceneIsInBuildSettings();

            if (UnityEditor.BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }
            else
            {
                Debug.LogWarning("iOS Build Support is not installed on this machine. iOS build target settings were saved where possible.");
            }

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Drone Simulator", "Configured iOS landscape build settings.", "OK");
            }
        }

        public static void EnsureSceneIsInBuildSettings()
        {
            if (!System.IO.File.Exists(ScenePath))
            {
                return;
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            EditorSceneManager.SaveOpenScenes();
        }
    }
}
