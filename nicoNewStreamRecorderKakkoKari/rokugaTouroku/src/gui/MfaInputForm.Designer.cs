/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2023/05/19
 * Time: 7:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku.gui
{
	partial class MfaInputForm
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
			this.sendToLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.codeText = new System.Windows.Forms.TextBox();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// sendToLabel
			// 
			this.sendToLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.sendToLabel.Location = new System.Drawing.Point(12, 39);
			this.sendToLabel.Name = "sendToLabel";
			this.sendToLabel.Size = new System.Drawing.Size(308, 19);
			this.sendToLabel.TabIndex = 0;
			this.sendToLabel.Text = " に確認コードを送信しました";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(182, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "メールに記載された6桁の数字を入力";
			// 
			// codeText
			// 
			this.codeText.Location = new System.Drawing.Point(12, 80);
			this.codeText.Name = "codeText";
			this.codeText.Size = new System.Drawing.Size(85, 19);
			this.codeText.TabIndex = 2;
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(232, 110);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(74, 23);
			this.cancelBtn.TabIndex = 4;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(152, 110);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(74, 23);
			this.okBtn.TabIndex = 3;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.OkBtnClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(257, 19);
			this.label1.TabIndex = 1;
			this.label1.Text = "ログインするには2段階認証の確認コードが必要です";
			// 
			// MfaInputForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(319, 145);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.codeText);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.sendToLabel);
			this.Name = "MfaInputForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "2段階認証";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.TextBox codeText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label sendToLabel;
	}
}
