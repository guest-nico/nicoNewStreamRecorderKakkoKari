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

using WebSocket4Net;
using namaichi.info;

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
		private long _openTime;
		private int lastSegmentNo;
		private int lastAccessingSegmentNo;
		private CookieContainer container;
		private int segmentSaveType = 0;
		private bool isTimeShift = false;
		private TimeShiftConfig tsConfig = null;
		private List<numTaskInfo> newGetTsTaskList = new List<numTaskInfo>();
		private List<string> recordedNo = new List<string>();
		private string baseUrl;
		private IRecorderProcess wr;
		private bool isReConnecting = false;
		private bool isRetry = true;
		private bool isEnd = false;
		private string hlsSegM3uUrl;
		private double recordedSecond = 0;
		private long recordedBytes = 0;
		private int lastRecordedSeconds = -1;
		private double lastFileSecond = 0;
		private int gotTsMaxNo = 0;
		public bool isEndProgram = false;
		private double allDuration = -1;
		private bool isDefaultEngine = true;
		private string anotherEngineCommand = ""; 
		private Html5Recorder h5r;
		private double targetDuration = 2;
		private object recordLock = new object();
		private List<string> segM3u8List = new List<string>();
		private List<numTaskInfo> gotTsList = new List<numTaskInfo>();
		private Task m3u8GetterTask = null;
		private Task tsGetterTask = null;
		private Task tsWriterTask = null;
		private List<string> debugWriteBuf = new List<string>();
		private bool isPlayOnlyMode;
		private OutputTimeShiftTsUrlList otstul;
		
		private string recordingQuality = null;
		private WebSocket ws;
		
		public Record(RecordingManager rm, bool isFFmpeg, 
		              RecordFromUrl rfu, string hlsUrl, 
		              string recFolderFile, int lastSegmentNo, 
		              CookieContainer container, bool isTimeShift, 
		              IRecorderProcess wr, string lvid, 
		              TimeShiftConfig tsConfig, long _openTime,
		              WebSocket ws) {
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
			this.tsConfig = tsConfig;
			isDefaultEngine = bool.Parse(rm.cfg.get("IsDefaultEngine"));
			if (rm.isPlayOnlyMode) isDefaultEngine = true;
			anotherEngineCommand = rm.cfg.get("anotherEngineCommand");
			targetDuration = (isTimeShift) ? 5 : 2;
			
			rm.isTitleBarInfo = bool.Parse(rm.cfg.get("IstitlebarInfo"));
			isPlayOnlyMode = rm.isPlayOnlyMode;
			this._openTime = _openTime;
			this.ws = ws;
		}
		public void record(string quality) {
			recordingQuality = quality;
			tsWriterTask = Task.Run(() => {startDebugWriter();});
			
			if (isTimeShift) {
				rm.form.addLogText("タイムシフトの録画を開始します(画質:" + quality + ")");
//				timeShiftMultiRecord();
				timeShiftOnTimeRecord();
			} else {
				rm.form.addLogText("録画を開始します(画質:" + quality + ")");
				realTimeRecord();
			}
			
		}
		private void realTimeRecord() {
			hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
			rm.hlsUrl = hlsSegM3uUrl;
			rm.form.setPlayerBtnEnable(true);
			
			if (isDefaultEngine) {
				m3u8GetterTask = Task.Run(() => {startM3u8Getter();});
				tsGetterTask = Task.Run(() => {startTsGetter();});
				tsWriterTask = Task.Run(() => {startTsWriter();});
				
			}
//			Task.Run(() => {syncCheck();});
			
			
			while (rm.rfu == rfu && isRetry) {
				if (isReConnecting) {
					Thread.Sleep(100);
					
					
					continue;
				}
				//if (hlsSegM3uUrl == null && !isDefaultEngine) {
				if (hlsSegM3uUrl == null) {
					addDebugBuf("hlsSegM3u8 url null reconnect");
					if (!isReConnecting) reConnect(); 
					setReconnecting(true);
					continue;
				}
				
				if (hlsSegM3uUrl.IndexOf("start_time") > -1) {
					addDebugBuf("got timeshift playlist");
					break;
				}
				
				if (isDefaultEngine) {
					Thread.Sleep(500);
					
				} else {
					var aer = new AnotherEngineRecorder(rm, rfu);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					recFolderFile = wr.getRecFilePath(_openTime)[1];
					setReconnecting(true);
//					if (!isReConnecting) 
						reConnect();
					
					continue;
					
				}
			}
			rm.hlsUrl = "end";
//			rm.form.setPlayerBtnEnable(false);
			
			if (isDefaultEngine && !isPlayOnlyMode) {
				waitForRecording();
				if (isEndProgram)
					rm.form.addLogText("録画を完了しました");
				if (segmentSaveType == 1 && 
				    	(rm.cfg.get("IsRenketuAfter") == "true" || rm.ri != null)) {
					util.debugWriteLine("renketu after");
					renketuAfter();
				} else if (segmentSaveType == 0) {
					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || rm.ri != null) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ".ts");
					}
				}
			}
				
			
			isEnd = true;
		}
		private void startM3u8Getter() {
			while (rm.rfu == rfu && isRetry) {
//				util.debugWriteLine("gc count " + GC.CollectionCount(0) + " " + GC.CollectionCount(1) + " " + GC.CollectionCount(2) + " " + GC.CollectionCount(3));
				//util.debugWriteLine("isreconnecting " + isReConnecting);
				addDebugBuf("isreconnecting " + isReConnecting);

				if (isReConnecting) {
					Thread.Sleep(100);
				
					continue;
				}
				addDebugBuf("m3u8 getter 0 " + hlsSegM3uUrl);
				//util.debugWriteLine("m3u8 getter 0 " + hlsSegM3uUrl);				
				
				if (hlsSegM3uUrl == null) {
					addDebugBuf("m3u8 getter hlssegm3u8 url null reconnect");
					if (!isReConnecting) reConnect(); 
					setReconnecting(true);
					continue;
				}
				
				if (hlsSegM3uUrl.IndexOf("start_time") > -1) {
					//util.debugWriteLine("got timeshift playlist");
					addDebugBuf("got timeshift playlist");
					break;
				}
				//util.debugWriteLine("getpage m3u8 mae");
				addDebugBuf("getpage m3u8 mae");
				var res = util.getPageSource(hlsSegM3uUrl, container, null, false, 2000);
				if (res == null) {
					addDebugBuf("m3u8 getter segM3u8List res null reconnect");
					setReconnecting(true);
	//				if (!isReConnecting) 
						reConnect();
					
					continue;
				}
				//util.debugWriteLine(res);
				addDebugBuf(res);
				var isTimeShiftPlaylist = util.getRegGroup(res, "(#STREAM-DURATION)") != null;
				if (!isTimeShift && isTimeShiftPlaylist) {
					isRetry = false;
					return;
					//return -1;
				}
				
				var addeddFutureList = getAddedNearList(res);
				
				//util.debugWriteLine("timeshift check go");
				addDebugBuf("timeshift check go");
				//lock (segM3u8List) {
					//segM3u8List.Add(res);
					segM3u8List.Add(addeddFutureList);
				//}
				//util.debugWriteLine("seg m3u8 add");
				addDebugBuf("seg m3u8 add");
				
				var _targetDuration = util.getRegGroup(res, "#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*)");
				if (_targetDuration != null) {
					targetDuration = double.Parse(_targetDuration);
				}
				//util.debugWriteLine("sleep mae");
				addDebugBuf("sleep mae");
				Thread.Sleep((int)(targetDuration * 500));
				//util.debugWriteLine("targetduration " + targetDuration);
				addDebugBuf("targetduration " + targetDuration);
			}
		}
		private void startTsGetter() {
			while (true) {
				var isM3u8GetterEnd = (m3u8GetterTask.IsCanceled ||
						m3u8GetterTask.IsCompleted ||
						m3u8GetterTask.IsFaulted);
				if (isM3u8GetterEnd && segM3u8List.Count == 0) break;
				

				foreach (var _s in new List<string>(segM3u8List)) {
					var baseTime = getBaseTimeFromPlaylist(_s);
					var second = 0.0;
					var secondSum = 0.0;
					var targetDuration = 2.0;
//					lock(recordLock) {
						foreach (var s in _s.Split('\n')) {
							//var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
							var _second = util.getRegGroup(s, "^#EXTINF:(.+),");
							if (_second != null) {
								second = double.Parse(_second, System.Globalization.NumberStyles.Float);
								secondSum += second;
							}
							var _targetDuration = util.getRegGroup(s, "^#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*)");
							if (_targetDuration != null) {
								targetDuration = double.Parse(_targetDuration);
							}
							var _endList = util.getRegGroup(s, "^(#EXT-X-ENDLIST)$");
							if (_endList != null) {
								isRetry = false;
								isEndProgram = true;
							}
							var _allDuration = util.getRegGroup(s, "^#STREAM-DURATION:(.+)");
							if (_allDuration != null) {
								allDuration = double.Parse(_allDuration, System.Globalization.NumberStyles.Float);
							}
							
							if (s.IndexOf(".ts") < 0) continue;
							var no = int.Parse(util.getRegGroup(s, "(\\d+).ts"));
							var url = baseUrl + s;
							
							var isInList = false;
							lock (newGetTsTaskList) {
								foreach (var t in newGetTsTaskList)
									if (t.no == no) isInList = true;
							}
							
							var startTime = baseTime + secondSum - second;
							if (isTimeShift && 
							    	((tsConfig.timeType == 0 && startTime < tsConfig.timeSeconds) ||
							     	(tsConfig.timeType == 1 && startTime <= tsConfig.timeSeconds))) continue;
							var startTimeStr = util.getSecondsToStr(startTime);
							
							if (no > lastSegmentNo && !isInList && no > gotTsMaxNo) {
								var fileName = util.getRegGroup(s, "(.+?.ts)\\?");
								//fileName = util.getRegGroup(fileName, "(\\d+)") + ".ts";
								fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ".ts";
								addDebugBuf(no + fileName);
								//util.debugWriteLine(no + " " + fileName);
								var nti = new numTaskInfo(no, url, second, fileName);
								//newGetTsTaskList.Add(new numTaskInfo(no, url, second, fileName));
								//Task.Run(() => getTsTask(url, startTime));
								//getTsTask(url, startTime);
								byte[] tsBytes;
								addDebugBuf("getfilebyte " + url);
								tsBytes = util.getFileBytes(url, container);
								if (tsBytes == null) {
									addDebugBuf("tsBytes null " + url);
//									if (!isReConnecting) {
//										setReconnecting(true);
//										reConnect();
//									}
									segM3u8List.Clear();
									continue;
								}
								addDebugBuf("tsBytes get ok " + url);
								nti.res = tsBytes;
								gotTsList.Add(nti);
								recordedSecond += nti.second;
								recordedBytes += tsBytes.Length;
								lastFileSecond = nti.second;
								if (nti.no > gotTsMaxNo) gotTsMaxNo = nti.no;
							
							}
							segM3u8List.Remove(_s);
						}
//					}
			
				}
//				segM3u8List.Clear();
				Thread.Sleep(300);
			}
		}
		
		
		
		
		private void startTsWriter() {
			while (true) {
				var isTsGetterEnd = (tsGetterTask.IsCanceled ||
						tsGetterTask.IsCompleted ||
						tsGetterTask.IsFaulted);
				if (isTsGetterEnd && gotTsList.Count == 0) break;
				
				var count = gotTsList.Count;
				foreach (var s in new List<numTaskInfo>(gotTsList)) {
					addDebugBuf("s.no " + s.no.ToString() + " lastsegmentno " + lastSegmentNo.ToString());
					if (s.no <= lastSegmentNo) {
						gotTsList.Remove(s);
						continue;
					}
					bool ret;
					if (isPlayOnlyMode) ret = true;
					else ret = writeFile(s);
					//util.debugWriteLine("write ok " + s.no);
					addDebugBuf("write ok " + s.no);
					if (ret) {
						//recordedNo.Add(newGetTsTaskList[i].no.ToString());
						recordedNo.Add(s.fileName);
						lastSegmentNo = s.no;
						var fName = util.getRegGroup(s.fileName, ".*(\\\\|/|^)(.+)", 2);
//							if (fName == 
						lastRecordedSeconds = util.getSecondsFromStr(fName);
					}
					gotTsList.Remove(s);
				}
				if (count > 0 && !isPlayOnlyMode) setRecordState();
//				gotTsList.Clear();
				
				Thread.Sleep(300);
			}
		}
		private void syncCheck() {
			while (!isEnd) {
				if (DateTime.Now - lastsync > TimeSpan.FromSeconds(15)) {
				         	if (!wr.isJikken) {
				         		var url = hlsSegM3uUrl.Replace("ts/playlist.m3u8", "stream_sync.json");
				         		util.debugWriteLine(util.getPageSource(url, container, null, false, 2000));
							}

					lastsync = DateTime.Now;
				}
				Thread.Sleep(5);
			}
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
				addDebugBuf(info.no + " " + info.url);
				addDebugBuf("record file path " + recFolderFile + ".ts");
				
				//file lock check
				if (File.Exists(recFolderFile + ".ts"))
					File.Move(recFolderFile + ".ts", recFolderFile + ".ts");
				
				var w = new FileStream(recFolderFile + ".ts", FileMode.Append, FileAccess.Write);
				w.Write(info.res, 0, info.res.Length);
				w.Close();
				if (isTimeShift) {
					var newName = newTimeShiftFileName(recFolderFile, info.fileName);
					File.Move(recFolderFile + ".ts", newName + ".ts");
					recFolderFile = newName;
				}
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
			while(true) {
				if (tsWriterTask.IsCanceled || 
				    	tsWriterTask.IsCompleted ||
				    	tsWriterTask.IsFaulted) break;
				Thread.Sleep(200);
				/*
				lock (newGetTsTaskList) {
					var isAllEnd = true;
					foreach (var n in newGetTsTaskList) {
						if (n.res == null) isAllEnd = false;
					}
					if (isAllEnd) break;
				}
				*/
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
				setReconnecting(true);
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
		DateTime lastsync = DateTime.Now;
		private double addNewTsTaskList(string hlsSegM3uUrl) {
			addDebugBuf("addNewTsTaskList");
//			var wc = new WebHeaderCollection();

			addDebugBuf("getpage mae");
			
			var res = util.getPageSource(hlsSegM3uUrl, container, null, false, 2000);
			addDebugBuf("addNewTsTaskList segm3u get " + res);
//			util.debugWriteLine("m3u8 " + res);
			
			if (otstul != null && !otstul.isStarted) outputTimeShiftTsUrlList(res);
			
			//shuusei? 
			if (res == null || (lastSegmentNo != -1 && res.IndexOf(lastSegmentNo.ToString()) == -1)) {
			//if (res == null) {
				util.debugWriteLine("nuke? lastSegmentNo " + lastSegmentNo + " res " + res);
				setReconnecting(true);
//				if (!isReConnecting) 
					reConnect();
				
				return 1.0;
			}
			var isTimeShiftPlaylist = util.getRegGroup(res, "(#STREAM-DURATION)") != null;
			if (!isTimeShift && isTimeShiftPlaylist) {
				return -1;
			}
			
			
			var baseTime = getBaseTimeFromPlaylist(res);
			var second = 0.0;
			var secondSum = 0.0;
			var targetDuration = 2.0;
			lock(recordLock) {
				foreach (var s in res.Split('\n')) {
					var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
					if (_second != null) {
						second = double.Parse(_second);
						secondSum += second;
					}
					var _targetDuration = util.getRegGroup(s, "^#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*)");
					if (_targetDuration != null) {
						targetDuration = double.Parse(_targetDuration);
					}
					var _endList = util.getRegGroup(s, "^(#EXT-X-ENDLIST)$");
					if (_endList != null) {
						isRetry = false;
						isEndProgram = true;
					}
					var _allDuration = util.getRegGroup(s, "^#STREAM-DURATION:(\\d+)");
					if (_allDuration != null) {
						allDuration = double.Parse(_allDuration);
					}
					
					if (s.IndexOf(".ts") < 0) continue;
					//var no = int.Parse(util.getRegGroup(s, "(\\d+).ts"));
					var no = int.Parse(util.getRegGroup(s, "(\\d+)"));
					var url = baseUrl + s;
					
					var isInList = false;
					lock (newGetTsTaskList) {
						foreach (var t in newGetTsTaskList)
							if (t.no == no) isInList = true;
					}
					
					var startTime = baseTime + secondSum - second;
					if (isTimeShift && 
					    	((tsConfig.timeType == 0 && startTime < tsConfig.timeSeconds) ||
					     	(tsConfig.timeType == 1 && startTime <= tsConfig.timeSeconds))) continue;
					if (isTimeShift && tsConfig.endTimeSeconds != 0 && startTime > tsConfig.endTimeSeconds) {
						isRetry = false;
						isEndProgram = true;
						continue;
					}
					var startTimeStr = util.getSecondsToStr(startTime);
					
					if (no > lastSegmentNo && !isInList) {
						var fileName = util.getRegGroup(s, "(.+?.ts)\\?");
						//fileName = util.getRegGroup(fileName, "(\\d+)") + ".ts";
						fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ".ts";
						addDebugBuf(no + " " + fileName);
						
						newGetTsTaskList.Add(new numTaskInfo(no, url, second, fileName));
						//Task.Run(() => getTsTask(url, startTime));
						getTsTask(url, startTime);
					}
				}
			}
			return targetDuration;
		}
		private void getTsTask(string url, double startTime) {
			addDebugBuf("url " + url);
			byte[] tsBytes;
			try {
				tsBytes = util.getFileBytes(url, container);
				addDebugBuf("getfilebytes did url " + url + " " + tsBytes);
				
				lock (newGetTsTaskList) {
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].url == url) {
							if (tsBytes == null) {
								newGetTsTaskList.Clear();
								if (!isReConnecting) reConnect();
								setReconnecting(true);
								break;
							}
							newGetTsTaskList[i].res = tsBytes;
							recordedSecond += newGetTsTaskList[i].second;
							recordedBytes += tsBytes.Length;
						}
							
					}
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						addDebugBuf("write file " + i + " url " + newGetTsTaskList[i].no);
						if (newGetTsTaskList[i].res == null) break;
						
						if (newGetTsTaskList[i].no <= lastSegmentNo) {
							continue;
						}
						bool ret;
						if (isPlayOnlyMode) ret = true;
						else ret = writeFile(newGetTsTaskList[i]);

						addDebugBuf("write ok " + newGetTsTaskList[i].no);
						if (ret) {
							//recordedNo.Add(newGetTsTaskList[i].no.ToString());
							recordedNo.Add(newGetTsTaskList[i].fileName);
							lastSegmentNo = newGetTsTaskList[i].no;
							var fName = util.getRegGroup(newGetTsTaskList[i].fileName, ".*(\\\\|/|^)(.+)", 2);
//							if (fName == 
							lastRecordedSeconds = util.getSecondsFromStr(fName);
						} else {
							newGetTsTaskList.Clear();
							if (!isReConnecting) reConnect();
							setReconnecting(true);
							break;
						}
						
					}
					addDebugBuf("write ok2");
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].res != null) 
							newGetTsTaskList.RemoveAt(i);
					}
					
				}
				if (!isPlayOnlyMode)
					setRecordState();
				/*
				if (isTimeShift) {
					var keika = util.getSecondsToKeikaJikan(startTime);
					if (allDuration != -1) keika += "\n/" + util.getSecondsToKeikaJikan(allDuration);
					rm.form.setKeikaJikan(keika, "a");
				}
				*/
				
			} catch(Exception e) {
				util.debugWriteLine(e);
				util.debugWriteLine(e.Message + e.StackTrace + e.TargetSite);
				util.debugWriteLine(e.Message + e.StackTrace + e.TargetSite);
			}
			
		}
		private void setRecordState() {
			addDebugBuf("setRecordState");
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
//				util.getRegGroup(info.url, ".+/(.+?)\\?");
				info.fileName;
			addDebugBuf("original ts record " + path);
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
			var baseMasterUrl = hlsMasterUrl + "&start=";
//			baseMasterUrl = hlsMasterUrl + "";
			
			var segUrl = getHlsSegM3uUrl(hlsMasterUrl);
			if (segUrl == null) {
				if (!isReConnecting) reConnect();
				setReconnecting(true);
				return;
			}
			util.debugWriteLine("timeshift basemaster " + baseMasterUrl + " segUrl " + segUrl);
			var wc = new WebHeaderCollection();
			var segRes = util.getPageSource(segUrl, ref wc, container);
			if (segRes == null) {
				if (!isReConnecting) reConnect();
				setReconnecting(true);
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
				var _urls = getTimeshiftTSUrl(baseMasterUrl + (lastGetTime + targetDuration * 1));
				
				if (_urls == null) {
					if (!isReConnecting) reConnect();
					setReconnecting(true);
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
						//Task.Run(() => getTsTask(u.url, 0));
						getTsTask(u.url, 0);
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
					isEnd = true;
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

		public void reSetHlsUrl(string url, string quality, WebSocket _ws) {
			ws = _ws;
			if (recordingQuality != quality)
				rm.form.addLogText("画質を変更して再接続します(" + quality + ")");
			recordingQuality = quality;
			
			hlsMasterUrl = url;
			if (isTimeShift) {
				var start = 0;
				if (lastRecordedSeconds == -1) {
					start = (tsConfig.timeSeconds - 10 < 0) ? 0 : (tsConfig.timeSeconds - 10);
				} else {
					start = ((int)lastRecordedSeconds - 10 < 0) ? 0 : ((int)lastRecordedSeconds - 10);
				}
				hlsMasterUrl = hlsMasterUrl + "&start=" + (start.ToString());
				wr.tsHlsRequestTime = DateTime.Now;
				wr.tsStartTime = TimeSpan.FromSeconds((double)start);
			}
			hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
			
			setReconnecting(false);
		}
		private void renketuAfter() {
			var isFFmpegRenketuAfter = false;
			if (isFFmpegRenketuAfter) {
				ffmpegRenketuAfter();
			} else {
				rm.form.addLogText("連結処理を開始します");
				var outFName = streamRenketuAfter();
				rm.form.addLogText("連結処理を完了しました");
				
				if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || rm.ri != null) {
					var tf = new ThroughFFMpeg(rm);
					tf.start(outFName);
					
				}
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
		private string streamRenketuAfter() {
			var fName = util.getRegGroup(recFolderFile, ".+/(.+)");
			var outFName = recFolderFile + "/" + fName + ".ts";
			
			FileStream w; 
			try {
				util.debugWriteLine("renketu after out fname " + outFName);			
				if (outFName.Length > 245) outFName = recFolderFile + "/" + lvid + ".ts";
				if (outFName.Length > 245) outFName = recFolderFile + "/out.ts";
				util.debugWriteLine("renketu after out fname shuusei go " + outFName);
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
						return null;
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
			return w.Name;
		}
		private void timeShiftOnTimeRecord() {
			if (tsConfig.isOutputUrlList) {
				setOutputTimeShiftTsUrlListStartTime();
			}
			
			var start = (tsConfig.timeSeconds - 10 < 0) ? 0 : (tsConfig.timeSeconds - 10);
			var baseMasterUrl = hlsMasterUrl + "&start=" + (start.ToString());
			wr.tsHlsRequestTime = DateTime.Now;
			wr.tsStartTime = TimeSpan.FromSeconds((double)start);
			hlsSegM3uUrl = getHlsSegM3uUrl(baseMasterUrl);
			rm.hlsUrl = hlsSegM3uUrl;
			rm.form.setPlayerBtnEnable(true);
			
			while (rm.rfu == rfu && isRetry) {
				if (isReConnecting) {
					Thread.Sleep(100);
					continue;
				}
				if (hlsSegM3uUrl == null) {
					addDebugBuf("hlsSegM3uUrl null reconnect");
					setReconnecting(true);
					reConnect();
					continue;
				}
				
				if (isDefaultEngine) {

					    	targetDuration = addNewTsTaskList(hlsSegM3uUrl);

					
					Thread.Sleep((int)(targetDuration * 500));
				} else {
					var aer = new AnotherEngineRecorder(rm, rfu);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					var a = new System.Net.WebHeaderCollection();
					var _resM3u8 = util.getPageSource(hlsSegM3uUrl, ref a, container);
					if (_resM3u8 != null && _resM3u8.IndexOf("#EXT-X-ENDLIST") > -1) {
						isEndProgram = true;
						break;
					}
					                   
					recFolderFile = wr.getRecFilePath(_openTime)[1];
					 
					setReconnecting(true);
					reConnect();
					continue;
					
				}
				
			}
			rm.hlsUrl = "end";
//			rm.form.setPlayerBtnEnable(false);
			
			if (isDefaultEngine && !isPlayOnlyMode) {
				if (isEndProgram)
					rm.form.addLogText("録画を完了しました");
				if (segmentSaveType == 1 && 
				    	(rm.cfg.get("IsRenketuAfter") == "true" || rm.ri != null)) {
					util.debugWriteLine("renketu after");
					renketuAfter();
				} else if (segmentSaveType == 0) {
					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || rm.ri != null) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ".ts");
						
					}
				}
			}
			isEnd = true;
		}
		private double getBaseTimeFromPlaylist(string res) {
			//most extinf second
			var mostSegmentSecond = getMostSegmentSecond(res);
			
			var mediaSequenceNum = util.getRegGroup(res, "#EXT-X-MEDIA-SEQUENCE\\:(.+)");
			if (mediaSequenceNum == null) return -1;
			return mostSegmentSecond * double.Parse(mediaSequenceNum);
		}
		private double getMostSegmentSecond(string res) {
			var timeArr = new List<double[]>();
			foreach (var l in res.Split('\n')) {
				var _second = util.getRegGroup(l, "^#EXTINF:(\\d+(\\.\\d+)*)");
				if (_second == null) continue;
				var inKey = false;
				for (var i = 0; i < timeArr.Count; i++) {
					if (timeArr[i][0] == double.Parse(_second)) {
						timeArr[i][1] += 1;
						inKey = true;
					}
				}
				if (!inKey) timeArr.Add(new double[2]{double.Parse(_second), 1});
			}
			if (timeArr.Count == 0) return -1;
			
			var maxKey = 0;
			for (var j = 0; j < timeArr.Count; j++) {
				if (timeArr[j][1] > timeArr[maxKey][1])
					maxKey = j;
			}
			return timeArr[maxKey][0];
		}
		private string newTimeShiftFileName(string nowName, string newInfoName) {
			var newFileTime = util.getRegGroup(newInfoName, "(\\d+h\\d+m\\d+s)");
			var nowFileTime = util.getRegGroup(nowName, "(\\d+h\\d+m\\d+s)");
			return nowName.Replace(nowFileTime, newFileTime);
		}
		private void setReconnecting(bool b) {
			isReConnecting = b;
			if (isReConnecting) {
				rm.hlsUrl = "reconnecting";
//				rm.form.setPlayerBtnEnable(false);
			}
			else {
				rm.hlsUrl = hlsSegM3uUrl;
//				rm.form.setPlayerBtnEnable(true);
			}
		}
		private void startDebugWriter() {
			while (rm.rfu == rfu && isRetry) {
				var l = new List<string>(debugWriteBuf);
				foreach (var b in l) {
					util.debugWriteLine(b);
					debugWriteBuf.Remove(b);
				}
				Thread.Sleep(500);
			}
		}
		private void addDebugBuf(string s) {
			var dt = DateTime.Now.ToLongTimeString();
			debugWriteBuf.Add(dt + " " + s);
		}
		
		//debug
		private int lastGetPlayListMaxNo = 0;
		
		private string getAddedNearList(string res) {
			if (res.IndexOf("#EXT-X-ENDLIST") != -1) return res;
			
			//if (lastSegmentNo == 0 || lastFileSecond == 0) {
			if (lastSegmentNo == 0 && false) {
				return res;
			}
			var maxNo = 0;
			var maxLine = "";
			var minNo = 0;
			foreach (var l in res.Split('\n')) {
				if (l.IndexOf(".ts") != -1) {
					maxNo = int.Parse(util.getRegGroup(l, "(\\d+)"));
					maxLine = l;
					if (minNo == 0) minNo = maxNo; 
				}
			}
			//if (lastGetPlayListMaxNo != 0 && minNo > lastGetPlayListMaxNo) {
			addDebugBuf("minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString());
			//if (minNo > lastGetPlayListMaxNo) {
			if (minNo > gotTsMaxNo) {
				addDebugBuf("nuke? minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString());
				var inf = getMostSegmentSecond(res);
				var ins = "";
				var startNo = (minNo - gotTsMaxNo > 10) ? (minNo - 10) : (gotTsMaxNo);
				for (var i = startNo; i < minNo; i++) {
					ins += "#EXTINF:" + inf + ",\n";
					ins += maxLine.Replace(maxNo.ToString(), i.ToString()) + "\n";
				}
				res = res.Insert(res.IndexOf("EXTINF:"), ins);
				addDebugBuf("added list " + res);
			}
			lastGetPlayListMaxNo = maxNo;
			
			/*
			for (var i = maxNo + 1; i < gotTsMaxNo + 2; i++) {
				res += "#EXTINF:" + lastFileSecond + ",\n";
				res += maxLine.Replace(maxNo.ToString(), i.ToString()) + "\n";
			}
			*/
			addDebugBuf("added list " + res);
			
			return res;
		}
		private void setOutputTimeShiftTsUrlListStartTime() {
			var baseMasterUrl = hlsMasterUrl + "&start=0";
			var _hlsSegM3uUrl = getHlsSegM3uUrl(baseMasterUrl);
			var res = util.getPageSource(_hlsSegM3uUrl, container, null, false, 2000);
			
			otstul = new OutputTimeShiftTsUrlList(tsConfig, rm);
			otstul.setStartNum(res);
		}
		private void outputTimeShiftTsUrlList(string res) {
			Task.Run(() => {
				otstul.write(res, recFolderFile, baseUrl, tsConfig);
			});
		}
		private void reConnect() {
			if (wr.isJikken) {
				wr.reConnect();
			} else {
				((WebSocketRecorder)wr).reConnect(ws);
			}
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
