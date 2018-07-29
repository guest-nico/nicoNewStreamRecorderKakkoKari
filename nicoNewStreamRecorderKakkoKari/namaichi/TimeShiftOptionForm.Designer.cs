/*
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
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Window;
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
			this.groupBox1.Size = new System.Drawing.Size(299, 153);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "オプション";
			// 
			// isRenketuLastFile
			// 
			this.isRenketuLastFile.Location = new System.Drawing.Point(26, 113);
			this.isRenketuLastFile.Name = "isRenketuLastFile";
			this.isRenketuLastFile.Size = new System.Drawing.Size(231, 17);
			this.isRenketuLastFile.TabIndex = 4;
			this.isRenketuLastFile.Text = "前回のファイルと新しい録画を連結する";
			this.isRenketuLastFile.UseVisualStyleBackColor = true;
			// 
			// lastFileInfoLabel
			// 
			this.lastFileInfoLabel.Location = new System.Drawing.Point(25, 96);
			this.lastFileInfoLabel.Name = "lastFileInfoLabel";
			this.lastFileInfoLabel.Size = new System.Drawing.Size(250, 17);
			this.lastFileInfoLabel.TabIndex = 3;
			this.lastFileInfoLabel.Text = "(?時間?分?秒まで録画済み)";
			// 
			// sLabel
			// 
			this.sLabel.Location = new System.Drawing.Point(188, 44);
			this.sLabel.Name = "sLabel";
			this.sLabel.Size = new System.Drawing.Size(69, 12);
			this.sLabel.TabIndex = 2;
			this.sLabel.Text = "秒時点から";
			// 
			// sText
			// 
			this.sText.Location = new System.Drawing.Point(153, 41);
			this.sText.Name = "sText";
			this.sText.Size = new System.Drawing.Size(29, 19);
			this.sText.TabIndex = 1;
			this.sText.Text = "0";
			this.sText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// mLabel
			// 
			this.mLabel.Location = new System.Drawing.Point(130, 44);
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size(39, 12);
			this.mLabel.TabIndex = 2;
			this.mLabel.Text = "分";
			// 
			// mText
			// 
			this.mText.Location = new System.Drawing.Point(95, 41);
			this.mText.Name = "mText";
			this.mText.Size = new System.Drawing.Size(29, 19);
			this.mText.TabIndex = 1;
			this.mText.Text = "0";
			this.mText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// hLabel
			// 
			this.hLabel.Location = new System.Drawing.Point(60, 44);
			this.hLabel.Name = "hLabel";
			this.hLabel.Size = new System.Drawing.Size(39, 12);
			this.hLabel.TabIndex = 2;
			this.hLabel.Text = "時間";
			// 
			// hText
			// 
			this.hText.Location = new System.Drawing.Point(25, 41);
			this.hText.Name = "hText";
			this.hText.Size = new System.Drawing.Size(29, 19);
			this.hText.TabIndex = 1;
			this.hText.Text = "0";
			this.hText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// isFromLastTimeRadioBtn
			// 
			this.isFromLastTimeRadioBtn.Location = new System.Drawing.Point(6, 75);
			this.isFromLastTimeRadioBtn.Name = "isFromLastTimeRadioBtn";
			this.isFromLastTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isFromLastTimeRadioBtn.TabIndex = 0;
			this.isFromLastTimeRadioBtn.Text = "前回の続きから録画";
			this.isFromLastTimeRadioBtn.UseVisualStyleBackColor = true;
			// 
			// isStartTimeRadioBtn
			// 
			this.isStartTimeRadioBtn.Checked = true;
			this.isStartTimeRadioBtn.Location = new System.Drawing.Point(6, 18);
			this.isStartTimeRadioBtn.Name = "isStartTimeRadioBtn";
			this.isStartTimeRadioBtn.Size = new System.Drawing.Size(182, 18);
			this.isStartTimeRadioBtn.TabIndex = 0;
			this.isStartTimeRadioBtn.TabStop = true;
			this.isStartTimeRadioBtn.Text = "録画開始時間を指定";
			this.isStartTimeRadioBtn.UseVisualStyleBackColor = true;
			this.isStartTimeRadioBtn.CheckedChanged += new System.EventHandler(this.isStartTimeRadioBtn_CheckedChanged);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(215, 169);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(74, 23);
			this.cancelBtn.TabIndex = 3;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(135, 169);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(74, 23);
			this.okBtn.TabIndex = 2;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// TimeShiftOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(311, 200);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.groupBox1);
			this.Name = "TimeShiftOptionForm";
			this.Text = "タイムシフト録画設定";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
		}
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
