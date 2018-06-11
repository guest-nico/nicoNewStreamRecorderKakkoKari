using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using namaichi.config;

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
		Console.WriteLine(m.Groups.Count +""+ m.Groups[0]);
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
//			System.Diagnostics.Debug.WriteLine(Environment.GetCommandLineArgs().Length);
			f=System.IO.Path.GetFileName(f);

			string withoutKakutyousi = (f.IndexOf(".") < 0) ? f :
					util.getRegGroup(f,"^(.*)\\.");
			string kakutyousi = (f.IndexOf(".") < 0) ? null :
					util.getRegGroup(f,"^.*\\.(.*)");
			
			System.Diagnostics.Debug.WriteLine(getPath() + " " +withoutKakutyousi+" "+kakutyousi);
			//0-dir 1-withoutKakutyousi 2-kakutyousi
			return new String[]{getPath(), withoutKakutyousi, kakutyousi};
		}
	}

	public static string[] getRecFolderFilePath(string host, string group, string title, string lvId, string communityNum, string userId, config cfg) {
		
		host = getOkFileName(host);
		group = getOkFileName(group);
		title = getOkFileName(title);
		
		string[] jarpath = getJarPath();
//		System.Diagnostics.Debug.WriteLine(jarpath);
		//string dirPath = jarpath[0] + "\\rec\\" + host;
		string dirPath = (cfg.get("IsdefaultRecordDir") == "true") ?
			(jarpath[0] + "\\rec") : cfg.get("recordDir");
		
		if (cfg.get("IscreateSubfolder") == "true") {
			var sfn = getSubFolderName(host, group, title, lvId, communityNum, userId,  cfg);
			if (sfn == null) return null;
			dirPath += "/" + sfn;
		}
		if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
		if (!Directory.Exists(dirPath)) return null;

		for (int i = 0; i < 1000000; i++) {
			//ナツココ_co3387528(ドル円死亡遊戯)_lv298866554(ドル円死亡遊戯)0
			string name = dirPath + "/" + getFileName(host, group, title, lvId, communityNum,  cfg) + i;
			
			//string name = dirPath + "/" + host + "_" + communityNum + "(" + group + ")_" + lvId + "(" + title + ")" + i + "";
			
//			if (i == 54) return new String[]{dirPath, name};
			
			if (File.Exists(name + ".ts")) continue;
			
			System.Diagnostics.Debug.WriteLine(dirPath + " " + name);
			string[] ret = {dirPath, name};
			return ret;
		}
		return null;
	}
	public static string getOkFileName(string name) {
		string[] replaceCharacter = {"\\", "/", ":", "*", "?", "\"", "<", ">", "|"};
		foreach (string s in replaceCharacter) {
			name = name.Replace(s, " ");
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
	public static string getPageSource(string _url, ref WebHeaderCollection getheaders, CookieContainer container = null, string referer = "") {
		for (int i = 0; i < 3; i++) {
			try {
				var req = (HttpWebRequest)WebRequest.Create(_url);
				req.Proxy = null;
				req.AllowAutoRedirect = true;
	//			req.Headers = getheaders;
				req.Referer = referer;
				if (container != null) req.CookieContainer = container;
				var res = (HttpWebResponse)req.GetResponse();
				var dataStream = res.GetResponseStream();
				var reader = new StreamReader(dataStream);
				string resStr = reader.ReadToEnd();
				
				getheaders = res.Headers;
				return resStr;
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(e.Message+e.StackTrace);
				System.Threading.Thread.Sleep(3000);
				continue;
			}
		}
		return null;
	}
	public static int getPageType(string res) {
		var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
		if (res.IndexOf("<!doctype html>") > -1 && data != null) return 0;
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
	public static void writeFile(string name, string str) {
		var f = new System.IO.FileStream(name, FileMode.Append);
		var w = new StreamWriter(f);
		w.WriteLine(str);
		w.Close();
		f.Close();
	}
}