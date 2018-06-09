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

namespace namaichi.rec
{
	/// <summary>
	/// Description of FollowCommunity.
	/// </summary>
	public class FollowCommunity
	{
		public FollowCommunity()
		{
		}
		public bool followCommunity(string res, CookieContainer cc) {
			var comId = util.getRegGroup(res, "Nicolive_JS_Conf\\.Recommend = \\{type\\: 'community', community_id\\: '(co\\d+)'\\};");
			if (comId == null) return false;
			
			var isJoinedTask = join(comId, cc);
//			isJoinedTask.Wait();
			return isJoinedTask;
//			return false;
		}
		private bool join(string comId, CookieContainer cc) {
			var url = "https://com.nicovideo.jp/motion/" + comId;
			
			var headers = new WebHeaderCollection();
			try {
				var isJidouShounin = util.getPageSource(url, ref headers, cc).IndexOf("自動承認されます") > -1;
				if (!isJidouShounin) return false;
			} catch (Exception e) {
				return false;
			}
			
			
			try {
				var handler = new System.Net.Http.HttpClientHandler();
				handler.UseCookies = true;
				handler.CookieContainer = cc;
				handler.Proxy = null;
				
				
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
				req.Proxy = null;
				req.CookieContainer = cc;
				req.Referer = url;
				req.ContentLength = postDataBytes.Length;
				req.ContentType = "application/x-www-form-urlencoded";
//				req.Headers.Add("Referer", url);
				var stream = req.GetRequestStream();
				stream.Write(postDataBytes, 0, postDataBytes.Length);
				stream.Close();
				
				var res = req.GetResponse();
				var resStream = new System.IO.StreamReader(res.GetResponseStream());
				var resStr = resStream.ReadToEnd();
				resStream.Close();
				
				
//				Task<HttpResponseMessage> _resTask = http.PostAsync(url, content);
				
//				_resTask.Wait();
//				var _res = _resTask.Result;
				
//				var resTask = _res.Content.ReadAsStringAsync();
//				resTask.Wait();
//				var res = resTask.Result;
	//			var a = _res.Headers;
				
	//			if (res.IndexOf("login_status = 'login'") < 0) return null;
				
//				var cc = handler.CookieContainer;
				return resStr.IndexOf("フォローしました") > -1;
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(e.Message+e.StackTrace);
				return false;
			}
			
		}
	}
}
