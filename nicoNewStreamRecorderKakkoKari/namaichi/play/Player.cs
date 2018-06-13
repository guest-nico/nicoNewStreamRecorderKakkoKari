/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/05/03
 * Time: 20:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing.Text;
using namaichi.rec;

namespace namaichi.play
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	public class Player
	{
		private RecordingManager rm;
		public Player(RecordingManager rm)
		{
			this.rm = rm;
		}
		public void play() {
			util.debugWriteLine(rm.hlsUrl);
			int a;
			if (rm.hlsUrl == null) return;
			var isVlc = true;
			if (isVlc) vlcPlay(rm.hlsUrl);
			else ffPlay(rm.hlsUrl);
			
		}
		private void vlcPlay(string url) {
			var exe = "C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc";
			var args = new string[]{"\"" + url + "\""};
			play(exe, args);
		}
		private void ffPlay(string url) {
			var exe = util.getJarPath()[0] + "\\ffmpeg";
//			var args = new string[]{"-i", url}; 
			var args = new string[]{"-i", url, "-f", "matroska", "-", "|", "ffplay", "-i", "-"};
			play(exe, args);
		}
		private void play(string exe, string[] args) {
			var process = new System.Diagnostics.Process();
			process.StartInfo.FileName = exe;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.UseShellExecute = false;
//			process.StartInfo.CreateNoWindow = true;
			util.debugWriteLine(string.Join(" ", args));
			process.StartInfo.Arguments = string.Join(" ", args);
			
			try {
				process.Start();
				
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.StackTrace);
			}
		}
	}
}
