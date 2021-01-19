using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Principal;
using System.Linq;
using System.Text;
namespace gesobrowser
{
	public partial class BrowserForm : Form
	{
		private string
			Flashdll = "pepflashplayer.dll",
			Flashversion = "32.0.0.453",
			UserAgent =
			//"Mozilla/5.0 (Linux; Android 7.1.2; huawei p20 lite Build/huawei p20; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/80.0.3987.149 Mobile Safari/537.36";
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";

		private string AppPath, AppDir, AppName, AppNameWoExt, Proxyserver;

		//		public ChromiumWebBrowser chromeBrowser;

//		private BrowserForm Instance;
		private bool isAudio = false;
		public bool isDomain = false;
		private double ZoomLevel = 100;
		private InifileUtils Ini;
		private WinApi.WINDOWPLACEMENT Placement;
		private DialogResult DialogCloseShow = DialogResult.OK;
		private CefLibraryHandle LibraryLoader;
		public IList<BrowserTab> BrowserTablist = new List<BrowserTab>();
		private Color[] TabColor;
		private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
			DialogResult chk = DialogResult.OK;
			if (DialogCloseShow == DialogResult.OK) {
				chk = MessageBox.Show("ウィンドウを閉じますか？", "質問", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			}
			if (chk == DialogResult.OK)
			{
				//				WinApi.RECT rc = new WinApi.RECT();
				//				WinApi.RECT rw = new WinApi.RECT();
				//				WinApi.GetWindowRect(this.Handle, ref rw);
				//				WinApi.GetClientRect(this.Handle, ref rc);
				//				int new_width = rc.right;// (rw.right - rw.left);// - (rc.right - rc.left);
				//				int new_height = rc.bottom;// (rw.bottom - rw.top);// - (rc.bottom - rc.top);
				//				var hwnd = new WindowInteropHelper(this).Handle;
				WinApi.GetWindowPlacement(this.Handle, out Placement);
				//				int height = placement.normalPosition.bottom - placement.normalPosition.top;
				//int height = placement.normalPosition.bottom;
				//				string buf = $"{rw.left}, {rw.top}, {new_width}, {new_height}";
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
			else {
				e.Cancel = true;

			}
		}
		/*
				private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
				{
					ZoomLevel = double.Parse(comboBox1.SelectedItem.ToString());
		//            if (defaultZoomLevel>0) {
						ZoomLevel=ZoomLevel / 10;
					//			}else if (defaultZoomLevel < 0) {
					//				defaultZoomLevel = defaultZoomLevel * 10 - 1;
					//			}
		//			TabPage tab=tabControl1.SelectedTab;
					int i = tabControl1.SelectedIndex;
					BrowserTablist[i].chromeBrowser.SetZoomLevel(ZoomLevel);

				}
		*/
		//		public BrowserForm()
		//        {
		//            InitializeComponent();
		//			InitBrowser();
		//		}
		public BrowserForm(string url)
		{
			AppPath = System.Windows.Forms.Application.ExecutablePath;
			//ディレクトリ
			AppDir = System.IO.Path.GetDirectoryName(AppPath);
			//ファイル名
			AppName = System.IO.Path.GetFileName(AppPath);
			//ファイル名(拡張子含まず)
			AppNameWoExt = System.IO.Path.ChangeExtension(AppName, null);
			InitializeComponent();
			//			Process prevProcess = GetPreviousProcess();
			//			IntPtr he = this.Handle;

			//	String appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			System.IO.Directory.SetCurrentDirectory(AppDir);

			string icofile = AppNameWoExt + ".ico";

			if (File.Exists(icofile))
			{
				this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(icofile);
			}
			this.Text = AppNameWoExt;
			Ini = new InifileUtils(Path.Combine(AppDir, AppNameWoExt + ".ini"));
			string buf = Ini.GetValueString("Profile", "Window");

			if (buf != "")
			{
				int x, y, w, h;
				var fields = buf.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				x = int.Parse(fields[0]);
				y = int.Parse(fields[1]);
				w = int.Parse(fields[2]);
				h = int.Parse(fields[3]);
				//			sscanf(buf, "%d,%d,%d,%d", &x, &y, &w, &h);
				//WinApi.MoveWindow(this.Handle, x, y, 0, 0, 1);
				//				this.Size = new System.Drawing.Size(w, h);
				//this.Width = w;
				//this.Height = h;
				//				const uint SWP_NOSIZE = 0x0001;
				//				const uint SWP_NOMOVE = 0x0002;

				//				WinApi.SetWindowPos(this.Handle, new IntPtr(0), x, y, 0, 0, 1);
				//				this.ClientSize = new System.Drawing.Size(w, h);

				WinApi.GetWindowPlacement(this.Handle, out Placement);
				//placement.length = Marshal.SizeOf(typeof(WinApi.WINDOWPLACEMENT));
				Placement.normalPosition.left = x;
				Placement.normalPosition.top = y;
				Placement.normalPosition.right = w;
				Placement.normalPosition.bottom = h;
				Placement.showCmd = WinApi.SW.SHOWNORMAL;
				//				WinApi.SetWindowPlacement(this.Handle, ref placement);
			}
			buf = Ini.GetValueString("Profile", "ZoomLevel");
			if (buf != "")
			{
				ZoomLevel = double.Parse(buf);
			}
			buf = Ini.GetValueString("Profile", "audio");
			if (buf != "")
			{
				isAudio = bool.Parse(buf);
			}
			buf = Ini.GetValueString("Profile", "domain");
			if (buf != "")
			{
				isDomain = bool.Parse(buf);
			}
			buf = Ini.GetValueString("Profile", "DialogCloseShow");
			if (buf != "")
			{
				DialogCloseShow = (DialogResult)int.Parse(buf);
			}
			buf = Ini.GetValueString("Profile", "UserAgent");
			if (buf != "")
			{
				UserAgent = buf;
			}
			Proxyserver = Ini.GetValueString("Profile", "proxy");
			buf = Ini.GetValueString("Profile", "flashdll");
			if (buf != "")
			{
				Flashdll = buf;
			}
			buf = Ini.GetValueString("Profile", "flashversion");
			if (buf != "")
			{
				Flashversion = buf;
			}
			TabColor = new Color[4] { SystemColors.Control,Color.Black,Color.LightBlue, Color.Black};
			for (int i = 0; i < 4; i++)
			{
				buf = Ini.GetValueString("Profile", "TabColor" + (i + 1)) ;
				if (buf != "")
				{
					TabColor[i] = ColorTranslator.FromHtml(buf);
				}
			}
			//			TargetUrl = url;

			LibraryLoader = new CefLibraryHandle(Path.Combine(AppDir, 
#if tx64
        "x64"
#else
        "x86"
#endif
, "libcef.dll"));
			if (LibraryLoader.IsInvalid) { throw new Exception("no lib"); }

			InitBrowser();
			browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			base.ActiveControl = browserPanel;

			Newtabs(url);
			this.WindowState = System.Windows.Forms.FormWindowState.Normal;
			this.ShowInTaskbar = true;

			ContextMenuStrip cntextStrip = new ContextMenuStrip();
			cntextStrip.Items.Add("Tab Close", null, Closeck);
			cntextStrip.Items.Add(new ToolStripSeparator());
			cntextStrip.Items.Add("Tab Reload", null, Reloadck);
			cntextStrip.Items.Add(new ToolStripSeparator());
			tabControl1.ContextMenuStrip = cntextStrip;
//			ToolStripItem cmbBox = new ToolStripItem();
//			ToolStripItem[] cmbBox;// =new ToolStripItem[];
			string[] array = new string[]//();
//			this.comboBox1.Items.AddRange(new object[]
			{
				"500","400","300","200","175","150","140","120","110",
				"100","90","80","70","60","50","40","30","20","10"};
//				,"0","-10","-20","-30","-40","-50","-60","-70","-80","-90","-100"};
//			var lookup = array.ToLookup(cmbBox.Add(array));

			ToolStripMenuItem[] items = new ToolStripMenuItem[array.Length]; // You would obviously calculate this value at runtime
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = new ToolStripMenuItem(array[i],null, Zoomck);

			}

			ToolStripMenuItem mnuItem = new ToolStripMenuItem("Zoom");
			mnuItem.DropDownItems.AddRange(items);

			//			cntextStrip.Items.Add("Zoom", null, null);
			//			mnuItem.DropDownItems.AddRange(cmbBox);
			cntextStrip.Items.Add(mnuItem);
			cntextStrip.Items.Add(new ToolStripSeparator());
		}
		private double Zoomlv(double d) => Math.Log(d / 100.0) / Math.Log(double.Parse("1.2"));

//		private double Zoomlv2(double d)
//		{
//			double lZoomLevel = Math.Log(double.Parse("1.2"));
//			lZoomLevel = Math.Log(d / 100) / lZoomLevel;
//			return lZoomLevel;
//			return Math.Max(lZoomLevel,0);
//		}
		private void Zoomck(object sender, EventArgs e) {
//			ToolStripMenuItem x = (ToolStripMenuItem)sender;
			ZoomLevel = double.Parse(((ToolStripMenuItem)sender).Text.ToString());

			int i = tabControl1.SelectedIndex;
			if (BrowserTablist[i].ChromeBrowser != null) BrowserTablist[i].ChromeBrowser.SetZoomLevel(Zoomlv(ZoomLevel));
		}
		private void Closeck(object sender, EventArgs e)
		{
			int i = tabControl1.SelectedIndex;
//			IBrowser browser = BrowserTablist[i].ChromeBrowser != null?BrowserTablist[i].ChromeBrowser.GetBrowser():null;
			IBrowser browser = BrowserTablist[i].ChromeBrowser?.GetBrowser(); 
			if (tabControl1.TabCount > 1)
			{
				if(i!=0) tabControl1.SelectTab(i - 1);
				tabControl1.TabPages.Remove(BrowserTablist[i].Tab);
				if (browser != null) browser.CloseBrowser(true);
				BrowserTablist[i].Dispose();
				BrowserTablist.RemoveAt(i);
				if (tabControl1.TabCount == 1) { tabControl1.ItemSize = new Size(0, 1);
					tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
					tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
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
//		public delegate void EventHandler(object sender, EventArgs e);
		private void InitBrowser()
		{
//			Instance = this;
			if (Cef.IsInitialized == false)
			{
				CefSettings obj = new CefSettings();
				string Stmp = Path.Combine(AppDir, "plugins", Flashdll);
				if (File.Exists(Stmp)) {
					obj.CefCommandLineArgs.Add("ppapi-flash-version", Flashversion);
					obj.CefCommandLineArgs.Add("ppapi-flash-path", Stmp);
				}
				obj.Locale = "ja-JP";
				obj.UserAgent = UserAgent;
				obj.IgnoreCertificateErrors = true;
				obj.LogSeverity = LogSeverity.Disable;
				//				a = Path.Combine(appDir, appNameWoExt + "_UserData");
//				Stmp = Path.Combine(AppDir, "UserData", AppNameWoExt);
				obj.UserDataPath = Path.Combine(AppDir, "UserData", AppNameWoExt);
				obj.CachePath= obj.UserDataPath;
				//				obj.CefCommandLineArgs.Add("disable-gpu", "0");
				if(isAudio == false)
					obj.CefCommandLineArgs.Add("mute-audio");

				Stmp = Path.Combine(AppDir, 
#if tx64
        "x64"
#else
        "x86"
#endif
, "CefSharp.BrowserSubprocess.exe");
				//				if (!File.Exists(dllpp)) { throw new Exception("no lib"); }
				//				a = Path.Combine(appDir, "x64", appNameWoExt+"Subprocess.exe");
				//				if (!File.Exists(a)) {
				//					File.Copy(dllpp, a, false);
				/*
				var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				bool ret=principal.IsInRole(WindowsBuiltInRole.Administrator);
				if (!ret) {
//						var assembly = Assembly.GetEntryAssembly();
					string[] s = new string[] { targetUrl };
					var startInfo = new ProcessStartInfo(appPath, ToCommandArgs(s))
					{
						UseShellExecute = true,
						Verb = "runas",
					};

					try
					{
						Process.Start(startInfo);
					}
					catch (Win32Exception ex)
					{
						throw new Exception("no lib");
					}
				}*/
				//					WinApi.CreateSymbolicLink(a, dllpp, WinApi.SymbolicLink.File);
				//					uint error = WinApi.GetLastError();
				//					uint ERROR_PRIVILEGE_NOT_HELD = 1314;
				//					if (error == ERROR_PRIVILEGE_NOT_HELD)
				//					{
				//						const string SE_CREATE_SYMBOLIC_LINK_NAME = "SeCreateSymbolicLinkPrivilege";
				//						RunAsAdministrator(SE_CREATE_SYMBOLIC_LINK_NAME);
				//						WinApi.CreateSymbolicLink(a, dllpp, WinApi.SymbolicLink.File);
				//						error = WinApi.GetLastError();
				//					}
				//				}
				//				if (!File.Exists(Stmp)) { throw new Exception("no lib"); }
				obj.BrowserSubprocessPath = Stmp;
				obj.LocalesDirPath = Path.Combine(AppDir, 
#if tx64
        "x64"
#else
        "x86"
#endif
, "locales");
				obj.ResourcesDirPath = Path.Combine(AppDir, 
#if tx64
        "x64"
#else
        "x86"
#endif
);
				if (Proxyserver != "")
				{
					obj.CefCommandLineArgs.Add("proxy-server", Proxyserver);
				}
				
				//				obj.SetOffScreenRenderingBestPerformanceArgs();
				// Flashを有効化
				//				obj.CefCommandLineArgs.Add("enable-npapi", "1");
				//				obj.CefCommandLineArgs.Add("ppapi-flash-path", "pepflashplayer.dll");
				//				obj.CefCommandLineArgs.Add("ppapi-flash-version", "28.0.0.137");
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
			//			var cookieManager = Cef.GetGlobalCookieManager();
			//			cookieManager.SetStoragePath("cookies", true);
			var contx = Cef.GetGlobalRequestContext();
			Cef.UIThreadTaskFactory.StartNew(delegate {
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
//				chromeBrowser.GetBrowser().CloseBrowser(true);
				if(ChromeBrowser != null) ChromeBrowser.Dispose();
			}
			//static string Center(string s, int width)
			//{
			//	var margin = (width - s.Length) / 2;

			//	if (margin <= 0)
			//	{
			//		return s;
			//	}
			//	else
			//	{
			//		var padding = new String(' ', margin);

			//		return String.Concat(padding, s, padding);
			//	}
			//}
			public BrowserTab(TabControl t, string s, bool Create = true)
			{
                Tab = new TabPage
                {
                    Text = TabTitle(s),
                    Dock = System.Windows.Forms.DockStyle.Fill
                };
				int num = Math.Max(Tab.Text.Length, 30);
				//				StringBuilder sb = new StringBuilder(num+1);
				//char[] buf = new char[num + 1];
				//int i;
				//for (i = 0; i < num; i++)
				//{
				//	buf[i] = ' ';
				//	//sb.Append(" ");
				//}
				//buf[i] = char.MinValue;
				//Tab.Text += new string(buf);//string.Join("",buf);
				//				Tab.Text += sb.ToString();
				//Tab.Text = Center(Tab.Text, Tab.Text.Length+30);
				Tab.Text = Tab.Text.PadLeft(num);
				t.Controls.Add(Tab);
				t.SelectTab(t.TabCount - 1);
				if (t.TabCount == 1) { t.ItemSize = new Size(0, 1); }
				else if (t.TabCount == 2) {
					//					int w = Math.Max(tab.Text.Length, 30);
					//					Size si = Instance.ClientSize;
					//					w = Math.Max((s.Width / t.TabCount) / 4, w);
					//					t.ItemSize = new Size(si.Width, 30);
					t.ItemSize = new Size(30, 30);
					t.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
					t.Appearance = System.Windows.Forms.TabAppearance.Normal;
				}
				//                else
				//                {
				//					int w = Math.Max(tab.Text.Length, 30);
				//					Size si = Instance.ClientSize;
				//					w = Math.Max((si.Width / t.TabCount) / 4, w);
				//					t.ItemSize = new Size(w, 30);
				//				}
				ChromeBrowser = null;
				if (Create == false) return;
                ChromeBrowser = new ChromiumWebBrowser(s)
                {
                    Parent = Tab,
                    //Dock = DockStyle.Fill,
                    MenuHandler = new Handlers.ContextMenuHandler()
                };
                //				chromeBrowser.LifeSpanHandler = new Handlers.LifeSpanHandler(this);
            }
   //         public class WebBrowserl : WebBrowser, IDisposable
			//{

			//	public WebBrowserl(string s):base() {
			//		base.Navigate(s);
			//		FrameLoadEnd += ChromeBrowserOnFrameLoadEnd;
			//		base.DocumentCompleted += ChromeBrowserOnFrameLoadEnd;
			//	}
			//	public WebBrowserl GetBrowser() { return this; }
			//	public void CloseBrowser(bool a) { base.Dispose(); }
			//	public IContextMenuHandler MenuHandler { get; set; }
			//	public ILifeSpanHandler LifeSpanHandler { get; set; }
			//	//public FrameLoadEndEventArgs FrameLoadEnd;
			//	//{
			//	//	public static FrameLoadEnd operator +(FrameLoadEnd z, FrameLoadEnd w)
			//	//	{
			//	//		return z;
			//	//	}
			//	//}
			//}
			private string TabTitle(string url)
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(new Uri(url).Query);
				//			base.WindowState = FormWindowState.Maximized;
				string a = nameValueCollection["ServerId"];
				if (a != null && a.Length != 0 && url.IndexOf("sengokugifu") > 0)
				{
					this.Text = "戦国義風";
					return a + " " + nameValueCollection["nickname"];
					//				return "戦国義風 " + a + " " + nameValueCollection["nickname"];
				}
				a = nameValueCollection["server_id"];
				if (a != null && a.Length != 0 && url.IndexOf("ingame") > 0)
				{
					this.Text = "ドラゴンアウェイクン"; return a;
//					return "ドラゴンアウェイクン " + a;
				}
				a = nameValueCollection["sid"];
				if (a != null && a.Length != 0)
				{
					if (url.IndexOf("loas") > 0)
					{
						this.Text = "League of Angels 2"; return a;
//						return "League of Angels2 " + a;
					}
					else
					if (url.IndexOf("loa3") > 0)
					{
						this.Text = "League of Angels 3"; return a;
//						return "League of Angels3 " + a;
					}
				}
				a = nameValueCollection["site"];
				if (a != null && a.Length != 0)
				{
					//a = nameValueCollection["site"];
					if (url.IndexOf("wef.brabragames") > 0)
					{
						this.Text = "World End Fantasy"; return a;
//						return "World End Fantasy " + a;
					}
				}
				//if (url.IndexOf("//wef.") > 0)
				//{
				//	return this.Text = "World End Fantasy ";
				//}
				return this.Text = "gesobrowser";
			}

		}
			private void Newtabs(string url, bool Create = true)
		{
			if (!url.StartsWith("http"))
				return;
            BrowserTab browserTab = new BrowserTab(tabControl1, url, Create);
			this.Text = browserTab.Text;
			//            BrowserTab tab = browserTab;
			BrowserTablist.Add(browserTab);

			//			TabPage tab = new TabPage();
			//			tab.Text = Title(url);
			//			tab.Dock = System.Windows.Forms.DockStyle.Fill;
			//			tabControl1.Controls.Add(tab);
			//			tabControl1.SelectTab(tabControl1.TabCount - 1);

			//			tab.chromeBrowser = new ChromiumWebBrowser(url);
			//			tab.chromeBrowser.Parent = tab.tab;
			//			tab.chromeBrowser.Dock = DockStyle.Fill;
			//			tab.chromeBrowser.MenuHandler = new CustomMenuHandler();
			//			chromeBrowser.SetZoomLevel(ZoomLevel);
			browserTab.ChromeBrowser.LifeSpanHandler = new Handlers.LifeSpanHandler(this);
			browserTab.ChromeBrowser.RequestHandler = new Handlers.RequestHandler(this);
			if (tabControl1.TabCount > 1) return;

			//			BrowserSettings browserSettings = new BrowserSettings();
			//			browserSettings.AcceptLanguageList = "ja_JP";
			//			tab.chromeBrowser.BrowserSettings = browserSettings;
			//			browserPanel.Controls.Add(chromeBrowser);

			//			IKeyboardHandler keyboardHandler = new KeyboardHandler(this);
			//			chromeBrowser.KeyboardHandler = keyboardHandler;
			browserTab.ChromeBrowser.FrameLoadEnd += ChromeBrowserOnFrameLoadEnd;
			return;
		}
		private string ReceiveString(ref Message m)//IntPtr lParam)
		{
			string data;
			try
			{
				WinApi.COPYDATASTRUCT cds = (WinApi.COPYDATASTRUCT)m.GetLParam(typeof(WinApi.COPYDATASTRUCT));
				//WinApi.COPYDATASTRUCT cds = (WinApi.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.COPYDATASTRUCT));
				data = cds.lpData;
				data = data.Substring(0, cds.cbData / 2);
			}
			catch { data = null; }
			return data;
		}
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WinApi.WM_COPYDATA)//&& (int)m.WParam== WinApi.WM_USER1)
			{
				string str = ReceiveString(ref m); 
				if(str!=null)
					Newtabs(str);
			}
			base.WndProc(ref m);
		}
		//		static readonly Func<string, string> EscapeCommandArg = x => x.Contains(' ') ? string.Format("\"{0}\"", x) : x;
		//		static readonly Func<string[], string> ToCommandArgs = x => string.Join(" ", x.Select(EscapeCommandArg));
		/*
		private bool RunAsAdministrator(string lpPrivilegeName)
		{
			IntPtr hToken;
			WinApi.TOKEN_PRIVILEGES tokenPrivileges=new WinApi.TOKEN_PRIVILEGES();
			bool bRet;

			const uint TOKEN_ADJUST_PRIVILEGES = 0x20;
			const uint TOKEN_QUERY = 0x8;
			const int SE_PRIVILEGE_ENABLED = 0x2;

			if (!(bRet = WinApi.OpenProcessToken(WinApi.GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out hToken)))
			{
				return false;
			}
			//LUIDを取得する
			WinApi.TOKEN_PRIVILEGES tp = new WinApi.TOKEN_PRIVILEGES();
			tp.Attributes = SE_PRIVILEGE_ENABLED;
			tp.PrivilegeCount = 1;
			if (WinApi.LookupPrivilegeValue(null, lpPrivilegeName, out tp.Luid)){
				//特権を有効にする
				WinApi.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
				bRet = WinApi.GetLastError()==0;
			}
			WinApi.CloseHandle(hToken);

			return bRet;



			//1.OpenProcessToken関数で、プロセストークンを取得する
			bRet = WinApi.OpenProcessToken(WinApi.GetCurrentProcess(),TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,out hToken);
			if (!bRet)
			{
				return false;
			}

			//2.LookupPrivilegeValue関数で、特権に対応するLUID(ローカル一意識別子)を取得する
			bRet = WinApi.LookupPrivilegeValue(null, lpPrivilegeName, out tokenPrivileges.Luid);
			if (bRet)
			{

				//3.TOKEN_PRIVILEGES型のオブジェクトに、LUID(ローカル一意識別子)と特権の属性(有効にするか無効にするか)を指定する
				tokenPrivileges.PrivilegeCount = 1;
				tokenPrivileges.Attributes = SE_PRIVILEGE_ENABLED;

				//4.AdjustTokenPrivileges関数で、特権を有効にする
				WinApi.AdjustTokenPrivileges(hToken,
					false,
					ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);

				bRet = WinApi.GetLastError() == 0;

			}
			WinApi.CloseHandle(hToken);
			return bRet;
		}*/
//		public bool tabControl1_Remove()
//		{
//			if (tabControl1.TabCount > 1)
//			{
//				tabControl1.TabPages.Remove(tabControl1.SelectedTab);
//				return true;
//			}
//			return false;
//		}
		private void ChromeBrowserOnFrameLoadEnd(object sender, EventArgs e)
		{
			//			var chromiumBrowser = sender as ChromiumWebBrowser;
			//			chromiumBrowser?.ShowDevTools();
//			int i = tabControl1.SelectedIndex;
			BrowserTablist[0].ChromeBrowser.SetZoomLevel(Zoomlv(ZoomLevel));
			Process curProcess = Process.GetCurrentProcess();
			WinApi.SetWindowPlacement(curProcess.MainWindowHandle, ref Placement);
		}
		private void TabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
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

            //If we are currently painting the Selected TabItem we'll
            //change the brush colors and inflate the rectangle.
            if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
			{
				FillBrush.Color = TabColor[2];
				TextBrush.Color = TabColor[3];
				ItemRect.Inflate(2, 2);
			}

			//Set up rotation for left and right aligned tabs
			if (tabControl1.Alignment == TabAlignment.Left || tabControl1.Alignment == TabAlignment.Right)
			{
				float RotateAngle = 90;
				if (tabControl1.Alignment == TabAlignment.Left)
					RotateAngle = 270;
				PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
				e.Graphics.TranslateTransform(cp.X, cp.Y);
				e.Graphics.RotateTransform(RotateAngle);
				ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
			}

			//Next we'll paint the TabItem with our Fill Brush
			e.Graphics.FillRectangle(FillBrush, ItemRect);

			//Now draw the text.
			e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);

			//Reset any Graphics rotation
			e.Graphics.ResetTransform();

			//Finally, we should Dispose of our brushes.
			FillBrush.Dispose();
			TextBrush.Dispose();
		}

	}
	/*
	public class VisualTabControl : TabControl
	{
		public VisualTabControl()
			: base()
		{
			//Paintイベントで描画できるようにする
			this.SetStyle(ControlStyles.UserPaint, true);
			//ダブルバッファリングを有効にする
			this.DoubleBuffered = true;
			//this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			////this.SetStyle(ControlStyles.DoubleBuffer, true);
			//リサイズで再描画する
			this.ResizeRedraw = true;
			//this.SetStyle(ControlStyles.ResizeRedraw, true);

			//ControlStyles.UserPaintをTrueすると、
			//SizeModeは強制的にTabSizeMode.Fixedにされる
			this.SizeMode = TabSizeMode.Fixed;
			this.ItemSize = new Size(30, 30);
			this.Appearance = TabAppearance.Normal;
			this.Multiline = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//TabControlの背景を塗る
			e.Graphics.FillRectangle(SystemBrushes.Control, this.ClientRectangle);

			if (this.TabPages.Count <= 1)
				return;

			//TabPageの枠を描画する
			TabPage page = this.TabPages[this.SelectedIndex];
			Rectangle pageRect = new Rectangle(
				page.Bounds.X - 2,
				page.Bounds.Y - 2,
				page.Bounds.Width + 5,
				page.Bounds.Height + 5);
			TabRenderer.DrawTabPage(e.Graphics, pageRect);

			//タブを描画する
			for (int i = 0; i < this.TabPages.Count; i++)
			{
				page = this.TabPages[i];
				Rectangle tabRect = this.GetTabRect(i);
				if (tabRect.Width == 0 || tabRect.Height == 0)
					continue;
				//表示するタブの状態を決定する
				System.Windows.Forms.VisualStyles.TabItemState state;
				if (!this.Enabled)
				{
					state = System.Windows.Forms.VisualStyles.TabItemState.Disabled;
				}
				else if (this.SelectedIndex == i)
				{
					state = System.Windows.Forms.VisualStyles.TabItemState.Selected;
					e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), e.ClipRectangle);
				}
				else
				{
					state = System.Windows.Forms.VisualStyles.TabItemState.Normal;
					e.Graphics.FillRectangle(new SolidBrush(Control.DefaultBackColor), e.ClipRectangle);
				}

				//選択されたタブとページの間の境界線を消すために、
				//描画する範囲を大きくする
				if (this.SelectedIndex == i)
				{
					if (this.Alignment == TabAlignment.Top)
					{
						tabRect.Height += 1;
					}
					else if (this.Alignment == TabAlignment.Bottom)
					{
						tabRect.Y -= 2;
						tabRect.Height += 2;
					}
					else if (this.Alignment == TabAlignment.Left)
					{
						tabRect.Width += 1;
					}
					else if (this.Alignment == TabAlignment.Right)
					{
						tabRect.X -= 2;
						tabRect.Width += 2;
					}
				}

				//画像のサイズを決定する
				Size imgSize;
				if (this.Alignment == TabAlignment.Left ||
					this.Alignment == TabAlignment.Right)
				{
					imgSize = new Size(tabRect.Height, tabRect.Width);
				}
				else
				{
					imgSize = tabRect.Size;
				}

				//Bottomの時はTextを表示しない（Textを回転させないため）
				string tabText = page.Text;
				if (this.Alignment == TabAlignment.Bottom)
				{
					tabText = "";
				}

				//タブの画像を作成する
				Bitmap bmp = new Bitmap(imgSize.Width, imgSize.Height);
				Graphics g = Graphics.FromImage(bmp);
				//高さに1足しているのは、下にできる空白部分を消すため
				TabRenderer.DrawTabItem(g,
					new Rectangle(0, 0, bmp.Width, bmp.Height + 1),
					tabText,
					page.Font,
					false,
					state);
				g.Dispose();

				//画像を回転する
				if (this.Alignment == TabAlignment.Bottom)
				{
					bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
				}
				else if (this.Alignment == TabAlignment.Left)
				{
					bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
				}
				else if (this.Alignment == TabAlignment.Right)
				{
					bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
				}

				//Bottomの時はTextを描画する
				if (this.Alignment == TabAlignment.Bottom)
				{
					StringFormat sf = new StringFormat();
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					g = Graphics.FromImage(bmp);
					g.DrawString(page.Text,
						page.Font,
						SystemBrushes.ControlText,
						new RectangleF(0, 0, bmp.Width, bmp.Height),
						sf);
					g.Dispose();
					sf.Dispose();
				}

				//画像を描画する
				e.Graphics.DrawImage(bmp, tabRect.X, tabRect.Y, bmp.Width, bmp.Height);

				bmp.Dispose();
			}
		}
	}*/
}
