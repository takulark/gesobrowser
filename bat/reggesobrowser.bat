@echo off

set wfile=%1
set efile=%2
if "%wfile%"=="" (
exit /b 0
)
if "%efile%"=="" (
set efile=%1
)

reg delete HKEY_CLASSES_ROOT\%wfile% /f
reg delete HKEY_CLASSES_ROOT\%wfile%\shell /f
reg delete HKEY_CLASSES_ROOT\%wfile%\shell\open /f

reg add HKEY_CLASSES_ROOT\%wfile% /v "URL Protocol" /t reg_sz /d ""
reg add HKEY_CLASSES_ROOT\%wfile%\shell\open\command /v "" /t reg_sz /d "\"%~dp0%efile%.exe\" \"%%1\""

del %efile%.exe
copy /y gesobrowser.exe %efile%.exe
