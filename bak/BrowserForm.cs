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


namespace gesobrowser
{
    public partial class BrowserForm : Form
	{
		private static string UserAgent =
			//"Mozilla/5.0 (Linux; Android 7.1.2; huawei p20 lite Build/huawei p20; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/80.0.3987.149 Mobile Safari/537.36";
			"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.79 Safari/537.1";
		private string targetUrl, appPath, appDir, appName, appNameWoExt,a;
		private NameValueCollection nameValueCollection;
		public ChromiumWebBrowser chromeBrowser;
		private Panel browserPanel;

		private static BrowserForm Instance;
		public double defaultZoomLevel;
		private InifileUtils ini;

		private void MinToolStripMenuItem_Click(object sender, EventArgs e)
        {
			defaultZoomLevel -= 1;
			if (defaultZoomLevel <= -10.0)
			{
				defaultZoomLevel = -10.0;
			}

			chromeBrowser.SetZoomLevel(defaultZoomLevel);

		}

		private void MaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
			defaultZoomLevel += 1;
			if (defaultZoomLevel >= 10.0)
			{
				defaultZoomLevel = 10.0;
			}
			chromeBrowser.SetZoomLevel(defaultZoomLevel);

		}

        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
			DialogResult result = MessageBox.Show("ウィンドウを閉じますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				WinApi.RECT rc = new WinApi.RECT();
				WinApi.RECT rw = new WinApi.RECT();
				WinApi.GetWindowRect(this.Handle, ref rw);
				WinApi.GetClientRect(this.Handle, ref rc);
				int new_width = rc.right;// (rw.right - rw.left);// - (rc.right - rc.left);
				int new_height = rc.bottom;// (rw.bottom - rw.top);// - (rc.bottom - rc.top);
				string buf = $"{rw.left}, {rw.top}, {new_width}, {new_height}";
				ini.setValue("Profile", "Window", buf);
            }
            else {
				e.Cancel = true;

			}
		}

        private ToolStripMenuItem MaxToolStripMenuItem, MinToolStripMenuItem;
		private MenuStrip menuStrip1;
		public BrowserForm()
        {
//            InitializeComponent();
//			InitBrowser();
		}
        public BrowserForm(string url)
        {
			appPath = System.Windows.Forms.Application.ExecutablePath;
			//ディレクトリ
			appDir = System.IO.Path.GetDirectoryName(appPath);
			//ファイル名
			appName = System.IO.Path.GetFileName(appPath);
			//ファイル名(拡張子含まず)
			appNameWoExt = System.IO.Path.ChangeExtension(appName, null);
			//	String appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			System.IO.Directory.SetCurrentDirectory(appDir);

			string icofile = appNameWoExt + ".ico";
			if (File.Exists(icofile))
				this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(icofile);
			InitializeComponent();

			nameValueCollection = HttpUtility.ParseQueryString(new Uri(url).Query);
			//			base.WindowState = FormWindowState.Maximized;
			a = nameValueCollection["ServerId"];
			if (a != null && a.Length != 0 && url.IndexOf("sengokugifu") > 0)
			{
				this.Text = "戦国義風 " + a + " " + nameValueCollection["nickname"];
			}
			else
			{
				a = nameValueCollection["sid"];
				if (a != null && a.Length != 0)
				{
					if (url.IndexOf("loas") > 0)
					{
						this.Text = "League of Angels2 " + a;
					}
					else
					if (url.IndexOf("loa3") > 0)
					{
						this.Text = "League of Angels3 " + a;
					}
					else
						this.Text = "gesobrowser";
				}
				else
				{
					//a = nameValueCollection["server"];
					if (url.IndexOf("//wef.") > 0)
					{
						this.Text = "World End Fantasy " + a;
					}
					else
						this.Text = "gesobrowser";
				}
			}
			targetUrl = url;
			base.ActiveControl = browserPanel;
			InitBrowser();
		}
		private void InitBrowser()
		{
			Instance = this;
			if (Cef.IsInitialized == false)
			{
				CefSettings obj = new CefSettings();
				string dllpp = Path.Combine(appDir, "plugins", "pepflashplayer.dll");
				if (File.Exists(dllpp)) {
					obj.CefCommandLineArgs.Add("ppapi-flash-version", "32.0.0.453");
					obj.CefCommandLineArgs.Add("ppapi-flash-path", dllpp);
				}
				obj.Locale = "ja-JP";
				obj.UserAgent = UserAgent;
				obj.IgnoreCertificateErrors = true;
				obj.LogSeverity = LogSeverity.Disable;
				obj.UserDataPath =
				obj.CachePath = Path.Combine(appDir, appNameWoExt + "_UserData");
				//				obj.CefCommandLineArgs.Add("disable-gpu", "0");
				obj.CefCommandLineArgs.Add("mute-audio");

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
			chromeBrowser = new ChromiumWebBrowser(targetUrl);
			chromeBrowser.LifeSpanHandler = new LifeSpanHandler(this);
			BrowserSettings browserSettings = new BrowserSettings();
			browserSettings.AcceptLanguageList = "ja_JP";
			chromeBrowser.BrowserSettings = browserSettings;
			browserPanel.Controls.Add(chromeBrowser);
			chromeBrowser.Dock = DockStyle.Fill;
			//			IKeyboardHandler keyboardHandler = new KeyboardHandler(this);
			//			chromeBrowser.KeyboardHandler = keyboardHandler;


			ini = new InifileUtils(Path.Combine(appDir, appNameWoExt + ".ini"));
			string buf = ini.getValueString("Profile", "Window");

			if (buf != "")
			{
				int x, y, w, h;
				var fields = buf.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				x = int.Parse(fields[0]);
				y = int.Parse(fields[1]);
				w = int.Parse(fields[2]);
				h = int.Parse(fields[2]);
				//			sscanf(buf, "%d,%d,%d,%d", &x, &y, &w, &h);
				//WinApi.MoveWindow(this.Handle, x, y, 0, 0, 1);
				//				this.Size = new System.Drawing.Size(w, h);
				//this.Width = w;
				//this.Height = h;
//				const uint SWP_NOSIZE = 0x0001;
//				const uint SWP_NOMOVE = 0x0002;

				WinApi.SetWindowPos(this.Handle, new IntPtr(0), x, y, 0, 0,1);
				this.ClientSize = new System.Drawing.Size(w, h);
			}
			this.WindowState = System.Windows.Forms.FormWindowState.Normal;
			this.ShowInTaskbar = true;
		}
	}
}
