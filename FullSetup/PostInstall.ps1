<#
.SYNOPSIS
    Post-installation setup for Halliburton Automation Hub.
    Called by Setup-Services.cmd after MSI file copy.

.DESCRIPTION
    - Adds the bundled Node.js directory to the user's PATH
    - Registers a Task Scheduler task for auto-start on logon
    - Creates OneDrive MorningReports directories
    - Starts all PM2 services immediately
#>
param(
    [string]$InstallDir = $PSScriptRoot
)

$ErrorActionPreference = 'SilentlyContinue'

# Normalise path (remove trailing backslash)
$InstallDir = $InstallDir.TrimEnd('\')
$nodeDir    = Join-Path $InstallDir "node"
$pm2Cmd     = Join-Path $nodeDir "pm2.cmd"
$ecoConfig  = Join-Path $InstallDir "ecosystem.config.js"

Write-Host ""
Write-Host "  Halliburton Automation Hub — Post-Install Setup" -ForegroundColor Cyan
Write-Host "  ================================================" -ForegroundColor Cyan
Write-Host ""

# ── 1. Add bundled Node.js to user PATH ──────────────────────────────────────
Write-Host "  [1/4] Adding Node.js to user PATH..." -ForegroundColor Yellow

$currentPath = [Environment]::GetEnvironmentVariable('Path', 'User')
if ($currentPath -and $currentPath.ToLower().Contains($nodeDir.ToLower())) {
    Write-Host "        Already in PATH" -ForegroundColor Gray
} else {
    $newPath = if ($currentPath) { "$nodeDir;$currentPath" } else { $nodeDir }
    [Environment]::SetEnvironmentVariable('Path', $newPath, 'User')
    Write-Host "        Added: $nodeDir" -ForegroundColor Green
}

# Also set for the current session
$env:Path = "$nodeDir;$env:Path"
$env:npm_config_prefix = $nodeDir

# ── 2. Register Task Scheduler auto-start ────────────────────────────────────
Write-Host "  [2/4] Registering Task Scheduler auto-start..." -ForegroundColor Yellow

$taskName  = "Halliburton-AutomationHub"
$autoStart = Join-Path $InstallDir "auto-start.ps1"

# Remove existing task if any
schtasks /delete /tn $taskName /f 2>$null | Out-Null

$action  = "powershell.exe"
$argStr  = "-WindowStyle Hidden -ExecutionPolicy Bypass -NoProfile -File `"$autoStart`""
schtasks /create /tn $taskName /tr "$action $argStr" /sc onlogon /f /rl limited 2>$null | Out-Null

if ($?) {
    Write-Host "        Task '$taskName' registered (runs at logon)" -ForegroundColor Green
} else {
    Write-Host "        WARN: Could not register Task Scheduler task" -ForegroundColor Yellow
    Write-Host "        You can start services manually with: start.ps1" -ForegroundColor Yellow
}

# ── 3. Create OneDrive MorningReports directories ────────────────────────────
Write-Host "  [3/4] Creating MorningReports directories..." -ForegroundColor Yellow

$morningReportBase = Join-Path $env:USERPROFILE "OneDrive - Halliburton\Documents\MorningReports"
$dirs = @(
    (Join-Path $morningReportBase "incoming"),
    (Join-Path $morningReportBase "processed"),
    (Join-Path $morningReportBase "skipped")
)

foreach ($d in $dirs) {
    if (-not (Test-Path $d)) {
        New-Item $d -ItemType Directory -Force | Out-Null
        Write-Host "        Created: $d" -ForegroundColor Green
    }
}

# ── 4. Start PM2 services ───────────────────────────────────────────────────
Write-Host "  [4/4] Starting PM2 services..." -ForegroundColor Yellow

if (Test-Path $pm2Cmd) {
    Push-Location $InstallDir
    & $pm2Cmd start $ecoConfig 2>$null | Out-Null
    & $pm2Cmd save 2>$null | Out-Null
    Pop-Location
    Write-Host "        Services started via PM2" -ForegroundColor Green
} else {
    Write-Host "        WARN: pm2.cmd not found at $pm2Cmd" -ForegroundColor Yellow
}

# ── Done ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "  ✓ Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "    Automation Hub:  http://localhost:8080" -ForegroundColor White
Write-Host "    Kabal Scraper:   http://localhost:3000" -ForegroundColor White
Write-Host "    Morning Report:  http://localhost:3001" -ForegroundColor White
Write-Host ""
Write-Host "  Press any key to close..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
