using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gesobrowser
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			if (args.Length != 0)
			{
				if (args[0].StartsWith("sengoku://"))
				{
					Application.Run(new BrowserForm(args[0].Substring(10)));
				}
				else if (args[0].StartsWith("loa3mclient://"))
				{
					Application.Run(new BrowserForm(args[0].Substring(14)));
				}
				else if (args[0].StartsWith("loas2mclient://"))
				{
					Application.Run(new BrowserForm(args[0].Substring(15)));
				}
				else if (args[0].StartsWith("wefmclient://"))
				{
					Application.Run(new BrowserForm(args[0].Substring(13)));
				}
				else if (args[0].StartsWith("http://") || args[0].StartsWith("https://"))
				{
					Application.Run(new BrowserForm(args[0]));
				}
				else
				{
					MessageBox.Show("起動できません。");
				}
			}
			else
			{
//				Application.Run(new BrowserForm());
//				Process.Start("https://www.bitqueen.jp");
			}
		}
    }
}
