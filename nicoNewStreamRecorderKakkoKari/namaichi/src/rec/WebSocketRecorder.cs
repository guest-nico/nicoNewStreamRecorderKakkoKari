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
using System.Text;
using System.Xml.Linq;
using System.Security.Authentication;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;
using WebSocket4Net;
using Newtonsoft.Json;
using SuperSocket.ClientEngine.Proxy;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of WebSocketRecorder.
	/// </summary>
	public class WebSocketRecorder : IRecorderProcess
	{
		//public CookieContainer container;
		
		//private RecordingManager rm;
		//private RecordFromUrl rfu;
		//public Html5Recorder h5r;
		private WebSocket ws;
		private WebSocket[] wsc = new WebSocket[2];
		public Record rec;
		public RecordStateSetter rss = null;
		public StreamWriter commentSW;
		
		//public long serverTime;
		private string ticket;
		private bool isRetry = true;
		new public bool IsRetry {get{return isRetry;} set{
				//if (!value && (!isTimeShift || isChase) && rm.rfu == rfu && isGetComment == "true") {
				if (!value && isRetry && (!ri.si.isTimeShift || ri.isChase) && isGetComment == "true") {
					checkMissingComment();
				}
				isRetry = value;
			}}
		
		private string msThread;
		private string msStoreThread;
		private string threadkey = null;
		private string sendCommentBuf = null;
		private bool isSend184 = true;
		
		private bool isNoPermission = false;
		public bool isEndProgram = false;

		private bool isTimeShiftCommentGetEnd = false;
		private DateTime lastEndProgramCheckTime = DateTime.Now;
		private DateTime lastWebsocketConnectTime = DateTime.MinValue;
		
		//public TimeSpan jisa;
		
		private string roomName = "";
		
			
		private WebSocket[] himodukeWS = new WebSocket[2];
			
//		private System.Threading.Thread mainThread;
		//public TimeShiftCommentGetter tscg = null;
		
		bool isWaitNextConnection = false;
		List<WebSocket> wsList = new List<WebSocket>();
		
		private List<string> debugWriteBuf = new List<string>();
		private Task tsWriterTask = null;
		
		private RtmpRecorder rr;
		
		private string[] qualityRank = null;
		private string isGetComment = null;
		private bool isGetCommentXml = false;
		private bool isGetCommentXmlInfo = false;
		private string commentFileName = null;
		private bool isFirstCommentAfterOpenWsc = false;
		private string engineMode = null;
		
		//private bool isXmlComment = true;
		private XmlCommentGetter_ontime xcg = null;
		private TimeShiftCommentGetter_xml tscgx = null;
		
		//public List<string> chaseCommentBuf = new List<string>();
		
		private bool isNotSleep = false;
		private List<string> lastSaveComments = new List<string>();
		private DateTime lastOpenCommentSwDt = DateTime.MinValue;
		private bool isConvertSpace;
		//public List<string[]> commentReplaceList = null;
		private bool isSaveCommentOnlyRetryingRec;
		private bool isNormalizeComment;
		
		private object commentLock = new object();
		private bool isLogEnd = false;
		/*
		public WebSocketRecorder(string[] webSocketInfo, 
				CookieContainer container, string[] recFolderFile, 
				RecordingManager rm, RecordFromUrl rfu, 
				Html5Recorder h5r, long openTime, 
				//int lastSegmentNo, 
				bool isTimeShift, string lvid,
				TimeShiftConfig tsConfig, string userId, 
				bool isPremium, TimeSpan programTime, 
				string programType, long _openTime, bool isRtmp, 
				bool isRtmpOnlyPage, bool isChase, bool isRealtimeChase,
				bool isSaveComment, RecordStateSetter rss, long vposBaseTime
			)
		{
			this.webSocketInfo = webSocketInfo;
			this.container = container;
			this.recFolderFile = recFolderFile;
			this.rm = rm;
			this.rfu = rfu;
			this.h5r = h5r;
			this.openTime = openTime;
			//this.lastSegmentNo = lastSegmentNo;
			this.isTimeShift = isTimeShift;
			this.lvid = lvid;
			this.tsConfig = tsConfig;
			this.userId = userId;
			this.isPremium = isPremium;
			this.programTime = programTime;
			isJikken = false;
			this.programType = programType;
			this._openTime = _openTime;
			//this.isSub = isSub;
			this.isRtmp = isRtmp;
			this.rss = rss;
			
			this.qualityRank = (tsConfig == null || tsConfig.qualityRank == null) ?
					rm.cfg.get("qualityRank").Split(',') : tsConfig.qualityRank;
			this.isGetComment = rm.cfg.get("IsgetComment");
			this.isGetCommentXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.isGetCommentXmlInfo = bool.Parse(rm.cfg.get("IsgetcommentXmlInfo"));
			this.engineMode = rm.cfg.get("EngineMode");
			this.isNotSleep = bool.Parse(rm.cfg.get("IsNotSleep"));
			this.isRtmpOnlyPage = isRtmpOnlyPage;
			this.isChase = isChase;
			this.isRealtimeChase = isRealtimeChase;
			this.isSaveComment = isSaveComment;
			if (isChase && !isSaveComment) isHokan = true;
			isConvertSpace = bool.Parse(rm.cfg.get("IsCommentConvertSpace"));
			try {
				commentReplaceList = JsonConvert.DeserializeObject<List<string[]>>(rm.cfg.get("CommentReplaceText"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			isSaveCommentOnlyRetryingRec = bool.Parse(rm.cfg.get("IsSaveCommentOnlyRetryingRec"));
			isNormalizeComment = bool.Parse(rm.cfg.get("IsNormalizeComment"));
			this.vposBaseTime = vposBaseTime;
		}
		*/
		public WebSocketRecorder(CookieContainer container, RecordingManager rm, 
				RecordFromUrl rfu, bool isSaveComment, 
				RecordStateSetter rss, RecordInfo ri) {
			this.container = container;
			this.rm = rm;
			this.rfu = rfu;
			//this.h5r = h5r;
			this.isSaveComment = isSaveComment;
			this.rss = rss;
			this.ri = ri;
			isJikken = false;
			
			this.qualityRank = (ri.timeShiftConfig == null || ri.timeShiftConfig.qualityRank == null) ?
					rm.cfg.get("qualityRank").Split(',') : ri.timeShiftConfig.qualityRank;
			this.isGetComment = rm.cfg.get("IsgetComment");
			this.isGetCommentXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.isGetCommentXmlInfo = bool.Parse(rm.cfg.get("IsgetcommentXmlInfo"));
			this.engineMode = rm.cfg.get("EngineMode");
			this.isNotSleep = bool.Parse(rm.cfg.get("IsNotSleep"));
			
			if (ri.isChase && !isSaveComment) isHokan = true;
			isConvertSpace = bool.Parse(rm.cfg.get("IsCommentConvertSpace"));
			try {
				commentReplaceList = JsonConvert.DeserializeObject<List<string[]>>(rm.cfg.get("CommentReplaceText"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			isSaveCommentOnlyRetryingRec = bool.Parse(rm.cfg.get("IsSaveCommentOnlyRetryingRec"));
			isNormalizeComment = bool.Parse(rm.cfg.get("IsNormalizeComment"));
		}
		public bool start() {
			addDebugBuf("ws rec start");
			
			tsWriterTask = Task.Run(() => {startDebugWriter();});
			
			if (ri.si.isRtmpOnlyPage) {
				Task.Run(() => {
					rtmpRecord(null, null);
					saveXmlComment();
				});
			} else {
				if (ri.isRtmp && ri.si.isTimeShift) {
					Task.Run(() => rtmpRecord(null, null));
				}
				connect();
				addDebugBuf("rm.rfu dds1 " + rm.rfu);
			}
			
			addDebugBuf("ws main " + ws + " a " + (ws == null));
			
			if (!ri.si.isRtmpOnlyPage && !(ri.isChase && !isSaveComment))
				Task.Run(() => displaySchedule());
			
			WebSocket lastWebSocket = null;
			var stopWsCount = 0;
			var lastSendRequired = DateTime.Now;
			while (rm.rfu == rfu && IsRetry) {
				
				if (!ri.isRtmp && ws.State == WebSocket4Net.WebSocketState.Closed) {
					addDebugBuf("no connect loop ws close");
//					connect();
				}
				
				if (ws != null) {
					if ((ws.State != WebSocketState.Open || (rec != null && rec.isReConnecting)) 
					    		&& ws == lastWebSocket) {
						stopWsCount++;
						if (stopWsCount > 10) {
							addDebugBuf("stop ws count " + stopWsCount + " close ws / rec.isreconnecting " + (rec != null ? rec.isReConnecting.ToString() : ""));
							
							rm.form.addLogText("再接続中");
							
							#if DEBUG
								rm.form.addLogText("stop ws reConnect");
							#endif
							isWaitNextConnection = true;
							connectUntilOk();
							stopWsCount = 0;
						}
					} else stopWsCount = 0;
					lastWebSocket = ws;
				}
				
				if (isNotSleep && DateTime.Now - lastSendRequired > TimeSpan.FromSeconds(45)) {
					util.setThreadExecutionState();
					lastSendRequired = DateTime.Now;
				}
				
				System.Threading.Thread.Sleep(1000);
			}

			addDebugBuf("wsr end0");
			IsRetry = false;
			if (rr != null) rr.retryMode = (isEndProgram) ? 2 : 1;
//			if (rm.rfu != rfu && tscg != null) tscg.setIsRetry(false);
			
//			if (isTimeShift && rm.rfu == rfu && tscg != null) {
			if (ri.si.isTimeShift && rm.rfu == rfu && isGetComment == "true") {
//				while (rm.rfu == rfu && (tscg == null || !tscg.isEnd)) {
				while (rm.rfu == rfu && 
				       ((!ri.si.isRtmpOnlyPage && tscg != null && !tscg.isEnd) || 
				       		ri.si.isRtmpOnlyPage && tscgx != null && !tscgx.isEnd)) {
					addDebugBuf("tscg end wait loop tscg " + tscg);
					Thread.Sleep(1000);
				}
			}
//			tscg.setIsRetry(false);
			
			if (rm.rfu != rfu) {
				//if (rr != null) rr.isRetry = false;
				stopRecording();
//				ws.Close();
//				wsc.Close();
				if (rec != null) 
					rec.waitForEnd();
			} else if (rec != null) {
				rec.waitForEnd();
			}
			if (!IsRetry) {
				//if (rr != null) rr.isRetry = false;
				stopRecording();
				if (rec != null)
					rec.waitForEnd();
			}
			if (commentSW != null) closeWscProcess();
			
			if (ri.isChase && rec != null && !rec.isEndProgram && rm.rfu == rfu) {
				#if DEBUG
					//Task.Run(() => rm.form.addLogText("追っかけ録画処理開始"));
					rm.form.addLogText("追っかけ録画処理開始");
				#endif
				
				new ChaseLastRecord(ri.si.lvid, container, rm, 
						ri.si.recFolderFileInfo, ri.si.openTime, ri, 
						ri.timeShiftConfig, rfu, 
						rec != null ? rec.lastSegmentNo : -1).rec(rec.recFolderFile);
			}
			addDebugBuf("closed saikai");
			
			isLogEnd = true;
			if (rec != null) Record.isWriteCancel = false;
			if (ri.userId != null)
				rm.form.addLogText("録画終了処理を完了しました");
			return isNoPermission;
		}
		
		private bool connect() {
			displayDebug("ws connect");
			lock (this) {
				var passTime = (ri.si.isTimeShift && !ri.isRealtimeChase) ? 10 : 2;
				var  isPass = (TimeSpan.FromSeconds(passTime) > (DateTime.Now - lastWebsocketConnectTime));
				lastWebsocketConnectTime = DateTime.Now;
				if (isPass) {
					addDebugBuf("connect passtime " + passTime);
					Thread.Sleep(passTime * 1000);
				}
			}
			if (isWaitNextConnection) {
				addDebugBuf("connect WaitNextConnection");
				Thread.Sleep(90000);
				resetWebsocketInfo();
				isWaitNextConnection = false;
				addDebugBuf("after wait reset  " + " wsList " + wsList.Count);
			}
			
			if (ws != null)
				addDebugBuf("ws connect " + ws.GetHashCode());
			try {
				//ws = new WebSocket(webSocketInfo[0]);
				addDebugBuf("ws connect webSocketInfo[0] " + ri.webSocketRecInfo[0] + " wsList " + wsList.Count);
				ws = new WebSocket(ri.webSocketRecInfo[0], "", null, null, util.userAgent, "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
				ws.Proxy = util.wsProxy;
				ws.Opened += onOpen;
				ws.Closed += onClose;
				ws.DataReceived += onDataReceive;
				ws.MessageReceived += onMessageReceive;
				ws.Error += onError;
				
				ws.Open();
			} catch (Exception ee) {
				addDebugBuf("ws connect exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				Task.Run(() => {
				    endProgramCheck();
				});
				return false;
			}
			
			var _ws = ws;
			try {
//				wsList.Add(ws);
				
//				var _ws = ws; 
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
				try {
					ws.Close();
					_ws.Close();
				} catch (Exception eee) {
					addDebugBuf("ws connect exception " + eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);
				}
				addDebugBuf("ws connect exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				return false;
			}
			return true;
		}
		private void onOpen(object sender, EventArgs e) {
			displayDebug("ws open");
			addDebugBuf("on open rm.rfu dds2 " + rm.rfu + " ws " + sender.GetHashCode() + " wsList " + wsList.Count);
			
			if (sender != ws) {
				((WebSocket)sender).Close();
				addDebugBuf("hukusuu ws close " + sender + "/" + ws);
			}
			
			if (isNoPermission) {
				ri.webSocketRecInfo[1] = ri.webSocketRecInfo[2] == "1" ?
					ri.webSocketRecInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true")
					: ri.webSocketRecInfo[1].Replace("\"reconnect\":true", "\"reconnect\":false");
			}
							
			//String leoReq = "{\"type\":\"watch\",\"body\":{\"command\":\"playerversion\",\"params\":[\"leo\"]}}";
			//addDebugBuf("leoReq " + leoReq);
			//addDebugBuf("websocketinfo1 " + webSocketInfo[1]);
			//sendMessage(ws, leoReq);
			
			sendMessage(ws, ri.webSocketRecInfo[1]);
			
			
			addDebugBuf("open send  " + ws);
			addDebugBuf("rm " + rm + " rm.rfu " + rm.rfu + " rfu " + rfu);
			
//			if (rm.rfu != rfu) stopRecording();
		}
		private void onClose(object sender, EventArgs e) {
			displayDebug("ws close");
			addDebugBuf("on close " + e.ToString() + " ws hash " + sender.GetHashCode() + " istimeshift " + ri.si.isTimeShift + " wsList " + wsList.Count);
			try {
				addDebugBuf("wslist indexof onclose sender " + wsList.IndexOf((WebSocket)sender));
//				wsList.Remove((WebSocket)sender);
			} catch (Exception ee) {util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);}
			addDebugBuf("on close2 " + " wsList " + wsList.Count);
			
			Task.Run(() => {
			    endProgramCheck();
				
			});
			
			//stopRecording();
			
			if (rm.rfu == rfu && !isEndProgram && (WebSocket)sender == ws) {
				connectUntilOk();
				
//				ws.Open();
			}
			

		}
		private void endProgramCheck() {
			displayDebug("end check");
			addDebugBuf("endProgramCheck");
			if ((!ri.si.isTimeShift || ri.isChase) && isEndedProgram()) {
		        addDebugBuf("isEndprogram websocket");
				IsRetry = false;
				if (rr != null) rr.retryMode = 2;
				if (tscg != null) tscg.setIsRetry(false);
				isEndProgram = true;
				if (endTime == DateTime.MinValue)
						endTime = DateTime.Now;
				
			}
		}
		private void onError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			displayDebug("ws error");
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
			displayDebug("ws receive " + new Regex("ht2_nicolive=\\d+").Replace(e.Message, "ht2_nicolive=0"));
			var type = util.getRegGroup(e.Message, "\"" + ((ri.webSocketRecInfo[2] == "1" && e.Message.IndexOf("\"command\"") > -1) ? "command" : "type") + "\":\"(.+?)\"");
			
			if (sender != ws) {
				((WebSocket)sender).Close();
				addDebugBuf("hukusuu ws close " + sender + "/" + ws);
			}
			
//			addDebugBuf("ws " + ws);
			if (ws == null) return;
			//pong
			if (type == "ping") {
				sendPong();
			}
			
			//get message
			if (type == "room" || type == "messageServerUri" || type == "currentroom") {
				//if (isSub) return;
				if (!isSaveComment) return;
				
				setMsInfo(e.Message);
				if (ri.si.isTimeShift && !ri.isRealtimeChase) {
					if (tscg == null) {
						if (!(ri.isChase && chaseCommentBuf == null)) {
							tscg = new TimeShiftCommentGetter(msUri, msThread, msStoreUri, msStoreThread,                                  
									ri.userId, rm, rfu, rm.form, ri,
									ri.si.lvid, container,
									//ri.si.type, 
									this,
									//(ri.isRtmp) ? 0 : ri.timeShiftConfig.timeSeconds, 
									//(ri.isRtmp) ? false : ri.timeShiftConfig.isVposStartTime, 
									ri.isRtmp, rr, roomName, ri.timeShiftConfig,
									null, true, false, null, null);
							tscg.save();
						} else {
							util.debugWriteLine("not tscg ischase commentbuf null");
							#if DEBUG
								rm.form.addLogText("not tscg ischase commentbuf null");
							#endif
						}
					}
					
				}
				if (!ri.si.isTimeShift || ri.isChase) {
					if (rfu == rm.rfu && wsc[0] == null && IsRetry) {
						connectMessageServer((WebSocket)sender);
					}
				}
			}
			
			//record
			if (type == "stream" || type == "currentstream") {
				//addDebugBuf("mediaservertype = " + util.getRegGroup(e.Message, "(\"mediaServerType\".\".+?\")"));
				if (ri.isChase) addDebugBuf("isChase true isSaveComment " + isSaveComment);
				if (ri.si.isRtmpOnlyPage) {
					return;
							//isRtmp || 
//				    		(rm.cfg.get("IsHokan") == "true" && 
//				     		!rfu.isRtmpMain && !rm.isPlayOnlyMode && 
//				     		engineMode == "0" && !isTimeShift)) {
				}
				
				var bestGettableQuolity = getBestGettableQuolity(e.Message);
				if (ri.isChase && !isChaseStream(e.Message)) {
					var chaseReq = ri.webSocketRecInfo[2] == "1" ? 
						"{\"type\":\"watch\",\"body\":{\"command\":\"getstream\",\"requirement\":{\"protocol\":\"hls\",\"isChasePlay\":true}}}"
						: "{\"type\":\"changeStream\",\"data\":{\"quality\":\"" + bestGettableQuolity + "\",\"protocol\":\"hls\",\"latency\":\"low\",\"chasePlay\":true}}";
					sendMessage(ws, chaseReq);
					util.debugWriteLine("chase sent " + chaseReq);
					return;
				}
				if (ri.isChase) {
					addDebugBuf("got chaseStream");
				}
//				if (engineMode == "3") return;
				
				var currentQuality = util.getRegGroup(e.Message, "\"quality\":\"(.+?)\"");
				if (!(ri.isRtmp && ri.si.isTimeShift)) {
					if (isFirstChoiceQuality(currentQuality, bestGettableQuolity)) {
						if (ri.isRtmp) {
							Task.Run(() => rtmpRecord(e.Message, currentQuality));
						} else record(e.Message, currentQuality);
					} else 
						sendUseableStreamGetCommand(bestGettableQuolity);
				}
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
			}
			
			//new stream retry
			if (e.Message.IndexOf("\"NO_PERMISSION\"") >= 0
				    || e.Message.IndexOf("\"TAKEOVER\"") >= 0
				    || e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") >= 0
				   	|| e.Message.IndexOf("\"END_PROGRAM\"") >= 0
				    || e.Message.IndexOf("\"TOO_MANY_CONNECTIONS\"") >= 0
				    || e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0
				    || e.Message.IndexOf("\"CROWDED\"") >= 0
				    || e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0) {
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0) rm.form.addLogText("追い出されました。");
				
				//SERVICE_TEMPORARILY_UNAVAILABLE 予約枠開始後に何らかの問題？
				if (e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") > 0) {
					rm.form.addLogText("サーバーからデータの受信ができませんでした。リトライします。");
					Thread.Sleep(3000);
				}
				
				if (e.Message.IndexOf("\"END_PROGRAM\"") > 0 && !ri.si.isTimeShift) {
					addDebugBuf("END_PROGRAM receive");
					isEndProgram = true;
					if (endTime == DateTime.MinValue)
						endTime = DateTime.Now;
					IsRetry = false;
					if (rr != null) rr.retryMode = 2;
					if (tscg != null) tscg.setIsRetry(false);
					Thread.Sleep(15000);
				}
				
				addDebugBuf("websocket error kei");
//				connect(webSocketInfo[0].Replace("\"requireNewStream\":false", "\"requireNewStream\":true"));
				isNoPermission = true;
				if (e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0 ||
				    	e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0) {
					isWaitNextConnection = true;
					//{"type":"error","body":{"code":"CONNECT_ERROR"}}
					
					if (e.Message.IndexOf("\"TEMPORARILY_CROWDED\"") >= 0 || e.Message.IndexOf("CROWDED") > -1)
						rm.form.addLogText("満員でした");
					
					if (e.Message.IndexOf("\"CONNECT_ERROR\"") >= 0)
						rm.form.addLogText("接続エラーでした");
				//stopRecording();
//				reConnect();
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
				}
			} else if (e.Message.IndexOf("\"INTERNAL_SERVERERROR\"") >= 0 ||
			          	e.Message.IndexOf("\"INVALID_MESSAGE\"") >= 0) {
				try {
					ws.Close();
					#if DEBUG
						rm.form.addLogText(e.Message);
					#endif
				} catch (Exception ee) {
					addDebugBuf("notify ws close exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			} else if (type == "disconnect") {
				addDebugBuf("unknown disconnect");
				isNoPermission = true;
				//stopRecording();
//				reConnect();
			}
			//if (e.Message.IndexOf("\"command\":\"statistics\",\"params\"") >= 0
			if (type == "statistics") {
				//if (isSub) return;
				
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
					rm.form.addLogText("notify reconnect");
				#endif
			}
			//if (e.Message.IndexOf("{\"type\":\"error\",\"body\":{\"code\":\"CONTENT_NOT_READY\"}}") > -1) {
			if (e.Message.IndexOf("CONTENT_NOT_READY") > -1) {
				Thread.Sleep(3000);
				util.debugWriteLine("sendUseableStreamGetCommand");
				sendUseableStreamGetCommand("normal");
			}
			if (type == "serverTime" || type == "servertime") {
				if (ri.webSocketRecInfo[2] == "1") {
					var _t = (int)(long.Parse(util.getRegGroup(e.Message, "(\\d+)")) / 1000);
					jisa = util.getUnixToDatetime(_t) - DateTime.Now;
				} else {
					var _t = DateTime.Parse(util.getRegGroup(e.Message, "\"currentMs\":\"(.+?)\""));
					jisa = _t - DateTime.Now;
				}
				
			}
			//if (e.Message.IndexOf("\"command\":\"schedule\"") >= 0) {
			if (type == "schedule") {
			
				//if (isSub) return;
				DateTime beginTime, endTime;
				if (ri.webSocketRecInfo[2] == "1") {
					var _beginTime = (int)(long.Parse(util.getRegGroup(e.Message, "\"begintime\":(\\d+)")) / 1000);
					var _endTime = (int)(long.Parse(util.getRegGroup(e.Message, "\"endtime\":(\\d+)")) / 1000);
					beginTime = util.getUnixToDatetime(_beginTime);
					endTime = util.getUnixToDatetime(_endTime);
				} else {
					beginTime = DateTime.Parse(util.getRegGroup(e.Message, "\"begin\":\"(.+?)\""));
					endTime = DateTime.Parse(util.getRegGroup(e.Message, "\"end\":\"(.+?)\""));
				}
				ri.si.programTime = endTime - beginTime;
				
				//if (!isTimeShift)
//					displaySchedule();
				if (util.isStdIO) {
					util.consoleWrite("info.startTime:" + beginTime.ToString("MM/dd(ddd) HH:mm:ss"));
					util.consoleWrite("info.endTime:" + endTime.ToString("MM/dd(ddd) HH:mm:ss"));
					util.consoleWrite("info.programTime:" + ri.si.programTime.ToString("h'時間'mm'分'ss'秒'"));
				}
			}
			if (e.Message.IndexOf("\"NO_STREAM_AVAILABLE\"") >= 0) {
				rm.form.addLogText("配信データが取得できませんでした");
			}
			if (type == "postkey") {
				if (sendCommentBuf != null && (rfu.isPlayOnlyMode || wsc[0] != null))
					sendCommentWsc(e.Message);
			}
			if (type == "postCommentResult") {
			}
		}
		public void displaySchedule() {
			//Task.Run(() => {
			    DateTime keikaTimeStart = DateTime.MinValue;
				while (rm.rfu == rfu && IsRetry) {
	         		if (ri.si.isTimeShift && tsHlsRequestTime == DateTime.MinValue && !ri.isChase) {
	         			Thread.Sleep(1000);
	         			continue;
	         		}       
			        DateTime _keikaTimeStart = (!ri.si.isTimeShift || ri.isChase) ? (util.getUnixToDatetime(ri.si.openTime) - jisa) : (tsHlsRequestTime - tsStartTime - jisa);
			        if (keikaTimeStart == _keikaTimeStart)
			        	_keikaTimeStart = DateTime.MinValue;
			        else keikaTimeStart = _keikaTimeStart;
			        
			        TimeSpan _keikaJikanDt;
			        if (!ri.si.isTimeShift || ri.isChase)
						//_keikaJikanDt = (DateTime.Now - util.getUnixToDatetime(openTime) + jisa);
			        	_keikaJikanDt = (DateTime.Now - keikaTimeStart);
			        else {
			        	if (rec != null && rec.lastRecordedSeconds != -1)
							_keikaJikanDt = TimeSpan.FromSeconds(rec.lastRecordedSeconds);
						else _keikaJikanDt = (DateTime.Now - keikaTimeStart);
			        }
			        var keikaJikanH = (int)(_keikaJikanDt.TotalHours);
					var keikaJikan = _keikaJikanDt.ToString("''mm'分'ss'秒'");
					if (keikaJikanH != 0) keikaJikan = keikaJikanH.ToString() + "時間" + keikaJikan;
					
					var timeLabelKeikaH = (int)(_keikaJikanDt.TotalHours);
					var timeLabelKeika = _keikaJikanDt.ToString("''mm':'ss''");
					if (timeLabelKeikaH != 0) timeLabelKeika = timeLabelKeikaH.ToString() + ":" + timeLabelKeika;
					
					var programTimeStrH = (int)(ri.si.programTime.TotalHours);
					var programTimeStr = ri.si.programTime.ToString("''mm':'ss''");
					if (programTimeStrH != 0) programTimeStr = programTimeStrH.ToString() + ":" + programTimeStr;
					
					//var keikaJikan = _keikaJikanDt.ToString("H'時間'm'分's'秒'");
					//var programTimeStr = programTime.ToString("h'時間'm'分's'秒'");
					rm.form.setKeikaJikan(keikaJikan, timeLabelKeika + "/" + programTimeStr, _keikaJikanDt.ToString("h'時間'mm'分'ss'秒'"), _keikaTimeStart);
					System.Threading.Thread.Sleep(1000);
				}
			//});
		}
		/*
		private void sendIntervalPong() {
			while (true) {
				sendPong();
				System.Threading.Thread.Sleep(10000);
			}
		}
		*/
		private void sendPong() {
	    	try {
				if (ri.webSocketRecInfo[2] == "1") {
					var dt = System.DateTime.Now.ToShortTimeString();
					sendMessage(ws, "{\"body\":{},\"type\":\"pong\"}");
					sendMessage(ws, "{\"type\":\"watch\",\"body\":{\"command\":\"watching\",\"params\":[\"" + ri.webSocketRecInfo[3] + "\",\"-1\",\"0\"]}}");
				} else {
					sendMessage(ws, "{\"type\":\"pong\"}");
					sendMessage(ws, "{\"type\":\"keepSeat\"}");
				}
			} catch (Exception e) {
				addDebugBuf(e.Message+e.StackTrace);
			}
		}
		private void record(String message, string currentQuality) {
			string hlsUrl = util.getRegGroup(message, "\"uri\":\"(.+?)\"");;
			addDebugBuf("rec " + string.Join(" ", ri.recFolderFile));
			//rm.hlsUrl = hlsUrl;
			addDebugBuf(hlsUrl);
				
			if (rec == null) {
		        //rec = new Record(rm, true, rfu, hlsUrl, ri.recFolderFile[1], container, ri.si.isTimeShift, this, ri.si.lvid, ri.timeShiftConfig, ri.si.openTime, ws, ri.recFolderFile[2], ri.isRealtimeChase, h5r);
		        rec = new Record(rm, rfu, hlsUrl, container, this, ws, ri);
		        Task.Run(() => {
			        rec.record(currentQuality);
					if (rec.isEndProgram || isEndProgram) {
						addDebugBuf("stop websocket recd");
						rm.form.addLogTextDebug("end record");
						
						IsRetry = false;
						if (rr != null) rr.retryMode = 2;
	//						if (tscg != null) tscg.setIsRetry(false);
						isEndProgram = true;
					}
				});
			} else {
				//Task.Run(() => {
					rec.reSetHlsUrl(hlsUrl, currentQuality, ws, false);
				//});
	        }
		}
		private void connectMessageServer(WebSocket _ws) {
	    	addDebugBuf("connect message server");
	    	addDebugBuf("isretry " + IsRetry + " isend " + isEndProgram);
			
			var header =  new List<KeyValuePair<string, string>>();
			header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
			wsc[0] = new WebSocket(msUri, "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
			wsc[0].Proxy = util.wsProxy;
			wsc[0].Opened += onWscOpen;
			wsc[0].Closed += onWscClose;
			wsc[0].MessageReceived += onWscMessageReceive;
			wsc[0].Error += onWscError;
//			himodukeWS[0] = _ws;
//			himodukeWS[1] = wsc;
			
	        addDebugBuf(msUri);
	        
	        wsc[0].Open();
			
	        if (msStoreUri != null) {
	        	wsc[1] = new WebSocket(msStoreUri, "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
	        	wsc[1].Opened += onWscOpen;
	        	wsc[1].Closed += onWscClose;
	        	wsc[1].MessageReceived += onWscMessageReceive;
	        	wsc[1].Error += onWscError;
	        	wsc[1].Open();
	        }
	        
			addDebugBuf("ms start ws");
			
			
		}
		public void setMsInfo(string msg) {
			
			msUri = util.getRegGroup(msg, ri.webSocketRecInfo[2] == "1" ? 
					"messageServerUri\"\\:\"(ws.+?)\"" : "\"uri\":\"(ws.+?)\"");
			
			msThread = util.getRegGroup(msg, "threadId\":\"(.+?)\"");
			
			var res_from = (ri.si.isTimeShift && !ri.isChase) ? "-2000" : (lastSaveComments.Count > 0 ? "-150" : "-10");
			threadkey = util.getRegGroup(msg, "\"yourPostKey\".\"(.+?)\"");
			msReq = new string[] {"[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + ri.userId + "\",\"res_from\":" + res_from + ",\"with_global\":1,\"scores\":1,\"nicoru\":0" + (threadkey == null ? "" : (",\"threadkey\":\"" + threadkey + "\"")) + "}" + "},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]"};
			
			if (bool.Parse(rm.cfg.get("IsgetcommentStore"))) {
				msStoreUri = util.getRegGroup(msg, ri.webSocketRecInfo[2] == "1" ? 
					"messageServerUri\"\\:\"(ws.+?)\"" : "\"uri\":\"(ws.+?)\"");
				msStoreThread = util.getRegGroup(msg, "threadId\":\"(.+?)\"");
				msStoreReq = new string[] {"[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + ri.userId + "\",\"res_from\":" + res_from + ",\"with_global\":1,\"scores\":1,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]"};
			}
			
			var _roomName = util.getRegGroup(msg, ri.webSocketRecInfo[2] == "1" ? 
					"\"roomName\":\"(.+?)\"" : "\"name\":\"(.+?)\"");
			if (_roomName != null) roomName = _roomName;
		}
		public void stopRecording(WebSocket _ws, WebSocket[] _wsc) {
			displayDebug("stop recording");
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
				if (_wsc[0] != null && _wsc[0].State != WebSocketState.Closed && _wsc[0].State != WebSocketState.Closing) {
					addDebugBuf("state close wsc " + _wsc[0].State);
					_wsc[0].Close();
				}
			} catch (Exception e) {
				addDebugBuf("wsc0 close error");
				addDebugBuf(e.Message + e.StackTrace);
			}
			try {
				if (_wsc[1] != null && _wsc[1].State != WebSocketState.Closed && _wsc[1].State != WebSocketState.Closing) {
					addDebugBuf("state close wsc " + _wsc[1].State);
					_wsc[1].Close();
				}
			} catch (Exception e) {
				addDebugBuf("wsc1 close error");
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
		override public void stopRecording() {
			stopRecording(ws, wsc);
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
			string _visit, _comment;
			if (ri.webSocketRecInfo[2] == "1") {
				_visit = util.getRegGroup(e, "{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"(\\d+?)\",\"\\d+?\"");
				_comment = util.getRegGroup(e, "{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"\\d+?\",\"(\\d+?)\"");
			} else {
				_visit = util.getRegGroup(e, "\"data\":{\"viewers\":(\\d*),\"comments\":\\d*");
				_comment = util.getRegGroup(e, "\"data\":{\"viewers\":\\d*,\"comments\":(\\d*)");
			}
			try {
				if (_visit != null)
					visitCount = int.Parse(_visit).ToString("n0");
				if (_comment != null) 
					commentCount = int.Parse(_comment).ToString("n0");
				rm.form.setStatistics(_visit, _comment);
			} catch (Exception ee){
				addDebugBuf(ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
			}
		}
		private void onWscOpen(object sender, EventArgs e) {
			addDebugBuf("ms open a");
			
			if (rm.rfu != rfu) {
				//stopRecording();
//				wsc.Close();
				return;
			}			
			try {
				sendMessage((WebSocket)sender, sender == wsc[0] ? msReq[0] : msStoreReq[0]);
				isFirstCommentAfterOpenWsc = true;
				if (bool.Parse(isGetComment) && commentSW == null && !rfu.isPlayOnlyMode && sender == wsc[0]) {
					if (DateTime.Now < lastOpenCommentSwDt + TimeSpan.FromSeconds(3) 
					    	&& (rec.engineMode != "0" && rec.engineMode != "3")) {
						var __commentFileName = util.getOkCommentFileName(rm.cfg, commentFileName, ri.si.lvid, ri.si.isTimeShift, ri.isRtmp);
						try {
							File.Delete(__commentFileName);
						} catch (Exception ee) {util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);}
					}
					
					var fName = (commentFileName == null) ? ri.recFolderFile[1] : incrementRecFolderFile(commentFileName);
					//var fName = (commentFileName == null) ? recFolderFile[1] : incrementRecFolderFile(commentFileName);
					commentFileName = fName;
					var _commentFileName = util.getOkCommentFileName(rm.cfg, fName, ri.si.lvid, ri.si.isTimeShift, ri.isRtmp);
					var isExists = File.Exists(_commentFileName);
					commentSW = new StreamWriter(_commentFileName, false, System.Text.Encoding.UTF8);
					lastOpenCommentSwDt = DateTime.Now;
					
					if (!isExists) {
						if (isGetCommentXml) {
							commentSW.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
							if (!isGetCommentXmlInfo) 
								commentSW.WriteLine("<packet>");
							else {
								writeXmlStreamInfo(commentSW);
							}
					        commentSW.Flush();
						} else {
							commentSW.WriteLine("[");
						}
					}
					
			       
				}
			} catch (Exception ee) {
				addDebugBuf(ee.Message + " " + ee.StackTrace);
			}
			Task.Run(() => {pongWsc((WebSocket)sender);});
		}
		private void pongWsc(WebSocket _wsc) {
			while (_wsc.State == WebSocket4Net.WebSocketState.Open && !isEndProgram && IsRetry) {
				try {
					//_wsc.Send("");
					sendMessage(_wsc, "");
					Thread.Sleep(60000);
				} catch (Exception e) {
					addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private void onWscClose(object sender, EventArgs e) {
			addDebugBuf("ms onclose");
			
			if (rec != null && rec.engineMode != "0" & rec.engineMode != "3" && sender == wsc[0])
			//    DateTime.Now > lastOpenCommentSwDt + TimeSpan.FromSeconds(3)) return;
				closeWscProcess();
			if (sender == wsc[0]) wsc[0] = null;
			else wsc[1] = null;
			try {
				if (rm.rfu == rfu && IsRetry && !isTimeShiftCommentGetEnd) {
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
			if (ri.si.isTimeShift && isTimeShiftCommentGetEnd) return;
			
			if (commentSW != null) {
				try {
					if (isGetCommentXml)
						commentSW.WriteLine("</packet>");
					else {
						commentSW.BaseStream.Position -= 3;
						commentSW.WriteLine("");
						commentSW.WriteLine("]");
					}
					commentSW.Flush();
				} catch (Exception ee) {
					addDebugBuf(ee.Message + " " + ee.StackTrace + " " + ee.TargetSite);
				}
				
				
				var name = ((FileStream)commentSW.BaseStream).Name; 
				commentSW.Close();
				commentSW = null;
				lastSaveComments.Clear();
				try {
					if (rm.cfg.get("fileNameType") == "10" && (name.IndexOf("{w}") > -1 || name.IndexOf("{c}") > -1)) {
						Task.Run(() => {
				         	setRealTimeStatistics();
				         	var _name = name.Replace("{w}", visitCount.Replace("-", "")).Replace("{c}", commentCount.Replace("-", "")); 
				         	File.Move(name, _name);
				         	name = _name;
						});
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
				if (ri.isRtmp) {
					var baseN = util.getRegGroup(name, "(.+)\\.");
					util.debugWriteLine("closeWscProcess rtmp comment delete " + name + " " + baseN);
					if (baseN != null && !File.Exists(baseN + ".flv")) {
						try {
							if (File.ReadAllText(name).IndexOf("chat") == -1)
								File.Delete(name);
						} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						}
					}
				}
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
			if (lastSaveComments.Count > 0 && isFirstCommentAfterOpenWsc) {
				checkMissingComment();
			}
			isFirstCommentAfterOpenWsc = false;
			
			var eMessage = isConvertSpace ? util.getOkSJisOut(e.Message, rm.cfg.get("commentConvertStr")) : e.Message;
			//if (isNormalizeComment) eMessage = eMessage.Replace("\"premium\":24", "\"premium\":0");
			
			if (ri.si.isTimeShift && eMessage.StartsWith("{\"ping\":{\"content\":\"rf:") && !ri.isChase && sender == wsc[0]) {
				closeWscProcess();
				try {commentSW.Close();}
				catch (Exception eee) {addDebugBuf(eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);}
				isTimeShiftCommentGetEnd = true;
				rm.form.addLogText("コメントの保存を完了しました");
			}
			
			if (rm.rfu != rfu || ((!ri.si.isTimeShift || ri.isChase) && !IsRetry)) {
				try {
					if (wsc[0] != null) wsc[0].Close();
				} catch (Exception ee) {
					addDebugBuf("wsc0 message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
				}
				try {
					if (wsc[1] != null) wsc[1].Close();
				} catch (Exception ee) {
					addDebugBuf("wsc1 message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
				}
				//stopRecording();
				addDebugBuf("tigau rfu comment" + eMessage);
				return;
			}
			
			XDocument xml = null;
			try {
				xml = JsonConvert.DeserializeXNode(eMessage);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite + eMessage);
				rm.form.addLogText("error:" + eMessage);
				return;
			}
			var chatinfo = new ChatInfo(xml);
			
			XDocument chatXml;
			if (ri.si.isTimeShift && !ri.isChase) chatXml = chatinfo.getFormatXml(ri.si.openTime);
			else {
				if (!ri.si.isTimeShift || ri.isRealtimeChase) {
					if (ri.si.type == "official") {
						chatXml = chatinfo.getFormatXml(0, true, serverTime - ri.si._openTime);
					//} else chatXml = chatinfo.getFormatXml(serverTime);
					} else {
						//chatXml = chatinfo.getFormatXml(0, true, serverTime - _openTime);
						chatXml = chatinfo.getFormatXml(serverTime);
					}
				} else {
					while (firstSegmentSecond == -1 && rm.rfu == rfu) {
						Thread.Sleep(1000);
					}
					var vposStartTime = (ri.timeShiftConfig.isVposStartTime) ? (long)firstSegmentSecond : 0;
					if (ri.si.type == "official") {
						chatXml = chatinfo.getFormatXml(0, true, ri.timeShiftConfig.timeSeconds);
					} else {
						chatXml = chatinfo.getFormatXml(ri.si.openTime + vposStartTime);
					}
					
				}
			}
			if (isGetCommentXmlInfo && chatinfo.no == -1) 
				chatXml.Root.Add(new XAttribute("no", "0"));
			var chatXmlStr = chatXml.ToString();
			addDebugBuf("xml " + chatXmlStr);
			
			if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
					chatinfo.premium == "3")) return;
			if (chatinfo.root != "chat" && chatinfo.root != "thread") return;
			
			if (chatinfo.root == "thread" && lastSaveComments.Count == 0) {
				ticket = chatinfo.ticket;
				for (var i = 0; i < 30 && sync == 0 && rm.rfu == rfu; i++) 
					Thread.Sleep(1000);
				if (sync != 0) {
					util.debugWriteLine("sync set " + sync + " servertime " + chatinfo.serverTime);
					serverTime = sync / 1000;
				} else {
					serverTime = chatinfo.serverTime;
					//数秒過去の動画も取得できることを考慮
					serverTime -= ri.isRealtimeChase ? 20 : 3;
				}
				
				try {
					if (isGetCommentXmlInfo && commentSW != null && sender == wsc[0]) {
						if (!ri.si.isTimeShift || ri.isRealtimeChase)
							commentSW.WriteLine("<StartTime>" + serverTime + "</StartTime>");							
						else {
							var startTime = ri.si.openTime;
							var vposStartTime = (ri.timeShiftConfig.isVposStartTime) ? (long)firstSegmentSecond : 0;
							if (ri.si.type == "official") {
								startTime = ri.si._openTime + vposStartTime;
							} else {
								startTime = ri.si.openTime + vposStartTime;
							}
							commentSW.WriteLine("<StartTime>" + startTime + "</StartTime>");
						}
					}
				} catch (Exception ee) {
					addDebugBuf(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			
			addDebugBuf("wsc message " + ws);
			
			try {
				var isComSave = chatinfo.root == "thread" || ri.timeShiftConfig == null || ((!ri.timeShiftConfig.isAfterStartTimeComment ||
						chatinfo.date > ri.si._openTime + ri.timeShiftConfig.timeSeconds - 10) && 
					(!ri.timeShiftConfig.isBeforeEndTimeComment || 
						ri.timeShiftConfig.endTimeSeconds == 0 || 
						chatinfo.date < ri.si._openTime + ri.timeShiftConfig.endTimeSeconds + 10));
				if (!isComSave) 
					return;
			
				lock(commentLock) {
					if (commentSW != null) {
						var writeStr = (isGetCommentXml) ? 
							chatXmlStr : 
							(Regex.Replace(eMessage, 
				            	"\"vpos\"\\:(\\d+)", 
				            	"\"vpos\":" + chatinfo.vpos + ""));
						writeStr = util.getReplacedComment(writeStr, commentReplaceList);
						//writeStr = util.getOkSJisOut(writeStr, " ");
						
						if (ri.isChase && !ri.isRealtimeChase && chaseCommentBuf != null) {
							if (chaseCommentBuf.IndexOf(writeStr) == -1) {
								chaseCommentBuf.Add(writeStr);
								
								util.debugWriteLine("chase comment add " + writeStr);
								
								lastSaveComments.Add(writeStr);
								if (lastSaveComments.Count > 1110)
									lastSaveComments.RemoveRange(0, 100);
							}
						} else {
							if (lastSaveComments.IndexOf(writeStr) == -1 &&
							    	!(lastSaveComments.Count > 0 && chatinfo.root == "thread")) {
								if (!isSaveCommentOnlyRetryingRec || (rec == null || DateTime.Now - rec.lastWroteSegmentDt < TimeSpan.FromSeconds(10))) {
									commentSW.WriteLine(writeStr + (isGetCommentXml ? "" : ","));
									commentSW.Flush();
								}
							}
							lastSaveComments.Add(writeStr);
							if (lastSaveComments.Count > 1110)
								lastSaveComments.RemoveRange(0, 100);
						}
						addDebugBuf("write comment " + writeStr);
			             
					}
				}
           
			} catch (Exception ee) {addDebugBuf("comment write exception " + ee.Message + " " + ee.StackTrace);}
			
			
			chatinfo.contents = util.getReplacedComment(chatinfo.contents, commentReplaceList);
			if (chatinfo.vpos != 0)
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
			var __time = chat.date - ri.si.openTime; //- (60 * 60 * 9);

//			var __timeDt = util.getUnixToDatetime(__time);
//			var openTimeDt = util.getUnixToDatetime(openTime);
//			var __timeDt0 = __timeDt - openTimeDt;
//			var __timeDt1 = __timeDt0. - new timespunixKijunDt;
			var isMinus = __time < 0;
			if (isMinus) __time *= -1;
			var h = (int)(__time / (60 * 60));
			var m = (int)((__time % (60 * 60)) / 60);
			var _m = (m < 10) ? ("0" + m.ToString()) : m.ToString();
			var s = __time % 60;
			var _s = (s < 10) ? ("0" + s.ToString()) : s.ToString();
			var keikaTime = h + ":" + _m + ":" + _s + "";
			if (isMinus) keikaTime = "-" + keikaTime;
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
			
			//if (!isTimeShift || isRealtimeChase)
			if (!ri.si.isTimeShift || ri.isChase)
				rm.form.addComment(keikaTime, chat.contents, chat.userId, chat.score, c);
			
		}
		private bool isFirstChoiceQuality(string currentQuality, string bestGettableQuolity) {
//			var bestGettableQuality = getBestGettableQuolity(message);
			
			
			
			return currentQuality == bestGettableQuolity; 
			
		}
		private string getBestGettableQuolity(string msg) {
			//var qualityList = new List<string>{//"abr",
			//	"super_high", "high",
			//	"normal", "low", "super_low", "audio_high", "6Mbps1080p30fps"};
			var qualityList = config.config.qualityList.Values
					.Select(x => x.IndexOf("(") > -1 ? 
				    	util.getRegGroup(x, "\\((.+?)\\)") : x).ToList();
			
			var gettableList = ri.webSocketRecInfo[2] == "1" ? 
					util.getRegGroup(msg, "\"qualityTypes\"\\:\\[(.+?)\\]").Replace("\"", "").Split(',')
					: util.getRegGroup(msg, "\"availableQualities\"\\:\\[(.+?)\\]").Replace("\"", "").Split(',');
			var ranks = (rm.ri == null) ? (qualityRank) :
					rm.ri.qualityRank;
			//if (ranks.Length == 6) qualityList.Insert(0, "abr");
			
			var bestGettableQuality = "normal";
			foreach(var r in ranks) {
				try {
					var i = int.Parse(r);
					if (i >= qualityList.Count) continue;
					var q = qualityList[i];
					if (gettableList.Contains(q) && q != "abr") {
						bestGettableQuality = q;
						break;
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			var configQ = config.config.qualityList.Values.ToArray();
			var newQualities = gettableList.Where(x => 
					Array.Find(configQ, y => y.IndexOf(x) > -1) == null);
			foreach (var q in newQualities) {
				if (q == "abr") continue;
				config.config.qualityList.Add(config.config.qualityList.Count, q);
				rm.cfg.set("qualityList", JsonConvert.SerializeObject(namaichi.config.config.qualityList));
				rm.form.addLogText("未知の画質の" + q + "が見つかりました。「オプション」の「画質」タブのリストに追加します。");
			}
			
			return bestGettableQuality;
		}
		private void sendUseableStreamGetCommand(string bestGettableQuolity) {
			var req = "";
			
			if (ri.webSocketRecInfo[2] == "1") {
				req = (ri.isRtmp) ?
						("{\"type\":\"watch\",\"body\":{\"command\":\"getstream\",\"requirement\":{\"protocol\":\"rtmp\",\"quality\":\"" + bestGettableQuolity + "\"}}}")
						: ("{\"type\":\"watch\",\"body\":{\"command\":\"getstream\",\"requirement\":{\"protocol\":\"hls\",\"quality\":\"" + bestGettableQuolity + "\",\"isLowLatency\":false}}}");
			} else {
				var _latency = rm.cfg.get("latency");
				var latency = (_latency == "新方式の低遅延" || _latency == "1") ? "low" : "high";
				req = "{\"type\":\"changeStream\",\"data\":{\"quality\":\"" + bestGettableQuolity + "\",\"protocol\":\"hls" + (ri.isFmp4 ? "+fmp4" : "") + "\",\"latency\":\"" + latency + "\",\"chasePlay\":" + ri.isChase.ToString().ToLower() + "}}";
			}
			
			sendMessage(ws, req);
		}
		override public void reConnect() {
			displayDebug("reconnect");
			addDebugBuf("reconnect wr");
//			onOpen(null, null);
			try {
				ws.Close();
			} catch (Exception e) {
				addDebugBuf("reconnect ws exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
//			ws.Open();
		}
		override public void reConnect(WebSocket _ws) {
			displayDebug("ws reconnect");
			addDebugBuf("reconnect " + _ws + " " + _ws.GetHashCode() + " ws " + ws.GetHashCode());
			try {
				ws.Close();
			} catch (Exception e) {
				addDebugBuf("reconnect ws exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		public bool isEndedProgram() {
			var type = -1;
			try {
				var isPass = (DateTime.Now - lastEndProgramCheckTime < TimeSpan.FromSeconds(5));
				addDebugBuf("ispass " + isPass + " lastendprogramchecktime " + lastEndProgramCheckTime);
				if (isPass) return false;
				lastEndProgramCheckTime = DateTime.Now;
				
				var a = new System.Net.WebHeaderCollection();
				var res = util.getPageSource(ri.si.url, container, null, false, 15000);
				addDebugBuf("irasendedprogm url " + ri.si.url + " res==null " + (res == null));
				if (res == null) {
					res = util.getPageSource(ri.si.url, container, null, false, 15000, null, true);
					if (res != null && res.IndexOf("<title>ご指定のページが見つかりませんでした") > -1)
						return true;
					return isEndedProgramRtmp();
				}
				type = util.getPageType(res);
				addDebugBuf("isEndProgram type " + type);
				if (type == 0) return false;
				else if (type == 7 || type == 2 || type == 3 || type == 9) return true;
				
				if (res.IndexOf("user.login_status = 'not_login'") > -1) {
					addDebugBuf("isendprogram not login");
					var cg = new CookieGetter(rm.cfg);
					var cgTask = cg.getHtml5RecordCookie(ri.si.url);
					cgTask.Wait();
					container = cgTask.Result[0];
					res = util.getPageSource(ri.si.url, container, null, false, 5000);
					if (res == null) return isEndedProgramRtmp();
					res = System.Web.HttpUtility.HtmlDecode(res);
					var _webSocketInfo = RecordInfo.getWebSocketInfo(res, ri.isRtmp, ri.isChase, ri.si.isTimeShift, rm.form, ri.isFmp4);
					isNoPermission = true;
					addDebugBuf("isendprogram login websocketInfo " + ri.webSocketRecInfo[0] + " " + ri.webSocketRecInfo[1]);
					if (_webSocketInfo == null || _webSocketInfo[0] == null || _webSocketInfo[1] == null) {
						addDebugBuf(res);
					} else ri.webSocketRecInfo = _webSocketInfo;
				}
				if (res == null) return false;
				type = util.getPageType(res);
				addDebugBuf("is ended program  pagetype " + type);
				var isEnd = (type == 7 || type == 2 || type == 3 || type == 9);
				return isEnd;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return isEndedProgramRtmp();
				//return false;
			} finally {
				addDebugBuf("is ended program  pagetype " + type);
			}
		}
		public bool isEndedProgramRtmp() {
			util.debugWriteLine("isEndedProgramRtmp");
			return false;
			/*
			try {
				var url = "http://live.nicovideo.jp/api/getplayerstatus?v=" + lvid;
				var r = util.getPageSource(url, container);
				var isTs = false;
				var type = util.getPageTypeRtmp(r, ref isTs, false);
				var isEnd = (type == 7 || type == 2 || type == 3 || type == 9);
				return isEnd;
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				return false;
			}
			*/
		}
		override public void sendComment(string s, bool is184) {
			if (msThread == null) return;
			sendCommentBuf = s;
			isSend184 = is184;
			var vpos = (int)(((TimeSpan)(DateTime.Now - util.getUnixToDatetime(ri.si.openTime))).TotalMilliseconds / 10) + 60000 + 780;
			//ws.Send("{\"type\":\"watch\",\"body\":{\"command\":\"getpostkey\",\"params\":[\"" + msThread + "\"]}}");
			if (ri.webSocketRecInfo[2] == "1")
				sendMessage(ws, "{\"type\":\"watch\",\"body\":{\"command\":\"getpostkey\",\"params\":[\"" + msThread + "\"]}}");
			//else sendMessage(ws, "{\"type\":\"getPostkey\"}");
			else sendMessage(ws, "{\"type\":\"postComment\",\"data\":{\"text\":\"" + s + "\",\"vpos\":" + vpos + ",\"isAnonymous\":" + is184.ToString().ToLower() + ",\"color\":\"white\",\"size\":\"medium\",\"position\":\"naka\"}}");
		}
		public void sendCommentWsc(string s) {
			var vpos = (int)(((TimeSpan)(DateTime.Now - util.getUnixToDatetime(ri.si.openTime))).TotalMilliseconds / 10) + 60000 + 780;
			var mail = (isSend184) ? ",\"mail\":\"184 \"" : "";
			var premium = (ri.isPremium) ? "1" : "0";
			string postKey = null;
			if (ri.webSocketRecInfo[2] == "1") {
				postKey = util.getRegGroup(s, "params\"\\:\\[\"(.+?)\"");
				//vpos = 0;
				 
				//addDebugBuf("send comment " + command);
				//wsc.Send("[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"chat\":{\"thread\":\"" + msThread + "\",\"vpos\":" + vpos + mail + ",\"ticket\":\"" + ticket + "\",\"user_id\":\"" + userId + "\",\"premium\":1,\"content\":\"うむ\",\"postkey\":\".1535198509.6HZajH6n5HWGDXnbz2fI1-r5LLg\"}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]");
				//wsc.Send(command);
				
			} else {
				//{"type":"postkey","data":{"value":".1591116733.C73QqliT3h8zMzlQtRXDv9lsoZg","expireAt":"2070-11-03T18:43:56+09:00"}}
				//[{"ping":{"content":"rs:9"}},{"ping":{"content":"ps:45"}},{"chat":{"thread":"1670321107","vpos":255751,"mail":"184 ","ticket":"0x31e8d00","user_id":"225832","premium":1,"content":"ï¼ï¼ï¼ï¼ï¼ï¼ï¼","postkey":".1591116733.C73QqliT3h8zMzlQtRXDv9lsoZg"}},{"ping":{"content":"pf:45"}},{"ping":{"content":"rf:9"}}]
				postKey = util.getRegGroup(s, "\"value\":\"(.+?)\"");
			}
			var command = "[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"chat\":{\"thread\":\"" + msThread + "\",\"vpos\":" + vpos + mail + ",\"ticket\":\"" + ticket + "\",\"user_id\":\"" + ri.userId + "\",\"content\":\"" + sendCommentBuf + "\",\"postkey\":\"" + postKey + "\", \"premium\":" + premium + "}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]";
			sendMessage(wsc[0], command);
			sendCommentBuf = null;
		}
		//override public string[] getRecFilePath(long openTime) {
		/*
		public string[] getRecFilePath(long openTime) {
			return getRecFilePath();
		}
		 */
		override public string[] getRecFilePath() {
			return ri.getRecFilePath(ri.isRtmp, ri.timeShiftConfig, ri.isFmp4, rm.cfg, rm.form);
		}
		private void startDebugWriter() {
			#if !DEBUG
//				return;
			#endif
			while ((rm.rfu == rfu && !isLogEnd) || debugWriteBuf.Count > 0) {
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
							util.debugWriteLine(b);
							debugWriteBuf.Remove(b);
						}
					}
					Thread.Sleep(500);
				} catch (Exception e) {
					addDebugBuf("debug writer exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private void addDebugBuf(string s) {
			#if !DEBUG
//				return;
			#endif
			var dt = DateTime.Now.ToLongTimeString();
			debugWriteBuf.Add(dt + " " + s);
		}
		private void resetWebsocketInfo() {
			try {
				var cg = new CookieGetter(rm.cfg);
				var cgTask = cg.getHtml5RecordCookie(ri.si.url);
				cgTask.Wait();
				container = cgTask.Result[0];
				var res = util.getPageSource(ri.si.url, container, null, false, 5000);
				if (res == null) {
					util.debugWriteLine("resetWebsocketInfo get page null");
					return;
				}
				res = System.Web.HttpUtility.HtmlDecode(res);
				
				var _webSocketInfo = RecordInfo.getWebSocketInfo(res, ri.isRtmp, ri.isChase, ri.si.isTimeShift, rm.form, ri.isFmp4);
				if (_webSocketInfo == null) {
					addDebugBuf("resetWebsocketInfo _websocketInfo null");
					return;
				}
				isNoPermission = true;
				addDebugBuf("resetWebsocketInfo " + _webSocketInfo[0] + " " + _webSocketInfo[1]);
				if (_webSocketInfo[0] == null || _webSocketInfo[1] == null) {
					addDebugBuf(res);
				} else ri.webSocketRecInfo = _webSocketInfo;
			} catch (Exception e) {
				addDebugBuf("resetWebsocketInfo exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		override public void resetCommentFile() {
			addDebugBuf("resetCommentFile");
			
			
			if (xcg != null) {
				xcg.resetCommentFile();
			} else {
				try {
					if (commentSW != null) {
						if (isGetCommentXml)
							commentSW.WriteLine("</packet>");
						else {
							commentSW.BaseStream.Position -= 3;
							commentSW.WriteLine("");
							commentSW.WriteLine("]");
						}
						commentSW.Close();
						commentSW = null;
						lastSaveComments.Clear();
					}
					
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
				
				try {
					
					if (bool.Parse(rm.cfg.get("IsgetComment")) && commentSW == null && !rfu.isPlayOnlyMode) {
						if (DateTime.Now < lastOpenCommentSwDt + TimeSpan.FromSeconds(3) 
						    	&& (rec.engineMode != "0" && rec.engineMode != "3")) {
							var __commentFileName = util.getOkCommentFileName(rm.cfg, commentFileName, ri.si.lvid, ri.si.isTimeShift, ri.isRtmp);
							try {
								File.Delete(__commentFileName);
							} catch (Exception e) {util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);}
						}
			
						var fName = (commentFileName == null) ? ri.recFolderFile[1] : incrementRecFolderFile(commentFileName);
						commentFileName = fName;
						var _commentFileName = util.getOkCommentFileName(rm.cfg, fName, ri.si.lvid, ri.si.isTimeShift, ri.isRtmp);
						var isExists = File.Exists(_commentFileName);
						commentSW = new StreamWriter(_commentFileName, false, System.Text.Encoding.UTF8);
						lastOpenCommentSwDt = DateTime.Now;
						
						if (!isExists) {
							if (isGetCommentXml) {
								commentSW.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
						        commentSW.WriteLine("<packet>");
		//				        if (commentHead != null)
		//				        	commentSW.WriteLine(commentHead);
						        commentSW.Flush();
							}
						} else {
							commentSW.WriteLine("[");
						}
					}
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + " " + ee.StackTrace);
				}
				try {
					sendMessage(wsc[0], msReq[0]);
				} catch (Exception ee) {
					util.debugWriteLine("on open wsc0 req send exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
				try {
					if (wsc[1] != null)
						sendMessage(wsc[1], msStoreReq[0]);
				} catch (Exception ee) {
					util.debugWriteLine("on open wsc1 req send exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
		}
		private string incrementRecFolderFile(string recFolderFile) {
			var r = util.incrementRecFolderFile(recFolderFile);
			if (r == null) return getRecFilePath()[1];
			return r;
		}
		private void connectUntilOk() {
			while (rm.rfu == rfu && IsRetry) {
				try {
					if (!connect()) {
						Thread.Sleep(3000);
						
						#if DEBUG
							rm.form.addLogText("再接続試行");
						#endif
							
						continue;
					}
					break;
				} catch (Exception ee) {
					addDebugBuf(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
		}
		private void rtmpRecord(string websocketMsg, string quality) {
			try {
				string url = null;
				if (websocketMsg != null) {
					var _msg = util.getRegGroup(websocketMsg,  "\"currentStream\":{(.+?)}");
					if (_msg == null && false) {
						util.debugWriteLine("rtmpRecord _msg null");
						return;
					}
					url = util.getRegGroup(websocketMsg, "\"uri\":\"(.+?)\"") + "/" + util.getRegGroup(websocketMsg, "\"name\":\"(.+?)\"");
					util.debugWriteLine("rtmp url " + url);
				}
				
				rfu.subGotNumTaskInfo = new List<numTaskInfo>();
				if (rr != null) rr.resetRtmpUrl(url);
				else {
					rr = new RtmpRecorder(ri.si.lvid, container, rm, rfu, !ri.isRtmp, ri.recFolderFile, this, ri.si.openTime);
					Task.Run(() => {
						rr.record(url, quality);
						rm.hlsUrl = "end";
						if (rr.isEndProgram) {
							isEndProgram = true;
							if (endTime == DateTime.MinValue)
								endTime = DateTime.Now;
						}
						IsRetry = false;
					});
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			
			/*
			Task.Run(() => {
			    xcg = new XmlCommentGetter(lvid, container, rm, rfu, recFolderFile[1], this, isTimeShift, isRtmp, openTime, _openTime, serverTime);
			    xcg.get();
			});
			while (rm.rfu == rfu && isRetry) Thread.Sleep(1000);
			*/
			/*
			if (websocketMsg == null) {
				if (isTimeShift) {
					
					
				} else {
					/*
					xcg = new XmlCommentGetter_ontime( 
							userId, rm, rfu, rm.form, openTime, 
							recFolderFile, lvid, container, 
							programType, _openTime, this);
					xcg.save();
					*
				}
			}
			*/
		}
		private void saveXmlComment() {
			tscgx = new TimeShiftCommentGetter_xml ( 
					ri.userId, rm, rfu, rm.form, ri.si.openTime, 
					ri.recFolderFile, ri.si.lvid, container, 
					ri.si.type, ri.si._openTime, this, 
					(ri.isRtmp) ? 0 : ri.timeShiftConfig.timeSeconds, 
					(ri.isRtmp) ? false : ri.timeShiftConfig.isVposStartTime, ri.isRtmp);
			tscgx.save();
		}
		private bool isChaseStream(string msg) {
			var url = util.getRegGroup(msg, "(http.+?)\"");
			if (url == null) return false;
			
			var res = util.getPageSource(url, container);
			if (res == null) {
				return false;
			}
			string segUrl = null;
			foreach (var s in res.Split('\n')) {
				if (s.StartsWith("#") || s.IndexOf(".m3u8") < 0) continue;
				segUrl = s;
				break;
			}
			
			var masterBaseUrl = util.getRegGroup(url, "(.+/)");
			var baseUrl = util.getRegGroup(masterBaseUrl + segUrl, "(.+/)");
			addDebugBuf("master m3u8 res " + res);
			addDebugBuf("seg m3u8 " + 
					masterBaseUrl + segUrl);
			var _segUrl = masterBaseUrl + segUrl;
			
			res = util.getPageSource(_segUrl, container);
			var ret = res != null && res.IndexOf("#DMC-CURRENT-POSITION") != -1;
			addDebugBuf("isChaseStream " + ret);
			if (res != null) {
				var _duration = util.getRegGroup(res, "#STREAM-DURATION:(.+)");
				if (_duration != null && ri.timeShiftConfig.timeSeconds < 0) {
					var duration = (int)(double.Parse(_duration, System.Globalization.NumberStyles.Float));
					ri.timeShiftConfig.timeSeconds = duration + ri.timeShiftConfig.timeSeconds;
					ri.timeShiftConfig.endTimeSeconds = duration + ri.timeShiftConfig.endTimeSeconds;
				}
			}
				
			return ret;
		}
		override public void chaseCommentSum() {
			util.debugWriteLine("chaseCommentSum " + gotTsCommentList.Count() + " " + chaseCommentBuf.Count);
			var gotComList = new List<string>();
			gotComList.AddRange(gotTsCommentList);
			while (true) {
				try {
					foreach (var c in chaseCommentBuf) {
						if (gotComList.IndexOf(c) > -1) {
							gotComList.Remove(c);
							util.debugWriteLine("remove gotcomlist " + c);
						}
					}
					break;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			
			util.debugWriteLine("chasecomment sum gotts " + (gotTsCommentList.Length == 0 ? "no" : gotTsCommentList[0]));
			util.debugWriteLine("chasecomment sum chasecombuf " + (chaseCommentBuf.Count == 0 ? "no" : chaseCommentBuf[0]));
			
			string threadLine = null; 
			if (chaseCommentBuf.Count > 0) {
				threadLine = chaseCommentBuf[0];
				chaseCommentBuf.RemoveAt(0);
			}
			
			
			try {
				if (threadLine != null) commentSW.WriteLine(threadLine + (isGetCommentXml ? "" : ","));
				for (var i = 0; i < gotComList.Count; i++) {
					commentSW.WriteLine(gotComList[i] + (isGetCommentXml ? "" : ","));
				}
				foreach (var c in chaseCommentBuf)
					commentSW.WriteLine(c + (isGetCommentXml ? "" : ","));
				commentSW.Flush();
				chaseCommentBuf = null;
			} catch (Exception e) {
				addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void writeXmlStreamInfo(StreamWriter w) {
			w.WriteLine("<packet xmlns=\"http://posite-c.jp/niconamacommentviewer/commentlog/\">");
			w.WriteLine("<RoomLabel>" + roomName + "</RoomLabel>");
		}
		/*
		public void setRealTimeStatistics() {
			try {
				if (!visitCount.StartsWith("-")) {
			    	Thread.Sleep(10000);
			    	string visit, comment;
			    	var ret = getStatistics(rfu.lvid, container, out visit, out comment);
			    	if (ret) {
			    		if (!visitCount.StartsWith("-")) {
				    		visitCount = "-" + visit;
				    		commentCount = "-" + comment;
			    		}
			    	}
			    }
			} catch (Exception e) {
				addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		*/
		override internal bool getStatistics(string lvid, CookieContainer cc, out string visit, out string comment) {
			visit = "0";
			comment = "0";
			var url = "https://live.nicovideo.jp/watch/" + lvid;
			var res = util.getPageSource(url, cc);
			if (res == null) return false;
			var _visit = util.getRegGroup(res, "statistics&quot;:{&quot;watchCount&quot;:(\\d+),&quot;commentCount&quot;:\\d+}");
			var _comment = util.getRegGroup(res, "statistics&quot;:{&quot;watchCount&quot;:\\d+,&quot;commentCount&quot;:(\\d+)}");
			if (_visit == null || _comment == null) return false;
			visit = _visit;
			comment = _comment;
			return true;
		}
		public void displayDebug(string s) {
			#if DEBUG
				//rm.form.addLogText(DateTime.Now + " " + s);
			#endif
		}
		private void sendMessage(WebSocket w, string s) {
			util.debugWriteLine("ws send " + s);
			if (w != null)
				w.Send(s);
			else util.debugWriteLine("sendMessage w null");
		}
		override public void setSync(int no, double second, string m3u8Url) {
			try {
				if (ri.si.isTimeShift) {
					sync = no + ri.si._openTime * 1000;
					util.debugWriteLine("setSync ts " + no + " " + ri.si._openTime + " " + ri.si.openTime + " " + ri.si.vposBaseTime + " " + second);
					
					var _url = m3u8Url.Replace("1/ts/playlist.m3u8", "stream_sync.json");
					var _m = new Regex("(.+\\?).*?(ht2.+)").Match(_url);
					var _res = util.getPageSource(_m.Groups[1].Value + _m.Groups[2].Value);
					util.debugWriteLine("setSync ts res " + _res);
					
					return;
				}
				
				util.debugWriteLine("setSync " + no + " " + second + " " + m3u8Url + " " + ri.si._openTime + " " + ri.si.openTime + " " + ri.si.vposBaseTime);
				//var url = util.getRegGroup(mes, "\"syncUri\":\"(.+?)\"");
				var url = m3u8Url.Replace((ri.isFmp4 ? "mp4" : "ts") + "/playlist.m3u8", "stream_sync.json");
				var res = util.getPageSource(url);
				util.debugWriteLine("setSync res " + res);
				if (res == null) return;
				var m = new Regex("\"beginning_timestamp\":(\\d+),\"sequence\":(\\d+)").Match(res);
				if (!m.Success) return;
				
				var a = (ri.si._openTime - ri.si.vposBaseTime);
				var beginTime = long.Parse(m.Groups[1].Value);
				if (second != 0)
					sync = (long)(beginTime - (long.Parse(m.Groups[2].Value) - no) * second * 1000);
				else sync = beginTime;
				util.debugWriteLine("sync " + sync);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void checkMissingComment() {
			util.debugWriteLine("checkmissingcomment");
			try {
				if (tscg != null) {
					return;
				}
				if (lastSaveComments.Count == 0) return;
				
				/*
				var _lastCommentPos = util.getRegGroup(lastSaveComments[lastSaveComments.Count - 1], 
						isGetCommentXml ? "date=\"(\\d+)\"" : "\"date\":(\\d+)");
				if (_lastCommentPos != null) {
					util.debugWriteLine("not found lastComment dt" + lastSaveComments[lastSaveComments.Count - 1]);
					return;
				}
				var lastCommentPos = int.Parse(_lastCommentPos) - _openTime;
				var _tscfg = new TimeShiftConfig(0, 0, 0, lastCommentPos, 
						0, 0, 0, false, lastCommentPos, 0, 0, false, null, false, 0, false, tsConfig.isVposStartTime,  false, false
				*/
				var tsConfig = new TimeShiftConfig();
				tscg = new TimeShiftCommentGetter(msUri, msThread, msStoreUri, msStoreThread,                                  
						ri.userId, rm, rfu, rm.form, ri, 
						ri.si.lvid, container,
						//ri.si.type, 
						this,
						//(ri.isRtmp) ? 0 : tsConfig.timeSeconds, 
						//(ri.isRtmp) ? false : tsConfig.isVposStartTime, 
						ri.isRtmp, rr, roomName, tsConfig, 
						lastSaveComments[lastSaveComments.Count - 1], false,
					false, null, null);
				tscg.save();
				var t = DateTime.Now;
				while (!tscg.isEnd) {
					Thread.Sleep(1000);
					if (DateTime.Now - t > TimeSpan.FromSeconds(30) && 
					    	tscg.gotCommentList.Count == 0 &&
					    	tscg.gotCommentListBuf.Count == 0) return;
					if (DateTime.Now - t > TimeSpan.FromMinutes(5)) break;
					//if (rm.rfu != rfu || !isRetry) return;
				}
				var ind = Array.IndexOf(gotTsCommentList, lastSaveComments[lastSaveComments.Count - 1]);
				util.debugWriteLine("last comment ind " + ind + " " + gotTsCommentList.Length + " " + lastSaveComments.Count);
				if (ind == -1) return;
				for (var i = ind + 1; i < gotTsCommentList.Length; i++) {
					commentSW.WriteLine(gotTsCommentList[i]);
					commentSW.Flush();
					lastSaveComments.Add(gotTsCommentList[i]);
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			} finally {tscg = null;}
		}
	}
}