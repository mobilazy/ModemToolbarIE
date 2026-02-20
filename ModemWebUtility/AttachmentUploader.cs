using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModemWebUtility
{
    /// <summary>
    /// Handles uploading attachments to modem pages
    /// </summary>
    public class AttachmentUploader
    {
        private readonly CookieContainer cookieContainer = new CookieContainer();

        public string LastError { get; set; }

        public event EventHandler<UploadProgressEventArgs> UploadProgress;
        public event EventHandler<UploadCompletedEventArgs> UploadCompleted;

        /// <summary>
        /// Uploads a file to a modem using multipart/form-data
        /// </summary>
        /// <param name="filePath">Path to file to upload</param>
        /// <param name="destinationModemNumber">Modem ID (P_SSORD_ID)</param>
        /// <param name="docTyp">Document type value (1=WinPul, 2=Shipping, 3=BHA, 4=Download, 22=Other)</param>
        public async Task<bool> UploadFileAsync(string filePath, string destinationModemNumber, string docTyp = "")
        {
            LastError = null;

            try
            {
                if (!File.Exists(filePath))
                {
                    LastError = "File not found: " + filePath;
                    return false;
                }

                string fileName = Path.GetFileName(filePath);
                byte[] fileData = File.ReadAllBytes(filePath);

                OnUploadProgress(new UploadProgressEventArgs { FileName = fileName, BytesUploaded = 0, TotalBytes = fileData.Length });

                string url = HDocUtility.UrlDocregUpload;
                string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
                string docTypValue = string.IsNullOrWhiteSpace(docTyp) ? "22" : docTyp;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.UseDefaultCredentials = true;
                request.CookieContainer = cookieContainer;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    var encoding = Encoding.UTF8;

                    // Add doctyp field (document type selector)
                    byte[] doctypField = encoding.GetBytes($"--{boundary}\r\nContent-Disposition: form-data; name=\"doctyp\"\r\n\r\n{docTypValue}\r\n");
                    await requestStream.WriteAsync(doctypField, 0, doctypField.Length);

                    // Add file data (field name is "name" per the HTML form)
                    byte[] header = encoding.GetBytes($"--{boundary}\r\nContent-Disposition: form-data" + 
                        $"; name=\"name\"; filename=\"{fileName}\"\r\nContent-Type: application/octet-stream\r\n\r\n");
                    await requestStream.WriteAsync(header, 0, header.Length);
                    await requestStream.WriteAsync(fileData, 0, fileData.Length);

                    // Add p_ssord_id field (modem number)
                    byte[] ssordField = encoding.GetBytes($"\r\n--{boundary}\r\nContent-Disposition: form-data; name=\"p_ssord_id\"\r\n\r\n{destinationModemNumber}\r\n");
                    await requestStream.WriteAsync(ssordField, 0, ssordField.Length);

                    // Close boundary
                    byte[] footer = encoding.GetBytes($"--{boundary}--\r\n");
                    await requestStream.WriteAsync(footer, 0, footer.Length);
                }

                OnUploadProgress(new UploadProgressEventArgs { FileName = fileName, BytesUploaded = fileData.Length, TotalBytes = fileData.Length });

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    bool success = response.StatusCode == HttpStatusCode.OK;

                    OnUploadCompleted(new UploadCompletedEventArgs
                    {
                        FileName = fileName,
                        Success = success,
                        ErrorMessage = success ? null : $"Server returned {response.StatusCode}"
                    });

                    return success;
                }
            }
            catch (WebException wex)
            {
                LastError = $"Upload failed: {wex.Message}";
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        LastError += "\r\n" + reader.ReadToEnd();
                    }
                }

                OnUploadCompleted(new UploadCompletedEventArgs
                {
                    FileName = Path.GetFileName(filePath),
                    Success = false,
                    ErrorMessage = LastError
                });

                return false;
            }
            catch (Exception ex)
            {
                LastError = $"Upload failed: {ex.Message}";

                OnUploadCompleted(new UploadCompletedEventArgs
                {
                    FileName = Path.GetFileName(filePath),
                    Success = false,
                    ErrorMessage = LastError
                });

                return false;
            }
        }

        protected virtual void OnUploadProgress(UploadProgressEventArgs e)
        {
            UploadProgress?.Invoke(this, e);
        }

        protected virtual void OnUploadCompleted(UploadCompletedEventArgs e)
        {
            UploadCompleted?.Invoke(this, e);
        }
    }

    public class UploadProgressEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public long BytesUploaded { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage => TotalBytes > 0 ? (int)((BytesUploaded * 100) / TotalBytes) : 0;
    }

    public class UploadCompletedEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
