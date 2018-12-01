/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/21
 * Time: 18:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Windows.Forms;
using System.Drawing;
using System;

namespace namaichi
{
	partial class defaultFFplayController
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(defaultFFplayController));
			this.muteTip = new System.Windows.Forms.Label();
			this.volumeTip = new System.Windows.Forms.Label();
			this.muteButton = new System.Windows.Forms.PictureBox();
			this.mutedButton = new System.Windows.Forms.PictureBox();
			this.onMuteButton = new System.Windows.Forms.PictureBox();
			this.onMutedButton = new System.Windows.Forms.PictureBox();
			this.volumeBarArea = new System.Windows.Forms.Panel();
			this.timeLabel = new System.Windows.Forms.Label();
			this.volumeAllBarArea = new System.Windows.Forms.Panel();
			this.volumeAreaPanel = new System.Windows.Forms.Panel();
			this.reconnectBtn = new System.Windows.Forms.PictureBox();
			this.reconnectTip = new System.Windows.Forms.Label();
			this.reconnectOnBtn = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.muteButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mutedButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.onMuteButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.onMutedButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.reconnectBtn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.reconnectOnBtn)).BeginInit();
			this.SuspendLayout();
			// 
			// muteTip
			// 
			this.muteTip.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.muteTip.ForeColor = System.Drawing.Color.White;
			this.muteTip.Location = new System.Drawing.Point(128, 1);
			this.muteTip.Name = "muteTip";
			this.muteTip.Size = new System.Drawing.Size(80, 17);
			this.muteTip.TabIndex = 16;
			this.muteTip.Text = "ミュート(消音)";
			this.muteTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.muteTip.Visible = false;
			// 
			// volumeTip
			// 
			this.volumeTip.ForeColor = System.Drawing.Color.White;
			this.volumeTip.Location = new System.Drawing.Point(199, 1);
			this.volumeTip.Name = "volumeTip";
			this.volumeTip.Size = new System.Drawing.Size(69, 17);
			this.volumeTip.TabIndex = 17;
			this.volumeTip.Text = "音量：";
			this.volumeTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.volumeTip.Visible = false;
			// 
			// muteButton
			// 
			this.muteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.muteButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.muteButton.Image = ((System.Drawing.Image)(resources.GetObject("muteButton.Image")));
			this.muteButton.InitialImage = ((System.Drawing.Image)(resources.GetObject("muteButton.InitialImage")));
			this.muteButton.Location = new System.Drawing.Point(156, 15);
			this.muteButton.Name = "muteButton";
			this.muteButton.Size = new System.Drawing.Size(22, 22);
			this.muteButton.TabIndex = 12;
			this.muteButton.TabStop = false;
			this.muteButton.Click += new System.EventHandler(this.muteButton_Click);
			this.muteButton.MouseEnter += new System.EventHandler(this.muteButton_MouseEnter);
			this.muteButton.MouseLeave += new System.EventHandler(this.muteButton_MouseLeave);
			// 
			// mutedButton
			// 
			this.mutedButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.mutedButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.mutedButton.Image = ((System.Drawing.Image)(resources.GetObject("mutedButton.Image")));
			this.mutedButton.InitialImage = ((System.Drawing.Image)(resources.GetObject("mutedButton.InitialImage")));
			this.mutedButton.Location = new System.Drawing.Point(156, 15);
			this.mutedButton.Name = "mutedButton";
			this.mutedButton.Size = new System.Drawing.Size(22, 22);
			this.mutedButton.TabIndex = 9;
			this.mutedButton.TabStop = false;
			this.mutedButton.Click += new System.EventHandler(this.muteButton_Click);
			this.mutedButton.MouseEnter += new System.EventHandler(this.muteButton_MouseEnter);
			this.mutedButton.MouseLeave += new System.EventHandler(this.muteButton_MouseLeave);
			// 
			// onMuteButton
			// 
			this.onMuteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.onMuteButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.onMuteButton.Image = ((System.Drawing.Image)(resources.GetObject("onMuteButton.Image")));
			this.onMuteButton.InitialImage = ((System.Drawing.Image)(resources.GetObject("onMuteButton.InitialImage")));
			this.onMuteButton.Location = new System.Drawing.Point(156, 15);
			this.onMuteButton.Name = "onMuteButton";
			this.onMuteButton.Size = new System.Drawing.Size(22, 22);
			this.onMuteButton.TabIndex = 11;
			this.onMuteButton.TabStop = false;
			this.onMuteButton.Click += new System.EventHandler(this.muteButton_Click);
			this.onMuteButton.MouseEnter += new System.EventHandler(this.muteButton_MouseEnter);
			this.onMuteButton.MouseLeave += new System.EventHandler(this.muteButton_MouseLeave);
			// 
			// onMutedButton
			// 
			this.onMutedButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.onMutedButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.onMutedButton.Image = ((System.Drawing.Image)(resources.GetObject("onMutedButton.Image")));
			this.onMutedButton.InitialImage = ((System.Drawing.Image)(resources.GetObject("onMutedButton.InitialImage")));
			this.onMutedButton.Location = new System.Drawing.Point(156, 15);
			this.onMutedButton.Name = "onMutedButton";
			this.onMutedButton.Size = new System.Drawing.Size(22, 22);
			this.onMutedButton.TabIndex = 10;
			this.onMutedButton.TabStop = false;
			this.onMutedButton.Click += new System.EventHandler(this.muteButton_Click);
			this.onMutedButton.MouseEnter += new System.EventHandler(this.muteButton_MouseEnter);
			this.onMutedButton.MouseLeave += new System.EventHandler(this.muteButton_MouseLeave);
			// 
			// volumeBarArea
			// 
			this.volumeBarArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.volumeBarArea.Cursor = System.Windows.Forms.Cursors.Hand;
			this.volumeBarArea.Location = new System.Drawing.Point(183, 24);
			this.volumeBarArea.Name = "volumeBarArea";
			this.volumeBarArea.Size = new System.Drawing.Size(10, 4);
			this.volumeBarArea.TabIndex = 14;
			this.volumeBarArea.SizeChanged += new System.EventHandler(this.volumeBarArea_SizeChanged);
			this.volumeBarArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.volumeArea_MouseSet);
			this.volumeBarArea.MouseEnter += new System.EventHandler(this.volumeArea_MouseEnter);
			this.volumeBarArea.MouseLeave += new System.EventHandler(this.volumeArea_MouseLeave);
			this.volumeBarArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.volumeArea_MouseSet);
			// 
			// timeLabel
			// 
			this.timeLabel.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.timeLabel.ForeColor = System.Drawing.Color.White;
			this.timeLabel.Location = new System.Drawing.Point(6, 16);
			this.timeLabel.Name = "timeLabel";
			this.timeLabel.Size = new System.Drawing.Size(130, 22);
			this.timeLabel.TabIndex = 8;
			this.timeLabel.Text = "00:00/0:00:00";
			this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// volumeAllBarArea
			// 
			this.volumeAllBarArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
			this.volumeAllBarArea.Cursor = System.Windows.Forms.Cursors.Hand;
			this.volumeAllBarArea.Location = new System.Drawing.Point(183, 24);
			this.volumeAllBarArea.Name = "volumeAllBarArea";
			this.volumeAllBarArea.Size = new System.Drawing.Size(100, 4);
			this.volumeAllBarArea.TabIndex = 13;
			this.volumeAllBarArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.volumeArea_MouseSet);
			this.volumeAllBarArea.MouseEnter += new System.EventHandler(this.volumeArea_MouseEnter);
			this.volumeAllBarArea.MouseLeave += new System.EventHandler(this.volumeArea_MouseLeave);
			this.volumeAllBarArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.volumeArea_MouseSet);
			// 
			// volumeAreaPanel
			// 
			this.volumeAreaPanel.BackColor = System.Drawing.Color.Black;
			this.volumeAreaPanel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.volumeAreaPanel.Location = new System.Drawing.Point(183, 18);
			this.volumeAreaPanel.Name = "volumeAreaPanel";
			this.volumeAreaPanel.Size = new System.Drawing.Size(100, 16);
			this.volumeAreaPanel.TabIndex = 15;
			this.volumeAreaPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.volumeArea_MouseSet);
			this.volumeAreaPanel.MouseEnter += new System.EventHandler(this.volumeArea_MouseEnter);
			this.volumeAreaPanel.MouseLeave += new System.EventHandler(this.volumeArea_MouseLeave);
			this.volumeAreaPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.volumeArea_MouseSet);
			// 
			// reconnectBtn
			// 
			this.reconnectBtn.Image = ((System.Drawing.Image)(resources.GetObject("reconnectBtn.Image")));
			this.reconnectBtn.Location = new System.Drawing.Point(289, 15);
			this.reconnectBtn.Name = "reconnectBtn";
			this.reconnectBtn.Size = new System.Drawing.Size(22, 22);
			this.reconnectBtn.TabIndex = 18;
			this.reconnectBtn.TabStop = false;
			this.reconnectBtn.Click += new System.EventHandler(this.reconnectButton_Click);
			this.reconnectBtn.MouseEnter += new System.EventHandler(this.reconnectButton_MouseEnter);
			this.reconnectBtn.MouseLeave += new System.EventHandler(this.reconnectButton_MouseLeave);
			// 
			// reconnectTip
			// 
			this.reconnectTip.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.reconnectTip.ForeColor = System.Drawing.Color.White;
			this.reconnectTip.Location = new System.Drawing.Point(128, 1);
			this.reconnectTip.Name = "reconnectTip";
			this.reconnectTip.Size = new System.Drawing.Size(190, 17);
			this.reconnectTip.TabIndex = 19;
			this.reconnectTip.Text = "映像・音声が止まった際に押して下さい";
			this.reconnectTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.reconnectTip.Visible = false;
			// 
			// reconnectOnBtn
			// 
			this.reconnectOnBtn.Image = ((System.Drawing.Image)(resources.GetObject("reconnectOnBtn.Image")));
			this.reconnectOnBtn.Location = new System.Drawing.Point(289, 15);
			this.reconnectOnBtn.Name = "reconnectOnBtn";
			this.reconnectOnBtn.Size = new System.Drawing.Size(22, 22);
			this.reconnectOnBtn.TabIndex = 20;
			this.reconnectOnBtn.TabStop = false;
			this.reconnectOnBtn.Click += new System.EventHandler(this.reconnectButton_Click);
			this.reconnectOnBtn.MouseEnter += new System.EventHandler(this.reconnectButton_MouseEnter);
			this.reconnectOnBtn.MouseLeave += new System.EventHandler(this.reconnectButton_MouseLeave);
			// 
			// defaultFFplayController
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(321, 62);
			this.Controls.Add(this.reconnectTip);
			this.Controls.Add(this.reconnectBtn);
			this.Controls.Add(this.muteTip);
			this.Controls.Add(this.volumeTip);
			this.Controls.Add(this.muteButton);
			this.Controls.Add(this.mutedButton);
			this.Controls.Add(this.onMuteButton);
			this.Controls.Add(this.onMutedButton);
			this.Controls.Add(this.volumeBarArea);
			this.Controls.Add(this.timeLabel);
			this.Controls.Add(this.volumeAllBarArea);
			this.Controls.Add(this.volumeAreaPanel);
			this.Controls.Add(this.reconnectOnBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "defaultFFplayController";
			this.Text = "defaultFFplayController";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Close);
			((System.ComponentModel.ISupportInitialize)(this.muteButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mutedButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.onMuteButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.onMutedButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.reconnectBtn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.reconnectOnBtn)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.PictureBox reconnectOnBtn;
		private System.Windows.Forms.Label reconnectTip;
		private System.Windows.Forms.PictureBox reconnectBtn;
		private System.Windows.Forms.Panel volumeAreaPanel;
		private System.Windows.Forms.Panel volumeAllBarArea;
		private System.Windows.Forms.Label timeLabel;
		public System.Windows.Forms.Panel volumeBarArea;
		private System.Windows.Forms.PictureBox onMutedButton;
		private System.Windows.Forms.PictureBox onMuteButton;
		private System.Windows.Forms.PictureBox mutedButton;
		private System.Windows.Forms.PictureBox muteButton;
		private System.Windows.Forms.Label volumeTip;
		private System.Windows.Forms.Label muteTip;
		
		void volumeArea_MouseEnter(object sender, System.EventArgs _e)
		{
			
//			var e = PointToClient(Cursor.Position);
//			if (e.X >= 183 && e.Y >= 22 && e.X < 283 && e.Y < 30) {
				volumeBarArea.Location = new Point(183, 22);
				volumeBarArea.Height = 8;
				volumeAllBarArea.Location = new Point(183, 22);
				volumeAllBarArea.Height = 8;
				
				var color = (isMute) ? Color.FromArgb(78, 78, 78) : Color.FromArgb(255, 255, 255);
				volumeTip.ForeColor = color;
				volumeTip.Visible = true;
//			}
		}
		void volumeArea_MouseLeave(object sender, System.EventArgs _e)
		{
			var e = PointToClient(Cursor.Position);
			if (e.X < 183 || e.Y < 18 || e.X >= 283 || e.Y >= 34) {
				volumeBarArea.Location = new Point(183, 24);
				volumeBarArea.Height = 4;
				volumeAllBarArea.Location = new Point(183, 24);
				volumeAllBarArea.Height = 4;
				
				volumeTip.Visible = false;
			}
		}
		
		void volumeArea_MouseSet(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.None) return;
			
			var x = PointToClient(Cursor.Position).X;
			var v = x - volumeBarArea.Location.X;
			if (v > 100) v = 100;
			if (v < 0) v = 0;
			volumeBarArea.Width = v;
			//var b = e.Button;
			
			volumeTip.Text = "音量：" + v.ToString();
			if (isMute) volumeSet(-10);
			isMute = false;
			setMuteButtonImage();
			volumeAllBarArea.BackColor = Color.FromArgb(197, 197, 197);
			volumeBarArea.BackColor = Color.FromArgb(0, 128, 255);
			volumeTip.ForeColor = Color.FromArgb(255, 255, 255);;
			
		}
		void volumeBarArea_SizeChanged(object sender, System.EventArgs e)
		{
			try {
//				if (process == null) return;
				volumeSet(volumeBarArea.Width);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
	}
	
}
