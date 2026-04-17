@echo off
:: Halliburton Automation Hub — Pre-Uninstall Cleanup
:: Called by MSI before removing files.
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0PreUninstall.ps1" "%~dp0"
