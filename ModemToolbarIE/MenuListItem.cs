using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModemWebUtility;

namespace ModemToolbarIE
{
    internal class MenuListItem : BaseToolbarItem
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal MenuListItem(Toolbar engine,
            string caption,
            string hint,
            KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>[] links, System.Drawing.Image img)
            : base(engine, img)
        {
            this.Create(caption, hint, links);
        }

        public override ToolbarItemType TypeID
        {
            get { return ToolbarItemType.MainMenu; }
        }

        public void Create(string menuText,
            string menuTooltip,
            KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>[] links)
        {

            this.menuListItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuListItem.Text = menuText;
            

            foreach (KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]> link in links)
            {
                MenuStripItem mnu = new MenuStripItem(base.engine, this, link.Key, link.Value);
                menuListItem.DropDownItems.Add(mnu.menuStripItem);
            }

            Size sz = new Size(engine.MsContainer.Size.Width + this.menuListItem.Size.Width + 20, this.engine.MsContainer.Height);
            engine.MsContainer.Size = sz; 
            engine.MsContainer.Left = engine.TsContainer.Size.Width+150;

            engine.MenuStrip.Items.Add(this.menuListItem);
     
            engine.MsContainer.Refresh();
            //engine.Refresh();

        }

        
        private System.Windows.Forms.ToolStripMenuItem menuListItem;
    }
}
