@echo off
setlocal EnableExtensions
REM Usage: run-blazor-chat.cmd [PORT]
set PORT=%~1
if "%PORT%"=="" set PORT=5092

set PROJECT_DIR=%~dp0SEP3_T1_BlazerUI\BlazorApp1
if not exist "%PROJECT_DIR%\BlazorApp1.csproj" (
  echo ERROR: Could not find project at "%PROJECT_DIR%\BlazorApp1.csproj".
  exit /b 1
)

pushd "%PROJECT_DIR%"
echo Restoring packages...
dotnet restore || (echo Restore failed. && popd && exit /b 1)
echo Running BlazorApp1 on http://localhost:%PORT% ...
echo (Press Ctrl+C in this window to stop)
dotnet run -- --urls http://localhost:%PORT%
set ERR=%ERRORLEVEL%
popd
exit /b %ERR%
