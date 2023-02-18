@ECHO OFF

SETLOCAL

SET SCRIPTPATH=%~dp0
SET SCRIPTPATH=%SCRIPTPATH:~0,-1%

CD %SCRIPTPATH%

CALL %CTKBLDROOT%\setupEnv.cmd

SET BASENAME=Cyotek.Md5
SET RELDIR=src\bin\Release\
SET DLLNAME=%BASENAME%.dll

IF EXIST %RELDIR%*.*  DEL /F /Q %RELDIR%*.*

%msbuildexe% %BASENAME%.sln /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build

PUSHD %RELDIR%

CALL signcmd .\md5.exe

%zipexe% a %BASENAME%.1.x.x.zip -r

POPD

ENDLOCAL
