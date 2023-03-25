using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace gesobrowser
{
	public static class Setup
	{
		public static string AppPath { get; set; }
		public static string AppDir { get; set; }
		public static string AppName { get; set; }
		public static string AppNameWoExt { get; set; }
		public static string Dirx3264 { get; set; }
		public static int UserDataCnt { get; set; }
		//        public static string UserData { get; set; }
		public static void Init()
		{
			AppPath = Application.ExecutablePath;
			//ディレクトリ
			AppDir = Path.GetDirectoryName(AppPath);
			//ファイル名
			AppName = Path.GetFileName(AppPath);
			//ファイル名(拡張子含まず)
			AppNameWoExt = Path.ChangeExtension(AppName, null);
			string Ext =
#if tx64
							"x64";
#else
							"x86";
#endif
			int s = AppNameWoExt.LastIndexOf(Ext);
			if (s > 0)
			{
				Ext = AppNameWoExt.Substring(s);
				AppNameWoExt = AppNameWoExt.Substring(0, s - 1);
			}
			Dirx3264 = Path.Combine(AppDir, Ext);
			//            UserData = "UserData";
		}
	}
    public class DisposableList<T> : List<T>, IDisposable
    {
        public void Dispose()
        {
            Clear();
        }
    }

    static class Program
	{
		//private static bool StartsWith(string s, string v) => CultureInfo.CurrentCulture.CompareInfo.IsPrefix(s, v, CompareOptions.IgnoreCase);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string Client(string url)
		{
			if (url.StartsWith("sengoku://"))
			{
				return url.Substring(10);
			}
			else if (url.StartsWith("loa3mclient://"))
			{
				return url.Substring(14);
			}
			else if (url.StartsWith("loas2mclient://"))
			{
				return url.Substring(15);
			}
			else if (url.StartsWith("roadclient://"))
			{
				return url.Substring(13) + "&desktopType=1";
			}
			else if (url.StartsWith("dragonAwaken:", StringComparison.CurrentCultureIgnoreCase))
			{
				return url.Substring(13) + "&mi=true&webkit=true";
			}
			else if (url.StartsWith("dragonminiclient:", StringComparison.CurrentCultureIgnoreCase))
			{
				return url.Substring(21);
			}
			else if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				return url;
			}
			return "";
		}
		private static int GetProcessCompare(Process x, Process y)
		{
			//return ~x.StartTime.CompareTo(y.StartTime);
			int ret = 0;
            Int64 ticks1 = x.StartTime.ToBinary();// & 0x3FFFFFFFFFFFFFFF;
            Int64 ticks2 = y.StartTime.ToBinary();// & 0x3FFFFFFFFFFFFFFF;
            //DateTime ticks1 = x.StartTime;
            //DateTime ticks2 = y.StartTime;
            if (ticks1 > ticks2) ret = 1;
			else if (ticks1 < ticks2) ret = -1;
			else if (x.Id > y.Id) ret = 1;
			else if (x.Id < y.Id) ret = -1;

			return ~ret;
		}
		private static IntPtr GetProcess(int cnt)
		{
			int i, r;
			using Process curProcess = Process.GetCurrentProcess();
			Process[] procs = Process.GetProcesses(".");
			using DisposableList<Process> list = new DisposableList<Process>();

			for (i = 0; i < procs.Length; i++)
			{
				if (String.Equals(curProcess.ProcessName, procs[i].ProcessName, StringComparison.OrdinalIgnoreCase)
					&& procs[i].Id != curProcess.Id)
				{
					list.Add(procs[i]);
				}
				else
				{
					procs[i].Dispose();
				}
			}
			//curProcess.Dispose();
			Setup.UserDataCnt = list.Count;
			IntPtr whnd = IntPtr.Zero, lwhnd;
			if (list.Count > 0)
			{
				//if (list.Count > 1) list.OrderByDescending(x => x.StartTime);
				if (list.Count > 1) list.Sort(GetProcessCompare);

				for (i = 0; i < list.Count; i++)
				{
					lwhnd = list[i].MainWindowHandle;
					r = WinApi.SendMessage(lwhnd, WinApi.WM_APP1, (UIntPtr)WinApi.WM_APP1, (IntPtr)cnt);
					if (cnt == -1 || r < cnt)
					{
						whnd = lwhnd; cnt = 0;
					}
					list[i].Dispose();
				}
				//list.Dispose();
			}
			return whnd;
		}
		//private static Process GetPreviousProcess()
		//{
		//    Process curProcess = Process.GetCurrentProcess();
		//    Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);
		//    //Process[] allProcesses2 = Process.GetProcesses();
		//    foreach (Process checkProcess in allProcesses)
		//    {
		//        if (checkProcess.Id != curProcess.Id)
		//        {
		//            if (String.Compare(
		//                    checkProcess.MainModule.FileName,
		//                    curProcess.MainModule.FileName, true) == 0)
		//            {
		//                return checkProcess;
		//            }
		//        }
		//    }
		//    return null;
		//}
		//private static int GetPreviousProcessCnt()
		//{
		//    Process curProcess = Process.GetCurrentProcess();
		//    Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);
		//    int cnt = 0;
		//    foreach (Process checkProcess in allProcesses)
		//    {
		//        if (checkProcess.Id != curProcess.Id)
		//        {
		//            if (String.Compare(
		//                    checkProcess.MainModule.FileName,
		//                    curProcess.MainModule.FileName, true) == 0)
		//            {
		//                cnt++;
		//            }
		//        }
		//    }
		//    return cnt;
		//}
		private static Assembly Resolver(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("CefSharp"))
			{
				string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
				string archSpecificPath = Path.Combine(Setup.Dirx3264, assemblyName);
				return File.Exists(archSpecificPath)
							? Assembly.LoadFile(archSpecificPath)
							: throw new Exception("no lib");// null;
			}
			return null;
		}
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//AppDomain.CurrentDomain.AssemblyResolve += Resolver;
			//CefSharp.Cef.EnableHighDPISupport();
			//var exitCode = CefSharp.BrowserSubprocess.SelfHost.Main(args);
			//if (exitCode >= 0)
			//{
			//    return;
			//}
			if (args.Length == 0) { return; }
			Setup.Init();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			AppDomain.CurrentDomain.AssemblyResolve += Resolver;
			string arg = args[0];
			int cnt = -1;
			if (arg.StartsWith("tabcnt"))
			{
				cnt = Math.Max(int.Parse(arg.Substring(7)), 1);
				arg = args[1];
			}
			string url = Client(arg);
			if (url == "")
			{
				MessageBox.Show("起動できません。"); return;
			}
            IntPtr whnd = GetProcess(cnt);
            if (whnd == IntPtr.Zero)
                cnt = 0;
            switch (cnt)
			{
				case 0:
				case 1:
					Application.Run(new BrowserForm(url));
					return;
				default:
					WinApi.COPYDATASTRUCT cds = new WinApi.COPYDATASTRUCT
					{
						dwData = (IntPtr)WinApi.WM_APP,
						lpData = url,
						cbData = url.Length * sizeof(char)
					};
					IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
					Marshal.StructureToPtr(cds, pnt, false);
					WinApi.SendMessage(whnd, WinApi.WM_COPYDATA, (UIntPtr)WinApi.WM_APP, pnt);
					Marshal.FreeHGlobal(pnt);
					if (WinApi.IsIconic(whnd))
					{
						WinApi.ShowWindow(whnd, WinApi.SW.SHOWNORMAL);
					}
					WinApi.SetForegroundWindow(whnd);
					WinApi.UpdateWindow(whnd);
					WinApi.SetFocus(whnd);
					return;
			}
		}
	}
}

