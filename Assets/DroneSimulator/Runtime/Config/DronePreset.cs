using UnityEngine;
using DroneSimulator.Core;

namespace DroneSimulator.Config
{
    [CreateAssetMenu(menuName = "Drone Simulator/Drone Preset", fileName = "DronePreset")]
    public sealed class DronePreset : ScriptableObject
    {
        [Header("Frame")]
        [Min(0.05f)] public float massKg = 0.85f;
        [Min(0.05f)] public float armLengthMeters = 0.22f;

        [Header("Motor")]
        [Min(0f)] public float maxMotorThrustNewton = 8.5f;
        [Min(0f)] public float maxMotorReactionTorqueNewtonMeter = 0.18f;
        [Min(0.001f)] public float motorResponseTimeSeconds = 0.08f;
        [Range(0f, 1f)] public float idleMotorOutputWhenArmed = 0.04f;

        [Header("Aerodynamics")]
        [Min(0f)] public float linearDragCoefficient = 0.18f;
        [Min(0f)] public float angularDragCoefficient = 0.08f;

        [Header("Control")]
        [Range(0f, 60f)] public float maxStabilizedAngleDegrees = 28f;
        [Range(0f, 720f)] public float maxRateDegreesPerSecond = 220f;
        [Range(0f, 1f)] public float rollPitchMixAuthority = 0.32f;
        [Range(0f, 1f)] public float yawMixAuthority = 0.22f;
        public PidGains stabilizedAnglePid = new PidGains(4.5f, 0f, 0.08f, 0f, 4.5f);
        public PidGains rollPitchRatePid = new PidGains(0.08f, 0.01f, 0.001f, 1.0f, 1.0f);
        public PidGains yawRatePid = new PidGains(0.09f, 0.008f, 0.0008f, 1.0f, 1.0f);

        [Header("Input Feel")]
        [Range(0f, 0.35f)] public float stickDeadZone = 0.04f;
        [Range(0f, 0.9f)] public float stickExpo = 0.25f;
        [Range(0f, 0.9f)] public float throttleExpo = 0.15f;
    }
}
