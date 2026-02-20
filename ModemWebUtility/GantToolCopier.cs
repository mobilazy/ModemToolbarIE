using System;
using System.Collections.Generic;
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

        public string LastError { get; set; }
        public string DebugInfo { get; set; }

        /// <summary>
        /// Copy Gant tools from source modem to destination modem
        /// </summary>
        public async Task<bool> CopyGantToolsAsync(string sourceModemNumber, string destinationModemNumber)
        {
            LastError = null;
            DebugInfo = "";

            try
            {
                // Step 1: Fetch source modem's gant_tools page
                string sourceUrl = HDocUtility.UrlGantTools + sourceModemNumber;
                DebugInfo += $"Fetching source: {sourceUrl}\n";
                string sourceHtml = await FetchPageAsync(sourceUrl);
                DebugInfo += $"Source HTML length: {sourceHtml.Length}\n";

                // Step 2: Extract p.old_string from JavaScript
                string toolString = ExtractOldString(sourceHtml);
                
                if (string.IsNullOrWhiteSpace(toolString))
                {
                    LastError = "No tool configuration found on source modem";
                    DebugInfo += "ERROR: Could not extract p.old_string from source HTML\n";
                    return false;
                }

                DebugInfo += $"Extracted tool string: {toolString}\n";
                DebugInfo += $"Tool string length: {toolString.Length}\n";

                // Step 3: Get destination page and extract available tools
                string destUrl = HDocUtility.UrlGantTools + destinationModemNumber;
                DebugInfo += $"Destination URL: {destUrl}\n";
                
                string destHtml = await FetchPageAsync(destUrl);
                string currentDestTools = ExtractOldString(destHtml);
                DebugInfo += $"Current destination tools: '{currentDestTools}'\n";

                // Step 4: Clear destination first (to avoid string_number conflicts)
                DebugInfo += "Step 1: Clearing destination MWD tools...\n";
                string clearPostData = "quant=&dia=&mwd_tools=&ind=&string_number=" +
                                      $"&modem={destinationModemNumber}" +
                                      "&agm_tools=&agm_quant=&agm_sleeve=&eq_id=" +
                                      "&mlt_tools=&mlt_quant=&mlt_type=PKG";
                
                bool cleared = await PostToolDataAsync(destUrl, "", clearPostData, currentDestTools, clearOnly: true);
                if (!cleared)
                {
                    LastError = "Failed to clear destination tools before copying";
                    return false;
                }
                
                // Fetch destination again to get new state after clearing
                destHtml = await FetchPageAsync(destUrl);
                string clearedDestTools = ExtractOldString(destHtml);
                DebugInfo += $"After clearing, destination has: '{clearedDestTools}'\n";
                
                // Step 5: Extract tool codes from source, then map to tool IDs
                DebugInfo += "Step 2: Extracting source tools and copying to destination...\n";
                string postDataFromJs = ExtractAndMapPostData(sourceHtml, destHtml, destinationModemNumber);
                
                if (string.IsNullOrEmpty(postDataFromJs))
                {
                    LastError = "Failed to map tools from source to destination";
                    DebugInfo += "ERROR: Could not map tool data\n";
                    return false;
                }

                // Post with mapped data
                return await PostToolDataAsync(destUrl, toolString, postDataFromJs, clearedDestTools);
            }
            catch (Exception ex)
            {
                LastError = $"Copy failed: {ex.Message}";
                DebugInfo += $"EXCEPTION: {ex.Message}\n{ex.StackTrace}\n";
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

        private async Task<bool> PostToolDataAsync(string url, string toolString, string postData, string oldStrValue, bool clearOnly = false, int retryCount = 0)
        {
            try
            {
                if (retryCount > 3)
                {
                    LastError = "Too many retries (conflict resolution failed)";
                    DebugInfo += "ERROR: Maximum retry count exceeded\n";
                    return false;
                }
                
                // From JavaScript analysis: POST to gant_tools.set_gantstring
                // Remove query string if present and build correct endpoint
                string baseUrl = url.Split('?')[0]; // Remove ?mob_id=xxx
                string postUrl = baseUrl.Replace("gant_tools.web", "gant_tools.set_gantstring");
                
                // Add old_str (rollback value) to the POST data
                string fullPostData = postData + "&old_str=" + Uri.EscapeDataString(oldStrValue);
                
                byte[] data = Encoding.UTF8.GetBytes(fullPostData);

                if (retryCount == 0)
                {
                    DebugInfo += $"POST URL: {postUrl}\n";
                    DebugInfo += $"Using tool data extracted from source JavaScript...\n";
                    DebugInfo += $"old_str (rollback value): {oldStrValue}\n";
                    DebugInfo += $"Data Length: {data.Length}\n";
                    DebugInfo += $"POST Data: {fullPostData.Substring(0, Math.Min(800, fullPostData.Length))}\n";
                }
                else
                {
                    DebugInfo += $"RETRY #{retryCount}: old_str={oldStrValue.Substring(0, Math.Min(40, oldStrValue.Length))}...\n";
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.UseDefaultCredentials = true;
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;

                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }

                DebugInfo += "POST request sent, waiting for response...\n";

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string responseText = await reader.ReadToEndAsync();
                    
                    DebugInfo += $"Response Status: {response.StatusCode}\n";
                    DebugInfo += $"Response Length: {responseText.Length}\n";
                    DebugInfo += $"Response Text: {responseText.Substring(0, Math.Min(500, responseText.Length))}\n";
                    
                    // The server returns JavaScript with ret_val and ret_msg variables
                    // ret_val == -1 means error
                    // ret_val == 0 means success
                    // ret_val == 1 means conflict - someone else changed it, need to retry
                    
                    if (responseText.Contains("ret_val = -1") || responseText.Contains("ret_val=-1"))
                    {
                        // Error
                        var msgMatch = Regex.Match(responseText, @"ret_msg\s*=\s*[""']([^""']+)[""']");
                        if (msgMatch.Success)
                        {
                            LastError = $"Server error: {msgMatch.Groups[1].Value}";
                            DebugInfo += $"Server error message: {msgMatch.Groups[1].Value}\n";
                        }
                        else
                        {
                            LastError = "Server returned error (ret_val = -1)";
                        }
                        return false;
                    }
                    else if (responseText.Contains("ret_val = 1") || responseText.Contains("ret_val=1"))
                    {
                        // Conflict - extract server's current value
                        var msgMatch = Regex.Match(responseText, @"ret_msg\s*=\s*[""']([^""']+)[""']");
                        string serverCurrentTools = msgMatch.Success ? msgMatch.Groups[1].Value : "";
                        
                        DebugInfo += $"CONFLICT: ret_val=1\n";
                        DebugInfo += $"  Server says current tools: '{serverCurrentTools}'\n";
                        DebugInfo += $"  We tried to save: '{toolString}'\n";
                        
                        // Verify if tools were saved by fetching page again
                        DebugInfo += "  Verifying by fetching page again...\n";
                        string verifyHtml = await FetchPageAsync(url);
                        string actualTools = ExtractOldString(verifyHtml);
                        DebugInfo += $"  After POST, server ACTUALLY has: '{actualTools}'\n";
                        
                        if (actualTools == toolString)
                        {
                            DebugInfo += "SUCCESS: Tools WERE saved despite ret_val=1!\n";
                            return true;
                        }
                        else
                        {
                            DebugInfo += "FAILED: Tools were NOT saved. ret_val=1 blocked the save.\n";
                            LastError = $"Server rejected save due to conflict. Manual intervention needed.";
                            return false;
                        }
                    }
                    else if (responseText.Contains("ret_val = 0") || responseText.Contains("ret_val=0"))
                    {
                        // Clean success
                        DebugInfo += "Server returned ret_val=0 (success)\n";
                        
                        if (clearOnly)
                        {
                            // For clear operation, just check it succeeded
                            DebugInfo += "Clear operation successful!\n";
                            return true;
                        }
                        
                        // Verify by fetching page again
                        DebugInfo += "Verifying by fetching destination page...\n";
                        string verifyHtml = await FetchPageAsync(url);
                        string actualTools = ExtractOldString(verifyHtml);
                        DebugInfo += $"After POST, destination actually has: '{actualTools}'\n";
                        DebugInfo += $"We wanted to save: '{toolString}'\n";
                        
                        if (actualTools.Trim() == toolString.Trim())
                        {
                            DebugInfo += "VERIFIED: Tools match! Copy successful!\n";
                            return true;
                        }
                        else
                        {
                            DebugInfo += "VERIFICATION FAILED: Tools don't match!\n";
                            DebugInfo += $"  Expected: '{toolString}'\n";
                            DebugInfo += $"  Actual:   '{actualTools}'\n";
                            LastError = $"Server returned success but tools weren't saved correctly.";
                            return false;
                        }
                    }
                    
                    // If we get here, assume success based on status code
                    DebugInfo += "SUCCESS: Tools copied (assumed from status code)!\n";
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

        private string ExtractAndMapPostData(string sourceHtml, string destHtml, string modemNumber)
        {
            // Build a map of tool code -> tool ID from the SOURCE page
            // Source page has the actual tools defined; empty destination won't have tool definitions
            var toolMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            // Extract all tool definitions from SOURCE page
            // Format: new mwd_tool(tool_id, "code", "description")
            var sourceToolMatches = Regex.Matches(sourceHtml, @"new\s+mwd_tool\((\d+),\s*""([^""]+)""");
            foreach (Match match in sourceToolMatches)
            {
                string toolId = match.Groups[1].Value;
                string toolCode = match.Groups[2].Value;
                if (!toolMap.ContainsKey(toolCode))
                {
                    toolMap[toolCode] = toolId;
                }
            }
            
            DebugInfo += $"Built tool map with {toolMap.Count} tools\n";
            
            // Extract existing string_numbers from DESTINATION (even empty strings have them!)
            var destStringNumbers = new List<string>();
            var destStringMatches = Regex.Matches(destHtml, @"tmp_s\s*=\s*new\s+mwd_string\((\d+)\);.*?p\.gant_strings\.add\(tmp_s\.clone\(true\)\);", RegexOptions.Singleline);
            
            foreach (Match stringMatch in destStringMatches)
            {
                string stringBlock = stringMatch.Value;
                if (!stringBlock.Contains("type = 'W'"))
                    continue;
                    
                var stringNumMatch = Regex.Match(stringBlock, @"\.string_number\s*=\s*(\d+)");
                if (stringNumMatch.Success)
                {
                    destStringNumbers.Add(stringNumMatch.Groups[1].Value);
                }
            }
            
            DebugInfo += $"Found {destStringNumbers.Count} existing string_numbers in destination\n";
            
            // Now extract the structure from source page
            try
            {
                var sb = new StringBuilder();
                
                var stringMatches = Regex.Matches(sourceHtml, @"tmp_s\s*=\s*new\s+mwd_string\((\d+)\);.*?p\.gant_strings\.add\(tmp_s\.clone\(true\)\);", RegexOptions.Singleline);
                
                var quantList = new List<string>();
                var diaList = new List<string>();
                var stringsToolsList = new List<List<string>>(); // Tools for each string
                
                foreach (Match stringMatch in stringMatches)
                {
                    string stringBlock = stringMatch.Value;
                    
                    if (!stringBlock.Contains("type = 'W'"))
                        continue;
                    
                    // Extract diameter (required for all strings)
                    var diaMatch = Regex.Match(stringBlock, @"new\s+mwd_string\((\d+)\)");
                    if (diaMatch.Success)
                    {
                        diaList.Add(diaMatch.Groups[1].Value);
                    }
                    else
                    {
                        continue; // Skip if no diameter found
                    }
                    
                    // Extract quantity (default to 1 if not found)
                    var quantMatch = Regex.Match(stringBlock, @"\.quantity\s*=\s*(\d+)");
                    quantList.Add(quantMatch.Success ? quantMatch.Groups[1].Value : "1");
                    
                    // Extract tool CODES for this string and map to tool IDs
                    var stringTools = new List<string>();
                    var toolMatches = Regex.Matches(stringBlock, @"new\s+mwd_tool\(\d+,\s*""([^""]+)""");
                    foreach (Match toolMatch in toolMatches)
                    {
                        string toolCode = toolMatch.Groups[1].Value;
                        if (toolMap.ContainsKey(toolCode))
                        {
                            stringTools.Add(toolMap[toolCode]);
                        }
                        else
                        {
                            DebugInfo += $"WARNING: Tool code '{toolCode}' not found in tool map\n";
                        }
                    }
                    stringsToolsList.Add(stringTools);
                }
                
                // Build POST data in EXACT working order: quant, dia, mwd_tools, ind, string_number, modem, agm_tools, agm_quant, agm_sleeve, eq_id, mlt_tools, mlt_quant, mlt_type, old_str (added later)
                
                // Extract GP tool info from DESTINATION (tool_id and eq_id must match - they're tied together)
                var destGpMatch = Regex.Match(destHtml, @"tmp_s\s*=\s*new\s+gen_tool\('GP'\);.*?p\.gant_strings\.add\(tmp_s\.clone\(true\)\);", RegexOptions.Singleline);
                string destEqId = "";
                string destGpToolId = "";
                
                if (destGpMatch.Success)
                {
                    string destGpBlock = destGpMatch.Value;
                    var destToolIdMatch = Regex.Match(destGpBlock, @"\.tool_id\s*=\s*(\d+)");
                    var destEqIdMatch = Regex.Match(destGpBlock, @"\.eq_id\s*=\s*(\d+)");
                    
                    destGpToolId = destToolIdMatch.Success ? destToolIdMatch.Groups[1].Value : "";
                    destEqId = destEqIdMatch.Success ? destEqIdMatch.Groups[1].Value : "";
                }
                
                // Extract GP tool quantity from SOURCE (but use destination's tool_id and eq_id)
                var sourceGpMatch = Regex.Match(sourceHtml, @"tmp_s\s*=\s*new\s+gen_tool\('GP'\);.*?p\.gant_strings\.add\(tmp_s\.clone\(true\)\);", RegexOptions.Singleline);
                string agmQuant = "1";
                
                if (sourceGpMatch.Success)
                {
                    string sourceGpBlock = sourceGpMatch.Value;
                    var quantMatch = Regex.Match(sourceGpBlock, @"\.quantity\s*=\s*(\d+)");
                    
                    if (quantMatch.Success)
                    {
                        agmQuant = quantMatch.Groups[1].Value;
                    }
                }
                
                // Build flat tool array and calculate ind values
                var toolIdList = new List<string>();
                var indList = new List<string>();
                
                for (int i = 0; i < stringsToolsList.Count; i++)
                {
                    var stringTools = stringsToolsList[i];
                    
                    // ind is the starting index in the flat tool array for this string's tools
                    indList.Add(toolIdList.Count.ToString());
                    
                    // Add this string's tools to the flat array
                    toolIdList.AddRange(stringTools);
                    
                    DebugInfo += $"String {i}: {stringTools.Count} tools, ind={indList[i]}\n";
                }
                
                // 1. quant (repeating)
                for (int i = 0; i < quantList.Count; i++)
                {
                    if (i == 0)
                        sb.Append($"quant={quantList[i]}");
                    else
                        sb.Append($"&quant={quantList[i]}");
                }
                
                // 2. dia (repeating)
                for (int i = 0; i < diaList.Count; i++)
                {
                    sb.Append($"&dia={diaList[i]}");
                }
                
                // 3. mwd_tools (repeating, flat array of all tools)
                for (int i = 0; i < toolIdList.Count; i++)
                {
                    sb.Append($"&mwd_tools={toolIdList[i]}");
                }
                
                // 4. ind (repeating, starting index for each string's tools)
                for (int i = 0; i < indList.Count; i++)
                {
                    sb.Append($"&ind={indList[i]}");
                }
                
                // 5. string_number (repeating, use existing string_numbers from dest, -1 for new strings)
                for (int i = 0; i < quantList.Count; i++)
                {
                    if (i < destStringNumbers.Count)
                    {
                        sb.Append($"&string_number={destStringNumbers[i]}");
                        DebugInfo += $"Using existing string_number={destStringNumbers[i]} for string {i}\n";
                    }
                    else
                    {
                        sb.Append("&string_number=-1");
                        DebugInfo += $"Using string_number=-1 for new string {i}\n";
                    }
                }
                
                // 6. modem
                sb.Append($"&modem={modemNumber}");
                
                // 7-10. agm parameters (use destination's tool_id and eq_id - they're tied together)
                sb.Append($"&agm_tools={destGpToolId}");
                sb.Append($"&agm_quant={agmQuant}");
                sb.Append("&agm_sleeve=");
                sb.Append($"&eq_id={destEqId}");
                
                // 11-13. mlt parameters
                sb.Append("&mlt_tools=&mlt_quant=&mlt_type=PKG");
                
                return sb.ToString();
            }
            catch (Exception ex)
            {
                DebugInfo += $"ERROR mapping tools: {ex.Message}\n";
                return null;
            }
        }

        private string ExtractPostDataFromJavaScript(string html, string modemNumber)
        {
            // Extract JavaScript section that contains tool definitions
            // Format: tmp_s.add(new mwd_tool(tool_id, "code", "description"));
            // and: tmp_s.tool_id = 53; tmp_s.eq_id = 92074; (for GP tools)
            
            try
            {
                var sb = new StringBuilder();
                
                // Extract all MWD string definitions
                var stringMatches = Regex.Matches(html, @"tmp_s\s*=\s*new\s+mwd_string\((\d+)\);.*?p\.gant_strings\.add\(tmp_s\.clone\(true\)\);", RegexOptions.Singleline);
                
                var quantList = new List<string>();
                var diaList = new List<string>();
                var toolIdList = new List<string>();
                var indList = new List<string>();
                var stringNumList = new List<string>();
                
                int cumulativeIndCount = 0;
                
                foreach (Match stringMatch in stringMatches)
                {
                    string stringBlock = stringMatch.Value;
                    
                    // Check if this is an MWD string (type = 'W')
                    if (!stringBlock.Contains("type = 'W'"))
                        continue;
                    
                    // Extract diameter
                    var diaMatch = Regex.Match(stringBlock, @"new\s+mwd_string\((\d+)\)");
                    if (diaMatch.Success)
                    {
                        diaList.Add(diaMatch.Groups[1].Value);
                    }
                    
                    // Extract quantity
                    var quantMatch = Regex.Match(stringBlock, @"\.quantity\s*=\s*(\d+)");
                    if (quantMatch.Success)
                    {
                        quantList.Add(quantMatch.Groups[1].Value);
                    }
                    
                    // Extract string_number
                    var stringNumMatch = Regex.Match(stringBlock, @"\.string_number\s*=\s*(\d+)");
                    if (stringNumMatch.Success)
                    {
                        // Don't copy string_number from source - let destination DB create new ones
                        stringNumList.Add("");
                    }
                    else
                    {
                        stringNumList.Add("");
                    }
                    
                    // Extract all tool IDs in this string
                    var toolMatches = Regex.Matches(stringBlock, @"new\s+mwd_tool\((\d+),");
                    int toolsInString = 0;
                    foreach (Match toolMatch in toolMatches)
                    {
                        toolIdList.Add(toolMatch.Groups[1].Value);
                        toolsInString++;
                    }
                    
                    // Track cumulative index
                    cumulativeIndCount += toolsInString;
                    indList.Add(cumulativeIndCount.ToString());
                }
                
                // Build POST data with repeating parameter names (like JavaScript does)
                for (int i = 0; i < quantList.Count; i++)
                {
                    if (i == 0)
                        sb.Append($"quant={quantList[i]}");
                    else
                        sb.Append($"&quant={quantList[i]}");
                }
                
                sb.Append("&");
                for (int i = 0; i < diaList.Count; i++)
                {
                    if (i == 0)
                        sb.Append($"dia={diaList[i]}");
                    else
                        sb.Append($"&dia={diaList[i]}");
                }
                
                sb.Append("&");
                for (int i = 0; i < toolIdList.Count; i++)
                {
                    if (i == 0)
                        sb.Append($"mwd_tools={toolIdList[i]}");
                    else
                        sb.Append($"&mwd_tools={toolIdList[i]}");
                }
                
                sb.Append("&");
                for (int i = 0; i < indList.Count; i++)
                {
                    if (i < indList.Count - 1) // Don't add last ind per JavaScript logic
                    {
                        if (i == 0)
                            sb.Append($"ind={indList[i]}");
                        else
                            sb.Append($"&ind={indList[i]}");
                    }
                }
                
                sb.Append("&");
                for (int i = 0; i < stringNumList.Count; i++)
                {
                    if (i == 0)
                        sb.Append($"string_number={stringNumList[i]}");
                    else
                        sb.Append($"&string_number={stringNumList[i]}");
                }
                
                sb.Append($"&modem={modemNumber}");
                
                // Extract GP tool data (type = 'GP')
                var gpMatch = Regex.Match(html, @"tmp_s\s*=\s*new\s+gen_tool\('GP'\);.*?p\.gant_strings\.add\(tmp_s\.clone\(true\)\);", RegexOptions.Singleline);
                if (gpMatch.Success)
                {
                    string gpBlock = gpMatch.Value;
                    
                    var toolIdMatch = Regex.Match(gpBlock, @"\.tool_id\s*=\s*(\d+)");
                    var eqIdMatch = Regex.Match(gpBlock, @"\.eq_id\s*=\s*(\d+)");
                    var quantMatch = Regex.Match(gpBlock, @"\.quantity\s*=\s*(\d+)");
                    
                    if (toolIdMatch.Success)
                    {
                        sb.Append($"&agm_tools={toolIdMatch.Groups[1].Value}");
                        sb.Append($"&agm_quant={(quantMatch.Success ? quantMatch.Groups[1].Value : "1")}");
                        sb.Append("&agm_sleeve=");
                        // Don't copy eq_id - let destination DB create new one
                        sb.Append("&eq_id=");
                    }
                    else
                    {
                        // No GP tools
                        sb.Append("&agm_tools=&agm_quant=&agm_sleeve=&eq_id=");
                    }
                }
                else
                {
                    // No GP tools
                    sb.Append("&agm_tools=&agm_quant=&agm_sleeve=&eq_id=");
                }
                
                // MLT tools (not used in this case)
                sb.Append("&mlt_tools=&mlt_quant=&mlt_type=PKG");
                
                return sb.ToString();
            }
            catch (Exception ex)
            {
                DebugInfo += $"ERROR parsing JavaScript: {ex.Message}\n";
                return null;
            }
        }
    }
}
