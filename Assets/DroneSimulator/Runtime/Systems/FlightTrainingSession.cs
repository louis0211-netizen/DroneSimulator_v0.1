using DroneSimulator.Camera;
using DroneSimulator.Core;
using DroneSimulator.Input;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSimulator.Systems
{
    [DisallowMultipleComponent]
    public sealed class FlightTrainingSession : MonoBehaviour
    {
        [SerializeField] private DronePhysics dronePhysics;
        [SerializeField] private FlightController flightController;
        [SerializeField] private DroneInputManager inputManager;
        [SerializeField] private BatterySimulator batterySimulator;
        [SerializeField] private DroneCameraRig cameraRig;

        [Header("HUD")]
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text warningText;
        [SerializeField] private Text armButtonText;
        [SerializeField] private Text resetButtonText;
        [SerializeField] private Text cameraButtonText;
        [SerializeField] private Text modeButtonText;

        [Header("Milestones")]
        [SerializeField] private float takeoffAltitudeMeters = 1.0f;
        [SerializeField] private float hoverHoldSeconds = 1.4f;
        [SerializeField] private float hoverMaxSpeed = 1.2f;
        [SerializeField] private float commandHoldSeconds = 0.55f;
        [SerializeField] private float landedAltitudeMeters = 0.18f;
        [SerializeField] private float landedMaxSpeed = 0.8f;

        private TrainingStep currentStep;
        private float stepTimer;

        private enum TrainingStep
        {
            Arm,
            Takeoff,
            Hover,
            Forward,
            Backward,
            Strafe,
            Yaw,
            Land,
            CrashOrReset,
            Complete
        }

        private void Awake()
        {
            ResolveReferences();
            currentStep = TrainingStep.Arm;
        }

        private void Update()
        {
            ResolveReferences();
            UpdateTrainingStep();
            UpdateHud();
        }

        public void RestartTraining()
        {
            currentStep = TrainingStep.Arm;
            stepTimer = 0f;
        }

        private void ResolveReferences()
        {
            if (dronePhysics == null)
            {
                dronePhysics = Object.FindFirstObjectByType<DronePhysics>();
            }

            if (flightController == null && dronePhysics != null)
            {
                flightController = dronePhysics.GetComponent<FlightController>();
            }

            if (inputManager == null && dronePhysics != null)
            {
                inputManager = dronePhysics.GetComponent<DroneInputManager>();
            }

            if (batterySimulator == null && dronePhysics != null)
            {
                batterySimulator = dronePhysics.GetComponent<BatterySimulator>();
            }

            if (cameraRig == null)
            {
                cameraRig = Object.FindFirstObjectByType<DroneCameraRig>();
            }
        }

        private void UpdateTrainingStep()
        {
            if (dronePhysics == null || flightController == null)
            {
                return;
            }

            if (dronePhysics.HasCrashed)
            {
                currentStep = TrainingStep.CrashOrReset;
                stepTimer = 0f;
                return;
            }

            DroneInputState input = inputManager != null ? inputManager.LastInputState : DroneInputState.Neutral;

            switch (currentStep)
            {
                case TrainingStep.Arm:
                    if (flightController.IsArmed)
                    {
                        AdvanceTo(TrainingStep.Takeoff);
                    }
                    break;

                case TrainingStep.Takeoff:
                    if (dronePhysics.AltitudeMeters >= takeoffAltitudeMeters)
                    {
                        AdvanceTo(TrainingStep.Hover);
                    }
                    break;

                case TrainingStep.Hover:
                    if (dronePhysics.AltitudeMeters >= takeoffAltitudeMeters &&
                        dronePhysics.SpeedMetersPerSecond <= hoverMaxSpeed)
                    {
                        HoldThenAdvance(TrainingStep.Forward, hoverHoldSeconds);
                    }
                    else
                    {
                        stepTimer = 0f;
                    }
                    break;

                case TrainingStep.Forward:
                    RequireStickHold(input.pitch > 0.35f, TrainingStep.Backward);
                    break;

                case TrainingStep.Backward:
                    RequireStickHold(input.pitch < -0.35f, TrainingStep.Strafe);
                    break;

                case TrainingStep.Strafe:
                    RequireStickHold(Mathf.Abs(input.roll) > 0.35f, TrainingStep.Yaw);
                    break;

                case TrainingStep.Yaw:
                    RequireStickHold(Mathf.Abs(input.yaw) > 0.35f, TrainingStep.Land);
                    break;

                case TrainingStep.Land:
                    if (dronePhysics.AltitudeMeters <= landedAltitudeMeters &&
                        dronePhysics.SpeedMetersPerSecond <= landedMaxSpeed)
                    {
                        AdvanceTo(TrainingStep.Complete);
                    }
                    break;

                case TrainingStep.CrashOrReset:
                    if (!dronePhysics.HasCrashed && !flightController.IsArmed)
                    {
                        AdvanceTo(TrainingStep.Arm);
                    }
                    break;
            }
        }

        private void RequireStickHold(bool condition, TrainingStep nextStep)
        {
            if (condition)
            {
                HoldThenAdvance(nextStep, commandHoldSeconds);
            }
            else
            {
                stepTimer = 0f;
            }
        }

        private void HoldThenAdvance(TrainingStep nextStep, float holdSeconds)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= holdSeconds)
            {
                AdvanceTo(nextStep);
            }
        }

        private void AdvanceTo(TrainingStep nextStep)
        {
            currentStep = nextStep;
            stepTimer = 0f;
        }

        private void UpdateHud()
        {
            SetText(objectiveText, BuildObjectiveText());
            SetText(warningText, BuildWarningText());
            SetText(armButtonText, flightController != null && flightController.IsArmed ? "DISARM" : "ARM");
            SetText(resetButtonText, "RESET");
            SetText(cameraButtonText, cameraRig != null && cameraRig.IsFpvCameraActive ? "FPV" : "CHASE");
            SetText(modeButtonText, flightController != null ? flightController.CurrentMode.ToString().ToUpperInvariant() : "MODE");
        }

        private string BuildObjectiveText()
        {
            switch (currentStep)
            {
                case TrainingStep.Arm:
                    return "OBJECTIVE  ARM with throttle low";
                case TrainingStep.Takeoff:
                    return "OBJECTIVE  Raise throttle and take off";
                case TrainingStep.Hover:
                    return "OBJECTIVE  Hold a stable hover";
                case TrainingStep.Forward:
                    return "OBJECTIVE  Pitch forward";
                case TrainingStep.Backward:
                    return "OBJECTIVE  Pitch backward";
                case TrainingStep.Strafe:
                    return "OBJECTIVE  Roll left or right";
                case TrainingStep.Yaw:
                    return "OBJECTIVE  Yaw left or right";
                case TrainingStep.Land:
                    return "OBJECTIVE  Land gently";
                case TrainingStep.CrashOrReset:
                    return "OBJECTIVE  Crash detected. Tap RESET";
                case TrainingStep.Complete:
                    return "OBJECTIVE  Training loop complete";
                default:
                    return "OBJECTIVE  Ready";
            }
        }

        private string BuildWarningText()
        {
            if (dronePhysics != null && dronePhysics.HasCrashed)
            {
                return "CRASH  Motors disarmed";
            }

            if (batterySimulator != null && batterySimulator.IsEmpty)
            {
                return "BATTERY EMPTY  Motors disarmed";
            }

            if (batterySimulator != null && batterySimulator.Percent <= 20f)
            {
                return "LOW BATTERY";
            }

            if (flightController != null &&
                inputManager != null &&
                !flightController.IsArmed &&
                !flightController.CanArmAtThrottle(inputManager.LastInputState.throttle))
            {
                return "LOWER THROTTLE BEFORE ARM";
            }

            return string.Empty;
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
            {
                target.text = value;
            }
        }
    }
}
