/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/15
 * Time: 11:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using namaichi.info;
using Newtonsoft.Json;

namespace namaichi.rec
{
	/// <summary>
	/// Description of XmlCommentGetter.
	/// </summary>
	public class XmlCommentGetter_ontime
	{
		private string lvid;
		private CookieContainer container;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private string recFolderFile;
		private IRecorderProcess rp;
		private long openTime;
		private long _openTime;
		private long serverTime;
		
		private StreamWriter commentSW;
		private string commentFileName;
		private bool isTimeShift;
		private bool isRtmp;
		
		private string isGetComment;
		private string isGetCommentXml;
		private bool isReset = false;
		private bool isRetry = true;
		
		private string ticket;
		
		public XmlCommentGetter_ontime(string lvid, 
				CookieContainer container, RecordingManager rm, 
				RecordFromUrl rfu, string recFolderFile, 
				IRecorderProcess rp, bool isTimeShift, bool isRtmp,
				long openTime, long _openTime, long serverTime)
		{
			this.lvid = lvid;
			this.container = container;			
			this.rm = rm;
			this.rfu = rfu;
			this.recFolderFile = recFolderFile;
			this.rp = rp;
			this.isTimeShift = isTimeShift;
			this.isRtmp = isRtmp;
			this.isGetComment = rm.cfg.get("IsgetComment");
			this.isGetCommentXml = rm.cfg.get("IsgetcommentXml");
			this.openTime = openTime;
			this._openTime = _openTime;
			this.serverTime = serverTime;
		}
		public void get() {
//			rm.form.addComment("RTMPモードでは画面にコメント・来場者数・コメント数が表示されません");
			
			Task.Run(() => {heartBeater();});
			while (rm.rfu == rfu && isRetry) {
				isReset = false;
				
				string address, port, thread;
				if (!getInfo(out address, out port, out thread)) continue;
				var tcp = new TcpClient(address, int.Parse(port));
				var stream = tcp.GetStream();
				
				setStreamWriter();
				
				var request = Encoding.UTF8.GetBytes("<thread thread=\"" + thread + "\" version=\"20061206\" res_from=\"-5\" /> \0");
				stream.Write(request, 0, request.Length);
				stream.Flush();
				
				var b = new byte[(isTimeShift) ? (500 * 1000) : (10 * 1000)];
				var lastI = b.Length;
				while (rm.rfu == rfu && !isReset && isRetry) {
					var i = stream.Read(b, 0, b.Length);
					for (var j = i + 0; j < b.Length && j < lastI; j++)
						b[j] = 0;
					var s = Encoding.UTF8.GetString(b).Trim('\0');
					var ss = s.Split('\0');
					util.debugWriteLine(i + " " + ss + " ssLen " + ss.Length);
					
					foreach (var c in ss) {
						try {
							chatProcess(c);
						} catch (Exception e) {
							util.debugWriteLine(c);
							util.debugWriteLine(e.Message + e.StackTrace + e.Source + e.TargetSite);
						}
					}
					
					
				}
				stream.Close();
			}
		}
		private void chatProcess(string c) {
			XDocument chatXml = null;
			chatXml = XDocument.Parse(c);

			
			var chatinfo = new ChatInfo(chatXml);
			chatinfo.getFromXml(serverTime);
//			chatinfo.getFormatXml(serverTime);
			
			util.debugWriteLine("xml " + chatXml.ToString());
			
			if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 &&
					chatinfo.premium == "3")) return;
			if (chatinfo.root != "chat" && chatinfo.root != "thread") return;
			
			if (chatinfo.root == "thread") {
				serverTime = chatinfo.serverTime;
				ticket = chatinfo.ticket;
			}
			
			
//			Newtonsoft.Json
			//if (e.Message.IndexOf("chat") < 0 &&
			//    	e.Message.IndexOf("thread") < 0) return;
			
			
//            addDebugBuf(jsonCommentToXML(text));
			try {
				if (commentSW != null) {
					
					
					string writeStr = null;
					if (bool.Parse(isGetCommentXml)) 
						writeStr = chatXml.ToString();
					else {
						var json = JsonConvert.SerializeXNode(chatXml.FirstNode);
						writeStr = Regex.Replace(json,
			            	"\"vpos\"\\:(\\d+)", 
			            	"\"vpos\":" + chatinfo.vpos + "");
					}
					commentSW.WriteLine(writeStr);
					commentSW.Flush();
					util.debugWriteLine("write comment " + writeStr);
		             
				}
           
			} catch (Exception ee) {util.debugWriteLine("comment write exception " + ee.Message + " " + ee.StackTrace);}
			
			//if (!isTimeShift)
				addDisplayComment(chatinfo);

				
		}
		private void addDisplayComment(namaichi.info.ChatInfo chat) {
			
			if (chat.root.Equals("thread")) return;
			if (chat.contents == "再読み込みを行いました<br>読み込み中のままの方はお手数ですがプレイヤー下の更新ボタンをお試し下さい") {
				util.debugWriteLine("chat 再読み込みを行いました");
				return;
			}
			if (chat.contents == null) return;
//			var time = util.getUnixToDatetime(chat.vpos / 100);
//			var unixKijunDt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			var __time = chat.date - openTime; //- (60 * 60 * 9);

//			var __timeDt = util.getUnixToDatetime(__time);
//			var openTimeDt = util.getUnixToDatetime(openTime);
//			var __timeDt0 = __timeDt - openTimeDt;
//			var __timeDt1 = __timeDt0. - new timespunixKijunDt;
			var h = (int)(__time / (60 * 60));
			var m = (int)((__time % (60 * 60)) / 60);
			var _m = (m < 10) ? ("0" + m.ToString()) : m.ToString();
			var s = __time % 60;
			var _s = (s < 10) ? ("0" + s.ToString()) : s.ToString();
			var keikaTime = h + ":" + _m + ":" + _s + "";
			/*
//			- unixKijunDt;
			
//			var __time = new TimeSpan(chat.vpos * 10000);
			var h = (int)(__timeSpan.TotalHours);
			var m = __timeSpan.Minutes;
			var s = __timeSpan.Seconds;
			*/
//			- new TimeSpan(9,0,0);
			var c = (chat.premium == "3") ? "red" :
				((chat.premium == "7") ? "blue" : "black");
			
			if (!isTimeShift)
				rm.form.addComment(keikaTime, chat.contents, chat.userId, chat.score, c);
			
		}
		private bool getInfo(out string address, out string port, out string thread) {
			var url = "https://live.nicovideo.jp/api/getplayerstatus?v=" + lvid;
			var res = util.getPageSource(url, container, null, false, 5000);
			
			var xml = new XmlDocument();
			xml.LoadXml(res);
			try {
				address = xml.SelectSingleNode("/getplayerstatus/ms/addr").InnerText;
				port = xml.SelectSingleNode("/getplayerstatus/ms/port").InnerText;
				thread = xml.SelectSingleNode("/getplayerstatus/ms/thread").InnerText;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				address = port = thread = null;
				return false;
			}
			return true;
		}
		private void setStreamWriter() {
			try {
				var isGetComment = rm.cfg.get("IsgetComment");
				var isGetCommentXml = rm.cfg.get("IsgetcommentXml");
				
				if (bool.Parse(isGetComment) && commentSW == null && !rfu.isPlayOnlyMode) {
					var fName = (commentFileName == null) ? recFolderFile : incrementRecFolderFile(commentFileName);
					commentFileName = fName;
					var _commentFileName = util.getOkCommentFileName(rm.cfg, fName, lvid, isTimeShift, isRtmp);
					var isExists = File.Exists(_commentFileName);
					commentSW = new StreamWriter(_commentFileName, false, System.Text.Encoding.UTF8);
					
					if (bool.Parse(isGetCommentXml) && !isExists) {
//						Newtonsoft.Json.JsonConvert.SerializeXNode
					} 
			       
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		private void heartBeater() {
			while (rm.rfu == rfu) {
				try {
					var url = "http://ow.live.nicovideo.jp/api/heartbeat";
					var handler = new System.Net.Http.HttpClientHandler();
					handler.UseCookies = true;
					handler.CookieContainer = container;
					var http = new System.Net.Http.HttpClient(handler);
					handler.UseProxy = true;
					handler.Proxy = util.httpProxy;
					http.Timeout = TimeSpan.FromSeconds(5);					
					var contentStr = "{\"lang\":\"ja-jp\",\"locale\":\"JP\",\"seat_locale\":\"JP\",\"screen\":\"ScreenNormal\",\"v\":\"" + lvid + "\",\"datarate\":0}";
					
					var content = new System.Net.Http.StringContent(contentStr);
					content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
	
					var _res = http.PostAsync(url, content).Result;
					var res = _res.Content.ReadAsStringAsync();

				} catch (Exception e) {
					util.debugWriteLine(e.Message+e.StackTrace + e.Source + e.TargetSite);
				}
				Thread.Sleep(60000);
				
			}
		}
		public void resetCommentFile() {
			isReset = true;
		}
		private string incrementRecFolderFile(string recFolderFile) {
			var r = util.incrementRecFolderFile(recFolderFile);
			if (r == null) return rp.getRecFilePath()[1];
			return r;
		}
	}
}
