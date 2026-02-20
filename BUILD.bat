@echo off
echo ========================================
echo  Modem File Manager - Build Script
echo ========================================
echo.
echo This script will build your application.
echo Please wait...
echo.

REM Change to the directory where this script is located
cd /d "%~dp0"

REM Try to find Visual Studio and MSBuild
set "MSBUILD_PATH="

if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
)

if "%MSBUILD_PATH%"=="" (
    echo ERROR: Could not find MSBuild.exe
    echo.
    echo Please make sure Visual Studio is installed.
    echo Or use Visual Studio to build the project instead.
    echo See BUILD_INSTRUCTIONS.md for details.
    echo.
    pause
    exit /b 1
)

echo Found MSBuild at: %MSBUILD_PATH%
echo.

REM Build ModemWebUtility first
echo ========================================
echo Building ModemWebUtility...
echo ========================================
"%MSBUILD_PATH%" "ModemWebUtility\ModemWebUtility.csproj" /p:Configuration=Release /t:Build
if errorlevel 1 (
    echo.
    echo ERROR: Failed to build ModemWebUtility
    pause
    exit /b 1
)
echo ModemWebUtility built successfully!
echo.

REM Build ModemMergerWinFormsApp
echo ========================================
echo Building ModemMergerWinFormsApp...
echo ========================================
"%MSBUILD_PATH%" "ModemMergerWinFormsApp\ModemMergerWinFormsApp.csproj" /p:Configuration=Release /t:Build
if errorlevel 1 (
    echo.
    echo ERROR: Failed to build ModemMergerWinFormsApp
    pause
    exit /b 1
)
echo ModemMergerWinFormsApp built successfully!
echo.

echo ========================================
echo BUILD COMPLETED SUCCESSFULLY!
echo ========================================
echo.
echo Your application is ready at:
echo ModemMergerWinFormsApp\bin\Release\net6.0-windows\ModemMergerWinFormsApp.exe
echo.
echo You can now run the application!
echo.

REM Ask if user wants to open the output folder
set /p OPEN_FOLDER="Do you want to open the output folder? (Y/N): "
if /i "%OPEN_FOLDER%"=="Y" (
    start "" "ModemMergerWinFormsApp\bin\Release\net6.0-windows"
)

echo.
echo Press any key to exit...
pause > nul
