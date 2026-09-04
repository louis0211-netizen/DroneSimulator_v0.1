using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace DroneSimulator.Editor
{
    public static class IosBuildConfigurator
    {
        [MenuItem("Drone Simulator/Configure iOS Landscape Build")]
        public static void ConfigureIosLandscapeBuild()
        {
            PlayerSettings.companyName = "DroneSimulator";
            PlayerSettings.productName = "DroneSimulator_v0.1";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.dronesimulator.v01");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);

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
    }
}
