@ECHO OFF

SETLOCAL

SET SCRIPTPATH=%~dp0
SET SCRIPTPATH=%SCRIPTPATH:~0,-1%

CD %SCRIPTPATH%

CALL ..\..\..\build\set35vars.bat

%msbuildexe% Md5.sln /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build

CALL signcmd .\src\bin\Release\Md5.exe

ENDLOCAL