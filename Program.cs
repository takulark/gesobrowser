﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

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
            else if (url.StartsWith("roadclient://"))
            {
                return url.Substring(13) + "&desktopType=1";
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
                if (url != "")
                {
                    Process curProcess = GetPreviousProcess();
                    if (curProcess == null)
                    {
                        Application.Run(new BrowserForm(url));
                    }
                    else
                    {
                        WinApi.COPYDATASTRUCT cds = new WinApi.COPYDATASTRUCT
                        {
                            dwData = new IntPtr(WinApi.WM_APP),
                            lpData = url,
                            cbData = url.Length * sizeof(char)
                        };
                        //WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_COPYDATA, IntPtr.Zero, ref cds);
                        IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
                        Marshal.StructureToPtr(cds, pnt, false);
                        WinApi.SendMessage(curProcess.MainWindowHandle, WinApi.WM_COPYDATA, new UIntPtr(WinApi.WM_APP), pnt);
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
                                                       //Environment.Is64BitProcess ? "x64" : "x86",
                                                       , assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : throw new Exception("no lib");// null;
            }

            return null;
        }

    }
}
