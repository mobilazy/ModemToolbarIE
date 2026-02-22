using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemMergerWinFormsApp
{
    public class AttachmentForm : Form
    {
        private List<AttachmentInfo> attachments;
        private AttachmentDownloader downloader;
        private AttachmentUploader uploader;
        private string defaultDownloadPath;
        private string modemNumber;
        private GantParameters gantParams;

        public AttachmentForm() : this(string.Empty)
        {
        }

        public AttachmentForm(string modemNumber)
        {
            this.modemNumber = modemNumber;
            this.downloader = new AttachmentDownloader();
            this.uploader = new AttachmentUploader();
            this.defaultDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ModemAttachments");

            InitializeComponent();
            SetupEventHandlers();
            
            if (!string.IsNullOrEmpty(modemNumber))
            {
                txtModemNumber.Text = modemNumber;
                LoadAttachments();
            }
        }

        private void InitializeComponent()
        {
            this.lvAttachments = new ListView();
            this.btnDownloadSelected = new Button();
            this.btnDownloadAll = new Button();
            this.btnRefresh = new Button();
            this.btnOpenFolder = new Button();
            this.txtDownloadPath = new TextBox();
            this.btnBrowse = new Button();
            this.lblStatus = new Label();
            this.progressBar = new ProgressBar();
            this.lblTitle = new Label();
            this.txtModemNumber = new TextBox();
            this.btnLoadModem = new Button();
            this.lblModemNumber = new Label();
            this.tabControl = new TabControl();
            this.tabAttachments = new TabPage();
            this.tabGantTools = new TabPage();
            this.lvGantTools = new ListView();
            this.lblDestModem = new Label();
            this.txtDestModem = new TextBox();
            this.btnCopySelected = new Button();
            this.btnCopyAll = new Button();
            this.btnCopyGantTools = new Button();

            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Text = "Modem File Manager";

            // 
            // lblModemNumber
            // 
            this.lblModemNumber.AutoSize = true;
            this.lblModemNumber.Location = new System.Drawing.Point(12, 45);
            this.lblModemNumber.Text = "Modem Number:";

            // 
            // txtModemNumber
            // 
            this.txtModemNumber.Location = new System.Drawing.Point(120, 42);
            this.txtModemNumber.Size = new System.Drawing.Size(120, 23);
            this.txtModemNumber.MaxLength = 7;

            // 
            // btnLoadModem
            // 
            this.btnLoadModem.Location = new System.Drawing.Point(245, 40);
            this.btnLoadModem.Size = new System.Drawing.Size(100, 27);
            this.btnLoadModem.Text = "Load Modem";
            this.btnLoadModem.Click += BtnLoadModem_Click;

            // 
            // btnCopyHeader
            // 
            this.btnCopyHeader = new Button();
            this.btnCopyHeader.Location = new System.Drawing.Point(350, 40);
            this.btnCopyHeader.Size = new System.Drawing.Size(105, 27);
            this.btnCopyHeader.Text = "Copy Header";
            this.btnCopyHeader.Click += BtnCopyHeader_Click;

            // 
            // tabControl
            // 
            this.tabControl.Location = new System.Drawing.Point(12, 75);
            this.tabControl.Size = new System.Drawing.Size(760, 430);
            this.tabControl.Controls.Add(this.tabAttachments);
            this.tabControl.Controls.Add(this.tabGantTools);

            // 
            // tabAttachments
            // 
            this.tabAttachments.Text = "Attachments";
            this.tabAttachments.Controls.Add(this.lvAttachments);

            // 
            // lvAttachments
            // 
            this.lvAttachments.CheckBoxes = true;
            this.lvAttachments.FullRowSelect = true;
            this.lvAttachments.GridLines = true;
            this.lvAttachments.Location = new System.Drawing.Point(3, 3);
            this.lvAttachments.Size = new System.Drawing.Size(750, 395);
            this.lvAttachments.View = View.Details;
            this.lvAttachments.Columns.Add("File Name", 200);
            this.lvAttachments.Columns.Add("Type", 60);
            this.lvAttachments.Columns.Add("Doc Type", 100);
            this.lvAttachments.Columns.Add("Link Text", 150);
            this.lvAttachments.Columns.Add("Method", 80);
            this.lvAttachments.Columns.Add("URL", 160);
            this.lvAttachments.DoubleClick += LvAttachments_DoubleClick;

            // 
            // tabGantTools
            // 
            this.tabGantTools.Text = "Gant Tools";
            this.tabGantTools.Controls.Add(this.lvGantTools);

            // 
            // lvGantTools
            // 
            this.lvGantTools.FullRowSelect = true;
            this.lvGantTools.GridLines = true;
            this.lvGantTools.Location = new System.Drawing.Point(3, 3);
            this.lvGantTools.Size = new System.Drawing.Size(750, 395);
            this.lvGantTools.View = View.Details;
            this.lvGantTools.Columns.Add("Tool Name", 300);
            this.lvGantTools.Columns.Add("Description", 350);
            this.lvGantTools.Columns.Add("Additional Data", 100);

            // 
            // txtDownloadPath
            // 
            this.txtDownloadPath.Location = new System.Drawing.Point(12, 515);
            this.txtDownloadPath.Size = new System.Drawing.Size(600, 23);
            this.txtDownloadPath.Text = defaultDownloadPath;

            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(618, 513);
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += BtnBrowse_Click;

            // 
            // btnDownloadSelected
            // 
            this.btnDownloadSelected.Location = new System.Drawing.Point(12, 550);
            this.btnDownloadSelected.Size = new System.Drawing.Size(150, 30);
            this.btnDownloadSelected.Text = "Download Selected";
            this.btnDownloadSelected.Click += BtnDownloadSelected_Click;

            // 
            // btnDownloadAll
            // 
            this.btnDownloadAll.Location = new System.Drawing.Point(168, 550);
            this.btnDownloadAll.Size = new System.Drawing.Size(120, 30);
            this.btnDownloadAll.Text = "Download All";
            this.btnDownloadAll.Click += BtnDownloadAll_Click;

            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(294, 550);
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += BtnRefresh_Click;

            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(400, 550);
            this.btnOpenFolder.Size = new System.Drawing.Size(120, 30);
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.Click += BtnOpenFolder_Click;

            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 620);
            this.progressBar.Size = new System.Drawing.Size(760, 23);

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 645);
            this.lblStatus.Size = new System.Drawing.Size(760, 15);
            this.lblStatus.Text = "Enter modem number and click 'Load Modem'";

            // 
            // lblDestModem
            // 
            this.lblDestModem.AutoSize = true;
            this.lblDestModem.Location = new System.Drawing.Point(530, 550);
            this.lblDestModem.Text = "Copy to Modem:";

            // 
            // txtDestModem
            // 
            this.txtDestModem.Location = new System.Drawing.Point(630, 547);
            this.txtDestModem.Size = new System.Drawing.Size(100, 23);
            this.txtDestModem.MaxLength = 7;

            // 
            // btnCopySelected
            // 
            this.btnCopySelected.Location = new System.Drawing.Point(12, 585);
            this.btnCopySelected.Size = new System.Drawing.Size(150, 30);
            this.btnCopySelected.Text = "Copy Selected";
            this.btnCopySelected.Click += BtnCopySelected_Click;

            // 
            // btnCopyAll
            // 
            this.btnCopyAll.Location = new System.Drawing.Point(168, 585);
            this.btnCopyAll.Size = new System.Drawing.Size(140, 30);
            this.btnCopyAll.Text = "Copy All Attachments";
            this.btnCopyAll.Click += BtnCopyAll_Click;

            // 
            // btnCopyGantTools
            // 
            this.btnCopyGantTools.Location = new System.Drawing.Point(314, 585);
            this.btnCopyGantTools.Size = new System.Drawing.Size(140, 30);
            this.btnCopyGantTools.Text = "Copy Gant Tools";
            this.btnCopyGantTools.Click += BtnCopyGantTools_Click;

            // 
            // AttachmentForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 680);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblModemNumber);
            this.Controls.Add(this.txtModemNumber);
            this.Controls.Add(this.btnLoadModem);
            this.Controls.Add(this.btnCopyHeader);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.txtDownloadPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnDownloadSelected);
            this.Controls.Add(this.btnDownloadAll);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.lblDestModem);
            this.Controls.Add(this.txtDestModem);
            this.Controls.Add(this.btnCopySelected);
            this.Controls.Add(this.btnCopyAll);
            this.Controls.Add(this.btnCopyGantTools);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Modem File Manager";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupEventHandlers()
        {
            downloader.DownloadProgress += (s, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateProgress(e)));
                }
                else
                {
                    UpdateProgress(e);
                }
            };

            downloader.DownloadCompleted += (s, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => DownloadCompleted(e)));
                }
                else
                {
                    DownloadCompleted(e);
                }
            };
        }

        private void BtnLoadModem_Click(object sender, EventArgs e)
        {
            modemNumber = txtModemNumber.Text.Trim();
            if (string.IsNullOrWhiteSpace(modemNumber) || modemNumber.Length != 7)
            {
                MessageBox.Show("Please enter a valid 7-digit modem number.", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadAttachments();
        }

        private void BtnCopyHeader_Click(object sender, EventArgs e)
        {
            string sourceModem = txtModemNumber.Text.Trim();
            string destModem = txtDestModem.Text.Trim();

            if (string.IsNullOrWhiteSpace(sourceModem) || sourceModem.Length != 7)
            {
                MessageBox.Show("Please enter a valid 7-digit source modem number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(destModem) || destModem.Length != 7)
            {
                MessageBox.Show("Please enter a valid 7-digit destination modem number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sourceModem == destModem)
            {
                MessageBox.Show("Source and destination modem numbers must be different.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Copy header information from modem {sourceModem} to modem {destModem}?\n\n" +
                "Do you want to continue?",
                "Confirm Copy Header",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                lblStatus.Text = $"Copying header from {sourceModem} to {destModem}...";
                Application.DoEvents();

                ModemHeaderCopy headerCopy = new ModemHeaderCopy(sourceModem, destModem);
                bool success = headerCopy.CopyHeaderFields();

                if (success)
                {
                    lblStatus.Text = $"Header successfully copied from modem {sourceModem} to {destModem}";
                    MessageBox.Show($"Header fields successfully copied from modem {sourceModem} to modem {destModem}.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblStatus.Text = $"Failed to copy header from {sourceModem} to {destModem}";
                    MessageBox.Show($"Failed to copy header. The server returned an error.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error copying header: {ex.Message}";
                MessageBox.Show($"Error copying header:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAttachments()
        {
            lvAttachments.Items.Clear();
            lvGantTools.Items.Clear();
            attachments = new List<AttachmentInfo>();

            try
            {
                lblStatus.Text = $"Loading modem {modemNumber}...";
                Application.DoEvents();

                // Load attachments from modem view page (where docreg.download links are)
                string modemViewUrl = HDocUtility.UrlModemView + modemNumber;
                ModemConnection mcView = new ModemConnection(modemViewUrl);
                var viewHtmlDoc = mcView.GetHtmlAsHdoc();

                var viewParams = new GantParameters(viewHtmlDoc, modemNumber);
                attachments.AddRange(viewParams.Attachments);

                // Load Gant tools from gant_tools page
                string gantToolsUrl = HDocUtility.UrlGantTools + modemNumber;
                ModemConnection mcGant = new ModemConnection(gantToolsUrl);
                var gantHtmlDoc = mcGant.GetHtmlAsHdoc();

                gantParams = new GantParameters(gantHtmlDoc, modemNumber);
                
                // DEBUG: Show what HTML we're working with if no attachments found
                if (attachments.Count == 0)
                {
                    var debugInfo = $"Modem View HTML length: {viewHtmlDoc.DocumentNode.OuterHtml.Length} chars\n";
                    debugInfo += $"Found {attachments.Count} attachments\n";
                    
                    // Show all links found for debugging
                    var allLinks = viewHtmlDoc.DocumentNode.SelectNodes("//a[@href]");
                    if (allLinks != null)
                    {
                        debugInfo += $"\nAll {allLinks.Count} links on modem view page:\n";
                        foreach (var link in allLinks.Take(20))
                        {
                            var href = link.GetAttributeValue("href", "");
                            var text = System.Net.WebUtility.HtmlDecode(link.InnerText?.Trim() ?? "");
                            debugInfo += $"  - '{text}' -> {href.Substring(0, Math.Min(80, href.Length))}\n";
                        }
                    }
                    else
                    {
                        debugInfo += "No links found on page at all!\n";
                    }
                    
                    MessageBox.Show(debugInfo, "Debug: No Attachments Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                foreach (var attachment in attachments)
                {
                    var item = new ListViewItem(attachment.FileName);
                    item.SubItems.Add(attachment.FileType);
                    item.SubItems.Add(GetDocTypDescription(attachment.DocTyp));
                    item.SubItems.Add(attachment.LinkText);
                    item.SubItems.Add(attachment.DownloadMethod.ToString());
                    item.SubItems.Add(TruncateUrl(attachment.Url, 30));
                    item.Tag = attachment;
                    lvAttachments.Items.Add(item);
                }

                // Load Gant tools
                foreach (var tool in gantParams.Tools)
                {
                    if (!string.IsNullOrWhiteSpace(tool.ToolName))
                    {
                        var item = new ListViewItem(tool.ToolName);
                        item.SubItems.Add(tool.ToolDescription);
                        item.SubItems.Add(tool.AdditionalData);
                        item.Tag = tool;
                        lvGantTools.Items.Add(item);
                    }
                }

                lblTitle.Text = $"Modem File Manager - Modem {modemNumber}";
                lblStatus.Text = $"Found {attachments.Count} attachment(s) and {gantParams.Tools.Count} tool(s)";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading modem: {ex.Message}";
                MessageBox.Show($"Error loading modem {modemNumber}:\n\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDownloadSelected_Click(object sender, EventArgs e)
        {
            var selectedItems = GetCheckedItems();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one file to download.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var downloadPath = txtDownloadPath.Text;
            SetButtonsEnabled(false);

            int successCount = 0;
            int failCount = 0;
            string lastError = "";

            try
            {
                foreach (var item in selectedItems)
                {
                    var attachment = item.Tag as AttachmentInfo;
                    if (attachment != null)
                    {
                        lblStatus.Text = $"Downloading {attachment.FileName}...";
                        Application.DoEvents();
                        
                        bool success = await downloader.DownloadFileAsync(attachment, downloadPath);
                        if (success)
                        {
                            successCount++;
                        }
                        else
                        {
                            failCount++;
                            lastError = downloader.LastError ?? $"Failed to download {attachment.FileName}";
                        }
                    }
                }

                if (failCount == 0)
                {
                    lblStatus.Text = $"Downloaded {successCount} file(s) successfully";
                    MessageBox.Show($"Downloaded {successCount} file(s) to:\n{downloadPath}", 
                        "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblStatus.Text = $"Downloaded {successCount}, failed {failCount}";
                    MessageBox.Show($"Success: {successCount}\nFailed: {failCount}\n\nLast error: {lastError}\n\nCheck files at:\n{downloadPath}", 
                        "Download Completed with Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Download error: {ex.Message}";
                MessageBox.Show($"Error: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", "Download Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetButtonsEnabled(true);
                progressBar.Value = 0;
            }
        }

        private async void BtnDownloadAll_Click(object sender, EventArgs e)
        {
            if (attachments.Count == 0)
            {
                MessageBox.Show("No attachments found.", "No Files", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Download all {attachments.Count} file(s)?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var downloadPath = txtDownloadPath.Text;
            SetButtonsEnabled(false);

            int successCount = 0;
            int failCount = 0;
            string lastError = "";

            try
            {
                int count = 0;
                foreach (var attachment in attachments)
                {
                    count++;
                    lblStatus.Text = $"Downloading {count}/{attachments.Count}: {attachment.FileName}...";
                    Application.DoEvents();
                    
                    bool success = await downloader.DownloadFileAsync(attachment, downloadPath);
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        lastError = downloader.LastError ?? $"Failed to download {attachment.FileName}";
                    }
                }

                if (failCount == 0)
                {
                    lblStatus.Text = $"Downloaded {successCount} file(s) successfully";
                    MessageBox.Show($"Downloaded {successCount} file(s) to:\n{downloadPath}", 
                        "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblStatus.Text = $"Downloaded {successCount}, failed {failCount}";
                    MessageBox.Show($"Success: {successCount}\nFailed: {failCount}\n\nLast error: {lastError}\n\nCheck files at:\n{downloadPath}", 
                        "Download Completed with Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Download error: {ex.Message}";
                MessageBox.Show($"Error: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", "Download Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetButtonsEnabled(true);
                progressBar.Value = 0;
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(modemNumber))
            {
                LoadAttachments();
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            var downloadPath = txtDownloadPath.Text;
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = downloadPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = txtDownloadPath.Text;
                dialog.Description = "Select download folder";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDownloadPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void LvAttachments_DoubleClick(object sender, EventArgs e)
        {
            if (lvAttachments.SelectedItems.Count > 0)
            {
                var item = lvAttachments.SelectedItems[0];
                var attachment = item.Tag as AttachmentInfo;
                if (attachment != null)
                {
                    var info = $"File Name: {attachment.FileName}\n" +
                              $"File Type: {attachment.FileType}\n" +
                              $"Link Text: {attachment.LinkText}\n" +
                              $"Download Method: {attachment.DownloadMethod}\n" +
                              $"URL: {attachment.Url}";

                    MessageBox.Show(info, "Attachment Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void BtnCopySelected_Click(object sender, EventArgs e)
        {
            string destModem = txtDestModem.Text.Trim();
            if (string.IsNullOrWhiteSpace(destModem) || destModem.Length != 7)
            {
                MessageBox.Show("Please enter a valid 7-digit destination modem number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var checkedItems = GetCheckedItems();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one attachment to copy.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await CopyAttachments(checkedItems, destModem);
        }

        private async void BtnCopyAll_Click(object sender, EventArgs e)
        {
            string destModem = txtDestModem.Text.Trim();
            if (string.IsNullOrWhiteSpace(destModem) || destModem.Length != 7)
            {
                MessageBox.Show("Please enter a valid 7-digit destination modem number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lvAttachments.Items.Count == 0)
            {
                MessageBox.Show("No attachments to copy.", "No Attachments",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var allItems = new List<ListViewItem>();
            foreach (ListViewItem item in lvAttachments.Items)
            {
                allItems.Add(item);
            }

            await CopyAttachments(allItems, destModem);
        }

        private async void BtnCopyGantTools_Click(object sender, EventArgs e)
        {
            string destModem = txtDestModem.Text.Trim();
            if (string.IsNullOrWhiteSpace(destModem) || destModem.Length != 7)
            {
                MessageBox.Show("Please enter a valid 7-digit destination modem number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(modemNumber))
            {
                MessageBox.Show("Please load a source modem first.", "No Source Modem",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"This will copy Gant tool configuration from modem {modemNumber} to modem {destModem}.\n\nContinue?",
                "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            SetButtonsEnabled(false);
            lblStatus.Text = $"Copying Gant tools from {modemNumber} to {destModem}...";
            Application.DoEvents();

            try
            {
                var copier = new GantToolCopier();
                bool success = await copier.CopyGantToolsAsync(modemNumber, destModem);

                if (success)
                {
                    lblStatus.Text = $"Successfully copied Gant tools to modem {destModem}";
                    
                    // Prepare full log
                    string fullLog = $"Gant tools copied successfully to modem {destModem}!\n\n";
                    if (!string.IsNullOrWhiteSpace(copier.DebugInfo))
                    {
                        fullLog += "=== Debug Info ===\n" + copier.DebugInfo;
                    }
                    
                    // Show dialog with Copy Log button
                    ShowSuccessDialog($"Gant tools copied successfully to modem {destModem}!", fullLog);
                }
                else
                {
                    lblStatus.Text = $"Failed to copy Gant tools: {copier.LastError}";
                    
                    // Build full error message with debug info
                    string fullMessage = $"Failed to copy Gant tools:\n\n{copier.LastError}";
                    if (!string.IsNullOrWhiteSpace(copier.DebugInfo))
                    {
                        fullMessage += "\n\n=== Debug Info ===\n" + copier.DebugInfo;
                    }
                    
                    // Save to temp file for easy viewing
                    string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"GantToolsDebug_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    System.IO.File.WriteAllText(tempFile, fullMessage);
                    
                    // Copy to clipboard
                    try { Clipboard.SetText(fullMessage); } catch { }
                    
                    // Show message with file path and clipboard notice
                    string displayMessage = $"{copier.LastError}\n\n";
                    displayMessage += "Debug info copied to clipboard!\n";
                    displayMessage += $"Also saved to: {tempFile}\n\n";
                    displayMessage += "First 500 chars of debug:\n" + 
                                     (copier.DebugInfo.Length > 500 
                                         ? copier.DebugInfo.Substring(0, 500) + "..." 
                                         : copier.DebugInfo);
                    
                    MessageBox.Show(displayMessage, "Copy Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Error copying Gant tools:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        private void ShowSuccessDialog(string message, string logText)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Success";
                dialog.Size = new Size(400, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                
                var lblMessage = new Label
                {
                    Text = message,
                    Location = new Point(20, 20),
                    Size = new Size(340, 40),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                
                var btnOK = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(200, 75),
                    Size = new Size(80, 30)
                };
                
                var btnCopyLog = new Button
                {
                    Text = "Copy Log",
                    Location = new Point(290, 75),
                    Size = new Size(80, 30)
                };
                
                btnCopyLog.Click += (s, e) =>
                {
                    try
                    {
                        Clipboard.SetText(logText);
                        MessageBox.Show("Log copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to copy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                
                dialog.Controls.Add(lblMessage);
                dialog.Controls.Add(btnOK);
                dialog.Controls.Add(btnCopyLog);
                dialog.AcceptButton = btnOK;
                
                dialog.ShowDialog(this);
            }
        }

        private async Task CopyAttachments(List<ListViewItem> items, string destModem)
        {
            SetButtonsEnabled(false);
            progressBar.Value = 0;

            int successCount = 0;
            int failCount = 0;
            string tempFolder = Path.Combine(Path.GetTempPath(), "ModemCopy_" + Guid.NewGuid().ToString("N"));

            try
            {
                Directory.CreateDirectory(tempFolder);

                for (int i = 0; i < items.Count; i++)
                {
                    var attachment = items[i].Tag as AttachmentInfo;
                    if (attachment == null) continue;

                    lblStatus.Text = $"Copying {i + 1}/{items.Count}: {attachment.FileName}...";
                    progressBar.Value = (i * 100) / items.Count;
                    Application.DoEvents();

                    try
                    {
                        // Download to temp folder
                        if (await downloader.DownloadFileAsync(attachment, tempFolder))
                        {
                            string tempFile = Path.Combine(tempFolder, attachment.FileName);

                            // Upload to destination modem with exact document type (1:1 copy)
                            if (await uploader.UploadFileAsync(tempFile, destModem, attachment.DocTyp))
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                                lblStatus.Text = $"Failed to upload {attachment.FileName}: {uploader.LastError}";
                                await Task.Delay(1000);
                            }
                        }
                        else
                        {
                            failCount++;
                            lblStatus.Text = $"Failed to download {attachment.FileName}: {downloader.LastError}";
                            await Task.Delay(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        lblStatus.Text = $"Error copying {attachment.FileName}: {ex.Message}";
                        await Task.Delay(1000);
                    }
                }

                progressBar.Value = 100;
                lblStatus.Text = $"Copy complete: {successCount} succeeded, {failCount} failed";

                MessageBox.Show($"Copied {successCount} attachment(s) to modem {destModem}.\n{failCount} failed.",
                    "Copy Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                // Clean up temp files
                try
                {
                    if (Directory.Exists(tempFolder))
                        Directory.Delete(tempFolder, true);
                }
                catch { }

                SetButtonsEnabled(true);
            }
        }

        private List<ListViewItem> GetCheckedItems()
        {
            var items = new List<ListViewItem>();
            foreach (ListViewItem item in lvAttachments.Items)
            {
                if (item.Checked)
                    items.Add(item);
            }
            return items;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            btnDownloadSelected.Enabled = enabled;
            btnDownloadAll.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            btnLoadModem.Enabled = enabled;
            btnCopySelected.Enabled = enabled;
            btnCopyAll.Enabled = enabled;
            btnCopyGantTools.Enabled = enabled;
        }

        private void UpdateProgress(DownloadProgressEventArgs e)
        {
            progressBar.Value = Math.Min(e.PercentComplete, 100);
            lblStatus.Text = $"Downloading {e.FileName}: {e.PercentComplete}% ({FormatBytes(e.BytesDownloaded)} / {FormatBytes(e.TotalBytes)})";
        }

        private void DownloadCompleted(DownloadCompletedEventArgs e)
        {
            if (!e.Success)
            {
                lblStatus.Text = $"Download failed: {e.ErrorMessage}";
            }
        }

        private string TruncateUrl(string url, int maxLength)
        {
            if (string.IsNullOrEmpty(url) || url.Length <= maxLength)
                return url;

            return url.Substring(0, maxLength) + "...";
        }

        private string GetDocTypDescription(string docTyp)
        {
            if (string.IsNullOrWhiteSpace(docTyp))
                return "Other (22)";

            switch (docTyp)
            {
                case "1": return "WinPul (1)";
                case "2": return "Shipping (2)";
                case "3": return "BHA (3)";
                case "4": return "Download (4)";
                case "22": return "Other (22)";
                default: return $"Unknown ({docTyp})";
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        // Component declarations
        private ListView lvAttachments;
        private ListView lvGantTools;
        private Button btnDownloadSelected;
        private Button btnDownloadAll;
        private Button btnRefresh;
        private Button btnOpenFolder;
        private TextBox txtDownloadPath;
        private Button btnBrowse;
        private Label lblStatus;
        private ProgressBar progressBar;
        private Label lblTitle;
        private TextBox txtModemNumber;
        private Button btnLoadModem;
        private Button btnCopyHeader;
        private Label lblModemNumber;
        private TabControl tabControl;
        private TabPage tabAttachments;
        private TabPage tabGantTools;
        private Label lblDestModem;
        private TextBox txtDestModem;
        private Button btnCopySelected;
        private Button btnCopyAll;
        private Button btnCopyGantTools;
    }
}
