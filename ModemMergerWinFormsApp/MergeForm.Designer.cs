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
            this.SuspendLayout();
            // 
            // txtModemNo
            // 
            this.txtModemNo.Location = new System.Drawing.Point(35, 15);
            this.txtModemNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtModemNo.MaxLength = 7;
            this.txtModemNo.Name = "txtModemNo";
            this.txtModemNo.Size = new System.Drawing.Size(161, 23);
            this.txtModemNo.TabIndex = 0;
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
            // 
            // txtRenameMainTo
            // 
            this.txtRenameMainTo.Enabled = false;
            this.txtRenameMainTo.Location = new System.Drawing.Point(156, 618);
            this.txtRenameMainTo.MaxLength = 16;
            this.txtRenameMainTo.Name = "txtRenameMainTo";
            this.txtRenameMainTo.Size = new System.Drawing.Size(135, 23);
            this.txtRenameMainTo.TabIndex = 12;
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
            // MergeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 674);
            this.Controls.Add(this.txtRenameMainTo);
            this.Controls.Add(this.chkRenameMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTargetModem);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnAddSelected);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnResetForm);
            this.Controls.Add(this.btnFileManager);
            this.Controls.Add(this.btnGetModem);
            this.Controls.Add(this.txtModemNo);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MergeForm";
            this.Text = "ModemCopier 2.0";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
    }
}