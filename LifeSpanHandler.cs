using CefSharp;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace gesobrowser.Handlers
{
    public class ContextMenuHandler : IContextMenuHandler
    {
        private BrowserForm MyForm { get; set; }

        public ContextMenuHandler(BrowserForm form)
        {
            MyForm = form;
        }
        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {

            if (model.Count > 0)
            {
                model.AddSeparator();
            }

            model.AddItem((CefMenuCommand)26501, "Show DevTools");
            //				model.AddSeparator();
            //				model.AddItem((CefMenuCommand)26502, "Close DevTools");
            model.AddItem((CefMenuCommand)26502, "Tab menu");
            model.AddSeparator();
            // model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if (commandId == (CefMenuCommand)26501)
            {
                browser.GetHost().ShowDevTools();
                return true;
            }
            //				if (commandId == (CefMenuCommand)26502)
            //				{
            //					browser.GetHost().CloseDevTools();
            //					return true;
            //				}
            if (commandId == (CefMenuCommand)26502)
            {
                MyForm.Invoke(new Action(() =>
                {
                    System.Drawing.Point p = Cursor.Position;
                    MyForm.tabControl1.ContextMenuStrip.Show(p);
                    // t.ContextMenu.Show(t,p);
                 }));
                return true;
            }
            return false;
        }
        public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
        }
        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }

    }
    public class LifeSpanHandler : ILifeSpanHandler

    {
        private BrowserForm MyForm { get; set; }

        public LifeSpanHandler(BrowserForm form)
        {
            MyForm = form;
        }

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            if (browser.IsPopup)
            {
                var windHandler = browser.GetHost().GetWindowHandle();
                WinApi.SendMessage(windHandler, WinApi.WM_SETICON, new UIntPtr(1), MyForm.Icon.Handle);
            }

        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            //if (targetUrl.Contains("gamefront.sengokugifu.jp/pay.php"))
            //{
            //    Process.Start(targetUrl);
            //    return true;
            //}
            //else
            //if (targetUrl.Contains("secure.lionsfilm.co.jp/vg/shop"))
            //{
            //    Process.Start(targetUrl);
            //    return true;
            //}

            MyForm.Invoke(new Action(() =>
            {
                BrowserForm.BrowserTab browserTab = new BrowserForm.BrowserTab(MyForm, targetUrl, false);
                MyForm.BrowserTablist.Add(browserTab);
                #if Ver60
                windowInfo.ParentWindowHandle= browserTab.Tab.Handle;
                #else
                windowInfo.SetAsChild(browserTab.Tab.Handle);//, rect.Left, rect.Top, rect.Right, rect.Bottom);
                #endif
            }));
            return false;
        }
    }

#if Ver80
    public class ResourceRequestHandler : IResourceRequestHandler
    {
        private static string Getdomain(string host)
        {
            int index = host.LastIndexOf('.'), last = 3;
            while (index > 0 && index >= last - 3)
            {
                last = index;
                index = host.LastIndexOf('.', last - 1);
            }
            return host.Substring(index + 1);
        }
        public void Dispose()
        {
        }

        public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            do
            {
                string r = request.Url;
                if (Uri.TryCreate(r, UriKind.Absolute, out Uri url) == false)
                {
                    break;
                }
                if (Uri.TryCreate(request.ReferrerUrl, UriKind.Absolute, out Uri rurl) == false)
                {
                    break;
                }
                string s = url.Query;
                if (s != "" && (request.ResourceType == ResourceType.Stylesheet ||
                request.ResourceType == ResourceType.Script ||
                request.ResourceType == ResourceType.Image))
                {
                    request.Url = r.Substring(0, r.IndexOf(s));

                }
                string a = Getdomain(url.Host);
                string b = Getdomain(rurl.Host);
                if (b.StartsWith(a, StringComparison.CurrentCultureIgnoreCase))
                    return CefReturnValue.Continue;
            } while (false);

            frame.ExecuteJavaScriptAsync($"console.log('noload: {request.Url}');", frame.Url);
            return CefReturnValue.Cancel;
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return false;
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }
    }
    /*
	public class AuthDialog : Form
	{
		/// <summary>
		/// ダイアログに入力されたユーザ名
		/// </summary>
		public String UserName { get; private set; }

		/// <summary>
		/// ダイアログに入力されたパスワード
		/// </summary>
		public String Password { get; private set; }

		public AuthDialog()
		{
			InitializeComponent();
		}

		private void ConfirmBtn_Click(object sender, EventArgs e)
		{
			// 確定したユーザ名とパスワードをプロパティに設定する。
			UserName = userNameTxt.Text;
			Password = passwordTxt.Text;
		}
	}*/
    public class RequestHandler : IRequestHandler
    {
        private BrowserForm MyForm { get; set; }
        public RequestHandler(BrowserForm form)
        {
            MyForm = form;
        }
        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {/*
			if (myForm != null)
			{
				// 親コントロールのコンテキストで非同期にダイアログを表示する。
				myForm.BeginInvoke(new Action(() =>
				{
					// ログオンダイアログを表示する
					AuthDialog dlg = new AuthDialog();
					DialogResult ret = dlg.ShowDialog();

					if (ret == DialogResult.OK)
					{
						// 入力されたユーザ名とパスワードで認証を継続する。
						callback.Continue(dlg.UserName, dlg.Password);
					}
					else
					{
						// 認証処理をキャンセルする。
						callback.Cancel();
					}
				}));
			}
			else
			{
				// 認証処理をキャンセルする。
				callback.Cancel();
			}
	*/
            return true;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            if (MyForm.IsDomain == false && request.ReferrerUrl != "")
                return new ResourceRequestHandler();
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            //if (Uri.TryCreate(request.Url, UriKind.Absolute, out Uri url))
            //{
            //    if (url.LocalPath.EndsWith(".swf", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        int h = MyForm.Size.Height;
            //        string s = "document.body.style.heigh=-webkit-fill-available;";
            //        string script =
            //            "document.addEventListener('DOMContentLoaded', function(){document.body.style.heigh=-webkit-fill-available; });";
            //        frame.ExecuteJavaScriptAsync(script);
            //        frame.ExecuteJavaScriptAsync(s);
            //        s =$"{{document.body.style.heigh={h}pt}}";
            //        script =
            //            $"document.addEventListener(\'DOMContentLoaded\', function(){s});";
            //        frame.ExecuteJavaScriptAsync(script);
            //        frame.ExecuteJavaScriptAsync(s);
            //    }
            //}
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            callback.Continue(true);
            return true;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            callback.Continue(true);
            return true;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }
        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }
    }
#endif
#if Ver60
    public class RequestHandler : IRequestHandler
    {
        private BrowserForm MyForm { get; set; }
        public RequestHandler(BrowserForm form)
        {
            MyForm = form;
        }

        private static string Getdomain(string host)
        {
            int index = host.LastIndexOf('.'), last = 3;
            while (index > 0 && index >= last - 3)
            {
                last = index;
                index = host.LastIndexOf('.', last - 1);
            }
            return host.Substring(index + 1);
        }
        public bool CanGetCookies(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return true;
        }

        public bool CanSetCookie(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
        {
            return true;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;

        }

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
#if Ver70
         public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
#endif
        {
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            if (MyForm.IsDomain || request.ReferrerUrl == "")
                return CefReturnValue.Continue;
            if (Uri.TryCreate(request.Url, UriKind.Absolute, out Uri url) == false)
            {
                return CefReturnValue.Cancel;
            }
            if (Uri.TryCreate(request.ReferrerUrl, UriKind.Absolute, out Uri rurl) == false)
            {
                return CefReturnValue.Cancel;
            }
            string a = Getdomain(url.Host);
            string b = Getdomain(rurl.Host);
            if (b.StartsWith(a, StringComparison.CurrentCultureIgnoreCase))
                return CefReturnValue.Continue;
            frame.ExecuteJavaScriptAsync($"console.log('noload: {request.Url}');", frame.Url);
            return CefReturnValue.Cancel;

        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            callback.Continue(true);
            return true;

        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, string url)
        {
            return false;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            callback.Continue(true);
            return true;

        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }
    }
#endif
}
