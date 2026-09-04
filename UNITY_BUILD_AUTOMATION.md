# Unity Build Automation iOS Setup

This project is ready to be built by Unity Build Automation from GitHub.

## Project Values

- Repository: `https://github.com/louis0211-netizen/DroneSimulator_v0.1`
- Branch: `main`
- Unity version: `6000.3.23f1`
- Project subfolder: leave empty / repository root
- Target platform: iOS
- Bundle ID: `com.louis0211netizen.dronesimulator`
- Product name: `DroneSimulator_v0.1`
- Main scene: `Assets/DroneSimulator/Scenes/MVP_TrainingGround.unity`

## Required Apple Items

For a device-installable iOS build, Unity Build Automation needs Apple signing credentials:

- Apple Developer Program membership
- An explicit App ID using bundle ID `com.louis0211netizen.dronesimulator`
- The iPhone registered in Certificates, Identifiers & Profiles if using development or ad hoc testing
- An iOS development or ad hoc provisioning profile for that App ID and device
- A `.p12` signing certificate export
- The `.p12` password

## Unity Dashboard Setup

1. Open Unity Cloud Dashboard.
2. Create or open a Unity project for `DroneSimulator_v0.1`.
3. Go to Build Automation.
4. Connect source control to GitHub.
5. Select `louis0211-netizen/DroneSimulator_v0.1`.
6. Create an iOS build target.
7. Use Unity version `6000.3.23f1`.
8. Set the project path/subfolder to the repository root.
9. Set Bundle ID to `com.louis0211netizen.dronesimulator`.
10. Upload the iOS signing credentials.
11. Start a build from branch `main`.

## Installing on iPhone

Recommended first path:

1. Build a signed iOS artifact in Unity Build Automation.
2. Download the generated `.ipa`.
3. Install through the Apple-supported route you choose:
   - TestFlight through App Store Connect.
   - Ad hoc install for registered devices.
   - Xcode Devices and Simulators / Apple Configurator on a Mac.

## Notes

- Windows can continue to be the main coding machine for this project.
- Unity Build Automation replaces the local macOS Unity export step.
- A physical iPhone still requires Apple code signing.
- The GitHub Actions Unity build remains license-aware and only runs full GameCI build when Unity secrets are configured.
