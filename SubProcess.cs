using System;
using CefSharp.RenderProcess;
using CefSharp.Internals;
using CefSharp.BrowserSubprocess;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace gesobrowser
{
	public class Subprocess : IDisposable
	{
		public String parentProcessId;
		public Subprocess(IEnumerable<string> args) {
			parentProcessId = args.FirstOrDefault((string a) => a.StartsWith("--host-process-id"))?.Split('=').Last();
			//GetArgumentValue(args, "--host-process-id");
		}
		//private static string GetArgumentValue(IEnumerable<string> args, string argumentName)
		//{
		//	var arg = args.FirstOrDefault(a => a.StartsWith(argumentName));
		//	if (arg == null)
		//	{
		//		return null;
		//	}

		//	return arg.Split('=').Last();
		//}

        public void Dispose()
        {
        }

        public void Submain(string[] args)
		{
			if (parentProcessId != null)
			{
				string AppPath = System.Windows.Forms.Application.ExecutablePath;
				//ディレクトリ
				string AppDir = System.IO.Path.GetDirectoryName(AppPath);

				//				System.IO.Directory.SetCurrentDirectory(Path.Combine(AppDir, "x64"));
				Assembly asm;
				//ファイル名を指定してアセンブリを読み込む
				asm = Assembly.LoadFrom(Path.Combine(AppDir, "x64", "CefSharp.BrowserSubprocess.Core.dll"));
				var browserProcessExe = new WcfBrowserSubprocessExecutable();
				SubProcess.EnableHighDPISupport();
				IRenderProcessHandler handler = null;
				var result = browserProcessExe.Main(args, handler);
				//				return result;

			}
		}
	}
}

