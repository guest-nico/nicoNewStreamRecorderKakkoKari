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
using System.Xml;
//using System.Text.RegularExpressions;
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
		string recFolderFile;
		string lvid;
		string programType;
		string roomName = "";
		int startSecond;
		CookieContainer container;
		string ticket;
		string waybackKey;
		int gotMinTime;
		string[] gotMinXml = new string[2];
		int _gotMinTime;
		string[] _gotMinXml = new string[2];
		int when = 0;
		int lastLastRes = int.MaxValue;
		string threadLine;
		bool isGetXml = true;
		bool isGetCommentXmlInfo = false;
		List<GotCommentInfo> gotCommentList = new List<GotCommentInfo>();
		List<GotCommentInfo> gotCommentListBuf = new List<GotCommentInfo>();
		
		//private StreamWriter commentSW;
		private string fileName;
		WebSocket wsc = null;
		bool isSave = true;
		bool isRetry = true;
		public bool isEnd = false;
		public WebSocketRecorder rp;
		//private RecordStateSetter rss;
		
		int gotCount = 0;
		public string[] sortedComments;
//		public TimeShiftConfig tsConfig;
		private bool isVposStartTime;
		private bool isRtmp;
		private int[] quePosTimeList;
		private RtmpRecorder rr;
		private bool isConvertSpace;
		
		public TimeShiftCommentGetter(string message, 
				string userId, RecordingManager rm, 
				RecordFromUrl rfu, MainForm form, 
				long openTime, string[] recFolderFile, 
				string lvid, CookieContainer container, 
				string programType, long _openTime, 
				WebSocketRecorder rp, int startSecond, 
				bool isVposStartTime, bool isRtmp, 
				RtmpRecorder rr, RecordStateSetter rss, 
				string roomName)
		{
			this.uri = util.getRegGroup(message, rp.webSocketInfo[2] == "1" ? 
					"messageServerUri\"\\:\"(ws.+?)\"" : "uri\"\\:\"(ws.+?)\"");
			this.thread = util.getRegGroup(message, "threadId\":\"(.+?)\"");
			this.rm = rm;
			this.rfu = rfu;
			this.userId = userId;
			this.form = form;
			this.openTime = openTime;
			this.recFolderFile = recFolderFile[1];
			this.lvid = lvid;
			this.container = container;
			this.isGetXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.isGetCommentXmlInfo = bool.Parse(rm.cfg.get("IsgetcommentXmlInfo"));
			this.programType = programType;
			this._openTime = _openTime;
			this.rp = rp;
			this.startSecond = startSecond;
			//this.tsConfig = tsConfig;
			this.isVposStartTime = isVposStartTime;
			this.isRtmp = isRtmp;
			this.rr = rr;
			isConvertSpace = bool.Parse(rm.cfg.get("IsCommentConvertSpace"));
			this.roomName = roomName;
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
			        if (rp.isRtmp || rp.firstSegmentSecond != -1) break;
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
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Version", "13"));
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits"));
				
				//wsc = new WebSocket(msUri,  "", null, header, "", "", WebSocketVersion.Rfc6455);
				//wsc = new WebSocket(uri, "", null, header, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36", "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12);
				wsc = new WebSocket(uri, "", null, header, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36", "", WebSocketVersion.Rfc6455, null, SslProtocols.None);
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
			
			_gotMinTime = gotMinTime;
			_gotMinXml = new String[]{gotMinXml[0], gotMinXml[1]};
			
			var req = getReq("-1000");
			wsc.Send(req);
			util.debugWriteLine("ms open b " + req);
			
			if (rm.rfu != rfu) {
				//stopRecording();
//				wsc.Close();
				return;
			}			
			

		}
		
		private void onWscClose(object sender, EventArgs e) {
			util.debugWriteLine("ms tscg onclose " + lastLastRes + " " + gotCommentList.Count);
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
			gotCommentList = new List<TimeShiftCommentGetter.GotCommentInfo>();
			gotCommentListBuf = new List<TimeShiftCommentGetter.GotCommentInfo>();
			gotMinTime = util.getUnixTime();
			gotMinXml = new string[2];
			_gotMinTime = util.getUnixTime();
			_gotMinXml = new string[2];
			gotCount = 0;
		}
		
		
		
		private void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
			var eMessage = isConvertSpace ? util.getOkSJisOut(e.Message, " ") : e.Message;
			
			try {
				if (rm.rfu != rfu || !isRetry) {
					try {
						if (wsc != null) wsc.Close();
					} catch (Exception ee) {
						util.debugWriteLine("wsc message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
					}
					//stopRecording();
					util.debugWriteLine("tigau rfu comment" + eMessage + " " + isRetry);
					return;
				}
				
				
				var xml = JsonConvert.DeserializeXNode(eMessage);
				var chatinfo = new namaichi.info.ChatInfo(xml);
				
				XDocument chatXml;
				var vposStartTime = (isVposStartTime) ? (long)rp.firstSegmentSecond : 0;
				
				if (isRtmp) {
					chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
				} else {
					if (programType == "official") {
						chatXml = chatinfo.getFormatXml(0, true, vposStartTime);
	//					chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
					} else {
						chatXml = chatinfo.getFormatXml(openTime + vposStartTime);
	
					}
				}
				if (isGetCommentXmlInfo && chatinfo.no == -1) 
					chatXml.Root.Add(new XAttribute("no", "0"));
				
	//			else chatXml = chatinfo.getFormatXml(serverTime);
	//			util.debugWriteLine("xml " + chatXml.ToString());
				
				if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
						chatinfo.premium == "3")) return;
				if (chatinfo.root == "ping" && chatinfo.contents.IndexOf("rf:") > -1) {
					//var addList = gotCommentListBuf.Where((a) => gotCommentList.IndexOf(a) == -1);
					gotCommentList.AddRange(gotCommentListBuf);
					gotCommentListBuf = new List<GotCommentInfo>();
					gotMinTime = _gotMinTime;
					gotMinXml = _gotMinXml;
					wsc.Close();
				}
				if (chatinfo.root != "chat" && chatinfo.root != "thread") return;
				
				if (chatinfo.root == "thread") {
	//				serverTime = chatinfo.serverTime;
					ticket = chatinfo.ticket;
	//				lastLastRes = (chatinfo.lastRes == null) ? 0 : int.Parse(chatinfo.lastRes);
					//lastLastRes = (chatinfo.lastRes != null) ? int.Parse(chatinfo.lastRes) : 0;
					if (chatinfo.lastRes == null) chatinfo.lastRes = "0";
					if (eMessage.IndexOf("resultcode\":0") == -1) {
						//((WebSocket)(sender)).Close();
						return;
					}
					if (chatinfo.lastRes != null)
						lastLastRes = int.Parse(chatinfo.lastRes);
					util.debugWriteLine("thread " + eMessage);
				}
	//			util.debugWriteLine(chatXml.ToString());
	//			util.debugWriteLine(gotMinXml[1]);
	
				
				if (chatXml.ToString().Equals(_gotMinXml[1])) {
					isSave = false;
					//var addList = gotCommentListBuf.Where((a) => gotCommentList.IndexOf(a) == -1);
					gotCommentList.AddRange(gotCommentListBuf);
					gotCommentListBuf = new List<GotCommentInfo>();
					gotMinTime = _gotMinTime;
					gotMinXml = _gotMinXml;
				}
				if (!isSave) return;
				if (chatinfo.root == "chat" && chatinfo.date < _gotMinTime) {
					_gotMinTime = chatinfo.date;
					_gotMinXml[1] = _gotMinXml[0];
					_gotMinXml[0] = chatXml.ToString();
				}
				
	
				try {
	//				if (commentSW != null) {
						string s;
						if (isGetXml) {
							s = chatXml.ToString();
						} else {
							var vposReplaced = Regex.Replace(eMessage, 
				            	"\"vpos\"\\:(\\d+)", 
				            	"\"vpos\":" + chatinfo.vpos + "");
							s = vposReplaced;
						}
						
			            if (chatinfo.root == "thread") {
							if (threadLine == null) {
								threadLine = s;
								if (!rm.isPlayOnlyMode)
									form.addLogText("アリーナ席に" + chatinfo.lastRes + "件ぐらいのコメントが見つかりました(追い出しコメント含む)");
							}
			            } else {
	//		            	commentSW.WriteLine(s + "}>");
	//		            	commentSW.Flush();
	//		            	gotCount++;
	//		            	if (gotCount % 2000 == 0) form.addLogText(gotCount + "件のコメントを保存しました");
							gotCommentListBuf.Add(new GotCommentInfo(s, chatinfo.no, chatinfo.date, chatinfo.vpos));
							gotCount++;
			            	
							if (gotCount % 2000 == 0 && !rm.isPlayOnlyMode) {
								form.addLogText(gotCount + "件のコメントを保存しました");
								gotCommentList = gotCommentList.Distinct().ToList();
							}
			            }
	//				}
	           
				} catch (Exception ee) {util.debugWriteLine(ee.Message + " " + ee.StackTrace);}
				
				if (eMessage.IndexOf("rf:") > -1) {
					((WebSocket)(sender)).Close();
				}
				//if (!isTimeShift)
	//				addDisplayComment(chatinfo);
			} catch (Exception eee) {
				util.debugWriteLine(eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);
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
		private string getReq(string resfrom) {
//			var when = (isSave) ? lastGetChatTime.ToString() : openTime.ToString();
			when = gotMinTime + 10;
			//thread += "store";
			var ret = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + thread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + resfrom + ",\"with_global\":1,\"scores\":1,\"nicoru\":0,\"waybackkey\":\"" + waybackKey + "\",\"when\":" + when + "}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			util.debugWriteLine("tscg " + ret);
			return ret;
		}
		public void setIsRetry(bool b) {
			isRetry = b;
		}
		private void endProcess() {
			var isWrite = (rm.cfg.get("IsgetComment") == "true" && !rm.isPlayOnlyMode && !rp.isChase);
			if (isWrite)
				form.addLogText("コメントの後処理を開始します");
			
			gotCommentList = gotCommentList.Distinct().ToList();
			//foreach (var gci in gotCommentList)
			//	if (unique.IndexOf(gci) == -1) unique.Add(gci);
			//gotCommentList = unique;
			
			/*
			try {
				if (commentSW != null) commentSW.Close();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			*/
			util.debugWriteLine("end proccess gotCommentList count " + gotCommentList.Count);
//			string[] chats;
//			using(var r = new StreamReader(fileName)) {
//				chats = r.ReadToEnd().Split(new string[]{"}>\r\n"}, StringSplitOptions.RemoveEmptyEntries);
//		    }
			//var isXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			while (isRtmp && (rr == null || rr.fileNameList == null)) {
				Thread.Sleep(1000);
			}
				
			var keys = new List<int>();
			foreach (var c in gotCommentList) {
//				util.debugWriteLine(c);
//				util.debugWriteLine(util.getRegGroup(c, "date.+?(\\d+)"));
				//var date = int.Parse(util.getRegGroup(c, "date.+?(\\d+)"));
				var date = c.date;
				keys.Add(date);
			}
			
			//Array.Sort(keys.ToArray(), chats);
			
			gotCommentList.Sort(new Comparison<GotCommentInfo>(commentListCompare));
			var chats = gotCommentList.Select(x => x.comment).ToArray();
			rp.gotTsCommentList = chats;
			
			if (rp.isChase) {
				while (rp.chaseCommentBuf.Count == 0 
				       && rm.rfu == rfu) Thread.Sleep(1000);
				rp.chaseCommentSum();
			}
			if (!isWrite) return;
			
			var fileNum = (quePosTimeList != null) ? quePosTimeList.Length : 1;
			for (int j = 0; j < fileNum; j++) {
				if (j != 0) recFolderFile = incrementRecFolderFile(recFolderFile);
				
				fileName = util.getOkCommentFileName(rm.cfg, recFolderFile, lvid, true, isRtmp);
				if (isRtmp) {
					//fileName = getTsRecordIndexRecFolderFile(fileName, j + 1);
					fileName = rr.fileNameList[j] + 
						((rm.cfg.get("IsgetcommentXml") == "true") ? ".xml" : ".json");
				}
				
				if (rm.cfg.get("fileNameType") == "10")
					fileName = fileName.Replace("{w}", rp.visitCount.ToString()).Replace("{c}", rp.commentCount.ToString());
				using (var w = new StreamWriter(fileName + "_", false, System.Text.Encoding.UTF8)) {
					if (isGetXml) {
						w.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
						if (!isGetCommentXmlInfo) 
							w.WriteLine("<packet>");
						else {
							writeXmlStreamInfo(w);
						}
					    
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
							if (j == fileNum - 1 || vpos < quePosTimeList[j + 1]) {
								var oriVposPart = util.getRegGroup(chats[i], "(vpos.+?-*\\d+)");
								var newVposPart = oriVposPart.Replace(vpos.ToString(), (vpos - quePosTimeList[j]).ToString());
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
		private string getTsRecordIndexRecFolderFile(string recFolderFile, int recordIndex) {
			var r = new Regex("(\\d+).xml$");
			var m = r.Match(recFolderFile);
			if (m == null || m.Length <= 0) return recFolderFile;
			//var _new = (int.Parse(m.Groups[1].Value) + 1).ToString();
			var _new = recordIndex.ToString();
			
			
			return r.Replace(recFolderFile, _new + ".xml");
		}
		private int commentListCompare(GotCommentInfo x, GotCommentInfo y) {
			if (x.no >= 0 && y.no >= 0 && x.no != y.no) {
				return x.no - y.no;
			}
			if (x.date != y.date) 
				return x.date - y.date;
			
			if (x.vpos != y.vpos)
				return x.vpos.CompareTo(y.vpos);
			return x.comment.CompareTo(y.comment);
		}
		private void writeXmlStreamInfo(StreamWriter w) {
			var startTime = openTime;
			var vposStartTime = (isVposStartTime) ? (long)rp.firstSegmentSecond : 0;
			if (programType == "official") {
				startTime = _openTime + vposStartTime;
			} else {
				startTime = openTime + vposStartTime;
			}
			w.WriteLine("<packet xmlns=\"http://posite-c.jp/niconamacommentviewer/commentlog/\">");
			w.WriteLine("<RoomLabel>" + roomName + "</RoomLabel>");
			w.WriteLine("<StartTime>" + startTime + "</StartTime>");
		}
		class GotCommentInfo {
			public string comment = null;
			public int no;
			public int date;
			public long vpos;
			public GotCommentInfo(string comment, int no, int date, long vpos) {
				this.comment = comment;
				this.no = no;
				this.date = date;
				this.vpos = vpos;
			}
			public override bool Equals(object gci) {
				if (gci == null) return false;
				return comment.Equals(((GotCommentInfo)gci).comment);
			}
			public override int GetHashCode() {
				return comment.GetHashCode();
			}
		}
	}
}
