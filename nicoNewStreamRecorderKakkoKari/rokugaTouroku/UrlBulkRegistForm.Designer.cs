/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/21
 * Time: 4:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku
{
	partial class UrlBulkRegistForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.registText = new System.Windows.Forms.TextBox();
			this.registBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(218, 52);
			this.label1.TabIndex = 0;
<<<<<<< HEAD
			this.label1.Text = "(入力例)\r\nhttps://live.nicovideo.jp/watch/XXXXX\r\nhttps://live.nicovideo.jp/watch/YYYYY" +
			"\r\nhttps://live.nicovideo.jp/watch/AAAA\r\n";
=======
			this.label1.Text = "(入力例)\r\nhttp://live.nicovideo.jp/watch/XXXXX\r\nhttp://live.nicovideo.jp/watch/YYYYY" +
			"\r\nhttp://live.nicovideo.jp/watch/AAAA\r\n";
>>>>>>> da2ceb1dec9975a74d9e4b0e4bfbb48a1dad3721
			// 
			// registText
			// 
			this.registText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.registText.Location = new System.Drawing.Point(13, 63);
			this.registText.Multiline = true;
			this.registText.Name = "registText";
			this.registText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.registText.Size = new System.Drawing.Size(330, 205);
			this.registText.TabIndex = 1;
			// 
			// registBtn
			// 
			this.registBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.registBtn.Location = new System.Drawing.Point(197, 276);
			this.registBtn.Name = "registBtn";
			this.registBtn.Size = new System.Drawing.Size(66, 23);
			this.registBtn.TabIndex = 2;
			this.registBtn.Text = "登録";
			this.registBtn.UseVisualStyleBackColor = true;
			this.registBtn.Click += new System.EventHandler(this.RegistBtnClick);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelBtn.Location = new System.Drawing.Point(278, 276);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(66, 23);
			this.cancelBtn.TabIndex = 3;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
			// 
			// UrlBulkRegistForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 309);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.registBtn);
			this.Controls.Add(this.registText);
			this.Controls.Add(this.label1);
			this.Name = "UrlBulkRegistForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "URL一括登録";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button registBtn;
		private System.Windows.Forms.TextBox registText;
		private System.Windows.Forms.Label label1;
	}
}
