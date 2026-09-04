@echo off
setlocal
cd /d "%~dp0.."
powershell -ExecutionPolicy Bypass -File "work\compile-check\Invoke-CompileCheck.ps1"
if errorlevel 1 exit /b %ERRORLEVEL%
powershell -ExecutionPolicy Bypass -File "work\compile-check\Invoke-EditorCompileCheck.ps1"
exit /b %ERRORLEVEL%

