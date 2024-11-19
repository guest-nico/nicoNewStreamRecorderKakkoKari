/*
 * Created by SharpDevelop.
 * User: ajkkh
 * Date: 2024/11/19
 * Time: 8:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using namaichi.utility;

namespace namaichi.gui
{
	/// <summary>
	/// Description of AikotobaInputForm.
	/// </summary>
	public partial class AikotobaInputForm : Form
	{
		string lvid = null;
		CookieContainer cc;
		public AikotobaInputForm(string lvid, CookieContainer cc)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			this.lvid = lvid;
			this.cc = cc;
		}
		void CancelBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void AuthBtnClick(object sender, EventArgs e)
		{
			string errMsg = null;
			if (sendAikotoba(out errMsg)) {
				DialogResult = DialogResult.OK;
				Close();
			} else {
				msgText.Text = errMsg;
			}
		}
		bool sendAikotoba(out string msg) {
			msg = null;
			
			var url = "https://live2.nicovideo.jp/unama/api/v2/programs/" + lvid + "/password/permission";
			var h = util.getHeader(cc, "https://live.nicovideo.jp/", url);
			h.Add("X-niconico-session", "cookie");
			h.Add("Content-Type", "application/json");
			var data = "{\"password\":\"" + passText.Text + "\"}";
			
			var r = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", data, false, true, true);
			util.debugWriteLine(r);
			if (r.IndexOf("\"status\":200") == -1) {
				msg = r;
				return false;
			}
			return true;
		}
	}
}
