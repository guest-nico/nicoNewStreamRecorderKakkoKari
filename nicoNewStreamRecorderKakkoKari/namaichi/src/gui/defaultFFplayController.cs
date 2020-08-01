/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/21
 * Time: 18:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace namaichi
{
	/// <summary>
	/// Description of defaultFFplayController.
	/// </summary>
	public partial class defaultFFplayController : Form
	{
		private config.config config;
		private play.Player player;
		public Process process;
		private bool onMuteButtonArea = false;
		public bool isMute = false;
		public int volume;
		
		public defaultFFplayController(config.config config, Process process, play.Player player)
		{
			this.config = config;
			this.process = process;
			this.player = player;
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			try {
				StartPosition = FormStartPosition.Manual;
				this.Location = new Point(int.Parse(config.get("defaultControllerX")),
						int.Parse(config.get("defaultControllerY")));
//				Left = int.Parse(config.get("X"));
//				Top = int.Parse(config.get("Y"));
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			
			try {
				util.debugWriteLine("dfc 0");
				var a = int.Parse(config.get("volume"));
				util.debugWriteLine("dfc 2 " + a);
				volumeBarArea.Width = 9;
				util.debugWriteLine("dfc 3 " + volumeBarArea.Width);
				volumeBarArea.Width = int.Parse(config.get("volume"));
				util.debugWriteLine("dfc 1");
				volumeTip.Text = "音量：" + volumeBarArea.Width;
				
				timeLabel.Paint += timeBorderPaint;
				volumeTip.Paint += volumeTipBorderPaint;
				muteTip.Paint += muteTipBorderPaint;
				reconnectTip.Paint += reconnectTipBorderPaint;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			util.setFontSize(int.Parse(config.get("fontSize")), this, false);
		}
		private void timeBorderPaint(object sender, PaintEventArgs e) {
			ControlPaint.DrawBorder(e.Graphics, timeLabel.ClientRectangle, Color.FromArgb(64, 64, 64), ButtonBorderStyle.Solid);
		}
		private void volumeTipBorderPaint(object sender, PaintEventArgs e) {
			var color = (isMute) ? Color.FromArgb(32, 32, 32) : Color.FromArgb(64, 64, 64);
			ControlPaint.DrawBorder(e.Graphics, volumeTip.ClientRectangle, color, ButtonBorderStyle.Solid);
		}
		private void muteTipBorderPaint(object sender, PaintEventArgs e) {
			muteTip.BringToFront();
			var color = Color.FromArgb(64, 64, 64);
			ControlPaint.DrawBorder(e.Graphics, muteTip.ClientRectangle, color, ButtonBorderStyle.Solid);
		}
		private void reconnectTipBorderPaint(object sender, PaintEventArgs e) {
			reconnectTip.BringToFront();
			var color = Color.FromArgb(64, 64, 64);
			ControlPaint.DrawBorder(e.Graphics, reconnectTip.ClientRectangle, color, ButtonBorderStyle.Solid);
		}
		void muteButton_Click(object sender, System.EventArgs e)
		{
			isMute = !isMute;
			if (isMute) {
				volumeAllBarArea.BackColor = Color.FromArgb(98, 98, 98);
				volumeBarArea.BackColor = Color.FromArgb(0, 64, 127);
			} else {
				volumeAllBarArea.BackColor = Color.FromArgb(197, 197, 197);
				volumeBarArea.BackColor = Color.FromArgb(0, 128, 255);
			}
			setMuteButtonImage();
			
			volumeSet(-10);
		}
		void muteButton_MouseEnter(object sender, EventArgs e)
		{
			onMuteButtonArea = true;
			setMuteButtonImage();
			
			muteTip.Text = (isMute) ? "ミュート解除" : "ミュート(消音)";
			muteTip.Visible = true;
		}
		void muteButton_MouseLeave(object sender, System.EventArgs _e)
		{
			var e = PointToClient(Cursor.Position);
			if (e.X < 156 || e.Y < 15 || e.X >= 178 || e.Y >= 37) {
				onMuteButtonArea = false;
				
				muteTip.Visible = false;
			} else onMuteButtonArea = true;
			setMuteButtonImage();
		}
		void setMuteButtonImage() {
			if (onMuteButtonArea && isMute) onMutedButton.BringToFront();
			if (onMuteButtonArea && !isMute) onMuteButton.BringToFront();
			if (!onMuteButtonArea && isMute) mutedButton.BringToFront();
			if (!onMuteButtonArea && !isMute) muteButton.BringToFront();
		}
		void reconnectButton_Click(object sender, System.EventArgs e)
		{
			player.isReconnect = true;
		}
		void reconnectButton_MouseEnter(object sender, EventArgs e)
		{
			//onReconnectButtonArea = true;
			reconnectOnBtn.BringToFront();
			reconnectTip.Visible = true;
		}
		void reconnectButton_MouseLeave(object sender, System.EventArgs _e)
		{
			var e = PointToClient(Cursor.Position);
			if (e.X < 289 || e.Y < 15 || e.X >= 311 || e.Y >= 37) {
//				onMuteButtonArea = false;
				
				reconnectTip.Visible = false;
				reconnectBtn.BringToFront();
				return;
			}
			reconnectOnBtn.BringToFront();
		}
		private void volumeSet(int v) {
			//v 0-100 mute -10
			try {
				//process.StandardInput.WriteLine(v.ToString());
//				util.debugWriteLine("volume set " + v);
				while (player == null || player.pipeWriter == null) System.Threading.Thread.Sleep(1000);
				player.pipeWriter.WriteLine(v.ToString());
				player.pipeWriter.Flush();
			} catch (Exception e) {
				util.debugWriteLine("volume set exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			volume = v;
		}
		void form_Close(object sender, FormClosingEventArgs e)
		{
			if (!kakuninClose()) e.Cancel = true;
		}
		bool kakuninClose() {
			try{
				util.debugWriteLine("defaultControllerX  " + Location.X + " defaultControllerX " + Location.Y + " volume " + volumeBarArea.Width.ToString());
				if (this.WindowState == FormWindowState.Normal) {
					config.set("defaultControllerX", Location.X.ToString());
					config.set("defaultControllerY", Location.Y.ToString());
					
				} else {
					config.set("defaultControllerX", RestoreBounds.X.ToString());
					config.set("defaultControllerX", RestoreBounds.Y.ToString());
				}
				config.set("volume", volumeBarArea.Width.ToString());

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			return true;
		}
		public void setTimeLabel(string s) {
			try {
				if (IsDisposed) return;
				this.Invoke((MethodInvoker)delegate() {
	       	       	try {
		       			timeLabel.Text = s;
	       	       	} catch (Exception e) {
	       	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	       	}
				});
			} catch (Exception e) {
				util.debugWriteLine("ctrl set time " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		public void reset() {
			isMute = false;
			onMuteButtonArea = false;
			try {
				if (IsDisposed) return;
				this.Invoke((MethodInvoker)delegate() {
	       	       	try {
						setMuteButtonImage();
						
						volumeAllBarArea.BackColor = Color.FromArgb(197, 197, 197);
						volumeBarArea.BackColor = Color.FromArgb(0, 128, 255);
						volumeBarArea.Width  = volume;
						
	       	       	} catch (Exception e) {
	       	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	       	}
				});
			} catch (Exception e) {
				util.debugWriteLine("reset mute btn image " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
	}
}
