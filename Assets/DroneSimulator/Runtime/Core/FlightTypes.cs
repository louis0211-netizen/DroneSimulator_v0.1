using System;
using UnityEngine;

namespace DroneSimulator.Core
{
    public enum FlightMode
    {
        Stabilized,
        Acro
    }

    public enum MotorCorner
    {
        FrontLeft,
        FrontRight,
        RearRight,
        RearLeft
    }

    public enum MotorSpinDirection
    {
        Clockwise = -1,
        CounterClockwise = 1
    }

    [Serializable]
    public struct DroneInputState
    {
        [Range(0f, 1f)] public float throttle;
        [Range(-1f, 1f)] public float roll;
        [Range(-1f, 1f)] public float pitch;
        [Range(-1f, 1f)] public float yaw;
        public bool armPressed;
        public bool resetPressed;
        public bool cameraPressed;
        public bool flightModePressed;

        public static DroneInputState Neutral => new DroneInputState
        {
            throttle = 0f,
            roll = 0f,
            pitch = 0f,
            yaw = 0f
        };
    }

    [Serializable]
    public struct FlightControlCommand
    {
        [Range(0f, 1f)] public float throttle;
        [Range(-1f, 1f)] public float roll;
        [Range(-1f, 1f)] public float pitch;
        [Range(-1f, 1f)] public float yaw;
        public bool armed;
        public FlightMode mode;

        public static FlightControlCommand Disarmed(FlightMode mode)
        {
            return new FlightControlCommand
            {
                throttle = 0f,
                roll = 0f,
                pitch = 0f,
                yaw = 0f,
                armed = false,
                mode = mode
            };
        }
    }

    [Serializable]
    public struct MotorDefinition
    {
        public MotorCorner corner;
        public MotorSpinDirection spinDirection;
        public Vector3 localPosition;
        public Vector3 localThrustAxis;
        [Min(0f)] public float maxThrustNewton;
        [Min(0f)] public float maxReactionTorqueNewtonMeter;
        [Min(0.001f)] public float responseTimeSeconds;

        public static MotorDefinition Create(
            MotorCorner corner,
            MotorSpinDirection spinDirection,
            Vector3 localPosition,
            float maxThrustNewton,
            float maxReactionTorqueNewtonMeter,
            float responseTimeSeconds)
        {
            return new MotorDefinition
            {
                corner = corner,
                spinDirection = spinDirection,
                localPosition = localPosition,
                localThrustAxis = Vector3.up,
                maxThrustNewton = maxThrustNewton,
                maxReactionTorqueNewtonMeter = maxReactionTorqueNewtonMeter,
                responseTimeSeconds = responseTimeSeconds
            };
        }
    }

    [Serializable]
    public struct MotorCommand
    {
        public MotorCorner corner;
        [Range(0f, 1f)] public float normalizedOutput;
        public float thrustNewton;
        public float reactionTorqueNewtonMeter;
    }
}

