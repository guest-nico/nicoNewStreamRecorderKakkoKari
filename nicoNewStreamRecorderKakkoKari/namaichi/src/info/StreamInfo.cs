/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2021/10/08
 * Time: 14:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Text.RegularExpressions;
using namaichi.rec;
using namaichi.utility;

namespace namaichi.info
{
	/// <summary>
	/// Description of StreamInfo.
	/// </summary>
	public class StreamInfo {
		public string url;
		public string lvid;
		public long openTime;
		
		public string type = null;
		public string data = null;
		public string res = null;
		public bool isRtmpOnlyPage = false;
		public bool isTimeShift = false;
		public bool isChasable = false;
		public bool isReservation = false;
		
		public string communityNum = null;
		
		public long endTime, _openTime, serverTime, vposBaseTime;
		public TimeSpan programTime;
		public string[] recFolderFileInfo;
		public bool isChannelPlus = false;
		
		public StreamInfo() {
		}
		public StreamInfo (string url, string lvid, bool isTimeShift, bool isChannelPlus) {
			this.url = url;
			this.lvid = lvid;
			this.isTimeShift = isTimeShift;
			this.isChannelPlus = isChannelPlus;
		}
		//public void set(string res, CookieContainer cc) {
		public void set(string res) {
			if (lvid.StartsWith("lv")) {
				type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
				data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
				this.res = res; 
				isRtmpOnlyPage = res.IndexOf("%3Cgetplayerstatus%20") > -1 || res.IndexOf("<getplayerstatus ") > -1;
				
				isChasable = util.getRegGroup(res, "&quot;permissions&quot;:\\[[^\\]]*(CHASE_PLAY)") != null &&
					res.IndexOf("isChasePlayEnabled&quot;:true") > -1;
				
				data = (isRtmpOnlyPage) ? System.Web.HttpUtility.UrlDecode(res) :
							System.Web.HttpUtility.HtmlDecode(data);
				isReservation = lvid.StartsWith("lv") ? data.IndexOf("\"reservation\"") > -1 : data.IndexOf("\"has_archived_files\":true") > -1;
				
				//long endTime, _openTime, serverTime, vposBaseTime;
				//openTime = endTime = _openTime = serverTime = vposBaseTime = 0;
			} else {
			    type = "chplus";
			    data = this.res = res;
			    isChasable = false;
			    isReservation = false;
		    }
			programTime = util.getUnixToDatetime(endTime) - util.getUnixToDatetime(openTime);
			recFolderFileInfo = getHtml5RecFolderFileInfo(data, type, isRtmpOnlyPage);
		}
		public bool getTimeInfo() {
			if (data == null) return false;
			
			if (lvid.StartsWith("lv")) {
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
			} else {
				string startTimeDtStr = util.getRegGroup(data, "\"live_started_at\":\"(.+?)\"");
				if (startTimeDtStr == null) startTimeDtStr = util.getRegGroup(data, "\"live_scheduled_start_at\":\"(.+?)\"");
				if (startTimeDtStr == null) startTimeDtStr = util.getRegGroup(data, "\"released_at\":\"(.+?)\"");
				var endTimeDtStr = util.getRegGroup(data, "\"live_finished_at\":\"(.+?)\"");
				if (endTimeDtStr == null) endTimeDtStr = util.getRegGroup(data, "\"live_scheduled_end_at\":\"(.+?)\"");
				if (endTimeDtStr == null) endTimeDtStr = util.getRegGroup(data, "\"released_at\":\"(.+?)\"");
				if (startTimeDtStr == null || endTimeDtStr == null) return false;
				openTime = _openTime = util.getUnixTime(DateTime.Parse(startTimeDtStr) + TimeSpan.FromHours(9));
				endTime = util.getUnixTime(DateTime.Parse(endTimeDtStr) + TimeSpan.FromHours(9));
				
				serverTime = vposBaseTime = util.getUnixTime();
			}
			return true;
		}
		private string[] getHtml5RecFolderFileInfo(string data, string type, bool isRtmpOnlyPage) {
			string host, group, title, communityNum, userId;
			host = group = title = communityNum = userId = null;
			
			if (lvid.StartsWith("lv")) {
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
							//var apiRes = util.getPageSource(url + "/programinfo", container);
						
						} else {
							
							var isChannel = util.getRegGroup(data, "visualProviderType\":\"(channel)\",\"title\"") != null;
				//			host = util.getRegGroup(res, "provider......name.....(.*?)\\\\\"");
							group = util.getRegGroup(data, "\"socialGroup\".*?\"name\".\"(.*?)\"");
				//			group = util.uniToOriginal(group);
				//			group = util.getRegGroup(res, "communityInfo.\".+?title.\"..\"(.+?).\"");
							host = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.*?)\"");
							if (string.IsNullOrEmpty(host)) host = group;
				//			System.out.println(group);
				//			host = util.uniToOriginal(host);
				//			title = util.getRegGroup(res, "\\\"programHeader\\\"\:\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\:\\\"(.*?)\\\"");
				//			title = util.getRegGroup(res, "\\\\\"programHeader\\\\\":\\{\\\\\"thumbnailUrl.+?\\\\\"title\\\\\":\\\\\"(.*?)\\\\\"");
							title = util.getRegGroup(data, "visualProviderType\":\"(community|channel)\",\"title\":\"(.*?)\",", 2);
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
			} else {
				communityNum = "000";
				group = host = "ニコニコチャンネルプラス"; 
				var _cn = util.getRegGroup(res, "\"fanclub_site\".+?\"id\":(\\d+)");
				if (_cn != null) {
					communityNum = _cn;
					var c = new Curl();
					var r = c.getStr("https://nfc-api.nicochannel.jp/fc/fanclub_sites/" + _cn + "/page_base_info", util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_2TLS);
					if (r != null) {
						var _group = util.getRegGroup(r, "\"fanclub_site_name\":\"(.+?)\",\"favicon");
						if (_group != null) group = host = _group;
						_cn = util.getRegGroup(r, "\"fanclub_code\":\"(.+?)\"");
						if (_cn != null) communityNum = _cn;
					}
				}
				title = util.getRegGroup(res, "\"title\":\"(.+?)\"");
				userId = util.getRegGroup(url, "https://nicochannel.jp/(.+?)/");
				if (userId == null) userId = communityNum;
				title = title.Replace("\\", "");
				host = host.Replace("\\", "");
				group = group.Replace("\\", "");
				
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum);
				if (host == null || group == null || title == null || communityNum == null) return null;
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum);
			}
			//if (communityNum != null) communityNum = communityNum;
			return new string[]{host, group, title, lvid, communityNum, userId};

		}
		public StreamInfo clone() {
			return (StreamInfo)this.MemberwiseClone();
			/*
			var ret = new StreamInfo();
			ret.lvid = lvid;
			ret.openTime = openTime;
			ret.type = type;
			ret.data = data;
			ret.isRtmpOnlyPage = isRtmpOnlyPage;
			ret.isTimeShift = isTimeShift;
			ret.isChasable = isChasable;
			ret.isReservation = isReservation;
			ret.communityNum = communityNum;
			ret.endTime = endTime;
			ret._openTime = _openTime;
			ret.serverTime = serverTime;
			ret.vposBaseTime = vposBaseTime;
			ret.programTime = programTime;
			return ret;
			*/
		}
	}
	public class RecordInfo {
		
		public string[] recFolderFile = null;
		public bool isRtmp = false;
		public StreamInfo si = null;
		public int pageType;
		
		public bool isChase = false;
		public bool isRealtimeChase = false;
		
		public bool isFmp4;
		public string[] webSocketRecInfo = null;
		public TimeShiftConfig timeShiftConfig = null;
		public string userId;
		public bool isPremium;
		
		public RecordInfo (StreamInfo si, int pageType, bool isRtmp) {
			this.si = si;
			this.isRtmp = isRtmp;
			if (si.isRtmpOnlyPage) isRtmp = true;
			//recFolderFileInfo = getHtml5RecFolderFileInfo(si.data, si.type, si.isRtmpOnlyPage);
			
			this.pageType = pageType; 
		}
		public void set(bool isChaseCheck, config.config cfg, MainForm form) {
			isChase = isChaseRec(isChaseCheck, si.isChasable, si.isReservation, si.data, cfg) && !isRtmp;
			if (isChase && !isRtmp) si.isTimeShift = true;
			
			util.debugWriteLine("pagetype " + pageType + " isChase" + isChase);
			
			isFmp4 = si.data.IndexOf("\"providerType\":\"community\"") > -1 &&
					pageType == 0 && !si.isTimeShift && cfg.get("latency") == "新方式の低遅延" && true;
			
			webSocketRecInfo = RecordInfo.getWebSocketInfo(si.data, isRtmp, isChase, si.isTimeShift, form, isFmp4);
			util.debugWriteLine("websocketrecinfo " + webSocketRecInfo);
			
			isRealtimeChase =  isChase && !isChaseCheck && 
					!(form.args.Length > 0 && bool.Parse(form.rec.cfg.get("IsArgChaseRecFromFirst")));
			
			userId = util.getRegGroup(si.res, "\"user\"\\:\\{\"user_id\"\\:(.+?),");
			if (userId == "null") {
				userId = "guest";
				form.addLogText("非ログインで開始を試みます");
			}
			isPremium = si.res.IndexOf("\"member_status\":\"premium\"") > -1;
		}
		
		public string[] getRecFilePath(bool isRtmp, TimeShiftConfig timeShiftConfig, bool isFmp4, config.config cfg, MainForm form) {
			util.debugWriteLine(si.openTime + " c " + si.recFolderFileInfo[0] + " timeshiftConfig " + timeShiftConfig);
			try {
				var id = si.isChannelPlus ? si.recFolderFileInfo[3].Substring(0, 12) : si.recFolderFileInfo[3];
				return util.getRecFolderFilePath(si.recFolderFileInfo[0], si.recFolderFileInfo[1], si.recFolderFileInfo[2], id, si.recFolderFileInfo[4], si.recFolderFileInfo[5], cfg, si.isTimeShift, timeShiftConfig, si.openTime, isRtmp, isFmp4, false, form);
			} catch (Exception e) {
				form.addLogText("保存先パスの取得もしくはフォルダの作成に失敗しました");
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		bool isChaseRec(bool isChaseCheck, bool isChasable, bool isReservation, string data, config.config cfg) {
			if (isChaseCheck) return true;
			if (si.isTimeShift) return false;
			if (!isChasable) return false;
			
			var isChaseRecord = bool.Parse(cfg.get("IsChaseRecord"));
			if (!isChaseRecord) return false;
			var isOnlyTimeShiftChase = bool.Parse(cfg.get("IsOnlyTimeShiftChase"));
			if (!isOnlyTimeShiftChase) return true;
			
			return isReservation;
			//var res = util.getPageSource("http://live.nicovideo.jp/api/getplayerstatus/" + lvid, container);
			//if (res == null) return false;
			//return res.IndexOf("<is_nonarchive_timeshift_enabled>1") > -1 ||
			//	res.IndexOf("is_archiveplayserver>1") > -1;
		}
		public static string[] getWebSocketInfo(string data, bool isRtmp, bool isChase, bool isTimeShift, MainForm form, bool isFmp4) {
//			util.debugWriteLine(data);
			var wsUrl = util.getRegGroup(data, "\"webSocketUrl\":\"(ws[\\d\\D]+?)\"");
			if (wsUrl == null) return null;

			var latency = form.rec.cfg.get("latency");
			wsUrl += "&frontend_id=" + ((latency == "1.0" || latency == "3.0" || isRtmp) ? "90" : "12");
			util.debugWriteLine("wsurl " + wsUrl);
			
			var broadcastId = util.getRegGroup(data, "\"broadcastId\"\\:\"(\\d+)\"");
			
			util.debugWriteLine("broadcastid " + broadcastId);
			
			if (isRtmp && false) {
				wsUrl = wsUrl.Replace("v2", "v1");
				broadcastId = util.getRegGroup(wsUrl, "watch/.*?(\\d+)");
			}
			
			//rtmp {"type":"watch","body":{"command":"getpermit","requirement":{"broadcastId":"17684079051334","route":"","stream":{"protocol":"rtmp","requireNewStream":true},"room":{"isCommentable":true,"protocol":"webSocket"}}}}
			string request = null;
			var ver = util.getRegGroup(wsUrl, "/v(\\d+)/");
			if (ver != "1" && ver != "2") {
				wsUrl = Regex.Replace(wsUrl, "/v" + ver + "/", "/v2/");
				ver = "2";
			}
				
			if (ver == "1") {
				if (broadcastId == null) {
					//broadcastId = util.getRegGroup(data, "webSocketUrl.+?watch/(\\d+).+?audience_token");
					#if DEBUG
					//	form.addLogText("broadcastId not found_");
					#endif
					//if (broadcastId == null) {
						form.addLogText("broadcastId not found");
						return null;
					//}l;
				}
				
				if (isRtmp && !isTimeShift) 
					request = ("{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"rtmp\",\"requireNewStream\":true},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}");
				else
					request = //(isChase) ? 
						//("{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":true,\"priorStreamQuality\":\"normal\", \"isLowLatency\": false},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}");
						("{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":true,\"priorStreamQuality\":\"normal\", \"isLowLatency\": false,\"isChasePlay\":false},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}");
			} else if (ver == "2") {
				
				request = isRtmp ? "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"normal\",\"protocol\":\"rtmp\",\"latency\":\"high\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}"
					: "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"normal\",\"protocol\":\"hls" + (isFmp4 ? "+fmp4" : "") + "\",\"latency\":\"" + (latency == "新方式の低遅延" || latency == "1" ? "low" : "high") + "\",\"chasePlay\":" + (isChase ? "true" : "false") + "},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";
			} else {
				form.addLogText("unknown type " + ver);
				return null;
			}
			
//			string request = "{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"rtmp\"},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}";
			util.debugWriteLine("request " + request);
			return new string[]{wsUrl, request, ver, broadcastId};
		}
		public bool setTimeShiftConfig(RecordingManager rm, bool isChaseCheck) {
			//timeshift option
			if (((si.isTimeShift && !isChase) || isChaseCheck) && !isRtmp) {
//						if (rm.ri != null) timeShiftConfig = rm.ri.tsConfig;
				if (rm.argTsConfig != null) {
					timeShiftConfig = getReadyArgTsConfig(rm.argTsConfig.clone(), si.recFolderFileInfo[0], si.recFolderFileInfo[1], si.recFolderFileInfo[2], si.recFolderFileInfo[3], si.recFolderFileInfo[4], si.recFolderFileInfo[5], si.openTime, (int)(si.openTime - si._openTime), rm);
				} else {
					timeShiftConfig = getTimeShiftConfig(si.recFolderFileInfo[0], si.recFolderFileInfo[1], si.recFolderFileInfo[2], si.recFolderFileInfo[3],si. recFolderFileInfo[4], si.recFolderFileInfo[5], rm.cfg, si.openTime, isChase, si._openTime, rm);
					if (timeShiftConfig == null) return false;
				}
			}
			if (!isChaseCheck && isChase)
				timeShiftConfig = new TimeShiftConfig();
			return true;
		}
		private TimeShiftConfig getTimeShiftConfig(string host, 
				string group, string title, string lvId, string communityNum, 
				string userId, config.config cfg, long startTime, bool isChase, 
				long openTime, RecordingManager rm) {
			var segmentSaveType = cfg.get("segmentSaveType");
			var _lvid = !si.isChannelPlus ? lvId : lvId.Substring(0, 12);
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, _lvid, communityNum, userId, cfg, startTime, isFmp4);
			util.debugWriteLine("timeshift lastfile " + lastFile);
			string[] lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType, isFmp4);
			if (lastFileTime == null)
				util.debugWriteLine("timeshift lastfiletime " + 
				                    ((lastFileTime == null) ? "null" : string.Join(" ", lastFileTime)));
			
			try {
				var prepTime = (int)(startTime - openTime);
				var o = new TimeShiftOptionForm(lastFileTime, segmentSaveType, rm.cfg, isChase, prepTime, isFmp4, si.isChannelPlus);
				
				
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
		
		private TimeShiftConfig getReadyArgTsConfig(
				TimeShiftConfig _tsConfig, string host, 
				string group, string title, string lvId, 
				string communityNum, string userId, 
				long _openTime, int prepTime, RecordingManager rm) {
			
			var segmentSaveType = rm.cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, rm.cfg, _openTime, isFmp4);
			util.debugWriteLine("timeshift lastfile " + lastFile + " host " + host + " title " + title);
			
			var lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType, isFmp4);
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
		public bool setRecFolderFile(RecordingManager rm) {
			if (!rm.rfu.isPlayOnlyMode) {
				util.debugWriteLine("rm.rfu " + rm.rfu.GetHashCode() + " rfu " + rm.rfu.GetHashCode());
				if (recFolderFile == null)
					recFolderFile = getRecFilePath(isRtmp, timeShiftConfig, isFmp4, rm.cfg, rm.form);
				if (recFolderFile == null || recFolderFile[0] == null) {
					//パスが長すぎ
					rm.form.addLogText("パスに問題があります。 " + (recFolderFile != null ? recFolderFile[1] : ""));
					util.debugWriteLine("too long path? " + (recFolderFile != null ? recFolderFile[1] : ""));
					return false;
				}
			} else {
				var fName = "a/" + util.getFileName(si.recFolderFileInfo[0], si.recFolderFileInfo[1], si.recFolderFileInfo[2], si.recFolderFileInfo[3], si.recFolderFileInfo[4], rm.cfg, si.openTime);
				recFolderFile = new String[]{fName, fName, fName};//new string[]{"", "", ""};
			}
			for (int i = 0; i < recFolderFile.Length; i++)
					util.debugWriteLine("recd " + i + " " + si.recFolderFileInfo[i]);
			return true;
		}
		public RecordInfo clone() {
			var ri = (RecordInfo)this.MemberwiseClone();
			ri.si = si.clone();
			return ri;
		}
	}
}
