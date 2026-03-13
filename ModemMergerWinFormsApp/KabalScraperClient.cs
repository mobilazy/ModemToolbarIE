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
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        private const string ScraperBaseUrl = "http://localhost:3000";

        public static async Task<bool> IsRunningAsync()
        {
            try
            {
                var resp = await _http.GetAsync(ScraperBaseUrl + "/health");
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<ScraperResult> ScrapeAsync(
            string operatorName,
            string rigName,
            string username,
            string password,
            bool headless = true,
            bool dryRun = true)
        {
            var payload = new
            {
                @operator = operatorName,
                rigName = rigName,
                username = username,
                password = password,
                headless = headless,
                dryRun = dryRun
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _http.PostAsync(ScraperBaseUrl + "/scrape", content);
            }
            catch (Exception ex)
            {
                return new ScraperResult { Success = false, Error = "Cannot reach scraper at localhost:3000 — " + ex.Message };
            }

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new ScraperResult { Success = false, Error = $"Scraper returned {(int)response.StatusCode}: {body}" };

            try
            {
                return JsonConvert.DeserializeObject<ScraperResult>(body)
                       ?? new ScraperResult { Success = false, Error = "Empty response from scraper" };
            }
            catch (Exception ex)
            {
                return new ScraperResult { Success = false, Error = "Failed to parse scraper response: " + ex.Message };
            }
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
        public static async Task<ShiftResult> ShiftModemDatesAsync(int modemId, int days)
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
                            newLoadout = ShiftDateField(f.Value, days);
                            formFields[i] = new KeyValuePair<string, string>(f.Key, newLoadout);
                            break;
                        case "P_DATE_ETA":
                            oldEta = f.Value;
                            newEta = ShiftDateField(f.Value, days);
                            formFields[i] = new KeyValuePair<string, string>(f.Key, newEta);
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

                if (actualLoad == oldDateLoad && actualLoadout == oldLoadout && actualEta == oldEta)
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
