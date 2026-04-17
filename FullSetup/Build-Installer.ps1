<#
.SYNOPSIS
    Builds the Halliburton Automation Hub MSI installer.

.DESCRIPTION
    One-stop build script that:
      1. Builds the ModemCopier .NET app (Release)
      2. Extracts bundled Node.js 20 and pre-installs PM2
      3. Stages all automation files
      4. Harvests directories with heat.exe
      5. Compiles + links with candle.exe / light.exe
      6. Outputs: bin\HalliburtonAutomationHub-Setup.msi

.PARAMETER SkipDotNetBuild
    Skip the .NET MSBuild step (use existing Release build)

.PARAMETER SourceRoot
    Path to the automation source files (hub-server.js, scraper-local/, etc.)
    Defaults to ..\..\  (the workspace root, two levels above FullSetup/)

.PARAMETER NodeZip
    Path to the Node.js 20 zip archive.
    Defaults to <SourceRoot>\node20.zip
#>
[CmdletBinding()]
param(
    [switch]$SkipDotNetBuild,
    [string]$SourceRoot = (Resolve-Path "$PSScriptRoot\..\..\"),
    [string]$NodeZip = ''
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# -- Helpers ---------------------------------------------------------------
function Step([string]$msg) { Write-Host "`n=== $msg ===" -ForegroundColor Cyan }
function Info([string]$msg) { Write-Host "  $msg" -ForegroundColor Gray }
function OK([string]$msg)   { Write-Host "  [OK] $msg" -ForegroundColor Green }
function Fail([string]$msg) { Write-Host "  [FAIL] $msg" -ForegroundColor Red; exit 1 }

$staging = "$PSScriptRoot\staging"
$binDir  = "$PSScriptRoot\bin"

# -- Resolve paths ---------------------------------------------------------
$modemCopierSrc = Join-Path $PSScriptRoot "..\ModemMergerWinFormsApp"
$releaseBin     = Join-Path $modemCopierSrc "bin\Release"
$solutionFile   = Join-Path $PSScriptRoot "..\ModemToolbarIE.sln"
if (-not $NodeZip) {
    $NodeZip = Join-Path $SourceRoot "node20.zip"
    if (-not (Test-Path $NodeZip)) { $NodeZip = Join-Path $SourceRoot "node20\node20.zip" }
}

# Locate AutomationHub source in git repo, fall back to workspace root
$autoHubGit = Join-Path $PSScriptRoot "..\AutomationHub"
if (Test-Path (Join-Path $autoHubGit "hub-server.js")) {
    $SourceRoot = $autoHubGit
    Info "Using AutomationHub source from git repo: $autoHubGit"
}

# -- Validate prerequisites ------------------------------------------------
Step "Validating prerequisites"

# WiX Toolset
$wixBinDir = ''
foreach ($candidate in @(
    (Get-Command candle.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source | Split-Path),
    "${env:WIX}bin",
    "C:\Program Files (x86)\WiX Toolset v3.14\bin",
    "C:\Program Files (x86)\WiX Toolset v3.11\bin"
)) {
    if ($candidate -and (Test-Path (Join-Path $candidate "candle.exe"))) {
        $wixBinDir = $candidate; break
    }
}
if (-not $wixBinDir) { Fail "WiX Toolset not found. Install from https://wixtoolset.org/releases/" }
OK "WiX Toolset: $wixBinDir"

$candle = Join-Path $wixBinDir "candle.exe"
$light  = Join-Path $wixBinDir "light.exe"
$heat   = Join-Path $wixBinDir "heat.exe"

if (-not (Test-Path $NodeZip)) { Fail "Node.js zip not found: $NodeZip" }
OK "Node.js zip: $NodeZip"

$requiredFiles = @(
    (Join-Path $SourceRoot "hub-server.js"),
    (Join-Path $SourceRoot "ecosystem.config.js"),
    (Join-Path $SourceRoot "process-code.js"),
    (Join-Path $SourceRoot "scraper-local\kabal-scraper.js"),
    (Join-Path $SourceRoot "morning-report\server.js"),
    (Join-Path $SourceRoot "workflows\index.html")
)
foreach ($f in $requiredFiles) {
    if (-not (Test-Path $f)) { Fail "Required source file not found: $f" }
}
OK "All automation source files found"

# =========================================================================
# STEP 1: Build .NET app
# =========================================================================
if (-not $SkipDotNetBuild) {
    Step "Building ModemCopier .NET app (Release)"

    # Try MSBuild from VS or .NET Framework
    $msbuild = ''
    foreach ($candidate in @(
        (Get-Command msbuild.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source),
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe",
        "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    )) {
        if ($candidate -and (Test-Path $candidate)) { $msbuild = $candidate; break }
    }

    if (-not $msbuild) {
        Write-Host "  MSBuild not found -- using existing Release build" -ForegroundColor Yellow
    } else {
        Info "MSBuild: $msbuild"
        # Restore NuGet packages first
        $nugetExe = Join-Path $PSScriptRoot "..\nuget.exe"
        if (Test-Path $nugetExe) {
            & $nugetExe restore $solutionFile -NonInteractive | Out-Null
        }
        & $msbuild $solutionFile /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal /nologo
        if ($LASTEXITCODE -ne 0) { Fail "MSBuild failed with exit code $LASTEXITCODE" }
        OK "Build succeeded"
    }
} else {
    Step "Skipping .NET build (using existing Release output)"
}

if (-not (Test-Path (Join-Path $releaseBin "ModemMergerWinFormsApp.exe"))) {
    Fail "ModemCopier Release build not found at: $releaseBin"
}

# =========================================================================
# STEP 2: Clean and create staging directory
# =========================================================================
Step "Preparing staging directory"

if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item $staging -ItemType Directory -Force | Out-Null
New-Item "$staging\ModemCopier" -ItemType Directory -Force | Out-Null
New-Item "$staging\vba" -ItemType Directory -Force | Out-Null
New-Item "$binDir" -ItemType Directory -Force | Out-Null

# =========================================================================
# STEP 3: Stage ModemCopier .NET app
# =========================================================================
Step "Staging ModemCopier .NET app"

$dotNetFiles = @(
    'ModemMergerWinFormsApp.exe',
    'ModemMergerWinFormsApp.exe.config',
    'ModemWebUtility.dll',
    'HtmlAgilityPack.dll',
    'Newtonsoft.Json.dll',
    'ClearScript.Core.dll',
    'ClearScript.V8.dll',
    'ClearScriptV8.win-x64.dll',
    'ClearScriptV8.win-x86.dll',
    'ModemWebUtility.XmlSerializers.dll'
)

foreach ($f in $dotNetFiles) {
    $src = Join-Path $releaseBin $f
    if (Test-Path $src) {
        Copy-Item $src "$staging\ModemCopier\" -Force
    } else {
        Write-Host "  WARN: $f not found in Release build -- skipping" -ForegroundColor Yellow
    }
}

# Rename the exe to ModemCopier.exe
if (Test-Path "$staging\ModemCopier\ModemMergerWinFormsApp.exe") {
    Rename-Item "$staging\ModemCopier\ModemMergerWinFormsApp.exe" "ModemCopier.exe" -Force
}
OK "ModemCopier files staged"

# =========================================================================
# STEP 4: Extract Node.js 20 and pre-install PM2
# =========================================================================
Step "Extracting Node.js 20 runtime"

$nodeStaging = "$staging\node"
Expand-Archive -Path $NodeZip -DestinationPath "$staging\_node_temp" -Force

# The zip contains a versioned directory (e.g., node-v20.19.0-win-x64/)
# Flatten it to staging\node\
$innerDir = Get-ChildItem "$staging\_node_temp" -Directory | Select-Object -First 1
if ($innerDir) {
    Move-Item $innerDir.FullName $nodeStaging -Force
} else {
    Rename-Item "$staging\_node_temp" "node" -Force
    $nodeStaging = "$staging\node"
}
Remove-Item "$staging\_node_temp" -Recurse -Force -ErrorAction SilentlyContinue
OK "Node.js extracted to staging\node\"

# Pre-install PM2 globally into the bundled node directory
Step "Pre-installing PM2 into bundled Node.js"

$env:npm_config_prefix = $nodeStaging
$npmCmd = Join-Path $nodeStaging "npm.cmd"
if (-not (Test-Path $npmCmd)) { Fail "npm.cmd not found in extracted Node.js at $nodeStaging" }

& $npmCmd install -g pm2 --prefer-offline 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Info "Retrying PM2 install without --prefer-offline..."
    & $npmCmd install -g pm2 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) { Fail "Failed to install PM2. Check network or npm cache." }
}

if (Test-Path (Join-Path $nodeStaging "pm2.cmd")) {
    OK "PM2 pre-installed: $(Join-Path $nodeStaging 'pm2.cmd')"
} else {
    Fail "PM2 installation did not create pm2.cmd"
}

# =========================================================================
# STEP 5: Stage automation files
# =========================================================================
Step "Staging automation source files"

# Top-level service files
foreach ($f in @('hub-server.js', 'ecosystem.config.js', 'process-code.js')) {
    Copy-Item (Join-Path $SourceRoot $f) "$staging\" -Force
}

# Start/auto-start scripts
foreach ($f in @('start.ps1', 'auto-start.ps1')) {
    $src = Join-Path $SourceRoot $f
    if (Test-Path $src) { Copy-Item $src "$staging\" -Force }
}

# scraper-local (full directory with node_modules)
Copy-Item (Join-Path $SourceRoot "scraper-local") "$staging\scraper-local" -Recurse -Force

# morning-report (full directory with node_modules)
Copy-Item (Join-Path $SourceRoot "morning-report") "$staging\morning-report" -Recurse -Force

# workflows (all HTML, JSON, backup files)
Copy-Item (Join-Path $SourceRoot "workflows") "$staging\workflows" -Recurse -Force
# Remove archive subfolder and backup files from staging
Remove-Item "$staging\workflows\archive" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$staging\workflows\*.backup" -Force -ErrorAction SilentlyContinue

# VBA macro
$vbaSrc = Join-Path $SourceRoot "workflows\outlook-morning-report.bas"
if (Test-Path $vbaSrc) { Copy-Item $vbaSrc "$staging\vba\" -Force }

OK "Automation files staged"

# Stage setup/teardown scripts (from FullSetup directory)
Copy-Item "$PSScriptRoot\PostInstall.ps1"     "$staging\" -Force
Copy-Item "$PSScriptRoot\PreUninstall.ps1"    "$staging\" -Force
Copy-Item "$PSScriptRoot\Setup-Services.cmd"  "$staging\" -Force
Copy-Item "$PSScriptRoot\Remove-Services.cmd" "$staging\" -Force
OK "Setup scripts staged"

# =========================================================================
# STEP 6: Harvest directories with heat.exe
# =========================================================================
Step "Harvesting directories with heat.exe"

$harvests = @(
    @{ Dir = "$staging\ModemCopier";    CG = "ModemCopierFiles";    DR = "MODEMCOPIERDIR";    Var = "var.ModemCopierSource";    Out = "ModemCopier-harvested.wxs" },
    @{ Dir = "$staging\node";           CG = "NodeRuntime";         DR = "NODEDIR";           Var = "var.NodeSource";           Out = "NodeRuntime-harvested.wxs" },
    @{ Dir = "$staging\scraper-local";  CG = "ScraperLocalFiles";   DR = "SCRAPERLOCALDIR";   Var = "var.ScraperSource";        Out = "ScraperLocal-harvested.wxs" },
    @{ Dir = "$staging\morning-report"; CG = "MorningReportFiles";  DR = "MORNINGREPORTDIR";  Var = "var.MorningReportSource";  Out = "MorningReport-harvested.wxs" },
    @{ Dir = "$staging\workflows";      CG = "WorkflowFiles";       DR = "WORKFLOWSDIR";      Var = "var.WorkflowsSource";      Out = "Workflows-harvested.wxs" }
)

foreach ($h in $harvests) {
    Info "Harvesting $($h.Dir) -> $($h.Out)"
    & $heat dir $h.Dir `
        -cg $h.CG `
        -dr $h.DR `
        -srd -ag -sfrag `
        -var $h.Var `
        -out (Join-Path $PSScriptRoot $h.Out) `
        -nologo
    if ($LASTEXITCODE -ne 0) { Fail "heat.exe failed for $($h.Dir)" }
}
OK "All directories harvested"

# =========================================================================
# STEP 7: Compile WiX sources (candle.exe)
# =========================================================================
Step "Compiling WiX sources (candle.exe)"

$wxsFiles = @("Product.wxs") + ($harvests | ForEach-Object { $_.Out })
$defineArgs = @(
    "-dStagingDir=$staging",
    "-dModemCopierSource=$staging\ModemCopier",
    "-dNodeSource=$staging\node",
    "-dScraperSource=$staging\scraper-local",
    "-dMorningReportSource=$staging\morning-report",
    "-dWorkflowsSource=$staging\workflows"
)

$candleArgs = $defineArgs + @("-nologo", "-ext", "WixUIExtension", "-ext", "WixUtilExtension") + $wxsFiles
& $candle @candleArgs
if ($LASTEXITCODE -ne 0) { Fail "candle.exe failed" }
OK "Compilation succeeded"

# =========================================================================
# STEP 8: Link into MSI (light.exe)
# =========================================================================
Step "Linking MSI (light.exe)"

$wixobjFiles = $wxsFiles | ForEach-Object { [IO.Path]::ChangeExtension($_, '.wixobj') }
$msiPath = Join-Path $binDir "HalliburtonAutomationHub-Setup.msi"

& $light -nologo `
    -ext WixUIExtension -ext WixUtilExtension `
    -spdb `
    -out $msiPath `
    @wixobjFiles
if ($LASTEXITCODE -ne 0) { Fail "light.exe failed" }

# =========================================================================
# DONE
# =========================================================================
$msiSize = [math]::Round((Get-Item $msiPath).Length / 1MB, 1)
Write-Host ""
Write-Host "========================================================" -ForegroundColor Green
Write-Host "  MSI built successfully!" -ForegroundColor Green
Write-Host "  $msiPath  ($msiSize MB)" -ForegroundColor White
Write-Host "========================================================" -ForegroundColor Green
