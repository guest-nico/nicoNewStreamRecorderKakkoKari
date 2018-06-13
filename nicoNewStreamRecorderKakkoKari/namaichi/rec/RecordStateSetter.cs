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
		public RecordStateSetter(MainForm form, RecordingManager rm, RecordFromUrl rfu)
		{
			this.form = form;
			this.rm = rm;
			this.rfu = rfu;
		}
		public void set(string data, string type, string[] recFolderFileInfo) {
			setInfo(data, form, type, recFolderFileInfo);
//			var a = await setInfo(data, form, type, recFolderFileInfo).ConfigureAwait(false);
			
			Task.Run(() => setSamune(data, form));
			
			while (rm.rfu == rfu) {
				var _keikaJikanDt = (DateTime.Now - openTimeDt);
				var keikaJikan = _keikaJikanDt.ToString("h'時間'm'分's'秒'");
				form.setKeikaJikan(keikaJikan);
				System.Threading.Thread.Sleep(100);
			}
			
		}
		private DateTime getOpenTimeDt(long startunix) {
			return util.getUnixToDatetime(startunix);
		}
		private void setInfo(string data, MainForm form, string type, string[] recFolderFileInfo) {
			
			//recfolderfileinfo host, group, title, lvid, communityNum, userId

			var host = recFolderFileInfo[0];
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
			var description = util.getRegGroup(data, "\"program\".+?\"description\":\"(.+?)\",\"").Replace("\\n", " ");
			try {
				description = Regex.Replace(description, "<.*?>", "");
				description = description.Replace("\\\"", "\"");
//				description = description.Replace("", "\"");
			} catch(Exception e) {
				util.debugWriteLine(e.Message);
			}
			
			form.setInfo(host, hostUrl, group, groupUrl, title, url, gentei, openTime, description);
		}
		private void setSamune(string data, MainForm form) {
			var samuneUrl = util.getRegGroup(data, "\"program\".+?\"thumbnail\":{\"imageUrl\":\"(.+?)\"");
			form.setSamune(samuneUrl);
		}
	}
}
