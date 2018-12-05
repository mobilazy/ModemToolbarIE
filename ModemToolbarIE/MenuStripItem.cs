using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemToolbarIE
{
    class MenuStripItem : BaseToolbarItem
    {

        private MenuListItem listItem;
        private MenuStripItem stripItem;
        private Toolbar Engine;

        private ModemPostObjects linkObject;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal MenuStripItem(Toolbar engine, MenuListItem _listItem,
            string caption,
            KeyValuePair<string, ModemPostObjects>[] links)
            : base(engine, null)
        {
            listItem = _listItem;
            Create(caption, links);
            Engine = engine;
        }

        internal MenuStripItem(Toolbar engine, MenuStripItem _stripItem,
            string caption,
            ModemPostObjects linkUrl)
          : base(engine, null)
        {
            stripItem = _stripItem;
            Create(caption, linkUrl);
            Engine = engine;
        }

        public override ToolbarItemType TypeID => ToolbarItemType.MainMenu;

        public void Create(string menuText,
            KeyValuePair<string, ModemPostObjects>[] links)
        {

            menuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStripItem.Text = menuText;


            foreach (KeyValuePair<string, ModemPostObjects> link in links)
            {
                MenuStripItem mnu = new MenuStripItem(base.engine, this, link.Key, link.Value);
                menuStripItem.DropDownItems.Add(mnu.menuStripItem);
            }


        }



        public void Create(string menuText,
            ModemPostObjects linkUrl)
        {

            menuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStripItem.Text = menuText;

            if ((linkUrl) != null)
            {
                linkObject = linkUrl;
                menuStripItem.Click += new EventHandler(menuListButton_Click);
            }



        }

        void menuListButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem menuItem = sender as System.Windows.Forms.ToolStripMenuItem;

          
            if (Engine.ModemStat == BandObjectLib.ModemEvents.BhaEdit)
            {
                ModemToolbarIE.Utility.ModemParameters mp = new Utility.ModemParameters(Engine.HtmlDoc, true, "P_10");
                ModemToolbarIE.Utility.ModemInsert mi = new Utility.ModemInsert(mp, linkObject, true);
            }
            else if (Engine.ModemStat == BandObjectLib.ModemEvents.Edit)
            {
                ModemToolbarIE.Utility.ModemParameters mp = new Utility.ModemParameters(Engine.HtmlDoc, true, "P_SSORD_ID");
                ModemToolbarIE.Utility.ModemInsert mi = new Utility.ModemInsert(mp, linkObject, false);
            }
            else
            {
                return;
            }

            Engine.RefreshPage();
        }




        public System.Windows.Forms.ToolStripMenuItem menuStripItem;
    }
}
