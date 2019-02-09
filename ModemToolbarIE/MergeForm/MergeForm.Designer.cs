namespace ModemToolbarIE.MergeForm
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddSelected = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.txtTargetModem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtModemNo
            // 
            this.txtModemNo.Location = new System.Drawing.Point(30, 13);
            this.txtModemNo.MaxLength = 7;
            this.txtModemNo.Name = "txtModemNo";
            this.txtModemNo.Size = new System.Drawing.Size(138, 20);
            this.txtModemNo.TabIndex = 0;
            // 
            // btnGetModem
            // 
            this.btnGetModem.Location = new System.Drawing.Point(174, 13);
            this.btnGetModem.Name = "btnGetModem";
            this.btnGetModem.Size = new System.Drawing.Size(75, 23);
            this.btnGetModem.TabIndex = 1;
            this.btnGetModem.Text = "Get Modem";
            this.btnGetModem.UseVisualStyleBackColor = true;
            this.btnGetModem.Click += new System.EventHandler(this.btnGetModem_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(30, 54);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(437, 472);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.Enter += new System.EventHandler(this.treeView1_Enter);
            this.treeView1.Leave += new System.EventHandler(this.treeView1_Leave);
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(553, 54);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(454, 472);
            this.treeView2.TabIndex = 3;
            this.treeView2.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView2_NodeMouseDoubleClick);
            this.treeView2.Enter += new System.EventHandler(this.treeView2_Enter);
            this.treeView2.Leave += new System.EventHandler(this.treeView2_Leave);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(553, 533);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(99, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(658, 533);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddSelected
            // 
            this.btnAddSelected.Location = new System.Drawing.Point(473, 169);
            this.btnAddSelected.Name = "btnAddSelected";
            this.btnAddSelected.Size = new System.Drawing.Size(75, 23);
            this.btnAddSelected.TabIndex = 6;
            this.btnAddSelected.Text = ">>";
            this.btnAddSelected.UseVisualStyleBackColor = true;
            this.btnAddSelected.Click += new System.EventHandler(this.btnAddSelected_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(474, 199);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveSelected.TabIndex = 7;
            this.btnRemoveSelected.Text = "<<";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // txtTargetModem
            // 
            this.txtTargetModem.Location = new System.Drawing.Point(649, 12);
            this.txtTargetModem.Name = "txtTargetModem";
            this.txtTargetModem.ReadOnly = true;
            this.txtTargetModem.Size = new System.Drawing.Size(100, 20);
            this.txtTargetModem.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(564, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Add to Modem:";
            // 
            // MergeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1031, 584);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTargetModem);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnAddSelected);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnGetModem);
            this.Controls.Add(this.txtModemNo);
            this.Name = "MergeForm";
            this.Text = "Merge from other modems";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtModemNo;
        private System.Windows.Forms.Button btnGetModem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddSelected;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.TextBox txtTargetModem;
        private System.Windows.Forms.Label label1;
    }
}