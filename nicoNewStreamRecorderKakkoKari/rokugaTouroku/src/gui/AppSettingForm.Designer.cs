/*
 * Created by SharpDevelop.
 * User: kogak
 * Date: 2025/06/02
 * Time: 0:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku.gui
{
	partial class AppSettingForm
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
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.fileNameTypeDokujiSetteiBtn = new System.Windows.Forms.Button();
			this.fileNameTypeRadioBtn9 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn8 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn7 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn5 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn6 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn4 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn3 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn2 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn1 = new System.Windows.Forms.RadioButton();
			this.fileNameTypeRadioBtn0 = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.housoushaUserIDRadioBtn = new System.Windows.Forms.RadioButton();
			this.userIDHousoushaRadioBtn = new System.Windows.Forms.RadioButton();
			this.userIDRadioBtn = new System.Windows.Forms.RadioButton();
			this.ComIDComNameRadioBtn = new System.Windows.Forms.RadioButton();
			this.comIDRadioBtn = new System.Windows.Forms.RadioButton();
			this.housoushaComIDRadioBtn = new System.Windows.Forms.RadioButton();
			this.comIDHousoushaRadioBtn = new System.Windows.Forms.RadioButton();
			this.comNameRadioBtn = new System.Windows.Forms.RadioButton();
			this.housoushaRadioBtn = new System.Windows.Forms.RadioButton();
			this.useSubFolderChk = new System.Windows.Forms.CheckBox();
			this.recFolderSanshouBtn = new System.Windows.Forms.Button();
			this.baseDirectoryText = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.extText = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox12 = new System.Windows.Forms.GroupBox();
			this.isMinimizedChkBox = new System.Windows.Forms.CheckBox();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox12.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(345, 562);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(74, 23);
			this.cancelBtn.TabIndex = 3;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(265, 562);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(74, 23);
			this.okBtn.TabIndex = 2;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.OkBtnClick);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.fileNameTypeDokujiSetteiBtn);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn9);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn8);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn7);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn5);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn6);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn4);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn3);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn2);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn1);
			this.groupBox2.Controls.Add(this.fileNameTypeRadioBtn0);
			this.groupBox2.Location = new System.Drawing.Point(5, 173);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(422, 283);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "録画ファイル名の書式";
			// 
			// fileNameTypeDokujiSetteiBtn
			// 
			this.fileNameTypeDokujiSetteiBtn.Location = new System.Drawing.Point(6, 255);
			this.fileNameTypeDokujiSetteiBtn.Name = "fileNameTypeDokujiSetteiBtn";
			this.fileNameTypeDokujiSetteiBtn.Size = new System.Drawing.Size(353, 21);
			this.fileNameTypeDokujiSetteiBtn.TabIndex = 1;
			this.fileNameTypeDokujiSetteiBtn.UseVisualStyleBackColor = true;
			this.fileNameTypeDokujiSetteiBtn.Click += new System.EventHandler(this.FileNameTypeDokujiSetteiBtnClick);
			// 
			// fileNameTypeRadioBtn9
			// 
			this.fileNameTypeRadioBtn9.Checked = true;
			this.fileNameTypeRadioBtn9.Location = new System.Drawing.Point(15, 228);
			this.fileNameTypeRadioBtn9.Name = "fileNameTypeRadioBtn9";
			this.fileNameTypeRadioBtn9.Size = new System.Drawing.Size(292, 21);
			this.fileNameTypeRadioBtn9.TabIndex = 0;
			this.fileNameTypeRadioBtn9.TabStop = true;
			this.fileNameTypeRadioBtn9.Text = "独自設定";
			this.fileNameTypeRadioBtn9.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn8
			// 
			this.fileNameTypeRadioBtn8.Checked = true;
			this.fileNameTypeRadioBtn8.Location = new System.Drawing.Point(15, 198);
			this.fileNameTypeRadioBtn8.Name = "fileNameTypeRadioBtn8";
			this.fileNameTypeRadioBtn8.Size = new System.Drawing.Size(344, 21);
			this.fileNameTypeRadioBtn8.TabIndex = 0;
			this.fileNameTypeRadioBtn8.TabStop = true;
			this.fileNameTypeRadioBtn8.Text = "日付_放送タイトル(放送ID)_名前_チャンネル名(チャンネルID)_xx";
			this.fileNameTypeRadioBtn8.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn7
			// 
			this.fileNameTypeRadioBtn7.Checked = true;
			this.fileNameTypeRadioBtn7.Location = new System.Drawing.Point(15, 178);
			this.fileNameTypeRadioBtn7.Name = "fileNameTypeRadioBtn7";
			this.fileNameTypeRadioBtn7.Size = new System.Drawing.Size(347, 21);
			this.fileNameTypeRadioBtn7.TabIndex = 0;
			this.fileNameTypeRadioBtn7.TabStop = true;
			this.fileNameTypeRadioBtn7.Text = "日付_チャンネル名(チャンネルID)_名前_放送タイトル(放送ID)_xx";
			this.fileNameTypeRadioBtn7.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn5
			// 
			this.fileNameTypeRadioBtn5.Checked = true;
			this.fileNameTypeRadioBtn5.Location = new System.Drawing.Point(15, 128);
			this.fileNameTypeRadioBtn5.Name = "fileNameTypeRadioBtn5";
			this.fileNameTypeRadioBtn5.Size = new System.Drawing.Size(314, 21);
			this.fileNameTypeRadioBtn5.TabIndex = 0;
			this.fileNameTypeRadioBtn5.TabStop = true;
			this.fileNameTypeRadioBtn5.Text = "放送タイトル(放送ID)_名前_チャンネル名(チャンネルID)_xx";
			this.fileNameTypeRadioBtn5.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn6
			// 
			this.fileNameTypeRadioBtn6.Checked = true;
			this.fileNameTypeRadioBtn6.Location = new System.Drawing.Point(15, 158);
			this.fileNameTypeRadioBtn6.Name = "fileNameTypeRadioBtn6";
			this.fileNameTypeRadioBtn6.Size = new System.Drawing.Size(344, 21);
			this.fileNameTypeRadioBtn6.TabIndex = 0;
			this.fileNameTypeRadioBtn6.TabStop = true;
			this.fileNameTypeRadioBtn6.Text = "日付_名前_チャンネル名(チャンネルID)_放送タイトル(放送ID)_xx";
			this.fileNameTypeRadioBtn6.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn4
			// 
			this.fileNameTypeRadioBtn4.Checked = true;
			this.fileNameTypeRadioBtn4.Location = new System.Drawing.Point(15, 108);
			this.fileNameTypeRadioBtn4.Name = "fileNameTypeRadioBtn4";
			this.fileNameTypeRadioBtn4.Size = new System.Drawing.Size(314, 21);
			this.fileNameTypeRadioBtn4.TabIndex = 0;
			this.fileNameTypeRadioBtn4.TabStop = true;
			this.fileNameTypeRadioBtn4.Text = "チャンネル名(チャンネルID)_名前_放送タイトル(放送ID)_xx";
			this.fileNameTypeRadioBtn4.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn3
			// 
			this.fileNameTypeRadioBtn3.Checked = true;
			this.fileNameTypeRadioBtn3.Location = new System.Drawing.Point(15, 88);
			this.fileNameTypeRadioBtn3.Name = "fileNameTypeRadioBtn3";
			this.fileNameTypeRadioBtn3.Size = new System.Drawing.Size(314, 21);
			this.fileNameTypeRadioBtn3.TabIndex = 0;
			this.fileNameTypeRadioBtn3.TabStop = true;
			this.fileNameTypeRadioBtn3.Text = "名前_チャンネル名(チャンネルID)_放送タイトル(放送ID)_xx";
			this.fileNameTypeRadioBtn3.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn2
			// 
			this.fileNameTypeRadioBtn2.Checked = true;
			this.fileNameTypeRadioBtn2.Location = new System.Drawing.Point(15, 58);
			this.fileNameTypeRadioBtn2.Name = "fileNameTypeRadioBtn2";
			this.fileNameTypeRadioBtn2.Size = new System.Drawing.Size(314, 21);
			this.fileNameTypeRadioBtn2.TabIndex = 0;
			this.fileNameTypeRadioBtn2.TabStop = true;
			this.fileNameTypeRadioBtn2.Text = "放送ID(放送タイトル)_名前_チャンネルID(チャンネル名)_xx";
			this.fileNameTypeRadioBtn2.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn1
			// 
			this.fileNameTypeRadioBtn1.Checked = true;
			this.fileNameTypeRadioBtn1.Location = new System.Drawing.Point(15, 38);
			this.fileNameTypeRadioBtn1.Name = "fileNameTypeRadioBtn1";
			this.fileNameTypeRadioBtn1.Size = new System.Drawing.Size(314, 21);
			this.fileNameTypeRadioBtn1.TabIndex = 0;
			this.fileNameTypeRadioBtn1.TabStop = true;
			this.fileNameTypeRadioBtn1.Text = "チャンネルID(チャンネル名)_名前_放送ID(放送タイトル)_xx";
			this.fileNameTypeRadioBtn1.UseVisualStyleBackColor = true;
			// 
			// fileNameTypeRadioBtn0
			// 
			this.fileNameTypeRadioBtn0.Checked = true;
			this.fileNameTypeRadioBtn0.Location = new System.Drawing.Point(15, 18);
			this.fileNameTypeRadioBtn0.Name = "fileNameTypeRadioBtn0";
			this.fileNameTypeRadioBtn0.Size = new System.Drawing.Size(314, 21);
			this.fileNameTypeRadioBtn0.TabIndex = 0;
			this.fileNameTypeRadioBtn0.TabStop = true;
			this.fileNameTypeRadioBtn0.Text = "名前_チャンネルID(チャンネル名)_放送ID(放送タイトル)_xx";
			this.fileNameTypeRadioBtn0.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.housoushaUserIDRadioBtn);
			this.groupBox1.Controls.Add(this.userIDHousoushaRadioBtn);
			this.groupBox1.Controls.Add(this.userIDRadioBtn);
			this.groupBox1.Controls.Add(this.ComIDComNameRadioBtn);
			this.groupBox1.Controls.Add(this.comIDRadioBtn);
			this.groupBox1.Controls.Add(this.housoushaComIDRadioBtn);
			this.groupBox1.Controls.Add(this.comIDHousoushaRadioBtn);
			this.groupBox1.Controls.Add(this.comNameRadioBtn);
			this.groupBox1.Controls.Add(this.housoushaRadioBtn);
			this.groupBox1.Controls.Add(this.useSubFolderChk);
			this.groupBox1.Controls.Add(this.recFolderSanshouBtn);
			this.groupBox1.Controls.Add(this.baseDirectoryText);
			this.groupBox1.Location = new System.Drawing.Point(5, 27);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox1.Size = new System.Drawing.Size(422, 143);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "基本フォルダ";
			// 
			// housoushaUserIDRadioBtn
			// 
			this.housoushaUserIDRadioBtn.Location = new System.Drawing.Point(279, 113);
			this.housoushaUserIDRadioBtn.Name = "housoushaUserIDRadioBtn";
			this.housoushaUserIDRadioBtn.Size = new System.Drawing.Size(141, 27);
			this.housoushaUserIDRadioBtn.TabIndex = 4;
			this.housoushaUserIDRadioBtn.Text = "放送者名＋ユーザーID";
			this.housoushaUserIDRadioBtn.UseVisualStyleBackColor = true;
			// 
			// userIDHousoushaRadioBtn
			// 
			this.userIDHousoushaRadioBtn.Location = new System.Drawing.Point(206, 70);
			this.userIDHousoushaRadioBtn.Name = "userIDHousoushaRadioBtn";
			this.userIDHousoushaRadioBtn.Size = new System.Drawing.Size(141, 27);
			this.userIDHousoushaRadioBtn.TabIndex = 4;
			this.userIDHousoushaRadioBtn.Text = "ユーザーID＋放送者名";
			this.userIDHousoushaRadioBtn.UseVisualStyleBackColor = true;
			// 
			// userIDRadioBtn
			// 
			this.userIDRadioBtn.Location = new System.Drawing.Point(98, 70);
			this.userIDRadioBtn.Name = "userIDRadioBtn";
			this.userIDRadioBtn.Size = new System.Drawing.Size(87, 27);
			this.userIDRadioBtn.TabIndex = 4;
			this.userIDRadioBtn.Text = "ユーザーID";
			this.userIDRadioBtn.UseVisualStyleBackColor = true;
			// 
			// ComIDComNameRadioBtn
			// 
			this.ComIDComNameRadioBtn.Location = new System.Drawing.Point(206, 93);
			this.ComIDComNameRadioBtn.Name = "ComIDComNameRadioBtn";
			this.ComIDComNameRadioBtn.Size = new System.Drawing.Size(153, 27);
			this.ComIDComNameRadioBtn.TabIndex = 4;
			this.ComIDComNameRadioBtn.Text = "チャンネルID＋チャンネル名";
			this.ComIDComNameRadioBtn.UseVisualStyleBackColor = true;
			// 
			// comIDRadioBtn
			// 
			this.comIDRadioBtn.Location = new System.Drawing.Point(98, 93);
			this.comIDRadioBtn.Name = "comIDRadioBtn";
			this.comIDRadioBtn.Size = new System.Drawing.Size(87, 27);
			this.comIDRadioBtn.TabIndex = 4;
			this.comIDRadioBtn.Text = "チャンネルID";
			this.comIDRadioBtn.UseVisualStyleBackColor = true;
			// 
			// housoushaComIDRadioBtn
			// 
			this.housoushaComIDRadioBtn.Location = new System.Drawing.Point(141, 113);
			this.housoushaComIDRadioBtn.Name = "housoushaComIDRadioBtn";
			this.housoushaComIDRadioBtn.Size = new System.Drawing.Size(141, 27);
			this.housoushaComIDRadioBtn.TabIndex = 4;
			this.housoushaComIDRadioBtn.Text = "放送者名＋チャンネルID";
			this.housoushaComIDRadioBtn.UseVisualStyleBackColor = true;
			// 
			// comIDHousoushaRadioBtn
			// 
			this.comIDHousoushaRadioBtn.Location = new System.Drawing.Point(5, 113);
			this.comIDHousoushaRadioBtn.Name = "comIDHousoushaRadioBtn";
			this.comIDHousoushaRadioBtn.Size = new System.Drawing.Size(143, 27);
			this.comIDHousoushaRadioBtn.TabIndex = 4;
			this.comIDHousoushaRadioBtn.Text = "チャンネルID＋放送者名";
			this.comIDHousoushaRadioBtn.UseVisualStyleBackColor = true;
			// 
			// comNameRadioBtn
			// 
			this.comNameRadioBtn.Location = new System.Drawing.Point(5, 93);
			this.comNameRadioBtn.Name = "comNameRadioBtn";
			this.comNameRadioBtn.Size = new System.Drawing.Size(87, 27);
			this.comNameRadioBtn.TabIndex = 4;
			this.comNameRadioBtn.Text = "チャンネル名";
			this.comNameRadioBtn.UseVisualStyleBackColor = true;
			// 
			// housoushaRadioBtn
			// 
			this.housoushaRadioBtn.Checked = true;
			this.housoushaRadioBtn.Location = new System.Drawing.Point(5, 70);
			this.housoushaRadioBtn.Name = "housoushaRadioBtn";
			this.housoushaRadioBtn.Size = new System.Drawing.Size(87, 27);
			this.housoushaRadioBtn.TabIndex = 4;
			this.housoushaRadioBtn.TabStop = true;
			this.housoushaRadioBtn.Text = "放送者名";
			this.housoushaRadioBtn.UseVisualStyleBackColor = true;
			// 
			// useSubFolderChk
			// 
			this.useSubFolderChk.Location = new System.Drawing.Point(4, 50);
			this.useSubFolderChk.Name = "useSubFolderChk";
			this.useSubFolderChk.Size = new System.Drawing.Size(216, 24);
			this.useSubFolderChk.TabIndex = 3;
			this.useSubFolderChk.Text = "以下のサブフォルダを作る";
			this.useSubFolderChk.UseVisualStyleBackColor = true;
			// 
			// recFolderSanshouBtn
			// 
			this.recFolderSanshouBtn.Location = new System.Drawing.Point(312, 17);
			this.recFolderSanshouBtn.Margin = new System.Windows.Forms.Padding(2);
			this.recFolderSanshouBtn.Name = "recFolderSanshouBtn";
			this.recFolderSanshouBtn.Size = new System.Drawing.Size(50, 20);
			this.recFolderSanshouBtn.TabIndex = 1;
			this.recFolderSanshouBtn.Text = "参照";
			this.recFolderSanshouBtn.UseVisualStyleBackColor = true;
			this.recFolderSanshouBtn.Click += new System.EventHandler(this.RecFolderSanshouBtnClick);
			// 
			// baseDirectoryText
			// 
			this.baseDirectoryText.Location = new System.Drawing.Point(4, 17);
			this.baseDirectoryText.Margin = new System.Windows.Forms.Padding(2);
			this.baseDirectoryText.Name = "baseDirectoryText";
			this.baseDirectoryText.Size = new System.Drawing.Size(304, 19);
			this.baseDirectoryText.TabIndex = 0;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.extText);
			this.groupBox3.Location = new System.Drawing.Point(5, 462);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(157, 52);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "形式";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 19);
			this.label1.TabIndex = 22;
			this.label1.Text = "拡張子：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// extText
			// 
			this.extText.Location = new System.Drawing.Point(69, 21);
			this.extText.Margin = new System.Windows.Forms.Padding(2);
			this.extText.Name = "extText";
			this.extText.Size = new System.Drawing.Size(58, 19);
			this.extText.TabIndex = 21;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(9, 10);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(443, 546);
			this.tabControl1.TabIndex = 7;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(435, 520);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "{file}引数";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(420, 19);
			this.label2.TabIndex = 23;
			this.label2.Text = "引数設定内の {file} をこちらで設定されたファイルパスに置き換えてアプリを起動します。";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.groupBox12);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(435, 520);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "一般";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// groupBox12
			// 
			this.groupBox12.Controls.Add(this.isMinimizedChkBox);
			this.groupBox12.Location = new System.Drawing.Point(5, 5);
			this.groupBox12.Name = "groupBox12";
			this.groupBox12.Size = new System.Drawing.Size(424, 48);
			this.groupBox12.TabIndex = 52;
			this.groupBox12.TabStop = false;
			this.groupBox12.Text = "起動設定";
			// 
			// isMinimizedChkBox
			// 
			this.isMinimizedChkBox.Location = new System.Drawing.Point(19, 18);
			this.isMinimizedChkBox.Name = "isMinimizedChkBox";
			this.isMinimizedChkBox.Size = new System.Drawing.Size(141, 19);
			this.isMinimizedChkBox.TabIndex = 2;
			this.isMinimizedChkBox.Text = "最小化状態で起動する";
			this.isMinimizedChkBox.UseVisualStyleBackColor = true;
			// 
			// AppSettingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(455, 597);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Name = "AppSettingForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "アプリ詳細設定";
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupBox12.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox isMinimizedChkBox;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TextBox extText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox baseDirectoryText;
		private System.Windows.Forms.Button recFolderSanshouBtn;
		private System.Windows.Forms.CheckBox useSubFolderChk;
		private System.Windows.Forms.RadioButton housoushaRadioBtn;
		private System.Windows.Forms.RadioButton comNameRadioBtn;
		private System.Windows.Forms.RadioButton comIDHousoushaRadioBtn;
		private System.Windows.Forms.RadioButton housoushaComIDRadioBtn;
		private System.Windows.Forms.RadioButton comIDRadioBtn;
		private System.Windows.Forms.RadioButton ComIDComNameRadioBtn;
		private System.Windows.Forms.RadioButton userIDRadioBtn;
		private System.Windows.Forms.RadioButton userIDHousoushaRadioBtn;
		private System.Windows.Forms.RadioButton housoushaUserIDRadioBtn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn0;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn1;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn2;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn3;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn4;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn6;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn5;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn7;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn8;
		private System.Windows.Forms.RadioButton fileNameTypeRadioBtn9;
		private System.Windows.Forms.Button fileNameTypeDokujiSetteiBtn;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
	}
}
