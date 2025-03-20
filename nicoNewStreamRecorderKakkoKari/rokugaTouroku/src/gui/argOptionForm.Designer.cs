/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/07
 * Time: 16:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku.gui
{
	partial class ArgOptionForm
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
			this.fileNameTypeLabel = new System.Windows.Forms.TextBox();
			this.fileNameTypeText = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.fileNameTypeTitleBtn = new System.Windows.Forms.Button();
			this.fileNameTypeSimpleBtn = new System.Windows.Forms.Button();
			this.fileNameTypeDateBtn = new System.Windows.Forms.Button();
			this.fileNameTypeDefaultBtn = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.fileNameTypeOkBtn = new System.Windows.Forms.Button();
			this.copyBtn = new System.Windows.Forms.Button();
			this.label23 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.fileNameTypeLabel);
			this.groupBox1.Controls.Add(this.fileNameTypeText);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(364, 97);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "引数の書式テスト";
			// 
			// fileNameTypeLabel
			// 
			this.fileNameTypeLabel.Location = new System.Drawing.Point(6, 26);
			this.fileNameTypeLabel.Name = "fileNameTypeLabel";
			this.fileNameTypeLabel.ReadOnly = true;
			this.fileNameTypeLabel.Size = new System.Drawing.Size(320, 19);
			this.fileNameTypeLabel.TabIndex = 2;
			// 
			// fileNameTypeText
			// 
			this.fileNameTypeText.Location = new System.Drawing.Point(6, 60);
			this.fileNameTypeText.Name = "fileNameTypeText";
			this.fileNameTypeText.Size = new System.Drawing.Size(320, 19);
			this.fileNameTypeText.TabIndex = 1;
			this.fileNameTypeText.TextChanged += new System.EventHandler(this.fileNameTypeText_Changed);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.fileNameTypeTitleBtn);
			this.groupBox2.Controls.Add(this.fileNameTypeSimpleBtn);
			this.groupBox2.Controls.Add(this.fileNameTypeDateBtn);
			this.groupBox2.Controls.Add(this.fileNameTypeDefaultBtn);
			this.groupBox2.Location = new System.Drawing.Point(12, 115);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(364, 56);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "テンプレート";
			// 
			// fileNameTypeTitleBtn
			// 
			this.fileNameTypeTitleBtn.Location = new System.Drawing.Point(246, 25);
			this.fileNameTypeTitleBtn.Name = "fileNameTypeTitleBtn";
			this.fileNameTypeTitleBtn.Size = new System.Drawing.Size(74, 23);
			this.fileNameTypeTitleBtn.TabIndex = 2;
			this.fileNameTypeTitleBtn.Text = "タイトル";
			this.fileNameTypeTitleBtn.UseVisualStyleBackColor = true;
			this.fileNameTypeTitleBtn.Visible = false;
			// 
			// fileNameTypeSimpleBtn
			// 
			this.fileNameTypeSimpleBtn.Location = new System.Drawing.Point(166, 25);
			this.fileNameTypeSimpleBtn.Name = "fileNameTypeSimpleBtn";
			this.fileNameTypeSimpleBtn.Size = new System.Drawing.Size(74, 23);
			this.fileNameTypeSimpleBtn.TabIndex = 2;
			this.fileNameTypeSimpleBtn.Text = "シンプル";
			this.fileNameTypeSimpleBtn.UseVisualStyleBackColor = true;
			this.fileNameTypeSimpleBtn.Visible = false;
			// 
			// fileNameTypeDateBtn
			// 
			this.fileNameTypeDateBtn.Location = new System.Drawing.Point(86, 25);
			this.fileNameTypeDateBtn.Name = "fileNameTypeDateBtn";
			this.fileNameTypeDateBtn.Size = new System.Drawing.Size(74, 23);
			this.fileNameTypeDateBtn.TabIndex = 2;
			this.fileNameTypeDateBtn.Text = "引数なし";
			this.fileNameTypeDateBtn.UseVisualStyleBackColor = true;
			this.fileNameTypeDateBtn.Visible = false;
			this.fileNameTypeDateBtn.Click += new System.EventHandler(this.FileNameTypeDateBtnClick);
			// 
			// fileNameTypeDefaultBtn
			// 
			this.fileNameTypeDefaultBtn.Location = new System.Drawing.Point(6, 25);
			this.fileNameTypeDefaultBtn.Name = "fileNameTypeDefaultBtn";
			this.fileNameTypeDefaultBtn.Size = new System.Drawing.Size(74, 23);
			this.fileNameTypeDefaultBtn.TabIndex = 2;
			this.fileNameTypeDefaultBtn.Text = "サンプル";
			this.fileNameTypeDefaultBtn.UseVisualStyleBackColor = true;
			this.fileNameTypeDefaultBtn.Click += new System.EventHandler(this.FileNameTypeDefaultBtnClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.Controls.Add(this.label16);
			this.groupBox3.Controls.Add(this.label22);
			this.groupBox3.Controls.Add(this.label24);
			this.groupBox3.Controls.Add(this.label23);
			this.groupBox3.Controls.Add(this.label21);
			this.groupBox3.Controls.Add(this.label20);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.label19);
			this.groupBox3.Controls.Add(this.label18);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.label17);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(12, 177);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(363, 230);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "変換記号";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(199, 134);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(92, 18);
			this.label15.TabIndex = 1;
			this.label15.Text = "{5} 配信者ID";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(17, 206);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(316, 18);
			this.label16.TabIndex = 0;
			this.label16.Text = "引数が空欄の場合、セグメントm3u8アドレスのみが設定されます。";
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(199, 170);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(155, 18);
			this.label22.TabIndex = 0;
			this.label22.Text = "{tsEnd} (TS設定の終了時間)";
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(17, 170);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(176, 18);
			this.label21.TabIndex = 0;
			this.label21.Text = "{tsStart} (TS設定の開始時間)";
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(17, 152);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(143, 18);
			this.label20.TabIndex = 0;
			this.label20.Text = "{us} (user_sessionクッキー)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 134);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "{url} (放送URL)";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(17, 116);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(92, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "{W} (曜日)";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(17, 98);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(92, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "{D} (日)";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(17, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(92, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "{M} (月)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(17, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "{y} (2桁の西暦)";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(115, 80);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(78, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "{s} (秒)";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(115, 62);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(78, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "{m} (分)";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(199, 116);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(92, 18);
			this.label14.TabIndex = 0;
			this.label14.Text = "{4} チャンネル名";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(199, 98);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(92, 18);
			this.label13.TabIndex = 0;
			this.label13.Text = "{3} チャンネルID";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(199, 80);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(92, 18);
			this.label12.TabIndex = 0;
			this.label12.Text = "{2} 配信者名";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(199, 62);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(92, 18);
			this.label11.TabIndex = 0;
			this.label11.Text = "{1} タイトル";
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(199, 26);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(104, 18);
			this.label19.TabIndex = 0;
			this.label19.Text = "{o} 保存ファイルパス";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(199, 26);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(92, 18);
			this.label18.TabIndex = 0;
			this.label18.Text = "{0} lv数字";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(199, 44);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(92, 18);
			this.label10.TabIndex = 0;
			this.label10.Text = "{0} lv数字";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(115, 44);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 18);
			this.label7.TabIndex = 0;
			this.label7.Text = "{h} (時)";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(17, 26);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(143, 18);
			this.label17.TabIndex = 0;
			this.label17.Text = "{i} (セグメントm3u8アドレス)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(17, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "{Y} (4桁の西暦)";
			// 
			// fileNameTypeOkBtn
			// 
			this.fileNameTypeOkBtn.Location = new System.Drawing.Point(301, 416);
			this.fileNameTypeOkBtn.Name = "fileNameTypeOkBtn";
			this.fileNameTypeOkBtn.Size = new System.Drawing.Size(74, 23);
			this.fileNameTypeOkBtn.TabIndex = 2;
			this.fileNameTypeOkBtn.Text = "OK";
			this.fileNameTypeOkBtn.UseVisualStyleBackColor = true;
			this.fileNameTypeOkBtn.Click += new System.EventHandler(this.fileNameTypeOkBtn_Click);
			// 
			// copyBtn
			// 
			this.copyBtn.Location = new System.Drawing.Point(174, 416);
			this.copyBtn.Name = "copyBtn";
			this.copyBtn.Size = new System.Drawing.Size(111, 23);
			this.copyBtn.TabIndex = 2;
			this.copyBtn.Text = "クリップボードにコピー";
			this.copyBtn.UseVisualStyleBackColor = true;
			this.copyBtn.Click += new System.EventHandler(this.CopyBtnClick);
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(17, 188);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(176, 18);
			this.label23.TabIndex = 0;
			this.label23.Text = "{proxyAddress} (プロキシアドレス)";
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(199, 188);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(143, 18);
			this.label24.TabIndex = 0;
			this.label24.Text = "{proxyPort} (プロキシポート)";
			// 
			// ArgOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(387, 451);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.copyBtn);
			this.Controls.Add(this.fileNameTypeOkBtn);
			this.Name = "ArgOptionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "引数の書式";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox fileNameTypeLabel;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button copyBtn;
		private System.Windows.Forms.Button fileNameTypeOkBtn;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button fileNameTypeDateBtn;
		private System.Windows.Forms.Button fileNameTypeSimpleBtn;
		private System.Windows.Forms.Button fileNameTypeTitleBtn;
		private System.Windows.Forms.Button fileNameTypeDefaultBtn;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox fileNameTypeText;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
