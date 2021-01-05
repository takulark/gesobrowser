using CefSharp;
using System;
using System.Diagnostics;


namespace gesobrowser
{
    class LifeSpanHandler : ILifeSpanHandler

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
		}

		public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
		{
		}

		public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
		{
//			Console.WriteLine("request url: " + targetUrl);
			newBrowser = myForm.chromeBrowser;
			if (targetUrl.Contains("gamefront.sengokugifu.jp/pay.php"))
			{
				Process.Start(targetUrl);
				return true;
			}
			if (targetUrl.Contains("secure.lionsfilm.co.jp/vg/shop"))
			{
				Process.Start(targetUrl);
				return true;
			}
			return true;
		}
	}

}
