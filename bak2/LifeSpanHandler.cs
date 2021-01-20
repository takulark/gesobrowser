using CefSharp;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using CefSharp.Structs;
using System.Collections.Generic;

namespace gesobrowser.Handlers
{
    public class ContextMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //				Console.WriteLine("Context menu opened");
            //				Console.WriteLine(parameters.MisspelledWord);

            if (model.Count > 0)
            {
                model.AddSeparator();
            }

            model.AddItem((CefMenuCommand)26501, "Show DevTools");
            //				model.AddSeparator();
            //				model.AddItem((CefMenuCommand)26502, "Close DevTools");
            model.AddSeparator();
            //				model.AddItem((CefMenuCommand)26503, "Tab Close");
            //				model.AddSeparator();
            //				model.AddItem((CefMenuCommand)26504, "Zoom +");
            //				model.AddItem((CefMenuCommand)26505, "Zoom -");
            //				model.AddSeparator();
            //				model.AddItem((CefMenuCommand)26506, "Tab Reload");
            //				To disable context mode then clear
            // model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            //				try {
            //				if (commandId == (CefMenuCommand)225)
            //				{
            //					Instance.newtabs("http://www.macromedia.com/support/documentation/en/flashplayer/help/settings_manager.html");
            //					return true;
            //				}
            //				if (commandId == (CefMenuCommand)226 || commandId == (CefMenuCommand)227)
            //				{
            //					Instance.newtabs("https://get.adobe.com/jp/flashplayer/about/");
            //					return true;
            //				}
            if (commandId == (CefMenuCommand)26501)
            {
                browser.GetHost().ShowDevTools();
                return true;
            }
            //				}
            //				finally {  }
            //				if (commandId == (CefMenuCommand)26502)
            //				{
            //					browser.GetHost().CloseDevTools();
            //					return true;
            //				}
            /*				if (commandId == (CefMenuCommand)26503)
                            {
                                //					if(Instance.tabControl1_Remove())
                                if (Instance.tabControl1.TabCount > 1)
                                {
                                    Instance.tabControl1.TabPages.Remove(Instance.tabControl1.SelectedTab);						
                                    browser.CloseBrowser(true);
                                }else
                                    Instance.Close();
                                return true;
                            }
                            if (commandId == (CefMenuCommand)26504)
                            {
                                double z=browser.GetHost().GetZoomLevel();
                                browser.GetHost().SetZoomLevel(z+1);
                                return true;
                            }
                            if (commandId == (CefMenuCommand)26505)
                            {
                                double z = browser.GetHost().GetZoomLevel();
                                browser.GetHost().SetZoomLevel(z -1);
                                return true;
                            }
                            if (commandId == (CefMenuCommand)26506)
                            {
                                browser.Reload();
                                return true;
                            }*/
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
		private BrowserForm myForm;

		public LifeSpanHandler(BrowserForm form)
		{
			myForm = form;
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
                WinApi.SendMessage(windHandler, WinApi.WM_SETICON, new UIntPtr(1), myForm.Icon.Handle);
            }

		}

		public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
		{
		}

		public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
		{
            //			Console.WriteLine("request url: " + targetUrl);
            //			int i = myForm.tabControl1.SelectedIndex;
            //			newBrowser = myForm.BrowserTablist[i].chromeBrowser;
            newBrowser = null;
            if (targetUrl.Contains("gamefront.sengokugifu.jp/pay.php"))
			{
				Process.Start(targetUrl);
				return true;
			}else
			if (targetUrl.Contains("secure.lionsfilm.co.jp/vg/shop"))
			{
				Process.Start(targetUrl);
				return true;
			}

            myForm.Invoke(new Action(() =>
            {
                // WebView無しの空のコンテナフォームを作成する。
                BrowserForm.BrowserTab browserTab = new BrowserForm.BrowserTab(myForm.tabControl1, targetUrl, false);
                //            BrowserTab tab = browserTab;
                myForm.BrowserTablist.Add(browserTab);

            // WebViewのコンテナ（親ウィンドウ）の情報を設定する。
            //                Rectangle rect = browserTab.ClientRectangle;
            windowInfo.SetAsChild(browserTab.Tab.Handle);//, rect.Left, rect.Top, rect.Right, rect.Bottom);

                // フォームを表示する
//                browserTab.Show();
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
//            return u;
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
            Uri url,rurl;
            if (Uri.TryCreate(request.Url, UriKind.Absolute, out url) == false)
            {
                //If we're unable to parse the Uri then cancel the request
                // avoid throwing any exceptions here as we're being called by unmanaged code
                return CefReturnValue.Cancel;
            }
            if (Uri.TryCreate(request.ReferrerUrl, UriKind.Absolute, out rurl) == false)
            {
                //If we're unable to parse the Uri then cancel the request
                // avoid throwing any exceptions here as we're being called by unmanaged code
                return CefReturnValue.Cancel;
            }
            string a = Getdomain(url.Host);
            string b = Getdomain(rurl.Host);
            if(b.StartsWith(a, StringComparison.CurrentCultureIgnoreCase))
                return CefReturnValue.Continue;
            frame.ExecuteJavaScriptAsync($"console.log('noload:{request.Url}');", frame.Url);
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
        private BrowserForm myForm;
        public RequestHandler(BrowserForm form)
        {
            myForm = form;
        }
        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {/*
            // コントロールのトップレベルのコントロールを取得（SimpleBrowserFrame）
//            SimpleBrowserFrame mainFrame = SimpleBrowserFrame.getMainFrame(browser);

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
            if(myForm.isDomain==false && request.ReferrerUrl!="")
                return new ResourceRequestHandler();
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            // ナビゲーションを許可する
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            // 証明書エラーでも継続する
            callback.Continue(true);
            return true;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            // Ctrl+左クリックでの別Tab表示は許可しない。
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            // クオータの要求は常に許可する。
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

/*    public class DisplayHandler : IDisplayHandler
    {
        public void OnAddressChanged(IWebBrowser chromiumWebBrowser, AddressChangedEventArgs addressChangedArgs)
        {
            throw new NotImplementedException();
        }

        public bool OnAutoResize(IWebBrowser chromiumWebBrowser, IBrowser browser, Size newSize)
        {
            throw new NotImplementedException();
        }

        public bool OnConsoleMessage(IWebBrowser chromiumWebBrowser, ConsoleMessageEventArgs consoleMessageArgs)
        {
            throw new NotImplementedException();
        }

        public void OnFaviconUrlChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls)
        {
            throw new NotImplementedException();
        }

        public void OnFullscreenModeChange(IWebBrowser chromiumWebBrowser, IBrowser browser, bool fullscreen)
        {
            throw new NotImplementedException();
        }

        public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
        {
            throw new NotImplementedException();
        }

        public void OnStatusMessage(IWebBrowser chromiumWebBrowser, StatusMessageEventArgs statusMessageArgs)
        {
            throw new NotImplementedException();
        }

        public void OnTitleChanged(IWebBrowser chromiumWebBrowser, TitleChangedEventArgs titleChangedArgs)
        {
            throw new NotImplementedException();
        }

        public bool OnTooltipChanged(IWebBrowser chromiumWebBrowser, ref string text)
        {
            throw new NotImplementedException();
        }
    }*/
}
