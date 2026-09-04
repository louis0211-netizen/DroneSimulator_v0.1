$ErrorActionPreference = "Stop"

if ($PSVersionTable.PSVersion.Major -lt 7) {
    $pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
    if ($pwsh -ne $null) {
        & $pwsh.Source -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath
        exit $LASTEXITCODE
    }
}

$projectRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$stubPath = Join-Path $PSScriptRoot "UnityEngineStubs.cs"
$runtimePath = Join-Path $projectRoot "Assets\DroneSimulator\Runtime"
$sources = @($stubPath) + (Get-ChildItem -Path $runtimePath -Filter "*.cs" -Recurse | ForEach-Object { $_.FullName })

Add-Type -Path $sources -CompilerOptions "/nowarn:0649,0414"
Write-Host "Compile check passed with UnityEngine stubs."
