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
	public class TimeShiftCommentGetter : ITimeShiftCommentGetter
	{
		string uri;
		string thread;
		string uriStore;
		string threadStore;
		string userId;
		MainForm form;
		RecordingManager rm;
		RecordFromUrl rfu;
		//long openTime = 0;
		//long _openTime = 0;
		//string recFolderFile;
		string lvid;
		//string programType;
		string roomName = "";
		//int startSecond;
		CookieContainer container;
		string ticket;
		string waybackKey;
		//int gotMinTime;
		//string[] gotMinXml = new string[2];
		//int _gotMinTime;
		//string[] _gotMinXml = new string[2];
		int when = 0;
		int lastLastRes = int.MaxValue;
		string threadLine;
		bool isGetXml = true;
		bool isGetCommentXmlInfo = false;
		//public List<GotCommentInfo> gotCommentList = new List<GotCommentInfo>();
		//public List<GotCommentInfo> gotCommentListBuf = new List<GotCommentInfo>();
		
		//private StreamWriter commentSW;
		//private string fileName;
		WebSocket wsc = null;
		bool isSave = true;
		//bool isRetry = true;
		//public bool isEnd = false;
		//public IRecorderProcess rp;
		//private bool isLog = true;
		//private RecordStateSetter rss;
		
		//int gotCount = 0;
		public string[] sortedComments;
//		public TimeShiftConfig tsConfig;
		//private bool isVposStartTime;
		private bool isRtmp;
		private int[] quePosTimeList;
		private RtmpRecorder rr;
		private bool isConvertSpace;
		private string commentConvertStr = null;
		//private TimeShiftConfig tsConfig = null;
		//private bool isStore;
		private bool isNormalizeComment;
		//private bool isReachStartTimeSave = false;
		//private string lastRealTimeComment = null;
		private WebSocketCurlWSC wsCurl = null;
		bool isUseCurlWs = false;
		
		public TimeShiftCommentGetter(string uri, string thread,
				string uriStore, string threadStore,        
				string userId, RecordingManager rm, 
				RecordFromUrl rfu, MainForm form,
				RecordInfo ri,
				string lvid, CookieContainer container, 
				IRecorderProcess rp, 
				bool isRtmp,
				RtmpRecorder rr, 
				string roomName, TimeShiftConfig tsConfig,
				string lastRealTimeComment = null, bool isLog = true,
				bool isStore = false, string _uri = null, string _thread = null
				)
		{
			this.uri = uri;
			this.thread = thread;

			this.rm = rm;
			this.rfu = rfu;
			this.userId = userId;
			this.form = form;
			//this.openTime = openTime;
			this.recFolderFile = ri.recFolderFile[1];
			this.lvid = lvid;
			this.container = container;
			this.isGetXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.isGetCommentXmlInfo = bool.Parse(rm.cfg.get("IsgetcommentXmlInfo"));
			//this.programType = programType;
			//this._openTime = _openTime;
			this.rp = rp;
			//this.startSecond = startSecond;
			//this.tsConfig = tsConfig;
			this.isVposStartTime = (ri.isRtmp) ? false : tsConfig.isVposStartTime;
			this.isRtmp = isRtmp;
			this.rr = rr;
			isConvertSpace = bool.Parse(rm.cfg.get("IsCommentConvertSpace"));
			commentConvertStr = rm.cfg.get("commentConvertStr");
			isNormalizeComment = bool.Parse(rm.cfg.get("IsNormalizeComment"));
			this.roomName = roomName;
			this.tsConfig = tsConfig;
			
			this.uriStore = uriStore;
			this.threadStore = threadStore;
			this.lastRealTimeComment = lastRealTimeComment;
			this.isLog = isLog;
			this.ri = ri;
			
			this.isStore = isStore;
			if (_uri != null) {
				uri = _uri;
				thread = _thread;
			}
			isUseCurlWs = false; //true || !util.isWin10Later();
		}
		override public void save() {
			
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
			
			when = util.getUnixTime() + (int)rp.jisa.TotalSeconds + 60 * 60 * 50;//(int)openTime
			gotMinTime = when;
	        
			
			Task.Run(() => {
			    while (true) {
			         		if (rp.ri.isRtmp || rp.firstSegmentSecond != -1 || !((WebSocketRecorder)rp).IsRetry) break;
					Thread.Sleep(500);
				}
	         	if (!((WebSocketRecorder)rp).IsRetry) {
	         		isEnd = true;
	         		util.debugWriteLine("tscg end rp.IsrRtry");
	         		#if DEBUG
	         			form.addLogText("tscg end rp.IsrRtry");
			        #endif
	         		return;
	         	}
			    util.debugWriteLine("ms start tscg " + rp.IsRetry + " " + isLog);
				connect();
			});
			
			
		}
		bool connect() {
			util.debugWriteLine("connect tscg ms");
			isSave = true;
			try {
				waybackKey = ""; //getWaybackKey();
				
				var header =  new List<KeyValuePair<string, string>>();
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Version", "13"));
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits"));
				
				if (isUseCurlWs) {
					var _wsi = new string[]{uri, null};
		    		wsCurl = new WebSocketCurlWSCTS(rm, this, _wsi);
		    		Task.Factory.StartNew(() => {
		    			wsCurl.connect();
						onWscClose(null, null);
					}, TaskCreationOptions.LongRunning);
				} else {
					wsc = new WebSocket(uri, "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null,  SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
					wsc.Opened += onWscOpen;
					wsc.Closed += onWscClose;
					wsc.MessageReceived += onWscMessageReceive;
					wsc.Error += onWscError;
					
			        util.debugWriteLine("connect tscg ms uri " + uri);
			        wsc.Open();
			        
			        try {
						Thread.Sleep(5000);
						if (wsc != null && wsc.State == WebSocketState.Connecting) {
							util.debugWriteLine("tscg wsc connect 5 seconds close");
							try {
								wsc.Close();
							} catch (Exception e) {
								util.debugWriteLine("tscg connect timeout ws exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
							}
						}
						
					} catch (Exception ee) {
						util.debugWriteLine("tscg ws connect exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
						return false;
					}
				}
		        return true;
			} catch (Exception e) {
				util.debugWriteLine("tscg connect exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				return false;
			}
		}
		public void onWscOpen(object sender, EventArgs e) {
			util.debugWriteLine("ms open tscg a");
			try {
				_gotMinTime = gotMinTime;
				_gotMinXml = new String[]{gotMinXml[0], gotMinXml[1]};
				
				var req = getReq("-1000");
				if (!isUseCurlWs) {
					if (wsc != null)
						wsc.Send(req);
				} else if (wsCurl != null) wsCurl.wsSend(req);
					
				util.debugWriteLine("ms open b " + req);
				
				if (rm.rfu != rfu) {
					return;
				}			
			} catch (Exception ee) {
				util.debugWriteLine("onwscopen " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}

		}
		public void onWscClose(object sender, EventArgs e) {
			util.debugWriteLine("ms tscg onclose " + lastLastRes + " " + gotCommentList.Count);
			//closeWscProcess();
			wsc = null;
			try {
				if ((lastLastRes < 900 || isReachStartTimeSave) && !isEnd) {
					endProcess();
					isEnd = true;
//					rm.form.addLogText("コメントの保存を完了しました");
					return;
				}
				#if DEBUG
					if (isEnd) form.addLogText("コメント処理.");
				#endif
				
				if ((rm.rfu == rfu && lastRealTimeComment == null) && isRetry && !isEnd) {
					while (true) {
						try {
							if (!connect()) {
								#if DEBUG
									form.addLogText("tscg　再試行");
								#endif
								Thread.Sleep(1000);
								continue;
							}
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
			util.debugWriteLine("ms tscg onerror " + e.Exception.Message + e.Exception.Source + e.Exception.StackTrace + e.Exception.TargetSite);
			/*
			gotCommentList = new List<TimeShiftCommentGetter.GotCommentInfo>();
			gotCommentListBuf = new List<TimeShiftCommentGetter.GotCommentInfo>();
			gotMinTime = util.getUnixTime();
			gotMinXml = new string[2];
			_gotMinTime = util.getUnixTime();
			_gotMinXml = new string[2];
			gotCount = 0;
			*/
		}
		
		
		
		public void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
			var eMessage = isConvertSpace ? util.getOkSJisOut(e.Message, commentConvertStr) : e.Message;
			//if (isNormalizeComment) eMessage = eMessage.Replace("\"premium\":24", "\"premium\":0");
			try {
				if ((rm.rfu != rfu && lastRealTimeComment == null) || !isRetry) {
					try {
						if (!isUseCurlWs)
							if (wsc != null) wsc.Close();
						else if (wsCurl != null) wsCurl.stop();
						
					} catch (Exception ee) {
						util.debugWriteLine("wsc message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
					}
					//stopRecording();
					util.debugWriteLine("tigau rfu comment" + eMessage + " " + isRetry);
					return;
				}
				
				XDocument xml = null;
				try {
					xml = JsonConvert.DeserializeXNode(eMessage);
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite + eMessage);
					try {
						var content = util.getRegGroup(eMessage, "\"content\":\"(.+?)\"}");
						if (content == null) {
							util.debugWriteLine("error mes " + eMessage);
							return;
						}
						if (content.IndexOf("\\") > -1)
							eMessage = eMessage.Replace("\"content\":\"" + content + "\"}", "\"content\":\"" + content.Replace("\\", "\\\\") + "\"}");
						xml = JsonConvert.DeserializeXNode(eMessage);
					} catch (Exception eee) {
						util.debugWriteLine(eee.Message + eee.Source + eee.StackTrace);
						//rm.form.addLogText("error:" + eMessage + " " + eee.Message + eee.Source + " " + ee.Message + ee.Source);
						return;
					}
				}
				var chatinfo = new ChatInfo(xml);
				
				XDocument chatXml;
				var vposStartTime = (isVposStartTime) ? (long)rp.firstSegmentSecond : 0;
				
				if (isRtmp) {
					chatXml = chatinfo.getFormatXml(ri.si._openTime + vposStartTime);
				} else {
					if (lastRealTimeComment == null) {
						if (ri.si.type == "official") {
							chatXml = chatinfo.getFormatXml(0, true, vposStartTime);
		//					chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
						} else {
							chatXml = chatinfo.getFormatXml(ri.si.openTime + vposStartTime);
		
						}
					} else {
						if (ri.si.type == "official") {
							chatXml = chatinfo.getFormatXml(0, true, rp.serverTime - ri.si._openTime);
						//} else chatXml = chatinfo.getFormatXml(serverTime);
						} else {
							//chatXml = chatinfo.getFormatXml(0, true, serverTime - _openTime);
							chatXml = chatinfo.getFormatXml(rp.serverTime);
						}
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
					if (!isUseCurlWs)
						wsc.Close();
					else {
						wsCurl.stop();
						onWscClose(null, null);
					}
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
				if (chatinfo.root == "chat" && chatinfo.date != 0 && chatinfo.date < _gotMinTime) {
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
								if (!rfu.isPlayOnlyMode && isLog)
									form.addLogText("アリーナ席に" + chatinfo.lastRes + "件ぐらいの" + (isStore ? "ストア" : "") + "コメントが見つかりました(追い出しコメント含む)");
							}
			            } else {
							var isMeetStartTimeSave = !tsConfig.isAfterStartTimeComment ||
	              					chatinfo.date > ri.si._openTime + tsConfig.timeSeconds - 10;
							var isMeetEndTimeSave = !tsConfig.isBeforeEndTimeComment || 
										tsConfig.endTimeSeconds == 0 || 
		  								chatinfo.date < ri.si._openTime + tsConfig.endTimeSeconds + 10;
							
							if (!isMeetStartTimeSave || s == lastRealTimeComment)
								isReachStartTimeSave = true;
								
							var isComSave = isMeetStartTimeSave && isMeetEndTimeSave;
	    					if (isComSave) {
								s = util.getReplacedComment(s, rp.commentReplaceList);
								gotCommentListBuf.Add(new GotCommentInfo(s, chatinfo.no, chatinfo.date, chatinfo.vpos));
								gotCount++;
				            	
								if (gotCount % 2000 == 0 && !rfu.isPlayOnlyMode) {
									if (isLog)
										form.addLogText(gotCount + "件のコメントを保存しました");
									gotCommentList = gotCommentList.Distinct().ToList();
								}
							}
			            }
	//				}
	           
				} catch (Exception ee) {util.debugWriteLine(ee.Message + " " + ee.StackTrace);}
				
				if (eMessage.IndexOf("rf:") > -1) {
					if (!isUseCurlWs)
						((WebSocket)(sender)).Close();
					else {
						wsCurl.stop();
						onWscClose(null, null);
					}
					
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
		/*
		override public void setIsRetry(bool b) {
			isRetry = b;
		}
		*/
		private void endProcess() {
			util.debugWriteLine("tscg end process");
			if (isStore) return;
			try {
			
				if (uriStore != null)
					saveStore();
				
				var isWrite = (!rfu.isPlayOnlyMode && !rp.ri.isChase && lastRealTimeComment == null);
				if (isWrite && isLog)
					form.addLogText("コメントの後処理を開始します");
				
				gotCommentList = gotCommentList.Distinct().ToList();
	
				util.debugWriteLine("end proccess gotCommentList count " + gotCommentList.Count);
	
				while (isRtmp && (rr == null || rr.fileNameList == null)) {
					Thread.Sleep(1000);
				}
				
				util.debugWriteLine("end proccess a");
				
				var keys = new List<int>();
				foreach (var c in gotCommentList) {
					var date = c.date;
					keys.Add(date);
				}
				
				util.debugWriteLine("end proccess b");
				
				gotCommentList.Sort(new Comparison<GotCommentInfo>(commentListCompare));
				var chats = gotCommentList.Select(x => x.comment).ToArray();
				rp.gotTsCommentList = chats;
				
				util.debugWriteLine("end proccess d");
				
				if (rp.ri.isChase && lastRealTimeComment == null && rp.chaseCommentBuf != null) {
					while (rp.chaseCommentBuf.Count == 0 
					       && rm.rfu == rfu) Thread.Sleep(1000);
					rp.chaseCommentSum();
				}
				
				util.debugWriteLine("end proccess c");
				
				if (!isWrite) return;
				
				var fileNum = (quePosTimeList != null) ? quePosTimeList.Length : 1;
				for (int j = 0; j < fileNum; j++) {
					if (j != 0) recFolderFile = incrementRecFolderFile(recFolderFile);
					
					var fileName = util.getOkCommentFileName(rm.cfg, recFolderFile, lvid, true, isRtmp);
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
						} else {
							w.WriteLine("[");
						}
						w.WriteLine(threadLine + ((!isGetXml && chats.Length != 0) ? "," : ""));
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
								w.WriteLine(chats[i] + ((!isGetXml && i != chats.Length - 1) ? "," : ""));
							}
							
						}
						if (isGetXml) {
							w.WriteLine("</packet>");
						} else {
							//w.BaseStream.Position -= 3;
							//w.WriteLine("");
							w.WriteLine("]");
						}
						w.Flush();
					}
					File.Delete(fileName);
					File.Move(fileName + "_", fileName);
				}
				if (isLog)
					form.addLogText("コメントの保存を完了しました");
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
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
		/*
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
		*/
		/*
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
		*/
		private void saveStore() {
			util.debugWriteLine("saveStore");
			var tscg = new TimeShiftCommentGetter(uri, thread,
					uriStore, threadStore,
					userId, rm, rfu, rm.form, ri, 
					lvid, container, 
					//programType, 
					rp,
					//startSecond, 
					//isVposStartTime, 
					isRtmp, rr, roomName, tsConfig, null, true, true, null, null);
			tscg.save();
				
			while (!tscg.isEnd && rm.rfu == rfu && isRetry && rp.IsRetry) {
				Thread.Sleep(500);
			}
			util.debugWriteLine("saveStore wait end " + tscg.gotCommentList.Count);
			gotCommentList.AddRange(tscg.gotCommentList);
		}
	}
	public class GotCommentInfo {
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
