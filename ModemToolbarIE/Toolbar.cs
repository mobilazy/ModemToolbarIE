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
        public List<MenuListItemClass> mlic = new List<MenuListItemClass>();
        public List<SearchBoxItemClass> sbic = new List<SearchBoxItemClass>();
        public List<LinkListItemClass> llic = new List<LinkListItemClass>();
        public int syncStatus = 0;

        MergeFormLink mergeButton;
        SearchBoxItem searchBox;

        List<BaseToolbarItem> baseToolbarItems = new List<BaseToolbarItem>();

        private System.Timers.Timer _timer;

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
                string result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //C:\Users\<user>\AppData\Roaming\myProgram\ 
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
            //MessageBox.Show("Break -2");

            CosturaUtility.Initialize();

            Assembly asm = Assembly.GetExecutingAssembly();
            string fullName = asm.GetModules()[0].FullyQualifiedName;
            toolbarFolder = Path.GetDirectoryName(fullName);
            dataFolder = DataFolder;
            try
            {
                cacheFolder = Path.Combine(dataFolder, "Cache");
                Directory.CreateDirectory(cacheFolder);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


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
            //StartWcfComms();
            StartLocalComms();
            //_timer = new System.Timers.Timer(150000) { AutoReset = true };
            //_timer.Elapsed += TimerElapsed;
            //_timer.Enabled = true;
            
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //StartWcfComms();
            StartLocalComms();
        }

        /*
        internal void StartWcfComms()
        {
            
            
            //MessageBox.Show("Start WCF firing with sync status: " + syncStatus);
            NetTcpBinding myBinding = new NetTcpBinding()
            {
                //CloseTimeout = TimeSpan.FromMinutes(2),
                ListenBacklog = 50,
                MaxBufferPoolSize = 10485760,
                MaxBufferSize = 10485760,
                MaxConnections = 50,
                MaxReceivedMessageSize = 10485760,
                //OpenTimeout = TimeSpan.FromMinutes(5),
                //PortSharingEnabled = false,
                //ReceiveTimeout = TimeSpan.FromMinutes(5),
                //SendTimeout = TimeSpan.FromMinutes(5),
                //TransactionFlow = false,
                //TransactionProtocol = TransactionProtocol.OleTransactions,
                //TransferMode = TransferMode.Buffered,
                //ReaderQuotas = new XmlDictionaryReaderQuotas()
                //{
                //    MaxArrayLength = 10240,
                //    MaxBytesPerRead = 10485760,
                //    MaxDepth = 100,
                //    MaxNameTableCharCount = 10485760,
                //    MaxStringContentLength = 10485760
                //},

                //ReliableSession = new OptionalReliableSession()
                //{
                //    Enabled = true,
                //    InactivityTimeout = TimeSpan.FromMinutes(10),
                //    Ordered = true
                //}

            };


            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:9124/");
            var client = new ModemService.WCFModemServiceClient(myBinding, endpoint);
            bool commError = false;

            try
            {
                var proxy = client.ChannelFactory.CreateChannel();

                if (syncStatus == proxy.GetSyncStatus())
                {
                    client.Close();
                    return;
                }

                SearchListClass sc = new SearchListClass();
                sc = proxy.GetSearchBoxItemClasses();
                sbic = new List<SearchBoxItemClass>();
                sbic = sc.List;

                LinkListClass lc = new LinkListClass();
                lc = proxy.GetLinkListItemClasses();
                llic = new List<LinkListItemClass>();
                llic = lc.List;

                MenuListClass mc = new MenuListClass();
                mc = proxy.GetMenuListItemClasses();
                mlic = new List<MenuListItemClass>();
                mlic = mc.List;

                CreateToolbarItems();
                syncStatus = proxy.GetSyncStatus();
                client.Close();


            }
            catch (CommunicationException e)
            {
                txtStatus.Text = "Comm Error";
                commError = true;
                client.Abort();
            }
            catch (TimeoutException e)
            {
                txtStatus.Text = "Timeout";
                commError = true;
                client.Abort();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error";
                commError = true;
                client.Abort();
                //MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (commError)
                {

                }

            }



        }
        */
        internal void StartLocalComms()
        {
            bool commError = false;

            try
            {

                using (NoProxySync npProxy = new NoProxySync())
                {
                    SearchListClass sc = new SearchListClass();
                    sc = npProxy.GetSearchBoxItemClasses();
                    sbic = new List<SearchBoxItemClass>();
                    sbic = sc.List;

                    LinkListClass lc = new LinkListClass();
                    lc = npProxy.GetLinkListItemClasses();
                    llic = new List<LinkListItemClass>();
                    llic = lc.List;

                    MenuListClass mc = new MenuListClass();
                    mc = npProxy.GetMenuListItemClasses();
                    mlic = new List<MenuListItemClass>();
                    mlic = mc.List;

                    CreateToolbarItems();

                }

                

            }
            catch (CommunicationException e)
            {
                txtStatus.Text = "Comm Error";
                commError = true;
            }
            catch (TimeoutException e)
            {
                txtStatus.Text = "Timeout";
                commError = true;
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error";
                commError = true;

            }
            finally
            {
                if (commError)
                {

                }

            }
        }

        internal void CreateToolbarItems()
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
                

                Assembly currentAssembly = Assembly.GetAssembly(GetType());
                //MessageBox.Show("Break 2");

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
                Image img = Image.FromStream(currentAssembly.GetManifestResourceStream("ModemToolbarIE.Resources.magic-wand.png"));
                mergeButton = new MergeFormLink(this, "Merge", "Merge From a modem", img);
                baseToolbarItems.Add(mergeButton);


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

    //[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //public class CallbackHandler : ModemToolbarIE.ModemService.IWCFModemServiceCallback
    //{
    //    private Toolbar toolbar;

    //    public CallbackHandler(Toolbar _toolbar)
    //    {
    //        toolbar = _toolbar;
    //    }

    //    public void Alive()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void DataLinkListItemClassEquals(LinkListClass linkList)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void DataMenuListItemClassEquals(MenuListClass menuList)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void DataSearchBoxItemClassEquals(SearchListClass searchBox)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SyncStatusEquals(int status)
    //    {
    //        MessageBox.Show("Sync status is: " + status.ToString());
    //        toolbar.syncStatus = status;

    //    }
    //}

    //public class ModemServiceClient : DuplexChannelFactory<ModemService.IWCFModemService>
    //{
    //    public ModemServiceClient(object callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
    //    : base(callbackInstance, binding, remoteAddress)
    //    {

    //    }
    //}



}
