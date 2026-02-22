# ModemCopier 2.0

A Windows application for copying GP BHA, DD BHA, and Loose BHA data between different modems in Halliburton's Oracle system.

## Features

- ✅ Copy GP (Geo-Pilot) BHA configurations between modems
- ✅ Copy DD (Directional Drilling) BHA configurations
- ✅ Copy Loose BHA items
- ✅ Cross-modem compatibility with automatic field adjustments
- ✅ Automatic field truncation to prevent Oracle buffer errors
- ✅ List of Values (LOV) ID updating for target modem compatibility
- ✅ Optional "Main" text renaming in descriptions

## System Requirements

- Windows 10/11
- .NET Framework 4.8
- Visual Studio 2022 (for building from source)

## Quick Start

### Option 1: Use Pre-built Application

1. Navigate to:
   ```
   ModemMergerWinFormsApp\bin\Release\
   ```
2. Double-click `ModemMergerWinFormsApp.exe`

### Option 2: Use Deployment Package

1. Navigate to:
   ```
   ModemCopierSetup\DeploymentPackage\
   ```
2. Double-click `Install.bat` to install
3. Use `Uninstall.bat` to remove

See [ModemCopierSetup/SETUP-GUIDE.md](ModemCopierSetup/SETUP-GUIDE.md) for detailed installation instructions.

## Building from Source

### Using Visual Studio

1. Open `ModemCopier.sln` in Visual Studio 2022
2. Press `Ctrl+Shift+B` or **Build** → **Build Solution**
3. Find the executable at:
   ```
   ModemMergerWinFormsApp\bin\Debug\ModemMergerWinFormsApp.exe
   ```

### Using MSBuild (Command Line)

```powershell
# Debug build
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\amd64\MSBuild.exe" ModemCopier.sln /p:Configuration=Debug

# Release build
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\amd64\MSBuild.exe" ModemCopier.sln /p:Configuration=Release
```

## Project Structure

```
ModemCopier/
├── ModemMergerWinFormsApp/    # Main application (Windows Forms)
├── ModemWebUtility/           # Core library (BHA logic, Oracle API)
├── ModemCopierSetup/          # Deployment packages (batch + WiX)
└── packages/                  # NuGet dependencies
```

## How It Works

1. **Connect** to Oracle system (automatic Windows Authentication)
2. **Load** source modem GP/DD/Loose BHA data from Oracle views
3. **Copy** selected items to target modem
4. **Adjust** fields automatically:
   - Updates all O_ fields (List of Values IDs) from target modem
   - Clears H_GP_COMPL_WARN completion warnings
   - Truncates long text fields to prevent Oracle errors
   - Preserves P_ descriptive fields from source

## Technical Details

### GP BHA Copying Process

1. Calls `InsertDefaultGP?p_ssord_id=TARGET_MODEM` to create empty GP
2. Loads `QueryViewByKey?P_GP_ID=NEW_GP_ID` to get target modem's valid LOV IDs
3. POSTs to `gp_mc.actionview` with Z_ACTION=UPDATE (header data)
4. POSTs to `gpitm_mc.actioninsert` with Z_ACTION=INSERT (component items)

### Field Mapping

- **P_ fields**: Descriptive data (copied from source)
- **O_ fields**: LOV IDs (updated from target modem - critical!)
- **H_ fields**: HTML elements (updated from target)
- **Z_ fields**: Action parameters (UPDATE for header)

### Oracle API Pattern

The application uses reverse-engineered Oracle Forms HTTP POST patterns with proper:
- Z_ACTION values (UPDATE for existing, INSERT for new)
- Z_CHK checksums
- Duplicate empty field submissions (required by Oracle)
- Session cookie management

## Known Limitations

- Field length limits enforced (auto-truncated):
  - P_GP_DESC: 50 chars
  - P_GP_COMMENT: 1000 chars
  - P_BIT_TYPE: 100 chars
- Requires Windows Authentication to Oracle system
- Same-session copying only (no cross-session persistence)

## Troubleshooting

**Oracle Error "buffer too small"**
- Fixed automatically with field truncation
- Ensure you're running the latest version

**Components not created**
- Verify Oracle returned 302 redirect (not 200 OK)
- Check if target modem has valid LOV IDs

**Empty fields after copy**
- Verify O_ fields are being updated from target modem
- Check Oracle session hasn't expired

## Version History

### v2.0.0 (Current)
- Renamed from "Merge from other modems" to "ModemCopier 2.0"
- Fixed cross-modem GP BHA copying
- Added field truncation for Oracle compatibility
- Updated ALL O_ fields from target modem
- Removed debug popups
- Created deployment packages

## License

Internal Halliburton tool - not for public distribution

## Author

Daulet Karakhanov (Halliburton)
