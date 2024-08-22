/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/22
 * Time: 4:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace namaichi
{
	/// <summary>
	/// Description of VersionForm.
	/// </summary>
	public partial class VersionForm : Form
	{
		private MainForm form;
		public VersionForm(int fontSize, MainForm form)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.form = form;
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			versionLabel.Text = util.versionStr + " (" + util.versionDayStr + ")";
			//communityLinkLabel.Links.Add(0, communityLinkLabel.Text.Length, "http://com.nicovideo.jp/community/co2414037");
			util.setFontSize(fontSize, this, false);
			lastVersionLabel.Font = new Font(lastVersionLabel.Font, FontStyle.Italic);
		}
		
		void okBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void communityLinkLabel_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try {
				System.Diagnostics.Process.Start("https://com.nicovideo.jp/community/co2414037");
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
				form.addLogText("ページの表示に失敗しました " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		
		void VersionFormLoad(object sender, EventArgs e)
		{
			Task.Factory.StartNew(() => checkLastVersion());
		}
		private void checkLastVersion() {
			var r = util.getPageSource("https://github.com/guest-nico/nicoNewStreamRecorderKakkoKari/commits/master/");
			if (r == null) {
				form.formAction(() =>
						lastVersionLabel.Text = "最新の利用可能なバージョンを確認できませんでした");
				return;
			}
			var m = new Regex("https://github.com/guest-nico/nicoNewStreamRecorderKakkoKari/releases/download/releases/(.+?).(zip|rar)").Match(r);
			if (!m.Success) {
				form.formAction(() =>
						lastVersionLabel.Text = "最新の利用可能なバージョンが見つかりませんでした");
				return;
			}
			var v = m.Groups[1].Value;
			if (v.IndexOf(util.versionStr) > -1)
				form.formAction(() => lastVersionLabel.Text = "ニコ生新配信録画ツール（仮は最新バージョンです");
			else {
				form.formAction(() => {
                	lastVersionLabel.Text = v + "が利用可能です";
                	lastVersionLabel.Links.Clear();
                	lastVersionLabel.Links.Add(0, v.Length, m.Value);
                	//lastVersionLabel.LinkArea = new LinkArea(0, v.Length);
                });
			}
		}
		
		void LastVersionLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			form.titleLabel_Click(sender, e);
		}
		
		void DownloadPageLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var url = "https://guest-nico.github.io/pages/downloads.html";
			util.openUrl(url, bool.Parse(form.rec.cfg.get("IsdefaultBrowserPath")), form.rec.cfg.get("browserPath"));
		}
	}
}
