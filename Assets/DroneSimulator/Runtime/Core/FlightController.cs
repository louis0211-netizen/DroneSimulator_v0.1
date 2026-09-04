using DroneSimulator.Config;
using UnityEngine;

namespace DroneSimulator.Core
{
    [DisallowMultipleComponent]
    public sealed class FlightController : MonoBehaviour
    {
        [SerializeField] private DronePreset preset;
        [SerializeField] private FlightMode flightMode = FlightMode.Stabilized;
        [SerializeField] private bool armed;
        [SerializeField] private float armThrottleLockout = 0.05f;

        private Rigidbody cachedRigidbody;
        private float flightStartTime;
        private AxisPidState rollAngleState;
        private AxisPidState pitchAngleState;
        private AxisPidState rollRateState;
        private AxisPidState pitchRateState;
        private AxisPidState yawRateState;

        public FlightMode CurrentMode => flightMode;
        public bool IsArmed => armed;
        public float FlightTimeSeconds => armed ? Time.time - flightStartTime : 0f;
        public float ArmThrottleLockout => armThrottleLockout;

        private void Awake()
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }

        public void ToggleArm(float currentThrottle)
        {
            if (armed)
            {
                Disarm();
                return;
            }

            if (currentThrottle <= armThrottleLockout)
            {
                Arm();
            }
        }

        public bool CanArmAtThrottle(float currentThrottle)
        {
            return currentThrottle <= armThrottleLockout;
        }

        public void Arm()
        {
            armed = true;
            flightStartTime = Time.time;
            ResetControlState();
        }

        public void Disarm()
        {
            armed = false;
            ResetControlState();
        }

        public void ToggleFlightMode()
        {
            flightMode = flightMode == FlightMode.Stabilized ? FlightMode.Acro : FlightMode.Stabilized;
            ResetControlState();
        }

        public FlightControlCommand BuildCommand(DroneInputState input)
        {
            if (input.armPressed)
            {
                ToggleArm(input.throttle);
            }

            if (input.flightModePressed)
            {
                ToggleFlightMode();
            }

            if (!armed)
            {
                return FlightControlCommand.Disarmed(flightMode);
            }

            return flightMode == FlightMode.Stabilized
                ? BuildStabilizedCommand(input)
                : BuildAcroCommand(input);
        }

        private FlightControlCommand BuildStabilizedCommand(DroneInputState input)
        {
            Vector3 localAngularVelocity = transform.InverseTransformDirection(cachedRigidbody.angularVelocity);
            Vector3 localEuler = NormalizeEuler(transform.localEulerAngles);

            float maxAngle = preset != null ? preset.maxStabilizedAngleDegrees : 28f;
            float maxRate = preset != null ? preset.maxRateDegreesPerSecond : 220f;
            float dt = Time.fixedDeltaTime;
            PidGains anglePid = preset != null ? preset.stabilizedAnglePid : new PidGains(4.5f, 0f, 0.08f, 0f, 4.5f);
            PidGains rollPitchRatePid = preset != null ? preset.rollPitchRatePid : new PidGains(0.08f, 0.01f, 0.001f, 1f, 1f);
            PidGains yawRatePid = preset != null ? preset.yawRatePid : new PidGains(0.09f, 0.008f, 0.0008f, 1f, 1f);

            float targetRoll = -input.roll * maxAngle;
            float targetPitch = input.pitch * maxAngle;

            float targetRollRate = Mathf.Clamp(PidController.Step(targetRoll, localEuler.z, dt, anglePid, ref rollAngleState), -maxRate, maxRate) * Mathf.Deg2Rad;
            float targetPitchRate = Mathf.Clamp(PidController.Step(targetPitch, localEuler.x, dt, anglePid, ref pitchAngleState), -maxRate, maxRate) * Mathf.Deg2Rad;
            float targetYawRate = input.yaw * maxRate * Mathf.Deg2Rad;

            float rollDemand = PidController.Step(targetRollRate, localAngularVelocity.z, dt, rollPitchRatePid, ref rollRateState);
            float pitchDemand = PidController.Step(targetPitchRate, localAngularVelocity.x, dt, rollPitchRatePid, ref pitchRateState);
            float yawDemand = PidController.Step(targetYawRate, localAngularVelocity.y, dt, yawRatePid, ref yawRateState);

            return new FlightControlCommand
            {
                throttle = Mathf.Clamp01(input.throttle),
                roll = Mathf.Clamp(rollDemand, -1f, 1f),
                pitch = Mathf.Clamp(pitchDemand, -1f, 1f),
                yaw = Mathf.Clamp(yawDemand, -1f, 1f),
                armed = true,
                mode = flightMode
            };
        }

        private FlightControlCommand BuildAcroCommand(DroneInputState input)
        {
            Vector3 localAngularVelocity = transform.InverseTransformDirection(cachedRigidbody.angularVelocity);
            float maxRate = preset != null ? preset.maxRateDegreesPerSecond : 220f;
            float dt = Time.fixedDeltaTime;
            PidGains rollPitchRatePid = preset != null ? preset.rollPitchRatePid : new PidGains(0.08f, 0.01f, 0.001f, 1f, 1f);
            PidGains yawRatePid = preset != null ? preset.yawRatePid : new PidGains(0.09f, 0.008f, 0.0008f, 1f, 1f);

            float targetRollRate = -input.roll * maxRate * Mathf.Deg2Rad;
            float targetPitchRate = input.pitch * maxRate * Mathf.Deg2Rad;
            float targetYawRate = input.yaw * maxRate * Mathf.Deg2Rad;

            return new FlightControlCommand
            {
                throttle = Mathf.Clamp01(input.throttle),
                roll = Mathf.Clamp(PidController.Step(targetRollRate, localAngularVelocity.z, dt, rollPitchRatePid, ref rollRateState), -1f, 1f),
                pitch = Mathf.Clamp(PidController.Step(targetPitchRate, localAngularVelocity.x, dt, rollPitchRatePid, ref pitchRateState), -1f, 1f),
                yaw = Mathf.Clamp(PidController.Step(targetYawRate, localAngularVelocity.y, dt, yawRatePid, ref yawRateState), -1f, 1f),
                armed = true,
                mode = flightMode
            };
        }

        private void ResetControlState()
        {
            PidController.Reset(ref rollAngleState);
            PidController.Reset(ref pitchAngleState);
            PidController.Reset(ref rollRateState);
            PidController.Reset(ref pitchRateState);
            PidController.Reset(ref yawRateState);
        }

        private static Vector3 NormalizeEuler(Vector3 euler)
        {
            return new Vector3(NormalizeAngle(euler.x), NormalizeAngle(euler.y), NormalizeAngle(euler.z));
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
