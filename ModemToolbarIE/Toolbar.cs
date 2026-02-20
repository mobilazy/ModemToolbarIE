using BandObjectLib;
using Microsoft.Win32;
using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml;
//using WcfServiceModemToolbarSync;

namespace ModemToolbarIE
{
    using ModemToolbarIE.LocalSync;
    using System.Threading.Tasks;
    using System.Timers;

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
        public static List<MenuListItemClass> mlic = new List<MenuListItemClass>();
        public static List<SearchBoxItemClass> sbic = new List<SearchBoxItemClass>();
        public static List<LinkListItemClass> llic = new List<LinkListItemClass>();
        public int syncStatus = 0;

        MergeFormLink mergeButton;
        SearchBoxItem searchBox;

        List<BaseToolbarItem> baseToolbarItems = new List<BaseToolbarItem>();

        #region "Static Code"

        //internal static DateTime InstallationDate
        //{
        //    get
        //    {
        //        try
        //        {
        //            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(SettingsKey, false))
        //            {
        //                string val = rk.GetValue(InstalledValue).ToString();
        //                DateTime result = new DateTime(Convert.ToInt64(val));
        //                return result;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return DateTime.MaxValue;
        //        }
        //    }
        //    set
        //    {
        //        using (RegistryKey rk = Registry.LocalMachine.CreateSubKey(SettingsKey))
        //        {
        //            rk.SetValue(InstalledValue, value.Ticks.ToString());
        //        }
        //    }
        //}

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
            catch (Exception) { }

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
                string result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //C:\Users\<user>\AppData\Roaming\myProgram\ 
                string filePath = Path.Combine(result, "ModemToolbar");
                try
                {
                    Directory.CreateDirectory(filePath);
                }
                catch (Exception)
                {


                }

                return filePath;
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
            //Assembly asm = Assembly.GetExecutingAssembly();
            //string fullName = asm.GetModules()[0].FullyQualifiedName;
            //toolbarFolder = Path.GetDirectoryName(fullName);
            InitializeComponent();
            dataFolder = DataFolder;
            try
            {
                cacheFolder = Path.Combine(dataFolder, "Cache");
                Directory.CreateDirectory(cacheFolder);

            }
            catch (Exception ex)
            {
                
            }
            HtmlDocCompleted += Toolbar_HtmlDocComplete;
            CosturaUtility.Initialize();
            

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
                case ModemEvents.GantTools:
                    txtStatus.Text = modemNo != "" ? "Gant Tools - " + modemNo : "Gant Tools";
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
                    BhaEditMode = true;
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
            menuStrip.Enabled = enable;
        }

        private bool CheckIfMwdEditEmpty(string _modemNo)
        {
            MwdBhaParameters mbp = new MwdBhaParameters(HtmlDoc);

            if (mbp.BhaCount > 0)
            {

                return false;
            }
            else
            {

                return true;
            }

        }

        public System.Windows.Forms.ToolStrip ToolStrip => toolStrip;

        public System.Windows.Forms.ToolStrip MenuStrip => menuStrip;

        public ToolStripContainer TsContainer => tsContainer;
        public ToolStripContainer MsContainer => msContainer;


        private void Toolbar_Load(object sender, EventArgs e)
        {
            CreateToolbarItems();
            Task.Run(()=> StartLocalComms());
        }

        private async void StartLocalComms()
        {
            try
            {

                using (NoProxySync npProxy = new NoProxySync())
                {
                    if (sbic.Count<1)
                    {
                        SearchListClass sc = new SearchListClass();
                        sc = await npProxy.GetSearchBoxItemClasses();
                        sbic = new List<SearchBoxItemClass>();
                        sbic.AddRange(sc.List.ToArray());
                    }

                    if (llic.Count<1)
                    {
                        LinkListClass lc = new LinkListClass();
                        lc = await npProxy.GetLinkListItemClasses();
                        llic = new List<LinkListItemClass>();
                        llic.AddRange(lc.List.ToArray());
                    }

                    if (mlic.Count <1)
                    {
                        MenuListClass mc = new MenuListClass();
                        mc = await npProxy.GetMenuListItemClasses();
                        mlic = new List<MenuListItemClass>();
                        mlic.AddRange(mc.List.ToArray());
                    }
                    

                    CreateToolbarItems();

                }
 
            }
            catch (CommunicationException e)
            {
                txtStatus.Text = "Comm Error";
            }
            catch (TimeoutException e)
            {
                txtStatus.Text = "Timeout";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error";

            }

        }

        private void CreateToolbarItems()
        {

            try
            {
                // Prevent redrowing of this toolbar control.
                SuspendLayout();
                // Clear previos state.
                Clear();
                toolStrip.Items.Clear();
                menuStrip.Items.Clear();
                tsContainer.ContentPanel.Size = new System.Drawing.Size(100, 0);
                tsContainer.Location = new System.Drawing.Point(150, 0);
                tsContainer.Size = new System.Drawing.Size(100, 25);
                msContainer.ContentPanel.Size = new System.Drawing.Size(100, 0);
                msContainer.Location = new System.Drawing.Point(250, 0);
                msContainer.Size = new System.Drawing.Size(100, 25);
 

                if (sbic.Count != 0)
                {
                    foreach (var item in sbic)
                    {
                        Image image;
                        using (var ms = new MemoryStream(item.Img))
                        {
                            image = Image.FromStream(ms);
                        }

                        SearchBoxItem sbtemp = new SearchBoxItem(this, item.ClearHistoryText, item.GreetingText, item.SearchURL, item.SearchBoxTooltip, item.ButtonText,
                            item.ButtonTooltip, item.InputBoxSize, item.InputBoxFlatStyle, image);
                        baseToolbarItems.Add(sbtemp);
                        searchBox = sbtemp;
                    }
                }

                if (llic.Count != 0)
                {
                    foreach (var item in llic)
                    {
                        Image image;
                        using (var ms = new MemoryStream(item.Img))
                        {
                            image = Image.FromStream(ms);
                        }
                        LinkListItem lltemp = new LinkListItem(this, item.Caption, item.Hint, item.Links, image);
                        baseToolbarItems.Add(lltemp);
                    }
                }

                //merge button
                Assembly currentAssembly = Assembly.GetAssembly(GetType());
                Image img = Image.FromStream(currentAssembly.GetManifestResourceStream("ModemToolbarIE.Resources.magic-wand.png"));
                mergeButton = new MergeFormLink(this, "Merge", "Merge From a modem", img);
                baseToolbarItems.Add(mergeButton);

                //attachment button
                Image imgAttach = Image.FromStream(currentAssembly.GetManifestResourceStream("ModemToolbarIE.Resources.view.png"));
                AttachmentFormLink attachmentButton = new AttachmentFormLink(this, "Files", "View and download attachments", imgAttach);
                baseToolbarItems.Add(attachmentButton);


                if (mlic.Count != 0)
                {
                    foreach (var item in mlic)
                    {

                        Image image;
                        using (var ms = new MemoryStream(item.Img))
                        {
                            image = Image.FromStream(ms);
                        }

                        MenuListItem mtemp = new MenuListItem(this, item.Caption, item.Hint, item.Links, image);
                        baseToolbarItems.Add(mtemp);
                    }
                }

            }
            finally
            {

                ResumeLayout();
                PerformLayout();

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
            foreach (BaseToolbarItem item in baseToolbarItems)
            {
                item.Reset();
            }
            baseToolbarItems.Clear();
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
