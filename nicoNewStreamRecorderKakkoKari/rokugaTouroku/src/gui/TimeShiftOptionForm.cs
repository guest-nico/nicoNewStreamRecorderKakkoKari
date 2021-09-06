/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/07/26
 * Time: 16:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using rokugaTouroku.info;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of TimeShiftOptionForm.
	/// </summary>
	public partial class TimeShiftOptionForm : Form
	{
		//private string[] lastFileTime;
		private string segmentSaveType;
		public TimeShiftConfig ret = null;
		private config.config config;
		public int h, m, s;
		public TimeShiftOptionForm( 
				string segmentSaveType, config.config config, TimeShiftConfig tsConfigIn)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//this.lastFileTime = lastFileTime;
			this.segmentSaveType = segmentSaveType;
			this.config = config;
			/*
			if (lastFileTime != null)
				lastFileInfoLabel.Text = "(" + lastFileTime[0] + 
					"時間" + lastFileTime[1] + "分" + lastFileTime[2] + "秒まで録画済み)";
			else {
				lastFileInfoLabel.Text = "(前回の録画ファイルが見つかりませんでした)";
				isRenketuLastFile.Enabled = false;
				isFromLastTimeRadioBtn.Enabled = false;
			}
			*/
			isRenketuLastFile.Visible = (segmentSaveType == "0");
			
			/*
			var isUrlList = bool.Parse(config.get("IsUrlList"));
			var openListCommand = config.get("openUrlListCommand");
			
			isUrlListChkBox.Checked = isUrlList;
			openListCommandText.Text = openListCommand;
			openListCommandText.Enabled = isUrlList;
			
			if (bool.Parse(config.get("IsM3u8List")))
				isM3u8RadioBtn.Checked = true;
			updateListSecondText.Text = config.get("M3u8UpdateSeconds");
			isOpenListCommandChkBox.Checked = bool.Parse(config.get("IsOpenUrlList"));
			*/
			setFormFromConfig();
						
			
			hText.Text = tsConfigIn.h.ToString();
			mText.Text = tsConfigIn.m.ToString();
			sText.Text = tsConfigIn.s.ToString();
			endHText.Text = tsConfigIn.endH.ToString();
			endMText.Text = tsConfigIn.endM.ToString();
			endSText.Text = tsConfigIn.endS.ToString();
			isRenketuLastFile.Checked = tsConfigIn.isContinueConcat;
			if (tsConfigIn.timeType == 1) isFromLastTimeRadioBtn.Checked = true;
			
			if (tsConfigIn.startTimeMode == 0) isMostStartTimeRadioBtn.Checked = true;
			else if (tsConfigIn.startTimeMode == 1) isStartTimeRadioBtn.Checked = true;
			else isFromLastTimeRadioBtn.Checked = true;
			
			if (tsConfigIn.endTimeMode == 0) isEndTimeRadioBtn.Checked = true;
			else if (tsConfigIn.endTimeMode == 1) isManualEndTimeRadioBtn.Checked = true;
			if (!tsConfigIn.isDeletePosTime) isDeletePosTimeChkBox.Checked = false;
			
			updateTimeShiftStartTimeChkBox();
			updateIsFromLastTimeRadioBtn();
			//updateIsM3u8RadioBtn_CheckedChanged();
			//updateIsOpenListCommandChkBoxCheckedChanged();
			//updateUrlListChkBoxCheckedChanged();
			updateIsManualEndTimeRadioBtn();
			
			util.setFontSize(int.Parse(config.get("fontSize")), this, false);
		}
		private void updateTimeShiftStartTimeChkBox() {
			hText.Enabled = mText.Enabled = sText.Enabled =
					isOpenTimeBaseStartChkBox.Enabled = 
						isStartTimeRadioBtn.Checked;;
		}	
		void cancelBtn_Click(object sender, EventArgs e)
		{
			Close();
		}
		
		void okBtn_Click(object sender, EventArgs e)
		{
			var startType = (isFromLastTimeRadioBtn.Checked) ? 1 : 0;
			var startTimeMode = (isMostStartTimeRadioBtn.Checked ? 0 :((isStartTimeRadioBtn.Checked) ? 1 : 2));
			var endTimeMode = isEndTimeRadioBtn.Checked ? 0 : 1;
			
			//var _h = (startType == 0) ? hText.Text : lastFileTime[0];
			//var _m = (startType == 0) ? mText.Text : lastFileTime[1];
			//var _s = (startType == 0) ? sText.Text : lastFileTime[2];
			var _h = hText.Text;
			var _m = mText.Text;
			var _s = sText.Text;
			int h;
			int m;
			int s;
			if (!int.TryParse(_h, out h) ||
			   	!int.TryParse(_m, out m) ||
			   	!int.TryParse(_s, out s)) {
				util.showMessageBoxCenterForm(this, "開始時間に数字以外が指定されています");
				return;
			}
			
			var _endH = endHText.Text;
			var _endM = endMText.Text;
			var _endS = endSText.Text;
			int endH;
			int endM;
			int endS;
			if (!int.TryParse(_endH, out endH) ||
			   	!int.TryParse(_endM, out endM) ||
			   	!int.TryParse(_endS, out endS)) {
				util.showMessageBoxCenterForm(this, "終了時間に数字以外が指定されています");
				return;
			}
			
			var timeSeconds = (startTimeMode == 1) ? (h * 3600 + m * 60 + s) : 0;
			var endTimeSeconds = endH * 3600 + endM * 60 + endS;
			if (endTimeMode == 0) endTimeSeconds = 0;
			if (endTimeMode == 1 && (endH != 0 || endM != 0 || endS != 0) && 
			    	endTimeSeconds < timeSeconds) {
				util.showMessageBoxCenterForm(this, "終了時間が開始時間より前に設定されています");
				return;
			}
			/*
			double updateSecond;
			if (!double.TryParse(updateListSecondText.Text, out updateSecond)) {
				util.showMessageBoxCenterForm(this, "M3U8の更新間隔に数字以外が指定されています");
				return;
			}
			if (updateSecond <= 0.5) {
				util.showMessageBoxCenterForm(this, "M3U8の更新間隔に0.5以下を指定することはできません");
				return;
			}
			*/
			/*
			var isUrlList = isUrlListChkBox.Checked;
			var openListCommand = openListCommandText.Text;
			var isM3u8List = isM3u8RadioBtn.Checked;
			var m3u8UpdateSeconds = updateSecond;
			var isOpenUrlList = isOpenListCommandChkBox.Checked;
			*/
			var isUrlList = false;
			var openListCommand = "";
			var isM3u8List = false;
			var m3u8UpdateSeconds = 5.1;
			var isOpenUrlList = false;
			//var startTimeMode = (isMostStartTimeRadioBtn.Checked ? 0 :((isStartTimeRadioBtn.Checked) ? 1 : 2));
			//var endTimeMode = isEndTimeRadioBtn.Checked ? 0 : 1;
			var isOpenTimeBaseStart = startTimeMode != 1 ? false : isOpenTimeBaseStartChkBox.Checked;
			var isOpenTimeBaseEnd = endTimeMode != 1 ? false : isOpenTimeBaseEndChkBox.Checked;
			
			ret = new TimeShiftConfig(startType, 
				h, m, s, endH, endM, endS, isRenketuLastFile.Checked, isUrlList, 
				openListCommand, isM3u8List, m3u8UpdateSeconds, isOpenUrlList,
				isSetVposStartTime.Checked, startTimeMode, endTimeMode, 
				isAfterStartTimeCommentChkBox.Checked, 
				isOpenTimeBaseStart, isOpenTimeBaseEnd, 
				isBeforeEndTimeCommentChkBox.Checked, isDeletePosTimeChkBox.Checked);
			
			/*
			config.set("IsUrlList", isUrlList.ToString().ToLower());
			config.set("IsM3u8List", isM3u8List.ToString().ToLower());
			config.set("M3u8UpdateSeconds", m3u8UpdateSeconds.ToString());
			config.set("IsOpenUrlList", isOpenUrlList.ToString().ToLower());
			config.set("openUrlListCommand", openListCommand);
			*/
			config.set("tsStartTimeMode", startTimeMode.ToString());
			config.set("tsEndTimeMode", endTimeMode.ToString());
			config.set("tsStartSecond", (h * 3600 + m * 60 + s).ToString());
			config.set("tsEndSecond", (endH * 3600 + endM * 60 + endS).ToString());
			config.set("tsIsDeletePosTime", isDeletePosTimeChkBox.Checked.ToString().ToLower());
			config.set("tsIsRenketu", isRenketuLastFile.Checked.ToString().ToLower());
			config.set("IsVposStartTime", isSetVposStartTime.Checked.ToString().ToLower());
			config.set("IsAfterStartTimeComment", isAfterStartTimeCommentChkBox.Checked.ToString().ToLower());
			config.set("tsBaseOpenTimeStart", isOpenTimeBaseStartChkBox.Checked.ToString().ToLower());
			config.set("tsBaseOpenTimeEnd", isOpenTimeBaseEndChkBox.Checked.ToString().ToLower());
			config.set("IsBeforeEndTimeComment", isBeforeEndTimeCommentChkBox.Checked.ToString().ToLower());
			Close();
		}
		
		void isStartTimeRadioBtn_CheckedChanged(object sender, EventArgs e)
		{
			updateTimeShiftStartTimeChkBox();
		}
		
		void isUrlListChkBox_CheckedChanged(object sender, EventArgs e)
		{
			updateUrlListChkBoxCheckedChanged();
		}
		void updateUrlListChkBoxCheckedChanged() {
			openListCommandText.Enabled = (isUrlListChkBox.Checked && isOpenListCommandChkBox.Checked);
			isM3u8RadioBtn.Enabled = isUrlListChkBox.Checked;
			isSimpleListRadioBtn.Enabled = isUrlListChkBox.Checked;
			updateListSecondText.Enabled = isUrlListChkBox.Checked && isM3u8RadioBtn.Checked;
			isOpenListCommandChkBox.Enabled = isUrlListChkBox.Checked;
			
			isUrlListLabelText0.Enabled = isUrlListChkBox.Checked;
			isUrlListLabelText1.Enabled = isUrlListChkBox.Checked;
			isUrlListLabelText2.Enabled = isUrlListChkBox.Checked;
			isUrlListLabelText3.Enabled = isUrlListChkBox.Checked;
		}
		
		void isM3u8RadioBtn_CheckedChanged(object sender, EventArgs e)
		{
			updateIsM3u8RadioBtn_CheckedChanged();
		}
		void updateIsM3u8RadioBtn_CheckedChanged() {
			updateListSecondText.Enabled = isM3u8RadioBtn.Checked;
		}
			
		void isOpenListCommandChkBox_CheckedChanged(object sender, EventArgs e)
		{
			updateIsOpenListCommandChkBoxCheckedChanged();
		}
		void updateIsOpenListCommandChkBoxCheckedChanged() {
			openListCommandText.Enabled = isOpenListCommandChkBox.Checked;
		}
		
		void IsFromLastTimeRadioBtnCheckedChanged(object sender, EventArgs e)
		{
			updateIsFromLastTimeRadioBtn();
		}
		void updateIsFromLastTimeRadioBtn() {
			isRenketuLastFile.Enabled = isFromLastTimeRadioBtn.Checked;
		}
		
		void IsManualEndTimeRadioBtnCheckedChanged(object sender, EventArgs e)
		{
			updateIsManualEndTimeRadioBtn();
		}
		void updateIsManualEndTimeRadioBtn() {
			endHText.Enabled = endMText.Enabled = 
					endSText.Enabled = isOpenTimeBaseEndChkBox.Enabled = 
					isDeletePosTimeChkBox.Enabled =
					isManualEndTimeRadioBtn.Checked;
		}
		private void setFormFromConfig() {
			var startMode = config.get("tsStartTimeMode");
			if (startMode == "0") isMostStartTimeRadioBtn.Checked = true;
			else if (startMode == "1") isStartTimeRadioBtn.Checked = true;
			else isFromLastTimeRadioBtn.Checked = true;
			if (config.get("tsEndTimeMode") == "0") isEndTimeRadioBtn.Checked = true;
			else isManualEndTimeRadioBtn.Checked = true;
			
			int startSeconds, endSeconds;
			if (int.TryParse(config.get("tsStartSecond"), out startSeconds)) {
				hText.Text = ((int)(startSeconds / 3600)).ToString();
				mText.Text = ((int)((startSeconds % 3600) / 60)).ToString();
				sText.Text = ((int)((startSeconds % 60) / 1)).ToString();
			}
			if (int.TryParse(config.get("tsEndSecond"), out endSeconds)) {
				endHText.Text = ((int)(endSeconds / 3600)).ToString();
				endMText.Text = ((int)((endSeconds % 3600) / 60)).ToString();
				endSText.Text = ((int)((endSeconds % 60) / 1)).ToString();
			}
			isDeletePosTimeChkBox.Checked = bool.Parse(config.get("tsIsDeletePosTime"));
			isRenketuLastFile.Checked = bool.Parse(config.get("tsIsRenketu"));
			isSetVposStartTime.Checked = bool.Parse(config.get("IsVposStartTime"));
			isAfterStartTimeCommentChkBox.Checked = bool.Parse(config.get("IsAfterStartTimeComment"));
			isBeforeEndTimeCommentChkBox.Checked = bool.Parse(config.get("IsBeforeEndTimeComment"));
			isOpenTimeBaseStartChkBox.Checked = bool.Parse(config.get("tsBaseOpenTimeStart"));
			isOpenTimeBaseEndChkBox.Checked = bool.Parse(config.get("tsBaseOpenTimeStart"));
		}
		void LastSettingBtnClick(object sender, System.EventArgs e)
		{
			setFormFromConfig();
		}
		void ResetBtnClick(object sender, EventArgs e)
		{
			isMostStartTimeRadioBtn.Checked = true;
			isEndTimeRadioBtn.Checked = true;
			hText.Text = "0";
			mText.Text = "0";
			sText.Text = "0";
			endHText.Text = "0";
			endMText.Text = "0";
			endSText.Text = "0";
			isDeletePosTimeChkBox.Checked = true;
			isRenketuLastFile.Checked = false;
			isSetVposStartTime.Checked = true;
			isOpenTimeBaseStartChkBox.Checked = false;
			isOpenTimeBaseEndChkBox.Checked = false;
			isAfterStartTimeCommentChkBox.Checked = false;
			isBeforeEndTimeCommentChkBox.Checked = false;
		}
	}
}
