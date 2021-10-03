/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2021/09/30
 * Time: 1:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using rokugaTouroku.info;
using SunokoLibrary.Application;

namespace rokugaTouroku.gui
{
	/// <summary>
	/// Description of accountForm.
	/// </summary>
	public partial class accountForm : Form
	{
		//private config.config config = null;
		//public bool isBrowser = false;
		//public CookieSourceInfo si = null;
		//public string aus = null;
		public CookieSourceInfo si = null;
		public AccountInfo ai = null;
		private config.config cfg;
		public accountForm(config.config config)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			mailText.Text = config.get("accountId");
        	passText.Text = config.get("accountPass");
        	
        	checkBoxShowAll.Checked = bool.Parse(config.get("IsBrowserShowAll"));
        	if (config.get("browserNum") == "1") useAccountLoginRadioBtn.Checked = true;
        	else useCookieRadioBtn.Checked = true;
        	useSecondLoginChkBox.Checked = bool.Parse(config.get("issecondlogin"));
        	isCookieFileSiteiChkBox.Checked = bool.Parse(config.get("iscookie"));
        	cookieFileText.Text = config.get("cookieFile");
        	
        	this.cfg = config;
		}
		void okBtnClick(object sender, EventArgs e)
		{
			try {
				if (useCookieRadioBtn.Checked) {
					var importer = nicoSessionComboBox1.Selector.SelectedImporter;
					if (importer != null && importer.SourceInfo != null) {
						si = importer.SourceInfo;
						if (isCookieFileSiteiChkBox.Checked)
							si = si.GenerateCopy(si.BrowserName, si.ProfileName, cookieFileText.Text);
					}
					
					ai = new AccountInfo(si, null, null, true);
				} else {
					ai = new AccountInfo(null, mailText.Text, passText.Text, false);
				}
				DialogResult = DialogResult.OK;
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		void CancelBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		void AccountFormLoad(object sender, EventArgs e)
		{
			try {
				var si = SourceInfoSerialize.load(false);
	        	nicoSessionComboBox1.Selector.SetInfoAsync(si);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		async void LoginBtnClick(object sender, EventArgs e)
		{
			var TargetUrl = new Uri("https://live.nicovideo.jp/");
			var cg = new rec.CookieGetter(cfg);
			var cc = await cg.getAccountCookie(mailText.Text, passText.Text);
			if (cc == null) {
				util.showMessageBoxCenterForm(this, "login error", "", MessageBoxButtons.OK);
				return;
			}
			if (cc.GetCookies(TargetUrl)["user_session"] == null &&
				                   cc.GetCookies(TargetUrl)["user_session_secure"] == null)
				util.showMessageBoxCenterForm(this, "no login", "", MessageBoxButtons.OK);
			else util.showMessageBoxCenterForm(this, "login ok", "", MessageBoxButtons.OK);
		}
	}
}
