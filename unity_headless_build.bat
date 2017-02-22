SET UNITY_FOLDER=Unity 5.6.0b9

cd %~dp0
if not exist "App" mkdir "App"

"C:\Program Files\%UNITY_FOLDER%\Editor\Unity.exe" -batchmode -quit -projectPath %~dp0 -executeMethod Autobuild.Build