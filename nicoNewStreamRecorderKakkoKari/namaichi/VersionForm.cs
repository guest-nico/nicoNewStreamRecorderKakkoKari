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

namespace namaichi
{
	/// <summary>
	/// Description of VersionForm.
	/// </summary>
	public partial class VersionForm : Form
	{
		public VersionForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			versionLabel.Text = util.versionStr + " (" + util.versionDayStr + ")";
			//communityLinkLabel.Links.Add(0, communityLinkLabel.Text.Length, "http://com.nicovideo.jp/community/co2414037");
		}
		
		void okBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void communityLinkLabel_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://com.nicovideo.jp/community/co2414037");
		}
	}
}
