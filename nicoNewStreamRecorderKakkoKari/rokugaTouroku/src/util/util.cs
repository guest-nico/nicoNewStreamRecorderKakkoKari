using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using namaichi.utility;
using rokugaTouroku.config;
using rokugaTouroku.info;

class app {
	public static void Mains(string[] args) {
		string a = util.getRegGroup("as32df5gh", "\\d([^0-9]).(.)", 1);
		Console.WriteLine(a);
		Console.WriteLine(util.getPath());
		Console.WriteLine(util.getTime());
		Console.WriteLine(util.getJarPath());
		Console.WriteLine(util.getOkFileName(".a\\\"aa|a"));
		//Console.WriteLine(util.getRecFolderFilePath("host", "group", "title", "lvid", "comnum")[0]);
		//Console.WriteLine(util.getRecFolderFilePath("host", "group", "title", "lvid", "comnum")[1]);
	}
}
class util {
	public static string versionStr = "ver0.1.3.10.78";
	public static string versionDayStr = "2023/11/25";
	public static string osName = null;
	public static bool isCurl = true;
	public static bool isWebRequestOk = false;
	
	public static string getRegGroup(string target, string reg, int group = 1) {
		Regex r = new Regex(reg);
		var m = r.Match(target);
//		Console.WriteLine(m.Groups.Count +""+ m.Groups[0]);
		if (m.Groups.Count>group) {
			return m.Groups[group].ToString();
		} else return null;
	}	
	public static string getPath() {
		string p  = System.IO.Path.GetDirectoryName(
			System.IO.Path.GetFullPath(
			System.Reflection.Assembly.GetExecutingAssembly().Location));
//		Console.WriteLine(p);
		return p;
	}
	public static string getTime() {
		return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
		
	}
	public static int getUnixTime() {
		return (int)(((TimeSpan)(DateTime.Now - new DateTime(1970, 1, 1))).TotalSeconds);
	}
	public static String[] getJarPath() {
		bool isTestMode = false;
		
		if (isTestMode) {	
			return new String[]{"C:\\Users\\pc\\desktop", "util", "exe"};
		} else {
//			string f=Environment.GetCommandLineArgs()[0];
			string f = System.Reflection.Assembly.GetExecutingAssembly().Location;
//			util.debugWriteLine(Environment.GetCommandLineArgs().Length);
			f=System.IO.Path.GetFileName(f);

			string withoutKakutyousi = (f.IndexOf(".") < 0) ? f :
					util.getRegGroup(f,"^(.*)\\.");
			string kakutyousi = (f.IndexOf(".") < 0) ? null :
					util.getRegGroup(f,"^.*\\.(.*)");
			
			util.debugWriteLine(getPath() + " " +withoutKakutyousi+" "+kakutyousi);
			//0-dir 1-withoutKakutyousi 2-kakutyousi
			return new String[]{getPath(), withoutKakutyousi, kakutyousi};
		}
	}

	public static string[] getRecFolderFilePath(string host, 
			string group, string title, string lvId, 
			string communityNum, string userId, config cfg, 
			bool isTimeShift, TimeShiftConfig tsConfig) {
		
		host = getOkFileName(host);
		group = getOkFileName(group);
		title = getOkFileName(title);
		
		string[] jarpath = getJarPath();
//		util.debugWriteLine(jarpath);
		//string dirPath = jarpath[0] + "\\rec\\" + host;
		string _dirPath = (cfg.get("IsdefaultRecordDir") == "true") ?
			(jarpath[0] + "\\rec") : cfg.get("recordDir");
		string dirPath = _dirPath;
		
		string sfn = null;
		if (cfg.get("IscreateSubfolder") == "true") {
			sfn = getSubFolderName(host, group, title, lvId, communityNum, userId,  cfg);
			if (sfn.Length > 120) sfn = sfn.Substring(0, 120);
			if (sfn == null) return null;
			dirPath += "/" + sfn;
		}


		var segmentSaveType = cfg.get("segmentSaveType");
		if (cfg.get("EngineMode") != "0") segmentSaveType = "0";
		
		bool _isTimeShift = isTimeShift;
		if (cfg.get("EngineMode") != "0") _isTimeShift = false;

		var name = getFileName(host, group, title, lvId, communityNum,  cfg);
		if (name.Length > 200) name = name.Substring(0, 200);
		
		//長いパス調整
		if (name.Length + dirPath.Length > 234) {
			name = lvId;
			if (name.Length + dirPath.Length > 234 && sfn != null) {
				sfn = sfn.Substring(0, 3);
				dirPath = _dirPath + "/" + sfn;
								
				if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
				if (!Directory.Exists(dirPath)) return null;
				
			}
		}
		if (name.Length + dirPath.Length > 234) return new string[]{null, name + " " + dirPath};
		
		if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
		if (!Directory.Exists(dirPath)) return null;
		
		var files = Directory.GetFiles(dirPath);
		string existFile = null;
		for (int i = 0; i < 1000000; i++) {
			var fName = dirPath + "/" + name + "_" + ((_isTimeShift) ? "ts" : "") + i.ToString();
			util.debugWriteLine(dirPath + " " + fName);
			
			if (!_isTimeShift) {
				if (segmentSaveType == "0" && (File.Exists(fName + ".ts") ||
						File.Exists(fName + ".xml"))) continue;
				else if (segmentSaveType == "1") {
					if (Directory.Exists(fName)) continue;
					Directory.CreateDirectory(fName);
					if (!Directory.Exists(fName)) return null;
				}
				
				
				string[] reta = {dirPath, fName};
				return reta;
			} else {
				if (segmentSaveType == "0") {
					var _existFile = util.existFile(files, "_ts_(\\d+h\\d+m\\d+s)_" + i.ToString(), name);
					if (_existFile != null) {
						existFile = _existFile;
						continue;
					}
					if (tsConfig.isContinueConcat) {
						if (i == 0) {
							var firstFile = dirPath + "/" + name + "_ts_0h0m0s_" + i.ToString();
							string[] retb = {dirPath, firstFile};
							return retb;
						} else {
							//fName = dirPath + "/" + name + "_" + ((isTimeShift) ? "ts" : "") + (i - 1).ToString();
							existFile = existFile.Substring(0, existFile.Length - 3);
							string[] retc = {dirPath, existFile};
							return retc;
						}
					} else {
						var firstFile = dirPath + "/" + name + "_ts_0h0m0s_" + i.ToString();
						string[] retd = {dirPath, firstFile};
						return retd;
					}
//					continue;
				}
				else if (segmentSaveType == "1") {
					if (Directory.Exists(fName)) {
						string[] rete = {dirPath, fName};
						return rete;
					} else if (File.Exists(fName)) {
						continue;
					}
					util.debugWriteLine(dirPath + " " + fName);
					Directory.CreateDirectory(fName);
					if (!Directory.Exists(fName)) return null;
					string[] retf = {dirPath, fName};
					return retf;
				}
			}
		}
		return null;
	}
	public static string getOkFileName(string name) {
		string[] replaceCharacter = {"\\", "/", ":", "*", "?", "\"", "<", ">", "|"};
		foreach (string s in replaceCharacter) {
			name = name.Replace(s, "_");
		}
		return name;
	}
	private static string getSubFolderName(string host, string group, string title, string lvId, string communityNum, string userId, config cfg) {
		var n = cfg.get("subFolderNameType");
		if (n == null) n = "1";
		if (n == "1") return host;
		else if (n == "2") return userId;
		else if (n == "3") return userId + "_" + host + "";
		else if (n == "4") return group;
		else if (n == "5") return communityNum;
		else if (n == "6") return communityNum + "_" + group + "";
		else if (n == "7") return communityNum + "_" + host + "";
		else if (n == "8") return host + "_" + communityNum + "";
		else return host;
	}
	private static string getFileName(string host, string group, string title, string lvId, string communityNum, config cfg) {
		var n = cfg.get("fileNameType");
		var _hiduke = DateTime.Now;
		var month = (_hiduke.Month < 10) ? ("0" + _hiduke.Month.ToString()) : (_hiduke.Month.ToString());
		var day = (_hiduke.Day < 10) ? ("0" + _hiduke.Day.ToString()) : (_hiduke.Day.ToString());
		var hiduke = _hiduke.Year + "年" + month + "月" + day + "日";
		if (n == null) n = "1";
		if (n == "1") return host + "_" + communityNum + "(" + group + ")_" + lvId + "(" + title + ")";
		else if (n == "2") return communityNum + "(" + group + ")_" + host + "_" + lvId + "(" + title + ")";
		
		else if (n == "3") return lvId + "(" + title + ")_" + host + "_" + communityNum + "(" + group + ")";
		else if (n == "4") return host + "_" + group + "(" + communityNum + ")_" + title + "(" + lvId + ")";
		else if (n == "5") return group + "(" + communityNum + ")_" + host + "_" + title + "(" + lvId + ")";
		else if (n == "6") return title + "(" + lvId + ")_" + host + "_" + group + "(" + communityNum + ")";
		else if (n == "7") return hiduke + "_" + host + "_" + group + "(" + communityNum + ")_" + title + "(" + lvId + ")";
		else if (n == "8") return hiduke + "_" + group + "(" + communityNum + ")_" + host + "_" + title + "(" + lvId + ")";
		else if (n == "9") return hiduke + "_" + title + "(" + lvId + ")_" + host + "_" + group + "(" + communityNum + ")";
		else if (n == "10") return getDokujiSetteiFileName(host, group, title, lvId, communityNum, cfg.get("filenameformat"), _hiduke);
		else return host + "_" + communityNum + "(" + group + ")_" + lvId + "(" + title + ")";
		
		
	}
	public static string getDokujiSetteiFileName(string host, string group, string title, string lvId, string communityNum, string format, DateTime _openTime) {
		var type = format;
		if (type == null) return "";
		//var dt = DateTime.Now;
		var dt = _openTime;
		var yearBuf = ("0000" + dt.Year.ToString());
		var year2 = yearBuf.Substring(yearBuf.Length - 2);
		var year4 = yearBuf.Substring(yearBuf.Length - 4);
		var monthBuf = "00" + dt.Month.ToString();
		var month = monthBuf.Substring(monthBuf.Length - 2);
		var dayBuf = "00" + dt.Day.ToString();
		var day = dayBuf.Substring(dayBuf.Length - 2);
		
		var week = dt.ToString("ddd");
		var hour = dt.ToString("HH");
		var minute = dt.ToString("mm");
		var second = dt.ToString("ss");
		
		type = type.Replace("{Y}", year4);
		type = type.Replace("{y}", year2);
		type = type.Replace("{M}", month);
		type = type.Replace("{D}", day);
		type = type.Replace("{W}", week);
		type = type.Replace("{h}", hour);
		type = type.Replace("{m}", minute);
		type = type.Replace("{s}", second);
		type = type.Replace("{0}", lvId);
		type = type.Replace("{1}", title);
		type = type.Replace("{2}", host);
		type = type.Replace("{3}", communityNum);
		type = type.Replace("{4}", group);
		type = getOkFileName(type);
		return type;
		
	}
	public static string getFileNameTypeSample(string filenametype) {
			//var format = cfg.get("filenameformat");
			return getDokujiSetteiFileName("放送者名", "コミュ名", "タイトル", "lv12345", "co9876", filenametype, DateTime.Now).Replace("{w}", "2").Replace("{c}", "1");
		}
	public static string getOkCommentFileName(config cfg, string fName, string lvid, bool isTimeShift) {
		var kakutyousi = (cfg.get("IsgetcommentXml") == "true") ? ".xml" : ".json";
		var engineMode = cfg.get("EngineMode");
		if (cfg.get("segmentSaveType") == "0" || engineMode != "0") {
			//renketu
			if (isTimeShift && engineMode == "0") {
				var time = getRegGroup(fName, "(_\\d+h\\d+m\\d+s_)");
				fName = fName.Replace(time, "");
			}
			util.debugWriteLine("comment file path " + fName + kakutyousi);
			return fName + kakutyousi;
		} else {
			
			var name = util.getRegGroup(fName, ".+/(.+)");
			if (fName.Length + name.Length > 245) name = lvid;
			util.debugWriteLine("comment file path " + fName + "/" + name + kakutyousi);
			return fName + "/" + name + kakutyousi;
		}
	}
	public static string getLastTimeshiftFileName(string host, 
			string group, string title, string lvId, string communityNum, 
			string userId, config cfg) {
		host = getOkFileName(host);
		group = getOkFileName(group);
		title = getOkFileName(title);
		
		string[] jarpath = getJarPath();
//		util.debugWriteLine(jarpath);
		//string dirPath = jarpath[0] + "\\rec\\" + host;
		string _dirPath = (cfg.get("IsdefaultRecordDir") == "true") ?
			(jarpath[0] + "\\rec") : cfg.get("recordDir");
		string dirPath = _dirPath;
		
		string sfn = null;
		if (cfg.get("IscreateSubfolder") == "true") {
			sfn = getSubFolderName(host, group, title, lvId, communityNum, userId,  cfg);
			if (sfn.Length > 120) sfn = sfn.Substring(0, 120);
			if (sfn == null) return null;
			dirPath += "/" + sfn;
		}


		var segmentSaveType = cfg.get("segmentSaveType");

		var name = getFileName(host, group, title, lvId, communityNum,  cfg);
		if (name.Length > 200) name = name.Substring(0, 200);
		
		//長いパス調整
		if (name.Length + dirPath.Length > 234) {
			name = lvId;
			if (name.Length + dirPath.Length > 234 && sfn != null) {
				sfn = sfn.Substring(0, 3);
				dirPath = _dirPath + "/" + sfn;
								
				if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
				if (!Directory.Exists(dirPath)) return null;
				
			}
		}
		if (name.Length + dirPath.Length > 234) return null;
		
		if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
		if (!Directory.Exists(dirPath)) return null;
		
		string existFile = null;
		var files = Directory.GetFiles(dirPath);
		for (int i = 0; i < 1000; i++) {
			var fName = dirPath + "/" + name + "_" + "ts" + i.ToString();
			
			if (segmentSaveType == "0") {
				//util.existFile(dirPath, name + "_ts_\\d+h\\d+m\\d+s_" + i.ToString());
				var _existFile = util.existFile(files, "_ts_(\\d+h\\d+m\\d+s)_" + i.ToString(), name);
				if (_existFile != null) {
					existFile = _existFile;
					continue;
				}

			}
			
			if (segmentSaveType == "1") {
				if (Directory.Exists(fName)) {
					return fName;
				}
				if (File.Exists(fName)) continue;
				
				/*
				try {
					//Directory.CreateDirectory(fName);
					if (Directory.Exists(fName)) return fName;
					continue;
				} catch (Exception e) {
					util.debugWriteLine("get last timeshift file create dir exception " + fName + e.Message + e.StackTrace + e.Source + e.TargetSite);
					continue;
				}
				*/
				
				return null;
			}
			
			util.debugWriteLine(dirPath + " " + fName);
			
			if (i == 0) {
				util.debugWriteLine("last timeshift file " + dirPath + "/" + name + "_" + "ts" + (i - 0).ToString());
				return null;
//			} else util.debugWriteLine("last timeshift file " + dirPath + "/" + name + "_" + "ts" + (i - 1).ToString());
			} else util.debugWriteLine("last timeshift file " + existFile);
//			return dirPath + "/" + name + "_" + "ts" + (i - 1).ToString();
			return existFile;
//			string[] ret = {dirPath, dirPath + "/" + name + "_" + "ts" + (i - 1).ToString()};
		}
		return null;
	}
	public static string[] getLastTimeShiftFileTime(string lastFile, string segmentSaveType) {
		if (lastFile == null) return null;
		string fname = null;
		if (segmentSaveType == "0") {
			fname = lastFile + "";
		} else {
			var ss = new List<string>();
			var key = new List<int>();
			foreach (var f in Directory.GetFiles(lastFile)) {
				if (!f.EndsWith(".ts")) continue;
				var name = util.getRegGroup(f, ".+\\\\(.+)");
				if (name == null) continue;
				if (util.getRegGroup(name, "(\\d+h\\d+m\\d+s)") == null) continue;;
				var _k = util.getRegGroup(name, "(\\d+)");
				if (_k == null) continue;
				ss.Add(f);
				key.Add(int.Parse(_k));
				  
			}
			if (ss.Count == 0) return null;
			var ssArr = ss.ToArray();
			Array.Sort(key.ToArray(), ssArr);
			fname = ssArr[ssArr.Length - 1];
		}
		if (!File.Exists(fname)) return null;
		var _name = util.getRegGroup(fname, ".+\\\\(.+)");
		var h = util.getRegGroup(_name, "(\\d+)h");
		var m = util.getRegGroup(_name, "(\\d+)m");
		var s = util.getRegGroup(_name, "(\\d+)s");
		if (h == null || m == null || s == null) return null;
		var ret = new string[]{h, m, s};
		return ret;
	}
	public static string postResStr(string url, Dictionary<string, string> headers, byte[] content, string method = "POST") {
		try {
			if (isCurl) {
				var d = content == null ? null : Encoding.UTF8.GetString(content);
				var r = new Curl().getStr(url, headers, CurlHttpVersion.CURL_HTTP_VERSION_1_1, method, d, false);
				return r;
			} else {
				var res = sendRequest(url, headers, content, method);
				if (res == null) {
					debugWriteLine("postResStr res null");
					return null;
				}
				
				debugWriteLine(res.StatusCode + " " + res.StatusDescription);
				
				//var resStream = res.GetResponseStream();
				using (var getResStream = res.GetResponseStream())
				using (var resStream = new System.IO.StreamReader(getResStream)) {
					var resStr = resStream.ReadToEnd();
					return resStr;
				}
			}
		} catch (Exception ee) {
			debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			return null;
		}
	}
	public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
	public static string getPageSource(string _url, CookieContainer container = null, string referer = null, bool isFirstLog = true, int timeoutMs = 5000) {
		timeoutMs = 5000;
		try {
			if (isCurl) {
				var h = getHeader(container, referer, _url);
				if (userAgent != null) h["User-Agent"] = userAgent;
				var r = new Curl().getStr(_url, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "GET", null, false);
				return r;
			}
			var req = (HttpWebRequest)WebRequest.Create(_url);
			req.Proxy = null;
			req.AllowAutoRedirect = true;
			if (referer != null) req.Referer = referer;
			if (container != null) req.CookieContainer = container;
			req.Headers.Add("Accept-Encoding", "gzip,deflate");
			req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			req.Timeout = timeoutMs;
			
			var res = (HttpWebResponse)req.GetResponse();
			var dataStream = res.GetResponseStream();
			var reader = new StreamReader(dataStream);
			
			var resStr = reader.ReadToEnd();
			return resStr;

		} catch (Exception e) {
			//System.Threading.Tasks.Task.Run(() => {
				util.debugWriteLine("getpage error " + _url + e.Message+e.StackTrace);
			//});
		}
		return null;
	}
	/*
	public static byte[] getFileBytes(string url, CookieContainer container) {
//		var a = container.GetCookieHeader(new Uri(_url));
		util.debugWriteLine("getfilebyte " + url);
		for (int i = 0; i < 1; i++) {
			try {
				var req = (HttpWebRequest)WebRequest.Create(url);
				req.Proxy = null;
				req.AllowAutoRedirect = true;
	//			req.Headers = getheaders;
//				if (referer != null) req.Referer = referer;
				if (container != null) req.CookieContainer = container;
				var res = (HttpWebResponse)req.GetResponse();
				var dataStream = res.GetResponseStream();
				
//				var reader = new StreamReader(dataStream);
				byte[] b = new byte[10000000];
				int pos = 0;
				var r = 0;
				while ((r = dataStream.Read(b, pos, 1000000)) > 0) {
//					if (dataStream.Read(b, (int)j, (int)dataStream.Length) == 0) break;
//					j = dataStream.Position;
					pos += r;
				}
				Array.Resize(ref b, pos);
				return b;
				
			} catch (Exception e) {
				util.debugWriteLine("getfile error " + url + e.Message+e.StackTrace);
//				System.Threading.Thread.Sleep(3000);
				continue;
			}
		}
		return null;
	}
	*/
	public static string existFile(string[] files, string reg, string startWith) {
//		var files = Directory.GetFiles(dirPath);
		foreach (var f in files) {
			var _f = getRegGroup(f, ".+\\\\(.+)");
			var isStartsWith = _f.StartsWith(startWith);
			if (!isStartsWith) continue;
			var _reg = util.getRegGroup(_f.Substring(startWith.Length), reg);
			if (_reg == null) continue;
//			util.debugWriteLine(_f.StartsWith(startWith));
//			util.debugWriteLine(util.getRegGroup(_f.Substring(startWith.Length), reg));
			return f;
//			if (issta_f.StartsWith(startWith) && 
//			    util.getRegGroup(_f.Substring(startWith.Length), reg) != null) return f;
		}
		return null;
	}
	public static string getSecondsToStr(double seconds) {
//		var dotSecond = ((int)((seconds % 1) * 10)).ToString("0");
		var second = ((int)((seconds % 60) * 1)).ToString("00");
		var minute = ((int)((seconds % 3600 / 60))).ToString("00");
		var hour = ((int)((seconds / 3600) * 1));
		var _hour = (hour < 100) ? hour.ToString("00") : hour.ToString();;
		var timeStr = _hour + "h" + minute + "m" + second + "s";
		return timeStr;
	}
	public static int getSecondsFromStr(string _s) {
		var h = getRegGroup(_s, "(\\d+)h");
		var m = getRegGroup(_s, "(\\d+)m");
		var s = getRegGroup(_s, "(\\d+)s");
		if (h == null || m == null || s == null) return -1;
		return int.Parse(h) * 3600 + int.Parse(m) * 60 + int.Parse(s);
	}
	public static int getPageType(string res) {
		//if (res.IndexOf("siteId&quot;:&quot;nicolive2") > -1) {
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			var status = (data == null) ? null : util.getRegGroup(data, "&quot;status&quot;:&quot;(.+?)&quot;");
			if (res.IndexOf("<!doctype html>") > -1 && data != null && status == "ON_AIR" && data.IndexOf("webSocketUrl&quot;:&quot;ws") > -1) return 0;
			else if (res.IndexOf("<!doctype html>") > -1 && data != null && status == "ENDED" && data.IndexOf("webSocketUrl&quot;:&quot;ws") > -1) return 7;
			else if (util.getRegGroup(res, "(混雑中ですが、プレミアム会員の方は優先して入場ができます)") != null ||
			        util.getRegGroup(res, "(ただいま、満員のため入場できません)") != null) return 1;
	//		else if (util.getRegGroup(res, "<div id=\"comment_arealv\\d+\">[^<]+この番組は\\d+/\\d+/\\d+\\(.\\) \\d+:\\d+に終了いたしました。<br>") != null) return 2;
			else if (res.IndexOf(" onclick=\"Nicolive.ProductSerial") > -1) return 8;
			//else if (res.IndexOf("※この放送はタイムシフトに対応しておりません。") > -1 && 
			//         res.IndexOf("に終了いたしました") > -1) return 2;
			//else if (util.getRegGroup(res, "(コミュニティフォロワー限定番組です。<br>)") != null) return 4;
			else if (res.IndexOf("isFollowerOnly&quot;:true") > -1 && res.IndexOf("isFollowed&quot;:false") > -1) return 4;
			else if (data.IndexOf("webSocketUrl&quot;:&quot;ws") == -1 && 
			         status == "ENDED") return 2;
			
			else if (status == "ENDED" && res.IndexOf(" onclick=\"Nicolive.WatchingReservation") > -1) return 9;
			//else if (util.getRegGroup(res, "(に終了いたしました)") != null) return 2;
			else if (status == "ENDED") return 2;
			else if (util.getRegGroup(res, "(<archive>1</archive>)") != null) return 3;
			else if (util.getRegGroup(res, "(チャンネル会員限定番組です。<br>)") != null) return 4;
			else if (util.getRegGroup(res, "(<h3>【会場のご案内】</h3>)") != null) return 6;
			else if (util.getRegGroup(res, "(この番組は放送者により削除されました。<br />|削除された可能性があります。<br />)") != null) return 2;
			return 5;
		//}
		//return 5;
	}

	
	public static DateTime getUnixToDatetime(long unix) {
		DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return UNIX_EPOCH.AddSeconds(unix).ToLocalTime();
	}
	public static string getSecondsToKeikaJikan(double seconds) {
		var second = ((int)((seconds % 60) * 1)).ToString("00");
		var minute = ((int)((seconds % 3600 / 60))).ToString("00");
		var hour = ((int)((seconds / 3600) * 1));
		var _hour = (hour < 100) ? hour.ToString("00") : hour.ToString();;
		return _hour + "時間" + minute + "分" + second + "秒";
	}
	public static string getUserName(string userId, out bool isFollow, CookieContainer container) {
		isFollow = false; 
		if (userId == "official" || userId == null || userId == "") return null;
		
		//http://ext.nicovideo.jp/thumb_user/10000
		//var url = "http://seiga.nicovideo.jp/api/user/info?id=" + userId;
		var url = "https://www.nicovideo.jp/user/" + userId;
		var res = util.getPageSource(url, container);

		if (res == null) return null;
		var name = util.getRegGroup(res, "<meta property=\"og:title\" content=\"(.+?)\">"); 
		if (name == null)
			name = util.getRegGroup(res, "<meta property=\"og:title\" content=\"(.+?)さんのユーザーページ\">");
		if (name == null) return null;
		if (name.EndsWith(" - niconico(ニコニコ)")) 
			name = name.Replace(" - niconico(ニコニコ)", "");
		//watching nowatching class
		if (res.IndexOf("class=\"watching\"") > -1) isFollow = true;
		return name;
	}
	public static string getCommunityName(string communityNum, out bool isFollow, CookieContainer cc) {
		isFollow = false;
		if (communityNum == null || communityNum == "" || communityNum == "official") return null;
		
		var isChannel = communityNum.IndexOf("ch") > -1;
		var url = (isChannel) ? 
			("https://ch.nicovideo.jp/" + communityNum) :
			("https://com.nicovideo.jp/community/" + communityNum);
		
//			var wc = new WebHeaderCollection();
		var res = util.getPageSource(url, cc);
//			util.debugWriteLine(container.GetCookieHeader(new Uri(url)) + util.getMainSubStr(isSub, true));
		
		if (res == null) {
			url = (isChannel) ? 
				("https://ch.nicovideo.jp/" + communityNum) :
				("https://com.nicovideo.jp/motion/" + communityNum);
			res = util.getPageSource(url, cc);
			if (res == null) return null;
			isFollow = res.IndexOf("<h2 class=\"pageHeader_title\">コミュニティにフォローリクエストを送る</h2>") == -1 &&
					util.getRegGroup(res, "<p class=\"error_description\">[\\s\\S]*?(コミュニティフォロワー)ではありません。") == null &&
					res.IndexOf("<h2 class=\"pageHeader_title\">コミュニティをフォローする</h2>") == -1;
		} else {
			isFollow = (isChannel) ? 
				(res.IndexOf("class=\"bookmark following btn_follow\"") > -1):
				(res.IndexOf("followButton follow\">フォロー") == -1);
		}
		if (res == null) return null;
		var title = (isChannel) ? 
//			util.getRegGroup(res, "<meta property=\"og\\:title\" content=\"(.+?) - ニコニコチャンネル") :
			util.getRegGroup(res, "<meta property=\"og:site_name\" content=\"(.+?)\"") :
			util.getRegGroup(res, "<meta property=\"og\\:title\" content=\"(.+?)-ニコニコミュニティ\"");
		if (title == null) title = util.getRegGroup(res, "<meta property=\"og:title\" content=\"(.+?)さんのコミュニティ-ニコニコミュニティ\">");
		
		//not login
		if (title == null) {
			url = "https://ext.nicovideo.jp/thumb_" + ((isChannel) ? "channel" : "community") + "/" + communityNum;
			res = getPageSource(url, cc, null, false, 3);
			title = getRegGroup(res, "<p class=\"chcm_tit\">(.+?)</p>");
		}
		if (title == null) isFollow = false;
		return title;
	}
	public static void writeFile(string name, string str) {
		using (var f = new System.IO.FileStream(name, FileMode.Append))
        using (var w = new StreamWriter(f)) {
	       	try {
				w.WriteLine(str);
	       	} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite);
	       	}
       }
//		w.Close();
//		f.Close();
	}
	public static bool isLogFile = false;
	public static void debugWriteLine(object str) {
		var dt = DateTime.Now.ToLongTimeString();
//		System.Console.WriteLine(dt + " " + str);
		try {
			#if DEBUG
				System.Diagnostics.Debug.WriteLine(str);
	//      		System.Diagnostics.Debug.WriteLine(
			#else
				if (isLogFile) System.Console.WriteLine(dt + " " + str);
			#endif
		} catch (Exception e) {
			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite + " " + e.Source);
		}
		
	}
	public static void showException(Exception eo, bool isMessageBox = true) {
		var frameCount = new System.Diagnostics.StackTrace().FrameCount;
		#if DEBUG
			if (isMessageBox && isLogFile) {
				if (frameCount > 50) {
					util.showMessageBoxCenterForm(null, "framecount stack", frameCount.ToString());
					return;
				}
			}
		#else
			
		#endif
		
		
		util.debugWriteLine("exception stacktrace framecount " + frameCount);
		
		util.debugWriteLine("show exception eo " + eo);
		if (eo == null) return;
		
		util.debugWriteLine("0 message " + eo.Message + "\nsource " + 
				eo.Source + "\nstacktrace " + eo.StackTrace + 
				"\n targetsite " + eo.TargetSite + "\n\n");
		
		var _eo = eo.GetBaseException();
		util.debugWriteLine("eo " + _eo);
		if (_eo != null) {
			util.debugWriteLine("1 message " + _eo.Message + "\nsource " + 
					_eo.Source + "\nstacktrace " + _eo.StackTrace + 
					"\n targetsite " + _eo.TargetSite + "\n\n");
		}
		
		_eo = eo.InnerException;
		util.debugWriteLine("eo " + _eo);
		if (_eo != null) {
			util.debugWriteLine("2 message " + _eo.Message + "\nsource " + 
					_eo.Source + "\nstacktrace " + _eo.StackTrace + 
					"\n targetsite " + _eo.TargetSite);
		}
		
		#if DEBUG
			if (isMessageBox && isLogFile)
				util.showMessageBoxCenterForm(null, "error", "error");
		#else
			
		#endif
	}
	public static void setLog(config config, string lv) {
		//test
		if (bool.Parse(config.get("IsLogFile"))) {
			//var name = (args.Length == 0) ? "lv_" : util.getRegGroup(args[0], "(lv\\d+)");
			var name = (lv == null) ? "rt_lv_" : lv;
			var logPath = util.getJarPath()[0] + "/" + name + ".txt";
			
			try {
				#if DEBUG
					System.Diagnostics.DefaultTraceListener dtl
				      = (System.Diagnostics.DefaultTraceListener)System.Diagnostics.Debug.Listeners["Default"];
					dtl.LogFileName = logPath;
				#else
					FileStream fs = new FileStream(logPath, 
				    		FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
					var w = new System.IO.StreamWriter(fs);
					w.AutoFlush = true;
					System.Console.SetOut(w);
				#endif
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			util.isLogFile = true;
		}
	}
	public static Bitmap getSamune(string url) {
   		WebClient cl = new WebClient();
   		cl.Proxy = null;
		
   		System.Drawing.Icon icon =  null;
		try {
   			util.debugWriteLine("samune url " + url);
   			byte[] pic = cl.DownloadData(url);
			
   			using (var  st = new System.IO.MemoryStream(pic)) {
				icon = Icon.FromHandle(new System.Drawing.Bitmap(st).GetHicon());
   			}
			//st.Close();
			
		} catch (Exception e) {
			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			return null;
		}
		return icon.ToBitmap();
	}
	public static string getMyName(CookieContainer cc, string us) {
		try {
			var url = "https://nvapi.nicovideo.jp/v1/users/me";
			//var us = cc.GetCookies(new Uri(url))["user_session"];
			//if (us == null) return null;
			var _h = new Dictionary<string, string>() {
				{"User-Agent", "Niconico/1.0 (Linux; U; Android 7.1.2; ja-jp; nicoandroid LGM-V300K) Version/6.14.1"},
				{"Cookie", "user_session=" + us},
					{"X-Frontend-Id", "1"},
					{"X-Frontend-Version", "6.14.1"},
					{"Connection", "keep-alive"},
					{"Upgrade-Insecure-Requests", "1"},
				};
			var res = postResStr(url, _h, null, "GET");
			var n = util.getRegGroup(res, "\"nickname\":\"(.+?)\"");
			return n;
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
		return null;
	}
	public static HttpWebResponse sendRequest(string url, Dictionary<string, string> headers, byte[] content, string method, CookieContainer cc = null) {
		try {
			var req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = method;
			req.Proxy = null;
			req.Headers.Add("Accept-Encoding", "gzip,deflate");
			req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			req.CookieContainer = cc;
			
			if (headers != null) {
				foreach (var h in headers) {
					if (h.Key.ToLower().Replace("-", "") == "contenttype")
						req.ContentType = h.Value;
					else if (h.Key.ToLower().Replace("-", "") == "useragent")
						req.UserAgent = h.Value;
					else if (h.Key.ToLower().Replace("-", "") == "connection")
						req.KeepAlive = h.Value.ToLower().Replace("-", "") == "keepalive";
					else if (h.Key.ToLower().Replace("-", "") == "accept")
						req.Accept = h.Value;
					else if (h.Key.ToLower().Replace("-", "") == "referer")
						req.Referer = h.Value;
					else req.Headers.Add(h.Key, h.Value);
				}
			}
				
			if (content != null) {
				using (var stream = req.GetRequestStream()) {
					try {
						stream.Write(content, 0, content.Length);
					} catch (Exception ee) {
			       		debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
			       	}
				}
			}
//					stream.Close();

			return (HttpWebResponse)req.GetResponse();
			
		} catch (WebException ee) {
			util.debugWriteLine(ee.Data + ee.Message + ee.Source + ee.StackTrace + ee.Status);
			try {
				return (HttpWebResponse)ee.Response;
				//using (var _rs = ee.Response.GetResponseStream())
				//using (var rs = new StreamReader(_rs)) {
				//	return rs.ReadToEnd();
				//}
			} catch (Exception eee) {
				util.debugWriteLine(eee.Message + eee.Source + eee.StackTrace);
				return null;
			}
		} catch (Exception ee) {
			debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			return null;
		}
	}
	public static List<Control> getChildControls(Control c) {
		//util.debugWriteLine("cname " + c.Name);
		var ret = new List<Control>();
		foreach (Control _c in c.Controls) {
			var children = getChildControls(_c);
			ret.Add(_c);
			ret.AddRange(children);
			//util.debugWriteLine(c.Name + " " + children.Count);
		}
		//util.debugWriteLine(c.Name + " " + ret.Count);
		return ret;
	}
	public static void setFontSize(float size, Form form, bool isKeepSize, int baseHeight = -1) {
    	try {
    		var workingArea = Screen.GetWorkingArea(new Point(0,0));
    		
			var _formsize = form.Size;
			var _bStyle = form.FormBorderStyle;
			
			baseHeight = baseHeight == -1 ? form.Height : baseHeight;
			
			if (size > form.Font.Size)
				form.Size = new Size(isKeepSize ? 918 : form.Width, baseHeight);
			
			var max = (int)(form.Font.Size * (workingArea.Height * 0.9 / form.Height));
			
			if (size > max) {
				size = max;
				util.showMessageBoxCenterForm(form, "画面上に表示できなくなる可能性があるため、" + size + "に設定されます");
			}
			
			form.Font = new Font(form.Font.FontFamily, size);
			if (isKeepSize) {
				//form.Size = _formsize;
			}
			
			var controls = util.getChildControls(form);
			foreach (Control c in controls) {
				if (c.ContextMenuStrip != null)
					c.ContextMenuStrip.Font = new Font(c.Font.FontFamily, size);

				if (c is DataGridView) {
					((DataGridView)c).ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
					((DataGridView)c).ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
					
					((DataGridView)c).RowTemplate.Height = (int)size;
					((DataGridView)c).AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
				}
				if (c is StatusStrip) {
					foreach (ToolStripLabel s in ((StatusStrip)c).Items)
						s.Font = new Font(c.Font.FontFamily, size);
				}
				c.Font = new Font(c.Font.FontFamily, size);
			}
    	} catch (Exception e) {
    		util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
    	}
	}
	[DllImport("user32.dll")]
	static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetCurrentThreadId();
	public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
	[DllImport("user32.dll")]
	public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
	[DllImport("user32.dll")]
	static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
	[DllImport("user32.dll")]
	static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, IntPtr threadId);
	[DllImport("user32.dll")]
	public static extern bool UnhookWindowsHookEx(IntPtr hHook);
	[DllImport("user32.dll")]
	public static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);
	public struct RECT {
		public int left;
		public int top;
		public int right;
		public int bottom;
	}
	private static Form messageBoxOwnerForm = null;
	private static IntPtr mBHook;
	private static IntPtr CBTProc(int nCode, IntPtr wParam, IntPtr lParam) {
		var HCBT_ACTIVATE = 5;
		if (nCode == HCBT_ACTIVATE) {
			RECT rectF, rectM; 
			GetWindowRect(messageBoxOwnerForm.Handle, out rectF);
			GetWindowRect(wParam, out rectM);
			var x = rectF.left + ((rectF.right - rectF.left) - (rectM.right - rectM.left)) / 2;
			var y = rectF.top + ((rectF.bottom - rectF.top) - (rectM.bottom - rectM.top)) / 2;
			
			uint SWP_NOSIZE = 1;
			uint SWP_NOZORDER = 4;
			uint SWP_NOACTIVATE = 16;
			if (x >= 0 && y >= 0)
				SetWindowPos(wParam, 0, x, y, 0, 0, 
						SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
			UnhookWindowsHookEx(mBHook);
		}
		return CallNextHookEx(mBHook, nCode, wParam, lParam);
	}
	public static DialogResult showMessageBoxCenterForm(Form form, string text, string caption = "", MessageBoxButtons btn = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None) {
		if (form != null) {
			var GWL_HINSTANCE = -6;
			var hInstance = GetWindowLong(form.Handle, GWL_HINSTANCE);
		    var threadId = GetCurrentThreadId();
		    var whCbt = 5;
		    messageBoxOwnerForm = form;
		    mBHook = SetWindowsHookEx(whCbt, new HookProc(CBTProc), hInstance, threadId);
		}
		return MessageBox.Show(text, caption, btn, icon);
	}
	public static void openUrlBrowser(string url, config config) {
		try {
			if (config.get("IsdefaultBrowserPath") == "true")
				Process.Start(url);
			else {
				var path = config.get("browserPath");
				if (path == null || path == "")
					Process.Start(url);
				else Process.Start(path, url);
			}
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
	}
	public static string getFFmpegDefaultArg(int afterConvertMode) {
		var _command = "";
		string path = "{path}";
		string tmp = "{tmp}";
		//8-vob 11-wav 15-mp4(再エンコード)
		if (afterConvertMode == 8 || 
		    	afterConvertMode == 11 || afterConvertMode == 15)
			_command = ("-i \"" + path + "\" -max_muxing_queue_size 1024 \"" + tmp + "\"");
		//10-mp3
		else if (afterConvertMode == 10)
			_command = ("-i \"" + path + "\" -b:a 128k \"" + tmp + "\"");
		//13-aac
		else if (afterConvertMode == 13)
			_command = ("-i \"" + path + "\" -f mp4 -vn -c copy \"" + tmp + "\"");
		//12-wma
		else if (afterConvertMode == 12)
			_command =  ("-i \"" + path + "\" -vn -c copy \"" + tmp + "\"");
		//14-ogg
		else if (afterConvertMode == 14)
			_command =  ("-i \"" + path + "\" -vn -max_muxing_queue_size 1024 \"" + tmp + "\"");
		//5-flv
		else if (afterConvertMode == 5)
			_command = ("-i \"" + path + "\" -c:v copy -c:a aac -bsf:a aac_adtstoasc \"" + tmp + "\"");
		else _command = ("-i \"" + path + "\" -c copy \"" + tmp + "\"");
		
		//flv
		if (path.EndsWith("flv")) {
			//avi 3
			if (afterConvertMode == 3)
				_command = ("-i \"" + path + "\" \"" + tmp + "\""); 
		}
		return _command;
	}
	public static Dictionary<string, string> getHeader(CookieContainer cc, string referer, string url) {
		var ret = new Dictionary<string, string>() {
			//{"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8"},
			{"Accept-Language", "ja,en-US;q=0.7,en;q=0.3"},
			{"Cache-Control", "no-cache"},
			{"User-Agent", userAgent}
		};
		if (cc != null) ret["Cookie"] = cc.GetCookieHeader(new Uri(url));
		if (referer != null) ret["Referer"] = referer;
		return ret;
	}
	public static string getPageSourceCurl(string url, CookieContainer cc, string referer) {
		var h = getHeader(cc, referer, url);
		var res = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false);
		return res;
	}
	public static string CheckOSName()
        {
            string result = "";

            System.Management.ManagementClass mc =
                new System.Management.ManagementClass("Win32_OperatingSystem");
            System.Management.ManagementObjectCollection moc = mc.GetInstances();

            try
            {
                foreach (System.Management.ManagementObject mo in moc)
                {
                    result = mo["Caption"].ToString();
                    if (mo["CSDVersion"] != null)
                        result += " " + mo["CSDVersion"].ToString();
                    result += " (" + mo["Version"].ToString() + ")";
                }
                osName = result;
            }
            catch (Exception e)
            {
                util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
                return result;
            }

            return result;
        }
        public static string CheckOSType()
        {
            string result = "";

            System.Management.ManagementClass mc =
                new System.Management.ManagementClass("Win32_OperatingSystem");
            System.Management.ManagementObjectCollection moc = mc.GetInstances();

            try
            {
                foreach (System.Management.ManagementObject mo in moc)
                {
                    if (mo["Version"].ToString().StartsWith("5.1"))
                        result = "XP";
                    else if (mo["Version"].ToString().StartsWith("6.0"))
                        result = "Vista";
                    else if (mo["Version"].ToString().StartsWith("6.1"))
                        result = "7";
                    else if (mo["Version"].ToString().StartsWith("6.2"))
                        result = "8";
                    else if (mo["Version"].ToString().StartsWith("6.3"))
                        result = "8.1";
                    else if (mo["Version"].ToString().StartsWith("10.0"))
                        result = "10";
                    else if (mo["Version"].ToString().StartsWith("11.0"))
                        result = "11";
                    else
                        result = "other";
                }
            }
            catch (Exception e)
            {
                util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
                return result;
            }
            return result;
        }
    public static bool isUseCurl(CurlHttpVersion httpVer) {
		//return true;
		
		if (httpVer != CurlHttpVersion.CURL_HTTP_VERSION_1_1) return true;
		if ((util.osName != null && 
		     (util.osName.IndexOf("Windows 1") > -1)) || util.isWebRequestOk)
			return false;
		return util.isCurl;
	}
    public static void saveBackupConfig(string path, string f) {
		try {
    		var p = path + "\\config_backup";
    		if (File.Exists(path + f + ".config")) {
    			if (!Directory.Exists(p))
    				Directory.CreateDirectory(p);
    			if (!Directory.Exists(p)) return;
    			
    			var dt = DateTime.Now.ToString("yyyyMMdd");
    			File.Copy(path + f + ".config", p + "\\" + f + dt + "backup.config", true);
    		}
    		var _fList = new List<string>(Directory.GetFiles(p, "*" + f + "*"));
    		var fList = new List<string>();
    		var dtL = new List<int>();
    		foreach (var _f in _fList) {
    			var d = util.getRegGroup(_f, f + "(\\d+)backup.config");
    			if (d == null) continue;
    			fList.Add(_f);
    			dtL.Add(int.Parse(d));
    		}
    		if (fList.Count <= 5) return;
    		var vals = fList.ToArray();
    		Array.Sort(dtL.ToArray(), vals);
    		for (var i = 0; i < vals.Length - 5; i++)
    			File.Delete(vals[i]);
    		
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
    }
}
