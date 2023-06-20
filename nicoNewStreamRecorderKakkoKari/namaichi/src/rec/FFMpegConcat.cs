/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/17
 * Time: 20:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Record.
	/// </summary>
	public class FFMpegConcat
	{
		private RecordingManager rm;
		private bool isFFmpeg = true;
		private RecordFromUrl rfu;
		private System.Diagnostics.Process process;
		private DateTime lastReadTime = DateTime.UtcNow;
		private int afterConvertMode = 0;
		public FFMpegConcat(RecordingManager rm, RecordFromUrl rfu) {
			this.rm = rm;
//			this.isFFmpeg = isFFmpeg;
			this.rfu = rfu;
			afterConvertMode = int.Parse(rm.cfg.get("afterConvertMode"));
		}
		public void concat(string outPath, List<string> files) {
			var input = "";
			var arg = "";
			var isM3u8 = true;
			if (isM3u8) {
				input += "#EXTM3U\n#EXT-X-VERSION:3\n#EXT-X-MEDIA-SEQUENCE:0\n";
				foreach (var s in files) input += "#EXTINF:7,\n" + s.Replace("\\", "/") + "\n";
				input += "#EXT-X-ENDLIST";
				arg = "-protocol_whitelist file,pipe -y -i -";
			} else {
				foreach (var s in files) input += "file '" + s.Replace("\\", "/") + "'\n";
				arg = "-protocol_whitelist file,pipe -f concat -safe 0 -y -i -";
			}
			
			outPath = replacedKakutyousi(outPath);
			
			//8-vob 11-wav 15-mp4(再エンコード)
			if (afterConvertMode == 8 || afterConvertMode == 11 || afterConvertMode == 15)
				arg += (" \"" + outPath + "\"");
			//10-mp3
			else if (afterConvertMode == 10)
				arg += (" -b:a 128k \"" + outPath + "\"");
			//12-wma
			else if (afterConvertMode == 12)
				arg +=  (" -vn -c copy \"" + outPath + "\"");
			//13-aac
			else if (afterConvertMode == 13)
				arg += (" -f mp4 -vn -c copy \"" + outPath + "\"");
			//14-ogg
			else if (afterConvertMode == 14)
				arg +=  (" -vn \"" + outPath + "\"");
			//5-flv
			else if (afterConvertMode == 5)
				arg += (" -c:v copy -c:a aac -bsf:a aac_adtstoasc \"" + outPath + "\"");
			else arg += (" -c copy \"" + outPath + "\"");
			
			//var arg = " -c copy \"" + outPath + "\" -y";
			
			
			arg = arg.Replace("\\", "/");
			try {
				EventHandler e = new EventHandler(appExitHandler);
				Application.ApplicationExit += e;
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = util.getJarPath()[0] + 
					"\\ffmpeg";
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.Arguments = arg;
			
				process.Start();
				//process.StandardInput.Write(input);
				
				var t = Task.Run(() => displayRecordStatus());
				var b = new byte[10000000];
				foreach (var fName in files) {
					using (var fs = new FileStream(fName, FileMode.Open)) {
						var c = fs.Read(b, 0, b.Length);
						//for (int i = 0; i < 10000000; i++)
						process.StandardInput.BaseStream.Write(b, 0, c);
						process.StandardInput.BaseStream.Flush();
					}
				}
				
				process.StandardInput.Close();
				
				//displayRecordStatus();
				Application.ApplicationExit -= e;
				
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.StackTrace + ee.Source + ee.TargetSite);
			}
		}
		public void recordCommand(string[] command, string m3u8, string pipeName) {
			util.debugWriteLine("rec start");
			util.debugWriteLine(String.Join(" ", command));

			EventHandler e = new EventHandler(appExitHandler);
			Application.ApplicationExit += e;
			
			process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "" + util.getJarPath()[0] + 
				((isFFmpeg) ? "\\ffmpeg" : "\\rtmpdump") + "";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Arguments = string.Join(" ", command);
			
//			util.debugWriteLine(command[command.Length - 2].Trim('\"'));
			/*
			if (System.IO.File.Exists(command[command.Length - 2].Trim('\"'))) {
			    	util.debugWriteLine(command[command.Length - 2]);
			    	util.debugWriteLine("exost file");
			    	var i = 3;
			    }
			*/
			
			try {
				process.Start();
				
				m3u8 = m3u8.Replace("\\", "/");
				var pipe = new NamedPipeServerStream(pipeName, PipeDirection.Out, 2, PipeTransmissionMode.Message, PipeOptions.None, 10000, 10000);
				var bytes = System.Text.Encoding.UTF8.GetBytes(m3u8);
//				var bytes = System.Text.Encoding.GetEncoding("shift-jis").GetBytes(m3u8);
//				var sj = System.Text.Encoding.GetEncoding("shift-jis").GetString(bytes); 
				pipe.WaitForConnection();
//				var w = new System.IO.StreamWriter(pipe 
				pipe.Write(bytes, 0, bytes.Length);
				pipe.Close();
				
				
//				var w = process.StandardInput.BaseStream;
//				var bytes = System.Text.Encoding.UTF8.GetBytes(m3u8);
//				var sj = System.Text.Encoding.GetEncoding("shift-jis").GetString(bytes);
//				w.Write(bytes, 0, bytes.Length);
//				w.WriteLine(m3u8);
//				w.Close();
				
				util.debugWriteLine(m3u8);
//				Task.Run(() => {
//				         	getStandardOutput();
//				});
				
				displayRecordStatus();
				util.debugWriteLine("stop record concat");
				stopRecording();
				Application.ApplicationExit -= e;
				
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.StackTrace);
			}
			
			
//			while(true)
//				System.Threading.Thread.Sleep(3000);
			util.debugWriteLine("rec end");
		}
		private void appExitHandler(object sender, EventArgs e) {
			stopRecording();
		}
		public void stopRecording() {
			util.debugWriteLine("isffmepg " + isFFmpeg);
	
			if (process == null || process.HasExited) return;
			
			if (isFFmpeg) {
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
			} else {
				process.Kill();
			}
			while(!process.HasExited) {
				System.Threading.Thread.Sleep(200);
			}
			
			util.debugWriteLine("destroy " + process.ExitCode);


		}
		public void displayRecordStatus() {
			var es = process.StandardError;
			
			
			while (!process.HasExited || true) {
				try {
					var line = es.ReadLine();
					lastReadTime = DateTime.UtcNow;
					
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
			
			
			if (isFFmpeg) {
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
				else rm.form.addLogText(line);
			} else {
	//			System.out.println(line.matches("^[0-9]") + "[" + line.substring(0,1) + ":");
				if (util.getRegGroup(line, "([0-9].*)") == null) {
	//				System.out.println("no suuji " + line);
					rm.form.addLogText(line);
				
				} else rm.form.setRecordState(line);
			}
			
		}
		public bool isStopRead() {
			var ret = DateTime.UtcNow - lastReadTime > new TimeSpan(0,0,30);
			if (ret) {
				var a = DateTime.UtcNow - lastReadTime;
				util.debugWriteLine(a);
			}
			return DateTime.UtcNow - lastReadTime > new TimeSpan(0,0,30);
		}
		private string replacedKakutyousi(string origin) {
			var ext = "ts";
			if (afterConvertMode == 0) ext = (origin.EndsWith("ts") ? "ts" : "flv");
			if (afterConvertMode == 1) ext = (origin.EndsWith("ts") ? "ts" : "flv");
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
			var originalExtLen = origin.EndsWith("ts") ? 2 : 3;
			var withoutExt = origin.Substring(0, origin.Length - originalExtLen - 1);
			withoutExt = util.getRegGroup(withoutExt, "(.+?)(\\d+)*$");
			var a = withoutExt.Length;
			for (var i = 0; i < 10000; i++)
				if (!File.Exists(withoutExt + i + "." + ext))
					return withoutExt + i + "." + ext;
			return withoutExt + 00 + "." + ext;
		}
	}
}
