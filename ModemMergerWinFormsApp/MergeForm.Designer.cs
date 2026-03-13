namespace ModemMergerWinFormsApp
{
    partial class MergeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabMerger = new System.Windows.Forms.TabPage();
            this.tabKabal = new System.Windows.Forms.TabPage();
            this.txtModemNo = new System.Windows.Forms.TextBox();
            this.btnGetModem = new System.Windows.Forms.Button();
            this.btnFileManager = new System.Windows.Forms.Button();
            this.btnResetForm = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddSelected = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.txtTargetModem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRenameMain = new System.Windows.Forms.CheckBox();
            this.txtRenameMainTo = new System.Windows.Forms.TextBox();
            // Kabal tab controls
            this.lblKabalCustomer = new System.Windows.Forms.Label();
            this.cmbKabalCustomer = new System.Windows.Forms.ComboBox();
            this.lblKabalRig = new System.Windows.Forms.Label();
            this.cmbKabalRig = new System.Windows.Forms.ComboBox();
            this.lblKabalStartDate = new System.Windows.Forms.Label();
            this.dtpKabalStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblKabalShiftDays = new System.Windows.Forms.Label();
            this.txtKabalShiftDays = new System.Windows.Forms.TextBox();
            this.btnKabalLoadModems = new System.Windows.Forms.Button();
            this.btnKabalShiftDates = new System.Windows.Forms.Button();
            this.btnKabalSyncKabal = new System.Windows.Forms.Button();
            this.dgvKabal = new System.Windows.Forms.DataGridView();
            this.lblKabalStatus = new System.Windows.Forms.Label();
            this.lblKabalUser = new System.Windows.Forms.Label();
            this.txtKabalUser = new System.Windows.Forms.TextBox();
            this.lblKabalPass = new System.Windows.Forms.Label();
            this.txtKabalPass = new System.Windows.Forms.TextBox();
            this.chkKabalHeadless = new System.Windows.Forms.CheckBox();
            this.btnKabalCopyClipboard = new System.Windows.Forms.Button();
            this.tabMain.SuspendLayout();
            this.tabMerger.SuspendLayout();
            this.tabKabal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKabal)).BeginInit();
            this.SuspendLayout();
            //
            // tabMain
            //
            this.tabMain.Controls.Add(this.tabMerger);
            this.tabMain.Controls.Add(this.tabKabal);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1203, 704);
            this.tabMain.TabIndex = 100;
            //
            // tabMerger
            //
            this.tabMerger.Controls.Add(this.txtModemNo);
            this.tabMerger.Controls.Add(this.btnGetModem);
            this.tabMerger.Controls.Add(this.btnFileManager);
            this.tabMerger.Controls.Add(this.btnResetForm);
            this.tabMerger.Controls.Add(this.treeView1);
            this.tabMerger.Controls.Add(this.treeView2);
            this.tabMerger.Controls.Add(this.btnAdd);
            this.tabMerger.Controls.Add(this.btnCancel);
            this.tabMerger.Controls.Add(this.btnAddSelected);
            this.tabMerger.Controls.Add(this.btnRemoveSelected);
            this.tabMerger.Controls.Add(this.txtTargetModem);
            this.tabMerger.Controls.Add(this.label1);
            this.tabMerger.Controls.Add(this.chkRenameMain);
            this.tabMerger.Controls.Add(this.txtRenameMainTo);
            this.tabMerger.Location = new System.Drawing.Point(4, 24);
            this.tabMerger.Name = "tabMerger";
            this.tabMerger.Padding = new System.Windows.Forms.Padding(3);
            this.tabMerger.Size = new System.Drawing.Size(1195, 676);
            this.tabMerger.TabIndex = 0;
            this.tabMerger.Text = "Modem Merger";
            this.tabMerger.UseVisualStyleBackColor = true;
            //
            // tabKabal
            //
            this.tabKabal.Controls.Add(this.lblKabalCustomer);
            this.tabKabal.Controls.Add(this.cmbKabalCustomer);
            this.tabKabal.Controls.Add(this.lblKabalRig);
            this.tabKabal.Controls.Add(this.cmbKabalRig);
            this.tabKabal.Controls.Add(this.lblKabalStartDate);
            this.tabKabal.Controls.Add(this.dtpKabalStartDate);
            this.tabKabal.Controls.Add(this.lblKabalShiftDays);
            this.tabKabal.Controls.Add(this.txtKabalShiftDays);
            this.tabKabal.Controls.Add(this.btnKabalLoadModems);
            this.tabKabal.Controls.Add(this.btnKabalShiftDates);
            this.tabKabal.Controls.Add(this.btnKabalSyncKabal);
            this.tabKabal.Controls.Add(this.btnKabalCopyClipboard);
            this.tabKabal.Controls.Add(this.dgvKabal);
            this.tabKabal.Controls.Add(this.lblKabalStatus);
            this.tabKabal.Controls.Add(this.lblKabalUser);
            this.tabKabal.Controls.Add(this.txtKabalUser);
            this.tabKabal.Controls.Add(this.lblKabalPass);
            this.tabKabal.Controls.Add(this.txtKabalPass);
            this.tabKabal.Controls.Add(this.chkKabalHeadless);
            this.tabKabal.Location = new System.Drawing.Point(4, 24);
            this.tabKabal.Name = "tabKabal";
            this.tabKabal.Padding = new System.Windows.Forms.Padding(3);
            this.tabKabal.Size = new System.Drawing.Size(1195, 676);
            this.tabKabal.TabIndex = 1;
            this.tabKabal.Text = "Modem Shifter";
            this.tabKabal.UseVisualStyleBackColor = true;
            //
            // txtModemNo
            //
            this.txtModemNo.Location = new System.Drawing.Point(35, 15);
            this.txtModemNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtModemNo.MaxLength = 7;
            this.txtModemNo.Name = "txtModemNo";
            this.txtModemNo.Size = new System.Drawing.Size(161, 23);
            this.txtModemNo.TabIndex = 0;
            this.txtModemNo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtModemNo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtModemNo.TextChanged += new System.EventHandler(this.txtModemNo_TextChanged);
            this.txtModemNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtModemNo_KeyPress);
            //
            // btnGetModem
            //
            this.btnGetModem.Location = new System.Drawing.Point(203, 15);
            this.btnGetModem.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetModem.Name = "btnGetModem";
            this.btnGetModem.Size = new System.Drawing.Size(88, 26);
            this.btnGetModem.TabIndex = 1;
            this.btnGetModem.Text = "Get Modem";
            this.btnGetModem.UseVisualStyleBackColor = true;
            this.btnGetModem.Click += new System.EventHandler(this.btnGetModem_Click);
            //
            // btnFileManager
            //
            this.btnFileManager.Location = new System.Drawing.Point(299, 15);
            this.btnFileManager.Margin = new System.Windows.Forms.Padding(4);
            this.btnFileManager.Name = "btnFileManager";
            this.btnFileManager.Size = new System.Drawing.Size(100, 26);
            this.btnFileManager.TabIndex = 10;
            this.btnFileManager.Text = "File Manager";
            this.btnFileManager.UseVisualStyleBackColor = true;
            this.btnFileManager.Click += new System.EventHandler(this.btnFileManager_Click);
            //
            // btnResetForm
            //
            this.btnResetForm.Location = new System.Drawing.Point(407, 15);
            this.btnResetForm.Margin = new System.Windows.Forms.Padding(4);
            this.btnResetForm.Name = "btnResetForm";
            this.btnResetForm.Size = new System.Drawing.Size(100, 26);
            this.btnResetForm.TabIndex = 13;
            this.btnResetForm.Text = "Reset Form";
            this.btnResetForm.UseVisualStyleBackColor = true;
            this.btnResetForm.Click += new System.EventHandler(this.btnResetForm_Click);
            //
            // treeView1
            //
            this.treeView1.Location = new System.Drawing.Point(35, 62);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(509, 544);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.Enter += new System.EventHandler(this.treeView1_Enter);
            this.treeView1.Leave += new System.EventHandler(this.treeView1_Leave);
            //
            // treeView2
            //
            this.treeView2.Location = new System.Drawing.Point(645, 62);
            this.treeView2.Margin = new System.Windows.Forms.Padding(4);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(529, 544);
            this.treeView2.TabIndex = 3;
            this.treeView2.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView2_NodeMouseDoubleClick);
            this.treeView2.Enter += new System.EventHandler(this.treeView2_Enter);
            this.treeView2.Leave += new System.EventHandler(this.treeView2_Leave);
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(645, 615);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(116, 26);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(767, 615);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(106, 26);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnAddSelected
            //
            this.btnAddSelected.Location = new System.Drawing.Point(552, 195);
            this.btnAddSelected.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddSelected.Name = "btnAddSelected";
            this.btnAddSelected.Size = new System.Drawing.Size(88, 26);
            this.btnAddSelected.TabIndex = 6;
            this.btnAddSelected.Text = ">>";
            this.btnAddSelected.UseVisualStyleBackColor = true;
            this.btnAddSelected.Click += new System.EventHandler(this.btnAddSelected_Click);
            //
            // btnRemoveSelected
            //
            this.btnRemoveSelected.Location = new System.Drawing.Point(553, 230);
            this.btnRemoveSelected.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(88, 26);
            this.btnRemoveSelected.TabIndex = 7;
            this.btnRemoveSelected.Text = "<<";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            //
            // txtTargetModem
            //
            this.txtTargetModem.Location = new System.Drawing.Point(757, 14);
            this.txtTargetModem.Margin = new System.Windows.Forms.Padding(4);
            this.txtTargetModem.MaxLength = 7;
            this.txtTargetModem.Name = "txtTargetModem";
            this.txtTargetModem.Size = new System.Drawing.Size(116, 23);
            this.txtTargetModem.TabIndex = 8;
            this.txtTargetModem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtTargetModem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtTargetModem.TextChanged += new System.EventHandler(this.txtTargetModem_TextChanged);
            this.txtTargetModem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTargetModem_KeyPress);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(658, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Add to Modem:";
            //
            // chkRenameMain
            //
            this.chkRenameMain.AutoSize = true;
            this.chkRenameMain.Location = new System.Drawing.Point(35, 620);
            this.chkRenameMain.Name = "chkRenameMain";
            this.chkRenameMain.Size = new System.Drawing.Size(115, 19);
            this.chkRenameMain.TabIndex = 11;
            this.chkRenameMain.Text = "Rename Main to:";
            this.chkRenameMain.UseVisualStyleBackColor = true;
            this.chkRenameMain.CheckedChanged += new System.EventHandler(this.chkRenameMain_CheckedChanged);
            //
            // txtRenameMainTo
            //
            this.txtRenameMainTo.Enabled = false;
            this.txtRenameMainTo.Location = new System.Drawing.Point(156, 618);
            this.txtRenameMainTo.MaxLength = 16;
            this.txtRenameMainTo.Name = "txtRenameMainTo";
            this.txtRenameMainTo.Size = new System.Drawing.Size(135, 23);
            this.txtRenameMainTo.TabIndex = 12;

            // ── Kabal Sync Tab Controls ──────────────────────────────────────
            //
            // lblKabalCustomer
            //
            this.lblKabalCustomer.AutoSize = true;
            this.lblKabalCustomer.Location = new System.Drawing.Point(20, 20);
            this.lblKabalCustomer.Name = "lblKabalCustomer";
            this.lblKabalCustomer.Text = "Customer";
            //
            // cmbKabalCustomer
            //
            this.cmbKabalCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKabalCustomer.Items.AddRange(new object[] { "select customer", "AkerBP", "ConocoPhillips", "Equinor", "V\u00e5r Energi" });
            this.cmbKabalCustomer.Location = new System.Drawing.Point(20, 40);
            this.cmbKabalCustomer.Name = "cmbKabalCustomer";
            this.cmbKabalCustomer.Size = new System.Drawing.Size(260, 23);
            this.cmbKabalCustomer.TabIndex = 0;
            this.cmbKabalCustomer.SelectedIndex = 0;
            this.cmbKabalCustomer.SelectedIndexChanged += new System.EventHandler(this.cmbKabalCustomer_SelectedIndexChanged);
            //
            // lblKabalRig
            //
            this.lblKabalRig.AutoSize = true;
            this.lblKabalRig.Location = new System.Drawing.Point(20, 75);
            this.lblKabalRig.Name = "lblKabalRig";
            this.lblKabalRig.Text = "Rig";
            //
            // cmbKabalRig
            //
            this.cmbKabalRig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKabalRig.Items.Add("select rig");
            this.cmbKabalRig.Location = new System.Drawing.Point(20, 95);
            this.cmbKabalRig.Name = "cmbKabalRig";
            this.cmbKabalRig.Size = new System.Drawing.Size(260, 23);
            this.cmbKabalRig.TabIndex = 1;
            this.cmbKabalRig.SelectedIndex = 0;
            //
            // lblKabalStartDate
            //
            this.lblKabalStartDate.AutoSize = true;
            this.lblKabalStartDate.Location = new System.Drawing.Point(20, 130);
            this.lblKabalStartDate.Name = "lblKabalStartDate";
            this.lblKabalStartDate.Text = "Start Date";
            //
            // dtpKabalStartDate
            //
            this.dtpKabalStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpKabalStartDate.Location = new System.Drawing.Point(20, 150);
            this.dtpKabalStartDate.Name = "dtpKabalStartDate";
            this.dtpKabalStartDate.Size = new System.Drawing.Size(260, 23);
            this.dtpKabalStartDate.TabIndex = 2;
            this.dtpKabalStartDate.Value = System.DateTime.Today;
            //
            // lblKabalShiftDays
            //
            this.lblKabalShiftDays.AutoSize = true;
            this.lblKabalShiftDays.Location = new System.Drawing.Point(20, 185);
            this.lblKabalShiftDays.Name = "lblKabalShiftDays";
            this.lblKabalShiftDays.Text = "Shift Days";
            //
            // txtKabalShiftDays
            //
            this.txtKabalShiftDays.Location = new System.Drawing.Point(20, 205);
            this.txtKabalShiftDays.Name = "txtKabalShiftDays";
            this.txtKabalShiftDays.Size = new System.Drawing.Size(260, 23);
            this.txtKabalShiftDays.TabIndex = 3;
            this.txtKabalShiftDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtKabalShiftDays_KeyPress);
            //
            // btnKabalLoadModems
            //
            this.btnKabalLoadModems.Location = new System.Drawing.Point(20, 245);
            this.btnKabalLoadModems.Name = "btnKabalLoadModems";
            this.btnKabalLoadModems.Size = new System.Drawing.Size(120, 30);
            this.btnKabalLoadModems.TabIndex = 4;
            this.btnKabalLoadModems.Text = "Load Modems";
            this.btnKabalLoadModems.UseVisualStyleBackColor = true;
            this.btnKabalLoadModems.Click += new System.EventHandler(this.btnKabalLoadModems_Click);
            //
            // btnKabalShiftDates
            //
            this.btnKabalShiftDates.BackColor = System.Drawing.Color.FromArgb(180, 100, 100);
            this.btnKabalShiftDates.ForeColor = System.Drawing.Color.White;
            this.btnKabalShiftDates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKabalShiftDates.Location = new System.Drawing.Point(148, 245);
            this.btnKabalShiftDates.Name = "btnKabalShiftDates";
            this.btnKabalShiftDates.Size = new System.Drawing.Size(108, 30);
            this.btnKabalShiftDates.TabIndex = 5;
            this.btnKabalShiftDates.Text = "Shift Dates";
            this.btnKabalShiftDates.UseVisualStyleBackColor = false;
            this.btnKabalShiftDates.Click += new System.EventHandler(this.btnKabalShiftDates_Click);
            //
            // btnKabalSyncKabal
            //
            this.btnKabalSyncKabal.BackColor = System.Drawing.Color.FromArgb(60, 120, 180);
            this.btnKabalSyncKabal.ForeColor = System.Drawing.Color.White;
            this.btnKabalSyncKabal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKabalSyncKabal.Location = new System.Drawing.Point(20, 283);
            this.btnKabalSyncKabal.Name = "btnKabalSyncKabal";
            this.btnKabalSyncKabal.Size = new System.Drawing.Size(236, 30);
            this.btnKabalSyncKabal.TabIndex = 6;
            this.btnKabalSyncKabal.Text = "Sync with Kabal";
            this.btnKabalSyncKabal.UseVisualStyleBackColor = false;
            this.btnKabalSyncKabal.Click += new System.EventHandler(this.btnKabalSyncKabal_Click);
            //
            // btnKabalCopyClipboard
            //
            this.btnKabalCopyClipboard.Location = new System.Drawing.Point(264, 283);
            this.btnKabalCopyClipboard.Name = "btnKabalCopyClipboard";
            this.btnKabalCopyClipboard.Size = new System.Drawing.Size(140, 30);
            this.btnKabalCopyClipboard.TabIndex = 20;
            this.btnKabalCopyClipboard.Text = "Copy to Clipboard";
            this.btnKabalCopyClipboard.UseVisualStyleBackColor = true;
            this.btnKabalCopyClipboard.Click += new System.EventHandler(this.btnKabalCopyClipboard_Click);
            //
            // lblKabalStatus
            //
            this.lblKabalStatus.AutoSize = false;
            this.lblKabalStatus.Location = new System.Drawing.Point(20, 322);
            this.lblKabalStatus.Name = "lblKabalStatus";
            this.lblKabalStatus.Size = new System.Drawing.Size(600, 20);
            this.lblKabalStatus.Text = "";
            //
            // dgvKabal
            //
            this.dgvKabal.AllowUserToAddRows = false;
            this.dgvKabal.AllowUserToDeleteRows = false;
            this.dgvKabal.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgvKabal.MultiSelect = true;
            this.dgvKabal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKabal.Location = new System.Drawing.Point(20, 350);
            this.dgvKabal.Name = "dgvKabal";
            this.dgvKabal.ReadOnly = false;
            this.dgvKabal.RowHeadersWidth = 25;
            this.dgvKabal.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKabal.Size = new System.Drawing.Size(1155, 250);
            this.dgvKabal.TabIndex = 7;
            this.dgvKabal.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKabal_CellContentClick);
            //
            // lblKabalUser
            //
            this.lblKabalUser.AutoSize = true;
            this.lblKabalUser.Location = new System.Drawing.Point(20, 612);
            this.lblKabalUser.Name = "lblKabalUser";
            this.lblKabalUser.Text = "Kabal Username";
            //
            // txtKabalUser
            //
            this.txtKabalUser.Location = new System.Drawing.Point(20, 630);
            this.txtKabalUser.Name = "txtKabalUser";
            this.txtKabalUser.Size = new System.Drawing.Size(200, 23);
            this.txtKabalUser.TabIndex = 8;
            //
            // lblKabalPass
            //
            this.lblKabalPass.AutoSize = true;
            this.lblKabalPass.Location = new System.Drawing.Point(230, 612);
            this.lblKabalPass.Name = "lblKabalPass";
            this.lblKabalPass.Text = "Password";
            //
            // txtKabalPass
            //
            this.txtKabalPass.Location = new System.Drawing.Point(230, 630);
            this.txtKabalPass.Name = "txtKabalPass";
            this.txtKabalPass.PasswordChar = '*';
            this.txtKabalPass.Size = new System.Drawing.Size(200, 23);
            this.txtKabalPass.TabIndex = 9;
            this.txtKabalPass.ShortcutsEnabled = false;
            //
            // chkKabalHeadless
            //
            this.chkKabalHeadless.AutoSize = true;
            this.chkKabalHeadless.Checked = true;
            this.chkKabalHeadless.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKabalHeadless.Location = new System.Drawing.Point(444, 632);
            this.chkKabalHeadless.Name = "chkKabalHeadless";
            this.chkKabalHeadless.Text = "Run headless";
            this.chkKabalHeadless.TabIndex = 10;
            this.chkKabalHeadless.UseVisualStyleBackColor = true;
            //
            // MergeForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 704);
            this.Controls.Add(this.tabMain);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MergeForm";
            this.Text = "ModemCopier 2.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MergeForm_FormClosing);
            this.tabMain.ResumeLayout(false);
            this.tabMerger.ResumeLayout(false);
            this.tabMerger.PerformLayout();
            this.tabKabal.ResumeLayout(false);
            this.tabKabal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKabal)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabMerger;
        private System.Windows.Forms.TabPage tabKabal;
        private System.Windows.Forms.TextBox txtModemNo;
        private System.Windows.Forms.Button btnGetModem;
        private System.Windows.Forms.Button btnFileManager;
        private System.Windows.Forms.Button btnResetForm;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddSelected;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.TextBox txtTargetModem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRenameMain;
        private System.Windows.Forms.TextBox txtRenameMainTo;
        // Kabal tab fields
        private System.Windows.Forms.Label lblKabalCustomer;
        private System.Windows.Forms.ComboBox cmbKabalCustomer;
        private System.Windows.Forms.Label lblKabalRig;
        private System.Windows.Forms.ComboBox cmbKabalRig;
        private System.Windows.Forms.Label lblKabalStartDate;
        private System.Windows.Forms.DateTimePicker dtpKabalStartDate;
        private System.Windows.Forms.Label lblKabalShiftDays;
        private System.Windows.Forms.TextBox txtKabalShiftDays;
        private System.Windows.Forms.Button btnKabalLoadModems;
        private System.Windows.Forms.Button btnKabalShiftDates;
        private System.Windows.Forms.Button btnKabalSyncKabal;
        private System.Windows.Forms.DataGridView dgvKabal;
        private System.Windows.Forms.Label lblKabalStatus;
        private System.Windows.Forms.Label lblKabalUser;
        private System.Windows.Forms.TextBox txtKabalUser;
        private System.Windows.Forms.Label lblKabalPass;
        private System.Windows.Forms.TextBox txtKabalPass;
        private System.Windows.Forms.CheckBox chkKabalHeadless;
        private System.Windows.Forms.Button btnKabalCopyClipboard;
    }
}
