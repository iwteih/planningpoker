@echo off

path=%path%;%windir%\Microsoft.net\Framework\v3.5

cd %~dp0

call version-number.bat

msbuild build.proj /p:CreateZip="%1" /p:CreatePackage="%2" /p:RunTests="%3" /p:CreateClickOnce="%4"

pause