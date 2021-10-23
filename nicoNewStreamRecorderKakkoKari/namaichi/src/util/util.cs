using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using namaichi;
using namaichi.config;
using namaichi.info;
using SuperSocket.ClientEngine.Proxy;

/*
class app {
	public static namaichi.MainForm form;
	public static void Mains(string[] args) {
		string a = util.getRegGroup("as32df5gh", "\\d([^0-9]).(.)", 1);
		util.consoleWrite(a);
		util.consoleWrite(util.getPath());
		util.consoleWrite(util.getTime());
		util.consoleWrite(util.getJarPath());
		util.consoleWrite(util.getOkFileName(".a\\\"aa|a", false, false));
		//util.consoleWrite(util.getRecFolderFilePath("host", "group", "title", "lvid", "comnum")[0]);
		//util.consoleWrite(util.getRecFolderFilePath("host", "group", "title", "lvid", "comnum")[1]);
	}
}
*/
class util {
	public static string versionStr = "ver0.88.53";
	public static string versionDayStr = "2021/10/23";
	public static bool isShowWindow = true;
	public static bool isStdIO = false;
	public static double dotNetVer = 0;
	public static WebProxy httpProxy = null;
	public static HttpConnectProxy wsProxy = null;
	
	public static string getRegGroup(string target, string reg, int group = 1, Regex r = null) {
		if (r == null)
			 r = new Regex(reg);
		var m = r.Match(target);
//		util.consoleWrite(m.Groups.Count +""+ m.Groups[0]);
		if (m.Groups.Count>group) {
			return m.Groups[group].ToString();
		} else return null;
	}	
	public static string getPath() {
		string p  = System.IO.Path.GetDirectoryName(
			System.IO.Path.GetFullPath(
			System.Reflection.Assembly.GetExecutingAssembly().Location));
//		util.consoleWrite(p);
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
			bool isTimeShift, TimeShiftConfig tsConfig, 
			long _openTime, bool isRtmp, bool isFmp4) {
		string name = null, dirPath = null;
		bool _isTimeShift = isTimeShift;
		var segmentSaveType = cfg.get("segmentSaveType");
		string ext = (isFmp4 ? ".mp4" : ".ts");
		if (tsConfig != null && tsConfig.lastFileName != null) {
			name = util.getRegGroup(tsConfig.lastFileName, ".+[\\\\/](.+)_ts_\\d+h\\d+m\\d+s");
			dirPath = util.getRegGroup(tsConfig.lastFileName, "(.+[\\\\/])");
			util.debugWriteLine("getRecFolderFilePath tsconfig.lastFileName " + name + " " + dirPath);
		} else {
			host = getOkFileName(host, isRtmp, false);
			group = getOkFileName(group, isRtmp, false);
			title = getOkFileName(title, isRtmp, false);
			
			string[] jarpath = getJarPath();
	//		util.debugWriteLine(jarpath);
			//string dirPath = jarpath[0] + "\\rec\\" + host;
			string _dirPath = (cfg.get("IsdefaultRecordDir") == "true") ?
				(jarpath[0] + "\\rec") : cfg.get("recordDir");
			dirPath = _dirPath;
			
			string sfn = null;
			if (cfg.get("IscreateSubfolder") == "true") {
				sfn = getSubFolderName(host, group, title, lvId, communityNum, userId,  cfg);
				if (sfn.Length > 120) sfn = sfn.Substring(0, 120);
				if (sfn == null) return null;
				dirPath += "/" + sfn;
			}
			
			if (cfg.get("EngineMode") != "0" || isRtmp) segmentSaveType = "0";
			
			//bool _isTimeShift = isTimeShift;
			if (cfg.get("EngineMode") != "0") _isTimeShift = false;
	
			name = getFileName(host, group, title, lvId, communityNum,  cfg, _openTime);
			if (name.IndexOf("\\") > -1) {
				sfn += "\\" + name.Substring(0, name.LastIndexOf("\\"));
				dirPath += "\\" + name.Substring(0, name.LastIndexOf("\\"));
				name = name.Substring(name.LastIndexOf("\\") + 1);
			}
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
			if (name.Length + dirPath.Length > 234) return new string[]{null, name + " " + dirPath, null};
			
			if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
			if (!Directory.Exists(dirPath)) return null;
		}
		
		var files = Directory.GetFiles(dirPath);
		string existFile = null;
		string existDt = null;
		string existDtFile = null;
		for (int i = 0; i < 1000000; i++) {
			var fName = dirPath + "/" + name + "_" + ((_isTimeShift) ? "ts" : "") + i.ToString();
			var originName = dirPath + "/" + name;
			util.debugWriteLine(dirPath + " " + fName);
			
			if (!_isTimeShift) {
				if (segmentSaveType == "0" && isExistAllExt(fName, dirPath)) continue;
				else if (segmentSaveType == "1") {
					isExistDirectory(fName, dirPath);
					
					if (Directory.Exists(fName)) continue;
					Directory.CreateDirectory(fName);
					if (!Directory.Exists(fName)) return null;
				}
				
				string[] reta = {dirPath, fName, originName};
				return reta;
			} else {
				if (isRtmp) {
					if (isExistAllExt(fName, dirPath)) continue;
					string[] reta = {dirPath, fName, originName};
					return reta;
				}
				
				if (segmentSaveType == "0") {
					var _existFile = util.existFile(files, "_ts(_\\d+h\\d+m\\d+s_)*" + i.ToString() + "", name);
					var _existDt = util.existFile(files, "_ts_(\\d+h\\d+m\\d+s)_" + i.ToString() + ext, name);
					var reg = "_ts_(\\d+h\\d+m\\d+s)_" + i.ToString() + ext;
					if (_existDt != null) {
						existDt = util.getRegGroup(_existDt, "(\\d+h\\d+m\\d+s)");
						existDtFile = _existDt;
					}
					if (_existFile != null) {
						existFile = _existFile;
						continue;
					}
					if (tsConfig.isContinueConcat) {
						if (i == 0 || existDt == null) {
							var firstFile = dirPath + "/" + name + "_ts_0h0m0s_" + i.ToString();
							string[] retb = {dirPath, firstFile, originName};
							return retb;
						} else {
							//fName = dirPath + "/" + name + "_" + ((isTimeShift) ? "ts" : "") + (i - 1).ToString();
//							if (_existDt == null) existFile = dirPath + "/" + name + "_ts_" + existDt + "_" + i.ToString() + ext;
//							existFile = Regex.Replace(existDtFile, "\\d+h\\d+m\\d+s", existDt);
							existFile = existDtFile.Substring(0, existDtFile.LastIndexOf("."));
//							existFile = existFile.Substring(0, existFile.Length - 3);
							string[] retc = {dirPath, existFile, originName};
							return retc;
						}
					} else {
						var firstFile = dirPath + "/" + name + "_ts_0h0m0s_" + i.ToString();
						string[] retd = {dirPath, firstFile, originName};
						return retd;
					}
//					continue;
				} else if (segmentSaveType == "1") {
					if (Directory.Exists(fName)) {
						string[] rete = {dirPath, fName, originName};
						return rete;
					} else if (File.Exists(fName)) {
						continue;
					}
					util.debugWriteLine(dirPath + " " + fName);
					Directory.CreateDirectory(fName);
					if (!Directory.Exists(fName)) return null;
					string[] retf = {dirPath, fName, originName};
					return retf;
				}
			}
		}
		return null;
	}
	public static string getOkFileName(string name, bool isRtmp, bool isDokuji) {
		if (isRtmp) name = getOkSJisOut(name);
		
		if (!isDokuji)
			name = name.Replace("\\", "￥");
		name = name.Replace("/", "／");
		name = name.Replace(":", "：");
		name = name.Replace("*", "＊");
		name = name.Replace("?", "？");
		name = name.Replace("\"", "”");
		name = name.Replace("<", "＜");
		name = name.Replace(">", "＞");
		name = name.Replace("|", "｜");
		/*
		string[] replaceCharacter = {"\\", "/", ":", "*", "?", "\"", "<", ">", "|"};
		foreach (string s in replaceCharacter) {
			name = name.Replace(s, "_");
		}
		*/
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
	public static string getFileName(string host, string group, string title, string lvId, string communityNum, config cfg, long _openTime) {
		var n = cfg.get("fileNameType");
		//var _hiduke = DateTime.Now;
		var _hiduke = getUnixToDatetime(_openTime);
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
		type = getOkFileName(type, false, true);
		return type;
		
	}
	public static string getFileNameTypeSample(string filenametype) {
			//var format = cfg.get("filenameformat");
			return getDokujiSetteiFileName("放送者名", "コミュ名", "タイトル", "lv12345", "co9876", filenametype, DateTime.Now).Replace("{w}", "2").Replace("{c}", "1");
		}
	public static string getOkCommentFileName(config cfg, string fName, string lvid, bool isTimeShift, bool isRtmp) {
		var kakutyousi = (cfg.get("IsgetcommentXml") == "true") ? ".xml" : ".json";
		var engineMode = cfg.get("EngineMode");
		if (cfg.get("segmentSaveType") == "0" || engineMode != "0" || isRtmp) {
			//renketu
			if (isTimeShift && engineMode == "0" && !isRtmp) {
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
			string userId, config cfg, long _openTime, bool isFmp4) {
		host = getOkFileName(host, false, false);
		group = getOkFileName(group, false, false);
		title = getOkFileName(title, false, false);
		var ext = (isFmp4 ? ".mp4" : ".ts");
		
		string[] jarpath = getJarPath();
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
		util.debugWriteLine("getLastTimeshiftFileName dirPath " + dirPath + " sfn " + sfn);

		var segmentSaveType = cfg.get("segmentSaveType");

		var name = getFileName(host, group, title, lvId, communityNum,  cfg, _openTime);
		if (name.IndexOf("\\") > -1) {
			sfn += "\\" + name.Substring(0, name.LastIndexOf("\\"));
			dirPath += "\\" + name.Substring(0, name.LastIndexOf("\\"));
			name = name.Substring(name.LastIndexOf("\\") + 1);
		}
		if (name.Length > 200) name = name.Substring(0, 200);
		
		util.debugWriteLine("getLastTimeshiftFileName name " + name);
		                    
		//長いパス調整
		if (name.Length + dirPath.Length > 234) {
			name = lvId;
			if (name.Length + dirPath.Length > 234 && sfn != null) {
				sfn = sfn.Substring(0, 3);
				dirPath = _dirPath + "/" + sfn;
				
				if (!Directory.Exists(dirPath)) {
					util.debugWriteLine("getLastTS FN too long not exist dir path " + dirPath);
					return null;
				}
			}
		}
		
		if (name.Length + dirPath.Length > 234) {
			util.debugWriteLine("too long " + name + " " + dirPath + " " + name.Length + " " + dirPath.Length);
			return null;
		}
		
		if (!Directory.Exists(dirPath)) {
			util.debugWriteLine("no exists " + dirPath);
			return null;
		}
		
		util.debugWriteLine("getLast TS FN 00");
		
		string existFile = null;
		var files = Directory.GetFiles(dirPath);
		for (int i = 0; i < 1000; i++) {
			var fName = dirPath + "/" + name + "_" + "ts" + i.ToString();
			
			if (segmentSaveType == "0") {
				//util.existFile(dirPath, name + "_ts_\\d+h\\d+m\\d+s_" + i.ToString());
				var _existFile = util.existFile(files, "_ts_(\\d+h\\d+m\\d+s)_" + i.ToString() + ext, name);
				util.debugWriteLine("getLastTimeshiftFileName existfile " + _existFile);
				if (_existFile != null) {
					existFile = _existFile;
					continue;
				}
			} else if (segmentSaveType == "1") {
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
			
			util.debugWriteLine("getLastTimeshiftFileName dirpath " + dirPath + " fname " + fName);
			
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
	public static string[] getLastTimeShiftFileTime(string lastFile, string segmentSaveType, bool isFmp4) {
		if (lastFile == null) return null;
		string fname = null;
		var ext = (isFmp4 ? ".mp4" : ".ts");
		
		if (segmentSaveType == "0") {
			fname = lastFile + "";
		} else {
			var ss = new List<string>();
			var key = new List<int>();
			foreach (var f in Directory.GetFiles(lastFile)) {
				if (!f.EndsWith(ext)) continue;
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
	private static bool isExistAllExt(string fName, string folder) {
		var ext = new string[] {".ts", ".xml", ".flv", ".avi", ".mp4",
				".mov", ".wmv", ".vob", ".mkv", ".mp3",
				".wav", ".wma", ".aac", ".ogg"};
		var files = Directory.GetFiles(folder);
		
		foreach (var e in ext) {
			var reg = new Regex(Regex.Escape(fName.Replace("\\", "/")).Replace("\\{w}", "\\d*").Replace("\\{c}", "\\d*") + e);
			foreach (var f in files) {
				if (reg.Match(f.Replace("\\", "/")).Success) return true;
				
				//if (File.Exists(fName + e)) return true;
			}
		}
		return false;
	}
	private static bool isExistDirectory(string fName, string dirPath) {
		var files = Directory.GetDirectories(dirPath);
		foreach (var f in files) {
			var reg = new Regex(Regex.Escape(fName.Replace("\\", "/")).Replace("\\{w}", "\\d*").Replace("\\{c}", "\\d*"));
			if (reg.Match(f.Replace("\\", "/")).Success) return true;
		}
		return false;
	}
	public static string incrementRecFolderFile(string recFolderFile) {
		if (recFolderFile.EndsWith("xml") || recFolderFile.EndsWith("json")) {
			var r = new Regex("(\\d+)\\.(xml|json)$");
			var m = r.Match(recFolderFile);
			if (m == null || m.Length <= 0) return null;//rp.getRecFilePath()[1];
			
			for (int i = int.Parse(m.Groups[1].Value); i < 10000; i++) {
				var _new = (i + 1).ToString() + "." + m.Groups[2];
				var _ret = r.Replace(recFolderFile, _new);
				if (File.Exists(_ret)) continue;
				return _ret;
			}
		} else {
			var r = new Regex("(\\d+)$");
			var m = r.Match(recFolderFile);
			if (m == null || m.Length <= 0) return null;//rp.getRecFilePath()[1];
			
			for (int i = int.Parse(m.Groups[1].Value); i < 10000; i++) {
				var _new = (int.Parse(m.Groups[1].Value) + 1).ToString();
				var _ret = r.Replace(recFolderFile, _new);
				if (File.Exists(_ret)) continue;
				return _ret;
			}
		}
		return null;
	}
	public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.104 Safari/537.36";
	public static string getPageSource(string _url, CookieContainer container = null, string referer = null, bool isFirstLog = true, int timeoutMs = 0, string userAgent = null, bool isGetErrorPage = false) {
		util.debugWriteLine("access__ getpage " + _url);
		if (_url == null) {
			util.debugWriteLine("getPageSource url null");
			return null;
		}
		//if (timeoutMs == 0) timeoutMs = 5000;
		timeoutMs = 5000;
		
		/*
		string a = "";
		try {
//			a = container.GetCookieHeader(new Uri(_url));
		} catch (Exception e) {
			util.debugWriteLine("getpage get cookie header error " + _url + e.Message+e.StackTrace);
			return null;
		}
		if (isFirstLog)
			util.debugWriteLine("getpagesource " + _url + " " + a);
		*/	
//		util.debugWriteLine("getpage 02");
		for (int i = 0; i < 1; i++) {
			try {
//				util.debugWriteLine("getpage 00");
				var req = (HttpWebRequest)WebRequest.Create(_url);
				req.Proxy = httpProxy;
				req.AllowAutoRedirect = true;
	//			req.Headers = getheaders;
//				util.debugWriteLine("getpage 03");
				if (referer != null) req.Referer = referer;
//				util.debugWriteLine("getpage 04");
				if (container != null) req.CookieContainer = container;
//				util.debugWriteLine("getpage 05");
				
				if (userAgent != null) 
					req.UserAgent = userAgent;
				req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

				req.Timeout = timeoutMs;
				req.KeepAlive = true;
				req.Accept = "*/*";
				
				using (var res = (HttpWebResponse)req.GetResponse())
				using (var dataStream = res.GetResponseStream())
				using (var reader = new StreamReader(dataStream)) {
					
					/*
					var resStrTask = reader.ReadToEndAsync();
					if (!resStrTask.Wait(5000)) return null;
					string resStr = resStrTask.Result;
					*/
	//				util.debugWriteLine("getpage 3");
					var resStr = reader.ReadToEnd();
	//				util.debugWriteLine("getpage 4");
					
	//				getheaders = res.Headers;
					return resStr;
				}
			} catch (Exception e) {
				System.Threading.Tasks.Task.Run(() => {
					util.debugWriteLine("getpage error " + _url + e.Message+e.StackTrace);
				});
				if (isGetErrorPage && e is WebException) {
					try {
						var res = ((WebException)e).Response;
						if (res == null) return null;
						using (var st = res.GetResponseStream())
						using (var r = new StreamReader(st)) {
							return r.ReadToEnd();
						}
					} catch (Exception ee) {
						util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
					}
				}
	//				System.Threading.Thread.Sleep(3000);
				continue;
			}
		}
			
		return null;
	}
	public static byte[] getFileBytes(string url, CookieContainer container, bool isRedirect = true, int mode = 0) {
		util.debugWriteLine("access__ getFilebytes " + url);
//		var a = container.GetCookieHeader(new Uri(_url));
		//util.debugWriteLine("getfilebyte " + url);
		for (int i = 0; i < 1; i++) {
			try {
				
				if (mode == 0 || mode == 1) {
					var req = (HttpWebRequest)WebRequest.Create(url);
					req.Proxy = httpProxy;
					req.AllowAutoRedirect = true;
					req.Timeout = 10000;
					req.KeepAlive = true;
					//req.UserAgent = "Lavf/56.36.100";
					req.Accept = "*/*";
									
		//			req.Headers = getheaders;
	//				if (referer != null) req.Referer = referer;
					
					if (container != null) req.CookieContainer = container;
					var res = (HttpWebResponse)req.GetResponse();
					var dataStream = res.GetResponseStream();
					
					if (mode == 0) {
						using (var ms = new MemoryStream()) {
							dataStream.CopyTo(ms);
							return ms.ToArray();
						}
					} else {
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
					
					}
				} else if (mode == 2) {
					using (var handler = new HttpClientHandler())
		            using (var client = new HttpClient(handler))
		            {
						
						handler.CookieContainer = container;
						handler.UseProxy = false;
						handler.AllowAutoRedirect = isRedirect;
						client.Timeout = TimeSpan.FromSeconds(10);
						
		                var result = client.GetByteArrayAsync(url).Result;
		                return result;
		            }
				}
			} catch (Exception e) {
				System.Threading.Tasks.Task.Run(() => {
					util.debugWriteLine("getfile error " + url + e.Message+e.StackTrace);
				});
//				System.Threading.Thread.Sleep(3000);
				continue;
			}
		}
		return null;
	}
	public static string postResStr(string url, Dictionary<string, string> headers, byte[] content) {
		try {
			var res = sendRequest(url, headers, content, "POST");
			if (res == null) {
				debugWriteLine("postResStr res null");
				return null;
			}
			
			debugWriteLine(res.StatusCode + " " + res.StatusDescription);
			
			//var resStream = res.GetResponseStream();
			using (var getResStream = res.GetResponseStream())
			using (var resStream = new System.IO.StreamReader(getResStream)) {
				//foreach (var h in res.Headers) Debug.WriteLine("header " + h + " " + res.Headers[h.ToString()]);
				/*
				List<byte> rb = new List<byte>();
				for (var i = 0; i < 10; i++) {
					var a = new byte[100000];
					var readC = resStream.Read(a, 0, a.Length);
					if (readC == 0) break;
					Debug.WriteLine("read c " + readC);
					for (var j = 0; j < readC; j++) rb.Add(a[j]);
					
					Debug.WriteLine("read " + i);
				}
				*/
				var resStr = resStream.ReadToEnd();
				//return getRegGroup(resStr,
				
				return resStr;
			}
		} catch (Exception ee) {
			debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			return null;
		}
	}
	public static HttpWebResponse sendRequest(string url, Dictionary<string, string> headers, byte[] content, string method, CookieContainer cc = null) {
		try {
			var req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = method;
			req.Proxy = httpProxy;
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
	public static bool isEndedProgram(string lvid, CookieContainer container, bool isSub) {
		var url = "https://live.nicovideo.jp/watch/" + lvid;
		
		//var a = new System.Net.WebHeaderCollection();
		var res = util.getPageSource(url, container);
		util.debugWriteLine("isendedprogram url " + url + " res==null " + (res == null) + util.getMainSubStr(isSub, true));
//			util.debugWriteLine("isendedprogram res " + res + util.getMainSubStr(isSub, true));
		if (res == null) return false;
		var isEnd = res.IndexOf("\"content_status\":\"closed\"") != -1 ||
				res.IndexOf("<title>番組がみつかりません") != -1 ||
				res.IndexOf("番組が見つかりません</span>") != -1;
		util.debugWriteLine("is ended program " + isEnd + util.getMainSubStr(isSub, true));
		return isEnd; 
	}
	public static string existFile(string[] files, string reg, string startWith) {
//		var files = Directory.GetFiles(dirPath);
		var startsReg = new Regex("^" + Regex.Escape(startWith).Replace("\\{w}", "\\d*").Replace("\\{c}", "\\d*"));
		
		foreach (var f in files) {
			var _f = getRegGroup(f, ".+\\\\(.+)");
			var nameLen = 0;
			var isStartsWith = false;
			if (startWith.IndexOf("{w}") > -1 || startWith.IndexOf("{c}") > -1) {
				var m = startsReg.Match(_f);
				isStartsWith = m.Success;
				nameLen = m.Length;
			} else {
				isStartsWith = _f.StartsWith(startWith);
				nameLen = startWith.Length;
			}
			if (!isStartsWith) continue;
			var _reg = util.getRegGroup(_f.Substring(nameLen), reg);
			if (_reg == null) continue;
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
			else if (res.IndexOf("isFollowerOnly&quot;:true") > -1 && res.IndexOf("isJoined&quot;:false") > -1 && res.IndexOf("[&quot;noTimeshiftProgram") == -1) return 4;
			
			else if (status == "ENDED" && res.IndexOf("rejectedReasons&quot;:[&quot;notHaveTimeshiftTicket") > -1) return 9;
			else if (status == "ENDED" && res.IndexOf("rejectedReasons&quot;:[&quot;notUseTimeshiftTicket") > -1) return 10;
			else if (data.IndexOf("webSocketUrl&quot;:&quot;ws") == -1 &&
			         status == "ENDED") return 2;
			else if (res.IndexOf("rejectedReasons&quot;:[&quot;notHavePayTicket") > -1) return 11;
			//else if (status == "ENDED" && res.IndexOf(" onclick=\"Nicolive.WatchingReservation") > -1) return 9;
			
			//else if (util.getRegGroup(res, "(に終了いたしました)") != null) return 2;
			else if (status == "ENDED") return 2;
			else if (util.getRegGroup(res, "(<archive>1</archive>)") != null) return 3;
			//else if (util.getRegGroup(res, "(チャンネル会員限定番組です。<br>)") != null) return 4;
			else if (util.getRegGroup(res, "(<h3>【会場のご案内】</h3>)") != null) return 6;
			else if (util.getRegGroup(res, "(この番組は放送者により削除されました。<br />|削除された可能性があります。<br />)") != null) return 2;
			else if (res.IndexOf("rejectedReasons&quot;:[&quot;notActivatedBySerial&quot;]") > -1) return 8;
			return 5;
		//}
		//return 5;
	}
	public static int getPageTypeRtmp(string res, ref bool isTimeshift, bool isSub) {
//		var res = getPlayerStatusRes;
		if (res.IndexOf("status=\"ok\"") > -1 && res.IndexOf("<archive>0</archive>") > -1) {
			isTimeshift = false;
			return 0;
		}
		if (res.IndexOf("status=\"ok\"") > -1 && res.IndexOf("<archive>1</archive>") > -1) {
			isTimeshift = true;
			return 7;
		}
		else if (res.IndexOf("<code>require_community_member</code>") > -1) return 4;
		else if (res.IndexOf("<code>closed</code>") > -1) return 2;
		else if (res.IndexOf("<code>comingsoon</code>") > -1) return 5;
		else if (res.IndexOf("<code>notfound</code>") > -1) return 5;
		else if (res.IndexOf("<code>deletedbyuser</code>") > -1) return 2;
		else if (res.IndexOf("<code>deletedbyvisor</code>") > -1) return 2;
		else if (res.IndexOf("<code>violated</code>") > -1) return 2;
		else if (res.IndexOf("<code>usertimeshift</code>") > -1) return 2;
		else if (res.IndexOf("<code>tsarchive</code>") > -1) return 5;
		else if (res.IndexOf("<code>unknown_error</code>") > -1) return 5;
		else if (res.IndexOf("<code>timeshift_ticket_exhaust</code>") > -1) return 2;
		else if (res.IndexOf("<code>timeshiftfull</code>") > -1) return 1;
		else if (res.IndexOf("<code>maintenance</code>") > -1) return 5;
		else if (res.IndexOf("<code>noauth</code>") > -1) return 5;
		else if (res.IndexOf("<code>full</code>") > -1) return 1;
		else if (res.IndexOf("<code>block_now_count_overflow</code>") > -1) return 5;
		else if (res.IndexOf("<code>premium_only</code>") > -1) return 5;
		else if (res.IndexOf("<code>selected-country</code>") > -1) return 5;
		else if (res.IndexOf("<code>notlogin</code>") > -1) return 8;
//		rm.form.addLogText(res + util.getMainSubStr(isSub, true));
		return 5;
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
	public static string getOkSJisOut(string s, string replaceStr = null) {
		try {
			replaceStr = replaceStr.Replace("\"", "\\\"");
			
			var a = System.Text.Encoding.GetEncoding("shift_jis");
			var r = a.GetString(a.GetBytes(s.Replace("?", "\a")));
			r = new Regex("\\?+").Replace(r, "?");
			return r.Replace("?", replaceStr == null ? "_" : replaceStr).Replace("\a", "?");
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			return s;
		}
	}
	public static bool isLogFile = false;
	public static List<string> debugWriteBuf = new List<string>();
	//public static Task debugWriteTask = null;
	public static void debugWriteLine(object str) {
		var dt = DateTime.Now.ToLongTimeString();
		
		try {
			#if DEBUG
				System.Diagnostics.Debug.WriteLine(str);
	//      		System.Diagnostics.Debug.WriteLine(
			#else
				if (isLogFile) {
					System.Console.WriteLine(dt + " " + str);
					//debugWriteBuf.Add(dt + " " + str);
				}
			#endif
		} catch (Exception e) {
//			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite + " " + e.Source);
			System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite + " " + e.Source);
		}
		
	}
	/*
	private static Task debugWriter() {
		while (true) {
			
		}
	}
	*/
	public static void showException(Exception eo, bool isMessageBox = true) {
		var frameCount = new System.Diagnostics.StackTrace().FrameCount;
		#if DEBUG
			if (isMessageBox && isLogFile) {
				if (frameCount > 150) {
					util.showMessageBoxCenterForm(null, "framecount stack", frameCount.ToString() + " " + namaichi.Program.arg + " " + DateTime.Now.ToString());
					return;
				}
			}
		#else
			
		#endif
		
		
		util.debugWriteLine("exception stacktrace framecount " + frameCount);
		util.debugWriteLine("show exception eo " + eo);
		if (eo == null) return;
		
		util.debugWriteLine("0 message exception " + eo.Message + "\nsource " + 
				eo.Source + "\nstacktrace " + eo.StackTrace + 
				"\n targetsite " + eo.TargetSite + "\n\n");
		
		var _eo = eo.GetBaseException();
		util.debugWriteLine("eo " + _eo);
		if (_eo != null) {
			util.debugWriteLine("1 message exception " + _eo.Message + "\nsource " + 
					_eo.Source + "\nstacktrace " + _eo.StackTrace + 
					"\n targetsite " + _eo.TargetSite + "\n\n");
		}
		
		_eo = eo.InnerException;
		util.debugWriteLine("eo " + _eo);
		if (_eo != null) {
			util.debugWriteLine("2 message exception " + _eo.Message + "\nsource " + 
					_eo.Source + "\nstacktrace " + _eo.StackTrace + 
					"\n targetsite " + _eo.TargetSite);
		}
		
		#if DEBUG
			if (isMessageBox && isLogFile)
				util.showMessageBoxCenterForm(null, "error " + eo.Message, "error " + namaichi.Program.arg);
		#else
			
		#endif
	}
	public static void setLog(config config, string lv) {
		if (bool.Parse(config.get("IsLogFile"))) {
			//var name = (args.Length == 0) ? "lv_" : util.getRegGroup(args[0], "(lv\\d+)");
			var name = (lv == null) ? "lv_" : lv;
			var logPath = util.getJarPath()[0] + "/" + name + ".txt";
			
			try {
				#if DEBUG
					System.Diagnostics.DefaultTraceListener dtl
				      = (System.Diagnostics.DefaultTraceListener)System.Diagnostics.Debug.Listeners["Default"];
					dtl.LogFileName = logPath;
				#else
					if (isStdIO) return; 
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
	public static bool isOkDotNet() {
		var ver = Get45PlusFromRegistry();
		return ver >= 4.52;
	}
	public static double Get45PlusFromRegistry() {
		const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
	
		using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
		{
			if (ndpKey != null && ndpKey.GetValue("Release") != null) {
				return CheckFor45PlusVersion((int) ndpKey.GetValue("Release"));
//			util.consoleWrite(".NET Framework Version: " + CheckFor45PlusVersion((int) ndpKey.GetValue("Release")));
			}
			else {
	//			util.consoleWrite(".NET Framework Version 4.5 or later is not detected.");
			} 
		}
		return -1;
	}
	private static double CheckFor45PlusVersion(int releaseKey)
   {
	  util.debugWriteLine("releasekey " + releaseKey);
      if (releaseKey >= 461808)
         return 4.72; //later
      if (releaseKey >= 461308)
         return 4.71;
      if (releaseKey >= 460798)
         return 4.7;
      if (releaseKey >= 394802)
         return 4.62;
      if (releaseKey >= 394254)
         return 4.61;      
      if (releaseKey >= 393295)
         return 4.6;      
      if (releaseKey >= 379893)
         return 4.52;      
      if (releaseKey >= 378675)
         return 4.51;      
      if (releaseKey >= 378389)
       return 4.5;      
    return -1;
   }
	public static string getMainSubStr(bool isSub, bool isKakko = false) {
		var ret = (isSub) ? "サブ" : "メイン";
		if (isKakko) ret = "(" + ret + ")";
		return ret;		
	}
public static void soundEnd(config cfg, MainForm form) {
		try {
			var path = (bool.Parse(cfg.get("IsSoundDefault"))) ? 
					(util.getJarPath()[0] + "/Sound/se_soc02.wav") : cfg.get("soundPath");
			if (path == "") path = util.getJarPath()[0] + "/Sound/se_soc02.wav";
			
			var volume = float.Parse(cfg.get("soundVolume")) / 100;
			if (volume < 0) volume = (float)0;
			if (volume > 1) volume = (float)1;
			util.debugWriteLine("sound path " + path + " volume " + volume);
			
			var isMCI = true;
			if (isMCI) {
				volume *= 1000;
				playSoundMCI(path, (int)volume, false, form);
			} else {
				//naudio
				/*
				var reader = new NAudio.Wave.AudioFileReader(path);
				var waveOut = new NAudio.Wave.WaveOut();
				waveOut.Init(reader);
				
				waveOut.Volume = volume;
				util.debugWriteLine("volume " + waveOut.Volume);
				waveOut.Play();
				while (waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing) Thread.Sleep(100);
				//waveOut.Dispose();
				reader.Close();
				*/
			}
			
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}                                
	}
	[System.Runtime.InteropServices.DllImport("winmm.dll",
    		CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
	private static extern int mciSendString(string command,
				[MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
	[System.Runtime.InteropServices.DllImport("winmm.dll",
    		CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public static extern bool mciGetErrorString(int dwError, 
			[MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder lpstrBuffer, int uLength);
    
	public static void playSoundMCI(string path, int volume, bool isAsync, MainForm form) {
		debugWriteLine("playsound mci " + path + " volume " + volume + " " + isAsync);
		
		var name = Guid.NewGuid();
	    var cmd = "open \"" + path + "\" type mpegvideo alias " + name;
	    debugWriteLine(cmd);
	    var ret = mciSendStringForm(cmd, null, 0, IntPtr.Zero, form);
	    
	    if (ret != 0) {
	    	debugWriteLine("playsound mci open error " + ret + " " + cmd);
	    	StringBuilder errMsg = new StringBuilder(1000);
			mciGetErrorString(ret, errMsg, 1000);
			debugWriteLine("mci err " + errMsg);
			form.addLogText("サウンドの再生中に問題が発生しました。ERROR:" + ret + ", メッセージ:" + errMsg + " パス:" + path + ", volume:" + volume);
	        return;
	    }
	    ret = mciSendStringForm("setaudio " + name + " volume to " + volume.ToString(), null, 0, IntPtr.Zero, form);
	    
	    mciSendStringForm("play " + name, null, 0, IntPtr.Zero, form);
	    
	    if (!isAsync) {
	    	StringBuilder status = new StringBuilder();
		    while(true) {
	    		Thread.Sleep(1000);
		    	status = new StringBuilder();
		    	ret = mciSendStringForm("status " + name + " mode", status, 256, IntPtr.Zero, form);
		    	
		    	debugWriteLine("rc " + ret + " " + status);
		    	if (status.ToString() != "playing") break;
		    }
	    }
	}
	private static int mciSendStringForm(string command,
			StringBuilder buffer, int bufferSize, IntPtr hwndCallback, MainForm form) {
		var ret = 0;
		form.formAction(() => 
				ret = mciSendString(command, buffer, bufferSize, hwndCallback), false);
		return ret;
	}
	[DllImport("kernel32.dll")]
	extern static int SetThreadExecutionState(uint esFlags);
	public static void setThreadExecutionState() {
		try {
			
			var r = SetThreadExecutionState((uint)1);
			//var r2 = SetThreadExecutionState((uint)2);
			//var r2 = SetThreadExecutionState(0x80000000);
			util.debugWriteLine("setThreadExecutionState " + r);
			
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
	}
	public static void dllCheck(namaichi.MainForm form) {
		var path = getJarPath()[0];
		var dlls = new string[]{"websocket4net.dll", 
				//"NAudio.dll",
				//"RtmpSharp2.dll", 
				"SnkLib.App.CookieGetter.Forms.dll",
				"SnkLib.App.CookieGetter.dll", "SuperSocket.ClientEngine.dll",
				"Microsoft.Web.XmlTransform.dll", "Newtonsoft.Json.dll",
				"System.Data.SQLite.dll", "x64/SQLite.Interop.dll",
				"x86/SQLite.Interop.dll", "x86/SnkLib.App.CookieGetter.x86Proxy.exe"};
		var isOk = new string[dlls.Length];
		var msg = "";
		foreach (var n in dlls) {
			if (!File.Exists(path + "/" + n)) 
				msg += (msg == "" ? "" : ",") + n;
		}
		if (msg == "") return;
		form.formAction(() => util.showMessageBoxCenterForm(form, path + "内に" + msg + "が見つかりませんでした"));
	}
	public static bool getStatistics(string lvid, CookieContainer cc, out string visit, out string comment) {
		visit = "0";
		comment = "0";
		var url = "https://live.nicovideo.jp/watch/" + lvid;
		var res = getPageSource(url, cc);
		if (res == null) return false;
		var _visit = util.getRegGroup(res, "statistics&quot;:{&quot;watchCount&quot;:(\\d+),&quot;commentCount&quot;:\\d+}");
		var _comment = util.getRegGroup(res, "statistics&quot;:{&quot;watchCount&quot;:\\d+,&quot;commentCount&quot;:(\\d+)}");
		if (_visit == null || _comment == null) return false;
		visit = _visit;
		comment = _comment;
		return true;
	}
	public static void setProxy(config cfg, MainForm form = null) {
		util.httpProxy = null;
		util.wsProxy = null;
		if (!bool.Parse(cfg.get("useProxy"))) return;
		
		var proxyAddress = cfg.get("proxyAddress");
		var proxyPort = cfg.get("proxyPort");
		if (proxyAddress == "" && proxyPort == "") return;
		try {
			var ip = IPAddress.Parse(proxyAddress);
			var endpoint = new IPEndPoint(ip, int.Parse(proxyPort));
			util.wsProxy = new HttpConnectProxy(endpoint);
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			if (form != null)
				form.addLogText("WebSocketのプロキシの設定に失敗しました。アドレス:" + proxyAddress + "ポート：" + proxyPort);
		}
		try {
			util.httpProxy = new WebProxy(proxyAddress, int.Parse(proxyPort));
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			if (form != null)
				form.addLogText("HTTPのプロキシの設定に失敗しました。アドレス:" + proxyAddress + "ポート：" + proxyPort);
		}
	}
	public static bool loginCheck(CookieContainer cc, string url, string log = null) {
		try {
			var us = cc.GetCookies(new Uri(url))["user_session"];
			if (us == null) {
				if (log != null)
					log += "Cookie内にユーザーセッションが見つかりませんでした。";
				return false;
			}
			var uid = util.getRegGroup(us.Value, "user_session_(.+?)_");
			if (uid == null) {
				if (log != null)
					log += "ユーザーセッション内にIDが見つかりませんでした。";
				return false;
			}
			var _url = "https://public.api.nicovideo.jp/v1/user/followees/niconico-users/" + uid + ".json";
			var res = util.getPageSource(_url, cc);
			if (log != null)
				log += res != null ? "ログインを確認できました。" : "ログインを確認できませんでした";
			return res != null;
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			return false;
		}
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
			var r = sendRequest(url, _h, null, "GET");
				
			if (r == null) return null;
			using (var st = r.GetResponseStream())
			using (var sr = new StreamReader(st)) {
				var res = sr.ReadToEnd();
				var n = util.getRegGroup(res, "\"nickname\":\"(.+?)\"");
				return n;
			}
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
		return null;
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
	public static void shutdown(string recEndProcess, Form form) {
		try {
			var p = new Process();
			p.StartInfo.FileName = "shutdown.exe";
			
			if (recEndProcess == "OSをシャットダウンする") {
				p.StartInfo.Arguments = "/s /t 60";
			} else if (recEndProcess == "OSをログオフする")
				p.StartInfo.Arguments = "/l";
			else if (recEndProcess == "OSを休止状態にする")
				p.StartInfo.Arguments = "/h";
			p.Start();
			
			if (recEndProcess == "OSをシャットダウンする") {
				var r = util.showMessageBoxCenterForm(form, "1分後にシャットアウトしてよろしいですか？", "確認", MessageBoxButtons.YesNo);
				if (r == DialogResult.No) {
					var _p = new Process();
					_p.StartInfo.FileName = "shutdown.exe";
					p.StartInfo.Arguments = "/a";
					p.Start();
				}
			}
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
	}
	public static string getAboutNumStr(string s) {
		if (s == null) return " ";
		if (s.Length >= 9) return s.Substring(0, s.Length - 8) + "億";
		else if (s.Length >= 8) return s.Substring(0, s.Length - 7) + "千万";
		else if (s.Length >= 5) return s.Substring(0, s.Length - 4) + "万";
		return s;
	}
	public static string getReplacedComment(string t, List<string[]> commentReplaceList) {
		if (commentReplaceList == null) return t;
		try {
			foreach (var s in commentReplaceList) {
				t = new Regex(s[0]).Replace(t, s[1]);
			}
			return t;
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			return t;
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
	public static void consoleWrite(string o) {
		try {
			Console.WriteLine(o);
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		}
	}
}
