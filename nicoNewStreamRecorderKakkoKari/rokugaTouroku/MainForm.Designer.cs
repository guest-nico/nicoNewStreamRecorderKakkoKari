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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.ComboBox comboBox1;
			this.recList = new System.Windows.Forms.DataGridView();
			this.放送ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.形式 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.状態 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.タイトル = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.放送者 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.コミュニティ名 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.開始時刻 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.終了時刻 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ログ = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
			this.addListBtn = new System.Windows.Forms.Button();
			this.clearBtn = new System.Windows.Forms.Button();
			this.recBtn = new System.Windows.Forms.Button();
			this.urlText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
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
			comboBox1 = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.recList)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.samuneBox)).BeginInit();
			this.SuspendLayout();
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
									this.状態,
									this.タイトル,
									this.放送者,
									this.コミュニティ名,
									this.開始時刻,
									this.終了時刻,
									this.ログ});
			this.recList.Location = new System.Drawing.Point(14, 42);
			this.recList.Name = "recList";
			this.recList.ReadOnly = true;
			this.recList.RowHeadersVisible = false;
			this.recList.RowTemplate.Height = 21;
			this.recList.Size = new System.Drawing.Size(919, 213);
			this.recList.TabIndex = 0;
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
			this.形式.DataPropertyName = "afterFFmpegMode";
			this.形式.HeaderText = "形式";
			this.形式.Name = "形式";
			this.形式.ReadOnly = true;
			this.形式.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileMenuItem,
									this.toolMenuItem,
									this.helpMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(954, 26);
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
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoSize = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(comboBox1);
			this.panel1.Controls.Add(this.addListBtn);
			this.panel1.Controls.Add(this.clearBtn);
			this.panel1.Controls.Add(this.recBtn);
			this.panel1.Controls.Add(this.urlText);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.recList);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(954, 301);
			this.panel1.TabIndex = 13;
			// 
			// addListBtn
			// 
			this.addListBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addListBtn.Location = new System.Drawing.Point(721, 10);
			this.addListBtn.Name = "addListBtn";
			this.addListBtn.Size = new System.Drawing.Size(97, 23);
			this.addListBtn.TabIndex = 3;
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
			this.clearBtn.TabIndex = 3;
			this.clearBtn.Text = "リストのクリア";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
			// 
			// recBtn
			// 
			this.recBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.recBtn.Location = new System.Drawing.Point(858, 10);
			this.recBtn.Name = "recBtn";
			this.recBtn.Size = new System.Drawing.Size(75, 23);
			this.recBtn.TabIndex = 3;
			this.recBtn.Text = "録画開始";
			this.recBtn.UseVisualStyleBackColor = true;
			this.recBtn.Click += new System.EventHandler(this.recBtn_Click);
			// 
			// urlText
			// 
			this.urlText.Location = new System.Drawing.Point(75, 12);
			this.urlText.Name = "urlText";
			this.urlText.Size = new System.Drawing.Size(314, 19);
			this.urlText.TabIndex = 2;
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(954, 459);
			this.tableLayoutPanel1.TabIndex = 15;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.AutoSize = true;
			this.panel3.BackColor = System.Drawing.SystemColors.Control;
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel3.Controls.Add(this.label12);
			this.panel3.Controls.Add(this.label13);
			this.panel3.Controls.Add(this.label9);
			this.panel3.Controls.Add(this.label7);
			this.panel3.Controls.Add(this.label2);
			this.panel3.Controls.Add(this.label5);
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
			this.panel3.Size = new System.Drawing.Size(954, 158);
			this.panel3.TabIndex = 15;
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label12.Location = new System.Drawing.Point(776, 29);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(174, 14);
			this.label12.TabIndex = 12;
			this.label12.Text = "sHigh,High,Normal,Low,sLow";
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label13.Location = new System.Drawing.Point(763, 15);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(99, 14);
			this.label13.TabIndex = 13;
			this.label13.Text = "画質設定";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(776, 94);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(331, 12);
			this.label9.TabIndex = 11;
			this.label9.Text = "コマンド：　ffmpeg -protocol_whitelist \"https,file,tls,tcp\" -i {i} {i}.ts";
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(776, 80);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(127, 14);
			this.label7.TabIndex = 11;
			this.label7.Text = "URLリスト：　M3U8形式で出力";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(776, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(164, 14);
			this.label2.TabIndex = 9;
			this.label2.Text = "0時間0分0秒時点から";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(763, 52);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 14);
			this.label5.TabIndex = 10;
			this.label5.Text = "タイムシフト設定";
			// 
			// descriptLabel
			// 
			this.descriptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.descriptLabel.Location = new System.Drawing.Point(474, 103);
			this.descriptLabel.Name = "descriptLabel";
			this.descriptLabel.Size = new System.Drawing.Size(272, 51);
			this.descriptLabel.TabIndex = 7;
			this.descriptLabel.Text = "barのような雰囲気で 酔っ払いのようにぐでんぐでんに配信していこう。 仕事、趣味のスマホアプリを開発をしたり \r\n１\r\n２\r\n３";
			// 
			// label19
			// 
			this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label19.Location = new System.Drawing.Point(461, 89);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(99, 14);
			this.label19.TabIndex = 8;
			this.label19.Text = "説明";
			// 
			// communityUrlLabel
			// 
			this.communityUrlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.communityUrlLabel.Location = new System.Drawing.Point(474, 66);
			this.communityUrlLabel.Name = "communityUrlLabel";
			this.communityUrlLabel.Size = new System.Drawing.Size(272, 14);
			this.communityUrlLabel.TabIndex = 5;
			this.communityUrlLabel.Text = "https://cas.nicovideo.jp/user/1970734/lv315770204";
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label15.Location = new System.Drawing.Point(461, 52);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(99, 14);
			this.label15.TabIndex = 6;
			this.label15.Text = "コミュニティURL";
			// 
			// urlLabel
			// 
			this.urlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.urlLabel.Location = new System.Drawing.Point(474, 29);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new System.Drawing.Size(272, 14);
			this.urlLabel.TabIndex = 3;
			this.urlLabel.Text = "http://live2.nicovideo.jp/watch/lv315767216";
			// 
			// label17
			// 
			this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label17.Location = new System.Drawing.Point(461, 15);
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
			this.communityLabel.Size = new System.Drawing.Size(165, 14);
			this.communityLabel.TabIndex = 2;
			this.communityLabel.Text = "私立CIJ学園　げーむ部";
			// 
			// programTimeLabel
			// 
			this.programTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.programTimeLabel.Location = new System.Drawing.Point(172, 103);
			this.programTimeLabel.Name = "programTimeLabel";
			this.programTimeLabel.Size = new System.Drawing.Size(103, 14);
			this.programTimeLabel.TabIndex = 2;
			this.programTimeLabel.Text = "3時間33分20秒";
			// 
			// hostLabel
			// 
			this.hostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.hostLabel.Location = new System.Drawing.Point(286, 66);
			this.hostLabel.Name = "hostLabel";
			this.hostLabel.Size = new System.Drawing.Size(165, 14);
			this.hostLabel.TabIndex = 2;
			this.hostLabel.Text = "有限会社フレンド";
			// 
			// endTimeLabel
			// 
			this.endTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.endTimeLabel.Location = new System.Drawing.Point(158, 66);
			this.endTimeLabel.Name = "endTimeLabel";
			this.endTimeLabel.Size = new System.Drawing.Size(103, 14);
			this.endTimeLabel.TabIndex = 2;
			this.endTimeLabel.Text = "09/19(水) 23:00:00";
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label11.Location = new System.Drawing.Point(273, 89);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(99, 14);
			this.label11.TabIndex = 2;
			this.label11.Text = "コミュニティ";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(145, 89);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(99, 14);
			this.label6.TabIndex = 2;
			this.label6.Text = "放送時間";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(273, 52);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(99, 14);
			this.label10.TabIndex = 2;
			this.label10.Text = "放送者";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(145, 52);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 14);
			this.label4.TabIndex = 2;
			this.label4.Text = "放送開始日時";
			// 
			// titleLabel
			// 
			this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.titleLabel.Location = new System.Drawing.Point(286, 29);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(165, 14);
			this.titleLabel.TabIndex = 2;
			this.titleLabel.Text = "【作業枠/おしごと編】ぼっちマンのちょろっとプログラミング枠【顔出し】ああああああああ";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(273, 15);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(99, 14);
			this.label8.TabIndex = 2;
			this.label8.Text = "タイトル";
			// 
			// startTimeLabel
			// 
			this.startTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.startTimeLabel.Location = new System.Drawing.Point(158, 29);
			this.startTimeLabel.Name = "startTimeLabel";
			this.startTimeLabel.Size = new System.Drawing.Size(103, 14);
			this.startTimeLabel.TabIndex = 2;
			this.startTimeLabel.Text = "09/19(水) 23:00:00";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(145, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 14);
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
			this.logText.Location = new System.Drawing.Point(763, 112);
			this.logText.Multiline = true;
			this.logText.Name = "logText";
			this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logText.Size = new System.Drawing.Size(187, 42);
			this.logText.TabIndex = 0;
			this.logText.Text = "1\r\n2\r\n3\r\n4";
			// 
			// comboBox1
			// 
			comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBox1.FormattingEnabled = true;
			comboBox1.Items.AddRange(new object[] {
									"flv(変換無し)",
									"avi",
									"mp4",
									"mov",
									"wmv",
									"vob",
									"mkv",
									"ts",
									"mp3(音声)",
									"wav(音声)",
									"wma(音声)",
									"aac(音声)",
									"ogg(音声)"});
			comboBox1.Location = new System.Drawing.Point(613, 11);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new System.Drawing.Size(90, 20);
			comboBox1.TabIndex = 5;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(954, 485);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "MainForm";
			this.Text = "rokugaTouroku";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Close);
			((System.ComponentModel.ISupportInitialize)(this.recList)).EndInit();
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
		private System.Windows.Forms.DataGridViewTextBoxColumn 形式;
		public System.Windows.Forms.DataGridView recList;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
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
