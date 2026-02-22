@echo off
setlocal enabledelayedexpansion

echo ============================================
echo ModemCopier 2.0 - Installation
echo Copyright 2026 Halliburton
echo ============================================
echo.

:: Set installation directory
set "INSTALL_DIR=%ProgramFiles%\Halliburton\ModemCopier 2.0"

:: Check if running as administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo This installer requires administrator privileges.
    echo Please right-click and select "Run as administrator"
    pause
    exit /b 1
)

echo Installing ModemCopier 2.0...
echo.

:: Create installation directory
if not exist "%INSTALL_DIR%" (
    mkdir "%INSTALL_DIR%"
    echo Created directory: %INSTALL_DIR%
)

:: Copy files
echo Copying application files...
xcopy /Y /Q ".\Release\*.*" "%INSTALL_DIR%\"

:: Rename executable
if exist "%INSTALL_DIR%\ModemMergerWinFormsApp.exe" (
    ren "%INSTALL_DIR%\ModemMergerWinFormsApp.exe" "ModemCopier.exe"
)

:: Create desktop shortcut
echo Creating desktop shortcut...
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%USERPROFILE%\Desktop\ModemCopier 2.0.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\ModemCopier.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.Description = 'ModemCopier 2.0 - Halliburton Modem Data Copy Tool'; $Shortcut.Save()"

:: Create Start Menu shortcut
echo Creating Start Menu shortcut...
set "START_MENU=%ProgramData%\Microsoft\Windows\Start Menu\Programs\Halliburton"
if not exist "%START_MENU%" mkdir "%START_MENU%"
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%START_MENU%\ModemCopier 2.0.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\ModemCopier.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.Description = 'ModemCopier 2.0 - Halliburton Modem Data Copy Tool'; $Shortcut.Save()"

echo.
echo ============================================
echo Installation completed successfully!
echo ============================================
echo.
echo Application installed to: %INSTALL_DIR%
echo Desktop shortcut created
echo Start Menu shortcut created
echo.
pause
