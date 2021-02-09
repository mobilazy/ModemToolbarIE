using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemToolbarIE.LocalSync
{
    public class SearchListClass
    {
        public List<SearchBoxItemClass> List { get; set; } = new List<SearchBoxItemClass>();
    }

    public class LinkListClass
    {
        public List<LinkListItemClass> List { get; set; } = new List<LinkListItemClass>();
    }

    public class MenuListClass
    {
        public List<MenuListItemClass> List { get; set; } = new List<MenuListItemClass>();
    }

    public class MenuListItemClass
    {
        private string caption;
        private string hint;
        private KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>[] links;
        private byte[] img;

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        public KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>[] Links
        {
            get { return links; }
            set { links = value; }
        }

        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }

    }

    public class SearchBoxItemClass
    {
        private string clearHistoryText;
        private string greetingText;
        private string searchURL;
        private string searchBoxTooltip;
        private string buttonText;
        private string buttonTooltip;
        private Size inputBoxSize;
        private FlatStyle inputBoxFlatStyle;
        private byte[] img;


        public string ClearHistoryText
        {
            get { return clearHistoryText; }
            set { clearHistoryText = value; }
        }

        public string GreetingText
        {
            get { return greetingText; }
            set { greetingText = value; }
        }

        public string SearchURL
        {
            get { return searchURL; }
            set { searchURL = value; }
        }

        public string SearchBoxTooltip
        {
            get { return searchBoxTooltip; }
            set { searchBoxTooltip = value; }
        }

        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; }
        }

        public string ButtonTooltip
        {
            get { return buttonTooltip; }
            set { buttonTooltip = value; }
        }

        public Size InputBoxSize
        {
            get { return inputBoxSize; }
            set { inputBoxSize = value; }
        }

        public FlatStyle InputBoxFlatStyle
        {
            get { return inputBoxFlatStyle; }
            set { inputBoxFlatStyle = value; }
        }

        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }


    }

    public class LinkListItemClass
    {
        private string caption;
        private string hint;
        private KeyValuePair<string, string>[] links;
        private byte[] img;

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        public KeyValuePair<string, string>[] Links
        {
            get { return links; }
            set { links = value; }
        }

        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }
    }
}
