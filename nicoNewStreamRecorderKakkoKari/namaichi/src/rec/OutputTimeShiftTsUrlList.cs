/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/09/16
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of OutputTimeShiftTsUrlList.
	/// </summary>
	public class OutputTimeShiftTsUrlList
	{
		TimeShiftConfig tsConfig;
		RecordingManager rm;
		
		int startNum;
		public bool isStarted = false;
		private string ext = null;
		
		public OutputTimeShiftTsUrlList(TimeShiftConfig tsConfig, RecordingManager rm)
		{
			this.tsConfig = tsConfig;
			this.rm = rm;
			ext = rm.rfu.h5r.ri.isFmp4 ? ".mp4" : ".ts";
		}
		public void setStartNum(string playList) {
			startNum = int.Parse(util.getRegGroup(playList, "(\\d+)" + ext));
		}
		public void write(string playList, string _path, string baseUrl, TimeShiftConfig tsConfig) {
			isStarted = true;
			var path = _path + ((tsConfig.isM3u8List) ? ".m3u8" : ".txt");
			
			var _hasuu = util.getRegGroup(playList, "(\\d\\d+)" + ext);
			var hasuu = (_hasuu == null) ? 0 : int.Parse(_hasuu);
			hasuu = hasuu % 5000;
			var _duration = util.getRegGroup(playList, "#DMC-STREAM-DURATION:(.+)");
			var duration = double.Parse(_duration, System.Globalization.NumberStyles.Float);
			var _temp = util.getRegGroup(playList, "(.+\\" + ext + ".+)");
			var temp = baseUrl + _temp;
			var tempNum = util.getRegGroup(_temp, "(\\d+)");
			
			var ret = "";
			if (startNum >= tsConfig.timeSeconds * 1000) 
				ret += temp.Replace(tempNum + ext, startNum.ToString() + ext);
			for (var i = 5000 + hasuu; i < duration * 1000; i += 5000) {
				if (i < tsConfig.timeSeconds * 1000 || 
				    (tsConfig.endTimeSeconds != 0 && i > tsConfig.endTimeSeconds * 1000)) continue;
				if (ret != "") ret += "\r\n";
				ret += temp.Replace(tempNum + ext, i + ext);
			}
			
			if (tsConfig.isM3u8List) {
				writeM3u8List(path, ret);
			} else {
				using (var w = new StreamWriter(path, false)) {
					w.Write(ret);
					//w.Flush();
					//w.Close();
				}
				if (tsConfig.isOpenUrlList)
					openFile(path);
			}
		}
		private string openFile(string listPath) {
			var listDirPath = new FileInfo(listPath).Directory.ToString();
			//var o = listDirPath + "/out" + ext;
			//var _command = tsConfig.openListCommand.Replace("{i}", "\"" + listPath + "\"");
			//var _i = util.getRegGroup(tsConfig.openListCommand, "(\\{i\\}[^s]*)");
			//var _command = Regex.Replace(tsConfig.openListCommand, "\\{i\\}([^s]*)", "{$1}
			var _command = Regex.Replace(tsConfig.openListCommand, "\\{i\\}(\\S*)", "\"" + listPath + "$1\"");
			//_command = _command.Replace("{o}", o);
			
			string f = null;
			string arg = null;
			if (_command.StartsWith("\"")) {
				f = util.getRegGroup(_command, "\"(.+?)\"");
				arg = util.getRegGroup(_command, "\".+?\"(.+)");
			} else {
				f = util.getRegGroup(_command, "(.+?) ");
				arg = util.getRegGroup(_command, ".+? (.+)");
			}
			if (f == null || arg == null) return f + " " + arg;
			
			util.debugWriteLine(f + " " + arg);
			var p = new Process();
			p.StartInfo.FileName = f;
			p.StartInfo.Arguments = arg;
			p.Start();
			return "ok";
		}
		private void writeM3u8List(string path, string buf) {
			using (var w = new StreamWriter(path, false)) {
				w.WriteLine("#EXTM3U");
				w.WriteLine("#EXT-X-VERSION:3");
				w.WriteLine("#EXT-X-TARGETDURATION:" + (tsConfig.m3u8UpdateSeconds + 3).ToString());
				w.Flush();
				var isOpened = false;
				foreach(var b in buf.Split('\n')) {
					w.WriteLine("#EXTINF:" + tsConfig.m3u8UpdateSeconds.ToString() + ",");
					w.WriteLine(b);
					w.Flush();
					
					if (tsConfig.isOpenUrlList && !isOpened) {
						var openRes = openFile(path);
						if (openRes != "ok") {
							rm.form.addLogText(openRes);
						}
						isOpened = true;
					}
					Thread.Sleep((int)(tsConfig.m3u8UpdateSeconds * 1000));
				}
				w.WriteLine("#EXT-X-ENDLIST");
				//w.Close();
			}
			
		}
	}
}
