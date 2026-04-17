# Halliburton Automation Hub — Auto-Start Script
# Triggered by Windows Task Scheduler at logon — no visible window
# All services run via PM2 (no Docker required)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

$logFile = Join-Path $scriptDir "auto-start.log"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

function Log($msg) {
    "$timestamp  $msg" | Out-File -Append -FilePath $logFile
}

Log "=== Auto-start triggered ==="

# ── Add bundled Node.js to PATH ──────────────────────────────────────
$nodeDir = Join-Path $scriptDir "node"
if (Test-Path (Join-Path $nodeDir "node.exe")) {
    $env:Path = "$nodeDir;$env:Path"
    $env:npm_config_prefix = $nodeDir
    Log "Using bundled Node.js: $nodeDir"
}

# ── Install scraper deps if needed ───────────────────────────────────
if (-not (Test-Path "scraper-local\node_modules")) {
    Log "Installing scraper dependencies..."
    Push-Location scraper-local
    npm config set strict-ssl false
    npm install 2>&1 | Out-File -Append -FilePath $logFile
    npm config set strict-ssl true
    Pop-Location
}

if (-not (Test-Path "morning-report\node_modules")) {
    Log "Installing morning-report dependencies..."
    Push-Location morning-report
    npm config set strict-ssl false
    npm install 2>&1 | Out-File -Append -FilePath $logFile
    npm config set strict-ssl true
    Pop-Location
}

# ── Resurrect PM2 processes ──────────────────────────────────────────
pm2 resurrect 2>&1 | Out-File -Append -FilePath $logFile
if ($LASTEXITCODE -ne 0) {
    Log "PM2 resurrect failed — starting fresh from ecosystem.config.js"
    pm2 start ecosystem.config.js 2>&1 | Out-File -Append -FilePath $logFile
    pm2 save 2>&1 | Out-File -Append -FilePath $logFile
}

Log "Auto-start complete"
