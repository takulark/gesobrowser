using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace gesobrowser
{
    public static class Setup
    {
        public static string AppPath { get; set; }
        public static string AppDir { get; set; }
        public static string AppName { get; set; }
        public static string AppNameWoExt { get; set; }
        public static string Dirx3264 { get; set; }
        //var t;
        public static void Init(){
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
            if (s>0)
            {
                Ext=AppNameWoExt.Substring(s);
                AppNameWoExt = AppNameWoExt.Substring(0,s-1);
            }
            Dirx3264 = Path.Combine(AppDir,Ext);
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

        private static Process GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);
            //Process[] allProcesses2 = Process.GetProcesses();
            foreach (Process checkProcess in allProcesses)
            {
                if (checkProcess.Id != curProcess.Id)
                {
                    if (String.Compare(
                            checkProcess.MainModule.FileName,
                            curProcess.MainModule.FileName, true) == 0)
                    {
                        return checkProcess;
                    }
                }
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
            if (args.Length != 0)
            {
                Setup.Init();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(defaultValue: false);
                AppDomain.CurrentDomain.AssemblyResolve += Resolver;
                string arg = args[0];
                int cnt = -1;
                if (arg.StartsWith("tabcnt"))
                {
                    cnt = Math.Max(int.Parse(arg.Substring(7)),1);
                    arg = args[1];
                }
                string url = Client(arg);
                if (url != "")
                {
                    Process curProcess = GetPreviousProcess();
                    switch (cnt) {
                        case 0:
                        case 1:
                            Application.Run(new BrowserForm(url));
                            return;
                        default:
                            if (curProcess == null)
                                goto case 0;
                            if (cnt != -1)
                            {
                                int r = WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_APP1, (UIntPtr)WinApi.WM_APP1, (IntPtr)cnt);
                                if (cnt <= r)
                                    goto case 0;
                            }
                            WinApi.COPYDATASTRUCT cds = new WinApi.COPYDATASTRUCT
                            {
                                dwData = new IntPtr(WinApi.WM_APP),
                                lpData = url,
                                cbData = url.Length * sizeof(char)
                            };
                            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
                            Marshal.StructureToPtr(cds, pnt, false);
                            WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_COPYDATA, (UIntPtr)WinApi.WM_APP, pnt);
                            Marshal.FreeHGlobal(pnt);
                            if (WinApi.IsIconic(curProcess.MainWindowHandle))
                            {
                                WinApi.ShowWindow(curProcess.MainWindowHandle, WinApi.SW.SHOWNORMAL);
                            }
                            WinApi.SetForegroundWindow(curProcess.MainWindowHandle);
                            WinApi.UpdateWindow(curProcess.MainWindowHandle);
                            WinApi.SetFocus(curProcess.MainWindowHandle);
                            return;
                    }
                    
                    //if (curProcess == null)
                    //{
                    //    Application.Run(new BrowserForm(url));
                    //}
                    //else
                    //{
                    //    WinApi.COPYDATASTRUCT cds = new WinApi.COPYDATASTRUCT
                    //    {
                    //        dwData = new IntPtr(WinApi.WM_APP),
                    //        lpData = url,
                    //        cbData = url.Length * sizeof(char)
                    //    };
                    //    //WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_COPYDATA, IntPtr.Zero, ref cds);
                    //    IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
                    //    Marshal.StructureToPtr(cds, pnt, false);
                    //    WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_COPYDATA, new UIntPtr(WinApi.WM_APP), pnt);
                    //    Marshal.FreeHGlobal(pnt);
                    //    if (WinApi.IsIconic(curProcess.MainWindowHandle))
                    //    {
                    //        WinApi.ShowWindow(curProcess.MainWindowHandle, WinApi.SW.SHOWNORMAL);
                    //    }
                    //    WinApi.SetForegroundWindow(curProcess.MainWindowHandle);
                    //    WinApi.UpdateWindow(curProcess.MainWindowHandle);
                    //    WinApi.SetFocus(curProcess.MainWindowHandle);
                    //}
                }
                else
                {
                    MessageBox.Show("起動できません。");
                }
            }
        }
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

    }
}
