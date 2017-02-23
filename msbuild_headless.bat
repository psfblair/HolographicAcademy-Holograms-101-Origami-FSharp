SET OUTPUT_DIR=App
set SOLUTION_FILE_NAME=Origami.sln

cd "%OUTPUT_DIR%"

msbuild.exe %SOLUTION_FILE_NAME% /t:Build /p:Configuration=Release;Platform=x86;AppxBundle=Always