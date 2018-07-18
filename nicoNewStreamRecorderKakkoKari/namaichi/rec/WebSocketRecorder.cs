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
		private string hlsUrl;
		private bool isNoPermission = false;
		private long openTime;
		public bool isEndProgram = false;
		
		private System.Threading.Thread mainThread;
		public WebSocketRecorder(string[] webSocketInfo, CookieContainer container, string[] recFolderFile, RecordingManager rm, RecordFromUrl rfu, Html5Recorder h5r, long openTime)
		{
			this.webSocketInfo = webSocketInfo;
			this.container = container;
			this.recFolderFile = recFolderFile;
			this.rm = rm;
			this.rfu = rfu;
			this.h5r = h5r;
			this.openTime = openTime;
			
		}
		public bool start() {
			util.debugWriteLine("rm.rfu dds0 " + rm.rfu);
			
			connect(webSocketInfo[0]);
			
			util.debugWriteLine("rm.rfu dds1 " + rm.rfu);
			
			broadcastId = util.getRegGroup(webSocketInfo[0], "watch/(.+?)\\?");
			userId = util.getRegGroup(webSocketInfo[0], "audience_token=.+?_(.+?)_");
			
			util.debugWriteLine("rm.rfu dds6 " + rm.rfu);
			util.debugWriteLine("rm.rfu dds5 " + rm.rfu);
			
			util.debugWriteLine("ws main " + ws + " a " + (ws == null));
			
			
			
			
//			while (ws.State != WebSocket4Net.WebSocketState.Closed) {
			while (rm.rfu == rfu && ws != null && isRetry && 
			       (rec == null || !rec.isStopRead())) {
				System.Threading.Thread.Sleep(1000);
				if (rec != null) 
					util.debugWriteLine("isStopread " + rec.isStopRead());
				
				//test
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			isRetry = false;
			
			
			util.debugWriteLine("rm.rfu dds3 " + rm.rfu);
			util.debugWriteLine("closed saikai");
			
			return isNoPermission;
		}
		private void connect(string url) {
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
			
			if (rm.rfu != rfu) stopRecording();
		}
		private void onClose(object sender, EventArgs e) {
			util.debugWriteLine("on close " + e.ToString());
			stopRecording();
			
			

		}
		private void onError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			util.debugWriteLine("on error " + e.Exception.Message);
			stopRecording();
//			ws.Open();
//			endStreamProcess();
		}
		private void onDataReceive(object sender, DataReceivedEventArgs e) {
			util.debugWriteLine("on data " + e.Data);
		}
		private void onMessageReceive(object sender, MessageReceivedEventArgs e) {
			util.debugWriteLine("receive " + e.Message);
			
			
//			util.debugWriteLine("ws " + ws);
			if (ws == null) return;
			//pong
			if (e.Message.IndexOf("\"ping\"") >= 0) {
				sendPong();
			}
			
			//get message
			if (e.Message.IndexOf("\"messageServerUri\"") >= 0) {
				connectMessageServer(e.Message);
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
			   	|| e.Message.IndexOf("\"END_PROGRAM\"") >= 0) {
				if (e.Message.IndexOf("\"TAKEOVER\"") >= 0) rm.form.addLogText("追い出されました。");
				
				//SERVICE_TEMPORARILY_UNAVAILABLE 予約枠開始後に何らかの問題？
				if (e.Message.IndexOf("\"SERVICE_TEMPORARILY_UNAVAILABLE\"") > 0) 
					rm.form.addLogText("サーバーからデータの受信ができませんでした。リトライします。");
			
				if (e.Message.IndexOf("\"END_PROGRAM\"") > 0)
					isEndProgram = true;
				
				util.debugWriteLine("kiteru");
//				connect(webSocketInfo[0].Replace("\"requireNewStream\":false", "\"requireNewStream\":true"));
				isNoPermission = true;
				stopRecording();
//				Task.Run(() => {
//				         	sendIntervalPong();
//				         });
			} else if (e.Message.IndexOf("\"disconnect\"") >= 0) {
				util.debugWriteLine("unknown disconnect");
				isNoPermission = true;
				stopRecording();
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
			Task.Run(() => {
				util.debugWriteLine("rec " + string.Join(" ", recFolderFile));
				hlsUrl = util.getRegGroup(message, "\"uri\":\"(.+?)\"");
				rm.hlsUrl = hlsUrl;
				util.debugWriteLine(hlsUrl);
//				string[] command = { 
//						"-i", "" + url + "", 
//						"-c", "copy", "-bsf:a",
//						"aac_adtstoasc", "\"" + recFolderFile[1] + ".mp4\""};
				var tuika = rm.cfg.get("ffmpegopt");
				var tuikaArr = tuika.Split(' ');
				string[] _command = {
						"-i", "" + hlsUrl + "", 
						"-c", "copy", "\"" + recFolderFile[1] + ".ts\"" };
//						"-c", "copy", "-bsf:a", "aac_adtstoasc", "\"" + recFolderFile[1] + ".ts\"" };
				var _buf = new List<string>();
				_buf.AddRange(_command.AsEnumerable());
				_buf.AddRange(tuikaArr.AsEnumerable());
				string[] command = _buf.ToArray();
				
				util.debugWriteLine(string.Join(" ", command));
				rec = new Record(rm, true, rfu);
				rec.recordCommand(command);
				 
				util.debugWriteLine("stop recd");
				isRetry = false;
				stopRecording();
			});
		}
		private void connectMessageServer(string message) {
	    	util.debugWriteLine("message");
			string msUri = util.getRegGroup(message, "(ws://.+?)\"");
			string msThread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
	//		String msReq = "[truncated][{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-1000,\"with_global\":1,\"scores\":1,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]\0";
	
			msReq = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-10,\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
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
			
	        util.debugWriteLine(msUri);
	        
	        wsc.Open();
	        	        
	        
			util.debugWriteLine("ms start");
			
			
		}
		public void stopRecording() {
			util.debugWriteLine("stop recording");
			try {
				if (ws != null && ws.State != WebSocketState.Closed) ws.Close();
//				ws = null;
			} catch (Exception e) {
				util.debugWriteLine("ws close error");
				util.debugWriteLine(e.Message + e.StackTrace);
			}
			try {
				if (wsc != null && wsc.State != WebSocketState.Closed && wsc.State != WebSocketState.Closing) {
					util.debugWriteLine("state close wsc " + WebSocketState.Closed + " " + wsc.State);					
					wsc.Close();
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
			try {
				if (rec != null) rec.stopRecording();
			} catch (Exception e) {
				util.debugWriteLine("rec close error");
				util.debugWriteLine(e.Message + e.StackTrace);
			}
			isRetry = false;
			
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
				visit = int.Parse(visit).ToString("n0");
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
				stopRecording();
			}			
			try {
				if (bool.Parse(rm.cfg.get("IsgetComment"))) {
					commentSW = new StreamWriter(recFolderFile[1] + ".xml", false, System.Text.Encoding.UTF8);
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
			if (commentSW != null) {
				if (bool.Parse(rm.cfg.get("IsgetcommentXml"))) {
					try {
						commentSW.WriteLine("</packet>");
						commentSW.Flush();
					} catch (Exception ee) {
						util.debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.TargetSite);
					}
				}
				commentSW.Close();
				commentSW = null;
			}
			stopRecording();
//			endStreamProcess();
		}
		private void onWscError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			util.debugWriteLine("ms onerror");
			stopRecording();
//			endStreamProcess();
		}
		private void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
//			util.debugWriteLine("ms " + e.Message);
			if (rm.rfu != rfu || !isRetry) {
				wsc.Close();
				stopRecording();
				util.debugWriteLine("tigau rfu comment" + e.Message);
			}
			
			var xml = JsonConvert.DeserializeXNode(e.Message);
			var chatinfo = new namaichi.info.ChatInfo(xml);
			var chatXml = chatinfo.getFormatXml(serverTime);
			
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
	}
}
