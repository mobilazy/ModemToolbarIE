using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModemWebUtility;

namespace ModemToolbarIE
{
    class MenuStripItem : BaseToolbarItem
    {

        private MenuListItem listItem;
        private MenuStripItem stripItem;
        private Toolbar Engine;

        private ModemMwdPostObjects linkObject;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal MenuStripItem(Toolbar engine, MenuListItem _listItem,
            string caption,
            KeyValuePair<string, ModemMwdPostObjects>[] links)
            : base(engine, null)
        {
            listItem = _listItem;
            Create(caption, links);
            Engine = engine;
        }

        internal MenuStripItem(Toolbar engine, MenuStripItem _stripItem,
            string caption,
            ModemMwdPostObjects linkUrl)
          : base(engine, null)
        {
            stripItem = _stripItem;
            Create(caption, linkUrl);
            Engine = engine;
        }

        public override ToolbarItemType TypeID => ToolbarItemType.MainMenu;

        public void Create(string menuText,
            KeyValuePair<string, ModemMwdPostObjects>[] links)
        {

            menuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStripItem.Text = menuText;


            foreach (KeyValuePair<string, ModemMwdPostObjects> link in links)
            {
                MenuStripItem mnu = new MenuStripItem(base.engine, this, link.Key, link.Value);
                menuStripItem.DropDownItems.Add(mnu.menuStripItem);
            }


        }



        public void Create(string menuText,
            ModemMwdPostObjects linkUrl)
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

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(HDocUtility.ConvertMshtmlToString(Engine.HtmlDoc));
            string mwdId = HDocUtility.GetInputByName("P_104", doc);
          
            if (Engine.ModemStat == BandObjectLib.ModemEvents.BhaEdit)
            {
                ModemParameters mp = new ModemParameters(Engine.HtmlDoc, true, "P_10");
                ModemMwdInsert mi = new ModemMwdInsert(mp, linkObject, true);
                
            }
            else if (Engine.ModemStat == BandObjectLib.ModemEvents.Edit)
            {
               ModemParameters mp = new ModemParameters(Engine.HtmlDoc, true, "P_SSORD_ID");
               ModemMwdInsert mi = new ModemMwdInsert(mp, linkObject, false);
            }
            else
            {
                return;
            }

            if (!String.IsNullOrEmpty(mwdId))
            {
                Engine.SmartNavigate(HDocUtility.BhaEditUrlwMwdId + mwdId);
            }
            else
            {
                Engine.RefreshPage();
            }
            
        }




        public System.Windows.Forms.ToolStripMenuItem menuStripItem;
    }
}
