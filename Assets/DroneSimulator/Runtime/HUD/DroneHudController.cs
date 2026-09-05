using DroneSimulator.Core;
using DroneSimulator.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSimulator.HUD
{
    public sealed class DroneHudController : MonoBehaviour
    {
        [SerializeField] private DronePhysics dronePhysics;
        [SerializeField] private FlightController flightController;
        [SerializeField] private BatterySimulator batterySimulator;
        [SerializeField] private Text altitudeText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text flightTimeText;
        [SerializeField] private Text flightModeText;
        [SerializeField] private Text batteryText;

        public void Configure(
            DronePhysics physics,
            FlightController controller,
            BatterySimulator battery,
            Text altitude,
            Text speed,
            Text flightTime,
            Text flightMode,
            Text batteryDisplay)
        {
            dronePhysics = physics;
            flightController = controller;
            batterySimulator = battery;
            altitudeText = altitude;
            speedText = speed;
            flightTimeText = flightTime;
            flightModeText = flightMode;
            batteryText = batteryDisplay;
        }

        private void Update()
        {
            if (dronePhysics == null || flightController == null)
            {
                return;
            }

            SetText(altitudeText, string.Format("ALT {0:0.0} m", dronePhysics.AltitudeMeters));
            SetText(speedText, string.Format("SPD {0:0.0} m/s", dronePhysics.SpeedMetersPerSecond));
            SetText(flightTimeText, string.Format("TIME {0:0.0} s", flightController.FlightTimeSeconds));
            SetText(flightModeText, string.Format("{0} {1}", flightController.CurrentMode, flightController.IsArmed ? "ARMED" : "DISARMED"));
            SetText(batteryText, batterySimulator != null
                ? string.Format("BAT {0:0}% {1:0.0}V", batterySimulator.Percent, batterySimulator.Voltage)
                : "BAT --");
        }

        public void ResetBattery()
        {
            if (batterySimulator != null)
            {
                batterySimulator.ResetBattery();
            }
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
