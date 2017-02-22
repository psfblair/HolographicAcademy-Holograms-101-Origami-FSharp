cd %~dp0
cd "App"

msbuild.exe Origami.sln /t:Build /p:Configuration=Release;Platform=x86;AppxBundle=Always
