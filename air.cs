using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace gesobrowser.Air
{
    class Air : IDisposable
    {
		private string AppPath, AppDir,AppName, AppNameWoExt;
		private Thread thread;
		public Air() { }
        public void Dispose()
        {
			//thread.Dispose();
        }
		private void CreateProcess(string szCmd, bool fWait)
        {
            Process p = new Process();
            // コマンドプロンプトと同じように実行します
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.Arguments = "/c " + szCmd; // 実行するファイル名（コマンド）

            p.StartInfo.CreateNoWindow = false;   // コンソール・ウィンドウは開かない
            p.StartInfo.UseShellExecute = false; // シェル機能を使用しない
            //p.StartInfo.RedirectStandardOutput = true;   // <-- これが「標準出力」リダイレクト
            p.Start();  // コマンドを実行します
 //           if (fWait)
                p.WaitForExit();
            p.Dispose();
        }
		private delegate bool SetDllDirectory(string lpPathName);
		private delegate bool CaptiveAppEntryWinMain(IntPtr a, IntPtr b);
		private bool aaaa2()
		{
			IntPtr dll = IntPtr.Zero;
			CaptiveAppEntryWinMain pCaptiveAppEntryWinMain;
			SetDllDirectory pSetDllDirectory;
			bool ret;

			IntPtr mh = GetModuleHandle("kernel32.dll");
			IntPtr iSetDllDirectory = GetProcAddress(mh, ("SetDllDirectoryW"));
			pSetDllDirectory
				= (SetDllDirectory)Marshal.GetDelegateForFunctionPointer(iSetDllDirectory, typeof(SetDllDirectory));
//			if (pSetDllDirectory) 
				pSetDllDirectory(string.Empty);
			//			if (bbbb(&a, &pCaptiveAppEntryWinMain)) return false;

			string sdll;
			sdll = Path.Combine(AppDir, "plugins" + "\\Adobe AIR\\Versions\\1.0\\Adobe AIR.dll");
			sdll  =Path.Combine(AppDir, "plugins" , "Adobe AIR" , "Versions" , "1.0" , "Adobe AIR.dll");
			if (!File.Exists(sdll)) return false;
			dll = LoadLibrary(sdll);
			IntPtr iCaptiveAppEntryWinMain = GetProcAddress(dll, "CaptiveAppEntryWinMain");
			pCaptiveAppEntryWinMain
				= (CaptiveAppEntryWinMain)Marshal.GetDelegateForFunctionPointer(iCaptiveAppEntryWinMain, typeof(CaptiveAppEntryWinMain));
			mh = GetModuleHandle(string.Empty);
			//	return pCaptiveAppEntryWinMain(mh,a);
			ret = pCaptiveAppEntryWinMain(mh, dll);
			FreeLibrary(dll);
			return ret;

		}
		private void start()
        {
			AppPath = System.Windows.Forms.Application.ExecutablePath;
			//ディレクトリ
			AppDir = System.IO.Path.GetDirectoryName(AppPath);
			AppName = System.IO.Path.GetFileName(AppPath);
			//ファイル名(拡張子含まず)
			AppNameWoExt = System.IO.Path.ChangeExtension(AppName, null);
			System.IO.Directory.SetCurrentDirectory((Path.Combine(AppDir, AppNameWoExt)));

			CreateProcess("cmd.exe /C b.bat", true);
			aaaa2();

		}
		public void Init() {
			thread = new Thread(new ThreadStart(() =>
			{
				start();
			}));

			thread.Start();
			thread.Join();

		}
		public Air GetBrowser() { return this; }
		public void CloseBrowser(bool a) { thread.Abort(); }
		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto)]
		static extern IntPtr GetModuleHandle(string modName);
		[DllImport("kernel32.dll")]
		static extern IntPtr LoadLibrary(string lpFileName);
		[DllImport("kernel32.dll")]
		static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
		[DllImport("kernel32.dll")]
		static extern bool FreeLibrary(IntPtr hLibModule);
	}
}