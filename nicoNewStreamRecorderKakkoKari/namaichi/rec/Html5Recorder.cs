/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/04/15
 * Time: 0:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using namaichi.config;
using namaichi.info;
using namaichi;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Html5Recorder.
	/// </summary>
	
		
	public class Html5Recorder
	{
		public string url;
		private CookieContainer container;
		private string lvid;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private bool isTimeShift;
		private TimeShiftConfig timeShiftConfig;
		private string[] recFolderFileInfo;
		private bool isSub;
	
		public Html5Recorder(string url, CookieContainer container, 
				string lvid, RecordingManager rm, RecordFromUrl rfu,
				bool isTimeShift, bool isSub)
		{
			this.url = url;
			this.container = container;
			this.lvid = lvid;
			this.rm = rm;
			this.rfu = rfu;
			this.isTimeShift = isTimeShift;
			this.isSub = isSub;
		}
		public int record(string res, bool isRtmp) {
			
			
//			for (int i = 0; i < webSocketRecInfo.Length; i++)
//				util.debugWriteLine(webSocketRecInfo[i]);
//			for (int i = 0; i < recFolderFileInfo.Length; i++)
//				util.debugWriteLine(recFolderFileInfo[i]);
			
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			var ret = html5Record(res, isRtmp).Result;
			util.debugWriteLine("html5 rec ret " + ret);
			return ret;
		
		}
		/*
		private string getPageSource(string _url) {
			var req = (HttpWebRequest)WebRequest.Create(_url);
			req.CookieContainer = container;
			var res = (HttpWebResponse)req.GetResponse();
			var dataStream = res.GetResponseStream();
			var reader = new StreamReader(dataStream);
			string resStr = reader.ReadToEnd();
			
			return resStr;
			
		}
		*/
		public string[] getWebSocketInfo(string data) {
//			util.debugWriteLine(data);
			var wsUrl = util.getRegGroup(data, "\"webSocketUrl\":\"([\\d\\D]+?)\"");
			util.debugWriteLine("wsurl " + wsUrl + util.getMainSubStr(isSub));
			//var broadcastId = util.getRegGroup(wsUrl, "/(\\d+)\\?");
			var broadcastId = util.getRegGroup(data, "\"broadcastId\"\\:\"(\\d+)\"");
			util.debugWriteLine("broadcastid " + broadcastId + util.getMainSubStr(isSub));
			string request = "{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":true,\"priorStreamQuality\":\"normal\", \"isLowLatency\": false},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}";
//			string request = "{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":true,\"priorStreamQuality\":\"normal\"},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}";
			util.debugWriteLine("request " + request + util.getMainSubStr(isSub));
			return new string[]{wsUrl, request};
		}
		private string[] getHtml5RecFolderFileInfo(string data, string type) {
			string host, group, title, communityNum, userId;
			host = group = title = communityNum = userId = null;
			if (type == "official") {
				group = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
				
	//			if (host == null) host = "official";
				host = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.+?)\"");
	//			group = "蜈ｬ蠑冗函謾ｾ騾・;
				if (host == null) group = "official";
				if (util.getRegGroup(data, "(\"socialGroup\".\\{\\},)") != null) host = "公式生放送";
				title = util.getRegGroup(data, "\"title\"\\:\"(.+?)\",");
//				title = util.uniToOriginal(title);
				communityNum = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
				if (communityNum == null) communityNum = "official";
				userId = "official";
				
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum);
				if (host == null || group == null || title == null || communityNum == null) return null;
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum);
			} else {
				bool isAPI = false;
				if (isAPI) {
					var a = new System.Net.WebHeaderCollection();
					var apiRes = util.getPageSource(url + "/programinfo", ref a, container);
				
				} else {
					var isChannel = util.getRegGroup(data, "visualProviderType\":\"(channel)\",\"title\"") != null;
		//			host = util.getRegGroup(res, "provider......name.....(.*?)\\\\\"");
					group = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
		//			group = util.uniToOriginal(group);
		//			group = util.getRegGroup(res, "communityInfo.\".+?title.\"..\"(.+?).\"");
					host = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.+?)\"");
		//			System.out.println(group);
		//			host = util.uniToOriginal(host);
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\:\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\:\\\"(.*?)\\\"");
		//			title = util.getRegGroup(res, "\\\\\"programHeader\\\\\":\\{\\\\\"thumbnailUrl.+?\\\\\"title\\\\\":\\\\\"(.*?)\\\\\"");
					title = util.getRegGroup(data, "visualProviderType\":\"(community|channel)\",\"title\":\"(.+?)\",", 2);
		//			communityNum = util.getRegGroup(res, "socialGroup: \\{[\\s\\S]*registrationUrl: \"http://com.nicovideo.jp/motion/(.*?)\\?");
					communityNum = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
		//			community = util.getRegGroup(res, "socialGroup\\:)");
					userId = (isChannel) ? "channel" : (util.getRegGroup(data, "supplier\":{\"name\".+?pageUrl\":\"http://www.nicovideo.jp/user/(\\d+?)\""));
					util.debugWriteLine("userid " + userId);
		
					util.debugWriteLine("title " + title);
					util.debugWriteLine("community " + communityNum);
		//			community = util.getRegGroup(res, "socialGr(oup:)");
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\\:\\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\\:\\\"(.*?)\\\"");
					//  ,\"programHeader\":{\"thumbnailUrl\":\"http:\/\/icon.nimg.jp\/community\/s\/123\/co1231728.jpg?1373210036\",\"title\":\"\u56F2\u7881\",\"provider
		//			title = util.uniToOriginal(title);
					
					util.debugWriteLine(host + " " + group + " " + title + " " + communityNum + " userid " + userId);
					if (host == null || group == null || title == null || communityNum == null || userId == null) return null;
				}
			}
			if (communityNum != null) rm.communityNum = communityNum;
			return new string[]{host, group, title, lvid, communityNum, userId};

		}
		async private Task<int> html5Record(string res, bool isRtmp) {
			//webSocketInfo 0-wsUrl 1-request
			//recFolderFileInfo host, group, title, lvid, communityNum
			//return 0-end stream 1-stop
			
	//		List<Cookie> cookies = context.getCookieStore().getCookies();
	//		for (Cookie cookie : cookies) System.out.println(cookie.getName() + " " + cookie.getValue());
			

	//		ExecutorService exec = Executors.newFixedThreadPool(1);
			
			
	
			string[] webSocketRecInfo;
			recFolderFileInfo = null;
			string[] recFolderFile = null;
				
//			Task displayTask = null;
//			var pageType = util.getPageType(res);
//			util.debugWriteLine("pagetype " + pageType);
				
			var lastSegmentNo = -1;
			
			var isNoPermission = false;
			while(rm.rfu == rfu) {
				var type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
				var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
				var pageType = util.getPageType(res);
				util.debugWriteLine("pagetype " + pageType + util.getMainSubStr(isSub));
				
				if (data == null || (pageType != 0 && pageType != 7)) {
					//processType 0-ok 1-retry 2-放送終了 3-その他の理由の終了
					var processType = processFromPageType(pageType);
					util.debugWriteLine("processType " + processType + util.getMainSubStr(isSub));
					//if (processType == 0 || processType == 1) continue;
					if (processType == 2) return 3;
//					if (processType == 3) return 0;
					
					System.Threading.Thread.Sleep(3000);
					
					res = getPageSourceFromNewCookie();
					
					continue;
				}
				
				
				data = System.Web.HttpUtility.HtmlDecode(data);
				long openTime = 0;
				if (data == null || 
				    !long.TryParse(util.getRegGroup(data, "\"beginTime\":(\\d+)"), out openTime))
						return 3;
	//				var openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"));
				long endTime = 0;
				if (data == null || 
				    !long.TryParse(util.getRegGroup(data, "\"endTime\":(\\d+)"), out endTime))
						return 3;				
				var programTime = util.getUnixToDatetime(endTime) - util.getUnixToDatetime(openTime);
				long _openTime = 0;
				if (data == null || 
				    !long.TryParse(util.getRegGroup(data, "\"openTime\":(\\d+)"), out _openTime))
						return 3;
				long serverTime = 0;
				if (data == null || 
				    !long.TryParse(util.getRegGroup(data, "\"serverTime\":(\\d+)"), out serverTime))
						return 3;
				var jisa = util.getUnixToDatetime(serverTime / 1000) - DateTime.Now;
	//			util.debugWriteLine(data);
				
				//0-wsUrl 1-request
				webSocketRecInfo = getWebSocketInfo(data);
				util.debugWriteLine("websocketrecinfo " + webSocketRecInfo);
				if (webSocketRecInfo == null) continue;
				
				util.debugWriteLine("isnopermission " + isNoPermission);
//				if (isNoPermission) webSocketRecInfo[1] = webSocketRecInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true");
				recFolderFileInfo = getHtml5RecFolderFileInfo(data, type);
				
				if (!isSub) {
					//timeshift option
					timeShiftConfig = null;
					if (isTimeShift) {
//						if (rm.ri != null) timeShiftConfig = rm.ri.tsConfig;
						if (rm.argTsConfig != null) {
							timeShiftConfig = getReadyArgTsConfig(rm.argTsConfig.clone(), recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], openTime);
						} else {
							timeShiftConfig = getTimeShiftConfig(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, openTime);
							if (timeShiftConfig == null) return 2;
							rm.cfg.set("IsUrlList", timeShiftConfig.isOutputUrlList.ToString().ToLower());
							rm.cfg.set("openUrlListCommand", timeShiftConfig.openListCommand);
						}
					}
				
					if (!rm.isPlayOnlyMode) {
						util.debugWriteLine("rm.rfu " + rm.rfu.GetHashCode() + " rfu " + rfu.GetHashCode());
						if (recFolderFile == null)
							recFolderFile = getRecFilePath(openTime);
						if (recFolderFile == null || recFolderFile[0] == null) {
							//パスが長すぎ
							rm.form.addLogText("パスに問題があります。 " + recFolderFile[1]);
							util.debugWriteLine("too long path? " + recFolderFile[1]);
							return 2;
						}
					} else recFolderFile = new string[]{"", "", ""};
				
					//display set
					var b = new RecordStateSetter(rm.form, rm, rfu, isTimeShift, false, recFolderFile, rm.isPlayOnlyMode);
					Task.Run(() => {
					       	b.set(data, type, recFolderFileInfo);
						});
					
					//hosoInfo
					if (rm.cfg.get("IshosoInfo") == "true" && !rm.isPlayOnlyMode)
						Task.Run(() => {b.writeHosoInfo();});
					
					util.debugWriteLine("form disposed" + rm.form.IsDisposed);
					util.debugWriteLine("recfolderfile test " + recFolderFileInfo);
					
					var fileName = System.IO.Path.GetFileName(recFolderFile[1]);
					rm.form.setTitle(fileName);
					
					
	//				if (recFolderFile == null) continue;
					
					for (int i = 0; i < recFolderFile.Length; i++)
						util.debugWriteLine("recd " + i + " " + recFolderFileInfo[i]);
					
				} else {
					recFolderFile = new string[]{"", "", ""};
				}
				
				var userId = util.getRegGroup(res, "\"user\"\\:\\{\"user_id\"\\:(.+?),");
				var isPremium = res.IndexOf("\"member_status\":\"premium\"") > -1;
				var wsr = new WebSocketRecorder(webSocketRecInfo, container, recFolderFile, rm, rfu, this, openTime, lastSegmentNo, isTimeShift, lvid, timeShiftConfig, userId, isPremium, programTime, type, _openTime, isSub, isRtmp);
				rm.wsr = wsr;
				try {
					isNoPermission = wsr.start();
					rm.wsr = null;
					if (wsr.isEndProgram)
						//return (isTimeShift) ? 1 : 3;
						return 3;
//					if (isTimeShift && wsr.isEndProgram) 
//						return 1;

						
				} catch (Exception e) {
					util.debugWriteLine("wsr start exception " + e.Message + e.StackTrace);
				}
				
//				System.Threading.Thread.Sleep(2000);
				
				util.debugWriteLine(rm.rfu + " " + rfu + " " + (rm.rfu == rfu));
				if (rm.rfu != rfu) break;
				
				res = getPageSourceFromNewCookie();
				
				
				
			}
			return 1;
		}
		private string getPageSourceFromNewCookie() {
			CookieGetter _cg = null;
			Task<CookieContainer[]> _cgtask = null;
			while (rm.rfu == rfu) {
				try {
					_cg = new CookieGetter(rm.cfg);
					_cgtask = _cg.getHtml5RecordCookie(url, isSub);
					_cgtask.Wait();
					
					if (_cgtask == null || _cgtask.Result == null) {
						System.Threading.Thread.Sleep(3000);
						continue;
					}
					
					//container = _cgtask.Result[(isSub) ? 1 : 0];
					container = _cgtask.Result[0];
					return _cg.pageSource;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
				} 
				
	//			var _c = new System.Net.WebHeaderCollection();
	//			return util.getPageSource(url, ref _c, container);
			}
			return "";
		}
		/*
		public bool isAliveStream() {
			var a = new System.Net.WebHeaderCollection();
			string res = util.getPageSource(url, ref a, container);
				
//			string res = getPageSource(url);
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			data = System.Web.HttpUtility.HtmlDecode(data);
			string[] info = getWebSocketInfo(data);
			return (info == null) ? false : true; 
		}
		*/
		private int processFromPageType(int pageType) {
			//ret 0-ok 1-retry 2-放送終了 3-その他の理由の終了
			if (pageType == 0 || pageType == 7) {
				return 0;
			} else if (pageType == 1) {
				rm.form.addLogText("満員です。" + util.getMainSubStr(isSub, true));
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
				
			} else if (pageType == 5) {
				rm.form.addLogText("接続エラー。10秒後リトライします。" + util.getMainSubStr(isSub, true));
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
			} else if (pageType == 6) {
//				rm.form.addLogText("接続エラー。10秒後リトライします。");
				System.Threading.Thread.Sleep(3000);
				return 1;
				
			} else if (pageType == 4) {
				rm.form.addLogText("require_community_menber" + util.getMainSubStr(isSub, true));
				if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
					if (rm.form.IsDisposed) return 2;
					try {
			        	rm.form.Invoke((MethodInvoker)delegate() {
			       			MessageBox.Show("コミュニティに入る必要があります：\nrequire_community_menber/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
						});
					} catch (Exception e) {
			       		util.showException(e);
			       	}
				}
				if (bool.Parse(rm.cfg.get("IsfailExit")) && util.isShowWindow) {
					rm.rfu = null;
					try {
						rm.form.Invoke((MethodInvoker)delegate() {
							try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						               }

						});
					} catch (Exception e) {
			       		util.showException(e);
			       	}
					
				}
				return 3;
				
			} else {
				var mes = "";
				if (pageType == 2) mes = "この放送は終了しています。" + util.getMainSubStr(isSub, true);
				if (pageType == 3) mes = "この放送は終了しています。" + util.getMainSubStr(isSub, true);
				rm.form.addLogText(mes);
				util.debugWriteLine("pagetype " + pageType + " end");
				
				if (bool.Parse(rm.cfg.get("IsdeleteExit")) && util.isShowWindow) {
					rm.rfu = null;
					try {
						rm.form.Invoke((MethodInvoker)delegate() {
			       			try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	       		}
						});
					} catch (Exception e) {
			       		util.showException(e);
			       	}
					
				}
				return 2;
				//var nh5r = new NotHtml5Recorder(url, container, lvid, rm, this);
				//nh5r.record(res);
			}
			
		}
		private TimeShiftConfig getTimeShiftConfig(string host, 
			string group, string title, string lvId, string communityNum, 
			string userId, config.config cfg, long _openTime) {
			var segmentSaveType = cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, cfg, _openTime);
			util.debugWriteLine("timeshift lastfile " + lastFile);
			string[] lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType);
			if (lastFileTime == null)
				util.debugWriteLine("timeshift lastfiletime " + 
				                    ((lastFileTime == null) ? "null" : string.Join(" ", lastFileTime)));
			
			try {
				var o = new TimeShiftOptionForm(lastFileTime, segmentSaveType, rm.cfg);
				
				try {
					rm.form.Invoke((MethodInvoker)delegate() {
		       		       	try {
				        	    o.ShowDialog(rm.form);
		       		       	} catch (Exception e) {
		       		       		util.debugWriteLine("timeshift option form invoke " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       		       	}
					});
				} catch (Exception e) {
					util.debugWriteLine("timeshift option form invoke try " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				}
				
				//if (o.ret == null) return null;
				return o.ret;
	        } catch (Exception ee) {
        		util.debugWriteLine(ee.Message + " " + ee.StackTrace);
	        }
			return null;
		}
		public string[] getRecFilePath(long openTime) {
			util.debugWriteLine(openTime + " c " + recFolderFileInfo[0] + " b " + " a " + timeShiftConfig);
			return util.getRecFolderFilePath(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, isTimeShift, timeShiftConfig, openTime);
		}
		private TimeShiftConfig getReadyArgTsConfig(
				TimeShiftConfig _tsConfig, string host, 
				string group, string title, string lvId, 
				string communityNum, string userId, 
				long _openTime) {
			
			var segmentSaveType = rm.cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, rm.cfg, _openTime);
			util.debugWriteLine("timeshift lastfile " + lastFile + " host " + host + " title " + title);
			
			var lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType);
			if (lastFileTime == null) {
				_tsConfig.timeType = 0;
			} else {
				if (_tsConfig.timeType == 1)
					_tsConfig.timeSeconds = 
						int.Parse(lastFileTime[0]) * 3600 + 
						int.Parse(lastFileTime[1]) * 60 +
						int.Parse(lastFileTime[2]);
			}
			if (_tsConfig.timeType == 0) _tsConfig.isContinueConcat = false;
			util.debugWriteLine("ready arg ts iscontinueconcat " + _tsConfig.isContinueConcat + " startType " + _tsConfig.timeType + " lastfiletime " + lastFileTime + " lastfile " + lastFile);
			return _tsConfig;
		}
	}
}
