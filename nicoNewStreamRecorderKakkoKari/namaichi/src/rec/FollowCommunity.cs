/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/29
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.IO;
using namaichi.config;
using namaichi.utility;

namespace namaichi.rec
{
	/// <summary>
	/// Description of FollowCommunity.
	/// </summary>
	public class FollowCommunity
	{
		//private bool isSub = false;
		public FollowCommunity()
		{
			//this.isSub = isSub;
		}
		public bool followCommunity(string res, CookieContainer cc, MainForm form, config.config cfg, bool isPlayOnlyMode) {
			var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
			var comId = (isJikken) ? util.getRegGroup(res, "&quot;followPageUrl&quot;\\:&quot;.+?motion/(.+?)&quot;") :
					//util.getRegGroup(res, "Nicolive_JS_Conf\\.Recommend = \\{type\\: 'community', community_id\\: '(co\\d+)'");
					util.getRegGroup(res, "community&quot;,&quot;id&quot;:&quot;(.+?)&");
				
			if (comId == null) {
				form.addLogText("この放送はフォローできませんでした。");
				return false;
			}
			
			var isJoinedTask = join2(comId, cc, form, cfg, isPlayOnlyMode);
//			isJoinedTask.Wait();
			return isJoinedTask;
//			return false;
		}
		private bool join2(string comId, CookieContainer cc, MainForm form, config.config cfg, bool isPlayOnlyMode) {
			var comUrl = "https://com.nicovideo.jp/community/" + comId;
			var comApiUrl = "https://com.nicovideo.jp/api/v1/communities.json?community_ids=" + comId.Substring(2);
			var joinUrl = "https://com.nicovideo.jp/api/v1/communities/" + comId.Substring(2) + "/follows.json";
			var headers = new Dictionary<string, string>();
			headers.Add("Accept", "application/json, text/plain, */*");
			headers.Add("Accept-Encoding", "gzip, deflate, br");
			headers.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
			headers.Add("Referer", "https://com.nicovideo.jp/motion/" + comId);
			headers.Add("User-Agent", util.userAgent);
			headers.Add("Cookie", cc.GetCookieHeader(new Uri(comApiUrl)));
			try {
				var res = "";
				var r = util.sendRequest(comApiUrl, headers, null, "GET", cc);
				using (var sr = new StreamReader(r.GetResponseStream())) {
					res = sr.ReadToEnd();
					util.debugWriteLine(res);
				}
				if (res == "") {
					form.addLogText("コミュニティ情報の取得に失敗しました");
					return false;
				}
				var isJidouShounin = res.IndexOf("\"community_auto_accept_entry\":1") > -1;
				var msg = (isJidouShounin ? "フォローを試みます。" : "自動承認ではありませんでした。");
				form.addLogText(msg);
				if (!isJidouShounin) return false;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				form.addLogText("何らかの問題によりフォローに失敗しました " + e.Message + e.StackTrace);
				return false;
			}
			
			try {
				headers.Remove("Content-Type");
				headers.Add("Content-Type", "application/x-www-form-urlencoded");
				headers.Add("Origin", "https://com.nicovideo.jp");
				headers.Add("X-Requested-By", "https://com.nicovideo.jp/motion/" + comId);
				foreach (var h in headers) util.debugWriteLine(h.Key + " " + h.Value);
				
				//var res = util.postResStr(joinUrl, headers, null, "POST");
				var res = "";
				var r = util.sendRequest(joinUrl, headers, null, "POST", cc);
				using (var sr = new StreamReader(r.GetResponseStream())) {
					res = sr.ReadToEnd();
					util.debugWriteLine(res);
				}
				var isSuccess = res.IndexOf("\"status\":200") > -1; 
				var _m = (isPlayOnlyMode) ? "視聴" : "録画";
				form.addLogText((isSuccess ?
						"フォローしました。" + _m + "開始までしばらくお待ちください。" : "フォローに失敗しました。"));
				return isSuccess;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return false;
			}
		}
		/*
		private bool join(string comId, CookieContainer cc, MainForm form, config.config cfg, bool isPlayOnlyMode) {
			for (int i = 0; i < 5; i++) {
				//var myPageUrl = "https://www.nicovideo.jp/my";
				var comUrl = "https://com.nicovideo.jp/community/" + comId; 
				var url = "https://com.nicovideo.jp/motion/" + comId;
				var headers = new WebHeaderCollection();
				headers.Add("Upgrade-Insecure-Requests", "1");
				headers.Add("User-Agent", util.userAgent);
				try {
					var cg = new CookieGetter(cfg);
					var cgret = cg.getHtml5RecordCookie(url);
					cgret.Wait();
					                                  
					
		//			cgret.ConfigureAwait(false);
					if (cgret == null || cgret.Result == null) {
						System.Threading.Thread.Sleep(3000);
						continue;
					}
					var _cc = cgret.Result[0];
//					var _cc = cgret.Result[(isSub) ? 1 : 0];
//					util.debugWriteLine(cg.pageSource);
					
					var res = util.getPageSource(url, _cc, comUrl);
					var isJidouShounin = res.IndexOf("自動承認されます") > -1;
	//				var _compage = util.getPageSource(url, ref headers, cc);
	//				var gateurl = "http://live.nicovideo.jp/gate/lv313793991";
	//				var __gatePage = util.getPageSource(gateurl, ref headers, cc);
	//				var _compage2 = util.getPageSource(url, ref headers, cc);
//					util.debugWriteLine(cc.GetCookieHeader(new Uri(url)));
					var msg = (isJidouShounin ? "フォローを試みます。" : "自動承認ではありませんでした。");
					form.addLogText(msg);
					
					if (!isJidouShounin) return false;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					return false;
				}
				
				
				try {
					var handler = new System.Net.Http.HttpClientHandler();
					handler.UseCookies = true;
					handler.CookieContainer = cc;
					handler.Proxy = util.httpProxy;
					
					
					var http = new System.Net.Http.HttpClient(handler);
					http.DefaultRequestHeaders.Referrer = new Uri(url);
					
					var content = new System.Net.Http.FormUrlEncodedContent(new Dictionary<string, string>
					{
						{"mode", "commit"}, {"title", "フォローリクエスト"}
					});
					
					var enc = Encoding.GetEncoding("UTF-8");
					string data =
					    "mode=commit&title=" + System.Web.HttpUtility.UrlEncode("フォローリクエスト", enc);
					byte[] postDataBytes = Encoding.ASCII.GetBytes(data);
	
					
					var req = (HttpWebRequest)WebRequest.Create(url);
					req.Method = "POST";
					req.Proxy = util.httpProxy;
					req.CookieContainer = cc;
					req.Referer = url;
					req.ContentLength = postDataBytes.Length;
					req.ContentType = "application/x-www-form-urlencoded";
					req.Headers.Add("Accept-Encoding", "gzip,deflate");
					req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
	//				req.Headers.Add("Referer", url);
					using (var stream = req.GetRequestStream()) {
						try {
							stream.Write(postDataBytes, 0, postDataBytes.Length);
						} catch (Exception e) {
				       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				       	}
					}
	//					stream.Close();
					
	
					var res = req.GetResponse();
					
					using (var resStream = new System.IO.StreamReader(res.GetResponseStream())) {
						var resStr = resStream.ReadToEnd();
		
						var isSuccess = resStr.IndexOf("フォローしました") > -1;
						var _m = (isPlayOnlyMode) ? "視聴" : "録画";
						form.addLogText((isSuccess ?
						                 "フォローしました。" + _m + "開始までしばらくお待ちください。" : "フォローに失敗しました。"));
						return isSuccess;
					}
	//				resStream.Close();
					
					
	//				Task<HttpResponseMessage> _resTask = http.PostAsync(url, content);
					
	//				_resTask.Wait();
	//				var _res = _resTask.Result;
					
	//				var resTask = _res.Content.ReadAsStringAsync();
	//				resTask.Wait();
	//				var res = resTask.Result;
		//			var a = _res.Headers;
					
		//			if (res.IndexOf("login_status = 'login'") < 0) return null;
					
	//				var cc = handler.CookieContainer;
					
				} catch (Exception e) {
					form.addLogText("フォローに失敗しました。");
					util.debugWriteLine(e.Message+e.StackTrace);
					continue;
//					return false;
				}
			}
			form.addLogText("フォローに失敗しました。");
			util.debugWriteLine("フォロー失敗");
			return false;
		}
		*/
	}
}
