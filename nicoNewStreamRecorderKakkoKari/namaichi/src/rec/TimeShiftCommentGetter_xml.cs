/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/22
 * Time: 16:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of TimeShiftCommentGetter_xml.
	/// </summary>
	public class TimeShiftCommentGetter_xml
	{
		//string uri;
		string thread;
		string userId;
		MainForm form;
		RecordingManager rm;
		RecordFromUrl rfu;
		long openTime = 0;
		long _openTime = 0;
		string recFolderFile;
		string lvid;
		string programType = null;
		int startSecond;
		CookieContainer container;
		string ticket;
		string waybackKey;
		int gotMinTime;
		long gotMinVpos;
		int lastGotMinTime;
		long lastGotMinVpos;
		string[] gotMinXml = new string[2];
		int when = 0;
		int lastLastRes = int.MaxValue;
		string threadLine;
		bool isGetXml;
		List<string> gotCommentList = new List<string>();
		
		//private StreamWriter commentSW;
		private string fileName;
		//bool isSave = true;
		bool isRetry = true;
		public bool isEnd = false;
		public WebSocketRecorder rp;
		
		int gotCount = 0;
		public string[] sortedComments;
//		public TimeShiftConfig tsConfig;
		private bool isVposStartTime;
		private bool isRtmp;
		private int[] quePosTimeList;
		private bool isTimeShift = true;
		
		public TimeShiftCommentGetter_xml( 
				string userId, RecordingManager rm, 
				RecordFromUrl rfu, MainForm form, 
				long openTime, string[] recFolderFile, 
				string lvid, CookieContainer container, 
				string programType, long _openTime, 
				WebSocketRecorder rp, int startSecond, 
				bool isVposStartTime, bool isRtmp)
		{
//			this.uri = util.getRegGroup(message, "messageServerUri\"\\:\"(ws.+?)\"");
//			this.thread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
			this.rm = rm;
			this.rfu = rfu;
			this.userId = userId;
			this.form = form;
			this.openTime = openTime;
			this.recFolderFile = recFolderFile[1];
			this.lvid = lvid;
			this.container = container;
			this.isGetXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
//			this.programType = programType;
			this._openTime = _openTime;
			this.rp = rp;
			this.startSecond = startSecond;
			//this.tsConfig = tsConfig;
			this.isVposStartTime = isVposStartTime;
			this.isRtmp = isRtmp;
		}
		public void save() {
			if (!bool.Parse(rm.cfg.get("IsgetComment"))) {
				isEnd = true;
				return;
			}

			if (isRtmp) {
				if (!setQuePosList()) {
					isEnd = true;
					return;
				}
			}
			
			
	        
			
//			Task.Run(() => {
			    while (true) {
			        if (rp.ri.isRtmp || rp.firstSegmentSecond != -1) break;
					Thread.Sleep(500);
				}
			    util.debugWriteLine("ms start");
				connect();
				
//			});
		}
		private bool setQuePosList() {
			var url = "https://live.nicovideo.jp/api/getplayerstatus/" + lvid;
			var res = util.getPageSource(url, container, null, false, 5000);
			
			var xml = new XmlDocument();
			xml.LoadXml(res);
			var que = xml.SelectSingleNode("/getplayerstatus/stream/quesheet");
			if (que == null) return false;
			var play = getPlay(que);
			if (play == null) {
				Thread.Sleep(3000);
				quePosTimeList = new int[]{0};
				return false;
			}
			quePosTimeList = getPublishList(que, play);
			return true;
//			if (publishList.Count == 0) quePosTimeList = new int[]{0};
		}
		private string getPlay(XmlNode ques) {
			string defaultP = null;
			string premiumP = null;
			foreach (XmlNode q in ques.ChildNodes) {
				var qi = q.InnerText;
				if (qi.StartsWith("/play case")) {
					if (qi.IndexOf("default:rtmp:") > -1)
						defaultP = util.getRegGroup(qi, "default:rtmp:(.+?)[, $]");
					if (qi.IndexOf("premium:rtmp:") > -1)
						premiumP = util.getRegGroup(qi, "premium:rtmp:(.+?)[, $]");
				} else 
					if (qi.StartsWith("/play")) return util.getRegGroup(qi, "rtmp:(.+?)[, $]");
			}
			if (premiumP != null) return premiumP;
			else if (defaultP != null) return defaultP;
			return null;
		}
		private int[] getPublishList(XmlNode que, string play) {
			var l = new List<int>();
			foreach (XmlNode q in que.ChildNodes) {
				var qi = q.InnerText;
				if (qi.StartsWith("/publish " + play))
					l.Add(int.Parse(q.Attributes["vpos"].Value));
			}
			return l.ToArray();
		}
		private string incrementRecFolderFile(string recFolderFile) {
			var r = util.incrementRecFolderFile(recFolderFile);
			if (r == null) return rp.getRecFilePath()[1];
			return r;
		}
		bool connect() {
			util.debugWriteLine("connect tscg ms");
			Task.Run(() => {heartBeater();});
			
			try {
				when = util.getUnixTime();//(int)openTime;
				lastGotMinTime = gotMinTime = when;
				lastGotMinVpos = lastGotMinVpos = 360000000;
				string address, port;//, thread;
				if (!getInfo(out address, out port, out thread)) return false;
				
				
				NetworkStream stream = null;
				var lastGotCommentListCount = 0;
				
				while (isRetry) {
					//isSave = true;
//					setStreamWriter();
					
					var tcp = new TcpClient(address, int.Parse(port));
					stream = tcp.GetStream();
					
					waybackKey = getWaybackKey();
					var request = getRequest();
					
					stream.Write(request, 0, request.Length);
					stream.Flush();
					stream.ReadTimeout = 5000;
					
					var b = new byte[(isTimeShift) ? (3000 * 1000) : (10 * 1000)];
					var lastI = b.Length;
					
					var isNextRequest = false;
					var getChatCount = 0;
					
					while (rm.rfu == rfu && isRetry && !isNextRequest) {
						var bList = new List<byte>();
						int i = -1;
						//var isBreak = false;
						while (i != 0) {
							try {
								i = stream.Read(b, 0, b.Length);
								for (var _i = 0; _i < i; _i++)
									bList.Add(b[_i]);
							} catch (Exception e) {
								util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
								//isBreak = true;
//								continue;
								break;
							}
							System.Threading.Thread.Sleep(1000);
						}
//						if (isBreak) break;
						
						for (var j = i + 0; j < b.Length && j < lastI && i > -1; j++) {
								b[j] = 0;

						}
						var s = Encoding.UTF8.GetString(bList.ToArray()).Trim('\0');
						if (s.Length == 0) break;
						var ss = s.Split('\0');
						util.debugWriteLine(i + " " + ss + " ssLen " + ss.Length);
						
						var isKaburiNasi = true;
						foreach (var c in ss) {
							try {
								isKaburiNasi = chatProcess(c);
								getChatCount++;
//								util.debugWriteLine("getchatcount " + getChatCount);
								if (getChatCount > 900 || getChatCount > lastLastRes - 1) 
									isKaburiNasi = false;
								
							} catch (Exception e) {
								util.debugWriteLine(c);
								util.debugWriteLine(e.Message + e.StackTrace + e.Source + e.TargetSite);
							}
						}
						lastGotMinTime = gotMinTime;
						lastGotMinVpos = gotMinVpos;
						
						
						if (!isKaburiNasi) {
							util.debugWriteLine("kaburinasi last_res " + lastLastRes + " gotCount " + gotCommentList.Count);
							Task.Run(() => {
					         	Thread.Sleep(1000);
					         	if (lastLastRes < 900)
					         		isRetry = false;
					         	else 
					         		isNextRequest = true;
					         	
							});
						}
						
						
					}
					if (gotCommentList.Count == lastGotCommentListCount) break;
					lastGotCommentListCount = gotCommentList.Count;
				}
				stream.Close();
				
				endProcess();
				isEnd = true;
				util.debugWriteLine("timeshift chat end");
		        return true;
			} catch (Exception e) {
				util.debugWriteLine("connect exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				return false;
			}
		}
		private string getWaybackKey() {
			var url = "https://live.nicovideo.jp/api/getwaybackkey?thread=" + thread;
			var r = util.getPageSource(url, container, null, false, 5000);
			if (r == null) {
				util.debugWriteLine("getwayback exception " + url);
				return null;
			}
			return util.getRegGroup(r, "waybackkey=(.+)");
		}
		/*
		private void setStreamWriter() {
			try {
				var isGetComment = rm.cfg.get("IsgetComment");
				var isGetCommentXml = rm.cfg.get("IsgetcommentXml");
				
				if (bool.Parse(isGetComment) && commentSW == null && !rm.isPlayOnlyMode) {
					var fName = (commentFileName == null) ? recFolderFile : incrementRecFolderFile(commentFileName);
					commentFileName = fName;
					var _commentFileName = util.getOkCommentFileName(rm.cfg, fName, lvid, isTimeShift, isRtmp);
					var isExists = File.Exists(_commentFileName);
					commentSW = new StreamWriter(_commentFileName, false, System.Text.Encoding.UTF8);
					
					if (bool.Parse(isGetCommentXml) && !isExists) {
//						Newtonsoft.Json.JsonConvert.SerializeXNode
					} 
			       
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		*/
		private bool getInfo(out string address, out string port, out string thread) {
			var url = "https://live.nicovideo.jp/api/getplayerstatus?v=" + lvid;
			var res = util.getPageSource(url, container, null, false, 5000);
			
			var xml = new XmlDocument();
			xml.LoadXml(res);
			try {
				address = xml.SelectSingleNode("/getplayerstatus/ms/addr").InnerText;
				port = xml.SelectSingleNode("/getplayerstatus/ms/port").InnerText;
				thread = xml.SelectSingleNode("/getplayerstatus/ms/thread").InnerText;
				userId = xml.SelectSingleNode("/getplayerstatus/user/user_id").InnerText;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				address = port = thread = null;
				return false;
			}
			return true;
		}
		private bool chatProcess(string c) {
//			util.debugWriteLine(c);
			XDocument chatXml = null;
			chatXml = XDocument.Parse(c);

			
			var chatinfo = new ChatInfo(chatXml);
			
			
			var vposStartTime = (isVposStartTime) ? (long)rp.firstSegmentSecond : 0;
			if (isRtmp) {
//				chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
				chatinfo.getFromXml(_openTime + vposStartTime);
			} else {
				if (programType == "official") {
//					chatXml = chatinfo.getFormatXml(0, true, vposStartTime);
					chatinfo.getFromXml(_openTime + vposStartTime);
//					chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
				} else 
//					chatXml = chatinfo.getFormatXml(openTime + vposStartTime);
					chatinfo.getFromXml(openTime + vposStartTime);
			}
			
//			chatinfo.getFromXml(serverTime);
//			chatinfo.getFormatXml(serverTime);
			
//			util.debugWriteLine("xml " + chatXml.ToString());
			
			if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 &&
					chatinfo.premium == "3")) return true;
			if (chatinfo.root != "chat" && chatinfo.root != "thread") return true;
			
			if (chatinfo.root == "thread") {
//				serverTime = chatinfo.serverTime;
				ticket = chatinfo.ticket;
//				lastLastRes = (chatinfo.lastRes == null) ? 0 : int.Parse(chatinfo.lastRes);
				if (chatinfo.lastRes != null) lastLastRes = int.Parse(chatinfo.lastRes);
			}
			
			if (chatXml.ToString().Equals(gotMinXml[1])) {
				//isSave = false;
			}
			if (chatinfo.root == "chat" && (chatinfo.date >= lastGotMinTime || chatinfo.vpos >= lastGotMinVpos)) {
//				return true;
			}
//			if (!isSave) return false;
			
			if (chatinfo.root == "chat" && (chatinfo.date < gotMinTime)) {
				gotMinTime = chatinfo.date;
				gotMinVpos = chatinfo.vpos;
				gotMinXml[1] = gotMinXml[0];
				gotMinXml[0] = chatXml.ToString();
			}
			
//			Newtonsoft.Json
			//if (e.Message.IndexOf("chat") < 0 &&
			//    	e.Message.IndexOf("thread") < 0) return;
			
			
//            addDebugBuf(jsonCommentToXML(text));
			try {
					
				string s = chatXml.ToString();
				
				if (chatinfo.root == "thread") {
					if (threadLine == null) {
						threadLine = s;
						if (!rfu.isPlayOnlyMode)
							form.addLogText("アリーナ席に" + chatinfo.lastRes + "件ぐらいのコメントが見つかりました(追い出しコメント含む)");
					}
	            } else {
//		            	commentSW.WriteLine(s + "}>");
//		            	commentSW.Flush();
//		            	gotCount++;
//		            	if (gotCount % 2000 == 0) form.addLogText(gotCount + "件のコメントを保存しました");
					if (gotCommentList.IndexOf(s) == -1)
	            		gotCommentList.Add(s);
					gotCount++;
	            	
	            	if (gotCount % 2000 == 0 && !rfu.isPlayOnlyMode) form.addLogText(gotCount + "件のコメントを保存しました");
	            }
				
//				util.debugWriteLine("write comment " + s);
		             
           
			} catch (Exception ee) {util.debugWriteLine("comment write exception " + ee.Message + " " + ee.StackTrace);}
			
			//if (!isTimeShift)
//				addDisplayComment(chatinfo);

			return true;
		}
		private byte[] getRequest() {
			when = lastGotMinTime + 10;
			var reqStr = "<thread thread=\"" + thread + "\" version=\"20061206\" res_from=\"-1000\" fork=\"0\" user_id=\"" + userId + "\" with_global=\"1\" scores=\"1\" nicoru=\"0\" waybackkey=\"" + waybackKey + "\" when=\"" + when + "\" /> \0";
			util.debugWriteLine(reqStr);
			return Encoding.UTF8.GetBytes(reqStr);
		}
		private void endProcess() {
			var isWrite = (rm.cfg.get("IsgetComment") == "true" && !rfu.isPlayOnlyMode);
			if (isWrite)
				form.addLogText("コメントの後処理を開始します");
			/*
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
			util.debugWriteLine("gotCommentList count " + gotCommentList.Count);
				
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
			
			var fileNum = (quePosTimeList != null) ? quePosTimeList.Length : 1;
			for (int j = 1; j < fileNum + 1; j++) {
				if (j != 0) recFolderFile = incrementRecFolderFile(recFolderFile);
				
				fileName = util.getOkCommentFileName(rm.cfg, recFolderFile, lvid, true, isRtmp);
				using (var w = new StreamWriter(fileName + "_", false, System.Text.Encoding.UTF8)) {
					if (isGetXml) {
						w.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
					    w.WriteLine("<packet>");
					    
					    w.Flush();
					}
					w.WriteLine(threadLine);
					for (var i = 0; i < chats.Length; i++) {
						if (quePosTimeList != null) {
							if (chats[i] == null) continue;
							var _vpos = util.getRegGroup(chats[i], "vpos.+?(-*\\d+)");
							if (_vpos == null) continue;
							var vpos = int.Parse(_vpos);
							
							if (isRtmp) {
								
							}
							if (j == fileNum - 0 || vpos < quePosTimeList[j + 0]) {
								var oriVposPart = util.getRegGroup(chats[i], "(vpos.+?-*\\d+)");
								var newVposPart = oriVposPart.Replace(vpos.ToString(), (vpos - quePosTimeList[j - 1]).ToString());
								w.WriteLine(chats[i].Replace(oriVposPart, newVposPart));
								chats[i] = null;
							}
						} else {
							w.WriteLine(chats[i]);
						}
						
					}
					if (isGetXml) {
						w.WriteLine("</packet>");
					}
					//w.Close();
				}
				
				File.Delete(fileName);
				File.Move(fileName + "_", fileName);
			}
			form.addLogText("コメントの保存を完了しました");
		}
		private void heartBeater() {
			while (rm.rfu == rfu) {
				try {
					var url = "http://ow.live.nicovideo.jp/api/heartbeat";
					var handler = new System.Net.Http.HttpClientHandler();
					handler.UseCookies = true;
					handler.CookieContainer = container;
					var http = new System.Net.Http.HttpClient(handler);
					handler.UseProxy = true;
					handler.Proxy = util.httpProxy;
					http.Timeout = TimeSpan.FromSeconds(5);					
					var contentStr = "{\"lang\":\"ja-jp\",\"locale\":\"JP\",\"seat_locale\":\"JP\",\"screen\":\"ScreenNormal\",\"v\":\"" + lvid + "\",\"datarate\":0}";
					
					var content = new System.Net.Http.StringContent(contentStr);
					content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
	
					var _res = http.PostAsync(url, content).Result;
					var res = _res.Content.ReadAsStringAsync();

				} catch (Exception e) {
					util.debugWriteLine(e.Message+e.StackTrace + e.Source + e.TargetSite);
				}
				Thread.Sleep(60000);
				
			}
		}
	}
}
