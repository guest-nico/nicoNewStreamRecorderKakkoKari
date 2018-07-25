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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Record.
	/// </summary>
	public class Record
	{
		private RecordingManager rm;
		private bool isFFmpeg;
		private RecordFromUrl rfu;
		private System.Diagnostics.Process process;
		private DateTime lastReadTime = DateTime.UtcNow;
		public string hlsMasterUrl;
		private string recFolderFile;
		private string lvid;
		private int lastSegmentNo;
		private int lastAccessingSegmentNo;
		private CookieContainer container;
		private int segmentSaveType = 0;
		private bool isTimeShift = false;
		private List<numTaskInfo> newGetTsTaskList = new List<numTaskInfo>();
		private List<string> recordedNo = new List<string>();
		private string baseUrl;
		private WebSocketRecorder wr;
		private bool isReConnecting = false;
		private bool isRetry = true;
		private bool isEnd = false;
		private string hlsSegM3uUrl;
		private double recordedSecond = 0;
		private long recordedBytes = 0;
		
		public Record(RecordingManager rm, bool isFFmpeg, 
		              RecordFromUrl rfu, string hlsUrl, 
		              string recFolderFile, int lastSegmentNo, 
		              CookieContainer container, bool isTimeShift, 
		              WebSocketRecorder wr, string lvid) {
			this.rm = rm;
			this.isFFmpeg = isFFmpeg;
			this.rfu = rfu;
			this.hlsMasterUrl = hlsUrl;
			this.recFolderFile = recFolderFile;
			this.lastSegmentNo = lastSegmentNo;
			this.container = container;
			this.isTimeShift = isTimeShift;
			segmentSaveType = int.Parse(rm.cfg.get("segmentSaveType"));
			this.wr = wr;
			this.lvid = lvid;

		}
		public void record() {
			if (isTimeShift) {
				rm.form.addLogText("タイムシフトの録画を開始します");
//				timeShiftMultiRecord();
				timeShiftOnTimeRecord();
			} else {
				rm.form.addLogText("録画を開始します");
				realTimeRecord();
			}
			
		}
		private void realTimeRecord() {
			hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
			while (rm.rfu == rfu && isRetry) {
				if (isReConnecting) {
					Thread.Sleep(100);
					continue;
				}
				if (hlsSegM3uUrl == null) {
					isReConnecting = true;
					wr.reConnect();
					continue;
				}
				addNewTsTaskList(hlsSegM3uUrl);
				
				
			}
			waitForRecording();
			if (segmentSaveType == 1 && 
				    rm.cfg.get("IsRenketuAfter") == "true") {
				util.debugWriteLine("renketu after");
				renketuAfter();
			}
				
			isEnd = true;
		}
		private void renketuRecord() {
//				string[] command = { 
//						"-i", "" + url + "", 
//						"-c", "copy", "-bsf:a",
//						"aac_adtstoasc", "\"" + recFolderFile[1] + ".mp4\""};
				var tuika = rm.cfg.get("ffmpegopt");
				var tuikaArr = tuika.Split(' ');
				string[] _command = {
						"-i", "" + hlsMasterUrl + "", 
						"-c", "copy", "\"" + recFolderFile + ".ts\"" };
//						"-c", "copy", "-bsf:a", "aac_adtstoasc", "\"" + recFolderFile[1] + ".ts\"" };
//				var _buf = new List<string>();
				var _buf = string.Join(" ", _command);
				_buf += " " + tuika;
//				_buf.AddRange(_command.AsEnumerable());
//				_buf.AddRange(tuikaArr.AsEnumerable());
//				_buf.AddRange(_command.GetEnumerator);
//				_buf.AddRange(tuikaArr.GetEnumerator);
//				string[] command = _buf.ToArray();
				string[] command = _buf.Split(' ');
				
//				util.debugWriteLine(string.Join(" ", command));
				util.debugWriteLine(_buf);
				
				var ffrec = new FFMpegRecord(rm, true, rfu);
				ffrec.recordCommand(command);
		}
		private bool streamRenketuRecord(numTaskInfo info) {
			try {
				util.debugWriteLine(info.no + " " + info.url);
				util.debugWriteLine("record file path " + recFolderFile + ".ts");
				var w = new FileStream(recFolderFile + ".ts", FileMode.Append, FileAccess.Write);
				w.Write(info.res, 0, info.res.Length);
				w.Close();
				return true; 
			} catch (Exception e) {
				util.debugWriteLine(e.Message+e.StackTrace + e.Source + e.TargetSite);
				return false;
			}
		}
		/*
		public bool isStopRead() {
			var ret = DateTime.UtcNow - lastReadTime > new TimeSpan(0,0,30);
			if (ret) {
				var a = DateTime.UtcNow - lastReadTime;
				util.debugWriteLine(a);
			}
			return ret;
		}
		*/
		private void waitForRecording() {
			util.debugWriteLine("wait for recording");
			//isRetry = false;
			while(newGetTsTaskList.Count > 0) {
				Thread.Sleep(200);
			}

		}
		public void waitForEnd() {
			util.debugWriteLine("wait for rec end");
			isRetry = false;
			while(!isEnd) {
				Thread.Sleep(200);
			}

		}
		private string getHlsSegM3uUrl(string masterUrl) {
			util.debugWriteLine("master m3u8 " + masterUrl);
			var wc = new WebHeaderCollection();
			var res = util.getPageSource(masterUrl, ref wc, container);
			if (res == null) {
				isReConnecting = true;
				return null;
			}
			string segUrl = null;
			foreach (var s in res.Split('\n')) {
				if (s.StartsWith("#") || s.IndexOf(".m3u8") < 0) continue;
				segUrl = s;
				break;
			}
			
			var masterBaseUrl = util.getRegGroup(masterUrl, "(.+/)");
			baseUrl = util.getRegGroup(masterBaseUrl + segUrl, "(.+/)");
			util.debugWriteLine("master m3u8 res " + res);
			util.debugWriteLine("seg m3u8 " + 
				masterBaseUrl + segUrl);
			return masterBaseUrl + segUrl;
		}
		private void addNewTsTaskList(string hlsSegM3uUrl) {
			var wc = new WebHeaderCollection();
			var res = util.getPageSource(hlsSegM3uUrl, ref wc, container, null, false);
//			util.debugWriteLine("m3u8 " + res);
			if (res == null) {
				wr.reConnect();
				isReConnecting = true;
				return;
			}
			var second = 0.0;
			foreach (var s in res.Split('\n')) {
				var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
				if (_second != null)
					second = double.Parse(_second);
				
				if (s.IndexOf(".ts") < 0) continue;
				var no = int.Parse(util.getRegGroup(s, "(\\d+).ts"));
				var url = baseUrl + s;
				
				var isInList = false;
				foreach (var t in newGetTsTaskList)
					if (t.no == no) isInList = true;
				
				if (no > lastSegmentNo && !isInList) {
					var fileName = util.getRegGroup(s, "(.+?.ts)\\?");
					util.debugWriteLine(no + " " + fileName);
					
					newGetTsTaskList.Add(new numTaskInfo(no, url, second, fileName));
					Task.Run(() => getTsTask(url));
				}
			}
		}
		private void getTsTask(string url) {
			util.debugWriteLine("url " + url);
			byte[] tsBytes;
			try {
				tsBytes = util.getFileBytes(url, container);
				
				lock (newGetTsTaskList) {
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].url == url) {
							if (tsBytes == null) {
								isReConnecting = true;
								newGetTsTaskList.Clear();
								wr.reConnect();
								break;
							}
							newGetTsTaskList[i].res = tsBytes;
							recordedSecond += newGetTsTaskList[i].second;
							recordedBytes += tsBytes.Length;
						}
							
					}
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].res == null) break; 
						var ret = writeFile(newGetTsTaskList[i]);
						if (ret) {
							//recordedNo.Add(newGetTsTaskList[i].no.ToString());
							recordedNo.Add(newGetTsTaskList[i].fileName);
							lastSegmentNo = newGetTsTaskList[i].no;
						}
						
					}
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].res != null) 
							newGetTsTaskList.RemoveAt(i);
					}
					
				}
				setRecordState();
				
			} catch(Exception e) {
				util.debugWriteLine(e);
				util.debugWriteLine(e.Message + e.StackTrace + e.TargetSite);
				util.debugWriteLine(e.Message + e.StackTrace + e.TargetSite);
			}
			
		}
		private void setRecordState() {
			var ret = "";
			var bytes = recordedBytes;
			long b = bytes % 1000;
			long kb = (bytes % 1000000) / 1000;
			long mb = (bytes % 1000000000) / 1000000;
			long gb = (bytes % 1000000000000) / 1000000000;
			string _kb = ((int)(bytes / 1000)).ToString();
			for (var i = 0; i < 9 - _kb.Length; i++)
				_kb = " " + _kb;
			ret += "size=" + _kb + "kB";
			
//			recordedSecond = 400000;
			var dotSecond = ((int)((recordedSecond % 1) * 100)).ToString("00");
			var second = ((int)((recordedSecond % 60) * 1)).ToString("00");
			var minute = ((int)((recordedSecond % 3600 / 60))).ToString("00");
			var hour = ((int)((recordedSecond / 3600) * 1));
			var _hour = (hour < 100) ? hour.ToString("00") : hour.ToString();;
			var timeStr = _hour + ":" + minute + ":" + second + "." + dotSecond;
			ret += " time= " + timeStr;
			
			var bitrate = recordedBytes * 8 / recordedSecond / 1000;
			ret += " bitrate= " + bitrate.ToString("0.0") + "kbits/s";
			rm.form.setRecordState(ret);
			
		}
		private bool writeFile(numTaskInfo info) {
			if (segmentSaveType == 0) {
				return streamRenketuRecord(info);
			} else {
				return originalTsRecord(info);
			}
		}
		private bool originalTsRecord(numTaskInfo info) {
			var path = recFolderFile + "/" + 
				util.getRegGroup(info.url, ".+/(.+?)\\?");
			util.debugWriteLine("original ts record " + path);
			try {
				var w = new FileStream(path, FileMode.Create, FileAccess.Write);
				w.Write(info.res, 0, info.res.Length);
				w.Close();
				return true; 
			} catch (Exception e) {
				util.debugWriteLine("original ts record exception " + e.Message+e.StackTrace + e.Source + e.TargetSite);
				return false;
			}
		}
		private void timeShiftMultiRecord() {
			//var baseMasterUrl = util.getRegGroup(hlsMasterUrl, "(.+start=)");
			var baseMasterUrl = hlsMasterUrl + "&satrt=";
			var segUrl = getHlsSegM3uUrl(hlsMasterUrl);
			if (segUrl == null) {
				wr.reConnect();
				return;
			}
			util.debugWriteLine("timeshift basemaster " + baseMasterUrl + " segUrl " + segUrl);
			var wc = new WebHeaderCollection();
			var segRes = util.getPageSource(segUrl, ref wc, container);
			if (segRes == null) {
				wr.reConnect();
				return;
			}
			util.debugWriteLine("seg res " + segRes);
			var targetDuration = double.Parse(util.getRegGroup(segRes, "#EXT-X-TARGETDURATION:(\\d+)"));
			util.debugWriteLine("target duration " + targetDuration);
			
			var lastGetTime = -1.0;
			
			//var urls = new List<string>();
			
			while(true) {
				if (isReConnecting) {
					Thread.Sleep(1000);
					continue;
				}
				var _urls = getTimeshiftTSUrl(baseMasterUrl + (lastGetTime + targetDuration * 2));
				if (_urls == null) {
					isReConnecting = true;
					wr.reConnect();
					continue;
				}
				lastGetTime += targetDuration * 2;
				
				foreach(var u in _urls) {
					
				
					var ts = util.getFileBytes(u.url, container);
					
					var isInList = false;
					foreach (var t in newGetTsTaskList)
						if (t.no == u.no) isInList = true;
					
					
					if (u.no > lastSegmentNo && !isInList) {
						
						util.debugWriteLine(u.no + " " + u.fileName);
						
						newGetTsTaskList.Add(u);
						Task.Run(() => getTsTask(u.url));
					
					}
				}
			}
			//while(true) Thread.Sleep(1000);
//			var st = new FileStream("http", FileMode.Open);
//			byte[st.Length] a;
//			st.Read(a, 0, st.Length);
				
		}
		private List<numTaskInfo> getTimeshiftTSUrl(string url) {
			var ret = new List<numTaskInfo>();
			var segUrl = getHlsSegM3uUrl(url);
			if (segUrl == null) return null;
			var wc = new WebHeaderCollection();
			var segRes = util.getPageSource(segUrl, ref wc, container);
			if (segRes == null) return null;
			
			var second = 0.0;
			foreach (var s in segRes.Split('\n')) {
				var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
				if (_second != null)
					second = double.Parse(_second);
					
				if (s.IndexOf("#EXT-X-ENDLIST") > -1) {
					ret.Add(null);
					continue;
				}
				if (s.IndexOf(".ts?") < 0) continue;
				var no = int.Parse(util.getRegGroup(s, "(\\d+).ts"));
				var tsUrl = baseUrl + s;
				var fileName = util.getRegGroup(s, "(.+?.ts)\\?");
				ret.Add(new numTaskInfo(no, tsUrl, second, fileName));
			}
			return ret;
		}

		public void reSetHlsUrl(string url) {
			hlsMasterUrl = url;
			isReConnecting = false;
			hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
		}
		private void renketuAfter() {
			var isFFmpegRenketuAfter = false;
			if (isFFmpegRenketuAfter) {
				ffmpegRenketuAfter();
			} else {
				streamRenketuAfter();
			}
		}
		private void ffmpegRenketuAfter() {
			var fName = util.getRegGroup(recFolderFile, ".+/(.+)");
			var m3u8 = "#EXTM3U\n#EXT-X-VERSION:3\n#EXT-X-TARGETDURATION:60\n";
			foreach (var s in recordedNo) 
				//m3u8 += "#EXTINF:0\n" + recFolderFile + "/" + s + ".ts\n";
				m3u8 += "#EXTINF:0\n" + recFolderFile + "/" + s + "\n";
			m3u8 += "#EXT-X-ENDLIST\n";
			var pipeName = DateTime.UtcNow.Hour + "" + DateTime.UtcNow.Minute + "" + DateTime.UtcNow.Second;
			string args = "-i \\\\.\\pipe\\" + pipeName + " -c copy \"" + recFolderFile + "/" + fName + ".ts\"";
			
			var r = new FFMpegConcat(rm, rfu);
			r.recordCommand(args.Split(' '), m3u8, pipeName);
		}
		private void streamRenketuAfter() {
			var fName = util.getRegGroup(recFolderFile, ".+/(.+)");
			var outFName = recFolderFile + "/" + fName + ".ts";
			
			FileStream w; 
			try {
				util.debugWriteLine("renketu after out fname " + outFName);			
				w = new FileStream(outFName, FileMode.Append, FileAccess.Write);
			} catch (PathTooLongException e) {
				try {
					util.debugWriteLine("renketu after out fname " + recFolderFile + "/" + lvid + ".ts");			
					w = new FileStream(recFolderFile + "/" + lvid + ".ts", FileMode.Append, FileAccess.Write);
				} catch (PathTooLongException ee) {
					try {
						util.debugWriteLine("renketu after out fname " + recFolderFile + "/_.ts");			
						w = new FileStream(recFolderFile + "/_.ts", FileMode.Append, FileAccess.Write);
					} catch (PathTooLongException eee) {
						util.debugWriteLine("renketu after too long");
						rm.form.addLogText("録画後に連結しようとしましたがパスが長すぎてファイルが開けませんでした " + recFolderFile + "/_.ts");
						return;
					}
					
				}
			}
			
				
			foreach (var s in recordedNo) {
				util.debugWriteLine(s);
				try {
					var r = new FileStream(recFolderFile + "/" + s + "", FileMode.Open, FileAccess.Read);
					
					var pos = 0;
					var readI = 0;
					var bytes = new byte[1000000];
					while((readI = r.Read(bytes, 0, bytes.Length)) != 0) {
						w.Write(bytes, 0, readI);
						pos += readI;
					}
					r.Close();
				} catch (Exception e) {
					util.debugWriteLine("renketu after write exception " + s + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			w.Close();
			
		}
		private void timeShiftOnTimeRecord() {
			var startTime = getTSStartTime();
			return;
			var baseMasterUrl = hlsMasterUrl + "&start=" + startTime;
			var segUrl = getHlsSegM3uUrl(baseMasterUrl);
			if (segUrl == null) {
				wr.reConnect();
				return;
			}
			util.debugWriteLine("timeshift basemaster " + baseMasterUrl + " segUrl " + segUrl);
			var wc = new WebHeaderCollection();
			var segRes = util.getPageSource(segUrl, ref wc, container);
			if (segRes == null) {
				wr.reConnect();
				return;
			}
			util.debugWriteLine("seg res " + segRes);
			var i = 8;
		}
		private string getTSStartTime() {
			var r = new FileStream("48498.ts", FileMode.Open, FileAccess.Read);
			var w = new FileStream("48498_henshuu.ts", FileMode.OpenOrCreate, FileAccess.ReadWrite);
			w.Position = w.Length - 192;
			var b = new byte[300];
			w.Read(b, 0, 200);
			//foreach (var _b in b) 
			util.debugWriteLine(BitConverter.ToString(b));
			w.Close();
			r.Close();
			return null;
		}
	}
	class numTaskInfo {
		public int no = -1;
		public string url = null;
		public byte[] res = null;
		public double second = 0;
		public string fileName = null;
		public numTaskInfo(int no, string url, double second, string fileName) {
			this.no = no;
			this.url = url;
			this.second = second;
			this.fileName = fileName;
		}
	}
		
}
