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
            FlightTrainingSession trainingSession = systems.AddComponent<FlightTrainingSession>();

            GameObject environment = new GameObject("Training Ground Builder");
            TrainingGroundBuilder trainingGroundBuilder = environment.AddComponent<TrainingGroundBuilder>();

            GameObject drone = CreateDrone(preset);
            DroneInputManager inputManager = drone.GetComponent<DroneInputManager>();
            FlightController flightController = drone.GetComponent<FlightController>();
            DronePhysics dronePhysics = drone.GetComponent<DronePhysics>();
            BatterySimulator batterySimulator = drone.GetComponent<BatterySimulator>();

            DroneCameraRig cameraRig = CreateCameraRig(drone.transform);
            CreateHudAndControls(inputManager, flightController, dronePhysics, batterySimulator, cameraRig, trainingSession, trainingGroundBuilder);

            EditorSceneManager.SaveScene(scene, ScenePath);
            IosBuildConfigurator.EnsureSceneIsInBuildSettings();
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
            DroneCameraRig cameraRig,
            FlightTrainingSession trainingSession,
            TrainingGroundBuilder trainingGroundBuilder)
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
                new Vector2(240f, 230f),
                new Vector2(0f, 0f),
                false);
            VirtualJoystick rightJoystick = CreateJoystick(
                safeArea.transform,
                "Right Stick Pitch/Roll",
                new Vector2(-240f, 230f),
                new Vector2(1f, 0f),
                true);
            SetObject(inputManager, "leftJoystick", leftJoystick);
            SetObject(inputManager, "rightJoystick", rightJoystick);

            DroneHudController hud = CreateHud(safeArea.transform, dronePhysics, flightController, batterySimulator);
            Text objectiveText = CreateTopCenterText(safeArea.transform, "Objective", new Vector2(0f, -44f), 30);
            Text warningText = CreateTopCenterText(safeArea.transform, "Warning", new Vector2(0f, -86f), 26);
            warningText.color = new Color(1f, 0.42f, 0.26f, 1f);

            CreateControlRail(safeArea.transform);
            UiButton armButton = CreateButton(safeArea.transform, "ARM", new Vector2(-168f, 240f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Color(0.11f, 0.56f, 0.64f, 0.86f));
            UiButton resetButton = CreateButton(safeArea.transform, "RESET", new Vector2(-168f, 170f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Color(0.16f, 0.18f, 0.2f, 0.86f));
            UiButton cameraButton = CreateButton(safeArea.transform, "FPV", new Vector2(-168f, 100f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Color(0.16f, 0.18f, 0.2f, 0.86f));
            UiButton modeButton = CreateButton(safeArea.transform, "STABILIZED", new Vector2(-168f, 30f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Color(0.16f, 0.18f, 0.2f, 0.86f));
            CreateEnvironmentSelector(safeArea.transform, trainingGroundBuilder, trainingSession);

            UnityEventTools.AddPersistentListener(armButton.button.onClick, inputManager.QueueArmToggle);
            UnityEventTools.AddPersistentListener(resetButton.button.onClick, inputManager.QueueReset);
            UnityEventTools.AddPersistentListener(cameraButton.button.onClick, inputManager.QueueCameraToggle);
            UnityEventTools.AddPersistentListener(modeButton.button.onClick, inputManager.QueueFlightModeToggle);
            UnityEventTools.AddPersistentListener(inputManager.ResetRequested, hud.ResetBattery);
            UnityEventTools.AddPersistentListener(inputManager.CameraToggleRequested, cameraRig.ToggleCamera);
            UnityEventTools.AddPersistentListener(inputManager.ResetRequested, trainingSession.RestartTraining);

            SetObject(trainingSession, "dronePhysics", dronePhysics);
            SetObject(trainingSession, "flightController", flightController);
            SetObject(trainingSession, "inputManager", inputManager);
            SetObject(trainingSession, "batterySimulator", batterySimulator);
            SetObject(trainingSession, "cameraRig", cameraRig);
            SetObject(trainingSession, "objectiveText", objectiveText);
            SetObject(trainingSession, "warningText", warningText);
            SetObject(trainingSession, "armButtonText", armButton.label);
            SetObject(trainingSession, "resetButtonText", resetButton.label);
            SetObject(trainingSession, "cameraButtonText", cameraButton.label);
            SetObject(trainingSession, "modeButtonText", modeButton.label);
        }

        private static VirtualJoystick CreateJoystick(Transform parent, string name, Vector2 anchoredPosition, Vector2 anchor, bool springY)
        {
            GameObject root = CreateUiRect(parent, name, anchoredPosition, new Vector2(300f, 300f), anchor, new Vector2(0.5f, 0.5f));
            UiRingGraphic baseRing = root.AddComponent<UiRingGraphic>();
            baseRing.color = new Color(0.62f, 0.86f, 1f, 0.28f);
            baseRing.InnerRadius = 0.72f;

            GameObject inner = CreateUiRect(root.transform, "Inner Stick Guide", Vector2.zero, new Vector2(176f, 176f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiRingGraphic innerRing = inner.AddComponent<UiRingGraphic>();
            innerRing.color = new Color(1f, 1f, 1f, 0.08f);
            innerRing.InnerRadius = 0.92f;

            GameObject handle = CreateUiRect(root.transform, "Handle", Vector2.zero, new Vector2(96f, 96f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiRingGraphic handleImage = handle.AddComponent<UiRingGraphic>();
            handleImage.color = new Color(0.88f, 0.96f, 1f, 0.52f);
            handleImage.InnerRadius = 0f;

            Text label = CreateCenteredText(root.transform, name.Contains("Throttle") ? "THR / YAW" : "PITCH / ROLL", new Vector2(0f, -184f), new Vector2(260f, 30f), 18);
            label.color = new Color(0.8f, 0.92f, 1f, 0.72f);

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
            Image panel = hudObject.AddComponent<Image>();
            panel.color = new Color(0.03f, 0.07f, 0.09f, 0.52f);
            DroneHudController hud = hudObject.AddComponent<DroneHudController>();

            Text altitude = CreateText(hudObject.transform, "Altitude", new Vector2(22f, -18f));
            Text speed = CreateText(hudObject.transform, "Speed", new Vector2(22f, -54f));
            Text time = CreateText(hudObject.transform, "Flight Time", new Vector2(22f, -90f));
            Text mode = CreateText(hudObject.transform, "Flight Mode", new Vector2(22f, -126f));
            Text battery = CreateText(hudObject.transform, "Battery", new Vector2(22f, -162f));

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

        private static void CreateControlRail(Transform parent)
        {
            GameObject rail = CreateUiRect(parent, "Flight Control Rail", new Vector2(-168f, 135f), new Vector2(250f, 330f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f));
            Image image = rail.AddComponent<Image>();
            image.color = new Color(0.02f, 0.03f, 0.04f, 0.22f);
        }

        private static UiButton CreateButton(Transform parent, string label, Vector2 anchoredPosition, Vector2 anchor, Vector2 pivot, Color color)
        {
            GameObject buttonObject = CreateUiRect(parent, label + " Button", anchoredPosition, new Vector2(210f, 58f), anchor, pivot);
            Image image = buttonObject.AddComponent<Image>();
            image.color = color;
            Button button = buttonObject.AddComponent<Button>();

            Text text = CreateCenteredText(buttonObject.transform, label + " Label", Vector2.zero, new Vector2(210f, 58f), 22);
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;

            return new UiButton(button, text);
        }

        private static void CreateEnvironmentSelector(Transform parent, TrainingGroundBuilder trainingGroundBuilder, FlightTrainingSession trainingSession)
        {
            GameObject panel = CreateUiRect(parent, "Environment Selector", new Vector2(-32f, -32f), new Vector2(600f, 148f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.03f, 0.07f, 0.09f, 0.48f);

            EnvironmentThemeSelector selector = panel.AddComponent<EnvironmentThemeSelector>();
            Text currentTheme = CreateText(panel.transform, "Current Environment", new Vector2(20f, -14f));
            currentTheme.fontSize = 19;
            currentTheme.color = new Color(0.7f, 0.9f, 1f, 0.9f);

            UiButton city = CreateButton(panel.transform, "CITY", new Vector2(20f, -68f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Color(0.11f, 0.16f, 0.19f, 0.8f));
            UiButton forest = CreateButton(panel.transform, "FOREST", new Vector2(164f, -68f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Color(0.1f, 0.22f, 0.13f, 0.8f));
            UiButton mountain = CreateButton(panel.transform, "MOUNTAIN", new Vector2(308f, -68f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Color(0.18f, 0.18f, 0.17f, 0.8f));
            UiButton beach = CreateButton(panel.transform, "BEACH", new Vector2(452f, -68f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Color(0.14f, 0.31f, 0.37f, 0.8f));

            RectTransform cityRect = city.button.GetComponent<RectTransform>();
            RectTransform forestRect = forest.button.GetComponent<RectTransform>();
            RectTransform mountainRect = mountain.button.GetComponent<RectTransform>();
            RectTransform beachRect = beach.button.GetComponent<RectTransform>();
            cityRect.sizeDelta = new Vector2(130f, 48f);
            forestRect.sizeDelta = new Vector2(130f, 48f);
            mountainRect.sizeDelta = new Vector2(130f, 48f);
            beachRect.sizeDelta = new Vector2(130f, 48f);

            city.label.fontSize = 18;
            forest.label.fontSize = 18;
            mountain.label.fontSize = 17;
            beach.label.fontSize = 18;

            UnityEventTools.AddPersistentListener(city.button.onClick, selector.SelectCity);
            UnityEventTools.AddPersistentListener(forest.button.onClick, selector.SelectForest);
            UnityEventTools.AddPersistentListener(mountain.button.onClick, selector.SelectMountain);
            UnityEventTools.AddPersistentListener(beach.button.onClick, selector.SelectBeach);

            SetObject(selector, "trainingGroundBuilder", trainingGroundBuilder);
            SetObject(selector, "trainingSession", trainingSession);
            SetObject(selector, "currentThemeText", currentTheme);
            SetObject(selector, "cityButtonText", city.label);
            SetObject(selector, "forestButtonText", forest.label);
            SetObject(selector, "mountainButtonText", mountain.label);
            SetObject(selector, "beachButtonText", beach.label);
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition)
        {
            GameObject textObject = CreateUiRect(parent, name, anchoredPosition, new Vector2(500f, 32f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.color = new Color(0.88f, 0.96f, 1f, 0.95f);
            text.alignment = TextAnchor.MiddleLeft;
            text.raycastTarget = false;
            text.text = name;
            return text;
        }

        private static Text CreateTopCenterText(Transform parent, string name, Vector2 anchoredPosition, int fontSize)
        {
            GameObject textObject = CreateUiRect(parent, name, anchoredPosition, new Vector2(980f, 44f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.text = name;
            return text;
        }

        private static Text CreateCenteredText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize)
        {
            GameObject textObject = CreateUiRect(parent, name, anchoredPosition, size, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
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

        private readonly struct UiButton
        {
            public readonly Button button;
            public readonly Text label;

            public UiButton(Button button, Text label)
            {
                this.button = button;
                this.label = label;
            }
        }
    }
}
