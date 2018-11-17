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
		
		public AnotherEngineRecorder(RecordingManager rm, RecordFromUrl rfu)
		{
			this.rm = rm;
			this.rfu = rfu;
		}
		public void record(string hlsSegM3uUrl, string recFolderFile, string command) {
<<<<<<< HEAD
			util.debugWriteLine("another rec start");
=======
			util.debugWriteLine("rec start");
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			util.debugWriteLine("command "  + command);

			var _command = command.Replace("{i}", hlsSegM3uUrl);
			var o = recFolderFile + ".ts";
			if (recFolderFile.IndexOf(" ") > -1) o = "\"" + o + "\"";
			_command = _command.Replace("{o}", o);
			util.debugWriteLine("_command " + _command);
			EventHandler e = new EventHandler(appExitHandler);
			Application.ApplicationExit += e;
			
			string f = null;
			string arg = null;
			if (_command.StartsWith("\"")) {
				f = util.getRegGroup(_command, "\"(.+?)\"");
				arg = util.getRegGroup(_command, "\".+?\"(.+)");
			} else {
				f = util.getRegGroup(_command, "(.+?) ");
				arg = util.getRegGroup(_command, ".+? (.+)");
			}
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
				
				waitProcess();
				util.debugWriteLine("stop record");
				stopRecording();
				Application.ApplicationExit -= e;
				
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
	
<<<<<<< HEAD
			
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			if (process == null || process.HasExited) return;

			try {
				process.Kill();
			} catch (Exception eee) {
				util.debugWriteLine(eee.Message + eee.StackTrace + eee.Source + eee.TargetSite);
			}
		
			while(!process.HasExited) {
				System.Threading.Thread.Sleep(200);
			}
			
			util.debugWriteLine("destroy " + process.ExitCode);


		}
		public void waitProcess() {
			//var es = process.StandardError;
			
			
			while (!process.HasExited) {
				//if (process.WaitForExit(100)) break;
				Thread.Sleep(100);
				
				if (rm.rfu != rfu) stopRecording();
			}
			
		}
		
	}
}
