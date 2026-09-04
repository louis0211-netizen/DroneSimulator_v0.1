using UnityEngine;

namespace DroneSimulator.Bootstrap
{
    public sealed class IosLandscapeConfigurator : MonoBehaviour
    {
        [SerializeField] private ScreenOrientation preferredOrientation = ScreenOrientation.LandscapeLeft;
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool keepScreenAwake = true;

        private void Awake()
        {
            Screen.orientation = preferredOrientation;
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = 0;

            if (keepScreenAwake)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }
    }
}

