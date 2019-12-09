using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ModemToolbarIE
{
    internal class SearchBoxItem : BaseToolbarItem
    {
        static byte[] Key = { 0x31, 0x56, 0x26, 0x89, 0x1a, 0x86, 0xaa, 0xb5, 0x8e, 0x86, 0x30, 0x00, 0x10, 0x54, 0x23, 0x82 };
        static byte[] IV = { 0x38, 0x94, 0x24, 0x47, 0x15, 0x96, 0x81, 0x89, 0x34, 0x27, 0x92, 0x96, 0x42, 0x23, 0x43, 0x13 };
        const string modemViewUrl = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID=";
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Toolbar engine</param>
        internal SearchBoxItem(Toolbar engine,
            string clearHistoryText,
            string greetingText,
            string searchURL,
            string searchBoxTooltip,
            string buttonText,
            string buttonTooltip,
            Size inputBoxSize,
            FlatStyle inputBoxFlatStyle,
            System.Drawing.Image img)
            : base(engine, img)
        {
            Create(clearHistoryText, greetingText, searchURL, searchBoxTooltip, buttonText, buttonTooltip,
                inputBoxSize, inputBoxFlatStyle);
        }

        /// <summary>
        /// Type identifier
        /// </summary>
        public override ToolbarItemType TypeID
        {
            get { return ToolbarItemType.SearchBox; }
        }

        /// <summary>
        /// Creates controls
        /// </summary>
        internal void Create(string clearHistoryText,
                              string greetingText,
                              string searchURL,
                              string searchBoxTooltip,
                              string buttonText,
                              string buttonTooltip,
                              Size inputBoxSize,
                              FlatStyle inputBoxFlatStyle)
        {
            this.clearHistoryText = clearHistoryText;
            this.greetingText = greetingText;
            this.searchURL = searchURL;

            Reset();

            this.searchInputBox = new System.Windows.Forms.ToolStripComboBox();
            this.searchButton = new System.Windows.Forms.ToolStripButton();

            // 
            // searchInputBox
            // 

            this.searchInputBox.ToolTipText = searchBoxTooltip;
            this.searchInputBox.FlatStyle = inputBoxFlatStyle;
            this.searchInputBox.Size = inputBoxSize;
            this.searchInputBox.ToolTipText = searchBoxTooltip;
            this.searchInputBox.SelectedIndexChanged += new System.EventHandler(this.searchInputBox_SelectedIndexChanged);
            this.searchInputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchInputBox_KeyDown);
            this.searchInputBox.Click += new System.EventHandler(this.searchInputBox_Click);
            // 
            // searchButton
            // 
            this.searchButton.ToolTipText = buttonTooltip;
            this.searchButton.Image = this.Image;
            this.searchButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.searchButton.ImageScaling = ToolStripItemImageScaling.None;
            this.searchButton.Text = buttonText;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            int marginPad = 15;
            this.searchButton.Margin = new Padding(0,0,marginPad,0);

            originalColor = this.searchInputBox.ForeColor;
            this.searchInputBox.ForeColor = Color.Gray;
            searchInputBox.GotFocus += new EventHandler(searchInputBox_GotFocus);
            searchInputBox.LostFocus += new EventHandler(searchInputBox_LostFocus);

            items.Add(searchInputBox);
            items.Add(searchButton);

            engine.LoadSearchHistory(this);

            
            engine.ToolStrip.Items.AddRange(this.items.ToArray());
            this.searchInputBox.Text = greetingText;
            Size sz = new Size(this.engine.TsContainer.Size.Width + this.searchInputBox.Size.Width + this.searchButton.Size.Width+  marginPad, this.engine.TsContainer.Height);
            engine.TsContainer.Size = sz;

            engine.TsContainer.Refresh();
        }

        /// <summary>
        /// Search query.
        /// </summary>
        internal string SearchText
        {
            get
            {
                if (firstClick) return "";
                return searchInputBox.Text;
            }
            set { searchInputBox.Text = value; }
        }


        internal byte[] EncryptedHistory
        {
            get
            {
                byte[] encrypted = null;
                RijndaelManaged RMCrypto = new RijndaelManaged();
                ICryptoTransform ct = RMCrypto.CreateEncryptor(Key, IV);
                StringBuilder flatHistory = new StringBuilder();
                foreach (string val in SearchHistory)
                {
                    flatHistory.AppendLine(val);
                }
                byte[] binaryData = Encoding.UTF8.GetBytes(flatHistory.ToString());
                using (MemoryStream memStream = new MemoryStream())
                {
                    long size = 0;
                    memStream.Write(BitConverter.GetBytes(size), 0, sizeof(long));
                    using (CryptoStream cs = new CryptoStream(memStream, ct, CryptoStreamMode.Write))
                    {
                        cs.Write(binaryData, 0, binaryData.Length);
                        cs.FlushFinalBlock();

                        size = memStream.Position - sizeof(long);
                        memStream.Seek(0, SeekOrigin.Begin);
                        memStream.Write(BitConverter.GetBytes(size), 0, sizeof(long));

                        memStream.Flush();
                        encrypted = memStream.ToArray();
                        return encrypted;
                    }
                }
            }
            set
            {

                this.InternalClearHistory();
                byte[] data;
                try
                {
                    byte[] encrypted = value;
                    RijndaelManaged RMCrypto = new RijndaelManaged();
                    ICryptoTransform ct = RMCrypto.CreateDecryptor(Key, IV);
                    long size = BitConverter.ToInt64(encrypted, 0);
                    using (MemoryStream memStream = new MemoryStream(encrypted, sizeof(long), encrypted.Length - sizeof(long), false))
                    {

                        using (CryptoStream cs = new CryptoStream(memStream, ct, CryptoStreamMode.Read))
                        {
                            data = new byte[size];
                            cs.Read(data, 0, data.Length);
                        }


                    }

                    string flatHistory = Encoding.UTF8.GetString(data, 0, data.Length);
                    string[] history = flatHistory.Split(System.Environment.NewLine.ToCharArray());
                    this.SearchHistory = history;

                }
                catch (Exception)
                {

                }

            }
        }
        /// <summary>
        /// Query history.
        /// </summary>
        internal string[] SearchHistory
        {
            get
            {
                List<string> result = new List<string>();
                int count = this.searchInputBox.Items.Count;
                for (int idx = 1; idx < count; ++idx)
                {
                    result.Add(this.searchInputBox.Items[idx].ToString());
                }

                return result.ToArray();
            }
            set
            {
                this.InternalClearHistory();
                int count = value.Length;
                for (int idx = count - 1; idx >= 0; --idx)
                {
                    this.InternalAddToHistory(value[idx]);
                }
            }
        }




        #region "Event Handlers"
        void searchInputBox_LostFocus(object sender, EventArgs e)
        {
            this.engine.InternalLostFocus(e);
        }

        void searchInputBox_GotFocus(object sender, EventArgs e)
        {
            this.engine.InternalGotFocus(e);
        }

        private void searchInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return) return;
            AddToHistory(this.searchInputBox.Text);
            DoSearch(this.searchInputBox.Text);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (firstClick) return;
            string query = this.searchInputBox.Text;
            AddToHistory(query);
            DoSearch(query);
        }

        private void searchInputBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreIndexChange) return;
            if (searchInputBox.SelectedIndex == 0)
            {
                ClearHistory();

            }
            else if (searchInputBox.SelectedIndex > 0)
            {
                string query = this.searchInputBox.Items[this.searchInputBox.SelectedIndex].ToString();

                //this.toolStripComboBox1.Text = query;
                try
                {
                    ignoreIndexChange = true;
                    int idx = AddToHistory(query);
                    if (idx >= 0) searchInputBox.SelectedIndex = idx;


                }
                finally
                {
                    ignoreIndexChange = false;
                }
                DoSearch(query);
            }
        }

        private void searchInputBox_Click(object sender, EventArgs e)
        {
            if (firstClick)
            {
                try
                {
                    ignoreIndexChange = true;
                    searchInputBox.Text = "";
                    this.searchInputBox.ForeColor = originalColor;
                }
                finally
                {
                    ignoreIndexChange = false;
                }
            }

            firstClick = false;
        }
        #endregion

        /// <summary>
        /// Performs search.
        /// </summary>
        /// <param name="query">Search query</param>
        internal void DoSearch(string query)
        {
            if (firstClick) return;
            if (string.IsNullOrEmpty(query)) return;
            query = query.Trim();
            if (string.IsNullOrEmpty(query)) return;
            int result;
            string queryString;
            if (query.Length == 7 && Int32.TryParse(query, out result))
            {
                queryString = modemViewUrl + query;
            }
            else
            {
                queryString = FormatSearchRequest(query);
            }
            
            engine.Navigate2(queryString);
        }

        /// <summary>
        /// Format search string.
        /// </summary>
        /// <param name="query">Pattern of search query string.</param>
        /// <returns>Result query string.</returns>
        internal string FormatSearchRequest(string query)
        {
            string queryString = string.Format(searchURL, HttpUtility.UrlEncode(query.Trim()));
            return queryString;
        }

        /// <summary>
        /// Adds a search query in history
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Index of the added query in combobox.</returns>
        internal int AddToHistory(string query)
        {
            int result = InternalAddToHistory(query);
            engine.SaveSearchHistory(this);

            return result;
        }

        /// <summary>
        /// Adds a search query in history
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Index of the added query in combobox.</returns>
        internal int InternalAddToHistory(string query)
        {
            if (String.IsNullOrEmpty(query)) return -1;
            string preparedQuery = query.Trim();
            if (String.IsNullOrEmpty(preparedQuery)) return -1;

            int count = this.searchInputBox.Items.Count;
            for (int idx = 1; idx < count; ++idx)
            {
                if (string.Compare(preparedQuery, this.searchInputBox.Items[idx].ToString(), false) == 0)
                {
                    this.searchInputBox.Items.RemoveAt(idx);
                    break;
                }
            }
            if (count == 0)
            {
                this.searchInputBox.Items.Add(clearHistoryText);
            }
            this.searchInputBox.Items.Insert(1, preparedQuery);

            return 1;
        }


        /// <summary>
        /// Clears a search history
        /// </summary>
        internal void ClearHistory()
        {
            InternalClearHistory();
            engine.SaveSearchHistory(this);
        }

        /// <summary>
        /// Clears a search history
        /// </summary>
        internal void InternalClearHistory()
        {
            this.searchInputBox.Items.Clear();
            this.searchInputBox.Text = "";
        }


        #region "Data Members"

        private System.Windows.Forms.ToolStripComboBox searchInputBox;
        private System.Windows.Forms.ToolStripButton searchButton;

        bool ignoreIndexChange = false;
        bool firstClick = true;
        Color originalColor;

        #region "Item Options"

        string clearHistoryText;
        string greetingText;
        string searchURL;

        #endregion;


        #endregion
    }
}
