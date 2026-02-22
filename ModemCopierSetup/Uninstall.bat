@echo off
setlocal enabledelayedexpansion

echo ============================================
echo ModemCopier 2.0 - Uninstallation
echo Copyright 2026 Halliburton
echo ============================================
echo.

:: Set installation directory
set "INSTALL_DIR=%ProgramFiles%\Halliburton\ModemCopier 2.0"

:: Check if running as administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo This uninstaller requires administrator privileges.
    echo Please right-click and select "Run as administrator"
    pause
    exit /b 1
)

echo Uninstalling ModemCopier 2.0...
echo.

:: Remove desktop shortcut
if exist "%USERPROFILE%\Desktop\ModemCopier 2.0.lnk" (
    del "%USERPROFILE%\Desktop\ModemCopier 2.0.lnk"
    echo Removed desktop shortcut
)

:: Remove Start Menu shortcut
set "START_MENU=%ProgramData%\Microsoft\Windows\Start Menu\Programs\Halliburton"
if exist "%START_MENU%\ModemCopier 2.0.lnk" (
    del "%START_MENU%\ModemCopier 2.0.lnk"
    echo Removed Start Menu shortcut
)

:: Remove installation directory
if exist "%INSTALL_DIR%" (
    rmdir /S /Q "%INSTALL_DIR%"
    echo Removed installation directory
)

:: Remove parent folder if empty
if exist "%ProgramFiles%\Halliburton" (
    rmdir "%ProgramFiles%\Halliburton" 2>nul
)

if exist "%START_MENU%" (
    rmdir "%START_MENU%" 2>nul
)

echo.
echo ============================================
echo Uninstallation completed successfully!
echo ============================================
echo.
pause
