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
using namaichi.gui;
using namaichi.utility;
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
			
			RecordLogInfo.loginType = browserNum == "2" ? "ブラウザログイン" : "アカウントログイン";
			if (cfg.argAi != null) {
				RecordLogInfo.loginLog += "ログイン引数が見つかりました。";
				if (cfg.argAi.isBrowser && cfg.argAi.si != null) {
					RecordLogInfo.loginLog += "引数によりブラウザログインを試行します。";
					reason = null;
					CookieContainer cc = await getBrowserCookie(isSub, cfg.argAi.si).ConfigureAwait(false);
					log += (cc == null) ? "引数で指定されたブラウザからユーザーセッションを取得できませんでした。ログインに使うブラウザの設定をご確認ください。他のブラウザやアカウントログインを試したり、ブラウザ上で一度ログインし直した後にもう一度ツール側で設定すると上手くいくかもしれません。" : "ブラウザからユーザーセッションを取得しました。";
					if (cc != null) {
						util.debugWriteLine("browser ishtml5login");
						if (isHtml5Login(cc, url)) {
							util.debugWriteLine("browser login ok");
							RecordLogInfo.loginLog += "ブラウザログイン引数からログインを確認しました。";
							return cc;
						} else RecordLogInfo.loginLog += "ブラウザログイン引数からログインを確認できませんでした。";
					}
				} else {
					RecordLogInfo.loginLog += "引数によりアカウントログインを試行します。";
					reason = null;
					var mail = cfg.argAi.accountId;
					var pass = cfg.argAi.accountPass;
					var accCC = getAccountCookie(mail, pass);
					log += (accCC == null) ? "引数で指定されたアカウントログインからユーザーセッションを取得できませんでした。" : "アカウントログインからユーザーセッションを取得しました。";
					if (accCC != null) {
						util.debugWriteLine("account ishtml5login");
						if (isHtml5Login(accCC, url)) {
							util.debugWriteLine("account login ok");
							RecordLogInfo.loginLog += "アカウントログイン引数からログインを確認しました。";
							return accCC;
						} else RecordLogInfo.loginLog += "アカウントログイン引数からログインを確認できませんでした。";
					}
				}
			}
			var userSessionCC = getUserSessionCC(userSession, userSessionSecure);
			log += (userSessionCC == null) ? "前回のユーザーセッションが見つかりませんでした。" : "前回のユーザーセッションが見つかりました。";
			if (userSessionCC != null && true) {
				RecordLogInfo.loginLog += "前回のユーザーセッションからログインを試行します。";
				RecordLogInfo.foundLastUs = "yes";
//				util.debugWriteLine(userSessionCC.GetCookieHeader(TargetUrl));
				util.debugWriteLine("usersessioncc ishtml5login");
				if (isHtml5Login(userSessionCC, url)) {
					RecordLogInfo.loginLog += "前回のユーザーセッションからログインを確認しました。";
					return userSessionCC;
				}　else RecordLogInfo.loginLog += "前回のユーザーセッションからログインを確認できませんでした。";
			} else RecordLogInfo.foundLastUs = "no";
			
			if (browserNum == "2") {
				RecordLogInfo.loginLog += "ブラウザからログインを試行します。";
				reason = null;
				CookieContainer cc = await getBrowserCookie(isSub).ConfigureAwait(false);
				log += (cc == null) ? "ブラウザからユーザーセッションを取得できませんでした。ログインに使うブラウザの設定をご確認ください。他のブラウザやアカウントログインを試したり、ブラウザ上で一度ログインし直した後にもう一度ツール側で設定すると上手くいくかもしれません。" : "ブラウザからユーザーセッションを取得しました。";
				if (cc != null) {
					RecordLogInfo.loginLog += "ブラウザからクッキーを取得しました。";
					util.debugWriteLine("browser ishtml5login");
					if (isHtml5Login(cc, url)) {
						RecordLogInfo.loginLog += "ブラウザからログインを確認しました。";
						util.debugWriteLine("browser login ok");
						return cc;
					} else RecordLogInfo.loginLog += "ブラウザからログインを確認できませんでした。";
				} else RecordLogInfo.loginLog += "ブラウザからクッキーを取得できませんでした。";
			}
			
			if (browserNum == "1" || 
			    	isSecondLogin == "true") {
				RecordLogInfo.loginLog += "アカウントログインからログインを試行します。";
				reason = null;
				var mail = accountId;
				var pass = accountPass;
				var accCC = getAccountCookie(mail, pass);
				log += (accCC == null) ? "アカウントログインからユーザーセッションを取得できませんでした。" : "アカウントログインからユーザーセッションを取得しました。";
				if (accCC != null) {
					util.debugWriteLine("account ishtml5login");
					RecordLogInfo.loginLog += "アカウントログインからクッキーを取得しました。";
					if (isHtml5Login(accCC, url)) {
						util.debugWriteLine("account login ok");
						RecordLogInfo.loginLog += "アカウントログインからログインを確認しました。";
						return accCC;
					} else RecordLogInfo.loginLog += "アカウントログインからログインを確認できませんでした。";
				} else RecordLogInfo.loginLog += "アカウントログインからクッキーを取得できませんでした。";
			}
			
			if (isWatchPage(url)) return new CookieContainer();
			return null;
		}
		private CookieContainer getUserSessionCC(string us, string uss) {
			if (us == null || us.Length == 0) return null;
			var cc = new CookieContainer();
			
			var c = new Cookie("user_session", us);
			var secureC = new Cookie("user_session_secure", uss);
			var age_auth = new Cookie("age_auth", cfg.get("age_auth"));
			cc = setUserSession(cc, c, secureC, age_auth);
 			cc.Add(TargetUrl, new Cookie("player_version", "leo", "/", ".nicovideo.jp"));
			return cc;
		}
		async private Task<CookieContainer> getBrowserCookie(bool isSub, CookieSourceInfo si = null) {
			if (si == null) si = SourceInfoSerialize.load(isSub);
			
			ICookieImporter importer = await SunokoLibrary.Application.CookieGetters.Default.GetInstanceAsync(si, false).ConfigureAwait(false);
//			var importers = new SunokoLibrary.Application.CookieGetters(true, null);
//			var importera = (await SunokoLibrary.Application.CookieGetters.Browsers.IEProtected.GetCookiesAsync(TargetUrl));
//			foreach (var rr in importer.Cookies)
//				util.debugWriteLine(rr);
			//importer = await importers.GetInstanceAsync(si, true);
			if (importer == null) return null;

			CookieImportResult result = await importer.GetCookiesAsync(TargetUrl).ConfigureAwait(false);
			if (result.Status != CookieImportState.Success) return null;
			
			var requireCookies = new List<Cookie>();
			var cc = new CookieContainer();
			cc.PerDomainCapacity = 200;
			foreach(Cookie _c in result.Cookies) {
				try {
					if (_c.Name == "age_auth" || _c.Name.IndexOf("user_session") > -1) {
						requireCookies.Add(new Cookie(_c.Name, _c.Value, "/", ".nicovideo.jp"));
					}
				} catch (Exception e) {
					util.debugWriteLine("cookie add browser " + _c.ToString() + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			foreach (var _c in requireCookies) cc.Add(_c);
						
			var c = cc.GetCookies(TargetUrl)["user_session"];
			var secureC = cc.GetCookies(TargetUrl)["user_session_secure"];
			
			if (c == null) {
				log += "ブラウザでログインし直すか、別のブラウザを試すか、アカウントログインを試すと上手くいくかもしれません。";
				return null;
			}
			return cc;
			
		}
		private bool isHtml5Login(CookieContainer cc, string url) {
			var ccc = cc.GetCookieHeader(new Uri(url));
			for (var i = 0; i < 3; i++) {
				try {
					util.debugWriteLine("ishtml5login getpage " + url);
					var _url = (isRtmp) ? ("https://live.nicovideo.jp/api/getplayerstatus/" + util.getRegGroup(url, "(lv\\d+)")) : url;
					//pageSource = util.getPageSource(_url, cc);
					var h = util.getHeader(cc, null, _url);
					pageSource = new Curl().getStr(_url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false, true, true);
					
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
					//pageSource = util.getPageSource(url, cc, null, false, 5, null, true);
					var h = util.getHeader(cc, null, url);
					pageSource = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false, true, true);
					
					var msg = "ページが取得できませんでした。" + url;
					if (pageSource != null)
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
					var curlR = new Curl().getStr(loginUrl, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", _d, true, true, false);
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
						setUserSession(cc, us, secureC, null);
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
						log += "not found location." + curlR;
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
						setUserSession(cc, us, secureC, null);
						return cc;
					}
					log += "not found session:Curl";
					return null; 
				}
				
				var r = util.sendRequest(loginUrl, h, d, "POST", cc);
				if (r == null) {
					log += "ログインページに接続できませんでした:default";
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
		private bool isWatchPage(string url) {
			try {
				util.debugWriteLine("isPlayable " + url);
				//var res = util.getPageSource(url);
				var h = util.getHeader(null, null, null);
				var res = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false, true, true);
				if (res != null && (res.IndexOf("wss://a.") > -1 || util.getPageType(res) == 2)) {
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