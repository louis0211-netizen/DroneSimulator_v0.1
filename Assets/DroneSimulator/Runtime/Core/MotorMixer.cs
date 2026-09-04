using UnityEngine;

namespace DroneSimulator.Core
{
    public sealed class MotorMixer
    {
        private MotorCommand[] commands;

        public MotorMixer(int motorCount)
        {
            commands = new MotorCommand[motorCount];
        }

        public MotorCommand[] Mix(FlightControlCommand control, MotorDefinition[] motors, Config.DronePreset preset)
        {
            if (motors == null)
            {
                return commands;
            }

            EnsureCommandCount(motors.Length);

            for (int i = 0; i < motors.Length; i++)
            {
                MotorDefinition motor = motors[i];
                float rollSign = motor.localPosition.x >= 0f ? -1f : 1f;
                float pitchSign = motor.localPosition.z >= 0f ? -1f : 1f;
                float yawSign = -(float)motor.spinDirection;

                float rollPitchAuthority = preset != null ? preset.rollPitchMixAuthority : 0.32f;
                float yawAuthority = preset != null ? preset.yawMixAuthority : 0.22f;
                float armedIdle = preset != null ? preset.idleMotorOutputWhenArmed : 0.04f;

                float mixed = control.throttle;
                mixed += control.roll * rollSign * rollPitchAuthority;
                mixed += control.pitch * pitchSign * rollPitchAuthority;
                mixed += control.yaw * yawSign * yawAuthority;

                float normalized = control.armed ? Mathf.Clamp01(mixed) : 0f;
                if (control.armed)
                {
                    normalized = Mathf.Max(normalized, armedIdle);
                }

                commands[i] = new MotorCommand
                {
                    corner = motor.corner,
                    normalizedOutput = normalized,
                    thrustNewton = normalized * motor.maxThrustNewton,
                    reactionTorqueNewtonMeter = normalized * motor.maxReactionTorqueNewtonMeter * (float)motor.spinDirection
                };
            }

            return commands;
        }

        private void EnsureCommandCount(int count)
        {
            if (commands.Length == count)
            {
                return;
            }

            commands = new MotorCommand[count];
        }
    }
}
