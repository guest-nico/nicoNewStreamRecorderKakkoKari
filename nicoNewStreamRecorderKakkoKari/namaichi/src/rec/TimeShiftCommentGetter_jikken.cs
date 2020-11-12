/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/13
 * Time: 15:50
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

/*
namespace namaichi.rec
{
	/// <summary>
	/// Description of TimeShiftCommentGetter.
	/// </summary>
	public class TimeShiftCommentGetter_jikken
	{
		string uri;
		string thread;
		string key;
		string userId;
		MainForm form;
		RecordingManager rm;
		RecordFromUrl rfu;
		private JikkenRecordProcess jrp;
		WatchingInfo wi;
		long openTime = 0;
		string[] recFolderFile;
		string lvid;
		CookieContainer container;
		int startSecond;
		string ticket;
		string waybackKey;
		int gotMinTime;
		string[] gotMinXml = new string[2];
		int when = 0;
		int lastLastRes = int.MaxValue;
		public string threadLine;
		public bool isGetXml;
		public List<string> gotCommentList = new List<string>();
		private DeflateDecoder deflateDecoder = new DeflateDecoder();
		
//		private StreamWriter commentSW;
//		private string fileName;
		WebSocket wsc = null;
		bool isSave = true;
		bool isRetry = true;
		public bool isEnd = false;
		
		int gotCount = 0;
		bool isChat;
//		public TimeShiftConfig tsConfig;
		bool isVposStartTime;
		
		public TimeShiftCommentGetter_jikken(JikkenRecordProcess jrp, string userId, RecordingManager rm, RecordFromUrl rfu, MainForm form, long openTime, string[] recFolderFile, string lvid, CookieContainer container, string thread, string key, bool isChat, WatchingInfo wi, int startSecond, bool isVposStartTime)
		{
			this.jrp = jrp;
			this.uri = jrp.msUri;
			this.thread = thread;
			this.key = key;
			this.rm = rm;
			this.rfu = rfu;
			this.userId = userId;
			this.form = form;
			this.openTime = openTime;
			this.recFolderFile = recFolderFile;
			this.lvid = lvid;
			this.container = container;
			this.isGetXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.isChat = isChat;
			this.wi = wi;
			this.startSecond = startSecond;
			this.isVposStartTime = isVposStartTime;
		}
		public void save() {
			if (!bool.Parse(rm.cfg.get("IsgetComment"))) {
//				isEnd = true;
//				return;
			}
			
			while (jrp.firstSegmentSecond == -1 && !jrp.isRtmp) {
				Thread.Sleep(500);
			}
			util.debugWriteLine("firstSegmentSecond " + jrp.firstSegmentSecond);
			try {
//				fileName = util.getOkCommentFileName(rm.cfg, recFolderFile[1], lvid, true);
//				var isExists = File.Exists(fileName);
//				commentSW = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
//				if (isGetXml && !isExists) {
					//commentSW.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
				    //commentSW.WriteLine("<packet>");
				    //commentSW.Flush();
//				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
			
			when = util.getUnixTime();//(int)openTime;
			gotMinTime = when;
	        
			connect();
			util.debugWriteLine("ms start");
			

		}
		bool connect() {
			util.debugWriteLine("connect ms");
			isSave = true;
			try {
				waybackKey = getWaybackKey();
				
				var header =  new List<KeyValuePair<string, string>>();
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
				//wsc = new WebSocket(msUri,  "", null, header, "", "", WebSocketVersion.Rfc6455);
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Extensions", "permessage-deflate"));
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Version", "13"));
				wsc = new WebSocket(uri, "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null, uri.StartsWith("wss") ? SslProtocols.Tls12 : SslProtocols.None);
				//wsc = new WebSocket(uri, "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12);
				wsc.Opened += onWscOpen;
				wsc.Closed += onWscClose;
				wsc.MessageReceived += onWscMessageReceive;
				wsc.Error += onWscError;
				
		        util.debugWriteLine(uri);
		        
		        wsc.Open();
		        return true;
			} catch (Exception e) {
				util.debugWriteLine("connect exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				return false;
			}
		}
		private void onWscOpen(object sender, EventArgs e) {
			util.debugWriteLine("ms open a");
			var req = getReq("-1000");
			wsc.Send(req);
			util.debugWriteLine("ms open b");
			
			if (rm.rfu != rfu) {
				//stopRecording();
//				wsc.Close();
				return;
			}			
			

		}
		
		private void onWscClose(object sender, EventArgs e) {
			util.debugWriteLine("ms onclose");
			//closeWscProcess();
			wsc = null;
			try {
				if (lastLastRes < 900) {
//					endProcess();
					isEnd = true;
//					rm.form.addLogText("コメントの保存を完了しました");
					return;
				}
				if (rm.rfu == rfu && isRetry && !isEnd) {
					while (true) {
						try {
							if (!connect()) continue;
							break;
						} catch (Exception ee) {
							util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
						}
					}
					
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			
		}

		private void onWscError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			util.debugWriteLine("ms onerror");
		}
		private void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
			
			if (rm.rfu != rfu || !isRetry) {
				try {
					if (wsc != null) wsc.Close();
				} catch (Exception ee) {
					util.debugWriteLine("wsc message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
				}
				//stopRecording();
				util.debugWriteLine("tigau rfu comment" + e.Message + " " + isRetry);
				return;
			}
			
			var decodedMsg = deflateDecoder.decode(e.Data);
//			util.debugWriteLine("decoded " + decodedMsg);
			
			var chatInfoList = jrp.getChatInfoList(decodedMsg);
			
			foreach (var chatinfo in chatInfoList) {
				
//				var xml = JsonConvert.DeserializeXNode(e.Message);
//				var chatinfo = new namaichi.info.ChatInfo(xml);
				
				XDocument chatXml;
				var vposStartTime = (isVposStartTime) ? (long)jrp.firstSegmentSecond : 0;
				chatXml = chatinfo.getFormatXml(openTime + vposStartTime);
	//			else chatXml = chatinfo.getFormatXml(serverTime);
	//			util.debugWriteLine("xml " + chatXml.ToString());
				
				if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
						chatinfo.premium == "3")) continue;
				if (chatinfo.root == "ping" && chatinfo.contents.IndexOf("rf:") > -1) {
					wsc.Close();
				}
				if (chatinfo.root != "chat" && chatinfo.root != "thread" && 
	    				chatinfo.root != "control") continue;
				
				if (chatinfo.root == "thread") {
					jrp.serverTime = chatinfo.serverTime;
//					ticket = chatinfo.ticket;
					jrp.jisa = util.getUnixToDatetime(jrp.serverTime) - DateTime.Now;

					lastLastRes = (chatinfo.lastRes == null) ? 0 : int.Parse(chatinfo.lastRes);
					
				}
	//			util.debugWriteLine(chatXml.ToString());
	//			util.debugWriteLine(gotMinXml[1]);
				if (chatXml.ToString().Equals(gotMinXml[1])) {
					
					isSave = false;
				}
				if (!isSave) continue;
				if ((chatinfo.root == "chat" || chatinfo.root == "control") && chatinfo.date < gotMinTime) {
					gotMinTime = chatinfo.date;
					gotMinXml[1] = gotMinXml[0];
					gotMinXml[0] = chatXml.ToString();
				}
				
	
				try {
	//				if (commentSW != null) {
						string s;
						if (isGetXml) {
							s = chatXml.ToString();
						} else {
							var vposReplaced = Regex.Replace(chatinfo.json, 
				            	"\"vpos\"\\:[ ]*(\\d+)", 
				            	"\"vpos\":" + chatinfo.vpos + "");
							s = vposReplaced;
						}
						
			            if (chatinfo.root == "thread") {
							if (threadLine == null) {
								threadLine = s;
								if (!rm.isPlayOnlyMode)
									form.addLogText("アリーナ席に" + chatinfo.lastRes + "件ぐらいのコメントが見つかりました(追い出しコメント含む)" + ((isChat) ? "(chat)" : "(control)"));
							}
			            } else {
	//		            	commentSW.WriteLine(s + "}>");
	//		            	commentSW.Flush();
							gotCommentList.Add(s);
			            	gotCount++;
			            	if (gotCount % 2000 == 0 && !rm.isPlayOnlyMode) form.addLogText(gotCount + "件のコメントを保存しました" + ((isChat) ? "(chat)" : "(control)"));
			            }
	//				}
	           
				} catch (Exception ee) {util.debugWriteLine(ee.Message + " " + ee.StackTrace);}
			}
			if (e.Message.IndexOf("rf:") > -1) 
				((WebSocket)(sender)).Close();
			//if (!isTimeShift)
//				addDisplayComment(chatinfo);

		}
		
		private string getWaybackKey() {
			var url = "https://api.cas.nicovideo.jp/v1/services/ex/waybackkey?threadId=" + thread;
			var r = util.getPageSource(url, container, null, false, 5000);
			if (r == null) {
				util.debugWriteLine("getwayback exception " + url);
				return null;
			}
			return util.getRegGroup(r, "\"key\":\"(.+)\"");
		}
		private string getReq(string resfrom) {
//			var when = (isSave) ? lastGetChatTime.ToString() : openTime.ToString();
			when = gotMinTime + 1;
			//var ret = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + thread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + resfrom + ",\"with_global\":1,\"scores\":1,\"nicoru\":0,\"waybackkey\":\"" + waybackKey + "\",\"when\":" + when + "}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			var ret = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + thread + "\",\"version\":\"" + wi.msVersion + "\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + resfrom + ",\"force_184\":\"0\",\"with_global\":1,\"scores\":1,\"nicoru\":0,\"threadkey\":\"" + key + "\",\"service\":\"LIVE\",\"when\":\"" + when + "\"}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			util.debugWriteLine(ret);
			return ret;
		}
		public void setIsRetry(bool b) {
			isRetry = b;
		}
		public static void endProcess(List<string> gotList, string fileName, MainForm form, bool isGetXml, string chatThreadLine, string controlThreadLine, JikkenRecordProcess jrp) {
			var isWrite = (form.rec.cfg.get("IsgetComment") == "true" && !form.rec.isPlayOnlyMode);
			if (isWrite)
				form.addLogText("コメントの後処理を開始します");
			
			var keys = new List<int>();
			foreach (var c in gotList) {
//				util.debugWriteLine(c);
//				util.debugWriteLine(util.getRegGroup(c, "date.+?(\\d+)"));
				var date = int.Parse(util.getRegGroup(c, "date.+?(\\d+)"));
				keys.Add(date);
			}
			var chats = gotList.ToArray();
			Array.Sort(keys.ToArray(), chats);
			
			jrp.gotTsCommentList = chats;
			if (!isWrite) return;
			
			var w = new StreamWriter(fileName + "_", false, System.Text.Encoding.UTF8);
			if (isGetXml) {
				w.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
			    w.WriteLine("<packet>");
			    w.Flush();
			}
			w.WriteLine(chatThreadLine);
			w.WriteLine(controlThreadLine);
			for (var i = 0; i < chats.Length; i++) {
				w.WriteLine(chats[i]);
			}
			if (isGetXml)
				w.WriteLine("</packet>");
			w.Close();
			
			File.Delete(fileName);
			File.Move(fileName + "_", fileName);
			form.addLogText("コメントの保存を完了しました");
			return;
		}
	}
}
*/