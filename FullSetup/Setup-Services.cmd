@echo off
:: Halliburton Automation Hub — Post-Install Service Configuration
:: This script is launched from the MSI Finish dialog or can be run manually.
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0PostInstall.ps1" "%~dp0"
