namespace ModemToolbarIE
{
    partial class PopupGo
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnOpenModem = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtModemNo
            // 
            this.txtModemNo.Location = new System.Drawing.Point(13, 13);
            this.txtModemNo.Name = "txtModemNo";
            this.txtModemNo.Size = new System.Drawing.Size(112, 20);
            this.txtModemNo.TabIndex = 0;
            this.txtModemNo.TextChanged += new System.EventHandler(this.txtModemNo_TextChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(131, 13);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(109, 16);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Enter Modem No";
            // 
            // btnOpenModem
            // 
            this.btnOpenModem.Location = new System.Drawing.Point(30, 46);
            this.btnOpenModem.Name = "btnOpenModem";
            this.btnOpenModem.Size = new System.Drawing.Size(75, 23);
            this.btnOpenModem.TabIndex = 2;
            this.btnOpenModem.Text = "Open";
            this.btnOpenModem.UseVisualStyleBackColor = true;
            this.btnOpenModem.Click += new System.EventHandler(this.btnOpenModem_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(134, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PopupGo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 81);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpenModem);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtModemNo);
            this.Name = "PopupGo";
            this.Text = "Open Modem";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtModemNo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnOpenModem;
        private System.Windows.Forms.Button btnCancel;
    }
}