<#
.SYNOPSIS
    Pre-uninstall cleanup for Halliburton Automation Hub.
    Called by Remove-Services.cmd before MSI removes files.

.DESCRIPTION
    - Stops all PM2 processes
    - Removes Task Scheduler auto-start task
    - Removes bundled Node.js from user PATH
#>
param(
    [string]$InstallDir = $PSScriptRoot
)

$ErrorActionPreference = 'SilentlyContinue'

$InstallDir = $InstallDir.TrimEnd('\')
$nodeDir    = Join-Path $InstallDir "node"
$pm2Cmd     = Join-Path $nodeDir "pm2.cmd"

# ── 1. Stop PM2 services ────────────────────────────────────────────────────
if (Test-Path $pm2Cmd) {
    $env:Path = "$nodeDir;$env:Path"
    & $pm2Cmd stop all 2>$null | Out-Null
    & $pm2Cmd delete all 2>$null | Out-Null
    & $pm2Cmd kill 2>$null | Out-Null
}

# Also kill any lingering node/pm2 processes from our install dir
Get-Process -Name "node" -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -and $_.Path.StartsWith($InstallDir) } |
    Stop-Process -Force -ErrorAction SilentlyContinue

# ── 2. Remove Task Scheduler task ───────────────────────────────────────────
schtasks /delete /tn "Halliburton-AutomationHub" /f 2>$null | Out-Null

# ── 3. Remove bundled Node.js from user PATH ────────────────────────────────
$currentPath = [Environment]::GetEnvironmentVariable('Path', 'User')
if ($currentPath) {
    $parts = $currentPath.Split(';') | Where-Object {
        $_.TrimEnd('\').ToLower() -ne $nodeDir.ToLower()
    }
    $newPath = ($parts | Where-Object { $_ }) -join ';'
    [Environment]::SetEnvironmentVariable('Path', $newPath, 'User')
}

# ── 4. Clean PM2 data directory ─────────────────────────────────────────────
$pm2Home = Join-Path $env:USERPROFILE ".pm2"
if (Test-Path $pm2Home) {
    Remove-Item $pm2Home -Recurse -Force -ErrorAction SilentlyContinue
}
