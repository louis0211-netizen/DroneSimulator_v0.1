using System;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace DroneSimulator.Input
{
    [Serializable]
    public sealed class MotionSensorFlightInput
    {
        [SerializeField] private bool autoEnableOnDevice = true;
        [SerializeField] private bool enableInEditor;
        [SerializeField, Range(0f, 3f)] private float rollSensitivity = 1.35f;
        [SerializeField, Range(0f, 3f)] private float pitchSensitivity = 1.2f;
        [SerializeField, Range(0f, 1f)] private float gyroRateAssist = 0.08f;
        [SerializeField, Range(1f, 30f)] private float accelerometerFilterHz = 12f;

        private Vector3 neutralAcceleration = new Vector3(0f, 0f, -1f);
        private Vector3 filteredAcceleration = new Vector3(0f, 0f, -1f);
        private bool initialized;
        private bool calibrated;

        public bool IsEnabled { get; private set; }

        public string StatusText
        {
            get
            {
                if (!IsEnabled)
                {
                    return "TILT OFF";
                }

                return SystemInfo.supportsGyroscope ? "TILT GYRO" : "TILT ACC";
            }
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            SetEnabled(ShouldAutoEnable());
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;

            if (SystemInfo.supportsGyroscope)
            {
                UnityInput.gyro.enabled = enabled;
            }

            if (enabled)
            {
                filteredAcceleration = ReadAccelerationOrDefault();
                CalibrateNeutral();
            }
        }

        public void ToggleEnabled()
        {
            SetEnabled(!IsEnabled);
        }

        public void CalibrateNeutral()
        {
            neutralAcceleration = ReadAccelerationOrDefault();
            filteredAcceleration = neutralAcceleration;
            calibrated = true;
        }

        public Vector2 ReadRollPitch()
        {
            Initialize();

            if (!IsEnabled)
            {
                return Vector2.zero;
            }

            Vector3 acceleration = ReadAccelerationOrDefault();
            float filter = 1f - Mathf.Exp(-Mathf.Max(1f, accelerometerFilterHz) * Time.deltaTime);
            filteredAcceleration = Vector3.Lerp(filteredAcceleration, acceleration, filter);

            if (!calibrated)
            {
                CalibrateNeutral();
            }

            Vector3 delta = filteredAcceleration - neutralAcceleration;
            float roll = Mathf.Clamp(delta.y * rollSensitivity, -1f, 1f);
            float pitch = Mathf.Clamp(-delta.x * pitchSensitivity, -1f, 1f);

            if (SystemInfo.supportsGyroscope && UnityInput.gyro.enabled)
            {
                Vector3 rate = UnityInput.gyro.rotationRateUnbiased;
                roll = Mathf.Clamp(roll - rate.y * gyroRateAssist, -1f, 1f);
                pitch = Mathf.Clamp(pitch + rate.x * gyroRateAssist, -1f, 1f);
            }

            return new Vector2(roll, pitch);
        }

        private static Vector3 ReadAccelerationOrDefault()
        {
            Vector3 acceleration = UnityInput.acceleration;
            return acceleration.magnitude > 0.001f ? acceleration : new Vector3(0f, 0f, -1f);
        }

        private bool ShouldAutoEnable()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return autoEnableOnDevice;
#else
            return enableInEditor;
#endif
        }
    }
}
