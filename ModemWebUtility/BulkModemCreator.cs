using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModemWebUtility
{
    // ── DTOs ──────────────────────────────────────────────────────────

    public class BulkCreateRequest
    {
        public int SourceModemId { get; set; }
        public string SectionFlags { get; set; } = "ORDERCOPY:H:B1:B2:M1:M2:L";
        public string LoadoutDate { get; set; }
        public string DateEta { get; set; }
        public string WellName { get; set; }
        public string SectionName { get; set; }
    }

    public class BulkCreateResult
    {
        public int SourceModemId { get; set; }
        public int NewModemId { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
        public string WellName { get; set; }
        public string SectionName { get; set; }
    }

    // ── Bulk creator ─────────────────────────────────────────────────

    public static class BulkModemCreator
    {
        private const string CopyUrl =
            "http://norwayappsprd.corp.halliburton.com/pls/log_web/mobss_order_copy_exec";

        // Reuse a cookie container so NTLM handshake cookie persists across calls
        private static readonly CookieContainer _cookies = new CookieContainer();

        /// <summary>
        /// Copy a modem via the server-side bulk copy endpoint.
        /// Returns the new modem ID extracted from the response.
        /// </summary>
        public static async Task<int> CopyModemAsync(int sourceModemId, string sParameters)
        {
            if (string.IsNullOrWhiteSpace(sParameters))
                sParameters = "ORDERCOPY:H:B1:B2:M1:M2:L";

            var url = $"{CopyUrl}?nOrderId={sourceModemId}&sParameters={Uri.EscapeDataString(sParameters)}";
            var html = await GetAsync(url).ConfigureAwait(false);

            // Extract new modem ID from the response
            var match = Regex.Match(html, @"P_SSORD_ID=(\d+)");
            if (!match.Success)
            {
                // Try a broader pattern — response may contain just the new order number
                match = Regex.Match(html, @"\b(\d{6,8})\b");
            }

            if (!match.Success)
                throw new InvalidOperationException(
                    $"Could not extract new modem ID from copy response (source={sourceModemId}). Response length={html.Length}");

            int newId = int.Parse(match.Groups[1].Value);

            // Verify the new modem actually exists by checking the edit page
            var verifyUrl = HDocUtility.UrlModemEdit + newId;
            var verifyHtml = await GetAsync(verifyUrl).ConfigureAwait(false);
            if (verifyHtml.Length < 500 || verifyHtml.IndexOf("P_SSORD_ID", StringComparison.OrdinalIgnoreCase) < 0)
                throw new InvalidOperationException(
                    $"New modem {newId} edit page did not load correctly (response length={verifyHtml.Length})");

            return newId;
        }

        /// <summary>
        /// Set absolute dates on a modem (header fields only).
        /// Uses the same form-walk + POST pattern as GantClient.ShiftModemDatesAsync
        /// but with absolute date values instead of relative shift.
        /// </summary>
        public static async Task SetModemDatesAsync(int modemId, string loadoutDate, string dateEta)
        {
            await Task.Run(() =>
            {
                var conn = new ModemConnection(HDocUtility.UrlModemEdit + modemId);
                var hDoc = conn.GetHtmlAsHdoc();

                // Walk ALL form elements in DOM order, first-wins dedup
                var formFields = new List<KeyValuePair<string, string>>();
                var seenNames = new HashSet<string>(StringComparer.Ordinal);

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
                        value = opt != null
                            ? System.Net.WebUtility.HtmlDecode(
                                string.IsNullOrEmpty(opt.GetAttributeValue("value", ""))
                                    ? opt.InnerText.Trim()
                                    : opt.GetAttributeValue("value", ""))
                            : "";
                    }
                    else if (node.Name == "textarea")
                    {
                        value = System.Net.WebUtility.HtmlDecode(node.InnerText);
                    }
                    else
                    {
                        if (type == "checkbox")
                            value = node.Attributes.Contains("checked") ? "1" : "0";
                        else
                            value = System.Net.WebUtility.HtmlDecode(
                                node.GetAttributeValue("value", ""));
                    }

                    formFields.Add(new KeyValuePair<string, string>(name, value));
                }

                // Override date fields with absolute values
                for (int i = 0; i < formFields.Count; i++)
                {
                    var f = formFields[i];
                    switch (f.Key)
                    {
                        case "P_LOADOUT_DATE":
                            if (!string.IsNullOrEmpty(loadoutDate))
                                formFields[i] = new KeyValuePair<string, string>(f.Key, loadoutDate);
                            break;
                        case "P_DATE_ETA":
                            if (!string.IsNullOrEmpty(dateEta))
                                formFields[i] = new KeyValuePair<string, string>(f.Key, dateEta);
                            break;
                        case "P_DATE_LOAD":
                            // Set P_DATE_LOAD same as P_LOADOUT_DATE when provided
                            if (!string.IsNullOrEmpty(loadoutDate))
                                formFields[i] = new KeyValuePair<string, string>(f.Key, loadoutDate);
                            break;
                        case "P_SSORD_ID":
                            formFields[i] = new KeyValuePair<string, string>(f.Key, modemId.ToString());
                            break;
                        case "Z_ACTION":
                            formFields[i] = new KeyValuePair<string, string>(f.Key, "");
                            break;
                    }
                }

                // Build POST body in DOM order using iso-8859-1 encoding
                var sb = new StringBuilder();
                foreach (var f in formFields)
                {
                    if (sb.Length > 0) sb.Append('&');
                    sb.Append(HDocUtility.FormUrlEncode(f.Key));
                    sb.Append('=');
                    sb.Append(HDocUtility.FormUrlEncode(f.Value));
                }

                byte[] postBytes = HDocUtility.CurrentEncoding.GetBytes(sb.ToString());

                HttpWebRequest postReq = (HttpWebRequest)WebRequest.Create(HDocUtility.UrlModemHeaderUpdate);
                postReq.Credentials = CredentialCache.DefaultNetworkCredentials;
                postReq.PreAuthenticate = false;
                postReq.Method = "POST";
                postReq.ContentType = "application/x-www-form-urlencoded";
                postReq.ContentLength = postBytes.Length;
                postReq.CookieContainer = new CookieContainer();

                using (var stream = postReq.GetRequestStream())
                    stream.Write(postBytes, 0, postBytes.Length);

                string responseText;
                using (var postResp = (HttpWebResponse)postReq.GetResponse())
                using (var reader = new StreamReader(postResp.GetResponseStream(), HDocUtility.CurrentEncoding))
                    responseText = reader.ReadToEnd();

                // Check for Oracle / form errors
                var oraErr = Regex.Match(responseText, @"ORA-\d+[^<]*", RegexOptions.IgnoreCase);
                if (oraErr.Success)
                    throw new InvalidOperationException("Oracle DB error: " + oraErr.Value.Trim());
                var appErr = Regex.Match(responseText, @"Error!</b>\s*<br[^>]*>\s*([^<]{1,200})", RegexOptions.IgnoreCase);
                if (appErr.Success)
                    throw new InvalidOperationException("Form error: " + appErr.Groups[1].Value.Trim());
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Orchestrate bulk creation: copy each modem, then optionally set dates.
        /// Runs sequentially to avoid server-side race conditions on order IDs.
        /// </summary>
        public static async Task<List<BulkCreateResult>> BulkCreateAsync(
            List<BulkCreateRequest> requests,
            Action<string> onStatus = null)
        {
            var results = new List<BulkCreateResult>();
            int done = 0;

            foreach (var req in requests)
            {
                done++;
                var result = new BulkCreateResult
                {
                    SourceModemId = req.SourceModemId,
                    WellName = req.WellName,
                    SectionName = req.SectionName,
                };

                try
                {
                    onStatus?.Invoke($"Copying modem {req.SourceModemId} ({done}/{requests.Count}) — {req.WellName} {req.SectionName}...");

                    int newId = await CopyModemAsync(req.SourceModemId, req.SectionFlags).ConfigureAwait(false);
                    result.NewModemId = newId;

                    // Set dates if provided
                    if (!string.IsNullOrEmpty(req.LoadoutDate) || !string.IsNullOrEmpty(req.DateEta))
                    {
                        onStatus?.Invoke($"Setting dates on new modem {newId}...");
                        await SetModemDatesAsync(newId, req.LoadoutDate, req.DateEta).ConfigureAwait(false);
                    }

                    result.Success = true;
                    onStatus?.Invoke($"Created modem {newId} from {req.SourceModemId}.");
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Error = ex.Message;
                    onStatus?.Invoke($"Failed to copy {req.SourceModemId}: {ex.Message}");
                }

                results.Add(result);
            }

            return results;
        }

        // ── HTTP helper (same NTLM auth pattern as GantClient) ──────

        private static async Task<string> GetAsync(string url)
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
    }
}
