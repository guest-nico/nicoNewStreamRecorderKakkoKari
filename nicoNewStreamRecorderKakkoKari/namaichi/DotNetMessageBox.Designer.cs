/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/16
 * Time: 13:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace namaichi
{
	partial class DotNetMessageBox
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.label2);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(365, 82);
			this.panel1.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label2.Location = new System.Drawing.Point(12, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(330, 21);
			this.label2.TabIndex = 2;
			this.label2.Text = "動作には.NET 4.5.2以上が推奨です。";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(254, 102);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(88, 26);
			this.button3.TabIndex = 3;
			this.button3.Text = "OK";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.Button3Click);
			// 
			// DotNetMessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 140);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "DotNetMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
	}
}
