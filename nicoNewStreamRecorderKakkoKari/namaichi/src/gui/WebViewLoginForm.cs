/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2026/08/13
 * Time: 22:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace namaichi.gui
{
	/// <summary>
	/// Description of WebViewLoginForm.
	/// </summary>
	public partial class WebViewLoginForm : Form
	{
		public string us = null;
		public WebViewLoginForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			try {
				webView21.EnsureCoreWebView2Async(null);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void WebView21CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
		{
			if (webView21.CoreWebView2 == null) {
				return;
			}
			
			webView21.CoreWebView2.Navigate("https://account.nicovideo.jp/spa/login/index.html");                      
			webView21.CoreWebView2.WebResourceResponseReceived +=
					CoreWebView2_WebResourceResponseReceived;
		}
		void CoreWebView2_WebResourceResponseReceived(
				object sender, CoreWebView2WebResourceResponseReceivedEventArgs e) {
			CoreWebView2HttpResponseHeaders h = e.Response.Headers;
			foreach (KeyValuePair<string, string> c in h) {
				//if (c.Key.IndexOf("Cookie") > -1)
					//Debug.WriteLine(c.Key + " " + c.Value);
				if (c.Key.IndexOf("user_session") > -1 || c.Value.IndexOf("user_session_") > -1) {
					us = util.getRegGroup(c.Value, "(user_session_.+?);");
					util.debugWriteLine(us);
					webView21.Visible = false;
					BeginInvoke(new Action(() => Close()));
					webView21.CoreWebView2.Stop();
				}
			}
		}
	}
}
