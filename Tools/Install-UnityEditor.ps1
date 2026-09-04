param(
    [string] $Version = "6000.3.23f1",
    [string[]] $Modules = @("ios")
)

$ErrorActionPreference = "Stop"

$unityCliCandidates = @(
    "C:\Program Files\Unity Hub\resources\cli\unity.exe",
    "$env:LOCALAPPDATA\Programs\Unity Hub\resources\cli\unity.exe"
)

$unityCli = $unityCliCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $unityCli) {
    $command = Get-Command unity -ErrorAction SilentlyContinue
    if ($command) {
        $unityCli = $command.Source
    }
}

if (-not $unityCli) {
    throw "Unity CLI was not found. Install Unity Hub or Unity CLI first."
}

$args = @("--no-banner", "--non-interactive", "install", $Version)
foreach ($module in $Modules) {
    $args += @("-m", $module)
}

Write-Host "Installing Unity Editor $Version with modules: $($Modules -join ', ')"
& $unityCli @args
