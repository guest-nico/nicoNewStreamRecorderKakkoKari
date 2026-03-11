/*
 * Created by SharpDevelop.
 * User: kogak
 * Date: 2025/06/02
 * Time: 0:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using rokugaTouroku.info;

namespace rokugaTouroku.gui
{
	/// <summary>
	/// Description of AppSettingForm.
	/// </summary>
	public partial class AppSettingForm : Form
	{
		//public Dictionary<string, string> ret = new Dictionary<string, string>();
		public AppSettingInfo ret = new AppSettingInfo();
		public bool isMin = false;
		string timeType = null;
		config.config cfg = null;
		
		public AppSettingForm(AppSettingInfo asi, config.config cfg, string timeType) {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.timeType = timeType;
			this.cfg = cfg;
			
			if (asi == null) return;
			
			if (string.IsNullOrEmpty(asi.baseDir))
				asi.baseDir = util.getJarPath()[0];
			baseDirectoryText.Text = asi.baseDir;
			useSubFolderChk.Checked = bool.Parse(asi.IscreateSubfolder);
			setSubFolderNameType(int.Parse(asi.subFolderNameType));
			setFileNameType(int.Parse(asi.fileNameType));
			ret.filenameformat = asi.filenameformat;
			fileNameTypeDokujiSetteiBtn.Text = util.getFileNameTypeSample(ret.filenameformat, false);
			extText.Text = asi.ext;
			//isMinimizedChkBox.Checked = asi.isMin;
			
			tabControl1.TabPages.RemoveAt(1);
		}
		void OkBtnClick(object sender, EventArgs e)
		{
			ret.baseDir = baseDirectoryText.Text;
			ret.IscreateSubfolder = useSubFolderChk.Checked.ToString();
			ret.subFolderNameType = getSubFolderNameType() + "";
			ret.fileNameType = getFileNameType() + "";
			ret.ext = extText.Text;
			//ret.isMin = isMinimizedChkBox.Checked;
			
			DialogResult = DialogResult.OK;
			Close();
		}
		void CancelBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		private void setSubFolderNameType(int subFolderNameType) {
        	if (subFolderNameType == 1) housoushaRadioBtn.Checked = true;
			else if (subFolderNameType == 2) userIDRadioBtn.Checked = true;
			else if (subFolderNameType == 3) userIDHousoushaRadioBtn.Checked = true;
			else if (subFolderNameType == 4) comNameRadioBtn.Checked = true;
			else if (subFolderNameType == 5) comIDRadioBtn.Checked = true;
			else if (subFolderNameType == 6) ComIDComNameRadioBtn.Checked = true;
			else if (subFolderNameType == 7) comIDHousoushaRadioBtn.Checked = true;
			else if (subFolderNameType == 8) housoushaComIDRadioBtn.Checked = true;
			else if (subFolderNameType == 9) housoushaUserIDRadioBtn.Checked = true;
			else housoushaRadioBtn.Checked = true;
        }
        private void setFileNameType(int nameType) {
        	if (nameType == 1) fileNameTypeRadioBtn0.Checked = true;
			else if (nameType == 2) fileNameTypeRadioBtn1.Checked = true;
			else if (nameType == 3) fileNameTypeRadioBtn2.Checked = true;
			else if (nameType == 4) fileNameTypeRadioBtn3.Checked = true;
			else if (nameType == 5) fileNameTypeRadioBtn4.Checked = true;
			else if (nameType == 6) fileNameTypeRadioBtn5.Checked = true;
			else if (nameType == 7) fileNameTypeRadioBtn6.Checked = true;
			else if (nameType == 8) fileNameTypeRadioBtn7.Checked = true;
			else if (nameType == 9) fileNameTypeRadioBtn8.Checked = true;
			else if (nameType == 10) fileNameTypeRadioBtn9.Checked = true;
			else fileNameTypeRadioBtn0.Checked = true;
        }
		int getSubFolderNameType() {
        	if (housoushaRadioBtn.Checked) return 1;
        	if (userIDRadioBtn.Checked) return 2;
        	if (userIDHousoushaRadioBtn.Checked) return 3;
        	if (comNameRadioBtn.Checked) return 4;
        	if (comIDRadioBtn.Checked) return 5;
        	if (ComIDComNameRadioBtn.Checked) return 6;
        	if (comIDHousoushaRadioBtn.Checked) return 7;
        	if (housoushaComIDRadioBtn.Checked) return 8;
        	if (housoushaUserIDRadioBtn.Checked) return 9;
        	return 1;
        }
        int getFileNameType() {
        	if (fileNameTypeRadioBtn0.Checked) return 1;
        	if (fileNameTypeRadioBtn1.Checked) return 2;
        	if (fileNameTypeRadioBtn2.Checked) return 3;
        	if (fileNameTypeRadioBtn3.Checked) return 4;
        	if (fileNameTypeRadioBtn4.Checked) return 5;
        	if (fileNameTypeRadioBtn5.Checked) return 6;
        	if (fileNameTypeRadioBtn6.Checked) return 7;
        	if (fileNameTypeRadioBtn7.Checked) return 8;
        	if (fileNameTypeRadioBtn8.Checked) return 9;
        	if (fileNameTypeRadioBtn9.Checked) return 10;
        	return 1;
        }
		void RecFolderSanshouBtnClick(object sender, EventArgs e)
		{
			var f = new FolderBrowserDialog();
			if (Directory.Exists(baseDirectoryText.Text))
				f.SelectedPath = baseDirectoryText.Text;
			DialogResult r = f.ShowDialog();
			
			util.debugWriteLine(f.SelectedPath);
			if (r == DialogResult.OK)
				baseDirectoryText.Text = f.SelectedPath;
		}
		void FileNameTypeDokujiSetteiBtnClick(object sender, EventArgs e)
		{
			
			var a = new fileNameOptionForm(ret.filenameformat, timeType, int.Parse(cfg.get("fontSize")));
			var res = a.ShowDialog();
			if (res != DialogResult.OK) return;
			fileNameTypeDokujiSetteiBtn.Text = util.getFileNameTypeSample(a.ret, false);
			ret.filenameformat = a.ret;
		}
	}
}
