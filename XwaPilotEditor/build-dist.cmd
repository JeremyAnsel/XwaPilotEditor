@echo off
setlocal

cd "%~dp0"

For %%a in (
"XwaPilotEditor\bin\Release\net48\*.dll"
"XwaPilotEditor\bin\Release\net48\*.exe"
"XwaPilotEditor\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)
