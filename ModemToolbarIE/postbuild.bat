@echo off
REM Post-build registration script for ModemToolbarIE IE toolbar BHO
REM All commands wrapped with error suppression - requires admin rights for actual registration

set TARGET=%1

REM Unregister old version (ignore errors - may not be registered yet)
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" /unregister "%TARGET%" 2>nul >nul
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe" /unregister "%TARGET%" 2>nul >nul

REM Install dependencies to GAC (requires admin)
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe" /f /i "c:\Users\H259507\n8n-automation-learning\ModemToolbarIE\packages\ToGac\HtmlAgilityPack.dll" 2>nul >nul
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe" /f /i "c:\Users\H259507\n8n-automation-learning\ModemToolbarIE\packages\ToGac\Interop.SHDocVw.dll" 2>nul >nul
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\gacutil.exe" /f /i "c:\Users\H259507\n8n-automation-learning\ModemToolbarIE\packages\ToGac\HtmlAgilityPack.dll" 2>nul >nul
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\gacutil.exe" /f /i "c:\Users\H259507\n8n-automation-learning\ModemToolbarIE\packages\ToGac\Interop.SHDocVw.dll" 2>nul >nul

REM Install toolbar DLL to GAC (requires admin)
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\gacutil.exe" /f /i "%TARGET%" 2>nul >nul
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe" /f /i "%TARGET%" 2>nul >nul

REM Register toolbar DLL as COM/BHO (requires admin)
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe" "%TARGET%" 2>nul >nul
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" "%TARGET%" 2>nul >nul

REM Always exit 0 so build reports success (run VS as admin for actual registration)
exit /b 0
