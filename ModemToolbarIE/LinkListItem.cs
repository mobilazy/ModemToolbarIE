using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemToolbarIE
{
    internal class LinkListItem : BaseToolbarItem
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal LinkListItem(Toolbar engine,
            string caption,
            string hint,
            KeyValuePair<string, string>[] links, System.Drawing.Image img)
            : base(engine, img)
        {
            this.Create(caption, hint, links);
        }

        public override ToolbarItemType TypeID
        {
            get { return ToolbarItemType.LinkList; }
        }

        public void Create(string buttonText,
            string buttonTooltip,
            KeyValuePair<string, string>[] links)
        {

            this.linkListButton = new System.Windows.Forms.ToolStripSplitButton();

            this.linkListButton.Image = this.Image;
            this.linkListButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkListButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.linkListButton.Text = buttonText;
            this.linkListButton.ToolTipText = buttonTooltip;
            int marginPad = 10;
            this.linkListButton.Margin = new System.Windows.Forms.Padding(0, 0, marginPad, 0);

            this.linkListButton.ButtonClick += new EventHandler(linkListButton_Click);

            foreach (KeyValuePair<string, string> link in links)
            {
                if (string.IsNullOrEmpty(link.Key))
                {
                    this.linkListButton.DropDownItems.Add(new System.Windows.Forms.ToolStripSeparator());
                }
                else
                {
                    System.Windows.Forms.ToolStripItem menuItem = this.linkListButton.DropDownItems.Add(link.Key);
                    menuItem.Tag = link.Value;
                    menuItem.Click += new EventHandler(menuItem_Click);
                }
            }


            items.Add(this.linkListButton);
            
            engine.ToolStrip.Items.AddRange(this.items.ToArray());
            Size sz = new Size(this.engine.TsContainer.Size.Width + this.linkListButton.Size.Width+ marginPad, this.engine.TsContainer.Height);
            engine.TsContainer.Size = sz;

            engine.TsContainer.Refresh();
        }

        void linkListButton_Click(object sender, EventArgs e)
        {
            this.linkListButton.DropDown.Visible = true;
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem menuItem = sender as System.Windows.Forms.ToolStripMenuItem;
            this.engine.SmartNavigate(menuItem.Tag.ToString());
        }


        private System.Windows.Forms.ToolStripSplitButton linkListButton;
    }
}
