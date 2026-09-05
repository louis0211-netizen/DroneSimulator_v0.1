using DroneSimulator.Camera;
using DroneSimulator.Core;
using DroneSimulator.Environment;
using DroneSimulator.Input;
using DroneSimulator.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DroneSimulator.HUD
{
    public static class MobileHudRuntimeBuilder
    {
        private const float ReferenceWidth = 2532f;
        private const float ReferenceHeight = 1170f;
        private static bool built;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureAfterSceneLoad()
        {
            Ensure(Object.FindFirstObjectByType<FlightTrainingSession>());
        }

        public static void Ensure(FlightTrainingSession trainingSession)
        {
            if (built)
            {
                return;
            }

            DronePhysics dronePhysics = Object.FindFirstObjectByType<DronePhysics>();
            FlightController flightController = dronePhysics != null ? dronePhysics.GetComponent<FlightController>() : Object.FindFirstObjectByType<FlightController>();
            DroneInputManager inputManager = dronePhysics != null ? dronePhysics.GetComponent<DroneInputManager>() : Object.FindFirstObjectByType<DroneInputManager>();
            BatterySimulator batterySimulator = dronePhysics != null ? dronePhysics.GetComponent<BatterySimulator>() : Object.FindFirstObjectByType<BatterySimulator>();
            DroneCameraRig cameraRig = Object.FindFirstObjectByType<DroneCameraRig>();
            TrainingGroundBuilder groundBuilder = Object.FindFirstObjectByType<TrainingGroundBuilder>();

            DisableExistingCanvases();
            EnsureEventSystem();

            GameObject canvasObject = new GameObject("Runtime Mobile Flight UI", typeof(RectTransform));
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            RectTransform canvasRect = (RectTransform)canvasObject.transform;
            Stretch(canvasRect);

            RectTransform safeRoot = CreateRect("Safe Area", canvasRect);
            safeRoot.gameObject.AddComponent<SafeAreaFitter>();
            Stretch(safeRoot);

            AddVignette(safeRoot);

            Text altitude = CreateHudLine(safeRoot, "ALT --", new Vector2(38f, -34f));
            Text speed = CreateHudLine(safeRoot, "SPD --", new Vector2(38f, -74f));
            Text time = CreateHudLine(safeRoot, "TIME --", new Vector2(38f, -114f));
            Text mode = CreateHudLine(safeRoot, "MODE --", new Vector2(38f, -154f));
            Text battery = CreateHudLine(safeRoot, "BAT --", new Vector2(38f, -194f));

            DroneHudController hud = canvasObject.AddComponent<DroneHudController>();
            hud.Configure(dronePhysics, flightController, batterySimulator, altitude, speed, time, mode, battery);

            Text objective = CreateStatusText(safeRoot, "OBJECTIVE  Ready", new Vector2(0f, -42f), 32, new Color(0.86f, 0.94f, 1f, 0.95f));
            Text warning = CreateStatusText(safeRoot, string.Empty, new Vector2(0f, -88f), 32, new Color(1f, 0.42f, 0.23f, 0.98f));

            JoystickParts leftJoystick = CreateJoystick(safeRoot, "Throttle / Yaw", new Vector2(292f, 250f), true, false);
            JoystickParts rightJoystick = CreateJoystick(safeRoot, "Pitch / Roll", new Vector2(-292f, 250f), false, true);
            if (inputManager != null)
            {
                inputManager.ConfigureVirtualJoysticks(leftJoystick.Joystick, rightJoystick.Joystick);
            }

            Text armLabel = CreateActionButton(safeRoot, new Vector2(-246f, 78f), UiIconType.Power, "ARM", () =>
            {
                if (inputManager != null)
                {
                    inputManager.QueueArmToggle();
                }
            });

            Text resetLabel = CreateActionButton(safeRoot, new Vector2(-82f, 78f), UiIconType.Reset, "RESET", () =>
            {
                if (inputManager != null)
                {
                    inputManager.QueueReset();
                }

                if (trainingSession != null)
                {
                    trainingSession.RestartTrainingAndDrone();
                }
            });

            Text cameraLabel = CreateActionButton(safeRoot, new Vector2(82f, 78f), UiIconType.Camera, "CAM", () =>
            {
                if (inputManager != null)
                {
                    inputManager.QueueCameraToggle();
                }

                if (cameraRig != null)
                {
                    cameraRig.ToggleCamera();
                }
            });

            Text modeLabel = CreateActionButton(safeRoot, new Vector2(246f, 78f), UiIconType.FlightMode, "MODE", () =>
            {
                if (inputManager != null)
                {
                    inputManager.QueueFlightModeToggle();
                }
            });

            EnvironmentThemeSelector selector = canvasObject.AddComponent<EnvironmentThemeSelector>();
            Text currentTheme = CreateThemeTitle(safeRoot);
            Text cityLabel = CreateThemeButton(safeRoot, new Vector2(-258f, 174f), UiIconType.City, "CITY", selector.SelectCity);
            Text forestLabel = CreateThemeButton(safeRoot, new Vector2(-86f, 174f), UiIconType.Forest, "FOREST", selector.SelectForest);
            Text mountainLabel = CreateThemeButton(safeRoot, new Vector2(86f, 174f), UiIconType.Mountain, "MTN", selector.SelectMountain);
            Text beachLabel = CreateThemeButton(safeRoot, new Vector2(258f, 174f), UiIconType.Beach, "BEACH", selector.SelectBeach);
            selector.Configure(groundBuilder, trainingSession, currentTheme, cityLabel, forestLabel, mountainLabel, beachLabel);

            if (trainingSession != null)
            {
                trainingSession.ConfigureHudControls(objective, warning, armLabel, resetLabel, cameraLabel, modeLabel);
            }

            built = true;
        }

        private static void DisableExistingCanvases()
        {
            foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                canvas.gameObject.SetActive(false);
            }
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        private static void AddVignette(RectTransform parent)
        {
            RectTransform vignette = CreateRect("Cockpit Screen Tint", parent);
            Stretch(vignette);
            Image image = vignette.gameObject.AddComponent<Image>();
            image.color = new Color(0.02f, 0.03f, 0.035f, 0.16f);
            image.raycastTarget = false;
        }

        private static Text CreateHudLine(RectTransform parent, string value, Vector2 offset)
        {
            Text text = CreateText("HUD " + value, parent, value, 31, new Color(0.92f, 0.96f, 0.98f, 0.95f), TextAnchor.MiddleLeft);
            RectTransform rect = (RectTransform)text.transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = new Vector2(360f, 38f);
            return text;
        }

        private static Text CreateStatusText(RectTransform parent, string value, Vector2 offset, int size, Color color)
        {
            Text text = CreateText("Status Text", parent, value, size, color, TextAnchor.MiddleCenter);
            RectTransform rect = (RectTransform)text.transform;
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = new Vector2(980f, 42f);
            return text;
        }

        private static Text CreateThemeTitle(RectTransform parent)
        {
            Text text = CreateText("Current Environment", parent, "ENV CITY", 20, new Color(0.78f, 0.9f, 1f, 0.9f), TextAnchor.MiddleCenter);
            RectTransform rect = (RectTransform)text.transform;
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 240f);
            rect.sizeDelta = new Vector2(360f, 30f);
            return text;
        }

        private static Text CreateActionButton(RectTransform parent, Vector2 position, UiIconType iconType, string label, UnityEngine.Events.UnityAction action)
        {
            RectTransform rect = CreateButtonRoot("Action " + label, parent, position, new Vector2(126f, 86f));
            AddIcon(rect, iconType, new Vector2(0f, 13f), new Vector2(38f, 38f), new Color(0.88f, 0.95f, 1f, 0.98f));
            Text text = CreateText(label + " Label", rect, label, 18, new Color(0.88f, 0.95f, 1f, 0.95f), TextAnchor.MiddleCenter);
            RectTransform labelRect = (RectTransform)text.transform;
            labelRect.anchorMin = new Vector2(0.5f, 0f);
            labelRect.anchorMax = new Vector2(0.5f, 0f);
            labelRect.pivot = new Vector2(0.5f, 0f);
            labelRect.anchoredPosition = new Vector2(0f, 9f);
            labelRect.sizeDelta = new Vector2(112f, 22f);
            rect.gameObject.GetComponent<Button>().onClick.AddListener(action);
            return text;
        }

        private static Text CreateThemeButton(RectTransform parent, Vector2 position, UiIconType iconType, string label, UnityEngine.Events.UnityAction action)
        {
            RectTransform rect = CreateButtonRoot("Theme " + label, parent, position, new Vector2(138f, 72f));
            AddIcon(rect, iconType, new Vector2(-34f, 1f), new Vector2(34f, 34f), new Color(0.56f, 0.92f, 0.78f, 0.98f));
            Text text = CreateText(label + " Label", rect, label, 17, new Color(0.9f, 0.96f, 0.94f, 0.95f), TextAnchor.MiddleCenter);
            RectTransform labelRect = (RectTransform)text.transform;
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = new Vector2(24f, 0f);
            labelRect.sizeDelta = new Vector2(76f, 24f);
            rect.gameObject.GetComponent<Button>().onClick.AddListener(action);
            return text;
        }

        private static RectTransform CreateButtonRoot(string name, RectTransform parent, Vector2 position, Vector2 size)
        {
            RectTransform rect = CreateRect(name, parent);
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.015f, 0.022f, 0.028f, 0.76f);

            rect.gameObject.AddComponent<Button>();
            return rect;
        }

        private static JoystickParts CreateJoystick(RectTransform parent, string label, Vector2 position, bool leftSide, bool throttleStick)
        {
            RectTransform root = CreateRect(label + " Joystick", parent);
            root.anchorMin = new Vector2(leftSide ? 0f : 1f, 0f);
            root.anchorMax = new Vector2(leftSide ? 0f : 1f, 0f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = position;
            root.sizeDelta = new Vector2(330f, 330f);

            UiRingGraphic outer = root.gameObject.AddComponent<UiRingGraphic>();
            outer.InnerRadius = 0.72f;
            outer.color = new Color(0.78f, 0.88f, 0.96f, 0.28f);
            outer.raycastTarget = true;

            RectTransform inner = CreateRect(label + " Inner Ring", root);
            Center(inner, new Vector2(236f, 236f));
            UiRingGraphic innerRing = inner.gameObject.AddComponent<UiRingGraphic>();
            innerRing.InnerRadius = 0.82f;
            innerRing.color = new Color(0.1f, 0.14f, 0.18f, 0.52f);
            innerRing.raycastTarget = false;

            RectTransform handle = CreateRect(label + " Handle", root);
            Center(handle, new Vector2(96f, 96f));
            UiRingGraphic handleDisc = handle.gameObject.AddComponent<UiRingGraphic>();
            handleDisc.InnerRadius = 0f;
            handleDisc.color = new Color(0.82f, 0.88f, 0.92f, 0.86f);
            handleDisc.raycastTarget = false;

            AddGripBar(handle, new Vector2(-20f, 10f));
            AddGripBar(handle, new Vector2(0f, 15f));
            AddGripBar(handle, new Vector2(20f, 10f));

            Text text = CreateText(label + " Label", root, label.ToUpperInvariant(), 16, new Color(0.85f, 0.94f, 1f, 0.76f), TextAnchor.MiddleCenter);
            RectTransform labelRect = (RectTransform)text.transform;
            labelRect.anchorMin = new Vector2(0.5f, 0f);
            labelRect.anchorMax = new Vector2(0.5f, 0f);
            labelRect.pivot = new Vector2(0.5f, 0f);
            labelRect.anchoredPosition = new Vector2(0f, -34f);
            labelRect.sizeDelta = new Vector2(280f, 24f);

            VirtualJoystick joystick = root.gameObject.AddComponent<VirtualJoystick>();
            joystick.Configure(handle, 112f, true, !throttleStick);
            return new JoystickParts(joystick);
        }

        private static void AddGripBar(RectTransform parent, Vector2 position)
        {
            RectTransform rect = CreateRect("Grip Bar", parent);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(8f, 38f);
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.08f, 0.1f, 0.12f, 0.5f);
            image.raycastTarget = false;
        }

        private static void AddIcon(RectTransform parent, UiIconType iconType, Vector2 position, Vector2 size, Color color)
        {
            RectTransform rect = CreateRect(iconType + " Icon", parent);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            UiIconGraphic icon = rect.gameObject.AddComponent<UiIconGraphic>();
            icon.IconType = iconType;
            icon.color = color;
            icon.raycastTarget = false;
        }

        private static Text CreateText(string name, RectTransform parent, string value, int fontSize, Color color, TextAnchor alignment)
        {
            RectTransform rect = CreateRect(name, parent);
            Text text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            return text;
        }

        private static RectTransform CreateRect(string name, RectTransform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            RectTransform rectTransform = (RectTransform)gameObject.transform;
            rectTransform.SetParent(parent, false);
            return rectTransform;
        }

        private static void Center(RectTransform rect, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private readonly struct JoystickParts
        {
            public JoystickParts(VirtualJoystick joystick)
            {
                Joystick = joystick;
            }

            public VirtualJoystick Joystick { get; }
        }
    }
}
