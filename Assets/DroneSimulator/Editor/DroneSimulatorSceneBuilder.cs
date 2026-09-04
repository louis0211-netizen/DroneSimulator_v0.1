using DroneSimulator.Bootstrap;
using DroneSimulator.Camera;
using DroneSimulator.Config;
using DroneSimulator.Core;
using DroneSimulator.Environment;
using DroneSimulator.HUD;
using DroneSimulator.Input;
using DroneSimulator.Systems;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DroneSimulator.Editor
{
    public static class DroneSimulatorSceneBuilder
    {
        private const string PresetPath = "Assets/DroneSimulator/Runtime/Config/DefaultDronePreset.asset";
        private const string ScenePath = "Assets/DroneSimulator/Scenes/MVP_TrainingGround.unity";

        [MenuItem("Drone Simulator/Create MVP Training Scene")]
        public static void CreateMvpTrainingScene()
        {
            DronePreset preset = GetOrCreatePreset();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject systems = new GameObject("Systems");
            systems.AddComponent<IosLandscapeConfigurator>();

            GameObject environment = new GameObject("Training Ground Builder");
            environment.AddComponent<TrainingGroundBuilder>();

            GameObject drone = CreateDrone(preset);
            DroneInputManager inputManager = drone.GetComponent<DroneInputManager>();
            FlightController flightController = drone.GetComponent<FlightController>();
            DronePhysics dronePhysics = drone.GetComponent<DronePhysics>();
            BatterySimulator batterySimulator = drone.GetComponent<BatterySimulator>();

            DroneCameraRig cameraRig = CreateCameraRig(drone.transform);
            CreateHudAndControls(inputManager, flightController, dronePhysics, batterySimulator, cameraRig);

            EditorSceneManager.SaveScene(scene, ScenePath);
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Drone Simulator", "Created MVP Training Scene at:\n" + ScenePath, "OK");
            }
        }

        private static DronePreset GetOrCreatePreset()
        {
            DronePreset preset = AssetDatabase.LoadAssetAtPath<DronePreset>(PresetPath);
            if (preset != null)
            {
                return preset;
            }

            preset = ScriptableObject.CreateInstance<DronePreset>();
            AssetDatabase.CreateAsset(preset, PresetPath);
            AssetDatabase.SaveAssets();
            return preset;
        }

        private static GameObject CreateDrone(DronePreset preset)
        {
            GameObject drone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            drone.name = "Quadcopter";
            drone.transform.position = new Vector3(0f, 0.45f, 0f);
            drone.transform.localScale = new Vector3(0.45f, 0.12f, 0.45f);

            Rigidbody body = drone.AddComponent<Rigidbody>();
            body.mass = preset.massKg;
            body.useGravity = true;
            body.linearDamping = 0f;
            body.angularDamping = 0f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            FlightController flightController = drone.AddComponent<FlightController>();
            DronePhysics dronePhysics = drone.AddComponent<DronePhysics>();
            DroneInputManager inputManager = drone.AddComponent<DroneInputManager>();
            BatterySimulator batterySimulator = drone.AddComponent<BatterySimulator>();

            SetObject(flightController, "preset", preset);
            SetObject(dronePhysics, "preset", preset);
            SetObject(inputManager, "preset", preset);
            SetObject(batterySimulator, "dronePhysics", dronePhysics);
            SetObject(batterySimulator, "flightController", flightController);

            CreateArmVisual("Front Left Motor", drone.transform, new Vector3(-preset.armLengthMeters, 0f, preset.armLengthMeters));
            CreateArmVisual("Front Right Motor", drone.transform, new Vector3(preset.armLengthMeters, 0f, preset.armLengthMeters));
            CreateArmVisual("Rear Right Motor", drone.transform, new Vector3(preset.armLengthMeters, 0f, -preset.armLengthMeters));
            CreateArmVisual("Rear Left Motor", drone.transform, new Vector3(-preset.armLengthMeters, 0f, -preset.armLengthMeters));

            return drone;
        }

        private static void CreateArmVisual(string name, Transform parent, Vector3 localPosition)
        {
            GameObject motor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            motor.name = name;
            motor.transform.SetParent(parent);
            motor.transform.localPosition = localPosition;
            motor.transform.localRotation = Quaternion.identity;
            motor.transform.localScale = new Vector3(0.18f, 0.035f, 0.18f);
        }

        private static DroneCameraRig CreateCameraRig(Transform drone)
        {
            GameObject rig = new GameObject("Camera Rig");
            DroneCameraRig cameraRig = rig.AddComponent<DroneCameraRig>();

            GameObject fpv = new GameObject("FPV Camera");
            fpv.transform.SetParent(drone);
            fpv.transform.localPosition = new Vector3(0f, 0.08f, 0.24f);
            fpv.transform.localRotation = Quaternion.identity;
            UnityEngine.Camera fpvCamera = fpv.AddComponent<UnityEngine.Camera>();
            fpvCamera.fieldOfView = 82f;

            GameObject chase = new GameObject("Chase Camera");
            chase.transform.position = new Vector3(0f, 2.2f, -5.5f);
            UnityEngine.Camera chaseCamera = chase.AddComponent<UnityEngine.Camera>();
            chaseCamera.fieldOfView = 68f;
            chaseCamera.enabled = false;

            SetObject(cameraRig, "fpvCamera", fpvCamera);
            SetObject(cameraRig, "chaseCamera", chaseCamera);
            SetObject(cameraRig, "target", drone);

            return cameraRig;
        }

        private static void CreateHudAndControls(
            DroneInputManager inputManager,
            FlightController flightController,
            DronePhysics dronePhysics,
            BatterySimulator batterySimulator,
            DroneCameraRig cameraRig)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            GameObject canvasObject = new GameObject("Landscape Touch HUD");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(2532f, 1170f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject safeArea = CreateUiRect(
                canvasObject.transform,
                "Safe Area",
                Vector2.zero,
                Vector2.zero,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            RectTransform safeAreaRect = safeArea.GetComponent<RectTransform>();
            safeAreaRect.anchorMin = Vector2.zero;
            safeAreaRect.anchorMax = Vector2.one;
            safeAreaRect.offsetMin = Vector2.zero;
            safeAreaRect.offsetMax = Vector2.zero;
            safeArea.AddComponent<SafeAreaFitter>();

            VirtualJoystick leftJoystick = CreateJoystick(
                safeArea.transform,
                "Left Stick Throttle/Yaw",
                new Vector2(220f, 220f),
                new Vector2(0f, 0f),
                false);
            VirtualJoystick rightJoystick = CreateJoystick(
                safeArea.transform,
                "Right Stick Pitch/Roll",
                new Vector2(-220f, 220f),
                new Vector2(1f, 0f),
                true);
            SetObject(inputManager, "leftJoystick", leftJoystick);
            SetObject(inputManager, "rightJoystick", rightJoystick);

            DroneHudController hud = CreateHud(safeArea.transform, dronePhysics, flightController, batterySimulator);

            Button armButton = CreateButton(safeArea.transform, "ARM", new Vector2(0f, 118f));
            Button resetButton = CreateButton(safeArea.transform, "RESET", new Vector2(0f, 48f));
            Button cameraButton = CreateButton(safeArea.transform, "CAMERA", new Vector2(0f, -22f));
            Button modeButton = CreateButton(safeArea.transform, "MODE", new Vector2(0f, -92f));

            UnityEventTools.AddPersistentListener(armButton.onClick, inputManager.QueueArmToggle);
            UnityEventTools.AddPersistentListener(resetButton.onClick, inputManager.QueueReset);
            UnityEventTools.AddPersistentListener(cameraButton.onClick, inputManager.QueueCameraToggle);
            UnityEventTools.AddPersistentListener(modeButton.onClick, inputManager.QueueFlightModeToggle);
            UnityEventTools.AddPersistentListener(inputManager.ResetRequested, hud.ResetBattery);
            UnityEventTools.AddPersistentListener(inputManager.CameraToggleRequested, cameraRig.ToggleCamera);
        }

        private static VirtualJoystick CreateJoystick(Transform parent, string name, Vector2 anchoredPosition, Vector2 anchor, bool springY)
        {
            GameObject root = CreateUiRect(parent, name, anchoredPosition, new Vector2(260f, 260f), anchor, new Vector2(0.5f, 0.5f));
            Image baseImage = root.AddComponent<Image>();
            baseImage.color = new Color(1f, 1f, 1f, 0.16f);

            GameObject handle = CreateUiRect(root.transform, "Handle", Vector2.zero, new Vector2(92f, 92f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.42f);

            VirtualJoystick joystick = root.AddComponent<VirtualJoystick>();
            SetObject(joystick, "handle", handle.GetComponent<RectTransform>());
            SetBool(joystick, "springToCenterY", springY);
            return joystick;
        }

        private static DroneHudController CreateHud(
            Transform parent,
            DronePhysics dronePhysics,
            FlightController flightController,
            BatterySimulator batterySimulator)
        {
            GameObject hudObject = CreateUiRect(parent, "HUD", new Vector2(32f, -32f), new Vector2(520f, 220f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            DroneHudController hud = hudObject.AddComponent<DroneHudController>();

            Text altitude = CreateText(hudObject.transform, "Altitude", new Vector2(0f, 0f));
            Text speed = CreateText(hudObject.transform, "Speed", new Vector2(0f, -36f));
            Text time = CreateText(hudObject.transform, "Flight Time", new Vector2(0f, -72f));
            Text mode = CreateText(hudObject.transform, "Flight Mode", new Vector2(0f, -108f));
            Text battery = CreateText(hudObject.transform, "Battery", new Vector2(0f, -144f));

            SetObject(hud, "dronePhysics", dronePhysics);
            SetObject(hud, "flightController", flightController);
            SetObject(hud, "batterySimulator", batterySimulator);
            SetObject(hud, "altitudeText", altitude);
            SetObject(hud, "speedText", speed);
            SetObject(hud, "flightTimeText", time);
            SetObject(hud, "flightModeText", mode);
            SetObject(hud, "batteryText", battery);

            return hud;
        }

        private static Button CreateButton(Transform parent, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObject = CreateUiRect(parent, label + " Button", anchoredPosition, new Vector2(180f, 56f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.08f, 0.09f, 0.1f, 0.82f);
            Button button = buttonObject.AddComponent<Button>();

            Text text = CreateText(buttonObject.transform, label + " Label", Vector2.zero);
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;

            return button;
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition)
        {
            GameObject textObject = CreateUiRect(parent, name, anchoredPosition, new Vector2(500f, 32f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.raycastTarget = false;
            text.text = name;
            return text;
        }

        private static GameObject CreateUiRect(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 anchor,
            Vector2 pivot)
        {
            GameObject target = new GameObject(name, typeof(RectTransform));
            RectTransform rect = target.GetComponent<RectTransform>();
            rect.SetParent(parent);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            return target;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
