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
		
		public RecordFromUrl(RecordingManager rm)
		{
			this.rm = rm;
			isRtmpMain = rm.cfg.get("EngineMode") == "2";
			
		}
		public int rec(string url, string lvid) {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			util.debugWriteLine("RecordFromUrl rec");
			util.debugWriteLine(url + " " + lvid);
			this.lvid = util.getRegGroup(lvid, "(lv\\d+)");
			this.url = util.getRegGroup(url, "([^,]+)");
			tsRecNumArr = (this.lvid == lvid) ? null : Array.ConvertAll<string, int>(util.getRegGroup(lvid, ",(.+)").Split(','), (i) => {return int.Parse(i);});
			
			var mainT = Task.Run<int>(() => {return _rec(this.url);});
			
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
			if (cc == null || cc == null) {
				rm.form.addLogText("ログインに失敗しました。");
				if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
					rm.form.formAction(() => 
							MessageBox.Show("ログインに失敗しました。\n" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None));
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
					
					if (rm.isPlayOnlyMode && pageType == 7 && isRtmp) isRtmp = false;
					
					if (isJikken) {
						//実験放送　なくし
						//recResult = jr.record(res, isRtmp);
					} else {
						var isTimeShift = pageType == 7;
						
						var h5r = new Html5Recorder(url, cc, lvid, rm, this, isTimeShift);
						recResult = h5r.record(res, isRtmp, pageType);
					}

					util.debugWriteLine("recresult " + recResult);
					return recResult;					
				} else if (pageType == 1) {
					rm.form.addLogText("満員です。");
					if (bool.Parse(rm.cfg.get("Isretry"))) {
						System.Threading.Thread.Sleep(10000);
						
						while(this == rm.rfu) {
							try {
								res = util.getPageSource(url, cc);
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
					rm.form.addLogText("require_community_menber");
					
					util.debugWriteLine(rm.cfg.get("IsautoFollowComgen"));
					if (bool.Parse(rm.cfg.get("IsautoFollowComgen"))) {
						
						var isFollow = new FollowCommunity().followCommunity(res, cc, rm.form, rm.cfg);
						util.debugWriteLine("isfollow " + isFollow);
						if (isFollow) {
							pageType = getPageAfterFollow(url, lvid, ref jr, out cc);
							util.debugWriteLine("pagetype_ " + pageType);
							continue;
						}
					}
					if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
						var ret = rm.form.formAction(() =>
								MessageBox.Show("コミュニティに入る必要があります：\nrequire_community_menber/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None));
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
					rm.form.formAction(() => {
						isYoyakuRes = MessageBox.Show(rm.form, "この番組の視聴には予約が必要です。予約しますか？", "", MessageBoxButtons.YesNo);
					});
					if (isYoyakuRes == DialogResult.No) return 2;
					
					var r = new Reservation(cc, lvid);
					var reserveRet = r.reserve();
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
								var isOpenMypageRes = MessageBox.Show(rm.form, "予約リストが一杯です。マイページを開きますか？", "", MessageBoxButtons.YesNo);
							    if (isOpenMypageRes == DialogResult.Yes) 
									System.Diagnostics.Process.Start("https://live.nicovideo.jp/my");
							});
						}
						return 2;
					}
				} else {
					var mes = "";
					if (pageType == 2 || pageType == 3) mes = "この放送は終了しています。";
					rm.form.addLogText(mes);
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
			while (this == rm.rfu) {
				try {
					if (isLogin && DateTime.Now - dt > TimeSpan.FromSeconds(15)) {
//						cc = null;
//						return -1;
					}
					
					if (isRtmpMain) url = url.Replace("live2.nicovideo.jp", "live.nicovideo.jp");
					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url);
					cgret.Wait();
					                           
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
						System.Threading.Thread.Sleep(3000);
						continue;
					}
				//			if (cgret == null) return true;
					cc = cgret.Result[0];
					util.debugWriteLine("container " + cc);
						
					res = cg.pageSource;
					
	//				Uri TargetUrl = new Uri("http://live.nicovideo.jp/");
	//				util.debugWriteLine("1 " + container.GetCookieHeader(TargetUrl));
	//				TargetUrl = new Uri("http://live2.nicovideo.jp/");
	//				util.debugWriteLine("2 " + container.GetCookieHeader(TargetUrl));
					//if (res.IndexOf("siteId&quot;:&quot;nicolive2") > -1) {
//					if (isRtmp) return getRtmpPageType(res, isSub, out rr, cc);
					if (isRtmpMain) {
						if (res.IndexOf("%3Cgetplayerstatus%20") > -1) {
							var _res = util.getRegGroup(res, "(%3Cgetplayerstatus%20.+?%3C%2Fgetplayerstatus%3E)");
							_res = System.Net.WebUtility.UrlDecode(_res);
							var isTimeShift = true;
							var ret = util.getPageTypeRtmp(_res, ref isTimeShift, false);
							//isRtmpMain = true;
							return ret;
						}
					}
					var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					if (isJikken) {
						//実験放送なくし
						//return getJikkenPageType(res, out jr, cc);
					} else {
						var _pageType = util.getPageType(res);
						util.debugWriteLine(_pageType);
						return _pageType;
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
					if (isLogin) {	
						rm.form.addLogText("ページの取得に失敗しました。");
						isLogin = false;
					}
				}
			}
			cc = null;
			return 5;
		}
		private int getPageAfterFollow(string url, string lvid, ref JikkenRecorder jr, out CookieContainer cc) {
			Uri TargetUrl = new Uri("https://live.nicovideo.jp");
			Uri TargetUrl2 = new Uri("https://live2.nicovideo.jp");
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
					/*
					var wc = new WebHeaderCollection();
					var referer =  "http://live.nicovideo.jp/gate/" + lvid;
					container.Add(TargetUrl, new Cookie("_gali", "jsFollowingAdMain"));
					container.Add(TargetUrl2, new Cookie("_gali", "jsFollowingAdMain"));
	//				container.Add(TargetUrl, new Cookie("_gali", "all"));
	//				container.Add(TargetUrl2, new Cookie("_gali", "all"));
					
					res = util.getPageSource(url + "?ref=grel", ref wc, container, "");
					
					var pagetype = util.getPageType(res);
					*/
					
	//				var pagetype = getPageType(url + "?ref=grel");
	//				if (pagetype != 5) return pagetype;
	//				if (res.IndexOf("会場のご案内") < 0) break;
					var _url = "https://live2.nicovideo.jp/watch/" + lvid;                              
					var req = (HttpWebRequest)WebRequest.Create(_url + "?ref=grel");
					req.Proxy = null;
					req.AllowAutoRedirect = true;
		//			req.Headers = getheaders;
					req.Referer = "https://live.nicovideo.jp/gate/" + lvid;
					//var ccInd = (isSub) ? 1 : 0;
					var ccInd = 0;
					cc.Add(TargetUrl, new Cookie("_gali", "box" + lvid));
					if (cc != null) req.CookieContainer = cc;
					var _res = (HttpWebResponse)req.GetResponse();
					var dataStream = _res.GetResponseStream();
					var reader = new StreamReader(dataStream);
					res = reader.ReadToEnd();
					var getheaders = _res.Headers;
					var resCookie = _res.Cookies;
					
					isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					int pagetype;
//					if (isRtmp) pagetype = getRtmpPageType(res, isSub, out rr, cc);
					//pagetype = (isJikken) ? getJikkenPageType(res, out jr, cc) : util.getPageType(res);
					pagetype = (isJikken) ? 0 : util.getPageType(res);					
					
					if (!isJikken && pagetype != 5 && pagetype != 9) return pagetype;
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
	}
}
