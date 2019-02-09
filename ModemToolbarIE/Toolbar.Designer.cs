using BandObjectLib;

namespace ModemToolbarIE
{
    partial class Toolbar : BandObject
    {
        private System.ComponentModel.IContainer components = null;

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
            this.tsContainer = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.msContainer = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsContainer.ContentPanel.SuspendLayout();
            this.tsContainer.SuspendLayout();
            this.msContainer.ContentPanel.SuspendLayout();
            this.msContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Enabled = false;
            this.txtStatus.Location = new System.Drawing.Point(0, 2);
            this.txtStatus.MaxLength = 7;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(150, 20);
            this.txtStatus.TabIndex = 2;
            this.txtStatus.TextChanged += new System.EventHandler(this.txtStatus_TextChanged);
            // 
            // tsContainer
            // 
            // 
            // tsContainer.ContentPanel
            // 
            this.tsContainer.ContentPanel.Controls.Add(this.toolStrip);
            this.tsContainer.ContentPanel.Size = new System.Drawing.Size(100, 0);
            this.tsContainer.Location = new System.Drawing.Point(150, 0);
            this.tsContainer.Name = "tsContainer";
            this.tsContainer.Size = new System.Drawing.Size(100, 25);
            this.tsContainer.TabIndex = 4;
            this.tsContainer.Text = "toolStripContainer1";
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(100, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // msContainer
            // 
            // 
            // msContainer.ContentPanel
            // 
            this.msContainer.ContentPanel.BackColor = System.Drawing.Color.Transparent;
            this.msContainer.ContentPanel.Controls.Add(this.menuStrip);
            this.msContainer.ContentPanel.Size = new System.Drawing.Size(100, 0);
            this.msContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msContainer.Location = new System.Drawing.Point(250, 0);
            this.msContainer.Name = "msContainer";
            this.msContainer.Size = new System.Drawing.Size(100, 25);
            this.msContainer.TabIndex = 6;
            this.msContainer.Text = "toolStripContainer1";
            // 
            // msContainer.TopToolStripPanel
            // 
            this.msContainer.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // menuStrip
            // 
            this.menuStrip.AutoSize = false;
            this.menuStrip.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(100, 25);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // Toolbar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.msContainer);
            this.Controls.Add(this.tsContainer);
            this.Controls.Add(this.txtStatus);
            this.MinSize = new System.Drawing.Size(345, 25);
            this.Name = "Toolbar";
            this.Size = new System.Drawing.Size(353, 28);
            this.Title = "Modem Toolbar";
            this.Load += new System.EventHandler(this.Toolbar_Load);
            this.tsContainer.ContentPanel.ResumeLayout(false);
            this.tsContainer.ContentPanel.PerformLayout();
            this.tsContainer.ResumeLayout(false);
            this.tsContainer.PerformLayout();
            this.msContainer.ContentPanel.ResumeLayout(false);
            this.msContainer.ResumeLayout(false);
            this.msContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.ToolStripContainer tsContainer;
        private System.Windows.Forms.ToolStripContainer msContainer;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
    }


    class CustomProfessionalColorTable : System.Windows.Forms.ProfessionalColorTable
    {
        public override System.Drawing.Color ToolStripGradientBegin
        {
            get { return System.Drawing.SystemColors.Control; }
        }

        public override System.Drawing.Color ToolStripGradientMiddle
        {
            get { return System.Drawing.SystemColors.Control; }
        }

        public override System.Drawing.Color ToolStripGradientEnd
        {
            get { return System.Drawing.SystemColors.Control; }
        }
    }

    class CustomToolStripProfessionalRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
    {
        public CustomToolStripProfessionalRenderer()
            : base(new CustomProfessionalColorTable())
        {

        }

        protected override void OnRenderToolStripBorder(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            // Don't draw a border
        }
    }
}
