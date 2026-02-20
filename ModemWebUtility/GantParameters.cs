using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ModemWebUtility
{
    /// <summary>
    /// Extracts parameters and data from Gantt Tools pages
    /// </summary>
    public class GantParameters
    {
        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();
        
        public string MobId { get; set; }
        public List<GantToolItem> Tools { get; set; } = new List<GantToolItem>();
        public List<AttachmentInfo> Attachments { get; set; } = new List<AttachmentInfo>();
        public Dictionary<string, string> PageData { get; set; } = new Dictionary<string, string>();

        public GantParameters(HtmlAgilityPack.HtmlDocument _hDoc, string mobId)
        {
            hDoc = _hDoc;
            MobId = mobId;
            Init();
        }

        public GantParameters(mshtml.HTMLDocument htmlDocument, string mobId)
        {
            hDoc.LoadHtml(LoadHtmlAgility(htmlDocument));
            MobId = mobId;
            Init();
        }

        private string LoadHtmlAgility(mshtml.HTMLDocument htmlDocument)
        {
            mshtml.IHTMLDocument3 idoc = (mshtml.IHTMLDocument3)htmlDocument;
            return idoc.documentElement.outerHTML;
        }

        private void Init()
        {
            ExtractTools();
            ExtractAttachments();
            ExtractPageData();
        }

        /// <summary>
        /// Extract tool information from tables
        /// </summary>
        private void ExtractTools()
        {
            try
            {
                var tables = hDoc.DocumentNode.SelectNodes("//table");
                if (tables == null) return;

                foreach (var table in tables)
                {
                    var rows = table.SelectNodes(".//tr");
                    if (rows == null) continue;

                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes(".//td");
                        if (cells != null && cells.Count >= 2)
                        {
                            var toolItem = new GantToolItem
                            {
                                ToolName = System.Net.WebUtility.HtmlDecode(cells[0].InnerText?.Trim() ?? ""),
                                ToolDescription = cells.Count > 1 ? System.Net.WebUtility.HtmlDecode(cells[1].InnerText?.Trim() ?? "") : "",
                                AdditionalData = cells.Count > 2 ? System.Net.WebUtility.HtmlDecode(cells[2].InnerText?.Trim() ?? "") : ""
                            };

                            if (!string.IsNullOrWhiteSpace(toolItem.ToolName))
                            {
                                Tools.Add(toolItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue
            }
        }

        /// <summary>
        /// Extract attachment links (both standard links and JavaScript handlers)
        /// </summary>
        private void ExtractAttachments()
        {
            try
            {
                // Extract standard <a> links
                ExtractStandardLinks();

                // Extract JavaScript onclick handlers
                ExtractJavaScriptLinks();

                // Extract form-based downloads
                ExtractFormDownloads();
            }
            catch (Exception ex)
            {
                // Log error but continue
            }
        }

        private void ExtractStandardLinks()
        {
            var links = hDoc.DocumentNode.SelectNodes("//a[@href]");
            if (links == null) return;

            // DEBUG: Log all links found
            System.Diagnostics.Debug.WriteLine($"DEBUG: Found {links.Count} total links on page");

            foreach (var link in links)
            {
                var href = link.GetAttributeValue("href", "");
                var text = System.Net.WebUtility.HtmlDecode(link.InnerText?.Trim() ?? "");

                // DEBUG: Log each link
                System.Diagnostics.Debug.WriteLine($"  Link: '{text}' -> {href}");

                // Check if this looks like a file download
                if (IsFileLink(href, text))
                {
                    System.Diagnostics.Debug.WriteLine($"    ^ MATCHED as file link");
                    var attachment = new AttachmentInfo
                    {
                        FileName = ExtractFileName(href, text),
                        Url = NormalizeUrl(href),
                        LinkText = text,
                        FileType = GetFileType(href),
                        DownloadMethod = DownloadMethod.DirectLink
                    };

                    Attachments.Add(attachment);
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"DEBUG: Extracted {Attachments.Count} attachments from standard links");
        }

        private void ExtractJavaScriptLinks()
        {
            // Find onclick handlers that might trigger downloads
            var elementsWithOnClick = hDoc.DocumentNode.SelectNodes("//*[@onclick]");
            if (elementsWithOnClick == null) return;

            foreach (var element in elementsWithOnClick)
            {
                var onclick = element.GetAttributeValue("onclick", "");
                var text = System.Net.WebUtility.HtmlDecode(element.InnerText?.Trim() ?? "");

                // Parse JavaScript function calls
                var jsInfo = ParseJavaScriptDownload(onclick, text);
                if (jsInfo != null)
                {
                    Attachments.Add(jsInfo);
                }
            }
        }

        private void ExtractFormDownloads()
        {
            var forms = hDoc.DocumentNode.SelectNodes("//form");
            if (forms == null) return;

            foreach (var form in forms)
            {
                var action = form.GetAttributeValue("action", "");
                var buttons = form.SelectNodes(".//input[@type='submit'] | .//button[@type='submit']");

                if (buttons != null && IsFileLink(action, action))
                {
                    foreach (var button in buttons)
                    {
                        var buttonText = button.GetAttributeValue("value", "") ?? button.InnerText;

                        var attachment = new AttachmentInfo
                        {
                            FileName = ExtractFileName(action, buttonText),
                            Url = NormalizeUrl(action),
                            LinkText = System.Net.WebUtility.HtmlDecode(buttonText?.Trim() ?? ""),
                            FileType = GetFileType(action),
                            DownloadMethod = DownloadMethod.FormSubmit,
                            FormAction = action,
                            FormMethod = form.GetAttributeValue("method", "GET")
                        };

                        Attachments.Add(attachment);
                    }
                }
            }
        }

        private AttachmentInfo ParseJavaScriptDownload(string onclick, string linkText)
        {
            if (string.IsNullOrWhiteSpace(onclick)) return null;

            // Common patterns for download functions
            var patterns = new[]
            {
                @"downloadFile\s*\(\s*['""]([^'""]+)['""]",
                @"window\.open\s*\(\s*['""]([^'""]+)['""]",
                @"location\.href\s*=\s*['""]([^'""]+)['""]",
                @"document\.location\s*=\s*['""]([^'""]+)['""]"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(onclick, pattern);
                if (match.Success)
                {
                    var url = match.Groups[1].Value;
                    if (IsFileLink(url, linkText))
                    {
                        return new AttachmentInfo
                        {
                            FileName = ExtractFileName(url, linkText),
                            Url = NormalizeUrl(url),
                            LinkText = linkText,
                            FileType = GetFileType(url),
                            DownloadMethod = DownloadMethod.JavaScript,
                            JavaScriptCode = onclick
                        };
                    }
                }
            }

            return null;
        }

        private bool IsFileLink(string url, string text)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            var lowerUrl = url.ToLower();
            var lowerText = text.ToLower();

            // PRIORITY: Check for docreg.download pattern (definitive attachment link)
            if (lowerUrl.Contains("docreg.download"))
                return true;

            // Check for common file extensions
            var fileExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv", ".zip", ".rar", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".txt", ".xml" };
            
            foreach (var ext in fileExtensions)
            {
                if (lowerUrl.Contains(ext) || lowerText.Contains(ext.TrimStart('.')))
                    return true;
            }

            // Check for download-related keywords
            var downloadKeywords = new[] { "download", "attachment", "file", "export", "report" };
            foreach (var keyword in downloadKeywords)
            {
                if (lowerText.Contains(keyword) || lowerUrl.Contains(keyword))
                    return true;
            }

            return false;
        }

        private string ExtractFileName(string url, string linkText)
        {
            // Handle docreg.download?p_file=FOLDER/filename.ext pattern
            var docregMatch = Regex.Match(url, @"p_file=(?:[^/]+/)*([^/&]+)");
            if (docregMatch.Success)
                return System.Net.WebUtility.UrlDecode(docregMatch.Groups[1].Value);

            // Try to extract from URL
            var urlMatch = Regex.Match(url, @"/([^/]+\.[a-zA-Z0-9]{2,4})(?:\?|$)");
            if (urlMatch.Success)
                return urlMatch.Groups[1].Value;

            // Try to extract from link text
            var textMatch = Regex.Match(linkText, @"([^\s]+\.[a-zA-Z0-9]{2,4})");
            if (textMatch.Success)
                return textMatch.Groups[1].Value;

            // Generate from link text
            return !string.IsNullOrWhiteSpace(linkText) ? 
                   Regex.Replace(linkText, @"[^\w\s-]", "").Trim() + ".file" : 
                   "attachment.file";
        }

        private string GetFileType(string url)
        {
            // Extract from filename in docreg.download?p_file=FOLDER/file.ext
            var docregMatch = Regex.Match(url, @"p_file=(?:[^/]+/)*[^/]+\.([a-zA-Z0-9]{2,4})");
            if (docregMatch.Success)
                return docregMatch.Groups[1].Value.ToUpper();

            // Standard extension extraction
            var match = Regex.Match(url.ToLower(), @"\.([a-zA-Z0-9]{2,4})(?:\?|$|&)");
            return match.Success ? match.Groups[1].Value.ToUpper() : "UNKNOWN";
        }

        private string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;

            // If relative URL, make it absolute
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var baseUrl = "http://norwayappsprd.corp.halliburton.com";
                if (!url.StartsWith("/"))
                    url = "/pls/log_web/" + url;
                return baseUrl + url;
            }

            return url;
        }

        private void ExtractPageData()
        {
            try
            {
                // Extract mob_id if present
                if (!string.IsNullOrEmpty(MobId))
                    PageData["mob_id"] = MobId;

                // Extract other form fields that might be useful
                var inputs = hDoc.DocumentNode.SelectNodes("//input[@name]");
                if (inputs != null)
                {
                    foreach (var input in inputs)
                    {
                        var name = input.GetAttributeValue("name", "");
                        var value = input.GetAttributeValue("value", "");
                        if (!string.IsNullOrWhiteSpace(name) && !PageData.ContainsKey(name))
                        {
                            PageData[name] = System.Net.WebUtility.HtmlDecode(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue
            }
        }
    }

    public class GantToolItem
    {
        public string ToolName { get; set; }
        public string ToolDescription { get; set; }
        public string AdditionalData { get; set; }
    }

    public class AttachmentInfo
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public string LinkText { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime? LastModified { get; set; }
        public DownloadMethod DownloadMethod { get; set; }
        public string FormAction { get; set; }
        public string FormMethod { get; set; }
        public string JavaScriptCode { get; set; }
    }

    public enum DownloadMethod
    {
        DirectLink,
        JavaScript,
        FormSubmit
    }
}
