using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using namaichi.config;
using namaichi.info;

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

		var name = getFileName(host, group, title, lvId, communityNum,  cfg);
		if (name.Length > 200) name = name.Substring(0, 200);
		
		//長いパス調整
		if (name.Length + dirPath.Length > 235) {
			name = lvId;
			if (name.Length + dirPath.Length > 240 && sfn != null) {
				sfn = sfn.Substring(0, 3);
				dirPath = _dirPath + "/" + sfn;
								
				if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
				if (!Directory.Exists(dirPath)) return null;
				
			}
		}
		if (name.Length + dirPath.Length > 235) return new string[]{null, name + " " + dirPath};
		
		if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
		if (!Directory.Exists(dirPath)) return null;
		
		var files = Directory.GetFiles(dirPath);
		string existFile = null;
		for (int i = 0; i < 1000000; i++) {
			var fName = dirPath + "/" + name + "_" + ((isTimeShift) ? "ts" : "") + i.ToString();
			util.debugWriteLine(dirPath + " " + fName);
			
			if (!isTimeShift) {
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
		else if (n == "10") return getDokujiSetteiFileName(host, group, title, lvId, communityNum, cfg.get("filenameformat"));
		else return host + "_" + communityNum + "(" + group + ")_" + lvId + "(" + title + ")";
		
		
	}
	public static string getDokujiSetteiFileName(string host, string group, string title, string lvId, string communityNum, string format) {
		var type = format;
		if (type == null) return "";
		var dt = DateTime.Now;
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
		
		return type;
		
	}
	public static string getFileNameTypeSample(string filenametype) {
			//var format = cfg.get("filenameformat");
			return getDokujiSetteiFileName("放送者名", "コミュ名", "タイトル", "lv12345", "co9876", filenametype);
		}
	public static string getOkCommentFileName(config cfg, string fName, string lvid, bool isTimeShift) {
		var kakutyousi = (cfg.get("IsgetcommentXml") == "true") ? ".xml" : ".json";
		if (cfg.get("segmentSaveType") == "0") {
			//renketu
			if (isTimeShift) {
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
		if (name.Length + dirPath.Length > 235) {
			name = lvId;
			if (name.Length + dirPath.Length > 240 && sfn != null) {
				sfn = sfn.Substring(0, 3);
				dirPath = _dirPath + "/" + sfn;
								
				if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
				if (!Directory.Exists(dirPath)) return null;
				
			}
		}
		if (name.Length + dirPath.Length > 230) return null;
		
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
	public static string getPageSource(string _url, ref WebHeaderCollection getheaders, CookieContainer container = null, string referer = null, bool isFirstLog = true) {
		string a;
		try {
			a = container.GetCookieHeader(new Uri(_url));
		} catch (Exception e) {
			util.debugWriteLine("getpage get cookie header error " + _url + e.Message+e.StackTrace);
			return null;
		}
		if (isFirstLog)
			util.debugWriteLine("getpagesource " + _url + " " + a);
			
		for (int i = 0; i < 1; i++) {
			try {
				var req = (HttpWebRequest)WebRequest.Create(_url);
				req.Proxy = null;
				req.AllowAutoRedirect = true;
	//			req.Headers = getheaders;
				if (referer != null) req.Referer = referer;
				if (container != null) req.CookieContainer = container;

				req.Timeout = 5000;
				var res = (HttpWebResponse)req.GetResponse();
				var dataStream = res.GetResponseStream();
				var reader = new StreamReader(dataStream);
				
				/*
				var resStrTask = reader.ReadToEndAsync();
				if (!resStrTask.Wait(5000)) return null;
				string resStr = resStrTask.Result;
				*/
				var resStr = reader.ReadToEnd();
				
				getheaders = res.Headers;
				return resStr;
	
			} catch (Exception e) {
				util.debugWriteLine("getpage error " + _url + e.Message+e.StackTrace);
	//				System.Threading.Thread.Sleep(3000);
				continue;
			}
		}
			
		return null;
	}
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
		var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
		var status = (data == null) ? null : util.getRegGroup(data, "&quot;status&quot;:&quot;(.+?)&quot;");
		if (res.IndexOf("<!doctype html>") > -1 && data != null && status == "ON_AIR") return 0;
		else if (res.IndexOf("<!doctype html>") > -1 && data != null && status == "ENDED") return 7;
		else if (util.getRegGroup(res, "(混雑中ですが、プレミアム会員の方は優先して入場ができます)") != null ||
		        util.getRegGroup(res, "(ただいま、満員のため入場できません)") != null) return 1;
//		else if (util.getRegGroup(res, "<div id=\"comment_arealv\\d+\">[^<]+この番組は\\d+/\\d+/\\d+\\(.\\) \\d+:\\d+に終了いたしました。<br>") != null) return 2;
		else if (util.getRegGroup(res, "(に終了いたしました)") != null) return 2;
		else if (util.getRegGroup(res, "(<archive>1</archive>)") != null) return 3;
		else if (util.getRegGroup(res, "(コミュニティフォロワー限定番組です。<br>)") != null) return 4;
		else if (util.getRegGroup(res, "(チャンネル会員限定番組です。<br>)") != null) return 4;
		else if (util.getRegGroup(res, "(<h3>【会場のご案内】</h3>)") != null) return 6;
		else return 5;
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
					MessageBox.Show("framecount stack", frameCount.ToString());
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
				MessageBox.Show("error", "error");
		#else
			
		#endif
	}
	public static void setLog(config config, string lv) {
		//test
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
}
