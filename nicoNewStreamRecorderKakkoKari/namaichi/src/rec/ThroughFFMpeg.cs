/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/07/27
 * Time: 17:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.IO;

namespace namaichi.rec
{
	/// <summary>
	/// Description of ThroughFFMpeg.
	/// </summary>
	public class ThroughFFMpeg
	{
		private System.Diagnostics.Process process;
		private RecordingManager rm;
		
		public ThroughFFMpeg(RecordingManager rm)
		{
			this.rm = rm;
		}
		public void start(string path, bool isConvert) {
			util.debugWriteLine("through ffmpeg path " + path);
			if (!spaceCheck(path)) return;
			rm.form.addLogText("FFmpeg処理を開始します");
			var fName = util.getRegGroup(path, ".+(\\\\|/)(.+)", 2);
			var dir = util.getRegGroup(path, "(.+(\\\\|/)).+");
			string tmp = dir + "_" + fName;
			util.debugWriteLine("through ffmpeg tmp " + tmp);
			
			var afterConvertMode = int.Parse(rm.cfg.get("afterConvertMode"));
			string outPath = path;
			if (isConvert && afterConvertMode > 0) 
				getConvertPaths(path, ref tmp, ref outPath, afterConvertMode);
			string _command;
			var cfgCommand = rm.cfg.get("afterConvertModeCmd");
			if (cfgCommand != "") _command = cfgCommand;
			else _command = util.getFFmpegDefaultArg(afterConvertMode);
			_command = _command.Replace("{path}", path).Replace("{tmp}", tmp);
			
			util.debugWriteLine("through command " + _command);
			
			var e = new EventHandler(appExitHandler);
			Application.ApplicationExit += e;
			
			process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "" + util.getJarPath()[0] + 
				("\\ffmpeg") + "";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Arguments = _command;
		
			try {
				process.Start();
				
//				Task.Run(() => {
//				         	getStandardOutput();
//				});
				
				displayRecordStatus();
//				util.debugWriteLine("stop record");
//				stopRecording();
				Application.ApplicationExit -= e;
				
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.StackTrace);
			}
			
			try {
				if (!File.Exists(tmp)) {
					util.debugWriteLine("through ffmpeg not exist tmp " + tmp);
					rm.form.addLogText("FFmpeg処理中に一時ファイルが見つかりませんでした");
					return;
				}
				File.Delete(path);
				File.Move(tmp, outPath);
			} catch (Exception eee) {
				util.debugWriteLine("through ffmpeg delete move exception");
				util.debugWriteLine(path + " " + tmp);
				util.debugWriteLine(eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);
				rm.form.addLogText("変換中にエラーが発生しました ffmpeg " + _command);
				rm.form.addLogText(eee.Message);
			}
			
			
//			while(true)
//				System.Threading.Thread.Sleep(3000);
			rm.form.addLogText("FFmpeg処理を完了しました");
			util.debugWriteLine("rec end through ffmpeg");
			
		}
		private void appExitHandler(object sender, EventArgs e) {
//			stopRecording();
		}
		private void getConvertPaths(string path, ref string tmp, ref string outPath, int afterConvertMode) {
			var ext = "";
//			if (afterConvertMode == 0 &&
//			    rm.cfg.get("IsRenketuAfter") == "true") ext = "ts";
			if (afterConvertMode == 0) ext = (tmp.EndsWith("ts") ? "ts" : "flv");
			if (afterConvertMode == 1) ext = (tmp.EndsWith("ts") ? "ts" : "flv");
			if (afterConvertMode == 2) ext = "ts";
			if (afterConvertMode == 3) ext = "avi";
			if (afterConvertMode == 4) ext = "mp4";
			if (afterConvertMode == 5) ext = "flv";
			if (afterConvertMode == 6) ext = "mov";
			if (afterConvertMode == 7) ext = "wmv";
			if (afterConvertMode == 8) ext = "vob";
			if (afterConvertMode == 9) ext = "mkv";
			if (afterConvertMode == 10) ext = "mp3";
			if (afterConvertMode == 11) ext = "wav";
			if (afterConvertMode == 12) ext = "wma";
			if (afterConvertMode == 13) ext = "aac";
			if (afterConvertMode == 14) ext = "ogg";
			if (afterConvertMode == 15) ext = "mp4";
			var originalExtLen = tmp.EndsWith("ts") ? 2 : 3;
			tmp = tmp.Substring(0, tmp.Length - originalExtLen) + ext;
//			tmp = tmp.Substring(0, tmp.Length - 2) + ext;
			outPath = path.Substring(0, path.Length - originalExtLen) + ext;
		}
		/*
		public void stopRecording() {
			util.debugWriteLine("stop recording through ffmpeg");
	
			if (process == null || process.HasExited) return;
			
			
			try {
				var i = process.StandardInput;
				i.WriteLine("q");
				i.Flush();
				i.Close();
			} catch (Exception ee) {
				if (!(ee is  System.ObjectDisposedException))
					util.debugWriteLine(ee.Message + ee.StackTrace);
				try {
					process.Kill();
				} catch (Exception eee) {
					util.debugWriteLine(eee.Message + eee.StackTrace);
				}
			};
			
			while(!process.HasExited) {
				System.Threading.Thread.Sleep(200);
			}
			
			util.debugWriteLine("destroy " + process.ExitCode);
		}
		*/
		public void displayRecordStatus() {
			var es = process.StandardError;
			
			
			while (!process.HasExited) {
				try {
					var line = es.ReadLine();
//					lastReadTime = DateTime.UtcNow;
					
					if (line == null) break;
					
					util.debugWriteLine("error " + line);
					displayStateGui(line);
					
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite);
				};
				
//				if (rm.rfu != rfu) stopRecording();
			}
			try {
				
				es.Close();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.StackTrace);
			}

		}
		private void displayStateGui(string line) {
			if (line.StartsWith("[hls")) return;
			if (line.IndexOf("Cannot reuse HTTP") != -1) return;
			if (line.IndexOf("Opening") != -1) return;
			if (line.IndexOf("Last message") != -1) return;
			if (line.IndexOf("skipping") != -1) return;
			if (line.IndexOf("expire") != -1) return;
			if (line.IndexOf("retrying with new") != -1) return;
			if (line.IndexOf("different host") != -1) return;
             
			if (line.IndexOf("http @ ") != -1) return;
			if (line.IndexOf("for reading") != -1) return;
			if (line.IndexOf("may result in incorrect") != -1) return;
			
			if (line.StartsWith("frame=")) rm.form.setRecordState(line);
			
//				util.getShiftJisToUni
//			else rm.form.addLogText(line);
			
		}
		private bool spaceCheck(string path) {
			try {
				var d = Directory.GetDirectoryRoot(path);
				if (d == null) return true;
				if (util.getRegGroup(d[0].ToString(), "([a-zA-Z])") == null) return true;
	
				var di = new DriveInfo(d);
				if (!File.Exists(path)) {
					rm.form.addLogText("変換対象のファイルが見つかりませんでした");
					return false;
				}
					
				if (di.AvailableFreeSpace < new FileInfo(path).Length * 1.5) {
					DialogResult adr = DialogResult.None;
					rm.form.formAction(() => adr = util.showMessageBoxCenterForm(rm.form, "空き容量が少なくなっています。変換しますか？", "", MessageBoxButtons.YesNo), false);
					return adr == DialogResult.Yes; 
				}
				return true;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				rm.form.addLogText("変換前の容量チェック中に問題が発生しました " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				return false;
			}
		}
	}
}
