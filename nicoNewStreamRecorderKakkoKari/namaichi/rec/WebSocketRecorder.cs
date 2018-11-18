/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/16
 * Time: 0:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WebSocket4Net;
using System.Security.Authentication;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading;
using System.Drawing;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of WebSocketRecorder.
	/// </summary>
	public class WebSocketRecorder : IRecorderProcess
	{
		private string[] webSocketInfo;
		private string broadcastId;
		private string userId;
		private string lvid;
		private bool isPremium = false;
<<<<<<< HEAD
		private string programType;
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		private CookieContainer container;
		private string[] recFolderFile;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		public Html5Recorder h5r;
		private WebSocket ws;
		private WebSocket wsc;
		private Record rec;
		private StreamWriter commentSW;
<<<<<<< HEAD
		//public string msUri;
		//public string[] msReq;
=======
		public string msUri;
		public string msReq;
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		private long serverTime;
		private string ticket;
		private bool isRetry = true;
		private string msThread;
		private string sendCommentBuf = null;
		private bool isSend184 = true;
		
		private bool isNoPermission = false;
<<<<<<< HEAD
		//public long openTime;
		public long _openTime;
		public bool isEndProgram = false;
		public int lastSegmentNo = -1;
		//public bool isTimeShift = false;
=======
		public long openTime;
		public bool isEndProgram = false;
		public int lastSegmentNo = -1;
		public bool isTimeShift = false;
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		private TimeShiftConfig tsConfig = null;
		private bool isTimeShiftCommentGetEnd = false;
		private DateTime lastEndProgramCheckTime = DateTime.Now;
		private DateTime lastWebsocketConnectTime = DateTime.Now;
		
		private TimeSpan jisa;
		//private DateTime beginTime = null;
		//private DateTime endTime = null;
		private TimeSpan programTime = TimeSpan.Zero;
		
<<<<<<< HEAD
//		public DateTime tsHlsRequestTime;
//		public TimeSpan tsStartTime;
=======
		public DateTime tsHlsRequestTime;
		public TimeSpan tsStartTime;
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			
		private WebSocket[] himodukeWS = new WebSocket[2];
			
//		private System.Threading.Thread mainThread;
		private TimeShiftCommentGetter tscg = null;
		
		bool isWaitNextConnection = false;
		List<WebSocket> wsList = new List<WebSocket>();
		
		private List<string> debugWriteBuf = new List<string>();
		private Task tsWriterTask = null;
		private bool isSub;
<<<<<<< HEAD
		private bool isRtmp;
		private RtmpRecorder rr;
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		
		private string qualityRank = null;
		private string isGetComment = null;
		private string isGetCommentXml = null;
		
		public WebSocketRecorder(string[] webSocketInfo, 
				CookieContainer container, string[] recFolderFile, 
				RecordingManager rm, RecordFromUrl rfu, 
				Html5Recorder h5r, long openTime, 
				int lastSegmentNo, bool isTimeShift, string lvid, 
				TimeShiftConfig tsConfig, string userId, 
<<<<<<< HEAD
				bool isPremium, TimeSpan programTime, 
<<<<<<< HEAD
				string programType, long _openTime, bool isSub, bool isRtmp
=======
				string programType, long _openTime, bool isSub
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			)
=======
				bool isPremium, TimeSpan programTime)
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		{
			this.webSocketInfo = webSocketInfo;
			this.container = container;
			this.recFolderFile = recFolderFile;
			this.rm = rm;
			this.rfu = rfu;
			this.h5r = h5r;
			this.openTime = openTime;
			this.lastSegmentNo = lastSegmentNo;
			this.isTimeShift = isTimeShift;
			this.lvid = lvid;
			this.tsConfig = tsConfig;
			this.userId = userId;
			this.isPremium = isPremium;
			this.programTime = programTime;
<<<<<<< HEAD
			isJikken = false;
			this.programType = programType;
			this._openTime = _openTime;
<<<<<<< HEAD
			this.isSub = isSub;
			this.isRtmp = isRtmp;
=======
<<<<<<< HEAD
			this.isSub = isSub;
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			this.qualityRank = rm.cfg.get("qualityRank");
			this.isGetComment = rm.cfg.get("IsgetComment");
			this.isGetCommentXml = rm.cfg.get("IsgetcommentXml");
<<<<<<< HEAD
=======
=======
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		}
		public bool start() {
			addDebugBuf("ws rec start");
			
			tsWriterTask = Task.Run(() => {startDebugWriter();});
			
//			connect(webSocketInfo[0]);
			if (isRtmp) {
				rr = new RtmpRecorder(lvid, container, rm, rfu, isSub, recFolderFile, this, openTime);
				Task.Run(() => {
					rr.record();
					rm.hlsUrl = "end";
					if (rr.isEndProgram) isEndProgram = true;
					isRetry = false;
				});
			}
			
			connect();
			
<<<<<<< HEAD
			addDebugBuf("rm.rfu dds1 " + rm.rfu);
=======
			util.debugWriteLine("rm.rfu dds1 " + rm.rfu);
			
			broadcastId = util.getRegGroup(webSocketInfo[0], "watch/(.+?)(\\?|/)");
//			userId = util.getRegGroup(webSocketInfo[0], "audience_token=.+?_(.+?)_");
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			
			broadcastId = util.getRegGroup(webSocketInfo[0], "watch/.*?(\\d+?)(\\?|/)");
//			userId = util.getRegGroup(webSocketInfo[0], "audience_token=.+?_(.+?)_");
			
			addDebugBuf("rm.rfu dds6 " + rm.rfu);
			
<<<<<<< HEAD
			addDebugBuf("ws main " + ws + " a " + (ws == null));
			
<<<<<<< HEAD
			if (!isSub)
				displaySchedule();
=======
<<<<<<< HEAD
			if (!isSub)
				displaySchedule();
=======
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			displaySchedule();
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			
//			while (ws.State != WebSocket4Net.WebSocketState.Closed) {
//			while (rm.rfu == rfu && ws != null && isRetry && 
//			       (rec == null || !rec.isStopRead())) {
			while (rm.rfu == rfu && isRetry) {
				
//				if (rec != null) 
//					addDebugBuf("isStopread " + rec.isStopRead());
				
				
				if (ws.State == WebSocket4Net.WebSocketState.Closed) {
					addDebugBuf("no connect loop ws close");
//					connect();
				}
				
//				if (DateTime.Now > DateTime.Parse("2018/10/19 4:43")) resetWebsocketInfo();
				//test
//				GC.Collect();
//				GC.WaitForPendingFinalizers();
				
				System.Threading.Thread.Sleep(100);
			}
//			while (isTimeShift && rm.rfu == rfu) 
//				System.Threading.Thread.Sleep(300);
			
<<<<<<< HEAD
			
			//util.debugWriteLine("loop end rm.rfu " + rm.rfu.GetHashCode() + " " + rfu.GetHashCode() + " isretry " + isRetry);
=======
<<<<<<< HEAD
			
			//util.debugWriteLine("loop end rm.rfu " + rm.rfu.GetHashCode() + " " + rfu.GetHashCode() + " isretry " + isRetry);
=======
			util.debugWriteLine("loop end rm.rfu " + rm.rfu.GetHashCode() + " " + rfu.GetHashCode() + " isretry " + isRetry);
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			isRetry = false;
			if (rr != null) rr.retryMode = (isEndProgram) ? 2 : 1;
			if (rm.rfu != rfu && tscg != null) tscg.setIsRetry(false);
			
			if (isTimeShift && rm.rfu == rfu && tscg != null) {
				while (rm.rfu == rfu && !tscg.isEnd) {
					Thread.Sleep(1000);
				}
			}
//			tscg.setIsRetry(false);
			
			
			if (rm.rfu != rfu) {
				//if (rr != null) rr.isRetry = false;
				stopRecording(ws, wsc);
//				ws.Close();
//				wsc.Close();
				if (rec != null) 
					rec.waitForEnd();
			}
			if (!isRetry) {
				//if (rr != null) rr.isRetry = false;
				stopRecording(ws, wsc);
				if (rec != null)
					rec.waitForEnd();
			}
			
			
			addDebugBuf("closed saikai");
			
			return isNoPermission;
		}
		private bool connect() {
			lock (this) {
				var  isPass = (TimeSpan.FromSeconds(0.5) > (DateTime.Now - lastWebsocketConnectTime));
				lastWebsocketConnectTime = DateTime.Now;
				if (isPass) 
					Thread.Sleep(500);
			}
			if (isWaitNextConnection) {
				Thread.Sleep(65000);
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> 7626697f2a2b755bc0d503452dc12ba5710be7f0
				resetWebsocketInfo();
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				isWaitNextConnection = false;
				addDebugBuf("after wait reset  " + " wsList " + wsList.Count);
			}
			
			
			addDebugBuf("ws connect");
			try {
				//ws = new WebSocket(webSocketInfo[0]);
<<<<<<< HEAD
				addDebugBuf("ws connect webSocketInfo[0] " + webSocketInfo[0] + " wsList " + wsList.Count);
=======
				addDebugBuf("ws connect webSocketInfo[0] " + webSocketInfo[0]);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				ws = new WebSocket(webSocketInfo[0], "", null, null, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36", "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12);
				ws.Opened += onOpen;
				ws.Closed += onClose;
				ws.DataReceived += onDataReceive;
				ws.MessageReceived += onMessageReceive;
				ws.Error += onError;
				
				ws.Open();
				wsList.Add(ws);
				
				var _ws = ws; 
				Thread.Sleep(5000);
				if (_ws != null && _ws.State == WebSocketState.Connecting) {
					addDebugBuf("ws connect 5 seconds close");
					try {
						_ws.Close();
					} catch (Exception e) {
						addDebugBuf("connect timeout ws exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
					}
				}
				
			} catch (Exception ee) {
				addDebugBuf("ws connect exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				return false;
			}
			return true;
		}
		private void onOpen(object sender, EventArgs e) {
<<<<<<< HEAD
			addDebugBuf("on open rm.rfu dds2 " + rm.rfu + " ws " + sender.GetHashCode() + " wsList " + wsList.Count);
=======
			addDebugBuf("on open rm.rfu dds2 " + rm.rfu + " ws " + sender.GetHashCode());
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			if (sender != ws) {
				((WebSocket)sender).Close();
				addDebugBuf("hukusuu ws close");
			}
			
			if (isNoPermission) webSocketInfo[1] = webSocketInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true");
							
			String leoReq = "{\"type\":\"watch\",\"body\":{\"command\":\"playerversion\",\"params\":[\"leo\"]}}";
			addDebugBuf("leoReq " + leoReq);
			addDebugBuf("websocketinfo1 " + webSocketInfo[1]);
			ws.Send(leoReq);
			
			
			ws.Send(webSocketInfo[1]);
			
			
			
			
			//test
//			for (int i = 0; i < 100; i++)
//				System.Threading.Tasks.Task.Run(() => {
//			                                	ws.Send(webSocketInfo[1]);
//			                                });
			
			/*
			if (isNoPermission)
				ws.Send(webSocketInfo[1]);
			else ws.Send(webSocketInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true"));
			*/
			addDebugBuf("open send  " + ws);
			addDebugBuf("rm " + rm + " rm.rfu " + rm.rfu + " rfu " + rfu);
			
//			if (rm.rfu != rfu) stopRecording();
		}
		private void onClose(object sender, EventArgs e) {
<<<<<<< HEAD
			addDebugBuf("on close " + e.ToString() + " ws hash " + sender.GetHashCode() + " istimeshift " + isTimeShift + " wsList " + wsList.Count);
			wsList.Remove((WebSocket)sender);
			addDebugBuf("on close2 " + " wsList " + wsList.Count);
=======
			addDebugBuf("on close " + e.ToString() + " ws hash " + sender.GetHashCode() + " istimeshift " + isTimeShift);
			
			Task.Run(() => {  
				if (!isTimeShift && isEndedProgram()) {
					isRetry = false;
					if (tscg != null) tscg.setIsRetry(false);
					isEndProgram = true;
				}
			});
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			Task.Run(() => {  
				if (!isTimeShift && isEndedProgram()) {
					isRetry = false;
<<<<<<< HEAD
					if (rr != null) rr.retryMode = 2;
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
					if (tscg != null) tscg.setIsRetry(false);
					isEndProgram = true;
				}
			});
			
			//stopRecording();
			if (rm.rfu == rfu && !isEndProgram && (WebSocket)sender == ws) {
				while (true) {
					try {
						if (!connect()) continue;
						break;
					} catch (Exception ee) {
						addDebugBuf(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
					}
				}
//				ws.Open();
			}
			

		}
		private void onError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			addDebugBuf("on error " + e.Exception.Message + " ws " + sender.GetHashCode());
			//stopRecording();
//			reConnect();
//			ws.Open();
//			endStreamProcess();
		}
		private void onDataReceive(object sender, DataReceivedEventArgs e) {
			addDebugBuf("on data " + e.Data);
		}
		private void onMessageReceive(object sender, MessageReceivedEventArgs e) {
			addDebugBuf("receive " + e.Message);
			
			if (sender != ws) {
				((WebSocket)sender).Close();
				addDebugBuf("hukusuu ws close");
			}
			
//			addDebugBuf("ws " + ws);
			if (ws == null) return;
			//pong
			if (e.Message.IndexOf("\"ping\"") >= 0) {
				sendPong();
			}
			
			//get message
			if (e.Message.IndexOf("\"messageServerUri\"") >= 0) {
				if (isSub) return;
				
				setMsInfo(e.Message);
				if (isTimeShift) {
					if (tscg == null) {
<<<<<<< HEAD
						tscg = new TimeShiftCommentGetter(e.Message, userId, rm, rfu, rm.form, openTime, recFolderFile, lvid, container, programType, _openTime, this, tsConfig.timeSeconds, tsConfig);
=======
						tscg = new TimeShiftCommentGetter(e.Message, userId, rm, rfu, rm.form, openTime, recFolderFile, lvid, container, programType, _openTime, this);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
						tscg.save();
						
					}
					
				} else {
					if (rfu == rm.rfu && wsc == null && isRetry) {
						connectMessageServer(e.Message, (WebSocket)sender);
					}
				}
			}
			
			//record
			if (e.Message.IndexOf("\"currentStream\"") >= 0) {
				addDebugBuf("mediaservertype = " + util.getRegGroup(e.Message, "(\"mediaServerType\".\".+?\")"));
				if (isRtmp) return;
				
				var bestGettableQuolity = getBestGettableQuolity(e.Message);
				var currentQuality = util.getRegGroup(e.Message, "\"quality\":\"(.+?)\"");
				if (isFirstChoiceQuality(currentQuality, bestGettableQuolity)) {
					record(e.Message, currentQuality);
				} else sendUseableStreamGetCommand(bestGettableQuolity);
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
			}
			
			//new stream retry
			if (e.Message.IndexOf("\"NO_PERMISSION\"") >= 0
			    || e.Message.IndexOf("\"TAKEOVER\"") >= 0
			    || e.Message.IndexOf("\"INTERNAL_SERVERERROR\"") >= 0
			    || e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") >= 0
			   	|| e.Message.IndexOf("\"END_PROGRAM\"") >= 0
			    || e.Message.IndexOf("\"TOO_MANY_CONNECTIONS\"") >= 0
			    || e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0
			   	|| e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0) {
<<<<<<< HEAD
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0 && !isRtmp) rm.form.addLogText("追い出されました。" + util.getMainSubStr(isSub, true));
				
				//SERVICE_TEMPORARILY_UNAVAILABLE 予約枠開始後に何らかの問題？
				if (e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") > 0 && !isRtmp) 
=======
<<<<<<< HEAD
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0 && !isRtmp) rm.form.addLogText("追い出されました。" + util.getMainSubStr(isSub, true));
				
				//SERVICE_TEMPORARILY_UNAVAILABLE 予約枠開始後に何らかの問題？
				if (e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") > 0 && !isRtmp) 
=======
<<<<<<< HEAD
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0) rm.form.addLogText("追い出されました。" + util.getMainSubStr(isSub, true));
=======
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0) rm.form.addLogText("追い出されました。");
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
				
				//SERVICE_TEMPORARILY_UNAVAILABLE 予約枠開始後に何らかの問題？
				if (e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") > 0) 
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
>>>>>>> 7626697f2a2b755bc0d503452dc12ba5710be7f0
					rm.form.addLogText("サーバーからデータの受信ができませんでした。リトライします。" + util.getMainSubStr(isSub, true));
			
				if (e.Message.IndexOf("\"END_PROGRAM\"") > 0) {
					isEndProgram = true;
					isRetry = false;
					if (rr != null) rr.retryMode = 2;
					if (tscg != null) tscg.setIsRetry(false);
				}
				
				addDebugBuf("kiteru");
//				connect(webSocketInfo[0].Replace("\"requireNewStream\":false", "\"requireNewStream\":true"));
				isNoPermission = true;
				if (e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0 ||
<<<<<<< HEAD
				    	e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0) {
					isWaitNextConnection = true;
					//{"type":"error","body":{"code":"CONNECT_ERROR"}}
					
					if (e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0 && !isRtmp)
						rm.form.addLogText("満員でした" + util.getMainSubStr(isSub, true));
					
					if (e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0 && !isRtmp)
						rm.form.addLogText("接続エラーでした" + util.getMainSubStr(isSub, true));
					#if DEBUG
					#endif
					
=======
<<<<<<< HEAD
				    	e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0) {
=======
				   	e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0)
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
					isWaitNextConnection = true;
					//{"type":"error","body":{"code":"CONNECT_ERROR"}}
					#if DEBUG
						if (e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0)
							rm.form.addLogText("満員でした");
						if (e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0)
							rm.form.addLogText("接続エラーでした");
					#endif
//					resetWebsocketInfo();
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				//stopRecording();
//				reConnect();
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
				}
			} else if (e.Message.IndexOf("\"disconnect\"") >= 0) {
				addDebugBuf("unknown disconnect");
				isNoPermission = true;
				//stopRecording();
//				reConnect();
			}
			if (e.Message.IndexOf("\"command\":\"statistics\",\"params\"") >= 0
			   	) {
				if (isSub) return;
				
				displayStatistics(e.Message);
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
			}
			if (e.Message.IndexOf("\"notifyType\":\"reconnect\"") >= 0) {
				//{"type":"notify","body":{"notifyType":"reconnect","audienceToken":"8269898711679_225832_1539687621_2779cc2ce649ffb2d8aad1ac5a90d025ee83b8b4","reconnectWaitTime":1}}
				var waitTime = util.getRegGroup(e.Message, "\"reconnectWaitTime\":(\\d+)");
				if (waitTime == null) return;
				Thread.Sleep(int.Parse(waitTime) * 1000);
				
				try {
					ws.Close();
				} catch (Exception ee) {
					addDebugBuf("notify ws close exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
				#if DEBUG
<<<<<<< HEAD
					if (!isRtmp)
						rm.form.addLogText("notify reconnect");
=======
<<<<<<< HEAD
					if (!isRtmp)
						rm.form.addLogText("notify reconnect");
=======
					rm.form.addLogText("notify reconnect");
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
>>>>>>> 7626697f2a2b755bc0d503452dc12ba5710be7f0
				#endif
				
				
			}
			if (e.Message.IndexOf("\"command\":\"servertime\",\"params\"") >= 0) {
				var _t = (int)(long.Parse(util.getRegGroup(e.Message, "(\\d+)")) / 1000);
				jisa = util.getUnixToDatetime(_t) - DateTime.Now;
				
			}
			if (e.Message.IndexOf("\"command\":\"schedule\"") >= 0) {
				if (isSub) return;
				
				var _beginTime = (int)(long.Parse(util.getRegGroup(e.Message, "\"begintime\":(\\d+)")) / 1000);
				var _endTime = (int)(long.Parse(util.getRegGroup(e.Message, "\"endtime\":(\\d+)")) / 1000);
				var beginTime = util.getUnixToDatetime(_beginTime);
				var endTime = util.getUnixToDatetime(_endTime);
				programTime = endTime - beginTime;
				
				//if (!isTimeShift)
//					displaySchedule();
				if (util.isStdIO) {
					Console.WriteLine("info.startTime:" + beginTime.ToString("MM/dd(ddd) HH:mm:ss"));
					Console.WriteLine("info.endTime:" + endTime.ToString("MM/dd(ddd) HH:mm:ss"));
					Console.WriteLine("info.programTime:" + programTime.ToString("h'時間'mm'分'ss'秒'"));
				}
			}
			if (e.Message.IndexOf("\"command\":\"postkey\"") >= 0) {
				if (sendCommentBuf != null && (rm.isPlayOnlyMode || wsc != null))
					sendCommentWsc(e.Message);
					
			}
		}
		public void displaySchedule() {
			DateTime keikaTimeStart = DateTime.MinValue;
			Task.Run(() => {
				while (rm.rfu == rfu && isRetry) {
<<<<<<< HEAD
	         		if (isTimeShift && tsHlsRequestTime == DateTime.MinValue) {
<<<<<<< HEAD
	         			Thread.Sleep(1000);
=======
<<<<<<< HEAD
	         			Thread.Sleep(1000);
=======
=======
	         		if (isTimeShift && tsHlsRequestTime == null) {
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
	         			Thread.Sleep(300);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
>>>>>>> 7626697f2a2b755bc0d503452dc12ba5710be7f0
	         			continue;
	         		}       
			        DateTime _keikaTimeStart = (!isTimeShift) ? (util.getUnixToDatetime(openTime) - jisa) : (tsHlsRequestTime - tsStartTime - jisa);
			        if (keikaTimeStart == _keikaTimeStart)
			        	_keikaTimeStart = DateTime.MinValue;
			        else keikaTimeStart = _keikaTimeStart;
			        
			        TimeSpan _keikaJikanDt;
			        if (!isTimeShift)
						//_keikaJikanDt = (DateTime.Now - util.getUnixToDatetime(openTime) + jisa);
			        	_keikaJikanDt = (DateTime.Now - keikaTimeStart);
					else 
						//_keikaJikanDt = (DateTime.Now - tsHlsRequestTime + tsStartTime + jisa);
						_keikaJikanDt = (DateTime.Now - keikaTimeStart);
			        var keikaJikanH = (int)(_keikaJikanDt.TotalHours);
					var keikaJikan = _keikaJikanDt.ToString("''mm'分'ss'秒'");
					if (keikaJikanH != 0) keikaJikan = keikaJikanH.ToString() + "時間" + keikaJikan;
					
					var timeLabelKeikaH = (int)(_keikaJikanDt.TotalHours);
					var timeLabelKeika = _keikaJikanDt.ToString("''mm':'ss''");
					if (timeLabelKeikaH != 0) timeLabelKeika = timeLabelKeikaH.ToString() + ":" + timeLabelKeika;
					
					var programTimeStrH = (int)(programTime.TotalHours);
					var programTimeStr = programTime.ToString("''mm':'ss''");
					if (programTimeStrH != 0) programTimeStr = programTimeStrH.ToString() + ":" + programTimeStr;
					
					//var keikaJikan = _keikaJikanDt.ToString("H'時間'm'分's'秒'");
					//var programTimeStr = programTime.ToString("h'時間'm'分's'秒'");
					rm.form.setKeikaJikan(keikaJikan, timeLabelKeika + "/" + programTimeStr, _keikaJikanDt.ToString("h'時間'mm'分'ss'秒'"), _keikaTimeStart);
<<<<<<< HEAD
					System.Threading.Thread.Sleep(1000);
=======
<<<<<<< HEAD
					System.Threading.Thread.Sleep(1000);
=======
					System.Threading.Thread.Sleep(100);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
>>>>>>> 7626697f2a2b755bc0d503452dc12ba5710be7f0
				}
			});
		}
		private void sendIntervalPong() {
			while (true) {
				sendPong();
				System.Threading.Thread.Sleep(10000);
			}
		}
		private void sendPong() {
	    	try {
				var dt = System.DateTime.Now.ToShortTimeString();
				ws.Send("{\"body\":{},\"type\":\"pong\"}");
				ws.Send("{\"type\":\"watch\",\"body\":{\"command\":\"watching\",\"params\":[\"" + broadcastId + "\",\"-1\",\"0\"]}}");
				addDebugBuf("send {\"body\":{},\"type\":\"pong\"} and watching" + dt);
				addDebugBuf("send {\"type\":\"watch\",\"body\":{\"command\":\"watching\",\"params\":[\"" + broadcastId + "\",\"-1\",\"0\"]}}" + dt);
			} catch (Exception e) {
				addDebugBuf(e.Message+e.StackTrace);
			}
		}
		private void record(String message, string currentQuality) {
			string hlsUrl = util.getRegGroup(message, "\"uri\":\"(.+?)\"");;
<<<<<<< HEAD
			addDebugBuf("rec " + string.Join(" ", recFolderFile));
			//rm.hlsUrl = hlsUrl;
			addDebugBuf(hlsUrl);
=======
			util.debugWriteLine("rec " + string.Join(" ", recFolderFile));
			//rm.hlsUrl = hlsUrl;
			util.debugWriteLine(hlsUrl);
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
				
			
			Task.Run(() => {
			   	if (rec == null) {
<<<<<<< HEAD
			        rec = new Record(rm, true, rfu, hlsUrl, recFolderFile[1], lastSegmentNo, container, isTimeShift, this, lvid, tsConfig, openTime, ws, recFolderFile[2], isSub);
=======
<<<<<<< HEAD
			         		rec = new Record(rm, true, rfu, hlsUrl, recFolderFile[1], lastSegmentNo, container, isTimeShift, this, lvid, tsConfig, openTime, ws, recFolderFile[2], isSub);
=======
<<<<<<< HEAD
					rec = new Record(rm, true, rfu, hlsUrl, recFolderFile[1], lastSegmentNo, container, isTimeShift, this, lvid, tsConfig, openTime, ws);
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
					
					rec.record(currentQuality);
=======
					rec = new Record(rm, true, rfu, hlsUrl, recFolderFile[1], lastSegmentNo, container, isTimeShift, this, lvid, tsConfig);
					
					rec.record();
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
					if (rec.isEndProgram) {
						addDebugBuf("stop websocket recd");
						isRetry = false;
						if (rr != null) rr.retryMode = 2;
//						if (tscg != null) tscg.setIsRetry(false);
						isEndProgram = true;
					}
	         	} else {
			    	rec.reSetHlsUrl(hlsUrl, currentQuality, ws);
	         	}
				 
//				stopRecording();
			});
		}
		private void connectMessageServer(string message, WebSocket _ws) {
<<<<<<< HEAD
	    	addDebugBuf("connect message server");
	    	addDebugBuf("isretry " + isRetry + " isend " + isEndProgram);
<<<<<<< HEAD
//			msUri = util.getRegGroup(message, "messageServerUri\"\\:\"(ws.+?)\"");
//			msThread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
=======
<<<<<<< HEAD
//			msUri = util.getRegGroup(message, "messageServerUri\"\\:\"(ws.+?)\"");
//			msThread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
=======
			msUri = util.getRegGroup(message, "messageServerUri\"\\:\"(ws.+?)\"");
=======
	    	util.debugWriteLine("connect message server");
	    	util.debugWriteLine("isretry " + isRetry + " isend " + isEndProgram);
			msUri = util.getRegGroup(message, "(ws://.+?)\"");
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			msThread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
	//		String msReq = "[truncated][{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-1000,\"with_global\":1,\"scores\":1,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]\0";
	
//			var res_from = (isTimeShift) ? "-2000" : "-10";
//			msReq = new string[] {"[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + res_from + ",\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]"};
//			msReq = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":1,\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			
	//		String msReq = "{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-100,\"with_global\":1,\"scores\":1,\"nicoru\":0}}";
	//		String msReq = "{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"res_from\":-1000}}";
	//		String msReq = "<thread thread=\"" + msThread + "\" version=\"20061206\" res_from=\"-100\" />\0";
			string msPort = util.getRegGroup(message, "jp:(.+?)/");
			string msAddr = util.getRegGroup(message, "://(.+?):");
			
			addDebugBuf(msAddr + " " + msPort);
			addDebugBuf("msuri " + msUri);
			addDebugBuf("msreq " + msReq[0]);
<<<<<<< HEAD
//			if (rm.isPlayOnlyMode) return;
=======
			if (rm.isPlayOnlyMode) return;
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			var header =  new List<KeyValuePair<string, string>>();
			header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
			//wsc = new WebSocket(msUri,  "", null, header, "", "", WebSocketVersion.Rfc6455);
			wsc = new WebSocket(msUri, "", null, header, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36", "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12);
			wsc.Opened += onWscOpen;
			wsc.Closed += onWscClose;
			wsc.MessageReceived += onWscMessageReceive;
			wsc.Error += onWscError;
			himodukeWS[0] = _ws;
			himodukeWS[1] = wsc;
			
	        addDebugBuf(msUri);
	        
	        wsc.Open();
	        	        
	        
			addDebugBuf("ms start");
			
			
		}
		public void setMsInfo(string msg) {
			msUri = util.getRegGroup(msg, "messageServerUri\"\\:\"(ws.+?)\"");
			msThread = util.getRegGroup(msg, "threadId\":\"(.+?)\"");
			var res_from = (isTimeShift) ? "-2000" : "-10";
			msReq = new string[] {"[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + res_from + ",\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]"};
		}
		public void stopRecording(WebSocket _ws, WebSocket _wsc) {
			addDebugBuf("stop recording");
			try {
				if (_ws != null && _ws.State != WebSocketState.Closed) {
					try {
						_ws.Close();
					} catch (Exception e) {
						addDebugBuf("stoprecording ws close exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
					}
				}
//				_ws = null;
			} catch (Exception e) {
				addDebugBuf("ws close error");
				addDebugBuf(e.Message + e.StackTrace);
			}
			try {
				if (_wsc != null && _wsc.State != WebSocketState.Closed && _wsc.State != WebSocketState.Closing) {
					addDebugBuf("state close wsc " + _wsc.State);					
					_wsc.Close();
				}
			} catch (Exception e) {
				addDebugBuf("wsc close error");
				addDebugBuf(e.Message + e.StackTrace);
			}
			try {
//				if (commentSW != null) commentSW.Close();
			} catch (Exception e) {
				addDebugBuf("comment sw close error");
				addDebugBuf(e.Message + e.StackTrace);
			}
			/*
			try {
				if (rec != null) rec.stopRecording();
			} catch (Exception e) {
				addDebugBuf("rec close error");
				addDebugBuf(e.Message + e.StackTrace);
			}
			isRetry = false;
			*/
		}
		/*
		private void endStreamProcess() {
			if (rm.rfu != rfu) return;
					
			//string[] recFolderFile = util.getRecFolderFilePath(recFolderFile[0], recFolderFile[1], recFolderFile[2], recFolderFile[3], recFolderFileInfo[4]);
			addDebugBuf("recforlderfie");
			addDebugBuf("recforlderfi " + string.Join(" ",recFolderFile));
			if (recFolderFile == null) return;
			
			if (!h5r.isAliveStream()) return;
			start();
		}
		*/
		private void displayStatistics(string e) {
			var visit = util.getRegGroup(e, "{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"(\\d+?)\",\"\\d+?\"", 1, rm.regGetter.getWrVisit());
			var comment = util.getRegGroup(e, "{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"\\d+?\",\"(\\d+?)\"", 1, rm.regGetter.getWrComment());
			try {
				if (visit != null)
					visit = int.Parse(visit).ToString("n0");
				if (comment != null) 
					comment = int.Parse(comment).ToString("n0");
				rm.form.setStatistics(visit, comment);
			} catch (Exception ee){
				addDebugBuf(ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
			}
		}
		private void onWscOpen(object sender, EventArgs e) {
			addDebugBuf("ms open a");
			wsc.Send(msReq[0]);
			addDebugBuf("ms open b");
			
			if (rm.rfu != rfu) {
				//stopRecording();
//				wsc.Close();
				return;
			}			
			try {
<<<<<<< HEAD
				if (bool.Parse(isGetComment) && commentSW == null && !rm.isPlayOnlyMode) {
=======
				if (bool.Parse(isGetComment) && commentSW == null) {
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
					var commentFileName = util.getOkCommentFileName(rm.cfg, recFolderFile[1], lvid, isTimeShift);
					var isExists = File.Exists(commentFileName);
					commentSW = new StreamWriter(commentFileName, false, System.Text.Encoding.UTF8);
					if (bool.Parse(isGetCommentXml) && !isExists) {
						commentSW.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
				        commentSW.WriteLine("<packet>");
				        commentSW.Flush();
					} 
			       
				}
			} catch (Exception ee) {
				addDebugBuf(ee.Message + " " + ee.StackTrace);
			}
			Task.Run(() => {pongWsc((WebSocket)sender);});
		}
		private void pongWsc(WebSocket _wsc) {
<<<<<<< HEAD
			while (_wsc.State == WebSocket4Net.WebSocketState.Open && !isEndProgram && isRetry) {
=======
			while (_wsc.State == WebSocket4Net.WebSocketState.Open) {
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
				try {
					_wsc.Send("");
					Thread.Sleep(60000);
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private void onWscClose(object sender, EventArgs e) {
			addDebugBuf("ms onclose");
			closeWscProcess();
			wsc = null;
			try {
				if (rm.rfu == rfu && isRetry && !isTimeShiftCommentGetEnd) {
					try {
						ws.Close();
					} catch (Exception ee) {
						addDebugBuf("on wsc close ws close exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
					}
				}
			} catch (Exception ee) {
				addDebugBuf(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			
		}
		private void closeWscProcess() {
			addDebugBuf("close wsc process");
			if (isTimeShift && isTimeShiftCommentGetEnd) return;
			
			if (commentSW != null) {
				if (bool.Parse(isGetCommentXml)) {
					try {
						commentSW.WriteLine("</packet>");
						commentSW.Flush();
					} catch (Exception ee) {
						addDebugBuf(ee.Message + " " + ee.StackTrace + " " + ee.TargetSite);
					}
				}
//				commentSW.Close();
//				commentSW = null;
			}
			

			//stopRecording();
//			endStreamProcess();
		}
		private void onWscError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			addDebugBuf("ms onerror");
			//stopRecording();
//			endStreamProcess();
		}
		private void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
			addDebugBuf("on wsc message " + e.Message);
			
			if (isTimeShift && e.Message.StartsWith("{\"ping\":{\"content\":\"rf:")) {
				closeWscProcess();
				try {commentSW.Close();}
				catch (Exception eee) {addDebugBuf(eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);}
				isTimeShiftCommentGetEnd = true;
				rm.form.addLogText("コメントの保存を完了しました");
			}
			
			if (rm.rfu != rfu || (!isTimeShift && !isRetry)) {
				try {
					if (wsc != null) wsc.Close();
				} catch (Exception ee) {
					addDebugBuf("wsc message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
				}
				//stopRecording();
				addDebugBuf("tigau rfu comment" + e.Message);
				return;
			}
			
			
			var xml = JsonConvert.DeserializeXNode(e.Message);
			var chatinfo = new namaichi.info.ChatInfo(xml);
			
			XDocument chatXml;
			if (isTimeShift) chatXml = chatinfo.getFormatXml(openTime);
			else chatXml = chatinfo.getFormatXml(serverTime);
			addDebugBuf("xml " + chatXml.ToString());
			
			if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
					chatinfo.premium == "3")) return;
			if (chatinfo.root != "chat" && chatinfo.root != "thread") return;
			
			if (chatinfo.root == "thread") {
				serverTime = chatinfo.serverTime;
				ticket = chatinfo.ticket;
			}
			
			addDebugBuf("wsc message " + ws);
			
//			Newtonsoft.Json
			//if (e.Message.IndexOf("chat") < 0 &&
			//    	e.Message.IndexOf("thread") < 0) return;
			
			
//            addDebugBuf(jsonCommentToXML(text));
			try {
				if (commentSW != null) {
					var writeStr = (bool.Parse(isGetCommentXml)) ? 
						chatXml.ToString() : 
						(Regex.Replace(e.Message, 
			            	"\"vpos\"\\:(\\d+)", 
			            	"\"vpos\":" + chatinfo.vpos + ""));
					commentSW.WriteLine(writeStr);
					commentSW.Flush();
					addDebugBuf("write comment " + writeStr);
		             
				}
           
			} catch (Exception ee) {addDebugBuf("comment write exception " + ee.Message + " " + ee.StackTrace);}
			
			//if (!isTimeShift)
				addDisplayComment(chatinfo);

		}
		private void addDisplayComment(namaichi.info.ChatInfo chat) {
			
			if (chat.root.Equals("thread")) return;
			if (chat.contents == "再読み込みを行いました<br>読み込み中のままの方はお手数ですがプレイヤー下の更新ボタンをお試し下さい") {
				addDebugBuf("chat 再読み込みを行いました");
				return;
			}
			if (chat.contents == null) return;
//			var time = util.getUnixToDatetime(chat.vpos / 100);
//			var unixKijunDt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			var __time = chat.date - openTime; //- (60 * 60 * 9);

//			var __timeDt = util.getUnixToDatetime(__time);
//			var openTimeDt = util.getUnixToDatetime(openTime);
//			var __timeDt0 = __timeDt - openTimeDt;
//			var __timeDt1 = __timeDt0. - new timespunixKijunDt;
			var h = (int)(__time / (60 * 60));
			var m = (int)((__time % (60 * 60)) / 60);
			var _m = (m < 10) ? ("0" + m.ToString()) : m.ToString();
			var s = __time % 60;
			var _s = (s < 10) ? ("0" + s.ToString()) : s.ToString();
			var keikaTime = h + ":" + _m + ":" + _s + "";
			/*
//			- unixKijunDt;
			
//			var __time = new TimeSpan(chat.vpos * 10000);
			var h = (int)(__timeSpan.TotalHours);
			var m = __timeSpan.Minutes;
			var s = __timeSpan.Seconds;
			*/
//			- new TimeSpan(9,0,0);
			var c = (chat.premium == "3") ? "red" :
				((chat.premium == "7") ? "blue" : "black");
			
			
			rm.form.addComment(keikaTime, chat.contents, chat.userId, chat.score, isTimeShift, c);
			
		}
		private bool isFirstChoiceQuality(string currentQuality, string bestGettableQuolity) {
//			var bestGettableQuality = getBestGettableQuolity(message);
			
			
			
			return currentQuality == bestGettableQuolity; 
			
		}
		private string getBestGettableQuolity(string msg) {
			var qualityList = new string[] {"abr", "super_high", "high",
				"normal", "low", "super_low"};
			var gettableList = util.getRegGroup(msg, "\"qualityTypes\"\\:\\[(.+?)\\]").Replace("\"", "").Split(',');
			var ranks = (rm.ri == null) ? (qualityRank.Split(',')) :
					rm.ri.qualityRank;
			
			var bestGettableQuality = "abr";
			foreach(var r in ranks) {
				var q = qualityList[int.Parse(r)];
				if (gettableList.Contains(q)) {
					bestGettableQuality = q;
					break;
				}
			}
			return bestGettableQuality;
		}
		private void sendUseableStreamGetCommand(string bestGettableQuolity) {
			var req = "{\"type\":\"watch\",\"body\":{\"command\":\"getstream\",\"requirement\":{\"protocol\":\"hls\",\"quality\":\"" + bestGettableQuolity + "\",\"isLowLatency\":false}}}";
			ws.Send(req);
		}
		override public void reConnect() {
			addDebugBuf("reconnect wr");
//			onOpen(null, null);
			try {
				ws.Close();
			} catch (Exception e) {
				addDebugBuf("reconnect ws exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
//			ws.Open();
		}
		public void reConnect(WebSocket _ws) {
			addDebugBuf("reconnect " + _ws + " " + _ws.GetHashCode() + " ws " + ws.GetHashCode());
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			try {
				ws.Close();
			} catch (Exception e) {
				addDebugBuf("reconnect ws exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
<<<<<<< HEAD
=======
=======
			//_ws.Close();
			ws.Close();
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		}
		private bool isEndedProgram() {
			var isPass = (DateTime.Now - lastEndProgramCheckTime < TimeSpan.FromSeconds(5));
			addDebugBuf("ispass " + isPass + " lastendprogramchecktime " + lastEndProgramCheckTime);
			if (isPass) return false;
			lastEndProgramCheckTime = DateTime.Now;
			
			var a = new System.Net.WebHeaderCollection();
			var res = util.getPageSource(h5r.url, ref a, container);
			addDebugBuf("isendedprogram url " + h5r.url + " res==null " + (res == null));
			if (res == null) return false;
			if (res.IndexOf("user.login_status = 'not_login'") > -1) {
				addDebugBuf("isendprogram not login");
				var cg = new CookieGetter(rm.cfg);
				var cgTask = cg.getHtml5RecordCookie(h5r.url, isSub);
				cgTask.Wait();
				container = cgTask.Result[0];
				res = util.getPageSource(h5r.url, container, null, false, 5000);
				res = System.Web.HttpUtility.HtmlDecode(res);
				var _webSocketInfo = h5r.getWebSocketInfo(res);
				isNoPermission = true;
				addDebugBuf("isendprogram login websocketInfo " + webSocketInfo[0] + " " + webSocketInfo[1]);
				if (_webSocketInfo[0] == null || _webSocketInfo[1] == null) {
					addDebugBuf(res);
				} else webSocketInfo = _webSocketInfo;
				return false;
			}
			if (res == null) return false;
			var type = util.getPageType(res);
			addDebugBuf("is ended program  pagetype " + type);
			var isEnd = (type == 7 || type == 2 || type == 3 || type == 9);
			return isEnd;
		}
		override public void sendComment(string s, bool is184) {
			if (msThread == null) return;
			sendCommentBuf = s;
			isSend184 = is184;
			ws.Send("{\"type\":\"watch\",\"body\":{\"command\":\"getpostkey\",\"params\":[\"" + msThread + "\"]}}");
		}
		public void sendCommentWsc(string s) {
			var postKey = util.getRegGroup(s, "params\"\\:\\[\"(.+?)\"");
			var vpos = (int)(((TimeSpan)(DateTime.Now - util.getUnixToDatetime(openTime))).TotalMilliseconds / 10) + 60000 + 780;
			vpos = 0;
			var mail = (isSend184) ? ",\"mail\":\"184 \"" : "";
			var premium = (isPremium) ? "1" : "0";
			var command = "[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"chat\":{\"thread\":\"" + msThread + "\",\"vpos\":" + vpos + mail + ",\"ticket\":\"" + ticket + "\",\"user_id\":\"" + userId + "\",\"content\":\"" + sendCommentBuf + "\",\"postkey\":\"" + postKey + "\", \"premium\":" + premium + "}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]"; 
			addDebugBuf("send comment " + command);
			//wsc.Send("[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"chat\":{\"thread\":\"" + msThread + "\",\"vpos\":" + vpos + mail + ",\"ticket\":\"" + ticket + "\",\"user_id\":\"" + userId + "\",\"premium\":1,\"content\":\"うむ\",\"postkey\":\".1535198509.6HZajH6n5HWGDXnbz2fI1-r5LLg\"}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]");
			wsc.Send(command);
			
			sendCommentBuf = null;
		}
		override public string[] getRecFilePath(long openTime) {
			return h5r.getRecFilePath(openTime, isRtmp);
		}
		private void startDebugWriter() {
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			#if !DEBUG
				return;
			#endif
			while ((rm.rfu == rfu && isRetry) || debugWriteBuf.Count > 0) {
				try {
					lock (debugWriteBuf) {
						string[] l = new String[debugWriteBuf.Count + 10];
						debugWriteBuf.CopyTo(l);
						//var l = new List<string>(debugWriteBuf);
						//string[] _l = debugWriteBuf.ToArray();
						//var l = new List<string>(_l.li);
	//					util.debugWriteLine("debugwritebuf count " + debugWriteBuf.Count);
//						var l = debugWriteBuf.ToList<string>();
						foreach (var b in l) {
							if (b == null) continue;
							util.debugWriteLine(b + " " + util.getMainSubStr(isSub, true));
							debugWriteBuf.Remove(b);
						}
<<<<<<< HEAD
					}
					Thread.Sleep(500);
=======
=======
			while ((rm.rfu == rfu && isRetry) || debugWriteBuf.Count > 0) {
				try {
					//var l = new List<string>(debugWriteBuf);
					//string[] _l = debugWriteBuf.ToArray();
					//var l = new List<string>(_l.li);
					util.debugWriteLine("debugwritebuf count " + debugWriteBuf.Count);
					var l = debugWriteBuf.ToList<string>();
					foreach (var b in l) {
						util.debugWriteLine(b);
						debugWriteBuf.Remove(b);
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
					}
<<<<<<< HEAD
					Thread.Sleep(500);
=======
					Thread.Sleep(100);
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
>>>>>>> 7626697f2a2b755bc0d503452dc12ba5710be7f0
				} catch (Exception e) {
					addDebugBuf("debug writer exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private void addDebugBuf(string s) {
			var dt = DateTime.Now.ToLongTimeString();
			debugWriteBuf.Add(dt + " " + s);
		}
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		private void resetWebsocketInfo() {
			try {
				var cg = new CookieGetter(rm.cfg);
				var cgTask = cg.getHtml5RecordCookie(h5r.url, isSub);
				cgTask.Wait();
				container = cgTask.Result[0];
				var res = util.getPageSource(h5r.url, container, null, false, 5000);
				res = System.Web.HttpUtility.HtmlDecode(res);
				var _webSocketInfo = h5r.getWebSocketInfo(res);
				isNoPermission = true;
				addDebugBuf("resetWebsocketInfo " + _webSocketInfo[0] + " " + _webSocketInfo[1]);
				if (_webSocketInfo[0] == null || _webSocketInfo[1] == null) {
					addDebugBuf(res);
				} else webSocketInfo = _webSocketInfo;
			} catch (Exception e) {
				addDebugBuf("resetWebsocketInfo exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
<<<<<<< HEAD
=======
=======
		public void sendComment(string s, bool is184) {
			if (msThread == null) return;
			sendCommentBuf = s;
			isSend184 = is184;
			ws.Send("{\"type\":\"watch\",\"body\":{\"command\":\"getpostkey\",\"params\":[\"" + msThread + "\"]}}");
		}
		public void sendCommentWsc(string s) {
			var postKey = util.getRegGroup(s, "params\"\\:\\[\"(.+?)\"");
			var vpos = (int)(((TimeSpan)(DateTime.Now - util.getUnixToDatetime(openTime))).TotalMilliseconds / 10) + 60000 + 780;
			vpos = 0;
			var mail = (isSend184) ? ",\"mail\":\"184 \"" : "";
			var premium = (isPremium) ? "1" : "0";
			var command = "[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"chat\":{\"thread\":\"" + msThread + "\",\"vpos\":" + vpos + mail + ",\"ticket\":\"" + ticket + "\",\"user_id\":\"" + userId + "\",\"content\":\"" + sendCommentBuf + "\",\"postkey\":\"" + postKey + "\", \"premium\":" + premium + "}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]"; 
			util.debugWriteLine("send comment " + command);
			//wsc.Send("[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"chat\":{\"thread\":\"" + msThread + "\",\"vpos\":" + vpos + mail + ",\"ticket\":\"" + ticket + "\",\"user_id\":\"" + userId + "\",\"premium\":1,\"content\":\"うむ\",\"postkey\":\".1535198509.6HZajH6n5HWGDXnbz2fI1-r5LLg\"}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]");
			wsc.Send(command);
			
			sendCommentBuf = null;
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		}
	}
}
	