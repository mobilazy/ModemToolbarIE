using HtmlAgilityPack;
using ModemWebUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ModemMergerWinFormsApp
{
    // ── Scraper Log ────────────────────────────────────────────────────

    /// <summary>
    /// Minimal file logger for the Kabal scraper. Writes to
    /// %APPDATA%\ModemMerger\scraper-log.txt (last run only).
    /// </summary>
    public static class ScrapeLog
    {
        private static readonly object _lock = new object();
        private static string _logDir;
        private static string _logPath;

        public static string LogPath => _logPath;

        public static void Start()
        {
            _logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ModemMerger");
            Directory.CreateDirectory(_logDir);
            _logPath = Path.Combine(_logDir, "scraper-log.txt");
            File.WriteAllText(_logPath,
                $"=== Kabal Scraper Log — {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
        }

        public static void Info(string message)  => Write("INFO ", message);
        public static void Warn(string message)  => Write("WARN ", message);
        public static void Error(string message) => Write("ERROR", message);

        private static void Write(string level, string message)
        {
            if (_logPath == null) return;
            var line = $"[{DateTime.Now:HH:mm:ss}] {level}  {message}{Environment.NewLine}";
            lock (_lock) { File.AppendAllText(_logPath, line); }
        }

        /// <summary>Captures screenshot + URL on error only.</summary>
        public static void SaveErrorSnapshot(OpenQA.Selenium.IWebDriver driver, string label)
        {
            try
            {
                Error($"[{label}] URL = {driver.Url}");
                var ss = driver as OpenQA.Selenium.ITakesScreenshot;
                if (ss != null)
                {
                    var shot = ss.GetScreenshot();
                    var shotPath = Path.Combine(_logDir, label.Replace(" ", "_") + ".png");
                    shot.SaveAsFile(shotPath, OpenQA.Selenium.ScreenshotImageFormat.Png);
                    Error($"[{label}] Screenshot → {shotPath}");
                }
            }
            catch { }
        }
    }

    // ── Scraper DTOs ─────────────────────────────────────────────────────

    public class KabalModem
    {
        [JsonProperty("modemNumber")]
        public string ModemNumber { get; set; }

        [JsonProperty("shippingDate")]
        public string ShippingDate { get; set; }

        [JsonProperty("kabalId")]
        public string KabalId { get; set; }

        [JsonProperty("packageName")]
        public string PackageName { get; set; }

        [JsonProperty("supplier")]
        public string Supplier { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public class ScraperResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("scraped")]
        public int Scraped { get; set; }

        [JsonProperty("dryRun")]
        public bool DryRun { get; set; }

        [JsonProperty("modems")]
        public List<KabalModem> Modems { get; set; } = new List<KabalModem>();

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    // ── TMUtils DTOs ─────────────────────────────────────────────────────

    public class TMUtilsModem
    {
        public int Id { get; set; }
        public string WellName { get; set; }
        public string WellSection { get; set; }
        public string StartDate { get; set; }
        public string LoadoutDate { get; set; }
        public string LoadDate { get; set; }
        [JsonProperty("P_LOADOUT_DATE")]
        public string PLoadoutDate { get; set; }
        [JsonProperty("P_DATE_LOAD")]
        public string PDateLoad { get; set; }
        [JsonProperty("P_DATE_ETA")]
        public string PDateEta { get; set; }
        [JsonProperty("P_SHIPTO_4")]
        public string PShipTo4 { get; set; }
    }

    public class TMUtilsCreationInputs
    {
        public Dictionary<string, string> RigNamesRecent { get; set; }
        public List<string[]> Customers { get; set; }
    }

    public class MoveResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class GantModem
    {
        public int Id { get; set; }
        public string WellName { get; set; }     // Wellbore
        public string WellSection { get; set; }  // Section
        public string StartDate { get; set; }    // DD.MM.YYYY (from Gant)
        public string RigName { get; set; }
        public string GantText { get; set; }
        public string ModemStatus { get; set; }
        public string CustomerName { get; set; }
        public string PShipTo1 { get; set; } = "";
        public string DateLoad { get; set; } = "";     // P_DATE_LOAD from modem edit page
        public string LoadoutDate { get; set; } = "";  // P_LOADOUT_DATE from modem edit page
        public string DateEta { get; set; } = "";      // P_DATE_ETA from modem edit page
        public string KabalSyncDate { get; set; } = ""; // shippingDate from Kabal sync (same as Modem Shifter)
        public string HRefno { get; set; } = "";       // H_REFNO — contains Kabal cargo ID (e.g. LC123456)
    }

    // ── TimePlanner DTOs ─────────────────────────────────────────────────

    public class TimePlannerTask
    {
        [JsonProperty("taskName")]
        public string TaskName { get; set; }
        [JsonProperty("startDateTime")]
        public string StartDateTime { get; set; }
        [JsonProperty("endDateTime")]
        public string EndDateTime { get; set; }
        [JsonProperty("isKeyTask")]
        public bool IsKeyTask { get; set; }
    }

    public class TimePlannerSection
    {
        [JsonProperty("wellName")]
        public string WellName { get; set; }
        [JsonProperty("planName")]
        public string PlanName { get; set; }
        [JsonProperty("sectionName")]
        public string SectionName { get; set; }
        [JsonProperty("sectionSize")]
        public string SectionSize { get; set; }
        [JsonProperty("muBhaDateTime")]
        public string MuBhaDateTime { get; set; }
        [JsonProperty("poohDateTime")]
        public string PoohDateTime { get; set; }
        [JsonProperty("durationDays")]
        public double DurationDays { get; set; }
        [JsonProperty("isWhipstock")]
        public bool IsWhipstock { get; set; }
        [JsonProperty("tasks")]
        public List<TimePlannerTask> Tasks { get; set; } = new List<TimePlannerTask>();
    }

    public class TimePlannerResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("sections")]
        public List<TimePlannerSection> Sections { get; set; } = new List<TimePlannerSection>();
        [JsonProperty("recentOperations")]
        public List<TimePlannerRecentOperation> RecentOperations { get; set; } = new List<TimePlannerRecentOperation>();
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }

    public class TimePlannerRecentOperation
    {
        [JsonProperty("wellName")]
        public string WellName { get; set; }
        [JsonProperty("taskName")]
        public string TaskName { get; set; }
        [JsonProperty("taskDateTime")]
        public string TaskDateTime { get; set; }
        [JsonProperty("hoursFromNow")]
        public double HoursFromNow { get; set; }
    }

    // ── Clients ──────────────────────────────────────────────────────────

    public static class KabalScraperClient
    {
        // Login entry point — triggers SSO and lands back on the APEX app after auth
        private const string LoginUrl = "https://account01.kabal.com/w/web/r/wels/kabal-account/";

        // Per-operator timestamp of last successfully verified login session.
        // If within 30 minutes, skip the session-check navigate+wait and go straight to the Gantt URL.
        private static readonly Dictionary<string, DateTime> _lastSessionVerified =
            new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private static readonly TimeSpan _sessionCacheWindow = TimeSpan.FromMinutes(30);

        // Persistent Selenium driver reused across calls (both Sync and TimePlanner).
        // Keyed by headless flag — headless and non-headless cannot share a driver instance.
        private static OpenQA.Selenium.IWebDriver _sharedDriver;
        private static bool _sharedDriverHeadless;
        private static readonly object _driverLock = new object();

        // Persistent user-data-dir so SSO session cookies survive across scrapes
        private static readonly string _userDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ModemMerger", "edge-profile");

        // Each operator lives in a separate APEX application (different App ID).
        // session=0 tells APEX to create a fresh session from active SSO cookies.
        private static readonly Dictionary<string, string> OperatorAppUrls =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Equinor",        "https://app01.kabal.com/mws/f?p=16786:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout" },
            { "Vår Energi",     "https://app01.kabal.com/mws/f?p=1364:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout" },
            { "ConocoPhillips", "https://app01.kabal.com/mws/f?p=1267:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout" },
            { "AkerBP",         "https://app01.kabal.com/mws/f?p=1276:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout" },
        };

        // Display name used when clicking the operator selector inside the APEX app
        private static readonly Dictionary<string, string> OperatorMapping =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AkerBP",         "AkerBP" },
            { "ConocoPhillips", "ConocoPhillips - Operator (NO)" },
            { "Equinor",        "Equinor Norge - Operator (NO)" },
            { "Vår Energi",     "Vaar Energi - Operator (NO)" },
        };

        // TimePlanner Gantt page per operator (page 3101).
        // The PATH and P3101_OPERATION_TYPE_FILTER arguments are required; without them
        // Kabal often loads only the vis.js shell and the rig rows stay blank/undefined.
        private static readonly Dictionary<string, string> TimePlannerUrls =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Equinor",        "https://app01.kabal.com/mws/f?p=16786:3101:0::NO::PATH,P3101_OPERATION_TYPE_FILTER:planning.rig.timeplanner_gantt,all:" },
            { "Vår Energi",     "https://app01.kabal.com/mws/f?p=1364:3101:0::NO::PATH,P3101_OPERATION_TYPE_FILTER:planning.rig.timeplanner_gantt,all:" },
            { "ConocoPhillips", "https://app01.kabal.com/mws/f?p=1267:3101:0::NO::PATH,P3101_OPERATION_TYPE_FILTER:planning.rig.timeplanner_gantt,all:" },
            { "AkerBP",         "https://app01.kabal.com/mws/f?p=1276:3101:0::NO::PATH,P3101_OPERATION_TYPE_FILTER:planning.rig.timeplanner_gantt,all:" },
        };

        // APEX App IDs per operator — used to verify we're on the right operator after session reuse
        private static readonly Dictionary<string, string> OperatorAppIds =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Equinor",        "16786" },
            { "Vår Energi",     "1364"  },
            { "ConocoPhillips", "1267"  },
            { "AkerBP",         "1276"  },
        };

        // Kabal ID prefixes per operator. H_REFNO contains e.g. "LC123456" for AkerBP.
        private static readonly Dictionary<string, string> KabalIdPrefixes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AkerBP",         "LC" },
            { "ConocoPhillips", "CONOCOPHILLIPS" },
            { "Equinor",        "EQNO" },
            { "Vår Energi",     "VAAR" },
        };

        /// <summary>
        /// Extract 6-digit Kabal cargo ID from H_REFNO value using operator-specific prefix.
        /// Returns the digits only (e.g. "123456") or null.
        /// </summary>
        public static string ExtractKabalId(string hRefno, string operatorName)
        {
            if (string.IsNullOrWhiteSpace(hRefno)) return null;
            string prefix;
            if (!KabalIdPrefixes.TryGetValue(operatorName ?? "", out prefix)) return null;
            int idx = hRefno.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;
            var after = hRefno.Substring(idx + prefix.Length).Trim();
            var m = Regex.Match(after, @"^\s*(\d{4,8})");
            return m.Success ? m.Groups[1].Value : null;
        }

        // Maps the short Gant/UI rig name to the exact Port Name in Kabal TimePlanner (col 2).
        // Confirmed from actual TimePlanner listing screenshots.
        private static readonly Dictionary<string, string> _rigToTimePlannerName =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // AkerBP
            { "Invincible", "Noble Invincible"  },
            { "Integrator", "Noble Integrator"  },
            { "SC8",        "Scarabeo 8"        },
            { "Deepsea Stavanger", "Deepsea Stavanger" },
            { "Nordkap",    "Deepsea Nordkapp"  },
            // Add other operators' rigs here as confirmed
        };

        /// <summary>
        /// Translate a short UI/Gant rig name to the exact Port Name used in Kabal TimePlanner.
        /// Returns the original name unchanged if no mapping exists.
        /// </summary>
        private static string ResolveTimePlannerRigName(string rigName)
        {
            if (string.IsNullOrWhiteSpace(rigName)) return rigName;
            foreach (var kv in _rigToTimePlannerName)
                if (rigName.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kv.Value;
            return rigName;
        }

        // Regex to extract well code from well names — e.g. "C-21" from "2/8-C-21 Valhall PWP"
        private static readonly Regex _wellCodeRx = new Regex(@"([A-Z]-\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Regex to extract section hole size — e.g. "16 1/2" or "12 1/4" or "12.25".
        // Also captures compound sizes like "17 1/2\" x 23 1/2\"" (bi-centric / combo BHA).
        // Try compound first (CompoundSizeRx), then single (SectionSizeRx).
        private static readonly Regex _sectionSizeRx = new Regex(
            @"(\d+(?:\s*\d+/\d+)?(?:\.\d+)?)\s*(?:""|''|in\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _sectionCompoundRx = new Regex(
            @"(\d+(?:\s*\d+/\d+)?(?:\.\d+)?\s*(?:""|''|in\b)?\s*x\s*\d+(?:\s*\d+/\d+)?(?:\.\d+)?\s*(?:""|''|in\b)?)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Regex to detect BHA section start markers:
        // MU / Make up / PU / Pick up / Run + BHA.
        private static readonly Regex _muBhaRx = new Regex(
            @"\b(?:M/?U|Make[\s-]*up|P/?U|Pick[\s-]*up|Run)\b.*\bBHA\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Regex to detect BHA section end markers:
        // POOH + BHA, or L/D / LD / Lay Down / Pull + BHA, or Rack Back aliases.
        private static readonly Regex _poohRx = new Regex(
            @"(?:\bPOOH\b.*\bBHA\b|\b(?:L/?D|LD|Lay\s*Down|Pull)\b.*\bBHA\b|\b(?:Rack\s*Back|R\s*/\s*B|RB)\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Hard end marker for section close preference (final lay down / rack back).
        private static readonly Regex _ldBhaRx = new Regex(
            @"(?:\b(?:L/?D|LD|Lay\s*Down|Pull)\b.*\bBHA\b|\b(?:Rack\s*Back|R\s*/\s*B|RB)\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Current-operation time window: include tasks from now-3h to now+3h.
        private static readonly TimeSpan _currentOpsWindow = TimeSpan.FromHours(3);

        // Regex to detect Whipstock tasks
        private static readonly Regex _whipstockRx = new Regex(
            @"\b(?:Whipstock|PU\s*W/?S)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Whipstock BHA can be a standalone section start even without explicit MU token.
        private static readonly Regex _whipstockBhaStartRx = new Regex(
            @"\bWhipstock\b.*\bBHA\b|\bBHA\b.*\bWhipstock\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Regex to detect D-section style headers (Kabal well timeplanner page 3103):
        // "D-02: Drill 17 1/2\" Section", "D-07: Drill 12 1/4\" section", etc.
        private static readonly Regex _dSectionRx = new Regex(
            @"\bDrill\s+\d.*\b[Ss]ection\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Regex to detect RAP-level drill/RIH-with-drill section headers.
        // These are collapsed parent rows in the APEX TimePlanner table when the section
        // has not yet started — no child tasks (M/U BHA etc.) are visible in the DOM.
        private static readonly Regex _rapDrillRx = new Regex(
            @"\bRAP\s*\d+\b.*\b(?:Drill|RIH\s*(?:&|with|w/))\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // DateTime patterns used in Kabal timeplanner task rows
        private static readonly string[] _kabalDateFormats = {
            "ddd dd-MM-yyyy HH:mm", // "Sun 19-04-2026 08:15"
            "dd-MM-yyyy HH:mm",
            "dd.MM.yyyy HH:mm",
            "dd/MM/yyyy HH:mm",
            "yyyy-MM-dd HH:mm",
        };

        /// <summary>
        /// Extract the well code (e.g. "C-21") from a well name string.
        /// Returns null if no pattern found.
        /// </summary>
        public static string ExtractWellCode(string wellName)
        {
            if (string.IsNullOrWhiteSpace(wellName)) return null;
            var m = _wellCodeRx.Match(wellName);
            return m.Success ? m.Groups[1].Value.ToUpperInvariant() : null;
        }

        /// Helper: get-or-create the shared Edge/Chromium driver.
        /// Disposes and recreates if the headless flag changed or the browser crashed.
        private static OpenQA.Selenium.IWebDriver GetOrCreateDriver(bool headless)
        {
            lock (_driverLock)
            {
                // Check if existing driver is still alive
                if (_sharedDriver != null)
                {
                    if (_sharedDriverHeadless != headless)
                    {
                        // Headless mode changed — kill and recreate
                        try { _sharedDriver.Quit(); } catch { }
                        _sharedDriver = null;
                    }
                    else
                    {
                        try
                        {
                            var _ = _sharedDriver.Url; // ping — throws if browser crashed
                            return _sharedDriver;       // reuse
                        }
                        catch
                        {
                            _sharedDriver = null; // crashed — fall through to create new
                        }
                    }
                }

                // Kill orphan msedge processes that are still using our user-data-dir.
                // These prevent a new browser from acquiring the profile lock.
                KillOrphanEdgeProcesses();

                // Remove stale lock / port files left by a previously crashed Edge process.
                // Without this, "--user-data-dir" startup fails with "DevToolsActivePort doesn't exist".
                Directory.CreateDirectory(_userDataDir);
                foreach (var lockFile in new[] { "SingletonLock", "SingletonSocket", "SingletonCookie", ".parentlock", "DevToolsActivePort" })
                {
                    var lp = Path.Combine(_userDataDir, lockFile);
                    try { if (File.Exists(lp)) File.Delete(lp); } catch { }
                }

                var options = new Microsoft.Edge.SeleniumTools.EdgeOptions();
                options.UseChromium = true;
                if (headless)
                {
                    options.AddArgument("--headless=new");
                    options.AddArgument("--disable-gpu");
                }
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--disable-extensions");
                options.AddArgument("--disable-popup-blocking");
                options.AddArgument("--no-first-run");
                options.AddArgument("--no-default-browser-check");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36 Edg/145.0.0.0");
                options.AddExcludedArgument("enable-automation");
                options.AddArgument("--user-data-dir=" + _userDataDir);

                var driverDir = FindEdgeDriver();
                if (string.IsNullOrEmpty(driverDir))
                {
                    var edgeMajor = GetInstalledEdgeMajorVersion();
                    throw new InvalidOperationException(
                        $"No compatible msedgedriver found for Edge major {edgeMajor}. " +
                        "Auto-download failed or internet access is blocked.");
                }

                // Retry driver creation up to 2 times — first failure may be a
                // transient crash caused by stale profile state.
                const int maxAttempts = 2;
                OpenQA.Selenium.IWebDriver d = null;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        if (driverDir != null)
                        {
                            var svc = Microsoft.Edge.SeleniumTools.EdgeDriverService.CreateChromiumService(driverDir);
                            svc.HideCommandPromptWindow = true;
                            d = new Microsoft.Edge.SeleniumTools.EdgeDriver(svc, options);
                        }
                        else
                        {
                            d = new Microsoft.Edge.SeleniumTools.EdgeDriver(options);
                        }
                        break; // success
                    }
                    catch (Exception ex) when (attempt < maxAttempts)
                    {
                        ScrapeLog.Warn($"Edge driver creation failed (attempt {attempt}/{maxAttempts}): {ex.Message}");
                        KillOrphanEdgeProcesses();
                        Thread.Sleep(1500);
                    }
                }
                d.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                d.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

                _sharedDriver = d;
                _sharedDriverHeadless = headless;
                return _sharedDriver;
            }
        }
        public static async Task<TimePlannerResult> ScrapeTimePlannerAsync(
            string operatorName,
            string rigName,
            string username,
            string password,
            List<string> wellCodesToMatch,
            bool headless = true,
            Action<string> onStatus = null)
        {
            return await Task.Run(() =>
            {
                ScrapeLog.Start();
                ScrapeLog.Info("TimePlanner ParserVersion=2026-05-19b");
                ScrapeLog.Info($"TimePlanner: Operator={operatorName}, Rig={rigName}, Wells={string.Join(",", wellCodesToMatch)}");

                OpenQA.Selenium.IWebDriver driver = null;
                try
                {
                    driver = GetOrCreateDriver(headless);

                    // ── Resolve target TimePlanner URL ──
                    string tpUrl;
                    if (!TimePlannerUrls.TryGetValue(operatorName, out tpUrl))
                        tpUrl = TimePlannerUrls.Values.First();

                    string expectedAppId;
                    OperatorAppIds.TryGetValue(operatorName, out expectedAppId);

                    if (string.IsNullOrEmpty(expectedAppId))
                    {
                        // Lookup failed — operator name likely has an encoding issue.
                        ScrapeLog.Error($"TimePlanner: No App ID found for operator '{operatorName}'. " +
                                        $"Known operators: {string.Join(", ", OperatorAppIds.Keys)}. " +
                                        "Check that the POST body is decoded as UTF-8.");
                        return new TimePlannerResult { Success = false, Error = $"Unknown operator '{operatorName}' — check encoding." };
                    }

                    // ── Session check with 30-minute cache ──
                    // If we verified this operator's session within the last 30 min, skip the
                    // navigate+wait check and go straight to the Gantt page.
                    bool needsLogin = true;
                    bool cacheHit;
                    lock (_lastSessionVerified)
                        cacheHit = _lastSessionVerified.TryGetValue(expectedAppId, out var lastOk)
                                   && (DateTime.UtcNow - lastOk) < _sessionCacheWindow;

                    if (cacheHit)
                    {
                        ScrapeLog.Info($"TimePlanner: Session cache hit for {operatorName} (App {expectedAppId}) — verified <30 min ago, skipping login check.");
                        onStatus?.Invoke("Session cached — loading Gantt...");
                        driver.Navigate().GoToUrl(tpUrl);
                        WaitForApexReady(driver, timeoutSeconds: 3);
                        if (driver.Url.Contains("p=" + expectedAppId + ":"))
                        {
                            ScrapeLog.Info($"TimePlanner: Cache confirmed. URL={driver.Url}");
                            needsLogin = false;
                        }
                        else
                        {
                            ScrapeLog.Warn($"TimePlanner: Cache stale — URL={driver.Url}. Will re-login.");
                            lock (_lastSessionVerified) _lastSessionVerified.Remove(expectedAppId);
                        }
                    }
                    else
                    {
                        onStatus?.Invoke("Checking existing session...");
                        driver.Navigate().GoToUrl(tpUrl);
                        WaitForApexReady(driver, timeoutSeconds: 5);

                        // Session is valid only if the URL contains this operator's exact APEX App ID
                        if (driver.Url.Contains("p=" + expectedAppId + ":"))
                        {
                            ScrapeLog.Info($"TimePlanner: Session reused for {operatorName} (App {expectedAppId}). URL={driver.Url}");
                            onStatus?.Invoke("Session valid — skipping login.");
                            needsLogin = false;
                            lock (_lastSessionVerified) _lastSessionVerified[expectedAppId] = DateTime.UtcNow;
                        }
                        else
                        {
                            ScrapeLog.Warn($"TimePlanner: Session is on wrong app or login. URL={driver.Url} — will re-login.");
                        }
                    }

                    if (needsLogin)
                    {
                        bool alreadyOnLogin = driver.Url.Contains("kabal-account") || driver.Url.Contains("login");
                        if (!alreadyOnLogin)
                        {
                            onStatus?.Invoke("Navigating to Kabal login...");
                            driver.Navigate().GoToUrl(LoginUrl);
                        }

                        onStatus?.Invoke("Entering credentials...");
                        var usernameField = WaitFor(driver, d =>
                        {
                            try
                            {
                                var el = d.FindElement(OpenQA.Selenium.By.CssSelector("input[type='text'], input[type='email']"));
                                return el.Displayed ? el : null;
                            }
                            catch { return null; }
                        }, timeoutSeconds: 5);
                        usernameField.Clear();
                        usernameField.SendKeys(username);
                        ClickSubmitButton(driver, usernameField);

                        onStatus?.Invoke("Entering password...");
                        var passwordField = WaitFor(driver, d =>
                        {
                            try
                            {
                                var el = d.FindElement(OpenQA.Selenium.By.CssSelector("input[type='password']"));
                                return el.Displayed ? el : null;
                            }
                            catch { return null; }
                        }, timeoutSeconds: 5);
                        passwordField.Clear();
                        passwordField.SendKeys(password);
                        ClickSubmitButton(driver, passwordField);
                        WaitForApexReady(driver, timeoutSeconds: 10);

                        // Operator selection
                        string operatorDisplay;
                        if (!OperatorMapping.TryGetValue(operatorName, out operatorDisplay))
                            operatorDisplay = operatorName;
                        try
                        {
                            onStatus?.Invoke($"Selecting operator: {operatorDisplay}...");
                            var opLink = WaitFor(driver, d =>
                            {
                                try
                                {
                                    var el = d.FindElement(OpenQA.Selenium.By.XPath($"//*[contains(text(), '{operatorDisplay}')]"));
                                    return el.Displayed ? el : null;
                                }
                                catch { return null; }
                            }, timeoutSeconds: 5);
                            opLink.Click();
                            WaitForApexReady(driver, timeoutSeconds: 10);
                        }
                        catch (Exception opEx)
                        {
                            ScrapeLog.Warn($"Operator selector skipped: {opEx.Message}");
                        }

                        // Navigate to timeplanner listing
                        onStatus?.Invoke("Navigating to timeplanner...");
                        driver.Navigate().GoToUrl(tpUrl);
                        WaitForApexReady(driver);

                        if (driver.Url.Contains("login") || driver.Url.Contains("kabal-account"))
                        {
                            return new TimePlannerResult { Success = false, Error = "Redirected to login — credentials may be incorrect." };
                        }
                        // Stamp the cache after a successful full login
                        lock (_lastSessionVerified) _lastSessionVerified[expectedAppId] = DateTime.UtcNow;
                    }

                    // ── Gantt view (page 3101) — find rig row, extract well bars ──
                    string resolvedRigName = ResolveTimePlannerRigName(rigName);
                    if (!string.Equals(resolvedRigName, rigName, StringComparison.OrdinalIgnoreCase))
                        ScrapeLog.Info($"TimePlanner: Rig name translated: '{rigName}' → '{resolvedRigName}'");
                    else
                        ScrapeLog.Info($"TimePlanner: Rig name (no translation needed): '{rigName}'");

                    var js = driver as OpenQA.Selenium.IJavaScriptExecutor;
                    // Ensure a large window so vis.js renders all group label rows (it virtualizes by viewport height)
                    driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
                    onStatus?.Invoke("Waiting for Gantt chart data...");
                    WaitForApexReady(driver);

                    // Poll for vis-timeline group labels to render. vis-label elements are the rig-name labels in
                    // the left group panel — they do NOT appear in body.innerText so cannot be detected by text search.
                    bool ganttDataLoaded = false;
                    for (int waitSec = 0; waitSec < 35; waitSec++)
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            var countRaw = js.ExecuteScript(
                                "var ls=document.querySelectorAll('.vis-label .vis-inner');" +
                                "var ne=0; for(var x=0;x<ls.length;x++){" +
                                "  var t=(ls[x].innerText||ls[x].textContent||'').trim();" +
                                "  if(t.length>1)ne++;}" +
                                "var it=document.querySelectorAll('.vis-item').length;" +
                                "return ne+'|'+ls.length+'|'+it;") as string;
                            if (countRaw != null)
                            {
                                var parts = countRaw.Split('|');
                                int nonEmptyN = parts.Length > 0 && int.TryParse(parts[0], out int np) ? np : 0;
                                int labelsN   = parts.Length > 1 && int.TryParse(parts[1], out int lp) ? lp : 0;
                                int itemsN    = parts.Length > 2 && int.TryParse(parts[2], out int ip) ? ip : 0;
                                if (nonEmptyN > 0)
                                {
                                    ScrapeLog.Info($"TimePlanner: Gantt data loaded — labels={labelsN} ({nonEmptyN} with text), items={itemsN}.");
                                    ganttDataLoaded = true;
                                    break;
                                }
                                else if (labelsN > 0 && waitSec % 5 == 4)
                                    ScrapeLog.Info($"TimePlanner: Gantt has {labelsN} label(s) but still empty text... ({waitSec + 1}s)");
                            }
                        }
                        catch { }
                        if (waitSec % 5 == 4)
                            ScrapeLog.Info($"TimePlanner: Still waiting for Gantt labels with text... ({waitSec + 1}s)");
                    }
                    if (!ganttDataLoaded)
                    {
                        // Labels may exist but have no text — proceed to diagnostic instead of hard-failing
                        long labelCount = 0;
                        try { labelCount = (long)(js.ExecuteScript("return document.querySelectorAll('.vis-label').length;") ?? 0L); } catch { }
                        if (labelCount == 0)
                        {
                            var snap = (js.ExecuteScript("return (document.body.innerText || '').substring(0, 600);") as string) ?? "";
                            ScrapeLog.Warn($"TimePlanner: Gantt did not render vis-labels within 35s. Text sample: {snap}");
                            return new TimePlannerResult { Success = false, Error = $"Gantt chart did not render for rig '{resolvedRigName}' within 35 seconds." };
                        }
                        ScrapeLog.Warn($"TimePlanner: Gantt has {labelCount} label(s) but no text after 35s — proceeding with diagnostic.");
                    }
                    Thread.Sleep(750); // extra buffer for all items to finish rendering

                    // ── Diagnostic: log the page structure so we can see what the Gantt looks like ──
                    var diagJs = @"
                        var info = { url: location.href, title: document.title, bodyClasses: document.body.className };
                        // Look for Gantt-like containers
                        var ganttEl = document.querySelector('.js-gantt, .a-GanttChart, .dhx_gantt, [class*=""gantt""], [class*=""Gantt""], [class*=""timeline""], [class*=""Timeline""]');
                        if (ganttEl) {
                            info.ganttTag = ganttEl.tagName;
                            info.ganttClass = ganttEl.className;
                            info.ganttChildCount = ganttEl.children.length;
                            info.ganttSample = ganttEl.innerHTML.substring(0, 2000);
                        }
                        // Look for SVG-based Gantt
                        var svgEl = document.querySelector('svg');
                        if (svgEl) {
                            info.svgFound = true;
                            info.svgClass = svgEl.className ? svgEl.className.baseVal || '' : '';
                            info.svgChildren = svgEl.children.length;
                        }
                        // Look for any clickable elements with rig or well names
                        var clickables = document.querySelectorAll('a, [onclick], [role=""button""], [class*=""bar""], [class*=""task""]');
                        var samples = [];
                        for (var i = 0; i < Math.min(clickables.length, 30); i++) {
                            var el = clickables[i];
                            var txt = (el.innerText || el.textContent || '').trim().substring(0, 80);
                            if (txt) samples.push({ tag: el.tagName, cls: (el.className || '').substring(0, 60), text: txt, href: (el.href || '').substring(0, 120) });
                        }
                        info.clickableSamples = samples;
                        // Look for table rows with rig names (APEX might render as table)
                        var trs = document.querySelectorAll('tr, [role=""row""]');
                        var rigRows = [];
                        for (var i = 0; i < trs.length; i++) {
                            var t = (trs[i].innerText || '').substring(0, 200);
                            if (t.toLowerCase().indexOf('noble') >= 0 || t.toLowerCase().indexOf('scarabeo') >= 0 ||
                                t.toLowerCase().indexOf('deepsea') >= 0 || t.toLowerCase().indexOf('invincible') >= 0 ||
                                t.toLowerCase().indexOf('integrator') >= 0 || t.toLowerCase().indexOf('stavanger') >= 0 ||
                                t.toLowerCase().indexOf('nordkapp') >= 0 || t.toLowerCase().indexOf('ivar') >= 0) {
                                rigRows.push({ idx: i, text: t, tag: trs[i].tagName, cls: (trs[i].className || '').substring(0,60) });
                            }
                        }
                        info.rigRows = rigRows;
                        // Also grab all text content that might contain rig names
                        var allText = document.body.innerText || '';
                        var rigIdx = allText.toLowerCase().indexOf('" + resolvedRigName.ToLower().Replace("'", "\\'") + @"');
                        if (rigIdx >= 0) {
                            info.rigFoundInText = true;
                            info.rigTextContext = allText.substring(Math.max(0, rigIdx - 50), rigIdx + 150);
                        } else {
                            info.rigFoundInText = false;
                        }
                        // Dump innerHTML of vis-label and vis-item elements for DOM structure analysis
                        var visl = document.querySelectorAll('.vis-label');
                        var labelDump = [];
                        for (var xi = 0; xi < Math.min(visl.length, 5); xi++) {
                            labelDump.push({
                                innerText: visl[xi].innerText,
                                textContent: visl[xi].textContent,
                                innerHTML: visl[xi].innerHTML.substring(0, 600),
                                childCount: visl[xi].children.length
                            });
                        }
                        info.labelDump = labelDump;
                        var visit = document.querySelectorAll('.vis-item');
                        var itemDump = [];
                        for (var xi = 0; xi < Math.min(visit.length, 5); xi++) {
                            itemDump.push({
                                innerText: visit[xi].innerText,
                                textContent: visit[xi].textContent,
                                innerHTML: visit[xi].innerHTML.substring(0, 600),
                                dataId: visit[xi].getAttribute('data-id')
                            });
                        }
                        info.itemDump = itemDump;
                        return JSON.stringify(info);
                    ";

                    var diagJson = js.ExecuteScript(diagJs) as string;
                    ScrapeLog.Info($"TimePlanner Gantt diagnostic:\n{diagJson}");

                    // ── Extract well bars for the target rig using vis.js DOM structure ──
                    // vis-label: rig group name labels in the left panel (.vis-label-set)
                    // vis-item:  well bar boxes in the center panel, absolutely positioned
                    // Match by vertical position: item.midY must fall within rig label's top/bottom bounds.
                    var extractWellsJs = @"
                        var rigNameLower = '" + resolvedRigName.ToLower().Replace("'", "\\'") + @"';
                        var result = { rigFound: false, wells: [], debug: { labels: 0, items: 0, labelTexts: [], allItemTexts: [], rowHeight: 0, skippedByRow: 0 } };

                        // Step 1: find the vis-label that matches the rig name.
                        // Text lives in .vis-inner (child div), not directly in .vis-label.
                        // vis.js only renders labels visible in the viewport — window must be large (1920x1080).
                        var inners = document.querySelectorAll('.vis-label .vis-inner');
                        // Also grab the parent .vis-label elements for BoundingClientRect (correct row bounds)
                        var labels = document.querySelectorAll('.vis-label');
                        result.debug.labels = inners.length;
                        var rigTop = -1, rigBottom = -1;
                        for (var i = 0; i < inners.length; i++) {
                            var txt = (inners[i].innerText || inners[i].textContent || '').trim();
                            result.debug.labelTexts.push(txt.substring(0, 60));
                            if (txt.toLowerCase().indexOf(rigNameLower) >= 0 && txt.length < rigNameLower.length + 30) {
                                var parentLabel = inners[i].closest('.vis-label') || inners[i].parentElement;
                                var r = parentLabel.getBoundingClientRect();
                                rigTop = r.top;  rigBottom = r.bottom;
                                result.rigFound = true;
                            }
                        }

                        // Step 2: gather vis-items that overlap (or are very close to) the rig row.
                        // vis.js can position bars a few px outside the label bounds, so midpoint-only
                        // checks with tight +/-5 tolerance can drop valid wells.
                        var allItems = document.querySelectorAll('.vis-item');
                        result.debug.items = allItems.length;
                        var rowHeight = (rigBottom > rigTop) ? (rigBottom - rigTop) : 40;
                        result.debug.rowHeight = Math.round(rowHeight);
                        var rigMid = (rigTop + rigBottom) / 2;
                        var nearTol = Math.max(18, rowHeight * 0.9);
                        var seen = {};
                        for (var j = 0; j < allItems.length; j++) {
                            var item = allItems[j];
                            var ir = item.getBoundingClientRect();
                            var midY = ir.top + ir.height / 2;
                            if (result.rigFound) {
                                var overlap = Math.min(ir.bottom, rigBottom) - Math.max(ir.top, rigTop);
                                var nearRow = Math.abs(midY - rigMid) <= nearTol;
                                if (!(overlap > 0 || nearRow)) { result.debug.skippedByRow++; continue; }
                            }
                            var cont = item.querySelector('.vis-item-content') || item;
                            var anch = cont.querySelector('a[href]');
                            // innerText for visible Svelte-rendered text; fallback to textContent
                            var name = anch
                                ? (anch.innerText || anch.textContent || '').trim()
                                : (cont.innerText || cont.textContent || '').trim();
                            name = name.replace(/\s+/g, ' ').trim();
                            var href = anch ? (anch.href || '') : '';
                            var dataId = item.getAttribute('data-id') || '';
                            result.debug.allItemTexts.push(name.substring(0, 50));
                            if (name && name.length > 2 && name !== 'undefined' && !seen[name+'|'+dataId]) {
                                seen[name+'|'+dataId] = true;
                                result.wells.push({ name: name, href: href, dataId: dataId,
                                                    midY: Math.round(midY), rigTop: Math.round(rigTop) });
                            }
                        }
                        return JSON.stringify(result);
                    ";

                    var wellsJson = js.ExecuteScript(extractWellsJs) as string;
                    ScrapeLog.Info($"TimePlanner well extraction:\n{wellsJson}");

                    var wellData = JsonConvert.DeserializeObject<Dictionary<string, object>>(wellsJson ?? "{}");
                    bool rigFound = wellData.ContainsKey("rigFound") && Convert.ToBoolean(wellData["rigFound"]);
                    var wellsArr = wellData.ContainsKey("wells")
                        ? JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(wellData["wells"].ToString())
                        : new List<Dictionary<string, object>>();

                    ScrapeLog.Info($"TimePlanner: Rig '{resolvedRigName}' found={rigFound}, wells found={wellsArr.Count}");

                    if (!rigFound)
                    {
                        ScrapeLog.Warn($"TimePlanner: Rig '{resolvedRigName}' not found on Gantt page. Check the diagnostic log above.");
                        return new TimePlannerResult
                        {
                            Success = false,
                            Error = $"Rig '{resolvedRigName}' not found on the Gantt chart. Check scraper log for details."
                        };
                    }

                    // Log discovered wells
                    foreach (var w in wellsArr)
                    {
                        var wn = w.ContainsKey("name") ? w["name"].ToString() : "";
                        var wh = w.ContainsKey("href") ? w["href"].ToString() : "";
                        ScrapeLog.Info($"  well bar: '{wn}' href='{wh}'");
                    }

                    var result = new TimePlannerResult { Success = true };
                    int planIdx = 0;

                    foreach (var w in wellsArr)
                    {
                        var wellName = w.ContainsKey("name") ? w["name"].ToString() : "";
                        var wellHref = w.ContainsKey("href") ? w["href"].ToString() : "";
                        var wellDataId = w.ContainsKey("dataId") ? w["dataId"].ToString() : "";

                        if (string.IsNullOrEmpty(wellName)) continue;
                        planIdx++;
                        ScrapeLog.Info($"TimePlanner: [WELL #{planIdx}] '{wellName}' href='{wellHref}' dataId='{wellDataId}'");
                        onStatus?.Invoke($"Scraping tasks for {wellName} ({planIdx}/{wellsArr.Count})...");

                        try
                        {
                            var urlBefore = driver.Url;
                            var handlesBefore = new HashSet<string>(driver.WindowHandles);

                            // ── Inject click interceptors before ANY interaction ───────────────────────
                            // We need to know EXACTLY what Kabal's Svelte/APEX handler calls when a
                            // vis-item is clicked: window.open, apex.navigation.redirect, $.dialog('open')
                            // or something else entirely.  We intercept all three paths.
                            js.ExecuteScript(@"
                                window._kabalClickResult = null;
                                // 1. window.open
                                var _oo = window.open;
                                window.open = function(u,n,f) {
                                    window._kabalClickResult = {type:'window.open', url: u||''};
                                    return _oo.call(this, u, n, f);
                                };
                                // 2. APEX navigation
                                try {
                                    if (window.apex && apex.navigation) {
                                        var _ar = apex.navigation.redirect;
                                        if (_ar) apex.navigation.redirect = function(u) {
                                            window._kabalClickResult = {type:'apex.redirect', url: u};
                                            return _ar.apply(this, arguments);
                                        };
                                        var _aw = apex.navigation.openInNewWindow;
                                        if (_aw) apex.navigation.openInNewWindow = function(u) {
                                            window._kabalClickResult = {type:'apex.openInNewWindow', url: u};
                                            return _aw.apply(this, arguments);
                                        };
                                    }
                                } catch(e) {}
                                // 3. jQuery UI dialog open
                                try {
                                    if (window.$ && $.fn && $.fn.dialog) {
                                        var _od = $.fn.dialog;
                                        $.fn.dialog = function(opt) {
                                            if (opt === 'open') {
                                                var el = this[0];
                                                window._kabalClickResult = {
                                                    type: 'jqui-dialog',
                                                    id: (el && el.id) || '',
                                                    cls: (el && el.className.substring(0,80)) || ''
                                                };
                                            }
                                            return _od.apply(this, arguments);
                                        };
                                    }
                                } catch(e) {}
                            ");

                            if (!string.IsNullOrEmpty(wellHref) && wellHref.StartsWith("http"))
                            {
                                // Direct href embedded in vis-item content anchor — fastest path
                                driver.Navigate().GoToUrl(wellHref);
                            }
                            else
                            {
                                // No href in item — click the vis-item by data-id via JS executor
                                bool clicked = false;
                                if (!string.IsNullOrEmpty(wellDataId))
                                {
                                    try
                                    {
                                        var visCss = ".vis-item[data-id='" + wellDataId + "']";
                                        var visEls = driver.FindElements(OpenQA.Selenium.By.CssSelector(visCss));
                                        if (visEls.Count > 0)
                                        {
                                            js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", visEls[0]);
                                            Thread.Sleep(150);
                                            js.ExecuteScript("arguments[0].click();", visEls[0]);
                                            clicked = true;
                                            ScrapeLog.Info($"  Clicked vis-item[data-id={wellDataId}]");
                                        }
                                    }
                                    catch (Exception clickEx) { ScrapeLog.Warn($"  vis-item click error: {clickEx.Message}"); }
                                }
                                if (!clicked)
                                {
                                    // Fallback: return the DOM element directly from JS, then use
                                    // Actions.MoveToElement+Click to physically move the mouse and fire
                                    // proper mouse events that vis.js/Svelte can intercept.
                                    try
                                    {
                                        var findItemByTextJs = @"
                                            var name = '" + wellName.Replace("'", "\\'") + @"';
                                            var items = document.querySelectorAll('.vis-item');
                                            for (var i = 0; i < items.length; i++) {
                                                var c = items[i].querySelector('.vis-item-content') || items[i];
                                                if ((c.textContent||'').trim().indexOf(name) >= 0) {
                                                    return items[i];
                                                }
                                            }
                                            return null;
                                        ";
                                        var targetEl = js.ExecuteScript(findItemByTextJs) as OpenQA.Selenium.IWebElement;
                                        if (targetEl != null)
                                        {
                                            js.ExecuteScript("arguments[0].scrollIntoView({block:'center',inline:'center'});", targetEl);
                                            Thread.Sleep(250);
                                            new OpenQA.Selenium.Interactions.Actions(driver)
                                                .MoveToElement(targetEl)
                                                .Click()
                                                .Perform();
                                            clicked = true;
                                            ScrapeLog.Info($"  Clicked vis-item via Actions.MoveToElement for '{wellName}'");
                                        }
                                        else
                                        {
                                            ScrapeLog.Warn($"  JS could not find vis-item for '{wellName}' — skipping.");
                                        }
                                    }
                                    catch (Exception fallbackEx) { ScrapeLog.Warn($"  Actions click error: {fallbackEx.Message}"); }

                                    if (!clicked) { ScrapeLog.Warn($"  Could not click well bar: '{wellName}'"); continue; }
                                }
                            }

                            // ── Wait 3s, then read intercepted result + window handles + dialog state ──
                            Thread.Sleep(3000);
                            var handlesAfter = driver.WindowHandles;
                            var newHandles = handlesAfter.Except(handlesBefore).ToList();
                            var clickResult = js.ExecuteScript(
                                "return window._kabalClickResult ? JSON.stringify(window._kabalClickResult) : null"
                            ) as string;
                            var dialogDump = js.ExecuteScript(@"
                                var ds = document.querySelectorAll('.ui-dialog');
                                return JSON.stringify({
                                    count: ds.length,
                                    dialogs: Array.from(ds).map(function(d) {
                                        return {
                                            id: d.id,
                                            cls: d.className.substring(0,80),
                                            open: d.style.display !== 'none' && !d.classList.contains('ui-dialog-hidden'),
                                            tabCnt: d.querySelectorAll('table').length,
                                            divCnt: d.querySelectorAll('div').length,
                                            len: d.innerHTML.length,
                                            html: d.innerHTML.substring(0, 500)
                                        };
                                    })
                                });
                            ") as string;
                            ScrapeLog.Info($"  Intercept={clickResult ?? "null"} | newTabs={newHandles.Count} | url={driver.Url}");
                            ScrapeLog.Info($"  Dialogs: {dialogDump}");

                            List<TimePlannerSection> sections;

                            if (newHandles.Count > 0)
                            {
                                // New tab opened — switch, parse, close, switch back
                                driver.SwitchTo().Window(newHandles[0]);
                                ScrapeLog.Info($"  Switched to new tab: {driver.Url}");
                                WaitForApexReady(driver);
                                // APEX renders table structure immediately but fills cell text asynchronously.
                                // Poll until at least one cell has meaningful content (max 12s).
                                string firstCellText = "";
                                for (int cw = 0; cw < 24; cw++)
                                {
                                    Thread.Sleep(500);
                                    try
                                    {
                                        firstCellText = js.ExecuteScript(
                                            "var cells = document.querySelectorAll('table td'); " +
                                            "for (var i = 0; i < cells.length; i++) { " +
                                            "  var td = cells[i]; " +
                                            "  var txt = (td.textContent || td.innerText || '').trim(); " +
                                            "  if (!txt) { " +
                                            "    var ctrl = td.querySelector('input,textarea,select'); " +
                                            "    txt = ctrl ? ((ctrl.value || ctrl.textContent || '').trim()) : ''; " +
                                            "  } " +
                                            "  if (txt) return txt; " +
                                            "} " +
                                            "return ''; ") as string ?? "";
                                        if (!string.IsNullOrEmpty(firstCellText)) break;
                                    }
                                    catch { }
                                }
                                ScrapeLog.Info($"  First cell text='{firstCellText}'. Parsing...");
                                sections = ParseTimePlannerTasks(js, wellName);
                                ScrapeLog.Info($"  New tab parsed {sections.Count} sections.");
                                driver.Close();
                                driver.SwitchTo().Window(handlesBefore.First());
                            }
                            else
                            {
                                // No new tab — check URL change or same-page dialog/inline content
                                string urlAfter = driver.Url;
                                bool navigated = !urlAfter.Equals(urlBefore, StringComparison.OrdinalIgnoreCase);
                                ScrapeLog.Info($"  navigated={navigated}");

                                if (navigated)
                                {
                                    WaitForApexReady(driver);
                                    Thread.Sleep(250);
                                    sections = ParseTimePlannerTasks(js, wellName);
                                }
                                else
                                {
                                    // Poll for tables OR dialog content (Kabal may use non-table elements)
                                    bool contentReady = false;
                                    for (int tw = 0; tw < 18; tw++)
                                    {
                                        Thread.Sleep(500);
                                        try
                                        {
                                            var rc = js.ExecuteScript(
                                                "var t = document.querySelector('table,div.ui-dialog-content'); " +
                                                "if (!t) return 0; " +
                                                "var r = t.querySelectorAll('tbody tr,tr'); " +
                                                "return r.length;");
                                            long rowCount = rc is long rl ? rl : (rc is int ri ? (long)ri : 0);
                                            if (rowCount > 0) { contentReady = true; break; }
                                        }
                                        catch { }
                                    }
                                    ScrapeLog.Info($"  Content ready={contentReady}. Parsing...");
                                    sections = ParseTimePlannerTasks(js, wellName);
                                    if (sections.Count == 0)
                                    {
                                        var domDump = js.ExecuteScript(@"
                                            var info = { tableCount: 0, tables: [], dialogs: [], bodyLen: document.body.innerHTML.length };
                                            document.querySelectorAll('table').forEach(function(t,i){
                                                if(i<3) info.tables.push({cls:t.className.substring(0,60),rows:t.querySelectorAll('tr').length,html:t.innerHTML.substring(0,300)});
                                            });
                                            info.tableCount = document.querySelectorAll('table').length;
                                            document.querySelectorAll('.ui-dialog').forEach(function(d){
                                                info.dialogs.push({id:d.id,open:d.style.display!=='none',len:d.innerHTML.length,html:d.innerHTML.substring(0,400)});
                                            });
                                            return JSON.stringify(info);
                                        ") as string;
                                        ScrapeLog.Warn($"  0 sections. DOM: {domDump?.Substring(0, Math.Min(800, domDump?.Length ?? 0))}");
                                    }
                                }
                            }

                            result.Sections.AddRange(sections);
                            ScrapeLog.Info($"  Extracted {sections.Count} sections for '{wellName}'");

                            // Navigate back to Gantt and wait for vis-labels to re-render
                            driver.Navigate().GoToUrl(tpUrl);
                            WaitForApexReady(driver);
                            for (int rw = 0; rw < 30; rw++)
                            {
                                Thread.Sleep(500);
                                try
                                {
                                    var lc = js.ExecuteScript("return document.querySelectorAll('.vis-label').length;");
                                    long lcount = lc is long ll ? ll : (lc is int li ? li : 0);
                                    if (lcount > 0) break;
                                }
                                catch { }
                            }
                            Thread.Sleep(250); // let items finish rendering
                        }
                        catch (Exception wellEx)
                        {
                            ScrapeLog.Warn($"  Error scraping well '{wellName}': {wellEx.Message}");
                            // Try to get back to Gantt for next well
                            try { driver.Navigate().GoToUrl(tpUrl); WaitForApexReady(driver); } catch { }
                        }
                    }

                    result.RecentOperations = BuildRecentOperations(result.Sections);
                    result.LastUpdated = DateTime.Now;

                    ScrapeLog.Info($"TimePlanner: Total sections extracted={result.Sections.Count} from {planIdx} wells.");
                    onStatus?.Invoke($"Done. Found {result.Sections.Count} drilling sections from {planIdx} wells.");
                    return result;
                }
                catch (Exception ex)
                {
                    ScrapeLog.Error($"TimePlanner: {ex.GetType().Name}: {ex.Message}");
                    if (driver != null) ScrapeLog.SaveErrorSnapshot(driver, "timeplanner_error");
                    return new TimePlannerResult { Success = false, Error = ex.Message };
                }
                // NOTE: driver is NOT disposed here — shared instance kept alive for session reuse.
            });
        }

        /// <summary>
        /// Parse the task table on a Kabal timeplanner detail page (page 3103).
        /// Groups tasks into drilling sections based on M/U BHA and POOH markers.
        /// </summary>
        private static List<TimePlannerSection> ParseTimePlannerTasks(
            OpenQA.Selenium.IJavaScriptExecutor js, string planName)
        {
            // Expand all collapsible rows so nested RAP tasks (including MU/POOH BHA lines)
            // are present in the DOM before table extraction.
            var expandRowsJs = @"
                function clickEl(el) {
                    try {
                        if (!el) return false;
                        if (el.dataset && el.dataset.mmClicked === '1') return false;
                        el.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
                        el.dispatchEvent(new MouseEvent('mouseup',   { bubbles: true }));
                        el.dispatchEvent(new MouseEvent('click',     { bubbles: true }));
                        if (typeof el.click === 'function') el.click();
                        if (el.dataset) el.dataset.mmClicked = '1';
                        return true;
                    } catch (e) { return false; }
                }

                var selectors = [
                    '[aria-expanded=false]',
                    'button[title*=Expand]',
                    'a[title*=Expand]',
                    '.a-TreeView-toggle.is-collapsed',
                    '.a-TreeView-toggle[aria-expanded=false]',
                    '.js-treeView-toggle[aria-expanded=false]',
                    '.fa-caret-right',
                    '.icon-right-arrow',
                    '.t-Report-report .is-collapsed .js-toggle',
                    '[aria-label*=Expand]',
                    '[title*=expand]',
                    '[class*=collapsed] [role=button]'
                ];

                var roots = document.querySelectorAll('table');
                var clicked = 0;
                for (var r = 0; r < roots.length; r++) {
                    var root = roots[r];

                    // Target RAP parent rows explicitly; many APEX reports hide children until
                    // the RAP row toggle/cell is clicked.
                    var rapRows = root.querySelectorAll('tr');
                    for (var rr = 0; rr < rapRows.length; rr++) {
                        var tr = rapRows[rr];
                        var txt = (tr.textContent || '').trim();
                        if (!/\bRAP\s*\d+/i.test(txt)) continue;

                        var cls = (tr.className || '').toLowerCase();
                        var trExpanded = tr.getAttribute ? tr.getAttribute('aria-expanded') : null;
                        var trCollapsed = cls.indexOf('collapsed') >= 0 || trExpanded === 'false';

                        var toggles = tr.querySelectorAll('[aria-expanded=false], .a-TreeView-toggle.is-collapsed, .js-treeView-toggle[aria-expanded=false], .fa-caret-right, .icon-right-arrow');
                        for (var ti = 0; ti < toggles.length; ti++) {
                            if (clickEl(toggles[ti])) clicked++;
                        }

                        // Some APEX layouts only expand when the first RAP cell is clicked.
                        // Do this only for rows that are explicitly collapsed.
                        if (trCollapsed) {
                            var firstCell = tr.querySelector('td');
                            if (firstCell && clickEl(firstCell)) clicked++;
                        }
                    }

                    for (var s = 0; s < selectors.length; s++) {
                        var nodes = [];
                        try { nodes = root.querySelectorAll(selectors[s]); } catch (e) { continue; }
                        for (var i = 0; i < nodes.length; i++) {
                            var n = nodes[i];
                            if (!n || n.disabled) continue;
                            if (n.getAttribute && n.getAttribute('aria-expanded') === 'true') continue;
                            if (clickEl(n)) clicked++;
                        }
                    }
                }
                return clicked;
            ";

            var countRowsJs = @"
                var tables = document.querySelectorAll('table');
                var best = 0;
                for (var t = 0; t < tables.length; t++) {
                    var r = tables[t].querySelectorAll('tbody tr').length;
                    if (r === 0) r = tables[t].querySelectorAll('tr').length;
                    if (r > best) best = r;
                }
                return best;
            ";

            int prevRows = -1;
            for (int pass = 0; pass < 3; pass++)   // max 3 passes — RAP fallback covers future sections
            {
                int beforeRows = 0;
                try
                {
                    var beforeObj = js.ExecuteScript(countRowsJs);
                    if (beforeObj is long bl) beforeRows = (int)bl;
                    else if (beforeObj is int bi) beforeRows = bi;
                    else int.TryParse(Convert.ToString(beforeObj), out beforeRows);
                }
                catch { }

                var clickObj = js.ExecuteScript(expandRowsJs);
                var clicks = 0;
                if (clickObj is long l) clicks = (int)l;
                else if (clickObj is int i) clicks = i;
                else int.TryParse(Convert.ToString(clickObj), out clicks);

                Thread.Sleep(120);

                int afterRows = beforeRows;
                try
                {
                    var afterObj = js.ExecuteScript(countRowsJs);
                    if (afterObj is long al) afterRows = (int)al;
                    else if (afterObj is int ai) afterRows = ai;
                    else int.TryParse(Convert.ToString(afterObj), out afterRows);
                }
                catch { }

                ScrapeLog.Info($"ParseTimePlannerTasks: expand pass {pass + 1} clicked={clicks}, rows={beforeRows}->{afterRows}");

                if ((clicks <= 0 && afterRows <= beforeRows) || (afterRows == prevRows && afterRows <= beforeRows))
                    break;

                prevRows = afterRows;
                Thread.Sleep(80);
            }

            // Extract task rows via JS from the table that actually contains populated cells.
            var extractTasksJs = @"
                function cellValue(td) {
                    if (!td) return '';
                    var txt = (td.textContent || td.innerText || '').trim();
                    if (txt) return txt;
                    var ctrl = td.querySelector('input,textarea,select');
                    if (ctrl) {
                        var v = (ctrl.value || ctrl.textContent || '').trim();
                        if (v) return v;
                    }
                    var titleEl = td.querySelector('[title]');
                    if (titleEl && titleEl.title) {
                        var t = titleEl.title.trim();
                        if (t) return t;
                    }
                    return '';
                }

                var tables = document.querySelectorAll('table');
                var best = null, bestScore = -1, bestRows = 0;
                for (var t = 0; t < tables.length; t++) {
                    var rows = tables[t].querySelectorAll('tbody tr');
                    if (rows.length === 0) rows = tables[t].querySelectorAll('tr');

                    var nonEmptyCells = 0;
                    var sampleCells = tables[t].querySelectorAll('td');
                    for (var c = 0; c < sampleCells.length; c++) {
                        if (cellValue(sampleCells[c])) nonEmptyCells++;
                    }

                    // Prefer table with populated cells; use row count only as a tiebreaker.
                    var score = nonEmptyCells * 1000 + rows.length;
                    if (score > bestScore) {
                        bestScore = score;
                        bestRows = rows.length;
                        best = tables[t];
                    }
                }
                if (!best) return JSON.stringify({ tableCount: tables.length, rows: [] });
                var headers = [];
                var ths = best.querySelectorAll('th');
                for (var i = 0; i < ths.length; i++) headers.push((ths[i].textContent || ths[i].innerText || '').trim().toLowerCase());
                var dataRows = best.querySelectorAll('tbody tr');
                if (dataRows.length === 0) {
                    // No tbody — skip first row (likely a header tr) and use all remaining tr
                    var allTr = best.querySelectorAll('tr');
                    dataRows = Array.prototype.slice.call(allTr, headers.length > 0 ? 1 : 0);
                }
                var data = [];
                for (var r = 0; r < dataRows.length; r++) {
                    var cells = dataRows[r].querySelectorAll('td');
                    if (cells.length === 0) continue;
                    var obj = {};
                    for (var c = 0; c < cells.length; c++) {
                        var key = c < headers.length ? (headers[c] || '') : '';
                        // APEX page 3103 often has blank header text; never allow empty keys.
                        if (!key) key = 'col_' + c;
                        // Keep all cells even when duplicate header captions exist.
                        if (Object.prototype.hasOwnProperty.call(obj, key)) key = key + '_' + c;
                        obj[key] = cellValue(cells[c]);
                    }
                    data.push(obj);
                }
                return JSON.stringify({ tableCount: tables.length, selectedRows: bestRows, selectedScore: bestScore, rows: data });
            ";

            var json = js.ExecuteScript(extractTasksJs) as string;
            // Result is now { tableCount: N, rows: [...] }
            List<Dictionary<string, string>> taskRows;
            int tableCount = 0;
            try
            {
                var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(json ?? "{}");
                if (wrapper != null && wrapper.ContainsKey("rows"))
                {
                    tableCount = wrapper.ContainsKey("tableCount") ? Convert.ToInt32(wrapper["tableCount"]) : 0;
                    taskRows = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(wrapper["rows"].ToString());
                    if (wrapper.ContainsKey("selectedRows") || wrapper.ContainsKey("selectedScore"))
                    {
                        var selectedRows = wrapper.ContainsKey("selectedRows") ? Convert.ToInt32(wrapper["selectedRows"]) : 0;
                        var selectedScore = wrapper.ContainsKey("selectedScore") ? Convert.ToInt32(wrapper["selectedScore"]) : 0;
                        ScrapeLog.Info($"ParseTimePlannerTasks: selected table rows={selectedRows}, score={selectedScore}");
                    }
                }
                else
                {
                    taskRows = new List<Dictionary<string, string>>();
                }
            }
            catch { taskRows = new List<Dictionary<string, string>>(); }
            ScrapeLog.Info($"ParseTimePlannerTasks: {tableCount} tables, {taskRows?.Count ?? 0} raw rows for '{planName}'");
            if (taskRows != null && taskRows.Count > 0)
            {
                ScrapeLog.Info($"  headers: {string.Join(" | ", taskRows[0].Keys)}");
                // Log first 5 rows for diagnosis
                int rowsToLog = Math.Min(taskRows.Count, 5);
                for (int i = 0; i < rowsToLog; i++)
                    ScrapeLog.Info($"  row[{i}]: {string.Join(" | ", taskRows[i].Select(kv => kv.Key + "=" + kv.Value))}");
            }

            // Parse tasks — look for task name (largest text column) and date columns
            var allTasks = new List<TimePlannerTask>();
            foreach (var row in taskRows)
            {
                string taskName = "", startDt = "", endDt = "";

                foreach (var kv in row)
                {
                    var val = kv.Value;
                    if (string.IsNullOrWhiteSpace(val)) continue;

                    // Detect date-like values
                    DateTime parsed;
                    if (TryParseKabalDate(val, out parsed))
                    {
                        if (string.IsNullOrEmpty(startDt))
                            startDt = val;
                        else if (string.IsNullOrEmpty(endDt))
                            endDt = val;
                        continue;
                    }

                    // Longest non-date text is likely the task name
                    if (val.Length > taskName.Length && !Regex.IsMatch(val, @"^\d+(\.\d+)?$"))
                        taskName = val;
                }

                if (!string.IsNullOrEmpty(taskName))
                {
                    bool isMuBha    = IsSectionStartTask(taskName);
                    bool isPooh     = _poohRx.IsMatch(taskName);
                    bool isWhip     = _whipstockRx.IsMatch(taskName);
                    bool isKey = isMuBha || isPooh || isWhip;
                    string matchTag = isMuBha ? "[MU-BHA]" : isPooh ? "[POOH]" : isWhip ? "[WHIP]" : "";
                    ScrapeLog.Info($"  task: '{taskName}' start='{startDt}' end='{endDt}' {matchTag}");
                    allTasks.Add(new TimePlannerTask
                    {
                        TaskName = taskName,
                        StartDateTime = startDt,
                        EndDateTime = endDt,
                        IsKeyTask = isKey,
                    });
                }
            }

            // Build all BHA sections for this well:
            // each MU/PU/RIH-with-BHA starts a section, first following POOH/LD-BHA ends it.
            var sections = new List<TimePlannerSection>();

            var timedTasks = new List<Tuple<TimePlannerTask, DateTime>>();
            foreach (var t in allTasks)
            {
                DateTime st;
                if (TryParseKabalDate(t.StartDateTime, out st))
                    timedTasks.Add(Tuple.Create(t, st));
            }
            timedTasks = timedTasks.OrderBy(x => x.Item2).ToList();

            TimePlannerSection current = null;
            string pendingSoftEnd = null;
            for (int ti = 0; ti < timedTasks.Count; ti++)
            {
                var tt = timedTasks[ti];
                var task = tt.Item1;

                if (IsSectionStartTask(task.TaskName))
                {
                    if (current != null && string.IsNullOrWhiteSpace(current.PoohDateTime))
                    {
                        current.PoohDateTime = !string.IsNullOrWhiteSpace(pendingSoftEnd) ? pendingSoftEnd : task.StartDateTime;
                        DateTime cMu, cEnd;
                        if (TryParseKabalDate(current.MuBhaDateTime, out cMu) && TryParseKabalDate(current.PoohDateTime, out cEnd))
                            current.DurationDays = Math.Round((cEnd - cMu).TotalDays, 2);
                        current = null;
                        pendingSoftEnd = null;
                    }

                    string size;
                    string sectionName;
                    ExtractSectionSizeAndName(task.TaskName, out size, out sectionName);

                    // If M/U BHA has no explicit size, infer from nearby drill/section/hole rows.
                    if (string.IsNullOrWhiteSpace(size))
                    {
                        for (int bi = ti - 1; bi >= 0 && bi >= ti - 8; bi--)
                        {
                            var prevTask = timedTasks[bi].Item1;
                            var prevName = prevTask.TaskName ?? "";
                            if (string.IsNullOrWhiteSpace(prevName)) continue;

                            // Prefer contextual rows that typically carry section size descriptors.
                            if (prevName.IndexOf("hole", StringComparison.OrdinalIgnoreCase) < 0 &&
                                prevName.IndexOf("section", StringComparison.OrdinalIgnoreCase) < 0 &&
                                prevName.IndexOf("drill", StringComparison.OrdinalIgnoreCase) < 0 &&
                                !_rapDrillRx.IsMatch(prevName))
                                continue;

                            string hintSize;
                            string hintSectionName;
                            ExtractSectionSizeAndName(prevName, out hintSize, out hintSectionName);
                            if (!string.IsNullOrWhiteSpace(hintSize))
                            {
                                size = hintSize;
                                sectionName = hintSectionName;
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(size) && _whipstockRx.IsMatch(task.TaskName))
                        sectionName = "Whipstock section";

                    current = new TimePlannerSection
                    {
                        WellName = planName,
                        PlanName = planName,
                        SectionName = sectionName,
                        SectionSize = size,
                        MuBhaDateTime = task.StartDateTime,
                        IsWhipstock = _whipstockRx.IsMatch(task.TaskName),
                    };
                    current.Tasks.Add(task);
                    sections.Add(current);
                    ScrapeLog.Info($"  >> Section STARTED: '{current.SectionName}' at {task.StartDateTime}");
                    continue;
                }

                if (_poohRx.IsMatch(task.TaskName) && current != null)
                {
                    current.Tasks.Add(task);

                    var endValue = !string.IsNullOrEmpty(task.EndDateTime) ? task.EndDateTime : task.StartDateTime;
                    if (_ldBhaRx.IsMatch(task.TaskName))
                    {
                        current.PoohDateTime = endValue;
                        DateTime muDt, poohDt;
                        if (TryParseKabalDate(current.MuBhaDateTime, out muDt) && TryParseKabalDate(current.PoohDateTime, out poohDt))
                            current.DurationDays = Math.Round((poohDt - muDt).TotalDays, 2);
                        current = null;
                        pendingSoftEnd = null;
                    }
                    else
                    {
                        // Interim POOH marker (e.g. below BOP) — keep as fallback,
                        // but wait for L/D BHA before closing the section.
                        pendingSoftEnd = endValue;
                    }
                    continue;
                }

                if (current != null)
                {
                    current.Tasks.Add(task);
                    if (_whipstockRx.IsMatch(task.TaskName))
                        current.IsWhipstock = true;
                }
            }

            if (current != null && string.IsNullOrWhiteSpace(current.PoohDateTime) && !string.IsNullOrWhiteSpace(pendingSoftEnd))
            {
                current.PoohDateTime = pendingSoftEnd;
                DateTime muDt, poohDt;
                if (TryParseKabalDate(current.MuBhaDateTime, out muDt) && TryParseKabalDate(current.PoohDateTime, out poohDt))
                    current.DurationDays = Math.Round((poohDt - muDt).TotalDays, 2);
            }

            // ── RAP-DRILL FALLBACK ─────────────────────────────────────────────────────
            // APEX only renders child tasks for the currently-active RAP; future RAPs are
            // collapsed and only their header row appears in the DOM.  Walk through all
            // RAP-level drill/RIH headers; if no section was found within ±2 h of that
            // RAP's start time (= child tasks were visible), synthesise a section from the
            // header row itself, using the next RAP header as the section end.
            var rapDrillList = timedTasks
                .Where(tt => _rapDrillRx.IsMatch(tt.Item1.TaskName))
                .ToList();

            for (int ri = 0; ri < rapDrillList.Count; ri++)
            {
                var rapTt    = rapDrillList[ri];
                var rapStart = rapTt.Item2;

                // Skip if existing sections already cover this RAP window.
                bool covered = sections.Any(s =>
                {
                    DateTime sd;
                    return TryParseKabalDate(s.MuBhaDateTime, out sd)
                           && Math.Abs((sd - rapStart).TotalHours) < 2.0;
                });
                if (covered) continue;

                // Use next RAP-drill header's start as this section's end.
                string rapEndDt = (ri + 1 < rapDrillList.Count)
                    ? rapDrillList[ri + 1].Item1.StartDateTime
                    : null;

                string size;
                string sectionName;
                ExtractSectionSizeAndName(rapTt.Item1.TaskName, out size, out sectionName);

                var syn = new TimePlannerSection
                {
                    WellName      = planName,
                    PlanName      = planName,
                    SectionName   = sectionName,
                    SectionSize   = size,
                    MuBhaDateTime = rapTt.Item1.StartDateTime,
                    PoohDateTime  = rapEndDt ?? "",
                    IsWhipstock   = false,
                };
                syn.Tasks.Add(rapTt.Item1);

                if (!string.IsNullOrEmpty(rapEndDt))
                {
                    DateTime synMu, synEnd;
                    if (TryParseKabalDate(syn.MuBhaDateTime, out synMu)
                        && TryParseKabalDate(rapEndDt, out synEnd))
                        syn.DurationDays = Math.Round((synEnd - synMu).TotalDays, 2);
                }

                sections.Add(syn);
                ScrapeLog.Info($"  >> Section (RAP fallback): '{syn.SectionName}' at {rapTt.Item1.StartDateTime}");
            }

            // ── D-SECTION FALLBACK ─────────────────────────────────────────────────────
            // The Kabal well timeplanner (page 3103) renders flat "D-XX: Drill SIZE Section"
            // top-level tasks. Sub-tasks (Run/Pull BHA) are nested but expansion often
            // yields 0 clicks (they are not collapsible APEX rows).  Synthesise sections
            // from these D-level headers when no section was found within ±2 h of their
            // start time, using the next D-section header's start as the section end.
            var dSectionList = timedTasks
                .Where(tt => _dSectionRx.IsMatch(tt.Item1.TaskName))
                .OrderBy(tt => tt.Item2)
                .ToList();

            for (int di = 0; di < dSectionList.Count; di++)
            {
                var dTt    = dSectionList[di];
                var dStart = dTt.Item2;

                bool covered = sections.Any(s =>
                {
                    DateTime sd;
                    return TryParseKabalDate(s.MuBhaDateTime, out sd)
                           && Math.Abs((sd - dStart).TotalHours) < 2.0;
                });
                if (covered) continue;

                string dEndDt = (di + 1 < dSectionList.Count)
                    ? dSectionList[di + 1].Item1.StartDateTime
                    : null;

                string dSize;
                string dSectionName;
                ExtractSectionSizeAndName(dTt.Item1.TaskName, out dSize, out dSectionName);

                var synD = new TimePlannerSection
                {
                    WellName      = planName,
                    PlanName      = planName,
                    SectionName   = dSectionName,
                    SectionSize   = dSize,
                    MuBhaDateTime = dTt.Item1.StartDateTime,
                    PoohDateTime  = dEndDt ?? "",
                    IsWhipstock   = false,
                };
                synD.Tasks.Add(dTt.Item1);

                if (!string.IsNullOrEmpty(dEndDt))
                {
                    DateTime synMu, synEnd;
                    if (TryParseKabalDate(synD.MuBhaDateTime, out synMu)
                        && TryParseKabalDate(dEndDt, out synEnd))
                        synD.DurationDays = Math.Round((synEnd - synMu).TotalDays, 2);
                }

                sections.Add(synD);
                ScrapeLog.Info($"  >> Section (D-section fallback): '{synD.SectionName}' at {dTt.Item1.StartDateTime}");
            }

            // Re-sort all sections by start time after both extraction passes.
            sections = sections
                .OrderBy(s =>
                {
                    DateTime sd;
                    return TryParseKabalDate(s.MuBhaDateTime, out sd) ? sd : DateTime.MaxValue;
                })
                .ToList();

            return sections;
        }

        private static List<TimePlannerRecentOperation> BuildRecentOperations(List<TimePlannerSection> sections)
        {
            var result = new List<TimePlannerRecentOperation>();
            var from = DateTime.Now - _currentOpsWindow;
            var to = DateTime.Now + _currentOpsWindow;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var sec in sections ?? new List<TimePlannerSection>())
            {
                foreach (var t in sec.Tasks ?? new List<TimePlannerTask>())
                {
                    DateTime dt;
                    if (!TryParseKabalDate(t.StartDateTime, out dt)) continue;
                    if (dt < from || dt > to) continue;

                    var key = (sec.WellName ?? "") + "|" + (t.TaskName ?? "") + "|" + dt.ToString("yyyyMMddHHmm");
                    if (!seen.Add(key)) continue;

                    result.Add(new TimePlannerRecentOperation
                    {
                        WellName = sec.WellName,
                        TaskName = t.TaskName,
                        TaskDateTime = t.StartDateTime,
                        HoursFromNow = Math.Round((dt - DateTime.Now).TotalHours, 2)
                    });
                }
            }

            return result
                .OrderBy(x => Math.Abs(x.HoursFromNow))
                .ThenBy(x => x.WellName)
                .ToList();
        }

        private static void ExtractSectionSizeAndName(string taskName, out string size, out string sectionName)
        {
            size = "";
            sectionName = taskName ?? "";
            if (string.IsNullOrWhiteSpace(taskName)) return;

            var compMatch = _sectionCompoundRx.Match(taskName);
            if (compMatch.Success)
            {
                size = compMatch.Groups[1].Value.Trim();
                // Normalize optional in/quotes to canonical quoted display for consistency.
                size = Regex.Replace(size, @"\s*(?:in\b|''|'')\s*", "\"");
                size = Regex.Replace(size, @"\s*x\s*", " x ");
                sectionName = size + " section";
                return;
            }

            var sizeMatch = _sectionSizeRx.Match(taskName);
            size = sizeMatch.Success ? sizeMatch.Groups[1].Value.Trim() : "";
            sectionName = !string.IsNullOrEmpty(size) ? size + "\" section" : taskName;
        }

        private static bool IsSectionStartTask(string taskName)
        {
            if (string.IsNullOrWhiteSpace(taskName)) return false;
            return _muBhaRx.IsMatch(taskName) || _whipstockBhaStartRx.IsMatch(taskName);
        }

        private static bool TryParseKabalDate(string value, out DateTime result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            // Strip leading day-of-week prefix (e.g. "Sun ")
            var cleaned = Regex.Replace(value.Trim(), @"^[A-Za-z]{2,3}\s+", "");
            return DateTime.TryParseExact(cleaned, _kabalDateFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Scrape the Kabal APEX Interactive Report directly via Selenium (Edge).
        /// No Node.js dependency — runs the browser from .NET.
        /// </summary>
        public static async Task<ScraperResult> ScrapeAsync(
            string operatorName,
            string rigName,
            string username,
            string password,
            bool headless = true,
            bool dryRun = true,
            Action<string> onStatus = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            return await Task.Run(() =>
            {
                ScrapeLog.Start();
                ScrapeLog.Info($"Operator={operatorName}, Rig={rigName}");

                OpenQA.Selenium.IWebDriver driver = null;
                try
                {
                    driver = GetOrCreateDriver(headless);

                    // ── Resolve target APEX URL ──
                    string targetUrl;
                    if (!OperatorAppUrls.TryGetValue(operatorName, out targetUrl))
                        targetUrl = OperatorAppUrls["Equinor"];

                    // ── 30-minute session cache ──
                    // Use a "sync:{appId}" key so it doesn't collide with the TimePlanner cache.
                    var cacheKey = "sync:" + (OperatorAppIds.TryGetValue(operatorName, out var aid) ? aid : operatorName);
                    bool needsLogin;
                    bool cacheHit;
                    lock (_lastSessionVerified)
                        cacheHit = _lastSessionVerified.TryGetValue(cacheKey, out var lastOk)
                                   && (DateTime.UtcNow - lastOk) < _sessionCacheWindow;

                    if (cacheHit)
                    {
                        ScrapeLog.Info($"ScrapeAsync: Session cache hit for {operatorName} — verified <30 min ago, skipping login check.");
                        onStatus?.Invoke("Session cached — loading data...");
                        needsLogin = false;
                    }
                    else
                    {
                        onStatus?.Invoke("Checking existing session...");
                        driver.Navigate().GoToUrl(targetUrl);
                        WaitForApexReady(driver, timeoutSeconds: 5);

                        if (driver.Url.Contains("app01.kabal.com") &&
                            !driver.Url.Contains("login") &&
                            !driver.Url.Contains("kabal-account"))
                        {
                            ScrapeLog.Info("ScrapeAsync: Session reused — skipping login.");
                            onStatus?.Invoke("Session valid — skipping login.");
                            needsLogin = false;
                            lock (_lastSessionVerified) _lastSessionVerified[cacheKey] = DateTime.UtcNow;
                        }
                        else
                        {
                            ScrapeLog.Info("ScrapeAsync: Session expired — full login required.");
                            needsLogin = true;
                        }
                    }

                    if (needsLogin)
                    {
                    // If session check redirected us to login already, skip redundant navigation
                    bool alreadyOnLogin = driver.Url.Contains("kabal-account") || driver.Url.Contains("login");
                    if (!alreadyOnLogin)
                    {
                        onStatus?.Invoke("Navigating to Kabal login...");
                        driver.Navigate().GoToUrl(LoginUrl);
                    }
                    else
                    {
                        onStatus?.Invoke("Already on login page...");
                        ScrapeLog.Info("Skipped LoginUrl navigation — already redirected to login.");
                    }

                    onStatus?.Invoke("Entering credentials...");
                    var usernameField = WaitFor(driver, d =>
                    {
                        try
                        {
                            var el = d.FindElement(OpenQA.Selenium.By.CssSelector("input[type='text'], input[type='email']"));
                            return el.Displayed ? el : null;
                        }
                        catch { return null; }
                    }, timeoutSeconds: 5);
                    usernameField.Clear();
                    usernameField.SendKeys(username);

                    // Submit username — use FindElements + no implicit wait to avoid 3s per failed selector
                    ClickSubmitButton(driver, usernameField);

                    onStatus?.Invoke("Entering password...");
                    var passwordField = WaitFor(driver, d =>
                    {
                        try
                        {
                            var el = d.FindElement(OpenQA.Selenium.By.CssSelector("input[type='password']"));
                            return el.Displayed ? el : null;
                        }
                        catch { return null; }
                    }, timeoutSeconds: 5);
                    passwordField.Clear();
                    passwordField.SendKeys(password);

                    // Submit password
                    ClickSubmitButton(driver, passwordField);
                    WaitForApexReady(driver, timeoutSeconds: 10);

                    // Operator selection (if present)
                    string operatorDisplay;
                    if (!OperatorMapping.TryGetValue(operatorName, out operatorDisplay))
                        operatorDisplay = operatorName;

                    try
                    {
                        onStatus?.Invoke($"Selecting operator: {operatorDisplay}...");
                        var opLink = WaitFor(driver, d =>
                        {
                            try
                            {
                                var el = d.FindElement(OpenQA.Selenium.By.XPath($"//*[contains(text(), '{operatorDisplay}')]"));
                                return el.Displayed ? el : null;
                            }
                            catch { return null; }
                        }, timeoutSeconds: 5);
                        opLink.Click();
                        WaitForApexReady(driver, timeoutSeconds: 10);
                    }
                    catch (Exception opEx)
                    {
                        ScrapeLog.Warn($"Operator selector skipped: {opEx.Message}");
                    }

                    // Navigate to operator APEX app page (with date range)
                    var navUrl = targetUrl;
                    if (dateFrom.HasValue && dateTo.HasValue)
                    {
                        var df = dateFrom.Value.ToString("dd.MM.yyyy");
                        var dt = dateTo.Value.ToString("dd.MM.yyyy");
                        navUrl = navUrl.Replace(
                            "P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout",
                            $"P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH,P328_FROM_DATE,P328_TO_DATE:loadout,,,cargo.operations.loadout,{df},{dt}");
                    }
                    onStatus?.Invoke($"Navigating to {operatorName} loadout page...");
                    driver.Navigate().GoToUrl(navUrl);
                    WaitForApexReady(driver);

                    if (driver.Url.Contains("login") || driver.Url.Contains("kabal-account:login"))
                    {
                        ScrapeLog.Error($"Redirected to login: {driver.Url}");
                        lock (_lastSessionVerified) _lastSessionVerified.Remove(cacheKey); // clear stale cache
                        return new ScraperResult { Success = false, Error = "Redirected back to login — credentials may be incorrect or SSO session expired." };
                    }
                    // Stamp the cache after a successful full login
                    lock (_lastSessionVerified) _lastSessionVerified[cacheKey] = DateTime.UtcNow;
                    } // end if (needsLogin)

                    // Session reuse path: re-navigate with dates
                    if (!needsLogin && dateFrom.HasValue && dateTo.HasValue)
                    {
                        var df = dateFrom.Value.ToString("dd.MM.yyyy");
                        var dt = dateTo.Value.ToString("dd.MM.yyyy");
                        var dateUrl = targetUrl.Replace(
                            "P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout",
                            $"P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH,P328_FROM_DATE,P328_TO_DATE:loadout,,,cargo.operations.loadout,{df},{dt}");
                        driver.Navigate().GoToUrl(dateUrl);
                        WaitForApexReady(driver);
                    }

                    // ── Apply APEX IR filters ──
                    onStatus?.Invoke("Configuring filters...");
                    ConfigureApexIR(driver, dateFrom, dateTo);

                    // ── Scrape APEX Interactive Report ──
                    onStatus?.Invoke("Scraping APEX table...");
                    var allRows = ScrapeApexIR(driver);

                    var modems = ParseModems(allRows);
                    ScrapeLog.Info($"Scraped {allRows.Count} rows → {modems.Count} modems.");
                    onStatus?.Invoke($"Parsed {modems.Count} modem records from {allRows.Count} rows.");

                    ScrapeLog.Info("Scrape OK.");
                    return new ScraperResult
                    {
                        Success = true,
                        Scraped = modems.Count,
                        DryRun = dryRun,
                        Modems = modems
                    };
                }
                catch (Exception ex)
                {
                    ScrapeLog.Error($"{ex.GetType().Name}: {ex.Message}");
                    if (driver != null) ScrapeLog.SaveErrorSnapshot(driver, "error");
                    return new ScraperResult { Success = false, Error = ex.Message };
                }
                // NOTE: driver is NOT disposed here — it is a shared instance kept alive for 30-min session reuse.
            });
        }

        /// <summary>
        /// Poll until <paramref name="condition"/> returns a non-null element or timeout elapses.
        /// Replaces WebDriverWait (not available without Selenium.Support package).
        /// </summary>
        private static OpenQA.Selenium.IWebElement WaitFor(
            OpenQA.Selenium.IWebDriver driver,
            Func<OpenQA.Selenium.IWebDriver, OpenQA.Selenium.IWebElement> condition,
            int timeoutSeconds = 15,
            [System.Runtime.CompilerServices.CallerLineNumber] int callerLine = 0)
        {
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            while (DateTime.UtcNow < deadline)
            {
                var el = condition(driver);
                if (el != null) return el;
                Thread.Sleep(150);
            }
            ScrapeLog.Error($"WaitFor (line {callerLine}) timed out after {timeoutSeconds}s.");
            ScrapeLog.SaveErrorSnapshot(driver, $"WaitFor_timeout_L{callerLine}");
            throw new TimeoutException($"WaitFor timed out after {timeoutSeconds}s");
        }

        /// <summary>
        /// Click a submit button on the page without wasting time on implicit waits.
        /// Falls back to pressing Enter on the given field if no submit button is found.
        /// </summary>
        private static void ClickSubmitButton(OpenQA.Selenium.IWebDriver driver, OpenQA.Selenium.IWebElement fallbackField)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            try
            {
                var btns = driver.FindElements(OpenQA.Selenium.By.CssSelector("button[type='submit']"));
                if (btns.Count > 0) { btns[0].Click(); return; }

                var inputs = driver.FindElements(OpenQA.Selenium.By.CssSelector("input[type='submit']"));
                if (inputs.Count > 0) { inputs[0].Click(); return; }

                fallbackField.SendKeys(OpenQA.Selenium.Keys.Return);
            }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            }
        }

        /// <summary>
        /// Wait until APEX page is idle: document.readyState=complete and no jQuery AJAX in flight.
        /// </summary>
        private static void WaitForApexReady(OpenQA.Selenium.IWebDriver driver, int timeoutSeconds = 15)
        {
            var js = driver as OpenQA.Selenium.IJavaScriptExecutor;
            if (js == null) { Thread.Sleep(300); return; }
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            // Check immediately first — page may already be ready
            try
            {
                var done = js.ExecuteScript(
                    "return document.readyState==='complete' && (typeof jQuery==='undefined' || jQuery.active===0)");
                if (done is bool && (bool)done) return;
            }
            catch { }
            // Brief pause then poll
            Thread.Sleep(50);
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var done = js.ExecuteScript(
                        "return document.readyState==='complete' && (typeof jQuery==='undefined' || jQuery.active===0)");
                    if (done is bool && (bool)done) return;
                }
                catch { }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Poll until at least one element matching <paramref name="cssSelector"/> exists and is displayed.
        /// </summary>
        private static System.Collections.ObjectModel.ReadOnlyCollection<OpenQA.Selenium.IWebElement>
            WaitForElements(OpenQA.Selenium.IWebDriver driver, string cssSelector, int timeoutSeconds = 10)
        {
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            while (DateTime.UtcNow < deadline)
            {
                var els = driver.FindElements(OpenQA.Selenium.By.CssSelector(cssSelector));
                if (els.Count > 0) return els;
                Thread.Sleep(150);
            }
            return driver.FindElements(OpenQA.Selenium.By.CssSelector(cssSelector));
        }

        /// <summary>
        /// Find msedgedriver.exe — first checks the application's own directory (for portable deployment),
        /// then falls back to the Selenium cache under %USERPROFILE%\.cache.
        /// </summary>
        private static string FindEdgeDriver()
        {
            var edgeMajor = GetInstalledEdgeMajorVersion();
            if (edgeMajor > 0)
                ScrapeLog.Info($"Edge browser major version detected: {edgeMajor}");
            else
                ScrapeLog.Warn("Could not detect installed Edge version. Will use newest available driver.");

            var candidates = new List<string>();

            // 1) App directory (same folder as the running .exe)
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(Path.Combine(appDir, "msedgedriver.exe")))
                candidates.Add(appDir);

            // 2) Current working directory
            var cwd = Directory.GetCurrentDirectory();
            if (File.Exists(Path.Combine(cwd, "msedgedriver.exe")))
                candidates.Add(cwd);

            // 3) Selenium cache
            var cacheRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache", "selenium", "msedgedriver", "win64");
            if (Directory.Exists(cacheRoot))
            {
                foreach (var dir in Directory.GetDirectories(cacheRoot).OrderByDescending(d => d))
                {
                    if (File.Exists(Path.Combine(dir, "msedgedriver.exe")))
                        candidates.Add(dir);
                }
            }

            // 4) AppData downloaded driver cache (managed by this app)
            var appCacheRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ModemMerger", "drivers", "msedgedriver");
            if (Directory.Exists(appCacheRoot))
            {
                foreach (var dir in Directory.GetDirectories(appCacheRoot).OrderByDescending(d => d))
                {
                    if (File.Exists(Path.Combine(dir, "msedgedriver.exe")))
                        candidates.Add(dir);
                }
            }

            // 5) PATH directories
            var envPath = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var p in envPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    var dir = p.Trim();
                    if (File.Exists(Path.Combine(dir, "msedgedriver.exe")))
                        candidates.Add(dir);
                }
                catch { }
            }

            // Select first compatible driver by major version.
            var distinctCandidates = candidates
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            string fallbackDir = null;
            foreach (var dir in distinctCandidates)
            {
                var exe = Path.Combine(dir, "msedgedriver.exe");
                var drvMajor = GetFileMajorVersion(exe);
                ScrapeLog.Info($"Found msedgedriver major={drvMajor} at {exe}");
                if (fallbackDir == null && drvMajor > 0)
                    fallbackDir = dir;
                if (edgeMajor > 0 && drvMajor == edgeMajor)
                    return dir;
            }

            // No compatible local driver found: auto-download matching one.
            if (edgeMajor > 0)
            {
                var downloaded = DownloadMatchingEdgeDriver(edgeMajor);
                if (!string.IsNullOrEmpty(downloaded))
                    return downloaded;
            }

            // Last fallback when Edge version is unknown.
            return edgeMajor <= 0 ? fallbackDir : null;
        }

        private static int GetInstalledEdgeMajorVersion()
        {
            var candidates = new[]
            {
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };

            foreach (var p in candidates)
            {
                try
                {
                    if (!File.Exists(p)) continue;
                    return GetFileMajorVersion(p);
                }
                catch { }
            }
            return 0;
        }

        private static int GetFileMajorVersion(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return 0;
                var fvi = FileVersionInfo.GetVersionInfo(filePath);
                return fvi.FileMajorPart;
            }
            catch { return 0; }
        }

        private static string DownloadMatchingEdgeDriver(int edgeMajor)
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                var latestVersion = GetLatestEdgeDriverVersion(edgeMajor);
                if (string.IsNullOrWhiteSpace(latestVersion))
                {
                    ScrapeLog.Warn($"Could not resolve latest EdgeDriver for major {edgeMajor}.");
                    return null;
                }

                var root = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ModemMerger", "drivers", "msedgedriver", latestVersion);
                Directory.CreateDirectory(root);

                var exePath = Path.Combine(root, "msedgedriver.exe");
                if (File.Exists(exePath) && GetFileMajorVersion(exePath) == edgeMajor)
                {
                    ScrapeLog.Info($"Using cached matching EdgeDriver at {exePath}");
                    return root;
                }

                var zipPath = Path.Combine(root, "edgedriver_win64.zip");
                var zipUrls = new[]
                {
                    $"https://msedgedriver.microsoft.com/{latestVersion}/edgedriver_win64.zip",
                    $"https://msedgedriver.azureedge.net/{latestVersion}/edgedriver_win64.zip"
                };

                bool downloadedZip = false;
                Exception lastDownloadErr = null;
                foreach (var zipUrl in zipUrls)
                {
                    try
                    {
                        ScrapeLog.Info($"Downloading EdgeDriver {latestVersion} from {zipUrl}");
                        using (var wc = new WebClient())
                            wc.DownloadFile(zipUrl, zipPath);
                        downloadedZip = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        lastDownloadErr = ex;
                        ScrapeLog.Warn($"Driver ZIP download failed from {zipUrl}: {ex.Message}");
                    }
                }

                if (!downloadedZip)
                    throw new InvalidOperationException(
                        $"Could not download EdgeDriver {latestVersion}. Last error: {lastDownloadErr?.Message}");

                ExtractZipWithPowerShell(zipPath, root);

                if (File.Exists(exePath) && GetFileMajorVersion(exePath) == edgeMajor)
                {
                    ScrapeLog.Info($"Downloaded matching EdgeDriver to {exePath}");
                    return root;
                }

                ScrapeLog.Warn("EdgeDriver download/extract completed but msedgedriver.exe was not usable.");
                return null;
            }
            catch (Exception ex)
            {
                ScrapeLog.Warn($"EdgeDriver auto-download failed: {ex.Message}");
                return null;
            }
        }

        private static string GetLatestEdgeDriverVersion(int edgeMajor)
        {
            var endpoints = new[]
            {
                $"https://msedgedriver.microsoft.com/LATEST_RELEASE_{edgeMajor}_WINDOWS",
                $"https://msedgedriver.microsoft.com/LATEST_RELEASE_{edgeMajor}",
                $"https://msedgedriver.azureedge.net/LATEST_RELEASE_{edgeMajor}_WINDOWS",
                $"https://msedgedriver.azureedge.net/LATEST_RELEASE_{edgeMajor}"
            };

            foreach (var ep in endpoints)
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        var bytes = wc.DownloadData(ep);
                        var variants = new[]
                        {
                            Encoding.UTF8.GetString(bytes),
                            Encoding.Unicode.GetString(bytes),
                            Encoding.BigEndianUnicode.GetString(bytes),
                            Encoding.ASCII.GetString(bytes)
                        };

                        foreach (var raw in variants)
                        {
                            if (string.IsNullOrWhiteSpace(raw)) continue;
                            var cleaned = raw
                                .Replace("\0", "")
                                .Trim('\uFEFF', '\uFFFE', ' ', '\r', '\n', '\t');
                            var m = Regex.Match(cleaned, @"\d+\.\d+\.\d+\.\d+");
                            if (m.Success)
                                return m.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScrapeLog.Warn($"Version lookup failed at {ep}: {ex.Message}");
                }
            }
            return null;
        }

        private static void ExtractZipWithPowerShell(string zipPath, string destinationDir)
        {
            var psZip = zipPath.Replace("'", "''");
            var psDst = destinationDir.Replace("'", "''");
            var cmd = $"Expand-Archive -LiteralPath '{psZip}' -DestinationPath '{psDst}' -Force";

            var psi = new ProcessStartInfo("powershell.exe", "-NoProfile -ExecutionPolicy Bypass -Command \"" + cmd + "\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var p = Process.Start(psi))
            {
                var stdout = p.StandardOutput.ReadToEnd();
                var stderr = p.StandardError.ReadToEnd();
                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"Expand-Archive failed (exit {p.ExitCode}). {stderr} {stdout}".Trim());
                }
            }
        }

        /// <summary>
        /// Kill any orphan msedge.exe processes that were launched with our
        /// custom user-data-dir. Without this, a crashed Edge keeps the profile
        /// directory locked and the next driver launch fails immediately.
        /// </summary>
        private static void KillOrphanEdgeProcesses()
        {
            try
            {
                // Normalise the path for a reliable command-line comparison
                var normalised = Path.GetFullPath(_userDataDir)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                // Single WMI query for all msedge processes — avoids per-PID round-trips
                // which can take several seconds each.
                var pidsToKill = new HashSet<int>();
                using (var searcher = new System.Management.ManagementObjectSearcher(
                    "SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'msedge.exe'"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        var cmdLine = obj["CommandLine"]?.ToString();
                        if (cmdLine != null && cmdLine.IndexOf(normalised, StringComparison.OrdinalIgnoreCase) >= 0)
                            pidsToKill.Add(Convert.ToInt32(obj["ProcessId"]));
                    }
                }

                foreach (var pid in pidsToKill)
                {
                    try
                    {
                        var proc = Process.GetProcessById(pid);
                        proc.Kill();
                        ScrapeLog.Warn($"Killed orphan msedge.exe PID {pid}");
                    }
                    catch { }
                }

                // Also kill orphan msedgedriver processes
                foreach (var proc in Process.GetProcessesByName("msedgedriver"))
                {
                    try { proc.Kill(); ScrapeLog.Warn($"Killed orphan msedgedriver PID {proc.Id}"); }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// Configure the APEX Interactive Report: set rows per page to max.
        /// Date range is already embedded in the navigation URL.
        /// </summary>
        private static void ConfigureApexIR(OpenQA.Selenium.IWebDriver driver,
            DateTime? dateFrom, DateTime? dateTo)
        {
            // Set rows per page to maximum via APEX Actions menu
            try
            {
                ClickActionsMenuItem(driver, "Rows Per Page");

                // Wait for submenu radio buttons to appear
                var menuItems = WaitForElements(driver,
                    "button[role='menuitemradio']", timeoutSeconds: 5);
                OpenQA.Selenium.IWebElement bestOption = null;
                int bestVal = 0;
                foreach (var mi in menuItems)
                {
                    var txt = mi.Text.Trim();
                    int val;
                    if (int.TryParse(txt, out val) && val > bestVal)
                    {
                        bestVal = val;
                        bestOption = mi;
                    }
                }
                if (bestOption != null)
                {
                    bestOption.Click();
                    WaitForApexReady(driver);
                }
            }
            catch (Exception ex)
            {
                ScrapeLog.Warn($"Rows per page setup failed: {ex.Message}");
            }

            // Close any open menus
            try
            {
                driver.FindElement(OpenQA.Selenium.By.TagName("body"))
                    .SendKeys(OpenQA.Selenium.Keys.Escape);
                Thread.Sleep(100);
            }
            catch { }
        }

        /// <summary>
        /// Click an item in the APEX IR "Actions" dropdown menu.
        /// </summary>
        private static void ClickActionsMenuItem(OpenQA.Selenium.IWebDriver driver, string itemText)
        {
            // Find and click the Actions button — use FindElements to avoid 3s implicit wait per miss
            string[] actionsSelectors = {
                "button.a-IRR-button--actions",
                ".a-IRR-buttons button",
                "button[data-action='show-ir-actions']",
                "button[title='Actions']",
            };
            OpenQA.Selenium.IWebElement actionsBtn = null;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            try
            {
                foreach (var sel in actionsSelectors)
                {
                    try
                    {
                        var els = driver.FindElements(OpenQA.Selenium.By.CssSelector(sel));
                        if (els.Count > 0 && els[0].Displayed) { actionsBtn = els[0]; break; }
                    }
                    catch { }
                }
                // Also try by text content
                if (actionsBtn == null)
                {
                    try
                    {
                        var els = driver.FindElements(OpenQA.Selenium.By.XPath(
                            "//button[contains(text(),'Actions') or contains(text(),'actions')]"));
                        if (els.Count > 0) actionsBtn = els[0];
                    }
                    catch { }
                }
            }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            }
            if (actionsBtn == null)
                throw new InvalidOperationException("Actions button not found on APEX IR page.");

            actionsBtn.Click();

            // Wait for menu items to appear
            var items = WaitForElements(driver,
                ".a-Menu-content a, .a-Menu-label, ul.a-Menu li, [role='menuitem'], .a-IRR-actions-menu a",
                timeoutSeconds: 5);
            foreach (var item in items)
            {
                try
                {
                    var txt = item.Text.Trim();
                    if (txt.IndexOf(itemText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.Click();
                        return;
                    }
                }
                catch { }
            }
            throw new InvalidOperationException($"Menu item '{itemText}' not found in Actions menu.");
        }

        /// <summary>
        /// Select an option in a &lt;select&gt; element by partial text match.
        /// </summary>
        private static void SelectOptionByPartialText(OpenQA.Selenium.IWebElement selectEl, string partialText)
        {
            var options = selectEl.FindElements(OpenQA.Selenium.By.TagName("option"));
            foreach (var opt in options)
            {
                if (opt.Text.IndexOf(partialText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    opt.Click();
                    return;
                }
            }
        }

        /// <summary>
        /// Extract all rows from an Oracle APEX Interactive Report, walking through pagination.
        /// Returns a list of dictionaries (column header → cell value).
        /// Uses JavaScript bulk extraction instead of per-cell Selenium calls for speed.
        /// </summary>
        private static List<Dictionary<string, string>> ScrapeApexIR(OpenQA.Selenium.IWebDriver driver)
        {
            var allRows = new List<Dictionary<string, string>>();
            int pageNum = 0;
            var js = driver as OpenQA.Selenium.IJavaScriptExecutor;

            // JavaScript that extracts all table data in one call (avoids hundreds of WebDriver round-trips)
            const string extractTableJs = @"
                var selectors = ['table.a-IRR-table', 'table[summary]', 'table.uReportStandard'];
                for (var s = 0; s < selectors.length; s++) {
                    var tbl = document.querySelector(selectors[s]);
                    if (!tbl) continue;
                    var headers = [];
                    var ths = tbl.querySelectorAll('th');
                    for (var i = 0; i < ths.length; i++) {
                        var t = ths[i].innerText.trim();
                        if (t.length > 0) headers.push(t);
                    }
                    var rows = tbl.querySelectorAll('tbody tr');
                    var data = [];
                    for (var r = 0; r < rows.length; r++) {
                        var cells = rows[r].querySelectorAll('td');
                        if (cells.length === 0) continue;
                        var obj = {};
                        for (var c = 0; c < cells.length; c++) {
                            var key = c < headers.length ? headers[c] : 'col_' + c;
                            obj[key] = cells[c].innerText.trim();
                        }
                        data.push(obj);
                    }
                    return JSON.stringify({found: true, rows: data});
                }
                return JSON.stringify({found: false, rows: []});
            ";

            while (true)
            {
                pageNum++;
                WaitForApexReady(driver);

                // Extract all rows from current page in a single JS call
                bool tableFound = false;
                if (js != null)
                {
                    try
                    {
                        var json = js.ExecuteScript(extractTableJs) as string;
                        if (json != null)
                        {
                            var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                            tableFound = parsed.ContainsKey("found") && (bool)parsed["found"];
                            if (tableFound)
                            {
                                var rowsArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(parsed["rows"].ToString());
                                allRows.AddRange(rowsArray);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ScrapeLog.Warn($"JS table extraction failed on page {pageNum}: {ex.Message}");
                    }
                }

                // Fallback to Selenium DOM walk if JS extraction failed
                if (!tableFound && js == null)
                {
                    string[] tableSelectors = {
                        "table.a-IRR-table",
                        "table[summary]",
                        "table.uReportStandard",
                    };
                    foreach (var sel in tableSelectors)
                    {
                        try
                        {
                            var tbl = driver.FindElement(OpenQA.Selenium.By.CssSelector(sel));
                            if (tbl == null) continue;
                            var thEls = driver.FindElements(OpenQA.Selenium.By.CssSelector(sel + " th"));
                            var headers = thEls.Select(th => th.Text.Trim()).Where(h => h.Length > 0).ToList();
                            var rows = driver.FindElements(OpenQA.Selenium.By.CssSelector(sel + " tbody tr"));
                            foreach (var row in rows)
                            {
                                try
                                {
                                    var cells = row.FindElements(OpenQA.Selenium.By.CssSelector("td"));
                                    if (cells.Count == 0) continue;
                                    var obj = new Dictionary<string, string>();
                                    for (int i = 0; i < cells.Count; i++)
                                    {
                                        var key = i < headers.Count ? headers[i] : $"col_{i}";
                                        obj[key] = cells[i].Text.Trim();
                                    }
                                    allRows.Add(obj);
                                }
                                catch { }
                            }
                            tableFound = true;
                            break;
                        }
                        catch { }
                    }
                }

                if (!tableFound)
                {
                    if (allRows.Count == 0) ScrapeLog.Warn("No APEX table found.");
                    break;
                }

                // Try to click Next — disable implicit wait to avoid 3s×4 = 12s delay on last page
                bool nextClicked = false;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                try
                {
                    string[] nextSelectors = {
                        "button[title='Next']",
                        "a[title='Next']",
                        ".a-IRR-pagination-item--next:not(.is-disabled)",
                        ".uButtonPaginationNext:not(:disabled)",
                    };
                    foreach (var ns in nextSelectors)
                    {
                        try
                        {
                            var found = driver.FindElements(OpenQA.Selenium.By.CssSelector(ns));
                            if (found.Count == 0) continue;
                            var nextBtn = found[0];
                            var disabled = nextBtn.GetAttribute("disabled");
                            var cls = nextBtn.GetAttribute("class") ?? "";
                            if (disabled == null && !cls.Contains("is-disabled") && !cls.Contains("disabled"))
                            {
                                nextBtn.Click();
                                nextClicked = true;
                                break;
                            }
                        }
                        catch { }
                    }
                }
                finally
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                }

                if (!nextClicked) break;
            }

            return allRows;
        }

        /// <summary>
        /// Parse raw table rows into KabalModem objects.
        /// </summary>
        private static List<KabalModem> ParseModems(List<Dictionary<string, string>> rows)
        {
            var modems = new List<KabalModem>();

            foreach (var row in rows)
            {
                // Normalise keys to lowercase
                var lc = row.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value);

                string modemNumber = "", packageName = "", kabalId = "", shippingDate = "", supplier = "", destination = "";

                foreach (var kv in lc)
                {
                    var val = kv.Value;
                    if (string.IsNullOrEmpty(val)) continue;

                    if (string.IsNullOrEmpty(kabalId) && Regex.IsMatch(val.Trim(), @"^[A-Z]{2,6}\d{4,10}$", RegexOptions.IgnoreCase))
                        kabalId = val.Trim();

                    if (string.IsNullOrEmpty(packageName) && (val.Contains("M-") || val.IndexOf("modem", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        packageName = val.Trim();
                        var m = Regex.Match(val, @"M-(\d+)", RegexOptions.IgnoreCase);
                        if (m.Success) modemNumber = m.Groups[1].Value;
                    }

                    if (string.IsNullOrEmpty(shippingDate) && Regex.IsMatch(val, @"\d{2}[-/\.]\d{2}[-/\.]\d{2,4}"))
                        shippingDate = val.Trim();
                    if (string.IsNullOrEmpty(shippingDate) && Regex.IsMatch(val, @"\d{4}-\d{2}-\d{2}"))
                        shippingDate = val.Trim();

                    if (string.IsNullOrEmpty(supplier) && Regex.IsMatch(val, @"HLB|Halliburton|Baker|Schlumberger|SLB", RegexOptions.IgnoreCase))
                        supplier = val.Trim();

                    if (string.IsNullOrEmpty(destination) && kv.Key.Contains("dest"))
                        destination = val.Trim();
                }

                if (string.IsNullOrEmpty(modemNumber))
                {
                    var m = Regex.Match(packageName + " " + kabalId, @"\b(\d{6,8})\b");
                    if (m.Success) modemNumber = m.Groups[1].Value;
                }

                if (!string.IsNullOrEmpty(modemNumber) || !string.IsNullOrEmpty(kabalId))
                {
                    modems.Add(new KabalModem
                    {
                        ModemNumber = modemNumber,
                        ShippingDate = shippingDate,
                        KabalId = kabalId,
                        PackageName = packageName,
                        Supplier = supplier,
                        Destination = destination,
                    });
                }
            }

            return modems;
        }
    }

    public static class TMUtilsClient
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private const string BaseUrl = "http://localhost:9001";

        public static async Task<TMUtilsCreationInputs> GetCreationInputsAsync()
        {
            var resp = await _http.GetAsync(BaseUrl + "/Modem/GetModemCreationInputs");
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TMUtilsCreationInputs>(body);
        }

        public static async Task<List<TMUtilsModem>> GetFromGantAsync(string customer, string rigId, string startDate)
        {
            var postData = $"Customer={Uri.EscapeDataString(customer)}&RigId={Uri.EscapeDataString(rigId)}&StartDate={Uri.EscapeDataString(startDate)}";
            var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _http.PostAsync(BaseUrl + "/Modem/GetFromGant", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TMUtilsModem>>(body) ?? new List<TMUtilsModem>();
        }

        public static async Task<MoveResult> MoveModemDatesAsync(int modemId, int numberOfDays)
        {
            var postData = $"ModemNumber={modemId}&NumberOfDays={numberOfDays}";
            var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _http.PostAsync(BaseUrl + "/Modem/MoveModemDates", content);
            var body = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<MoveResult>(body) ?? new MoveResult { Success = resp.IsSuccessStatusCode };
            }
            catch
            {
                return new MoveResult { Success = resp.IsSuccessStatusCode, Message = body };
            }
        }

        public static async Task<MoveResult> SetModemDatesAsync(int modemId, string loadoutDate, string dateLoad, string dateEta)
        {
            var postData = $"P_MODEM_ID={modemId}&P_LOADOUT_DATE={Uri.EscapeDataString(loadoutDate)}&P_DATE_LOAD={Uri.EscapeDataString(dateLoad)}&P_DATE_ETA={Uri.EscapeDataString(dateEta)}";
            var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _http.PostAsync(BaseUrl + "/Modem/MoveModemDates", content);
            var body = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<MoveResult>(body) ?? new MoveResult { Success = resp.IsSuccessStatusCode };
            }
            catch
            {
                return new MoveResult { Success = resp.IsSuccessStatusCode, Message = body };
            }
        }
    }

    // ── Gant client ──────────────────────────────────────────────────
    // Uses HttpWebRequest + DefaultNetworkCredentials + PreAuthenticate
    // — the same proven auth pattern that ModemDataPost uses for POSTs
    // to the same norwayappsprd host.

    public static class GantClient
    {
        private const string GantUrl = "http://norwayappsprd.corp.halliburton.com/pls/log_web/gant.fetch_data";

        // Shared cookie container so the Negotiate handshake cookie is reused
        private static readonly CookieContainer _cookies = new CookieContainer();

        // Bump connection limit for the modem host (default is 2, which serializes requests)
        static GantClient()
        {
            var sp = ServicePointManager.FindServicePoint(new Uri("http://norwayappsprd.corp.halliburton.com"));
            sp.ConnectionLimit = 15;
        }

        // Map UI customer name → Gant v_customer parameter
        private static readonly Dictionary<string, string> _customerIds =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Equinor",        "0000347501" },
            { "AkerBP",         "0000352210" },
            { "ConocoPhillips", "0000361542" },
            { "V\u00e5r Energi",     "0000317277" }
        };
        // Map UI customer name → partial match on Gant's CustomerName field.
        // Used to client-side filter when v_customer is empty (returns all customers).
        private static readonly Dictionary<string, string> _customerNameFilter =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Equinor",        "Equinor" },
            { "AkerBP",         "Aker" },
            { "ConocoPhillips", "Conoco" },
            { "Vår Energi",     "Vår" }
        };
        // Hardcoded rig → ship-to abbreviation (partial rig name match, checked before HTTP fetch)
        private static readonly Dictionary<string, string> _rigShipTo =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // HMF – Hammerfest
            { "Prospector",     "HMF" },
            { "Enabler",        "HMF" },
            // KSU – Kristiansund
            { "Heidrun",        "KSU" },
            { "Njord",          "KSU" },
            // FLO – Florø
            { "Promoter",       "FLO" },
            { "Snorre",         "FLO" },  // matches Snorre A and Snorre B
            // DUS – Dusavik
            { "W Linus",        "DUS" },
            { "Linus",          "DUS" },
            { "Ringhorne",      "DUS" },
            { "Pioneer",        "DUS" },
            // SSJ – Sandnessjøen (AkerBP SC8 only)
            { "SC8",            "SSJ" },
        };

        private static string RigToShipTo(string rigName)
        {
            if (string.IsNullOrWhiteSpace(rigName)) return null;
            foreach (var kv in _rigShipTo)
                if (rigName.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kv.Value;
            return null; // unknown – will fall back to HTTP fetch
        }

        public static async Task<List<GantModem>> FetchModemsAsync(string customerName, string rigFilter = null, DateTime? startDate = null)
        {
            string customerId;
            if (!_customerIds.TryGetValue(customerName, out customerId))
                customerId = "";

            var baseDate = startDate ?? DateTime.Today;
            var fra = baseDate.AddDays(-15).ToString("dd.MM.yyyy");
            var til = baseDate.AddDays(60).ToString("dd.MM.yyyy");

            var postBody =
                "tmp_fra=" + Uri.EscapeDataString(fra) +
                "&tmp_til=" + Uri.EscapeDataString(til) +
                "&v_customer=" + Uri.EscapeDataString(customerId) +
                "&v_rig_id=&show_all=&inc_mob=1&inc_zspo=0&no_rig=0&inc_mlt=";

            var html = await PostToGantAsync(postBody).ConfigureAwait(false);

            var modems = ParseGantHtml(html);

            // 1) Filter to selected rig (client-side on the lightweight Gant data)
            if (!string.IsNullOrEmpty(rigFilter) && rigFilter != "select rig")
                modems = modems
                    .Where(m => m.RigName != null &&
                                m.RigName.IndexOf(rigFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            // 3) Apply hardcoded ship-to map
            foreach (var m in modems)
            {
                var shipTo = RigToShipTo(m.RigName);
                if (shipTo != null)
                    m.PShipTo1 = shipTo;
            }

            // 4) Fetch view pages in parallel to get actual dates + ship-to
            var throttle = new SemaphoreSlim(20, 20);
            await Task.WhenAll(modems.Select(m => FetchModemDetailsAsync(m, throttle)));

            return modems;
        }

        private static async Task<string> GetFromGantAsync(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Credentials = CredentialCache.DefaultNetworkCredentials;
            req.PreAuthenticate = true;
            req.Method = "GET";
            req.CookieContainer = _cookies;
            req.KeepAlive = true;
            using (var resp = (HttpWebResponse)await req.GetResponseAsync().ConfigureAwait(false))
            using (var rdr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                return await rdr.ReadToEndAsync().ConfigureAwait(false);
        }

        private const string ModemEditUrl = "http://norwayappsprd.corp.halliburton.com/pls/log_web/mobssus_order_new$header_mc.QueryViewByKey?P_SSORD_ID=";
        private const string ModemViewUrl = "http://norwayappsprd.corp.halliburton.com/pls/log_web/mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID=";

        private static async Task FetchModemDetailsAsync(GantModem m, SemaphoreSlim throttle)
        {
            await throttle.WaitAsync().ConfigureAwait(false);
            try
            {
                var html = await GetFromGantAsync(ModemViewUrl + m.Id).ConfigureAwait(false);

                m.DateLoad    = ParseInputValueFast(html, "P_DATE_LOAD");
                m.LoadoutDate = ParseLabeledTdFast(html, "Loadout Date:");
                m.DateEta     = ParseLabeledTdFast(html, "Deliver To Customer:");

                if (string.IsNullOrEmpty(m.PShipTo1))
                    m.PShipTo1 = ParseHiddenByNameFast(html, "H_SHIPTO_1");

                m.HRefno = ParseHiddenByNameFast(html, "H_REFNO");
            }
            catch { /* best effort */ }
            finally { throttle.Release(); }
        }

        // ── Fast string-based parsers (no regex, ~10x faster on 46 KB pages) ──

        private static string ParseInputValueFast(string html, string fieldId)
        {
            // Look for id="FIELD" or id='FIELD'
            int idPos = IndexOfIgnoreCase(html, "id=\"" + fieldId + "\"", 0);
            if (idPos < 0) idPos = IndexOfIgnoreCase(html, "id='" + fieldId + "'", 0);
            if (idPos < 0) return "";

            // Find the enclosing <input or <span/<td tag start
            int tagStart = html.LastIndexOf('<', idPos);
            if (tagStart < 0) return "";

            // Find tag end
            int tagEnd = html.IndexOf('>', idPos);
            if (tagEnd < 0) return "";

            var tagStr = html.Substring(tagStart, tagEnd - tagStart + 1);

            // Extract value="..."
            int vPos = IndexOfIgnoreCase(tagStr, "value=\"", 0);
            if (vPos >= 0)
            {
                int vStart = vPos + 7;
                int vEnd = tagStr.IndexOf('"', vStart);
                if (vEnd > vStart) return tagStr.Substring(vStart, vEnd - vStart);
            }
            vPos = IndexOfIgnoreCase(tagStr, "value='", 0);
            if (vPos >= 0)
            {
                int vStart = vPos + 7;
                int vEnd = tagStr.IndexOf('\'', vStart);
                if (vEnd > vStart) return tagStr.Substring(vStart, vEnd - vStart);
            }

            // Fallback for <span id="FIELD">text</span>
            if (tagStr.EndsWith(">") && !tagStr.EndsWith("/>"))
            {
                int contentStart = tagEnd + 1;
                int closeTag = html.IndexOf('<', contentStart);
                if (closeTag > contentStart)
                    return html.Substring(contentStart, closeTag - contentStart).Trim();
            }

            return "";
        }

        private static string ParseLabeledTdFast(string html, string label)
        {
            // Pattern: <b>Label:</b></td><td ...>VALUE
            int pos = IndexOfIgnoreCase(html, "<b>" + label + "</b>", 0);
            if (pos < 0) return "";

            // Find next <td after the </td>
            int tdClose = IndexOfIgnoreCase(html, "</td>", pos);
            if (tdClose < 0) return "";
            int nextTd = IndexOfIgnoreCase(html, "<td", tdClose);
            if (nextTd < 0) return "";
            int gtPos = html.IndexOf('>', nextTd);
            if (gtPos < 0) return "";

            int valStart = gtPos + 1;
            int valEnd = html.IndexOf('<', valStart);
            if (valEnd < 0) return "";
            return html.Substring(valStart, valEnd - valStart).Trim();
        }

        private static string ParseHiddenByNameFast(string html, string name)
        {
            // Search for name="NAME" within an <input tag
            int pos = IndexOfIgnoreCase(html, "name=\"" + name + "\"", 0);
            if (pos < 0) pos = IndexOfIgnoreCase(html, "name='" + name + "'", 0);
            if (pos < 0) return "";

            // Find enclosing tag
            int tagStart = html.LastIndexOf('<', pos);
            if (tagStart < 0) return "";
            int tagEnd = html.IndexOf('>', pos);
            if (tagEnd < 0) return "";

            var tagStr = html.Substring(tagStart, tagEnd - tagStart + 1);

            int vPos = IndexOfIgnoreCase(tagStr, "value=\"", 0);
            if (vPos >= 0)
            {
                int vStart = vPos + 7;
                int vEnd = tagStr.IndexOf('"', vStart);
                if (vEnd > vStart) return tagStr.Substring(vStart, vEnd - vStart);
            }
            vPos = IndexOfIgnoreCase(tagStr, "value='", 0);
            if (vPos >= 0)
            {
                int vStart = vPos + 7;
                int vEnd = tagStr.IndexOf('\'', vStart);
                if (vEnd > vStart) return tagStr.Substring(vStart, vEnd - vStart);
            }
            return "";
        }

        private static int IndexOfIgnoreCase(string source, string value, int startIndex)
        {
            return source.IndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// POST via HttpWebRequest using the same auth pattern as ModemDataPost:
        ///   Credentials = DefaultNetworkCredentials, PreAuthenticate = true, CookieContainer.
        /// </summary>
        private static async Task<string> PostToGantAsync(string postBody)
        {
            byte[] data = Encoding.UTF8.GetBytes(postBody);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GantUrl);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.CookieContainer = _cookies;
            request.KeepAlive = true;

            using (var stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);

            using (var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        // Pre-compiled regex for Gant HTML parsing
        private static readonly Regex _infoBlockRx = new Regex(@"info\s*=\s*\{([\s\S]*?)\}", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _entryRx = new Regex(@"m(\d+)\s*:\s*'((?:[^'\\]|\\.)*)'", RegexOptions.Singleline | RegexOptions.Compiled);

        private static List<GantModem> ParseGantHtml(string html)
        {
            var result = new List<GantModem>();

            var infoMatch = _infoBlockRx.Match(html);
            if (!infoMatch.Success) return result;

            var infoBlock = infoMatch.Groups[1].Value;

            foreach (Match m in _entryRx.Matches(infoBlock))
            {
                int modemId;
                if (!int.TryParse(m.Groups[1].Value, out modemId)) continue;

                var raw = m.Groups[2].Value;
                var gm = new GantModem { Id = modemId };

                // The string uses \n as a literal two-char escape sequence in HTML source
                var parts = raw.Split(new string[] { @"\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var colon = part.IndexOf(':');
                    if (colon < 0) continue;
                    var key = part.Substring(0, colon).Trim();
                    var val = part.Substring(colon + 1).Trim();
                    switch (key)
                    {
                        case "Start Date":
                            // Format: DD.MM.YYYY HH:mm — keep only the date part
                            gm.StartDate = val.Length >= 10 ? val.Substring(0, 10) : val;
                            break;
                        case "Rig":          gm.RigName     = val; break;
                        case "Wellbore":     gm.WellName    = val; break;
                        case "Section":      gm.WellSection = val; break;
                        case "GantText":     gm.GantText    = val; break;
                        case "Modem Status": gm.ModemStatus = val; break;
                        case "Customer":     gm.CustomerName = val; break;
                    }
                }

                result.Add(gm);
            }

            return result;
        }

        // ── Shift modem dates directly via POST to norwayappsprd ─────────

        /// <summary>
        /// Shifts P_DATE_LOAD, P_LOADOUT_DATE, P_DATE_ETA by <paramref name="days"/> on the given modem.
        /// Walks form elements in DOM order (like TMUtils) — each field once, first-wins dedup.
        /// POST with PreAuthenticate=false → 401 NTLM → 200.
        /// </summary>
        public static async Task<ShiftResult> ShiftModemDatesAsync(int modemId, int days,
            bool shiftLoadout = true, bool shiftEta = true, bool shiftDateLoad = true)
        {
            return await Task.Run(() =>
            {
              try
              {
                // 1) GET modem edit page via ModemConnection (NTLM auth, iso-8859-1)
                var conn = new ModemConnection(HDocUtility.UrlModemEdit + modemId);
                var hDoc = conn.GetHtmlAsHdoc();

                // 2) Walk ALL form elements in DOM order, first-wins dedup.
                //    Data-row fields appear before dummy-row fields, so first-wins = data row.
                var formFields = new List<KeyValuePair<string, string>>();
                var seenNames  = new HashSet<string>(StringComparer.Ordinal);

                foreach (var node in hDoc.DocumentNode.Descendants()
                    .Where(n => n.Name == "input" || n.Name == "select" || n.Name == "textarea"))
                {
                    string name = node.GetAttributeValue("name", "");
                    if (string.IsNullOrEmpty(name)) continue;

                    string type = node.GetAttributeValue("type", "").ToLower();
                    if (type == "submit" || type == "button" || type == "reset") continue;

                    if (seenNames.Contains(name)) continue;
                    seenNames.Add(name);

                    string value;
                    if (node.Name == "select")
                    {
                        var opt = node.Descendants("option")
                            .FirstOrDefault(o => o.Attributes.Contains("selected"));
                        if (opt != null)
                        {
                            var v = opt.GetAttributeValue("value", "");
                            value = System.Net.WebUtility.HtmlDecode(
                                string.IsNullOrEmpty(v) ? opt.InnerText.Trim() : v);
                        }
                        else value = "";
                    }
                    else if (node.Name == "textarea")
                    {
                        value = System.Net.WebUtility.HtmlDecode(node.InnerText);
                    }
                    else // input
                    {
                        if (type == "checkbox")
                            value = node.Attributes.Contains("checked") ? "1" : "0";
                        else
                            value = System.Net.WebUtility.HtmlDecode(
                                node.GetAttributeValue("value", ""));
                    }

                    formFields.Add(new KeyValuePair<string, string>(name, value));
                }

                // 3) Shift the 3 date fields and override Z_ACTION
                string oldDateLoad = "", oldLoadout = "", oldEta = "";
                string newDateLoad = "", newLoadout = "", newEta = "";

                for (int i = 0; i < formFields.Count; i++)
                {
                    var f = formFields[i];
                    switch (f.Key)
                    {
                        case "P_DATE_LOAD":
                            oldDateLoad = f.Value;
                            if (shiftDateLoad)
                            {
                                newDateLoad = ShiftDateField(f.Value, days);
                                formFields[i] = new KeyValuePair<string, string>(f.Key, newDateLoad);
                            }
                            else
                                newDateLoad = f.Value;
                            break;
                        case "P_LOADOUT_DATE":
                            oldLoadout = f.Value;
                            if (shiftLoadout)
                            {
                                newLoadout = ShiftDateField(f.Value, days);
                                formFields[i] = new KeyValuePair<string, string>(f.Key, newLoadout);
                            }
                            else
                                newLoadout = f.Value;
                            break;
                        case "P_DATE_ETA":
                            oldEta = f.Value;
                            if (shiftEta)
                            {
                                newEta = ShiftDateField(f.Value, days);
                                formFields[i] = new KeyValuePair<string, string>(f.Key, newEta);
                            }
                            else
                                newEta = f.Value;
                            break;
                        case "P_SSORD_ID":
                            formFields[i] = new KeyValuePair<string, string>(f.Key, modemId.ToString());
                            break;
                        case "Z_ACTION":
                            formFields[i] = new KeyValuePair<string, string>(f.Key, ""); // must be empty
                            break;
                    }
                }

                // 4) Build POST body in DOM order using iso-8859-1 encoding
                var sb = new StringBuilder();
                foreach (var f in formFields)
                {
                    if (sb.Length > 0) sb.Append('&');
                    sb.Append(HDocUtility.FormUrlEncode(f.Key));
                    sb.Append('=');
                    sb.Append(HDocUtility.FormUrlEncode(f.Value));
                }

                // 5) POST with PreAuthenticate=false → 401 NTLM → 200
                byte[] postBytes = HDocUtility.CurrentEncoding.GetBytes(sb.ToString());

                HttpWebRequest postReq = (HttpWebRequest)WebRequest.Create(HDocUtility.UrlModemHeaderUpdate);
                postReq.Credentials      = CredentialCache.DefaultNetworkCredentials;
                postReq.PreAuthenticate  = false;
                postReq.Method           = "POST";
                postReq.ContentType      = "application/x-www-form-urlencoded";
                postReq.ContentLength    = postBytes.Length;
                postReq.CookieContainer  = new CookieContainer();

                using (var stream = postReq.GetRequestStream())
                    stream.Write(postBytes, 0, postBytes.Length);

                string responseText;
                using (var postResp = (HttpWebResponse)postReq.GetResponse())
                using (var reader = new StreamReader(postResp.GetResponseStream(), HDocUtility.CurrentEncoding))
                    responseText = reader.ReadToEnd();

                // 6) Check response for Oracle / form errors
                var oraErr = Regex.Match(responseText, @"ORA-\d+[^<]*", RegexOptions.IgnoreCase);
                if (oraErr.Success)
                    throw new InvalidOperationException("Oracle DB error: " + oraErr.Value.Trim());
                var appErr = Regex.Match(responseText, @"Error!</b>\s*<br[^>]*>\s*([^<]{1,200})", RegexOptions.IgnoreCase);
                if (appErr.Success)
                    throw new InvalidOperationException("Form error: " + appErr.Groups[1].Value.Trim());

                // 7) Verify the dates actually changed by re-reading the edit page
                var verifyConn = new ModemConnection(HDocUtility.UrlModemEdit + modemId);
                var verifyDoc  = verifyConn.GetHtmlAsHdoc();
                var actualLoad = System.Net.WebUtility.HtmlDecode(
                    verifyDoc.DocumentNode.SelectSingleNode("//input[@name='P_DATE_LOAD']")
                        ?.GetAttributeValue("value", "") ?? "");
                var actualLoadout = System.Net.WebUtility.HtmlDecode(
                    verifyDoc.DocumentNode.SelectSingleNode("//input[@name='P_LOADOUT_DATE']")
                        ?.GetAttributeValue("value", "") ?? "");
                var actualEta = System.Net.WebUtility.HtmlDecode(
                    verifyDoc.DocumentNode.SelectSingleNode("//input[@name='P_DATE_ETA']")
                        ?.GetAttributeValue("value", "") ?? "");

                if ((!shiftDateLoad || actualLoad == oldDateLoad) &&
                    (!shiftLoadout || actualLoadout == oldLoadout) &&
                    (!shiftEta || actualEta == oldEta))
                    throw new InvalidOperationException(
                        "POST returned 200 but dates unchanged — update had no effect");

                return new ShiftResult
                {
                    Success        = true,
                    NewDateLoad    = newDateLoad,
                    NewLoadoutDate = newLoadout,
                    NewDateEta     = newEta
                };
              }
              catch (Exception ex)
              {
                  return new ShiftResult { Success = false, Error = ex.Message };
              }
            });
        }

        private static string ShiftDateField(string value, int days)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            // Try DD.MM.YYYY HH:mm first, then DD.MM.YYYY
            DateTime dt;
            if (DateTime.TryParseExact(value.Trim(), "dd.MM.yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.AddDays(days).ToString("dd.MM.yyyy HH:mm");

            if (DateTime.TryParseExact(value.Trim(), "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.AddDays(days).ToString("dd.MM.yyyy");

            return value; // fallback — leave unchanged
        }


    }

    public class ShiftResult
    {
        public bool Success { get; set; }
        public string NewDateLoad { get; set; }
        public string NewLoadoutDate { get; set; }
        public string NewDateEta { get; set; }
        public string Error { get; set; }   // non-null when Success == false
    }
}
