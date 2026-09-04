using DroneSimulator.Core;
using UnityEngine;

namespace DroneSimulator.Systems
{
    [DisallowMultipleComponent]
    public sealed class BatterySimulator : MonoBehaviour
    {
        [SerializeField] private DronePhysics dronePhysics;
        [SerializeField] private FlightController flightController;
        [SerializeField] private float capacityMilliampHours = 1300f;
        [SerializeField] private float nominalVoltage = 14.8f;
        [SerializeField] private float minVoltage = 13.2f;
        [SerializeField] private float maxMotorCurrentAmps = 18f;
        [SerializeField] private bool disarmWhenEmpty = true;

        private float remainingMilliampHours;

        public float Percent => capacityMilliampHours > 0f ? Mathf.Clamp01(remainingMilliampHours / capacityMilliampHours) * 100f : 0f;
        public float Voltage => Mathf.Lerp(minVoltage, nominalVoltage, Percent / 100f);
        public bool IsEmpty => Percent <= 0.01f;

        private void Awake()
        {
            remainingMilliampHours = capacityMilliampHours;

            if (dronePhysics == null)
            {
                dronePhysics = GetComponent<DronePhysics>();
            }

            if (flightController == null)
            {
                flightController = GetComponent<FlightController>();
            }
        }

        private void Update()
        {
            if (flightController == null || !flightController.IsArmed)
            {
                return;
            }

            float currentAmps = EstimateCurrentDrawAmps();
            float consumedMilliampHours = currentAmps * 1000f * (Time.deltaTime / 3600f);
            remainingMilliampHours = Mathf.Max(0f, remainingMilliampHours - consumedMilliampHours);

            if (disarmWhenEmpty && IsEmpty)
            {
                flightController.Disarm();
            }
        }

        public void ResetBattery()
        {
            remainingMilliampHours = capacityMilliampHours;
        }

        private float EstimateCurrentDrawAmps()
        {
            MotorCommand[] commands = dronePhysics != null ? dronePhysics.CurrentMotorCommands : null;
            if (commands == null || commands.Length == 0)
            {
                return 0f;
            }

            float total = 0f;
            for (int i = 0; i < commands.Length; i++)
            {
                total += commands[i].normalizedOutput * maxMotorCurrentAmps;
            }

            return total;
        }
    }
}

