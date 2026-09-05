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

        public void Configure(
            TrainingGroundBuilder builder,
            FlightTrainingSession session,
            Text current,
            Text city,
            Text forest,
            Text mountain,
            Text beach)
        {
            trainingGroundBuilder = builder;
            trainingSession = session;
            currentThemeText = current;
            cityButtonText = city;
            forestButtonText = forest;
            mountainButtonText = mountain;
            beachButtonText = beach;
            currentTheme = trainingGroundBuilder != null ? trainingGroundBuilder.CurrentTheme : EnvironmentTheme.City;
            UpdateLabels();
        }

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
            SetText(target, GetShortDisplayName(theme));
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

        private static string GetShortDisplayName(EnvironmentTheme theme)
        {
            switch (theme)
            {
                case EnvironmentTheme.City:
                    return "CITY";
                case EnvironmentTheme.Forest:
                    return "FOREST";
                case EnvironmentTheme.Mountain:
                    return "MTN";
                case EnvironmentTheme.Beach:
                    return "BEACH";
                default:
                    return "ENV";
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
