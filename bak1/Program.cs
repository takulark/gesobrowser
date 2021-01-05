using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace gesobrowser
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
		static string client(string url)
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
			else if (url.StartsWith("wefmclient://"))
			{
				return url.Substring(13);
			}
			else if (url.StartsWith("roadclient://"))
			{
				return url.Substring(13)+"&desktopType=1";
			}
			else if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				return url;
			}
			return "";
        }

		static void Main(string[] args)
        {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			AppDomain.CurrentDomain.AssemblyResolve += Resolver;

			if (args.Length != 0)
			{
				string url = client(args[0]);
				if (url!="")
				{
					Application.Run(new BrowserForm(url));
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
													   Environment.Is64BitProcess ? "x64" : "x86",
													   assemblyName);

				return File.Exists(archSpecificPath)
						   ? Assembly.LoadFile(archSpecificPath)
						   : throw new Exception("no lib");// null;
			}

			return null;
		}

	}
}
