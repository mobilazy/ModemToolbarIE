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

        public ModemHeaderCopy(string sourceModemNo, string targetModemNo)
        {
            this.sourceModemNo = sourceModemNo;
            this.targetModemNo = targetModemNo;
        }

        public bool CopyHeaderFields()
        {
            // 1) Walk SOURCE edit page in DOM order — same code as ShiftModemDatesAsync
            var sourceConn = new ModemConnection(HDocUtility.UrlModemEdit + sourceModemNo);
            var sourceHDoc = sourceConn.GetHtmlAsHdoc();
            var sourceValues = WalkFormFields(sourceHDoc);

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

            // 3) Override target P_ fields with source values (skip system fields)
            for (int i = 0; i < formFields.Count; i++)
            {
                var f = formFields[i];

                if (f.Key == "Z_ACTION")
                {
                    formFields[i] = new KeyValuePair<string, string>(f.Key, "");
                    continue;
                }

                if (f.Key.StartsWith("P_") && !_skipOverride.Contains(f.Key))
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
    }
}
