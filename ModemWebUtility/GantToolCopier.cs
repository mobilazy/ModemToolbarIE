using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModemWebUtility
{
    /// <summary>
    /// Copies Gant tool configuration from one modem to another
    /// </summary>
    public class GantToolCopier
    {
        private readonly CookieContainer cookieContainer = new CookieContainer();

        public string LastError { get; private set; }

        /// <summary>
        /// Copy Gant tools from source modem to destination modem
        /// </summary>
        public async Task<bool> CopyGantToolsAsync(string sourceModemNumber, string destinationModemNumber)
        {
            LastError = null;

            try
            {
                // Step 1: Fetch source modem's gant_tools page
                string sourceUrl = HDocUtility.UrlGantTools + sourceModemNumber;
                string sourceHtml = await FetchPageAsync(sourceUrl);

                // Step 2: Extract p.old_string from JavaScript
                string toolString = ExtractOldString(sourceHtml);
                
                if (string.IsNullOrWhiteSpace(toolString))
                {
                    LastError = "No tool configuration found on source modem";
                    return false;
                }

                // Step 3: POST the tool string to destination modem
                string destUrl = HDocUtility.UrlGantTools + destinationModemNumber;
                
                // First, fetch the dest page to get session/cookies
                await FetchPageAsync(destUrl);

                // Now POST the tool string
                return await PostToolStringAsync(destUrl, toolString, destinationModemNumber);
            }
            catch (Exception ex)
            {
                LastError = $"Copy failed: {ex.Message}";
                return false;
            }
        }

        private async Task<string> FetchPageAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UseDefaultCredentials = true;
            request.CookieContainer = cookieContainer;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private string ExtractOldString(string html)
        {
            // Look for: p.old_string = "1(F0C0G...)9 ~  ~ 1(9600/17.50/G/EDL)GP "
            var match = Regex.Match(html, @"p\.old_string\s*=\s*""([^""]+)""", RegexOptions.IgnoreCase);
            
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        private async Task<bool> PostToolStringAsync(string url, string toolString, string modemNumber)
        {
            try
            {
                // The gant_tools.web page likely accepts a parameter with the tool string
                // Common parameter names: tool_string, gant_string, old_string, etc.
                // We'll try posting as form data
                
                string postData = $"mob_id={modemNumber}&tool_string={Uri.EscapeDataString(toolString)}";
                byte[] data = Encoding.ASCII.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.UseDefaultCredentials = true;
                request.CookieContainer = cookieContainer;

                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException wex)
            {
                LastError = $"POST failed: {wex.Message}";
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    using (var reader = new System.IO.StreamReader(errorResponse.GetResponseStream()))
                    {
                        LastError += "\r\n" + await reader.ReadToEndAsync();
                    }
                }
                return false;
            }
        }
    }
}
