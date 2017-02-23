SET UNITY_FOLDER=Unity 5.6.0b9
SET OUTPUT_DIR=App

cd %~dp0
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

"C:\Program Files\%UNITY_FOLDER%\Editor\Unity.exe" -batchmode -quit -projectPath %~dp0 -executeMethod Autobuild.Build