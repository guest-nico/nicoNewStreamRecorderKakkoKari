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
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of WebSocketRecorder.
	/// </summary>
	public class WebSocketRecorder
	{
		private string[] webSocketInfo;
		private string broadcastId;
		private string userId;
		private string lvid;
		private CookieContainer container;
		private string[] recFolderFile;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private Html5Recorder h5r;
		private WebSocket ws;
		private WebSocket wsc;
		private Record rec;
		private StreamWriter commentSW;
		private string msReq;
		private long serverTime;
		private bool isRetry = true;
		
		private bool isNoPermission = false;
		private long openTime;
		public bool isEndProgram = false;
		public int lastSegmentNo = 0;
		private bool isTimeShift = false;
		private TimeShiftConfig tsConfig = null;
		
		private WebSocket[] himodukeWS = new WebSocket[2];
			
		private System.Threading.Thread mainThread;
		public WebSocketRecorder(string[] webSocketInfo, 
				CookieContainer container, string[] recFolderFile, 
				RecordingManager rm, RecordFromUrl rfu, 
				Html5Recorder h5r, long openTime, 
				int lastSegmentNo, bool isTimeShift, string lvid, 
				TimeShiftConfig tsConfig)
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
		}
		public bool start() {
			util.debugWriteLine("rm.rfu dds0 " + rm.rfu);
			
//			connect(webSocketInfo[0]);
			connect();
			
			util.debugWriteLine("rm.rfu dds1 " + rm.rfu);
			
			broadcastId = util.getRegGroup(webSocketInfo[0], "watch/(.+?)\\?");
			userId = util.getRegGroup(webSocketInfo[0], "audience_token=.+?_(.+?)_");
			
			util.debugWriteLine("rm.rfu dds6 " + rm.rfu);
			
			util.debugWriteLine("ws main " + ws + " a " + (ws == null));
			
			
			
			
//			while (ws.State != WebSocket4Net.WebSocketState.Closed) {
//			while (rm.rfu == rfu && ws != null && isRetry && 
//			       (rec == null || !rec.isStopRead())) {
			while (rm.rfu == rfu && isRetry) {
				System.Threading.Thread.Sleep(1000);
//				if (rec != null) 
//					util.debugWriteLine("isStopread " + rec.isStopRead());
				
				//test
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			isRetry = false;
			
			if (rm.rfu != rfu) {
				stopRecording(ws, wsc);
//				ws.Close();
//				wsc.Close();
				if (rec != null) 
					rec.waitForEnd();
			}
			if (!isRetry) {
				stopRecording(ws, wsc);
				if (rec != null)
					rec.waitForEnd();
			}
			
			util.debugWriteLine("rm.rfu dds3 " + rm.rfu);
			util.debugWriteLine("closed saikai");
			
			return isNoPermission;
		}
		private void connect() {
			ws = new WebSocket(webSocketInfo[0]);
			ws.Opened += onOpen;
			ws.Closed += onClose;
			ws.DataReceived += onDataReceive;
			ws.MessageReceived += onMessageReceive;
			ws.Error += onError;
			
			ws.Open();
			
		}
		private void onOpen(object sender, EventArgs e) {
			util.debugWriteLine("on open rm.rfu dds2 " + rm.rfu);
			
			if (sender != ws) {
				((WebSocket)sender).Close();
				util.debugWriteLine("hukusuu ws close");
			}
			
			if (isNoPermission) webSocketInfo[1] = webSocketInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true");
							
			String leoReq = "{\"type\":\"watch\",\"body\":{\"command\":\"playerversion\",\"params\":[\"leo\"]}}";
			util.debugWriteLine("leoReq " + leoReq);
			util.debugWriteLine("websocketinfo1 " + webSocketInfo[1]);
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
			util.debugWriteLine("open send  " + ws);
			util.debugWriteLine("rm " + rm + " rm.rfu " + rm.rfu + " rfu " + rfu);
			
//			if (rm.rfu != rfu) stopRecording();
		}
		private void onClose(object sender, EventArgs e) {
			util.debugWriteLine("on close " + e.ToString());
			//stopRecording();
			if (rm.rfu == rfu && !isEndProgram && (WebSocket)sender == ws) {
				connect();
//				ws.Open();
				if (!isTimeShift && isEndedProgram()) {
					isRetry = false;
					isEndProgram = true;
				}
			}
			

		}
		private void onError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			util.debugWriteLine("on error " + e.Exception.Message);
			//stopRecording();
//			reConnect();
//			ws.Open();
//			endStreamProcess();
		}
		private void onDataReceive(object sender, DataReceivedEventArgs e) {
			util.debugWriteLine("on data " + e.Data);
		}
		private void onMessageReceive(object sender, MessageReceivedEventArgs e) {
			util.debugWriteLine("receive " + e.Message);
			
			if (sender != ws) {
				((WebSocket)sender).Close();
				util.debugWriteLine("hukusuu ws close");
			}
			
//			util.debugWriteLine("ws " + ws);
			if (ws == null) return;
			//pong
			if (e.Message.IndexOf("\"ping\"") >= 0) {
				sendPong();
			}
			
			//get message
			if (e.Message.IndexOf("\"messageServerUri\"") >= 0) {
				if (rfu == rm.rfu && wsc == null && isRetry)
					connectMessageServer(e.Message, (WebSocket)sender);
			}
			
			//record
			if (e.Message.IndexOf("\"currentStream\"") >= 0) {
			
			
				util.debugWriteLine("mediaservertype = " + util.getRegGroup(e.Message, "(\"mediaServerType\".\".+?\")"));
				var bestGettableQuolity = getBestGettableQuolity(e.Message);
				if (isFirstChoiceQuality(e.Message, bestGettableQuolity)) {
					record(e.Message);
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
			    || e.Message.IndexOf("\"TOO_MANY_CONNECTIONS\"") >= 0) {
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0) rm.form.addLogText("追い出されました。");
				
				//SERVICE_TEMPORARILY_UNAVAILABLE 予約枠開始後に何らかの問題？
				if (e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") > 0) 
					rm.form.addLogText("サーバーからデータの受信ができませんでした。リトライします。");
			
				if (e.Message.IndexOf("\"END_PROGRAM\"") > 0) {
					isEndProgram = true;
					isRetry = false;
				}
				
				util.debugWriteLine("kiteru");
//				connect(webSocketInfo[0].Replace("\"requireNewStream\":false", "\"requireNewStream\":true"));
				isNoPermission = true;
				//stopRecording();
//				reConnect();
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
			} else if (e.Message.IndexOf("\"disconnect\"") >= 0) {
				util.debugWriteLine("unknown disconnect");
				isNoPermission = true;
				//stopRecording();
//				reConnect();
			}
			if (e.Message.IndexOf("\"command\":\"statistics\",\"params\"") >= 0
			   	) {
				displayStatistics(e.Message);
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
			}

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
				util.debugWriteLine("send {\"body\":{},\"type\":\"pong\"} and watching" + dt);
				util.debugWriteLine("send {\"type\":\"watch\",\"body\":{\"command\":\"watching\",\"params\":[\"" + broadcastId + "\",\"-1\",\"0\"]}}" + dt);
			} catch (Exception e) {
				util.debugWriteLine(e.Message+e.StackTrace);
			}
		}
		private void record(String message) {
			string hlsUrl = util.getRegGroup(message, "\"uri\":\"(.+?)\"");;
			util.debugWriteLine("rec " + string.Join(" ", recFolderFile));
			rm.hlsUrl = hlsUrl;
			util.debugWriteLine(hlsUrl);
				
			
			Task.Run(() => {
			   	if (rec == null) {
					rec = new Record(rm, true, rfu, hlsUrl, recFolderFile[1], lastSegmentNo, container, isTimeShift, this, lvid, tsConfig);
					rec.record();
	         	} else {
			    	rec.reSetHlsUrl(hlsUrl);
	         	}
				 
				util.debugWriteLine("stop recd");
//				isRetry = false;
//				stopRecording();
			});
		}
		private void connectMessageServer(string message, WebSocket _ws) {
	    	util.debugWriteLine("connect message server");
	    	util.debugWriteLine("isretry " + isRetry + " isend " + isEndProgram);
			string msUri = util.getRegGroup(message, "(ws://.+?)\"");
			string msThread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
	//		String msReq = "[truncated][{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-1000,\"with_global\":1,\"scores\":1,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]\0";
	
			var res_from = (isTimeShift) ? "1" : "-10";
			msReq = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + res_from + ",\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
//			msReq = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":1,\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			
	//		String msReq = "{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-100,\"with_global\":1,\"scores\":1,\"nicoru\":0}}";
	//		String msReq = "{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"res_from\":-1000}}";
	//		String msReq = "<thread thread=\"" + msThread + "\" version=\"20061206\" res_from=\"-100\" />\0";
			string msPort = util.getRegGroup(message, "jp:(.+?)/");
			string msAddr = util.getRegGroup(message, "://(.+?):");
			
			util.debugWriteLine(msAddr + " " + msPort);
			util.debugWriteLine("msuri " + msUri);
			util.debugWriteLine("msreq " + msReq);
			
			var header =  new List<KeyValuePair<string, string>>();
			header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
			wsc = new WebSocket(msUri,  "", null, header, "", "", WebSocketVersion.Rfc6455);
			wsc.Opened += onWscOpen;
			wsc.Closed += onWscClose;
			wsc.MessageReceived += onWscMessageReceive;
			wsc.Error += onWscError;
			himodukeWS[0] = _ws;
			himodukeWS[1] = wsc;
			
	        util.debugWriteLine(msUri);
	        
	        wsc.Open();
	        	        
	        
			util.debugWriteLine("ms start");
			
			
		}
		public void stopRecording(WebSocket _ws, WebSocket _wsc) {
			util.debugWriteLine("stop recording");
			try {
				if (_ws != null && _ws.State != WebSocketState.Closed) _ws.Close();
//				_ws = null;
			} catch (Exception e) {
				util.debugWriteLine("ws close error");
				util.debugWriteLine(e.Message + e.StackTrace);
			}
			try {
				if (_wsc != null && _wsc.State != WebSocketState.Closed && _wsc.State != WebSocketState.Closing) {
					util.debugWriteLine("state close wsc " + WebSocketState.Closed + " " + _wsc.State);					
					_wsc.Close();
				}
			} catch (Exception e) {
				util.debugWriteLine("wsc close error");
				util.debugWriteLine(e.Message + e.StackTrace);
			}
			try {
//				if (commentSW != null) commentSW.Close();
			} catch (Exception e) {
				util.debugWriteLine("comment sw close error");
				util.debugWriteLine(e.Message + e.StackTrace);
			}
			/*
			try {
				if (rec != null) rec.stopRecording();
			} catch (Exception e) {
				util.debugWriteLine("rec close error");
				util.debugWriteLine(e.Message + e.StackTrace);
			}
			isRetry = false;
			*/
		}
		/*
		private void endStreamProcess() {
			if (rm.rfu != rfu) return;
					
			//string[] recFolderFile = util.getRecFolderFilePath(recFolderFile[0], recFolderFile[1], recFolderFile[2], recFolderFile[3], recFolderFileInfo[4]);
			util.debugWriteLine("recforlderfie");
			util.debugWriteLine("recforlderfi " + string.Join(" ",recFolderFile));
			if (recFolderFile == null) return;
			
			if (!h5r.isAliveStream()) return;
			start();
		}
		*/
		private void displayStatistics(string e) {
			var visit = util.getRegGroup(e, "{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"(\\d+?)\",\"\\d+?\"");
			var comment = util.getRegGroup(e, "{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"\\d+?\",\"(\\d+?)\"");
			try {
				if (visit != null)
					visit = int.Parse(visit).ToString("n0");
				if (comment != null) 
					comment = int.Parse(comment).ToString("n0");
				rm.form.setStatistics(visit, comment);
			} catch (Exception ee){
				util.debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
			}
		}
		private void onWscOpen(object sender, EventArgs e) {
			util.debugWriteLine("ms open a");
			wsc.Send(msReq);
			util.debugWriteLine("ms open b");
			
			if (rm.rfu != rfu) {
				//stopRecording();
//				wsc.Close();
				return;
			}			
			try {
				if (bool.Parse(rm.cfg.get("IsgetComment")) && commentSW == null) {
					var commentFileName = util.getOkCommentFileName(rm.cfg, recFolderFile[1], lvid);
					commentSW = new StreamWriter(commentFileName, false, System.Text.Encoding.UTF8);
			        if (bool.Parse(rm.cfg.get("IsgetcommentXml"))) {
						commentSW.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
				        commentSW.WriteLine("<packet>");
				        commentSW.Flush();
					} 
			       
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}

		}
		
		private void onWscClose(object sender, EventArgs e) {
			util.debugWriteLine("ms onclose");
			closeWscProcess();
			wsc = null;
			try {
				
				ws.Close();
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			
		}
		private void closeWscProcess() {
			if (commentSW != null) {
				if (bool.Parse(rm.cfg.get("IsgetcommentXml"))) {
					try {
						commentSW.WriteLine("</packet>");
						commentSW.Flush();
					} catch (Exception ee) {
						util.debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.TargetSite);
					}
				}
//				commentSW.Close();
//				commentSW = null;
			}
			

			//stopRecording();
//			endStreamProcess();
		}
		private void onWscError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			util.debugWriteLine("ms onerror");
			//stopRecording();
//			endStreamProcess();
		}
		private void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
			util.debugWriteLine("on wsc message " + e.Message);
			if (rm.rfu != rfu || !isRetry) {
				try {
					if (wsc != null) wsc.Close();
				} catch (Exception ee) {
					util.debugWriteLine("wsc message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
				}
				//stopRecording();
				util.debugWriteLine("tigau rfu comment" + e.Message);
				return;
			}
			
			
			if (isTimeShift && e.Message.StartsWith("{\"ping\":{\"content\":\"rf:")) {
				closeWscProcess();
			}
			
			var xml = JsonConvert.DeserializeXNode(e.Message);
			var chatinfo = new namaichi.info.ChatInfo(xml);
			
			XDocument chatXml;
			if (isTimeShift) chatXml = chatinfo.getFormatXml(openTime);
			else chatXml = chatinfo.getFormatXml(serverTime);
			util.debugWriteLine("xml " + chatXml.ToString());
			
			if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
					chatinfo.premium == "3")) return;
			if (chatinfo.root != "chat" && chatinfo.root != "thread") return;
			
			if (chatinfo.root == "thread") {
				serverTime = chatinfo.serverTime;
			}
			
			util.debugWriteLine("wsc message " + ws);
			
//			Newtonsoft.Json
			//if (e.Message.IndexOf("chat") < 0 &&
			//    	e.Message.IndexOf("thread") < 0) return;
			
			
//            util.debugWriteLine(jsonCommentToXML(text));
			try {
				if (commentSW != null) {
					if (bool.Parse(rm.cfg.get("IsgetcommentXml"))) {
						commentSW.WriteLine(chatXml.ToString());
					} else {
						var vposReplaced = Regex.Replace(e.Message, 
			            	"\"vpos\"\\:(\\d+)", 
			            	"\"vpos\":" + chatinfo.vpos + "");
						commentSW.WriteLine(vposReplaced);
					}
		            commentSW.Flush();
				}
           
			} catch (Exception ee) {util.debugWriteLine(ee.Message + " " + ee.StackTrace);}
			
			if (!isTimeShift)
				addDisplayComment(chatinfo);

		}
		private void addDisplayComment(namaichi.info.ChatInfo chat) {
			
			if (chat.root.Equals("thread")) return;
			if (chat.contents == "再読み込みを行いました<br>読み込み中のままの方はお手数ですがプレイヤー下の更新ボタンをお試し下さい") {
				util.debugWriteLine("chat 再読み込みを行いました");
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
			rm.form.addComment(keikaTime, chat.contents);
		}
		private bool isFirstChoiceQuality(string message, string bestGettableQuolity) {
//			var bestGettableQuality = getBestGettableQuolity(message);
			
			var currentQuality = util.getRegGroup(message, "\"quality\":\"(.+?)\"");
			
			return currentQuality == bestGettableQuolity; 
			
		}
		private string getBestGettableQuolity(string msg) {
			var qualityList = new string[] {"abr", "super_high", "high",
				"normal", "low", "super_low"};
			var gettableList = util.getRegGroup(msg, "\"qualityTypes\"\\:\\[(.+?)\\]").Replace("\"", "").Split(',');
			var ranks = rm.cfg.get("qualityRank").Split(',');
			
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
		public void reConnect() {
			util.debugWriteLine("reconnect");
//			onOpen(null, null);
			ws.Close();
//			ws.Open();
		}
		private bool isEndedProgram() {
			var a = new System.Net.WebHeaderCollection();
			var res = util.getPageSource(h5r.url, ref a, container);
			util.debugWriteLine("isendedprogram url " + h5r.url);
//			util.debugWriteLine("isendedprogram res " + res);
			if (res == null) return false;
			var type = util.getPageType(res);
			util.debugWriteLine("is ended program  pagetype " + type);
			return (type == 7 || type == 2 || type == 3);
		}
	}
}
	