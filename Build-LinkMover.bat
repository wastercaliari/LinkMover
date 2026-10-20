@echo off
setlocal
set "DOTNET=%~dp0.dotnet\dotnet.exe"
if not exist "%DOTNET%" set "DOTNET=dotnet"
"%DOTNET%" build "%~dp0LinkMover.slnx" -c Release
if errorlevel 1 pause
