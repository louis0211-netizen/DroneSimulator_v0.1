param(
    [string] $ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [string] $UnityVersion = "6000.3.23f1",
    [string] $LogFile = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot "..")).Path "work\unity-batchmode.log")
)

$ErrorActionPreference = "Stop"

$unityCandidates = @(
    "C:\Program Files\Unity\Hub\Editor\$UnityVersion\Editor\Unity.com",
    "C:\Program Files\Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe",
    "$env:ProgramFiles\Unity\Hub\Editor\$UnityVersion\Editor\Unity.com",
    "$env:ProgramFiles\Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe"
)

$unity = $unityCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $unity) {
    $command = Get-Command Unity.exe -ErrorAction SilentlyContinue
    if ($command) {
        $unity = $command.Source
    }
}

if (-not $unity) {
    throw "Unity.exe was not found for version $UnityVersion."
}

Write-Host "Running Unity batchmode compile/import check..."
if (Test-Path $LogFile) {
    Remove-Item -LiteralPath $LogFile -Force
}

& $unity -batchmode -nographics -quit -projectPath $ProjectPath -logFile $LogFile
$exitCode = $LASTEXITCODE

if ((Test-Path $LogFile) -and (Select-String -Path $LogFile -Pattern "No valid Unity Editor license found" -Quiet)) {
    Write-Host "Unity batchmode is blocked by licensing. Open Unity Hub, sign in, and activate an Editor license."
    Write-Host "Tail of log:"
    Get-Content $LogFile -Tail 80
    exit 198
}

if ((Test-Path $LogFile) -and (Select-String -Path $LogFile -Pattern "Compiler errors|Compilation failed|Build failed" -Quiet)) {
    Write-Host "Unity batchmode found compile/build errors. Tail of log:"
    Get-Content $LogFile -Tail 120
    exit 1
}

if ($exitCode -ne 0) {
    Write-Host "Unity batchmode failed. Tail of log:"
    if (Test-Path $LogFile) {
        Get-Content $LogFile -Tail 120
    } else {
        Write-Host "Unity did not create a log file at $LogFile"
    }
    exit $exitCode
}

Write-Host "Unity batchmode check passed. Log: $LogFile"
