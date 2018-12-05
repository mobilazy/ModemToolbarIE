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


            items.Add(this.linkButton);
            engine.ToolStrip.Items.AddRange(this.items.ToArray());
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
}
