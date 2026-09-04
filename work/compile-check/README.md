# Compile Check

Use this folder for local validation helpers.

Preferred check:

```powershell
Unity.exe -batchmode -quit -projectPath "C:\Users\HP\Documents\Codex\2026-09-04\dronesimulator-v0-1-ios-iphone-landscape" -logFile "work\unity-compile.log"
```

This environment did not have Unity or a C# compiler in PATH when the project was scaffolded, so Unity import/compile needs to be run after installing or locating Unity 6.

Fallback C# syntax/type check used by Codex in this environment:

```powershell
pwsh -ExecutionPolicy Bypass -File "work\compile-check\Invoke-CompileCheck.ps1"
```

This compiles the runtime scripts against local Unity API stubs. It catches C# errors and namespace conflicts, but Unity batchmode remains the authoritative check.

Editor script fallback check:

```powershell
pwsh -ExecutionPolicy Bypass -File "work\compile-check\Invoke-EditorCompileCheck.ps1"
```
