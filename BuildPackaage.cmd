set batPath = %~dp0
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\build.ps1'"
Pause