# ModemToolbarIE

Modem Copier / TimePlanner tooling for modem creation, date shifting, and Kabal scraping (WinForms + local web interface).

## Dependencies

### Required runtime

- Windows 10/11
- .NET Framework 4.8 runtime
- Microsoft Edge (Chromium)
- Edge WebDriver (`msedgedriver.exe`) compatible with installed Edge major version

### Required build tooling

- Visual Studio 2022 (or MSBuild with .NET desktop build tools)
- NuGet package restore enabled (`packages.config` style)

### Core NuGet dependencies

`ModemMergerWinFormsApp/packages.config`

- HtmlAgilityPack 1.11.43
- Newtonsoft.Json 13.0.1
- Microsoft.ClearScript 7.3.0 (+ Core, V8)
- Microsoft.Edge.SeleniumTools 3.141.2
- Selenium.WebDriver 3.141.0

`ModemWebUtility/packages.config`

- Costura.Fody 5.7.0 (dev dependency)
- Fody 6.6.3 (dev dependency)
- HtmlAgilityPack 1.11.43
- Newtonsoft.Json 13.0.1
- Microsoft.ClearScript 7.3.0 family
- .NET Standard compatibility packages (`System.*`, `NETStandard.Library`, etc.)

For full package list and exact versions, see:

- `ModemMergerWinFormsApp/packages.config`
- `ModemWebUtility/packages.config`

### Selenium / EdgeDriver notes

The scraper uses Selenium with Edge (`Microsoft.Edge.SeleniumTools`).
`KabalScraperClient` looks for `msedgedriver.exe` in these locations:

1. App directory
2. Current working directory
3. `%USERPROFILE%\.cache\selenium\msedgedriver\win64\...`
4. `%APPDATA%\ModemMerger\drivers\msedgedriver\...`

If no compatible driver is found, it attempts automatic download. Internet-restricted environments should pre-provision the driver.

### Node.js

Node.js is optional for this repository itself. The primary scraper logic in this repo is .NET + Selenium.

Node may still be required for external helper scripts/workflows in sibling folders (outside this repo), e.g. `scraper-local/`.

## Build

From repo root:

```powershell
"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" ".\ModemToolbarIE.sln" /t:Build /p:Configuration=Release /p:Platform="Any CPU"
```
