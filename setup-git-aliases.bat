@echo off
setlocal enabledelayedexpansion

REM Path to alias file relative to repo
set "ALIAS_FILE=.gitaliases"

REM Get full path to repo root
for /f %%i in ('git rev-parse --show-toplevel') do set "REPO_ROOT=%%i"
set "FULL_PATH=%REPO_ROOT%\%ALIAS_FILE%"

REM Check if alias already configured
git config --global --get include.path | findstr /C:"%FULL_PATH%" >nul
if %errorlevel%==0 (
    echo Git alias file already included
) else (
    git config --global --add include.path "%FULL_PATH%"
    echo Alias file added to your global Git config
)

endlocal