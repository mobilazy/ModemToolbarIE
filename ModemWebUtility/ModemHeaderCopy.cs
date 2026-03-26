using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ModemWebUtility
{
    public class ModemHeaderCopy
    {
        private string sourceModemNo;
        private string targetModemNo;

        // Fields to never override — keep target's own values
        private static readonly HashSet<string> _skipOverride = new HashSet<string>(StringComparer.Ordinal)
        {
            "P_SSORD_ID", "P_10"
        };

        // Only these specific header fields get copied from source to target.
        private static readonly HashSet<string> _copyFields = new HashSet<string>(StringComparer.Ordinal)
        {
            // 3 dates
            "P_DATE_LOAD", "P_LOADOUT_DATE", "P_DATE_ETA",
            // Ship from 1-5
            "P_SHIPFROM_1", "P_SHIPFROM_2", "P_SHIPFROM_3", "P_SHIPFROM_4", "P_SHIPFROM_5",
            // Ship from location dropdown (Sperry Drilling Services)
            "P_L_SHIPFROM_LOC2",
            // Job duration
            "P_MOB_DURATION",
            // Well Section
            "P_WELL_SECTION",
            // Reason for shipment
            "P_REASON_FOR_SHIPMENT"
        };

        public ModemHeaderCopy(string sourceModemNo, string targetModemNo)
        {
            this.sourceModemNo = sourceModemNo;
            this.targetModemNo = targetModemNo;
        }

        public bool CopyHeaderFields()
        {
            // 1) Read SOURCE field values.
            //    Primary:  edit URL — works for modems in Active/Planned status and returns a form
            //              with real <input> elements that WalkFormFields can read.
            //    Fallback: view URL — used for Ready/Shipped modems whose edit URL returns HTTP 200
            //              with an Oracle error body ("Row deleted by another user") rather than a
            //              proper edit form. The view page renders values as <td id="..."> cells
            //              which ReadViewPageFields extracts via GetInputByName + GetInnerTextByTdId.
            var sourceConn = new ModemConnection(HDocUtility.UrlModemEdit + sourceModemNo);
            var sourceHDoc = sourceConn.GetHtmlAsHdoc();

            Dictionary<string, string> sourceValues;
            if (IsOracleErrorPage(sourceHDoc))
            {
                // Edit page is unavailable for this modem status — read from the view URL instead
                var viewConn = new ModemConnection(HDocUtility.UrlModemView + sourceModemNo);
                var viewHDoc = viewConn.GetHtmlAsHdoc();
                sourceValues = ReadViewPageFields(viewHDoc);
            }
            else
            {
                sourceValues = WalkFormFields(sourceHDoc);
            }

            // 2) Walk TARGET edit page in DOM order — build full form field list
            var targetConn = new ModemConnection(HDocUtility.UrlModemEdit + targetModemNo);
            var targetHDoc = targetConn.GetHtmlAsHdoc();

            var formFields = new List<KeyValuePair<string, string>>();
            var seenNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var node in targetHDoc.DocumentNode.Descendants()
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
                        value = WebUtility.HtmlDecode(
                            string.IsNullOrEmpty(v) ? opt.InnerText.Trim() : v);
                    }
                    else value = "";
                }
                else if (node.Name == "textarea")
                {
                    value = WebUtility.HtmlDecode(node.InnerText);
                }
                else
                {
                    if (type == "checkbox")
                        value = node.Attributes.Contains("checked") ? "1" : "0";
                    else
                        value = WebUtility.HtmlDecode(
                            node.GetAttributeValue("value", ""));
                }

                formFields.Add(new KeyValuePair<string, string>(name, value));
            }

            // 3) Override only whitelisted header fields with source values
            for (int i = 0; i < formFields.Count; i++)
            {
                var f = formFields[i];

                if (f.Key == "Z_ACTION")
                {
                    formFields[i] = new KeyValuePair<string, string>(f.Key, "");
                    continue;
                }

                if (_copyFields.Contains(f.Key))
                {
                    string srcVal;
                    if (sourceValues.TryGetValue(f.Key, out srcVal))
                        formFields[i] = new KeyValuePair<string, string>(f.Key, srcVal);
                }
            }

            // 4) Build POST body in DOM order, iso-8859-1 encoding
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

            // 6) Check for errors
            var oraErr = Regex.Match(responseText, @"ORA-\d+[^<]*", RegexOptions.IgnoreCase);
            if (oraErr.Success)
                throw new InvalidOperationException("Oracle DB error: " + oraErr.Value.Trim());
            var appErr = Regex.Match(responseText, @"Error!</b>\s*<br[^>]*>\s*([^<]{1,200})", RegexOptions.IgnoreCase);
            if (appErr.Success)
                throw new InvalidOperationException("Form error: " + appErr.Groups[1].Value.Trim());

            return true;
        }

        /// <summary>
        /// Returns true when the Oracle page is an error screen rather than an editable form.
        /// The edit URL for Ready/Shipped modems returns HTTP 200 with an error body like:
        /// "Error! ... Row deleted by another user" instead of the actual form.
        /// </summary>
        private static bool IsOracleErrorPage(HtmlDocument hDoc)
        {
            string html = hDoc.DocumentNode.OuterHtml;
            return html.IndexOf("Row deleted by another user", StringComparison.OrdinalIgnoreCase) >= 0
                || (Regex.IsMatch(html, @"<i>Error!</i>", RegexOptions.IgnoreCase)
                    && hDoc.DocumentNode.SelectSingleNode("//input[@name='P_SSORD_ID']") == null);
        }

        /// <summary>
        /// Walk all form elements in DOM order, first-wins dedup.
        /// Identical logic to ShiftModemDatesAsync — returns name→value dictionary.
        /// </summary>
        private static Dictionary<string, string> WalkFormFields(HtmlDocument hDoc)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var node in hDoc.DocumentNode.Descendants()
                .Where(n => n.Name == "input" || n.Name == "select" || n.Name == "textarea"))
            {
                string name = node.GetAttributeValue("name", "");
                if (string.IsNullOrEmpty(name)) continue;
                if (fields.ContainsKey(name)) continue;

                string type = node.GetAttributeValue("type", "").ToLower();
                if (type == "submit" || type == "button" || type == "reset") continue;

                string value;
                if (node.Name == "select")
                {
                    var opt = node.Descendants("option")
                        .FirstOrDefault(o => o.Attributes.Contains("selected"));
                    if (opt != null)
                    {
                        var v = opt.GetAttributeValue("value", "");
                        value = WebUtility.HtmlDecode(
                            string.IsNullOrEmpty(v) ? opt.InnerText.Trim() : v);
                    }
                    else value = "";
                }
                else if (node.Name == "textarea")
                {
                    value = WebUtility.HtmlDecode(node.InnerText);
                }
                else
                {
                    if (type == "checkbox")
                        value = node.Attributes.Contains("checked") ? "1" : "0";
                    else
                        value = WebUtility.HtmlDecode(
                            node.GetAttributeValue("value", ""));
                }

                fields[name] = value;
            }

            return fields;
        }

        /// <summary>
        /// Reads the whitelisted copy-field values from the READ-ONLY view page.
        /// Oracle APEX / Oracle Forms view pages render each item either as a hidden
        /// &lt;input name="..."&gt; (preserving the raw/internal value — important for LOV
        /// dropdowns like P_L_SHIPFROM_LOC2) or as &lt;td id="..."&gt;displayed text&lt;/td&gt;.
        /// We try the input approach first so LOV fields get their internal ID, and
        /// fall back to the td text approach for plain display-only fields.
        /// </summary>
        private Dictionary<string, string> ReadViewPageFields(HtmlDocument hDoc)
        {
            var values = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (string fieldName in _copyFields)
            {
                // Hidden/visible input first — preserves the actual stored value (e.g. LOV IDs)
                string val = HDocUtility.GetInputByName(fieldName, hDoc);

                if (string.IsNullOrEmpty(val))
                {
                    // Fall back: Oracle view pages display field text inside <td id="FIELDNAME">
                    val = HDocUtility.GetInnerTextByTdId(fieldName, hDoc);
                }

                if (!string.IsNullOrEmpty(val))
                    values[fieldName] = val;
            }

            return values;
        }
    }
}
