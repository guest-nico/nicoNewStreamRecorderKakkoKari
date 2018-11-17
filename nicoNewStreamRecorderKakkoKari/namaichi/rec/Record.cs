/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/17
 * Time: 20:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
<<<<<<< HEAD
using System.Globalization;
=======
<<<<<<< HEAD
using System.Globalization;
=======
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
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
		private int lastSegmentNo = -1;
		private DateTime lastWroteSegmentDt = DateTime.MinValue;
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
		private int gotTsMaxNo = -1;
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
		private string recFolderFileOrigin;
		private bool isSub;
		private int baseNo = 0;
		
		public Record(RecordingManager rm, bool isFFmpeg, 
		              RecordFromUrl rfu, string hlsUrl, 
		              string recFolderFile, int lastSegmentNo, 
		              CookieContainer container, bool isTimeShift, 
		              IRecorderProcess wr, string lvid, 
		              TimeShiftConfig tsConfig, long _openTime,
		              WebSocket ws, string recFolderFileOrigin, bool isSub) {
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
			this.recFolderFileOrigin = recFolderFileOrigin;
			this.isSub = isSub;
		}
		public void record(string quality) {
			recordingQuality = quality;
			tsWriterTask = Task.Run(() => {startDebugWriter();});
			
			var _m = (isPlayOnlyMode) ? "視聴" : "録画";
			if (isTimeShift) {
				rm.form.addLogText("タイムシフトの" + _m + "を開始します(画質:" + quality + ")");
//				timeShiftMultiRecord();
				timeShiftOnTimeRecord();
			} else {
<<<<<<< HEAD
				if (isSub) {
					rm.form.addLogText(_m + "をスタンバイします(画質:" + quality + " " + "サブ)");
				} else
					rm.form.addLogText(_m + "を開始します(画質:" + quality + " " + "メイン)");
=======
				if (isSub)
					rm.form.addLogText("録画をスタンバイします(画質:" + quality + " " + "サブ)");
				else 
					rm.form.addLogText("録画を開始します(画質:" + quality + " " + "メイン)");
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
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
				if (!isSub)
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
			if (isSub) {
				isEnd = true;
				return;
			}
			
			rm.hlsUrl = "end";
//			rm.form.setPlayerBtnEnable(false);

			
			
			
			if (isDefaultEngine && !isPlayOnlyMode) {
<<<<<<< HEAD
				addDebugBuf("rec end shori gottslist count " + gotTsList.Count);
				if (rfu.subGotNumTaskInfo != null)
					addDebugBuf("subgot " + rfu.subGotNumTaskInfo.Count);
=======
				addDebugBuf("rec end shori gottslist count " + gotTsList.Count + " subgot " + rfu.subGotNumTaskInfo.Count);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				if (gotTsList.Count > 10) {
					displayWriteRemainGotTsData();
					
				}
				waitForRecording();
				
				if (rfu.subGotNumTaskInfo != null && rfu.subGotNumTaskInfo.Count > 20) {
					rm.form.addLogText("抜けセグメントの補完を試みます");
					var dsp = new DropSegmentProcess(lastWroteSegmentDt, lastSegmentNo, this, recFolderFileOrigin, rfu, rm);
					dsp.writeRemaining();
				}
				
				if (isEndProgram)
					rm.form.addLogText("録画を完了しました");
				if (segmentSaveType == 1 && 
				    	(rm.cfg.get("IsRenketuAfter") == "true" ||
				     int.Parse(rm.cfg.get("afterConvertMode")) != 0)) {
					addDebugBuf("renketu after");
					renketuAfter();
				} else if (segmentSaveType == 0) {
					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || 
					    int.Parse(rm.cfg.get("afterConvertMode")) != 0) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ".ts", true);
					}
				}
				rm.form.addLogText("録画終了処理を完了しました");
			}
				
			
			isEnd = true;
		}
		private void startM3u8Getter() {
			while (rm.rfu == rfu && isRetry) {
//				util.debugWriteLine("gc count " + GC.CollectionCount(0) + " " + GC.CollectionCount(1) + " " + GC.CollectionCount(2) + " " + GC.CollectionCount(3));
				//util.debugWriteLine("isreconnecting " + isReConnecting);
				addDebugBuf("isreconnecting " + isReConnecting);

				if (isReConnecting) {
					Thread.Sleep(500);
				
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
					isEndProgram = true;
					isRetry = false;
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
				var isTimeShiftPlaylist = res.IndexOf("#STREAM-DURATION") > -1;
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
				if (!isPlayOnlyMode)
					segM3u8List.Add(addeddFutureList);
				//}
				//util.debugWriteLine("seg m3u8 add");
				addDebugBuf("seg m3u8 add");
				
				var _targetDuration = util.getRegGroup(res, "#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*)", 1, rm.regGetter.getExtXTargetDuration());
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
<<<<<<< HEAD
//				addDebugBuf("tsGetter loop segM3u8List count " + segM3u8List.Count);
=======
				addDebugBuf("tsGetter loop segM3u8List count " + segM3u8List.Count);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				var isM3u8GetterEnd = (m3u8GetterTask.IsCanceled ||
						m3u8GetterTask.IsCompleted ||
						m3u8GetterTask.IsFaulted);
				if (isM3u8GetterEnd && segM3u8List.Count == 0) {
					addDebugBuf("ts getter end");
					break;
				}
				

				foreach (var _s in new List<string>(segM3u8List)) {
					var baseTime = getBaseTimeFromPlaylist(_s);
					var second = 0.0;
					var secondSum = 0.0;
					var targetDuration = 2.0;
//					lock(recordLock) {
						var getFileBytesTasks = new List<Task<numTaskInfo>>();
						foreach (var s in _s.Split('\n')) {
							//var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
							var _second = util.getRegGroup(s, "^#EXTINF:(.+),", 1, rm.regGetter.getExtInf());
							if (_second != null) {
								second = double.Parse(_second, NumberStyles.Float);
								secondSum += second;
							}
							var _targetDuration = util.getRegGroup(s, "^#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*)", 1, rm.regGetter.get_ExtXTargetDuration());
							if (_targetDuration != null) {
								targetDuration = double.Parse(_targetDuration);
							}
							var _endList = util.getRegGroup(s, "^(#EXT-X-ENDLIST)$", 1, rm.regGetter.getExtXEndlist());
							if (_endList != null) {
								isRetry = false;
								isEndProgram = true;
							}
							var _allDuration = util.getRegGroup(s, "^#STREAM-DURATION:(.+)", 1, rm.regGetter.getStreamDuration());
							if (_allDuration != null) {
								allDuration = double.Parse(_allDuration, NumberStyles.Float);
							}
							
							if (s.IndexOf(".ts") < 0) continue;
							var no = int.Parse(util.getRegGroup(s, "(\\d+).ts", 1, rm.regGetter.getTs()));
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
							
							if (no + baseNo > lastSegmentNo && !isInList && no + baseNo > gotTsMaxNo) {
<<<<<<< HEAD
								var fileName = util.getRegGroup(s, "(.+?.ts)\\?", 1, rm.regGetter.getTs2());
=======
								var fileName = util.getRegGroup(s, "(.+?.ts)\\?");
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
								//fileName = util.getRegGroup(fileName, "(\\d+)") + ".ts";
								//fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ".ts";
								fileName = (no + baseNo).ToString() + "_" + startTimeStr + ".ts";
								addDebugBuf(no + " " + fileName + " baseNo " + baseNo);
								//util.debugWriteLine(no + " " + fileName);
								
<<<<<<< HEAD
								var nti = new numTaskInfo(no + baseNo, url, second, fileName, startTime, no);
=======
								var nti = new numTaskInfo(no + baseNo, url, second, fileName, no);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
								getFileBytesTasks.Add(getFileBytesNti(nti));
							}
							
							
						}
						foreach (var _nti in getFileBytesTasks) _nti.Wait();
						foreach (var _nti in getFileBytesTasks) {
							var _res = _nti.Result;
							if (_res.res == null) {
								addDebugBuf("tsBytes null " + _res.url);
//									if (!isReConnecting) {
//										setReconnecting(true);
//										reConnect();
//									}
								segM3u8List.Clear();
								continue;
							}
							addDebugBuf("tsBytes get ok " + _res.url);
							//nti.res = _nti.tsBytes;
						
							if (isSub) rfu.subGotNumTaskInfo.Add(_res);
							else gotTsList.Add(_res);
							
							recordedSecond += _res.second;
							recordedBytes += _res.res.Length;
							lastFileSecond = _res.second;
							if (_res.no > gotTsMaxNo) gotTsMaxNo = _res.no;
						}
						
						segM3u8List.Remove(_s);
//					}
			
				}
//				segM3u8List.Clear();
				Thread.Sleep(300);
			}
		}
		
		
		
		
		private void startTsWriter() {
			while (true) {
<<<<<<< HEAD
//				addDebugBuf("ts writer loop gotTsListCount " + gotTsList.Count);
=======
				addDebugBuf("ts writer loop gotTsListCount " + gotTsList.Count);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				var isTsGetterEnd = (tsGetterTask.IsCanceled ||
						tsGetterTask.IsCompleted ||
						tsGetterTask.IsFaulted);
				if (isTsGetterEnd && gotTsList.Count == 0) break;
				
				var count = gotTsList.Count;
				foreach (var s in new List<numTaskInfo>(gotTsList)) {
					addDebugBuf("s.no " + s.no.ToString() + " s.originNo " + s.originNo + " lastsegmentno " + lastSegmentNo.ToString() + " " + baseNo);
					if (s.no <= lastSegmentNo) {
						gotTsList.Remove(s);
						continue;
					}
					
					bool ret;
					if (isPlayOnlyMode) ret = true;
					else {
						var before = DateTime.Now;
						ret = writeFile(s);
						addDebugBuf("write time " + (DateTime.Now - before));
						if (DateTime.Now - before > TimeSpan.FromSeconds(2))
							Thread.Sleep(2000);
					}
					//util.debugWriteLine("write ok " + s.no);
<<<<<<< HEAD
					
=======
					addDebugBuf("write ok " + s.no + " origin " + s.originNo);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
					if (ret) {
						if (wr.firstSegmentSecond == -1) 
							wr.firstSegmentSecond = s.startSecond;
						
						addDebugBuf("write ok " + s.no + " origin " + s.originNo);
						//recordedNo.Add(newGetTsTaskList[i].no.ToString());
						if (rfu.subGotNumTaskInfo != null)
							addDebugBuf("subGotTs count " + rfu.subGotNumTaskInfo.Count);
						
//						var isDropTest = (lastSegmentNo % 20 == 0);
//						isDropTest = false;
						
						if (lastSegmentNo != -1 && s.no - lastSegmentNo > 1) {
//						if (s.no - lastSegmentNo > 1) {
							addDebugBuf("nuke ari s.no " + s.no + " lastsegmentno " + lastSegmentNo + " s.originNo " + s.originNo);
							var _lastWroteSegmentDt = lastWroteSegmentDt;
							var _lastSegmentNo = lastSegmentNo;
							//Task.Run(() => dropSegmentProcess(s, _lastWroteSegmentDt, _lastSegmentNo));
							dropSegmentProcess(s, _lastWroteSegmentDt, _lastSegmentNo);
						}
						
						recordedNo.Add(s.fileName);
						lastSegmentNo = s.no;
						lastWroteSegmentDt = s.dt;
<<<<<<< HEAD
						var fName = util.getRegGroup(s.fileName, ".*(\\\\|/|^)(.+)", 2, rm.regGetter.getFName());
=======
						var fName = util.getRegGroup(s.fileName, ".*(\\\\|/|^)(.+)", 2);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
//							if (fName == 
						lastRecordedSeconds = util.getSecondsFromStr(fName);
						
						if (rfu.subGotNumTaskInfo != null)
							deleteOldSubTs();
					}
					gotTsList.Remove(s);
					Thread.Sleep(100);
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
				         		addDebugBuf(util.getPageSource(url, container, null, false, 2000));
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
				addDebugBuf(_buf);
				
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
				addDebugBuf(e.Message+e.StackTrace + e.Source + e.TargetSite);
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
			addDebugBuf("wait for recording");
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
			addDebugBuf("wait for rec end");
			isRetry = false;
			while(!isEnd) {
				Thread.Sleep(200);
			}

		}
		private string getHlsSegM3uUrl(string masterUrl) {
			addDebugBuf("master m3u8 " + masterUrl);
			var wc = new WebHeaderCollection();
			var res = util.getPageSource(masterUrl, ref wc, container);
			if (res == null) {
				reConnect(); 
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
			addDebugBuf("master m3u8 res " + res);
			addDebugBuf("seg m3u8 " + 
				masterBaseUrl + segUrl);
			return masterBaseUrl + segUrl;
		}
		DateTime lastsync = DateTime.Now;
		private double addNewTsTaskList(string hlsSegM3uUrl) {
			addDebugBuf("addNewTsTaskList" + hlsSegM3uUrl);
//			var wc = new WebHeaderCollection();

			addDebugBuf("getpage mae");
			
			var res = util.getPageSource(hlsSegM3uUrl, container, null, false, 2000);
			addDebugBuf("addNewTsTaskList segm3u get " + res);
//			util.debugWriteLine("m3u8 " + res);
			
			 
			if (otstul != null && !otstul.isStarted) outputTimeShiftTsUrlList(res);
			
			
			//shuusei? 
<<<<<<< HEAD
			int min = (res == null) ? -1 : int.Parse(util.getRegGroup(res, "(\\d+).ts", 1, rm.regGetter.getTs()));
			//if (res == null || (lastSegmentNo != -1 && res.IndexOf(lastSegmentNo.ToString()) == -1)) {
			if (res == null || (lastSegmentNo != -1 && min != -1 && min > lastSegmentNo)) {
			//if (res == null) {
				addDebugBuf("nuke? lastSegmentNo " + lastSegmentNo + " res " + res);
=======
			int min = (res == null) ? -1 : int.Parse(util.getRegGroup(res, "(\\d+).ts"));
			//if (res == null || (lastSegmentNo != -1 && res.IndexOf(lastSegmentNo.ToString()) == -1)) {
			if (res == null || (lastSegmentNo != -1 && min != -1 && min > lastSegmentNo)) {
			//if (res == null) {
<<<<<<< HEAD
				addDebugBuf("nuke? lastSegmentNo " + lastSegmentNo + " res " + res);
=======
				util.debugWriteLine("nuke? lastSegmentNo " + lastSegmentNo + " res " + res);
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
//				rm.form.addLogText("リトライ lastSegmentNo " + lastSegmentNo + " res " + res + " min " + min);
				setReconnecting(true);
//				if (!isReConnecting) 
					reConnect();
				
				return 1.0;
			}
			var isTimeShiftPlaylist = res.IndexOf("#STREAM-DURATION") > -1;
			if (!isTimeShift && isTimeShiftPlaylist) {
				return -1;
			}
			double streamDuration = -1; 
			var _streamDuration = util.getRegGroup(res, "#STREAM-DURATION:(.+)");
			if (_streamDuration != null) streamDuration = double.Parse(_streamDuration);
			
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
<<<<<<< HEAD
					var isEndList = s.IndexOf("(#EXT-X-ENDLIST)") > -1;
					if (isEndList) {
=======
					var _endList = util.getRegGroup(s, "(#EXT-X-ENDLIST)");
					if (_endList != null) {
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
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
						
						newGetTsTaskList.Add(new numTaskInfo(no, url, second, fileName, startTime));
						//Task.Run(() => getTsTask(url, startTime));
						getTsTask(url, startTime);
					}
					if (isEndTimeshift(streamDuration)) {
						isRetry = false;
						isEndProgram = true;
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
//								rm.form.addLogText("セグメント取得エラー " + url);
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
							if (wr.firstSegmentSecond == -1) 
								wr.firstSegmentSecond = newGetTsTaskList[i].startSecond;
							
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
				addDebugBuf(e.ToString());
				addDebugBuf(e.Message + e.StackTrace + e.TargetSite);
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
				addDebugBuf("original ts record exception " + e.Message+e.StackTrace + e.Source + e.TargetSite);
				return false;
			}
		}
		/*
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
		*/
		/*
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
		*/
		public void reSetHlsUrl(string url, string quality, WebSocket _ws) {
			addDebugBuf("resetHlsUrl oldurl " + hlsMasterUrl + " new url " + url);
			ws = _ws;
			if (recordingQuality != quality)
				rm.form.addLogText("画質を変更して再接続します(" + quality + ")" + util.getMainSubStr(isSub, false));
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
				
				if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
				   	int.Parse(rm.cfg.get("afterConvertMode")) != 0) {
					var tf = new ThroughFFMpeg(rm);
					tf.start(outFName, true);
					
				}
				if (rm.ri != null && rm.ri.afterFFmpegMode != 0) {
					
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
				addDebugBuf("renketu after out fname " + outFName);			
				if (outFName.Length > 245) outFName = recFolderFile + "/" + lvid + ".ts";
				if (outFName.Length > 245) outFName = recFolderFile + "/out.ts";
				addDebugBuf("renketu after out fname shuusei go " + outFName);
				w = new FileStream(outFName, FileMode.Append, FileAccess.Write);
			} catch (PathTooLongException e) {
				try {
					addDebugBuf("renketu after out fname " + recFolderFile + "/" + lvid + ".ts");			
					w = new FileStream(recFolderFile + "/" + lvid + ".ts", FileMode.Append, FileAccess.Write);
				} catch (PathTooLongException ee) {
					try {
						addDebugBuf("renketu after out fname " + recFolderFile + "/_.ts");			
						w = new FileStream(recFolderFile + "/_.ts", FileMode.Append, FileAccess.Write);
					} catch (PathTooLongException eee) {
						addDebugBuf("renketu after too long");
						rm.form.addLogText("録画後に連結しようとしましたがパスが長すぎてファイルが開けませんでした " + recFolderFile + "/_.ts");
						return null;
					}
					
				}
			}
			
				
			foreach (var s in recordedNo) {
				addDebugBuf(s);
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
					addDebugBuf("renketu after write exception " + s + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
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
					var recStartTime = DateTime.Now;
					var startPlayList = util.getPageSource(hlsSegM3uUrl, container, null, false, 2000);
					
					var aer = new AnotherEngineRecorder(rm, rfu);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					if (isAnotherEngineTimeShiftEnd(recStartTime, hlsSegM3uUrl, startPlayList)) {
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
			
			if (isEndProgram) {
				rm.form.addLogText("録画を完了しました");
			}
			if (isDefaultEngine && !isPlayOnlyMode) {
<<<<<<< HEAD
				if (isEndProgram && segmentSaveType == 0) {
					renameWithoutTime(recFolderFile);
					
				}
				if (segmentSaveType == 1 &&
=======
				if (isEndProgram)
					rm.form.addLogText("録画を完了しました");
				if (segmentSaveType == 1 && 
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				    	(rm.cfg.get("IsRenketuAfter") == "true" || 
				     int.Parse(rm.cfg.get("afterConvertMode")) != 0)) {
					addDebugBuf("renketu after");
					renketuAfter();
				} else if (segmentSaveType == 0) {
					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || 
				    	    int.Parse(rm.cfg.get("afterConvertMode")) != 0) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ".ts", true);
						
					}
				}
			}
			isEnd = true;
		}
		private double getBaseTimeFromPlaylist(string res) {
			//most extinf second
			var mostSegmentSecond = getMostSegmentSecond(res);
			
			var mediaSequenceNum = util.getRegGroup(res, "#EXT-X-MEDIA-SEQUENCE\\:(.+)", 1, rm.regGetter.getExtXMediaSequence());
			if (mediaSequenceNum == null) return -1;
			return mostSegmentSecond * double.Parse(mediaSequenceNum);
		}
		private double getMostSegmentSecond(string res) {
			var timeArr = new List<double[]>();
			foreach (var l in res.Split('\n')) {
				//var _second = util.getRegGroup(l, "^#EXTINF:(\\d+(\\.\\d+)*)");
				var _second = util.getRegGroup(l, "^#EXTINF:(.+),", 1, rm.regGetter.getExtInf());
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
			#if !DEBUG
				return;
			#endif
			
			while (rm.rfu == rfu && isRetry) {
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				try {
					lock (debugWriteBuf) {
						string[] l = new String[debugWriteBuf.Count + 10];
						debugWriteBuf.CopyTo(l);
	//					var l = debugWriteBuf.ToList<string>();
						//var l = new List<string>(debugWriteBuf);
						foreach (var b in l) {
							if (b == null) continue;
							util.debugWriteLine(b + " " + util.getMainSubStr(isSub, true));
							debugWriteBuf.Remove(b);
						}
					}
<<<<<<< HEAD
					Thread.Sleep(100);
				} catch (Exception e) {
=======
					Thread.Sleep(500);
				} catch (Exception e) {
=======
				var l = debugWriteBuf.ToList<string>();
				//var l = new List<string>(debugWriteBuf);
				foreach (var b in l) {
					util.debugWriteLine(b);
					debugWriteBuf.Remove(b);
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				}
			}
		}
		public void addDebugBuf(string s) {
<<<<<<< HEAD
			#if !DEBUG
				return;
			#endif
			
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
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
<<<<<<< HEAD
					maxNo = int.Parse(util.getRegGroup(l, "(\\d+)\\.ts", 1, rm.regGetter.getMaxNo()));
=======
					maxNo = int.Parse(util.getRegGroup(l, "(\\d+)\\.ts"));
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
					maxLine = l;
					if (minNo == 0) minNo = maxNo; 
				}
			}
			if (maxNo + baseNo < gotTsMaxNo) {
				addDebugBuf("base no change maxno " + maxNo + " gotTsMaxNo " + gotTsMaxNo);
				baseNo = gotTsMaxNo;
			}
			
			//if (lastGetPlayListMaxNo != 0 && minNo > lastGetPlayListMaxNo) {
			addDebugBuf("minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString() + " base no " + baseNo);
			//if (minNo > lastGetPlayListMaxNo) {
<<<<<<< HEAD
			if (minNo + baseNo > gotTsMaxNo) {
				addDebugBuf("nuke? minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString() + " gottsmaxno " + gotTsMaxNo);
				var inf = getMostSegmentSecond(res);
				var ins = "";
				var startNo = (minNo + baseNo - gotTsMaxNo > 7) ? (minNo - 7) : (gotTsMaxNo - baseNo + 1);
=======
<<<<<<< HEAD
			if (minNo + baseNo > gotTsMaxNo) {
				addDebugBuf("nuke? minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString() + " gottsmaxno " + gotTsMaxNo);
				var inf = getMostSegmentSecond(res);
				var ins = "";
				var startNo = (minNo + baseNo - gotTsMaxNo > 7) ? (minNo - 7) : (gotTsMaxNo - baseNo + 1);
=======
			if (minNo > gotTsMaxNo) {
				addDebugBuf("nuke? minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString() + " gottsmaxno " + gotTsMaxNo);
				var inf = getMostSegmentSecond(res);
				var ins = "";
				var startNo = (minNo - gotTsMaxNo > 10) ? (minNo - 10) : (gotTsMaxNo + 1);
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				for (var i = startNo; i < minNo; i++) {
					if (i < 0) continue;
					ins += "#EXTINF:" + inf + ",\n";
					ins += maxLine.Replace(maxNo.ToString() + ".ts", i.ToString() + ".ts") + "\n";
				}
				res = res.Insert(res.IndexOf("EXTINF:"), ins);
				addDebugBuf("added list " + res);
			}
			lastGetPlayListMaxNo = maxNo + baseNo;
			
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
		private bool isEndTimeshift(double streamDuration) {
			if (streamDuration == -1) return false;
			return lastSegmentNo + 5000 > streamDuration * 1000;
		}
<<<<<<< HEAD
		private bool isAnotherEngineTimeShiftEnd(DateTime recStartTime, string hlsSegM3uUrl, string startPlayList) {
			if (startPlayList == null) return false;
			var lastTsNum = util.getRegGroup(startPlayList, "[\\s\\S]+\n(\\d+).ts", 1, rm.regGetter.getLastTsNum());
=======
<<<<<<< HEAD
		private bool isAnotherEngineTimeShiftEnd(DateTime recStartTime, string hlsSegM3uUrl, string startPlayList) {
			if (startPlayList == null) return false;
			var lastTsNum = util.getRegGroup(startPlayList, "[\\s\\S]+\n(\\d+).ts");
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			if (lastTsNum == null) 
				return false;
			
			double streamDuration = -1;
<<<<<<< HEAD
			var _streamDuration = (startPlayList == null) ? null : util.getRegGroup(startPlayList, "#STREAM-DURATION:(.+)", 1, rm.regGetter.getStreamDuration());
=======
			var _streamDuration = (startPlayList == null) ? null : util.getRegGroup(startPlayList, "#STREAM-DURATION:(.+)");
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			if (_streamDuration == null) return false;
			streamDuration = double.Parse(_streamDuration, NumberStyles.Float);
			
			TimeSpan diffTime = DateTime.Now - recStartTime;
			addDebugBuf(diffTime.TotalMilliseconds.ToString());
			if (int.Parse(lastTsNum) + diffTime.TotalMilliseconds > streamDuration * 1000)
				return true;
			return false;
			/*
			var a = new System.Net.WebHeaderCollection();
			var _resM3u8 = util.getPageSource(hlsSegM3uUrl, ref a, container);
			var _lastSegTime = (_resM3u8 == null) ? null : util.getRegGroup(_resM3u8, ".+(\\d+).ts");
			var lastSegTime = (_lastSegTime == null) ? -1 : double.Parse(_lastSegTime);
			
			double streamDuration = -1;
			var _streamDuration = (_resM3u8 == null) ? null : util.getRegGroup(_resM3u8, "#STREAM-DURATION:(.+)");
			if (_streamDuration != null) streamDuration = double.Parse(_streamDuration);
			
			if (streamDuration == -1 || _resM3u8 == null || lastSegTime == -1) return false;
			return lastSegTime + 5000 > streamDuration * 1000 || 
					_resM3u8.IndexOf("#EXT-X-ENDLIST") > -1;
			*/
		}
		private void dropSegmentProcess(numTaskInfo s, DateTime _lastWroteSegmentDt, int _lastSegmentNo) {
			var dsp = new DropSegmentProcess(_lastWroteSegmentDt, _lastSegmentNo, this, recFolderFileOrigin, rfu, rm);
			dsp.start(s);
		}
		private void deleteOldSubTs() {
			try {
				var c = rfu.subGotNumTaskInfo.Count;
				for (var i = 0; i < c; i++) {
					var t = (lastFileSecond != 0) ? (lastFileSecond * 7) : 10;
<<<<<<< HEAD
					addDebugBuf("delete old subts i " + i + " c " + c + " count " + rfu.subGotNumTaskInfo.Count);
					if (rfu.subGotNumTaskInfo[0].dt < 
					    	lastWroteSegmentDt - TimeSpan.FromSeconds(t)) {
						addDebugBuf("delete subGotTs subTs.dt " + rfu.subGotNumTaskInfo[0].dt + " " + rfu.subGotNumTaskInfo[0].no + " originNo " + rfu.subGotNumTaskInfo[0].originNo + " lastWroteDt " + lastWroteSegmentDt);
						rfu.subGotNumTaskInfo.Remove(rfu.subGotNumTaskInfo[0]);
=======
					if (rfu.subGotNumTaskInfo[0].dt < 
					    	lastWroteSegmentDt - TimeSpan.FromSeconds(t)) {
						addDebugBuf("delete subGotTs subTs.dt " + rfu.subGotNumTaskInfo[0].dt + " " + rfu.subGotNumTaskInfo[0].no + " originNo " + rfu.subGotNumTaskInfo[0].originNo + " lastWroteDt " + lastWroteSegmentDt);
						rfu.subGotNumTaskInfo.RemoveAt(0);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
					}
				}
			} catch (Exception e) {
				addDebugBuf("delete old sub ts exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		async Task<numTaskInfo> getFileBytesNti(numTaskInfo nti) {
			byte[] tsBytes;
			addDebugBuf("getfilebyte " + nti.url);
			nti.res = util.getFileBytes(nti.url, container);
			return nti;
		}
		private void displayWriteRemainGotTsData() {
<<<<<<< HEAD
//			Task.Run(() => {
				while (gotTsList.Count > 0) {
					rm.form.addLogText("未書き込みのデータを書き込んでいます...(" + gotTsList.Count + "件)");
					addDebugBuf("mi kakikomi write write " + gotTsList.Count);
					Thread.Sleep(10000);
				}
//			});
		}
		private void renameWithoutTime(string name) {
			var time = util.getRegGroup(name, "(\\d+h\\d+m\\d+s)", 1, rm.regGetter.getRenameWithoutTime_time());
			var num = util.getRegGroup(name, "\\d+h\\d+m\\d+s_(\\d+)", 1, rm.regGetter.getRenameWithoutTime_num());
			
			try {
				for (int i = int.Parse(num); i < 1000; i++) {
					var newName = name.Replace("_" + time + "_" + num, i.ToString());
					if (File.Exists(newName + ".ts")) continue;
					File.Move(recFolderFile + ".ts", newName + ".ts");
					return;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
=======
			Task.Run(() => {
				while (gotTsList.Count > 5) {
					rm.form.addLogText("未書き込みのデータを書き込んでいます...(" + gotTsList.Count + "件)");
					Thread.Sleep(10);
				}
			});
=======
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
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		}
	}
}
