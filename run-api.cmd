@echo off
setlocal EnableExtensions
REM Usage: run-api.cmd [PORT]
set PORT=%~1
if "%PORT%"=="" set PORT=8082

set PROJECT_DIR=%~dp0AspCoreWebAPI
if not exist "%PROJECT_DIR%\AspCoreWebAPI.csproj" (
  echo ERROR: Could not find project at "%PROJECT_DIR%\AspCoreWebAPI.csproj".
  exit /b 1
)

pushd "%PROJECT_DIR%"
echo Restoring packages...
dotnet restore || (echo Restore failed. && popd && exit /b 1)
echo Running AspCoreWebAPI on http://localhost:%PORT% ...
echo (Press Ctrl+C in this window to stop)
dotnet run -- --urls http://localhost:%PORT%
set ERR=%ERRORLEVEL%
popd
exit /b %ERR%

