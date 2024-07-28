/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/05/06
 * Time: 20:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using rokugaTouroku.config;
using rokugaTouroku.rec;
using SunokoLibrary.Application;
using SunokoLibrary.Application.Browsers;
using SunokoLibrary.Windows.ViewModels;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of optionForm.
	/// </summary>
	public partial class optionForm : Form
	{
		private config.config cfg;
		
		static readonly Uri TargetUrl = new Uri("https://live.nicovideo.jp/");
		private string fileNameFormat;
//		private string 
		
		public optionForm(config.config cfg)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.StartPosition = FormStartPosition.CenterParent;
			//util.debugWriteLine(p.X + " " + p.Y);
			InitializeComponent();
			//this.Location = p;
			this.cfg = cfg;
			
			nicoSessionComboBox1.Selector.PropertyChanged += Selector_PropertyChanged;
			nicoSessionComboBox2.Selector.PropertyChanged += Selector2_PropertyChanged;
			
			//tabControl1.TabPages.RemoveAt(6);
			setBackColor(Color.FromArgb(int.Parse(cfg.get("tourokuBackColor"))));
			setForeColor(Color.FromArgb(int.Parse(cfg.get("tourokuForeColor"))));
			
			util.setFontSize(int.Parse(cfg.get("fontSize")), this, false);
		}
		
		void hozonFolderSanshouBtn_Click(object sender, EventArgs e)
		{
			var f = new FolderBrowserDialog();
			if (System.IO.Directory.Exists(recordDirectoryText.Text))
				f.SelectedPath = recordDirectoryText.Text;
			DialogResult r = f.ShowDialog();
			
			util.debugWriteLine(f.SelectedPath);
			if (r == DialogResult.OK)
				recordDirectoryText.Text = f.SelectedPath;
		}
		
		void fileNameOptionBtn(object sender, EventArgs e)
		{
			
		}
		void FileNameDokujiSetteiBtn_Click(object sender, EventArgs e)
		{
			var a = new fileNameOptionForm(fileNameFormat, int.Parse(cfg.get("fontSize")));
			var res = a.ShowDialog();
			if (res != DialogResult.OK) return;
			fileNameTypeDokujiSetteiBtn.Text = util.getFileNameTypeSample(a.ret);
			fileNameFormat = a.ret;
		}
		
		void optionOk_Click(object sender, EventArgs e)
		{
			var formData = getFormData();
			cfg.saveFromForm(formData);
			Close();
			
			//main cookie
			var importer = nicoSessionComboBox1.Selector.SelectedImporter;
			if (importer == null || importer.SourceInfo == null) return;
			var si = importer.SourceInfo;
			
			if (isCookieFileSiteiChkBox.Checked)
				SourceInfoSerialize.save(si.GenerateCopy(si.BrowserName, si.ProfileName, cookieFileText.Text), false);
			else SourceInfoSerialize.save(si, false); 
			
			//sub cookie
			var importer2 = nicoSessionComboBox2.Selector.SelectedImporter;
			if (importer2 == null || importer2.SourceInfo == null) return;
			var si2 = importer2.SourceInfo;
			
			if (isCookieFileSiteiChkBox2.Checked)
				SourceInfoSerialize.save(si2.GenerateCopy(si2.BrowserName, si2.ProfileName, cookieFileText2.Text), true);
			else SourceInfoSerialize.save(si2, true);
			DialogResult = DialogResult.OK;
		}

		private Dictionary<string, string> getFormData() {
			//var selectedImporter = nicoSessionComboBox1.Selector.SelectedImporter;
//			var browserName = (selectedImporter != null) ? selectedImporter.SourceInfo.BrowserName : "";
			var browserNum = (useCookieRadioBtn.Checked) ? "2" : "1";
			var browserNum2 = (useCookieRadioBtn2.Checked) ? "2" : "1";
			return new Dictionary<string, string>(){
				{"accountId",mailText.Text},
				{"accountPass",passText.Text},
				//{"user_session",passText.Text},
				{"browserNum",browserNum},
//				{"isAllBrowserMode",checkBoxShowAll.Checked.ToString().ToLower()},
				{"issecondlogin",useSecondLoginChkBox.Checked.ToString().ToLower()},
				{"IsdefaultBrowserPath",isDefaultBrowserPathChkBox.Checked.ToString().ToLower()},
				{"browserPath",browserPathText.Text},
				{"Isminimized",isMinimizedChkBox.Checked.ToString().ToLower()},
				{"IscloseExit",isCloseExitChkBox.Checked.ToString().ToLower()},
				{"IsfailExit",isFailExit.Checked.ToString().ToLower()},
				{"IsgetComment",isGetCommentChkBox.Checked.ToString().ToLower()},
				{"IsmessageBox",isMessageBoxChkBox.Checked.ToString().ToLower()},
				{"IshosoInfo",isHosoInfoChkBox.Checked.ToString().ToLower()},
				{"IsDescriptionTag",isDescriptionTagChkBox.Checked.ToString().ToLower()},
//				{"Islog",isLogChkBox.Checked.ToString().ToLower()},
				{"IstitlebarInfo",isTitleBarInfoChkBox.Checked.ToString().ToLower()},
//				{"Islimitpopup",isLimitPopupChkBox.Checked.ToString().ToLower()},
				{"Isretry",isRetryChkBox.Checked.ToString().ToLower()},
				{"IsdeleteExit",isDeleteExitChkBox.Checked.ToString().ToLower()},
				{"IsgetcommentXml",(!isCommentJson.Checked).ToString().ToLower()},
				{"IsgetcommentXmlInfo",isCommentXmlInfo.Checked.ToString().ToLower()},
				{"IsCommentConvertSpace",isCommentConvertSpaceChkbox.Checked.ToString().ToLower()},
				{"commentConvertStr",commentConvertStrText.Text},
				{"IsSaveCommentOnlyRetryingRec",isSaveCommentOnlyRetryingRecChkBox.Checked.ToString().ToLower()},
				{"IsDisplayComment",isDisplayCommentChkbox.Checked.ToString().ToLower()},
				{"IsNormalizeComment",isNormalizeCommentChkBox.Checked.ToString().ToLower()},
				{"CommentReplaceText",getCommentReplaceText()},
				{"IstitlebarSamune",isTitleBarSamune.Checked.ToString().ToLower()},
				{"IsautoFollowComgen",isAutoFollowComGen.Checked.ToString().ToLower()},
				{"qualityRank",getQualityRank()},
				{"IsMiniStart",isMiniStartChkBox.Checked.ToString().ToLower()},
				{"IsConfirmCloseMsgBox",isConfirmCloseMsgBoxChkBox.Checked.ToString().ToLower()},
				{"IsRecBtnOnlyMouse",isRecBtnOnlyMouseChkBox.Checked.ToString().ToLower()},
				{"reserveMessage",reserveMessageList.Text},
				{"IsLogFile",isLogFileChkBox.Checked.ToString().ToLower()},
				{"IsNotSleep",isNotSleepChkBox.Checked.ToString().ToLower()},
				{"IsRestoreLocation",isRestoreLocationChkBox.Checked.ToString().ToLower()},
				
				{"IsSegmentNukeInfo",isSegmentNukeInfoChkBox.Checked.ToString().ToLower()},
				{"segmentSaveType",getSegmentSaveType()},
				{"IsRenketuAfter",isRenketuAfterChkBox.Checked.ToString().ToLower()},
//				{"IsAfterRenketuFFmpeg",isAfterRenketuFFmpegChkBox.Checked.ToString().ToLower()},
//				{"IsDefaultEngine",isDefaultEngineChkBox.Checked.ToString().ToLower()},
				{"EngineMode",getEngineMode()},
				{"anotherEngineCommand",anotherEngineCommandText.Text},
				{"IsDefaultRtmpPath",isDefaultRtmpChkBox.Checked.ToString().ToLower()},
				{"rtmpPath",rtmpPathText.Text},
				{"latency",latencyList.Text},
				{"IsChaseRecord",isChaseRecordRadioBtn.Checked.ToString().ToLower()},
				{"IsArgChaseRecFromFirst",isArgChaseRecFromFirstChkBox.Checked.ToString().ToLower()},
				{"IsOnlyTimeShiftChase",isOnlyTimeShiftChaseChkBtn.Checked.ToString().ToLower()},
				{"IsChaseReserveRec",isChaseReserveRecChkBox.Checked.ToString().ToLower()},
				
				{"IsUsePlayer",isUsePlayerChkBox.Checked.ToString().ToLower()},
				{"IsUseCommentViewer",isUseCommentViewerChkBox.Checked.ToString().ToLower()},
				{"IsDefaultPlayer",isDefaultPlayerRadioBtn.Checked.ToString().ToLower()},
				{"IsDefaultCommentViewer",isDefaultCommentViewerRadioBtn.Checked.ToString().ToLower()},
				{"anotherPlayerPath",anotherPlayerPathText.Text},
				{"anotherCommentViewerPath",anotherCommentViewerPathText.Text},
				{"afterConvertMode",getAfterConvertType()},
				{"afterConvertModeCmd",afterConvertModeCmdText.Text},
				{"IsSoundEnd",isSoundEndChkBox.Checked.ToString().ToLower()},
				{"soundPath",soundPathText.Text},
				{"IsSoundDefault",isDefaultSoundChkBtn.Checked.ToString().ToLower()},
				{"soundVolume",volumeBar.Value.ToString()},
				{"playerArgs",playerArgsText.Text},
				
				{"cookieFile",cookieFileText.Text},
				{"iscookie",isCookieFileSiteiChkBox.Checked.ToString().ToLower()},
				{"IsBrowserShowAll",checkBoxShowAll.Checked.ToString().ToLower()},
				{"recordDir",recordDirectoryText.Text},
				{"IsdefaultRecordDir",useDefaultRecFolderChk.Checked.ToString().ToLower()},
				{"IsSecondRecordDir",useSecondRecFolderChk.Checked.ToString().ToLower()},
				{"secondRecordDir",secondRecFolderText.Text},
				{"IscreateSubfolder",useSubFolderChk.Checked.ToString().ToLower()},
				{"subFolderNameType",getSubFolderNameType() + ""},
				{"fileNameType",getFileNameType() + ""},
				{"filenameformat",fileNameFormat},
				//{"ffmpegopt",ffmpegoptText.Text},
				{"user_session",""},
				{"user_session_secure",""},
				
				{"IsHokan",isHokanChkBox.Checked.ToString().ToLower()},
				{"accountId2",mailText2.Text},
				{"accountPass2",passText2.Text},
				{"browserNum2",browserNum2},
				{"issecondlogin2",useSecondLoginChkBox2.Checked.ToString().ToLower()},
				{"cookieFile2",cookieFileText2.Text},
				{"iscookie2",isCookieFileSiteiChkBox2.Checked.ToString().ToLower()},
				{"user_session2",""},
				{"user_session_secure2",""},
				
				{"chPlus_access_token",""},
				{"chPlus_cookie", ""},
				
				{"rokugaTourokuMaxRecordingNum",maxRecordingNum.Text},
				{"IsDuplicateConfirm",isDuplicateConfirmChkBox.Checked.ToString().ToLower()},
				{"IsDeleteConfirmMessageRt",isDeleteConfirmMessageRtCheckBtn.Checked.ToString().ToLower()},
				
				{"useProxy",useProxyChkBox.Checked.ToString().ToLower()},
				{"proxyAddress",proxyAddressText.Text},
				{"proxyPort",proxyPortText.Text},
				
				{"fontSize",fontList.Value.ToString()},
				{"IsTray",IsTrayChkBox.Checked.ToString().ToLower()},
			};
			
		}
		
		async void Selector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
			//if (isInitRun) initRec();
			try {
	            switch(e.PropertyName)
	            {
	                case "SelectedIndex":
	                    var cookieContainer = new CookieContainer();
	                    var currentGetter = nicoSessionComboBox1.Selector.SelectedImporter;
	                    if (currentGetter != null)
	                    {
	                        var result = await currentGetter.GetCookiesAsync(TargetUrl);
	                        
	                        var cookie = result.Status == CookieImportState.Success ? result.Cookies["user_session"] : null;
	//                        foreach (var c in result.Cookies)
	//                        	util.debugWriteLine(c);
	                        //logText.Text += cookie.Name + cookie.Value+ cookie.Expires;
	                        
	                        //UI更新
	//                        txtCookiePath.Text = currentGetter.SourceInfo.CookiePath;
	//                        btnOpenCookieFileDialog.Enabled = true;
	//                        txtUserSession.Text = cookie != null ? cookie.Value : null;
	//                        txtUserSession.Enabled = result.Status == CookieImportState.Success;
	                        //Properties.Settings.Default.SelectedSourceInfo = currentGetter.SourceInfo;
	                        //Properties.Settings.Default.Save();
	                        //cfg.set("browserNum", nicoSessionComboBox1.Selector.SelectedIndex.ToString());
	                        //if (cookie != null) cfg.set("user_session", cookie.Value);
	                        //cfg.set("isAllBrowserMode", nicoSessionComboBox1.Selector.IsAllBrowserMode.ToString());
	                    }
	                    else
	                    {
	//                        txtCookiePath.Text = null;
	//                        txtUserSession.Text = null;
	//                        txtUserSession.Enabled = false;
	//                        btnOpenCookieFileDialog.Enabled = false;
	                    }
	                    break;                    
	            }
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
        }
		async void Selector2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
			//if (isInitRun) initRec();
			
            switch(e.PropertyName)
            {
                case "SelectedIndex":
                    var cookieContainer = new CookieContainer();
                    var currentGetter = nicoSessionComboBox2.Selector.SelectedImporter;
                    if (currentGetter != null)
                    {
                        var result = await currentGetter.GetCookiesAsync(TargetUrl);
                        
                        var cookie = result.Status == CookieImportState.Success ? result.Cookies["user_session"] : null;
//                        foreach (var c in result.Cookies)
//                        	util.debugWriteLine(c);
                        //logText.Text += cookie.Name + cookie.Value+ cookie.Expires;
                        
                        //UI更新
//                        txtCookiePath.Text = currentGetter.SourceInfo.CookiePath;
//                        btnOpenCookieFileDialog.Enabled = true;
//                        txtUserSession.Text = cookie != null ? cookie.Value : null;
//                        txtUserSession.Enabled = result.Status == CookieImportState.Success;
                        //Properties.Settings.Default.SelectedSourceInfo = currentGetter.SourceInfo;
                        //Properties.Settings.Default.Save();
                        //cfg.set("browserNum", nicoSessionComboBox1.Selector.SelectedIndex.ToString());
                        //if (cookie != null) cfg.set("user_session", cookie.Value);
                        //cfg.set("isAllBrowserMode", nicoSessionComboBox1.Selector.IsAllBrowserMode.ToString());
                    }
                    else
                    {
//                        txtCookiePath.Text = null;
//                        txtUserSession.Text = null;
//                        txtUserSession.Enabled = false;
//                        btnOpenCookieFileDialog.Enabled = false;
                    }
                    break;
            }
        }
		void btnReload_Click(object sender, EventArgs e)
        { 
			//var si = nicoSessionComboBox1.Selector.SelectedImporter.SourceInfo;
			//util.debugWriteLine(si.EngineId + " " + si.BrowserName + " " + si.ProfileName);
//			var a = new SunokoLibrary.Application.Browsers.FirefoxImporterFactory();
//			foreach (var b in a.GetCookieImporters()) {
//				var c = b.GetCookiesAsync(TargetUrl);
//				c.ConfigureAwait(false);
				
//				util.debugWriteLine(c.Result.Cookies["user_session"]);
//			}
				
			//SmartImporterFactory.blinkWithoutPathList.Clear();
			//SmartImporterFactory.geckoWithoutPathList.Clear();
			var tsk = nicoSessionComboBox1.Selector.UpdateAsync(); 
		}
		void btnReload2_Click(object sender, EventArgs e)
        { 
			//var si = nicoSessionComboBox1.Selector.SelectedImporter.SourceInfo;
			//util.debugWriteLine(si.EngineId + " " + si.BrowserName + " " + si.ProfileName);
//			var a = new SunokoLibrary.Application.Browsers.FirefoxImporterFactory();
//			foreach (var b in a.GetCookieImporters()) {
//				var c = b.GetCookiesAsync(TargetUrl);
//				c.ConfigureAwait(false);
				
//				util.debugWriteLine(c.Result.Cookies["user_session"]);
//			}
				
			//SmartImporterFactory.blinkWithoutPathList.Clear();
			//SmartImporterFactory.geckoWithoutPathList.Clear();
			var tsk = nicoSessionComboBox2.Selector.UpdateAsync(); 
		}
        void btnOpenCookieFileDialog_Click(object sender, EventArgs e)
        { var tsk = nicoSessionComboBox1.ShowCookieDialogAsync(); }
        void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
        { nicoSessionComboBox1.Selector.IsAllBrowserMode = checkBoxShowAll.Checked;
//        	cfg.set("isAllBrowserMode", nicoSessionComboBox1.Selector.IsAllBrowserMode.ToString());
        }
        void btnOpenCookieFileDialog2_Click(object sender, EventArgs e)
        { var tsk = nicoSessionComboBox2.ShowCookieDialogAsync(); }
        void checkBoxShowAll2_CheckedChanged(object sender, EventArgs e)
        { nicoSessionComboBox2.Selector.IsAllBrowserMode = checkBoxShowAll2.Checked;
//        	cfg.set("isAllBrowserMode", nicoSessionComboBox1.Selector.IsAllBrowserMode.ToString());
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
        private void setFormFromConfig() {
        	mailText.Text = cfg.get("accountId");
        	passText.Text = cfg.get("accountPass");
        	
        	if (cfg.get("browserNum") == "1") useAccountLoginRadioBtn.Checked = true;
        	else useCookieRadioBtn.Checked = true; 
        	useSecondLoginChkBox.Checked = bool.Parse(cfg.get("issecondlogin"));
        	isDefaultBrowserPathChkBox.Checked = bool.Parse(cfg.get("IsdefaultBrowserPath"));
        	isDefaultBrowserPathChkBox_UpdateAction();
        	browserPathText.Text = cfg.get("browserPath");
        	isMinimizedChkBox.Checked = bool.Parse(cfg.get("Isminimized"));
        	isCloseExitChkBox.Checked = bool.Parse(cfg.get("IscloseExit"));
        	isFailExit.Checked = bool.Parse(cfg.get("IsfailExit"));
        	isGetCommentChkBox.Checked = bool.Parse(cfg.get("IsgetComment"));
        	isMessageBoxChkBox.Checked = bool.Parse(cfg.get("IsmessageBox"));
        	isHosoInfoChkBox.Checked = bool.Parse(cfg.get("IshosoInfo"));
        	isDescriptionTagChkBox.Checked = bool.Parse(cfg.get("IsDescriptionTag"));
        	isDescriptionChkBox_UpdateAction();
//        	isLogChkBox.Checked = bool.Parse(cfg.get("Islog"));
        	isTitleBarInfoChkBox.Checked = bool.Parse(cfg.get("IstitlebarInfo"));
//        	isLimitPopupChkBox.Checked = bool.Parse(cfg.get("Islimitpopup"));
        	isRetryChkBox.Checked = bool.Parse(cfg.get("Isretry"));
        	isDeleteExitChkBox.Checked = bool.Parse(cfg.get("IsdeleteExit"));
        	setCommentChkBox();
        	isGetCommentChkBox_UpdateAction();
        	isCommentConvertSpaceChkbox.Checked = bool.Parse(cfg.get("IsCommentConvertSpace"));
        	commentConvertStrText.Text = cfg.get("commentConvertStr");
        	isSaveCommentOnlyRetryingRecChkBox.Checked = bool.Parse(cfg.get("IsSaveCommentOnlyRetryingRec"));
        	isDisplayCommentChkbox.Checked = bool.Parse(cfg.get("IsDisplayComment"));
        	isNormalizeCommentChkBox.Checked = bool.Parse(cfg.get("IsNormalizeComment"));
        	setCommentReplaceTextForm(cfg.get("CommentReplaceText"));
        	isTitleBarSamune.Checked = bool.Parse(cfg.get("IstitlebarSamune"));
        	isAutoFollowComGen.Checked = bool.Parse(cfg.get("IsautoFollowComgen"));
        	setInitQualityRankList(cfg.get("qualityRank"));
        	isMiniStartChkBox.Checked = bool.Parse(cfg.get("IsMiniStart"));
        	isConfirmCloseMsgBoxChkBox.Checked = bool.Parse(cfg.get("IsConfirmCloseMsgBox"));
        	isRecBtnOnlyMouseChkBox.Checked = bool.Parse(cfg.get("IsRecBtnOnlyMouse"));
        	reserveMessageList.Text = string.IsNullOrEmpty(cfg.get("reserveMessage")) ? "ダイアログで確認" : cfg.get("reserveMessage");
        	isLogFileChkBox.Checked = bool.Parse(cfg.get("IsLogFile"));
        	isNotSleepChkBox.Checked = bool.Parse(cfg.get("IsNotSleep"));
        	isRestoreLocationChkBox.Checked = bool.Parse(cfg.get("IsRestoreLocation"));
        	
        	isSegmentNukeInfoChkBox.Checked = bool.Parse(cfg.get("IsSegmentNukeInfo"));
        	setSegmentSaveType(cfg.get("segmentSaveType"));
        	isRenketuAfterChkBox.Checked = bool.Parse(cfg.get("IsRenketuAfter"));
        	isRenketuAfterChkBox_UpdateAction();
//        	isAfterRenketuFFmpegChkBox.Checked = bool.Parse(cfg.get("IsAfterRenketuFFmpeg"));
//        	isDefaultEngineChkBox.Checked = bool.Parse(cfg.get("IsDefaultEngine"));
        	setEngineType(cfg.get("EngineMode"));
			anotherEngineCommandText.Text = cfg.get("anotherEngineCommand");
			isDefaultRtmpChkBox.Checked = bool.Parse(cfg.get("IsDefaultRtmpPath"));
			rtmpPathText.Text = cfg.get("rtmpPath");
			isDefaultEngineChkBox_UpdateAction();
			latencyList.Text = cfg.get("latency");
			isChaseRecordRadioBtn.Checked = bool.Parse(cfg.get("IsChaseRecord"));
			isArgChaseRecFromFirstChkBox.Checked = bool.Parse(cfg.get("IsArgChaseRecFromFirst"));
			isOnlyTimeShiftChaseChkBtn.Checked = bool.Parse(cfg.get("IsOnlyTimeShiftChase"));
			isChaseRecordRadioBtn_UpdateAction();
			isChaseReserveRecChkBox.Checked = bool.Parse(cfg.get("IsChaseReserveRec"));
			
			setPlayerType();
			setCommentViewerType();
			anotherPlayerPathText.Text = cfg.get("anotherPlayerPath");
			anotherCommentViewerPathText.Text = cfg.get("anotherCommentViewerPath");
			isUsePlayerChkBox.Checked = bool.Parse(cfg.get("IsUsePlayer"));
			isUseCommentViewerChkBox.Checked = bool.Parse(cfg.get("IsUseCommentViewer"));
			playerArgsText.Text = cfg.get("playerArgs");
			isUsePlayerChkBox_UpdateAction();
			isUseCommentViewerChkBox_UpdateAction();
			
			setConvertList(int.Parse(cfg.get("afterConvertMode")));
			afterConvertModeCmdText.Text = cfg.get("afterConvertModeCmd");
			isSoundEndChkBox.Checked = bool.Parse(cfg.get("IsSoundEnd"));
			soundPathText.Text = cfg.get("soundPath");
			isDefaultSoundChkBtn.Checked = bool.Parse(cfg.get("IsSoundDefault"));
			volumeBar.Value = int.Parse(cfg.get("soundVolume"));
			updateIsSoundEndChkBox();
			
        	isCookieFileSiteiChkBox.Checked = bool.Parse(cfg.get("iscookie"));
        	isCookieFileSiteiChkBox_UpdateAction();
        	cookieFileText.Text = cfg.get("cookieFile");
        	recordDirectoryText.Text = cfg.get("recordDir");
        	useDefaultRecFolderChk.Checked = bool.Parse(cfg.get("IsdefaultRecordDir"));
        	useDefaultRecFolderChkBox_UpdateAction();
        	useSecondRecFolderChk.Checked = bool.Parse(cfg.get("IsSecondRecordDir"));
        	secondRecFolderText.Text = cfg.get("secondRecordDir");
        	UseSecondRecFolderChkBox_UpdateAction();
        	useSubFolderChk.Checked = bool.Parse(cfg.get("IscreateSubfolder"));
        	useSubFolderChk_UpdateAction();
        	setSubFolderNameType(int.Parse(cfg.get("subFolderNameType")));
        	setFileNameType(int.Parse(cfg.get("fileNameType")));
        	fileNameFormat = cfg.get("filenameformat");
        	fileNameTypeDokujiSetteiBtn.Text = util.getFileNameTypeSample(fileNameFormat);
        	//ffmpegoptText.Text = cfg.get("ffmpegopt");
        	
        	isHokanChkBox.Checked = bool.Parse(cfg.get("IsHokan"));
        	isSubHokanChkBox_updateAction();
        	mailText2.Text = cfg.get("accountId2");
        	passText2.Text = cfg.get("accountPass2");
        	if (cfg.get("browserNum2") == "1") useAccountLoginRadioBtn2.Checked = true;
        	else useCookieRadioBtn2.Checked = true; 
        	useSecondLoginChkBox2.Checked = bool.Parse(cfg.get("issecondlogin2"));
        	isCookieFileSiteiChkBox2.Checked = bool.Parse(cfg.get("iscookie2"));
        	isCookieFileSiteiChkBox2_UpdateAction();
        	cookieFileText2.Text = cfg.get("cookieFile2");
        	checkBoxShowAll.Checked = bool.Parse(cfg.get("IsBrowserShowAll"));
        	
        	var si = SourceInfoSerialize.load(false);
        	nicoSessionComboBox1.Selector.SetInfoAsync(si);
        	var si2 = SourceInfoSerialize.load(true);
        	nicoSessionComboBox2.Selector.SetInfoAsync(si2);
        	
        	maxRecordingNum.Text= cfg.get("rokugaTourokuMaxRecordingNum");
        	isDuplicateConfirmChkBox.Checked = bool.Parse(cfg.get("IsDuplicateConfirm"));
        	isDeleteConfirmMessageRtCheckBtn.Checked = bool.Parse(cfg.get("IsDeleteConfirmMessageRt"));
        	
        	proxyAddressText.Text = cfg.get("proxyAddress");
        	proxyPortText.Text = cfg.get("proxyPort");
        	useProxyChkBox.Checked = bool.Parse(cfg.get("useProxy"));
        	
        	fontList.Value = decimal.Parse(cfg.get("fontSize"));
        	IsTrayChkBox.Checked = bool.Parse(cfg.get("IsTray"));
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
		
		void optionCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
		
		void cookieFileSiteiSanshouBtn_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Multiselect = false;
			var result = dialog.ShowDialog();
			if (result != DialogResult.OK) return;
			
			cookieFileText.Text = dialog.FileName;
		}
		void cookieFileSiteiSanshouBtn2_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Multiselect = false;
			var result = dialog.ShowDialog();
			if (result != DialogResult.OK) return;
			
			cookieFileText2.Text = dialog.FileName;
		}
		void isCookieFileSiteiChkBox_CheckedChanged(object sender, EventArgs e)
		{
			isCookieFileSiteiChkBox_UpdateAction();
		}
		void isCookieFileSiteiChkBox2_CheckedChanged(object sender, EventArgs e)
		{
			isCookieFileSiteiChkBox2_UpdateAction();
		}
		void isCookieFileSiteiChkBox_UpdateAction() {
//			cookieFileText.Enabled = isCookieFileSiteiChkBox.Checked;
//			cookieFileSanshouBtn.Enabled = isCookieFileSiteiChkBox.Checked;
		}
		void isCookieFileSiteiChkBox2_UpdateAction() {
//			cookieFileText.Enabled = isCookieFileSiteiChkBox.Checked;
//			cookieFileSanshouBtn.Enabled = isCookieFileSiteiChkBox.Checked;
		}
		void useDefaultRecFolderChkBox_CheckedChanged(object sender, EventArgs e)
		{
			useDefaultRecFolderChkBox_UpdateAction();
		}
		void useDefaultRecFolderChkBox_UpdateAction()
		{
			recordDirectoryText.Enabled = !useDefaultRecFolderChk.Checked;
			recFolderSanshouBtn.Enabled = !useDefaultRecFolderChk.Checked;
		}
		
		void useSubFolderChk_CheckedChanged(object sender, EventArgs e)
		{
			useSubFolderChk_UpdateAction();
		}
		void useSubFolderChk_UpdateAction()
		{
			/*
			housoushaRadioBtn.Enabled = useSubFolderChk.Checked;
			userIDRadioBtn.Enabled = useSubFolderChk.Checked;
			userIDHousoushaRadioBtn.Enabled = useSubFolderChk.Checked;
			comNameRadioBtn.Enabled = useSubFolderChk.Checked;
			comIDRadioBtn.Enabled = useSubFolderChk.Checked;
			ComIDComNameRadioBtn.Enabled = useSubFolderChk.Checked;
			comIDHousoushaRadioBtn.Enabled = useSubFolderChk.Checked;
			housoushaComIDRadioBtn.Enabled = useSubFolderChk.Checked;
			*/
		}
		
		void loginBtn_Click(object sender, EventArgs e)
		{
			
			var cg = new rec.CookieGetter(cfg);
			var cc = cg.getAccountCookie(mailText.Text, passText.Text);
			if (cc == null) {
				util.showMessageBoxCenterForm(this, "login error3 " + cg.log, "", MessageBoxButtons.OK);
				util.dllCheck(new MainForm(null));
				return;
			}
			if (cc.GetCookies(TargetUrl)["user_session"] == null &&
				                   cc.GetCookies(TargetUrl)["user_session_secure"] == null)
				util.showMessageBoxCenterForm(this, "no login", "", MessageBoxButtons.OK);
			else util.showMessageBoxCenterForm(this, "login ok", "", MessageBoxButtons.OK);
			
			//util.showMessageBoxCenterForm(this, "aa");
		}
		void loginBtn2_Click(object sender, EventArgs e)
		{
			
			var cg = new rec.CookieGetter(cfg);
			var cc = cg.getAccountCookie(mailText2.Text, passText2.Text);
			if (cc == null) {
				util.showMessageBoxCenterForm(this, "login error4 " + cg.log, "", MessageBoxButtons.OK);
				return;
			}
			if (cc.GetCookies(TargetUrl)["user_session"] == null &&
				                   cc.GetCookies(TargetUrl)["user_session_secure"] == null)
				util.showMessageBoxCenterForm(this, "no login", "", MessageBoxButtons.OK);
			else util.showMessageBoxCenterForm(this, "login ok", "", MessageBoxButtons.OK);
			
			//util.showMessageBoxCenterForm(this, "aa");
		}
		
		void isDefaultBrowserPathChkBox_CheckedChanged(object sender, EventArgs e)
		{
			isDefaultBrowserPathChkBox_UpdateAction();
		}
		void isDefaultBrowserPathChkBox_UpdateAction()
		{
			browserPathText.Enabled = !isDefaultBrowserPathChkBox.Checked;
			browserPathSanshouBtn.Enabled = !isDefaultBrowserPathChkBox.Checked;
		}
		
		
		void browserPathSanshouBtn_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Multiselect = false;
			var result = dialog.ShowDialog();
			if (result != DialogResult.OK) return;
			
			browserPathText.Text = dialog.FileName;
		}
		
		void isGetCommentChkBox_CheckedChanged(object sender, EventArgs e)
		{
			isGetCommentChkBox_UpdateAction();
		}
		void isGetCommentChkBox_UpdateAction()
		{
			isCommentXML.Enabled = isCommentJson.Enabled = 
					isCommentXmlInfo.Enabled = 
						isGetCommentChkBox.Checked;
		}
		
		void highRankBtn_Click(object sender, EventArgs e)
		{
			List<int> ranks = new List<int>() {7,6,8,0,1,2,3,4,5,9};
			for (var i = ranks.Count; i < config.config.qualityList.Count; i++)
				ranks.Add(i);
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
		}
		void lowRankBtn_Click(object sender, EventArgs e)
		{
			List<int> ranks = new List<int>() {9, 5, 4, 3, 2, 1, 0, 8, 6, 7};
			for (var i = ranks.Count; i < config.config.qualityList.Count; i++)
				ranks.Add(i);
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
		}
		public object[] getRanksToItems(int[] ranks, ListBox owner) {
			var items = config.config.qualityList;
			/*
			var items = new Dictionary<int, string> {
				//{0, "自動(abr)"}, 
				{0, "3Mbps(super_high)"},
				{1, "2Mbps(high)"}, {2, "1Mbps(normal)"},
				{3, "384kbps(low)"}, {4, "192kbps(super_low)"},
				{5, "音声のみ(audio_high)"}, {6, "6Mbps(6Mbps1080p30fps)"} 
			};
			*/
//			var ret = new ListBox.ObjectCollection(owner);
			var ret = new List<object>();
			for (int i = 0; i < ranks.Length; i++) {
				if (ranks[i] >= items.Count) continue;
				ret.Add((i + 1) + ". " + items[ranks[i]]);
			}
			foreach (var k in items.Keys)
				if (Array.IndexOf(ranks, k) == -1)
					ret.Add((ret.Count + 1) + ". " + items[k]);
			return ret.ToArray();
		}
		void UpBtn_Click(object sender, EventArgs e)
		{
			var selectedIndex = qualityListBox.SelectedIndex;
			if (selectedIndex < 1) return;
			
			var ranks = getItemsToRanks(qualityListBox.Items);
			var selectedVal = ranks[selectedIndex + 0];
			ranks.RemoveAt(selectedIndex);
			var addIndex = (selectedIndex == 0) ? 0 : (selectedIndex - 1);
			ranks.Insert(addIndex, selectedVal);
			
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
			qualityListBox.SetSelected(addIndex, true);
		}
		void DownBtn_Click(object sender, EventArgs e)
		{
			var selectedIndex = qualityListBox.SelectedIndex;
			var itemCount = qualityListBox.Items.Count;
			if (selectedIndex > itemCount - 2) return;
			
			var ranks = getItemsToRanks(qualityListBox.Items);
			var selectedVal = ranks[selectedIndex + 0];
			ranks.RemoveAt(selectedIndex);
			var addIndex = (selectedIndex == itemCount) ? itemCount : (selectedIndex + 1);
			ranks.Insert(addIndex, selectedVal);
			
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
			qualityListBox.SetSelected(addIndex, true);
		}
		List<int> getItemsToRanks(ListBox.ObjectCollection items) {
			var itemsDic = config.config.qualityList;
			/*
			var items = new Dictionary<int, string> {
				//{0, "自動(abr)"}, 
				{0, "3Mbps(super_high)"},
				{1, "2Mbps(high)"}, {2, "1Mbps(normal)"},
				{3, "384kbps(low)"}, {4, "192kbps(super_low)"},
				{5, "音声のみ(audio_high)"}, {6, "6Mbps(6Mbps1080p30fps)"} 
			};
			*/
			var ret = new List<int>();
			for (int i = 0; i < items.Count; i++) {
				foreach (KeyValuePair <int, string> p in itemsDic) {
					var itemName = util.getRegGroup(items[i].ToString(), " (.+)");
					if (p.Value == itemName) ret.Add(p.Key);
				}
			}
			return ret;
		}
		string getQualityRank() {
			var buf = getItemsToRanks(qualityListBox.Items);
			var ret = "";
			foreach (var r in buf) {
				if (ret != "") ret += ",";
				ret += r;
			}
			return ret;
		}
		void setInitQualityRankList(string qualityRank) {
			var ranks = new List<int>();
			foreach (var r in qualityRank.Split(','))
				ranks.Add(int.Parse(r));
//			ranks.AddRange(qualityRank.Split(','));
			
			qualityListBox.Items.Clear();
			/*
			if (ranks.Count == 6) {
				ranks.Remove(0);
				for (var i = 0; i < ranks.Count; i++) ranks[i] -= 1;
			}
			*/
			var items = getRanksToItems(ranks.ToArray(), qualityListBox);
			qualityListBox.Items.AddRange(items);
		}
		string getSegmentSaveType() {
			if (isSegmentRenketuRadioBtn.Checked) return "0";
			else if (isSegmentNotRenketuRadioBtn.Checked) return "1";
//			else if (isSegmentBothRadioBtn.Checked) return "2";
			return "0";
		}
		void setSegmentSaveType(string segmentSaveType) {
			if (segmentSaveType == "0") isSegmentRenketuRadioBtn.Checked = true;
			else if (segmentSaveType == "1") isSegmentNotRenketuRadioBtn.Checked = true;
//			else if (segmentSaveType == "2") isSegmentBothRadioBtn.Checked = true;
		}
		void isSegmentNotRenketuRadioBtn_CheckedChanged(object sender, EventArgs e)
		{
			isRenketuAfterChkBox_UpdateAction();
		}
		void isRenketuAfterChkBox_UpdateAction() {
			isRenketuAfterChkBox.Enabled = isSegmentNotRenketuRadioBtn.Checked;
		}
		void isDefaultEngineChkBox_UpdateAction() {
			if (isDefaultEngineChkBox.Checked) {
				anotherEngineCommandText.Enabled = false;
				isSegmentRenketuRadioBtn.Enabled = true;
				isSegmentNotRenketuRadioBtn.Enabled = true;
				isRenketuAfterChkBox.Enabled = isDefaultEngineChkBox.Checked
						&& isSegmentNotRenketuRadioBtn.Checked;
				//isRenketuAfterChkBox_UpdateAction();
				
				rtmpPathText.Enabled = rtmpPathSanshouBtn.Enabled = 
					isDefaultRtmpChkBox.Enabled = false;
			} else if (isAnotherEngineChkBox.Checked) {
				anotherEngineCommandText.Enabled = true;
				isSegmentRenketuRadioBtn.Enabled = false;
				isSegmentNotRenketuRadioBtn.Enabled = false;
				isRenketuAfterChkBox.Enabled = false;
				
				rtmpPathText.Enabled = rtmpPathSanshouBtn.Enabled = 
					isDefaultRtmpChkBox.Enabled = false;
			} else if (isRtmpEngine.Checked) {
				anotherEngineCommandText.Enabled = false;
				isSegmentRenketuRadioBtn.Enabled = false;
				isSegmentNotRenketuRadioBtn.Enabled = false;
				isRenketuAfterChkBox.Enabled = false;
				
				rtmpPathText.Enabled = rtmpPathSanshouBtn.Enabled =
						isRtmpEngine.Checked && !isDefaultRtmpChkBox.Checked;
				isDefaultRtmpChkBox.Enabled = true;
			} else {
				anotherEngineCommandText.Enabled = false;
				isSegmentRenketuRadioBtn.Enabled = false;
				isSegmentNotRenketuRadioBtn.Enabled = false;
				isRenketuAfterChkBox.Enabled = false;
				
				rtmpPathText.Enabled = rtmpPathSanshouBtn.Enabled = 
					isDefaultRtmpChkBox.Enabled = false;
			}
		}
		void setEngineType(string EngineMode) {
			if (EngineMode == "0") isDefaultEngineChkBox.Checked = true;
			else if (EngineMode == "1") isAnotherEngineChkBox.Checked = true;
			else if (EngineMode == "2") isRtmpEngine.Checked = true;
			else isNoEngine.Checked = true;
			isDefaultEngineChkBox_UpdateAction();
		}
		
		void isDefaultEngineChkBox_CheckedChanged(object sender, EventArgs e)
		{
			isDefaultEngineChkBox_UpdateAction();
		}
		void setPlayerType() {
			if (bool.Parse(cfg.get("IsDefaultPlayer")))
				isDefaultPlayerRadioBtn.Checked = true;
			else  
				isAnotherPlayerRadioBtn.Checked = true;
			isDefaultPlayerRadioBtn_UpdateAction();
		}
		void setCommentViewerType() {
			if (bool.Parse(cfg.get("IsDefaultCommentViewer")))
				isDefaultCommentViewerRadioBtn.Checked = true;
			else 
				isAnotherCommentViewerRadioBtn.Checked = true;
			isDefaultCommentViewerRadioBtn_UpdateAction();
		}
		void isDefaultPlayerRadioBtn_UpdateAction() {
			if (isDefaultPlayerRadioBtn.Checked) {
				anotherPlayerPathText.Enabled = false;
				anotherPlayerSanshouBtn.Enabled = false;
			} else {
				anotherPlayerPathText.Enabled = true;
				anotherPlayerSanshouBtn.Enabled = true;
			}
		}
		void isDefaultCommentViewerRadioBtn_UpdateAction() {
			if (isDefaultCommentViewerRadioBtn.Checked) {
				anotherCommentViewerPathText.Enabled = false;
				anotherCommentViewerSanshouBtn.Enabled = false;
			} else {
				anotherCommentViewerPathText.Enabled = true;
				anotherCommentViewerSanshouBtn.Enabled = true;
			}
		}
		void isDefaultPlayerRadioBtn_CheckedChanged(object sender, EventArgs e)
		{
			isDefaultPlayerRadioBtn_UpdateAction();
		}
		void isDefaultCommentViewerRadioBtn_CheckedChanged(object sender, EventArgs e)
		{
			isDefaultCommentViewerRadioBtn_UpdateAction();
		}
		void isHosoInfoChkBox_CheckedChanged(object sender, EventArgs e)
		{
			isDescriptionChkBox_UpdateAction();
		}
		void isDescriptionChkBox_UpdateAction() {
			isDescriptionTagChkBox.Enabled = isHosoInfoChkBox.Checked;
		}
		void anotherPlayerSanshouBtn_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Multiselect = false;
			var result = dialog.ShowDialog();
			if (result != DialogResult.OK) return;
			
			anotherPlayerPathText.Text = dialog.FileName;
		}
		void anotherCommentViewerSanshouBtn_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Multiselect = false;
			var result = dialog.ShowDialog();
			if (result != DialogResult.OK) return;
			
			anotherCommentViewerPathText.Text = dialog.FileName;
		}
		void setConvertList(int afterConvertMode) {
			var t = "処理しない";
			if (afterConvertMode == 1) t = "形式を変更せず処理する";
			if (afterConvertMode == 2) t = "ts";
			if (afterConvertMode == 3) t = "avi";			
			if (afterConvertMode == 4) t = "mp4";
			if (afterConvertMode == 5) t = "flv";
			if (afterConvertMode == 6) t = "mov";
			if (afterConvertMode == 7) t = "wmv";
			if (afterConvertMode == 8) t = "vob";
			if (afterConvertMode == 9) t = "mkv";
			if (afterConvertMode == 10) t = "mp3(音声)";
			if (afterConvertMode == 11) t = "wav(音声)";
			if (afterConvertMode == 12) t = "wma(音声)";
			if (afterConvertMode == 13) t = "aac(音声)";
			if (afterConvertMode == 14) t = "ogg(音声)";
			if (afterConvertMode == 15) t = "mp4(再エンコード)";
			afterConvertModeList.Text = t;
		}
		private string getAfterConvertType() {
			var t = afterConvertModeList.Text;
			if (t == "処理しない") return "0";
			if (t == "形式を変更せず処理する") return "1";
			if (t == "ts") return "2";
			if (t == "avi") return "3";
			if (t == "mp4") return "4";
			if (t == "flv") return "5";
			if (t == "mov") return "6";
			if (t == "wmv") return "7";
			if (t == "vob") return "8";
			if (t == "mkv") return "9";
			if (t == "mp3(音声)") return "10";
			if (t == "wav(音声)") return "11";
			if (t == "wma(音声)") return "12";
			if (t == "aac(音声)") return "13";
			if (t == "ogg(音声)") return "14";
			if (t == "mp4(再エンコード)") return "15";
			return "0";
		}
		
		
		void isSubHokanChkBox_CheckedChanged(object sender, EventArgs e)
		{
			isSubHokanChkBox_updateAction();
		}
		void isSubHokanChkBox_updateAction() {
			var _checked = isHokanChkBox.Checked; 
			useCookieRadioBtn2.Enabled = _checked;
			checkBoxShowAll2.Enabled = _checked;
			nicoSessionComboBox2.Enabled = _checked;
			btnReload2.Enabled = _checked;
			isCookieFileSiteiChkBox2.Enabled = _checked;
			cookieFileText2.Enabled = _checked;
			cookieFileSanshouBtn2.Enabled = _checked;
			useSecondLoginChkBox2.Enabled = _checked;
			useAccountLoginRadioBtn2.Enabled = _checked;
			mailText2.Enabled = _checked;
			passText2.Enabled = _checked;
			loginBtn2.Enabled = _checked;
			subMailLabel2.Enabled = _checked;
			subPassLabel2.Enabled = _checked;
		}
		void IsUsePlayerChkBoxCheckedChanged(object sender, EventArgs e)
		{
			isUsePlayerChkBox_UpdateAction();
		}
		void IsUseCommentViewerChkBoxCheckedChanged(object sender, EventArgs e)
		{
			isUseCommentViewerChkBox_UpdateAction();
		}
		void isUsePlayerChkBox_UpdateAction() {
			var c = isUsePlayerChkBox.Checked;
			isDefaultPlayerRadioBtn.Enabled = c;
			isAnotherPlayerRadioBtn.Enabled = c;
			anotherPlayerPathText.Enabled = c && isAnotherPlayerRadioBtn.Checked;
			anotherPlayerSanshouBtn.Enabled = c && isAnotherPlayerRadioBtn.Checked;
			playerArgsText.Enabled = c;			
		}
		void isUseCommentViewerChkBox_UpdateAction() {
			var c = isUseCommentViewerChkBox.Checked;
			isDefaultCommentViewerRadioBtn.Enabled = c;
			isAnotherCommentViewerRadioBtn.Enabled = c;
			anotherCommentViewerPathText.Enabled = c && isAnotherCommentViewerRadioBtn.Checked;
			anotherCommentViewerSanshouBtn.Enabled = c && isAnotherCommentViewerRadioBtn.Checked;
		}
		string getEngineMode() {
			if (isAnotherEngineChkBox.Checked) return "1";
			if (isRtmpEngine.Checked) return "2";
			if (isNoEngine.Checked) return "3";
			if (isDefaultEngineChkBox.Checked) return "0";
			return "0";
		}
		void IsSoundEndChkBoxCheckedChanged(object sender, EventArgs e)
		{
			updateIsSoundEndChkBox();
		}
		void updateIsSoundEndChkBox() {
			soundPathText.Enabled = 
				isSoundEndChkBox.Checked && !isDefaultSoundChkBtn.Checked;
			soundSanshouBtn.Enabled = 
				isSoundEndChkBox.Checked && !isDefaultSoundChkBtn.Checked;
			isDefaultSoundChkBtn.Enabled = isSoundEndChkBox.Checked;
			volumeBar.Enabled = isSoundEndChkBox.Checked;
			volumeText.Enabled = isSoundEndChkBox.Checked;
		}
		
		void IsDefaultSoundChkBtnCheckedChanged(object sender, EventArgs e)
		{
			updateIsSoundEndChkBox();
		}
		void SoundSanshouBtnClick(object sender, EventArgs e)
		{
			var f = new OpenFileDialog();
			f.DefaultExt = ".wav";
			f.FileName = "se_soc02.wav";
			f.Filter = "WAV形式(*.wav)|*.wav*";
			f.Multiselect = false;
			var a = f.ShowDialog();
			if (a == DialogResult.OK) {
				soundPathText.Text = f.FileName;
			}
		}
		void RtmpPathSanshouBtnClick(object sender, EventArgs e)
		{
			var f = new OpenFileDialog();
			f.DefaultExt = ".exe";
			f.FileName = "rtmpdump.exe";
			f.Filter = "EXE形式(*.exe)|*.exe*";
			f.Multiselect = false;
			var a = f.ShowDialog();
			if (a == DialogResult.OK) {
				rtmpPathText.Text = f.FileName;
			}
		}
		void VolumeBarValueChanged(object sender, EventArgs e)
		{
			util.debugWriteLine(volumeBar.Value);
			volumeText.Text = "音量：" + volumeBar.Value;
		}
		void isChaseRecordRadioBtn_UpdateAction() {
			isOnlyTimeShiftChaseChkBtn.Enabled = 
					isArgChaseRecFromFirstChkBox.Enabled =
					isChaseRecordRadioBtn.Checked;
		}
		void IsChaseRecordRadioBtnCheckedChanged(object sender, EventArgs e)
		{
			isChaseRecordRadioBtn_UpdateAction();
		}
		
		void OptionFormLoad(object sender, EventArgs e)
		{
			setFormFromConfig();
		}
		private void setBackColor(Color color) {
			BackColor = color;
			var c = getChildControls(this);
			foreach (var _c in c)
				if (//_c.GetType() == typeof(GroupBox) ||
				    _c.GetType() == typeof(System.Windows.Forms.Panel) || 
				    _c.GetType() == typeof(System.Windows.Forms.Form) 
				   	//_c.GetType() == typeof(System.Windows.Forms.TabPage) ||
				   //	_c.GetType() == typeof(System.Windows.Forms.TabControl)
				   )
						_c.BackColor = color;
		}
		private void setForeColor(Color color) {
			var c = getChildControls(this);
			foreach (var _c in c)
				if (//_c.GetType() == typeof(GroupBox) ||
				    _c.GetType() == typeof(Label) ||
				    _c.GetType() == typeof(CheckBox) ||
				   	_c.GetType() == typeof(RadioButton)) _c.ForeColor = color;
			
		}
		private List<Control> getChildControls(Control c) {
			util.debugWriteLine("cname " + c.Name);
			var ret = new List<Control>();
			foreach (Control _c in c.Controls) {
				ret.Add(_c);
				if (_c.GetType() != typeof(GroupBox)) {
					var children = getChildControls(_c);
					ret.AddRange(children);
				   }
				//util.debugWriteLine(c.Name + " " + children.Count);
			}
			util.debugWriteLine(c.Name + " " + ret.Count);
			return ret;
		}
		private void setCommentChkBox() {
			var isXml = bool.Parse(cfg.get("IsgetcommentXml"));
			var isXmlInfo = bool.Parse(cfg.get("IsgetcommentXmlInfo"));
			isCommentXML.Checked = isXml && !isXmlInfo;
        	isCommentJson.Checked = !isXml;
        	isCommentXmlInfo.Checked = isXmlInfo;
		}
		void UseProxyChkBox_CheckedChanged(object sender, EventArgs e)
		{
			useProxyUpdate();
		}
		void useProxyUpdate() {
			proxyAddressText.Enabled = proxyPortText.Enabled = 
				proxyAddressLabel.Enabled = proxyPortLabel.Enabled = 
					useProxyChkBox.Checked;
		}
		void ApplyBtnClick(object sender, EventArgs e)
		{
			util.setFontSize((int)fontList.Value, this, false);
		}
		void IsCommentConvertSpaceChkboxCheckedChanged(object sender, EventArgs e)
		{
			commentConvertStrText.Enabled = isCommentConvertSpaceChkbox.Checked;
		}
		void commentReplaceTextEnter(object sender, EventArgs e)
		{
		}
		void commentReplaceTextLeave(object sender, EventArgs e)
		{
		}
		void commentReplaceTextMouseDown(object sender, MouseEventArgs e)
		{
		}
		void OptionFormMouseDown(object sender, MouseEventArgs e)
		{
			
		}
		void CommentReplaceTextClick(object sender, System.EventArgs e)
		{
		}
		void CommentReplaceTextTextChanged(object sender, EventArgs e)
		{
		}
		string getCommentReplaceText() {
			var r = new List<string[]>();
			try {
				foreach (DataGridViewRow _r in commentReplaceList.Rows) {
					var c0 = _r.Cells[0].Value;
					var c1 = _r.Cells[1].Value;
					if (c0 == null) continue;
					var s1 = c1 == null ? "" : c1.ToString().Replace("\\n", "\n");
					r.Add(new string[]{c0.ToString(), s1});
				}
				var rr = JsonConvert.SerializeObject(r);
				return rr.ToString();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return "[]";
			}
		}
		void setCommentReplaceTextForm(string t) {
			var o = JsonConvert.DeserializeObject<List<string[]>>(t);
			try {
				for (var i = 0; i < o.Count; i++) {
					commentReplaceList.Rows.Add(new string[]{o[i][0], o[i][1].Replace("\n", "\\n")});
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void commentReplaceEditBtnClick(object sender, EventArgs e)
		{
			if (commentReplaceEditBtn.Text == "編集") {
				commentReplaceList.Height = 100;
				commentReplaceEditBtn.Text = "戻す";
			} else {
				commentReplaceList.Height = 19;
				commentReplaceEditBtn.Text = "編集";
			}
		}
		void AfterConvertModeCmdDefaultBtnClick(object sender, EventArgs e)
		{
			setDefaultFFmpegCmd();
		}
		private void setDefaultFFmpegCmd() {
			var type = getAfterConvertType();
			afterConvertModeCmdText.Text = util.getFFmpegDefaultArg(int.Parse(type));
		}
		void AfterConvertModeListSelectedIndexChanged(object sender, EventArgs e)
		{
			setDefaultFFmpegCmd();
		}
		void UseSecondRecFolderChkCheckedChanged(object sender, EventArgs e)
		{
			UseSecondRecFolderChkBox_UpdateAction();
		}
		void UseSecondRecFolderChkBox_UpdateAction() {
			secondRecFolderText.Enabled = 
				secondRecFolderSanshouBtn.Enabled = 
					useSecondRecFolderChk.Checked;
		}
		void SecondRecFolderSanshouBtnClick(object sender, EventArgs e)
		{
			var f = new FolderBrowserDialog();
			if (Directory.Exists(secondRecFolderText.Text))
				f.SelectedPath = secondRecFolderText.Text;
			DialogResult r = f.ShowDialog();
			
			util.debugWriteLine(f.SelectedPath);
			if (r == DialogResult.OK)
				secondRecFolderText.Text = f.SelectedPath;
		}
	}
}
