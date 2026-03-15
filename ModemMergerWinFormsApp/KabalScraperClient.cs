using HtmlAgilityPack;
using ModemWebUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    /// Rolling file logger for the Kabal scraper. Writes timestamped entries to
    /// %APPDATA%\ModemMerger\scraper-log.txt (keeps the last run only).
    /// Call <see cref="Start"/> at the beginning of a scrape, then <see cref="Info"/>/<see cref="Warn"/>/<see cref="Error"/>
    /// throughout. <see cref="SavePageSnapshot"/> captures URL + page-source excerpt + screenshot.
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

            // Overwrite previous run
            File.WriteAllText(_logPath,
                $"=== Kabal Scraper Log — {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
        }

        public static void Info(string message)  => Write("INFO ", message);
        public static void Warn(string message)  => Write("WARN ", message);
        public static void Error(string message)  => Write("ERROR", message);

        private static void Write(string level, string message)
        {
            if (_logPath == null) return;
            var line = $"[{DateTime.Now:HH:mm:ss.fff}] {level}  {message}{Environment.NewLine}";
            lock (_lock) { File.AppendAllText(_logPath, line); }
        }

        /// <summary>
        /// Dump current URL, page title, first 2000 chars of page source, and a screenshot.
        /// </summary>
        public static void SavePageSnapshot(OpenQA.Selenium.IWebDriver driver, string label)
        {
            try
            {
                Info($"[{label}] URL = {driver.Url}");
                Info($"[{label}] Title = {driver.Title}");

                var src = driver.PageSource ?? "";
                if (src.Length > 2000) src = src.Substring(0, 2000) + "… (truncated)";
                Info($"[{label}] PageSource ↓\n{src}");

                // Save screenshot
                var ss = driver as OpenQA.Selenium.ITakesScreenshot;
                if (ss != null)
                {
                    var shot = ss.GetScreenshot();
                    var shotPath = Path.Combine(_logDir, label.Replace(" ", "_") + ".png");
                    shot.SaveAsFile(shotPath, OpenQA.Selenium.ScreenshotImageFormat.Png);
                    Info($"[{label}] Screenshot → {shotPath}");
                }
            }
            catch (Exception ex) { Warn($"[{label}] Snapshot failed: {ex.Message}"); }
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
    }

    // ── Clients ──────────────────────────────────────────────────────────

    public static class KabalScraperClient
    {
        // Login entry point — triggers SSO and lands back on the APEX app after auth
        private const string LoginUrl = "https://account01.kabal.com/w/web/r/wels/kabal-account/";

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
            Action<string> onStatus = null)
        {
            return await Task.Run(() =>
            {
                ScrapeLog.Start();
                ScrapeLog.Info($"Operator={operatorName}, Rig={rigName}, Headless={headless}, DryRun={dryRun}");

                Microsoft.Edge.SeleniumTools.EdgeOptions options = null;
                OpenQA.Selenium.IWebDriver driver = null;
                try
                {
                    options = new Microsoft.Edge.SeleniumTools.EdgeOptions();
                    options.UseChromium = true;
                    if (headless)
                    {
                        options.AddArgument("--headless=new");
                        options.AddArgument("--disable-gpu");
                    }
                    options.AddArgument("--no-sandbox");
                    options.AddArgument("--disable-dev-shm-usage");
                    options.AddArgument("--disable-blink-features=AutomationControlled");
                    options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36 Edg/145.0.0.0");
                    options.AddExcludedArgument("enable-automation");

                    // Find cached msedgedriver to bypass Selenium Manager (firewall blocks downloads)
                    var driverDir = FindCachedEdgeDriver();
                    ScrapeLog.Info($"EdgeDriver dir = {driverDir ?? "(none — using default)"}");
                    if (driverDir != null)
                    {
                        var svc = Microsoft.Edge.SeleniumTools.EdgeDriverService.CreateChromiumService(driverDir);
                        svc.HideCommandPromptWindow = true;
                        driver = new Microsoft.Edge.SeleniumTools.EdgeDriver(svc, options);
                    }
                    else
                    {
                        driver = new Microsoft.Edge.SeleniumTools.EdgeDriver(options);
                    }
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    ScrapeLog.Info("EdgeDriver started OK.");

                    // ── Step 1: Login ──
                    onStatus?.Invoke("Navigating to Kabal login...");
                    ScrapeLog.Info($"Navigating to {LoginUrl}");
                    driver.Navigate().GoToUrl(LoginUrl);
                    System.Threading.Thread.Sleep(2000);
                    ScrapeLog.SavePageSnapshot(driver, "01_after_login_nav");

                    onStatus?.Invoke("Entering credentials...");
                    ScrapeLog.Info("Looking for username field (input[type='text'], input[type='email'])...");
                    var usernameField = WaitFor(driver, d =>
                    {
                        try
                        {
                            var el = d.FindElement(OpenQA.Selenium.By.CssSelector("input[type='text'], input[type='email']"));
                            return el.Displayed ? el : null;
                        }
                        catch { return null; }
                    });
                    ScrapeLog.Info("Username field found — entering credentials.");
                    usernameField.Clear();
                    usernameField.SendKeys(username);

                    // Submit username (multi-step login: username first, then password)
                    ScrapeLog.Info("Submitting username (step 1)...");
                    try
                    {
                        driver.FindElement(OpenQA.Selenium.By.CssSelector("button[type='submit']")).Click();
                    }
                    catch
                    {
                        // Some APEX forms use input[type=submit] or a regular button
                        try { driver.FindElement(OpenQA.Selenium.By.CssSelector("input[type='submit']")).Click(); }
                        catch { usernameField.SendKeys(OpenQA.Selenium.Keys.Return); }
                    }
                    System.Threading.Thread.Sleep(2000);
                    ScrapeLog.SavePageSnapshot(driver, "02_after_username_submit");

                    // Wait for password field (may be on same page or new page)
                    onStatus?.Invoke("Entering password...");
                    ScrapeLog.Info("Waiting for password field...");
                    var passwordField = WaitFor(driver, d =>
                    {
                        try
                        {
                            var el = d.FindElement(OpenQA.Selenium.By.CssSelector("input[type='password']"));
                            return el.Displayed ? el : null;
                        }
                        catch { return null; }
                    });
                    ScrapeLog.Info("Password field found.");
                    passwordField.Clear();
                    passwordField.SendKeys(password);

                    ScrapeLog.Info("Submitting password (step 2)...");
                    try
                    {
                        driver.FindElement(OpenQA.Selenium.By.CssSelector("button[type='submit']")).Click();
                    }
                    catch
                    {
                        try { driver.FindElement(OpenQA.Selenium.By.CssSelector("input[type='submit']")).Click(); }
                        catch { passwordField.SendKeys(OpenQA.Selenium.Keys.Return); }
                    }
                    System.Threading.Thread.Sleep(3000);
                    ScrapeLog.SavePageSnapshot(driver, "03_after_password_submit");

                    // ── Step 2: Operator selection (if present) ──
                    string operatorDisplay;
                    if (!OperatorMapping.TryGetValue(operatorName, out operatorDisplay))
                        operatorDisplay = operatorName;

                    try
                    {
                        onStatus?.Invoke($"Selecting operator: {operatorDisplay}...");
                        ScrapeLog.Info($"Looking for operator link: '{operatorDisplay}'");
                        var opLink = WaitFor(driver, d =>
                        {
                            try
                            {
                                var el = d.FindElement(OpenQA.Selenium.By.XPath($"//*[contains(text(), '{operatorDisplay}')]"));
                                return el.Displayed ? el : null;
                            }
                            catch { return null; }
                        });
                        ScrapeLog.Info("Operator link found — clicking.");
                        opLink.Click();
                        System.Threading.Thread.Sleep(3000);
                        ScrapeLog.SavePageSnapshot(driver, "04_after_operator_select");
                    }
                    catch (Exception opEx)
                    {
                        ScrapeLog.Warn($"Operator selector not found (may already be in app): {opEx.Message}");
                    }

                    // ── Step 3: Navigate to operator APEX app page ──
                    string targetUrl;
                    if (!OperatorAppUrls.TryGetValue(operatorName, out targetUrl))
                        targetUrl = OperatorAppUrls["Equinor"];

                    onStatus?.Invoke($"Navigating to {operatorName} loadout page...");
                    ScrapeLog.Info($"Navigating to APEX app: {targetUrl}");
                    driver.Navigate().GoToUrl(targetUrl);
                    System.Threading.Thread.Sleep(3000);
                    ScrapeLog.SavePageSnapshot(driver, "05_after_apex_nav");

                    // Verify not redirected back to login
                    if (driver.Url.Contains("login") || driver.Url.Contains("kabal-account:login"))
                    {
                        ScrapeLog.Error($"Redirected back to login. Current URL: {driver.Url}");
                        return new ScraperResult { Success = false, Error = "Redirected back to login — credentials may be incorrect or SSO session expired." };
                    }

                    // ── Step 4: Scrape APEX Interactive Report ──
                    onStatus?.Invoke("Scraping APEX table...");
                    ScrapeLog.Info("Starting APEX IR scrape...");
                    var allRows = ScrapeApexIR(driver);
                    ScrapeLog.Info($"ScrapeApexIR returned {allRows.Count} rows.");
                    if (allRows.Count > 0)
                        ScrapeLog.Info($"First row keys: {string.Join(", ", allRows[0].Keys)}");

                    // ── Step 5: Parse modem records ──
                    var modems = ParseModems(allRows);
                    ScrapeLog.Info($"ParseModems → {modems.Count} modems.");
                    onStatus?.Invoke($"Parsed {modems.Count} modem records from {allRows.Count} rows.");

                    ScrapeLog.Info("Scrape completed successfully.");
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
                    if (driver != null) ScrapeLog.SavePageSnapshot(driver, "XX_error");
                    ScrapeLog.Error(ex.StackTrace);
                    return new ScraperResult { Success = false, Error = ex.Message };
                }
                finally
                {
                    try { driver?.Quit(); } catch { }
                    ScrapeLog.Info("Driver quit. Log closed.");
                }
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
            int attempts = 0;
            while (DateTime.UtcNow < deadline)
            {
                attempts++;
                var el = condition(driver);
                if (el != null)
                {
                    ScrapeLog.Info($"WaitFor (line {callerLine}) succeeded after {attempts} attempts.");
                    return el;
                }
                System.Threading.Thread.Sleep(500);
            }
            ScrapeLog.Error($"WaitFor (line {callerLine}) timed out after {timeoutSeconds}s / {attempts} attempts.");
            ScrapeLog.SavePageSnapshot(driver, $"WaitFor_timeout_L{callerLine}");
            throw new TimeoutException($"WaitFor timed out after {timeoutSeconds}s");
        }

        /// <summary>
        /// Find the cached msedgedriver directory under %USERPROFILE%\.cache\selenium\msedgedriver.
        /// Returns the directory containing msedgedriver.exe, or null if not found.
        /// </summary>
        private static string FindCachedEdgeDriver()
        {
            var cacheRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache", "selenium", "msedgedriver", "win64");
            if (!Directory.Exists(cacheRoot)) return null;

            // Pick the newest version folder containing msedgedriver.exe
            foreach (var dir in Directory.GetDirectories(cacheRoot)
                         .OrderByDescending(d => d))
            {
                if (File.Exists(Path.Combine(dir, "msedgedriver.exe")))
                    return dir;
            }
            return null;
        }

        /// <summary>
        /// Extract all rows from an Oracle APEX Interactive Report, walking through pagination.
        /// Returns a list of dictionaries (column header → cell value).
        /// </summary>
        private static List<Dictionary<string, string>> ScrapeApexIR(OpenQA.Selenium.IWebDriver driver)
        {
            var allRows = new List<Dictionary<string, string>>();
            int pageNum = 0;

            while (true)
            {
                pageNum++;
                System.Threading.Thread.Sleep(1500);

                var headers = new List<string>();
                bool tableFound = false;

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
                        ScrapeLog.Info($"[Page {pageNum}] Found table with selector '{sel}'");

                        // Collect headers
                        var thEls = driver.FindElements(OpenQA.Selenium.By.CssSelector(sel + " th"));
                        headers = thEls.Select(th => th.Text.Trim()).Where(h => h.Length > 0).ToList();
                        ScrapeLog.Info($"[Page {pageNum}] Headers ({headers.Count}): {string.Join(" | ", headers)}");

                        // Collect data rows
                        var rows = driver.FindElements(OpenQA.Selenium.By.CssSelector(sel + " tbody tr"));
                        int rowsBefore = allRows.Count;
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
                            catch { /* skip malformed row */ }
                        }
                        tableFound = true;
                        ScrapeLog.Info($"[Page {pageNum}] Collected {allRows.Count - rowsBefore} rows (total: {allRows.Count}).");
                        break;
                    }
                    catch { /* selector not found, try next */ }
                }

                if (!tableFound)
                {
                    ScrapeLog.Warn($"[Page {pageNum}] No table found with any selector. Stopping pagination.");
                    if (allRows.Count == 0) ScrapeLog.SavePageSnapshot(driver, "05_no_table_found");
                    break;
                }

                // Try to click Next pagination button
                bool nextClicked = false;
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
                        var nextBtn = driver.FindElement(OpenQA.Selenium.By.CssSelector(ns));
                        var disabled = nextBtn.GetAttribute("disabled");
                        var cls = nextBtn.GetAttribute("class") ?? "";
                        if (disabled == null && !cls.Contains("is-disabled") && !cls.Contains("disabled"))
                        {
                            nextBtn.Click();
                            nextClicked = true;
                            break;
                        }
                    }
                    catch { /* not found */ }
                }

                if (!nextClicked)
                {
                    ScrapeLog.Info($"[Page {pageNum}] No next button — pagination complete.");
                    break;
                }
                ScrapeLog.Info($"[Page {pageNum}] Next button clicked — loading next page...");
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
            var throttle = new SemaphoreSlim(10, 10);
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
            bool shiftLoadout = true, bool shiftEta = true)
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
                            newDateLoad = ShiftDateField(f.Value, days);
                            formFields[i] = new KeyValuePair<string, string>(f.Key, newDateLoad);
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

                if (actualLoad == oldDateLoad &&
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
