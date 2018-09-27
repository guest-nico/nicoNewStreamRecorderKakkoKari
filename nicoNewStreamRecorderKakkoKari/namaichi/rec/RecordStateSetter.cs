/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/26
 * Time: 19:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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
		private MainForm form;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private bool isTimeShift;
<<<<<<< HEAD
		private bool isJikken = false;
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift, bool isJikken)
=======
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu, bool isTimeShift)
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		{
			this.form = form;
			this.rm = rm;
			this.rfu = rfu;
			this.isTimeShift = isTimeShift;
<<<<<<< HEAD
			this.isJikken = isJikken;
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		}
		public void set(string data, string type, string[] recFolderFileInfo) {
			setInfo(data, form, type, recFolderFileInfo);
//			var a = await setInfo(data, form, type, recFolderFileInfo).ConfigureAwait(false);
			
			Task.Run(() => setSamune(data, form));
			
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
		private DateTime getOpenTimeDt(long startunix) {
			return util.getUnixToDatetime(startunix);
		}
		private void setInfo(string data, MainForm form, string type, string[] recFolderFileInfo) {
			
			//recfolderfileinfo host, group, title, lvid, communityNum, userId

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
			if (!isJikken) {
				hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "supplier\":{\"name\".\".+?\",\"pageUrl\":\"(.+?)\"") : null;
				groupUrl = util.getRegGroup(data, "\"socialGroupPageUrl\":\"(.+?)\"");
				gentei = (data.IndexOf("\"isFollowerOnly\":true") > -1) ? "限定" : "オープン";
	//			var _openTime = long.Parse(util.getRegGroup(data, "\"openTime\":(\\d+)"));
				_openTime = long.Parse(util.getRegGroup(data, "\"beginTime\":(\\d+)"));
			} else {
				hostUrl = (type == "community" || type == "user") ? util.getRegGroup(data, "broadcaster\":{.+?\"pageUrl\":\"(.+?)\"") : null;
				groupUrl = "https://com.nicovideo.jp/community/" + recFolderFileInfo[4];
				gentei = (data.IndexOf("\"type\":\"memberOnly\"") > -1) ? "限定" : "オープン";
				_openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)")) / 1000;
			}
			openTimeDt = getOpenTimeDt(_openTime);
			var openTime = openTimeDt.ToString("MM/dd(ddd) HH:mm:ss");
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			
			form.setInfo(host, hostUrl, group, groupUrl, title, url, gentei, openTime, description);
		}
		private void setSamune(string data, MainForm form) {
			var samuneUrl = util.getRegGroup(data, "\"program\".+?\"thumbnail\":{\"imageUrl\":\"(.+?)\"");
			form.setSamune(samuneUrl);
		}
	}
}
