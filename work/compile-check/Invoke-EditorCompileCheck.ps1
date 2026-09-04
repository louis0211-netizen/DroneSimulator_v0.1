$ErrorActionPreference = "Stop"

if ($PSVersionTable.PSVersion.Major -lt 7) {
    $pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
    if ($pwsh -ne $null) {
        & $pwsh.Source -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath
        exit $LASTEXITCODE
    }
}

$projectRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$runtimePath = Join-Path $projectRoot "Assets\DroneSimulator\Runtime"
$editorPath = Join-Path $projectRoot "Assets\DroneSimulator\Editor"
$sources = @(
    (Join-Path $PSScriptRoot "UnityEngineStubs.cs"),
    (Join-Path $PSScriptRoot "UnityEditorStubs.cs")
) + (Get-ChildItem -Path $runtimePath -Filter "*.cs" -Recurse | ForEach-Object { $_.FullName }) +
    (Get-ChildItem -Path $editorPath -Filter "*.cs" -Recurse | ForEach-Object { $_.FullName })

Add-Type -Path $sources -CompilerOptions "/nowarn:0649,0414,0219"
Write-Host "Editor compile check passed with Unity stubs."

