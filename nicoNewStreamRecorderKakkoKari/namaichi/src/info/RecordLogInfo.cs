/*
 * Created by SharpDevelop.
 * User: ajkkh
 * Date: 2023/07/30
 * Time: 0:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using namaichi.info;

//namespace namaichi.info
//{
	/// <summary>
	/// Description of RecordInfo.
	/// </summary>
	public class RecordLogInfo
	{
		public static DateTime startTime = DateTime.MinValue;
		public static DateTime endTime = DateTime.MinValue;
		public static string lvid = "";
		public static string loginType = "";
		public static string foundLastUs = "";
		public static string loginLog = "";
		public static string recType = ""; //realtime timeshift chase
		//public static string lastState = "";
		public static string recordedStatus = "";
		public static List<string> recBytesData = new List<string>();
		
		public RecordLogInfo()
		{
		}
		public static void clear() {
			startTime = endTime = DateTime.MinValue;
			lvid = loginType = foundLastUs = loginLog = 
					recType = recordedStatus = "";
			recBytesData.Clear();
		}
		public static string getText() {
			var osName = util.osName;
			if (osName != null){
				 var _osName = util.getRegGroup(util.osName, "(.+)\\(");
				 if (_osName != null) osName = _osName;
			}
			var r = "開始時間: " + startTime.ToString() + "\r\n";
			r += "終了時間: " + endTime.ToString() + "\r\n";
			r += "放送ID: " + lvid + "\r\n";
			r += "ログイン方法: " + loginType + "\r\n";
			r += "前回のユーザーセッション: " + foundLastUs + "\r\n";
			r += "ログインログ: " + loginLog + "\r\n";
			r += "動作種別: " + recType + "\r\n";;
			r += "転送状況: " + recordedStatus + "\r\n";
			r += "OS: " + osName + "\r\n";
			return r;
		}
		public static string getFileText() {
			var r = string.Join("\r\n", recBytesData.ToArray());
			return r;
		}
		public static void addrecBytesData(numTaskInfo nti) {
			try {
				recBytesData.Add(nti.no + "," + (nti.res == null ? "null" : nti.res.Length.ToString()) + "," + nti.second);
			} catch (Exception e) {
				recBytesData.Add("nti error " + e.Message + e.Source);
			}
		}
	}
//}
