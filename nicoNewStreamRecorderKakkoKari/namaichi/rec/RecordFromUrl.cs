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
//		private CookieContainer[] container;
		private RecordingManager rm;
		private string res;
		private bool isJikken = false;
//		private JikkenRecorder jr;
		private string lvid;
		public List<numTaskInfo> subGotNumTaskInfo = null;
		public string[] id = new string[2];
		//public bool isWaitMainTask = true;
		private bool isSubAccountHokan = true;
		private bool isRtmpMain = false;
		
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
			
<<<<<<< HEAD
			isSubAccountHokan = true;
			isRtmpMain = false;
=======
			isSubAccountHokan = false;
			isRtmpMain = true;
>>>>>>> fee0d0e09501d3dc7af30e73409c32ddb0f932c8
			
			var mainT = Task.Run<int>(() => {return _rec(url, false);});
			Task subT = null;
//			var isSecond = true;
			if (rm.cfg.get("IsHokan") == "true" && !isRtmpMain && !rm.isPlayOnlyMode) {
				subGotNumTaskInfo = new List<numTaskInfo>();
				subT = Task.Run(() => {_rec(url, true);});
			}
			try {
				while (true) {
					if (mainT.Wait(1000)) break;
				}
				var ret = mainT.Result;
				if (ret == 3 && subT != null) subT.Wait();
				return ret;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				//return mainT.Result;
				return 1;
			}
		}
		
		private int _rec(string url, bool isSub) {
			JikkenRecorder jr = null;
			RtmpRecorder rr = null;
			var isRtmp = !isSubAccountHokan && (isRtmpMain || isSub);
			CookieContainer cc;
			
			var pageType = this.getPageType(url, true, isSub, ref jr, out cc);
			if (pageType == -2 && isSub) return 2;
			if (pageType == -1) return 2;
			
			//var ccInd = (isSub) ? 1 : 0;
			var ccInd = 0;
			util.debugWriteLine("pagetype " + pageType + " container " + cc + " isSub " + isSub);
			if (cc == null || cc == null) {
				rm.form.addLogText("ログインに失敗しました。(" + util.getMainSubStr(isSub) + ")");
				if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
					if (rm.form.IsDisposed) return 2;
					try {
			        	rm.form.Invoke((MethodInvoker)delegate() {
						               	MessageBox.Show("ログインに失敗しました。(" + util.getMainSubStr(isSub) + ")\n" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
						});
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
				}
				if (bool.Parse(rm.cfg.get("IsfailExit"))) {
					rm.rfu = null;
					if (util.isShowWindow) {
						try {
							rm.form.Invoke((MethodInvoker)delegate() {
				       			try { rm.form.Close();} 
								catch (Exception e) {
			       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       		}
							});
						} catch (Exception e) {
				       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				       	}
					}
				}
				
				return 2;
			}
			
			util.debugWriteLine("pagetype " + pageType);
			
			while (true && this == rm.rfu) {
				util.debugWriteLine("pagetype " + pageType);
				if (pageType == 0 || (!isRtmp && pageType == 7)) {
					var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					int recResult = 0;
					
					if (isJikken) {
						if (!jr.isLive && isSub) return 2;
						recResult = jr.record(res, isRtmp);
					} else {
						var isTimeShift = pageType == 7;
						if (isTimeShift && isSub) return 2;
						
						var h5r = new Html5Recorder(url, cc, lvid, rm, this, isTimeShift, isSub);
						recResult = h5r.record(res, isRtmp);
					}

					util.debugWriteLine("recresult " + recResult);
					return recResult;					
				} else if (pageType == 1) {
					rm.form.addLogText("満員です。(" + util.getMainSubStr(isSub) + ")");
					if (bool.Parse(rm.cfg.get("Isretry"))) {
						System.Threading.Thread.Sleep(10000);
						
						while(this == rm.rfu) {
							try {
								var wc = new WebHeaderCollection();
								res = util.getPageSource(url, ref wc, cc);
								
								isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
								var _pageType = (isJikken) ? getJikkenPageType(res, isSub, out jr, cc) : util.getPageType(res);
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
						rm.form.addLogText("接続エラー。10秒後リトライします。(" + util.getMainSubStr(isSub) + ")");
						System.Threading.Thread.Sleep(10000);
						
						try {
//							var wc = new WebHeaderCollection();
//							res = util.getPageSource(url, ref wc, container);
//							pageType = util.getPageType(res);
							pageType = getPageType(url, false, isSub, ref jr, out cc);
							util.debugWriteLine("pagetype_ " + pageType);
						} catch (Exception e) {
							util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
//							rm.form.addLogText(e.Message + " " + e.StackTrace + " ");
						}
						continue;
					} else {
						rm.form.addLogText("接続エラー(" + util.getMainSubStr(isSub) + ")");
						return 2;
					}
					
				} else if (pageType == 6) {
					util.debugWriteLine("pagetype6process");
					System.Threading.Thread.Sleep(3000);
					try {
						pageType = getPageType(url, false, isSub, ref jr, out cc);
						util.debugWriteLine("pagetype_ " + pageType);
					} catch (Exception e) {
						util.debugWriteLine(e.Message + " " + e.StackTrace + " ");
						rm.form.addLogText(e.Message + " " + e.StackTrace + " ");
					}
					continue;

					
				} else if (pageType == 4) {
					rm.form.addLogText("require_community_menber(" + util.getMainSubStr(isSub) + ")");
//					rm.form.addLogText(res);
					
					util.debugWriteLine(rm.cfg.get("IsautoFollowComgen"));
					if (bool.Parse(rm.cfg.get("IsautoFollowComgen"))) {
						
						var isFollow = new FollowCommunity(isSub).followCommunity(res, cc, rm.form, rm.cfg);
						util.debugWriteLine("isfollow " + isFollow);
						if (isFollow) {
//							var wc = new WebHeaderCollection();
//							var referer = "http://live.nicovideo.jp/gate/" + lvid;
							pageType = getPageAfterFollow(url, lvid, isSub, ref jr, out cc);
							util.debugWriteLine("pagetype_ " + pageType);
							continue;
						}
					}
					if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
						if (rm.form.IsDisposed) return 2;
						try {
				        	rm.form.Invoke((MethodInvoker)delegate() {
				       			MessageBox.Show("コミュニティに入る必要があります：\nrequire_community_menber/" + lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
							});
						} catch (Exception e) {
				       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				       	}
					}
					if (bool.Parse(rm.cfg.get("IsfailExit"))) {
						rm.rfu = null;
						if (util.isShowWindow) {
							try {
								rm.form.Invoke((MethodInvoker)delegate() {
					       			try { rm.form.Close();} 
									catch (Exception e) {
				       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				       	       		}
								});
							} catch (Exception e) {
					       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
					       	}
						}
					}
					return 2;
					
				} else if (pageType == 8) {
					rm.form.addLogText("この番組の視聴にはシリアル番号が必要です。(" + util.getMainSubStr(isSub) + ")");
					return 2;
				} else if (pageType == 9) {
					if (isSub) return 2;
					
					rm.form.addLogText("この番組の視聴には予約が必要です。(" + util.getMainSubStr(isSub) + ")");
					DialogResult isYoyakuRes = DialogResult.None;
					if (util.isShowWindow) {
						rm.form.Invoke((MethodInvoker)delegate() {
							isYoyakuRes = MessageBox.Show(rm.form, "この番組の視聴には予約が必要です。予約しますか？", "", MessageBoxButtons.YesNo);
						               });
					} else {
						isYoyakuRes = MessageBox.Show("この番組の視聴には予約が必要です。予約しますか？", "", MessageBoxButtons.YesNo);
					}
					if (isYoyakuRes == DialogResult.No) return 2;
					
					var r = new Reservation(cc, lvid);
					var reserveRet = r.reserve();
					if (reserveRet == "ok") {
						rm.form.addLogText("予約しました");
						//res = util.getPageSource(url, container, null, false, 2000);
						pageType = getPageType(url, false, isSub, ref jr, out cc);
						continue;
					} else {
						rm.form.addLogText(reserveRet);
						rm.form.addLogText("予約できませんでした");
						if (reserveRet == "予約リストが一杯です。") {
							DialogResult isOpenMypageRes = DialogResult.None;
							if (util.isShowWindow) {
								rm.form.Invoke((MethodInvoker)delegate() {
									isOpenMypageRes = MessageBox.Show(rm.form, "予約リストが一杯です。マイページを開きますか？", "", MessageBoxButtons.YesNo);
								});
							} else {
								isOpenMypageRes = MessageBox.Show("予約リストが一杯です。マイページを開きますか？", "", MessageBoxButtons.YesNo);
							}
							if (isOpenMypageRes == DialogResult.Yes) 
								System.Diagnostics.Process.Start("http://live.nicovideo.jp/my");
					
						}
						return 2;
					}
				} else {
					var mes = "";
					if (pageType == 2) mes = "この放送は終了しています。(" + util.getMainSubStr(isSub) + ")";
					if (pageType == 3) mes = "この放送は終了しています。(" + util.getMainSubStr(isSub) + ")";
					if (pageType == 7 && isRtmp) mes = "この放送は終了しています。(" + util.getMainSubStr(isSub) + ")";
					rm.form.addLogText(mes);
					util.debugWriteLine("pagetype " + pageType + " 終了");
					
					if (bool.Parse(rm.cfg.get("IsdeleteExit"))) {
						rm.rfu = null;
						if (util.isShowWindow) {
							try {
								rm.form.Invoke((MethodInvoker)delegate() {
					       			try { rm.form.Close();} 
									 catch (Exception e) {
				       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				       	       		}
								});
							} catch (Exception e) {
					       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
					       	}
						}
					}
					return 2;
					//var nh5r = new NotHtml5Recorder(url, container, lvid, rm, this);
					//nh5r.record(res);
				}
			}
			return 2;
            

		}
		public int getPageType(string url, bool isLogin, bool isSub, ref JikkenRecorder jr, out CookieContainer cc) {
			var dt = DateTime.Now;
			var isFirst = true;
			while (this == rm.rfu) {
				try {
					if (isLogin && DateTime.Now - dt > TimeSpan.FromSeconds(15)) {
						cc = null;
						return -1;
					}
					
					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url, isSub);
					cgret.Wait();
					                           
					if (isSub && cg.id != null)  id[1] = cg.id;
					if (!isSub && cg.id != null)  id[0] = cg.id;
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
							rm.form.addLogText(cg.log + "(" + util.getMainSubStr(isSub) + ")");
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
					var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					if (isJikken) {
						return getJikkenPageType(res, isSub, out jr, cc);
					} else {
						var _pageType = util.getPageType(res);
						util.debugWriteLine(_pageType);
						return _pageType;
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
					if (isLogin) {	
						rm.form.addLogText("ページの取得に失敗しました。(" + util.getMainSubStr(isSub) + ")");
						isLogin = false;
					}
				}
			}
			cc = null;
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
		private int getPageAfterFollow(string url, string lvid, bool isSub, ref JikkenRecorder jr, out CookieContainer cc) {
			Uri TargetUrl = new Uri("http://live.nicovideo.jp");
			Uri TargetUrl2 = new Uri("http://live2.nicovideo.jp");
			for (int i = 0; this == rm.rfu; i++) {
				try {
					var cg = new CookieGetter(rm.cfg);
					var cgret = cg.getHtml5RecordCookie(url, isSub);
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
					var _url = "http://live2.nicovideo.jp/watch/" + lvid;                              
					var req = (HttpWebRequest)WebRequest.Create(_url + "?ref=grel");
					req.Proxy = null;
					req.AllowAutoRedirect = true;
		//			req.Headers = getheaders;
					req.Referer = "http://live.nicovideo.jp/gate/" + lvid;
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
					
	//				if (res.IndexOf("会場のご案内") < 0) break;
					  
					isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
					int pagetype;
//					if (isRtmp) pagetype = getRtmpPageType(res, isSub, out rr, cc);
					pagetype = (isJikken) ? getJikkenPageType(res, isSub, out jr, cc) : util.getPageType(res); 
					
//					var pagetype = util.getPageType(res);
					if (!isJikken && pagetype != 5) return pagetype;
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
		private int getJikkenPageType(string res, bool isSub, out JikkenRecorder jr, CookieContainer cc) {
//			if (jr == null)
			//var ccInd = (isSub) ? 1 : 0;
			var ccInd = 0;
			jr = new JikkenRecorder(res, lvid, cc, rm.cfg, rm, this, isSub);
//			rm.jr = jr;
			return jr.getPageType();
		}
		/*
		private int getRtmpPageType(string res, bool isSub, out RtmpRecorder rr, CookieContainer cc) {
			rr = new RtmpRecorder(res, lvid, cc, rm.cfg, rm, this, isSub);
			return rr.getPageType();
		}
		*/
	}
}
