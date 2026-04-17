# Halliburton Automation Hub — Manual Startup Script
# Starts all services via PM2 (no Docker, no visible CMD windows)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

# ── Detect and add bundled Node.js to PATH if present ─────────────────────
$nodeDir = Join-Path $scriptDir "node"
if (Test-Path (Join-Path $nodeDir "node.exe")) {
    $env:Path = "$nodeDir;$env:Path"
    $env:npm_config_prefix = $nodeDir
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Halliburton Automation Hub - Startup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ── 1. Check Node.js & PM2 ──────────────────────────────────────────
Write-Host "[1/3] Checking Node.js & PM2..." -ForegroundColor Yellow
$nodeVersion = node --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Node.js not found. Please install Node.js or run the MSI installer." -ForegroundColor Red
    exit 1
}
Write-Host "Node.js version: $nodeVersion" -ForegroundColor Green

$pm2Version = pm2 --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "PM2 not found. Installing..." -ForegroundColor Yellow
    npm install -g pm2
}
Write-Host ""

# ── 2. Install dependencies if needed ───────────────────────────────
if (-not (Test-Path "scraper-local\node_modules")) {
    Write-Host "[Installing scraper dependencies...]" -ForegroundColor Yellow
    Push-Location scraper-local
    npm config set strict-ssl false
    npm install
    npm config set strict-ssl true
    Pop-Location
    Write-Host "Dependencies installed" -ForegroundColor Green
    Write-Host ""
}

if (-not (Test-Path "morning-report\node_modules")) {
    Write-Host "[Installing morning-report dependencies...]" -ForegroundColor Yellow
    Push-Location morning-report
    npm config set strict-ssl false
    npm install
    npm config set strict-ssl true
    Pop-Location
    Write-Host "Dependencies installed" -ForegroundColor Green
    Write-Host ""
}

# ── 3. Start all services via PM2 ───────────────────────────────────
Write-Host "[2/3] Starting services via PM2 (background)..." -ForegroundColor Yellow
pm2 delete all 2>$null  # clean slate — ignore errors if nothing running
pm2 start ecosystem.config.js
pm2 save
Write-Host ""

# ── Status ───────────────────────────────────────────────────────────
Write-Host "[3/3] Service status:" -ForegroundColor Yellow
pm2 list
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  All services are running!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Automation Hub:    http://localhost:8080" -ForegroundColor White
Write-Host "  Kabal Scraper:     http://localhost:3000" -ForegroundColor White
Write-Host "  Morning Report:    http://localhost:3001" -ForegroundColor White
Write-Host ""
Write-Host "Useful PM2 commands:" -ForegroundColor Yellow
Write-Host "  pm2 list          - Show running services" -ForegroundColor White
Write-Host "  pm2 logs          - View live logs" -ForegroundColor White
Write-Host "  pm2 restart all   - Restart all services" -ForegroundColor White
Write-Host "  pm2 stop all      - Stop all services" -ForegroundColor White
Write-Host ""
