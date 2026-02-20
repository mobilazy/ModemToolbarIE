using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ModemWebUtility
{
    /// <summary>
    /// Handles downloading attachments from modem pages
    /// </summary>
    public class AttachmentDownloader
    {
        private readonly CookieContainer cookieContainer = new CookieContainer();
        
        public string LastError { get; private set; }

        public event EventHandler<DownloadProgressEventArgs> DownloadProgress;
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        /// <summary>
        /// Downloads a file using Windows Integrated Authentication
        /// </summary>
        public async Task<bool> DownloadFileAsync(AttachmentInfo attachment, string destinationFolder)
        {
            LastError = null;
            
            try
            {
                Directory.CreateDirectory(destinationFolder);
                var filePath = Path.Combine(destinationFolder, SanitizeFileName(attachment.FileName));

                switch (attachment.DownloadMethod)
                {
                    case DownloadMethod.DirectLink:
                        return await DownloadDirectLink(attachment.Url, filePath, attachment.FileName);

                    case DownloadMethod.JavaScript:
                        // Extract URL from JavaScript and download
                        return await DownloadDirectLink(attachment.Url, filePath, attachment.FileName);

                    case DownloadMethod.FormSubmit:
                        return await DownloadViaForm(attachment, filePath);

                    default:
                        LastError = "Unknown download method";
                        return false;
                }
            }
            catch (Exception ex)
            {
                LastError = $"{ex.Message}";
                OnDownloadCompleted(new DownloadCompletedEventArgs
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    FilePath = ""
                });
                return false;
            }
        }

        private async Task<bool> DownloadDirectLink(string url, string filePath, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    LastError = "URL is empty";
                    return false;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.PreAuthenticate = true;
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    long totalBytes = response.ContentLength;
                    long downloadedBytes = 0;

                    using (Stream responseStream = response.GetResponseStream())
                    using (FileStream fileStream = File.Create(filePath))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            downloadedBytes += bytesRead;

                            // Report progress
                            if (totalBytes > 0)
                            {
                                int percentComplete = (int)((downloadedBytes * 100) / totalBytes);
                                OnDownloadProgress(new DownloadProgressEventArgs
                                {
                                    FileName = fileName,
                                    BytesDownloaded = downloadedBytes,
                                    TotalBytes = totalBytes,
                                    PercentComplete = percentComplete
                                });
                            }
                        }
                    }
                }

                OnDownloadCompleted(new DownloadCompletedEventArgs
                {
                    Success = true,
                    FilePath = filePath,
                    ErrorMessage = null
                });

                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Error downloading {fileName}: {ex.Message}";
                
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { }
                }

                OnDownloadCompleted(new DownloadCompletedEventArgs
                {
                    Success = false,
                    FilePath = filePath,
                    ErrorMessage = ex.Message
                });
                
                return false;
            }
        }

        private async Task<bool> DownloadViaForm(AttachmentInfo attachment, string filePath)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(attachment.FormAction);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.PreAuthenticate = true;
                request.Method = attachment.FormMethod?.ToUpper() ?? "POST";
                request.CookieContainer = cookieContainer;
                request.ContentType = "application/x-www-form-urlencoded";

                // For now, submit empty form data
                // TODO: Extract form fields if needed
                if (request.Method == "POST")
                {
                    byte[] postData = new byte[0];
                    request.ContentLength = postData.Length;
                    
                    using (Stream requestStream = await request.GetRequestStreamAsync())
                    {
                        await requestStream.WriteAsync(postData, 0, postData.Length);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = File.Create(filePath))
                {
                    await responseStream.CopyToAsync(fileStream);
                }

                OnDownloadCompleted(new DownloadCompletedEventArgs
                {
                    Success = true,
                    FilePath = filePath,
                    ErrorMessage = null
                });

                return true;
            }
            catch (Exception ex)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                throw;
            }
        }

        /// <summary>
        /// Synchronous download for compatibility
        /// </summary>
        public bool DownloadFile(AttachmentInfo attachment, string destinationFolder)
        {
            return DownloadFileAsync(attachment, destinationFolder).GetAwaiter().GetResult();
        }

        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "download.file";

            // Remove invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }

        protected virtual void OnDownloadProgress(DownloadProgressEventArgs e)
        {
            DownloadProgress?.Invoke(this, e);
        }

        protected virtual void OnDownloadCompleted(DownloadCompletedEventArgs e)
        {
            DownloadCompleted?.Invoke(this, e);
        }
    }

    public class DownloadProgressEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public int PercentComplete { get; set; }
    }

    public class DownloadCompletedEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
    }
}
