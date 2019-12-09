using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Timers;
using System.Windows.Forms;
using WcfServiceModemToolbarSync.Database;


namespace WcfServiceModemToolbarSync
{

    using Link = KeyValuePair<string, string>;
    using MenuLink = KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>;
    using PostLink = KeyValuePair<string, ModemMwdPostObjects>;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WCFModemService : IWCFModemService
    {
        ServiceHost host;
        private readonly System.Timers.Timer _timer;
        Assembly currentAssembly;

        public bool IsUpdated { get; set; } = false;
        public int RemoteDbAuditTableSize = 0;

        List<MenuListItemClass> menuListItemClasses = new List<MenuListItemClass>();
        List<SearchBoxItemClass> searchBoxItemClasses = new List<SearchBoxItemClass>();
        List<LinkListItemClass> linkListItemClasses = new List<LinkListItemClass>();

        DbHelper dbHelper = new DbHelper();
        const string connectionStringName = "WcfServiceModemToolbarSync.Properties.Settings.RemoteDb";
        private string connectionString;



        public WCFModemService()
        {
            currentAssembly = Assembly.GetAssembly(GetType());
            try
            {


                if (File.Exists(RemoteFilepathFromFile()))
                {
                    DbHelper.remoteFile = RemoteFilepathFromFile();

                }

            }
            catch (Exception)
            {

            }


            if (SetDbConnectionString())
            {
                InitPublicProperties();
                TimerElapsed(null, null);
            }


            _timer = new System.Timers.Timer(120000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
            _timer.Enabled = true;


        }

        private void FixConnectionString()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        private bool SetDbConnectionString()
        {
            if (IsRemoteDbAvailable())
            {
                connectionString = dbHelper.remoteConnectionString;

                try
                {
                    CopyFile(DbHelper.remoteFile, DbHelper.localFile, true);

                }
                catch (Exception e)
                {
                    System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nCopy File Error: => " + e.ToString());
                }

                return true;

            }
            else if (IsLocalDbAvailable())
            {
                connectionString = dbHelper.localConnectionString;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsRemoteDbAvailable()
        {
            if (!File.Exists(DbHelper.remoteFile))
            {
                //MessageBox.Show("Remote not accessibøe");
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "IsRemoteDbAvailable => Remote not accessibøe \n");
                return false;
            }


            connectionString = dbHelper.remoteConnectionString;
            bool returnValue = false;

            try
            {
                CheckDbChanged();
                returnValue = true;
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nIsRemoteDbAvailable check DB error: => \n" + e.ToString());
            }

            return returnValue;
        }

        private bool IsLocalDbAvailable()
        {
            if (!File.Exists(DbHelper.localFile))
            {
                //MessageBox.Show("local not accessibøe"); 
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "IsLocalDbAvailable local not accessibøe");
                return false;
            }

            connectionString = dbHelper.localConnectionString;
            bool returnValue = false;

            try
            {
                CheckDbChanged();
                returnValue = true;
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nIslocalDbAvailable check DB error: => \n" + e.ToString());
            }

            return returnValue;
        }

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

        internal static void CopyFile(string srcFile, string dstFiler, bool overwrite)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dstFiler));
            }
            catch (Exception) { }

            System.IO.File.Copy(srcFile, dstFiler, overwrite);



        }

        public static string DataFolder
        {
            get
            {
                string result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //C:\Users\<user>\AppData\Roaming\myProgram\ 
                return Path.Combine(result, "ModemToolbar");
            }
        }

        public static string ProgramFolder
        {
            get
            {
                string result = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(result, @"remotefilename.dat");

            }

                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nCurrent Directory: " + Environment.CurrentDirectory);
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nEnvironment.SpecialFolder.ApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nEnvironment.SpecialFolder.ProgramFilesX86: " + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nAppDomain.CurrentDomain.BaseDirectory: " + AppDomain.CurrentDomain.BaseDirectory);
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nAssembly.GetExecutingAssembly().Location: " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nDirectory.GetCurrentDirectory(): " + System.IO.Directory.GetCurrentDirectory());
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nPath.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase): " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
                //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "\nPath.GetDirectoryName(Application.ExecutablePath): " + System.IO.Path.GetDirectoryName(Application.ExecutablePath));
        
}

    public static string RemoteFilepathFromFile()
    {
        string filepath = ProgramFolder + @"\remotefilename.dat";
        return System.IO.File.ReadAllText(filepath);
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (!SetDbConnectionString())
        {
            return;
        }

        if (!CheckDbChanged())
        {
            InitPublicProperties();

        }

        IsUpdated = true;

    }

    private bool CheckDbChanged()
    {

        Database.RemoteDbTableAdapters.tblAuditTrailTableAdapter auditAdapter = new Database.RemoteDbTableAdapters.tblAuditTrailTableAdapter();
        auditAdapter.Connection.ConnectionString = connectionString;
        int? count;
        bool isCountsEqual = false;

        count = auditAdapter.TableCountQuery();

        if (count.HasValue)
        {

            if (count.Value == RemoteDbAuditTableSize)
            {
                isCountsEqual = true;
            }
            else
            {
                isCountsEqual = false;
                RemoteDbAuditTableSize = count.Value;
            }

        }
        //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "Check DB Changes => \n connection string => " + connectionString) ;

        auditAdapter.Dispose();
        return isCountsEqual;

    }

    public void InitPublicProperties()
    {
        //here connect to remote db and populate items for toolbar
        menuListItemClasses = new List<MenuListItemClass>();
        linkListItemClasses = new List<LinkListItemClass>();
        searchBoxItemClasses = new List<SearchBoxItemClass>();

        menuListItemClasses = PopulateMenuLinkItems();
        linkListItemClasses = PopulateLinkItems();
        searchBoxItemClasses = PopulateSearchBox();


    }



    private Dictionary<int, MwdCompPosts> GetRowsToModemComp(IEnumerable<RemoteDb.SubToolsRow> comps)
    {
        Dictionary<int, MwdCompPosts> mcpDic = new Dictionary<int, MwdCompPosts>();
        int i = 0;
        foreach (RemoteDb.SubToolsRow row in comps)
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

    private Dictionary<int, MwdSoftPosts> GetRowsToModemSoft(IEnumerable<RemoteDb.SoftwareRow> comps)
    {
        Dictionary<int, MwdSoftPosts> mcpDic = new Dictionary<int, MwdSoftPosts>();
        int i = 0;
        foreach (RemoteDb.SoftwareRow row in comps)
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


    private string StringDbNull(RemoteDb.SubToolsRow row, string fieldName)
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

    private string StringDbNull(RemoteDb.SoftwareRow row, string fieldName)
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

    private string IntDbNull(RemoteDb.SubToolsRow row, string fieldName)
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

    /// <summary>
    /// Class to generate the Standrand tool menu
    /// it connects to Acces Database and poplate the menus
    /// </summary>
    /// <returns></returns>
    private List<MenuListItemClass> PopulateMenuLinkItems()
    {
        //check if there are any tools in tools menu in Db. if there are no tools return.
        Database.RemoteDbTableAdapters.ToolsTableAdapter toolAdapter = new Database.RemoteDbTableAdapters.ToolsTableAdapter();
        toolAdapter.Connection.ConnectionString = connectionString;
        RemoteDb.ToolsDataTable dsTools = new RemoteDb.ToolsDataTable();

        toolAdapter.Fill(dsTools);

        int? count = toolAdapter.ToolsCount();

        if (count.HasValue)
        {
            if (count == 0)
            {
                return null;
            }
        }

        //menu sub items is collection of items used to populate the menu. Key is the menu name (or toolname + tool size), and value is what is used to generate the sub menu
        Dictionary<string, List<PostLink>> menuSubItems = new Dictionary<string, List<PostLink>>();
        //this is also key value pair, key is the Menu display name, value is a class that gets posted to webpage when clicked.


        List<MenuListItemClass> menuClass = new List<MenuListItemClass>();


        foreach (RemoteDb.ToolsRow item in dsTools.Rows)
        {
            Database.RemoteDbTableAdapters.SubToolsTableAdapter compAdapter = new Database.RemoteDbTableAdapters.SubToolsTableAdapter();
            compAdapter.Connection.ConnectionString = connectionString;
            Database.RemoteDbTableAdapters.SoftwareTableAdapter softAdapter = new Database.RemoteDbTableAdapters.SoftwareTableAdapter();
            softAdapter.Connection.ConnectionString = connectionString;

            RemoteDb.SubToolsDataTable subTools = new RemoteDb.SubToolsDataTable();
            RemoteDb.SoftwareDataTable softTools = new RemoteDb.SoftwareDataTable();

            compAdapter.Fill(subTools);
            softAdapter.Fill(softTools);

            var varTools = from tool in subTools
                           where tool.ToolID == item.ID
                           select tool;

            var varSofts = from soft in softTools
                           where soft.ToolID == item.ID
                           select soft;

            //this is modem object, subcomponent and tools soft etc, which gets posted to webpage when menu is clicked.
            //it is derived from database items
            ModemMwdPostObjects mpo = new ModemMwdPostObjects(); //(ModemNoEngine, htmlDocument);
            mpo.MwdBhaPost = GetBhaObj(item.ToolName, "Please check components accordingly", true);
            mpo.MwdCompPostDict = GetRowsToModemComp(varTools);
            mpo.MwdSoftPostDict = GetRowsToModemSoft(varSofts);

            PostLink lnk = new PostLink(item.ToolName, mpo);

            string keyForDic = item.Category + "/" + item.ToolSize.ToString();

            if (menuSubItems.ContainsKey(keyForDic))
            {
                menuSubItems[keyForDic].Add(lnk);
            }
            else
            {
                List<PostLink> lst = new List<PostLink>();
                lst.Add(lnk);
                menuSubItems[keyForDic] = lst;

            }

            //System.IO.File.AppendAllText(@"C:\Users\h111765\wcferrorlog.txt", "Populate Meny => /n connection string => \n" + connectionString);

            compAdapter.Dispose();
            softAdapter.Dispose();
        }

        toolAdapter.Dispose();



        Image img = Image.FromStream(currentAssembly.GetManifestResourceStream("WcfServiceModemToolbarSync.Resources.view.png"));
        byte[] imageByte;

        using (var ms = new MemoryStream())
        {
            img.Save(ms, ImageFormat.Png);
            imageByte = ms.ToArray();
        }

        Dictionary<string, List<MenuLink>> menuLinkPairs = new Dictionary<string, List<MenuLink>>();

        foreach (var item in menuSubItems)
        {
            string[] typeSize;
            typeSize = item.Key.Split('/');

            MenuLink ml = new MenuLink(typeSize[1], item.Value.ToArray());

            if (menuLinkPairs.ContainsKey(typeSize[0]))
            {
                menuLinkPairs[typeSize[0]].Add(ml);
            }
            else
            {
                List<MenuLink> mllink = new List<MenuLink>();
                mllink.Add(ml);
                menuLinkPairs[typeSize[0]] = mllink;
            }



        }

        foreach (var item in menuLinkPairs)
        {
            //string[] typeSize;
            //typeSize = item.Key.Split('/');

            MenuListItemClass mlic = new MenuListItemClass();


            mlic.Caption = item.Key; // typeSize[0];
            mlic.Hint = item.Key; // typeSize[0];
            mlic.Links = item.Value.ToArray();
            mlic.Img = imageByte;

            menuClass.Add(mlic);
        }

        return menuClass;
    }

    public List<LinkListItemClass> PopulateLinkItems()
    {
        linkListItemClasses = new List<LinkListItemClass>();

        Link link1 = new Link("Gant", "http://tanwebs.corp.halliburton.com/pls/log_web/gant.web");
        Link link2 = new Link("SperryWeb", "http://sperryweb.corp.halliburton.com/");
        Link link3 = new Link("Tool Softwares", "http://sperryweb.corp.halliburton.com/PESoftTest/download/");
        Link link4 = new Link("Rob Stat", "http://34.163.71.70/robSTAT.htm");

        //
        //List of links
        //

        Image img = Image.FromStream(currentAssembly.GetManifestResourceStream("WcfServiceModemToolbarSync.Resources.magic-wand.png"));
        byte[] imageByte;

        using (var ms = new MemoryStream())
        {
            img.Save(ms, ImageFormat.Png);
            imageByte = ms.ToArray();
        }

        LinkListItemClass llic = new LinkListItemClass();
        llic.Caption = "My Links";
        llic.Hint = "Some Useful Link";
        llic.Links = new Link[] { link1, link2, link3, link4 };
        llic.Img = imageByte;

        linkListItemClasses.Add(llic);

        return linkListItemClasses;
    }

    public List<SearchBoxItemClass> PopulateSearchBox()
    {
        searchBoxItemClasses = new List<SearchBoxItemClass>();

        Image img = Image.FromStream(currentAssembly.GetManifestResourceStream("WcfServiceModemToolbarSync.Resources.view.png"));
        byte[] imageByte;

        using (var ms = new MemoryStream())
        {
            img.Save(ms, ImageFormat.Png);
            imageByte = ms.ToArray();
        }

        SearchBoxItemClass sbic = new SearchBoxItemClass();
        sbic.ClearHistoryText = "<clear>";
        sbic.GreetingText = "terms to serach!";
        sbic.SearchURL = "http://sphq.corp.halliburton.com/Search/Pages/results.aspx?k={0}";
        sbic.SearchBoxTooltip = "Search here";
        sbic.ButtonText = "Search";
        sbic.ButtonTooltip = "Click to search";
        sbic.InputBoxSize = new Size(160, 16);
        sbic.InputBoxFlatStyle = FlatStyle.System;
        sbic.Img = imageByte;

        searchBoxItemClasses.Add(sbic);

        return searchBoxItemClasses;

    }

    public int GetSyncStatus()
    {
        return RemoteDbAuditTableSize;

    }

    //public void CallbackMenuListItemClass()
    //{
    //    if (Callback != null)
    //    {
    //        MenuListClass mc = new MenuListClass();
    //        mc.List = menuListItemClasses;
    //        Callback.DataMenuListItemClassEquals(mc);
    //    }

    //}

    //public void CallbackSearchBoxItemClass()
    //{
    //    if (Callback != null)
    //    {
    //        SearchListClass sc = new SearchListClass();
    //        sc.List = searchBoxItemClasses;
    //        Callback.DataSearchBoxItemClassEquals(sc);
    //    }

    //}

    //public void CallbackLinkListItemClass()
    //{
    //    if (Callback != null)
    //    {
    //        LinkListClass lc = new LinkListClass();
    //        lc.List = linkListItemClasses;
    //        Callback.DataLinkListItemClassEquals(lc);
    //    }

    //}

    public MenuListClass GetMenuListItemClasses()
    {
        MenuListClass mc = new MenuListClass();
        mc.List = menuListItemClasses;
        return mc;
    }

    public SearchListClass GetSearchBoxItemClasses()
    {
        SearchListClass sc = new SearchListClass();
        sc.List = searchBoxItemClasses;
        return sc;
    }

    public LinkListClass GetLinkListItemClasses()
    {
        LinkListClass lc = new LinkListClass();
        lc.List = linkListItemClasses;
        return lc;
    }

    //IWCFModemServiceCallback Callback
    //{
    //    get
    //    {
    //        return OperationContext.Current.GetCallbackChannel<IWCFModemServiceCallback>();
    //    }
    //}
}
}
