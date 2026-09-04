@echo off
setlocal
cd /d "%~dp0.."
powershell -ExecutionPolicy Bypass -File "Tools\Run-UnityBatchCompile.ps1"
exit /b %ERRORLEVEL%

