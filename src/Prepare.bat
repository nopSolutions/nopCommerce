@echo off
cd /d %~dp0build
cmd /c "build.bat" & cd .. & pause & exit /b
