/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/26
 * Time: 19:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace namaichi.rec
{
	/// <summary>
	/// Description of RecordStateSetter.
	/// </summary>
	public class RecordStateSetter
	{
		private DateTime openTimeDt;
		private DateTime endTimeDt;
		private MainForm form;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private bool isTimeShift;
<<<<<<< HEAD
		private bool isJikken = false;
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		private string[] recFolderFile;
		
		private string openTime;
		private string endTime;
		private string title;
		private string gentei;
		private string host;
		private string group;
		private string description;
		private string url;
		private string groupUrl;
		private string hostUrl;
		private string samuneUrl;
<<<<<<< HEAD
		private string tag;
		
		private bool isPlayOnlyMode = false;
			
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift, bool isJikken, string[] recFolderFile, bool isPlayOnlyMode)
=======
			
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift, bool isJikken, string[] recFolderFile)
=======
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift, bool isJikken)
=======
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift)
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		{
			this.form = form;
			this.rm = rm;
			this.rfu = rfu;
			this.isTimeShift = isTimeShift;
<<<<<<< HEAD
			this.isJikken = isJikken;
<<<<<<< HEAD
			this.recFolderFile = recFolderFile;
			this.isPlayOnlyMode = isPlayOnlyMode;
=======
<<<<<<< HEAD
			this.recFolderFile = recFolderFile;
=======
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
		}
		public void set(string data, string type, string[] recFolderFileInfo) {
			setInfo(data, form, type, recFolderFileInfo);
//			var a = await setInfo(data, form, type, recFolderFileInfo).ConfigureAwait(false);
			
			Task.Run(() => setSamune(data, form));
			
			if (util.isStdIO) writeStdIOInfo();
			
			if (isTimeShift) return;
			
			/*
			while (rm.rfu == rfu) {
				var _keikaJikanDt = (DateTime.Now - openTimeDt);
				var keikaJikan = _keikaJikanDt.ToString("h'時間'm'分's'秒'");
				form.setKeikaJikan(keikaJikan, "a");
				System.Threading.Thread.Sleep(100);
			}
			*/
		}
		private DateTime getUnixToDt(long startunix) {
			return util.getUnixToDatetime(startunix);
		}
		private void setInfo(string data, MainForm form, string type, string[] recFolderFileInfo) {
			
			//recfolderfileinfo host, group, title, lvid, communityNum, userId

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			host = recFolderFileInfo[0];
			group = recFolderFileInfo[1];
			title = recFolderFileInfo[2];
			url = util.getRegGroup(data, "\"watchPageUrl\":\"(.+?)\"");
			description = util.getRegGroup(data, "\"program\".+?\"description\":\"(.+?)\",\"");
			if (rm.cfg.get("IsDescriptionTag") == "false") {
				description = description.Replace("\\n", " ");
				try {
					description = Regex.Replace(description, "<script>.*?</script>", "");
					description = Regex.Replace(description, "<.*?>", "");
					description = description.Replace("\\\"", "\"");
	//				description = description.Replace("", "\"");
				} catch(Exception e) {
					util.debugWriteLine(e.Message);
				}
<<<<<<< HEAD
			}
//			string hostUrl, groupUrl, gentei;
			long _openTime, _endTime;
=======
			}
//			string hostUrl, groupUrl, gentei;
			long _openTime, _endTime;
=======
			var host = recFolderFileInfo[0];
<<<<<<< HEAD
			var group = recFolderFileInfo[1];
			var title = recFolderFileInfo[2];
			var url = util.getRegGroup(data, "\"watchPageUrl\":\"(.+?)\"");
=======
			var hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "supplier\":{\"name\".\".+?\",\"pageUrl\":\"(.+?)\"") : null;
			var group = recFolderFileInfo[1];
			var groupUrl = util.getRegGroup(data, "\"socialGroupPageUrl\":\"(.+?)\"");
			var title = recFolderFileInfo[2];
			var url = util.getRegGroup(data, "\"watchPageUrl\":\"(.+?)\"");
			var gentei = (data.IndexOf("\"isFollowerOnly\":true") > -1) ? "限定" : "オープン";
//			var _openTime = long.Parse(util.getRegGroup(data, "\"openTime\":(\\d+)"));
			var _openTime = long.Parse(util.getRegGroup(data, "\"beginTime\":(\\d+)"));
			
			openTimeDt = getOpenTimeDt(_openTime);
			var openTime = openTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			var description = util.getRegGroup(data, "\"program\".+?\"description\":\"(.+?)\",\"").Replace("\\n", " ");
			try {
				description = Regex.Replace(description, "<.*?>", "");
				description = description.Replace("\\\"", "\"");
//				description = description.Replace("", "\"");
			} catch(Exception e) {
				util.debugWriteLine(e.Message);
			}
<<<<<<< HEAD
			string hostUrl, groupUrl, gentei;
			long _openTime;
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			if (!isJikken) {
				hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "supplier\":{\"name\".\".+?\",\"pageUrl\":\"(.+?)\"") : null;
				groupUrl = util.getRegGroup(data, "\"socialGroupPageUrl\":\"(.+?)\"");
				gentei = (data.IndexOf("\"isFollowerOnly\":true") > -1) ? "限定" : "オープン";
	//			var _openTime = long.Parse(util.getRegGroup(data, "\"openTime\":(\\d+)"));
				_openTime = long.Parse(util.getRegGroup(data, "\"beginTime\":(\\d+)"));
				_endTime = long.Parse(util.getRegGroup(data, "\"endTime\":(\\d+)"));
			} else {
				hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "broadcaster\":{.+?\"pageUrl\":\"(.+?)\"") : null;
				groupUrl = "https://com.nicovideo.jp/community/" + recFolderFileInfo[4];
				gentei = (data.IndexOf("\"type\":\"memberOnly\"") > -1) ? "限定" : "オープン";
				_openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)")) / 1000;
				_endTime = long.Parse(util.getRegGroup(data, "\"endTimeMs\":(\\d+)")) / 1000;
			}
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			openTimeDt = getUnixToDt(_openTime);
			openTime = openTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
			endTimeDt = getUnixToDt(_endTime);
			endTime = endTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
<<<<<<< HEAD
=======
			
			samuneUrl = util.getRegGroup(data, "\"program\".+?\"thumbnail\":{\"imageUrl\":\"(.+?)\"");
=======
			openTimeDt = getOpenTimeDt(_openTime);
			var openTime = openTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			
			samuneUrl = util.getRegGroup(data, "\"program\".+?\"thumbnail\":{\"imageUrl\":\"(.+?)\"");
			tag = getTag(data);
			form.setInfo(host, hostUrl, group, groupUrl, title, url, gentei, openTime, description);
		}
		private void setSamune(string data, MainForm form) {
			form.setSamune(samuneUrl);
		}
		public void writeHosoInfo() {
			while (openTime == null) 
				System.Threading.Thread.Sleep(100);
			
			var sw = new StreamWriter(recFolderFile[2] + ".txt", false);
			sw.WriteLine("[放送開始時間] " + openTime);
			sw.WriteLine("[タイトル] " + title);
			sw.WriteLine("[限定] " + gentei);
			sw.WriteLine("[放送タイプ] " + ((isJikken) ? "nicocas" : "nicolive2"));
			sw.WriteLine("[放送者] " + host);
			sw.WriteLine("[コミュニティ名] " + group);
			sw.WriteLine("[説明] " + description);
			sw.WriteLine("[放送URL] " + url);
			if (groupUrl != null)
				sw.WriteLine("[コミュニティURL] " + groupUrl);
			if (hostUrl != null)
				sw.WriteLine("[放送者URL] " + hostUrl);
<<<<<<< HEAD
			sw.WriteLine("[タグ] " + tag);
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
			sw.Close();
		}
		private void writeStdIOInfo() {
			Console.WriteLine("info.title:" + title);
			Console.WriteLine("info.host:" + host);
			Console.WriteLine("info.communityName:" + group);
			Console.WriteLine("info.url:" + url);
			Console.WriteLine("info.communityUrl:" + groupUrl);
			Console.WriteLine("info.description:" + description);
			Console.WriteLine("info.startTime:" + openTime);
			Console.WriteLine("info.endTime:" + endTime);
			var ts = (endTimeDt - openTimeDt);
			Console.WriteLine("info.programTime:" + ts.ToString("h'時間'mm'分'ss'秒'"));
//			var a = "info.programTime:" + ts.ToString("h'時間'mm'分'ss'秒'");
//			util.debugWriteLine(a);
			Console.WriteLine("info.samuneUrl:" + samuneUrl);
		}
<<<<<<< HEAD
		private string getTag(string data) {
			var _t = util.getRegGroup(data, "\"tag\":\\{\"list\":\\[(.+?)\\]");
			if (_t == null) return "取得できませんでした";
			var m = Regex.Matches(data, "\"text\":\"(.*?)\"");
			var ret = "";
			foreach (var _m in m) {
				if (ret != "") ret += ",";
				ret += ((Match)_m).Groups[1];
			}
			return ret;	
		}
=======
>>>>>>> b77d287f700e628ca0b621134ab8ddd993dbb4fc
	}
}
