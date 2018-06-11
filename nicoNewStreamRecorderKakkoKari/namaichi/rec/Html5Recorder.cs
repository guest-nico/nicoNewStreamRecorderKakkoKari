/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/04/15
 * Time: 0:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Html5Recorder.
	/// </summary>
	
		
	public class Html5Recorder
	{
		private string url;
		private CookieContainer container;
		private string lvid;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		
	
		public Html5Recorder(string url, CookieContainer container, 
				string lvid, RecordingManager rm, RecordFromUrl rfu)
		{
			this.url = url;
			this.container = container;
			this.lvid = lvid;
			this.rm = rm;
			this.rfu = rfu;
			
		}
		public int record(string res) {
			
			
//			for (int i = 0; i < webSocketRecInfo.Length; i++)
//				System.Diagnostics.Debug.WriteLine(webSocketRecInfo[i]);
//			for (int i = 0; i < recFolderFileInfo.Length; i++)
//				System.Diagnostics.Debug.WriteLine(recFolderFileInfo[i]);
			
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			return html5Record(res).Result;
		
		}
		/*
		private string getPageSource(string _url) {
			var req = (HttpWebRequest)WebRequest.Create(_url);
			req.CookieContainer = container;
			var res = (HttpWebResponse)req.GetResponse();
			var dataStream = res.GetResponseStream();
			var reader = new StreamReader(dataStream);
			string resStr = reader.ReadToEnd();
			
			return resStr;
			
		}
		*/
		private string[] getWebSocketInfo(string data) {
//			System.Diagnostics.Debug.WriteLine(data);
			var wsUrl = util.getRegGroup(data, "\"webSocketUrl\":\"([\\d\\D]+?)\"");
			System.Diagnostics.Debug.WriteLine(wsUrl);
			var broadcastId = util.getRegGroup(wsUrl, "/(\\d+)\\?");
			System.Diagnostics.Debug.WriteLine(broadcastId);
			string request = "{\"type\":\"watch\",\"body\":{\"command\":\"getpermit\",\"requirement\":{\"broadcastId\":\"" + broadcastId + "\",\"route\":\"\",\"stream\":{\"protocol\":\"hls\",\"requireNewStream\":false,\"priorStreamQuality\":\"normal\"},\"room\":{\"isCommentable\":true,\"protocol\":\"webSocket\"}}}}";
			System.Diagnostics.Debug.WriteLine(request);
			return new string[]{wsUrl, request};
		}
		private string[] getHtml5RecFolderFileInfo(string data, string type) {
			string host, group, title, communityNum, userId;
			host = group = title = communityNum = userId = null;
			if (type == "official") {
				host = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
				if (util.getRegGroup(data, "(\"socialGroup\".\\{\\},)") != null) host = "公式生放送";
	//			if (host == null) host = "official";
				group = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.+?)\"");
	//			group = "蜈ｬ蠑冗函謾ｾ騾・;
				if (group == null) group = "official";
				title = util.getRegGroup(data, "\"title\"\\:\"(.+?)\",");
//				title = util.uniToOriginal(title);
				communityNum = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
				if (communityNum == null) communityNum = "official";
				userId = "official";
				
				System.Diagnostics.Debug.WriteLine(host + " " + group + " " + title + " " + communityNum);
				if (host == null || group == null || title == null || communityNum == null) return null;
				System.Diagnostics.Debug.WriteLine(host + " " + group + " " + title + " " + communityNum);
			} else {
				bool isAPI = false;
				if (isAPI) {
					var a = new System.Net.WebHeaderCollection();
					var apiRes = util.getPageSource(url + "/programinfo", ref a, container);
				
				} else {
					var isChannel = util.getRegGroup(data, "visualProviderType\":\"(channel)\",\"title\"") != null;
		//			host = util.getRegGroup(res, "provider......name.....(.*?)\\\\\"");
					group = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
		//			group = util.uniToOriginal(group);
		//			group = util.getRegGroup(res, "communityInfo.\".+?title.\"..\"(.+?).\"");
					host = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.+?)\"");
		//			System.out.println(group);
		//			host = util.uniToOriginal(host);
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\:\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\:\\\"(.*?)\\\"");
		//			title = util.getRegGroup(res, "\\\\\"programHeader\\\\\":\\{\\\\\"thumbnailUrl.+?\\\\\"title\\\\\":\\\\\"(.*?)\\\\\"");
					title = util.getRegGroup(data, "visualProviderType\":\"(community|channel)\",\"title\":\"(.+?)\",", 2);
		//			communityNum = util.getRegGroup(res, "socialGroup: \\{[\\s\\S]*registrationUrl: \"http://com.nicovideo.jp/motion/(.*?)\\?");
					communityNum = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
		//			community = util.getRegGroup(res, "socialGroup\\:)");
					userId = (isChannel) ? "channel" : (util.getRegGroup(data, "supplier\":{\"name\".+?pageUrl\":\"http://www.nicovideo.jp/user/(\\d+?)\""));
					System.Diagnostics.Debug.WriteLine("userid " + userId);
		
					System.Diagnostics.Debug.WriteLine("title " + title);
					System.Diagnostics.Debug.WriteLine("community " + communityNum);
		//			community = util.getRegGroup(res, "socialGr(oup:)");
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\\:\\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\\:\\\"(.*?)\\\"");
					//  ,\"programHeader\":{\"thumbnailUrl\":\"http:\/\/icon.nimg.jp\/community\/s\/123\/co1231728.jpg?1373210036\",\"title\":\"\u56F2\u7881\",\"provider
		//			title = util.uniToOriginal(title);
					
					System.Diagnostics.Debug.WriteLine(host + " " + group + " " + title + " " + communityNum + " userid " + userId);
					if (host == null || group == null || title == null || communityNum == null || userId == null) return null;
				}
			}
			return new string[]{host, group, title, lvid, communityNum, userId};

		}
		async private Task<int> html5Record(string res) {
			//webSocketInfo 0-wsUrl 1-request
			//recFolderFileInfo host, group, title, lvid, communityNum
			//return 0-end stream 1-stop
			
	//		List<Cookie> cookies = context.getCookieStore().getCookies();
	//		for (Cookie cookie : cookies) System.out.println(cookie.getName() + " " + cookie.getValue());
			

	//		ExecutorService exec = Executors.newFixedThreadPool(1);
			
			
	
			string[] webSocketRecInfo;
			string[] recFolderFileInfo;
			
			Task displayTask = null;
			
			var isNoPermission = false;
			while(rm.rfu == rfu) {
				var type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
				var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
				if (data == null) {
					
					var pageType = util.getPageType(res);
					
					//processType 0-ok 1-retry 2-放送終了 3-その他の理由の終了
					var processType = processFromPageType(pageType);
					//if (processType == 0 || processType == 1) continue;
					if (processType == 2) return 3;
//					if (processType == 3) return 0;
					
					System.Threading.Thread.Sleep(3000);
					
					res = getPageSourceFromNewCookie();
					
					continue;
				}
				
				
				
				data = System.Web.HttpUtility.HtmlDecode(data);
				var openTime = long.Parse(util.getRegGroup(data, "\"beginTime\":(\\d+)"));
//				var openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"));
							
				
	//			System.Diagnostics.Debug.WriteLine(data);
				
				//0-wsUrl 1-request
				webSocketRecInfo = getWebSocketInfo(data);
				if (webSocketRecInfo == null) continue;
				if (isNoPermission) webSocketRecInfo[1] = webSocketRecInfo[1].Replace("\"requireNewStream\":false", "\"requireNewStream\":true");
				recFolderFileInfo = getHtml5RecFolderFileInfo(data, type);
				
				
			
				//display set
				Task.Run(() => {
			         	var b = new RecordStateSetter(rm.form, rm, rfu);
			         	b.set(data, type, recFolderFileInfo);

//			         	int abcd;
			         });
//				new RecordStateSetter().set(data, rm.form, type, recFolderFileInfo);
			
				//System.Threading.Thread.Sleep(20000);
				
				
				System.Diagnostics.Debug.WriteLine("rm.rfu " + rm.rfu.GetHashCode() + " rfu " + rfu.GetHashCode());
				string[] recFolderFile = util.getRecFolderFilePath(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg);
				
				System.Diagnostics.Debug.WriteLine("recforlderfie test");
				System.Diagnostics.Debug.WriteLine("recforlderfi test " + recFolderFileInfo);
				
				if (!rm.form.IsDisposed) {
		        	rm.form.Invoke((MethodInvoker)delegate() {
				        var fileName = System.IO.Path.GetFileName(recFolderFile[1]);
				        rm.form.Text = fileName;
					});
				}
				
				
				System.Diagnostics.Debug.WriteLine("recforlderfie");
				System.Diagnostics.Debug.WriteLine("recforlderfi " + recFolderFileInfo);
				
				if (recFolderFile == null) continue;
				
				for (int i = 0; i < recFolderFile.Length; i++)
					System.Diagnostics.Debug.WriteLine("recd " + i + " " + recFolderFileInfo[i]);
				
				
				var wsr = new WebSocketRecorder(webSocketRecInfo, container, recFolderFile, rm, rfu, this, openTime);
				try {
					isNoPermission = wsr.start();
				} catch (Exception e) {
					System.Diagnostics.Debug.WriteLine(e.Message + e.StackTrace);
				}
				
				if (rm.rfu != rfu) break;
				
				res = getPageSourceFromNewCookie();
				
				
				
			}
			return 1;
		}
		private string getPageSourceFromNewCookie() {
			CookieGetter _cg = null;
			Task<CookieContainer> _cgtask = null;
			while (rm.rfu == rfu) {
				try {
					_cg = new CookieGetter(rm.cfg);
					_cgtask = _cg.getHtml5RecordCookie(url);
					_cgtask.Wait();
					
					if (_cgtask == null || _cgtask.Result == null) {
						System.Threading.Thread.Sleep(3000);
						continue;
					}
					
					container = _cgtask.Result;
					return _cg.pageSource;
				} catch (Exception e) {
					System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
				} 
				
	//			var _c = new System.Net.WebHeaderCollection();
	//			return util.getPageSource(url, ref _c, container);
			}
			return "";
		}
		/*
		public bool isAliveStream() {
			var a = new System.Net.WebHeaderCollection();
			string res = util.getPageSource(url, ref a, container);
				
//			string res = getPageSource(url);
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			data = System.Web.HttpUtility.HtmlDecode(data);
			string[] info = getWebSocketInfo(data);
			return (info == null) ? false : true; 
		}
		*/
		private int processFromPageType(int pageType) {
			//ret 0-ok 1-retry 2-放送終了 3-その他の理由の終了
			if (pageType == 0) {
				return 0;
			} else if (pageType == 1) {
				rm.form.addLogText("満員です。");
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
				
			} else if (pageType == 5) {
				rm.form.addLogText("接続エラー。10秒後リトライします。");
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
			} else if (pageType == 6) {
//				rm.form.addLogText("接続エラー。10秒後リトライします。");
				System.Threading.Thread.Sleep(3000);
				return 1;
				
			} else if (pageType == 4) {
				rm.form.addLogText("require_community_menber");
				if (bool.Parse(rm.cfg.get("IsmessageBox"))) {
					if (rm.form.IsDisposed) return 2;
		        	rm.form.Invoke((MethodInvoker)delegate() {
		       			MessageBox.Show("コミュニティに入る必要があります：\nrequire_community_menber/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
					});
				}
				if (bool.Parse(rm.cfg.get("IsfailExit"))) {
					rm.rfu = null;
					rm.form.Invoke((MethodInvoker)delegate() {
		       			rm.form.Close();
					});
					
				}
				return 3;
				
			} else {
				var mes = "";
				if (pageType == 2) mes = "この放送は終了しています。";
				if (pageType == 3) mes = "この放送は終了しています。";
				rm.form.addLogText(mes);
				
				if (bool.Parse(rm.cfg.get("IsdeleteExit"))) {
					rm.rfu = null;
					rm.form.Invoke((MethodInvoker)delegate() {
		       			rm.form.Close();
					});
					
				}
				return 2;
				//var nh5r = new NotHtml5Recorder(url, container, lvid, rm, this);
				//nh5r.record(res);
			}
			
		}
	}
}
