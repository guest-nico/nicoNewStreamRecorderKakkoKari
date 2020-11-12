/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/29
 * Time: 14:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;

using namaichi.info;


namespace namaichi.rec
{
	/// <summary>
	/// Description of JikkenRecorder.
	/// </summary>
	public class JikkenRecorder
	{
		/*
		private string lvid;
		private CookieContainer container;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		public string status;
		public bool isLive;
		private bool isTimeshiftEnabled = false;
//		private string userId;
		private bool isBroadcaster = false;
		private string actionTrackId;
		private string[] availableQualities;
		private string watchingUrl;
		private string streamCapacity;
		public string requestQuality;
		private config.config config;
		
		private string watchingRes;
		private string hlsUrl;
		private string msUri;
		private WatchingInfo wi;
//		private TimeSpan jisa = TimeSpan.MinValue;
		
		private string[] recFolderFileInfo;
		private TimeShiftConfig timeShiftConfig;
		
		//private bool isSub;
			
		public JikkenRecorder(string res, string lvid, CookieContainer container, config.config config, RecordingManager rm, RecordFromUrl rfu)
		{
			this.lvid = lvid;
			this.container = container;
			this.config = config;
			this.rm = rm;
			this.rfu = rfu;
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			status = (data == null) ? null : util.getRegGroup(data, "&quot;status&quot;:&quot;(.+?)&quot;");
			//if (status != "ON_AIR" && status != "ENDED") return 5;
			isLive = status == "ON_AIR";
			isTimeshiftEnabled = res.IndexOf("isTimeshiftEnabled&quot;:true") > -1;
//			userId = util.getRegGroup(res, "\"user_id\"\\:(\\d+),");
			var _isBroadcaster = util.getRegGroup(data, "&quot;isBroadcaster&quot;\\:(.+?),");
			if (_isBroadcaster != null) isBroadcaster = bool.Parse(_isBroadcaster);
			isBroadcaster = false;
			actionTrackId = getActionTrackId();
			//maxQuality = util.getRegGroup(data, "&quot;maxQuality&quot;\\:&quot;(.+?)&quot;");
			
			watchingUrl = "https://api.cas.nicovideo.jp/v1/services/live/programs/" + lvid + "/watching" + 
				((isLive) ? "" : "-archive");
			this.streamCapacity = util.getRegGroup(data, "&quot;maxQuality&quot;\\:&quot;(.+?)&quot;");
			util.debugWriteLine("maxQuality " + streamCapacity);
			//var wc = new WebHeaderCollection();
			//var watchingRes = util.getPageSource
//				public static int getPageTypeJikken(string res) {
		
//			if (res.IndexOf("<!doctype html>") > -1 && data != null && status == "ON_AIR") return 0;
//			else if (res.IndexOf("<!doctype html>") > -1 && data != null && status == "ENDED") return 7;
			//this.isSub = isSub;
		}
		public int record(string res, bool isRtmp) {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			recFolderFileInfo = null;
			string[] recFolderFile = null;
			var type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			
			data = System.Web.HttpUtility.HtmlDecode(data);
			long openTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"), out openTime))
					return 3;
//				var openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"));
			openTime = (long)(openTime / 1000);
			long endTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "\"endTimeMs\":(\\d+)"), out endTime))
					return 3;				
			endTime = (long)(endTime / 1000);
			var programTime = util.getUnixToDatetime(endTime) - util.getUnixToDatetime(openTime);
			long releaseTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "\"releaseTimeMs\":(\\d+)"), out releaseTime))
					return 3;				
			releaseTime = (long)(releaseTime / 1000);
			long serverTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "\"serverTimeMs\":(\\d+)"), out serverTime))
					return 3;				
			var jisa = util.getUnixToDatetime((long)(serverTime / 1000)) - DateTime.Now;
			
				
//			util.debugWriteLine(data + util.getMainSubStr(isSub, true));
			
			recFolderFileInfo = getRecFolderFileInfo(data, type);
			//if (!isSub) {
				timeShiftConfig = null;
				if (!isLive && !isRtmp) {
					if (rm.ri != null) timeShiftConfig = rm.ri.tsConfig;
					if (rm.argTsConfig != null) {
						timeShiftConfig = getReadyArgTsConfig(rm.argTsConfig.clone(), recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], releaseTime);
					} else {
						timeShiftConfig = getTimeShiftConfig(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, releaseTime);
						if (timeShiftConfig == null) return 2;
						
					}
				}
				
				if (!rm.isPlayOnlyMode) {
		//			util.debugWriteLine("rm.rfu " + rm.rfu.GetHashCode() + " rfu " + rfu.GetHashCode() + util.getMainSubStr(isSub, true));
		//			if (recFolderFile == null)
						recFolderFile = getRecFilePath(releaseTime, isRtmp);
					if (recFolderFile == null || recFolderFile[0] == null) {
						//パスが長すぎ
						rm.form.addLogText("パスに問題があります。 " + recFolderFile[1]);
						util.debugWriteLine("too long path? " + recFolderFile[1]);
						return 2;
					}
				} else recFolderFile = new string[]{"", "", ""};
					
				//display set
				var b = new RecordStateSetter(rm.form, rm, rfu, !isLive, true, recFolderFile, rm.isPlayOnlyMode, false);
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
			//} else {
			//	recFolderFile = new string[]{"", "", ""};
			//}
			
			for (int i = 0; i < recFolderFile.Length; i++)
				util.debugWriteLine("recd " + i + " " + recFolderFileInfo[i]);
			var userId = util.getRegGroup(res, "\"user\"\\:\\{\"user_id\"\\:(.+?),");
			var isPremium = res.IndexOf("\"member_status\":\"premium\"") > -1;
			wi = new WatchingInfo(watchingRes);
			var jrp = new JikkenRecordProcess(container, recFolderFile, rm, rfu, this, openTime, !isLive, lvid, timeShiftConfig, userId, isPremium, programTime, wi, releaseTime, false, isRtmp);
			rm.wsr = jrp;
			try {
				jrp.start();
				rm.wsr = null;
				//if (jrp.isEndProgram)
				if (rm.rfu != rfu) return 1;
				return 3;
			} catch (Exception e) {
				util.debugWriteLine("jsp start exception " + e.Message + e.StackTrace);
			}
			return 1;
		}
			
		public int getPageType() {
			if (status == "RELEASED") return 5;
			if (!isLive && !isTimeshiftEnabled) return 2;
			
			if (availableQualities == null && 
			    	!setAvailableQuality()) return 5;
			requestQuality = getBestGettableQuolity();
			
			var _res = getWatching();
			_res.Wait();
			watchingRes = _res.Result;
			util.debugWriteLine("watching res " + watchingRes);
			
			var ret = getPageTypeFromWatching(watchingRes);
			util.debugWriteLine("pagetype cas " + ret);
			return ret;
		}
		private bool setAvailableQuality() {
			var url = "https://api.cas.nicovideo.jp/v1/services/live/programs/" + lvid + "/watching-qualities";
			var wc = new WebHeaderCollection();
			var res = util.getPageSource(url, ref wc, container);
			if (res == null) return false;
			var list = util.getRegGroup(res, "availableQualities\"\\:\\[(.+?)\\]");
			if (list == null) return false;
			list = list.Replace("\"", "");
			//var quaLityList = list.Split(',');
			//quaLityList.Where((x) => x.IndexOf("broadcaster") == -1));
			streamCapacity = list.Split(',')[0];
			
//			list = list.Replace("low", "super_low").Replace("middle", "low").Replace("superhigh", "high").Replace("ultrahigh", "super_high").Replace("high", "normal");
			availableQualities = list.Split(','); 
			
			return true;
		}
		private string getBestGettableQuolity() {
			//var qualityList = new string[] {"abr", "super_high", "high",
			//	"normal", "low", "super_low"};
			var qualityList = new string[] {"auto", "ultrahigh", "superhigh",
				"high", "middle", "low"};
			//var gettableList = util.getRegGroup(msg, "\"qualityTypes\"\\:\\[(.+?)\\]").Replace("\"", "").Split(',');
			var gettableList = availableQualities;
			Array.Resize(ref gettableList, gettableList.Length + 1);
			gettableList[gettableList.Length - 1] = "auto";
			//var ranks = config.get("qualityRank").Split(',');
			var ranks = (rm.ri == null) ? (rm.cfg.get("qualityRank").Split(',')) :
					rm.ri.qualityRank;
			
			var bestGettableQuality = "abr";
			foreach(var r in ranks) {
				var q = qualityList[int.Parse(r)];
				if (gettableList.Contains(q)) {
					bestGettableQuality = q;
					break;
				}
			}
			//bestGettableQuality.Replace("super_low", "low").Replace("low", "middle").Replace("normal", "high").Replace("high", "super_high");
			return bestGettableQuality;
		}
		async public Task<string> getWatching() {
			util.debugWriteLine("watching post");
			try {
				var handler = new System.Net.Http.HttpClientHandler();
				handler.UseCookies = true;
				handler.CookieContainer = container;
				var http = new System.Net.Http.HttpClient(handler);
				handler.UseProxy = false;
				http.DefaultRequestHeaders.Add("X-Frontend-Id", "91");
				http.DefaultRequestHeaders.Add("X-Connection-Environment", "ethernet");
				http.DefaultRequestHeaders.Add("Host", "api.cas.nicovideo.jp");
				http.DefaultRequestHeaders.Add("User-Agent", util.userAgent);
				http.DefaultRequestHeaders.Add("Accept", "application/json");
				
//				var contentStr = "{\"actionTrackId\":\"" + actionTrackId + "\",\"streamProtocol\":\"rtmp\",\"streamQuality\":\"" + requestQuality + "\"";
				var contentStr = "{\"actionTrackId\":\"" + actionTrackId + "\",\"streamProtocol\":\"https\",\"streamQuality\":\"" + requestQuality + "\"";
				if (streamCapacity != "ultrahigh") contentStr += ", \"streamCapacity\":\"" + streamCapacity + "\"";
				if (isLive) contentStr += ",\"isBroadcaster\":" + isBroadcaster.ToString().ToLower() + ",\"isLowLatencyStream\":false";
				contentStr += "}";
				util.debugWriteLine(contentStr);
				
				var content = new System.Net.Http.StringContent(contentStr);
				content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

				http.Timeout = TimeSpan.FromSeconds(5);
				var _t = http.PostAsync(watchingUrl, content);
				_t.Wait();
				var _res = _t.Result;
				var res = await _res.Content.ReadAsStringAsync();
	//			var a = _res.Headers;
				
	//			if (res.IndexOf("login_status = 'login'") < 0) return null;
				
//				cc = handler.CookieContainer;
				
				return res;
//				return cc;
			} catch (Exception e) {
				util.debugWriteLine("get watching exception " + e.Message+e.StackTrace);
				return null;
			}
		}
		async public Task<string> getWatchingPut() {
			util.debugWriteLine("watching put");
			for (var i = 0; i < 3; i++) {
				try {
					var handler = new System.Net.Http.HttpClientHandler();
					handler.UseCookies = true;
					handler.CookieContainer = container;
					var http = new System.Net.Http.HttpClient(handler);
					handler.UseProxy = false;
					http.DefaultRequestHeaders.Add("X-Frontend-Id", "91");
					http.DefaultRequestHeaders.Add("X-Connection-Environment", "ethernet");
					http.DefaultRequestHeaders.Add("Host", "api.cas.nicovideo.jp");
					http.DefaultRequestHeaders.Add("User-Agent", util.userAgent);
					http.DefaultRequestHeaders.Add("Accept", "application/json");
					
					var contentStr = "{\"actionTrackId\":\"" + actionTrackId + "\",\"isBroadcaster\":\"" + isBroadcaster.ToString().ToLower() + "\"}";
					
					http.Timeout = TimeSpan.FromSeconds(5);
					
					var content = new System.Net.Http.StringContent(contentStr);
					content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
	
					//var _res = await http.PutAsync(watchingUrl, content);
					
						
					var t = http.PutAsync(watchingUrl, content);
					t.Wait();
					var _res = t.Result;
					var res = await _res.Content.ReadAsStringAsync();
		//			var a = _res.Headers;
					
		//			if (res.IndexOf("login_status = 'login'") < 0) return null;
					
	//				cc = handler.CookieContainer;
					
					return res;
	//				return cc;
				} catch (Exception e) {
					util.debugWriteLine("watching put exception " + e.Message+e.StackTrace + e.Source + e.TargetSite);
//					return null;
				}
			}
			return null;
		}
		async public Task<string> sendComment(string str, bool is184) {
			try {
				var url = "https://api.cas.nicovideo.jp/v1/services/live/programs/" + lvid + "/comments";
				var handler = new System.Net.Http.HttpClientHandler();
				handler.UseCookies = true;
				handler.CookieContainer = container;
				var http = new System.Net.Http.HttpClient(handler);
				handler.UseProxy = false;
				http.DefaultRequestHeaders.Add("X-Frontend-Id", "91");
				http.DefaultRequestHeaders.Add("X-Connection-Environment", "ethernet");
				http.DefaultRequestHeaders.Add("Host", "api.cas.nicovideo.jp");
				http.DefaultRequestHeaders.Add("User-Agent", util.userAgent);
				http.DefaultRequestHeaders.Add("Accept", "application/json");
				
				var _184 = (is184) ? "184 " : "";
				var vpos = (int)(((TimeSpan)(DateTime.Now - wi.roomSince)).TotalMilliseconds / 10);
				var contentStr = "{\"message\":\"" + str + "\",\"command\":\"" + _184 + "\",\"multi\":[],\"vpos\":\"" + vpos + "\"}";
				
				http.Timeout = TimeSpan.FromSeconds(5);
				
				var content = new System.Net.Http.StringContent(contentStr);
				content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

				var _res = await http.PostAsync(url, content);
				var res = await _res.Content.ReadAsStringAsync();
	//			var a = _res.Headers;
				
	//			if (res.IndexOf("login_status = 'login'") < 0) return null;
				
//				cc = handler.CookieContainer;
				
				return res;
//				return cc;
			} catch (Exception e) {
				util.debugWriteLine(e.Message+e.StackTrace);
				return null;
			}
		}
		private int getPageTypeFromWatching(string res) {
			if (res.IndexOf("\"status\":201") > -1) return (isLive) ? 0 : 7;
			else if (res.IndexOf("\"errorCode\":\"MEMBER_ONLY\"") > -1) return 4;
			else if (res.IndexOf("\"errorCode\":\"EXPIRED_REGULAR_USER\"") > -1) return 2;
			else if (res.IndexOf("\"errorCode\":\"EXPIRED_ARCHIVE_WATCH\"") > -1) return 2;
			else if (res.IndexOf("\"errorCode\":\"INTERNAL_SERVER_ERROR\"") > -1) return 5;
			else if (res.IndexOf("\"errorCode\":\"INVALID_PARAMETER\"") > -1) return 5;
			else if (res.IndexOf("\"errorCode\":\"NOT_PLAYABLE\"") > -1) return 2;
			else if (res.IndexOf("errorMessage\":\"program ended.\"") > -1) return 2;
//			else if (res.IndexOf("errorMessage\":\"Bad request") > -1) return 5;
			rm.form.addLogText(res);
			return 5;
		}
		private string getActionTrackId() {
			//var s = new System.Security.Cryptography.SHA256Managed();
			//s.ComputeHash(
			var t = (long)(((TimeSpan)(DateTime.Now - new DateTime(1970, 1, 1))).TotalSeconds * 1000);
			return Guid.NewGuid().ToString("N").Substring(0,10) + "_" + t;
		}
		private string[] getRecFolderFileInfo(string data, string type) {
			string host, group, title, communityNum, userId;
			host = group = title = communityNum = userId = null;
			if (type == "official") {
				host = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
				if (util.getRegGroup(data, "(\"socialGroup\".\\{\\},)") != null) host = "公式生放送";
	//			if (host == null) host = "official";
				group = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.+?)\"");
				if (group == null) group = "official";
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
//					var a = new System.Net.WebHeaderCollection();
//					var apiRes = util.getPageSource(url + "/programinfo", ref a, container);
				
				} else {
					var isCommunity = util.getRegGroup(data, "providerType\":\"(community)\",\"title\"") != null;
		//			host = util.getRegGroup(res, "provider......name.....(.*?)\\\\\"");
					//group = util.getRegGroup(data, "\"community\".+?\"name\".\"(.+?)\"");
		//			group = util.uniToOriginal(group);
		//			group = util.getRegGroup(res, "communityInfo.\".+?title.\"..\"(.+?).\"");
					host = util.getRegGroup(data, "\"broadcaster\".+?\"nickname\".\"(.+?)\"");
		//			System.out.println(group);
		//			host = util.uniToOriginal(host);
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\:\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\:\\\"(.*?)\\\"");
		//			title = util.getRegGroup(res, "\\\\\"programHeader\\\\\":\\{\\\\\"thumbnailUrl.+?\\\\\"title\\\\\":\\\\\"(.*?)\\\\\"");
					title = util.getRegGroup(data, "providerType\":\"(community|channel)\",\"title\":\"(.+?)\",", 2);
		//			communityNum = util.getRegGroup(res, "socialGroup: \\{[\\s\\S]*registrationUrl: \"http://com.nicovideo.jp/motion/(.*?)\\?");
					communityNum = util.getRegGroup(data, "\"community\"\\:\\{\"id\"\\:\"(.+?)\"");
		//			community = util.getRegGroup(res, "socialGroup\\:)");
					group = getCommunityName(communityNum);
					userId = util.getRegGroup(data, "\"broadcaster\"..\"id\".\"(.+?)\"");
					//userId = (isChannel) ? "channel" : (util.getRegGroup(data, "supplier\":{\"name\".+?pageUrl\":\"http://www.nicovideo.jp/user/(\\d+?)\""));
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
		private string getCommunityName(string communityNum) {
			var isChannel = communityNum.IndexOf("ch") > -1;
			var url = (isChannel) ? 
				("https://ch.nicovideo.jp/" + communityNum) :
				("https://com.nicovideo.jp/community/" + communityNum);
			
			var wc = new WebHeaderCollection();
			var res = util.getPageSource(url, ref wc, container);
//			util.debugWriteLine(container.GetCookieHeader(new Uri(url)) + util.getMainSubStr(isSub, true));
			
			if (res == null) {
				url = (isChannel) ? 
					("https://ch.nicovideo.jp/" + communityNum) :
					("https://com.nicovideo.jp/motion/" + communityNum);
				res = util.getPageSource(url, ref wc, container);
			}
			if (res == null) return "com";
			var title = (isChannel) ? 
				util.getRegGroup(res, "<meta property=\"og\\:title\" content=\"(.+?) - ニコニコチャンネル") :
				util.getRegGroup(res, "<meta property=\"og\\:title\" content=\"(.+?)-ニコニコミュニティ\"");
			return title;
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
		public string[] getRecFilePath(long releaseTime, bool isRtmp) {
			util.debugWriteLine(releaseTime + " c " + recFolderFileInfo[0] + " b " + !isLive + " a " + timeShiftConfig);
			return util.getRecFolderFilePath(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, !isLive, timeShiftConfig, releaseTime, isRtmp);
		}
		private TimeShiftConfig getReadyArgTsConfig(
				TimeShiftConfig _tsConfig, string host, 
				string group, string title, string lvId, 
				string communityNum, string userId, 
				long _openTime) {
			var segmentSaveType = rm.cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, rm.cfg, _openTime);
			util.debugWriteLine("timeshift lastfile " + lastFile);
			util.debugWriteLine("arg tsconfig vpos " + _tsConfig.isVposStartTime);
			
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
			return _tsConfig;
		}
		
		*/
	}
}

