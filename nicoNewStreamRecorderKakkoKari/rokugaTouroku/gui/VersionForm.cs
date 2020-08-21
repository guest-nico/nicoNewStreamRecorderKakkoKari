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

namespace rokugaTouroku
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
			//communityLinkLabel.Links.Add(0, communityLinkLabel.Text.Length, "https://com.nicovideo.jp/community/co2414037");
			util.setFontSize(fontSize, this, false);
			lastVersionLabel.Font = new Font(lastVersionLabel.Font, FontStyle.Italic);
		}
		
		void okBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void communityLinkLabel_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://com.nicovideo.jp/community/co2414037");
		}
		
		void VersionFormLoad(object sender, EventArgs e)
		{
			Task.Factory.StartNew(() => checkLastVersion());
		}
		private void checkLastVersion() {
			var r = util.getPageSource("https://com.nicovideo.jp/community/co2414037");
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
			if (v.IndexOf(util.versionStr.Substring(3)) > -1)
				form.formAction(() => lastVersionLabel.Text = "ニコ生録画登録ツール（仮は最新バージョンです");
			else {
				form.formAction(() => {
                	lastVersionLabel.Text = v + "が利用可能です";
                	lastVersionLabel.Links.Clear();
                	lastVersionLabel.Links.Add(0, v.Length, m.Value);
                	//lastVersionLabel.LinkArea = new LinkArea(0, v.Length);
                });
			}
		}
		
		void LastVersionLabelLinkClicked(object _sender, LinkLabelLinkClickedEventArgs e)
		{
			util.debugWriteLine("click");
			LinkLabel sender = (LinkLabel)_sender;
			if (e.Button == MouseButtons.Left) {
				if (sender.Links.Count > 0 && sender.Links[0].Length != 0) {
					string url = (string)sender.Links[0].LinkData;
					if (bool.Parse(form.config.get("IsdefaultBrowserPath"))) {
						System.Diagnostics.Process.Start(url);
					} else {
						var p = form.config.get("browserPath");
						System.Diagnostics.Process.Start(p, url);
					}
				}
			} else {
//				if (sender.Links.Count > 0 && sender.Links[0].Length != 0) {
//					labelUrl = (string)sender.Links[0].LinkData;
//					mainWindowRightClickMenu.Show(Cursor.Position);
//				}
			}
		}
	}
}
