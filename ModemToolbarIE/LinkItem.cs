using ModemToolbarIE.MergeForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemToolbarIE
{
    internal class LinkItem : BaseToolbarItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal LinkItem(Toolbar engine, string caption, string hint, string url, System.Drawing.Image img)
            : base(engine, img)
        {
            Create(caption, hint, url);
        }

        public override ToolbarItemType TypeID
        {
            get { return ToolbarItemType.Link; }
        }

        public void Create(string buttonText,
           string buttonTooltip,
           string targetURL)
        {
            this.targetURL = targetURL;
            this.linkButton = new System.Windows.Forms.ToolStripButton();

            this.linkButton.Image = this.Image;
            this.linkButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.linkButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.linkButton.Text = buttonText;
            this.linkButton.ToolTipText = buttonTooltip;
            this.linkButton.Click += new EventHandler(linkButton_Click);
            int marginPad = 15;
            this.linkButton.Margin = new System.Windows.Forms.Padding(0, 0, marginPad, 0);

            items.Add(this.linkButton);
            
            engine.ToolStrip.Items.AddRange(this.items.ToArray());
            Size sz = new Size(this.engine.TsContainer.Size.Width + this.linkButton.Size.Width+  marginPad, this.engine.TsContainer.Height);
            engine.TsContainer.Size = sz;
            engine.TsContainer.Refresh();
        }

        void linkButton_Click(object sender, EventArgs e)
        {
            engine.SmartNavigate(this.targetURL);
        }

        //public override bool RestoreGuts (System.Xml.XmlElement element)
        //{
        //    Reset ();
        //    try
        //    {
        //        base.RestoreGuts (element);
        //        ReadImage (element);                
        //        string caption = ReadString (element, "caption");
        //        string hint = ReadString (element, "hint");
        //        string url = ReadTag (element, "url");
        //        this.Create (caption, hint, url);

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return false;
        //}



        private System.Windows.Forms.ToolStripButton linkButton;
        private string targetURL;
    }

    internal class MergeFormLink : BaseToolbarItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal MergeFormLink(Toolbar engine, string caption, string hint, System.Drawing.Image img)
            : base(engine, img)
        {
            Create(caption, hint);
        }

        public override ToolbarItemType TypeID
        {
            get { return ToolbarItemType.Link; }
        }

        public void Create(string buttonText,
           string buttonTooltip)
        {
            
            this.linkButton = new System.Windows.Forms.ToolStripButton();

            this.linkButton.Image = this.Image;
            this.linkButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.linkButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.linkButton.Text = buttonText;
            this.linkButton.ToolTipText = buttonTooltip;
            this.linkButton.Enabled = false;
            this.linkButton.Click += new EventHandler(linkButton_Click);
            engine.HtmlDocCompleted += Engine_HtmlDocCompleted;
            int marginPad = 15;
            this.linkButton.Margin = new System.Windows.Forms.Padding(0,0,marginPad,0);
            items.Add(this.linkButton);
            
            engine.ToolStrip.Items.AddRange(this.items.ToArray());
            Size sz = new Size(this.engine.TsContainer.Size.Width + this.linkButton.Size.Width+marginPad + 30, this.engine.TsContainer.Height);
            engine.TsContainer.Size = sz;
            engine.TsContainer.Refresh();
        }

        private void Engine_HtmlDocCompleted(object sender, BandObjectLib.ModemEventArgs e)
        {
            BandObjectLib.ModemEvents me = e.ModemEvent;
            if (me == BandObjectLib.ModemEvents.Edit || me == BandObjectLib.ModemEvents.BhaEdit)
            {
                this.linkButton.Enabled = true;
            }
            else
            {
                this.linkButton.Enabled = false;
            }
        }

        void linkButton_Click(object sender, EventArgs e)
        {
            MergeForm.MergeForm mf = new MergeForm.MergeForm(engine);
            mf.Show();
        }

        //public override bool RestoreGuts (System.Xml.XmlElement element)
        //{
        //    Reset ();
        //    try
        //    {
        //        base.RestoreGuts (element);
        //        ReadImage (element);                
        //        string caption = ReadString (element, "caption");
        //        string hint = ReadString (element, "hint");
        //        string url = ReadTag (element, "url");
        //        this.Create (caption, hint, url);

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return false;
        //}



        private System.Windows.Forms.ToolStripButton linkButton;
       // private ModemPostObjects targetObj;
    }
}
