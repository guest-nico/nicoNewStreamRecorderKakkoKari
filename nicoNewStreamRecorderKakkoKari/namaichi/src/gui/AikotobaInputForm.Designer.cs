/*
 * Created by SharpDevelop.
 * User: ajkkh
 * Date: 2024/11/19
 * Time: 8:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace namaichi.gui
{
	partial class AikotobaInputForm
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
			this.passText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.authBtn = new System.Windows.Forms.Button();
			this.msgText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// passText
			// 
			this.passText.Location = new System.Drawing.Point(12, 57);
			this.passText.Name = "passText";
			this.passText.Size = new System.Drawing.Size(282, 19);
			this.passText.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(305, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "放送者から共有された合い言葉を入力してください";
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(138, 123);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(75, 23);
			this.cancelBtn.TabIndex = 2;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(319, 27);
			this.label2.TabIndex = 1;
			this.label2.Text = "※合い言葉は半角英数のみで最大16文字です\r\n※空白を入れず、大文字・小文字に注意して入力してください";
			// 
			// authBtn
			// 
			this.authBtn.Location = new System.Drawing.Point(219, 123);
			this.authBtn.Name = "authBtn";
			this.authBtn.Size = new System.Drawing.Size(75, 23);
			this.authBtn.TabIndex = 2;
			this.authBtn.Text = "認証する";
			this.authBtn.UseVisualStyleBackColor = true;
			this.authBtn.Click += new System.EventHandler(this.AuthBtnClick);
			// 
			// msgText
			// 
			this.msgText.ForeColor = System.Drawing.Color.Red;
			this.msgText.Location = new System.Drawing.Point(16, 37);
			this.msgText.Name = "msgText";
			this.msgText.Size = new System.Drawing.Size(338, 17);
			this.msgText.TabIndex = 3;
			// 
			// AikotobaInputForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 170);
			this.Controls.Add(this.msgText);
			this.Controls.Add(this.authBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.passText);
			this.Name = "AikotobaInputForm";
			this.Text = "合言葉を入力する";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label msgText;
		private System.Windows.Forms.Button authBtn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox passText;
	}
}
