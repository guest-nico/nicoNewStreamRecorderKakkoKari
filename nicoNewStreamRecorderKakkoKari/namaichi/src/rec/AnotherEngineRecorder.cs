/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/19
 * Time: 18:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
		public void record(string hlsSegM3uUrl, string recFolderFile, string command, string header) {
			util.debugWriteLine("another rec start");
			util.debugWriteLine("command "  + command);

			System.IO.Directory.SetCurrentDirectory(util.getJarPath()[0]);
			
			var _command = command.Replace("{i}", "\"" + hlsSegM3uUrl + "\"");
			_command = getAddedExtRecFilePath(recFolderFile, _command, header);
			
			
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
			
			STARTUPINFO si = new STARTUPINFO();
			PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
			
			si.cb = (uint)Marshal.SizeOf(si);
			
			si.dwFlags = 0x00000001; //STARTF_USESHOWWINDOW
			si.wShowWindow = 7; //SW_SHOWMINNOACTIVE
			bool r = CreateProcess(
					null,
					f + " " + arg,
					IntPtr.Zero,
					IntPtr.Zero,
					false,
					0,
					IntPtr.Zero,
					null,
					ref si,
					out pi);
			if (!r) {
				Thread.Sleep(1000);
				util.debugWriteLine("create false");
				return;
			}
			util.debugWriteLine("testcommand " + f + " " + arg);
			process = Process.GetProcessById((int)pi.dwProcessId);
			/*
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
			*/
//			util.debugWriteLine(command[command.Length - 2].Trim('\"'));
			
			try {
				//var h = GetForegroundWindow();
				//util.debugWriteLine("getforeground " + h);
				//process.Start();
				/*
				for (var i = 0; i < 50; i++) {
					if (GetForegroundWindow() == h) {
						Thread.Sleep(100);
						util.debugWriteLine("getforeground2 " + h);
						rm.form.addLogText("get2 ok");
						continue;
					}
					for (var j = 0; j < 50; j++) {
						var foreH = SetForegroundWindow(h);
						util.debugWriteLine("setforeground " + foreH);
						if (foreH) {
							rm.form.addLogText("set ok");
							break;
						}
						rm.form.addLogText("set no");
						Thread.Sleep(100);
					}
					break;
				}
				*/
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
			
			util.debugWriteLine("destroy " + process.HasExited);


		}
		public void waitProcess(string hlsSegM3uUrl) {
			//var es = process.StandardError;
			
			if (process.HasExited)
				util.debugWriteLine("start waitprocess " + process.HasExited);
			while (true) {
				if (process.HasExited) {
					util.debugWriteLine("waitprocess hasexited");
					break;
				}
				Thread.Sleep(3000);
				
				if (rec.ri.timeShiftConfig != null && isEndTime(hlsSegM3uUrl)) {
					rec.isEndProgram = true;
					util.debugWriteLine("end waitprocess ");
					stopRecording();
				}
				if (rm.rfu != rfu) stopRecording();
			}
			if (process.HasExited)
				util.debugWriteLine("end waitprocess " + process.HasExited);
		}
		private string getAddedExtRecFilePath(string recFolderFile, string command, string header) {
			var r = new Regex("\\{o\\}(\\.\\S+)");
			var m = r.Match(command);
			if (m.Success) 
				ext = m.Groups[1].ToString();
			
			command = r.Replace(command, "\"" + recFolderFile + "${1}\"");
			command = command.Replace("{o}", "\"" + recFolderFile + ext + "\"");
			command = command.Replace("{h}", header);
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
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CreateProcess(
        string lpApplicationName,
        string lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);

    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFO
    {
        public uint cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public uint dwX;
        public uint dwY;
        public uint dwXSize;
        public uint dwYSize;
        public uint dwXCountChars;
        public uint dwYCountChars;
        public uint dwFillAttribute;
        public uint dwFlags;
        public ushort wShowWindow;
        public ushort cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public uint dwProcessId;
        public uint dwThreadId;
    }
	}
}
