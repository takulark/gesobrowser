using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace gesobrowser
{
    static class Program

	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		//       private static bool StartsWith(string s, string v) => CultureInfo.CurrentCulture.CompareInfo.IsPrefix(s, v, CompareOptions.IgnoreCase);
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
			//else if (url.StartsWith("wefmclient://"))
			//{
			//	return url.Substring(13);
			//}
			else if (url.StartsWith("roadclient://"))
			{
				return url.Substring(13)+"&desktopType=1";
			}
			else if (url.StartsWith("dragonAwaken:", StringComparison.CurrentCultureIgnoreCase))
			{
				return url.Substring(13) + "&mi=true&webkit=true";
			}
			else if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				return url;
			}
			return "";
        }

		// 実行中の同じアプリケーションのプロセスを取得する
		private static Process GetPreviousProcess()
		{
			Process curProcess = Process.GetCurrentProcess();
			Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);
//			Process[] allProcesses2 = Process.GetProcesses();
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


	static void Main(string[] args)
		{
			//AppDomain.CurrentDomain.AssemblyResolve += Resolver;
			//var a=new Subprocess(args);
			//if (a.parentProcessId != null)
			//{
			//	a.Submain(args);
			//	a.Dispose();
			//	return;
			//}
			//a.Dispose();
			if (args.Length != 0)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(defaultValue: false);
				AppDomain.CurrentDomain.AssemblyResolve += Resolver;

				string url = Client(args[0]);
				if (url!="")
				{
					Process curProcess = GetPreviousProcess();
					if (curProcess == null)
					{
						Application.Run(new BrowserForm(url));
						/*
						string AppPath = System.Windows.Forms.Application.ExecutablePath;
						//ファイル名
						string AppName = System.IO.Path.GetFileName(AppPath);
						//ファイル名(拡張子含まず)
						string AppNameWoExt = System.IO.Path.ChangeExtension(AppName, null);
						IntPtr call(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) {
							switch (uMsg)
							{
								case WinApi.WM_DESTROY:
									WinApi.PostQuitMessage(0);
									break;
								default:
									return WinApi.DefWindowProc(hWnd, uMsg, wParam, lParam);
							}
							return IntPtr.Zero;
							
						}

						IntPtr h = WinApi.NewWindow(AppNameWoExt, Process.GetCurrentProcess().Handle,call);
						Thread thread = new Thread(new ThreadStart(() =>
						{
							Application.Run(new BrowserForm(url));
						}));
//						thread.IsBackground = true;
						thread.Start();
						thread.Join();
						WinApi.SendMessage(h, WinApi.WM_DESTROY, UIntPtr.Zero, IntPtr.Zero);*/
					}
					else
					{
                        WinApi.COPYDATASTRUCT cds = new WinApi.COPYDATASTRUCT
                        {
                            dwData = new IntPtr(WinApi.WM_APP),
                            lpData = url,
                            cbData = url.Length * sizeof(char)
                        };
                        IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
						Marshal.StructureToPtr(cds, pnt, false);
						WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_COPYDATA, new UIntPtr(WinApi.WM_APP), pnt);
						//WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_USER1, IntPtr.Zero, ref cds);
						Marshal.FreeHGlobal(pnt);
						if (WinApi.IsIconic(curProcess.MainWindowHandle))
						{
							WinApi.ShowWindow(curProcess.MainWindowHandle, WinApi.SW.SHOWNORMAL);
						}
						WinApi.SetForegroundWindow(curProcess.MainWindowHandle);
						WinApi.UpdateWindow(curProcess.MainWindowHandle);
						WinApi.SetFocus(curProcess.MainWindowHandle);
					}
				}
				else
				{
					MessageBox.Show("起動できません。");
				}
			}
//			else
//			{
//				Application.Run(new BrowserForm());
//				Process.Start("https://www.bitqueen.jp");
//			}
		}
		private static Assembly Resolver(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("CefSharp"))
			{
				string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
				string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
#if tx64
        "x64"
#else
        "x86"
#endif
//													   Environment.Is64BitProcess ? "x64" : "x86",
													   ,assemblyName);

				return File.Exists(archSpecificPath)
						   ? Assembly.LoadFile(archSpecificPath)
						   : throw new Exception("no lib");// null;
			}

			return null;
		}

	}
}
