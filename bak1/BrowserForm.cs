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

namespace gesobrowser
{
    public partial class BrowserForm : Form
	{
		private static string UserAgent =
			//"Mozilla/5.0 (Linux; Android 7.1.2; huawei p20 lite Build/huawei p20; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/80.0.3987.149 Mobile Safari/537.36";
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";
		private string targetUrl, appPath, appDir, appName, appNameWoExt,a, proxyserver,
			flashdll= "pepflashplayer.dll",
			flashversion= "32.0.0.453";
		public ChromiumWebBrowser chromeBrowser;
		private Panel browserPanel;

		//		private static BrowserForm Instance;
		private bool audio = false;
		private double ZoomLevel=0;
		private InifileUtils ini;
		private WinApi.WINDOWPLACEMENT placement;
		private DialogResult DialogCloseShow = DialogResult.OK;
		private CefLibraryHandle libraryLoader;

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
				WinApi.GetWindowPlacement(this.Handle, out placement);
				//				int height = placement.normalPosition.bottom - placement.normalPosition.top;
				//int height = placement.normalPosition.bottom;
				//				string buf = $"{rw.left}, {rw.top}, {new_width}, {new_height}";
				string buf = $"{placement.normalPosition.left}, {placement.normalPosition.top}, {placement.normalPosition.right}, {placement.normalPosition.bottom}";
				if (placement.normalPosition.left >= 0)
				{ ini.setValue("Profile", "Window", buf); }
				ini.setValue("Profile", "ZoomLevel", ZoomLevel.ToString());
				ini.setValue("Profile", "DialogCloseShow", (int)DialogCloseShow);
				ini.setValue("Profile", "UserAgent", UserAgent);
				buf = $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}";
				ini.setValue("Profile", "version", buf);
				libraryLoader.Dispose();
			}
			else {
				e.Cancel = true;

			}
		}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			ZoomLevel = double.Parse(comboBox1.SelectedItem.ToString());
//            if (defaultZoomLevel>0) {
				ZoomLevel=ZoomLevel / 10;
//			}else if (defaultZoomLevel < 0) {
//				defaultZoomLevel = defaultZoomLevel * 10 - 1;
//			}

			chromeBrowser.SetZoomLevel(ZoomLevel);

		}
/*
		// 実行中の同じアプリケーションのプロセスを取得する
		private static Process GetPreviousProcess()
		{
			Process curProcess = Process.GetCurrentProcess();
			Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

			foreach (Process checkProcess in allProcesses)
			{
				// 自分自身のプロセスIDは無視する
				if (checkProcess.Id != curProcess.Id)
				{
					// プロセスのフルパス名を比較して同じアプリケーションか検証
					if (String.Compare(
							checkProcess.MainModule.FileName,
							curProcess.MainModule.FileName, true) == 0)
					{
						// 同じフルパス名のプロセスを取得
						return checkProcess;
					}
				}
			}

			// 同じアプリケーションのプロセスが見つからない！  
			return null;
		}
*/
//		public BrowserForm()
//        {
//            InitializeComponent();
//			InitBrowser();
//		}
		static string Title(string url)
        {
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(new Uri(url).Query);
			//			base.WindowState = FormWindowState.Maximized;
			string a = nameValueCollection["ServerId"];
			if (a != null && a.Length != 0 && url.IndexOf("sengokugifu") > 0)
			{
				return "戦国義風 " + a + " " + nameValueCollection["nickname"];
			}
			a = nameValueCollection["sid"];
			if (a != null && a.Length != 0)
			{
				if (url.IndexOf("loas") > 0)
				{
					return "League of Angels2 " + a;
				}
				else
				if (url.IndexOf("loa3") > 0)
				{
					return "League of Angels3 " + a;
				}
			}
			a = nameValueCollection["site"];
			if (a != null && a.Length != 0)
			{
				//a = nameValueCollection["site"];
				if (url.IndexOf("wef.brabragames") > 0)
					{
						return "World End Fantasy " + a;
					}
			}
			if (url.IndexOf("//wef.") > 0)
			{
				return "World End Fantasy ";
			}
			return "gesobrowser"; ;
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
			InitializeComponent();
//			Process prevProcess = GetPreviousProcess();
//			IntPtr he = this.Handle;

			//	String appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			System.IO.Directory.SetCurrentDirectory(appDir);

			string icofile = appNameWoExt + ".ico";
			if (File.Exists(icofile))
				this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(icofile);
			this.Text = Title(url);
			ini = new InifileUtils(Path.Combine(appDir, appNameWoExt + ".ini"));
			string buf = ini.getValueString("Profile", "Window");

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

				WinApi.GetWindowPlacement(this.Handle, out placement);
				//placement.length = Marshal.SizeOf(typeof(WinApi.WINDOWPLACEMENT));
				placement.normalPosition.left = x;
				placement.normalPosition.top = y;
				placement.normalPosition.right = w;
				placement.normalPosition.bottom=h;
				placement.showCmd = WinApi.SW.SHOWNORMAL;
//				WinApi.SetWindowPlacement(this.Handle, ref placement);
			}
			buf = ini.getValueString("Profile", "ZoomLevel");
			if (buf != "")
			{
				ZoomLevel = double.Parse(buf);
			}
			buf = ini.getValueString("Profile", "audio");
			if (buf != "")
			{
				audio = bool.Parse(buf);
			}
			buf = ini.getValueString("Profile", "DialogCloseShow");
			if (buf != "")
			{
				DialogCloseShow = (DialogResult)int.Parse(buf);
			}
			buf = ini.getValueString("Profile", "UserAgent");
			if (buf != "")
			{
				UserAgent = buf;
			}
			proxyserver = ini.getValueString("Profile", "proxy");
			buf = ini.getValueString("Profile", "flashdll");
			if (buf != "")
			{
				flashdll = buf;
			}
			buf = ini.getValueString("Profile", "flashversion");
			if (buf != "")
			{
				flashversion = buf;
			}

			targetUrl = url;
			base.ActiveControl = browserPanel;
			libraryLoader = new CefLibraryHandle(Path.Combine(appDir, "x64", "libcef.dll"));
			if (libraryLoader.IsInvalid) { throw new Exception("no lib"); }

			InitBrowser();
			this.WindowState = System.Windows.Forms.FormWindowState.Normal;
			this.ShowInTaskbar = true;

		}
		private void InitBrowser()
		{
			//			Instance = this;
			if (Cef.IsInitialized == false)
			{
				CefSettings obj = new CefSettings();
				string dllpp = Path.Combine(appDir, "plugins", flashdll);
				if (File.Exists(dllpp)) {
					obj.CefCommandLineArgs.Add("ppapi-flash-version", flashversion);
					obj.CefCommandLineArgs.Add("ppapi-flash-path", dllpp);
				}
				obj.Locale = "ja-JP";
				obj.UserAgent = UserAgent;
				obj.IgnoreCertificateErrors = true;
				obj.LogSeverity = LogSeverity.Disable;
				//				a = Path.Combine(appDir, appNameWoExt + "_UserData");
				a = Path.Combine(appDir, "UserData", appNameWoExt);
				obj.UserDataPath = obj.CachePath = a;
				//				obj.CefCommandLineArgs.Add("disable-gpu", "0");
				if(audio == false)
					obj.CefCommandLineArgs.Add("mute-audio");

				a=dllpp = Path.Combine(appDir, "x64", "CefSharp.BrowserSubprocess.exe");
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
				if (!File.Exists(a)) { throw new Exception("no lib"); }
				obj.BrowserSubprocessPath = a;
				obj.LocalesDirPath = Path.Combine(appDir, "x64", "locales");
				obj.ResourcesDirPath = Path.Combine(appDir, "x64");
				if (proxyserver != "")
				{
					obj.CefCommandLineArgs.Add("proxy-server", proxyserver);
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
			chromeBrowser = new ChromiumWebBrowser(targetUrl);
			chromeBrowser.LifeSpanHandler = new LifeSpanHandler(this);
			BrowserSettings browserSettings = new BrowserSettings();
			browserSettings.AcceptLanguageList = "ja_JP";
			chromeBrowser.BrowserSettings = browserSettings;
			browserPanel.Controls.Add(chromeBrowser);
			chromeBrowser.Dock = DockStyle.Fill;
			//			IKeyboardHandler keyboardHandler = new KeyboardHandler(this);
			//			chromeBrowser.KeyboardHandler = keyboardHandler;
			chromeBrowser.FrameLoadEnd += ChromeBrowserOnFrameLoadEnd;
			chromeBrowser.MenuHandler = new CustomMenuHandler();
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
		private void ChromeBrowserOnFrameLoadEnd(object sender, EventArgs e)
		{
//			var chromiumBrowser = sender as ChromiumWebBrowser;
//			chromiumBrowser?.ShowDevTools();
			chromeBrowser.SetZoomLevel(ZoomLevel);
			Process curProcess = Process.GetCurrentProcess();
			WinApi.SetWindowPlacement(curProcess.MainWindowHandle, ref placement);
		}
		private class CustomMenuHandler : IContextMenuHandler
		{
			void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
			{
				Console.WriteLine("Context menu opened");
				Console.WriteLine(parameters.MisspelledWord);

				if (model.Count > 0)
				{
					model.AddSeparator();
				}

				model.AddItem((CefMenuCommand)26501, "Show DevTools");
				model.AddItem((CefMenuCommand)26502, "Close DevTools");


				//To disable context mode then clear
				// model.Clear();
			}

			bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
			{
				if (commandId == (CefMenuCommand)26501)
				{
					browser.GetHost().ShowDevTools();
					return true;
				}
				if (commandId == (CefMenuCommand)26502)
				{
					browser.GetHost().CloseDevTools();
					return true;
				}

				return false;
			}
			void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) {
			}
			bool IContextMenuHandler.RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) {
				return false;
			}

		}
	}
}
