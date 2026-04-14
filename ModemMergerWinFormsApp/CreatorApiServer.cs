using ModemWebUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModemMergerWinFormsApp
{
    /// <summary>
    /// Lightweight self-hosted HTTP server that exposes modem creator functionality
    /// to the Automation Hub web UI. Runs on localhost:9002 alongside the WinForms app.
    /// </summary>
    public sealed class CreatorApiServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Func<List<GantModem>> _getLoadedModems;
        private const int Port = 9002;

        /// <param name="getLoadedModems">Returns the modems already loaded in the Shifter tab.</param>
        public CreatorApiServer(Func<List<GantModem>> getLoadedModems)
        {
            _getLoadedModems = getLoadedModems ?? (() => new List<GantModem>());
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{Port}/");
        }

        public void Start()
        {
            _listener.Start();
            Task.Run(() => ListenLoop(_cts.Token));
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _listener.Stop(); } catch { }
            try { _listener.Close(); } catch { }
        }

        private async Task ListenLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var ctx = await _listener.GetContextAsync().ConfigureAwait(false);
                    // Fire and forget each request (don't block the listener)
                    _ = Task.Run(() => HandleRequest(ctx));
                }
                catch (ObjectDisposedException) { break; }
                catch (HttpListenerException) { break; }
                catch { }
            }
        }

        private async Task HandleRequest(HttpListenerContext ctx)
        {
            try
            {
                // CORS headers for browser access
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                ctx.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (ctx.Request.HttpMethod == "OPTIONS")
                {
                    ctx.Response.StatusCode = 204;
                    ctx.Response.Close();
                    return;
                }

                var path = ctx.Request.Url.AbsolutePath.TrimEnd('/').ToLowerInvariant();

                switch (path)
                {
                    case "/api/modems":
                        await HandleGetModems(ctx);
                        break;
                    case "/api/timeplanner":
                        await HandleScrapeTimePlanner(ctx);
                        break;
                    case "/api/bulkcreate":
                        await HandleBulkCreate(ctx);
                        break;
                    case "/api/health":
                        WriteJson(ctx, new { status = "ok", port = Port });
                        break;
                    case "/api/log":
                        await HandleGetLog(ctx);
                        break;
                    default:
                        ctx.Response.StatusCode = 404;
                        WriteJson(ctx, new { error = "Not found" });
                        break;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ctx.Response.StatusCode = 500;
                    WriteJson(ctx, new { error = ex.Message });
                }
                catch { }
            }
        }

        // ── GET /api/log ─────────────────────────────────────────────────
        private Task HandleGetLog(HttpListenerContext ctx)
        {
            var logPath = ScrapeLog.LogPath;
            string content = "";
            if (!string.IsNullOrEmpty(logPath) && File.Exists(logPath))
            {
                try { content = File.ReadAllText(logPath, System.Text.Encoding.UTF8); }
                catch (Exception ex) { content = $"[Error reading log: {ex.Message}]"; }
            }
            else
            {
                // Try default path even if ScrapeLog.Start() not called yet
                var defaultPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ModemMerger", "scraper-log.txt");
                if (File.Exists(defaultPath))
                {
                    try { content = File.ReadAllText(defaultPath, System.Text.Encoding.UTF8); }
                    catch (Exception ex) { content = $"[Error reading log: {ex.Message}]"; }
                }
                else
                    content = "(No scraper log found yet — run a scrape first)";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "text/plain; charset=utf-8";
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.OutputStream.Close();
            return Task.CompletedTask;
        }

        // ── GET /api/modems ──────────────────────────────────────────────
        // Returns modems for the given customer/rig.
        // If the Shifter tab has already loaded them, returns those directly.
        // Otherwise fetches from Gant on the fly using customer+rig from the request body.
        private async Task HandleGetModems(HttpListenerContext ctx)
        {
            var cached = _getLoadedModems();
            if (cached != null && cached.Count > 0)
            {
                WriteJson(ctx, cached);
                return;
            }

            // No cached modems — fetch from Gant directly using request body
            try
            {
                var body = ReadBody(ctx);
                string customer = "", rig = "";
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var req = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                    if (req != null)
                    {
                        if (req.ContainsKey("customer")) customer = req["customer"].ToString();
                        if (req.ContainsKey("rig"))      rig      = req["rig"].ToString();
                    }
                }

                if (string.IsNullOrEmpty(customer))
                {
                    WriteJson(ctx, new List<GantModem>());
                    return;
                }

                var modems = await GantClient.FetchModemsAsync(customer, rig, DateTime.Today).ConfigureAwait(false);
                WriteJson(ctx, modems);
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = 500;
                WriteJson(ctx, new { error = ex.Message });
            }
        }

        // ── POST /api/timeplanner ────────────────────────────────────────
        // Body: { "operator": "AkerBP", "rig": "Noble Invincible",
        //         "username": "...", "password": "...",
        //         "wellCodes": ["C-21", "C-11", ...], "headless": true }
        private async Task HandleScrapeTimePlanner(HttpListenerContext ctx)
        {
            var body = ReadBody(ctx);
            var req = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

            string operatorName = req.ContainsKey("operator") ? req["operator"].ToString() : "";
            string rig = req.ContainsKey("rig") ? req["rig"].ToString() : "";
            string user = req.ContainsKey("username") ? req["username"].ToString() : "";
            string pass = req.ContainsKey("password") ? req["password"].ToString() : "";
            bool headless = !req.ContainsKey("headless") || Convert.ToBoolean(req["headless"]);

            var wellCodes = new List<string>();
            if (req.ContainsKey("wellCodes"))
            {
                var arr = JsonConvert.DeserializeObject<List<string>>(req["wellCodes"].ToString());
                if (arr != null) wellCodes = arr;
            }

            var result = await KabalScraperClient.ScrapeTimePlannerAsync(
                operatorName, rig, user, pass, wellCodes, headless);

            WriteJson(ctx, result);
        }

        // ── POST /api/bulkcreate ─────────────────────────────────────────
        // Body: { "requests": [ { sourceModemId, sectionFlags, loadoutDate, dateEta, wellName, sectionName }, ... ] }
        private async Task HandleBulkCreate(HttpListenerContext ctx)
        {
            var body = ReadBody(ctx);
            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

            List<BulkCreateRequest> requests;
            if (wrapper.ContainsKey("requests"))
                requests = JsonConvert.DeserializeObject<List<BulkCreateRequest>>(wrapper["requests"].ToString());
            else
                requests = JsonConvert.DeserializeObject<List<BulkCreateRequest>>(body);

            var results = await BulkModemCreator.BulkCreateAsync(requests ?? new List<BulkCreateRequest>());
            WriteJson(ctx, results);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private static string ReadBody(HttpListenerContext ctx)
        {
            // Always decode as UTF-8. HttpListener.ContentEncoding defaults to ISO-8859-1
            // when the client sends Content-Type: application/json without an explicit charset,
            // which corrupts non-ASCII characters like 'å' in "Vår Energi" to "VÃ¥r Energi".
            using (var reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8))
                return reader.ReadToEnd();
        }

        private static void WriteJson(HttpListenerContext ctx, object data)
        {
            ctx.Response.ContentType = "application/json; charset=utf-8";
            var json = JsonConvert.SerializeObject(data, Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var bytes = Encoding.UTF8.GetBytes(json);
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }
    }
}
