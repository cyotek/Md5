@ECHO OFF

SETLOCAL

SET SCRIPTPATH=%~dp0
SET SCRIPTPATH=%SCRIPTPATH:~0,-1%

SET DST=%CTKBLDROOT%..\..\..\bin\

CD %SCRIPTPATH%

CALL build.cmd

COPY .\src\bin\Release\Md5.* %DST% /Y

ENDLOCAL