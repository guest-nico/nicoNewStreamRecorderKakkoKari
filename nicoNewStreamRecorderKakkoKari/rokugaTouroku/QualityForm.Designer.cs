/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/10/04
 * Time: 4:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku
{
	partial class QualityForm
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
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.downBtn = new System.Windows.Forms.Button();
			this.upBtn = new System.Windows.Forms.Button();
			this.qualityListBox = new System.Windows.Forms.ListBox();
			this.lowRankBtn = new System.Windows.Forms.Button();
			this.highRankBtn = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.groupBox7.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.downBtn);
			this.groupBox7.Controls.Add(this.upBtn);
			this.groupBox7.Controls.Add(this.qualityListBox);
			this.groupBox7.Controls.Add(this.lowRankBtn);
			this.groupBox7.Controls.Add(this.highRankBtn);
			this.groupBox7.Location = new System.Drawing.Point(5, 10);
			this.groupBox7.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox7.Size = new System.Drawing.Size(248, 193);
			this.groupBox7.TabIndex = 4;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "録画画質優先順位";
			// 
			// downBtn
			// 
			this.downBtn.Location = new System.Drawing.Point(184, 63);
			this.downBtn.Name = "downBtn";
			this.downBtn.Size = new System.Drawing.Size(42, 30);
			this.downBtn.TabIndex = 4;
			this.downBtn.Text = "下へ";
			this.downBtn.UseVisualStyleBackColor = true;
			this.downBtn.Click += new System.EventHandler(this.DownBtn_Click);
			// 
			// upBtn
			// 
			this.upBtn.Location = new System.Drawing.Point(184, 27);
			this.upBtn.Name = "upBtn";
			this.upBtn.Size = new System.Drawing.Size(42, 30);
			this.upBtn.TabIndex = 4;
			this.upBtn.Text = "上へ";
			this.upBtn.UseVisualStyleBackColor = true;
			this.upBtn.Click += new System.EventHandler(this.UpBtn_Click);
			// 
			// qualityListBox
			// 
			this.qualityListBox.FormattingEnabled = true;
			this.qualityListBox.ItemHeight = 12;
			this.qualityListBox.Items.AddRange(new object[] {
									"1. 自動(abr)",
									"2. 3Mbps(super_high)",
									"3. 2Mbps(high・高画質)",
									"4. 1Mbps(normal・低画質)",
									"5. 384kbps(low)",
									"6. 192kbps(super_low)"});
			this.qualityListBox.Location = new System.Drawing.Point(26, 27);
			this.qualityListBox.Name = "qualityListBox";
			this.qualityListBox.Size = new System.Drawing.Size(143, 112);
			this.qualityListBox.TabIndex = 3;
			// 
			// lowRankBtn
			// 
			this.lowRankBtn.Location = new System.Drawing.Point(129, 156);
			this.lowRankBtn.Name = "lowRankBtn";
			this.lowRankBtn.Size = new System.Drawing.Size(86, 23);
			this.lowRankBtn.TabIndex = 1;
			this.lowRankBtn.Text = "なるべく低画質";
			this.lowRankBtn.UseVisualStyleBackColor = true;
			this.lowRankBtn.Click += new System.EventHandler(this.lowRankBtn_Click);
			// 
			// highRankBtn
			// 
			this.highRankBtn.Location = new System.Drawing.Point(26, 156);
			this.highRankBtn.Name = "highRankBtn";
			this.highRankBtn.Size = new System.Drawing.Size(86, 23);
			this.highRankBtn.TabIndex = 1;
			this.highRankBtn.Text = "なるべく高画質";
			this.highRankBtn.UseVisualStyleBackColor = true;
			this.highRankBtn.Click += new System.EventHandler(this.highRankBtn_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(164, 219);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(74, 23);
			this.button4.TabIndex = 6;
			this.button4.Text = "キャンセル";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(84, 219);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(74, 23);
			this.button3.TabIndex = 5;
			this.button3.Text = "OK";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// QualityForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(258, 251);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.groupBox7);
			this.Name = "QualityForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "画質設定";
			this.groupBox7.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button highRankBtn;
		private System.Windows.Forms.Button lowRankBtn;
		private System.Windows.Forms.ListBox qualityListBox;
		private System.Windows.Forms.Button upBtn;
		private System.Windows.Forms.Button downBtn;
		private System.Windows.Forms.GroupBox groupBox7;
	}
}
