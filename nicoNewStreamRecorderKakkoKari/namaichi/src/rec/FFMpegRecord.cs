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

namespace namaichi.rec
{
	/// <summary>
	/// Description of Record.
	/// </summary>
	public class FFMpegRecord
	{
		private RecordingManager rm;
		private bool isFFmpeg;
		private RecordFromUrl rfu;
		private System.Diagnostics.Process process;
		private DateTime lastReadTime = DateTime.UtcNow;
		
		public FFMpegRecord(RecordingManager rm, bool isFFmpeg, RecordFromUrl rfu) {
			this.rm = rm;
			this.isFFmpeg = isFFmpeg;
			this.rfu = rfu;
		}
		public void recordCommand(string[] command) {
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
				
//				Task.Run(() => {
//				         	getStandardOutput();
//				});
				
				displayRecordStatus();
				util.debugWriteLine("stop record");
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
			
			
			while (!process.HasExited) {
				try {
					var line = es.ReadLine();
					lastReadTime = DateTime.UtcNow;
					
					if (line == null) break;
					
					util.debugWriteLine("error " + line);
					displayStateGui(line);
					
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite);
				};
				
				if (rm.rfu != rfu) stopRecording();
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
	}
}
