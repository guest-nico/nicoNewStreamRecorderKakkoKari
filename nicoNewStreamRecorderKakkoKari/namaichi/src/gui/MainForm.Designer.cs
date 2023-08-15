using System.Runtime.InteropServices;
/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/06
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace namaichi
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.urlText = new System.Windows.Forms.TextBox();
			this.recBtn = new System.Windows.Forms.Button();
			this.logText = new System.Windows.Forms.TextBox();
			this.recordStateLabel = new System.Windows.Forms.Label();
			this.groupLabel = new System.Windows.Forms.Label();
			this.samuneBox = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.録画フォルダを開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openSettingFolderMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.openTourokuExeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.recEndMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recEndNothingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recEndShutdownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recEndLogOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recEndSuspendMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.visualMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.formColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.characterColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.linkColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openReadmeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.更新方法VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.バージョン情報VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.streamInfoGroupBox = new System.Windows.Forms.GroupBox();
			this.reservationBox = new System.Windows.Forms.PictureBox();
			this.endTimeLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.miniStreamStateLabel = new System.Windows.Forms.Label();
			this.typeLabel = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.descriptLabel = new System.Windows.Forms.Label();
			this.keikaTimeLabel = new System.Windows.Forms.Label();
			this.startTimeLabel = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.genteiLabel = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.hostLabel = new System.Windows.Forms.LinkLabel();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.titleLabel = new System.Windows.Forms.LinkLabel();
			this.label5 = new System.Windows.Forms.Label();
			this.communityLabel = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.mainWindowRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.urlLabel = new System.Windows.Forms.Label();
			this.streamStateGroupBox = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.commentLabel = new System.Windows.Forms.Label();
			this.visitLabel = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.recordGroupBox = new System.Windows.Forms.GroupBox();
			this.commentList = new System.Windows.Forms.DataGridView();
			this.時間 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.playerBtn = new System.Windows.Forms.Button();
			this.miniBtn = new System.Windows.Forms.Button();
			this.isChaseChkBtn = new System.Windows.Forms.CheckBox();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.notifyIconMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.notifyIconRecentSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.openNotifyIconMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.closeNotifyIconMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.直近の動作の記録を確認するVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.samuneBox)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.streamInfoGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.reservationBox)).BeginInit();
			this.mainWindowRightClickMenu.SuspendLayout();
			this.streamStateGroupBox.SuspendLayout();
			this.recordGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.commentList)).BeginInit();
			this.notifyIconMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// urlText
			// 
			this.urlText.Location = new System.Drawing.Point(69, 38);
			this.urlText.Margin = new System.Windows.Forms.Padding(2);
			this.urlText.Name = "urlText";
			this.urlText.Size = new System.Drawing.Size(241, 19);
			this.urlText.TabIndex = 0;
			// 
			// recBtn
			// 
			this.recBtn.Location = new System.Drawing.Point(314, 35);
			this.recBtn.Margin = new System.Windows.Forms.Padding(2);
			this.recBtn.Name = "recBtn";
			this.recBtn.Size = new System.Drawing.Size(75, 24);
			this.recBtn.TabIndex = 1;
			this.recBtn.Text = "録画開始";
			this.recBtn.UseVisualStyleBackColor = true;
			this.recBtn.Click += new System.EventHandler(this.recBtnAction);
			// 
			// logText
			// 
			this.logText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.logText.Location = new System.Drawing.Point(6, 276);
			this.logText.Margin = new System.Windows.Forms.Padding(2);
			this.logText.Multiline = true;
			this.logText.Name = "logText";
			this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logText.Size = new System.Drawing.Size(249, 74);
			this.logText.TabIndex = 5;
			// 
			// recordStateLabel
			// 
			this.recordStateLabel.Location = new System.Drawing.Point(5, 14);
			this.recordStateLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.recordStateLabel.Name = "recordStateLabel";
			this.recordStateLabel.Size = new System.Drawing.Size(155, 37);
			this.recordStateLabel.TabIndex = 6;
			// 
			// groupLabel
			// 
			this.groupLabel.Location = new System.Drawing.Point(0, 0);
			this.groupLabel.Name = "groupLabel";
			this.groupLabel.Size = new System.Drawing.Size(100, 23);
			this.groupLabel.TabIndex = 0;
			this.groupLabel.Text = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			// 
			// samuneBox
			// 
			this.samuneBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.samuneBox.ErrorImage = null;
			this.samuneBox.Image = ((System.Drawing.Image)(resources.GetObject("samuneBox.Image")));
			this.samuneBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("samuneBox.InitialImage")));
			this.samuneBox.Location = new System.Drawing.Point(12, 61);
			this.samuneBox.Margin = new System.Windows.Forms.Padding(2);
			this.samuneBox.Name = "samuneBox";
			this.samuneBox.Size = new System.Drawing.Size(141, 150);
			this.samuneBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.samuneBox.TabIndex = 0;
			this.samuneBox.TabStop = false;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileMenuItem,
									this.toolMenuItem,
									this.visualMenuItem,
									this.helpMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(719, 24);
			this.menuStrip1.TabIndex = 11;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.録画フォルダを開くToolStripMenuItem,
									this.openSettingFolderMenu,
									this.toolStripSeparator3,
									this.openTourokuExeMenu,
									this.toolStripSeparator1,
									this.終了ToolStripMenuItem});
			this.fileMenuItem.Name = "fileMenuItem";
			this.fileMenuItem.ShowShortcutKeys = false;
			this.fileMenuItem.Size = new System.Drawing.Size(67, 20);
			this.fileMenuItem.Text = "ファイル(&F)";
			// 
			// 録画フォルダを開くToolStripMenuItem
			// 
			this.録画フォルダを開くToolStripMenuItem.Name = "録画フォルダを開くToolStripMenuItem";
			this.録画フォルダを開くToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.録画フォルダを開くToolStripMenuItem.Text = "録画フォルダを開く(&O)";
			this.録画フォルダを開くToolStripMenuItem.Click += new System.EventHandler(this.openRecFolderMenu_Click);
			// 
			// openSettingFolderMenu
			// 
			this.openSettingFolderMenu.Name = "openSettingFolderMenu";
			this.openSettingFolderMenu.Size = new System.Drawing.Size(217, 22);
			this.openSettingFolderMenu.Text = "設定ファイルフォルダーを開く(&F)";
			this.openSettingFolderMenu.Click += new System.EventHandler(this.OpenSettingFolderMenuClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(214, 6);
			// 
			// openTourokuExeMenu
			// 
			this.openTourokuExeMenu.Name = "openTourokuExeMenu";
			this.openTourokuExeMenu.Size = new System.Drawing.Size(217, 22);
			this.openTourokuExeMenu.Text = "録画登録ツールを起動する(&E)";
			this.openTourokuExeMenu.Click += new System.EventHandler(this.OpenTourokuExeMenuClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(214, 6);
			// 
			// 終了ToolStripMenuItem
			// 
			this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
			this.終了ToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.終了ToolStripMenuItem.Text = "終了(&X)";
			this.終了ToolStripMenuItem.Click += new System.EventHandler(this.endMenu_Click);
			// 
			// toolMenuItem
			// 
			this.toolMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.optionMenuItem,
									this.toolStripSeparator4,
									this.recEndMenuItem});
			this.toolMenuItem.Name = "toolMenuItem";
			this.toolMenuItem.ShowShortcutKeys = false;
			this.toolMenuItem.Size = new System.Drawing.Size(60, 20);
			this.toolMenuItem.Text = "ツール(&T)";
			// 
			// optionMenuItem
			// 
			this.optionMenuItem.Name = "optionMenuItem";
			this.optionMenuItem.ShowShortcutKeys = false;
			this.optionMenuItem.Size = new System.Drawing.Size(168, 22);
			this.optionMenuItem.Text = "オプション(&O)";
			this.optionMenuItem.Click += new System.EventHandler(this.optionItem_Select);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(165, 6);
			// 
			// recEndMenuItem
			// 
			this.recEndMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.recEndNothingMenuItem,
									this.recEndShutdownMenuItem,
									this.recEndLogOffMenuItem,
									this.recEndSuspendMenuItem});
			this.recEndMenuItem.Name = "recEndMenuItem";
			this.recEndMenuItem.Size = new System.Drawing.Size(168, 22);
			this.recEndMenuItem.Text = "録画終了時の動作";
			this.recEndMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.RecEndMenuItemDropDownItemClicked);
			// 
			// recEndNothingMenuItem
			// 
			this.recEndNothingMenuItem.Checked = true;
			this.recEndNothingMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.recEndNothingMenuItem.Name = "recEndNothingMenuItem";
			this.recEndNothingMenuItem.Size = new System.Drawing.Size(178, 22);
			this.recEndNothingMenuItem.Text = "何もしない";
			// 
			// recEndShutdownMenuItem
			// 
			this.recEndShutdownMenuItem.Name = "recEndShutdownMenuItem";
			this.recEndShutdownMenuItem.Size = new System.Drawing.Size(178, 22);
			this.recEndShutdownMenuItem.Text = "OSをシャットダウンする";
			// 
			// recEndLogOffMenuItem
			// 
			this.recEndLogOffMenuItem.Name = "recEndLogOffMenuItem";
			this.recEndLogOffMenuItem.Size = new System.Drawing.Size(178, 22);
			this.recEndLogOffMenuItem.Text = "OSをログオフする";
			// 
			// recEndSuspendMenuItem
			// 
			this.recEndSuspendMenuItem.Name = "recEndSuspendMenuItem";
			this.recEndSuspendMenuItem.Size = new System.Drawing.Size(178, 22);
			this.recEndSuspendMenuItem.Text = "OSを休止状態にする";
			// 
			// visualMenuItem
			// 
			this.visualMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.formColorMenuItem,
									this.characterColorMenuItem,
									this.linkColorMenuItem});
			this.visualMenuItem.Name = "visualMenuItem";
			this.visualMenuItem.Size = new System.Drawing.Size(58, 20);
			this.visualMenuItem.Text = "表示(&V)";
			// 
			// formColorMenuItem
			// 
			this.formColorMenuItem.Name = "formColorMenuItem";
			this.formColorMenuItem.Size = new System.Drawing.Size(160, 22);
			this.formColorMenuItem.Text = "ウィンドウの色(&W)";
			this.formColorMenuItem.Click += new System.EventHandler(this.ColorMenuItemClick);
			// 
			// characterColorMenuItem
			// 
			this.characterColorMenuItem.Name = "characterColorMenuItem";
			this.characterColorMenuItem.Size = new System.Drawing.Size(160, 22);
			this.characterColorMenuItem.Text = "文字の色(&S)";
			this.characterColorMenuItem.Click += new System.EventHandler(this.CharacterColorMenuItemClick);
			// 
			// linkColorMenuItem
			// 
			this.linkColorMenuItem.Name = "linkColorMenuItem";
			this.linkColorMenuItem.Size = new System.Drawing.Size(160, 22);
			this.linkColorMenuItem.Text = "リンク文字の色(&L)";
			this.linkColorMenuItem.Click += new System.EventHandler(this.LinkColorMenuItemClick);
			// 
			// helpMenuItem
			// 
			this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.openReadmeMenu,
									this.直近の動作の記録を確認するVToolStripMenuItem,
									this.更新方法VToolStripMenuItem,
									this.バージョン情報VToolStripMenuItem});
			this.helpMenuItem.Name = "helpMenuItem";
			this.helpMenuItem.ShowShortcutKeys = false;
			this.helpMenuItem.Size = new System.Drawing.Size(65, 20);
			this.helpMenuItem.Text = "ヘルプ(&H)";
			// 
			// openReadmeMenu
			// 
			this.openReadmeMenu.Name = "openReadmeMenu";
			this.openReadmeMenu.Size = new System.Drawing.Size(223, 22);
			this.openReadmeMenu.Text = "readme.htmlを開く(&V)";
			this.openReadmeMenu.Click += new System.EventHandler(this.OpenReadmeMenuClick);
			// 
			// 更新方法VToolStripMenuItem
			// 
			this.更新方法VToolStripMenuItem.Name = "更新方法VToolStripMenuItem";
			this.更新方法VToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
			this.更新方法VToolStripMenuItem.Text = "更新方法(&U)";
			this.更新方法VToolStripMenuItem.Click += new System.EventHandler(this.updateMenu_Click);
			// 
			// バージョン情報VToolStripMenuItem
			// 
			this.バージョン情報VToolStripMenuItem.Name = "バージョン情報VToolStripMenuItem";
			this.バージョン情報VToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
			this.バージョン情報VToolStripMenuItem.Text = "バージョン情報(&A)";
			this.バージョン情報VToolStripMenuItem.Click += new System.EventHandler(this.versionMenu_Click);
			// 
			// streamInfoGroupBox
			// 
			this.streamInfoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.streamInfoGroupBox.Controls.Add(this.reservationBox);
			this.streamInfoGroupBox.Controls.Add(this.endTimeLabel);
			this.streamInfoGroupBox.Controls.Add(this.label4);
			this.streamInfoGroupBox.Controls.Add(this.miniStreamStateLabel);
			this.streamInfoGroupBox.Controls.Add(this.typeLabel);
			this.streamInfoGroupBox.Controls.Add(this.label7);
			this.streamInfoGroupBox.Controls.Add(this.label10);
			this.streamInfoGroupBox.Controls.Add(this.descriptLabel);
			this.streamInfoGroupBox.Controls.Add(this.keikaTimeLabel);
			this.streamInfoGroupBox.Controls.Add(this.startTimeLabel);
			this.streamInfoGroupBox.Controls.Add(this.label8);
			this.streamInfoGroupBox.Controls.Add(this.genteiLabel);
			this.streamInfoGroupBox.Controls.Add(this.label6);
			this.streamInfoGroupBox.Controls.Add(this.hostLabel);
			this.streamInfoGroupBox.Controls.Add(this.label3);
			this.streamInfoGroupBox.Controls.Add(this.label2);
			this.streamInfoGroupBox.Controls.Add(this.titleLabel);
			this.streamInfoGroupBox.Controls.Add(this.label5);
			this.streamInfoGroupBox.Controls.Add(this.communityLabel);
			this.streamInfoGroupBox.Controls.Add(this.label1);
			this.streamInfoGroupBox.Location = new System.Drawing.Point(179, 76);
			this.streamInfoGroupBox.Name = "streamInfoGroupBox";
			this.streamInfoGroupBox.Size = new System.Drawing.Size(539, 180);
			this.streamInfoGroupBox.TabIndex = 17;
			this.streamInfoGroupBox.TabStop = false;
			this.streamInfoGroupBox.Text = "番組情報";
			// 
			// reservationBox
			// 
			this.reservationBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.reservationBox.Image = ((System.Drawing.Image)(resources.GetObject("reservationBox.Image")));
			this.reservationBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("reservationBox.InitialImage")));
			this.reservationBox.Location = new System.Drawing.Point(518, 23);
			this.reservationBox.Name = "reservationBox";
			this.reservationBox.Size = new System.Drawing.Size(15, 15);
			this.reservationBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.reservationBox.TabIndex = 7;
			this.reservationBox.TabStop = false;
			this.reservationBox.Visible = false;
			// 
			// endTimeLabel
			// 
			this.endTimeLabel.Location = new System.Drawing.Point(78, 163);
			this.endTimeLabel.Name = "endTimeLabel";
			this.endTimeLabel.Size = new System.Drawing.Size(118, 14);
			this.endTimeLabel.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 163);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 15);
			this.label4.TabIndex = 0;
			this.label4.Text = "終了時刻";
			// 
			// miniStreamStateLabel
			// 
			this.miniStreamStateLabel.Location = new System.Drawing.Point(156, 50);
			this.miniStreamStateLabel.Name = "miniStreamStateLabel";
			this.miniStreamStateLabel.Size = new System.Drawing.Size(100, 17);
			this.miniStreamStateLabel.TabIndex = 1;
			this.miniStreamStateLabel.Visible = false;
			// 
			// typeLabel
			// 
			this.typeLabel.Location = new System.Drawing.Point(78, 106);
			this.typeLabel.Name = "typeLabel";
			this.typeLabel.Size = new System.Drawing.Size(118, 17);
			this.typeLabel.TabIndex = 6;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 106);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(56, 19);
			this.label7.TabIndex = 5;
			this.label7.Text = "放送種別";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(215, 85);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(85, 21);
			this.label10.TabIndex = 4;
			this.label10.Text = "説明";
			// 
			// descriptLabel
			// 
			this.descriptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.descriptLabel.Location = new System.Drawing.Point(214, 106);
			this.descriptLabel.Name = "descriptLabel";
			this.descriptLabel.Size = new System.Drawing.Size(310, 72);
			this.descriptLabel.TabIndex = 3;
			// 
			// keikaTimeLabel
			// 
			this.keikaTimeLabel.Location = new System.Drawing.Point(78, 144);
			this.keikaTimeLabel.Name = "keikaTimeLabel";
			this.keikaTimeLabel.Size = new System.Drawing.Size(100, 17);
			this.keikaTimeLabel.TabIndex = 3;
			// 
			// startTimeLabel
			// 
			this.startTimeLabel.Location = new System.Drawing.Point(78, 125);
			this.startTimeLabel.Name = "startTimeLabel";
			this.startTimeLabel.Size = new System.Drawing.Size(100, 17);
			this.startTimeLabel.TabIndex = 3;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 144);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(56, 19);
			this.label8.TabIndex = 0;
			this.label8.Text = "経過時間";
			// 
			// genteiLabel
			// 
			this.genteiLabel.Location = new System.Drawing.Point(78, 87);
			this.genteiLabel.Name = "genteiLabel";
			this.genteiLabel.Size = new System.Drawing.Size(118, 17);
			this.genteiLabel.TabIndex = 3;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 125);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 19);
			this.label6.TabIndex = 0;
			this.label6.Text = "開始時刻";
			// 
			// hostLabel
			// 
			this.hostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.hostLabel.Location = new System.Drawing.Point(78, 68);
			this.hostLabel.Name = "hostLabel";
			this.hostLabel.Size = new System.Drawing.Size(438, 12);
			this.hostLabel.TabIndex = 2;
			this.hostLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.titleLabel_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 87);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 19);
			this.label3.TabIndex = 0;
			this.label3.Text = "限定放送";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 19);
			this.label2.TabIndex = 0;
			this.label2.Text = "放送者";
			// 
			// titleLabel
			// 
			this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.titleLabel.Location = new System.Drawing.Point(78, 26);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(446, 12);
			this.titleLabel.TabIndex = 2;
			this.titleLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.titleLabel_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 26);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(66, 19);
			this.label5.TabIndex = 0;
			this.label5.Text = "タイトル";
			// 
			// communityLabel
			// 
			this.communityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.communityLabel.Location = new System.Drawing.Point(78, 49);
			this.communityLabel.Name = "communityLabel";
			this.communityLabel.Size = new System.Drawing.Size(446, 12);
			this.communityLabel.TabIndex = 2;
			this.communityLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.titleLabel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "コミュニティ";
			// 
			// mainWindowRightClickMenu
			// 
			this.mainWindowRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem1,
									this.toolStripMenuItem2,
									this.toolStripMenuItem3,
									this.toolStripSeparator2,
									this.toolStripMenuItem4});
			this.mainWindowRightClickMenu.Name = "contextMenuStrip1";
			this.mainWindowRightClickMenu.Size = new System.Drawing.Size(199, 98);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(198, 22);
			this.toolStripMenuItem1.Text = "放送URLをコピー";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.copyUrlMenu_Clicked);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(198, 22);
			this.toolStripMenuItem2.Text = "コミュニティURLをコピー";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.copyCommunityUrlMenu_Clicked);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(198, 22);
			this.toolStripMenuItem3.Text = "放送者と放送URLをコピー";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.copyHost_UrlMenu_Clicked);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(198, 22);
			this.toolStripMenuItem4.Text = "録画フォルダを開く";
			this.toolStripMenuItem4.Click += new System.EventHandler(this.openRecFolderMenu_Click);
			// 
			// urlLabel
			// 
			this.urlLabel.Location = new System.Drawing.Point(6, 41);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new System.Drawing.Size(58, 18);
			this.urlLabel.TabIndex = 18;
			this.urlLabel.Text = "放送URL";
			// 
			// streamStateGroupBox
			// 
			this.streamStateGroupBox.Controls.Add(this.label15);
			this.streamStateGroupBox.Controls.Add(this.commentLabel);
			this.streamStateGroupBox.Controls.Add(this.visitLabel);
			this.streamStateGroupBox.Controls.Add(this.label13);
			this.streamStateGroupBox.Location = new System.Drawing.Point(467, 30);
			this.streamStateGroupBox.Name = "streamStateGroupBox";
			this.streamStateGroupBox.Size = new System.Drawing.Size(229, 40);
			this.streamStateGroupBox.TabIndex = 19;
			this.streamStateGroupBox.TabStop = false;
			this.streamStateGroupBox.Text = "番組状況";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(113, 15);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(52, 17);
			this.label15.TabIndex = 2;
			this.label15.Text = "/　コメント数";
			// 
			// commentLabel
			// 
			this.commentLabel.Location = new System.Drawing.Point(171, 15);
			this.commentLabel.Name = "commentLabel";
			this.commentLabel.Size = new System.Drawing.Size(53, 11);
			this.commentLabel.TabIndex = 1;
			// 
			// visitLabel
			// 
			this.visitLabel.Location = new System.Drawing.Point(60, 16);
			this.visitLabel.Name = "visitLabel";
			this.visitLabel.Size = new System.Drawing.Size(59, 13);
			this.visitLabel.TabIndex = 1;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 16);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(57, 17);
			this.label13.TabIndex = 0;
			this.label13.Text = "来場者数";
			// 
			// recordGroupBox
			// 
			this.recordGroupBox.Controls.Add(this.recordStateLabel);
			this.recordGroupBox.Location = new System.Drawing.Point(6, 217);
			this.recordGroupBox.Name = "recordGroupBox";
			this.recordGroupBox.Size = new System.Drawing.Size(167, 54);
			this.recordGroupBox.TabIndex = 20;
			this.recordGroupBox.TabStop = false;
			this.recordGroupBox.Text = "転送状況";
			// 
			// commentList
			// 
			this.commentList.AllowUserToAddRows = false;
			this.commentList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.commentList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.commentList.BackgroundColor = System.Drawing.SystemColors.Window;
			this.commentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.commentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.時間,
									this.Column1});
			this.commentList.Location = new System.Drawing.Point(262, 262);
			this.commentList.Name = "commentList";
			this.commentList.RowHeadersVisible = false;
			this.commentList.RowTemplate.Height = 21;
			this.commentList.Size = new System.Drawing.Size(450, 88);
			this.commentList.TabIndex = 21;
			// 
			// 時間
			// 
			this.時間.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.時間.FillWeight = 35F;
			this.時間.HeaderText = "時間";
			this.時間.Name = "時間";
			this.時間.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.時間.Width = 93;
			// 
			// Column1
			// 
			this.Column1.HeaderText = "コメント";
			this.Column1.Name = "Column1";
			// 
			// playerBtn
			// 
			this.playerBtn.Location = new System.Drawing.Point(397, 35);
			this.playerBtn.Name = "playerBtn";
			this.playerBtn.Size = new System.Drawing.Size(64, 24);
			this.playerBtn.TabIndex = 22;
			this.playerBtn.Text = "視聴";
			this.playerBtn.UseVisualStyleBackColor = true;
			this.playerBtn.Click += new System.EventHandler(this.PlayerBtnClick);
			// 
			// miniBtn
			// 
			this.miniBtn.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.miniBtn.Location = new System.Drawing.Point(698, 35);
			this.miniBtn.Name = "miniBtn";
			this.miniBtn.Size = new System.Drawing.Size(19, 23);
			this.miniBtn.TabIndex = 23;
			this.miniBtn.Text = "小";
			this.miniBtn.UseVisualStyleBackColor = true;
			this.miniBtn.Click += new System.EventHandler(this.MiniBtnClick);
			// 
			// isChaseChkBtn
			// 
			this.isChaseChkBtn.Location = new System.Drawing.Point(319, 65);
			this.isChaseChkBtn.Name = "isChaseChkBtn";
			this.isChaseChkBtn.Size = new System.Drawing.Size(171, 15);
			this.isChaseChkBtn.TabIndex = 24;
			this.isChaseChkBtn.Text = "録画設定して追っかけ録画";
			this.isChaseChkBtn.UseVisualStyleBackColor = true;
			// 
			// notifyIcon
			// 
			this.notifyIcon.BalloonTipTitle = "title";
			this.notifyIcon.ContextMenuStrip = this.notifyIconMenuStrip;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "ニコ生新配信録画ツール（仮";
			this.notifyIcon.DoubleClick += new System.EventHandler(this.NotifyIconDoubleClick);
			// 
			// notifyIconMenuStrip
			// 
			this.notifyIconMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.notifyIconRecentSeparator,
									this.openNotifyIconMenu,
									this.toolStripSeparator5,
									this.closeNotifyIconMenu});
			this.notifyIconMenuStrip.Name = "notifyIconMenuStrip";
			this.notifyIconMenuStrip.Size = new System.Drawing.Size(99, 60);
			// 
			// notifyIconRecentSeparator
			// 
			this.notifyIconRecentSeparator.Name = "notifyIconRecentSeparator";
			this.notifyIconRecentSeparator.Size = new System.Drawing.Size(95, 6);
			this.notifyIconRecentSeparator.Visible = false;
			// 
			// openNotifyIconMenu
			// 
			this.openNotifyIconMenu.Name = "openNotifyIconMenu";
			this.openNotifyIconMenu.Size = new System.Drawing.Size(98, 22);
			this.openNotifyIconMenu.Text = "開く";
			this.openNotifyIconMenu.Click += new System.EventHandler(this.OpenNotifyIconMenuClick);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(95, 6);
			// 
			// closeNotifyIconMenu
			// 
			this.closeNotifyIconMenu.Name = "closeNotifyIconMenu";
			this.closeNotifyIconMenu.Size = new System.Drawing.Size(98, 22);
			this.closeNotifyIconMenu.Text = "終了";
			this.closeNotifyIconMenu.Click += new System.EventHandler(this.CloseNotifyIconMenuClick);
			// 
			// 直近の動作の記録を確認するVToolStripMenuItem
			// 
			this.直近の動作の記録を確認するVToolStripMenuItem.Name = "直近の動作の記録を確認するVToolStripMenuItem";
			this.直近の動作の記録を確認するVToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
			this.直近の動作の記録を確認するVToolStripMenuItem.Text = "直近の動作記録を確認する(&R)";
			this.直近の動作の記録を確認するVToolStripMenuItem.Click += new System.EventHandler(this.openRecordLogToolStripMenuItemClick);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(719, 362);
			this.ContextMenuStrip = this.mainWindowRightClickMenu;
			this.Controls.Add(this.streamStateGroupBox);
			this.Controls.Add(this.isChaseChkBtn);
			this.Controls.Add(this.streamInfoGroupBox);
			this.Controls.Add(this.miniBtn);
			this.Controls.Add(this.playerBtn);
			this.Controls.Add(this.commentList);
			this.Controls.Add(this.recordGroupBox);
			this.Controls.Add(this.urlLabel);
			this.Controls.Add(this.logText);
			this.Controls.Add(this.recBtn);
			this.Controls.Add(this.urlText);
			this.Controls.Add(this.samuneBox);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "MainForm";
			this.Text = "ニコ生新配信録画ツール（仮 ver0.86.15";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Close);
			this.Load += new System.EventHandler(this.mainForm_Load);
			this.SizeChanged += new System.EventHandler(this.MainFormSizeChanged);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainFormDragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainFormDragEnter);
			((System.ComponentModel.ISupportInitialize)(this.samuneBox)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.streamInfoGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.reservationBox)).EndInit();
			this.mainWindowRightClickMenu.ResumeLayout(false);
			this.streamStateGroupBox.ResumeLayout(false);
			this.recordGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.commentList)).EndInit();
			this.notifyIconMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem 直近の動作の記録を確認するVToolStripMenuItem;
		private System.Windows.Forms.PictureBox reservationBox;
		public System.Windows.Forms.Label miniStreamStateLabel;
		private System.Windows.Forms.ToolStripMenuItem closeNotifyIconMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem openNotifyIconMenu;
		private System.Windows.Forms.ToolStripSeparator notifyIconRecentSeparator;
		private System.Windows.Forms.ContextMenuStrip notifyIconMenuStrip;
		public System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ToolStripMenuItem recEndSuspendMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recEndLogOffMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recEndShutdownMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recEndNothingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recEndMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem openTourokuExeMenu;
		private System.Windows.Forms.ToolStripMenuItem openReadmeMenu;
		private System.Windows.Forms.ToolStripMenuItem openSettingFolderMenu;
		private System.Windows.Forms.ToolStripMenuItem linkColorMenuItem;
		private System.Windows.Forms.ToolStripMenuItem characterColorMenuItem;
		private System.Windows.Forms.ToolStripMenuItem formColorMenuItem;
		private System.Windows.Forms.ToolStripMenuItem visualMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 更新方法VToolStripMenuItem;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label endTimeLabel;
		public System.Windows.Forms.CheckBox isChaseChkBtn;
		private System.Windows.Forms.Button miniBtn;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip mainWindowRightClickMenu;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label typeLabel;
		public System.Windows.Forms.Button playerBtn;
		public System.Windows.Forms.Label commentLabel;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn 時間;
		private System.Windows.Forms.DataGridView commentList;
		private System.Windows.Forms.GroupBox recordGroupBox;
		private System.Windows.Forms.Label label13;
		public System.Windows.Forms.Label visitLabel;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox streamStateGroupBox;
		private System.Windows.Forms.Label urlLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel communityLabel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.LinkLabel titleLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel hostLabel;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label genteiLabel;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label startTimeLabel;
		private System.Windows.Forms.Label keikaTimeLabel;
		private System.Windows.Forms.Label descriptLabel;
		private System.Windows.Forms.GroupBox streamInfoGroupBox;
		private System.Windows.Forms.ToolStripMenuItem バージョン情報VToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem 録画フォルダを開くToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem optionMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		public System.Windows.Forms.Label recordStateLabel;
		public System.Windows.Forms.TextBox logText;
		public System.Windows.Forms.Button recBtn;
		public System.Windows.Forms.TextBox urlText;
		private System.Windows.Forms.Label groupLabel;
		private System.Windows.Forms.PictureBox samuneBox;
	}
}
