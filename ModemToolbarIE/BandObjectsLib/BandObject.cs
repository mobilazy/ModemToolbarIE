//	Copyright Pavel Zolnikov, 2002
//
//			BandObject - implements generic Band Object functionality.

using Microsoft.Win32;
using mshtml;
using SHDocVw;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ModemWebUtility;

//[assembly: AssemblyVersion("1.0.0.0")]
//[assembly: AssemblyKeyFile(@"..\..\..\BandObjects.snk")]

namespace BandObjectLib
{
    /// <summary>
    /// Implements generic Band Object functionality. 
    /// </summary>
    /// <example>
    /// [Guid("YOURGUID-GOES-HERE-YOUR-GUIDGOESHERE")]
    /// [BandObject("Hello World Bar", BandObjectStyle.Horizontal | BandObjectStyle.ExplorerToolbar , HelpText = "Shows bar that says hello.")]
    /// public class HelloWorldBar : BandObject
    /// { /*...*/ }
    /// </example>
    public class BandObject : UserControl, IObjectWithSite, IDeskBand, IDockingWindow, IOleWindow, IInputObject
    {
        /// <summary>
        /// Reference to the host explorer.
        /// </summary>
        protected WebBrowserClass Explorer; //protected changed to internal
        
        protected IInputObjectSite BandObjectSite;
        
        /// <summary>
        /// This event is fired after reference to hosting explorer is retreived and stored in Explorer property.
        /// </summary>
        public event EventHandler ExplorerAttached;
        public event EventHandler<ModemEventArgs> HtmlDocCompleted;

        protected HTMLDocument htmlDocument;
        protected static BandObjectStyle style = BandObjectStyle.ExplorerToolbar;
        protected static string toolbarName = "ModemToolbar";
        protected static string toolbarHelpText = "ModemToolbar";
        private string mNo = "";
        private string currentUrl;

        public HTMLDocument HtmlDoc
        {
            get { return htmlDocument; }
        }

        public string ModemNoEngine
        {
            get { return mNo; }
        }

        public BandObject()
        {
            //assembly resolve

            //MessageBox.Show("Break Bandobject 1");
            
            FixConnectionString();
            InitializeComponent();
            BackColor = Color.Transparent;
        }

        

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ExplorerBar
            // 
            this.AutoSize = true;
            Name = "BandObject";
            this.Size = new System.Drawing.Size(1450, 25);
            this.ResumeLayout(false);
        }

        private void FixConnectionString()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            
        }

        public void RefreshPage()
        {
            if (currentUrl != "")
            {
                Navigate2(currentUrl);
            }
        }

        public void OnDocumentComplete(object pDisp, ref object URL)
        {
            htmlDocument = (HTMLDocument)Explorer.Document;
 
            if (URL.ToString().Contains("www.google.com"))
            {
                IHTMLElement head = (IHTMLElement)((IHTMLElementCollection)
                           htmlDocument.all.tags("head")).item(null, 0);
                IHTMLScriptElement scriptObject =
                  (IHTMLScriptElement)htmlDocument.createElement("script");
                scriptObject.type = @"text/javascript";
                scriptObject.text = "\nfunction hidediv(){document.getElementById" +
                                    "('myOwnUniqueId12345').style.visibility = 'hidden';}\n\n";
                ((HTMLHeadElement)head).appendChild((IHTMLDOMNode)scriptObject);

                string div = "<div id=\"myOwnUniqueId12345\" style=\"position:" +
                             "fixed;bottom:0px;right:0px;z-index:9999;width=300px;" +
                             "height=150px;\"> <div style=\"position:relative;" +
                             "float:right;font-size:9px;\"><a " +
                             "href=\"javascript:hidediv();\">close</a></div>" +
                    "My content goes here ...</div>";

                htmlDocument.body.insertAdjacentHTML("afterBegin", div);

            }

            ModemEvents me = ModemEvents.None;
            
            

            if (URL.ToString().Contains(@"http://tanwebs.corp.halliburton.com/pls/log_web/"))
            {

                currentUrl = URL.ToString();

                if (URL.ToString().Contains(@"mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID"))
                {
                    me = ModemEvents.View;
                    int str = URL.ToString().IndexOf("P_SSORD_ID=")+11;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_vieword$mwddwd_mc.QueryViewByKey?P_MWDDWD_ID"))
                {
                    me = ModemEvents.BhaView;
                    int str = URL.ToString().IndexOf("&P_3=")+5;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_vieword$motor_mc.QueryViewByKey?P_MOTORS_ID"))
                {
                    me = ModemEvents.DdView;
                    int str = URL.ToString().IndexOf("&P_3=")+5;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_vieword$gp_mc.QueryViewByKey?P_GP_ID"))
                {
                    me = ModemEvents.GpView;
                    int str = URL.ToString().IndexOf("&P_3=")+5;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_order_new$header_mc.QueryViewByKey?P_SSORD_ID"))
                {
                    me = ModemEvents.Edit;
                    int str = URL.ToString().IndexOf("P_SSORD_ID=")+11;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_order_new$bha_mc.QueryViewByKey?P_MWDDWD_ID"))
                {
                    me = ModemEvents.BhaEdit;
                    int str = URL.ToString().IndexOf("&P_10=")+6;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_order_new$motor_mc.QueryViewByKey?P_MOTORS_ID"))
                {
                    me = ModemEvents.DdEdit;
                    int str = URL.ToString().IndexOf("&P_10=")+6;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"mobssus_order_new$gp_mc.QueryViewByKey?P_GP_ID"))
                {
                    me = ModemEvents.GpEdit;
                    int str = URL.ToString().IndexOf("&P_10=")+6;
                    mNo = URL.ToString().Substring(str, 7);
                }
                else if (URL.ToString().Contains(@"gant.web"))
                {
                    me = ModemEvents.Gant;
                    mNo = "";
                }
                else if(URL.ToString().Contains(@"bha_mc.actionview"))
                {
                    ModemParameters mp = new ModemParameters(htmlDocument, true, "P_10");
                    me = ModemEvents.BhaEdit;
                    mNo = mp.ModemNo;

                }
                else if (URL.ToString().Contains(@"order_mc.actionview"))
                {
                    ModemParameters mp = new ModemParameters(htmlDocument, true, "P_SSORD_ID");
                    me = ModemEvents.View;
                    mNo = mp.ModemNo;
                }
                else if (URL.ToString().Contains(@"header_mc.actionview"))
                {
                    ModemParameters mp = new ModemParameters(htmlDocument, true, "P_SSORD_ID");
                    me = ModemEvents.Edit;
                    mNo = mp.ModemNo;
                }
                else
                {
                    ModemParameters mp = new ModemParameters(htmlDocument, true, "P_SSORD_ID");
                    me = ModemEvents.View;
                    mNo = mp.ModemNo;
                }
            }

            OnHtmlDocComplete(me, mNo);

        }

        public void OnBeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers,
        ref bool Cancel)
        {
            //if navigating to modem site then do something before modem site has opened.




        }

        protected virtual void OnHtmlDocComplete(ModemEvents ea, string modemNo)
        {
            HtmlDocCompleted?.Invoke(this, new ModemEventArgs { ModemEvent = ea, ModemNo = modemNo });

        }

        public void Navigate2(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return;
                object val = null;
                object objURL = url;
                Explorer.Navigate2(ref objURL, ref val, ref val, ref val, ref val);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Title of band object. Displayed at the left or on top of the band object.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("")]
        public String Title
        {
            get => _title;
            set => _title = value;
        }
        String _title;



        /// <summary>
        /// Minimum size of the band object. Default value of -1 sets no minimum constraint.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "-1,-1")]
        public Size MinSize
        {
            get => _minSize;
            set => _minSize = value;
        }
        Size _minSize = new Size(-1, -1);

        /// <summary>
        /// Maximum size of the band object. Default value of -1 sets no maximum constraint.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "-1,-1")]
        public Size MaxSize
        {
            get => _maxSize;
            set => _maxSize = value;
        }
        Size _maxSize = new Size(-1, -1);

        /// <summary>
        /// Says that band object's size must be multiple of this size. Defauilt value of -1 does not set this constraint.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "-1,-1")]
        public Size IntegralSize
        {
            get => _integralSize;
            set => _integralSize = value;
        }
        Size _integralSize = new Size(-1, -1);


        public virtual void GetBandInfo(
            UInt32 dwBandID,
            UInt32 dwViewMode,
            ref DESKBANDINFO dbi)
        {
            //dbi.wszTitle = this.Title;

            //dbi.ptActual.X = this.Size.Width;
            //dbi.ptActual.Y = this.Size.Height;

            //dbi.ptMaxSize.X = this.MaxSize.Width;
            //dbi.ptMaxSize.Y = this.MaxSize.Height;

            //dbi.ptMinSize.X = this.MinSize.Width;
            //dbi.ptMinSize.Y = this.MinSize.Height;

            //dbi.ptIntegral.X = this.IntegralSize.Width;
            //dbi.ptIntegral.Y = this.IntegralSize.Height;

            //dbi.dwModeFlags = DBIM.TITLE | DBIM.ACTUAL | DBIM.MAXSIZE | DBIM.MINSIZE | DBIM.INTEGRAL;

            if ((dbi.dwMask & DBIM.MINSIZE) != 0)
            {
                dbi.ptMinSize.X = MinSize.Width == 0 ? -1 : MinSize.Width;
                dbi.ptMinSize.Y = MinSize.Height == 0 ? -1 : MinSize.Height;
                if (dbi.ptMinSize.X <= 0 || dbi.ptMinSize.Y <= 0)
                {
                    dbi.dwMask = dbi.dwMask ^ DBIM.MINSIZE;
                }
            }
            if ((dbi.dwMask & DBIM.MAXSIZE) != 0)
            {
                dbi.ptMaxSize.X = MaxSize.Width == 0 ? -1 : MaxSize.Width;
                dbi.ptMaxSize.Y = MaxSize.Height == 0 ? -1 : MaxSize.Height;

                if (dbi.ptMaxSize.X <= 0 || dbi.ptMaxSize.Y <= 0)
                {
                    dbi.dwMask = dbi.dwMask ^ DBIM.MAXSIZE;
                }
            }
            if ((dbi.dwMask & DBIM.INTEGRAL) != 0)
            {
                dbi.ptIntegral.X = IntegralSize.Width;
                dbi.ptIntegral.Y = IntegralSize.Height;

                if (dbi.ptIntegral.X <= 0 || dbi.ptIntegral.Y <= 0)
                {
                    dbi.dwMask = dbi.dwMask ^ DBIM.INTEGRAL;
                }
            }
            if ((dbi.dwMask & DBIM.ACTUAL) != 0)
            {
                dbi.ptActual.X = Size.Width;
                dbi.ptActual.Y = Size.Height;
            }
            if ((dbi.dwMask & DBIM.TITLE) != 0)
            {
                if (string.IsNullOrEmpty(Title))
                {
                    dbi.dwMask = dbi.dwMask ^ DBIM.TITLE;
                }
                else
                {
                    dbi.wszTitle = Title;
                }
            }
            if ((dbi.dwMask & DBIM.MODEFLAGS) != 0)
            {
                dbi.dwModeFlags = /*DBIMF.VARIABLEHEIGHT | */ DBIMF.BREAK | DBIMF.ALWAYSGRIPPER;
            }

            if ((dbi.dwMask & DBIM.BKCOLOR) != 0)
            {
                dbi.dwMask = dbi.dwMask ^ DBIM.BKCOLOR;
            }
        }

        /// <summary>
        /// Called by explorer when band object needs to be showed or hidden.
        /// </summary>
        /// <param name="fShow"></param>
        public virtual void ShowDW(bool fShow)
        {
            if (fShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Called by explorer when window is about to close.
        /// </summary>
        public virtual void CloseDW(UInt32 dwReserved)
        {
            Dispose(true);
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public virtual void ResizeBorderDW(IntPtr prcBorder, Object punkToolbarSite, bool fReserved) { }

        public virtual void GetWindow(out System.IntPtr phwnd)
        {
            phwnd = Handle;
        }

        public virtual void ContextSensitiveHelp(bool fEnterMode) { }

        public virtual void SetSite(object pUnkSite)
        {
            if (BandObjectSite != null)
            {
                Marshal.ReleaseComObject(BandObjectSite);
            }

            if (Explorer != null)
            {
                Marshal.ReleaseComObject(Explorer);
                Explorer = null;
            }

            
            BandObjectSite = (IInputObjectSite)pUnkSite;
            
            if (BandObjectSite != null)
            {
                //pUnkSite is a pointer to object that implements IOleWindowSite or something  similar
                //we need to get access to the top level object - explorer itself
                //to allows this explorer objects also implement IServiceProvider interface
                //(don't mix it with System.IServiceProvider!)
                //we get this interface and ask it to find WebBrowserApp
                _IServiceProvider sp = BandObjectSite as _IServiceProvider;
                Guid guid = ExplorerGUIDs.IID_IWebBrowserApp;
                Guid riid = ExplorerGUIDs.IID_IUnknown;

                try
                {
                    object w;
                    sp.QueryService(
                        ref guid,
                        ref riid,
                        out w);

                    //once we have interface to the COM object we can create RCW from it
                    Explorer = (WebBrowserClass)Marshal.CreateWrapperOfType(
                        w as IWebBrowser,
                        typeof(WebBrowserClass)
                        );

                    Explorer.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                    Explorer.BeforeNavigate2 += new DWebBrowserEvents2_BeforeNavigate2EventHandler(this.OnBeforeNavigate2);

                    OnExplorerAttached(EventArgs.Empty);
                }
                catch (COMException)
                {
                    //we anticipate this exception in case our object instantiated 
                    //as a Desk Band. There is no web browser service available.
                }
            }
            else
            {
                Explorer.DocumentComplete -= new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                Explorer.BeforeNavigate2 -= new DWebBrowserEvents2_BeforeNavigate2EventHandler(this.OnBeforeNavigate2);
                Explorer = null;
            }

           

        }

        

        public virtual void GetSite(ref Guid riid, out Object ppvSite)
        {
            ppvSite = BandObjectSite;


        }

        /// <summary>
        /// Called explorer when focus has to be chenged.
        /// </summary>
        public virtual void UIActivateIO(Int32 fActivate, ref MSG Msg)
        {
            if (fActivate != 0)
            {
                Control ctrl = GetNextControl(this, true);//first
                if (ModifierKeys == Keys.Shift)
                {
                    ctrl = GetNextControl(ctrl, false);//last
                }

                if (ctrl != null)
                {
                    ctrl.Select();
                }

                Focus();
            }
        }

        public virtual Int32 HasFocusIO()
        {
            return ContainsFocus ? 0 : 1; //S_OK : S_FALSE;
        }

        /// <summary>
        /// Called by explorer to process keyboard events. Undersatands Tab and F6.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>S_OK if message was processed, S_FALSE otherwise.</returns>
        public virtual Int32 TranslateAcceleratorIO(ref MSG msg)
        {
            if (msg.message == 0x100)//WM_KEYDOWN
            {
                if (msg.wParam == (uint)Keys.Tab || msg.wParam == (uint)Keys.F6)//keys used by explorer to navigate from control to control
                {
                    if (SelectNextControl(
                            ActiveControl,
                            ModifierKeys == Keys.Shift ? false : true,
                            true,
                            true,
                            false)
                        )
                    {
                        return 0;//S_OK
                    }
                }
            }

            return 1;//S_FALSE
        }

        /// <summary>
        /// Override this method to handle ExplorerAttached event.
        /// </summary>
        /// <param name="ea"></param>
        protected virtual void OnExplorerAttached(EventArgs ea)
        {
            if (ExplorerAttached != null)
            {
                ExplorerAttached(this, ea);
            }
        }

        /// <summary>
        /// Notifies explorer of focus change.
        /// </summary>
        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            BandObjectSite.OnFocusChangeIS(this as IInputObject, 1);
        }
        /// <summary>
        /// Notifies explorer of focus change.
        /// </summary>
        protected override void OnLostFocus(System.EventArgs e)
        {
            InternalLostFocus(e);
        }

        public void InternalLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            if (ActiveControl == null)
            {
                BandObjectSite.OnFocusChangeIS(this as IInputObject, 0);
            }
        }
        public void InternalGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            BandObjectSite.OnFocusChangeIS(this as IInputObject, 1);
        }

        /// <summary>
        /// Called when derived class is registered as a COM server.
        /// </summary>
        [ComRegisterFunctionAttribute]
        public static void Register(Type t)
        {
            string guid = t.GUID.ToString("B");

            RegistryKey rkClass = Registry.ClassesRoot.CreateSubKey(@"CLSID\" + guid);
            RegistryKey rkCat = rkClass.CreateSubKey("Implemented Categories");

            string name = toolbarName;
            string help = toolbarHelpText;

            BandObjectAttribute[] boa = (BandObjectAttribute[])t.GetCustomAttributes(
                typeof(BandObjectAttribute),
                false);

            //string name = t.Name;
            //string help = t.Name;
            BandObjectStyle style = 0;
            if (boa.Length == 1)
            {
                if (boa[0].Name != null)
                {
                    name = boa[0].Name;
                }

                if (boa[0].HelpText != null)
                {
                    help = boa[0].HelpText;
                }

                style = boa[0].Style;
            }

            rkClass.SetValue(null, name);
            rkClass.SetValue("MenuText", name);
            rkClass.SetValue("HelpText", help);

            if (0 != (style & BandObjectStyle.Vertical))
            {
                rkCat.CreateSubKey("{00021493-0000-0000-C000-000000000046}");
            }

            if (0 != (style & BandObjectStyle.Horizontal))
            {
                rkCat.CreateSubKey("{00021494-0000-0000-C000-000000000046}");
            }

            if (0 != (style & BandObjectStyle.TaskbarToolBar))
            {
                rkCat.CreateSubKey("{00021492-0000-0000-C000-000000000046}");
            }

            if (0 != (style & BandObjectStyle.ExplorerToolbar))
            {
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Toolbar").SetValue(guid, name);
            }
        }

        /// <summary>
        /// Called when derived class is unregistered as a COM server.
        /// </summary>
        [ComUnregisterFunctionAttribute]
        public static void Unregister(Type t)
        {
            string guid = t.GUID.ToString("B");
            BandObjectAttribute[] boa = (BandObjectAttribute[])t.GetCustomAttributes(
                typeof(BandObjectAttribute),
                false);

            BandObjectStyle style = 0;
            if (boa.Length == 1)
            {
                style = boa[0].Style;
            }

            if (0 != (style & BandObjectStyle.ExplorerToolbar))
            {
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Toolbar").DeleteValue(guid, false);
            }

            Registry.ClassesRoot.CreateSubKey(@"CLSID").DeleteSubKeyTree(guid);
        }

        #region "Interop"
        [DllImport("user32.dll")]
        public static extern int TranslateMessage(ref MSG lpMsg);

        [DllImport("user32", EntryPoint = "DispatchMessage")]
        static extern bool DispatchMessage(ref MSG msg);

        [DllImport("uxtheme", ExactSpelling = true)]
        public extern static Int32 DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref Rectangle pRect);

        abstract class ExplorerGUIDs
        {
            public static readonly Guid IID_IWebBrowserApp = new Guid("{0002DF05-0000-0000-C000-000000000046}");
            public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
        }
        #endregion;

        #region Transparency

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor == Color.Transparent)
            {
                IntPtr hdc = e.Graphics.GetHdc();
                Rectangle rec = new Rectangle(e.ClipRectangle.Left,
                e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
                try
                {
                    DrawThemeParentBackground(Handle, hdc, ref rec);
                    e.Graphics.ReleaseHdc(hdc);
                }
                catch (Exception)
                {
                    e.Graphics.ReleaseHdc(hdc);
                    base.OnPaintBackground(e);
                }
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        #endregion
    }
}