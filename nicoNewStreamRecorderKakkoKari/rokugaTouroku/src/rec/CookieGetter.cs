/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/13
 * Time: 4:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using namaichi.utility;
using rokugaTouroku.gui;
using SunokoLibrary.Application;
using System.Net.Http;
using System.Collections.Generic;

namespace rokugaTouroku.rec
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
		private bool isSub;
		
		public CookieGetter(config.config cfg)
		{
			this.cfg = cfg;
		}
		async public Task<CookieContainer[]> getHtml5RecordCookie(string url, bool isSub) {
			this.isSub = isSub; 
			CookieContainer cc;
			if (!isSub) {
				cc = await getCookieContainer(cfg.get("BrowserNum"),
						cfg.get("issecondlogin"), cfg.get("accountId"), 
						cfg.get("accountPass"), cfg.get("user_session"),
						cfg.get("user_session_secure"), false, 
						url);
				if (cc != null) {
					var c = cc.GetCookies(TargetUrl)["user_session"];
					var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
					if (c != null)
						cfg.set("user_session", c.Value);
					if (secureC != null)
						cfg.set("user_session_secure", secureC.Value);
				}
				
			} else {
				cc = await getCookieContainer(cfg.get("BrowserNum2"),
						cfg.get("issecondlogin2"), cfg.get("accountId2"), 
						cfg.get("accountPass2"), cfg.get("user_session2"),
						cfg.get("user_session_secure2"), false, 
						url);
				if (cc != null) {
					var c = cc.GetCookies(TargetUrl)["user_session2"];
					var secureC = cc.GetCookies(TargetUrl)["user_session_secure2"];
					if (c != null)
						cfg.set("user_session2", c.Value);
					if (secureC != null)
						cfg.set("user_session_secure2", secureC.Value);
				}
			}
			
			var ret = new CookieContainer[]{cc};
			return ret;
		}
		async private Task<CookieContainer> getCookieContainer(
				string browserNum, string isSecondLogin, string accountId,
				string accountPass, string userSession, string userSessionSecure,
				bool isSub, string url) {
			
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
				CookieContainer cc = await getBrowserCookie(isSub).ConfigureAwait(false);
				log += (cc == null) ? "ブラウザからユーザーセッションを取得できませんでした。" : "ブラウザからユーザーセッションを取得しました。";
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
				var mail = accountId;
				var pass = accountPass;
				var accCC = getAccountCookie(mail, pass);
				log += (accCC == null) ? "アカウントログインからユーザーセッションを取得できませんでした。" : "アカウントログインからユーザーセッションを取得しました。";
				if (accCC != null) {
					util.debugWriteLine("account ishtml5login");
					if (isHtml5Login(accCC, url)) {
						util.debugWriteLine("account login ok");
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
						return accCC;
					}
				}
			}
			return null;
		}
		private CookieContainer getUserSessionCC(string us, string uss) {
			//var us = cfg.get("user_session");
			//var uss = cfg.get("user_session_secure");
			if ((us == null || us.Length == 0) &&
			    (uss == null || uss.Length == 0)) return null;
			var cc = new CookieContainer();
			
			var c = new Cookie("user_session", us, "/", ".nicovideo.jp");
			var secureC = new Cookie("user_session_secure", uss, "/", ".nicovideo.jp");
			cc = setUserSession(cc, c, secureC);
 			cc.Add(new Cookie("player_version", "leo", "/", ".nicovideo.jp"));
			return cc;
		}
		async private Task<CookieContainer> getBrowserCookie(bool isSub) {
			var si = SourceInfoSerialize.load(isSub);
			
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

			var cc = new CookieContainer();
			cc.PerDomainCapacity = 200;
			foreach(Cookie _c in result.Cookies) {
				try {
					if (_c.Name == "age_auth" || _c.Name.IndexOf("user_session") > -1) {
						cc.Add(new Cookie(_c.Name, _c.Value, "/", ".nicovideo.jp"));
					}
				} catch (Exception e) {
					util.debugWriteLine("cookie add browser " + _c.ToString() + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			
			var c = cc.GetCookies(TargetUrl)["user_session"];
			var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
			return cc;
		}
		private bool isHtml5Login(CookieContainer cc, string url) {
			try {
				util.debugWriteLine("ishtml5login getpage " + url);
				//pageSource = util.getPageSource(url + "",cc);
				pageSource = util.getPageSourceCurl(url, cc, null);
				util.debugWriteLine("ishtml5login getpage ok");
			} catch (Exception e) {
				util.debugWriteLine("cookiegetter ishtml5login " + e.Message+e.StackTrace);
				pageSource = "";
				log += "ページの取得中にエラーが発生しました。" + e.Message + e.Source + e.TargetSite + e.StackTrace;
				return false;
			}
			
			if (pageSource == null) {
				log += "ページが取得できませんでした。";
				return false;
			}
			var isLogin = !(pageSource.IndexOf("\"login_status\":\"login\"") < 0 &&
			   	pageSource.IndexOf("login_status = 'login'") < 0); 
			util.debugWriteLine("islogin " + isLogin);
			log += (isLogin) ? "ログインに成功しました。" : "ログインに失敗しました";
			if (!isLogin) log += pageSource;
			if (isLogin) {
				id = util.getRegGroup(pageSource, "\"user_id\":(\\d+)");
				if (id == null) id = util.getRegGroup(pageSource, "user_id = (\\d+)");
			}
			return isLogin;
		}
		public CookieContainer getAccountCookie(string mail, string pass) {
			if (mail == null || pass == null) {
				log += ((mail == null) ? "not set mail." : "") + ((pass == null) ? "not set mail." : "");
				return null;
			}
			
			var isNew = true;
			
			string loginUrl;
			Dictionary<string, string> param;
			if (isNew) {
				loginUrl = "https://account.nicovideo.jp/login/redirector?show_button_twitter=1&site=niconico&show_button_facebook=1&sec=header_pc&next_url=/";
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
				var h = new Dictionary<string, string>();
				h.Add("Referer", "https://account.nicovideo.jp/login?site=niconico&next_url=%2F&sec=header_pc&cmnhd_ref=device%3Dpc%26site%3Dniconico%26pos%3Dheader_login%26page%3Dtop");
				h.Add("Content-Type", "application/x-www-form-urlencoded");
				h.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
				h.Add("User-Agent", util.userAgent);
				
				var _d = "mail_tel=" + WebUtility.UrlEncode(param["mail_tel"]) + "&password=" + WebUtility.UrlEncode(param["password"]) + "&auth_id=" + param["auth_id"];
				var d = Encoding.ASCII.GetBytes(_d);
				var cc = new CookieContainer();
				
				util.debugWriteLine(cc.GetCookieHeader(new Uri(loginUrl)));
				
				if (util.isUseCurl(CurlHttpVersion.CURL_HTTP_VERSION_1_1)) {
					var curlR = new Curl().getStr(loginUrl, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "POST", _d, true, true, false);
					if (curlR == null) {
						log += "ログインページに接続できませんでした:Curl";
						return null;
					}
					var m = new Regex("Set-Cookie: (.+?)=(.+?);").Matches(curlR);
					if (m.Count == 0) {
						log += "保存するクッキーがありませんでした:Curl";
						return null;
					}
					Cookie us = null, secureC = null;  
					foreach (Match _m in m) {
						if (_m.Groups[1].Value == "user_session") us = new Cookie(_m.Groups[1].Value, _m.Groups[2].Value);
						if (_m.Groups[1].Value == "user_session_secure") secureC = new Cookie(_m.Groups[1].Value, _m.Groups[2].Value);
					}
					if (us != null) {
						setUserSession(cc, us, secureC);
						return cc;
					}
					
					var locationM = new Regex("Location: (.+)").Matches(curlR);
					if (locationM.Count == 0) {
						log += "ログイン接続の転送先が見つかりませんでした:Curl";
						return null;
					}
					var location = locationM[locationM.Count - 1].Groups[1].Value;
					location = util.getRegGroup(curlR, "Location: (.+)\r");
					if (location == null) {
						log += "not found location." + curlR + ":Curl";
						return null;
					}
					//location = WebUtility.UrlDecode(location);
					
					var setCookie = new Dictionary<string, string>();
					var setCookieM = new Regex("Set-Cookie: (.+?)=(.*?);").Matches(curlR);
					foreach (Match _m in setCookieM) {
						var key = _m.Groups[1].Value;
						if (setCookie.ContainsKey(key)) {
						    	if (_m.Groups[2].Value == "") setCookie.Remove(key);
						    	else if (_m.Groups[2].Value != "") setCookie[key] = _m.Groups[2].Value;
						    }
						else if (_m.Groups[2].Value != "") setCookie.Add(key, _m.Groups[2].Value);
					}
					h["Cookie"] = string.Join("; ", setCookie.Select(x => x.Key + "=" + x.Value).ToArray());
					h.Remove("Content-Type");
					var curlR2 = new Curl().getStr(location, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "GET", null, true, true, false);
					
					var browName = util.getRegGroup(curlR2, "id=\"deviceNameInput\".+?value=\"(.+?)\"");
	                if (browName == null) browName = "Google Chrome (Windows)";
	                var mfaUrl = util.getRegGroup(curlR2, "<form action=\"(.+?)\"");
	                if (mfaUrl == null || mfaUrl.IndexOf("/mfa") == -1) {
	                	var notice = util.getRegGroup(curlR2, "\"notice__text\">(.+?)</p>");
						if (notice != null) log += " notice:" + notice;
	                	else log += "2段階認証のURLを取得できませんでした。:Curl";
						return null;
	                }
	                mfaUrl = "https://account.nicovideo.jp" + mfaUrl;
	                //mfaUrl = WebUtility.UrlDecode(mfaUrl);
	                var sendTo = util.getRegGroup(curlR2, "class=\"userAccount\">(.+?)</span>");
	                if (sendTo == null && util.getRegGroup(curlR2, "(スマートフォンのアプリを使って)") != null) {
	                	sendTo = "app";
	                }
	                var f = new MfaInputForm(sendTo);
	                
	                var dr = f.ShowDialog();
	                if (f.code == null) {
	                	log += "二段階認証のコードが入力されていませんでした:Curl";
	                	return null;
	                }
	                util.debugWriteLine(mfaUrl);
	                h["Referer"] = location;
	                h["Origin"] = "https://account.nicovideo.jp";
	                h["Content-Type"] = "application/x-www-form-urlencoded";
	                _d = "otp=" + f.code + "&loginBtn=%E3%83%AD%E3%82%B0%E3%82%A4%E3%83%B3&device_name=Google+Chrome+%28Windows%29";
	                var curlR3 = new Curl().getStr(mfaUrl, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "POST", _d, true, true, false);
	                if (curlR3 == null) {
	                	log += "二段階認証のコードを正常に送信できませんでした:Curl";
	                	return null;
	                }
	                setCookieM = new Regex("Set-Cookie: (.+?)=(.*?);").Matches(curlR3);
					foreach (Match _m in setCookieM) {
						var key = _m.Groups[1].Value;
						if (setCookie.ContainsKey(key)) {
						    	if (_m.Groups[2].Value == "") setCookie.Remove(key);
						    	else setCookie[key] = _m.Groups[2].Value;
						    }
						else setCookie.Add(key, _m.Groups[2].Value);
					}
	                h["Cookie"] = string.Join("; ", setCookie.Select(x => x.Key + "=" + x.Value).ToArray());
	                var location2 = util.getRegGroup(curlR3, "Location: (.+)\r");
	                
	                var curlR4 = new Curl().getStr(location2, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "GET", null, true, true, false);
	                m = new Regex("Set-Cookie: (.+?)=(.+?);").Matches(curlR4);
	                if (m.Count == 0) {
	                	log += "not set cookie." + curlR4 + ":Curl";
	                	return null;
	                }
					foreach (Match _m in m) {
						if (_m.Groups[1].Value == "user_session") us = new Cookie(_m.Groups[1].Value, _m.Groups[2].Value);
						if (_m.Groups[1].Value == "user_session_secure") secureC = new Cookie(_m.Groups[1].Value, _m.Groups[2].Value);
					}
					if (us != null) {
						setUserSession(cc, us, secureC);
						return cc;
					}
					log += "not found session:Curl";
					return null; 
				}
				
				var r = util.sendRequest(loginUrl, h, d, "POST", cc);
				if (r == null) {
					log += "ログインページに接続できませんでした:default";
					log += getTestPos(loginUrl, h, d, "POST", cc);
					return null;
				}
				
				var _cc = cc.GetCookies(new Uri(loginUrl));
				if (_cc["user_session"] != null) {
					//cc.Add(r.Cookies["user_session"]);
					return cc;
				}
				if (r.ResponseUri == null || !r.ResponseUri.AbsolutePath.StartsWith("/mfa")) {
					log += "ログインに失敗しました。:default";
					try {
						using (var sr = new StreamReader(r.GetResponseStream())) {
							var res = sr.ReadToEnd();
							var notice = util.getRegGroup(res, "\"notice__text\">(.+?)</p>");
							if (notice != null) log += " " + notice;
						}
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					}
					return null;
				}
				using (var sr = new StreamReader(r.GetResponseStream())) {
					var res = sr.ReadToEnd();
					util.debugWriteLine(res);
					
					var browName = util.getRegGroup(res, "id=\"deviceNameInput\".+?value=\"(.+?)\"");
	                if (browName == null) browName = "Google Chrome (Windows)";
	                var mfaUrl = util.getRegGroup(res, "<form action=\"(.+?)\"");
	                if (mfaUrl == null) {
	                	log += "2段階認証のURLを取得できませんでした。:default";
						return null;
	                }
	                mfaUrl = "https://account.nicovideo.jp" + mfaUrl;
	                var sendTo = util.getRegGroup(res, "class=\"userAccount\">(.+?)</span>");
	                if (sendTo == null && util.getRegGroup(res, "(スマートフォンのアプリを使って)") != null) {
	                	sendTo = "app";
	                }
	                var f = new MfaInputForm(sendTo);
	                
	                var dr = f.ShowDialog();
	                if (f.code == null) {
	                	log += "二段階認証のコードが入力されていませんでした:default";
	                	return null;
	                }
	                util.debugWriteLine(mfaUrl);
	                h["Referer"] = r.ResponseUri.OriginalString;
	                h["Origin"] = "https://account.nicovideo.jp";
	                _d = "otp=" + f.code + "&loginBtn=%E3%83%AD%E3%82%B0%E3%82%A4%E3%83%B3&device_name=Google+Chrome+%28Windows%29";
	                d = Encoding.ASCII.GetBytes(_d);
	                var _r = util.sendRequest(mfaUrl, h, d, "POST", cc);
	                if (_r == null) {
	                	log += "二段階認証のコードを正常に送信できませんでした:default";
	                	return null;
	                }
	                using (var _sr = new StreamReader(_r.GetResponseStream())) {
	                	res = _sr.ReadToEnd();
	                	util.debugWriteLine(res);
	                }
	                _cc = cc.GetCookies(new Uri(loginUrl));
					if (_cc["user_session"] != null) {
						return cc;
	                } else {
	                	log += "2段階認証のログインに失敗しました:default";
	                	return null;
	                }
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message+e.StackTrace);
				log += e.Message + e.Source + e.StackTrace;
				return null;
			}
		}
		private CookieContainer setUserSession(CookieContainer cc, 
				Cookie c, Cookie secureC) {
			if (c != null && c.Value != "") {
				cc.Add(new Cookie(c.Name, c.Value, "/", ".nicovideo.jp"));
			}
			if (secureC != null && secureC.Value != "") {
				cc.Add(new Cookie(secureC.Name, secureC.Value, "/", ".nicovideo.jp"));
			}
			return cc;
		}
		string getTestPos(string url, Dictionary<string, string> headers, byte[] content, string method, CookieContainer cc = null) {
			try {
				var req = (HttpWebRequest)WebRequest.Create(url);
				req.Method = method;
				req.Proxy = null;
				req.Headers.Add("Accept-Encoding", "gzip,deflate");
				req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				req.CookieContainer = cc;
				
				if (headers != null) {
					foreach (var h in headers) {
						if (h.Key.ToLower().Replace("-", "") == "contenttype")
							req.ContentType = h.Value;
						else if (h.Key.ToLower().Replace("-", "") == "useragent")
							req.UserAgent = h.Value;
						else if (h.Key.ToLower().Replace("-", "") == "connection")
							req.KeepAlive = h.Value.ToLower().Replace("-", "") == "keepalive";
						else if (h.Key.ToLower().Replace("-", "") == "accept")
							req.Accept = h.Value;
						else if (h.Key.ToLower().Replace("-", "") == "referer")
							req.Referer = h.Value;
						else req.Headers.Add(h.Key, h.Value);
					}
				}
					
				if (content != null) {
					using (var stream = req.GetRequestStream()) {
						try {
							stream.Write(content, 0, content.Length);
						} catch (Exception ee) {
				       		util.debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
				       	}
					}
				}
	//					stream.Close();
				var webreq = (System.Net.HttpWebResponse)req.GetResponse();
				using (var sr = new StreamReader(webreq.GetResponseStream())) {
					return sr.ReadToEnd();
				}
				
				
			} catch (WebException ee) {
				util.debugWriteLine(ee.Data + ee.Message + ee.Source + ee.StackTrace + ee.Status);
				try {
					var webreq = (System.Net.HttpWebResponse)ee.Response;
					using (var sr = new StreamReader(webreq.GetResponseStream())) {
						return sr.ReadToEnd();
					}
					//using (var _rs = ee.Response.GetResponseStream())
					//using (var rs = new StreamReader(_rs)) {
					//	return rs.ReadToEnd();
					//}
				} catch (Exception eee) {
					util.debugWriteLine(eee.Message + eee.Source + eee.StackTrace);
					return null;
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				return null;
			}
		}
	}
	
}
