@echo off
cd /d %~dp0build
cmd /c "build.bat /target:Deploy" & cd .. & pause & exit /b
