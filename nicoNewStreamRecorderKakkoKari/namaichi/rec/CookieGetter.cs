/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/13
 * Time: 4:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using SunokoLibrary.Application;
using System.Net.Http;
using System.Collections.Generic;

namespace namaichi.rec
{
	/// <summary>
	/// Description of CookieGetter.
	/// </summary>
	public class CookieGetter
	{
		private CookieContainer cc;
		public string pageSource = null;
		public bool isHtml5 = false;
		private config.config cfg;
		static readonly Uri TargetUrl = new Uri("http://live.nicovideo.jp/");
		static readonly Uri TargetUrl2 = new Uri("http://live2.nicovideo.jp");
		static readonly Uri TargetUrl3 = new Uri("https://com.nicovideo.jp");
		
		public CookieGetter(config.config cfg)
		{
			this.cfg = cfg;
		}
<<<<<<< HEAD
		async public Task<CookieContainer[]> getHtml5RecordCookie(string url) {
			string us;
			string uss;
			var cc0 = await getCookieContainer(cfg.get("BrowserNum"),
					cfg.get("issecondlogin"), cfg.get("accountId"), 
					cfg.get("accountPass"), cfg.get("user_session"),
					cfg.get("user_session_secure"), false, 
					url);
			if (cc0 != null) {
				var c = cc0.GetCookies(TargetUrl)["user_session"];
				var secureC = cc0.GetCookies(TargetUrl)["user_session_secure"];
				if (c != null)
					cfg.set("user_session", c.Value);
				if (secureC != null)
					cfg.set("user_session_secure", secureC.Value);
			}
			var ret = new CookieContainer[]{cc0};
			return ret;
		}
		async private Task<CookieContainer> getCookieContainer(
				string browserNum, string isSecondLogin, string accountId,
				string accountPass, string userSession, string userSessionSecure,
				bool isSub, string url) {
			
			var userSessionCC = getUserSessionCC(userSession, userSessionSecure);
=======
		async public Task<CookieContainer> getHtml5RecordCookie(string url) {
			var userSessionCC = getUserSessionCC();
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			if (userSessionCC != null && true) {
//				util.debugWriteLine(userSessionCC.GetCookieHeader(TargetUrl));
				util.debugWriteLine("usersessioncc ishtml5login");
				if (isHtml5Login(userSessionCC, url)) {
<<<<<<< HEAD
					/*
					var c = userSessionCC.GetCookies(TargetUrl)["user_session"];
					var secureC = userSessionCC.GetCookies(TargetUrl)["user_session_secure"];
					if (c != null)
						//cfg.set("user_session", c.Value);
						us = c.Value;
					if (secureC != null)
						//cfg.set("user_session_secure", secureC.Value);
						uss = secureC.Value;
					*/
=======
					
					var c = userSessionCC.GetCookies(TargetUrl)["user_session"];
					var secureC = userSessionCC.GetCookies(TargetUrl)["user_session_secure"];
					if (c != null)
						cfg.set("user_session", c.Value);
					if (secureC != null)
						cfg.set("user_session_secure", secureC.Value);
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
					return userSessionCC;
				}
			}
			
<<<<<<< HEAD
			if (browserNum == "2") {
				CookieContainer cc = await getBrowserCookie(isSub).ConfigureAwait(false);
=======
			if (cfg.get("BrowserNum") == "2") {
				CookieContainer cc = await getBrowserCookie().ConfigureAwait(false);
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
				
				if (cc != null) {
					util.debugWriteLine("browser ishtml5login");
					if (isHtml5Login(cc, url)) {
//						util.debugWriteLine("browser 1 " + cc.GetCookieHeader(TargetUrl));
//						util.debugWriteLine("browser 2 " + cc.GetCookieHeader(new Uri("http://live2.nicovideo.jp")));
						util.debugWriteLine("browser login ok");
<<<<<<< HEAD
						/*
						var c = cc.GetCookies(TargetUrl)["user_session"];
						var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
						if (c != null)
							//cfg.set("user_session", c.Value);
							us = c.Value;
						if (secureC != null)
							//cfg.set("user_session_secure", secureC.Value);
							uss = secureC.Value;
						*/
=======
						var c = cc.GetCookies(TargetUrl)["user_session"];
						var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
						if (c != null)
							cfg.set("user_session", c.Value);
						if (secureC != null)
							cfg.set("user_session_secure", secureC.Value);
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
						return cc;
					}
					
				}
			}
			
<<<<<<< HEAD
			if (browserNum == "1" || 
			    isSecondLogin == "true") {
				var mail = accountId;
				var pass = accountPass;
=======
			if (cfg.get("BrowserNum") == "1" || 
			    cfg.get("issecondlogin") == "true") {
				var mail = cfg.get("accountId");
				var pass = cfg.get("accountPass");
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
				var accCC = await getAccountCookie(mail, pass).ConfigureAwait(false);
				if (accCC != null) {
					util.debugWriteLine("account ishtml5login");
					if (isHtml5Login(accCC, url)) {
						util.debugWriteLine("account login ok");
<<<<<<< HEAD
						/*
						var c = accCC.GetCookies(TargetUrl)["user_session"];
						var secureC = accCC.GetCookies(TargetUrl)["user_session_secure"];
						if (c != null)
							//cfg.set("user_session", c.Value);
							us = c.Value;
						if (secureC != null)
							//cfg.set("user_session_secure", secureC.Value);
							uss = secureC.Value;
						*/
=======
						var c = accCC.GetCookies(TargetUrl)["user_session"];
						var secureC = accCC.GetCookies(TargetUrl)["user_session_secure"];
						if (c != null)
							cfg.set("user_session", c.Value);
						if (secureC != null)
							cfg.set("user_session_secure", secureC.Value);
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
						return accCC;
					}
				}
			}
			return null;
		}
<<<<<<< HEAD
		private CookieContainer getUserSessionCC(string us, string uss) {
			//var us = cfg.get("user_session");
			//var uss = cfg.get("user_session_secure");
=======
		private CookieContainer getUserSessionCC() {
			var us = cfg.get("user_session");
			var uss = cfg.get("user_session_secure");
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			if ((us == null || us.Length == 0) &&
			    (uss == null || uss.Length == 0)) return null;
			var cc = new CookieContainer();
			
			var c = new Cookie("user_session", us);
			var secureC = new Cookie("user_session_secure", uss);
			cc = copyUserSession(cc, c, secureC);
 			cc.Add(TargetUrl, new Cookie("player_version", "leo"));

			
			//test
//			cc.Add(TargetUrl, new Cookie("nicosid", "1527623077.1259703149"));
//			cc.Add(TargetUrl, new Cookie("_td", "9278c72a-9d4e-4b77-ac40-73f972913d26"));
//			cc.Add(TargetUrl, new Cookie("_gid", "GA1.2.266519775.1527623073"));
//			cc.Add(TargetUrl, new Cookie("_ga", "GA1.2.1892636543.1527623073"));
//			cc.SetCookies(TargetUrl,"optimizelyBuckets=%7B%7D; optimizelySegments=%7B%223152721399%22%3A%22search%22%2C%223155720808%22%3A%22gc%22%2C%223199620088%22%3A%22false%22%2C%223214930722%22%3A%22false%22%2C%223218750517%22%3A%22referral%22%2C%223219110468%22%3A%22none%22%2C%223233940089%22%3A%22gc%22%2C%223235780522%22%3A%22none%22%2C%225140350011%22%3A%22%25E3%2583%25AD%25E3%2582%25B0%25E3%2582%25A4%25E3%2583%25B3%25E6%25B8%2588%22%2C%225130920861%22%3A%22%25E4%25B8%2580%25E8%2588%25AC%25E4%25BC%259A%25E5%2593%25A1%22%2C%225137970544%22%3A%22216pt%25E6%259C%25AA%25E6%25BA%2580%22%2C%229019961413%22%3A%22%25E9%259D%259E%25E5%25AF%25BE%25E8%25B1%25A1%22%7D; nicorepo_filter=all;  optimizelyEndUserId=oeu1527671506390r0.4517391591303288; " +
//			cc.Add(c);
//			cc.Add(TargetUrl, c);
			return cc;
		}
<<<<<<< HEAD
		async private Task<CookieContainer> getBrowserCookie(bool isSub) {
			var si = SourceInfoSerialize.load(isSub);
=======
		async private Task<CookieContainer> getBrowserCookie() {
			var si = SourceInfoSerialize.load();
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			
//			var importer = await SunokoLibrary.Application.CookieGetters.Default.GetInstanceAsync(si, false);
			ICookieImporter importer = await SunokoLibrary.Application.CookieGetters.Default.GetInstanceAsync(si, false).ConfigureAwait(false);
//			var importers = new SunokoLibrary.Application.CookieGetters(true, null);
//			var importera = (await SunokoLibrary.Application.CookieGetters.Browsers.IEProtected.GetCookiesAsync(TargetUrl));
//			foreach (var rr in importer.Cookies)
//				util.debugWriteLine(rr);
			//importer = await importers.GetInstanceAsync(si, true);
			if (importer == null) return null;

			CookieImportResult result = await importer.GetCookiesAsync(TargetUrl).ConfigureAwait(false);
			if (result.Status != CookieImportState.Success) return null;
			
			//if (result.Cookies["user_session"] == null) return null;
			//var cookie = result.Cookies["user_session"].Value;

			//util.debugWriteLine("usersession " + cookie);
			
			var cc = new CookieContainer();
			foreach(Cookie _c in result.Cookies) {
				try {
					cc.Add(_c);
				} catch (Exception e) {
					util.debugWriteLine("cookie add browser " + _c.ToString() + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
//			result.AddTo(cc);
			
			var c = cc.GetCookies(TargetUrl)["user_session"];
			var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
			cc = copyUserSession(cc, c, secureC);
			
			
			return cc;
			
		}
		private bool isHtml5Login(CookieContainer cc, string url) {
			var headers = new WebHeaderCollection();
			try {
				util.debugWriteLine("ishtml5login getpage " + url);
				pageSource = util.getPageSource(url + "", ref headers, cc);
				util.debugWriteLine("ishtml5login getpage ok");
			} catch (Exception e) {
				util.debugWriteLine("cookiegetter ishtml5login " + e.Message+e.StackTrace);
				pageSource = "";
				return false;
			}
//			isHtml5 = (headers.Get("Location") == null) ? false : true;
<<<<<<< HEAD
			if (pageSource == null) return false;
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			var isLogin = !(pageSource.IndexOf("\"login_status\":\"login\"") < 0 &&
			   	pageSource.IndexOf("login_status = 'login'") < 0); 
			util.debugWriteLine("islogin " + isLogin);
			return isLogin;
		}
		async public Task<CookieContainer> getAccountCookie(string mail, string pass) {
			
			if (mail == null || pass == null) return null;
			
			var loginUrl = "https://secure.nicovideo.jp/secure/login?site=nicolive";
<<<<<<< HEAD
//			var param = "mail=" + mail + "&password=" + pass;
=======
			var param = "mail=" + mail + "&password=" + pass;
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
			
			try {
				var handler = new System.Net.Http.HttpClientHandler();
				handler.UseCookies = true;
				var http = new System.Net.Http.HttpClient(handler);
				var content = new System.Net.Http.FormUrlEncodedContent(new Dictionary<string, string>
				{
					{"mail", mail}, {"password", pass}
				});
				
				var _res = await http.PostAsync(loginUrl, content);
				var res = await _res.Content.ReadAsStringAsync();
	//			var a = _res.Headers;
				
	//			if (res.IndexOf("login_status = 'login'") < 0) return null;
				
				cc = handler.CookieContainer;
				
//				return cc;
			} catch (Exception e) {
				util.debugWriteLine(e.Message+e.StackTrace);
				return null;
			}
			
			
			var c = cc.GetCookies(TargetUrl)["user_session"];
			var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
			cc = copyUserSession(cc, c, secureC);
			/*
			var encoder = System.Text.Encoding.GetEncoding("UTF=8");
			var sr = new System.IO.StreamReader(resStream, encoder);
			var xml = sr.ReadToEnd();
			sr.Close();
			resStream.Close();
			
			if (xml.IndexOf("not login") != -1) return null;
			*/
			return cc;
				
		}
		private CookieContainer copyUserSession(CookieContainer cc, 
				Cookie c, Cookie secureC) {
			if (c != null && c.Value != "") {
				cc.Add(TargetUrl, new Cookie(c.Name, c.Value));
				cc.Add(TargetUrl2, new Cookie(c.Name, c.Value));
				cc.Add(TargetUrl3, new Cookie(c.Name, c.Value));
			}
			if (secureC != null && secureC.Value != "") {
				cc.Add(TargetUrl, new Cookie(secureC.Name, secureC.Value));
				cc.Add(TargetUrl2, new Cookie(secureC.Name, secureC.Value));
				cc.Add(TargetUrl3, new Cookie(secureC.Name, secureC.Value));
			}
			return cc;
		}
	}
	
}
