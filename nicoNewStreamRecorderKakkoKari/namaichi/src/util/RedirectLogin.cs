/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2022/05/20
 * Time: 18:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using namaichi.info;
using namaichi.rec;
using namaichi.utility;
using Newtonsoft.Json;

namespace namaichi.utility
{
	/// <summary>
	/// Description of RedirectLogin.
	/// </summary>
	public class RedirectLogin
	{
		public CookieContainer cc = new CookieContainer();
		Curl curl = new Curl();
		ChannelPlusRecorder cpr = null;
		StreamInfo si = null;
		
		string auth = null;
		string chPlusCookie = null;
		DateTime lastTimeGotAuth = DateTime.MinValue;
		
		public RedirectLogin(ChannelPlusRecorder cpr, StreamInfo si)
		{
			this.cpr = cpr;
			this.si = si;
		}
		public string getAuth() {
			//if (auth == null) return null;;
			
			if (auth == null) {
				//init
				var loginRes = init();
				//0-login 1-ok 2-no auth
				if (loginRes == 0) {
					auth = loginAuth();
					if (auth == null) auth = "";
					return auth;
				}
				else if (loginRes == 1) {
					return auth;
				}
				else if (loginRes == 2) {
					auth = "";
					cpr.rm.cfg.set("chPlus_access_token", "");
					cpr.rm.cfg.set("chPlus_refreshCookie", "");
					return null;
				}
				return null;
			} else {
				if (auth == "") return null;
				if (DateTime.Now - lastTimeGotAuth > TimeSpan.FromMinutes(5) && string.IsNullOrEmpty(auth)) {
					if (!refreshAuth()) {
						auth = "";
						return null;
					}
				}
				lastTimeGotAuth = DateTime.Now;
				return auth;
			}
		}
		public string loginAuth() {
			//FCS000xx
			var channelNum = si.recFolderFileInfo[4];
			var channelName = si.recFolderFileInfo[5];
			try {
				var authUrl = "https://auth.sheeta.com/auth/realms/FCS00001/protocol/openid-connect/auth?client_id=" + channelNum + "&response_type=code&scope=openid&kc_idp_hint=niconico&redirect_uri=https://nicochannel.jp/" + channelName + "/login";
				var r = curl.getStr(authUrl, util.getHeader(), CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", "", true);
				util.debugWriteLine("RedirectLogin auth r0 " + r);
				if (r == null) return null;
				setHeaderToCookie(".sheeta.com", r, cc);
				var location = util.getRegGroup(r, "location: (.+)\r");
				if (location == null) return null;
				util.debugWriteLine("RedirectLgin authUrl location0 " + location);
				/*
				location: https://auth.sheeta.com/auth/realms/FCS00001/broker/niconico/login?session_code=OU4UwAJieZ22xEmvdlY0qurLuBtChfs8cpLVu7gtbJg&client_id=FCS00124&tab_id=a6eges1Vz3Y
				set-cookie: AUTH_SESSION_ID=
				set-cookie: AUTH_SESSION_ID_LEGACY=
				set-cookie: KC_RESTART=
				*/
				
				//https://auth.sheeta.com/auth/realms/FCS00001/broker/niconico/login?session_code=OU4UwAJieZ22xEmvdlY0qurLuBtChfs8cpLVu7gtbJg&client_id=FCS00124&tab_id=a6eges1Vz3Y
				var h = util.getHeader();
				h.Add("Cookie", cc.GetCookieHeader(new Uri(location)));
				r = curl.getStr(location, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", "", true);
				util.debugWriteLine("RedirectLogin auth r1 " + r);
				if (r == null) return null;
				location = util.getRegGroup(r, "location: (.+)\r");
				//location = location.Replace("%3A", ":").Replace("%2F", "/");
				if (location == null) return null;
				util.debugWriteLine("RedirectLgin authUrl location1 " + location);
				//location: https://oauth.nicovideo.jp/oauth2/authorize?scope=openid+email+profile&state=aaa&response_type=code&client_id=aaa&redirect_uri=https%3A%2F%2Fauth.sheeta.com%2Fauth%2Frealms%2FFCS00001%2Fbroker%2Fniconico%2Fendpoint&nonce=aaa
				
				h = util.getHeader();
				var nicoC = cc.GetCookieHeader(new Uri(location));
				h.Add("Cookie", nicoC);
				h.Remove("Accept-Encoding");
				r = curl.getStr(location, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "GET", "", true);
				util.debugWriteLine("RedirectLogin auth r2 " + r);
				if (r == null) return null;
				var _location = util.getRegGroup(r, "[lL]ocation: (.+)\r");
				if (_location == null) {
					if (r.IndexOf("/oauth2/authorized") == -1) return null;
					var authPostUrl = "https://oauth.nicovideo.jp/oauth2/authorized";
					var _h = new Dictionary<string, string>();
					_h.Add("Referer", location);
					_h.Add("Content-Type", "application/x-www-form-urlencoded");
					_h.Add("User-Agent", util.userAgent);
					_h.Add("Cookie", cc.GetCookieHeader(new Uri(authPostUrl)));
					var authPostD = "";
					var f = new string[]{"random", "scope", "redirect_uri", "uri_token", "client_id", "csrf_token", "response_type", "nonce", "state", "accepted"};
					foreach (var _f in f) {
						var d = util.getRegGroup(r, "name=\"" + _f + "\" value=\"(.+?)\"");
						if (d == null) {
							cpr.rm.form.addLogText("認可画面が取得できませんでした");
							return null;
						}
						authPostD += (authPostD == "" ? "" : "&") + 
								_f + "=" + d;
					}
					util.debugWriteLine("authPostD + " + authPostD);
					var authPostRes = DialogResult.No;
					cpr.rm.form.formAction(() => authPostRes = MessageBox.Show("ニコニコチャンネルプラスへのログインを認可しますか？(ニコニコのアカウント設定ページで取り消すことができます)", "", MessageBoxButtons.YesNo), false);
					if (authPostRes == DialogResult.No) return null;
					r = curl.getStr(authPostUrl, _h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "POST", authPostD, true);
					if (r == null) return null;
					_location = util.getRegGroup(r, "[lL]ocation: (.+)\r");
					util.debugWriteLine("RedirectLgin authUrl location authPost " + _location);
					if (_location == null) return null;
				}
				location = _location;
				util.debugWriteLine("RedirectLgin authUrl location2 " + location);
				//Location: https://auth.sheeta.com/auth/realms/FCS00001/broker/niconico/endpoint?code=aaa&state=aaa
				//X-Niconico-Id: 000
				
				
				h = util.getHeader();
				h.Add("Cookie", cc.GetCookieHeader(new Uri(location)));
				r = curl.getStr(location, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", "", true);
				util.debugWriteLine("RedirectLogin auth r3 " + r);
				if (r == null) return null;
				setHeaderToCookie(".sheeta.com", r, cc);
				location = util.getRegGroup(r, "location: (.+)\r");
				if (location == null) return null;
				util.debugWriteLine("RedirectLgin authUrl location3 " + location);
				var code = util.getRegGroup(location, "code=([^&]+)");
				if (code == null) return null;
				/*
				location: https://nicochannel.jp/a-ways-studio/login?session_state=caec60d5-7318-44c7-a271-6583f7294ca6&code=4823d7f6-43ae-4ea7-b833-2af042864a3a.caec60d5-7318-44c7-a271-6583f7294ca6.9f6f4d66-72e5-414d-b02b-66715219d55c
				set-cookie: KEYCLOAK_LOCALE=
				set-cookie: KC_RESTART=
				set-cookie: KEYCLOAK_IDENTITY=
				set-cookie: KEYCLOAK_IDENTITY_LEGACY=
				set-cookie: KEYCLOAK_SESSION=
				set-cookie: KEYCLOAK_SESSION_LEGACY=
				set-cookie: KEYCLOAK_REMEMBER_ME=xxx;
				*/
				
				h = util.getHeader();
				h.Add("Cookie", cc.GetCookieHeader(new Uri(location)));
				r = curl.getStr(location, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", "", true);
				util.debugWriteLine("RedirectLogin auth r4 " + r);
				if (r == null) return null;
				setHeaderToCookie(".sheeta.com", r, cc);
				
				var url = "https://nfc-api.nicochannel.jp/fc/fanclub_groups/1/sns_login";
				//{"key_cloak_user":{"code":"b6bd7ee2-22f7-4307-b67d-79c2c7648923.823f0639-4afe-4fc8-8d04-fe297131b0bf.67ff777c-4599-43be-ba64-437ebfb1f261","redirect_uri":"https://nicochannel.jp/fairy_teatime/login"},"fanclub_site":{"id":34}}
				var data = "{\"key_cloak_user\":{\"code\":\"" + code + "\",\"redirect_uri\":\"https://nicochannel.jp/" + channelName + "/login\"},\"fanclub_site\":{\"id\":" + int.Parse(channelNum.Substring(3)).ToString() + "}}";
				util.debugWriteLine("redirect login code data " + data);
				h = util.getHeader();
				h.Add("Cookie", cc.GetCookieHeader(new Uri(url)));
				h.Add("Content-Type", "application/json");
				r = curl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", data, true);
				util.debugWriteLine("RedirectLogin auth r5 " + r);
				if (r == null) return null;
				setHeaderToCookie(".nicochannel.jp", r, cc);
				util.debugWriteLine("RedirectLgin authUrl location5 " + location);
				var access_token = util.getRegGroup(r, "access_token\":\"(.+?)\"");
				if (access_token == null) return null;
				//set-cookie: FCS00001=eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJkNGYyMGYwMC05ODFiLTQ3N2QtYWEwMi1iZjZjMGY4NDk4MTYifQ.eyJleHAiOjE2NTU3MTE3MDEsImlhdCI6MTY1MzExOTcwMSwianRpIjoiNGNjZjBlMzgtYjc0NS00MmEwLTgyNTEtOGE2NmU2YjFkNGE2IiwiaXNzIjoiaHR0cHM6Ly9hdXRoLnNoZWV0YS5jb20vYXV0aC9yZWFsbXMvRkNTMDAwMDEiLCJhdWQiOiJodHRwczovL2F1dGguc2hlZXRhLmNvbS9hdXRoL3JlYWxtcy9GQ1MwMDAwMSIsInN1YiI6ImE1NzdhNTVmLWNjZTMtNDZjNC1iOTg1LTBkYjg1YmJjZGViOSIsInR5cCI6IlJlZnJlc2giLCJhenAiOiJGQ1MwMDExNiIsInNlc3Npb25fc3RhdGUiOiIyZGY1NzcyZS03MzNiLTQwZjItOGIzZC0xNmQ1ODhiZmFmZTAiLCJzY29wZSI6Im9wZW5pZCBlbWFpbCBwcm9maWxlIiwic2lkIjoiMmRmNTc3MmUtNzMzYi00MGYyLThiM2QtMTZkNTg4YmZhZmUwIn0.bpAluJ60Sx2mVWaoUUgcHTZjdN_Pv4cD1P51l5rEJsE; Path=/fc/fanclub_groups/1/auth; Max-Age=2592000; HttpOnly; Secure; SameSite=Strict
				//set-cookie: client_id=FCS00116; Path=/fc/fanclub_groups/1/auth; Max-Age=2592000; HttpOnly; Secure; SameSite=Strict
				//{"data":{"access_token":"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICItUnRFd09TbFVCalFza0IzQWROdmdyZmRhZHllbm1reVF1SW96dG5hdno4In0.eyJleHAiOjE2NTMxMjAwMDEsImlhdCI6MTY1MzExOTcwMSwiYXV0aF90aW1lIjoxNjUzMTE5NzAxLCJqdGkiOiIwOWVjOTY3NC1kZGJmLTQ4ODQtODU4Ny1hMTY5NmVlYjVjY2UiLCJpc3MiOiJodHRwczovL2F1dGguc2hlZXRhLmNvbS9hdXRoL3JlYWxtcy9GQ1MwMDAwMSIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiJhNTc3YTU1Zi1jY2UzLTQ2YzQtYjk4NS0wZGI4NWJiY2RlYjkiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJGQ1MwMDExNiIsInNlc3Npb25fc3RhdGUiOiIyZGY1NzcyZS03MzNiLTQwZjItOGIzZC0xNmQ1ODhiZmFmZTAiLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwiZGVmYXVsdC1yb2xlcy1mY3MwMDAwMSIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgZW1haWwgcHJvZmlsZSIsInNpZCI6IjJkZjU3NzJlLTczM2ItNDBmMi04YjNkLTE2ZDU4OGJmYWZlMCIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJuaWNrbmFtZSI6IuOCsuOCueODiCIsInByZWZlcnJlZF91c2VybmFtZSI6Im5pY29uaWNvXzk0ODkxODkyIiwiZW1haWwiOiJndWVzdG5pY29uQGdtYWlsLmNvbSJ9.PShpEIKaxr4DMBQW7E2ZwvK4v_KzVKnfv8l9l2NKqtns1Aw20aHWBHTU3aoFz6V9eKOk65oQ64g7rbQ2-wvGO6IVmyKTrdWu8tts_HlzDTkE6W6AdIAZBOzrz385fKssKkk-mkkTuEGU0BDjlwANVDIyNnq_n8UFfqmYf6h6vthDl1sp96lqGlnKYjAPOmW6nx0USZK9soXVAzLRtqXQUt6qSIrRhgWO5dqVrD4ffPK16FJlwwqK0qc5WFKGoi-Qu3758BkYA17b4dkQSLwakTSR-k8WbYb69RAz9OQ2ElyQ81OtbMd-bIPZdKvPyEvpJRJApavEZpS-OeS9NEwdvQ","email_verified":true,"link":{"logout":"/fc/fanclub_groups/1/auth/logout","refresh":"/fc/fanclub_groups/1/auth/refresh"}}}

				cpr.rm.cfg.set("chPlus_access_token", access_token);
				//var _cc = JsonConvert.SerializeObject(cc);
				cpr.rm.cfg.set("chPlus_refreshCookie", cc.GetCookieHeader(new Uri(url)));
				//var refleshUrl = "/fc/fanclub_groups/1/auth/refresh";
				var a = cc.GetCookieHeader(new Uri(url));
				return access_token;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
		}
		void setHeaderToCookie(string host, string r, CookieContainer cc) {
			var m = new Regex("set-cookie: (.+?)=(.*?);").Matches(r);
			foreach (Match _m in m)
				cc.Add(new Cookie(_m.Groups[1].Value, _m.Groups[2].Value, "/", host));
		}
		public bool isActiveAuth(string channelNum) {
			var at = cpr.rm.cfg.get("chPlus_access_token");
			if (at == "") return false;
			
			var fNum = int.Parse(channelNum.Substring(3)).ToString();
			var h = util.getHeader();
			h.Add("Content-Type", "application/json");
			h.Add("Authorization", "Bearer " + at);
			var r = curl.getStr("https://nfc-api.nicochannel.jp/fc/fanclub_sites/" + fNum + "/user_info", h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", "null", true);
			util.debugWriteLine("user info " + r);
			return r.IndexOf("\"fanclub_member\":null") == -1;
		}
		string getNicoIdByChPlus() {
			try {
				var chAt = cpr.rm.cfg.get("chPlus_access_token");
				if (chAt == "") return null;
					
				//var _cn = util.getRegGroup(vpRes, "\"fanclub_code\":\".*(\\d+)\"");
				//if (_cn == null) return null;
				var _cn = si.recFolderFileInfo[4];
				//FCS00000
				var cn = int.Parse(_cn.Substring(3).ToString());
				var url = "https://nfc-api.nicochannel.jp/fc/fanclub_sites/" + cn + "/user_info";
				var h = util.getHeader();
				h.Add("Authorization", "Bearer " + chAt);
				var r = curl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", "null", true);
				if (r == null) return null;
				var chId = util.getRegGroup(r, "\"fanclub_member\".+?\"id\":(\\d+)");
				
				url = "https://nfc-api.nicochannel.jp/fc/fanclub_sites/" + cn + "/fanclub_members/" + chId + "/mypage";
				r = curl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", "", true);
				if (r == null) return null;
				var nicoId = util.getRegGroup(r, "\"user_id\":\"(\\d+)\"");
				return nicoId;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
		}
		bool setCookie(string chNum, string chName) {
			/*
			if (isActiveAuth(chNum)) {
				return; 
			}
			*/
			//FCS00000
			
			try {
				var cg = new CookieGetter(cpr.rm.cfg);
				for (var i = 0; i < 2; i++) {
					try {
						var cgret = cg.getHtml5RecordCookie("https://live.nicovideo.jp/", false).Result;
						//cgret.Wait();
						if (cgret == null || cgret[0] == null)
							continue;
						cc = cgret[0];
						break;
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace);
						Thread.Sleep(5000);
					}
				}
				if (cc == null) {
					cpr.rm.form.addLogText("ニコニコにログインできませんでした");
					cpr.rm.form.addLogText(cg.log);
					return false;
				}
				/*
				var _auth = rl.loginAuth(cc, chNum, chName);
				if (_auth == null) {
					rm.form.addLogText("ニコニコチャンネルプラスにログインできませんでした");
					return new CookieContainer();
				}
				//auth = _auth;
				return rl.cc;
				*/
				return true;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return false;
			}
		}
		bool refreshAuth() {
			if (string.IsNullOrEmpty(auth)) return false;
			var c = cpr.rm.cfg.get("chPlus_refreshCookie");
			if (c == "") return false;
			
			try {
				var url = "https://nfc-api.nicochannel.jp/fc/fanclub_groups/1/auth/refresh";
				var h = util.getHeader();
				h.Add("Authorization", "Bearer " + auth);
				h.Add("Cookie", c);
				var r = curl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", "", true);
				if (r == null) return false;
				var _auth = util.getRegGroup(r, "\"access_token\":\"(.+?)\"");
				if (_auth == null) return false;
				auth = _auth;
				setHeaderToCookie(".nicochannel.jp", r, cc);
				c =  cc.GetCookieHeader(new Uri(url));
				cpr.rm.cfg.set("chPlus_access_token", auth);
				cpr.rm.cfg.set("chPlus_refreshCookie", c);
				chPlusCookie = c; 
				return true;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return false;
			}
		}
		int init() {
			//0-login 1-ok 2-no auth
			var isNicoLogin = setCookie(si.recFolderFileInfo[4], si.recFolderFileInfo[5]);
			if (!isNicoLogin) {
				auth = "";
				return 2;
			}
			
			auth = cpr.rm.cfg.get("chPlus_access_token");
			chPlusCookie = cpr.rm.cfg.get("chPlus_refreshCookie");
			if (auth != "") {
				if (!refreshAuth()) return 0;
			}
			
			var nicoId = getNicoIdByChPlus();
			if (nicoId == null) {
				return 0;
			} else {
				var us = cpr.rm.cfg.get("user_session");
				if (us == null) {
					auth = "";
					return 0;
				}
				var usId =  util.getRegGroup(us, "user_session_(\\d+)");
				if (usId == null) {
					auth = "";
					return 0;
				}
				if (nicoId != usId) {
					return 0;
				} else {
					return 1;
				}
			}
		}
	}
}
