using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            KeyValuePair<string, KeyValuePair<string, ModemPostObjects>[]>[] links, System.Drawing.Image img)
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
            KeyValuePair<string, KeyValuePair<string, ModemPostObjects>[]>[] links)
        {

            this.menuListItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuListItem.Text = menuText;
            

            foreach (KeyValuePair<string, KeyValuePair<string, ModemPostObjects>[]> link in links)
            {
                MenuStripItem mnu = new MenuStripItem(base.engine, this, link.Key, link.Value);
                menuListItem.DropDownItems.Add(mnu.menuStripItem);
            }

            
            
            engine.MenuStrip.Items.Add(this.menuListItem);
            
        }

        
        private System.Windows.Forms.ToolStripMenuItem menuListItem;
    }
}
