﻿/*
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
using System.Net;

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
		private bool isJikken = false;
		private string[] recFolderFile;
		
		public string openTime;
		private string endTime;
		public string title;
		private string gentei;
		public string host;
		public string group;
		private string description;
		public string url;
		public string groupUrl;
		public string hostUrl;
		public string samuneUrl;
		private string tag;
		public int watchCount = 0;
		public int commentCount = 0;
		
		private bool isPlayOnlyMode = false;
		private bool isDescriptionTag;
		private bool isRtmpOnlyPage = false;
		private bool isChase = false;
			
		public bool isWrite = false;
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift, bool isJikken, string[] recFolderFile, bool isPlayOnlyMode, bool isRtmpOnlyPage, bool isChase)
		{
			this.form = form;
			this.rm = rm;
			this.rfu = rfu;
			this.isTimeShift = isTimeShift;
			this.isJikken = isJikken;
			this.recFolderFile = recFolderFile;
			this.isPlayOnlyMode = isPlayOnlyMode;
			this.isDescriptionTag = bool.Parse(rm.cfg.get("IsDescriptionTag"));
			this.isRtmpOnlyPage = isRtmpOnlyPage;
			this.isChase = isChase;
		}
		public void set(string data, string type, string[] recFolderFileInfo) {
			setInfo(data, form, type, recFolderFileInfo);
//			var a = await setInfo(data, form, type, recFolderFileInfo).ConfigureAwait(false);
			
			Task.Run(() => setSamune(data, form));
			
			if (util.isStdIO) writeStdIOInfo();
			
			if (isTimeShift) {
				return;
			}
			
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

			host = recFolderFileInfo[0];
			group = recFolderFileInfo[1];
			title = recFolderFileInfo[2];
			url = util.getRegGroup(data, "\"watchPageUrl\":\"(.+?)\"");
			if (url == null) url = util.getRegGroup(data, "og:url\" content=\"(.+?)\"");
			description = util.getRegGroup(data, "\"program\".+?\"description\":\"(.+?)\",\"");
			if (description == null) description = util.getRegGroup(data, "<description>(.+?)</description>");
			
			description = description.Replace("\\n", " ");
			if (!isDescriptionTag) {
				
				try {
					description = Regex.Replace(description, "<script>.*?</script>", "");
					description = Regex.Replace(description, "<.*?>", "");
					description = description.Replace("\\\"", "\"");
	//				description = description.Replace("", "\"");
				} catch(Exception e) {
					util.debugWriteLine(e.Message);
				}
			}
//			string hostUrl, groupUrl, gentei;
			long _openTime, _endTime;
			
			if (!isJikken) {
				hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "supplier\":{\"name\".\".+?\",\"pageUrl\":\"(.+?)\"") : null;
				groupUrl = util.getRegGroup(data, "\"socialGroupPageUrl\":\"(.+?)\"");
				gentei = (data.IndexOf("\"isFollowerOnly\":true") > -1) ? "限定" : "オープン";
	//			var _openTime = long.Parse(util.getRegGroup(data, "\"openTime\":(\\d+)"));
				var _openTimeStr = util.getRegGroup(data, "\"beginTime\":(\\d+)");
				var _endTimeStr = util.getRegGroup(data, "\"endTime\":(\\d+)");
				
				if (_openTimeStr == null) _openTimeStr = util.getRegGroup(data, "<start_time>(\\d+)");
				if (_endTimeStr == null) _endTimeStr = util.getRegGroup(data, "<end_time>(\\d+)");
				if (_openTimeStr == null) {
					_openTimeStr = "0";
					_endTimeStr = "0";
				}
				_openTime = long.Parse(_openTimeStr);
				_endTime = long.Parse(_endTimeStr);
			} else {
				hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "broadcaster\":{.+?\"pageUrl\":\"(.+?)\"") : null;
				groupUrl = "https://com.nicovideo.jp/community/" + recFolderFileInfo[4];
				gentei = (data.IndexOf("\"type\":\"memberOnly\"") > -1) ? "限定" : "オープン";
				_openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)")) / 1000;
				_endTime = long.Parse(util.getRegGroup(data, "\"endTimeMs\":(\\d+)")) / 1000;
			}
			openTimeDt = getUnixToDt(_openTime);
			openTime = openTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
			endTimeDt = getUnixToDt(_endTime);
			endTime = endTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
			
			//samuneUrl = util.getRegGroup(data, "\"program\".+?\"thumbnail\":{\"imageUrl\":\"(.+?)\"");
			samuneUrl = util.getRegGroup(data, "\"thumbnailImageUrl\":\"(.+?)\"");
			if (samuneUrl == null) samuneUrl = util.getRegGroup(data, "\"small\":\"(.+?)\"");
			if (samuneUrl == null) samuneUrl = util.getRegGroup(data, "thumbnail:.+?'(https*://.+?)'");
			if (samuneUrl == null) samuneUrl = util.getRegGroup(data, "<thumb_url>(.+?)</thumb_url>");
			tag = getTag(data);
			var formEndTime = (isTimeShift && !isChase) ? endTime : "";
			setStatistics(data);
			form.setInfo(host, hostUrl, group, groupUrl, title, url, gentei, openTime, description, isJikken, formEndTime);
		}
		private void setSamune(string data, MainForm form) {
			form.setSamune(samuneUrl);
		}
		public void writeHosoInfo() {
			while (openTime == null) 
				System.Threading.Thread.Sleep(100);
			
			var ext = (isDescriptionTag) ? ".html" : ".txt";
			StreamWriter sw;
			try {
				using (sw = new StreamWriter(recFolderFile[2] + ext, false)) {
					var br = (isDescriptionTag) ? "<br />" : "";
					sw.WriteLine("[放送開始時間] " + openTimeDt.ToString("yyyy/MM/dd(ddd) HH:mm:ss") + br);
					sw.WriteLine("[タイトル] " + title + br);
					sw.WriteLine("[限定] " + gentei + br);
					sw.WriteLine("[放送タイプ] " + ((isJikken) ? "nicocas" : (isRtmpOnlyPage) ? "nicolive" : "nicolive2") + br);
					sw.WriteLine("[放送者] " + host + br);
					sw.WriteLine("[コミュニティ名] " + group + br);
					sw.WriteLine("[説明] " + description + br);
					sw.WriteLine("[放送URL] " + url + br);
					if (groupUrl != null)
						sw.WriteLine("[コミュニティURL] " + groupUrl + br);
					if (hostUrl != null)
						sw.WriteLine("[放送者URL] " + hostUrl + br);
					sw.WriteLine("[タグ] " + tag + br);
					if (isTimeShift && !isChase) sw.WriteLine("[放送終了時間] " + endTimeDt.ToString("yyyy/MM/dd(ddd) HH:mm:ss") + br);
					//sw.Close();
					
					isWrite = true;
				}
			} catch (Exception e) {
				rm.form.addLogText("番組情報をテキストへ保存中に問題が発生しました。 " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				rm.form.addLogText(recFolderFile[2] + ext);
				return;
			}
			
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
		private string getTag(string data) {
			var _t = util.getRegGroup(data, "\"tag\":\\{.*?\"list\":\\[{(\"text.+?)\\]");
			MatchCollection m;
			if (_t == null) {
//				var __t = util.getRegGroup(data, "<ul id=\"livetags\"(.+?)</ul>");
//				if (__t == null) return "取得できませんでした";
				m = new Regex("keyword=(.+?)&amp").Matches(data);
//				if (mm.Count == 0) return "取得できませんでした";
//				foreach (Match _m in m) 
//					util.debugWriteLine(_m.Groups[1]);
			} else {
				m = Regex.Matches(_t, "\"text\":\"(.*?)\"");
			}
			var ret = "";
			foreach (var _m in m) {
				if (ret != "") ret += ",";
				ret += ((Match)_m).Groups[1];
			}
			return ret;	
		}
		public void writeEndTime(CookieContainer cc, DateTime endTime) {
			if (endTime == DateTime.MinValue) return;
			
			var ext = (isDescriptionTag) ? ".html" : ".txt";
			//StreamWriter sw;
			try {
				if (!File.Exists(recFolderFile[2] + ext)) return;
				using (var sw = new StreamWriter(recFolderFile[2] + ext, true)) {
					var br = (isDescriptionTag) ? "<br />" : "";
					sw.WriteLine("[放送終了時間] " + endTime.ToString("MM/dd(ddd) HH:mm:ss") + br);
				}
				//sw.Close();
			} catch (Exception e) {
				rm.form.addLogText("放送終了時間を番組情報テキストへ書き込みに問題が発生しました。 " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				rm.form.addLogText(recFolderFile[2] + ext);
				return;
			}
		}
		private void setStatistics(string data) {
			try {
				var _watchCount = util.getRegGroup(data, "\"statistics\":\\{\"watchCount\":(\\d+),\"commentCount\":\\d*\\}");
				var _commentCount = util.getRegGroup(data, "\"statistics\":\\{\"watchCount\":\\d*,\"commentCount\":(\\d+)\\}");
				if (_watchCount != null) watchCount = int.Parse(_watchCount);
				if (_commentCount != null) commentCount = int.Parse(_commentCount);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		public void renameStatistics(string watchCount, string commentCount) {
			var ext = (isDescriptionTag) ? ".html" : ".txt";
			var newName = (recFolderFile[2] + ext).Replace("{w}", watchCount).Replace("{c}", commentCount);
			try {
				if (!File.Exists(recFolderFile[2] + ext)) return;
				if (newName == recFolderFile[2] + ext)
					return;
				if (File.Exists(newName) && File.Exists(recFolderFile[2] + ext)) 
					File.Delete(newName);
				File.Move(recFolderFile[2] + ext, newName);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				
			}
		}
	}
}
