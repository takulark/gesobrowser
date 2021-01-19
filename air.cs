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
            // �R�}���h�v�����v�g�Ɠ����悤�Ɏ��s���܂�
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.Arguments = "/c " + szCmd; // ���s����t�@�C�����i�R�}���h�j

            p.StartInfo.CreateNoWindow = false;   // �R���\�[���E�E�B���h�E�͊J���Ȃ�
            p.StartInfo.UseShellExecute = false; // �V�F���@�\���g�p���Ȃ�
            //p.StartInfo.RedirectStandardOutput = true;   // <-- ���ꂪ�u�W���o�́v���_�C���N�g
            p.Start();  // �R�}���h�����s���܂�
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
			//�f�B���N�g��
			AppDir = System.IO.Path.GetDirectoryName(AppPath);
			AppName = System.IO.Path.GetFileName(AppPath);
			//�t�@�C����(�g���q�܂܂�)
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