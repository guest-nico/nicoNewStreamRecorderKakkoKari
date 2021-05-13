﻿/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/07/26
 * Time: 16:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace namaichi
{
	partial class TimeShiftOptionForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.isAfterStartTimeCommentChkBox = new System.Windows.Forms.CheckBox();
			this.isOpenTimeBaseStartChkBox = new System.Windows.Forms.CheckBox();
			this.isMostStartTimeRadioBtn = new System.Windows.Forms.RadioButton();
			this.isSetVposStartTime = new System.Windows.Forms.CheckBox();
			this.openLastFileBtn = new System.Windows.Forms.Button();
			this.isRenketuLastFile = new System.Windows.Forms.CheckBox();
			this.lastFileInfoLabel = new System.Windows.Forms.Label();
			this.sLabel = new System.Windows.Forms.Label();
			this.sText = new System.Windows.Forms.TextBox();
			this.mLabel = new System.Windows.Forms.Label();
			this.mText = new System.Windows.Forms.TextBox();
			this.hLabel = new System.Windows.Forms.Label();
			this.hText = new System.Windows.Forms.TextBox();
			this.isFromLastTimeRadioBtn = new System.Windows.Forms.RadioButton();
			this.isStartTimeRadioBtn = new System.Windows.Forms.RadioButton();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.isUrlListLabelText2 = new System.Windows.Forms.Label();
			this.isUrlListLabelText3 = new System.Windows.Forms.Label();
			this.isUrlListLabelText1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.isUrlListLabelText0 = new System.Windows.Forms.Label();
			this.updateListSecondText = new System.Windows.Forms.TextBox();
			this.isM3u8RadioBtn = new System.Windows.Forms.RadioButton();
			this.isSimpleListRadioBtn = new System.Windows.Forms.RadioButton();
			this.isOpenListCommandChkBox = new System.Windows.Forms.CheckBox();
			this.openListCommandText = new System.Windows.Forms.TextBox();
			this.isUrlListChkBox = new System.Windows.Forms.CheckBox();
			this.resetBtn = new System.Windows.Forms.Button();
			this.lastSettingBtn = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.isOpenTimeBaseEndChkBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.endSText = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.endMText = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.endHText = new System.Windows.Forms.TextBox();
			this.isManualEndTimeRadioBtn = new System.Windows.Forms.RadioButton();
			this.isEndTimeRadioBtn = new System.Windows.Forms.RadioButton();
			this.isDeletePosTimeChkBox = new System.Windows.Forms.CheckBox();
			this.isBeforeEndTimeCommentChkBox = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Window;
			this.groupBox1.Controls.Add(this.isAfterStartTimeCommentChkBox);
			this.groupBox1.Controls.Add(this.isOpenTimeBaseStartChkBox);
			this.groupBox1.Controls.Add(this.isMostStartTimeRadioBtn);
			this.groupBox1.Controls.Add(this.isSetVposStartTime);
			this.groupBox1.Controls.Add(this.openLastFileBtn);
			this.groupBox1.Controls.Add(this.isRenketuLastFile);
			this.groupBox1.Controls.Add(this.lastFileInfoLabel);
			this.groupBox1.Controls.Add(this.sLabel);
			this.groupBox1.Controls.Add(this.sText);
			this.groupBox1.Controls.Add(this.mLabel);
			this.groupBox1.Controls.Add(this.mText);
			this.groupBox1.Controls.Add(this.hLabel);
			this.groupBox1.Controls.Add(this.hText);
			this.groupBox1.Controls.Add(this.isFromLastTimeRadioBtn);
			this.groupBox1.Controls.Add(this.isStartTimeRadioBtn);
			this.groupBox1.Location = new System.Drawing.Point(5, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(329, 212);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "録画開始時間";
			// 
			// isAfterStartTimeCommentChkBox
			// 
			this.isAfterStartTimeCommentChkBox.Location = new System.Drawing.Point(6, 181);
			this.isAfterStartTimeCommentChkBox.Name = "isAfterStartTimeCommentChkBox";
			this.isAfterStartTimeCommentChkBox.Size = new System.Drawing.Size(244, 17);
			this.isAfterStartTimeCommentChkBox.TabIndex = 8;
			this.isAfterStartTimeCommentChkBox.Text = "録画開始時間付近以降のコメントを保存する";
			this.isAfterStartTimeCommentChkBox.UseVisualStyleBackColor = true;
			// 
			// isOpenTimeBaseStartChkBox
			// 
			this.isOpenTimeBaseStartChkBox.Location = new System.Drawing.Point(189, 45);
			this.isOpenTimeBaseStartChkBox.Name = "isOpenTimeBaseStartChkBox";
			this.isOpenTimeBaseStartChkBox.Size = new System.Drawing.Size(135, 15);
			this.isOpenTimeBaseStartChkBox.TabIndex = 7;
			this.isOpenTimeBaseStartChkBox.Text = "開演時間を基準にする";
			this.isOpenTimeBaseStartChkBox.UseVisualStyleBackColor = true;
			this.isOpenTimeBaseStartChkBox.Visible = false;
			// 
			// isMostStartTimeRadioBtn
			// 
			this.isMostStartTimeRadioBtn.Checked = true;
			this.isMostStartTimeRadioBtn.Location = new System.Drawing.Point(6, 18);
			this.isMostStartTimeRadioBtn.Name = "isMostStartTimeRadioBtn";
			this.isMostStartTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isMostStartTimeRadioBtn.TabIndex = 6;
			this.isMostStartTimeRadioBtn.TabStop = true;
			this.isMostStartTimeRadioBtn.Text = "最初から";
			this.isMostStartTimeRadioBtn.UseVisualStyleBackColor = true;
			// 
			// isSetVposStartTime
			// 
			this.isSetVposStartTime.Checked = true;
			this.isSetVposStartTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.isSetVposStartTime.Location = new System.Drawing.Point(6, 159);
			this.isSetVposStartTime.Name = "isSetVposStartTime";
			this.isSetVposStartTime.Size = new System.Drawing.Size(244, 17);
			this.isSetVposStartTime.TabIndex = 5;
			this.isSetVposStartTime.Text = "コメントのvposは録画開始時間を起点にする";
			this.isSetVposStartTime.UseVisualStyleBackColor = true;
			// 
			// openLastFileBtn
			// 
			this.openLastFileBtn.Location = new System.Drawing.Point(235, 116);
			this.openLastFileBtn.Name = "openLastFileBtn";
			this.openLastFileBtn.Size = new System.Drawing.Size(88, 20);
			this.openLastFileBtn.TabIndex = 15;
			this.openLastFileBtn.Text = "ファイルを選択";
			this.openLastFileBtn.UseVisualStyleBackColor = true;
			this.openLastFileBtn.Click += new System.EventHandler(this.OpenLastFileBtnClick);
			// 
			// isRenketuLastFile
			// 
			this.isRenketuLastFile.Location = new System.Drawing.Point(26, 137);
			this.isRenketuLastFile.Name = "isRenketuLastFile";
			this.isRenketuLastFile.Size = new System.Drawing.Size(231, 17);
			this.isRenketuLastFile.TabIndex = 5;
			this.isRenketuLastFile.Text = "前回のファイルと新しい録画を連結する";
			this.isRenketuLastFile.UseVisualStyleBackColor = true;
			// 
			// lastFileInfoLabel
			// 
			this.lastFileInfoLabel.Location = new System.Drawing.Point(25, 120);
			this.lastFileInfoLabel.Name = "lastFileInfoLabel";
			this.lastFileInfoLabel.Size = new System.Drawing.Size(225, 17);
			this.lastFileInfoLabel.TabIndex = 3;
			this.lastFileInfoLabel.Text = "(?時間?分?秒まで録画済み)";
			// 
			// sLabel
			// 
			this.sLabel.Location = new System.Drawing.Point(188, 68);
			this.sLabel.Name = "sLabel";
			this.sLabel.Size = new System.Drawing.Size(69, 12);
			this.sLabel.TabIndex = 2;
			this.sLabel.Text = "秒時点から";
			// 
			// sText
			// 
			this.sText.Location = new System.Drawing.Point(153, 65);
			this.sText.Name = "sText";
			this.sText.Size = new System.Drawing.Size(29, 19);
			this.sText.TabIndex = 3;
			this.sText.Text = "0";
			this.sText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// mLabel
			// 
			this.mLabel.Location = new System.Drawing.Point(130, 68);
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size(39, 12);
			this.mLabel.TabIndex = 2;
			this.mLabel.Text = "分";
			// 
			// mText
			// 
			this.mText.Location = new System.Drawing.Point(95, 65);
			this.mText.Name = "mText";
			this.mText.Size = new System.Drawing.Size(29, 19);
			this.mText.TabIndex = 2;
			this.mText.Text = "0";
			this.mText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// hLabel
			// 
			this.hLabel.Location = new System.Drawing.Point(60, 68);
			this.hLabel.Name = "hLabel";
			this.hLabel.Size = new System.Drawing.Size(39, 12);
			this.hLabel.TabIndex = 2;
			this.hLabel.Text = "時間";
			// 
			// hText
			// 
			this.hText.Location = new System.Drawing.Point(25, 65);
			this.hText.Name = "hText";
			this.hText.Size = new System.Drawing.Size(29, 19);
			this.hText.TabIndex = 1;
			this.hText.Text = "0";
			this.hText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// isFromLastTimeRadioBtn
			// 
			this.isFromLastTimeRadioBtn.Location = new System.Drawing.Point(6, 99);
			this.isFromLastTimeRadioBtn.Name = "isFromLastTimeRadioBtn";
			this.isFromLastTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isFromLastTimeRadioBtn.TabIndex = 4;
			this.isFromLastTimeRadioBtn.Text = "前回の続きから録画";
			this.isFromLastTimeRadioBtn.UseVisualStyleBackColor = true;
			this.isFromLastTimeRadioBtn.CheckedChanged += new System.EventHandler(this.IsFromLastTimeRadioBtnCheckedChanged);
			// 
			// isStartTimeRadioBtn
			// 
			this.isStartTimeRadioBtn.Location = new System.Drawing.Point(6, 42);
			this.isStartTimeRadioBtn.Name = "isStartTimeRadioBtn";
			this.isStartTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isStartTimeRadioBtn.TabIndex = 0;
			this.isStartTimeRadioBtn.Text = "録画開始時間を指定";
			this.isStartTimeRadioBtn.UseVisualStyleBackColor = true;
			this.isStartTimeRadioBtn.CheckedChanged += new System.EventHandler(this.isStartTimeRadioBtn_CheckedChanged);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(245, 396);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(74, 23);
			this.cancelBtn.TabIndex = 16;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(165, 396);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(74, 23);
			this.okBtn.TabIndex = 15;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.SystemColors.Window;
			this.groupBox3.Controls.Add(this.isUrlListLabelText2);
			this.groupBox3.Controls.Add(this.isUrlListLabelText3);
			this.groupBox3.Controls.Add(this.isUrlListLabelText1);
			this.groupBox3.Controls.Add(this.panel2);
			this.groupBox3.Controls.Add(this.isOpenListCommandChkBox);
			this.groupBox3.Controls.Add(this.openListCommandText);
			this.groupBox3.Controls.Add(this.isUrlListChkBox);
			this.groupBox3.Location = new System.Drawing.Point(5, 249);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(299, 197);
			this.groupBox3.TabIndex = 27;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "セグメントURLリスト";
			this.groupBox3.Visible = false;
			// 
			// isUrlListLabelText2
			// 
			this.isUrlListLabelText2.Location = new System.Drawing.Point(45, 134);
			this.isUrlListLabelText2.Name = "isUrlListLabelText2";
			this.isUrlListLabelText2.Size = new System.Drawing.Size(205, 16);
			this.isUrlListLabelText2.TabIndex = 26;
			this.isUrlListLabelText2.Text = "(例) notepad {i}";
			// 
			// isUrlListLabelText3
			// 
			this.isUrlListLabelText3.Location = new System.Drawing.Point(13, 165);
			this.isUrlListLabelText3.Name = "isUrlListLabelText3";
			this.isUrlListLabelText3.Size = new System.Drawing.Size(57, 22);
			this.isUrlListLabelText3.TabIndex = 22;
			this.isUrlListLabelText3.Text = "コマンド：";
			this.isUrlListLabelText3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// isUrlListLabelText1
			// 
			this.isUrlListLabelText1.Location = new System.Drawing.Point(45, 116);
			this.isUrlListLabelText1.Name = "isUrlListLabelText1";
			this.isUrlListLabelText1.Size = new System.Drawing.Size(145, 17);
			this.isUrlListLabelText1.TabIndex = 10;
			this.isUrlListLabelText1.Text = "{i}　リストファイルのパス";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.isUrlListLabelText0);
			this.panel2.Controls.Add(this.updateListSecondText);
			this.panel2.Controls.Add(this.isM3u8RadioBtn);
			this.panel2.Controls.Add(this.isSimpleListRadioBtn);
			this.panel2.Location = new System.Drawing.Point(25, 40);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(259, 55);
			this.panel2.TabIndex = 9;
			// 
			// isUrlListLabelText0
			// 
			this.isUrlListLabelText0.Location = new System.Drawing.Point(168, 29);
			this.isUrlListLabelText0.Name = "isUrlListLabelText0";
			this.isUrlListLabelText0.Size = new System.Drawing.Size(73, 17);
			this.isUrlListLabelText0.TabIndex = 2;
			this.isUrlListLabelText0.Text = "秒ごとに更新";
			// 
			// updateListSecondText
			// 
			this.updateListSecondText.Location = new System.Drawing.Point(126, 26);
			this.updateListSecondText.Name = "updateListSecondText";
			this.updateListSecondText.Size = new System.Drawing.Size(36, 19);
			this.updateListSecondText.TabIndex = 12;
			// 
			// isM3u8RadioBtn
			// 
			this.isM3u8RadioBtn.Location = new System.Drawing.Point(3, 27);
			this.isM3u8RadioBtn.Name = "isM3u8RadioBtn";
			this.isM3u8RadioBtn.Size = new System.Drawing.Size(117, 16);
			this.isM3u8RadioBtn.TabIndex = 11;
			this.isM3u8RadioBtn.TabStop = true;
			this.isM3u8RadioBtn.Text = "M3U8形式で出力する";
			this.isM3u8RadioBtn.UseVisualStyleBackColor = true;
			this.isM3u8RadioBtn.CheckedChanged += new System.EventHandler(this.isM3u8RadioBtn_CheckedChanged);
			// 
			// isSimpleListRadioBtn
			// 
			this.isSimpleListRadioBtn.Checked = true;
			this.isSimpleListRadioBtn.Location = new System.Drawing.Point(3, 5);
			this.isSimpleListRadioBtn.Name = "isSimpleListRadioBtn";
			this.isSimpleListRadioBtn.Size = new System.Drawing.Size(199, 16);
			this.isSimpleListRadioBtn.TabIndex = 10;
			this.isSimpleListRadioBtn.TabStop = true;
			this.isSimpleListRadioBtn.Text = "シンプルなテキスト形式で出力する";
			this.isSimpleListRadioBtn.UseVisualStyleBackColor = true;
			// 
			// isOpenListCommandChkBox
			// 
			this.isOpenListCommandChkBox.Location = new System.Drawing.Point(28, 98);
			this.isOpenListCommandChkBox.Name = "isOpenListCommandChkBox";
			this.isOpenListCommandChkBox.Size = new System.Drawing.Size(229, 18);
			this.isOpenListCommandChkBox.TabIndex = 13;
			this.isOpenListCommandChkBox.Text = "URLリストファイルを以下のコマンドで開く";
			this.isOpenListCommandChkBox.UseVisualStyleBackColor = true;
			this.isOpenListCommandChkBox.CheckedChanged += new System.EventHandler(this.isOpenListCommandChkBox_CheckedChanged);
			// 
			// openListCommandText
			// 
			this.openListCommandText.Location = new System.Drawing.Point(76, 165);
			this.openListCommandText.Name = "openListCommandText";
			this.openListCommandText.Size = new System.Drawing.Size(208, 19);
			this.openListCommandText.TabIndex = 14;
			// 
			// isUrlListChkBox
			// 
			this.isUrlListChkBox.Location = new System.Drawing.Point(6, 24);
			this.isUrlListChkBox.Name = "isUrlListChkBox";
			this.isUrlListChkBox.Size = new System.Drawing.Size(251, 18);
			this.isUrlListChkBox.TabIndex = 9;
			this.isUrlListChkBox.Text = "セグメントファイルのURLリストを出力する";
			this.isUrlListChkBox.UseVisualStyleBackColor = true;
			this.isUrlListChkBox.CheckedChanged += new System.EventHandler(this.isUrlListChkBox_CheckedChanged);
			// 
			// resetBtn
			// 
			this.resetBtn.Location = new System.Drawing.Point(85, 396);
			this.resetBtn.Name = "resetBtn";
			this.resetBtn.Size = new System.Drawing.Size(74, 23);
			this.resetBtn.TabIndex = 32;
			this.resetBtn.Text = "リセット";
			this.resetBtn.UseVisualStyleBackColor = true;
			this.resetBtn.Click += new System.EventHandler(this.ResetBtnClick);
			// 
			// lastSettingBtn
			// 
			this.lastSettingBtn.Location = new System.Drawing.Point(5, 396);
			this.lastSettingBtn.Name = "lastSettingBtn";
			this.lastSettingBtn.Size = new System.Drawing.Size(74, 23);
			this.lastSettingBtn.TabIndex = 31;
			this.lastSettingBtn.Text = "前回の設定";
			this.lastSettingBtn.UseVisualStyleBackColor = true;
			this.lastSettingBtn.Click += new System.EventHandler(this.LastSettingBtnClick);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Window;
			this.groupBox2.Controls.Add(this.isOpenTimeBaseEndChkBox);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.endSText);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.endMText);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.endHText);
			this.groupBox2.Controls.Add(this.isManualEndTimeRadioBtn);
			this.groupBox2.Controls.Add(this.isEndTimeRadioBtn);
			this.groupBox2.Controls.Add(this.isDeletePosTimeChkBox);
			this.groupBox2.Controls.Add(this.isBeforeEndTimeCommentChkBox);
			this.groupBox2.Location = new System.Drawing.Point(5, 228);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(329, 152);
			this.groupBox2.TabIndex = 28;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "録画終了時間";
			// 
			// isOpenTimeBaseEndChkBox
			// 
			this.isOpenTimeBaseEndChkBox.Location = new System.Drawing.Point(189, 45);
			this.isOpenTimeBaseEndChkBox.Name = "isOpenTimeBaseEndChkBox";
			this.isOpenTimeBaseEndChkBox.Size = new System.Drawing.Size(135, 15);
			this.isOpenTimeBaseEndChkBox.TabIndex = 7;
			this.isOpenTimeBaseEndChkBox.Text = "開演時間を基準にする";
			this.isOpenTimeBaseEndChkBox.UseVisualStyleBackColor = true;
			this.isOpenTimeBaseEndChkBox.Visible = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(60, 89);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(234, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "0時間0分0秒を指定すると最後まで録画します";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(188, 67);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(69, 12);
			this.label6.TabIndex = 2;
			this.label6.Text = "秒時点まで";
			// 
			// endSText
			// 
			this.endSText.Location = new System.Drawing.Point(153, 64);
			this.endSText.Name = "endSText";
			this.endSText.Size = new System.Drawing.Size(29, 19);
			this.endSText.TabIndex = 8;
			this.endSText.Text = "0";
			this.endSText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(130, 67);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(39, 12);
			this.label7.TabIndex = 2;
			this.label7.Text = "分";
			// 
			// endMText
			// 
			this.endMText.Location = new System.Drawing.Point(95, 64);
			this.endMText.Name = "endMText";
			this.endMText.Size = new System.Drawing.Size(29, 19);
			this.endMText.TabIndex = 7;
			this.endMText.Text = "0";
			this.endMText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(60, 67);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(39, 12);
			this.label8.TabIndex = 2;
			this.label8.Text = "時間";
			// 
			// endHText
			// 
			this.endHText.Location = new System.Drawing.Point(25, 64);
			this.endHText.Name = "endHText";
			this.endHText.Size = new System.Drawing.Size(29, 19);
			this.endHText.TabIndex = 6;
			this.endHText.Text = "0";
			this.endHText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// isManualEndTimeRadioBtn
			// 
			this.isManualEndTimeRadioBtn.Location = new System.Drawing.Point(6, 42);
			this.isManualEndTimeRadioBtn.Name = "isManualEndTimeRadioBtn";
			this.isManualEndTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isManualEndTimeRadioBtn.TabIndex = 0;
			this.isManualEndTimeRadioBtn.Text = "録画終了時間を指定";
			this.isManualEndTimeRadioBtn.UseVisualStyleBackColor = true;
			this.isManualEndTimeRadioBtn.CheckedChanged += new System.EventHandler(this.IsManualEndTimeRadioBtnCheckedChanged);
			// 
			// isEndTimeRadioBtn
			// 
			this.isEndTimeRadioBtn.Checked = true;
			this.isEndTimeRadioBtn.Location = new System.Drawing.Point(6, 18);
			this.isEndTimeRadioBtn.Name = "isEndTimeRadioBtn";
			this.isEndTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isEndTimeRadioBtn.TabIndex = 0;
			this.isEndTimeRadioBtn.TabStop = true;
			this.isEndTimeRadioBtn.Text = "最後まで";
			this.isEndTimeRadioBtn.UseVisualStyleBackColor = true;
			// 
			// isDeletePosTimeChkBox
			// 
			this.isDeletePosTimeChkBox.Location = new System.Drawing.Point(25, 110);
			this.isDeletePosTimeChkBox.Name = "isDeletePosTimeChkBox";
			this.isDeletePosTimeChkBox.Size = new System.Drawing.Size(299, 17);
			this.isDeletePosTimeChkBox.TabIndex = 8;
			this.isDeletePosTimeChkBox.Text = "完了時にファイル名の時間位置を削除する";
			this.isDeletePosTimeChkBox.UseVisualStyleBackColor = true;
			// 
			// isBeforeEndTimeCommentChkBox
			// 
			this.isBeforeEndTimeCommentChkBox.Location = new System.Drawing.Point(7, 133);
			this.isBeforeEndTimeCommentChkBox.Name = "isBeforeEndTimeCommentChkBox";
			this.isBeforeEndTimeCommentChkBox.Size = new System.Drawing.Size(250, 17);
			this.isBeforeEndTimeCommentChkBox.TabIndex = 8;
			this.isBeforeEndTimeCommentChkBox.Text = "録画終了時間付近以前のコメントを保存する";
			this.isBeforeEndTimeCommentChkBox.UseVisualStyleBackColor = true;
			// 
			// TimeShiftOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(341, 428);
			this.Controls.Add(this.resetBtn);
			this.Controls.Add(this.lastSettingBtn);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Name = "TimeShiftOptionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "タイムシフト録画設定";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button openLastFileBtn;
		private System.Windows.Forms.CheckBox isDeletePosTimeChkBox;
		private System.Windows.Forms.CheckBox isBeforeEndTimeCommentChkBox;
		private System.Windows.Forms.CheckBox isAfterStartTimeCommentChkBox;
		private System.Windows.Forms.CheckBox isOpenTimeBaseEndChkBox;
		private System.Windows.Forms.CheckBox isOpenTimeBaseStartChkBox;
		private System.Windows.Forms.Button lastSettingBtn;
		private System.Windows.Forms.Button resetBtn;
		private System.Windows.Forms.RadioButton isMostStartTimeRadioBtn;
		private System.Windows.Forms.RadioButton isEndTimeRadioBtn;
		private System.Windows.Forms.RadioButton isManualEndTimeRadioBtn;
		private System.Windows.Forms.CheckBox isSetVposStartTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox isUrlListChkBox;
		private System.Windows.Forms.TextBox openListCommandText;
		private System.Windows.Forms.CheckBox isOpenListCommandChkBox;
		private System.Windows.Forms.RadioButton isSimpleListRadioBtn;
		private System.Windows.Forms.RadioButton isM3u8RadioBtn;
		private System.Windows.Forms.TextBox updateListSecondText;
		private System.Windows.Forms.Label isUrlListLabelText0;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label isUrlListLabelText1;
		private System.Windows.Forms.Label isUrlListLabelText3;
		private System.Windows.Forms.Label isUrlListLabelText2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox endHText;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox endMText;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox endSText;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.CheckBox isRenketuLastFile;
		private System.Windows.Forms.Label lastFileInfoLabel;
		private System.Windows.Forms.RadioButton isStartTimeRadioBtn;
		private System.Windows.Forms.RadioButton isFromLastTimeRadioBtn;
		private System.Windows.Forms.TextBox hText;
		private System.Windows.Forms.Label hLabel;
		private System.Windows.Forms.TextBox mText;
		private System.Windows.Forms.Label mLabel;
		private System.Windows.Forms.TextBox sText;
		private System.Windows.Forms.Label sLabel;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
