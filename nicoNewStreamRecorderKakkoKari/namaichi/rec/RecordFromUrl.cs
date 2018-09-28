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

namespace namaichi.rec
{
	/// <summary>
	/// Description of RecordFromUrl.
	/// </summary>
	public class RecordFromUrl
	{
		private CookieContainer[] container;
		private RecordingManager rm;
		private string res;
		private bool isJikken = false;
		private JikkenRecorder jr;
		private string lvid;
		
		public RecordFromUrl(RecordingManager rm)
		{
			this.rm = rm;
			//CookieContainer container = new CookieContainer();
	        //container.Add(cookie);
			//this.container = container;
		}
		public int rec(string url, string lvid) {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			util.debugWriteLine("RecordFromUrl rec");
			util.debugWriteLine(url + " " + lvid);
			this.lvid = lvid;
			
			var mainT = Task.Run<int>(() => {return _rec(url, false);});
			try {
				mainT.Wait();
				return mainT.Result;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return mainT.Result;
			}
			
			
		}
		private int _rec(string url, bool isSub) {
			var pageType = this.getPageType(url, true, isSub);
			var ccInd = (isSub) ? 1 : 0;
			util.debugWriteLine("pagetype " + pageType + " container " + container + " isSub " + isSub);
			if (container == null || container[ccInd] == null) {
				rm.form.addLogText("ログインに失敗しました。");
				if (bool.Parse(rm.cfg.get("IsmessageBox"))) {
					if (rm.form.IsDisposed) return 2;
					try {
			        	rm.form.Invoke((MethodInvoker)delegate() {
			       			MessageBox.Show("ログインに失敗しました。\n" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
						});
					} catch (Exception e) {
			       		util.showException(e);
			       	}
				}
				if (bool.Parse(rm.cfg.get("IsfailExit"))) {
					rm.rfu = null;
					try {
						rm.form.Invoke((MethodInvoker)delegate() {
			       			try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	       		}
						});
					} catch (Exception e) {
			       		util.showException(e);
			       	}
					
				}
				
				return 2;
			}
			
			util.debugWriteLine("pagetype " + pageType);
			
			while (true && this == rm.rfu) {
				util.debugWriteLine("pagetype " + pageType);
				if (pageType == 0 || pageType == 7) {
					var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					int recResult = 0;
					if (isJikken) {
						recResult = jr.record(res);
					} else {
						var isTimeShift = pageType == 7;
						
						var h5r = new Html5Recorder(url, container[ccInd], lvid, rm, this, isTimeShift, isSub);
						recResult = h5r.record(res);
					}
					util.debugWriteLine("recresult " + recResult);
					return recResult;					
				} else if (pageType == 1) {
					rm.form.addLogText("満員です。");
					if (bool.Parse(rm.cfg.get("Isretry"))) {
						System.Threading.Thread.Sleep(10000);
						
						while(this == rm.rfu) {
							try {
								var wc = new WebHeaderCollection();
								res = util.getPageSource(url, ref wc, container[ccInd]);
								
								isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
								var _pageType = (isJikken) ? getJikkenPageType(res, isSub) : util.getPageType(res);
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
//							var wc = new WebHeaderCollection();
//							res = util.getPageSource(url, ref wc, container);
//							pageType = util.getPageType(res);
							pageType = getPageType(url, false, isSub);
							util.debugWriteLine("pagetype_ " + pageType);
						} catch (Exception e) {
							util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
//							rm.form.addLogText(e.Message + " " + e.StackTrace + " ");
						}
						continue;
					} else {
						rm.form.addLogText("接続エラー");
						return 2;
					}
					
				} else if (pageType == 6) {
					util.debugWriteLine("pagetype6process");
					System.Threading.Thread.Sleep(3000);
					try {
						pageType = getPageType(url, false, isSub);
						util.debugWriteLine("pagetype_ " + pageType);
					} catch (Exception e) {
						util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
						rm.form.addLogText(e.Message + " " + e.StackTrace + " ");
					}
					continue;

					
				} else if (pageType == 4) {
					rm.form.addLogText("require_community_menber");
//					rm.form.addLogText(res);
					
					util.debugWriteLine(rm.cfg.get("IsautoFollowComgen"));
					if (bool.Parse(rm.cfg.get("IsautoFollowComgen"))) {
						
						var isFollow = new FollowCommunity(isSub).followCommunity(res, container[ccInd], rm.form, rm.cfg);
						util.debugWriteLine("isfollow " + isFollow);
						if (isFollow) {
//							var wc = new WebHeaderCollection();
//							var referer = "http://live.nicovideo.jp/gate/" + lvid;
							pageType = getPageAfterFollow(url, lvid, isSub);
							util.debugWriteLine("pagetype_ " + pageType);
							continue;
						}
					}
					if (bool.Parse(rm.cfg.get("IsmessageBox"))) {
						if (rm.form.IsDisposed) return 2;
						try {
				        	rm.form.Invoke((MethodInvoker)delegate() {
				       			MessageBox.Show("コミュニティに入る必要があります：\nrequire_community_menber/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
							});
						} catch (Exception e) {
				       		util.showException(e);
				       	}
					}
					if (bool.Parse(rm.cfg.get("IsfailExit"))) {
						rm.rfu = null;
						try {
							rm.form.Invoke((MethodInvoker)delegate() {
				       			try { rm.form.Close();} 
								catch (Exception e) {
			       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       		}
							});
						} catch (Exception e) {
				       		util.showException(e);
				       	}
						
					}
					return 2;
					
				} else if (pageType == 8) {
					rm.form.addLogText("この番組の視聴にはシリアル番号が必要です。");
					return 2;
				} else if (pageType == 9) {
					rm.form.addLogText("この番組の視聴には予約が必要です。");
					DialogResult isYoyakuRes = DialogResult.None;
					rm.form.Invoke((MethodInvoker)delegate() {
						isYoyakuRes = MessageBox.Show(rm.form, "この番組の視聴には予約が必要です。予約しますか？", "", MessageBoxButtons.YesNo);
					               });
					if (isYoyakuRes == DialogResult.No) return 2;
					
					var r = new Reservation(container[ccInd], lvid);
					var reserveRet = r.reserve();
					if (reserveRet == "ok") {
						rm.form.addLogText("予約しました");
						//res = util.getPageSource(url, container, null, false, 2000);
						pageType = getPageType(url, false, isSub);
						continue;
					} else {
						rm.form.addLogText("予約できませんでした");
						rm.form.addLogText(reserveRet);
						return 2;
					}
				} else {
					var mes = "";
					if (pageType == 2) mes = "この放送は終了しています。";
					if (pageType == 3) mes = "この放送は終了しています。";
					rm.form.addLogText(mes);
					util.debugWriteLine("pagetype " + pageType + " 終了");
					
					if (bool.Parse(rm.cfg.get("IsdeleteExit"))) {
						rm.rfu = null;
						try {
							rm.form.Invoke((MethodInvoker)delegate() {
				       			try { rm.form.Close();} 
								 catch (Exception e) {
			       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       		}
							});
						} catch (Exception e) {
				       		util.showException(e);
				       	}
						
					}
					return 2;
					//var nh5r = new NotHtml5Recorder(url, container, lvid, rm, this);
					//nh5r.record(res);
				}
			}
			return 2;
            

		}
		public int getPageType(string url, bool isLogin, bool isSub) {
			while (this == rm.rfu) {
				try {
					
					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url);
					cgret.Wait();
					                                  
					
		//			cgret.ConfigureAwait(false);
					if (cgret == null || cgret.Result[0] == null) {
						util.debugWriteLine("cgret " + cgret);
						if (isLogin) {
							rm.form.addLogText(cg.log);
//							rm.form.addLogText("ログインに失敗しました。");
							isLogin = false;
						}
						System.Threading.Thread.Sleep(3000);
						continue;
					}
		//			if (cgret == null) return true;
					container = cgret.Result;
					util.debugWriteLine("container " + container);
					

	
					res = cg.pageSource;
					
	//				Uri TargetUrl = new Uri("http://live.nicovideo.jp/");
	//				util.debugWriteLine("1 " + container.GetCookieHeader(TargetUrl));
	//				TargetUrl = new Uri("http://live2.nicovideo.jp/");
	//				util.debugWriteLine("2 " + container.GetCookieHeader(TargetUrl));
					//if (res.IndexOf("siteId&quot;:&quot;nicolive2") > -1) {
					var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					if (isJikken) {
						return getJikkenPageType(res, isSub);
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
			return 5;
			
			/*
			var req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Timeout = 15000;
            req.CookieContainer = this.container;
            req.AllowAutoRedirect  = false;
            
            var res = (HttpWebResponse)req.GetResponse();
            return (res.Headers.Get("Location") == null) ? false : true;
            */
		}
		private int getPageAfterFollow(string url, string lvid, bool isSub) {
			Uri TargetUrl = new Uri("http://live.nicovideo.jp");
			Uri TargetUrl2 = new Uri("http://live2.nicovideo.jp");
			for (int i = 0; this == rm.rfu; i++) {
				try {
					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url);
					cgret.Wait();
					
					if (cgret == null || cgret.Result == null) {
						System.Threading.Thread.Sleep(1000);
						continue;
					}
					container = cgret.Result;
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
					var _url = "http://live2.nicovideo.jp/watch/" + lvid;                              
					var req = (HttpWebRequest)WebRequest.Create(_url + "?ref=grel");
					req.Proxy = null;
					req.AllowAutoRedirect = true;
		//			req.Headers = getheaders;
					req.Referer = "http://live.nicovideo.jp/gate/" + lvid;
					var ccInd = (isSub) ? 1 : 0;
					container[ccInd].Add(TargetUrl, new Cookie("_gali", "box" + lvid));
					if (container[ccInd] != null) req.CookieContainer = container[ccInd];
					var _res = (HttpWebResponse)req.GetResponse();
					var dataStream = _res.GetResponseStream();
					var reader = new StreamReader(dataStream);
					res = reader.ReadToEnd();
					var getheaders = _res.Headers;
					var resCookie = _res.Cookies;
					
	//				if (res.IndexOf("会場のご案内") < 0) break;
					isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					var pagetype = (isJikken) ? getJikkenPageType(res, isSub) : util.getPageType(res);
								
//					var pagetype = util.getPageType(res);
					if (!isJikken && pagetype != 5) return pagetype;
					if (isJikken && pagetype != 4) return pagetype;
					util.debugWriteLine(i);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
				}
				System.Threading.Thread.Sleep(3000);
			}
			return -1;
		}
		private int getJikkenPageType(string res, bool isSub) {
//			if (jr == null)
			var ccInd = (isSub) ? 1 : 0;
			jr = new JikkenRecorder(res, lvid, container[ccInd], rm.cfg, rm, this);
//			rm.jr = jr;
			return jr.getPageType();
		}
	}
}
