using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MenuItem : Attribute
    {
        public MenuItem(string menuItem) { }
    }

    public static class AssetDatabase
    {
        public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object => null;
        public static void CreateAsset(UnityEngine.Object asset, string path) { }
        public static void SaveAssets() { }
    }

    public static class EditorUtility
    {
        public static bool DisplayDialog(string title, string message, string ok) => true;
    }

    public class SerializedObject
    {
        public SerializedObject(UnityEngine.Object target) { }
        public SerializedProperty FindProperty(string propertyName) => new SerializedProperty();
        public void ApplyModifiedPropertiesWithoutUndo() { }
    }

    public class SerializedProperty
    {
        public UnityEngine.Object objectReferenceValue;
        public bool boolValue;
    }

    public enum BuildTargetGroup { iOS }
    public enum BuildTarget { iOS }
    public enum BuildOptions { None }
    public enum ScriptingImplementation { IL2CPP }
    public enum ApiCompatibilityLevel { NET_Standard_2_1 }

    public struct BuildPlayerOptions
    {
        public string[] scenes;
        public string locationPathName;
        public BuildTarget target;
        public BuildOptions options;
    }

    public static class BuildPipeline
    {
        public static UnityEditor.Build.Reporting.BuildReport BuildPlayer(BuildPlayerOptions options) => new UnityEditor.Build.Reporting.BuildReport();
        public static bool IsBuildTargetSupported(BuildTargetGroup group, BuildTarget target) => true;
    }

    public static class PlayerSettings
    {
        public static string companyName;
        public static string productName;
        public static UIOrientation defaultInterfaceOrientation;
        public static bool allowedAutorotateToPortrait;
        public static bool allowedAutorotateToPortraitUpsideDown;
        public static bool allowedAutorotateToLandscapeLeft;
        public static bool allowedAutorotateToLandscapeRight;
        public static void SetApplicationIdentifier(BuildTargetGroup group, string identifier) { }
        public static void SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget buildTarget, string identifier) { }
        public static void SetScriptingBackend(BuildTargetGroup group, ScriptingImplementation implementation) { }
        public static void SetScriptingBackend(UnityEditor.Build.NamedBuildTarget buildTarget, ScriptingImplementation implementation) { }
        public static void SetApiCompatibilityLevel(BuildTargetGroup group, ApiCompatibilityLevel level) { }
    }

    public static class EditorUserBuildSettings
    {
        public static bool SwitchActiveBuildTarget(BuildTargetGroup group, BuildTarget target) => true;
    }
}

namespace UnityEditor.Build
{
    public struct NamedBuildTarget
    {
        public static NamedBuildTarget iOS => new NamedBuildTarget();
    }

    public class BuildFailedException : System.Exception
    {
        public BuildFailedException(string message) : base(message) { }
    }
}

namespace UnityEditor.Build.Reporting
{
    public enum BuildResult { Succeeded, Failed }

    public class BuildSummary
    {
        public BuildResult result;
    }

    public class BuildReport
    {
        public BuildSummary summary = new BuildSummary();
    }
}

namespace UnityEditor.Events
{
    public static class UnityEventTools
    {
        public static void AddPersistentListener(UnityEvent unityEvent, UnityAction call) { }
    }
}

namespace UnityEditor.SceneManagement
{
    public enum NewSceneSetup { EmptyScene }
    public enum NewSceneMode { Single }

    public static class EditorSceneManager
    {
        public static Scene NewScene(NewSceneSetup setup, NewSceneMode mode) => new Scene();
        public static bool SaveScene(Scene scene, string dstScenePath) => true;
    }
}
