using System;
using UnityEngine;

namespace DroneSimulator.Core
{
    [Serializable]
    public struct PidGains
    {
        [Min(0f)] public float proportional;
        [Min(0f)] public float integral;
        [Min(0f)] public float derivative;
        [Min(0f)] public float integralLimit;
        [Min(0f)] public float outputLimit;

        public PidGains(float proportional, float integral, float derivative, float integralLimit, float outputLimit)
        {
            this.proportional = proportional;
            this.integral = integral;
            this.derivative = derivative;
            this.integralLimit = integralLimit;
            this.outputLimit = outputLimit;
        }
    }

    [Serializable]
    public struct AxisPidState
    {
        public float integral;
        public float previousError;
        public bool hasPreviousError;
    }

    public static class PidController
    {
        public static float Step(float target, float measured, float deltaTime, PidGains gains, ref AxisPidState state)
        {
            float error = target - measured;
            float safeDeltaTime = Mathf.Max(0.0001f, deltaTime);

            state.integral += error * safeDeltaTime;
            if (gains.integralLimit > 0f)
            {
                state.integral = Mathf.Clamp(state.integral, -gains.integralLimit, gains.integralLimit);
            }

            float derivative = state.hasPreviousError ? (error - state.previousError) / safeDeltaTime : 0f;
            state.previousError = error;
            state.hasPreviousError = true;

            float output = (error * gains.proportional) + (state.integral * gains.integral) + (derivative * gains.derivative);
            if (gains.outputLimit > 0f)
            {
                output = Mathf.Clamp(output, -gains.outputLimit, gains.outputLimit);
            }

            return output;
        }

        public static void Reset(ref AxisPidState state)
        {
            state.integral = 0f;
            state.previousError = 0f;
            state.hasPreviousError = false;
        }
    }
}

