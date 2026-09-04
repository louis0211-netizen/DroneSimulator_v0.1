# GitHub Setup

## Recommended Repository Flow

1. Create an empty GitHub repository named `DroneSimulator_v0.1`.
2. Add it as the remote:

```powershell
git remote add origin https://github.com/YOUR_ACCOUNT/DroneSimulator_v0.1.git
```

3. Push the initial commit:

```powershell
git push -u origin main
```

## Workflows

### Fallback C# Checks

File:

`.github/workflows/fallback-csharp-checks.yml`

Purpose:

- Runs on `windows-latest`.
- Does not require Unity.
- Compiles Runtime and Editor scripts against local Unity stubs.
- Catches ordinary C# syntax/type errors early.

### Unity Compile

File:

`.github/workflows/unity-compile.yml`

Purpose:

- Always runs the local C# stub compile checks on `windows-latest`.
- Uses GameCI Unity Builder only when `UNITY_LICENSE` is configured.
- Imports the Unity project and builds `StandaloneWindows64` when licensed.
- Shows a GitHub Actions notice instead of failing when Unity secrets have not been added yet.

### iOS Xcode Export

File:

`.github/workflows/ios-xcode-export.yml`

Purpose:

- Manual `workflow_dispatch` only.
- Runs on `macos-latest`.
- Exports an iOS Xcode project using Unity.
- Requires Unity license secrets.

## Required GitHub Secrets

For full GameCI Unity build/export workflows, add these repository secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

For Unity Personal, generate and store a Unity license file following GameCI activation instructions.

Without these secrets, `Unity Compile` still performs the fallback C# compile checks and skips the GameCI build step.

## Local Validation

From the project root:

```powershell
Tools\Run-FallbackCompileChecks.cmd
Tools\Run-UnityBatchCompile.cmd
```

## Platform Notes

- Windows is fine for Unity source development and compile/import validation.
- iOS export and installation to iPhone should be done on macOS with Xcode and iOS Build Support.
