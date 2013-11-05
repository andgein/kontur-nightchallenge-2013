@echo off

call rm-dir.cmd .deploy\bin || exit /b 1

c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe nc2013\nc2013.sln /p:Configuration=Release /v:m || exit /b 1

pushd nc2013
erase src.zip || exit /b 1
..\7za.exe a -ir!* -xr@..\srcFilesToExclude.txt src.zip || exit /b 1
move /y src.zip ..\.deploy || exit /b 1
popd

pushd .deploy
erase corewar.zip || exit /b 1
..\7za.exe a -ir!bin\* corewar.zip src.zip || exit /b 1
move /y corewar.zip bin\StaticContent || exit /b 1
popd

erase corewar-deploy.zip || exit /b 1
7za.exe a -ir!.deploy\bin\* -i!run-production.cmd -i!production-config.sample -xr!.deploy\logs\* corewar-deploy.zip || exit /b 1

echo Corewar build succeeded!

exit /b 0
