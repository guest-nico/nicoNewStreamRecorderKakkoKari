/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/14
 * Time: 0:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Threading.Tasks;
using namaichi;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using namaichi.info;
using namaichi.utility;

namespace namaichi.rec
{
	/// <summary>
	/// Description of RecordFromUrl.
	/// </summary>
	public class RecordFromUrl
	{
		public string url;
//		private CookieContainer[] container;
		private RecordingManager rm;
		private string res;
		private bool isJikken = false;
//		private JikkenRecorder jr;
		public string lvid;
		public int[] tsRecNumArr;
		public List<numTaskInfo> subGotNumTaskInfo = null;
		public string[] id = new string[2];
		public bool isRtmpMain = false;
		//public bool isRtmpTimeShiftEnabled = true;
		public byte[] firstFlvData = null;
		public bool isPlayOnlyMode = false;
		
		public Html5Recorder h5r = null;
		
		public RecordFromUrl(RecordingManager rm, bool isPlayOnlyMode)
		{
			this.rm = rm;
			isRtmpMain = rm.cfg.get("EngineMode") == "2";
			this.isPlayOnlyMode = isPlayOnlyMode; 
		}
		public int rec(string url, string lvid) {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			util.debugWriteLine("RecordFromUrl rec");
			util.debugWriteLine(url + " " + lvid);
			this.lvid = lvid.StartsWith("lv") ? util.getRegGroup(lvid, "(lv\\d+)") : lvid;
			this.url = util.getRegGroup(url, "([^,]+)");
			tsRecNumArr = (this.lvid == lvid) ? null : Array.ConvertAll<string, int>(util.getRegGroup(lvid, ",(.+)").Split(','), (i) => {return int.Parse(i);});
			
			var mainT = Task.Run<int>(() => {
				if (lvid.StartsWith("lv")) return _rec(this.url);
				else return new ChannelPlusRecorder(url, rm.form, rm, lvid, this).run();
			});
			
			try {
				while (true) {
					if (mainT.Wait(1000)) break;
				}
				var ret = mainT.Result;
				return ret;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				//return mainT.Result;
				return 1;
			}
		}
		
		private int _rec(string url) {
			JikkenRecorder jr = null;
			//RtmpRecorder rr = null;
			var isRtmp = isRtmpMain;
			CookieContainer cc;
			
			var pageType = this.getPageType(url, true, ref jr, out cc);
			if (pageType == -1) return 2;
			
			util.debugWriteLine("pagetype " + pageType + " container " + cc);
			if (cc == null) {
				rm.form.addLogText("ログインに失敗しました。");
				if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
					rm.form.formAction(() => 
							util.showMessageBoxCenterForm(rm.form, "ログインに失敗しました。\n" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None), false);
				}
				if (bool.Parse(rm.cfg.get("IsfailExit"))) {
					rm.rfu = null;
					rm.form.close();
				}
				return 2;
			}
			
			util.debugWriteLine("pagetype " + pageType);
			
			while (this == rm.rfu) {
				util.debugWriteLine("pagetype " + pageType);
				if (pageType == 0 || pageType == 7) {
					var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					int recResult = 0;
					
					if (isPlayOnlyMode && pageType == 7 && isRtmp) isRtmp = false;
					
					if (isJikken) {
						//実験放送　なくし
						//recResult = jr.record(res, isRtmp);
					} else {
						var isTimeShift = pageType == 7;
						
						h5r = new Html5Recorder(cc, rm, this);
						recResult = h5r.record(res, isRtmp, pageType, url, lvid, isTimeShift);
					}

					util.debugWriteLine("recresult " + recResult);
					return recResult;					
				} else if (pageType == 1) {
					rm.form.addLogText("満員です。");
					if (bool.Parse(rm.cfg.get("Isretry"))) {
						System.Threading.Thread.Sleep(10000);
						
						while(this == rm.rfu) {
							try {
								//res = util.getPageSource(url, cc);
								var h = util.getHeader(cc, null, url);
								res = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false);
								isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
								//var _pageType = (isJikken) ? getJikkenPageType(res, out jr, cc) : util.getPageType(res);
								var _pageType = (isJikken) ? 0 : util.getPageType(res);
								util.debugWriteLine(_pageType);
								if (pageType != 1) continue;
								
								System.Threading.Thread.Sleep(5000);
							} catch (Exception e) {
								util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
							}
						}
						continue;
					} else {
						return 2;
					}
					
				} else if (pageType == 5) {
					var reason = util.getRegGroup(res, "rejectedReasons&quot;:\\[(.*?)\\]");
					if (reason != null) {
						if (reason.IndexOf("notAllowedCountry") > -1) {
							rm.form.addLogText("この国からの接続は許可されていません");
							return 2;
						}
					}
					rm.form.addLogText("reason:" + reason);
					
					if (bool.Parse(rm.cfg.get("Isretry"))) {
						rm.form.addLogText("接続エラー。10秒後リトライします。");
						System.Threading.Thread.Sleep(10000);
						
						try {
							pageType = getPageType(url, false, ref jr, out cc);
							util.debugWriteLine("pagetype_ " + pageType);
						} catch (Exception e) {
							util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
						}
						continue;
					} else {
						rm.form.addLogText("接続エラー");
						return 2;
					}
					
				} else if (pageType == 6) {
					util.debugWriteLine("pagetype 6 process");
					System.Threading.Thread.Sleep(3000);
					try {
						pageType = getPageType(url, false, ref jr, out cc);
						util.debugWriteLine("pagetype_ " + pageType);
					} catch (Exception e) {
						util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
						rm.form.addLogText(e.Message + " " + e.StackTrace + " ");
					}
					continue;

				} else if (pageType == 4) {
					rm.form.addLogText("require_community_member");
					
					util.debugWriteLine(rm.cfg.get("IsautoFollowComgen"));
					if (bool.Parse(rm.cfg.get("IsautoFollowComgen"))) {
						
						var isFollow = new FollowCommunity().followCommunity(res, cc, rm.form, rm.cfg, isPlayOnlyMode);
						util.debugWriteLine("isfollow " + isFollow);
						if (isFollow) {
							pageType = getPageAfterFollow(url, lvid, ref jr, out cc);
							util.debugWriteLine("pagetype_ " + pageType);
							continue;
						}
					}
					if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
						var ret = rm.form.formAction(() =>
								util.showMessageBoxCenterForm(rm.form, "コミュニティに入る必要があります：\nrequire_community_member/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None), false);
						if (!ret) return 2;
					}
					if (bool.Parse(rm.cfg.get("IsfailExit"))) {
						rm.rfu = null;
						rm.form.close();
					}
					return 2;
					
				} else if (pageType == 8) {
					rm.form.addLogText("この番組の視聴にはシリアル番号が必要です。");
					return 2;
				} else if (pageType == 9) {
					rm.form.addLogText("この番組の視聴には予約が必要です。");
					
					DialogResult isYoyakuRes = DialogResult.None;
					var reserveMessage = rm.cfg.get("reserveMessage");
					if (reserveMessage != "ダイアログで確認") {
						isYoyakuRes = reserveMessage == "常に予約する" ? DialogResult.Yes : DialogResult.No;
					} else {
						rm.form.formAction(() => {
							isYoyakuRes = util.showMessageBoxCenterForm(rm.form, "この番組の視聴には予約が必要です。予約しますか？", "", MessageBoxButtons.YesNo);
						}, false);
					}
					if (isYoyakuRes == DialogResult.No) return 2;
					
					
					var r = new Reservation(cc, lvid);
					//var reserveRet = r.reserve();
					var reserveRet = r.live2Reserve2();
					if (reserveRet == "ok") {
						rm.form.addLogText("予約しました");
						pageType = getPageType(url, false, ref jr, out cc);
						continue;
					} else {
						rm.form.addLogText(reserveRet);
						rm.form.addLogText("予約できませんでした");
						if (reserveRet == "予約リストが一杯です。") {
							//DialogResult isOpenMypageRes = DialogResult.None;
							rm.form.formAction(() => {
								var isOpenMypageRes = util.showMessageBoxCenterForm(rm.form, "予約リストが一杯です。マイページを開きますか？", "", MessageBoxButtons.YesNo);
							    if (isOpenMypageRes == DialogResult.Yes) 
									System.Diagnostics.Process.Start("https://live.nicovideo.jp/my");
							}, false);
						}
						return 2;
					}
				} else if (pageType == 10) {
					var r = new Reservation(cc, lvid).useLive2Reserve2();
					if (!r) {
						rm.form.addLogText("この番組のチケットを正常に使用できませんでした。");
						return 2;
					}
					pageType = getPageType(url, false, ref jr, out cc);
					util.debugWriteLine("pagetype 10_ " + pageType);
					continue;
				} else if (pageType == 11) {
					rm.form.addLogText("この番組は有料チケットが必要です。");
					return 2;
				} else if (pageType == 12) {
					rm.form.addLogText("ニコニコ実況放送でした。");
					return 2;
				} else if (pageType == 13) {
					try {
						pageType = getPageType(url, false, ref jr, out cc);
						util.debugWriteLine("pagetype_ " + pageType);
					} catch (Exception e) {
						util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
					}
					continue;
				} else {
					var mes = "";
					if (pageType == 2 || pageType == 3) mes = "この放送は終了しています。";
					rm.form.addLogText(mes);
					var reason = util.getRegGroup(res, "rejectedReasons&quot;:\\[(.*?)\\]");
					if (reason != null) {
						rm.form.addLogText("(reason: " + reason.Replace("&quot;", "\"") + ")");
						if (reason.IndexOf("notAllowedCountry") > -1) 
							rm.form.addLogText("この国からの接続は許可されていません");
					}
					util.debugWriteLine("pagetype " + pageType + " 終了");
					
					if (bool.Parse(rm.cfg.get("IsdeleteExit"))) {
						rm.rfu = null;
						rm.form.close();
					}
					return 2;
				}
			}
			return 2;
		}
		public int getPageType(string url, bool isLogin, ref JikkenRecorder jr, out CookieContainer cc) {
			var dt = DateTime.Now;
			var isFirst = true;
			CookieGetter.isLoginCheck = true;
			while (this == rm.rfu) {
				try {
					if (isLogin && DateTime.Now - dt > TimeSpan.FromSeconds(15)) {
//						cc = null;
//						return -1;
					}
					
					if (isRtmpMain) 
						url = url.Replace("live2.nicovideo.jp", "live.nicovideo.jp");

					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url);
					cgret.Wait();
					CookieGetter.isLoginCheck = false;
					                           
					//if (isSub && cg.id != null)  id[1] = cg.id;
					if (cg.id != null)  id[0] = cg.id;
					if (id[0] != null && id[1] != null && id[0] == id[1]) {
						rm.form.addLogText("メインアカウントとサブアカウントのIDが同じでした");
						util.debugWriteLine("メインアカウントとサブアカウントのIDが同じでした");
						cc = null;
						return -2;
					}
					
		//			cgret.ConfigureAwait(false);
					if (cgret == null || cgret.Result[0] == null) {
						util.debugWriteLine("cgret " + cgret);
						if (isLogin && isFirst) {
							rm.form.addLogText(cg.log);
//							rm.form.addLogText("ログインに失敗しました。");
							isFirst = false;
						}
						if (bool.Parse(rm.cfg.get("IsdeleteExit"))) {
							cc = null;
							rm.rfu = null;
							rm.form.close();
							return 2;
						}
						if (cg.reason != null) {
							cc = null;
							if (cg.reason == "not_login")
								rm.form.formAction(() => 
									util.showMessageBoxCenterForm(rm.form, "ログインに失敗しました。\n" + lvid));
							return -1;
						}
						if (cg.pageSource != null && 
						    	util.getRegGroup(cg.pageSource, "(この番組は放送者により削除されました。<br />|削除された可能性があります。<br />)|\">お探しのページは削除されたか") != null) {
							cc = null;
							return 2;
						}
						
						System.Threading.Thread.Sleep(
								(cg.pageSource != null && (cg.pageSource.IndexOf("ご指定のページが見つかりませんでした") > -1 ||
									cg.pageSource.IndexOf("ただいまメンテナンス中です。") > -1)) ? 300000 : 10000);
						continue;
					}
					cc = cgret.Result[0];
					util.debugWriteLine("container " + cc);
						
					res = cg.pageSource;
					if (string.IsNullOrEmpty(rm.form.getTitleLabelText()))
						setDisplay(res);
					
					if (isRtmpMain && false) {
						//if (res.IndexOf("%3Cgetplayerstatus%20") > -1) {
						if (res.IndexOf("player_type = null") > -1) {
							if (res.IndexOf("\"timeshift_reservation") > -1) return 9;
							else if (res.IndexOf("\"Nicolive.WatchingReservation.confirm") > -1) return 10;
							return 2;
						} else if (res.IndexOf("player_type = 'flash'") > -1 && 
						           res.IndexOf("%3Cgetplayerstatus%20") > -1) {
							var _res = util.getRegGroup(res, "(%3Cgetplayerstatus%20.+?%3C%2Fgetplayerstatus%3E)");
							_res = System.Net.WebUtility.UrlDecode(_res);
							var isTimeShift = true;
							var ret = util.getPageTypeRtmp(_res, ref isTimeShift, false);
							
							//isRtmpMain = true;
							return ret;
						} else if (res.IndexOf("<!doctype html>") == -1) {
							var __url = "http://live.nicovideo.jp/api/getplayerstatus?v=" + lvid;
							var __res = util.getPageSource(__url, cc);
							var isTimeShift = true;
							var ret = util.getPageTypeRtmp(__res, ref isTimeShift, false);
							res += __res;
							return ret; 
						}
					} else {
						var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
						if (isJikken) {
							//実験放送なくし
							//return getJikkenPageType(res, out jr, cc);
						} else {
							var _pageType = util.getPageType(res);
							util.debugWriteLine(_pageType);
							
							if (_pageType == 13) {
								var opentime = util.getRegGroup(res, "&quot;openTime&quot;:(\\d+)");
								var serverTime = util.getRegGroup(res, "&quot;serverTime&quot;:(\\d+)");
								var timeLeft = long.Parse(opentime) - long.Parse(serverTime) / 1000;
								var waitSecond = timeLeft < 60 ? 10 : (timeLeft < 600 ? 60 : 600);
								rm.form.addLogText("放送が開始されていませんでした。待機します。");//(" + waitSecond + "秒)");
								#if DEBUG
									rm.form.addLogText("(" + waitSecond + "秒)now" + DateTime.Now + " o" + opentime + " s" + serverTime);
								#endif
								System.Threading.Thread.Sleep(waitSecond * 1000);
							}
							return _pageType;
						}
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
					if (isLogin) {	
						rm.form.addLogText("ページの取得に失敗しました。" + e.Message + e.Source + e.StackTrace);
						isLogin = false;
					}
				}
			}
			cc = null;
			return 5;
		}
		private int getPageAfterFollow(string url, string lvid, ref JikkenRecorder jr, out CookieContainer cc) {
			Uri TargetUrl = new Uri("https://live.nicovideo.jp");
			//Uri TargetUrl2 = new Uri("https://live2.nicovideo.jp");
			for (int i = 0; this == rm.rfu; i++) {
				try {
					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url);
					cgret.Wait();
					
					if (cgret == null || cgret.Result == null) {
						System.Threading.Thread.Sleep(1000);
						continue;
					}
					cc = cgret.Result[0];
					
	//				var pagetype = getPageType(url + "?ref=grel");
	//				if (pagetype != 5) return pagetype;
	//				if (res.IndexOf("会場のご案内") < 0) break;
					
					
					var _url = "https://live.nicovideo.jp/watch/" + lvid;
					//res = util.getPageSource(_url, cc, _url);
					var h = util.getHeader(cc, null, _url);
					res = new Curl().getStr(_url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false, true, true);
					isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					int pagetype;
//					if (isRtmp) pagetype = getRtmpPageType(res, isSub, out rr, cc);
					//pagetype = (isJikken) ? getJikkenPageType(res, out jr, cc) : util.getPageType(res);
					pagetype = (isJikken) ? 0 : util.getPageType(res);					
					
					if (!isJikken && pagetype != 5 && pagetype != 9 && pagetype != 4) return pagetype;
					if (isJikken && pagetype != 4) return pagetype;
					util.debugWriteLine(i);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
				}
				System.Threading.Thread.Sleep(3000);
			}
			cc = null;
			return -1;
		}
		/*
		private int getJikkenPageType(string res, out JikkenRecorder jr, CookieContainer cc) {
			jr = new JikkenRecorder(res, lvid, cc, rm.cfg, rm, this);
			return jr.getPageType();
		}
		*/
		void setDisplay(string res) {
			try {
				var isEnded = res.IndexOf("\"content_status\":\"closed\"") > -1 ||
					res.IndexOf("\"content_status\":\"ENDED\"") > -1;
				var si = new StreamInfo(url, lvid, isEnded, false);
				si.set(res);
				if (!si.getTimeInfo()) return;
				
				var rss = new RecordStateSetter(rm.form, false, isPlayOnlyMode, si.isRtmpOnlyPage, si.isReservation);
				Task.Run(() => {
				       	rss.set(si.data, si.recFolderFileInfo, res);
				});
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
	}
}
