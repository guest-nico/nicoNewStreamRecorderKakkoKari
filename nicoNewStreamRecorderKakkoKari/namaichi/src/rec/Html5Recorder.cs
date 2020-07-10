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
		public string[] recFolderFileInfo;
		public string[] recFolderFile = null;
		//private bool isSub;
		
		private long openTime;
		public WebSocketRecorder wsr = null;
	
		public Html5Recorder(string url, CookieContainer container, 
				string lvid, RecordingManager rm, RecordFromUrl rfu,
				bool isTimeShift)
		{
			this.url = url;
			this.container = container;
			this.lvid = lvid;
			this.rm = rm;
			this.rfu = rfu;
			this.isTimeShift = isTimeShift;
			//this.isSub = isSub;
		}
		public int record(string res, bool isRtmp, int pageType) {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			//var ret = html5Record(res, isRtmp, pageType).Result;
			var ret = html5Record(res, isRtmp, pageType);
			util.debugWriteLine("html5 rec ret " + ret);
			return ret;
		}
		
		public static string[] getWebSocketInfo(string data, bool isRtmp, bool isChase, bool isTimeShift, MainForm form) {
//			util.debugWriteLine(data);
			var wsUrl = util.getRegGroup(data, "\"webSocketUrl\":\"(ws[\\d\\D]+?)\"");
			if (wsUrl == null) return null;

			var latency = float.Parse(form.rec.cfg.get("latency"));
			wsUrl += "&frontend_id=" + (latency % 1 == 0 || isRtmp ? "90" : "12");
			util.debugWriteLine("wsurl " + wsUrl);
			
			var broadcastId = util.getRegGroup(data, "\"broadcastId\"\\:\"(\\d+)\"");
			
			
			util.debugWriteLine("broadcastid " + broadcastId);
			
			
			if (isRtmp) {
				wsUrl = wsUrl.Replace("v2", "v1");
				broadcastId = util.getRegGroup(wsUrl, "watch/.*?(\\d+)");
			}
			
			//rtmp {"type":"watch","body":{"command":"getpermit","requirement":{"broadcastId":"17684079051334","route":"","stream":{"protocol":"rtmp","requireNewStream":true},"room":{"isCommentable":true,"protocol":"webSocket"}}}}
			string request = null;
			var ver = util.getRegGroup(wsUrl, "/v(\\d+)/");
			if (ver == "1") {
				if (broadcastId == null) {
					//broadcastId = util.getRegGroup(data, "webSocketUrl.+?watch/(\\d+).+?audience_token");
					#if DEBUG
					//	form.addLogText("broadcastId not found_");
					#endif
					//if (broadcastId == null) {
						form.addLogText("broadcastId not found");
						return null;
					//}
				}
				
				if (isRtmp && !isTimeShift) 
					request = ("{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"rtmp\",\"requireNewStream\":true},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}");
				else
					request = //(isChase) ? 
						//("{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":true,\"priorStreamQuality\":\"normal\", \"isLowLatency\": false},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}");
						("{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":true,\"priorStreamQuality\":\"normal\", \"isLowLatency\": false,\"isChasePlay\":false},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}");
			} else if (ver == "2") {
				request = isRtmp ? "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"normal\",\"protocol\":\"rtmp\",\"latency\":\"high\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}"
					//: "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"normal\",\"protocol\":\"hls\",\"latency\":\"" + (latency < 1.1 ? "low" : "high") + "\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";
					: "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"normal\",\"protocol\":\"hls\",\"latency\":\"" + (latency < 1.1 ? "low" : "high") + "\",\"chasePlay\":" + (isChase ? "true" : "false") + "},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";
			} else {
				form.addLogText("unknown type " + ver);
				return null;
			}
			
//			string request = "{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"rtmp\"},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}";
			util.debugWriteLine("request " + request);
			return new string[]{wsUrl, request, ver};
		}
		private string[] getHtml5RecFolderFileInfo(string data, string type, bool isRtmpOnlyPage) {
			string host, group, title, communityNum, userId;
			host = group = title = communityNum = userId = null;
			
			if (!isRtmpOnlyPage) {
//				host = group = title = communityNum = userId = null;
				if (type == "official") {
					group = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
					
		//			if (host == null) host = "official";
					host = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.*?)\"");
					if (host == null || host == "") {
						//group = "official";
						host = "公式生放送";
					}
					if (group == null || group == "") group = "official"; 
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
						//var a = new System.Net.WebHeaderCollection();
						var apiRes = util.getPageSource(url + "/programinfo", container);
					
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
						userId = (isChannel) ? "channel" : (util.getRegGroup(data, "supplier\":{\"name\".+?pageUrl\":\"https*://www.nicovideo.jp/user/(\\d+?)\""));
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
				
			} else {
				group = util.getRegGroup(data, "class=\"ch_name\" title=\"(.+?)\"");
				if (group == null) group = "official";
				
				host = util.getRegGroup(data, "（提供:(.+?)）");
				if (host == null) host = "公式生放送";
				title = util.getRegGroup(data, "<title>(.+?)</title>");
//				title = util.uniToOriginal(title);
				communityNum = util.getRegGroup(data, "channel/(ch\\d+)");
				if (communityNum == null) communityNum = "official";
				userId = "official";
				
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum);
				if (host == null || group == null || title == null || communityNum == null) return null;
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum);
			}
			if (communityNum != null) rm.communityNum = communityNum;
			return new string[]{host, group, title, lvid, communityNum, userId};

		}
		private int html5Record(string res, bool isRtmp, int pageType) {
			//webSocketInfo 0-wsUrl 1-request
			//recFolderFileInfo host, group, title, lvid, communityNum
			//return 0-end stream 1-stop
			
			string[] webSocketRecInfo;
			recFolderFileInfo = null;
			
			
			var isNoPermission = false;
			while(rm.rfu == rfu) {
				var type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
				var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
				var isRtmpOnlyPage = res.IndexOf("%3Cgetplayerstatus%20") > -1 || res.IndexOf("<getplayerstatus ") > -1;
				if (isRtmpOnlyPage) isRtmp = true;
				var isChasable = util.getRegGroup(res, "&quot;permissions&quot;:\\[[^\\]]*(CHASE_PLAY)") != null &&
					res.IndexOf("isChasePlayEnabled&quot;:true") > -1;
				var isChaseCheck = rm.form.isChaseChkBtn.Checked;
				if (isChaseCheck && (!isChasable || pageType != 0)) {
					rm.form.addLogText("追いかけ再生ができませんでした");
					return 3;
				}
				//util.debugWriteLine(data);
				
				var isChase = isChaseRec(isChaseCheck, isChasable, lvid) && !isRtmp;
				if (isChase && !isRtmp) isTimeShift = true;
				
				//;,&quot;permissions&quot;:[&quot;CHASE_PLAY&quot;],&quot;
				//var pageType = util.getPageType(res);
//				var pageType = pageType;
				util.debugWriteLine("pagetype " + pageType + " isChase" + isChase);
				
				if ((data == null && !isRtmpOnlyPage) || (pageType != 0 && pageType != 7)) {
					//processType 0-ok 1-retry 2-放送終了 3-その他の理由の終了
					var processType = processFromPageType(pageType);
					util.debugWriteLine("processType " + processType);
					//if (processType == 0 || processType == 1) continue;
					if (processType == 2) return 3;
//					if (processType == 3) return 0;
					
					System.Threading.Thread.Sleep(3000);
					
					res = getPageSourceFromNewCookie();
					
					continue;
				}
				
				data = (isRtmpOnlyPage) ? System.Web.HttpUtility.UrlDecode(res) :
						System.Web.HttpUtility.HtmlDecode(data);
				
				long endTime, _openTime, serverTime, vposBaseTime;
//				DateTime programTime, jisa;
				openTime = endTime = _openTime = serverTime = vposBaseTime = 0;
				
				if (!getTimeInfo(data, ref openTime, ref endTime, 
						ref _openTime, ref serverTime, ref vposBaseTime, isRtmpOnlyPage))
					return 3;
				
				var programTime = util.getUnixToDatetime(endTime) - util.getUnixToDatetime(openTime);
				var jisa = util.getUnixToDatetime(serverTime / 1000) - DateTime.Now;

				//0-wsUrl 1-request
				webSocketRecInfo = getWebSocketInfo(data, isRtmp, isChase, isTimeShift, rm.form);
				util.debugWriteLine("websocketrecinfo " + webSocketRecInfo);
				if (!isRtmpOnlyPage && webSocketRecInfo == null) break;
				
				util.debugWriteLine("isnopermission " + isNoPermission);
//				if (isNoPermission) webSocketRecInfo[1] = webSocketRecInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true");
				recFolderFileInfo = getHtml5RecFolderFileInfo(data, type, isRtmpOnlyPage);
				
				
				//timeshift option
				timeShiftConfig = null;
				if (((isTimeShift && !isChase) || isChaseCheck) && !isRtmp) {
//						if (rm.ri != null) timeShiftConfig = rm.ri.tsConfig;
					if (rm.argTsConfig != null) {
						timeShiftConfig = getReadyArgTsConfig(rm.argTsConfig.clone(), recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], openTime, (int)(openTime - _openTime));
					} else {
						timeShiftConfig = getTimeShiftConfig(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, openTime, isChase, _openTime);
						if (timeShiftConfig == null) return 2;
//							rm.cfg.set("IsUrlList", timeShiftConfig.isOutputUrlList.ToString().ToLower());
//							rm.cfg.set("openUrlListCommand", timeShiftConfig.openListCommand);
					}
				}
				if (!isChaseCheck && isChase)
					timeShiftConfig = new TimeShiftConfig();
			
				var isRealtimeChase =  isChase && !isChaseCheck && 
						!(rm.form.args.Length > 0 && bool.Parse(rm.cfg.get("IsArgChaseRecFromFirst")));
				
				if (!rfu.isPlayOnlyMode) {
					util.debugWriteLine("rm.rfu " + rm.rfu.GetHashCode() + " rfu " + rfu.GetHashCode());
					if (recFolderFile == null)
						recFolderFile = getRecFilePath(isRtmp);
					if (recFolderFile == null || recFolderFile[0] == null) {
						//パスが長すぎ
						rm.form.addLogText("パスに問題があります。 " + (recFolderFile != null ? recFolderFile[1] : ""));
						util.debugWriteLine("too long path? " + (recFolderFile != null ? recFolderFile[1] : ""));
						return 2;
					}
				} else {
					var fName = "a/" + util.getFileName(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], rm.cfg, openTime);
					recFolderFile = new String[]{fName, fName, fName};//new string[]{"", "", ""};
				}
			
				//display set
				var rss = new RecordStateSetter(rm.form, rm, rfu, isTimeShift, false, recFolderFile, rfu.isPlayOnlyMode, isRtmpOnlyPage, isChase);
				Task.Run(() => {
				       	rss.set(data, type, recFolderFileInfo);
				       	
				       	//hosoInfo
						if (rm.cfg.get("IshosoInfo") == "true" && !rfu.isPlayOnlyMode)
							rss.writeHosoInfo();
					});
				
				util.debugWriteLine("form disposed" + rm.form.IsDisposed);
				util.debugWriteLine("recfolderfile test " + recFolderFileInfo);
				
				var fileName = System.IO.Path.GetFileName(recFolderFile[1]);
				rm.form.setTitle(fileName);
				
				
//				if (recFolderFile == null) continue;
				
				for (int i = 0; i < recFolderFile.Length; i++)
					util.debugWriteLine("recd " + i + " " + recFolderFileInfo[i]);
					
				
				var userId = util.getRegGroup(res, "\"user\"\\:\\{\"user_id\"\\:(.+?),");
				var isPremium = res.IndexOf("\"member_status\":\"premium\"") > -1;
				wsr = new WebSocketRecorder(webSocketRecInfo, container, recFolderFile, rm, rfu, this, openTime, isTimeShift, lvid, timeShiftConfig, userId, isPremium, programTime, type, _openTime, isRtmp, isRtmpOnlyPage, isChase, isRealtimeChase, true, rss, vposBaseTime);
				rm.wsr = wsr;
				try {
					isNoPermission = wsr.start();
					
					if (rm.cfg.get("fileNameType") == "10" && (recFolderFile[1].IndexOf("{w}") > -1 || recFolderFile[1].IndexOf("{c}") > -1))
						renameStatistics(rss);
					
					rm.wsr = null;
					if (wsr.isEndProgram) {
						if ((!isTimeShift || isChase) && rss.isWrite)
							rss.writeEndTime(container, wsr.endTime);
						return 3;
					}
						
				} catch (Exception e) {
					rm.form.addLogText("録画中に予期せぬ問題が発生しました");
					util.debugWriteLine("wsr start exception " + e.Message + e.StackTrace);
				}
				
				
				util.debugWriteLine(rm.rfu + " " + rfu + " " + (rm.rfu == rfu));
				if (rm.rfu != rfu || isRtmp) break;
				
				res = getPageSourceFromNewCookie();
			}
			return 1;
		}
		private string getPageSourceFromNewCookie() {
			while (rm.rfu == rfu) {
				try {
					var _cg = new CookieGetter(rm.cfg);
					var _cgtask = _cg.getHtml5RecordCookie(url);
					_cgtask.Wait();
					
					if (_cgtask == null || _cgtask.Result == null) {
						System.Threading.Thread.Sleep(3000);
						continue;
					}
					container = _cgtask.Result[0];
					return _cg.pageSource;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
				} 
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
				rm.form.addLogText("満員です。");
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
				
			} else if (pageType == 5) {
				rm.form.addLogText("接続エラー。10秒後リトライします。");
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
				rm.form.addLogText("require_community_member");
				if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
					if (rm.form.IsDisposed) return 2;
					try {
						rm.form.formAction(() => {
			       			MessageBox.Show("コミュニティに入る必要があります：\nrequire_community_member/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
						}, false);
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
				}
				if (bool.Parse(rm.cfg.get("IsfailExit")) && util.isShowWindow) {
					rm.rfu = null;
					try {
						rm.form.formAction(() => {
							try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
							}
						});
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
					
				}
				return 3;
				
			} else {
				var mes = "";
				if (pageType == 2) mes = "この放送は終了しています。";
				if (pageType == 3) mes = "この放送は終了しています。";
				rm.form.addLogText(mes);
				util.debugWriteLine("pagetype " + pageType + " end");
				
				if (bool.Parse(rm.cfg.get("IsdeleteExit")) && util.isShowWindow) {
					rm.rfu = null;
					try {
						rm.form.formAction(() => {
			       			try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	       		}
						});
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
					
				}
				return 2;
				//var nh5r = new NotHtml5Recorder(url, container, lvid, rm, this);
				//nh5r.record(res);
			}
			
		}
		
		private TimeShiftConfig getTimeShiftConfig(string host, 
				string group, string title, string lvId, string communityNum, 
				string userId, config.config cfg, long startTime, bool isChase, 
				long openTime) {
			var segmentSaveType = cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, cfg, startTime);
			util.debugWriteLine("timeshift lastfile " + lastFile);
			string[] lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType);
			if (lastFileTime == null)
				util.debugWriteLine("timeshift lastfiletime " + 
				                    ((lastFileTime == null) ? "null" : string.Join(" ", lastFileTime)));
			
			try {
				var prepTime = (int)(startTime - openTime);
				var o = new TimeShiftOptionForm(lastFileTime, segmentSaveType, rm.cfg, isChase, prepTime);
				
				try {
					rm.form.formAction(() => {
	       		       	try {
			        	    o.ShowDialog(rm.form);
	       		       	} catch (Exception e) {
	       		       		util.debugWriteLine("timeshift option form invoke " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       		       	}
					}, false);
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
		//public string[] getRecFilePath(long openTime, bool isRtmp) {
		public string[] getRecFilePath(bool isRtmp) {
			util.debugWriteLine(openTime + " c " + recFolderFileInfo[0] + " timeshiftConfig " + timeShiftConfig);
			try {
				return util.getRecFolderFilePath(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, isTimeShift, timeShiftConfig, openTime, isRtmp);
			} catch (Exception e) {
				rm.form.addLogText("保存先パスの取得もしくはフォルダの作成に失敗しました");
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		private TimeShiftConfig getReadyArgTsConfig(
				TimeShiftConfig _tsConfig, string host, 
				string group, string title, string lvId, 
				string communityNum, string userId, 
				long _openTime, int prepTime) {
			
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
			
			if (_tsConfig.isOpenTimeBaseStartArg) _tsConfig.timeSeconds += prepTime;
			if (_tsConfig.isOpenTimeBaseEndArg) _tsConfig.endTimeSeconds += prepTime;
			return _tsConfig;
		}
		private bool getTimeInfo(string data, ref long openTime, ref long endTime, 
				ref long _openTime, ref long serverTime, ref long vposBaseTime, bool isRtmpOnlyPage) {
			if (data == null) return false;
			
			if (!isRtmpOnlyPage) {
				if (!long.TryParse(util.getRegGroup(data, "\"beginTime\":(\\d+)"), out openTime))
						return false;
	//				var openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"));
				if (!long.TryParse(util.getRegGroup(data, "\"endTime\":(\\d+)"), out endTime))
						return false;				
				if (!long.TryParse(util.getRegGroup(data, "\"openTime\":(\\d+)"), out _openTime))
						return false;
				if (!long.TryParse(util.getRegGroup(data, "\"serverTime\":(\\d+)"), out serverTime))
						return false;
				if (!long.TryParse(util.getRegGroup(data, "\"vposBaseTime\":(\\d+)"), out vposBaseTime))
						return false;
			} else {
				if (!long.TryParse(util.getRegGroup(data, "<start_time>(\\d+)"), out openTime))
						return false;
	//				var openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"));
				if (!long.TryParse(util.getRegGroup(data, "<end_time>(\\d+)"), out endTime))
						return false;				
				if (!long.TryParse(util.getRegGroup(data, "<base_time>(\\d+)"), out _openTime))
						return false;
				if (!long.TryParse(util.getRegGroup(data, "status=\"ok\" time=\"(\\d+)\""), out serverTime))
						return false;
			}
			return true;
		}
		bool isChaseRec(bool isChaseCheck, bool isChasable, string lvid) {
			if (isChaseCheck) return true;
			if (isTimeShift) return false;
			if (!isChasable) return false;
			
			var isChaseRecord = bool.Parse(rm.cfg.get("IsChaseRecord"));
			if (!isChaseRecord) return false;
			var isOnlyTimeShiftChase = bool.Parse(rm.cfg.get("IsOnlyTimeShiftChase"));
			if (!isOnlyTimeShiftChase) return true;
			
			var res = util.getPageSource("http://live.nicovideo.jp/api/getplayerstatus/" + lvid, container);
			if (res == null) return false;
			return res.IndexOf("<is_nonarchive_timeshift_enabled>1") > -1 ||
				res.IndexOf("is_archiveplayserver>1") > -1;
		}
		private void renameStatistics(RecordStateSetter rss) {
			try {
				wsr.setRealTimeStatistics();
				
				rss.renameStatistics(wsr.visitCount.Replace("-", ""), wsr.commentCount.Replace("-", ""));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
	}
}

