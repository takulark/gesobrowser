using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace gesobrowser
{
    public partial class BrowserForm : Form
    {
        private string Flashdll { get; set; }
        private string Flashversion { get; set; }
        private string UserAgent { get; set; }
        private string AppPath { get; set; }
        private string AppDir { get; set; }
        private string AppName { get; set; }
        private string AppNameWoExt { get; set; }
        private string Proxyserver { get; set; }
        private bool IsAudio { get; set; }
        public bool IsDomain { get; set; }
        private double ZoomLevel { get; set; }
        private InifileUtils Ini { get; set; }
        private WinApi.WINDOWPLACEMENT Placement;
        private DialogResult DialogCloseShow { get; set; }
        private CefLibraryHandle LibraryLoader { get; set; }
        public IList<BrowserTab> BrowserTablist { get; set; }
        private Color[] TabColor { get; set; }

        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult chk = DialogResult.OK;
            if (DialogCloseShow == DialogResult.OK)
            {
                chk = MessageBox.Show("ウィンドウを閉じますか？", "質問", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            if (chk == DialogResult.OK)
            {
                WinApi.GetWindowPlacement(this.Handle, out Placement);
                string Stmp = $"{Placement.normalPosition.left}, {Placement.normalPosition.top}, {Placement.normalPosition.right}, {Placement.normalPosition.bottom}";
                if (Placement.normalPosition.left >= 0)
                { Ini.SetValue("Profile", "Window", Stmp); }
                Ini.SetValue("Profile", "ZoomLevel", ZoomLevel.ToString());
                Ini.SetValue("Profile", "DialogCloseShow", (int)DialogCloseShow);
                Ini.SetValue("Profile", "UserAgent", UserAgent);
                Stmp = $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}";
                Ini.SetValue("Profile", "version", Stmp);
                for (int i = 0; i < 4; i++)
                {
                    Ini.SetValue("Profile", "TabColor" + (i + 1), ColorTranslator.ToHtml(TabColor[i]));

                }
                LibraryLoader.Dispose();
                Ini.Dispose();
            }
            else
            {
                e.Cancel = true;

            }
        }
        public BrowserForm(string url)
        {
            AppPath = Application.ExecutablePath;
            //ディレクトリ
            AppDir = Path.GetDirectoryName(AppPath);
            //ファイル名
            AppName = Path.GetFileName(AppPath);
            //ファイル名(拡張子含まず)
            AppNameWoExt = Path.ChangeExtension(AppName, null);
            InitializeComponent();
            Directory.SetCurrentDirectory(AppDir);

            string icofile = AppNameWoExt + ".ico";

            if (File.Exists(icofile))
            {
                this.Icon = Icon.ExtractAssociatedIcon(icofile);
            }
            this.Text = AppNameWoExt;
            Ini = new InifileUtils(Path.Combine(AppDir, AppNameWoExt + ".ini"));
            string buf = Ini.GetValueString("Profile", "Window");
            if (buf != "")
            {
                //int x, y, w, h;
                string[] fields = buf.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //x = int.Parse(fields[0]);
                //y = int.Parse(fields[1]);
                //w = int.Parse(fields[2]);
                //h = int.Parse(fields[3]);
                WinApi.GetWindowPlacement(this.Handle, out Placement);
                Placement.normalPosition.left = int.Parse(fields[0]);
                Placement.normalPosition.top = int.Parse(fields[1]);
                Placement.normalPosition.right = int.Parse(fields[2]);
                Placement.normalPosition.bottom = int.Parse(fields[3]);
                Placement.showCmd = WinApi.SW.SHOWNORMAL;
            }
            ZoomLevel = 100;
            buf = Ini.GetValueString("Profile", "ZoomLevel");
            if (buf != "")
            {
                ZoomLevel = double.Parse(buf);
            }
            IsAudio = false;
            buf = Ini.GetValueString("Profile", "audio");
            if (buf != "")
            {
                IsAudio = int.Parse(buf) == 0 ? false : true;//bool.Parse(buf);
            }
            IsDomain = false;
            buf = Ini.GetValueString("Profile", "domain");
            if (buf != "")
            {
                IsDomain = int.Parse(buf) == 0 ? false : true;//bool.Parse(buf);
            }
            DialogCloseShow = DialogResult.OK;
            buf = Ini.GetValueString("Profile", "DialogCloseShow");
            if (buf != "")
            {
                DialogCloseShow = (DialogResult)int.Parse(buf);
            
            }
            UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";
            buf = Ini.GetValueString("Profile", "UserAgent");
            if (buf != "")
            {
                UserAgent = buf;
            }
            Proxyserver = Ini.GetValueString("Profile", "proxy");
            Flashdll = "pepflashplayer.dll";
            buf = Ini.GetValueString("Profile", "flashdll");
            if (buf != "")
            {
                Flashdll = buf;
            }
            Flashversion = "32.0.0.453";
            buf = Ini.GetValueString("Profile", "flashversion");
            if (buf != "")
            {
                Flashversion = buf;
            }
            TabColor = new Color[4] { SystemColors.Control, Color.Black, Color.LightBlue, Color.Black };
            for (int i = 0; i < 4; i++)
            {
                buf = Ini.GetValueString("Profile", "TabColor" + (i + 1));
                if (buf != "")
                {
                    TabColor[i] = ColorTranslator.FromHtml(buf);
                }
            }
            LibraryLoader = new CefLibraryHandle(Path.Combine(AppDir,
#if tx64
        "x64"
#else
	"x86"
#endif
		, "libcef.dll"));
            if (LibraryLoader.IsInvalid) { throw new Exception("no lib"); }
            
            BrowserTablist = new List<BrowserTab>();
            InitBrowser();
            browserPanel.Dock = DockStyle.Fill;
            tabControl1.Dock = DockStyle.Fill;
            base.ActiveControl = browserPanel;

            Newtabs(url);
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;

            //ContextMenuStrip cntextStrip = new ContextMenuStrip();
            //cntextStrip.Items.Add("Tab Close", null, Closeck);
            //cntextStrip.Items.Add(new ToolStripSeparator());
            //cntextStrip.Items.Add("Tab Reload", null, Reloadck);
            //cntextStrip.Items.Add(new ToolStripSeparator());
            //tabControl1.ContextMenuStrip = cntextStrip;
            //string[] array = new string[]{
            //    "500","400","300","200","175","150","140","120","110",
            //    "100","90","80","70","60","50","40","30","20","10"};

            //ToolStripMenuItem[] items = new ToolStripMenuItem[array.Length];
            //for (int i = 0; i < items.Length; i++)
            //{
            //    items[i] = new ToolStripMenuItem(array[i], null, Zoomck);

            //}

            //ToolStripMenuItem mnuItem = new ToolStripMenuItem("Zoom");
            //mnuItem.DropDownItems.AddRange(items);
            //cntextStrip.Items.Add(mnuItem);
            //cntextStrip.Items.Add(new ToolStripSeparator());
            string[] array = new string[]{
                "500","400","300","200","175","150","140","120","110",
                "100","90","80","70","60","50","40","30","20","10"};

            ToolStripMenuItem[] items = new ToolStripMenuItem[array.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new ToolStripMenuItem(array[i], null, Zoomck);

            }

            ToolStripMenuItem mnuItem = new ToolStripMenuItem("Zoom");
            mnuItem.DropDownItems.AddRange(items);
            tabControl1.ContextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    {
                        "Tab Close",(Image)null,(EventHandler)Closeck
                    },
                    (ToolStripItem)new ToolStripSeparator(),
                    {
                        "Tab Reload",(Image)null,(EventHandler)Reloadck
                    },
                    (ToolStripItem)new ToolStripSeparator(),
                    (ToolStripItem)mnuItem,
                    (ToolStripItem)new ToolStripSeparator()
                }
            };
            //tabControl1.ContextMenuStrip = contextMenuStrip;
        }
        private double Zoomlv(double d) => Math.Log(d / 100.0) / Math.Log(double.Parse("1.2"));

        //private double Zoomlv(double d)
        //{
        //	double lZoomLevel = Math.Log(double.Parse("1.2"));
        //	lZoomLevel = Math.Log(d / 100) / lZoomLevel;
        //	return lZoomLevel;
        //	return Math.Max(lZoomLevel,0);
        //}
        private void Zoomck(object sender, EventArgs e)
        {
            ZoomLevel = double.Parse(((ToolStripMenuItem)sender).Text.ToString());

            int i = tabControl1.SelectedIndex;
            if (BrowserTablist[i].ChromeBrowser != null) BrowserTablist[i].ChromeBrowser.SetZoomLevel(Zoomlv(ZoomLevel));
        }
        private void Closeck(object sender, EventArgs e)
        {
            int i = tabControl1.SelectedIndex;
            //IBrowser browser = BrowserTablist[i].ChromeBrowser != null?BrowserTablist[i].ChromeBrowser.GetBrowser():null;
            IBrowser browser = BrowserTablist[i].ChromeBrowser?.GetBrowser();
            if (tabControl1.TabCount > 1)
            {
                if (i != 0) tabControl1.SelectTab(i - 1);
                tabControl1.TabPages.Remove(BrowserTablist[i].Tab);
                if (browser != null) browser.CloseBrowser(true);
                BrowserTablist[i].Dispose();
                BrowserTablist.RemoveAt(i);
                if (tabControl1.TabCount == 1)
                {
                    tabControl1.ItemSize = new Size(0, 1);
                    tabControl1.SizeMode = TabSizeMode.Fixed;
                    tabControl1.Appearance = TabAppearance.FlatButtons;
                }
            }
            else
                Close();
        }
        private void Reloadck(object sender, EventArgs e)
        {
            int i = tabControl1.SelectedIndex;
            if (BrowserTablist[i].ChromeBrowser != null) BrowserTablist[i].ChromeBrowser.GetBrowser().Reload();
        }
        private void InitBrowser()
        {
            if (Cef.IsInitialized == false)
            {
                string dirx3264 = Path.Combine(AppDir,
#if tx64
                    "x64"
#else
                    "x86"
#endif
                    );
                CefSettings obj = new CefSettings
                {
                    Locale = "ja-JP",
                    UserAgent = UserAgent,
                    IgnoreCertificateErrors = true,
                    LogSeverity = LogSeverity.Disable,
                    UserDataPath = Path.Combine(AppDir, "UserData", AppNameWoExt),
                    LocalesDirPath = Path.Combine(dirx3264, "locales"),
                    ResourcesDirPath = dirx3264,
                    BrowserSubprocessPath = Path.Combine(dirx3264, "CefSharp.BrowserSubprocess.exe")
                    //BrowserSubprocessPath = AppPath;
                };
                obj.CachePath = obj.UserDataPath;
                string Stmp = Path.Combine(AppDir, "plugins", Flashdll);
                if (File.Exists(Stmp))
                {
                    obj.CefCommandLineArgs.Add("ppapi-flash-version", Flashversion);
                    obj.CefCommandLineArgs.Add("ppapi-flash-path", Stmp);
                }
                if (IsAudio == false)
                    obj.CefCommandLineArgs.Add("mute-audio");
                if (Proxyserver != "")
                {
                    obj.CefCommandLineArgs.Add("proxy-server", Proxyserver);
                }

                // Flashを有効化
                //				obj.CefCommandLineArgs.Add("enable-npapi", "1");
                //				obj.CefCommandLineArgs.Add("debug-plugin-loading", "1");
                //				obj.CefCommandLineArgs.Add("allow-outdated-plugins", "1");
                //				obj.CefCommandLineArgs.Add("always-authorize-plugins", "1");
                //				obj.CefCommandLineArgs.Add("disable-web-security", "1");
                //				obj.CefCommandLineArgs.Remove("enable-system-flash");
                //				obj.CefCommandLineArgs.Add("enable-system-flash", "1");
                //				obj.CefCommandLineArgs.Add("plugin-policy", "allow");
                //				obj.CefCommandLineArgs.Add("disable-plugins-discovery", "1");
                Cef.Initialize(obj);
            }
            IRequestContext contx = Cef.GetGlobalRequestContext();
            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                contx.SetPreference("profile.default_content_setting_values.plugins", 1, out string err);
            });
        }
        public class BrowserTab : IDisposable
        {
            public string Text { get; set; }
            public TabPage Tab { get; set; }
            public ChromiumWebBrowser ChromeBrowser { get; set; }
            public void Dispose()
            {
                Tab.Dispose();
                if (ChromeBrowser != null) ChromeBrowser.Dispose();
            }
            public BrowserTab(TabControl t, string s, bool Create = true)
            {
                Tab = new TabPage
                {
                    Text = TabTitle(s),
                    Dock = DockStyle.Fill
                };
                int num = Math.Max(Tab.Text.Length, 30);
                Tab.Text = Tab.Text.PadLeft(num);
                t.Controls.Add(Tab);
                t.SelectTab(t.TabCount - 1);
                if (t.TabCount == 1) { t.ItemSize = new Size(0, 1); }
                else if (t.TabCount == 2)
                {
                    t.ItemSize = new Size(30, 30);
                    t.SizeMode = TabSizeMode.FillToRight;
                    t.Appearance = TabAppearance.Normal;
                }
                ChromeBrowser = null;
                if (Create == false) return;
                ChromeBrowser = new ChromiumWebBrowser(s)
                {
                    Parent = Tab,
                    //Dock = DockStyle.Fill,
                    MenuHandler = new Handlers.ContextMenuHandler()
                };
            }
            private string TabTitle(string url)
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(new Uri(url).Query);
                string a = nameValueCollection["ServerId"];
                if (a != null && a.Length != 0 && url.IndexOf("sengokugifu") > 0)
                {
                    this.Text = "戦国義風";
                    return a + " " + nameValueCollection["nickname"];
                }
                a = nameValueCollection["server_id"];
                if (a != null && a.Length != 0 && url.IndexOf("ingame") > 0)
                {
                    if (url.IndexOf("dragonknight") > 0)
                    {
                        this.Text = "ドラグーン・ナイツ"; return a;
                    }
                    else
                    {
                        this.Text = "ドラゴンアウェイクン"; return a;
                    }
                }
                a = nameValueCollection["sid"];
                if (a != null && a.Length != 0)
                {
                    if (url.IndexOf("loas") > 0)
                    {
                        this.Text = "League of Angels 2"; return a;
                    }
                    else
                    if (url.IndexOf("loa3") > 0)
                    {
                        this.Text = "League of Angels 3"; return a;
                    }
                }
                a = nameValueCollection["site"];
                if (a != null && a.Length != 0)
                {
                    if (url.IndexOf("wef.brabragames") > 0)
                    {
                        this.Text = "World End Fantasy"; return a;
                    }
                }
                return this.Text = "gesobrowser";
            }

        }
        private void Newtabs(string url, bool Create = true)
        {
            if (!url.StartsWith("http"))
                return;
            BrowserTab browserTab = new BrowserTab(tabControl1, url, Create);
            this.Text = browserTab.Text;
            BrowserTablist.Add(browserTab);
            browserTab.ChromeBrowser.LifeSpanHandler = new Handlers.LifeSpanHandler(this);
            browserTab.ChromeBrowser.RequestHandler = new Handlers.RequestHandler(this);
            if (tabControl1.TabCount > 1) return;

            //BrowserSettings browserSettings = new BrowserSettings();
            //browserSettings.AcceptLanguageList = "ja_JP";
            //browserTab.chromeBrowser.BrowserSettings = browserSettings;

            //IKeyboardHandler keyboardHandler = new KeyboardHandler(this);
            //browserTab.chromeBrowser.KeyboardHandler = keyboardHandler;
            browserTab.ChromeBrowser.FrameLoadEnd += ChromeBrowserOnFrameLoadEnd;
            return;
        }
        //private string ReceiveString(ref Message m)
        //{
        //    string data;
        //    try
        //    {
        //        WinApi.COPYDATASTRUCT cds = (WinApi.COPYDATASTRUCT)m.GetLParam(typeof(WinApi.COPYDATASTRUCT));
        //        //WinApi.COPYDATASTRUCT cds = (WinApi.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.COPYDATASTRUCT));
        //        data = cds.lpData;
        //        data = data.Substring(0, cds.cbData / 2);
        //    }
        //    catch { data = null; }
        //    return data;
        //}
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_COPYDATA)//&& (int)m.WParam== WinApi.WM_USER1)
            {
                //string str = ReceiveString(ref m);
                //if (str != null)
                //    Newtabs(str);
                try
                {
                    WinApi.COPYDATASTRUCT cds = (WinApi.COPYDATASTRUCT)m.GetLParam(typeof(WinApi.COPYDATASTRUCT));
                    //WinApi.COPYDATASTRUCT cds = (WinApi.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.COPYDATASTRUCT));
                    string data = cds.lpData;
                    data = data.Substring(0, cds.cbData / 2);
                    Newtabs(data);
                }
                catch { }
                return;
            }
            base.WndProc(ref m);
        }
        private void ChromeBrowserOnFrameLoadEnd(object sender, EventArgs e)
        {
            BrowserTablist[0].ChromeBrowser.SetZoomLevel(Zoomlv(ZoomLevel));
            Process curProcess = Process.GetCurrentProcess();
            WinApi.SetWindowPlacement(curProcess.MainWindowHandle, ref Placement);
        }
        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage CurrentTab = tabControl1.TabPages[e.Index];
            Rectangle ItemRect = tabControl1.GetTabRect(e.Index);
            SolidBrush FillBrush = new SolidBrush(TabColor[0]);
            SolidBrush TextBrush = new SolidBrush(TabColor[1]);
            StringFormat sf = new StringFormat((StringFormatFlags)0, 0)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            if (Convert.ToBoolean(e.State & DrawItemState.Selected))
            {
                FillBrush.Color = TabColor[2];
                TextBrush.Color = TabColor[3];
                ItemRect.Inflate(2, 2);
            }
            //if (tabControl1.Alignment == TabAlignment.Left || tabControl1.Alignment == TabAlignment.Right)
            //{
            //    float RotateAngle = 90;
            //    if (tabControl1.Alignment == TabAlignment.Left)
            //        RotateAngle = 270;
            //    PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
            //    e.Graphics.TranslateTransform(cp.X, cp.Y);
            //    e.Graphics.RotateTransform(RotateAngle);
            //    ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
            //}
            e.Graphics.FillRectangle(FillBrush, ItemRect);
            e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);
            e.Graphics.ResetTransform();
            FillBrush.Dispose();
            TextBrush.Dispose();
        }
    }
 }
