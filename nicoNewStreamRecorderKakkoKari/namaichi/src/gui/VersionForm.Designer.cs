/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/22
 * Time: 4:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace namaichi
{
	partial class VersionForm
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
			this.label3 = new System.Windows.Forms.Label();
			this.versionLabel = new System.Windows.Forms.Label();
			this.panel5 = new System.Windows.Forms.Panel();
			this.lastVersionLabel = new System.Windows.Forms.LinkLabel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.panel4 = new System.Windows.Forms.Panel();
			this.communityLinkLabel = new System.Windows.Forms.LinkLabel();
			this.downloadPageLinkLabel = new System.Windows.Forms.LinkLabel();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(318, 13);
			this.panel1.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Top;
			this.label3.Location = new System.Drawing.Point(0, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(318, 24);
			this.label3.TabIndex = 4;
			this.label3.Text = "ニコ生新配信録画ツール （仮";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// versionLabel
			// 
			this.versionLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.versionLabel.Location = new System.Drawing.Point(0, 37);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(318, 19);
			this.versionLabel.TabIndex = 5;
			this.versionLabel.Text = "ver 0.86.15 (2018/09/22)";
			this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel5
			// 
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(0, 56);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(318, 5);
			this.panel5.TabIndex = 33;
			// 
			// lastVersionLabel
			// 
			this.lastVersionLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.lastVersionLabel.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lastVersionLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.lastVersionLabel.Location = new System.Drawing.Point(0, 61);
			this.lastVersionLabel.Name = "lastVersionLabel";
			this.lastVersionLabel.Size = new System.Drawing.Size(318, 19);
			this.lastVersionLabel.TabIndex = 40;
			this.lastVersionLabel.Text = "新しいバージョンを確認中です";
			this.lastVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lastVersionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LastVersionLabelLinkClicked);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.button1);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(0, 128);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(318, 33);
			this.panel3.TabIndex = 43;
			// 
			// button1
			// 
			this.button1.AutoSize = true;
			this.button1.Location = new System.Drawing.Point(127, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 24);
			this.button1.TabIndex = 23;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.okBtnClick);
			// 
			// panel4
			// 
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Location = new System.Drawing.Point(0, 118);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(318, 10);
			this.panel4.TabIndex = 42;
			// 
			// communityLinkLabel
			// 
			this.communityLinkLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.communityLinkLabel.Location = new System.Drawing.Point(0, 99);
			this.communityLinkLabel.Name = "communityLinkLabel";
			this.communityLinkLabel.Size = new System.Drawing.Size(318, 19);
			this.communityLinkLabel.TabIndex = 41;
			this.communityLinkLabel.TabStop = true;
			this.communityLinkLabel.Text = "https://com.nicovideo.jp/community/co2414037";
			this.communityLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.communityLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.communityLinkLabel_Click);
			// 
			// downloadPageLinkLabel
			// 
			this.downloadPageLinkLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.downloadPageLinkLabel.Location = new System.Drawing.Point(0, 80);
			this.downloadPageLinkLabel.Name = "downloadPageLinkLabel";
			this.downloadPageLinkLabel.Size = new System.Drawing.Size(318, 19);
			this.downloadPageLinkLabel.TabIndex = 49;
			this.downloadPageLinkLabel.TabStop = true;
			this.downloadPageLinkLabel.Text = "https://guest-nico.github.io/pages/downloads.html";
			this.downloadPageLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.downloadPageLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DownloadPageLinkLabelLinkClicked);
			// 
			// VersionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(318, 173);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.communityLinkLabel);
			this.Controls.Add(this.downloadPageLinkLabel);
			this.Controls.Add(this.lastVersionLabel);
			this.Controls.Add(this.panel5);
			this.Controls.Add(this.versionLabel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "VersionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "バージョン情報";
			this.Load += new System.EventHandler(this.VersionFormLoad);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.LinkLabel downloadPageLinkLabel;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.LinkLabel lastVersionLabel;
		public System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.LinkLabel communityLinkLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label versionLabel;
	}
}
