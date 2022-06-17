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
using System.Threading;

namespace namaichi.rec
{
	/// <summary>
	/// Description of CookieGetter.
	/// </summary>
	public class CookieGetter
	{
		public string pageSource = null;
		public bool isHtml5 = false;
		private config.config cfg;
		public string log = "";
		public string id = null;
		static readonly Uri TargetUrl = new Uri("https://live.nicovideo.jp/");
		static readonly Uri TargetUrl2 = new Uri("https://live2.nicovideo.jp");
		static readonly Uri TargetUrl3 = new Uri("https://com.nicovideo.jp");
		static readonly Uri TargetUrl4 = new Uri("https://watch.live.nicovideo.jp/api/");
		static readonly Uri TargetUrl5 = new Uri("https://www.nicovideo.jp/");
		//private bool isSub;
		private bool isRtmp = false;
		public string reason = null;
		public static bool isLoginCheck = false;
		
		public CookieGetter(config.config cfg)
		{
			this.cfg = cfg;
		}
		async public Task<CookieContainer[]> getHtml5RecordCookie(string url, bool isSub = false) {
			CookieContainer cc;
			
			var num = isSub ? "2" : "";
			cc = await getCookieContainer(cfg.get("BrowserNum" + num),
					cfg.get("issecondlogin" + num), cfg.get("accountId" + num), 
					cfg.get("accountPass" + num), cfg.get("user_session" + num),
					cfg.get("user_session_secure" + num), isSub, 
					url).ConfigureAwait(false);
			if (cc != null) {
				var c = cc.GetCookies(TargetUrl)["user_session"];
				var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
				var age_auth = cc.GetCookies(TargetUrl)["age_auth"];
				
				var l = new List<KeyValuePair<string, string>>();
				if (c != null && cfg.argAi == null)
					l.Add(new KeyValuePair<string, string>("user_session" + num, c.Value));
				if (secureC != null && cfg.argAi == null)
					l.Add(new KeyValuePair<string, string>("user_session_secure" + num, secureC.Value));
				if (age_auth != null)
					l.Add(new KeyValuePair<string, string>("age_auth", age_auth.Value));
				cfg.set(l);
			}
			
			
			var ret = new CookieContainer[]{cc};
			return ret;
		}
		async private Task<CookieContainer> getCookieContainer(
				string browserNum, string isSecondLogin, string accountId,
				string accountPass, string userSession, string userSessionSecure,
				bool isSub, string url) {
			
			if (cfg.argAi != null) {
				if (cfg.argAi.isBrowser && cfg.argAi.si != null) {
					reason = null;
					CookieContainer cc = await getBrowserCookie(isSub, cfg.argAi.si).ConfigureAwait(false);
					log += (cc == null) ? "引数で指定されたブラウザからユーザーセッションを取得できませんでした。ログインに使うブラウザの設定をご確認ください。他のブラウザやアカウントログインを試したり、ブラウザ上で一度ログインし直した後にもう一度ツール側で設定すると上手くいくかもしれません。" : "ブラウザからユーザーセッションを取得しました。";
					if (cc != null) {
						util.debugWriteLine("browser ishtml5login");
						if (isHtml5Login(cc, url)) {
							util.debugWriteLine("browser login ok");
							return cc;
						}
					}
				} else {
					reason = null;
					var mail = cfg.argAi.accountId;
					var pass = cfg.argAi.accountPass;
					var accCC = await getAccountCookie(mail, pass).ConfigureAwait(false);
					log += (accCC == null) ? "引数で指定されたアカウントログインからユーザーセッションを取得できませんでした。" : "アカウントログインからユーザーセッションを取得しました。";
					if (accCC != null) {
						util.debugWriteLine("account ishtml5login");
						if (isHtml5Login(accCC, url)) {
							util.debugWriteLine("account login ok");
							return accCC;
						}
					}
				}
			}
			var userSessionCC = getUserSessionCC(userSession, userSessionSecure);
			log += (userSessionCC == null) ? "前回のユーザーセッションが見つかりませんでした。" : "前回のユーザーセッションが見つかりました。";
			if (userSessionCC != null && true) {
//				util.debugWriteLine(userSessionCC.GetCookieHeader(TargetUrl));
				util.debugWriteLine("usersessioncc ishtml5login");
				if (isHtml5Login(userSessionCC, url)) {
					return userSessionCC;
				}
			}
			
			if (browserNum == "2") {
				reason = null;
				CookieContainer cc = await getBrowserCookie(isSub).ConfigureAwait(false);
				log += (cc == null) ? "ブラウザからユーザーセッションを取得できませんでした。ログインに使うブラウザの設定をご確認ください。他のブラウザやアカウントログインを試したり、ブラウザ上で一度ログインし直した後にもう一度ツール側で設定すると上手くいくかもしれません。" : "ブラウザからユーザーセッションを取得しました。";
				if (cc != null) {
					util.debugWriteLine("browser ishtml5login");
					if (isHtml5Login(cc, url)) {
						util.debugWriteLine("browser login ok");
						return cc;
					}
				}
			}
			
			if (browserNum == "1" || 
			    	isSecondLogin == "true") {
				reason = null;
				var mail = accountId;
				var pass = accountPass;
				var accCC = await getAccountCookie(mail, pass).ConfigureAwait(false);
				log += (accCC == null) ? "アカウントログインからユーザーセッションを取得できませんでした。" : "アカウントログインからユーザーセッションを取得しました。";
				if (accCC != null) {
					util.debugWriteLine("account ishtml5login");
					if (isHtml5Login(accCC, url)) {
						util.debugWriteLine("account login ok");
						return accCC;
					}
				}
			}
			
			if (isPlayable(url)) return new CookieContainer();
			return null;
		}
		private CookieContainer getUserSessionCC(string us, string uss) {
			//var us = cfg.get("user_session");
			//var uss = cfg.get("user_session_secure");
			//if ((us == null || us.Length == 0) &&
			//    (uss == null || uss.Length == 0)) return null;
			if (us == null || us.Length == 0) return null;
			var cc = new CookieContainer();
			
			var c = new Cookie("user_session", us);
			var secureC = new Cookie("user_session_secure", uss);
			var age_auth = new Cookie("age_auth", cfg.get("age_auth"));
			cc = setUserSession(cc, c, secureC, age_auth);
 			cc.Add(TargetUrl, new Cookie("player_version", "leo", "/", ".nicovideo.jp"));
			
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
		async private Task<CookieContainer> getBrowserCookie(bool isSub, CookieSourceInfo si = null) {
			if (si == null) si = SourceInfoSerialize.load(isSub);
			
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
			
			var requireCookies = new List<Cookie>();
			var cc = new CookieContainer();
			cc.PerDomainCapacity = 200;
			foreach(Cookie _c in result.Cookies) {
				try {
					//cc.Add(_c);
					if (_c.Name == "age_auth" || _c.Name.IndexOf("user_session") > -1) {
						requireCookies.Add(new Cookie(_c.Name, _c.Value, "/", ".nicovideo.jp"));
					}
				} catch (Exception e) {
					util.debugWriteLine("cookie add browser " + _c.ToString() + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
//			result.AddTo(cc);
			foreach (var _c in requireCookies) cc.Add(_c);
						
			var c = cc.GetCookies(TargetUrl)["user_session"];
			var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
			
			if (c == null) {
				log += "ブラウザでログインし直すか、別のブラウザを試すか、アカウントログインを試すと上手くいくかもしれません。";
				return null;
			}
			
			//cc = copyUserSession(cc, c, secureC);
			return cc;
			
		}
		private bool isHtml5Login(CookieContainer cc, string url) {
			//cc.Add(new Uri(url), new Cookie("age_auth", "0"));
			//cc = new CookieContainer();
			//cc.SetCookies(new Uri(url), "nicosid=1546686738.281900968; _ga=GA1.2.1892466675.1546686717; nicorepo_filter=myself; _a1_sync=!rld|1554233235889; __gads=ID=bbe8b59a1a1b56cf:T=1561492336:S=ALNI_Ma4kmS8DGEBaEGXwCi23MfjLj42XA; __utma=8292653.1892466675.1546686717.1591843710.1591846656.472; __utmz=8292653.1591846656.472.472.utmcsr=com.nicovideo.jp|utmccn=(referral)|utmcmd=referral|utmcct=/bbs/co13528; _ga_8W314HNSE8=GS1.1.1591655100.48.1.1591655101.0; nicolivehistory=%5B326227658%2C326227874%2C326232036%2C325096106%2C326370706%2C326365094%2C326371606%2C326371601%2C326367676%2C326360477%2C326367014%2C326320571%2C326344349%2C326221118%2C326344412%2C326271502%2C326250313%2C326360917%2C326271409%2C326359986%2C326371303%2C325885062%2C326368355%2C326391017%2C326341505%2C326334584%2C326392793%2C326409873%2C326423760%2C326162806%5D; cto_bundle=0DSTLl9DZTkzY01sQnlXSExUWTRqNnJtaGU4SkNPaU9LNk54czQ2RlBWbEs5SzlpVnBPQTdhZVZ5ODY4UkJJZmI1azBKN3pNbEclMkJOWHh2eGxDQVZyM0w0d1dpekxPcFlia3BoZmpSZyUyRkRwYzRwcUt6eiUyRkZhWUZlc1ZScVpVQ21KVHJ1amMwQ3lDeDdyekNHVEVVUjdhbyUyRjA3d2N3aHhZWnRzSGhpJTJGWjR1MDZEbXVKem9TSERaTFh1czJtRDN4VDBXOSUyRm4; optimizelyEndUserId=oeu1586031666784r0.061993650074855355; optimizelySegments=%7B%223176911475%22%3A%22referral%22%2C%223188621092%22%3A%22ff%22%2C%223192211201%22%3A%22false%22%2C%223217420529%22%3A%22none%22%7D; optimizelyBuckets=%7B%7D; _gid=GA1.2.1042875258.1590148254; user_session=user_session_94891892_a185c5a223715a06175322b8fbba9e7da493ed42b41eead10c10087711e4cc6a; user_session_secure=OTQ4OTE4OTI6TGNjQ2JOU1dxWWNhRDZ4TElrTFNmcXlNbm8tU2pWd2pPbDExSnpqUHNPbg; _td=f8e7235a-76a7-4e6f-8191-e75ca65aaff5; __utmc=8292653; pt_s_4ef1ca6b=1591584731613; pt_s_57cf6e43=1591761797274; _dd_l=1; _dd=4c25775e-2e9e-498c-becc-cf1020e55e26; _gali=root".Replace(";", ","));
			var ccc = cc.GetCookieHeader(new Uri(url));
			for (var i = 0; i < 3; i++) {
				//var headers = new WebHeaderCollection();
				try {
					util.debugWriteLine("ishtml5login getpage " + url);
					var _url = (isRtmp) ? ("https://live.nicovideo.jp/api/getplayerstatus/" + util.getRegGroup(url, "(lv\\d+)")) : url;
					pageSource = util.getPageSource(_url, cc);
					
					util.debugWriteLine("ishtml5login getpage ok");

				} catch (Exception e) {
					util.debugWriteLine("cookiegetter ishtml5login " + e.Message+e.StackTrace);
					pageSource = "";
					var msg = "ページの取得中にエラーが発生しました。" + e.Message + e.Source + e.TargetSite + e.StackTrace;
					if (!log.EndsWith(msg)) log += msg;
					Thread.Sleep(3000);
					continue;
				}
	//			isHtml5 = (headers.Get("Location") == null) ? false : true;
				if (pageSource == null) {
					util.debugWriteLine("not get page");
					Thread.Sleep(3000);
					pageSource = util.getPageSource(url, cc, null, false, 5, null, true);
					
					var msg = "ページが取得できませんでした。" + url;
					msg += util.getRegGroup(pageSource, "<title>(.+?)</title>");
					if (!log.EndsWith(msg)) log += msg;
					
					Thread.Sleep(3000);
					continue;
				}
				if (pageSource.IndexOf("<p class=\"textTitle\">年齢認証</p>") > -1) {
					log += "この放送は年齢認証が必要です。ブラウザで年齢認証をしてください。";
					reason = "age";
					return false;
				}
				if (pageSource.IndexOf("content=\"https://jk.nicovideo.jp/\"") != -1) {
				    return true;
			    }
				if (pageSource.IndexOf("\"login_status\"") == -1 &&
				     	pageSource.IndexOf("login_status") == -1) {
					var msg = "放送ページを正常に取得できませんでした。";
					if (!log.EndsWith(msg)) log += msg;
					util.debugWriteLine(pageSource);
				    Thread.Sleep(5000);
				    continue;
			    }
	
				//"login_status":"not_login"
				if (pageSource.IndexOf("\"login_status\":\"not_login\"") != -1 ||
				    	pageSource.IndexOf("login_status = 'not_login'") != -1) {
					log += "ログインしていませんでした。";
					reason = "not_login";
					return false;
				}
				
				var isLogin = !(pageSource.IndexOf("\"login_status\":\"login\"") < 0 &&
				   	pageSource.IndexOf("login_status = 'login'") < 0);
				if (isRtmp) isLogin = pageSource.IndexOf("<code>notlogin</code>") == -1;
				util.debugWriteLine("islogin " + isLogin);
				log += (isLogin) ? "ログインに成功しました。" : "ログインが確認できませんでした。";
	//			if (!isLogin) log += pageSource;
				if (isLogin) {
	//				id = (isRtmp) ? util.getRegGroup(pageSource, "<user_id>(\\d+)</user_id>")
	//					: util.getRegGroup(pageSource, "\"user_id\":(\\d+)");
					id = util.getRegGroup(pageSource, "\"user_id\":(\\d+)");
					if (id == null) id = util.getRegGroup(pageSource, "user_id = (\\d+)");
					util.debugWriteLine("id " + id);
				} else {
					util.debugWriteLine("not login " + pageSource.Substring(0, 1000));
					util.debugWriteLine(cc.GetCookieHeader(new Uri(url)));
				}
				return isLogin;
			}
			if (isLoginCheck)
				util.loginCheck(cc, url, log);
			return false;
		}
		async public Task<CookieContainer> getAccountCookie(string mail, string pass) {
			
			if (mail == null || pass == null) return null;
			
			var isNew = true;
			
			string loginUrl;
			Dictionary<string, string> param;
			if (isNew) {
				loginUrl = "https://account.nicovideo.jp/login/redirector";
				param = new Dictionary<string, string> {
					{"mail_tel", mail}, {"password", pass}, {"auth_id", "15263781"}//dummy
				};
			} else {
				loginUrl = "https://secure.nicovideo.jp/secure/login?site=nicolive";
				param = new Dictionary<string, string> {
					{"mail", mail}, {"password", pass}
				};
			}
			
			try {
				var handler = new System.Net.Http.HttpClientHandler();
				if (util.httpProxy != null) {
					handler.UseProxy = true;
					handler.Proxy = util.httpProxy;
				}
				handler.UseCookies = true;
				var http = new System.Net.Http.HttpClient(handler);
				var content = new System.Net.Http.FormUrlEncodedContent(param);
				
				var _res = await http.PostAsync(loginUrl, content).ConfigureAwait(false);
				var res = await _res.Content.ReadAsStringAsync().ConfigureAwait(false);

				var cc = handler.CookieContainer;
				var cookies = cc.GetCookies(TargetUrl);
				var c = cookies["user_session"];
				var secureC = cookies["user_session_secure"];
				//cc = copyUserSession(cc, c, secureC);
				log += (c == null) ? "ユーザーセッションが見つかりませんでした。" : "ユーザーセッションが見つかりました。";
				log += (secureC == null) ? "secureユーザーセッションが見つかりませんでした。" : "secureユーザーセッションが見つかりました。";
				if (c == null && secureC == null) return null;
				
				return cc;
			
			} catch (Exception e) {
				util.debugWriteLine(e.Message+e.StackTrace);
				return null;
			}
		}
		private CookieContainer setUserSession(CookieContainer cc, 
				Cookie c, Cookie secureC, Cookie age_auth = null) {
			if (c != null && c.Value != "") {
				cc.Add(new Cookie(c.Name, c.Value, "/", ".nicovideo.jp"));
			}
			if (secureC != null && secureC.Value != "") {
				cc.Add(new Cookie(secureC.Name, secureC.Value, "/", ".nicovideo.jp"));
			}
			if (age_auth != null && age_auth.Value != "") {
				cc.Add(new Cookie(age_auth.Name, age_auth.Value, "/", ".nicovideo.jp"));
				//var ageAuth = "0";
				//"https://live2.nicovideo.jp/watch/lv320739154
			}
			return cc;
		}
		private bool isPlayable(string url) {
			try {
				util.debugWriteLine("isPlayable " + url);
				var res = util.getPageSource(url);
				if (res != null && res.IndexOf("wss://a.") > -1) {
					id = "0";
					pageSource = res;
					return true;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			return false;
		}
	}
}
