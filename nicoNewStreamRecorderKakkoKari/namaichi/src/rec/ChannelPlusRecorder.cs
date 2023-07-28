/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2022/05/03
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using namaichi.info;
using namaichi.utility;
using Newtonsoft.Json;
using WebSocket4Net;

namespace namaichi.rec
{
	/// <summary>
	/// Description of ChannelPlusRecorder.
	/// </summary>
	public class ChannelPlusRecorder : IRecorderProcess
	{
		MainForm form = null;
		//RecordingManager rm = null;
		Record rec = null;
		M3u8Writer writer = null;
		string url = null;
		string id = null;
		Curl videoPageCurl = new Curl();
		bool isPlayOnlyMode = false;
		//RecordFromUrl rfu = null;
		
		string videoId = null;
		string fcId = null;
		//string audienceToken = null;
		string sessionId = null;
		//string auth = null;
		KeyValuePair<string, string>? hlsUrl = null;
		//RecordInfo ri = null;
		//CookieContainer container = null;
		
		public bool isEndProgram = false;
		//private bool isRetry = true;

		//private bool isTimeShiftCommentGetEnd = false;
		private DateTime lastEndProgramCheckTime = DateTime.Now;
		//TimeShiftCommentGetter_chPlus tscg = null;
		public RedirectLogin rl = null;
		
		public ChannelPlusRecorder(string url, MainForm form, RecordingManager rm, string id, RecordFromUrl rfu)
		{
			videoPageCurl = new Curl();
			
			this.url = url;
			//id = util.getRegGroup(url, "(live|video)*/(sm[^\\?]+)", 2);
			this.id = id;
			this.form = form;
			this.rm = rm;
			this.rfu = rfu;
			try {
				commentReplaceList = JsonConvert.DeserializeObject<List<string[]>>(rm.cfg.get("CommentReplaceText"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			
			container = new CookieContainer();
			
		}
		public int run() {
			if (!videoPageCurl.isInitialized()) {
				rm.form.addLogText("DLLファイルの読み込みに失敗しました。Visual Studio2015のVisual C++再頒布可能パッケージをインストールすると読み込めるようになるかもしれません。");
				rm.form.addLogText("https://www.microsoft.com/ja-jp/download/details.aspx?id=48145");
				return 2;
			}
			
			if (rm.cfg.get("EngineMode") == "1" || rm.cfg.get("EngineMode") == "2") {
				rm.form.addLogText("ニコニコチャンネルプラスは標準のHLS録画エンジンのみ使用できます。");
				return 2;
			}
			
			var h = util.getHeader(null, "https://nicochannel.jp", null);
			var fcName = util.getRegGroup(id, "(.+?)/");
			var channelsPageUrl = "https://nfc-api.nicochannel.jp/fc/content_providers/channels";
			var channelsRes = videoPageCurl.getStr(channelsPageUrl, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS);
			fcId = util.getRegGroup(channelsRes, fcName + "\",.+?\"id\":(\\d+)");
			if (fcId == null) {
				form.addLogText("チャンネルIDが取得できませんでした " + fcName + " " + channelsRes);
				return 2;
			}
			while (rm.rfu == rfu) {
				videoId = util.getRegGroup(id, ".+/(.+)");
				var videoPageUrl = "https://nfc-api.nicochannel.jp/fc/video_pages/" + videoId;
				h.Add("fc_site_id", fcId);
				h.Add("fc_use_device", "null");
				var vpRes = videoPageCurl.getStr(videoPageUrl, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS);
				util.debugWriteLine(vpRes);
				if (vpRes == null) {
					Thread.Sleep(30000);
					continue;
				}
				//setCookie(ri.si.recFolderFileInfo[4], ri.si.recFolderFileInfo[5]);
				
				var pageType = getPageType(vpRes);
				var isEnded = vpRes.IndexOf("\"live_finished_at\":null") == -1 || vpRes.IndexOf("\"live_scheduled_start_at\":null") > -1;
				var _si = new StreamInfo(url, videoId, isEnded, true);
				_si.set(vpRes);
				_si.getTimeInfo();
				
				ri = new RecordInfo(_si, pageType, false);
				setFormInfo(vpRes);
				 
				if (pageType == 0 || pageType == 7) {
					return _rec(vpRes);
				} else if (pageType == 2) {
					form.addLogText("放送を取得できませんでした。");
					return 2;
				} else if (pageType == 5) {
					form.addLogText("放送を取得できませんでした。");
					Thread.Sleep(10000);
				} else {
					form.addLogText("放送を取得できませんでした。");
					return 2;
				}
			}
			return 2;
		}
		public int getPageType(string vpRes) {
			if (vpRes.IndexOf("\"authenticated_url\":\"https://") > -1) 
				return vpRes.IndexOf("\"live_finished_at\":\"2") > -1 ? 7 : 0;
			return 2;
		}
		int _rec(string res) {
			rl = new RedirectLogin(this, ri.si);
			var _auth = rl.getAuth();
			util.debugWriteLine("_rec getAuth " + _auth);
			if (_auth == null) form.addLogText("ニコニコチャンネルプラスに非ログインで開始を試みます");
			
			
			ri.set(false, rm.cfg, rm.form);
			if (!ri.setTimeShiftConfig(rm, false)) {
				util.debugWriteLine("not setTimeshiftConfig");
				return 2;
			}
				
			if (!ri.setRecFolderFile(rm)) {
				util.debugWriteLine("not setRecFolderFile");
				return 2;
			}
				
			var r = setParams(res);
			if (!r) {
				form.addLogText("接続情報を取得できませんでした");
				return 2;
			}
			
			var rt = Task.Run(() => record());
			var rs = Task.Run(() => saveComment(res));
			var ds = Task.Run(() => displaySchedule());
			while (rm.rfu == rfu && IsRetry) {
				Thread.Sleep(1000);
				if (util.isTaskEnd(rt) && util.isTaskEnd(rs)) break;
			}
			
			try {
				if (writer != null) writer.stop();
				
				while (rm.rfu == rfu && tscg != null 
				       && !tscg.isEnd && !util.isTaskEnd(rs))
					Thread.Sleep(1000);
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			return 0;
		}
		bool setParams(string res) {
			/*
			audienceToken = sessionId = null;
			hlsUrl = null;
			audienceToken = getAudienceToken();
			Debug.WriteLine("audience token " + audienceToken);
			if (audienceToken == null) {
				util.debugWriteLine("audience_tokenが取得できませんでした");
				return false;
			}
			*/
			sessionId = getSessionId();
			Debug.WriteLine("session_id " + sessionId);
			if (sessionId == null) {
				util.debugWriteLine("session_idが取得できませんでした");
				return false;
			}
			
			hlsUrl = getM3u8Url(res, sessionId);
			Debug.WriteLine("hlsUrl " + hlsUrl);
			if (hlsUrl == null) {
				util.debugWriteLine("M3U8のURLが取得できませんでした");
				return false;
			}
			return true;

		}
		/*
		string getAudienceToken() {
			for (var i = 0; i < 10; i++) {
				try {
					var url = "https://nfc-api.nicochannel.jp/fc/video_pages/" + id + "/audience_token";
					var h = Curl.getDefaultHeaders("https://nicochannel.jp");
					var res = videoPageCurl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS);
					return util.getRegGroup(res, "\"access_token\":\"(.+?)\"");
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					Thread.Sleep(10000);
				}
			}
			return null;
		}
		*/
		string getSessionId() {
			for (var i = 0; i < 10; i++) {
				try {
					var url = "https://nfc-api.nicochannel.jp/fc/video_pages/" + videoId + "/session_ids";
					var h = util.getHeader(null, "https://nicochannel.jp", null);
					h["Content-Type"] = "application/json";
					h["Accept"] = "application/json, text/plain, */*";
					h.Add("fc_site_id", fcId);
					h.Add("fc_use_device", "null");
					var _auth = rl.getAuth();
					if (_auth != null) h.Add("Authorization", "Bearer " + _auth);
					var res = videoPageCurl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", "{}");
					return util.getRegGroup(res, "\"session_id\":\"(.+?)\"");
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					Thread.Sleep(10000);
				}
			}
			return null;
		}
		KeyValuePair<string, string>? getM3u8Url(string res, string sessionId) {
			try {
				var _m3u8Url = util.getRegGroup(res, "\"authenticated_url\":\"(.+?)\"");
				if (_m3u8Url == null) {
					Debug.WriteLine("not found m3u8url");
					return null;
				}
				_m3u8Url = _m3u8Url.Replace("{session_id}", sessionId);
				var h = util.getHeader(null, "https://nicochannel.jp", null);
				var m3u8Res = videoPageCurl.getStr(_m3u8Url, h, CurlHttpVersion.CURL_HTTP_VERSION_3);
				Debug.WriteLine("m3u8 res " + m3u8Res);
				var names = new Regex("RESOLUTION=([^,]+)").Matches(m3u8Res);
				var urls = new Regex("(https://.+)").Matches(m3u8Res);
				if (names.Count != urls.Count) {
					form.addLogText("正しく画質を取得できませんでした");
					return urls.Count == 0 ? null : (KeyValuePair<string, string>?)new KeyValuePair<string, string>("_", urls[0].Value);
				}
				var d = new Dictionary<string, string>();
				for (var i = 0; i < names.Count; i++)
					d.Add(names[i].Value.Replace("RESOLUTION=", ""), urls[i].Value);
				return getBestGettableQuolity(d);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				Thread.Sleep(10000);
			}
			return null;
		}
		private KeyValuePair<string, string>? getBestGettableQuolity(Dictionary <string, string> d) {
			//	{0, "1080p(1920x1080)"},
			//	{1, "360p(640x360)"}, {2, "240p(426x240)"}
			//};
			//var qualityRank = "240p(426x240),1080p(1920x1080),360p(640x360),720p(1280x720)";
			var qualityRank = rm.cfg.get("chPlusQualityRank");
			var qr = qualityRank.Split(',').Select(x => util.getRegGroup(x, "\\((.+?)\\)"));
			                                       
			foreach (var q in qr) {
				foreach (var _d in d) {
					if (_d.Key == q) return _d;
				}
			}
			return null;
		}
		void setFormInfo(string res) {
			if (!string.IsNullOrEmpty(form.getTitleLabelText())) return;
			try {
				var rss = new RecordStateSetter(rm.form, false, isPlayOnlyMode, false, ri.si.isReservation, true);
				Task.Run(() => {
				       	rss.set(ri.si.data + "\"watchPageUrl\":\"" + url + "\"", ri.si.recFolderFileInfo, res);
				});
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
		void record() {
			try {
				var port = int.Parse(rm.cfg.get("localServerPortList"));
				
				writer = new M3u8Writer(rm, rfu, ri);
				writer.run(hlsUrl.Value.Value, hlsUrl.Value.Key, port);
				IsRetry = false;
				isEndProgram = true;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
		void saveComment(string vpRes) {
			var group_id = util.getRegGroup(vpRes, "\"comment_group_id\":\"(.+?)\"");
			//var url = "https://comm-api.sheeta.com/messages.history?limit=120&oldest=2022-04-20T12:03:28.000Z&sort_direction=asc";
			try {
				if (ri.si.isTimeShift) {
					if (tscg == null) 
						tscg = new TimeShiftCommentGetter_chPlus(form, rm, rfu, ri, videoId, group_id, vpRes, true, ri.timeShiftConfig, this, rl);
					tscg.save();
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
		override public void reConnect() {
			
		}
		override public void reConnect(WebSocket ws) {}
		override public string[] getRecFilePath() {
			return ri.getRecFilePath(ri.isRtmp, ri.timeShiftConfig, ri.isFmp4, rm.cfg, rm.form);
		}
		override public void sendComment(string s, bool is184) {}
		override public void resetCommentFile() {
			
		}
		override public void setSync(int no, double second, string m3u8Url) {
			try {
				return;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		override internal bool getStatistics(string lvid, CookieContainer cc, out string visit, out string comment) {
			visit = "0";
			comment = "0";
			var videoPageUrl = "https://nfc-api.nicochannel.jp/fc/video_pages/" + videoId;
			var h = util.getHeader(null, "https://nicochannel.jp");
			var _auth = rl.getAuth();
			if (_auth != null) h.Add("Authorization", "Bearer " + _auth);
			var vpRes = videoPageCurl.getStr(videoPageUrl, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS);
			util.debugWriteLine(vpRes);
			if (vpRes == null) {
				return false;
			}
			var _visit = util.getRegGroup(vpRes, "\"number_of_comments\":(\\d+)");
			var _comment = util.getRegGroup(vpRes, "\"total_views\":(\\d+)");
			if (_visit == null || _comment == null) return false;
			visit = _visit;
			comment = _comment;
			return true;
		}
		override public void stopRecording() {}
		override public void chaseCommentSum() {}
		public void displaySchedule() {
			try {
				DateTime keikaTimeStart = DateTime.MinValue;
				while (rm.rfu == rfu && IsRetry) {
					
					if (rec == null || rec.lastRecordedSeconds == -1) {
						Thread.Sleep(1000);
						continue;
					}
					
					DateTime _keikaTimeStart = (tsHlsRequestTime - tsStartTime - jisa);
			        if (keikaTimeStart == _keikaTimeStart)
			        	_keikaTimeStart = DateTime.MinValue;
			        else keikaTimeStart = _keikaTimeStart;
			        
					var _keikaJikanDt = TimeSpan.FromSeconds(rec.lastRecordedSeconds);
					var keikaJikanH = (int)(_keikaJikanDt.TotalHours);
					var keikaJikan = _keikaJikanDt.ToString("''mm'分'ss'秒'");
					if (keikaJikanH != 0) keikaJikan = keikaJikanH.ToString() + "時間" + keikaJikan;
					
					var timeLabelKeikaH = (int)(_keikaJikanDt.TotalHours);
					var timeLabelKeika = _keikaJikanDt.ToString("''mm':'ss''");
					if (timeLabelKeikaH != 0) timeLabelKeika = timeLabelKeikaH.ToString() + ":" + timeLabelKeika;
					
					var programTimeStrH = (int)(ri.si.programTime.TotalHours);
					var programTimeStr = ri.si.programTime.ToString("''mm':'ss''");
					if (programTimeStrH != 0) programTimeStr = programTimeStrH.ToString() + ":" + programTimeStr;
					
					//var keikaJikan = _keikaJikanDt.ToString("H'時間'm'分's'秒'");
					//var programTimeStr = programTime.ToString("h'時間'm'分's'秒'");
					rm.form.setKeikaJikan(keikaJikan, timeLabelKeika + "/" + programTimeStr, _keikaJikanDt.ToString("h'時間'mm'分'ss'秒'"), _keikaTimeStart);
					System.Threading.Thread.Sleep(1000);
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
		
	}
	public class chPlusMsgInfo {
		public string created_at; //"2022-00-00T00:00:00.000Z"
		public string end_time_in_seconds; //null
		public string group_id; //"00000000-0000-0000-0000-aaaaa0000000"
		public string id; //"00000000-0000-0000-0000-aaaaa0000000"
		public List<string> mentions; //[]
		public string message; //"message"
		public string nickname; //"nickname"
		public int playback_time; //120
		public bool priority; //false
		public string sender_id; //"00000000-0000-0000-0000-aaaaa0000000"
		public string sent_at; //"2022-00-00T00:00:00.000Z"
		public string updated_at; //"2022-00-00T00:00:00.000Z"
		public ChatInfo getChat() {
			var _xml = new XDocument();
			_xml.Add(new XElement("chat"));
			
			_xml.Root.SetElementValue("thread", id);
			_xml.Root.SetElementValue("vpos", playback_time * 100);
			_xml.Root.SetElementValue("date", util.getUnixTime(DateTime.Parse(sent_at)));
			_xml.Root.SetElementValue("date_usec", util.getUnixTime(DateTime.Parse(sent_at)));
			_xml.Root.SetElementValue("mail", "");
			_xml.Root.SetElementValue("user_id", sender_id);
			_xml.Root.SetElementValue("premium", priority ? "3" : "1");
			_xml.Root.SetElementValue("content", message);
			//_xml.Root.SetAttributeValue("anonymity", ""); 184 = 1
			_xml.Root.Add(message);
			var c = new ChatInfo(_xml);
			
			return c;
		}
	}
	public class M3u8Writer {
		RecordingManager rm;
		RecordFromUrl rfu;
		RecordInfo ri;
		Curl curl = new Curl();
		TcpListener listener = null;
		
		public M3u8Writer(RecordingManager rm, RecordFromUrl rfu, RecordInfo ri) {
			this.rm = rm;
			this.rfu = rfu;
			this.ri = ri;
		}
		public void run(string segUrl, string quality, int port) {
			util.debugWriteLine("m3u8writer " + segUrl);
			try {
				var res = curl.getStr(segUrl, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_3);
				if (res == null) return;
				res = res.Replace("\r", "");
				
				var lines = res.Split('\n');
				string[] tsUrlList = null;
				var _lines = getTimeFilterRes(res, lines, out tsUrlList);
				if (_lines.Length != 0) lines = _lines;
				
				rm.form.addLogText("マニフェストファイルを出力します(画質:" + quality + ")");
				//Task.Run(() => runServer(port, lines));
				runServer(port, lines, tsUrlList);
				 
				/*
				var tsNum = lines.Count();
				util.debugWriteLine("tsnum " + tsNum);
				var f = new Regex("(_\\d+h\\d+m\\d+s)_").Replace(ri.recFolderFile[1], "") + ".m3u8";
				
				
				bool isFirstWrote = true;
				for (var i = 0; i < tsNum; i++) {
					bool isWroteTsLine;
					var isEnd = writeFile(lines, i, tsNum, f, out isWroteTsLine, port, lastUrl);
					if (rm.rfu != rfu) break;
					if (isFirstWrote && isWroteTsLine) {
						Thread.Sleep(30000);
						isFirstWrote = false;
					}
					if (isEnd) {
						
						File.Delete(f);
						return;
					}
				}
				*/
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			
		}
		bool writeM3u8(string[] lines, int i, int port, string lastUrl, Stream sw) {
			util.debugWriteLine("m3u8 writefile lines.len " + lines.Length + " i " + i);
			//isWroteTsLine = false;
			var tsCount = 0;
			var isWrite = false;
			string lastWriteUrl = null;
			var startSeconds = 0.0;
			//using (var sw = new StreamWriter(f, false)) {
			//var buf = "HTTP/1.1 200 OK\r\n";
			//buf += "content-type: application/vnd.apple.mpegurl\r\n";
			
			var bodyBuf = "";

				foreach (var l in lines) {
					if (rm.rfu != rfu) break;
					
					if (!l.StartsWith("#EXTINF")) {
						if (l.StartsWith("#EXT-X-ENDLIST")) {
							if (lastWriteUrl != lastUrl) continue;
							//sw.WriteLine(l);
							bodyBuf += l + "\r\n";
							//return true;
						}
						else {
							//if (l.StartsWith("#EXT-X-PLAYLIST-TYPE:VOD")) continue;
							if (l.StartsWith("#EXT-X-MEDIA-SEQUENCE:")) {
								//sw.WriteLine("#EXT-X-MEDIA-SEQUENCE:" + i);
								bodyBuf += "#EXT-X-MEDIA-SEQUENCE:" + i + "\r\n";
								util.debugWriteLine("#EXT-X-MEDIA-SEQUENCE:" + (i - 28));
							} else if (l.StartsWith("#EXT-X-KEY:METHOD=")) {
								var uri = util.getRegGroup(l, "URI=\"(.+)\"");
								if (uri == null) continue;
								//sw.WriteLine("#EXT-X-KEY:METHOD=AES-128,URI=\"http://127.0.0.1:" + port.ToString()
								//             + "/" + HttpUtility.UrlEncode(uri));
								bodyBuf += "#EXT-X-KEY:METHOD=AES-128,URI=\"http://127.0.0.1:" + port.ToString()
								             + "/" + HttpUtility.UrlEncode(uri) + "\r\n";
								util.debugWriteLine("#EXT-X-KEY:METHOD=AES-128,URI=\"http://127.0.0.1:" + port.ToString() + "/" + HttpUtility.UrlEncode(uri));
							} else if (l.StartsWith("http") && isWrite) {
								//sw.WriteLine("http://127.0.0.1:" + port.ToString() + "/" + HttpUtility.UrlEncode(l));
								bodyBuf += "http://127.0.0.1:" + port.ToString() + "/" + HttpUtility.UrlEncode(l) + "\r\n";
								util.debugWriteLine("http://127.0.0.1:" + port.ToString() + "/" + HttpUtility.UrlEncode(l));
								lastWriteUrl = l;
							} 
							else if (!l.StartsWith("http")) {
								//sw.WriteLine(l);
								bodyBuf += l + "\r\n";
								util.debugWriteLine(l);
							}
						}
					} else {
						var _t = util.getRegGroup(l, "#EXTINF:(([^,])+)");
						var t = _t == null ? 10 : double.Parse(_t);
						
						if (tsCount >= i && tsCount < i + 3) {
							//sw.WriteLine(l);
							bodyBuf += l + "\r\n";
							util.debugWriteLine(l);
							//if (!isWroteTsLine) 
								setKeikaJikan(startSeconds);
							isWrite = true;
							//isWroteTsLine = true;
						} else isWrite = false;
						startSeconds += t;
						tsCount++;
					}
				}
			//}
			//sw.WriteLine("");
			//sw.Write(buf);
			var bodyB = Encoding.ASCII.GetBytes(bodyBuf);
			var buf = "HTTP/1.1 200 OK\r\n";
			buf += "content-type: application/vnd.apple.mpegurl\r\n";
			//buf += "Connection: Keep-Alive\r\n";
			buf += "Connection: close\r\n";
			buf += "Keep-Alive: timeout=15, max=100\r\n";
			buf += "Content-Length: " + bodyB.Length + "\r\n";
			buf += "\r\n";
			buf += bodyBuf;
			
			var b = Encoding.ASCII.GetBytes(buf);
			sw.Write(b, 0, b.Length);
			//if (isWroteTsLine)
				//Thread.Sleep(string.IsNullOrEmpty(tsSeconds) ? 5000 : int.Parse(tsSeconds) * 1000 / 2);
				if (lastWriteUrl == lastUrl)
					util.debugWriteLine("last write url");
			//Thread.Sleep((lastWriteUrl == lastUrl) ? 20000 : 5000);
			return lastWriteUrl == lastUrl;
		}
		void setKeikaJikan(double startSeconds) {
			var _keikaJikanDt = TimeSpan.FromSeconds(startSeconds);
			var keikaJikanH = (int)(_keikaJikanDt.TotalHours);
			var keikaJikan = _keikaJikanDt.ToString("''mm'分'ss'秒'");
			if (keikaJikanH != 0) keikaJikan = keikaJikanH.ToString() + "時間" + keikaJikan;
			
			var timeLabelKeikaH = (int)(_keikaJikanDt.TotalHours);
			var timeLabelKeika = _keikaJikanDt.ToString("''mm':'ss''");
			if (timeLabelKeikaH != 0) timeLabelKeika = timeLabelKeikaH.ToString() + ":" + timeLabelKeika;
			
			var programTimeStrH = (int)(ri.si.programTime.TotalHours);
			var programTimeStr = ri.si.programTime.ToString("''mm':'ss''");
			if (programTimeStrH != 0) programTimeStr = programTimeStrH.ToString() + ":" + programTimeStr;
			
			rm.form.setKeikaJikan(keikaJikan, timeLabelKeika + "/" + programTimeStr, _keikaJikanDt.ToString("h'時間'mm'分'ss'秒'"), DateTime.Now);
					
		}
		string[] getTimeFilterRes(string res, string[] lines, out string[] tsUrlList) {
			tsUrlList = null;
			try {
				var timeList = lines.Where(x => x.StartsWith("#EXTINF:"))
						.Select(x => util.getRegGroup(x, "#EXTINF:(([^,])+)")).ToArray();
				var tsList = lines.Where(x => x.StartsWith("http")).ToArray();
				
				util.debugWriteLine(timeList.Length + " " + tsList.Length);
				if (timeList.Length != tsList.Length) return lines;
				
				var startSeconds = 0.0;
				var wList = new List<KeyValuePair<string, string>>();
				for (var i = 0; i < timeList.Length; i++) {
					var isTimeOk = (startSeconds + double.Parse(timeList[i]) > ri.timeShiftConfig.timeSeconds)
							&& (ri.timeShiftConfig.endTimeSeconds == 0 || 
						    	startSeconds < ri.timeShiftConfig.endTimeSeconds);
					startSeconds += double.Parse(timeList[i]);
					if (isTimeOk) wList.Add(new KeyValuePair<string, string>(timeList[i], tsList[i]));
				}
				
				var ret = lines.Where(x => !x.StartsWith("#EXTINF:") &&
				                      !x.StartsWith("http") && !x.StartsWith("#EXT-X-ENDLIST")).ToList();
				foreach (var t in wList) {
					ret.Add("#EXTINF:" + t.Key);
					ret.Add(t.Value);
				}
				ret.Add("#EXT-X-ENDLIST");
				tsUrlList = wList.Select(x => x.Value).ToArray();
				return ret.ToArray();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return lines;
			}
		}
		void runServer(int port, string[] lines, string[] tsUrlList) {
			if (tsUrlList.Length == 0) return;
			//while (rm.rfu == rfu) {
				try {
					listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
					listener.Start();
					rm.form.addLogText("http://127.0.0.1:" + port.ToString() + "/segment.m3u8");
					
					var startTime = DateTime.Now;
					var i = 0;
					Task.Run(() => {
			         	for (i = 0; i < tsUrlList.Length; i++) {
							Thread.Sleep(i == 0 || i == tsUrlList.Length - 1 ? 25000 : 5000);
							if (rm.rfu != rfu) break;
							util.debugWriteLine("sleep i " + i + " tsUrlList.Len " + tsUrlList.Length);
			         	}
					    util.debugWriteLine("m3u8 loop end");
						stop();
						i = -1;
					});
					while (rm.rfu == rfu && i != -1) {
						try {
							using (var client = listener.AcceptTcpClient())
							using (var sr = new StreamReader(client.GetStream()))
							using (var sw = new StreamWriter(client.GetStream())) {
								var buf = new List<string>();
								while (true) {
									var l = sr.ReadLine();
									buf.Add(l);
									util.debugWriteLine("ab " + l);
									if (l == null || l.Length == 0) break;
									if (l.StartsWith("GET ")) {
										if (l.IndexOf("/segment.m3u8") > -1) {
											writeM3u8(lines, i, port, tsUrlList[tsUrlList.Length - 1], client.GetStream());
											break;
										} else {
											var _url = util.getRegGroup(l, "(http.+) ");
											if (_url == null) break;
											var url = HttpUtility.UrlDecode(_url);
											util.debugWriteLine("url " + url);
											
											var ver = url.IndexOf("key?") > -1 ? CurlHttpVersion.CURL_HTTP_VERSION_3 : CurlHttpVersion.CURL_HTTP_VERSION_2TLS;
											string d = null; 
											var b = curl.getBytes(url, util.getHeader(), ver, "GET", d, true);
											client.GetStream().Write(b, 0, b.Length);
										}
									} else {
										util.debugWriteLine("ttt " + l);
									}
								}
							}
						} catch (Exception ee) {
							util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
						}
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					Thread.Sleep(1000);
				} finally {
					stop();
				}
			//}
			rm.form.addLogText("マニフェストファイルの出力を完了しました。");
			Thread.Sleep(20000);
		}
		public void stop() {
			try {
				if (listener != null) listener.Stop();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
	}
}