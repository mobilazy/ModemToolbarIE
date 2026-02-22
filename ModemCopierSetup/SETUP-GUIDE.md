# ModemCopier 2.0 - Setup Guide

## Current Status
✅ Application renamed from "ModemMergerWinFormsApp" to "ModemCopier 2.0"
✅ Deployment package created with installer scripts
✅ Release build completed

## Quick Deployment (No WiX Required)

### Location
`ModemCopierSetup\DeploymentPackage\`

### Contents
- **Install.bat** - Installs ModemCopier 2.0 to Program Files
- **Uninstall.bat** - Removes ModemCopier 2.0
- **README.txt** - Installation instructions
- **Release\** - Application binaries (renamed executable to ModemCopier.exe)

### Installation Steps
1. Navigate to `ModemCopierSetup\DeploymentPackage\`
2. Right-click `Install.bat`
3. Select "Run as administrator"
4. Application installs to: `C:\Program Files\Halliburton\ModemCopier 2.0`
5. Desktop and Start Menu shortcuts created

### Distribution
To distribute to users:
1. Zip the entire `DeploymentPackage` folder
2. Send zip file to users
3. Users extract and run Install.bat as administrator

## Professional MSI Installer (WiX Toolset - Optional)

If you want a professional .msi installer instead of batch scripts:

### Prerequisites
Download and install WiX Toolset v3.11 or later:
https://wixtoolset.org/releases/

### Build MSI
1. Install WiX Toolset
2. Open Visual Studio
3. Add `ModemCopierSetup\ModemCopierSetup.wixproj` to solution
4. Build the project
5. MSI file will be in `ModemCopierSetup\bin\Release\ModemCopier-Setup.msi`

### WiX Project Files Created
- `Product.wxs` - Installer definition
- `ModemCopierSetup.wixproj` - WiX project file
- `License.rtf` - License agreement

### MSI Features
- ✅ Professional Windows Installer (.msi)
- ✅ GUI installation wizard
- ✅ Add/Remove Programs integration
- ✅ Desktop shortcut option
- ✅ Start Menu shortcuts
- ✅ Proper uninstall support
- ✅ Version upgrade handling

## Application Details

### Product Name
ModemCopier 2.0

### Company
Halliburton

### Version
2.0.0.0

### Executable
ModemCopier.exe (renamed from ModemMergerWinFormsApp.exe)

### Install Location
`C:\Program Files\Halliburton\ModemCopier 2.0\`

### Dependencies Included
- ClearScript.Core.dll
- ClearScript.V8.dll
- ClearScriptV8.win-x64.dll
- ClearScriptV8.win-x86.dll
- HtmlAgilityPack.dll
- Newtonsoft.Json.dll
- ModemWebUtility.dll

## Files Modified
1. `ModemMergerWinFormsApp\Properties\AssemblyInfo.cs`
   - AssemblyTitle: "ModemCopier 2.0"
   - AssemblyProduct: "ModemCopier 2.0"
   - AssemblyCompany: "Halliburton"
   - Version: 2.0.0.0

## Next Steps

### For Immediate Use
Run `Install.bat` from the DeploymentPackage folder (as admin)

### For Distribution
Create a zip file of DeploymentPackage and share with users

### For Professional Installer
Install WiX Toolset and build the MSI project

## Notes
- Requires .NET Framework 4.8
- Requires Windows 7 or later
- Installation requires administrator privileges
- Executable is renamed to ModemCopier.exe during installation
