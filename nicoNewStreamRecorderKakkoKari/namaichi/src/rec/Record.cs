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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using namaichi.utility;
using WebSocket4Net;
using System.Globalization;
using System.Diagnostics;
using System.Text;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Record.
	/// </summary>
	public class Record
	{
		private RecordingManager rm;
		//private bool isFFmpeg;
		private RecordFromUrl rfu;
		//private System.Diagnostics.Process process;
		private DateTime lastReadTime = DateTime.UtcNow;
		public string hlsMasterUrl;
		public string recFolderFile;
		//private string lvid;
		//private long _openTime;
		public int lastSegmentNo = -1;
		public DateTime lastWroteSegmentDt = DateTime.MinValue;
		//private int lastAccessingSegmentNo;
		private CookieContainer container;
		private int segmentSaveType = 0;
		//private bool isTimeShift = false;
		//public TimeShiftConfig tsConfig = null;
		private List<numTaskInfo> newGetTsTaskList = new List<numTaskInfo>();
		private List<string> recordedNo = new List<string>();
		private string baseUrl;
		private IRecorderProcess wr;
		public bool isReConnecting = false;
		public bool isRetry = true;
		private bool isEnd = false;
		private string hlsSegM3uUrl;
		private double recordedSecond = 0;
		private long recordedBytes = 0;
		public int lastRecordedSeconds = -1;
		private double lastFileSecond = 0;
		private int gotTsMaxNo = -1;
		public bool isEndProgram = false;
		public bool isTsEndTimeEnd = false;
		private double allDuration = -1;
		public string engineMode = "0";
		private string anotherEngineCommand = ""; 
		//private Html5Recorder h5r;
		private double targetDuration = 2;
		private object recordLock = new object();
		private List<string> segM3u8List = new List<string>();
		private List<numTaskInfo> gotTsList = new List<numTaskInfo>();
		private Task m3u8GetterTask = null;
		private Task tsGetterTask = null;
		private Task tsWriterTask = null;
		private List<string> debugWriteBuf = new List<string>();
		private bool isPlayOnlyMode;
		//private OutputTimeShiftTsUrlList otstul;
		private FileStream renketuRealTimeFS = null;
		
		private string recordingQuality = null;
		private WebSocket ws;
		//private string recFolderFileOrigin;
		//private bool isSub;
		private int baseNo = 0;
		
//		private string lastTsPlayList = null;
//		private DateTime lastTsPlayListTime = DateTime.MinValue;
		private double lastWroteFileSecond = -1;
		private DateTime lastAccessPlaylistTime = DateTime.Now;
		
//		private RealTimeFFmpeg realtimeFFmpeg = null;
		//private bool isRealtimeChase = false;
		public DropSegmentProcess dsp = null;
		private bool isSpeedUp = false;
		private string ua = null;//"Lavf/56.36.100";
		private string referer = null;
		public static bool isWriteCancel = false;
		public string ext = null;
		private int baseTfdt = -1;
		private byte[] initMp4 = null;
		private numTaskInfo lastWriteFmp4Nti = null;
		private double lateTime = 100;
		public RecordInfo ri = null;
		Curl getM3u8Curl = null;
		Curl getTsCurl = null;
		bool isSavedKey = false;
		int channelPlusTimeShiftTotalTime = -1;
		bool isWebRequest = util.isWebRequestOk;
		
		//debug
		private int lastGetPlayListMaxNo = 0;
		
		private bool IsSecondRecordDir = false;
		private string secondRecordDir = "";
		/*
		public Record(RecordingManager rm, bool isFFmpeg, 
		              RecordFromUrl rfu, string hlsUrl, 
		              string recFolderFile, //int lastSegmentNo, 
		              CookieContainer container, bool isTimeShift, 
		              IRecorderProcess wr, string lvid, 
		              TimeShiftConfig tsConfig, long _openTime,
		              WebSocket ws, string recFolderFileOrigin,
		              bool isRealtimeChase, Html5Recorder h5r) {
			this.rm = rm;
			this.rfu = rfu;
			this.hlsMasterUrl = hlsUrl;
			this.recFolderFile = recFolderFile;
			this.container = container;
			this.isTimeShift = isTimeShift;
			segmentSaveType = int.Parse(rm.cfg.get("segmentSaveType"));
			this.wr = wr;
			this.lvid = lvid;
			this.tsConfig = tsConfig;
			engineMode = rm.cfg.get("EngineMode");
			if (rfu.isPlayOnlyMode) engineMode = "0";
			anotherEngineCommand = rm.cfg.get("anotherEngineCommand");
			targetDuration = (isTimeShift) ? 5 : 2;
			
			rm.isTitleBarInfo = bool.Parse(rm.cfg.get("IstitlebarInfo"));
			isPlayOnlyMode = rfu.isPlayOnlyMode;
			//this._openTime = _openTime;
			this.ws = ws;
			this.recFolderFileOrigin = recFolderFileOrigin;
			this.isRealtimeChase = isRealtimeChase;
			this.h5r = h5r;
			this.referer = null;//rfu.url;
			ext = h5r.ri.isFmp4 ? ".mp4" : ".ts";
		}
		*/
		public Record(RecordingManager rm, RecordFromUrl rfu, 
				string hlsUrl, CookieContainer container, IRecorderProcess wr, 
				WebSocket ws, RecordInfo ri) {
			this.rm = rm;
			this.rfu = rfu;
			this.recFolderFile = ri.recFolderFile[1];
			this.hlsMasterUrl = hlsUrl;
			this.container = container;
			//this.isTimeShift = ri.si.isTimeShift;
			this.wr = wr;
			//this.lvid = ri.si.lvid;
			//this.tsConfig = ri.timeShiftConfig;
			this.ws = ws;
			//this.recFolderFileOrigin = ri.recFolderFile[2];
			//this.isRealtimeChase = ri.isRealtimeChase;
			//this.h5r = h5r;
			segmentSaveType = int.Parse(rm.cfg.get("segmentSaveType"));
			engineMode = rm.cfg.get("EngineMode");
			if (rfu.isPlayOnlyMode) engineMode = "0";
			anotherEngineCommand = rm.cfg.get("anotherEngineCommand");
			targetDuration = (ri.si.isTimeShift) ? 5 : 2;
			IsSecondRecordDir = bool.Parse(rm.cfg.get("IsSecondRecordDir"));
			secondRecordDir = rm.cfg.get("secondRecordDir");
			
			rm.isTitleBarInfo = bool.Parse(rm.cfg.get("IstitlebarInfo"));
			isPlayOnlyMode = rfu.isPlayOnlyMode;
			//this._openTime = ri.si.openTime;
			this.referer = null;//rfu.url;
			ext = ri.isFmp4 ? ".mp4" : ".ts";
			this.ri = ri;
		}
		public Record(RecordingManager rm, RecordFromUrl rfu, 
				string segHlsUrl, CookieContainer container, 
				RecordInfo ri, IRecorderProcess wr) {
			this.rm = rm;
			this.rfu = rfu;
			this.recFolderFile = ri.recFolderFile[1];
			this.hlsSegM3uUrl = segHlsUrl;
			this.container = container;
			this.wr = wr;
			segmentSaveType = int.Parse(rm.cfg.get("segmentSaveType"));
			engineMode = rm.cfg.get("EngineMode");
			//if (rfu.isPlayOnlyMode) engineMode = "0";
			anotherEngineCommand = rm.cfg.get("anotherEngineCommand");
			targetDuration = (ri.si.isTimeShift) ? 10 : 2;
			IsSecondRecordDir = bool.Parse(rm.cfg.get("IsSecondRecordDir"));
			secondRecordDir = rm.cfg.get("secondRecordDir");
			
			rm.isTitleBarInfo = bool.Parse(rm.cfg.get("IstitlebarInfo"));
			isPlayOnlyMode = false;//rfu.isPlayOnlyMode;
			this.referer = null;//rfu.url;
			ext = ".ts";
			this.ri = ri;
			getM3u8Curl = new Curl();
			getTsCurl = new Curl();
		}
		public void record(string quality) {
			recordingQuality = quality;
			tsWriterTask = Task.Factory.StartNew(() => {startDebugWriter();}, TaskCreationOptions.LongRunning);
			
			var _m = (isPlayOnlyMode) ? "視聴" : "録画";
			if (ri.si.isTimeShift) {
				var isHokan = !wr.isSaveComment && ri.isChase; 
				if (engineMode != "3" && !isHokan)
					rm.form.addLogText((ri.isChase ? "追っかけ再生" : "タイムシフト") + "の" + _m + "を開始します(画質:" + quality + ")");
//				timeShiftMultiRecord();
				timeShiftOnTimeRecord();
			} else {
				if (engineMode != "3") {
					//if (isSub) {
					//	rm.form.addLogText(_m + "をスタンバイします(画質:" + quality + " " + "サブ)");
					//} else
						rm.form.addLogText(_m + "を開始します(画質:" + quality + " " + "メイン)");
				}
				realTimeRecord();
			}
			
		}
		private void realTimeRecord() {
			if (hlsSegM3uUrl == null)
				hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
			rm.hlsUrl = hlsSegM3uUrl;
			rm.form.setPlayerBtnEnable(true);
			
			if (engineMode == "0") {
				m3u8GetterTask = Task.Factory.StartNew(() => {startM3u8Getter();}, TaskCreationOptions.LongRunning);
				tsGetterTask = Task.Factory.StartNew(() => {startTsGetter();}, TaskCreationOptions.LongRunning);
				//if (!isSub)
					tsWriterTask = Task.Factory.StartNew(() => {startTsWriter();}, TaskCreationOptions.LongRunning);
				
			}
//			Task.Run(() => {syncCheck();});
			
//			if (isFFmpegThrough()) realtimeFFmpeg = new RealTimeFFmpeg();
			
			var isFirst = true;
			while (rm.rfu == rfu && isRetry) {
				if (isReConnecting) {
					Thread.Sleep(500);
					
					
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
				
				if (engineMode == "0" || engineMode == "3") {
					Thread.Sleep(500);
					
				} else {
					if (wr != null) {
						wr.setSync(0, 0, hlsSegM3uUrl);
						if (!isFirst) wr.resetCommentFile();
					}
					isFirst = false;
					
					var aer = new AnotherEngineRecorder(rm, rfu, this);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					recFolderFile = util.incrementRecFolderFile(recFolderFile);//wr.getRecFilePath()[1];
					setReconnecting(true);
					if (wr != null)
						wr.sync = 0;
					
					reConnect();
					continue;
					
				}
			}
			/*
			if (isSub) {
				isEnd = true;
				return;
			}
			*/
			rm.hlsUrl = "end";
//			rm.form.setPlayerBtnEnable(false);
			
			if (engineMode == "0" && !isPlayOnlyMode) {
				addDebugBuf("rec end shori gottslist count " + gotTsList.Count);
				if (rfu.subGotNumTaskInfo != null)
					addDebugBuf("subgot " + rfu.subGotNumTaskInfo.Count);
				if (gotTsList.Count > 10) {
					displayWriteRemainGotTsData();
					
				}
				waitForRecording();
				if (renketuRealTimeFS != null) {
					try {
						renketuRealTimeFS.Close();
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					}
				}
				if (rm.cfg.get("fileNameType") == "10" && (recFolderFile.IndexOf("{w}") > -1 || recFolderFile.IndexOf("{c}") > -1))
					renameStatistics();
				
				if (rfu.subGotNumTaskInfo != null)
					addDebugBuf("rfu.subGotNumTaskInfo.count " + rfu.subGotNumTaskInfo.Count);
				var isManyNti = (rfu.subGotNumTaskInfo == null || rfu.subGotNumTaskInfo.Count == 0) ? false : (rfu.subGotNumTaskInfo[rfu.subGotNumTaskInfo.Count - 1].dt - rfu.subGotNumTaskInfo[0].dt > TimeSpan.FromSeconds(25));
				if (isManyNti) addDebugBuf("isManyNti subgot [0].dt + " + rfu.subGotNumTaskInfo[0].dt + " [count - 1] " + rfu.subGotNumTaskInfo[rfu.subGotNumTaskInfo.Count - 1].dt);
//				isManyNti = true;
				if (rfu.subGotNumTaskInfo != null && isManyNti) {
					rm.form.addLogText("抜けセグメントの補完を試みます");
					addDebugBuf("抜けセグメントの補完を試みます");
					if (dsp == null) {
						dsp = new DropSegmentProcess(lastWroteSegmentDt, lastSegmentNo, this, ri.recFolderFile[2], rfu, rm, ri);
						dsp.writeRemaining();
						dsp = null;
					} 
				}
				segM3u8List.Clear();
				gotTsList.Clear();
				
				if (isEndProgram) {
					rm.form.addLogText("録画を完了しました");
					if (wr.endTime == DateTime.MinValue)
						wr.endTime = DateTime.Now;
				}
				
				if (segmentSaveType == 1 && 
				    	(rm.cfg.get("IsRenketuAfter") == "true"
//				     || (int.Parse(rm.cfg.get("afterConvertMode")) != 0 &&
//				     	int.Parse(rm.cfg.get("afterConvertMode")) != 1))) {
				    )) {
					addDebugBuf("renketu after");
					renketuAfter();
				} else if (segmentSaveType == 0) {
//					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || 
//					    (int.Parse(rm.cfg.get("afterConvertMode")) != 0 &&
//					     	int.Parse(rm.cfg.get("afterConvertMode")) != 1)) {
					if (int.Parse(rm.cfg.get("afterConvertMode")) > 0) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ext, true);
					}
				}
				//rm.form.addLogText("録画終了処理を完了しました");
			}
			isEnd = true;
		}
		private void startM3u8Getter() {
			var reconnectingCount = 0;
			while (rm.rfu == rfu && isRetry) {
				try {
	//				if (DateTime.Now.Minute % 5 == 0) Thread.Sleep(50 * 1000);
					
	//				util.debugWriteLine("gc count " + GC.CollectionCount(0) + " " + GC.CollectionCount(1) + " " + GC.CollectionCount(2) + " " + GC.CollectionCount(3));
					//util.debugWriteLine("isreconnecting " + isReConnecting);
					addDebugBuf("isreconnecting " + isReConnecting);
	
					
					if (isReConnecting) {
						reconnectingCount++;
						if (reconnectingCount > 20) {
							isReConnecting = false;
							
						}
						Thread.Sleep(500);
						continue;
					}
					reconnectingCount = 0;
					
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
						rm.form.addLogTextDebug("record hlsUrl not start_time " + hlsSegM3uUrl);
						break;
					}
					//util.debugWriteLine("getpage m3u8 mae");
					addDebugBuf("getpage m3u8 mae");
					//var res = util.getPageSource(hlsSegM3uUrl, null, referer, false, 2000, ua);
					var res = getPageSourceRec(hlsSegM3uUrl);
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
					if (!ri.si.isTimeShift && isTimeShiftPlaylist) {
						rm.form.addLogTextDebug("record not contain stream-duration  " + res);
						isRetry = false;
						return;
						//return -1;
					}
					
					var addeddFutureList = getAddedNearList(res);
					
					//util.debugWriteLine("timeshift check go");
					addDebugBuf("timeshift check go");
					//lock (segM3u8List) {
						//segM3u8List.Add(res);
					if (!isPlayOnlyMode) {
						if (addeddFutureList != null) {
							segM3u8List.Add(addeddFutureList);
						} else {
							addDebugBuf("add m3u8 null");
							#if DEBUG
								rm.form.addLogText("add m3u8 null");
							#endif
						}
					}
					//}
					//util.debugWriteLine("seg m3u8 add");
					addDebugBuf("seg m3u8 add");
					
					var _targetDuration = util.getRegGroup(res, "#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*(e\\d+)*)", 1, rm.regGetter.getExtXTargetDuration());
					if (_targetDuration != null) {
						targetDuration = double.Parse(_targetDuration, NumberStyles.Float);
					}
					var intervalTime = (int)(targetDuration * 1000);
					var elapsedTime = (int)((TimeSpan)(DateTime.Now - lastAccessPlaylistTime)).TotalMilliseconds;
					util.debugWriteLine("wait time " + intervalTime + " " + elapsedTime);
					Thread.Sleep(elapsedTime > intervalTime ? 0 : intervalTime - elapsedTime);
					lastAccessPlaylistTime = DateTime.Now;
					//util.debugWriteLine("targetduration " + targetDuration);
					addDebugBuf("targetduration " + targetDuration);
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
					rm.form.addLogText("プレイリストの取得中に何らかの問題が発生しました " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			addDebugBuf("m3u8 getter end segm8u8List len " + segM3u8List.Count);
		}
		private void startTsGetter() {
			while (true) {
				try {
					var isM3u8GetterEnd = (m3u8GetterTask.IsCanceled ||
							m3u8GetterTask.IsCompleted ||
							m3u8GetterTask.IsFaulted);
					if (isM3u8GetterEnd && segM3u8List.Count == 0) {
						addDebugBuf("ts getter end");
						break;
					}
					
					var isBreak = false;
					foreach (var _s in new List<string>(segM3u8List)) {
						if (isBreak) break;
	//					var sss = new List<string>(segM3u8List);
						if (_s == null) continue;
						var baseTime = getBaseTimeFromPlaylist(_s);
						var second = 0.0;
						var secondSum = 0.0;
						var targetDuration = 2.0;
						
//					lock(recordLock) {
							var getFileBytesTasks = new List<Task<numTaskInfo>>();
	//						var line = ;
							foreach (var s in _s.Split('\n')) {
	//							var s = line[i];
								//var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
								
								var _initMp4 = util.getRegGroup(s, null, 1, rm.regGetter.getExtXMap());
								if (_initMp4 != null && initMp4 == null) {
									var _url = baseUrl + _initMp4;
									var nti = new numTaskInfo(-1, _url, 0, null, 0, 0);
									var r = getFileBytesNti(nti);
									if (r == null) {
										segM3u8List.Clear();
										break;
									}
									else getFileBytesTasks.Add(r);
									continue;
								}
								
								var _second = util.getRegGroup(s, "^#EXTINF:(.+),", 1, rm.regGetter.getExtInf());
								if (_second != null) {
									second = double.Parse(_second, NumberStyles.Float);
									secondSum += second;
								}
								var _targetDuration = util.getRegGroup(s, "^#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*(e\\d+)*)", 1, rm.regGetter.get_ExtXTargetDuration());
								if (_targetDuration != null) {
									targetDuration = double.Parse(_targetDuration, NumberStyles.Float);
								}
								var _endList = util.getRegGroup(s, "^(#EXT-X-ENDLIST)$", 1, rm.regGetter.getExtXEndlist());
								if (_endList != null) {
									isRetry = false;
									isEndProgram = true;
									rm.form.addLogTextDebug("record endlist " + _s);
								}
								var _allDuration = util.getRegGroup(s, "^#STREAM-DURATION:(.+)", 1, rm.regGetter.getStreamDuration());
								if (_allDuration != null) {
									allDuration = double.Parse(_allDuration, NumberStyles.Float);
								}
								
								if (s.IndexOf(ext) < 0) continue;
								var no = int.Parse(util.getRegGroup(s, "(\\d+)" + ext, 1, rm.regGetter.getTs()));
								var url = baseUrl + s;
								
								var isInList = false;
								lock (newGetTsTaskList) {
									foreach (var t in newGetTsTaskList)
										if (t.no == no) isInList = true;
								}
								
								var startTime = baseTime + secondSum - second;
								if (ri.si.isTimeShift && 
								    	((ri.timeShiftConfig.timeType == 0 && startTime < ri.timeShiftConfig.timeSeconds) ||
								     	(ri.timeShiftConfig.timeType == 1 && startTime <= ri.timeShiftConfig.timeSeconds))) continue;
								var startTimeStr = util.getSecondsToStr(startTime);
								
								if (no + baseNo > lastSegmentNo && !isInList && no + baseNo > gotTsMaxNo) {
									var fileName = util.getRegGroup(s, "(.+?" + ext + ")\\?", 1, rm.regGetter.getTs2());
									//fileName = util.getRegGroup(fileName, "(\\d+)") + ext;
									//fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ext;
									fileName = (no + baseNo).ToString() + "_" + startTimeStr + ext;
									addDebugBuf(no + " " + fileName + " baseNo " + baseNo);
									//util.debugWriteLine(no + " " + fileName);
									
									var nti = new numTaskInfo(no + baseNo, url, second, fileName, startTime, no);
									//var t = Task.Run(() => getFileBytesNti(nti));
	
									var r = getFileBytesNti(nti);
									if (r == null) segM3u8List.Clear();
									else getFileBytesTasks.Add(r);
	//								getFileBytesTasks.Add(t);
								}
								
								
							}
							foreach (var _nti in getFileBytesTasks) _nti.Start();
							foreach (var _nti in getFileBytesTasks) _nti.Wait();
							foreach (var _nti in getFileBytesTasks) {
								var _res = _nti.Result;
								if (_res.res == null) {
									addDebugBuf("tsBytes null " + _res.url + " segm3u8List.count" + segM3u8List.Count);
									if (segM3u8List.Count > 0)
										addDebugBuf("m3u8list[0] " + segM3u8List[0]);
	//									if (!isReConnecting) {
	//										setReconnecting(true);
	//										reConnect();
	//									}
									segM3u8List.Clear();
									
									isBreak = true;

									continue;
								}
								
								addDebugBuf("tsBytes get ok " + _res.url);
								//nti.res = _nti.tsBytes;
							
								//if (isSub) rfu.subGotNumTaskInfo.Add(_res);
								//else 
								if (_res.url.IndexOf("init1.mp4") > -1)
									initMp4 = _res.res;
								else {
									if (ri.isFmp4) baseTfdt = getChangedTfdtMp4(_res.res, baseTfdt, out _res.res);
									gotTsList.Add(_res);
								}
								
								//recordedSecond += _res.second;
								//recordedBytes += _res.res.Length;
								lastFileSecond = _res.second;
								if (_res.no > gotTsMaxNo) gotTsMaxNo = _res.no;
							}
							segM3u8List.Remove(_s);
	//					}
				
					}
	//				segM3u8List.Clear();
					Thread.Sleep(300);
					
					
					
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
					rm.form.addLogText("動画ファイルの取得中に何らかの問題が発生しました " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			
			addDebugBuf("ts getter end gotTsList len " + gotTsList.Count);
		}
		private void startTsWriter() {
			while (rfu == rm.rfu || gotTsList.Count != 0) {
				try {
	//				addDebugBuf("ts writer loop gotTsListCount " + gotTsList.Count);
					var isTsGetterEnd = (tsGetterTask.IsCanceled ||
							tsGetterTask.IsCompleted ||
							tsGetterTask.IsFaulted);
					if (isTsGetterEnd && gotTsList.Count == 0) {
						addDebugBuf("startTsWriter end isTsGetterEnd " + isTsGetterEnd + " gotTsList.count " + gotTsList.Count);
						break;
					}
					
					var count = gotTsList.Count;
					numTaskInfo firstNukeFmp4WriteNti = null;
					foreach (var s in new List<numTaskInfo>(gotTsList)) {
						addDebugBuf("s " + s + " s == null " + (s == null) + " gotTsListCount " + gotTsList.Count);
						if (s == null) {
							gotTsList.Remove(s);
							#if DEBUG
								rm.form.addLogText("ts writer s null");
							#endif
							continue;
						}
						
						addDebugBuf("s.no " + s.no.ToString() + " s.originNo " + s.originNo + " lastsegmentno " + lastSegmentNo.ToString() + " " + baseNo);
						if (s.no <= lastSegmentNo) {
							gotTsList.Remove(s);
							continue;
						}
						
						if (s.res == null) {
							gotTsList.Remove(s);
							gotTsMaxNo = lastSegmentNo;
							continue;
						}
						
						if (!isPlayOnlyMode) {
							var isNuke = lastSegmentNo != -1 && s.no - lastSegmentNo > 1;
							
							if (isWriteCancel) return;
							if (ri.isFmp4 && isNuke && lastWriteFmp4Nti != null) {
								//writeComplementMp4File(s);
								firstNukeFmp4WriteNti = s;
							}
						}
					}
					if (firstNukeFmp4WriteNti != null) writeComplementMp4File(firstNukeFmp4WriteNti);
					
					var isRenketuRecord = segmentSaveType == 0;
					if (isRenketuRecord) {
						tsListRenketuWrite();
						if (count > 0 && !isPlayOnlyMode) setRecordState();
						Thread.Sleep(gotTsList.Count < 10 ? 2000 : 1000);
						continue;
					}
					count = gotTsList.Count;
					foreach (var s in new List<numTaskInfo>(gotTsList)) {
						var isNuke = lastSegmentNo != -1 && s.no - lastSegmentNo > 1;
						
						bool ret;
						if (isPlayOnlyMode) ret = true; 
						else {
							var before = DateTime.Now;
							ret = writeFile(s);
							addDebugBuf("write time " + (DateTime.Now - before));
							if (DateTime.Now - before > TimeSpan.FromSeconds(2))
								Thread.Sleep(1000);
						}
						
						if (ret) {
							if (wr != null && wr.firstSegmentSecond == -1) 
								wr.firstSegmentSecond = s.startSecond;
							
							addDebugBuf("write ok " + s.no + " origin " + s.originNo + " " + s.fileName);
							if (rfu.subGotNumTaskInfo != null)
								addDebugBuf("subGotTs count " + rfu.subGotNumTaskInfo.Count);
							
							
							if (isNuke) {
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
							lastWroteFileSecond = s.second;
							if (ri.isFmp4) lastWriteFmp4Nti = s;
							var fName = util.getRegGroup(s.fileName, ".*(\\\\|/|^)(.+)", 2, rm.regGetter.getFName());
	//							if (fName == 
							lastRecordedSeconds = util.getSecondsFromStr(fName);
							
							if (rfu.subGotNumTaskInfo != null)
								deleteOldSubTs();
						} else {
							Thread.Sleep(1000);
							break;
						}
						gotTsList.Remove(s);
						Thread.Sleep(100);
					}
					if (renketuRealTimeFS != null) renketuRealTimeFS.Flush();
					if (count > 0 && !isPlayOnlyMode) setRecordState();
	//				gotTsList.Clear();
					
					Thread.Sleep(300);
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
					rm.form.addLogText("動画データの書き込み中に何らかの問題が発生しました " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			addDebugBuf("ts writer end gotTsList len " + gotTsList.Count);
		}
		void tsListRenketuWrite() {
			var writeNti = new List<numTaskInfo>();
			
			numTaskInfo nukeS = null;
			var count = gotTsList.Count;
			for (var i = 0; i < count; i++) {
				var s = gotTsList[i];
				var isNuke = lastSegmentNo != -1 && 
					(writeNti.Count == 0 ? 
					 	(s.no - lastSegmentNo > 1) :
					 	(s.no - writeNti[writeNti.Count - 1].no > 1));
				
				if (!isPlayOnlyMode) {
					if (writeNti.Count >= 10) break;
					if (wr.sync == 0 && writeNti.Count > 0) break;
					writeNti.Add(s);
					
					if (isNuke) {
						nukeS = s;
						break;
					}
				}
				
			}
			if (writeNti.Count == 0) return;
			
			var writeBList = new List<byte>();
			for (var i = 0; i < writeNti.Count; i++) {
				var nti = writeNti[i];
				try {
					writeBList.AddRange(nti.res);
				} catch (Exception e) {
					rm.form.addLogText(e.Message + e.Source + e.Source);
					writeNti.RemoveRange(i, writeNti.Count - i);
					break;
				}
			}
			var allSecond = writeNti.Select(x => x.second).Sum();
			var _writeNti = new numTaskInfo(writeNti[0].no, "", allSecond, "", writeNti[0].startSecond, writeNti[0].originNo);
			_writeNti.res = writeBList.ToArray();
			
			var before = DateTime.Now;
			var ret = writeFile(_writeNti);
			addDebugBuf("write time " + (DateTime.Now - before));
			
			if (ret) {
				//var s = writeNti[writeNti.Count - 1];
				if (wr != null && wr.firstSegmentSecond == -1) 
					wr.firstSegmentSecond = writeNti[0].startSecond;
				
				if (rfu.subGotNumTaskInfo != null)
					addDebugBuf("subGotTs count " + rfu.subGotNumTaskInfo.Count);
				
				if (nukeS != null) {
					addDebugBuf("nuke ari s.no " + nukeS.no + " lastsegmentno " + lastSegmentNo + " s.originNo " + nukeS.originNo);
					var _lastWroteSegmentDt = lastWroteSegmentDt;
					var _lastSegmentNo = lastSegmentNo;
					//Task.Run(() => dropSegmentProcess(s, _lastWroteSegmentDt, _lastSegmentNo));
					dropSegmentProcess(nukeS, _lastWroteSegmentDt, _lastSegmentNo);
				}
				
				foreach (var nti in writeNti) {
					addDebugBuf("renketu write ok " + nti.no + " origin " + nti.originNo + " second " + nti.second);
					recordedNo.Add(nti.fileName);
					lastSegmentNo = nti.no;
					lastWroteSegmentDt = nti.dt;
					lastWroteFileSecond = nti.second;
					if (ri.isFmp4) lastWriteFmp4Nti = nti;
					var fName = util.getRegGroup(nti.fileName, ".*(\\\\|/|^)(.+)", 2, rm.regGetter.getFName());
					lastRecordedSeconds = util.getSecondsFromStr(fName);
					
					recordedSecond += nti.second;
					recordedBytes += nti.res.Length;
				}
				
				if (rfu.subGotNumTaskInfo != null)
					deleteOldSubTs();
			} else {
				Thread.Sleep(1000);
				return;
			}
			foreach (var nti in writeNti)
				gotTsList.Remove(nti);
			//if (renketuRealTimeFS != null)
			//	renketuRealTimeFS.Flush();
			Thread.Sleep(100);
		}
		/*
		private void syncCheck() {
			while (!isEnd) {
				if (DateTime.Now - lastsync > TimeSpan.FromSeconds(15)) {
				         	if (!wr.isJikken) {
				         		var url = hlsSegM3uUrl.Replace("ts/playlist.m3u8", "stream_sync.json");
				         		addDebugBuf(util.getPageSource(url, null, referer, false, 2000, ua));
							}

					lastsync = DateTime.Now;
				}
				Thread.Sleep(5);
			}
		}
		*/
		/*
		private void renketuRecord() {
//				string[] command = { 
//						"-i", "" + url + "", 
//						"-c", "copy", "-bsf:a",
//						"aac_adtstoasc", "\"" + recFolderFile[1] + ".mp4\""};
				var tuika = rm.cfg.get("ffmpegopt");
				var tuikaArr = tuika.Split(' ');
				string[] _command = {
						"-i", "" + hlsMasterUrl + "", 
						"-c", "copy", "\"" + recFolderFile + ext + "\"" };
//						"-c", "copy", "-bsf:a", "aac_adtstoasc", "\"" + recFolderFile[1] + ext + "\"" };
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
		*/
		private bool streamRenketuRecord(numTaskInfo info) {
			try {
				addDebugBuf(info.no + " " + info.url);
				addDebugBuf("record file path " + recFolderFile + ext);
				
				addDebugBuf("test record file path " + recFolderFile + ext);
				
				var isWriteInitMp4 = !File.Exists(recFolderFile + ext) && initMp4 != null;
				var isRealTimeOpenOnce = true && !ri.si.isTimeShift;
				if (isRealTimeOpenOnce) {
					addDebugBuf("isRalTimeOpenOnce write " + info.no);
					if (renketuRealTimeFS == null) 
						renketuRealTimeFS = new FileStream(recFolderFile + ext, FileMode.Append, FileAccess.Write);
					if (isWriteInitMp4) renketuRealTimeFS.Write(initMp4, 0, initMp4.Length);
					renketuRealTimeFS.Write(info.res, 0, info.res.Length);
				} else {
					//file lock check
					if (File.Exists(recFolderFile + ext)) {
						using (var checkIO = new FileStream(recFolderFile + ext, FileMode.Append, FileAccess.Write)) {
						}
					}
					
					//var isWriteInitMp4 = !File.Exists(recFolderFile + ext) && initMp4 != null;
					using (var w = new FileStream(recFolderFile + ext, FileMode.Append, FileAccess.Write)) {
						w.Seek(0, SeekOrigin.End);
						if (isWriteInitMp4) w.Write(initMp4, 0, initMp4.Length);
						
						util.debugWriteLine("streamRenketuRecord cc　" + info.res.Length);
						w.Write(info.res, 0, info.res.Length);
						//w.Close();
						util.debugWriteLine("streamRenketuRecord bb");
						w.Flush(true);
						util.debugWriteLine("streamRenketuRecord aa");
					}
				}
				if (ri.si.isTimeShift) {
					var newName = newTimeShiftFileName(recFolderFile, info.fileName);
					while (true) {
						try {
							File.Move(recFolderFile + ext, newName + ext);
							break;
						} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
							rm.form.addLogText("ファイルの書き込み後の処理に失敗しました。リトライします。" + recFolderFile + ext + " " + newName + ext);
							Thread.Sleep(1000);
						}
					}
					recFolderFile = newName;
				}
				return true; 
			} catch (Exception e) {
				addDebugBuf(e.Message+e.StackTrace + e.Source + e.TargetSite);
				rm.form.addLogText("セグメントファイルを連結して書き込み中に何らかの問題が発生しました。 " + e.Message + e.StackTrace);
				Thread.Sleep(1000);
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
			addDebugBuf("wait for recording end");
		}
		public void waitForEnd() {
			rm.form.addLogTextDebug("record waitend ");
			addDebugBuf("wait for rec end");
			isRetry = false;
			while(!isEnd) {
				Thread.Sleep(200);
			}
			addDebugBuf("wait end for rec end");
		}
		private string getHlsSegM3uUrl(string masterUrl) {
			addDebugBuf("master m3u8 " + masterUrl);
			//var wc = new WebHeaderCollection();
			testWebRequest(masterUrl);
			var res = util.getPageSource(masterUrl, null, referer, false, 2000, ua);
			if (res == null) {
				addDebugBuf("getHlsSegM3uUrl res null reconnect " + masterUrl);
				addDebugBuf("master url res null");
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
		private double addNewTsTaskList(string hlsSegM3uUrl) {
			addDebugBuf("addNewTsTaskList " + hlsSegM3uUrl);
//			var wc = new WebHeaderCollection();

			addDebugBuf("getpage mae");
			
			var res = !ri.si.isChannelPlus ? 
					//util.getPageSource(hlsSegM3uUrl, null, referer, false, 2000, ua) :
					getPageSourceRec(hlsSegM3uUrl) :
					getM3u8Curl.getStr(hlsSegM3uUrl, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_3);
			addDebugBuf("addNewTsTaskList segm3u get len 1000 " + (res == null ? "" : (res.Substring(0, res.Length > 2000 ? 2000 : res.Length))));
//			util.debugWriteLine("m3u8 " + res);
			
			 
			//if (otstul != null && !otstul.isStarted) outputTimeShiftTsUrlList(res);
			
			
			//shuusei? 
			var n = res == null ? null : util.getRegGroup(res, "(\\d+)" + ext, 1, rm.regGetter.getTs());
			if (n == null) {
				setReconnecting(true); 
				reConnect();
				return 3;	
			}
			int min = (res == null) ? -1 : int.Parse(n);
			//if (res == null || (lastSegmentNo != -1 && res.IndexOf(lastSegmentNo.ToString()) == -1)) {
			if (res == null || (lastSegmentNo != -1 && min != -1 && min > lastSegmentNo)) {
			//if (res == null) {
				addDebugBuf("nuke? reconnect lastSegmentNo " + lastSegmentNo + " min " + " min " + min + " res " + res);
//				rm.form.addLogText("リトライ lastSegmentNo " + lastSegmentNo + " res " + res + " min " + min);
				addDebugBuf("ts task res " + res + " / lastsegno " + lastSegmentNo + " / min " + min + " reconnect");
				lateTime = 100;
				setReconnecting(true);
//				if (!isReConnecting) 
					reConnect();
				
				return 3.0;
			}
			var isTimeShiftPlaylist = res.IndexOf("#STREAM-DURATION") > -1;
			if (!ri.si.isTimeShift && isTimeShiftPlaylist) {
				return -1;
			}
			double streamDuration = -1; 
			var _streamDuration = util.getRegGroup(res, "#STREAM-DURATION:(.+)");
			if (_streamDuration != null) {
				streamDuration = double.Parse(_streamDuration, NumberStyles.Float);
				if (ri.timeShiftConfig.endTimeSeconds < 0)
					ri.timeShiftConfig.endTimeSeconds = (int)(streamDuration + 15);
			}
			
			var baseTime = getBaseTimeFromPlaylist(res);
			var second = 0.0;
			var secondSum = 0.0;
			var targetDuration = 2.0;
			lock(recordLock) {
				bool _isRetry = isRetry, _isEndProgram = isEndProgram, _isEnd = isEnd;
				var lastListSegNo = -1;
				foreach (var s in res.Split('\n')) {
					if (ri.si.isChannelPlus && newGetTsTaskList.Count > 2) break;
					
					var _second = util.getRegGroup(s, "^#EXTINF:(\\d+(\\.\\d+)*)");
					if (_second != null) {
						second = double.Parse(_second);
						secondSum += second;
					}
					var _targetDuration = util.getRegGroup(s, "^#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*(e\\d+)*)");
					if (_targetDuration != null) {
						targetDuration = double.Parse(_targetDuration, NumberStyles.Float);
					}
					var isEndList = s.IndexOf("#EXT-X-ENDLIST") > -1;
					if (isEndList) {
						_isRetry = false;
						_isEndProgram = true;
					}
					var _allDuration = util.getRegGroup(s, "^#STREAM-DURATION:(.+)");
					if (_allDuration != null) {
						allDuration = double.Parse(_allDuration, NumberStyles.Float);
					}
					if (!isSavedKey && s.IndexOf("#EXT-X-KEY:METHOD=") > -1) {
						var key = util.getRegGroup(s, "URI=\"(.+?)\"");
						if (key != null) {
							saveKey(key);
						}
					}
					
					if (s.IndexOf(ext) < 0) continue;
					var no = lastListSegNo = !ri.si.isChannelPlus ? 
							//int.Parse(util.getRegGroup(s, ".+\\D(\\d+)")) :
							int.Parse(util.getRegGroup(s, "(\\d+)")) :
							int.Parse(util.getRegGroup(s, "/(\\d{5})/") + util.getRegGroup(s, ".+\\D(\\d+)"));
					var url = baseUrl + s;
					
					var isInList = false;
					lock (newGetTsTaskList) {
						foreach (var t in newGetTsTaskList)
							if (t.no == no) isInList = true;
					}
					
					if (isEndTimeshift(streamDuration, res, second) && !ri.isChase) {
						addDebugBuf("isEndTimeshift true");
						_isRetry = false;
						_isEndProgram = true;
						lastSegmentNo = no;
						break;
					}
					
					util.debugWriteLine("ri.timeShiftConfig type time " + ri.timeShiftConfig.timeType + " " + ri.timeShiftConfig.timeSeconds);
					var startTime = baseTime + secondSum - second;
					var startTimeStr = util.getSecondsToStr(startTime);
					if (lastSegmentNo == -1 && ri.timeShiftConfig.timeSeconds != 0) {
						if (recFolderFile.IndexOf(startTimeStr) > -1)
							lastSegmentNo = no;
					}
					
					var isBeforeTsConfigStartT = ri.si.isTimeShift && 
					    	((ri.timeShiftConfig.timeType == 0 && startTime < ri.timeShiftConfig.timeSeconds) ||
					     	(ri.timeShiftConfig.timeType == 1 && startTime <= ri.timeShiftConfig.timeSeconds));
					if (isBeforeTsConfigStartT && 
							(ri.timeShiftConfig.endTimeSeconds > 0 && startTime + second >= ri.timeShiftConfig.endTimeSeconds)) {
						isBeforeTsConfigStartT = false;
					}
					if (isBeforeTsConfigStartT) continue;
					if (ri.si.isTimeShift && ri.timeShiftConfig.endTimeSeconds > 0 && startTime >= ri.timeShiftConfig.endTimeSeconds) {
						addDebugBuf("timeshift ri.timeShiftConfig.endtime " + ri.timeShiftConfig.endTimeSeconds + " now starttime " + startTime + " ri.timeShiftConfig.timeseconds " + ri.timeShiftConfig.timeSeconds);
						_isRetry = false;
						_isEndProgram = true;
						continue;
					}
					
					
					if (no > lastSegmentNo && !isInList) {
						util.debugWriteLine("add newGetTsTaskList no " + no + " lastsegNo " + lastSegmentNo);
						if (engineMode == "3") {
							if (wr.firstSegmentSecond == -1) 
								wr.firstSegmentSecond = startTime;
							_isRetry = false;
							_isEnd = _isEndProgram = true;
							break;
						}
						
						var fileName = util.getRegGroup(s, "\\D*(.+?" + ext + ")");
						fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ext;
						addDebugBuf(no + " " + fileName);
						
						newGetTsTaskList.Add(new numTaskInfo(no, url, second, fileName, startTime));
						if (ri.si.isChannelPlus) Thread.Sleep(5000);
						//Task.Run(() => getTsTask(url, startTime));
						//getTsTask(url, startTime);
					}
					
					if (ri.isChase && hlsSegM3uUrl.IndexOf("hlsarchive") > -1) {
						addDebugBuf("ischase segM3uUrl hlsarchive end");
						_isRetry = false;
//						isEndProgram = true;
					}
					
				}
				
				if (newGetTsTaskList.Count != 0) {
					try {
						getNtiListRes();
						
						if (ri.si.isChannelPlus && channelPlusTimeShiftTotalTime == -1)
							channelPlusTimeShiftTotalTime = getChannelPlusTimeShiftTotalTime(res);
						for (var i = 0; i < newGetTsTaskList.Count; i++) {
							if (newGetTsTaskList.Count == 0) break;
							getTsTask(newGetTsTaskList[0], newGetTsTaskList[0].res);
							
						}
					} catch (Exception e) {
						addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
					}
				}
				
				if (lastSegmentNo == lastListSegNo && !_isRetry) {
					isRetry = _isRetry;
					isEndProgram = _isEndProgram;
					isEnd = _isEnd;
				}
				
				
				if (!ri.si.isChannelPlus) {
					lateTime = streamDuration - lastSegmentNo / 1000;
					util.debugWriteLine("lateTime " + lateTime);
					if (lateTime > 11 || !ri.isChase) {
						if (!isSpeedUp) setSpeed(true);
					} else if (lateTime < 7 && false) {
						if (isSpeedUp) {
							setSpeed(false);
						}
					}
					if (ri.isChase) {
						if (hlsSegM3uUrl.IndexOf("start=") > -1) {
							if (lateTime < 15) {
								setReconnecting(true);
								reConnect();
							}
						} else {
							if (lateTime > 15) {
								setReconnecting(true);
								reConnect();
							}
						}
					}
				}
				
				addDebugBuf(rm.form.getKeikaTime());
			}
			return targetDuration;
		}
		private void getTsTask(numTaskInfo nti, byte[] tsBytes) {
			addDebugBuf("getTsTask url " + nti.url);
			var url = nti.url;
			var startTime = nti.startSecond;
			
			try {
				lock (newGetTsTaskList) {
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].url == url) {
							if (tsBytes == null || (!ri.si.isChannelPlus && lastSegmentNo > 10000 && newGetTsTaskList[0].no - lastSegmentNo > 5500)) {
								newGetTsTaskList.Clear();
								if (!isReConnecting) {
									addDebugBuf("getTsTask !isReconnecting reconnect");
									//debug
									util.debugWriteLine("get bytes " + tsBytes + " / lastsegno " + lastSegmentNo + "/ no " + (newGetTsTaskList != null && newGetTsTaskList.Count > 0 ? newGetTsTaskList[0].no : -10));
									
									reConnect();
								}
//								rm.form.addLogText("セグメント取得エラー " + url);
								setReconnecting(true);
								break;
							}
						}
							
					}
					var recordedNti = new List<numTaskInfo>();
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						addDebugBuf("write file " + i + " url " + newGetTsTaskList[i].no + " lastsegNo " + lastSegmentNo);
						if (newGetTsTaskList[i].res == null) break;
						
						if (newGetTsTaskList[i].no <= lastSegmentNo) {
							continue;
						}
						bool ret;
						if (isPlayOnlyMode) ret = true;
						else {
							if (isWriteCancel) return;
							
							if (ri.isChase) {
								lock (gotTsList) {
									gotTsList.Add(newGetTsTaskList[i]);
								}
								ret = true;
							}
							else {
								ret = writeFile(newGetTsTaskList[i]);
							}
						}

						if (ret) {
							addDebugBuf("write ok " + ret + " " + newGetTsTaskList[i].no);
							if (wr.firstSegmentSecond == -1) 
								wr.firstSegmentSecond = newGetTsTaskList[i].startSecond;
							
							var a = recordedSecond;
							if (!ri.isChase) {
								recordedSecond += newGetTsTaskList[i].second; 	
								recordedBytes += newGetTsTaskList[i].res.Length;
							}
							var b = recordedSecond;
							addDebugBuf("aads " + a + " " + b + " no " + newGetTsTaskList[i].no);
							
							//recordedNo.Add(newGetTsTaskList[i].no.ToString());
							recordedNo.Add(newGetTsTaskList[i].fileName);
							lastSegmentNo = newGetTsTaskList[i].no;
							var fName = util.getRegGroup(newGetTsTaskList[i].fileName, ".*(\\\\|/|^)(.+)", 2);
//							if (fName == 
							lastRecordedSeconds = util.getSecondsFromStr(fName);
							lastWroteFileSecond = newGetTsTaskList[i].second;
							
							util.debugWriteLine("aaaaa " + ri.timeShiftConfig.endTimeSeconds + " " + startTime + " " +  newGetTsTaskList[i].startSecond + " " + nti.second + " a " + (startTime + nti.second) + " " + (startTime + nti.second >= ri.timeShiftConfig.endTimeSeconds) + " " + newGetTsTaskList.Count + " " + i);
							if (ri.si.isTimeShift && ri.timeShiftConfig.endTimeSeconds > 0 && newGetTsTaskList[i].startSecond + nti.second >= ri.timeShiftConfig.endTimeSeconds) {
								addDebugBuf("getTsTask timeshift ri.timeShiftConfig.endtime " + ri.timeShiftConfig.endTimeSeconds + " now starttime " + startTime + " ri.timeShiftConfig.timeseconds " + ri.timeShiftConfig.timeSeconds);
								isRetry = false;
								isEndProgram = true;
								if (!ri.timeShiftConfig.isDeletePosTime) isTsEndTimeEnd = true;
								isEnd = true;
								continue;
							}
							recordedNti.Add(newGetTsTaskList[i]);
							
						} else {
							newGetTsTaskList.Clear();
							if (!isReConnecting) {
								addDebugBuf("getTsTask write ret false reconnect");
								addDebugBuf("not write");
								reConnect();
							}
							setReconnecting(true);
							break;
						}
						
					}
					addDebugBuf("write ok2 " + nti.no);
					foreach (var _nti in recordedNti) newGetTsTaskList.Remove(_nti);
					/*
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].res != null) 
							newGetTsTaskList.RemoveAt(i);
					}
					*/
				}
				if (!isPlayOnlyMode && !ri.isChase)
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
			addDebugBuf("setRecordState " + recordedBytes + " " + recordedSecond);
			var ret = "";
			var bytes = recordedBytes;
			//long b = bytes % 1000;
			//long kb = (bytes % 1000000) / 1000;
			//long mb = (bytes % 1000000000) / 1000000;
			//long gb = (bytes % 1000000000000) / 1000000000;
			//string _kb = ((int)(bytes / 1000)).ToString("n0");
			 
			//for (var i = 0; i < 9 - _kb.Length; i++)
			//	_kb = " " + _kb;
			//ret += "size=" + _kb + "kB\n";
			var size = bytes > 1000000 ? 
					((bytes / 1000000.0).ToString("n3") + "MB") :
					(((int)(bytes / 1000)).ToString() + "KB");
			ret += "size=   " + size + "\n";
			
			
//			recordedSecond = 400000;
			var dotSecond = ((int)((recordedSecond % 1) * 100)).ToString("00");
			var second = ((int)((recordedSecond % 60) * 1)).ToString("00");
			var minute = ((int)((recordedSecond % 3600 / 60))).ToString("00");
			var hour = ((int)((recordedSecond / 3600) * 1));
			var _hour = (hour < 100) ? hour.ToString("00") : hour.ToString();;
			var timeStr = _hour + ":" + minute + ":" + second + "." + dotSecond;
			ret += "time= " + timeStr + "\n";
			
			var bitrate = recordedBytes * 8 / recordedSecond / 1000;
			ret += "bitrate= " + bitrate.ToString("0.0") + "kbits/s";
			
			int per = 0;
			if (ri.si.isTimeShift && !ri.isChase) {
				if (!ri.si.isChannelPlus) {
					per = (int)(((lastSegmentNo + 5200) / 10) / allDuration);
					if (per > 100) per = 100;
					ret = "(" + per + "%) " + ret;
				} else {
					if (ri.si.isTimeShift && channelPlusTimeShiftTotalTime != -1) {
						var _per = (int)((1.0 * (lastRecordedSeconds + 10.2) / channelPlusTimeShiftTotalTime) * 100);
						per = _per > 100 ? 100 : (_per < 0 ? 0 : _per);
						ret = "(" + per + "%)" + ret;
					} else {
						ret = "(_%)" + ret;
						per = 0;
					}
					
				}
			}
			var titleT = ret.Replace('\n', ' ');
			//ret += "(" + percent + ")" + (lastSegmentNo / 10) + " " + allDuration;
			rm.form.setRecordState(ret, titleT, per);
			RecordLogInfo.recordedStatus = ret;
		}
		private bool writeFile(numTaskInfo info) {
			var ret = false;
			if (!checkFreeSpace(info)) return false;
			RecordLogInfo.addrecBytesData(info);
			if (segmentSaveType == 0) {
				ret = streamRenketuRecord(info);
			} else {
				ret = originalTsRecord(info);
			}
			
			if (wr.sync == 0 && wr != null) {
				wr.setSync(
					info.no, info.second,hlsSegM3uUrl);
			}
			return ret;
		}
		private bool originalTsRecord(numTaskInfo info) {
			var path = recFolderFile + "/" + 
//				util.getRegGroup(info.url, ".+/(.+?)\\?");
				info.fileName;
			addDebugBuf("original ts record " + path);
			try {
				using (var w = new FileStream(path, FileMode.Create, FileAccess.Write)) {
					if (initMp4 != null) w.Write(initMp4, 0, initMp4.Length);
					w.Write(info.res, 0, info.res.Length);
					//w.Close();
				}
				return true; 
			} catch (Exception e) {
				addDebugBuf("original ts record exception " + e.Message+e.StackTrace + e.Source + e.TargetSite);
				rm.form.addLogText("セグメントファイルを連結せずに書き込み中に何らかの問題が発生しました。 " + e.Message + e.StackTrace);
				return false;
			}
		}
		public void reSetHlsUrl(string url, string quality, WebSocket _ws, bool isChaseRealing) {
			addDebugBuf("resetHlsUrl oldurl " + hlsMasterUrl + " new url " + url);
			ws = _ws;
			if (recordingQuality != quality)
				rm.form.addLogText("画質を変更して再接続します(" + quality + ")");
			recordingQuality = quality;
			
			if (!ri.si.isChannelPlus) {
				hlsMasterUrl = url;
				if (ri.si.isTimeShift && 
				    	!(ri.isChase && lateTime < 15)) {
					var start = 0;
					//var recorded = lastRecordedSeconds < lastSegmentNo / 1000 ? lastRecordedSeconds : lastSegmentNo; 
					if (lastRecordedSeconds == -1) {
						start = (ri.timeShiftConfig.timeSeconds - 10 < 0) ? 0 : (ri.timeShiftConfig.timeSeconds - 10);
					} else {
						if (isChaseRealing)
							start = (int)lastRecordedSeconds + 20;
						else start = ((int)lastRecordedSeconds - 10 < 0) ? 0 : ((int)lastRecordedSeconds - 10);
					}
					
					hlsMasterUrl = hlsMasterUrl + "&start=" + (start.ToString());
					
					wr.tsHlsRequestTime = DateTime.Now;
					wr.tsStartTime = TimeSpan.FromSeconds((double)start);
				}
				hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
			} else {
				hlsSegM3uUrl = url;
			}
			
			setReconnecting(false);
			isSpeedUp = false;
		}
		private void renketuAfter() {
			var isFFmpegRenketuAfter = false;
			if (isFFmpegRenketuAfter) {
				ffmpegRenketuAfter();
			} else {
				rm.form.addLogText("連結処理を開始します");
				var outFName = streamRenketuAfter();
				rm.form.addLogText("連結処理を完了しました");
				
//				if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
//				   	int.Parse(rm.cfg.get("afterConvertMode")) > 1) {
				if (int.Parse(rm.cfg.get("afterConvertMode")) > 0) {
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
				//m3u8 += "#EXTINF:0\n" + recFolderFile + "/" + s + ext + "\n";
				m3u8 += "#EXTINF:0\n" + recFolderFile + "/" + s + "\n";
			m3u8 += "#EXT-X-ENDLIST\n";
			var pipeName = DateTime.UtcNow.Hour + "" + DateTime.UtcNow.Minute + "" + DateTime.UtcNow.Second;
			string args = "-i \\\\.\\pipe\\" + pipeName + " -c copy \"" + recFolderFile + "/" + fName + ext + "\"";
			
			var r = new FFMpegConcat(rm, rfu);
			r.recordCommand(args.Split(' '), m3u8, pipeName);
		}
		private string streamRenketuAfter() {
			try {
				return new ArgConcat(rm, new string[]{recFolderFile}).concat();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
			/*
			var fName = util.getRegGroup(recFolderFile, ".+/(.+)");
			var outFName = recFolderFile + "/" + fName + ext;
			
			FileStream w; 
			try {
				addDebugBuf("renketu after out fname " + outFName);			
				if (outFName.Length > 245) outFName = recFolderFile + "/" + ri.si.lvid + ext;
				if (outFName.Length > 245) outFName = recFolderFile + "/out" + ext;
				addDebugBuf("renketu after out fname shuusei go " + outFName);
				using (w = new FileStream(outFName, FileMode.Append, FileAccess.Write)) {
					_streamRenketuAfterWrite(w);
					return w.Name;
				}
			} catch (PathTooLongException) {
				try {
					addDebugBuf("renketu after out fname " + recFolderFile + "/" + ri.si.lvid + ext);			
					using (w = new FileStream(recFolderFile + "/" + ri.si.lvid + ext, FileMode.Append, FileAccess.Write)) {
						_streamRenketuAfterWrite(w);
						return w.Name;
					}
				} catch (PathTooLongException) {
					try {
						addDebugBuf("renketu after out fname " + recFolderFile + "/_" + ext);			
						using (w = new FileStream(recFolderFile + "/_" + ext, FileMode.Append, FileAccess.Write)) {
							_streamRenketuAfterWrite(w);
							return w.Name;
						}
					} catch (PathTooLongException) {
						addDebugBuf("renketu after too long");
						rm.form.addLogText("録画後に連結しようとしましたがパスが長すぎてファイルが開けませんでした " + recFolderFile + "/_" + ext);
						return null;
					}
				}
			}

			//w.Close();
			//return w.Name;
			*/
		}
		private void _streamRenketuAfterWrite(FileStream w) {
			foreach (var s in recordedNo) {
				addDebugBuf(s);
				try {
					using (var r = new FileStream(recFolderFile + "/" + s + "", FileMode.Open, FileAccess.Read)) {
						
						var pos = 0;
						var readI = 0;
						var bytes = new byte[1000000];
						while((readI = r.Read(bytes, 0, bytes.Length)) != 0) {
							w.Write(bytes, 0, readI);
							pos += readI;
						}
						//r.Close();
					}
				} catch (Exception e) {
					addDebugBuf("renketu after write exception " + s + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private void timeShiftOnTimeRecord() {
			/*
			if (ri.timeShiftConfig.isOutputUrlList) {
				setOutputTimeShiftTsUrlListStartTime();
			}
			*/
			if (hlsSegM3uUrl == null) {
				var start = (ri.timeShiftConfig.timeSeconds - 10 < 0) ? 0 : (ri.timeShiftConfig.timeSeconds - 10);
				var baseMasterUrl = hlsMasterUrl;
				if (!ri.isRealtimeChase) baseMasterUrl += "&start=" + (start.ToString());
				
				wr.tsHlsRequestTime = DateTime.Now;
				wr.tsStartTime = TimeSpan.FromSeconds((double)start);
				hlsSegM3uUrl = getHlsSegM3uUrl(baseMasterUrl);
			}
			rm.hlsUrl = hlsSegM3uUrl;
			rm.form.setPlayerBtnEnable(true);
			
			var isWriteEnd = new bool[1]{false};
			if (engineMode == "0" && ri.isChase)
				tsWriterTask = Task.Factory.StartNew(() => timeShiftWriter(isWriteEnd), TaskCreationOptions.LongRunning);
			var isFirst = true;
			while (rm.rfu == rfu && isRetry) {
				if (isReConnecting) {
					Thread.Sleep(3000);
					continue;
				}
				if (hlsSegM3uUrl == null) {
					addDebugBuf("hlsSegM3uUrl null reconnect");
					addDebugBuf("hls segM3u8 null");
					setReconnecting(true);
					reConnect();
					continue;
				}
				
				if (engineMode == "0" || engineMode == "3") {

					targetDuration = addNewTsTaskList(hlsSegM3uUrl);

					if (engineMode == "3" && 
					    	wr.tscg != null && 
					    	wr.tscg.isEnd) {
						isRetry = false;
						isEndProgram = true;
					}
					var intervalTime = (int)(targetDuration * (ri.isRealtimeChase ? 500 : 1000));
					var elapsedTime = (int)((TimeSpan)(DateTime.Now - lastAccessPlaylistTime)).TotalMilliseconds;
					util.debugWriteLine("wait time intervalTime " + intervalTime + " elapsedTime " + elapsedTime + " recordedSeconds " + recordedSecond);
					Thread.Sleep(elapsedTime > intervalTime ? 0 : intervalTime - elapsedTime);
					lastAccessPlaylistTime = DateTime.Now;
				} else {
					if (!isFirst) wr.resetCommentFile();
					isFirst = false;
					
					var recStartTime = DateTime.Now;
					var startPlayList = !ri.si.isChannelPlus ? 
							util.getPageSource(hlsSegM3uUrl, null, referer, false, 2000, ua) :
							getM3u8Curl.getStr(hlsSegM3uUrl, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_3);
					
					var _currentPos = util.getRegGroup(startPlayList, "#CURRENT-POSITION:(\\d+)");
					wr.firstSegmentSecond = (_currentPos == null) ? 0 : double.Parse(_currentPos, NumberStyles.Float);
					var aer = new AnotherEngineRecorder(rm, rfu, this);
					setSpeed(true);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					if (isEndProgram || isAnotherEngineTimeShiftEnd(recStartTime, hlsSegM3uUrl, startPlayList) && !ri.isRealtimeChase) {
						isEndProgram = true;
						break;
					}
					
					//recFolderFile = wr.getRecFilePath()[1];
					recFolderFile = util.incrementRecFolderFile(recFolderFile);
					 
					setReconnecting(true);
					reConnect();
					continue;
					
				}
				
			}
			rm.hlsUrl = "end";
			isWriteEnd[0] = true;
			
//			rm.form.setPlayerBtnEnable(false);
			
			
			if (rm.cfg.get("fileNameType") == "10" && (recFolderFile.IndexOf("{w}") > -1 || recFolderFile.IndexOf("{c}") > -1))
				renameStatistics();
			
			if (engineMode == "0" && !isPlayOnlyMode) {
				if (gotTsList.Count > 10) {
					displayWriteRemainGotTsData();
				}
				if (ri.isChase)
					waitForRecording();
			
				if (isEndProgram && segmentSaveType == 0 && !isTsEndTimeEnd) {
					renameWithoutTime(recFolderFile);
				}
				if (segmentSaveType == 1 &&
				    	(rm.cfg.get("IsRenketuAfter") == "true" 
//				     || int.Parse(rm.cfg.get("afterConvertMode")) > 1)
				    )) {
					addDebugBuf("renketu after");
					renketuAfter();
				} else if (segmentSaveType == 0) {
//					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || 
//				    	    int.Parse(rm.cfg.get("afterConvertMode")) > 1) {
					if (int.Parse(rm.cfg.get("afterConvertMode")) > 0 && 
					    (isEndProgram || rm.rfu != rfu)) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ext, true);
						
					}
				}
			}
			
			if (isEndProgram && !wr.isHokan) {
				rm.form.addLogText("録画を完了しました");
			}
			isEnd = true;
		}
		private void timeShiftWriter(bool[] isWriteEnd) {
			var lastWroteNo = -1;
			while (rfu == rm.rfu) {
				try {
					//util.debugWriteLine("timeshift writer + " + gotTsList.Count);
					if (isWriteEnd[0] && gotTsList.Count == 0) break;
					
					var removeNti = new List<numTaskInfo>();
					lock (gotTsList) {
						
						for (var i = 0; i < (gotTsList.Count < 5 ? gotTsList.Count : 5); i++) {
							if (gotTsList[i].no <= lastWroteNo) continue;
							
							var r = writeFile(gotTsList[i]);
							util.debugWriteLine("timeshift writer write " + r + " " + gotTsList[i].no + " " + gotTsList.Count + " " + i + " " + rm.hlsUrl);
							if (isWriteCancel) return;
							if (r) {
								recordedSecond += gotTsList[i].second;
								recordedBytes += gotTsList[i].res.Count();
								removeNti.Add(gotTsList[i]);
								lastWroteNo = gotTsList[i].no;
								
							} else {
								break;
							}
							
						}
						foreach (var n in removeNti)
							gotTsList.Remove(n);
					}
					if (removeNti.Count != 0)
						setRecordState();
					
					Thread.Sleep(300);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			util.debugWriteLine("timeshift writer end");
		}
		private double getBaseTimeFromPlaylist(string res) {
			//most extinf second
			if (res == null) {
				addDebugBuf("basetime from playlist res == null");
				#if DEBUG
					rm.form.addLogText("basetime from playlist res == null");
				#endif
			}
			var mostSegmentSecond = getMostSegmentSecond(res);
			
			var mediaSequenceNum = util.getRegGroup(res, "#EXT-X-MEDIA-SEQUENCE\\:(.+)", 1, rm.regGetter.getExtXMediaSequence());
			if (mediaSequenceNum == null) return -1;
			return mostSegmentSecond * (double.Parse(mediaSequenceNum) - (ri.si.isChannelPlus ? 1 : 0));
		}
		private double getMostSegmentSecond(string res) {
			var timeArr = new List<double[]>();
			foreach (var l in res.Split('\n')) {
				//var _second = util.getRegGroup(l, "^#EXTINF:(\\d+(\\.\\d+)*)");
				var _second = util.getRegGroup(l, "^#EXTINF:(.+),", 1, rm.regGetter.getExtInf());
				if (_second == null) continue;
				var inKey = false;
				for (var i = 0; i < timeArr.Count; i++) {
					if (timeArr[i][0] == double.Parse(_second, NumberStyles.Float)) {
						timeArr[i][1] += 1;
						inKey = true;
					}
				}
				if (!inKey) timeArr.Add(new double[2]{double.Parse(_second, NumberStyles.Float), 1});
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
			if (newFileTime == null || nowFileTime == null)
				util.debugWriteLine("newtimeshift file name null " + newFileTime + " / " + nowFileTime);
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
			while ((rm.rfu == rfu || !isEnd) || debugWriteBuf.Count != 0) {
				try {
					lock (debugWriteBuf) {
						string[] l = new String[debugWriteBuf.Count + 10];
						debugWriteBuf.CopyTo(l);
	//					var l = debugWriteBuf.ToList<string>();
						//var l = new List<string>(debugWriteBuf);
						foreach (var b in l) {
							if (b == null) continue;
							util.debugWriteLine(b);
							debugWriteBuf.Remove(b);
						}
					}
					Thread.Sleep(500);
				} catch (Exception) {
				}
			}
			util.debugWriteLine("start debug writer end rm.rfu == rfu " + (rm.rfu == rfu) + " isend " + isEnd + " debugWriteBuf.count " + debugWriteBuf.Count);
		}
		public void addDebugBuf(string s) {
			var dt = DateTime.Now.ToLongTimeString();
			debugWriteBuf.Add(dt + " " + s);
		}
		
		
		
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
				if (l.IndexOf(ext) != -1 && !l.StartsWith("#")) {
					maxNo = int.Parse(util.getRegGroup(l, "(\\d+)\\" + ext, 1, rm.regGetter.getMaxNo()));
					maxLine = l;
					if (minNo == 0) minNo = maxNo; 
				}
			}
			if (maxNo + baseNo + 3 < gotTsMaxNo) {
				addDebugBuf("base no change maxno " + maxNo + " gotTsMaxNo " + gotTsMaxNo);
				baseNo = gotTsMaxNo;
			}
			
			//if (lastGetPlayListMaxNo != 0 && minNo > lastGetPlayListMaxNo) {
			addDebugBuf("minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString() + " base no " + baseNo);
			//if (minNo > lastGetPlayListMaxNo) {
			if (minNo + baseNo > gotTsMaxNo) {
				addDebugBuf("nuke? minNo " + minNo + " lastSegmentNo " + lastSegmentNo + "lastGetPlayListMaxNo " + lastGetPlayListMaxNo.ToString() + " gottsmaxno " + gotTsMaxNo);
				if (res == null) {
					addDebugBuf("added near list res == null");
					#if DEBUG
						rm.form.addLogText("added near list from playlist res == null");
					#endif
				}
				
				var inf = getMostSegmentSecond(res);
				var ins = "";
				var sakanoboriNo = 3;
				var startNo = (minNo + baseNo - gotTsMaxNo > sakanoboriNo) ? (minNo - sakanoboriNo) : (gotTsMaxNo - baseNo + 1);
				for (var i = startNo; i < minNo; i++) {
					if (i < 0) continue;
					ins += "#EXTINF:" + inf + ",\n";
					ins += maxLine.Replace(maxNo.ToString() + ext, i.ToString() + ext) + "\n";
				}
				res = res.Insert(res.IndexOf("EXTINF:") - 1, ins);
				res = new Regex("#EXT-X-MEDIA-SEQUENCE:(\\d+)").Replace(res, "#EXT-X-MEDIA-SEQUENCE:" + startNo);
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
		/*
		private void setOutputTimeShiftTsUrlListStartTime() {
			var baseMasterUrl = hlsMasterUrl + "&start=0";
			var _hlsSegM3uUrl = getHlsSegM3uUrl(baseMasterUrl);
			var res = util.getPageSource(_hlsSegM3uUrl, null, referer, false, 2000, ua);
			
			otstul = new OutputTimeShiftTsUrlList(ri.timeShiftConfig, rm);
			otstul.setStartNum(res);
		}
		private void outputTimeShiftTsUrlList(string res) {
			Task.Run(() => {
				otstul.write(res, recFolderFile, baseUrl, ri.timeShiftConfig);
			});
		}
		*/
		private void reConnect() {
			if ((wr as WebSocketRecorder) == null) return;
			var isUseCurlWs = ((WebSocketRecorder)wr).isUseCurlWs; 
			if (!isUseCurlWs &&  (wr == null || ws == null)) return;
			wr.reConnect(ws);
		}
		private bool isEndTimeshift(double streamDuration, string res, double second) {
			if (ri.si.isChannelPlus) {
				var tsNumList = new Regex("/(\\d{5})/.+\\D(\\d+).ts").Matches(res);
				if (tsNumList.Count > 0) {
					var last = int.Parse(tsNumList[tsNumList.Count - 1].Groups[1].Value + tsNumList[tsNumList.Count - 1].Groups[2].Value);
					return lastSegmentNo == last;
				} else return true;
			}
			var ret = streamDuration - ((double)lastSegmentNo / 1000 + second) < 0.5;
			if (lastSegmentNo > 5 && lastWroteFileSecond != 5 && lastWroteFileSecond != -1 && ret) ret = true;
			addDebugBuf("isendTimeshift streamDuration " + streamDuration + " second " + second + " lastSegmentNo " + lastSegmentNo + " " + lastWroteFileSecond + " ret " + ret);
			
			if (res.IndexOf("#EXT-X-ENDLIST") > -1) {
				//var tsNumList = new Regex("(\\d+).ts\\?start").Matches(res);
				var tsNumList = new Regex("(\\d+).ts").Matches(res);
				if (tsNumList.Count > 0) {
					var last = int.Parse(tsNumList[tsNumList.Count - 1].Groups[1].Value);
					if (lastSegmentNo == last) {
						#if DEBUG
							rm.form.addLogText("isendTimeShift EXT-X-endlist end ts ret " + ret);
						#endif
						ret = true;
					}
					if (ri.si.isTimeShift && ri.timeShiftConfig != null &&
							((ri.timeShiftConfig.timeType == 0 && last / 1000 < ri.timeShiftConfig.timeSeconds) ||
					     (ri.timeShiftConfig.timeType == 1 && last / 1000 <= ri.timeShiftConfig.timeSeconds))) {
						#if DEBUG
							rm.form.addLogText("isendTimeShift EXT-X-endlist end timeseconds last" + ret);
						#endif
						ret = true;
					}
				}
			}
			return ret;
		}
		private bool isAnotherEngineTimeShiftEnd(DateTime recStartTime, string hlsSegM3uUrl, string startPlayList) {
			if (ri.si.isChannelPlus) return false;
			
			if (startPlayList == null) return false;
			var lastTsNum = util.getRegGroup(startPlayList, "[\\s\\S]+\n(\\d+)" + ext, 1, rm.regGetter.getLastTsNum());
			if (lastTsNum == null) 
				return false;
			
			double streamDuration = -1;
			var _streamDuration = (startPlayList == null) ? null : util.getRegGroup(startPlayList, "#STREAM-DURATION:(.+)", 1, rm.regGetter.getStreamDuration());
			if (_streamDuration == null) return false;
			streamDuration = double.Parse(_streamDuration, NumberStyles.Float);
			
			TimeSpan diffTime = DateTime.Now - recStartTime;
			addDebugBuf(diffTime.TotalMilliseconds.ToString());
			if (int.Parse(lastTsNum) + diffTime.TotalMilliseconds + 10000 > streamDuration * 1000)
				return true;
			return false;
			/*
			var a = new System.Net.WebHeaderCollection();
			var _resM3u8 = util.getPageSource(hlsSegM3uUrl, ref a, container);
			var _lastSegTime = (_resM3u8 == null) ? null : util.getRegGroup(_resM3u8, ".+(\\d+)" + ext);
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
			if (dsp == null) {
				dsp = new DropSegmentProcess(_lastWroteSegmentDt, _lastSegmentNo, this, ri.recFolderFile[2], rfu, rm, ri);
				if (!dsp.start(s)) dsp = null;
				//dsp = null;
			} else dsp.updateHokanEndtime();
		}
		private void deleteOldSubTs() {
			var i = 0;
			try {
				var c = rfu.subGotNumTaskInfo.Count;
				for (i = 0; i < c; i++) {
					if (rfu.subGotNumTaskInfo[0] == null) {
						rfu.subGotNumTaskInfo.RemoveAt(0);
						continue;
					}
					var t = (lastFileSecond != 0) ? (lastFileSecond * 13) : 15;
//					addDebugBuf("delete old subts i " + i + " c " + c + " count " + rfu.subGotNumTaskInfo.Count);
					if (rfu.subGotNumTaskInfo[0].dt <
					    	lastWroteSegmentDt - TimeSpan.FromSeconds(t)) {
//						addDebugBuf("delete subGotTs subTs.dt " + rfu.subGotNumTaskInfo[0].dt + " " + rfu.subGotNumTaskInfo[0].no + " originNo " + rfu.subGotNumTaskInfo[0].originNo + " lastWroteDt " + lastWroteSegmentDt);
						rfu.subGotNumTaskInfo.Remove(rfu.subGotNumTaskInfo[0]);
					}
				}
			} catch (Exception e) {
				addDebugBuf("delete old sub ts exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				addDebugBuf("lastFileSecond " + lastFileSecond + " lastWroteSegmentDt " + lastWroteSegmentDt + " i " + i);
				addDebugBuf("subgotNumTaskInfo.count " + rfu.subGotNumTaskInfo.Count + " [0] " + ((rfu.subGotNumTaskInfo.Count > 0) ? rfu.subGotNumTaskInfo[0] : null));
				#if DEBUG
					rm.form.addLogText("delete old SubTs Exception");
				#endif
			}
		}
		private Task<numTaskInfo> getFileBytesNti(numTaskInfo nti) {
			//byte[] tsBytes;
			addDebugBuf("getfilebyte " + nti.url);
			try {
				return new Task<numTaskInfo>(() => {
					nti.res = getFileBytesRec(nti.url);
                 	return nti;
                 });
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
		}
		public byte[] getFileBytesRec(string url) {
			try  {
				var h = util.getHeader();
				h.Add("Accept", "*/*");
				h.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
				h.Add("Origin", "https://live.nicovideo.jp");
				h.Add("Referer", "https://live.nicovideo.jp/");
				if (isWebRequest) {
					var r = util.sendRequest(url, h, null, "GET", null);
					if (r == null) return null;
					if (r.StatusCode != HttpStatusCode.OK)
						return null;
					
					using (var _r = r.GetResponseStream()) {
						var ms = new MemoryStream();
						_r.CopyTo(ms);
						var res = ms.ToArray();
						return res;
					}
				} else {
					string d = null;
					var res = new Curl().getBytes(url, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "GET", d, false);
					return res;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
		}
		string getPageSourceRec(string url) {
			try  {
				var r = getFileBytesRec(url);
				if (r == null) return null;
				var _r = Encoding.UTF8.GetString(r);
				return _r;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
		}
		private void displayWriteRemainGotTsData() {
			if (isWriteCancel) return;
//			Task.Run(() => {
				while (gotTsList.Count > 0 && !util.isTaskEnd(tsWriterTask)) {
					rm.form.addLogText("未書き込みのデータを書き込んでいます...(" + gotTsList.Count + "件)");
					addDebugBuf("mi kakikomi write write " + gotTsList.Count);
					Thread.Sleep(10000);
				}
//			});
			gotTsList.Clear();
		}
		private void renameWithoutTime(string name) {
			var time = util.getRegGroup(name, "(\\d+h\\d+m\\d+s)", 1, rm.regGetter.getRenameWithoutTime_time());
			var num = util.getRegGroup(name, "\\d+h\\d+m\\d+s_(\\d+)", 1, rm.regGetter.getRenameWithoutTime_num());
			
			try {
				for (int i = int.Parse(num); i < 1000; i++) {
					var newName = name.Replace("_" + time + "_" + num, i.ToString());
					if (File.Exists(newName + ext) || !File.Exists(recFolderFile + ext)) continue;
					File.Move(recFolderFile + ext, newName + ext);
					recFolderFile = newName;
					return;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void renameStatistics() {
			try {
				wr.setRealTimeStatistics();
				var visit = wr.visitCount.Replace("-", "");
				var comment = wr.commentCount.Replace("-", "");
				try {
					var nF = ri.recFolderFile[2] + "n.txt";
					if (File.Exists(nF))
						File.Move(nF, nF.Replace("{w}", visit).Replace("{c}", comment));
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				}
				
				if (File.Exists(recFolderFile + ext)) {
					var newName = recFolderFile.Replace("{w}", visit).Replace("{c}", comment);
					File.Move(recFolderFile + ext, newName + ext);
					recFolderFile = newName;
				}
				
				try {
					if (Directory.Exists(recFolderFile)) {
						wr.IsRetry = false;
						wr.stopRecording();
						var newName = recFolderFile.Replace("{w}", wr.visitCount.Replace("-", "")).Replace("{c}", wr.commentCount.Replace("-", ""));
						Directory.Move(recFolderFile, newName);
						recFolderFile = newName;
					}
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		/*
		private bool isFFmpegThrough() {
			if (engineMode == "0" && !isPlayOnlyMode) {
				if (segmentSaveType == 1 && 
				    	(rm.cfg.get("IsRenketuAfter") == "true")) {
					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
				   		int.Parse(rm.cfg.get("afterConvertMode")) > 1) {
						return true;
					}
				} else if (segmentSaveType == 0) {
					if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" || 
					    (int.Parse(rm.cfg.get("afterConvertMode")) != 0 &&
					     	int.Parse(rm.cfg.get("afterConvertMode")) != 1)) {
						return true;
					}
				}
			}
			return false;
		}
		*/
		private void setSpeed(bool isUp) {
			//return;
			
			util.debugWriteLine("setSpeed isup " +isUp);
			var speed = isUp ? (ri.isPremium ? "2" : "1.25") : "1";
			var url = hlsMasterUrl.Replace("master.m3u8", "play_control.json") + "&play_speed=" + speed;
			util.debugWriteLine("setSpeed url " + url);
			var res = util.getPageSource(url, null, referer, false, 2000, ua);
			util.debugWriteLine("setSpeed res " + res);
			if (res != null && res.IndexOf("\"message\":\"ok\"") > -1) {
				isSpeedUp = isUp;
				util.debugWriteLine("setSpeed ok");
			}
		}
		
		private bool checkFreeSpace(numTaskInfo nti) {
			if (isWriteCancel) return false;
			
			var driveName = Directory.GetDirectoryRoot(recFolderFile);
			if (driveName == null) return true;
			if (driveName.StartsWith("\\\\")) return true;
			
			var availableFreeSpace = util.getAvailableFreeSpace(recFolderFile);
			
			if (rm.rfu == rfu && !isEnoughSpaceF(availableFreeSpace, nti)
					&& IsSecondRecordDir && Directory.Exists(secondRecordDir)) {
				changeSecondRecordDir();
				availableFreeSpace = util.getAvailableFreeSpace(recFolderFile);
			}
			while (rm.rfu == rfu && !isEnoughSpaceF(availableFreeSpace, nti)) {
				var isRetryConfirm = showConfirmFreeSpaceRetryMsgBox(availableFreeSpace, driveName);
				
				if (!isRetryConfirm) {
					rm.stopRecording(false);
					isWriteCancel = true;
					return false;
				}
				availableFreeSpace = util.getAvailableFreeSpace(recFolderFile);
			}
			util.debugWriteLine("free space " + availableFreeSpace);
			return true;
		}
		private bool showConfirmFreeSpaceRetryMsgBox(long availableFreeSpace, string driveName) {
			var isCancel = false;
			var m = driveName + "の空き容量が" + (availableFreeSpace / 1000) + "KBです。";
			util.debugWriteLine(m);
			rm.form.addLogText(m);
			
			rm.form.formAction(() => {
				try {
					var r = util.showMessageBoxCenterForm(rm.form, m, "", MessageBoxButtons.RetryCancel);
					if (r == DialogResult.Cancel) {
						
						isCancel = true;
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}, false);
			return !isCancel;
		}
		private bool isEnoughSpaceF(long availableFreeSpace, numTaskInfo nti) {
			return availableFreeSpace > 10000000 && availableFreeSpace > nti.res.Length;
		}
		private void changeSecondRecordDir() {
			var newRecDir = util.getRecFolderFilePath(ri.si.recFolderFileInfo[0], ri.si.recFolderFileInfo[1], ri.si.recFolderFileInfo[2], ri.si.lvid, ri.si.recFolderFileInfo[4], ri.si.recFolderFileInfo[5], rm.cfg, ri.si.isTimeShift, ri.timeShiftConfig, ri.si.openTime, ri.isRtmp, ri.isFmp4, true, rm.form);
			if (newRecDir[1] != recFolderFile) {
				recFolderFile = newRecDir[1];
				ri.recFolderFile = newRecDir;
				((WebSocketRecorder)wr).clearCommentFileName();
				wr.resetCommentFile();
				if (renketuRealTimeFS != null) {
					try {
						renketuRealTimeFS.Close();
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					}
					renketuRealTimeFS = null;
				}
			}
		}
		private int getChangedTfdtMp4(byte[] b, int baseTfdt, out byte[] newTfdtB) {
			newTfdtB = null;
			var isLog = false;
			try {
				var pos = 0;
				using (var ms = new MemoryStream()) {
					while (pos < b.Length) {
						var size = BitConverter.ToInt32((new byte[]{b[pos + 3], b[pos + 2], b[pos + 1], b[pos + 0]}), 0);
						if (size == 0) {
							util.debugWriteLine("atom parse error size 0");
							rm.form.addLogText("atom parse error size 0");
							return -1;
						}
						var type = Encoding.ASCII.GetString(b, pos + 4, 4);
						
						if (isLog) Debug.WriteLine("size " + size + " pos " + pos + " type " + type);
						if (type == "moof") {
							var _pos = pos + 8;
							while (_pos < pos + size) {
								var _size = BitConverter.ToInt32((new byte[]{b[_pos + 3], b[_pos + 2], b[_pos + 1], b[_pos + 0]}), 0);
								if (_size == 0) {
									util.debugWriteLine("atom parse error size 0");
									rm.form.addLogText("atom parse error size 0");
									return -1;
								}
								var _type = Encoding.ASCII.GetString(b, _pos + 4, 4);
								if (isLog) util.debugWriteLine("  moof " + _size + " " + _type);
								if (_type == "traf") {
									var __pos = _pos + 8;
									while (__pos < _pos + _size) {
										var __size = BitConverter.ToInt32((new byte[]{b[__pos + 3], b[__pos + 2], b[__pos + 1], b[__pos + 0]}), 0);
										if (__size == 0) {
											util.debugWriteLine("atom parse error size 0");
											rm.form.addLogText("atom parse error size 0");
											return -1;
										}
										var __type = Encoding.ASCII.GetString(b, __pos + 4, 4);
										if (isLog) Debug.WriteLine("    traf " + __size + " " + __type);
										if (__type == "tfdt") {
											var ___size = BitConverter.ToInt32((new byte[]{b[__pos + 3], b[__pos + 2], b[__pos + 1], b[__pos + 0]}), 0);
											if (___size == 0) {
												util.debugWriteLine("atom parse error size 0");
												rm.form.addLogText("atom parse error size 0");
												return -1;
											}
											var tfdt = BitConverter.ToInt32((new byte[]{b[__pos + ___size - 1], b[__pos + ___size - 2], b[__pos + ___size - 3], b[__pos + ___size - 4]}), 0);
											if (isLog) Debug.WriteLine("      tfdt " + tfdt);
											if (baseTfdt == -1) 
												baseTfdt = tfdt;
											
											var newTfdt = BitConverter.GetBytes(tfdt - baseTfdt);
											if (isLog) util.debugWriteLine("      new tfdt " + (tfdt - baseTfdt) + " " + newTfdt);
											
											for (var i = 1; i < 5; i++) 
												b[__pos + ___size - i] = i > newTfdt.Length ? (byte)0 : newTfdt[i - 1];
											tfdt = BitConverter.ToInt32((new byte[]{b[__pos + ___size - 1], b[__pos + ___size - 2], b[__pos + ___size - 3], b[__pos + ___size - 4]}), 0);
											if (isLog) util.debugWriteLine("      tfdt2 " + tfdt);
											
										}
										__pos += __size;
									}
								}
								_pos += _size;
							}
						}
						ms.Write(b, pos, size);
						
						pos += (int)size;
					}
					newTfdtB = ms.ToArray();
					return baseTfdt;
				}
			} catch (Exception ee) {
				Debug.WriteLine("atom parse error " + ee.Message + ee.Source + ee.StackTrace);
				rm.form.addLogText("atom parse error " + ee.Message + ee.Source + ee.StackTrace);
				return -1;
			}
		}
		void writeComplementMp4File(numTaskInfo nti) {
			util.debugWriteLine("writeComplementMp4File " + lastSegmentNo + " " + nti.no);
			for (var i = 0; i < nti.no - lastSegmentNo - 1; i++)
				writeFile(lastWriteFmp4Nti);
		}
		void saveKey(string file) {
			try {
				var curl = new Curl();
				string d = null;
				var b = curl.getBytes(file, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_3, "GET", d, false);
				if (b == null) return;
				//var f = util.getRegGroup(ri.recFolderFile[1], "(_\\d+h\\d+m\\d+s)_");
				var f = new Regex("(_\\d+h\\d+m\\d+s)_").Replace(ri.recFolderFile[1], "");
				//if (f == null) f = ri.recFolderFile[2];
				using (var fs = new FileStream(f + ".key", FileMode.OpenOrCreate)) {
					fs.Write(b, 0, b.Length);
				}
				isSavedKey = true;
			} catch (Exception e) {
				addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void getNtiListRes() {
			if (!ri.si.isChannelPlus) {
				var getByteThread = new Task[newGetTsTaskList.Count];
				for (var i = 0; i < newGetTsTaskList.Count; i++) {
					var ng = new NtiGetter(newGetTsTaskList[i], getTsCurl, ri);
					getByteThread[i] = Task.Run(() => ng.get(container, this));
					//if (ri.si.isChannelPlus) Thread.Sleep(2000);
				}
				foreach (var t in getByteThread) t.Wait();
			} else {
				var urls = newGetTsTaskList.Select(x => x.url).ToList();
				var rList = new List<byte[]>();
				for (var i = 0; i < newGetTsTaskList.Count; i++) {
					rList.Add(new Curl().getBytes(urls[i], util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", "", false));
				}
				//var rList = new Curl().getBytes(urls, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false, true);
				//if (rList == null || rList.Count != newGetTsTaskList.Count)
				if (rList.Count != newGetTsTaskList.Count)
					throw new Exception("error curl get ts");
				for (var i = 0; i < newGetTsTaskList.Count; i++)
					newGetTsTaskList[i].res = rList[i];
				util.debugWriteLine("test");
			}
		}
		int getChannelPlusTimeShiftTotalTime(string res) {
			var m = new Regex("#EXTINF:(\\d+)").Matches(res);
			var sum = 0;
			foreach (Match _m in m) sum += int.Parse(_m.Groups[1].Value);
			return sum == 0 ? -1 : sum;
		}
		void testWebRequest(string url) {
			try {
				var h = util.getHeader();
				h.Add("Accept", "*/*");
				h.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
				h.Add("Origin", "https://live.nicovideo.jp");
				h.Add("Referer", "https://live.nicovideo.jp/");
				var r = util.sendRequest(url, h, null, "GET", null);
				if (r != null) {
					using (var _r = r.GetResponseStream())
					using (var sr = new StreamReader(_r)) {
						var buf = sr.ReadToEnd();
						if (buf.IndexOf(".m3u8") > -1) {
							isWebRequest = true;
							#if DEBUG
								//rm.form.addLogText("新ライブラリを使わずに接続を試みます");
							#endif
						}
					}
				} else {
					#if DEBUG
						rm.form.addLogText("新ライブラリを使って接続を試みます");
					#endif
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
	}
	class NtiGetter {
		private numTaskInfo nti;
		private Curl curl;
		private RecordInfo ri;
		public NtiGetter(numTaskInfo nti, Curl curl, RecordInfo ri) {
			this.nti = nti;
			this.curl = curl;
			this.ri = ri;
		}
		public numTaskInfo get(CookieContainer cc, Record rec) {
			if (!ri.si.isChannelPlus)
				try {
                    nti.res = rec.getFileBytesRec(nti.url);
                }
                catch (Exception e) {
                    util.debugWriteLine(e.Message + e.Source + e.StackTrace);
                    return null;
                }
				//nti.res = util.getFileBytes(nti.url, null, true, 0);
			else {
				var h = util.getHeader();
				h.Add("Accept", @"*/*");
				h.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
				h.Add("Origin", "https://live.nicovideo.jp");
				h.Add("Referer", "https://live.nicovideo.jp/");
                try {
                    nti.res = rec.getFileBytesRec(nti.url);
                }
                catch (Exception e) {
                    util.debugWriteLine(e.Message + e.Source + e.StackTrace);
                    return null;
                }
                //nti.res = curl.getBytes(nti.url, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", d, false);
			}
			return nti;
		}
	}
}