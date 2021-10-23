/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/19
 * Time: 18:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class AnotherEngineRecorder
	{
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private System.Diagnostics.Process process;
		public string ext = null;
		private Record rec = null;
		
		public AnotherEngineRecorder(RecordingManager rm, RecordFromUrl rfu, Record rec)
		{
			this.rm = rm;
			this.rfu = rfu;
			ext = rfu.h5r.ri.isFmp4 ? ".mp4" : ".ts";
			this.rec = rec;
		}
		public void record(string hlsSegM3uUrl, string recFolderFile, string command) {
			util.debugWriteLine("another rec start");
			util.debugWriteLine("command "  + command);

			System.IO.Directory.SetCurrentDirectory(util.getJarPath()[0]);
			
			var _command = command.Replace("{i}", hlsSegM3uUrl);
			_command = getAddedExtRecFilePath(recFolderFile, _command);
			
			
			util.debugWriteLine("_command " + _command);
			EventHandler e = new EventHandler(appExitHandler);
			Application.ApplicationExit += e;
			
			string f = null;
			string arg = null;
			if (_command.StartsWith("\"")) {
				f = util.getRegGroup(_command, "\"(.+?)\"");
				arg = util.getRegGroup(_command, "\".+?\"(.*)");
			} else {
				f = util.getRegGroup(_command, "(.+?) ");
				arg = util.getRegGroup(_command, ".+? (.*)");
				if (f == null) {
					f = _command;
					arg = "";
				}
			}
			if (arg == null) arg = "";
				
			/*
			if (_command.StartsWith("\"")) {
				f = util.getRegGroup(_command, "\"(.+?)\"");
				arg = util.getRegGroup(_command, "\".+?\"(.+)");
			} else {
				f = util.getRegGroup(_command, "(.+?) ");
				arg = util.getRegGroup(_command, ".+? (.+)");
			}
			*/
			if (f == null || arg == null) return;
			
			process = new System.Diagnostics.Process();
			process.StartInfo.FileName = f;
			//process.StartInfo.RedirectStandardOutput = true;
			//process.StartInfo.RedirectStandardError = true;
			//process.StartInfo.RedirectStandardInput = true;
			//process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
			//process.StartInfo.UseShellExecute = false;
			//process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Arguments = arg;
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
			//process.StartInfo.UseShellExecute = true;
			
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
				
				waitProcess(hlsSegM3uUrl);
				util.debugWriteLine("stop record");
				stopRecording();
				Application.ApplicationExit -= e;
				
				if (rm.cfg.get("fileNameType") == "10" && (recFolderFile.IndexOf("{w}") > -1 || recFolderFile.IndexOf("{c}") > -1)) 
					renameStatistics(recFolderFile);
				
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.StackTrace);
			}
			
			
//			while(true)
//				System.Threading.Thread.Sleep(3000);
			
			                   
			util.debugWriteLine("another rec end");
		}
		private void appExitHandler(object sender, EventArgs e) {
			stopRecording();
		}
		public void stopRecording() {
			util.debugWriteLine("stop recording another engine");
	
			
			if (process == null || process.HasExited) return;

			try {
				if (process.CloseMainWindow()) {
					for (var i = 0; i < 10; i++) {
						if (process.HasExited) break;
						Thread.Sleep(1000);
					}
					process.Kill();
				} else process.Kill();
				
//				process.Kill();
			} catch (Exception eee) {
				util.debugWriteLine(eee.Message + eee.StackTrace + eee.Source + eee.TargetSite);
			}
		
			while(!process.HasExited) {
				System.Threading.Thread.Sleep(1000);
			}
			
			util.debugWriteLine("destroy " + process.ExitCode);


		}
		public void waitProcess(string hlsSegM3uUrl) {
			//var es = process.StandardError;
			
			
			while (!process.HasExited) {
				Thread.Sleep(3000);
				
				if (rec.ri.timeShiftConfig != null && isEndTime(hlsSegM3uUrl)) {
					rec.isEndProgram = true;
					stopRecording();
				}
				if (rm.rfu != rfu) stopRecording();
			}
			
		}
		private string getAddedExtRecFilePath(string recFolderFile, string command) {
			var r = new Regex("\\{o\\}(\\.\\S+)");
			var m = r.Match(command);
			if (m.Success) 
				ext = m.Groups[1].ToString();
			
			command = r.Replace(command, "\"" + recFolderFile + "${1}\"");
			command = command.Replace("{o}", "\"" + recFolderFile + ext + "\"");
//			if (recFolderFile.IndexOf(" ") > -1) o = "\"" + o + "\"";
//			_command = _command.Replace("{o}", o);
			return command;
		}
		private void renameStatistics(string recFolderFile) {
			try {
				Task.Run(() => {
				    var wsr = rfu.h5r.wsr;
				    wsr.setRealTimeStatistics();
					
				    //File.Move(name, name.Replace("{w}", visitCount.Replace("-", "")).Replace("{c}", commentCount.Replace("-", "")));
				    var newName = recFolderFile.Replace("{w}", wsr.visitCount.Replace("-", "")).Replace("{c}", wsr.commentCount.Replace("-", "")) + ext;
					if (File.Exists(newName)) 
						return;
						
					File.Move(recFolderFile + ext , newName);
				});

			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private bool isEndTime(string hlsSegM3uUrl) {
			var res = util.getPageSource(hlsSegM3uUrl, null);
			if (res == null) return false;
			if (rec.ri.timeShiftConfig.endTimeSeconds == 0) return false;
			
			var dur = util.getRegGroup(res, "#CURRENT-POSITION:(\\d+)");
			if (dur == null) return false;
			var m = new Regex("#EXTINF\\:(\\d+)").Matches(res);
			if (m.Count == 0) return false;
			double sum = 0;
			foreach (Match _m in m) {
				sum += double.Parse(_m.Groups[1].Value);
			}
			util.debugWriteLine(dur + " " + sum);
			return double.Parse(dur) + sum > rec.ri.timeShiftConfig.endTimeSeconds + 10;
		}
	}
}
