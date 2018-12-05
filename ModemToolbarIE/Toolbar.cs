using BandObjectLib;
using Microsoft.Win32;
using ModemToolbarIE.Db;
using mshtml;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace ModemToolbarIE
{
    using Link = KeyValuePair<string, string>;
    using MenuLink = KeyValuePair<string, KeyValuePair<string, ModemPostObjects>[]>;
    using PostLink = KeyValuePair<string, ModemPostObjects>;

    [Guid("0823E052-F731-40A2-BE47-42527C602B0D")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)] //this is added from ieengtbr

    [BandObject("Modem Toolbar", BandObjectStyle.Horizontal
         | BandObjectStyle.ExplorerToolbar, HelpText = "Modem Toolbar for helping with modem creation")]

    public partial class Toolbar : BandObject
    {

        internal const string AppKey = "Software\\IEToolbar";
        internal const string SettingsKey = AppKey + "\\Settings";
        internal const string LastRunValue = "LastRun";
        internal const string InstalledValue = "Installed";
        const string RegHistoryValue = "History";
        internal string toolbarFolder;

        internal string dataFolder;
        internal string imagesFolder;
        internal string settingsFolder;
        internal string rssFolder;
        internal string cacheFolder;


        internal const string cmdPrefix = "meeks://";
        internal const string cmdClearHistory = "ClearHistory";

        private bool bhaEditMode = false;
        public ModemEvents ModemStat { get; set; }

        MenuListItem menu;
        SearchBoxItem searchBox;
        LinkListItem buttonItems;

        List<BaseToolbarItem> items = new List<BaseToolbarItem>();

        #region "Static Code"

        internal static DateTime InstallationDate
        {
            get
            {
                try
                {
                    using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(SettingsKey, false))
                    {
                        string val = rk.GetValue(InstalledValue).ToString();
                        DateTime result = new DateTime(Convert.ToInt64(val));
                        return result;
                    }
                }
                catch (Exception)
                {
                    return DateTime.MaxValue;
                }
            }
            set
            {
                using (RegistryKey rk = Registry.LocalMachine.CreateSubKey(SettingsKey))
                {
                    rk.SetValue(InstalledValue, value.Ticks.ToString());
                }
            }
        }

        /// <summary>
        /// Forms URL for internal command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static string WrapInternalCommand(string command)
        {
            return cmdPrefix + command;
        }

        /// <summary>
        /// Gets internal command from URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static string UnwrapInternalCommand(string url)
        {
            if (!url.StartsWith(cmdPrefix, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            return url.Substring(cmdPrefix.Length);
        }

        /// <summary>
        /// Loads image from HTTP protocol.
        /// </summary>

        /// <summary>
        /// Copys the foder with subfolders.
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="dstFolder"></param>
        internal static void CopyFolder(string srcFolder, string dstFolder, bool overwrite)
        {
            try
            {
                Directory.CreateDirectory(dstFolder);
            }
            catch (Exception) { }

            DirectoryInfo di = new DirectoryInfo(srcFolder);
            foreach (FileInfo fi in di.GetFiles())
            {
                fi.CopyTo(Path.Combine(dstFolder, fi.Name), overwrite);
            }

            foreach (DirectoryInfo sdi in di.GetDirectories())
            {
                if (sdi.Name == "." || sdi.Name == "..")
                {
                    continue;
                }

                CopyFolder(sdi.FullName, Path.Combine(dstFolder, sdi.Name), overwrite);
            }

        }

        /// <summary>
        /// Retrieves DateTime from XML encoded string.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        internal static DateTime StringToDateTime(string val)
        {
            return XmlConvert.ToDateTime(val, XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// Encode DateTime for storing in XML
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        internal static string DateTimeToString(DateTime val)
        {
            return XmlConvert.ToString(val, XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// Saves the XML document in local file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        internal static void SaveXml(string url, string fileName)
        {
            using (XmlReader rdr = XmlReader.Create(url))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(rdr);
                doc.Save(fileName);
            }
        }

        /// <summary>
        /// Folder with application data and files.
        /// </summary>
        public static string DataFolder
        {
            get
            {
                string result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(result, "ModemToolbar");
            }
        }
        #endregion

        private bool BhaEditMode
        {
            get => bhaEditMode;

            set
            {
                bhaEditMode = value;
                EnableToolstrip(value);
            }
        }

        public Toolbar()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string fullName = asm.GetModules()[0].FullyQualifiedName;
            toolbarFolder = Path.GetDirectoryName(fullName);
            dataFolder = DataFolder;
            try
            {
                cacheFolder = Path.Combine(dataFolder, "Cache");
                Directory.CreateDirectory(cacheFolder);
            }
            catch (Exception) { }




            //try
            //{
            //    settingsFolder = Path.Combine(dataFolder, "Settings");
            //    //if (!Directory.Exists (settingsFolder))
            //    {
            //        //MessageBox.Show (Path.Combine (toolbarFolder, "Settings"), settingsFolder);
            //        CopyFolder(Path.Combine(toolbarFolder, "Settings"), settingsFolder, false);

            //    }

            //}
            //catch (Exception) { }

            //try
            //{
            //    imagesFolder = Path.Combine(cacheFolder, "Images");
            //    //if (!Directory.Exists (imagesFolder))
            //    {
            //        CopyFolder(Path.Combine(toolbarFolder, "Images"), imagesFolder, false);
            //    }
            //}
            //catch (Exception) { }

            InitializeComponent();
            HtmlDocCompleted += Toolbar_HtmlDocComplete;
        }




        private void Toolbar_HtmlDocComplete(object sender, ModemEventArgs e)
        {
            ModemEvents modemState = e.ModemEvent;
            string modemNo = e.ModemNo;

            ModemStat = modemState;

            switch (modemState)
            {
                case ModemEvents.None:
                    txtStatus.Text = "Not Modem";
                    BhaEditMode = false;
                    break;
                case ModemEvents.Gant:
                    txtStatus.Text = "Modem Gant";
                    BhaEditMode = false;
                    break;
                case ModemEvents.View:
                    txtStatus.Text = modemNo + " - View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.BhaView:
                    txtStatus.Text = modemNo + " - Mwd View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.DdView:
                    txtStatus.Text = modemNo + " - Dd View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.GpView:
                    txtStatus.Text = modemNo + " - Gp View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.Edit:
                    txtStatus.Text = modemNo + " - Edit";
                    BhaEditMode = false;
                    break;
                case ModemEvents.BhaEdit:
                    txtStatus.Text = modemNo + " - Mwd Edit";
                    BhaEditMode = CheckIfMwdEditEmpty(modemNo);
                    break;
                case ModemEvents.DdEdit:
                    txtStatus.Text = modemNo + " - Dd Edit";
                    BhaEditMode = false;
                    break;
                case ModemEvents.GpEdit:
                    txtStatus.Text = modemNo + " - Gp Edit";
                    BhaEditMode = false;
                    break;
                default:
                    BhaEditMode = false;
                    break;
            }
        }

        private void EnableToolstrip(bool enable)
        {
            //menuStrip.Enabled = enable;
        }

        private bool CheckIfMwdEditEmpty(string _modemNo)
        {
            

            try
            {
                Utility.ModemParameters  modemParameters = new Utility.ModemParameters(htmlDocument, _modemNo);
                if (modemParameters.MwdBhaCount == 0)
                {
                    return true;
                }

                return false;

            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }


            //MessageBox.Show(modemParameters.MwdBhaCount.ToString());
            return true;
            
        }

        public System.Windows.Forms.ToolStrip ToolStrip => toolStrip;

        public System.Windows.Forms.ToolStrip MenuStrip => menuStrip;


        private void Toolbar_Load(object sender, EventArgs e)
        {
            CreateToolbarItems();
        }

        internal void CreateToolbarItems()
        {
            try
            {
                // Prevent redrowing of this toolbar control.
                SuspendLayout();

                // Clear previos state.
                Clear();

                Assembly currentAssembly = Assembly.GetAssembly(GetType());

                //
                // Lwd Tools Menu
                //

                List<PostLink> RllLnk9 = new List<PostLink>();
                List<PostLink> RllLnk8 = new List<PostLink>();
                List<PostLink> RllLnk6 = new List<PostLink>();
                List<PostLink> RllLnk4 = new List<PostLink>();

                List<PostLink> PulPmLnk9 = new List<PostLink>();
                List<PostLink> PulPmLnk8 = new List<PostLink>();
                List<PostLink> PulPmLnk6 = new List<PostLink>();
                List<PostLink> PulPmLnk4 = new List<PostLink>();

                List<PostLink> LwdLnk9 = new List<PostLink>();
                List<PostLink> LwdLnk8 = new List<PostLink>();
                List<PostLink> LwdLnk6 = new List<PostLink>();
                List<PostLink> LwdLnk4 = new List<PostLink>();

                List<PostLink> OtherLnk9 = new List<PostLink>();
                List<PostLink> OtherLnk8 = new List<PostLink>();
                List<PostLink> OtherLnk6 = new List<PostLink>();
                List<PostLink> OtherLnk4 = new List<PostLink>();

                PostLink lnk;

                using (var db = new ToolDb())
                {
                    ModemToolbarIE.Db.ToolDbTableAdapters.ToolsTableAdapter toolAdapter = new Db.ToolDbTableAdapters.ToolsTableAdapter();
                    ToolDb.ToolsDataTable dsTools = new ToolDb.ToolsDataTable();
                    toolAdapter.Fill(dsTools);



                    foreach (ToolDb.ToolsRow item in dsTools.Rows)
                    {
                        Db.ToolDbTableAdapters.SubToolsTableAdapter compAdapter = new Db.ToolDbTableAdapters.SubToolsTableAdapter();
                        Db.ToolDbTableAdapters.SoftwareTableAdapter softAdapter = new Db.ToolDbTableAdapters.SoftwareTableAdapter();

                        ToolDb.SubToolsDataTable subTools = new ToolDb.SubToolsDataTable();
                        ToolDb.SoftwareDataTable softTools = new ToolDb.SoftwareDataTable();

                        compAdapter.Fill(subTools);
                        softAdapter.Fill(softTools);

                        var varTools = from tool in subTools
                                       where tool.ToolID == item.ID 
                                       select tool;

                        var varSofts = from soft in softTools
                                       where soft.ToolID == item.ID
                                       select soft;

                        ModemPostObjects mpo = new ModemPostObjects(ModemNoEngine, htmlDocument);
                        mpo.MwdBhaPost = GetBhaObj(item.ToolName, "Please check components accordingly", true);
                        mpo.MwdCompPostDict = GetRowsToModemComp(varTools);
                        mpo.MwdSoftPostDict = GetRowsToModemSoft(varSofts);

                        lnk = new PostLink(item.ToolName, mpo);

                        if (item.ToolSize == 9)
                        {
                            switch (item.Category)
                            {
                                case "RLL":
                                    RllLnk9.Add(lnk);
                                    break;
                                case "Pulser-PM":
                                    PulPmLnk9.Add(lnk);
                                    break;
                                case "LWD":
                                    LwdLnk9.Add(lnk);
                                    break;
                                case "Other":
                                    OtherLnk9.Add(lnk);
                                    break;
                                default:
                                    break;
                            }

                        }
                        else if (item.ToolSize == 8)
                        {
                            switch (item.Category)
                            {
                                case "RLL":
                                    RllLnk8.Add(lnk);
                                    break;
                                case "Pulser-PM":
                                    PulPmLnk8.Add(lnk);
                                    break;
                                case "LWD":
                                    LwdLnk8.Add(lnk);
                                    break;
                                case "Other":
                                    OtherLnk8.Add(lnk);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (item.ToolSize == 6)
                        {
                            switch (item.Category)
                            {
                                case "RLL":
                                    RllLnk6.Add(lnk);
                                    break;
                                case "Pulser-PM":
                                    PulPmLnk6.Add(lnk);
                                    break;
                                case "LWD":
                                    LwdLnk6.Add(lnk);
                                    break;
                                case "Other":
                                    OtherLnk6.Add(lnk);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (item.ToolSize == 4)
                        {
                            switch (item.Category)
                            {
                                case "RLL":
                                    RllLnk4.Add(lnk);
                                    break;
                                case "Pulser-PM":
                                    PulPmLnk4.Add(lnk);
                                    break;
                                case "LWD":
                                    LwdLnk4.Add(lnk);
                                    break;
                                case "Other":
                                    OtherLnk4.Add(lnk);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }


                }

                MenuLink mRll950 = new MenuLink("950", RllLnk9.ToArray());
                MenuLink mRll800 = new MenuLink("800", RllLnk8.ToArray());
                MenuLink mRll675 = new MenuLink("675", RllLnk6.ToArray());
                MenuLink mRll475 = new MenuLink("475", RllLnk4.ToArray());

                MenuLink mPulPm950 = new MenuLink("950", PulPmLnk9.ToArray());
                MenuLink mPulPm800 = new MenuLink("800", PulPmLnk8.ToArray());
                MenuLink mPulPm675 = new MenuLink("675", PulPmLnk6.ToArray());
                MenuLink mPulPm475 = new MenuLink("475", RllLnk4.ToArray());

                MenuLink mLwd950 = new MenuLink("950", LwdLnk9.ToArray());
                MenuLink mLwd800 = new MenuLink("800", LwdLnk8.ToArray());
                MenuLink mLwd675 = new MenuLink("675", LwdLnk6.ToArray());
                MenuLink mLwd475 = new MenuLink("475", LwdLnk4.ToArray());

                MenuLink mOther950 = new MenuLink("950", OtherLnk9.ToArray());
                MenuLink mOther800 = new MenuLink("800", OtherLnk8.ToArray());
                MenuLink mOther675 = new MenuLink("675", OtherLnk6.ToArray());
                MenuLink mOther475 = new MenuLink("475", OtherLnk4.ToArray());

                Image img = Image.FromStream(currentAssembly.GetManifestResourceStream("ModemToolbarIE.Resources.view.png"));

                MenuListItem mRll = new MenuListItem(this, "RLL", "Add RLL", new MenuLink[] { mRll950, mRll800, mRll675, mRll475 }, img);
                MenuListItem mPulPm = new MenuListItem(this, "Pulser-PM", "Add Pulser or PM", new MenuLink[] { mPulPm950, mPulPm800, mPulPm675, mPulPm475 }, img);
                MenuListItem mLwd = new MenuListItem(this, "LWD", "Add LWD", new MenuLink[] { mLwd950, mLwd800, mLwd675, mLwd475 }, img);
                MenuListItem mOther = new MenuListItem(this, "Other", "Add Other Tools", new MenuLink[] { mOther950, mOther800, mOther675, mOther475 }, img);

                items.Add(mRll);
                items.Add(mPulPm);
                items.Add(mLwd);
                items.Add(mOther);

                //
                // Loose Items Menu
                //



                //
                // Search box
                //

                img = Image.FromStream(currentAssembly.GetManifestResourceStream("ModemToolbarIE.Resources.view.png"));

                searchBox = new SearchBoxItem(this, "<clear>", "terms to serach!",
                    "http://www.google.ru/search?q={0}",
                    "Search here", "Search", "Click to search", new Size(160, 16),
                    FlatStyle.System, img);

                items.Add(searchBox);

                Link link1 = new Link("Quality", "http://www.yahoo.com/");
                Link link2 = new Link("Imagination", "http://www.youtube.com");
                Link link3 = new Link("Expertise", "http://www.uis.no/");
                Link link4 = new Link("Reliability", "http://www.halliburton.com/");

                //
                //List of links
                //

                img = Image.FromStream(currentAssembly.GetManifestResourceStream("ModemToolbarIE.Resources.magic-wand.png"));

                buttonItems = new LinkListItem(this, "Advantages", "Advantages",
                    new Link[] { link1, link2, link3, link4 }, img);

                items.Add(buttonItems);

                //
                // RSS links
                //



            }
            finally
            {
                ResumeLayout();
            }
        }

        private Dictionary<int, MwdCompPosts> GetRowsToModemComp(IEnumerable<ToolDb.SubToolsRow> comps)
        {
            Dictionary<int, MwdCompPosts> mcpDic = new Dictionary<int, MwdCompPosts>();
            int i = 0;
            foreach (ToolDb.SubToolsRow row in comps)
            {
                MwdCompPosts mcp = new MwdCompPosts();
                mcp.P_SEQ_NO = IntDbNull(row, "Sequence");
                mcp.P_L_TORQUE = IntDbNull(row, "Torque");
                mcp.P_L_THREAD_TOP = StringDbNull(row, "ThreadTop");
                mcp.P_L_THREAD_BTM = StringDbNull(row, "ThreadBottom");
                mcp.P_DESCRIPTION = StringDbNull(row, "Description");
                mcp.P_COMMENTS = StringDbNull(row, "Comments");
                mcpDic.Add(i, mcp);
                i++;
            } 

            return mcpDic;
        }

        private Dictionary<int, MwdSoftPosts> GetRowsToModemSoft(IEnumerable <ToolDb.SoftwareRow> comps)
        {
            Dictionary<int, MwdSoftPosts> mcpDic = new Dictionary<int, MwdSoftPosts>();
            int i = 0;
            foreach (ToolDb.SoftwareRow row in comps)
            {
                MwdSoftPosts mcp = new MwdSoftPosts();
                mcp.P_L_MSR_SENSOR = StringDbNull(row, "SoftwareSensor");
                mcp.P_OPS_VERSION = StringDbNull(row, "SoftwareVersion");
                mcp.P_WS_VERSION = "";

                mcpDic.Add(i, mcp);
                i++;
            }

            return mcpDic;
        }

        private MwdBhaPosts GetBhaObj(string bhaDesc, string bhaComments, bool bhaHC)
        {
            MwdBhaPosts mbp = new MwdBhaPosts();
            mbp.P_BHA_DESC = bhaDesc;
            mbp.P_BHA_NUM = "";
            mbp.P_EXP_MAX_TEMP = "";
            mbp.P_HC_TOOL = bhaHC ? "Yes" : "";
            mbp.P_L_IMPELLERSIZE = " ";
            mbp.P_L_ORIFFICESIZE = "";
            mbp.P_L_STATORSIZE = " ";
            mbp.P_MWDDWD_ADD_INFO = bhaComments;
            mbp.P_POPPET_STANDOFF = "";
            mbp.P_PULSER_OILTYPE = "";
            mbp.P_RASOURCE_ID = "";

            return mbp;

        }


        private string StringDbNull(ToolDb.SubToolsRow row, string fieldName)
        {
            if (!DBNull.Value.Equals(row[fieldName]))
            {
                return (string)row[fieldName];
            }
            else
            {
                return ""; // returns the default value for the type
            }

        }

        private string StringDbNull(ToolDb.SoftwareRow row, string fieldName)
        {
            if (!DBNull.Value.Equals(row[fieldName]))
            {
                return (string)row[fieldName];
            }
            else
            {
                return ""; // returns the default value for the type
            }

        }

        private string IntDbNull(ToolDb.SubToolsRow row, string fieldName)
        {
            if (!DBNull.Value.Equals(row[fieldName]))
            {
                return ((int)row[fieldName]).ToString(); 
            }
            else
            {
                return ""; // returns the default value for the type
            }

        }

        internal void SmartNavigate(string url)
        {
            string cmd = UnwrapInternalCommand(url);
            if (null != cmd)
            {
                if (string.Compare(cmd, cmdClearHistory, true) == 0)
                {
                    searchBox.ClearHistory();
                    return;
                }
            }

            Navigate2(string.Format(url, searchBox.SearchText));
        }

        internal void Clear()
        {
            foreach (BaseToolbarItem item in items)
            {
                item.Reset();
            }
            items.Clear();
        }

        internal void SaveSearchHistory(SearchBoxItem item)
        {
            byte[] data = item.EncryptedHistory;
            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(SettingsKey))
            {
                rk.SetValue(RegHistoryValue, data, RegistryValueKind.Binary);
            }
        }


        internal void LoadSearchHistory(SearchBoxItem item)
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(SettingsKey))
            {
                try
                {
                    byte[] data = rk.GetValue(RegHistoryValue) as byte[];
                    item.EncryptedHistory = data;
                }
                catch (Exception)
                {
                }
            }

        }

        internal void SaveString(string name, string value)
        {
            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(SettingsKey))
            {
                rk.SetValue(name, value, RegistryValueKind.String);
            }
        }

        internal string LoadString(string name)
        {
            string result = null;
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(SettingsKey))
            {
                try
                {
                    result = rk.GetValue(name) as string;
                }
                catch (Exception)
                {
                }
            }
            return result;

        }

        private void txtStatus_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }


}
