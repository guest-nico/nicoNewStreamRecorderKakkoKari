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

namespace namaichi.rec
{
	/// <summary>
	/// Description of TimeShiftCommentGetter.
	/// </summary>
	public class TimeShiftCommentGetter
	{
		string uri;
		string thread;
		string userId;
		MainForm form;
		RecordingManager rm;
		RecordFromUrl rfu;
		long openTime = 0;
		long _openTime = 0;
		string[] recFolderFile;
		string lvid;
		string programType;
		int startSecond;
		CookieContainer container;
		string ticket;
		string waybackKey;
		int gotMinTime;
		string[] gotMinXml = new string[2];
		int when = 0;
		int lastLastRes = int.MaxValue;
		string threadLine;
		bool isGetXml;
		List<string> gotCommentList = new List<string>();
		
		private StreamWriter commentSW;
		private string fileName;
		WebSocket wsc = null;
		bool isSave = true;
		bool isRetry = true;
		public bool isEnd = false;
		public WebSocketRecorder rp;
		
		int gotCount = 0;
		public string[] sortedComments;
<<<<<<< HEAD
		public TimeShiftConfig tsConfig;
		
		public TimeShiftCommentGetter(string message, string userId, RecordingManager rm, RecordFromUrl rfu, MainForm form, long openTime, string[] recFolderFile, string lvid, CookieContainer container, string programType, long _openTime, WebSocketRecorder rp, int startSecond, TimeShiftConfig tsConfig)
=======
		
		public TimeShiftCommentGetter(string message, string userId, RecordingManager rm, RecordFromUrl rfu, MainForm form, long openTime, string[] recFolderFile, string lvid, CookieContainer container, string programType, long _openTime, WebSocketRecorder rp)
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		{
			this.uri = util.getRegGroup(message, "messageServerUri\"\\:\"(ws.+?)\"");
			this.thread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
			this.rm = rm;
			this.rfu = rfu;
			this.userId = userId;
			this.form = form;
			this.openTime = openTime;
			this.recFolderFile = recFolderFile;
			this.lvid = lvid;
			this.container = container;
			this.isGetXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.programType = programType;
			this._openTime = _openTime;
			this.rp = rp;
<<<<<<< HEAD
			this.startSecond = startSecond;
			this.tsConfig = tsConfig;
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		}
		public void save() {
			if (!bool.Parse(rm.cfg.get("IsgetComment"))) {
				isEnd = true;
				return;
			}

			
				try {
//					if (!rm.isPlayOnlyMode)
//						fileName = util.getOkCommentFileName(rm.cfg, recFolderFile[1], lvid, true);
					/*
					var isExists = File.Exists(fileName);
					commentSW = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
					if (isGetXml && !isExists) {
						//commentSW.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
					    //commentSW.WriteLine("<packet>");
					    //commentSW.Flush();
					}
					*/
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + " " + ee.StackTrace);
				}
			
			when = util.getUnixTime();//(int)openTime;
			gotMinTime = when;
	        
			
			Task.Run(() => {
			    while (true) {
					if (rp.firstSegmentSecond != -1) break;
					Thread.Sleep(500);
				}
				connect();
				util.debugWriteLine("ms start");
			});
		}
		bool connect() {
			util.debugWriteLine("connect tscg ms");
			isSave = true;
			try {
				waybackKey = getWaybackKey();
				
				var header =  new List<KeyValuePair<string, string>>();
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
				//wsc = new WebSocket(msUri,  "", null, header, "", "", WebSocketVersion.Rfc6455);
				wsc = new WebSocket(uri, "", null, header, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36", "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12);
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
			util.debugWriteLine("ms tscg onclose");
			//closeWscProcess();
			wsc = null;
			try {
				if (lastLastRes < 900) {
					endProcess();
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
			
			
			var xml = JsonConvert.DeserializeXNode(e.Message);
			var chatinfo = new namaichi.info.ChatInfo(xml);
			
			XDocument chatXml;
			var vposStartTime = (tsConfig.isVposStartTime) ? (long)rp.firstSegmentSecond : 0;
			if (programType == "official")
				chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
			else chatXml = chatinfo.getFormatXml(openTime + vposStartTime);
//			else chatXml = chatinfo.getFormatXml(serverTime);
//			util.debugWriteLine("xml " + chatXml.ToString());
			
			if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
					chatinfo.premium == "3")) return;
			if (chatinfo.root == "ping" && chatinfo.contents.IndexOf("rf:") > -1) {
				wsc.Close();
			}
			if (chatinfo.root != "chat" && chatinfo.root != "thread") return;
			
			if (chatinfo.root == "thread") {
//				serverTime = chatinfo.serverTime;
				ticket = chatinfo.ticket;
				lastLastRes = (chatinfo.lastRes == null) ? 0 : int.Parse(chatinfo.lastRes);
				
			}
//			util.debugWriteLine(chatXml.ToString());
//			util.debugWriteLine(gotMinXml[1]);
			if (chatXml.ToString().Equals(gotMinXml[1])) {
				
				isSave = false;
			}
			if (!isSave) return;
			if (chatinfo.root == "chat" && chatinfo.date < gotMinTime) {
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
						var vposReplaced = Regex.Replace(e.Message, 
			            	"\"vpos\"\\:(\\d+)", 
			            	"\"vpos\":" + chatinfo.vpos + "");
						s = vposReplaced;
					}
					
		            if (chatinfo.root == "thread") {
						if (threadLine == null) {
							threadLine = s;
							if (!rm.isPlayOnlyMode)
								form.addLogText(chatinfo.lastRes + "件ぐらいのコメントが見つかりました(追い出しコメント含む)");
						}
		            } else {
//		            	commentSW.WriteLine(s + "}>");
//		            	commentSW.Flush();
//		            	gotCount++;
//		            	if (gotCount % 2000 == 0) form.addLogText(gotCount + "件のコメントを保存しました");
		            	gotCommentList.Add(s);
						gotCount++;
		            	
		            	if (gotCount % 2000 == 0 && !rm.isPlayOnlyMode) form.addLogText(gotCount + "件のコメントを保存しました");
		            }
//				}
           
			} catch (Exception ee) {util.debugWriteLine(ee.Message + " " + ee.StackTrace);}
			
				if (e.Message.IndexOf("rf:") > -1) 
					((WebSocket)(sender)).Close();
			//if (!isTimeShift)
//				addDisplayComment(chatinfo);

		}
		
		private string getWaybackKey() {
			var url = "http://live.nicovideo.jp/api/getwaybackkey?thread=" + thread;
			var r = util.getPageSource(url, container, null, false, 5000);
			if (r == null) {
				util.debugWriteLine("getwayback exception " + url);
				return null;
			}
			return util.getRegGroup(r, "waybackkey=(.+)");
		}
		private string getReq(string resfrom) {
//			var when = (isSave) ? lastGetChatTime.ToString() : openTime.ToString();
			when = gotMinTime + 1;
			var ret = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + thread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + resfrom + ",\"with_global\":1,\"scores\":1,\"nicoru\":0,\"waybackkey\":\"" + waybackKey + "\",\"when\":" + when + "}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			util.debugWriteLine(ret);
			return ret;
		}
		public void setIsRetry(bool b) {
			isRetry = b;
		}
		private void endProcess() {
			var isWrite = (rm.cfg.get("IsgetComment") == "true" && !rm.isPlayOnlyMode);
			if (isWrite)
				form.addLogText("コメントの後処理を開始します");
<<<<<<< HEAD
			/*
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			try {
				if (commentSW != null) commentSW.Close();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			*/
//			string[] chats;
//			using(var r = new StreamReader(fileName)) {
//				chats = r.ReadToEnd().Split(new string[]{"}>\r\n"}, StringSplitOptions.RemoveEmptyEntries);
//		    }
			//var isXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			var keys = new List<int>();
			foreach (var c in gotCommentList) {
//				util.debugWriteLine(c);
//				util.debugWriteLine(util.getRegGroup(c, "date.+?(\\d+)"));
				var date = int.Parse(util.getRegGroup(c, "date.+?(\\d+)"));
				keys.Add(date);
			}
			var chats = gotCommentList.ToArray();
			Array.Sort(keys.ToArray(), chats);
			
			rp.gotTsCommentList = chats;
			if (!isWrite) return;
			
<<<<<<< HEAD
			fileName = util.getOkCommentFileName(rm.cfg, recFolderFile[1], lvid, true);
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			var w = new StreamWriter(fileName + "_", false, System.Text.Encoding.UTF8);
			if (isGetXml) {
				w.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
			    w.WriteLine("<packet>");
			    
			    w.Flush();
			}
			w.WriteLine(threadLine);
			for (var i = 0; i < chats.Length; i++) {
				w.WriteLine(chats[i]);
			}
			if (isGetXml) {
				w.WriteLine("</packet>");
			}
			w.Close();
			
			File.Delete(fileName);
			File.Move(fileName + "_", fileName);
			form.addLogText("コメントの保存を完了しました");
		}
	}
}
