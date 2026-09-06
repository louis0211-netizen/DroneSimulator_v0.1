using DroneSimulator.Core;
using DroneSimulator.Config;
using UnityEngine;
using UnityEngine.Events;

namespace DroneSimulator.Input
{
    [DisallowMultipleComponent]
    public sealed class DroneInputManager : MonoBehaviour, IFlightInputSource
    {
        [SerializeField] private DronePreset preset;

        [Header("Virtual Joysticks")]
        [SerializeField] private VirtualJoystick leftJoystick;
        [SerializeField] private VirtualJoystick rightJoystick;

        [Header("Keyboard Debug")]
        [SerializeField] private bool enableKeyboardInEditor = true;
        [SerializeField] private float keyboardThrottleSpeed = 0.55f;

        [Header("Mobile Motion Sensors")]
        [SerializeField] private MotionSensorFlightInput motionSensors = new MotionSensorFlightInput();
        [SerializeField, Range(0f, 1f)] private float motionSensorBlend = 0.82f;

        [Header("Action Events")]
        [SerializeField] private UnityEvent resetRequested = new UnityEvent();
        [SerializeField] private UnityEvent cameraToggleRequested = new UnityEvent();

        private DroneInputState state;
        private bool armQueued;
        private bool resetQueued;
        private bool cameraQueued;
        private bool flightModeQueued;

        public DroneInputState LastInputState => state;
        public bool MotionControlsEnabled => motionSensors != null && motionSensors.IsEnabled;
        public string MotionControlsStatus => motionSensors != null ? motionSensors.StatusText : "TILT OFF";
        public UnityEvent ResetRequested => resetRequested;
        public UnityEvent CameraToggleRequested => cameraToggleRequested;

        private void Awake()
        {
            if (motionSensors == null)
            {
                motionSensors = new MotionSensorFlightInput();
            }

            motionSensors.Initialize();
        }

        public void ConfigureVirtualJoysticks(VirtualJoystick newLeftJoystick, VirtualJoystick newRightJoystick)
        {
            leftJoystick = newLeftJoystick;
            rightJoystick = newRightJoystick;
        }

        public DroneInputState ReadInput()
        {
            UpdateContinuousInput();

            state.armPressed = Consume(ref armQueued);
            state.resetPressed = Consume(ref resetQueued);
            state.cameraPressed = Consume(ref cameraQueued);
            state.flightModePressed = Consume(ref flightModeQueued);

            return state;
        }

        public void QueueArmToggle()
        {
            armQueued = true;
        }

        public void QueueReset()
        {
            resetQueued = true;
            resetRequested.Invoke();
        }

        public void QueueCameraToggle()
        {
            cameraQueued = true;
            cameraToggleRequested.Invoke();
        }

        public void QueueFlightModeToggle()
        {
            flightModeQueued = true;
        }

        public void ToggleMotionControls()
        {
            EnsureMotionSensors();
            motionSensors.ToggleEnabled();
        }

        public void CalibrateMotionNeutral()
        {
            EnsureMotionSensors();
            motionSensors.CalibrateNeutral();
        }

        private void UpdateContinuousInput()
        {
            Vector2 left = leftJoystick != null ? leftJoystick.Value : Vector2.zero;
            Vector2 right = rightJoystick != null ? rightJoystick.Value : Vector2.zero;
            Vector2 motion = Vector2.zero;

            EnsureMotionSensors();
            if (motionSensors != null)
            {
                motion = motionSensors.ReadRollPitch();
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableKeyboardInEditor)
            {
                ReadKeyboardDebug(ref left, ref right);
            }
#endif

            float deadZone = preset != null ? preset.stickDeadZone : 0.04f;
            float stickExpo = preset != null ? preset.stickExpo : 0.25f;
            float throttleExpo = preset != null ? preset.throttleExpo : 0.15f;

            state.throttle = ApplyThrottleCurve(left.y, throttleExpo);
            state.yaw = ApplyExpo(ApplyDeadZone(left.x, deadZone), stickExpo);
            state.pitch = ApplyExpo(ApplyDeadZone(BlendStickAndMotion(right.y, motion.y), deadZone), stickExpo);
            state.roll = ApplyExpo(ApplyDeadZone(BlendStickAndMotion(right.x, motion.x), deadZone), stickExpo);
        }

        private void ReadKeyboardDebug(ref Vector2 left, ref Vector2 right)
        {
            if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                left.y += keyboardThrottleSpeed * Time.deltaTime;
            }

            if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                left.y -= keyboardThrottleSpeed * Time.deltaTime;
            }

            left.y = Mathf.Clamp01(state.throttle + left.y);
            left.x += UnityEngine.Input.GetAxisRaw("Horizontal");
            right.y += UnityEngine.Input.GetAxisRaw("Vertical");

            if (UnityEngine.Input.GetKey(KeyCode.A))
            {
                right.x -= 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.D))
            {
                right.x += 1f;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                armQueued = true;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                resetQueued = true;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.C))
            {
                cameraQueued = true;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                flightModeQueued = true;
            }
        }

        private static float ApplyDeadZone(float value, float deadZone)
        {
            float clampedDeadZone = Mathf.Clamp(deadZone, 0f, 0.95f);
            float magnitude = Mathf.Abs(value);
            if (magnitude < clampedDeadZone)
            {
                return 0f;
            }

            float rescaled = (magnitude - clampedDeadZone) / (1f - clampedDeadZone);
            return Mathf.Clamp(rescaled * Mathf.Sign(value), -1f, 1f);
        }

        private static float ApplyExpo(float value, float expo)
        {
            float clampedExpo = Mathf.Clamp01(expo);
            float cubic = value * value * value;
            return Mathf.Lerp(value, cubic, clampedExpo);
        }

        private static float ApplyThrottleCurve(float value, float expo)
        {
            float throttle = Mathf.Clamp01(value);
            float curved = throttle * throttle;
            return Mathf.Lerp(throttle, curved, Mathf.Clamp01(expo));
        }

        private float BlendStickAndMotion(float stickValue, float motionValue)
        {
            float stick = Mathf.Clamp(stickValue, -1f, 1f);
            float stickAuthority = Mathf.Abs(stick);
            float motionAuthority = Mathf.Clamp01(motionSensorBlend) * (1f - stickAuthority * 0.55f);
            return Mathf.Clamp(stick + motionValue * motionAuthority, -1f, 1f);
        }

        private void EnsureMotionSensors()
        {
            if (motionSensors == null)
            {
                motionSensors = new MotionSensorFlightInput();
            }

            motionSensors.Initialize();
        }

        private static bool Consume(ref bool queued)
        {
            bool value = queued;
            queued = false;
            return value;
        }
    }
}
