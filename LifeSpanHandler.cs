using CefSharp;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace gesobrowser.Handlers
{
    public class ContextMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {

            if (model.Count > 0)
            {
                model.AddSeparator();
            }

            model.AddItem((CefMenuCommand)26501, "Show DevTools");
            //				model.AddSeparator();
            //				model.AddItem((CefMenuCommand)26502, "Close DevTools");
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
            MyForm.Invoke(new Action(() =>
            {
                BrowserForm.BrowserTab browserTab = new BrowserForm.BrowserTab(MyForm.tabControl1, targetUrl, false);
                MyForm.BrowserTablist.Add(browserTab);
                windowInfo.SetAsChild(browserTab.Tab.Handle);//, rect.Left, rect.Top, rect.Right, rect.Bottom);
            }));
            return false;
        }
    }
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
}
