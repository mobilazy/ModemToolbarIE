@echo off
setlocal enabledelayedexpansion

echo ============================================
echo ModemCopier 2.0 - Build Deployment Package
echo ============================================
echo.

set "SOURCE_DIR=..\ModemMergerWinFormsApp\bin\Release"
set "DEPLOY_DIR=.\DeploymentPackage"
set "RELEASE_DIR=.\DeploymentPackage\Release"

:: Clean and create deployment directory
if exist "%DEPLOY_DIR%" (
    echo Cleaning existing deployment package...
    rmdir /S /Q "%DEPLOY_DIR%"
)

echo Creating deployment package structure...
mkdir "%DEPLOY_DIR%"
mkdir "%RELEASE_DIR%"

:: Copy application files
echo Copying application files from Release build...
xcopy /Y /Q "%SOURCE_DIR%\*.exe" "%RELEASE_DIR%\"
xcopy /Y /Q "%SOURCE_DIR%\*.dll" "%RELEASE_DIR%\"
xcopy /Y /Q "%SOURCE_DIR%\*.config" "%RELEASE_DIR%\"

:: Copy installer scripts
echo Copying installer scripts...
copy /Y "Install.bat" "%DEPLOY_DIR%\"
copy /Y "Uninstall.bat" "%DEPLOY_DIR%\"

:: Create README
echo Creating README...
(
echo ModemCopier 2.0 - Deployment Package
echo =====================================
echo.
echo INSTALLATION INSTRUCTIONS:
echo 1. Right-click on Install.bat
echo 2. Select "Run as administrator"
echo 3. Follow the on-screen instructions
echo.
echo The application will be installed to:
echo C:\Program Files\Halliburton\ModemCopier 2.0
echo.
echo Desktop and Start Menu shortcuts will be created.
echo.
echo UNINSTALLATION:
echo 1. Right-click on Uninstall.bat
echo 2. Select "Run as administrator"
echo.
echo SYSTEM REQUIREMENTS:
echo - Windows 7 or later
echo - .NET Framework 4.8 or later
echo - Administrator privileges for installation
echo.
echo Copyright 2026 Halliburton
) > "%DEPLOY_DIR%\README.txt"

echo.
echo ============================================
echo Deployment package created successfully!
echo ============================================
echo.
echo Location: %DEPLOY_DIR%
echo.
echo You can now:
echo 1. Zip the DeploymentPackage folder for distribution
echo 2. Or create a self-extracting archive
echo.
pause
