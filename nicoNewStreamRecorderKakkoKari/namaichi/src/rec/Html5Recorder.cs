/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/04/15
 * Time: 0:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using namaichi.config;
using namaichi.info;
using namaichi;
using namaichi.utility;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Html5Recorder.
	/// </summary>
	
		
	public class Html5Recorder
	{
		//public string url;
		private CookieContainer container;
		private RecordingManager rm;
		private RecordFromUrl rfu;

		public RecordInfo ri = null;
		public WebSocketRecorder wsr = null;
	
		public Html5Recorder(CookieContainer container, 
				RecordingManager rm, RecordFromUrl rfu)
		{
			
			this.container = container;
			this.rm = rm;
			this.rfu = rfu;
		}
		public int record(string res, bool isRtmp, int pageType, string url, string lvid, bool isTimeShift) {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			//var ret = html5Record(res, isRtmp, pageType).Result;
			var ret = html5Record(res, isRtmp, pageType, url, lvid, isTimeShift);
			util.debugWriteLine("html5 rec ret " + ret);
			return ret;
		}
		private int html5Record(string res, bool isRtmp, int pageType, string url, string lvid, bool isTimeShift) {
			//webSocketInfo 0-wsUrl 1-request
			//recFolderFileInfo host, group, title, lvid, communityNum
			//return 0-end stream 1-stop
			
			//this.url = url;
			
			while(rm.rfu == rfu) {
				var si = new StreamInfo(url, lvid, isTimeShift, false);
				si.set(res);
				if (!si.getTimeInfo()) return 3;
				
				if ((si.data == null && !si.isRtmpOnlyPage) || (pageType != 0 && pageType != 7)) {
					//processType 0-ok 1-retry 2-放送終了 3-その他の理由の終了
					var processType = processFromPageType(pageType);
					util.debugWriteLine("processType " + processType);
					if (processType == 2) return 3;
					
					System.Threading.Thread.Sleep(3000);
					res = getPageSourceFromNewCookie();
					continue;
				}
				
				var isChaseCheck = rm.form.isChaseChkBtn.Checked;
				if (isChaseCheck && (!si.isChasable || pageType != 0)) {
					rm.form.addLogText("追いかけ再生ができませんでした");
					return 2;
				}
				
				ri = new RecordInfo(si, pageType, isRtmp);
				ri.set(isChaseCheck, rm.cfg, rm.form);
				RecordLogInfo.recType = ri.si.isTimeShift ? (ri.isChase ? "chase" : "timeshift") : "realtime";
				
				if (!si.isRtmpOnlyPage && ri.webSocketRecInfo == null) return 1;
				if (!ri.setTimeShiftConfig(rm, isChaseCheck)) return 2;
				if (!ri.setRecFolderFile(rm)) return 2;
				
				//display set
				var rss = new RecordStateSetter(rm.form, false, rfu.isPlayOnlyMode, si.isRtmpOnlyPage, si.isReservation);
				Task.Run(() => {
				       	rss.set(si.data, si.recFolderFileInfo, res);
				});
				
				rss.setOutFileName(ri.recFolderFile[2], ri.timeShiftConfig);
				Task.Run(() => {
				       	rm.form.setTitle(ri.recFolderFile[1]);
				       	
				       	//hosoInfo
						if (rm.cfg.get("IshosoInfo") == "true" && !rfu.isPlayOnlyMode)
							rss.writeHosoInfo();
					});
				
				util.debugWriteLine("form disposed" + rm.form.IsDisposed);
				util.debugWriteLine("recfolderfile test " + si.recFolderFileInfo);
				
				wsr = new WebSocketRecorder(container, rm, rfu, true, rss, ri);
				rm.wsr = wsr;
				try {
					wsr.start();
					
					if (rm.cfg.get("fileNameType") == "10" && (ri.recFolderFile[1].IndexOf("{w}") > -1 || ri.recFolderFile[1].IndexOf("{c}") > -1))
						renameStatistics(rss);
					renameTitle();
					
					rm.wsr = null;
					if (wsr.isEndProgram) {
						if ((!si.isTimeShift || ri.isChase) && rss.isWrite)
							rss.writeEndTime(container, wsr.endTime);
						return 3;
					}
						
				} catch (Exception e) {
					rm.form.addLogText("録画中に予期せぬ問題が発生しました " + e.Message + e.StackTrace + e.Source + e.TargetSite);
					util.debugWriteLine("wsr start exception " + e.Message + e.StackTrace);
				}
				
				
				util.debugWriteLine(rm.rfu + " " + rfu + " " + (rm.rfu == rfu));
				if (rm.rfu != rfu || ri.isRtmp) break;
				
				res = getPageSourceFromNewCookie();
			}
			return 1;
		}
		private string getPageSourceFromNewCookie() {
			while (rm.rfu == rfu) {
				try {
					var _cg = new CookieGetter(rm.cfg);
					var _cgtask = _cg.getHtml5RecordCookie(ri.si.url);
					_cgtask.Wait();
					
					if (_cgtask == null || _cgtask.Result == null) {
						System.Threading.Thread.Sleep(3000);
						continue;
					}
					container = _cgtask.Result[0];
					return _cg.pageSource;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace);
					System.Threading.Thread.Sleep(3000);
				} 
			}
			return "";
		}
		/*
		public bool isAliveStream() {
			var a = new System.Net.WebHeaderCollection();
			string res = util.getPageSource(url, ref a, container);
				
//			string res = getPageSource(url);
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			data = System.Web.HttpUtility.HtmlDecode(data);
			string[] info = getWebSocketInfo(data);
			return (info == null) ? false : true; 
		}
		*/
		
		private int processFromPageType(int pageType) {
			//ret 0-ok 1-retry 2-放送終了 3-その他の理由の終了
			if (pageType == 0 || pageType == 7) {
				return 0;
			} else if (pageType == 1) {
				rm.form.addLogText("満員です。");
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
				
			} else if (pageType == 5) {
				rm.form.addLogText("接続エラー。10秒後リトライします。");
				if (bool.Parse(rm.cfg.get("Isretry"))) {
					System.Threading.Thread.Sleep(10000);
					return 1;
				} else {
					return 3;
				}
			} else if (pageType == 6) {
//				rm.form.addLogText("接続エラー。10秒後リトライします。");
				System.Threading.Thread.Sleep(3000);
				return 1;
				
			} else if (pageType == 4) {
				rm.form.addLogText("require_community_member");
				if (bool.Parse(rm.cfg.get("IsmessageBox")) && util.isShowWindow) {
					if (rm.form.IsDisposed) return 2;
					try {
						rm.form.formAction(() => {
			       			util.showMessageBoxCenterForm(rm.form, "コミュニティに入る必要があります：\nrequire_community_member/" + ri.si.lvid, "", MessageBoxButtons.OK, MessageBoxIcon.None);
						}, false);
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
				}
				if (bool.Parse(rm.cfg.get("IsfailExit")) && util.isShowWindow) {
					rm.rfu = null;
					try {
						rm.form.formAction(() => {
							try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
							}
						});
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
					
				}
				return 3;
				
			} else {
				var mes = "";
				if (pageType == 2) mes = "この放送は終了しています。";
				if (pageType == 3) mes = "この放送は終了しています。";
				rm.form.addLogText(mes);
				util.debugWriteLine("pagetype " + pageType + " end");
				
				if (bool.Parse(rm.cfg.get("IsdeleteExit")) && util.isShowWindow) {
					rm.rfu = null;
					try {
						rm.form.formAction(() => {
			       			try { rm.form.Close();} 
							catch (Exception e) {
		       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	       		}
						});
					} catch (Exception e) {
			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	}
					
				}
				return 2;
				//var nh5r = new NotHtml5Recorder(url, container, lvid, rm, this);
				//nh5r.record(res);
			}
			
		}
		
		private TimeShiftConfig getTimeShiftConfig(string host, 
				string group, string title, string lvId, string communityNum, 
				string userId, config.config cfg, long startTime, bool isChase, 
				long openTime) {
			var segmentSaveType = cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, cfg, startTime, ri.isFmp4);
			util.debugWriteLine("timeshift lastfile " + lastFile);
			string[] lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType, ri.isFmp4);
			if (lastFileTime == null)
				util.debugWriteLine("timeshift lastfiletime " + 
				                    ((lastFileTime == null) ? "null" : string.Join(" ", lastFileTime)));
			
			try {
				var prepTime = (int)(startTime - openTime);
				var o = new TimeShiftOptionForm(lastFileTime, segmentSaveType, rm.cfg, isChase, prepTime, ri.isFmp4, ri.si.isChannelPlus);
				
				
				try {
					rm.form.formAction(() => {
	       		       	try {
			        	    o.ShowDialog(rm.form);
	       		       	} catch (Exception e) {
	       		       		util.debugWriteLine("timeshift option form invoke " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       		       	}
					}, false);
				} catch (Exception e) {
					util.debugWriteLine("timeshift option form invoke try " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				}
				
				//if (o.ret == null) return null;
				return o.ret;
	        } catch (Exception ee) {
        		util.debugWriteLine(ee.Message + " " + ee.StackTrace);
	        }
			return null;
		}
		
		private TimeShiftConfig getReadyArgTsConfig(
				TimeShiftConfig _tsConfig, string host, 
				string group, string title, string lvId, 
				string communityNum, string userId, 
				long _openTime, int prepTime) {
			
			var segmentSaveType = rm.cfg.get("segmentSaveType");
			var lastFile = util.getLastTimeshiftFileName(host,
					group, title, lvId, communityNum, userId, rm.cfg, _openTime, ri.isFmp4);
			util.debugWriteLine("timeshift lastfile " + lastFile + " host " + host + " title " + title);
			
			var lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType, ri.isFmp4);
			if (lastFileTime == null) {
				_tsConfig.timeType = 0;
			} else {
				if (_tsConfig.timeType == 1)
					_tsConfig.timeSeconds = 
						int.Parse(lastFileTime[0]) * 3600 + 
						int.Parse(lastFileTime[1]) * 60 +
						int.Parse(lastFileTime[2]);
			}
			if (_tsConfig.timeType == 0) _tsConfig.isContinueConcat = false;
			util.debugWriteLine("ready arg ts iscontinueconcat " + _tsConfig.isContinueConcat + " startType " + _tsConfig.timeType + " lastfiletime " + lastFileTime + " lastfile " + lastFile);
			
			if (_tsConfig.isOpenTimeBaseStartArg) _tsConfig.timeSeconds += prepTime;
			if (_tsConfig.isOpenTimeBaseEndArg) _tsConfig.endTimeSeconds += prepTime;
			return _tsConfig;
		}
		
		
		private void renameStatistics(RecordStateSetter rss) {
			try {
				wsr.setRealTimeStatistics();
				
				rss.renameStatistics(wsr.visitCount.Replace("-", ""), wsr.commentCount.Replace("-", ""));
			} catch (Exception e) {
				rm.form.addLogText("ファイル名の変更中に問題が発生しました。" + e.Message + e.Source + e.StackTrace);
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void renameTitle() {
			var _si = new StreamInfo(ri.si.url, ri.si.lvid, ri.si.isTimeShift, false);
			//var res = util.getPageSource(ri.si.url, container);
			var h = util.getHeader(container, null, ri.si.url);
			var res = new Curl().getStr(ri.si.url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false);
			for (var i = 0; i < 10; i++) {
				if (res != null) break;
				Thread.Sleep(1000);
				new Curl().getStr(ri.si.url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false);
			}
			if (res == null || res.IndexOf("<script id=\"embedded-data\"") == -1) {
				rm.form.addLogText("終了処理後に放送タイトルの変更をチェックできませんでした:" + res);
				return;
			}
			_si.set(res);
			if (_si.recFolderFileInfo[2] == ri.si.recFolderFileInfo[2]) return;
			foreach (var f in Directory.GetFiles(ri.recFolderFile[0])) {
				var newF = f.Replace(ri.si.recFolderFileInfo[2], _si.recFolderFileInfo[2]);
				try {
					if (f.IndexOf(ri.si.lvid) > -1 && f.IndexOf(ri.si.recFolderFileInfo[2]) > -1) {
						if (!File.Exists(newF)) {
							File.Move(f, newF);
							continue;
						}
						rm.form.addLogText("ファイル名を" + f + "から" + newF + "へ変更できませんでした");
					}
				} catch (Exception e) {
					rm.form.addLogText("ファイル名を" + f + "から" + newF + "へ変更できませんでした");
					rm.form.addLogText(e.Message + e.Source + e.StackTrace);
				}
			}
		}
	}
}

