using DroneSimulator.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSimulator.Environment
{
    [DisallowMultipleComponent]
    public sealed class EnvironmentThemeSelector : MonoBehaviour
    {
        [SerializeField] private TrainingGroundBuilder trainingGroundBuilder;
        [SerializeField] private FlightTrainingSession trainingSession;
        [SerializeField] private Text currentThemeText;
        [SerializeField] private Text cityButtonText;
        [SerializeField] private Text forestButtonText;
        [SerializeField] private Text mountainButtonText;
        [SerializeField] private Text beachButtonText;

        private EnvironmentTheme currentTheme;

        private void Awake()
        {
            ResolveReferences();
            currentTheme = trainingGroundBuilder != null ? trainingGroundBuilder.CurrentTheme : EnvironmentTheme.City;
            UpdateLabels();
        }

        public void SelectCity()
        {
            SelectTheme(EnvironmentTheme.City);
        }

        public void SelectForest()
        {
            SelectTheme(EnvironmentTheme.Forest);
        }

        public void SelectMountain()
        {
            SelectTheme(EnvironmentTheme.Mountain);
        }

        public void SelectBeach()
        {
            SelectTheme(EnvironmentTheme.Beach);
        }

        private void SelectTheme(EnvironmentTheme theme)
        {
            ResolveReferences();
            currentTheme = theme;

            if (trainingGroundBuilder != null)
            {
                trainingGroundBuilder.SetTheme(theme);
            }

            if (trainingSession != null)
            {
                trainingSession.RestartTrainingAndDrone();
            }

            UpdateLabels();
        }

        private void ResolveReferences()
        {
            if (trainingGroundBuilder == null)
            {
                trainingGroundBuilder = Object.FindFirstObjectByType<TrainingGroundBuilder>();
            }

            if (trainingSession == null)
            {
                trainingSession = Object.FindFirstObjectByType<FlightTrainingSession>();
            }
        }

        private void UpdateLabels()
        {
            SetText(currentThemeText, "ENV " + GetDisplayName(currentTheme));
            SetButtonText(cityButtonText, EnvironmentTheme.City);
            SetButtonText(forestButtonText, EnvironmentTheme.Forest);
            SetButtonText(mountainButtonText, EnvironmentTheme.Mountain);
            SetButtonText(beachButtonText, EnvironmentTheme.Beach);
        }

        private void SetButtonText(Text target, EnvironmentTheme theme)
        {
            string prefix = currentTheme == theme ? "> " : string.Empty;
            SetText(target, prefix + GetDisplayName(theme).ToUpperInvariant());
        }

        private static string GetDisplayName(EnvironmentTheme theme)
        {
            switch (theme)
            {
                case EnvironmentTheme.City:
                    return "City";
                case EnvironmentTheme.Forest:
                    return "Forest";
                case EnvironmentTheme.Mountain:
                    return "Mountain";
                case EnvironmentTheme.Beach:
                    return "Beach";
                default:
                    return "Unknown";
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
