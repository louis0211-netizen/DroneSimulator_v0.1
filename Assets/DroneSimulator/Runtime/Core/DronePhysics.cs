using DroneSimulator.Config;
using UnityEngine;

namespace DroneSimulator.Core
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(FlightController))]
    [DisallowMultipleComponent]
    public sealed class DronePhysics : MonoBehaviour
    {
        [SerializeField] private DronePreset preset;
        [SerializeField] private MotorDefinition[] motors;
        [SerializeField] private bool createDefaultMotorLayout = true;
        [SerializeField] private float crashAngularVelocity = 18f;
        [SerializeField] private float crashImpactVelocity = 7f;

        private Rigidbody body;
        private FlightController flightController;
        private DroneSimulator.Input.IFlightInputSource inputSource;
        private MotorMixer mixer;
        private MotorCommand[] currentMotorCommands;
        private float[] motorOutputs;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private bool crashed;

        public bool HasCrashed => crashed;
        public MotorCommand[] CurrentMotorCommands => currentMotorCommands;
        public float AltitudeMeters => transform.position.y;
        public float SpeedMetersPerSecond => body != null ? body.linearVelocity.magnitude : 0f;
        public float VerticalSpeedMetersPerSecond => body != null ? body.linearVelocity.y : 0f;
        public Vector3 WorldVelocity => body != null ? body.linearVelocity : Vector3.zero;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            flightController = GetComponent<FlightController>();
            inputSource = GetComponent<DroneSimulator.Input.IFlightInputSource>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            ApplyPresetToRigidbody();

            if ((motors == null || motors.Length == 0) && createDefaultMotorLayout)
            {
                motors = CreateDefaultMotors();
            }

            mixer = new MotorMixer(motors != null ? motors.Length : 0);
            motorOutputs = new float[motors != null ? motors.Length : 0];
        }

        private void FixedUpdate()
        {
            DroneInputState input = inputSource != null ? inputSource.ReadInput() : DroneInputState.Neutral;

            if (input.resetPressed)
            {
                ResetDrone();
                return;
            }

            FlightControlCommand control = flightController.BuildCommand(input);
            currentMotorCommands = mixer.Mix(control, motors, preset);

            ApplyMotors(Time.fixedDeltaTime);
            ApplyAerodynamicDrag();

            if (body.angularVelocity.magnitude > crashAngularVelocity)
            {
                crashed = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude >= crashImpactVelocity)
            {
                crashed = true;
                flightController.Disarm();
            }
        }

        public void ResetDrone()
        {
            crashed = false;
            flightController.Disarm();
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;

            for (int i = 0; i < motorOutputs.Length; i++)
            {
                motorOutputs[i] = 0f;
            }
        }

        private void ApplyMotors(float deltaTime)
        {
            if (motors == null || currentMotorCommands == null)
            {
                return;
            }

            for (int i = 0; i < motors.Length; i++)
            {
                MotorDefinition motor = motors[i];
                MotorCommand command = currentMotorCommands[i];
                float response = Mathf.Max(0.001f, motor.responseTimeSeconds);
                motorOutputs[i] = Mathf.MoveTowards(motorOutputs[i], command.normalizedOutput, deltaTime / response);

                float thrust = motorOutputs[i] * motor.maxThrustNewton;
                float reactionTorque = motorOutputs[i] * motor.maxReactionTorqueNewtonMeter * (float)motor.spinDirection;

                Vector3 worldForce = transform.TransformDirection(motor.localThrustAxis.normalized) * thrust;
                Vector3 worldPosition = transform.TransformPoint(motor.localPosition);
                body.AddForceAtPosition(worldForce, worldPosition, ForceMode.Force);
                body.AddRelativeTorque(Vector3.up * reactionTorque, ForceMode.Force);

                currentMotorCommands[i] = new MotorCommand
                {
                    corner = motor.corner,
                    normalizedOutput = motorOutputs[i],
                    thrustNewton = thrust,
                    reactionTorqueNewtonMeter = reactionTorque
                };
            }
        }

        private void ApplyAerodynamicDrag()
        {
            float linearDrag = preset != null ? preset.linearDragCoefficient : 0.18f;
            float angularDrag = preset != null ? preset.angularDragCoefficient : 0.08f;

            body.AddForce(-body.linearVelocity * linearDrag, ForceMode.Force);
            body.AddTorque(-body.angularVelocity * angularDrag, ForceMode.Force);
        }

        private void ApplyPresetToRigidbody()
        {
            if (preset != null)
            {
                body.mass = preset.massKg;
            }

            body.useGravity = true;
            body.maxAngularVelocity = 40f;
        }

        private MotorDefinition[] CreateDefaultMotors()
        {
            float arm = preset != null ? preset.armLengthMeters : 0.22f;
            float thrust = preset != null ? preset.maxMotorThrustNewton : 8.5f;
            float torque = preset != null ? preset.maxMotorReactionTorqueNewtonMeter : 0.18f;
            float response = preset != null ? preset.motorResponseTimeSeconds : 0.08f;

            return new[]
            {
                MotorDefinition.Create(MotorCorner.FrontLeft, MotorSpinDirection.CounterClockwise, new Vector3(-arm, 0f, arm), thrust, torque, response),
                MotorDefinition.Create(MotorCorner.FrontRight, MotorSpinDirection.Clockwise, new Vector3(arm, 0f, arm), thrust, torque, response),
                MotorDefinition.Create(MotorCorner.RearRight, MotorSpinDirection.CounterClockwise, new Vector3(arm, 0f, -arm), thrust, torque, response),
                MotorDefinition.Create(MotorCorner.RearLeft, MotorSpinDirection.Clockwise, new Vector3(-arm, 0f, -arm), thrust, torque, response)
            };
        }
    }
}
