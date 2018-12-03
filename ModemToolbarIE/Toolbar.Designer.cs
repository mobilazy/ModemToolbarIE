using BandObjectLib;

namespace ModemToolbarIE
{
    partial class Toolbar : BandObject
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnGoToModem = new System.Windows.Forms.Button();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addRLLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test1Level1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test2Level2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test1Level2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test2Level2ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addPulserPMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLWDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLooseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addOthersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInject = new System.Windows.Forms.Button();
            this.btnWarn = new System.Windows.Forms.Button();
            this.btnError = new System.Windows.Forms.Button();
            this.btnGoToGant = new System.Windows.Forms.Button();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Enabled = false;
            this.txtStatus.Location = new System.Drawing.Point(4, 3);
            this.txtStatus.MaxLength = 7;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(139, 20);
            this.txtStatus.TabIndex = 2;
            // 
            // btnGoToModem
            // 
            this.btnGoToModem.BackColor = System.Drawing.Color.Silver;
            this.btnGoToModem.Location = new System.Drawing.Point(149, 1);
            this.btnGoToModem.Name = "btnGoToModem";
            this.btnGoToModem.Size = new System.Drawing.Size(43, 24);
            this.btnGoToModem.TabIndex = 3;
            this.btnGoToModem.Text = "Go";
            this.btnGoToModem.UseVisualStyleBackColor = false;
            this.btnGoToModem.Click += new System.EventHandler(this.btnGoToModem_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(411, 0);
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(195, 1);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(411, 23);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.Color.Transparent;
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRLLToolStripMenuItem,
            this.addPulserPMToolStripMenuItem,
            this.addLWDToolStripMenuItem,
            this.addLooseToolStripMenuItem,
            this.addOthersToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(411, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addRLLToolStripMenuItem
            // 
            this.addRLLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test1Level1ToolStripMenuItem,
            this.test2Level2ToolStripMenuItem});
            this.addRLLToolStripMenuItem.Enabled = false;
            this.addRLLToolStripMenuItem.Name = "addRLLToolStripMenuItem";
            this.addRLLToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.addRLLToolStripMenuItem.Text = "Add RLL";
            // 
            // test1Level1ToolStripMenuItem
            // 
            this.test1Level1ToolStripMenuItem.Name = "test1Level1ToolStripMenuItem";
            this.test1Level1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.test1Level1ToolStripMenuItem.Text = "Test1Level1";
            // 
            // test2Level2ToolStripMenuItem
            // 
            this.test2Level2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test1Level2ToolStripMenuItem,
            this.test2Level2ToolStripMenuItem1});
            this.test2Level2ToolStripMenuItem.Name = "test2Level2ToolStripMenuItem";
            this.test2Level2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.test2Level2ToolStripMenuItem.Text = "Test2Level2";
            // 
            // test1Level2ToolStripMenuItem
            // 
            this.test1Level2ToolStripMenuItem.Name = "test1Level2ToolStripMenuItem";
            this.test1Level2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.test1Level2ToolStripMenuItem.Text = "Test1Level2";
            // 
            // test2Level2ToolStripMenuItem1
            // 
            this.test2Level2ToolStripMenuItem1.Name = "test2Level2ToolStripMenuItem1";
            this.test2Level2ToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.test2Level2ToolStripMenuItem1.Text = "Test2Level2";
            // 
            // addPulserPMToolStripMenuItem
            // 
            this.addPulserPMToolStripMenuItem.Enabled = false;
            this.addPulserPMToolStripMenuItem.Name = "addPulserPMToolStripMenuItem";
            this.addPulserPMToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.addPulserPMToolStripMenuItem.Text = "Add Pulser-PM";
            // 
            // addLWDToolStripMenuItem
            // 
            this.addLWDToolStripMenuItem.Enabled = false;
            this.addLWDToolStripMenuItem.Name = "addLWDToolStripMenuItem";
            this.addLWDToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.addLWDToolStripMenuItem.Text = "Add LWD";
            // 
            // addLooseToolStripMenuItem
            // 
            this.addLooseToolStripMenuItem.Enabled = false;
            this.addLooseToolStripMenuItem.Name = "addLooseToolStripMenuItem";
            this.addLooseToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.addLooseToolStripMenuItem.Text = "Add Loose";
            // 
            // addOthersToolStripMenuItem
            // 
            this.addOthersToolStripMenuItem.Enabled = false;
            this.addOthersToolStripMenuItem.Name = "addOthersToolStripMenuItem";
            this.addOthersToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.addOthersToolStripMenuItem.Text = "Add Others";
            // 
            // btnInject
            // 
            this.btnInject.BackColor = System.Drawing.Color.Silver;
            this.btnInject.Location = new System.Drawing.Point(609, 1);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(39, 24);
            this.btnInject.TabIndex = 5;
            this.btnInject.Text = "iON";
            this.btnInject.UseVisualStyleBackColor = false;
            // 
            // btnWarn
            // 
            this.btnWarn.BackColor = System.Drawing.Color.Silver;
            this.btnWarn.Location = new System.Drawing.Point(654, 1);
            this.btnWarn.Name = "btnWarn";
            this.btnWarn.Size = new System.Drawing.Size(45, 24);
            this.btnWarn.TabIndex = 6;
            this.btnWarn.Text = "Warn";
            this.btnWarn.UseVisualStyleBackColor = false;
            // 
            // btnError
            // 
            this.btnError.BackColor = System.Drawing.Color.Silver;
            this.btnError.Location = new System.Drawing.Point(705, 0);
            this.btnError.Name = "btnError";
            this.btnError.Size = new System.Drawing.Size(41, 24);
            this.btnError.TabIndex = 7;
            this.btnError.Text = "Err";
            this.btnError.UseVisualStyleBackColor = false;
            // 
            // btnGoToGant
            // 
            this.btnGoToGant.BackColor = System.Drawing.Color.Silver;
            this.btnGoToGant.Location = new System.Drawing.Point(752, 0);
            this.btnGoToGant.Name = "btnGoToGant";
            this.btnGoToGant.Size = new System.Drawing.Size(53, 24);
            this.btnGoToGant.TabIndex = 8;
            this.btnGoToGant.Text = "Gant";
            this.btnGoToGant.UseVisualStyleBackColor = false;
            this.btnGoToGant.Click += new System.EventHandler(this.btnGoToGant_Click);
            // 
            // Toolbar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.btnGoToGant);
            this.Controls.Add(this.btnError);
            this.Controls.Add(this.btnWarn);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.btnGoToModem);
            this.Controls.Add(this.txtStatus);
            this.MinSize = new System.Drawing.Size(650, 24);
            this.Name = "Toolbar";
            this.Size = new System.Drawing.Size(827, 24);
            this.Title = "Hello Bar";
            this.Load += new System.EventHandler(this.Toolbar_Load);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnGoToModem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addRLLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPulserPMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLWDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLooseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addOthersToolStripMenuItem;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.Button btnWarn;
        private System.Windows.Forms.Button btnError;
        private System.Windows.Forms.Button btnGoToGant;
        private System.Windows.Forms.ToolStripMenuItem test1Level1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem test2Level2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem test1Level2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem test2Level2ToolStripMenuItem1;
    }
}
