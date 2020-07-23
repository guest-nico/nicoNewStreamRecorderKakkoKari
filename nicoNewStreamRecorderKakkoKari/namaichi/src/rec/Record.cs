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
using WebSocket4Net;
using System.Globalization;
using System.Diagnostics;
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
		//private System.Diagnostics.Process process;
		private DateTime lastReadTime = DateTime.UtcNow;
		public string hlsMasterUrl;
		public string recFolderFile;
		private string lvid;
		private long _openTime;
		private int lastSegmentNo = -1;
		public DateTime lastWroteSegmentDt = DateTime.MinValue;
		//private int lastAccessingSegmentNo;
		private CookieContainer container;
		private int segmentSaveType = 0;
		private bool isTimeShift = false;
		public TimeShiftConfig tsConfig = null;
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
		private int lastRecordedSeconds = -1;
		private double lastFileSecond = 0;
		private int gotTsMaxNo = -1;
		public bool isEndProgram = false;
		private double allDuration = -1;
		public string engineMode = "0";
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
		//private bool isSub;
		private int baseNo = 0;
		
//		private string lastTsPlayList = null;
//		private DateTime lastTsPlayListTime = DateTime.MinValue;
		private double lastWroteFileSecond = -1;
		private DateTime lastAccessPlaylistTime = DateTime.Now;
		
//		private RealTimeFFmpeg realtimeFFmpeg = null;
		private bool isRealtimeChase = false;
		public DropSegmentProcess dsp = null;
		private bool isSpeedUp = false;
		private string ua = null;//"Lavf/56.36.100";
		private string referer = null;
		public static bool isWriteCancel = false;
		
		public Record(RecordingManager rm, bool isFFmpeg, 
		              RecordFromUrl rfu, string hlsUrl, 
		              string recFolderFile, //int lastSegmentNo, 
		              CookieContainer container, bool isTimeShift, 
		              IRecorderProcess wr, string lvid, 
		              TimeShiftConfig tsConfig, long _openTime,
		              WebSocket ws, string recFolderFileOrigin,
		              bool isRealtimeChase, Html5Recorder h5r) {
			this.rm = rm;
			this.isFFmpeg = isFFmpeg;
			this.rfu = rfu;
			this.hlsMasterUrl = hlsUrl;
			this.recFolderFile = recFolderFile;
			//this.lastSegmentNo = lastSegmentNo;
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
			this._openTime = _openTime;
			this.ws = ws;
			this.recFolderFileOrigin = recFolderFileOrigin;
			//this.isSub = isSub;
			this.isRealtimeChase = isRealtimeChase;
			this.h5r = h5r;
			this.referer = null;//rfu.url;
		}
		public void record(string quality) {
			recordingQuality = quality;
			tsWriterTask = Task.Run(() => {startDebugWriter();});
			
			var _m = (isPlayOnlyMode) ? "視聴" : "録画";
			if (isTimeShift) {
				var isHokan = !((WebSocketRecorder)wr).isSaveComment && ((WebSocketRecorder)wr).isChase; 
				if (engineMode != "3" && !isHokan)
					rm.form.addLogText((((WebSocketRecorder)wr).isChase ? "追っかけ再生" : "タイムシフト") + "の" + _m + "を開始します(画質:" + quality + ")");
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
			hlsSegM3uUrl = getHlsSegM3uUrl(hlsMasterUrl);
			rm.hlsUrl = hlsSegM3uUrl;
			rm.form.setPlayerBtnEnable(true);
			
			if (engineMode == "0") {
				m3u8GetterTask = Task.Run(() => {startM3u8Getter();});
				tsGetterTask = Task.Run(() => {startTsGetter();});
				//if (!isSub)
					tsWriterTask = Task.Run(() => {startTsWriter();});
				
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
						((WebSocketRecorder)wr).setSync(0, 0, hlsSegM3uUrl);
						if (!isFirst) wr.resetCommentFile();
					}
					isFirst = false;
					
					var aer = new AnotherEngineRecorder(rm, rfu);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					recFolderFile = util.incrementRecFolderFile(recFolderFile);//wr.getRecFilePath()[1];
					setReconnecting(true);
//					if (!isReConnecting)
					//((WebSocketRecorder)wr).setSync(0, 0, hlsSegM3uUrl);					
					if (wr != null)
						((WebSocketRecorder)wr).sync = 0;
					
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
			
			if (rm.cfg.get("fileNameType") == "10" && (recFolderFile.IndexOf("{w}") > -1 || recFolderFile.IndexOf("{c}") > -1))
				renameStatistics();
			
			if (engineMode == "0" && !isPlayOnlyMode) {
				addDebugBuf("rec end shori gottslist count " + gotTsList.Count);
				if (rfu.subGotNumTaskInfo != null)
					addDebugBuf("subgot " + rfu.subGotNumTaskInfo.Count);
				if (gotTsList.Count > 10) {
					displayWriteRemainGotTsData();
					
				}
				waitForRecording();
				
				if (rfu.subGotNumTaskInfo != null)
					addDebugBuf("rfu.subGotNumTaskInfo.count " + rfu.subGotNumTaskInfo.Count);
				var isManyNti = (rfu.subGotNumTaskInfo == null || rfu.subGotNumTaskInfo.Count == 0) ? false : (rfu.subGotNumTaskInfo[rfu.subGotNumTaskInfo.Count - 1].dt - rfu.subGotNumTaskInfo[0].dt > TimeSpan.FromSeconds(25));
				if (isManyNti) addDebugBuf("isManyNti subgot [0].dt + " + rfu.subGotNumTaskInfo[0].dt + " [count - 1] " + rfu.subGotNumTaskInfo[rfu.subGotNumTaskInfo.Count - 1].dt);
//				isManyNti = true;
				if (rfu.subGotNumTaskInfo != null && isManyNti) {
					rm.form.addLogText("抜けセグメントの補完を試みます");
					addDebugBuf("抜けセグメントの補完を試みます");
					if (dsp == null) {
						dsp = new DropSegmentProcess(lastWroteSegmentDt, lastSegmentNo, this, recFolderFileOrigin, rfu, rm, h5r);
						dsp.writeRemaining();
						dsp = null;
					} 
				}
				
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
						tf.start(recFolderFile + ".ts", true);
					}
				}
				rm.form.addLogText("録画終了処理を完了しました");
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
					var res = util.getPageSource(hlsSegM3uUrl, null, referer, false, 2000, ua);
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
					rm.form.addLogText(e.Message + e.Source + e.StackTrace + e.TargetSite);
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
									var fileName = util.getRegGroup(s, "(.+?.ts)\\?", 1, rm.regGetter.getTs2());
									//fileName = util.getRegGroup(fileName, "(\\d+)") + ".ts";
									//fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ".ts";
									fileName = (no + baseNo).ToString() + "_" + startTimeStr + ".ts";
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
	//								segM3u8List.Clear();
									
									segM3u8List.RemoveRange(0, segM3u8List.Count - 1);
									isBreak = true;

									continue;
								}
								
								addDebugBuf("tsBytes get ok " + _res.url);
								//nti.res = _nti.tsBytes;
							
								//if (isSub) rfu.subGotNumTaskInfo.Add(_res);
								//else 
									gotTsList.Add(_res);
								
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
					
					
					
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
					rm.form.addLogText(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			
			addDebugBuf("ts getter end gotTsList len " + gotTsList.Count);
		}
		
		
		
		
		private void startTsWriter() {
			while (true) {
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
					foreach (var s in new List<numTaskInfo>(gotTsList)) {
						addDebugBuf("s " + s + " s == null " + (s == null));
						if (s == null) {
							gotTsList.RemoveAt(0);
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
						
						bool ret;
						if (isPlayOnlyMode) ret = true;
						else {
							var before = DateTime.Now;
							
							if (isWriteCancel) return;
							ret = writeFile(s);
							addDebugBuf("write time " + (DateTime.Now - before));
							if (DateTime.Now - before > TimeSpan.FromSeconds(2))
								Thread.Sleep(2000);
						}
						//util.debugWriteLine("write ok " + s.no);
						
						if (ret) {
							if (wr != null && wr.firstSegmentSecond == -1) 
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
							lastWroteFileSecond = s.second;
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
					if (count > 0 && !isPlayOnlyMode) setRecordState();
	//				gotTsList.Clear();
					
					Thread.Sleep(300);
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
					rm.form.addLogText(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			addDebugBuf("ts writer end gotTsList len " + gotTsList.Count);
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
		*/
		private bool streamRenketuRecord(numTaskInfo info) {
			try {
				addDebugBuf(info.no + " " + info.url);
				addDebugBuf("record file path " + recFolderFile + ".ts");
				
				//file lock check
				if (File.Exists(recFolderFile + ".ts")) {
					//File.Move(recFolderFile + ".ts", recFolderFile + ".ts");
					using (var checkIO = new FileStream(recFolderFile + ".ts", FileMode.Append, FileAccess.Write)) {
					}
				}
				addDebugBuf("test record file path " + recFolderFile + ".ts");
				
				using (var w = new FileStream(recFolderFile + ".ts", FileMode.Append, FileAccess.Write)) {
					w.Seek(0, SeekOrigin.End);
					util.debugWriteLine("streamRenketuRecord cc　" + info.res.Length);
					w.Write(info.res, 0, info.res.Length);
					//w.Close();
					util.debugWriteLine("streamRenketuRecord bb");
					w.Flush(true);
					util.debugWriteLine("streamRenketuRecord aa");
				}
				if (isTimeShift) {
					var newName = newTimeShiftFileName(recFolderFile, info.fileName);
					File.Move(recFolderFile + ".ts", newName + ".ts");
					recFolderFile = newName;
				}
				return true; 
			} catch (Exception e) {
				addDebugBuf(e.Message+e.StackTrace + e.Source + e.TargetSite);
				rm.form.addLogText(e.Message + e.StackTrace);
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
			var res = util.getPageSource(masterUrl, null, referer, false, 2000, ua);
			if (res == null) {
				addDebugBuf("getHlsSegM3uUrl res null reconnect " + masterUrl);
				((WebSocketRecorder)wr).displayDebug("master url res null");
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
			addDebugBuf("addNewTsTaskList " + hlsSegM3uUrl);
//			var wc = new WebHeaderCollection();

			addDebugBuf("getpage mae");
			
			var res = util.getPageSource(hlsSegM3uUrl, null, referer, false, 2000, ua);
			addDebugBuf("addNewTsTaskList segm3u get " + res);
//			util.debugWriteLine("m3u8 " + res);
			
			 
			if (otstul != null && !otstul.isStarted) outputTimeShiftTsUrlList(res);
			
			
			//shuusei? 
			int min = (res == null) ? -1 : int.Parse(util.getRegGroup(res, "(\\d+).ts", 1, rm.regGetter.getTs()));
			//if (res == null || (lastSegmentNo != -1 && res.IndexOf(lastSegmentNo.ToString()) == -1)) {
			if (res == null || (lastSegmentNo != -1 && min != -1 && min > lastSegmentNo)) {
			//if (res == null) {
				addDebugBuf("nuke? lastSegmentNo " + lastSegmentNo + " min " + " min " + min + " res " + res);
//				rm.form.addLogText("リトライ lastSegmentNo " + lastSegmentNo + " res " + res + " min " + min);
				((WebSocketRecorder)wr).displayDebug("ts task res " + res + " / lastsegno " + lastSegmentNo + " / min " + min + " reconnect");
				setReconnecting(true);
//				if (!isReConnecting) 
					reConnect();
				
				return 3.0;
			}
			var isTimeShiftPlaylist = res.IndexOf("#STREAM-DURATION") > -1;
			if (!isTimeShift && isTimeShiftPlaylist) {
				return -1;
			}
			double streamDuration = -1; 
			var _streamDuration = util.getRegGroup(res, "#STREAM-DURATION:(.+)");
			if (_streamDuration != null) {
				streamDuration = double.Parse(_streamDuration, NumberStyles.Float);
				if (tsConfig.endTimeSeconds < 0)
					tsConfig.endTimeSeconds = (int)(streamDuration + 15);
			}
			
			var baseTime = getBaseTimeFromPlaylist(res);
			var second = 0.0;
			var secondSum = 0.0;
			var targetDuration = 2.0;
			lock(recordLock) {
				bool _isRetry = isRetry, _isEndProgram = isEndProgram, _isEnd = isEnd;
				var lastListSegNo = -1;
				foreach (var s in res.Split('\n')) {
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
					
					if (s.IndexOf(".ts") < 0) continue;
					var no = lastListSegNo = int.Parse(util.getRegGroup(s, "(\\d+)"));
					var url = baseUrl + s;
					
					var isInList = false;
					lock (newGetTsTaskList) {
						foreach (var t in newGetTsTaskList)
							if (t.no == no) isInList = true;
					}
					
					if (isEndTimeshift(streamDuration, res, second) && !((WebSocketRecorder)wr).isChase) {
						addDebugBuf("isEndTimeshift true");
						_isRetry = false;
						_isEndProgram = true;
					}
					
					var startTime = baseTime + secondSum - second;
					if (isTimeShift && 
					    	((tsConfig.timeType == 0 && startTime < tsConfig.timeSeconds) ||
					     	(tsConfig.timeType == 1 && startTime <= tsConfig.timeSeconds))) continue;
					if (isTimeShift && tsConfig.endTimeSeconds > 0 && startTime >= tsConfig.endTimeSeconds) {
						addDebugBuf("timeshift tsConfig.endtime " + tsConfig.endTimeSeconds + " now starttime " + startTime + " tsConfig.timeseconds " + tsConfig.timeSeconds);
						_isRetry = false;
						_isEndProgram = true;
						continue;
					}
					var startTimeStr = util.getSecondsToStr(startTime);
					
					if (no > lastSegmentNo && !isInList) {
						if (engineMode == "3") {
							if (wr.firstSegmentSecond == -1) 
								wr.firstSegmentSecond = startTime;
							_isRetry = false;
							_isEnd = _isEndProgram = true;
							break;
						}
						
						var fileName = util.getRegGroup(s, "(.+?.ts)\\?");
						//fileName = util.getRegGroup(fileName, "(\\d+)") + ".ts";
						fileName = util.getRegGroup(fileName, "(\\d+)\\.") + "_" + startTimeStr + ".ts";
						addDebugBuf(no + " " + fileName);
						
						newGetTsTaskList.Add(new numTaskInfo(no, url, second, fileName, startTime));
						//Task.Run(() => getTsTask(url, startTime));
						//getTsTask(url, startTime);
					}
					
					if (((WebSocketRecorder)wr).isChase && hlsSegM3uUrl.IndexOf("hlsarchive") > -1) {
						addDebugBuf("ischase segM3uUrl hlsarchive end");
						_isRetry = false;
//						isEndProgram = true;
					}
					
				}

				if (newGetTsTaskList.Count != 0) {
					//var lastListSegNo = newGetTsTaskList[newGetTsTaskList.Count - 1].no;
					//var getByteList = new List<byte[]>();
					var getByteThread = new Task[newGetTsTaskList.Count];
					try {
						for (var i = 0; i < newGetTsTaskList.Count; i++) {
							var ng = new NtiGetter(newGetTsTaskList[i]);
							getByteThread[i] = Task.Run(() => ng.get(container));
							
						}
						foreach (var t in getByteThread) t.Wait();
						
						
						for (var i = 0; i < getByteThread.Length; i++) {
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
				
				var lateTime = streamDuration - lastSegmentNo / 1000;
				util.debugWriteLine("lateTime " + lateTime);
				if (lateTime > 11 || !((WebSocketRecorder)wr).isChase) {
					if (!isSpeedUp) setSpeed(true);
				} else if (lateTime < 7) {
					if (isSpeedUp) {
						setSpeed(false);
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
							if (tsBytes == null || (lastSegmentNo > 10000 && newGetTsTaskList[i].no - lastSegmentNo > 5500)) {
								newGetTsTaskList.Clear();
								if (!isReConnecting) {
									addDebugBuf("getTsTask !isReconnecting reconnect");
									//debug
									((WebSocketRecorder)wr).displayDebug("get bytes " + tsBytes + " / lastsegno " + lastSegmentNo + "/ no " + (newGetTsTaskList != null && newGetTsTaskList.Count > 0 ? newGetTsTaskList[0].no : -10));
									
									reConnect();
								}
//								rm.form.addLogText("セグメント取得エラー " + url);
								setReconnecting(true);
								break;
							}
							var a = recordedSecond;
							newGetTsTaskList[i].res = tsBytes;
							if (!((WebSocketRecorder)wr).isChase) {
								recordedSecond += newGetTsTaskList[i].second; 	
								recordedBytes += tsBytes.Length;
							}
							var b = recordedSecond;
							addDebugBuf("aads " + a + " " + b + " no " + newGetTsTaskList[i].no);
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
						else {
							if (isWriteCancel) return;
							
							if (((WebSocketRecorder)wr).isChase)
								gotTsList.Add(newGetTsTaskList[i]);
							else ret = writeFile(newGetTsTaskList[i]);
							ret = true;
						}

						addDebugBuf("write ok " + ret + " " + newGetTsTaskList[i].no);
						if (ret) {
							if (wr.firstSegmentSecond == -1) 
								wr.firstSegmentSecond = newGetTsTaskList[i].startSecond;
							
							//recordedNo.Add(newGetTsTaskList[i].no.ToString());
							recordedNo.Add(newGetTsTaskList[i].fileName);
							lastSegmentNo = newGetTsTaskList[i].no;
							var fName = util.getRegGroup(newGetTsTaskList[i].fileName, ".*(\\\\|/|^)(.+)", 2);
//							if (fName == 
							lastRecordedSeconds = util.getSecondsFromStr(fName);
							lastWroteFileSecond = newGetTsTaskList[i].second;
							
							util.debugWriteLine("aaaaa " + tsConfig.endTimeSeconds + " " + startTime + " " +  newGetTsTaskList[i].startSecond + " " + nti.second + " a " + (startTime + nti.second) + " " + (startTime + nti.second >= tsConfig.endTimeSeconds) + " " + newGetTsTaskList.Count + " " + i);
							if (isTimeShift && tsConfig.endTimeSeconds > 0 && newGetTsTaskList[i].startSecond + nti.second >= tsConfig.endTimeSeconds) {
								addDebugBuf("getTsTask timeshift tsConfig.endtime " + tsConfig.endTimeSeconds + " now starttime " + startTime + " tsConfig.timeseconds " + tsConfig.timeSeconds);
								isRetry = false;
								isEndProgram = true;
								continue;
							}
							
						} else {
							newGetTsTaskList.Clear();
							if (!isReConnecting) {
								addDebugBuf("getTsTask write ret false reconnect");
								((WebSocketRecorder)wr).displayDebug("not write");
								reConnect();
							}
							setReconnecting(true);
							break;
						}
						
					}
					addDebugBuf("write ok2");
					newGetTsTaskList.Remove(nti);
					/*
					for (int i = 0; i < newGetTsTaskList.Count; i++) {
						if (newGetTsTaskList[i].res != null) 
							newGetTsTaskList.RemoveAt(i);
					}
					*/
				}
				if (!isPlayOnlyMode && !((WebSocketRecorder)wr).isChase)
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
			long b = bytes % 1000;
			long kb = (bytes % 1000000) / 1000;
			long mb = (bytes % 1000000000) / 1000000;
			long gb = (bytes % 1000000000000) / 1000000000;
			string _kb = ((int)(bytes / 1000)).ToString();
			for (var i = 0; i < 9 - _kb.Length; i++)
				_kb = " " + _kb;
			ret += "size=" + _kb + "kB\n";
			
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
			
			if (isTimeShift && !((WebSocketRecorder)wr).isChase) {
				var per = (int)(((lastSegmentNo + 5000) / 10) / allDuration);
				if (per > 100) per = 100;
				ret = "(" + per + "%) " + ret;
				//ret += "(" + (int)((lastSegmentNo / 10) / allDuration) + "%)";
			}
			var titleT = ret.Replace('\n', ' ');
			//ret += "(" + percent + ")" + (lastSegmentNo / 10) + " " + allDuration;
			rm.form.setRecordState(ret, titleT);
			
		}
		private bool writeFile(numTaskInfo info) {
			var ret = false;
			if (!checkFreeSpace(info)) return false;
			if (segmentSaveType == 0) {
				ret = streamRenketuRecord(info);
			} else {
				ret = originalTsRecord(info);
			}
			
			if (lastSegmentNo == -1 && wr != null) {
				((WebSocketRecorder)wr).setSync(
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
					w.Write(info.res, 0, info.res.Length);
					//w.Close();
				}
				return true; 
			} catch (Exception e) {
				addDebugBuf("original ts record exception " + e.Message+e.StackTrace + e.Source + e.TargetSite);
				rm.form.addLogText(e.Message + e.StackTrace);
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
		public void reSetHlsUrl(string url, string quality, WebSocket _ws, bool isChaseRealing) {
			addDebugBuf("resetHlsUrl oldurl " + hlsMasterUrl + " new url " + url);
			ws = _ws;
			if (recordingQuality != quality)
				rm.form.addLogText("画質を変更して再接続します(" + quality + ")");
			recordingQuality = quality;
			
			hlsMasterUrl = url;
			if (isTimeShift) {
				var start = 0;
				//var recorded = lastRecordedSeconds < lastSegmentNo / 1000 ? lastRecordedSeconds : lastSegmentNo; 
				if (lastRecordedSeconds == -1) {
					start = (tsConfig.timeSeconds - 10 < 0) ? 0 : (tsConfig.timeSeconds - 10);
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
				using (w = new FileStream(outFName, FileMode.Append, FileAccess.Write)) {
					_streamRenketuAfterWrite(w);
					return w.Name;
				}
			} catch (PathTooLongException) {
				try {
					addDebugBuf("renketu after out fname " + recFolderFile + "/" + lvid + ".ts");			
					using (w = new FileStream(recFolderFile + "/" + lvid + ".ts", FileMode.Append, FileAccess.Write)) {
						_streamRenketuAfterWrite(w);
						return w.Name;
					}
				} catch (PathTooLongException) {
					try {
						addDebugBuf("renketu after out fname " + recFolderFile + "/_.ts");			
						using (w = new FileStream(recFolderFile + "/_.ts", FileMode.Append, FileAccess.Write)) {
							_streamRenketuAfterWrite(w);
							return w.Name;
						}
					} catch (PathTooLongException) {
						addDebugBuf("renketu after too long");
						rm.form.addLogText("録画後に連結しようとしましたがパスが長すぎてファイルが開けませんでした " + recFolderFile + "/_.ts");
						return null;
					}
				}
			}

			//w.Close();
			//return w.Name;
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
			if (tsConfig.isOutputUrlList) {
				setOutputTimeShiftTsUrlListStartTime();
			}
			
			var start = (tsConfig.timeSeconds - 10 < 0) ? 0 : (tsConfig.timeSeconds - 10);
			var baseMasterUrl = hlsMasterUrl;
			if (!isRealtimeChase) baseMasterUrl += "&start=" + (start.ToString());
			
			wr.tsHlsRequestTime = DateTime.Now;
			wr.tsStartTime = TimeSpan.FromSeconds((double)start);
			hlsSegM3uUrl = getHlsSegM3uUrl(baseMasterUrl);
			rm.hlsUrl = hlsSegM3uUrl;
			rm.form.setPlayerBtnEnable(true);
			
			var isWriteEnd = new bool[1]{false};
			if (engineMode == "0" && ((WebSocketRecorder)wr).isChase)
				tsWriterTask = Task.Run(() => timeShiftWriter(isWriteEnd));
			var isFirst = true;
			while (rm.rfu == rfu && isRetry) {
				if (isReConnecting) {
					Thread.Sleep(3000);
					continue;
				}
				if (hlsSegM3uUrl == null) {
					addDebugBuf("hlsSegM3uUrl null reconnect");
					((WebSocketRecorder)wr).displayDebug("hls segM3u8 null");
					setReconnecting(true);
					reConnect();
					continue;
				}
				
				if (engineMode == "0" || engineMode == "3") {

					targetDuration = addNewTsTaskList(hlsSegM3uUrl);

					if (engineMode == "3" && 
					    	((WebSocketRecorder)wr).tscg != null && 
					    	((WebSocketRecorder)wr).tscg.isEnd) {
						isRetry = false;
						isEndProgram = true;
					}
					var intervalTime = (int)(targetDuration * (isRealtimeChase ? 500 : 1000));
					var elapsedTime = (int)((TimeSpan)(DateTime.Now - lastAccessPlaylistTime)).TotalMilliseconds;
					util.debugWriteLine("wait time " + intervalTime + " " + elapsedTime);
					Thread.Sleep(elapsedTime > intervalTime ? 0 : intervalTime - elapsedTime);
					lastAccessPlaylistTime = DateTime.Now;
				} else {
					if (!isFirst) wr.resetCommentFile();
					isFirst = false;
					
					var recStartTime = DateTime.Now;
					var startPlayList = util.getPageSource(hlsSegM3uUrl, null, referer, false, 2000, ua);
					
					var _currentPos = util.getRegGroup(startPlayList, "#CURRENT-POSITION:(\\d+)");
					wr.firstSegmentSecond = (_currentPos == null) ? 0 : double.Parse(_currentPos, NumberStyles.Float);
					var aer = new AnotherEngineRecorder(rm, rfu);
					aer.record(hlsSegM3uUrl, recFolderFile, anotherEngineCommand);
					
					if (isAnotherEngineTimeShiftEnd(recStartTime, hlsSegM3uUrl, startPlayList) && !isRealtimeChase) {
						isEndProgram = true;
						break;
					}
					
					recFolderFile = wr.getRecFilePath()[1];
					 
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
				if (((WebSocketRecorder)wr).isChase)
					waitForRecording();
			
				if (isEndProgram && segmentSaveType == 0) {
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
					if (int.Parse(rm.cfg.get("afterConvertMode")) > 0) {
						var tf = new ThroughFFMpeg(rm);
						tf.start(recFolderFile + ".ts", true);
						
					}
				}
			}
			if (isEndProgram && !((WebSocketRecorder)wr).isHokan) {
				rm.form.addLogText("録画を完了しました");
			}
			isEnd = true;
		}
		private void timeShiftWriter(bool[] isWriteEnd) {
			var lastWroteNo = -1;
			while (true) {
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
			#if !DEBUG
//				return;
			#endif
			
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
			#if !DEBUG
//				return;
			#endif
			
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
					maxNo = int.Parse(util.getRegGroup(l, "(\\d+)\\.ts", 1, rm.regGetter.getMaxNo()));
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
					ins += maxLine.Replace(maxNo.ToString() + ".ts", i.ToString() + ".ts") + "\n";
				}
				res = res.Insert(res.IndexOf("EXTINF:") - 1, ins);
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
			var res = util.getPageSource(_hlsSegM3uUrl, null, referer, false, 2000, ua);
			
			otstul = new OutputTimeShiftTsUrlList(tsConfig, rm);
			otstul.setStartNum(res);
		}
		private void outputTimeShiftTsUrlList(string res) {
			Task.Run(() => {
				otstul.write(res, recFolderFile, baseUrl, tsConfig);
			});
		}
		private void reConnect() {
			if (wr == null) return;
			if (wr.isJikken) {
				wr.reConnect();
			} else {
				((WebSocketRecorder)wr).reConnect(ws);
			}
		}
		private bool isEndTimeshift(double streamDuration, string res, double second) {
			var ret = streamDuration - ((double)lastSegmentNo / 1000 + second) < 0.5;
			if (lastSegmentNo > 5 && lastWroteFileSecond != 5 && lastWroteFileSecond != -1 && ret) ret = true;
			addDebugBuf("isendTimeshift streamDuration " + streamDuration + " second " + second + " lastSegmentNo " + lastSegmentNo + " " + lastWroteFileSecond + " ret " + ret);
//			var ret = lastSegmentNo
//			var rett = streamDuration - (lastSegmentNo / 1000 + second);
			if (ret)
				return true;
			else return false;
//			return ret;
			
			/*
			if (streamDuration == -1) return false;
			if (lastSegmentNo + 5000 > streamDuration * 1000) return true;
			
			if (lastTsPlayList == res) {
				if (DateTime.Now - lastTsPlayListTime > TimeSpan.FromSeconds(10))
					return true;
			} else {
				lastTsPlayList = res;
				lastTsPlayListTime = DateTime.Now;
			}
			return false;
			*/
		}
		private bool isAnotherEngineTimeShiftEnd(DateTime recStartTime, string hlsSegM3uUrl, string startPlayList) {
			if (startPlayList == null) return false;
			var lastTsNum = util.getRegGroup(startPlayList, "[\\s\\S]+\n(\\d+).ts", 1, rm.regGetter.getLastTsNum());
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
			if (dsp == null) {
				dsp = new DropSegmentProcess(_lastWroteSegmentDt, _lastSegmentNo, this, recFolderFileOrigin, rfu, rm, h5r);
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
						#if DEBUG
//							rm.form.addLogText("delete old SubTs Exception");
						#endif
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
			return new Task<numTaskInfo>(() => {
				nti.res = util.getFileBytes(nti.url, null);
				return nti;
			});
			//return ret;
		}
		
		private void displayWriteRemainGotTsData() {
			if (isWriteCancel) return;
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
					recFolderFile = newName;
					return;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void renameStatistics() {
			try {
				var wsr = (WebSocketRecorder)wr;
				wsr.setRealTimeStatistics();
				
				if (File.Exists(recFolderFile + ".ts")) {
					var newName = recFolderFile.Replace("{w}", wsr.visitCount.Replace("-", "")).Replace("{c}", wsr.commentCount.Replace("-", ""));
					File.Move(recFolderFile + ".ts", newName + ".ts");
					recFolderFile = newName;
				}
				
				try {
					if (Directory.Exists(recFolderFile)) {
						//Thread.Sleep(5000);
						wsr.isRetry = false;
						wsr.stopRecording();
						//Thread.Sleep(2000);
						var newName = recFolderFile.Replace("{w}", wsr.visitCount.Replace("-", "")).Replace("{c}", wsr.commentCount.Replace("-", ""));
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
			var speed = isUp ? (((WebSocketRecorder)wr).isPremium ? "2" : "1.25") : "1";
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
			
			var d = Directory.GetDirectoryRoot(recFolderFile);
			if (d == null) return true;
			if (util.getRegGroup(d[0].ToString(), "([a-zA-Z])") == null) return true;

			var di = new DriveInfo(d);
			
			while (rm.rfu == rfu && di.AvailableFreeSpace < nti.res.Length) {
				var m = di.Name + "の空き容量が" + (di.AvailableFreeSpace / 1000) + "KBです。";
				
				util.debugWriteLine(m);
				rm.form.addLogText(m);
				var isBreak = false;
				rm.form.formAction(() => {
					try {
						var r = MessageBox.Show(rm.form, m, "", MessageBoxButtons.RetryCancel);
						if (r == DialogResult.Cancel) {
							rm.stopRecording(false);
							
							isWriteCancel = true;
							isBreak = true;
						}
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					}
				}, false);
				if (isBreak) return false;
			}
			util.debugWriteLine("free space " + di.AvailableFreeSpace);
			return true;
		}
	}
	class NtiGetter {
		private numTaskInfo nti;
		public NtiGetter(numTaskInfo nti) {
			this.nti = nti;
		}
		public numTaskInfo get(CookieContainer cc) {
			nti.res = util.getFileBytes(nti.url, null, true, 0);
			return nti;
		}
	}
}