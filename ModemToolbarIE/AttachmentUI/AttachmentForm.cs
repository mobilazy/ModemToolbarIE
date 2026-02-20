using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ModemToolbarIE.AttachmentUI
{
    public partial class AttachmentForm : Form
    {
        private Toolbar engine;
        private List<AttachmentInfo> attachments;
        private AttachmentDownloader downloader;
        private string defaultDownloadPath;

        public AttachmentForm(Toolbar engine)
        {
            this.engine = engine;
            this.downloader = new AttachmentDownloader();
            this.defaultDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ModemAttachments");

            InitializeComponent();
            SetupEventHandlers();
            LoadAttachments();
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

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 12);
            this.lblTitle.Text = "Attachments & Files";

            // 
            // lvAttachments
            // 
            this.lvAttachments.CheckBoxes = true;
            this.lvAttachments.FullRowSelect = true;
            this.lvAttachments.GridLines = true;
            this.lvAttachments.Location = new Point(12, 45);
            this.lvAttachments.Size = new Size(760, 400);
            this.lvAttachments.View = View.Details;
            this.lvAttachments.Columns.Add("File Name", 250);
            this.lvAttachments.Columns.Add("Type", 80);
            this.lvAttachments.Columns.Add("Link Text", 200);
            this.lvAttachments.Columns.Add("Method", 100);
            this.lvAttachments.Columns.Add("URL", 130);
            this.lvAttachments.DoubleClick += LvAttachments_DoubleClick;

            // 
            // txtDownloadPath
            // 
            this.txtDownloadPath.Location = new Point(12, 455);
            this.txtDownloadPath.Size = new Size(600, 23);
            this.txtDownloadPath.Text = defaultDownloadPath;

            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new Point(618, 453);
            this.btnBrowse.Size = new Size(75, 27);
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += BtnBrowse_Click;

            // 
            // btnDownloadSelected
            // 
            this.btnDownloadSelected.Location = new Point(12, 490);
            this.btnDownloadSelected.Size = new Size(150, 30);
            this.btnDownloadSelected.Text = "Download Selected";
            this.btnDownloadSelected.Click += BtnDownloadSelected_Click;

            // 
            // btnDownloadAll
            // 
            this.btnDownloadAll.Location = new Point(168, 490);
            this.btnDownloadAll.Size = new Size(120, 30);
            this.btnDownloadAll.Text = "Download All";
            this.btnDownloadAll.Click += BtnDownloadAll_Click;

            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new Point(294, 490);
            this.btnRefresh.Size = new Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += BtnRefresh_Click;

            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new Point(400, 490);
            this.btnOpenFolder.Size = new Size(120, 30);
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.Click += BtnOpenFolder_Click;

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(12, 530);
            this.progressBar.Size = new Size(760, 23);

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(12, 560);
            this.lblStatus.Size = new Size(760, 15);
            this.lblStatus.Text = "Ready";

            // 
            // AttachmentForm
            // 
            this.ClientSize = new Size(784, 591);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lvAttachments);
            this.Controls.Add(this.txtDownloadPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnDownloadSelected);
            this.Controls.Add(this.btnDownloadAll);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Modem Attachments";
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

        private void LoadAttachments()
        {
            lvAttachments.Items.Clear();
            attachments = new List<AttachmentInfo>();

            try
            {
                var htmlDoc = engine.HtmlDoc;
                var modemNo = engine.ModemNoEngine;

                // Extract attachments based on current page type
                if (engine.ModemStat == BandObjectLib.ModemEvents.GantTools)
                {
                    var gantParams = new GantParameters(htmlDoc, modemNo);
                    attachments.AddRange(gantParams.Attachments);
                    lblTitle.Text = $"Attachments & Files - Gant Tools (mob_id: {modemNo})";
                }
                else
                {
                    // Extract from any modem page
                    var gantParams = new GantParameters(htmlDoc, modemNo);
                    attachments.AddRange(gantParams.Attachments);
                    lblTitle.Text = $"Attachments & Files - Modem {modemNo}";
                }

                // Populate ListView
                foreach (var attachment in attachments)
                {
                    var item = new ListViewItem(attachment.FileName);
                    item.SubItems.Add(attachment.FileType);
                    item.SubItems.Add(attachment.LinkText);
                    item.SubItems.Add(attachment.DownloadMethod.ToString());
                    item.SubItems.Add(TruncateUrl(attachment.Url, 30));
                    item.Tag = attachment;
                    lvAttachments.Items.Add(item);
                }

                lblStatus.Text = $"Found {attachments.Count} attachment(s)";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading attachments: {ex.Message}";
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDownloadSelected_Click(object sender, EventArgs e)
        {
            var selectedItems = GetCheckedItems();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one file to download.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var downloadPath = txtDownloadPath.Text;
            btnDownloadSelected.Enabled = false;
            btnDownloadAll.Enabled = false;

            try
            {
                foreach (var item in selectedItems)
                {
                    var attachment = item.Tag as AttachmentInfo;
                    lblStatus.Text = $"Downloading {attachment.FileName}...";
                    await downloader.DownloadFileAsync(attachment, downloadPath);
                }

                lblStatus.Text = $"Downloaded {selectedItems.Count} file(s) successfully";
                MessageBox.Show($"Downloaded {selectedItems.Count} file(s) to:\n{downloadPath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Download error: {ex.Message}";
                MessageBox.Show($"Error: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDownloadSelected.Enabled = true;
                btnDownloadAll.Enabled = true;
                progressBar.Value = 0;
            }
        }

        private async void BtnDownloadAll_Click(object sender, EventArgs e)
        {
            if (attachments.Count == 0)
            {
                MessageBox.Show("No attachments found.", "No Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Download all {attachments.Count} file(s)?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var downloadPath = txtDownloadPath.Text;
            btnDownloadSelected.Enabled = false;
            btnDownloadAll.Enabled = false;

            try
            {
                int count = 0;
                foreach (var attachment in attachments)
                {
                    count++;
                    lblStatus.Text = $"Downloading {count}/{attachments.Count}: {attachment.FileName}...";
                    await downloader.DownloadFileAsync(attachment, downloadPath);
                }

                lblStatus.Text = $"Downloaded {attachments.Count} file(s) successfully";
                MessageBox.Show($"Downloaded {attachments.Count} file(s) to:\n{downloadPath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Download error: {ex.Message}";
                MessageBox.Show($"Error: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDownloadSelected.Enabled = true;
                btnDownloadAll.Enabled = true;
                progressBar.Value = 0;
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAttachments();
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            var downloadPath = txtDownloadPath.Text;
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            Process.Start("explorer.exe", downloadPath);
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

                var info = $"File Name: {attachment.FileName}\n" +
                          $"File Type: {attachment.FileType}\n" +
                          $"Link Text: {attachment.LinkText}\n" +
                          $"Download Method: {attachment.DownloadMethod}\n" +
                          $"URL: {attachment.Url}";

                MessageBox.Show(info, "Attachment Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void UpdateProgress(DownloadProgressEventArgs e)
        {
            progressBar.Value = e.PercentComplete;
            lblStatus.Text = $"Downloading {e.FileName}: {e.PercentComplete}% ({FormatBytes(e.BytesDownloaded)} / {FormatBytes(e.TotalBytes)})";
        }

        private void DownloadCompleted(DownloadCompletedEventArgs e)
        {
            if (e.Success)
            {
                // Success handled in button click handler
            }
            else
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
        private Button btnDownloadSelected;
        private Button btnDownloadAll;
        private Button btnRefresh;
        private Button btnOpenFolder;
        private TextBox txtDownloadPath;
        private Button btnBrowse;
        private Label lblStatus;
        private ProgressBar progressBar;
        private Label lblTitle;
    }
}
