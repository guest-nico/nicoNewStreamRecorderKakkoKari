/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/10
 * Time: 16:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku
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
			this.afterConvertModeList = new System.Windows.Forms.ComboBox();
			this.recList = new System.Windows.Forms.DataGridView();
			this.放送ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.形式 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.画質 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.タイムシフト設定 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.状態 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.タイトル = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.放送者 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.コミュニティ名 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.開始時刻 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.終了時刻 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ログ = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.openWatchUrlMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.openCommunityUrlMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.copyWatchUrlMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.copyCommunityUrlMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.openRecFolderMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.reAddRowMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteRowMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.録画フォルダを開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.バージョン情報VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label18 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.setTimeshiftBtn = new System.Windows.Forms.Button();
			this.qualityBtn = new System.Windows.Forms.Button();
			this.addListBtn = new System.Windows.Forms.Button();
			this.clearBtn = new System.Windows.Forms.Button();
			this.recBtn = new System.Windows.Forms.Button();
			this.urlText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.keikaTimeLabel = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.descriptLabel = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.communityUrlLabel = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.urlLabel = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.communityLabel = new System.Windows.Forms.Label();
			this.programTimeLabel = new System.Windows.Forms.Label();
			this.hostLabel = new System.Windows.Forms.Label();
			this.endTimeLabel = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.titleLabel = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.startTimeLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.samuneBox = new System.Windows.Forms.PictureBox();
			this.logText = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.recList)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.samuneBox)).BeginInit();
			this.SuspendLayout();
			// 
			// afterConvertModeList
			// 
			this.afterConvertModeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.afterConvertModeList.FormattingEnabled = true;
			this.afterConvertModeList.Items.AddRange(new object[] {
									"ts(変換無し)",
									"avi",
									"mp4",
									"flv",
									"mov",
									"wmv",
									"vob",
									"mkv",
									"mp3(音声)",
									"wav(音声)",
									"wma(音声)",
									"aac(音声)",
									"ogg(音声)"});
			this.afterConvertModeList.Location = new System.Drawing.Point(551, 39);
			this.afterConvertModeList.Name = "afterConvertModeList";
			this.afterConvertModeList.Size = new System.Drawing.Size(90, 20);
			this.afterConvertModeList.TabIndex = 5;
			// 
			// recList
			// 
			this.recList.AllowUserToAddRows = false;
			this.recList.AllowUserToDeleteRows = false;
			this.recList.AllowUserToResizeRows = false;
			this.recList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.recList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.recList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.放送ID,
									this.形式,
									this.画質,
									this.タイムシフト設定,
									this.状態,
									this.タイトル,
									this.放送者,
									this.コミュニティ名,
									this.開始時刻,
									this.終了時刻,
									this.ログ});
			this.recList.ContextMenuStrip = this.contextMenuStrip1;
			this.recList.Location = new System.Drawing.Point(14, 68);
			this.recList.Name = "recList";
			this.recList.ReadOnly = true;
			this.recList.RowHeadersVisible = false;
			this.recList.RowTemplate.Height = 21;
			this.recList.Size = new System.Drawing.Size(876, 187);
			this.recList.TabIndex = 6;
			this.recList.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.recListCell_MouseDown);
			this.recList.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.recList_DataError);
			this.recList.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.recList_FocusRowEnter);
			// 
			// 放送ID
			// 
			this.放送ID.DataPropertyName = "id";
			this.放送ID.HeaderText = "放送ID";
			this.放送ID.Name = "放送ID";
			this.放送ID.ReadOnly = true;
			// 
			// 形式
			// 
			this.形式.DataPropertyName = "afterConvertType";
			this.形式.HeaderText = "形式";
			this.形式.Name = "形式";
			this.形式.ReadOnly = true;
			this.形式.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// 画質
			// 
			this.画質.DataPropertyName = "quality";
			this.画質.HeaderText = "画質";
			this.画質.Name = "画質";
			this.画質.ReadOnly = true;
			// 
			// タイムシフト設定
			// 
			this.タイムシフト設定.DataPropertyName = "timeShift";
			this.タイムシフト設定.HeaderText = "タイムシフト設定";
			this.タイムシフト設定.MinimumWidth = 195;
			this.タイムシフト設定.Name = "タイムシフト設定";
			this.タイムシフト設定.ReadOnly = true;
			this.タイムシフト設定.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.タイムシフト設定.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.タイムシフト設定.Width = 195;
			// 
			// 状態
			// 
			this.状態.DataPropertyName = "state";
			this.状態.HeaderText = "状態";
			this.状態.Name = "状態";
			this.状態.ReadOnly = true;
			// 
			// タイトル
			// 
			this.タイトル.DataPropertyName = "title";
			this.タイトル.HeaderText = "タイトル";
			this.タイトル.Name = "タイトル";
			this.タイトル.ReadOnly = true;
			// 
			// 放送者
			// 
			this.放送者.DataPropertyName = "host";
			this.放送者.HeaderText = "放送者";
			this.放送者.Name = "放送者";
			this.放送者.ReadOnly = true;
			// 
			// コミュニティ名
			// 
			this.コミュニティ名.DataPropertyName = "communityName";
			this.コミュニティ名.HeaderText = "コミュニティ名";
			this.コミュニティ名.Name = "コミュニティ名";
			this.コミュニティ名.ReadOnly = true;
			// 
			// 開始時刻
			// 
			this.開始時刻.DataPropertyName = "startTime";
			this.開始時刻.HeaderText = "開始時刻";
			this.開始時刻.Name = "開始時刻";
			this.開始時刻.ReadOnly = true;
			// 
			// 終了時刻
			// 
			this.終了時刻.DataPropertyName = "endTime";
			this.終了時刻.HeaderText = "終了時刻";
			this.終了時刻.Name = "終了時刻";
			this.終了時刻.ReadOnly = true;
			// 
			// ログ
			// 
			this.ログ.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ログ.DataPropertyName = "log";
			this.ログ.HeaderText = "ログ";
			this.ログ.MinimumWidth = 60;
			this.ログ.Name = "ログ";
			this.ログ.ReadOnly = true;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.openWatchUrlMenu,
									this.openCommunityUrlMenu,
									this.copyWatchUrlMenu,
									this.copyCommunityUrlMenu,
									this.toolStripSeparator2,
									this.openRecFolderMenu,
									this.toolStripSeparator3,
									this.reAddRowMenu,
									this.toolStripSeparator4,
									this.deleteRowMenu});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(221, 176);
			// 
			// openWatchUrlMenu
			// 
			this.openWatchUrlMenu.Name = "openWatchUrlMenu";
			this.openWatchUrlMenu.Size = new System.Drawing.Size(220, 22);
			this.openWatchUrlMenu.Text = "放送ページへ移動";
			this.openWatchUrlMenu.Click += new System.EventHandler(this.openWatchUrlMenu_Click);
			// 
			// openCommunityUrlMenu
			// 
			this.openCommunityUrlMenu.Name = "openCommunityUrlMenu";
			this.openCommunityUrlMenu.Size = new System.Drawing.Size(220, 22);
			this.openCommunityUrlMenu.Text = "コミュニティページへ移動";
			this.openCommunityUrlMenu.Click += new System.EventHandler(this.openWatchUrlMenu_Click);
			// 
			// copyWatchUrlMenu
			// 
			this.copyWatchUrlMenu.Name = "copyWatchUrlMenu";
			this.copyWatchUrlMenu.Size = new System.Drawing.Size(220, 22);
			this.copyWatchUrlMenu.Text = "放送URLをコピー";
			this.copyWatchUrlMenu.Click += new System.EventHandler(this.copyWatchUrlMenu_Click);
			// 
			// copyCommunityUrlMenu
			// 
			this.copyCommunityUrlMenu.Name = "copyCommunityUrlMenu";
			this.copyCommunityUrlMenu.Size = new System.Drawing.Size(220, 22);
			this.copyCommunityUrlMenu.Text = "コミュニティURLをコピー";
			this.copyCommunityUrlMenu.Click += new System.EventHandler(this.copyCommunityUrlMenu_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(217, 6);
			// 
			// openRecFolderMenu
			// 
			this.openRecFolderMenu.Name = "openRecFolderMenu";
			this.openRecFolderMenu.Size = new System.Drawing.Size(220, 22);
			this.openRecFolderMenu.Text = "録画フォルダを開く";
			this.openRecFolderMenu.Click += new System.EventHandler(this.openRecFolderMenu_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(217, 6);
			// 
			// reAddRowMenu
			// 
			this.reAddRowMenu.Name = "reAddRowMenu";
			this.reAddRowMenu.Size = new System.Drawing.Size(220, 22);
			this.reAddRowMenu.Text = "この行を再登録する";
			this.reAddRowMenu.Click += new System.EventHandler(this.reAddRowMenu_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(217, 6);
			// 
			// deleteRowMenu
			// 
			this.deleteRowMenu.Name = "deleteRowMenu";
			this.deleteRowMenu.Size = new System.Drawing.Size(220, 22);
			this.deleteRowMenu.Text = "この行を削除する";
			this.deleteRowMenu.Click += new System.EventHandler(this.deleteRowMenu_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileMenuItem,
									this.toolMenuItem,
									this.helpMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(911, 26);
			this.menuStrip1.TabIndex = 12;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.録画フォルダを開くToolStripMenuItem,
									this.toolStripSeparator1,
									this.終了ToolStripMenuItem});
			this.fileMenuItem.Name = "fileMenuItem";
			this.fileMenuItem.ShowShortcutKeys = false;
			this.fileMenuItem.Size = new System.Drawing.Size(85, 22);
			this.fileMenuItem.Text = "ファイル(&F)";
			// 
			// 録画フォルダを開くToolStripMenuItem
			// 
			this.録画フォルダを開くToolStripMenuItem.Name = "録画フォルダを開くToolStripMenuItem";
			this.録画フォルダを開くToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
			this.録画フォルダを開くToolStripMenuItem.Text = "録画フォルダを開く(&O)";
			this.録画フォルダを開くToolStripMenuItem.Click += new System.EventHandler(this.openRecFolderMenu_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
			// 
			// 終了ToolStripMenuItem
			// 
			this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
			this.終了ToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
			this.終了ToolStripMenuItem.Text = "終了(&X)";
			this.終了ToolStripMenuItem.Click += new System.EventHandler(this.endMenu_Click);
			// 
			// toolMenuItem
			// 
			this.toolMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.optionMenuItem});
			this.toolMenuItem.Name = "toolMenuItem";
			this.toolMenuItem.ShowShortcutKeys = false;
			this.toolMenuItem.Size = new System.Drawing.Size(74, 22);
			this.toolMenuItem.Text = "ツール(&T)";
			// 
			// optionMenuItem
			// 
			this.optionMenuItem.Name = "optionMenuItem";
			this.optionMenuItem.ShowShortcutKeys = false;
			this.optionMenuItem.Size = new System.Drawing.Size(147, 22);
			this.optionMenuItem.Text = "オプション(&O)";
			this.optionMenuItem.Click += new System.EventHandler(this.optionItem_Select);
			// 
			// helpMenuItem
			// 
			this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.バージョン情報VToolStripMenuItem});
			this.helpMenuItem.Name = "helpMenuItem";
			this.helpMenuItem.ShowShortcutKeys = false;
			this.helpMenuItem.Size = new System.Drawing.Size(75, 22);
			this.helpMenuItem.Text = "ヘルプ(&H)";
			// 
			// バージョン情報VToolStripMenuItem
			// 
			this.バージョン情報VToolStripMenuItem.Name = "バージョン情報VToolStripMenuItem";
			this.バージョン情報VToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.バージョン情報VToolStripMenuItem.Text = "バージョン情報(&A)";
			this.バージョン情報VToolStripMenuItem.Click += new System.EventHandler(this.versionMenu_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoSize = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.label18);
			this.panel1.Controls.Add(this.label16);
			this.panel1.Controls.Add(this.label14);
			this.panel1.Controls.Add(this.setTimeshiftBtn);
			this.panel1.Controls.Add(this.afterConvertModeList);
			this.panel1.Controls.Add(this.qualityBtn);
			this.panel1.Controls.Add(this.addListBtn);
			this.panel1.Controls.Add(this.clearBtn);
			this.panel1.Controls.Add(this.recBtn);
			this.panel1.Controls.Add(this.urlText);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.recList);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(911, 301);
			this.panel1.TabIndex = 13;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(506, 40);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(38, 17);
			this.label18.TabIndex = 11;
			this.label18.Text = "形式：";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(12, 40);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(90, 17);
			this.label16.TabIndex = 11;
			this.label16.Text = "タイムシフト設定：";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(319, 40);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(42, 17);
			this.label14.TabIndex = 9;
			this.label14.Text = "画質：";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// setTimeshiftBtn
			// 
			this.setTimeshiftBtn.Location = new System.Drawing.Point(106, 37);
			this.setTimeshiftBtn.Name = "setTimeshiftBtn";
			this.setTimeshiftBtn.Size = new System.Drawing.Size(199, 23);
			this.setTimeshiftBtn.TabIndex = 3;
			this.setTimeshiftBtn.Text = "0時間0分0秒-0時間0分0秒";
			this.setTimeshiftBtn.UseVisualStyleBackColor = true;
			this.setTimeshiftBtn.Click += new System.EventHandler(this.setTimeshiftBtn_Click);
			// 
			// qualityBtn
			// 
			this.qualityBtn.Location = new System.Drawing.Point(367, 37);
			this.qualityBtn.Name = "qualityBtn";
			this.qualityBtn.Size = new System.Drawing.Size(119, 23);
			this.qualityBtn.TabIndex = 4;
			this.qualityBtn.Text = "超高,高,中,低,超低,自";
			this.qualityBtn.UseVisualStyleBackColor = true;
			this.qualityBtn.Click += new System.EventHandler(this.qualityBtn_Click);
			// 
			// addListBtn
			// 
			this.addListBtn.Location = new System.Drawing.Point(430, 10);
			this.addListBtn.Name = "addListBtn";
			this.addListBtn.Size = new System.Drawing.Size(97, 23);
			this.addListBtn.TabIndex = 1;
			this.addListBtn.Text = "録画リスト追加";
			this.addListBtn.UseVisualStyleBackColor = true;
			this.addListBtn.Click += new System.EventHandler(this.addListBtn_Click);
			// 
			// clearBtn
			// 
			this.clearBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.clearBtn.Location = new System.Drawing.Point(13, 264);
			this.clearBtn.Name = "clearBtn";
			this.clearBtn.Size = new System.Drawing.Size(95, 23);
			this.clearBtn.TabIndex = 7;
			this.clearBtn.Text = "リストのクリア";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
			// 
			// recBtn
			// 
			this.recBtn.Location = new System.Drawing.Point(567, 10);
			this.recBtn.Name = "recBtn";
			this.recBtn.Size = new System.Drawing.Size(75, 23);
			this.recBtn.TabIndex = 2;
			this.recBtn.Text = "録画開始";
			this.recBtn.UseVisualStyleBackColor = true;
			this.recBtn.Click += new System.EventHandler(this.recBtn_Click);
			// 
			// urlText
			// 
			this.urlText.Location = new System.Drawing.Point(75, 12);
			this.urlText.Name = "urlText";
			this.urlText.Size = new System.Drawing.Size(311, 19);
			this.urlText.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "放送URL：";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 26);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(911, 459);
			this.tableLayoutPanel1.TabIndex = 15;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.AutoSize = true;
			this.panel3.BackColor = System.Drawing.SystemColors.Control;
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel3.Controls.Add(this.keikaTimeLabel);
			this.panel3.Controls.Add(this.label7);
			this.panel3.Controls.Add(this.descriptLabel);
			this.panel3.Controls.Add(this.label19);
			this.panel3.Controls.Add(this.communityUrlLabel);
			this.panel3.Controls.Add(this.label15);
			this.panel3.Controls.Add(this.urlLabel);
			this.panel3.Controls.Add(this.label17);
			this.panel3.Controls.Add(this.communityLabel);
			this.panel3.Controls.Add(this.programTimeLabel);
			this.panel3.Controls.Add(this.hostLabel);
			this.panel3.Controls.Add(this.endTimeLabel);
			this.panel3.Controls.Add(this.label11);
			this.panel3.Controls.Add(this.label6);
			this.panel3.Controls.Add(this.label10);
			this.panel3.Controls.Add(this.label4);
			this.panel3.Controls.Add(this.titleLabel);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Controls.Add(this.startTimeLabel);
			this.panel3.Controls.Add(this.label3);
			this.panel3.Controls.Add(this.samuneBox);
			this.panel3.Controls.Add(this.logText);
			this.panel3.Location = new System.Drawing.Point(0, 301);
			this.panel3.Margin = new System.Windows.Forms.Padding(0);
			this.panel3.MinimumSize = new System.Drawing.Size(4, 158);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(911, 158);
			this.panel3.TabIndex = 15;
			// 
			// keikaTimeLabel
			// 
			this.keikaTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.keikaTimeLabel.Location = new System.Drawing.Point(158, 140);
			this.keikaTimeLabel.Name = "keikaTimeLabel";
			this.keikaTimeLabel.Size = new System.Drawing.Size(207, 14);
			this.keikaTimeLabel.TabIndex = 17;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(145, 126);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(207, 14);
			this.label7.TabIndex = 16;
			this.label7.Text = "経過時間";
			// 
			// descriptLabel
			// 
			this.descriptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.descriptLabel.Location = new System.Drawing.Point(431, 103);
			this.descriptLabel.Name = "descriptLabel";
			this.descriptLabel.Size = new System.Drawing.Size(272, 51);
			this.descriptLabel.TabIndex = 7;
			// 
			// label19
			// 
			this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label19.Location = new System.Drawing.Point(418, 89);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(99, 14);
			this.label19.TabIndex = 8;
			this.label19.Text = "説明";
			// 
			// communityUrlLabel
			// 
			this.communityUrlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.communityUrlLabel.Location = new System.Drawing.Point(431, 66);
			this.communityUrlLabel.Name = "communityUrlLabel";
			this.communityUrlLabel.Size = new System.Drawing.Size(272, 14);
			this.communityUrlLabel.TabIndex = 5;
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label15.Location = new System.Drawing.Point(418, 52);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(99, 14);
			this.label15.TabIndex = 6;
			this.label15.Text = "コミュニティURL";
			// 
			// urlLabel
			// 
			this.urlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.urlLabel.Location = new System.Drawing.Point(431, 29);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new System.Drawing.Size(272, 14);
			this.urlLabel.TabIndex = 3;
			// 
			// label17
			// 
			this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label17.Location = new System.Drawing.Point(418, 15);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(99, 14);
			this.label17.TabIndex = 4;
			this.label17.Text = "放送URL";
			// 
			// communityLabel
			// 
			this.communityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.communityLabel.Location = new System.Drawing.Point(286, 103);
			this.communityLabel.Name = "communityLabel";
			this.communityLabel.Size = new System.Drawing.Size(207, 14);
			this.communityLabel.TabIndex = 2;
			// 
			// programTimeLabel
			// 
			this.programTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.programTimeLabel.Location = new System.Drawing.Point(158, 103);
			this.programTimeLabel.Name = "programTimeLabel";
			this.programTimeLabel.Size = new System.Drawing.Size(207, 14);
			this.programTimeLabel.TabIndex = 2;
			// 
			// hostLabel
			// 
			this.hostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.hostLabel.Location = new System.Drawing.Point(286, 66);
			this.hostLabel.Name = "hostLabel";
			this.hostLabel.Size = new System.Drawing.Size(207, 14);
			this.hostLabel.TabIndex = 2;
			// 
			// endTimeLabel
			// 
			this.endTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.endTimeLabel.Location = new System.Drawing.Point(158, 66);
			this.endTimeLabel.Name = "endTimeLabel";
			this.endTimeLabel.Size = new System.Drawing.Size(207, 14);
			this.endTimeLabel.TabIndex = 2;
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label11.Location = new System.Drawing.Point(273, 89);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(207, 14);
			this.label11.TabIndex = 2;
			this.label11.Text = "コミュニティ";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(145, 89);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(207, 14);
			this.label6.TabIndex = 2;
			this.label6.Text = "放送時間";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(273, 52);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(207, 14);
			this.label10.TabIndex = 2;
			this.label10.Text = "放送者";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(145, 52);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(207, 14);
			this.label4.TabIndex = 2;
			this.label4.Text = "放送終了日時";
			// 
			// titleLabel
			// 
			this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.titleLabel.Location = new System.Drawing.Point(286, 29);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(207, 14);
			this.titleLabel.TabIndex = 2;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(273, 15);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(207, 14);
			this.label8.TabIndex = 2;
			this.label8.Text = "タイトル";
			// 
			// startTimeLabel
			// 
			this.startTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.startTimeLabel.Location = new System.Drawing.Point(158, 29);
			this.startTimeLabel.Name = "startTimeLabel";
			this.startTimeLabel.Size = new System.Drawing.Size(207, 14);
			this.startTimeLabel.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(145, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(207, 14);
			this.label3.TabIndex = 2;
			this.label3.Text = "放送開始日時";
			// 
			// samuneBox
			// 
			this.samuneBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.samuneBox.ErrorImage = null;
			this.samuneBox.Image = ((System.Drawing.Image)(resources.GetObject("samuneBox.Image")));
			this.samuneBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("samuneBox.InitialImage")));
			this.samuneBox.Location = new System.Drawing.Point(12, 15);
			this.samuneBox.Margin = new System.Windows.Forms.Padding(2);
			this.samuneBox.Name = "samuneBox";
			this.samuneBox.Size = new System.Drawing.Size(128, 128);
			this.samuneBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.samuneBox.TabIndex = 1;
			this.samuneBox.TabStop = false;
			// 
			// logText
			// 
			this.logText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.logText.Location = new System.Drawing.Point(720, 0);
			this.logText.Multiline = true;
			this.logText.Name = "logText";
			this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logText.Size = new System.Drawing.Size(187, 154);
			this.logText.TabIndex = 8;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(911, 485);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "MainForm";
			this.Text = "rokugaTouroku";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Close);
			this.Load += new System.EventHandler(this.form_Load);
			((System.ComponentModel.ISupportInitialize)(this.recList)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.samuneBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem deleteRowMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem reAddRowMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem openRecFolderMenu;
		private System.Windows.Forms.ToolStripMenuItem copyCommunityUrlMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem copyWatchUrlMenu;
		private System.Windows.Forms.ToolStripMenuItem openCommunityUrlMenu;
		private System.Windows.Forms.ToolStripMenuItem openWatchUrlMenu;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label keikaTimeLabel;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.DataGridViewTextBoxColumn 画質;
		public System.Windows.Forms.Button qualityBtn;
		public System.Windows.Forms.Button setTimeshiftBtn;
		private System.Windows.Forms.DataGridViewTextBoxColumn タイムシフト設定;
		public System.Windows.Forms.ComboBox afterConvertModeList;
		private System.Windows.Forms.DataGridViewTextBoxColumn 形式;
		public System.Windows.Forms.DataGridView recList;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label descriptLabel;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label urlLabel;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label communityUrlLabel;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label hostLabel;
		private System.Windows.Forms.Label communityLabel;
		private System.Windows.Forms.Label startTimeLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label endTimeLabel;
		private System.Windows.Forms.Label programTimeLabel;
		private System.Windows.Forms.TextBox logText;
		private System.Windows.Forms.PictureBox samuneBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button clearBtn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ログ;
		private System.Windows.Forms.DataGridViewTextBoxColumn 終了時刻;
		private System.Windows.Forms.DataGridViewTextBoxColumn 開始時刻;
		private System.Windows.Forms.DataGridViewTextBoxColumn コミュニティ名;
		private System.Windows.Forms.DataGridViewTextBoxColumn 放送者;
		private System.Windows.Forms.DataGridViewTextBoxColumn タイトル;
		private System.Windows.Forms.DataGridViewTextBoxColumn 状態;
		private System.Windows.Forms.DataGridViewTextBoxColumn 放送ID;
		public System.Windows.Forms.Button addListBtn;
		public System.Windows.Forms.Button recBtn;
		public System.Windows.Forms.TextBox urlText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem バージョン情報VToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
		public System.Windows.Forms.ToolStripMenuItem optionMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem 録画フォルダを開くToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;


		
		
	}
}
