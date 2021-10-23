/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/10/30
 * Time: 20:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using namaichi.info;
using SuperSocket.ClientEngine;

namespace namaichi.rec
{
	/// <summary>
	/// Description of RtmpRecorder.
	/// </summary>
	public class RtmpRecorder
	{
		//private string getPlayerStatusRes;
		private string lvid;
		private CookieContainer container;
//		/private config.config cfg;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private string recFolderFile;
		private IRecorderProcess wr;
		private long openTime;
		
		private DateTime lastGetPlayerStatusAccessTime = DateTime.MinValue;
		private bool isSub;
		private bool isTimeshift;
		public int retryMode = 0;//0-retry 1-stop 2-endProgram
		
		private Process rtmpdumpP;
		private Process ffmpegP;
		
		private DateTime lastConnectTime = DateTime.MinValue;
		public bool isEndProgram = false;
		private DateTime lastEndProgramCheckTime = DateTime.Now;
		public int subNtiGroupNum = 0;
		private int afterConvertMode;
		private int tsRecordIndex = 0;
		private int tsRecordNum = 0;
		
		private string rtmpUrl;
		//private string que;
		private string ticket;
		
		public List<string> fileNameList = null;
		
		public RtmpRecorder(string lvid, CookieContainer container, 
				RecordingManager rm, RecordFromUrl rfu, bool isSub,
				string[] recFolderFile, IRecorderProcess wr, long openTime) {
//			this.getPlayerStatusRes = getPlayerStatusRes;
			this.lvid = lvid;
			this.container = container;
//			this.cfg = cfg;
			this.rm = rm;
			this.rfu = rfu;
			this.isSub = isSub;
			this.wr = wr;
			this.openTime = openTime;
			this.recFolderFile = recFolderFile[1];
			rm.isTitleBarInfo = bool.Parse(rm.cfg.get("IstitlebarInfo"));
			afterConvertMode = int.Parse(rm.cfg.get("afterConvertMode"));
		}
		public void record(string rtmp2Url, string quality) {

			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			util.debugWriteLine("rtmp recorder" + util.getMainSubStr(isSub, true));
			var _m = (rfu.isPlayOnlyMode) ? "視聴" : "録画";
			if (wr.ri.si.isTimeShift) {
				rm.form.addLogText("タイムシフトの" + _m + "を開始します");
			} else {
				if (isSub) {
					rm.form.addLogText(_m + "をスタンバイします(サブ)");
				} else
					rm.form.addLogText(_m + "を開始します(" + (quality == null ? "" : ("画質:" + quality + " ")) + "メイン)");
			}
			if (rtmp2Url != null) rtmpUrl = rtmp2Url;
			
			var convertList = new List<string>();
			var isFirst = true;
			var isFailedRec = false;
			try {
				//var sleepSec = 1;
				while (rm.rfu == rfu && retryMode == 0) {
					util.debugWriteLine("rtmp　録画" + ((isFirst) ? "開始" : "再開"));
					
					if (DateTime.Now < lastConnectTime + TimeSpan.FromSeconds(5)) {
						//Thread.Sleep(sleepSec * 10000);
						Thread.Sleep(5000);
						//if (sleepSec < 6) sleepSec++;
						continue;
					}
					lastConnectTime = DateTime.Now;
					
					var rtmpdumpArg = getProcessArgs(rtmp2Url != null, isFirst);
					if (rtmpdumpArg == "end") {
						isEndProgram = true;
						break;
					} else if (rtmpdumpArg == "no") {
						rm.form.addLogText("RTMPデータが見つかりませんでした");
						return;
					}
					if (rtmpdumpArg == null) continue;
					
					if (rfu.isPlayOnlyMode) {
						while(rm.rfu == rfu && retryMode == 0) {
							Thread.Sleep(1000);
						}
						return;
					}
					
					if (isTimeshift) {
						timeshiftRecord(rtmpdumpArg);
						return;
					}
					
					if (!rfu.isPlayOnlyMode)
						getProcess(out rtmpdumpP, out ffmpegP, rtmpdumpArg);
					
					if (!isSub) {
						if (!isFirst && !rfu.isPlayOnlyMode && !isFailedRec) wr.resetCommentFile();
						isFirst = false;
						isFailedRec = false;
						
						Task.Run(() => errorReadProcess(rtmpdumpP));
						
						testDebugWriteLine("rtmpdump　待機");
						while(rm.rfu == rfu && retryMode == 0 && !rtmpdumpP.HasExited) {
							if (!rfu.isPlayOnlyMode && rtmpdumpP.WaitForExit(1000)) break;
							if (rfu.isPlayOnlyMode) Thread.Sleep(1000);
						}
						testDebugWriteLine("rtmpdump　終了準備　" + retryMode);
						
						util.debugWriteLine("rtmp Process loop end");
						
						if (rm.rfu != rfu || retryMode == 1) {
							util.debugWriteLine("retrymode " + retryMode);
							stopRecording();
						} else {
							//end program
							while (rm.rfu == rfu && !rtmpdumpP.HasExited) {
								Thread.Sleep(1000);
							}
						}
						testDebugWriteLine("rtmpdump　終了");
						
						
						try {
							var f = new FileInfo(util.getOkSJisOut(recFolderFile) + ".flv");
							if (f != null && f.Exists && f.Length == 0) {
								util.debugWriteLine("rtmp delete " + recFolderFile);
								//File.Delete(f.FullName + ".flv");
								File.Delete(f.FullName + "");
								f = new FileInfo(util.getOkSJisOut(recFolderFile) + ".xml");
								if (f != null && f.Exists)
								    File.Delete(f.FullName);
								
								isFailedRec = true;
							} else {
	//							if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
	//							   		(afterConvertMode != "0" && afterConvertMode != "4")) {
								if (afterConvertMode > 0) {
									convertList.Add(recFolderFile + ".flv");
								}
								//recFolderFile = wr.getRecFilePath()[1];
								recFolderFile = util.incrementRecFolderFile(recFolderFile);//wr.getRecFilePath()[1];
							}
						} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
							
						}
						
						if (rm.rfu != rfu || retryMode != 0) break;
						wr.reConnect();
						
					} else {
						var rtmpdumpTask = 
								Task.Run(() => rtmpdumpReadFFmpegWriteProcess(rtmpdumpP, ffmpegP));
	//					var ffmpegTask = Task.Run(() => ffmpegReadProcess(rtmpdumpP));
						
						var isContinue = false;
						while (rm.rfu == rfu && retryMode == 0 && !rtmpdumpP.HasExited) {
	//						if (taskEnd(rtmpdumpTask) && taskEnd(ffmpegTask)) {
							if (taskEnd(rtmpdumpTask)) {
								isContinue = true;
							    break;
							}
	//						if (taskEnd(rtmpdumpTask) || taskEnd(ffmpegTask)) {
							if (taskEnd(rtmpdumpTask)) {
	//							stopRecording();
	//							break;
							}
							
							
							Thread.Sleep(1000);
						}
						util.debugWriteLine("rtmp rec end isContinue " + isContinue + "(サブ)");
						if (isContinue || retryMode == 0) {
							Thread.Sleep(1000);
							continue;
						}
						
						util.debugWriteLine("rtmp rec go retryMode " + retryMode + "(サブ)");
						if (rm.rfu != rfu || retryMode == 1) {
							stopRecording();
						}
						if (retryMode == 2) {
							//end program
							util.debugWriteLine("rtmp endprogram retryMode " + retryMode);
							while (rm.rfu == rfu) {
								if ((rtmpdumpP == null || rtmpdumpP.HasExited) && (ffmpegP == null || ffmpegP.HasExited)) break;
								Thread.Sleep(1000);
							}
						}
						util.debugWriteLine("rtmp rec end");
						if (rm.rfu != rfu || retryMode != 0) break;
					}
				}
			} catch (Exception e) {
				util.debugWriteLine("rtmp total error " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				testDebugWriteLine("rtmpエラー " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			testDebugWriteLine("rtmp録画終了");
			
			//convert
			foreach(var f in convertList) {
				var tf = new ThroughFFMpeg(rm);
				tf.start(f, true);
			}
		}
		private string getProcessArgs(bool isRtmp2, bool isFirst) {
			testDebugWriteLine("getProcessArgs");
			if (isRtmp2) {
				if (!isFirst && ((WebSocketRecorder)wr).isEndedProgram())
					return "end";
				var ret = "-vr " + rtmpUrl + " ";
				rm.hlsUrl = ret;
				ret += "-o \"" + util.getOkSJisOut(recFolderFile) + ".flv\"";
				util.debugWriteLine("getProcessArgs rtmp2Url exist " + ret);
				
				
				return ret;
			}
			
			var url = "https://live.nicovideo.jp/api/getplayerstatus/" + lvid;
			while (rm.rfu == rfu && retryMode == 0
			      ) {
				var res = util.getPageSource(url, container, null, false, 3000);
				util.debugWriteLine(res + util.getMainSubStr(isSub, true));
				if (res == null) {
					Task.Run(() => isEndedProgram());
					Thread.Sleep(3000);
					testDebugWriteLine("getplayerstatus取得失敗");
					continue;
				}
				if (res.IndexOf("<code>notlogin</code>") > -1) {
					var c = new CookieGetter(rm.cfg);
					var t = c.getHtml5RecordCookie(rfu.url);
					t.Wait();
					if (t.Result != null && t.Result[0] != null)
						container = t.Result[0];
					testDebugWriteLine("getplayerstatus not login");
					continue;
				}
				var pageType = util.getPageTypeRtmp(res, ref isTimeshift, isSub);
				testDebugWriteLine("getplayerstatus　取得 " + pageType);
				if (pageType == 1) {
					Thread.Sleep(90000);
					continue;
				}
				if (!wr.ri.si.isTimeShift && (pageType == 7 || pageType == 2)) {
					retryMode = 2;
					//test
					rm.form.addLogText(pageType + " " + res);
					return "end";
				}
				if (pageType == 2 || pageType == 3) {
					return "no";
				}
				if (pageType != 0 && pageType != 7) {
					Thread.Sleep(3000);
					continue;
				}
				var xml = new XmlDocument();
				xml.LoadXml(res);
				
//				util.debugWriteLine(container.GetCookieHeader(new Uri(url)));
				var type = util.getRegGroup(res, "<provider_type>(.+?)</provider_type>");
//				string rtmpurl = null, ticket = null;
				getTicketUrl(out rtmpUrl, out ticket);
				if (ticket == null) {
					Thread.Sleep(3000);
					continue;
				}
				 
				var arg = getArgFromRes(xml, pageType, type, ticket, rtmpUrl);
				if (arg.IndexOf("?invalid") > -1) {
					Thread.Sleep(60000);
					continue;
				}
				
				testDebugWriteLine("rtmpdump 引数取得 " + ((arg==null) ? "失敗" : "成功"));
				if (arg == null) continue;
				return arg;
			}
			
			Thread.Sleep(3000);
			return null;
		}
		private void getTicketUrl(out string url, out string ticket) {
			var edgeurl = "https://watch.live.nicovideo.jp/api/getedgestatus?v=" + lvid;
//			util.debugWriteLine(container.GetCookieHeader(new Uri(edgeurl)));
			var res = util.getPageSource(edgeurl, container, null, false, 5000);
			if (res == null) {
				url = null;
				ticket = null;
				return;
			}
			url = util.getRegGroup(res, "<url>(.+?)</url>");
			ticket = util.getRegGroup(res, "<ticket>(.+?)</ticket>");
//			int i = 0;
		}
		private string getArgFromRes(XmlDocument xml, int pageType, string type, string ticket, string url) {
			if (pageType == 0) {
				if (type == "official") {
					var _ticket = xml.SelectSingleNode("/getplayerstatus/tickets");
//					string ticket = null;
					if (_ticket != null) {
						var name = _ticket.ChildNodes[0].Attributes["name"];
						if (name != null) {
							string arg = null;
							if (name.Value.IndexOf("@") > -1) {
								ticket = _ticket.ChildNodes[0].InnerText;
								arg = "-vr \"rtmp://nlakmjpk.live.nicovideo.jp:1935/live/" + name.Value + "?" + ticket + "\"";
							} else {
								if (_ticket.ChildNodes.Count == 10) ticket = _ticket.ChildNodes[0].InnerText;
								else ticket = _ticket.ChildNodes[_ticket.ChildNodes.Count - 1].InnerText;
								arg = "-vr \"rtmp://smilevideo.fc.llnwd.net:1935/smilevideo/s_" + lvid + "?" + ticket + "\"";
							}
							if (ticket == null) {
								Thread.Sleep(3000);
								return null;
							}
							if (!isSub) rm.hlsUrl = arg;
							if (!isSub && !rfu.isPlayOnlyMode) arg += " -o \"" + util.getOkSJisOut(recFolderFile) + ".flv\"";
							util.debugWriteLine(arg + util.getMainSubStr(isSub, true));
							return arg;
						}
					}
					Thread.Sleep(3000);
					return null;
					
				} else {
					var _contentsUrl = xml.SelectSingleNode("/getplayerstatus/stream/contents_list/contents");
					var contentsUrl = (_contentsUrl == null) ? null : _contentsUrl.InnerText.Substring(5);
					/*
					var _rtmpUrl = xml.SelectSingleNode("/getplayerstatus/rtmp/url");
					var rtmpUrl = (_rtmpUrl == null) ? null : _rtmpUrl.InnerText;
					var _ticket = xml.SelectSingleNode("/getplayerstatus/rtmp/ticket");
					var ticket = (_ticket == null) ? null : _ticket.InnerText;
					*/
					util.debugWriteLine(type + " contentsUrl " + contentsUrl + " rtmpUrl " + rtmpUrl + " ticket " + ticket + util.getMainSubStr(isSub, true));
					var arg = "-vr " + rtmpUrl + "/" + lvid + " -N " + contentsUrl + " -C S:" + ticket;
					if (!isSub) rm.hlsUrl = arg;
					
					if (!isSub && !rfu.isPlayOnlyMode) arg += " -o \"" + util.getOkSJisOut(recFolderFile) + ".flv\"";
					util.debugWriteLine(arg + util.getMainSubStr(isSub, true));
					if (contentsUrl == null || rtmpUrl == null || ticket == null) {
						Thread.Sleep(3000);
						return null;
					}
					//rtmpdump エラー
					//derKakkoKari\namaichi\bin\Debug\rec/武田庵路/武田庵路_co2760796(武田食堂)_lv316893954(【世界名作RPG劇場】LIVE A ﾖVI˩ 最終章 中世編【Part8】)_1.flv"(
					//˩˩˩˩˩˩˩˩
					//˩
					return arg;
				}
			} else {
				//timeshift
				var isPremium = xml.SelectSingleNode("/getplayerstatus/user/is_premium").InnerText == "1";
				if (type == "official" || type == "channel") {
					/*
					var _url = xml.SelectSingleNode("/getplayerstatus/rtmp/url");
					rtmpUrl = (_url == null) ? null : _url.InnerText;
					var _ticket = xml.SelectSingleNode("/getplayerstatus/rtmp/ticket");
					ticket = (_ticket == null) ? null : _ticket.InnerText;
					*/
					var que = xml.SelectSingleNode("/getplayerstatus/stream/quesheet");
					if (rtmpUrl == null || ticket == null || que == null) {
						Thread.Sleep(3000);
						return null;
					}
					
					var play = getPlay(que, isPremium);
					if (play == null) {
						Thread.Sleep(3000);
						return null;
					}
					var publishList = getPublishList(que, play);
					if (publishList.Count == 0) return "no";
					
					//var quePosTimeList = getQuePosTimeList(que, play);
					//var startRecVpos =　(int.Parse(xml.SelectSingleNode("/getplayerstatus/stream/open_time").InnerText) -
					//                    int.Parse(xml.SelectSingleNode("/getplayerstatus/stream/start_time").InnerText)) * 100;
					
					var arg = "";
					for (var i = 0; i < publishList.Count; i++) {
						var a = publishList[i];
//						string _out = (arg == "") ? util.getOkSJisOut(recFolderFile[1]) : wr.getRecFilePath(0)[1];
						if (arg != "") arg += "$";
						var app = util.getRegGroup(rtmpUrl, "(fileorigin.+)");
						//var _a = (true && quePosTimeList[i] < startRecVpos) ? "" : (" -A " + (startRecVpos - quePosTimeList[i]));
						var _a = "";
						
						var _y = a.IndexOf(".flv") > -1 ? "/mp4:" : " -y mp4:";
						//_y = " -y mp4:";
						
						arg += "-r " + rtmpUrl + _y + a + " -a " + app + " -p http://live.nicovideo.jp/watch/" + lvid + " -s \"http://live.nicovideo.jp/nicoliveplayer.swf\" -f \"WIN 29,0,0,113\" -t " + rtmpUrl + " -C S:" + ticket + _a + " -m 300 -o ";
						//arg += "-r " + url + " -y mp4:" + a + " -C S:" + ticket + " -o ";
						 
					}
					rm.hlsUrl = "timeshift";
					util.debugWriteLine(arg + util.getMainSubStr(isSub, true));
					return arg;
					
				} else {
					
					/*
					var _url = xml.SelectSingleNode("/getplayerstatus/rtmp/url");
					var __url = (_url == null) ? null : _url.InnerText;
					var _ticket = xml.SelectSingleNode("/getplayerstatus/rtmp/ticket");
					var __ticket = (_ticket == null) ? null : _ticket.InnerText;
					*/
					var que = xml.SelectSingleNode("/getplayerstatus/stream/quesheet");
					if (rtmpUrl == null || ticket == null || que == null) {
						Thread.Sleep(3000);
						return null;
					}
					
					var play = getPlay(que, isPremium);
					if (play == null) {
						Thread.Sleep(3000);
						return null;
					}
					var publishList = getPublishList(que, play);
					if (publishList.Count == 0) return "no";
					
					var arg = "";
					foreach (var a in publishList) {
						if (arg != "") arg += "$";
						arg += "-vr " + rtmpUrl + " -N " + a + " -C S:" + ticket + " -p http://live.nicovideo.jp/watch/" + lvid + " -s http://live.nicovideo.jp/nicoliveplayer.swf?180116154229 -f \"WIN 29,0,0,113\" " + " -o ";
					}
					rm.hlsUrl = "timeshift";
					util.debugWriteLine(arg + util.getMainSubStr(isSub, true));
					return arg;
					
				}
			}
		}
		private string getPlay(XmlNode ques, bool isPremium) {
			string defaultP = null;
			string premiumP = null;
			foreach (XmlNode q in ques.ChildNodes) {
				var qi = q.InnerText;
				if (qi.StartsWith("/play case")) {
					if (qi.IndexOf("default:rtmp:") > -1)
						defaultP = util.getRegGroup(qi, "default:rtmp:(.+?)[, $]");
					if (qi.IndexOf("premium:rtmp:") > -1)
						premiumP = util.getRegGroup(qi, "premium:rtmp:(.+?)[, $]");
				} else 
					if (qi.StartsWith("/play")) return util.getRegGroup(qi, "rtmp:(.+?)[, $]");
			}
			if (premiumP != null) return premiumP;
			else if (defaultP != null) return defaultP;
			return null;
		}
		private List<string> getPublishList(XmlNode que, string play) {
			var l = new List<string>();
			foreach (XmlNode q in que.ChildNodes) {
				var qi = q.InnerText;
				if (qi.StartsWith("/publish " + play))
					l.Add(util.getRegGroup(qi, "/publish " + play + " (.+)"));
			}
			return l;
		}
		private List<int> getQuePosTimeList(XmlNode que, string play) {
			var l = new List<int>();
			foreach (XmlNode q in que.ChildNodes) {
				var qi = q.InnerText;
				if (qi.StartsWith("/publish " + play))
					l.Add(int.Parse(q.Attributes["vpos"].Value));
			}
			return l;
		}
		/*
		public int getPageType(string res) {
//			var res = getPlayerStatusRes;
			if (res.IndexOf("status=\"ok\"") > -1 && res.IndexOf("<archive>0</archive>") > -1) {
				isTimeshift = false;
				return 0;
			}
			if (res.IndexOf("status=\"ok\"") > -1 && res.IndexOf("<archive>1</archive>") > -1) {
				isTimeshift = true;
				return 7;
			}
			else if (res.IndexOf("<code>require_community_member</code>") > -1) return 4;
			else if (res.IndexOf("<code>closed</code>") > -1) return 2;
			else if (res.IndexOf("<code>comingsoon</code>") > -1) return 5;
			else if (res.IndexOf("<code>notfound</code>") > -1) return 2;
			else if (res.IndexOf("<code>deletedbyuser</code>") > -1) return 2;
			else if (res.IndexOf("<code>deletedbyvisor</code>") > -1) return 2;
			else if (res.IndexOf("<code>violated</code>") > -1) return 2;
			else if (res.IndexOf("<code>usertimeshift</code>") > -1) return 2;
			else if (res.IndexOf("<code>tsarchive</code>") > -1) return 2;
			else if (res.IndexOf("<code>unknown_error</code>") > -1) return 5;
			else if (res.IndexOf("<code>timeshift_ticket_exhaust</code>") > -1) return 2;
			else if (res.IndexOf("<code>timeshiftfull</code>") > -1) return 1;
			else if (res.IndexOf("<code>maintenance</code>") > -1) return 5;
			else if (res.IndexOf("<code>noauth</code>") > -1) return 5;
			else if (res.IndexOf("<code>full</code>") > -1) return 1;
			else if (res.IndexOf("<code>block_now_count_overflow</code>") > -1) return 5;
			else if (res.IndexOf("<code>premium_only</code>") > -1) return 5;
			else if (res.IndexOf("<code>selected-country</code>") > -1) return 5;
			else if (res.IndexOf("<code>notlogin</code>") > -1) return 8;
			rm.form.addLogText(res + util.getMainSubStr(isSub, true));
			return 5;
		}
		*/
		private string[] getRecFolderFileInfo(string res, string type) {
			/*
			string host, group, title, communityNum, userId;
			host = group = title = communityNum = userId = null;
			if (type == "official") {
//				host = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
//				if (util.getRegGroup(data, "(\"socialGroup\".\\{\\},)") != null) host = "公式生放送";
				host = "公式生放送";
	//			if (host == null) host = "official";
				//group = util.getRegGroup(data, "\"supplier\"..\"name\".\"(.+?)\"");
//				group = "official";
				if (group == null) group = "official";
				title = util.getRegGroup(data, "<title>(.+?)</title>");
//				title = util.uniToOriginal(title);
//				communityNum = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
				if (communityNum == null) communityNum = "official";
				userId = "official";
				
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum + util.getMainSubStr(isSub, true));
				if (host == null || group == null || title == null || communityNum == null) return null;
				util.debugWriteLine(host + " " + group + " " + title + " " + communityNum + util.getMainSubStr(isSub, true));
			} else {
				bool isAPI = false;
				if (isAPI) {
//					var a = new System.Net.WebHeaderCollection();
//					var apiRes = util.getPageSource(url + "/programinfo", ref a, container);
				
				} else {
					var isCommunity = util.getRegGroup(data, "providerType\":\"(community)\",\"title\"") != null;
		//			host = util.getRegGroup(res, "provider......name.....(.*?)\\\\\"");
					//group = util.getRegGroup(data, "\"community\".+?\"name\".\"(.+?)\"");
		//			group = util.uniToOriginal(group);
		//			group = util.getRegGroup(res, "communityInfo.\".+?title.\"..\"(.+?).\"");
					host = util.getRegGroup(data, "<owner_name>(.+?)</owner_name>");
		//			System.out.println(group);
		//			host = util.uniToOriginal(host);
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\:\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\:\\\"(.*?)\\\"");
		//			title = util.getRegGroup(res, "\\\\\"programHeader\\\\\":\\{\\\\\"thumbnailUrl.+?\\\\\"title\\\\\":\\\\\"(.*?)\\\\\"");
					title = util.getRegGroup(data, "<title>(.+?)</title>");
		//			communityNum = util.getRegGroup(res, "socialGroup: \\{[\\s\\S]*registrationUrl: \"http://com.nicovideo.jp/motion/(.*?)\\?");
					communityNum = util.getRegGroup(data, "<default_community>(.+?)</default_community>");
		//			community = util.getRegGroup(res, "socialGroup\\:)");
					group = getCommunityName(communityNum);
					userId = util.getRegGroup(data, "\"broadcaster\"..\"id\".\"(.+?)\"");
					//userId = (isChannel) ? "channel" : (util.getRegGroup(data, "supplier\":{\"name\".+?pageUrl\":\"http://www.nicovideo.jp/user/(\\d+?)\""));
					util.debugWriteLine("userid " + userId + util.getMainSubStr(isSub, true));
		
					util.debugWriteLine("title " + title + util.getMainSubStr(isSub, true));
					util.debugWriteLine("community " + communityNum + util.getMainSubStr(isSub, true));
		//			community = util.getRegGroup(res, "socialGr(oup:)");
		//			title = util.getRegGroup(res, "\\\"programHeader\\\"\\:\\{\\\"thumbnailUrl\\\".+?\\\"title\\\"\\:\\\"(.*?)\\\"");
					//  ,\"programHeader\":{\"thumbnailUrl\":\"http:\/\/icon.nimg.jp\/community\/s\/123\/co1231728.jpg?1373210036\",\"title\":\"\u56F2\u7881\",\"provider
		//			title = util.uniToOriginal(title);
					
					util.debugWriteLine(host + " " + group + " " + title + " " + communityNum + " userid " + userId + util.getMainSubStr(isSub, true));
					if (host == null || group == null || title == null || communityNum == null || userId == null) return null;
				}
			}
			if (communityNum != null) rm.communityNum = communityNum;
			return new string[]{host, group, title, lvid, communityNum, userId};
			*/
			return null;
		}
		
		private void getProcess(out Process rtmpdumpP, out Process ffmpegP, string rtmpdumpArg) {
			testDebugWriteLine("rtmpdump起動");
			
			System.IO.Directory.SetCurrentDirectory(util.getJarPath()[0]);
			rtmpdumpP = ffmpegP = null;
			rtmpdumpP = new Process();
			var si = new ProcessStartInfo();
			si.FileName = (bool.Parse(rm.cfg.get("IsDefaultRtmpPath"))) ? 
				"rtmpdump.exe" : rm.cfg.get("rtmpPath");
			si.Arguments = rtmpdumpArg;
			si.UseShellExecute = false;
			si.CreateNoWindow = true;
			util.debugWriteLine("rtmp get process " + util.getMainSubStr(isSub, true));
			if (isSub) 
				si.RedirectStandardOutput = true;
			else si.RedirectStandardError = true;
			rtmpdumpP.StartInfo = si;
			rtmpdumpP.Start();
			
			if (isSub && false) {
				ffmpegP = new Process();
				var ffmpegSi = new ProcessStartInfo();
				ffmpegSi.FileName = "ffmpeg.exe";
				var ffmpegArg = "-i - -c mpegts -y pipe:1.ts";
				ffmpegSi.Arguments = ffmpegArg;
				ffmpegSi.RedirectStandardInput = true;
				ffmpegSi.RedirectStandardOutput = true;
				ffmpegSi.UseShellExecute = false;
				ffmpegSi.CreateNoWindow = true;
				ffmpegP.StartInfo = ffmpegSi;
				ffmpegP.Start();
			}
			EventHandler e = new EventHandler(appExitHandler);
			Application.ApplicationExit += e;
		}
		private void rtmpdumpReadFFmpegWriteProcess(
				Process rtmpdumpP, Process ffmpegP) {
			util.debugWriteLine("rtmpdumpReadFFmpegWriteProcess start");
			Stream o;
			try {
				Thread.Sleep(500);
				o = rtmpdumpP.StandardOutput.BaseStream;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return;
			}
//			var _is = ffmpegP.StandardInput.BaseStream;
			var b = new byte[100000000];
			while (!rtmpdumpP.HasExited && rfu == rm.rfu) {
				try {
					var i = o.Read(b, 0, b.Length);
//					if (isFirst) 
//					Debug.WriteLine("rtmpdump " + i);
//					if (rm.isPlayOnlyMode) continue;
					
					if (i == 0) {
						util.debugWriteLine("rtmpdump read i " + (" get 0"));
					}
					
					var bb = b.CloneRange(0, i);
					if (rfu.firstFlvData == null && bb.Length > 0) {
						rfu.firstFlvData = bb;
						util.debugWriteLine("rtmp set firstData len " + bb.Length);
					}
					var nti = new numTaskInfo(subNtiGroupNum, null, 1, null, 0, 0);
					nti.res = bb;
					rfu.subGotNumTaskInfo.Add(nti);
					
				} catch (Exception ee) {
					Debug.WriteLine("rtmpdump read exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			subNtiGroupNum++;
			util.debugWriteLine("rtmpdumpReadFFmpegWriteProcess end");
		}
		private void ffmpegReadProcess(Process ffmpegP) {
			util.debugWriteLine("ffmpegReadProcess start");
			Stream _os;
			try {
				_os = ffmpegP.StandardOutput.BaseStream;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return;
			}
			
			var resBuf = new List<byte>();
			var b = new byte[100000000];
			while (!ffmpegP.HasExited) {
				try {
					
					var i = _os.Read(b, 0, b.Length);
//					if (isFirst) 
//					Debug.WriteLine("ff " + i);
					var bb = b.CloneRange(0, i);

					var nti = new numTaskInfo(0, null, 1, null, 0, 0);
					nti.res = bb;
//					util.debugWriteLine("ffmpeg " + nti.res.Length + util.getMainSubStr(isSub, true));
					
//					if (DateTime.Now - nti.dt > TimeSpan.FromSeconds(2)) {
						rfu.subGotNumTaskInfo.Add(nti);
//					} 
						
					
				} catch (Exception ee) {
					Debug.WriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			util.debugWriteLine("ffmpegReadProcess end");
		}
		private void errorReadProcess(Process p) {
			Debug.WriteLine("error read start");
			testDebugWriteLine("rtmpdump読み取り開始");
			
			var isRecordLive = p.StartInfo.Arguments.StartsWith("-vr");
			
			string receivedData = null, lastReceivedData = null;
			Task.Run(() => {
				try {
					while (!p.HasExited) {
			         	Thread.Sleep(200000);
			         	if (receivedData == lastReceivedData) {
			         		_stopRecording(p);
			         		break;
			         	}
			         	lastReceivedData = receivedData;
					}
	         	} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	         	}
			});
			StreamReader _os;
			try {
				_os = p.StandardError;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				testDebugWriteLine("rtmpdump読み取りできませんでした");
				return;
			}
			while (!p.HasExited) {
				try {
					var i = _os.ReadLine();
//					if (i.Length == 0) break;
					if (i == null || i.Length == 0) continue;
					var isState = i.Length > 3 && 
							(i.Substring(i.Length - 2) == "%)" ||
							i.Substring(i.Length - 3) == "sec");
					if (isState) {
						rm.form.setRecordState(i + ((isRecordLive) ? "" : ("(" + (tsRecordIndex + 1) + "/" + tsRecordNum + ")")));
					} else rm.form.addLogText(i);
					
					receivedData = i;
					//if (i.IndexOf("Download complete") > -1) {
					//	rm.form.setRecordStateComplete();
					//}
					
					//ts nasi
//					if (i.IndexOf("Starting download at: 0.000 kB") > -1) 
//						
				} catch (Exception ee) {
					Debug.WriteLine("error read exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			Debug.WriteLine("error read end");
			testDebugWriteLine("rtmpdump読み取り終了");
		}
		private void timeshiftRecord(string rtmpdumpArg) {
//			Process rtmpdumpP, ffmpegP;
			var convertList = new List<string>();
			var argList = rtmpdumpArg.Split('$');
			tsRecordNum = argList.Length;
			setFileNameList(argList.Length, recFolderFile);
			try {
				for (tsRecordIndex = 0; tsRecordIndex < argList.Length; tsRecordIndex++) {
					if (rfu.tsRecNumArr != null && 
					    	Array.IndexOf(rfu.tsRecNumArr, tsRecordIndex + 1) == -1) continue;
					if (tsRecordIndex != 0) {
						var _args = getProcessArgs(false, false);
						if (_args != null) argList = _args.Split('$');
						//recFolderFile = wr.getRecFilePath();
						//recFolderFile = incrementRecFolderFile(recFolderFile);
						
					}
					recFolderFile = getTsRecordIndexRecFolderFile(recFolderFile, tsRecordIndex + 1);
					
					
					//var _arg = argList[tsRecordIndex] + "\"" + recFolderFile + ".flv\"";
					var _arg = argList[tsRecordIndex] + "\"" + fileNameList[tsRecordIndex] + ".flv\"";
					
					if (_arg.StartsWith("-r")) {
						makeTs(_arg);
					}
					getProcess(out rtmpdumpP, out ffmpegP, _arg);
					
					Task.Run(() => errorReadProcess(rtmpdumpP));
					
					while(rm.rfu == rfu && !rtmpdumpP.HasExited) {
						if (rtmpdumpP.WaitForExit(1000)) break;
					}
					util.debugWriteLine("rtmp Process loop end");
					
					if (rm.rfu != rfu) {
						stopRecording();
					}
					try {
//						if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
//						   		(afterConvertMode != "0" && afterConvertMode != "4")) {
						if (afterConvertMode > 0) {
							convertList.Add(recFolderFile + ".flv");
						}
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						
					}
					if (rm.rfu != rfu) break;
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			//convert
			foreach(var f in convertList) {
				var tf = new ThroughFFMpeg(rm);
				tf.start(f, true);
			}
			if (rm.rfu == rfu) isEndProgram = true;
			util.debugWriteLine("timeshift rtmp record end");
		}
		private bool taskEnd(Task t) {
			if (t == null) return true;
			return (t.IsCanceled ||
				t.IsCompleted || t.IsFaulted);
		}
		public void stopRecording() {
			util.debugWriteLine("rtmp rec stop recording" + util.getMainSubStr(isSub, true));
			_stopRecording(rtmpdumpP);
			_stopRecording(ffmpegP);
			//retryMode = 1;
		}
		private void _stopRecording(Process p) {
			try {
				util.debugWriteLine("stop recording rtmp p " + p + " p.hasexited= " + ((p == null) ? "" : p.HasExited.ToString()));
				
				if (p == null || p.HasExited) return;
				try {
					p.Kill();
				} catch (Exception eee) {
					util.debugWriteLine(eee.Message + eee.StackTrace + eee.Source + eee.TargetSite + util.getMainSubStr(isSub, true));
				}
			
				while(!p.HasExited) {
					System.Threading.Thread.Sleep(200);
				}
				util.debugWriteLine("destroy " + p.ExitCode + util.getMainSubStr(isSub, true));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void appExitHandler(object sender, EventArgs e) {
			stopRecording();
		}
		private bool isEndedProgram() {
			var isPass = (DateTime.Now - lastEndProgramCheckTime < TimeSpan.FromSeconds(5)); 
			if (isPass) return false;
			lastEndProgramCheckTime = DateTime.Now;
			
			isEndProgram = util.isEndedProgram(lvid, container, isSub);
			if (isEndProgram) retryMode = 2;
			return isEndProgram;
		}
		private void makeTs(string _arg) {
			util.debugWriteLine("make ts " + _arg);
			return;
			/*
			var start = DateTime.Now;
			var que = util.getRegGroup(_arg, "(/content.+?) ");
			//while (rm.rfu == rfu && DateTime.Now - start < TimeSpan.FromSeconds(10)) {
			while (rm.rfu == rfu) {
				//var cl = new RtmpClient(rtmpUrl, que, ticket, rm);
				//if (cl.makeTs()) break;
			}
			Thread.Sleep(5000);
			util.debugWriteLine("make ts end");
			*/
		}
		private string incrementRecFolderFile(string recFolderFile) {
			var r = new Regex("(\\d+)$");
			var m = r.Match(recFolderFile);
			if (m == null || m.Length <= 0) return wr.getRecFilePath()[1];
			var _new = (int.Parse(m.Groups[1].Value) + 1).ToString();
			return r.Replace(recFolderFile, _new);
//			return recFolderFile.Replace(m.Groups[0].Value, _new);
		}
		private string getTsRecordIndexRecFolderFile(string recFolderFile, int recordIndex) {
			var r = new Regex("(\\d+(\\(\\d+\\))*)$");
			var m = r.Match(recFolderFile);
			if (m == null || m.Length <= 0) 
				return wr.getRecFilePath()[1];
			//var _new = (int.Parse(m.Groups[1].Value) + 1).ToString();
			var num = m.Groups[1].ToString();
			var _new = recordIndex.ToString();
			
			var baseName = r.Replace(recFolderFile, _new);
			for (var i = 0; i < 10000; i++) {
				var name = baseName;
				if (i != 0) name += "(" + i + ")"; 
				if (!File.Exists(name + ".flv"))
					return name;
			}
			return wr.getRecFilePath()[1];
		}
		private void testDebugWriteLine(string s) {
			util.debugWriteLine("rtmp debug " + s);
			#if DEBUG
				//rm.form.addLogText(s);	
			#endif
		}
		private void setFileNameList(int listNum, string recFolderFile) {
			var ret = new List<string>();
			for (var j = 1; j < listNum + 1; j++) {
				var r = new Regex("(\\d+(\\(\\d+\\))*)$");
				var m = r.Match(recFolderFile);
				if (m == null || m.Length <= 0) {
					ret.Add(wr.getRecFilePath()[1]);
					continue;
				}
				//var _new = (int.Parse(m.Groups[1].Value) + 1).ToString();
				var num = m.Groups[1].ToString();
				var _new = j.ToString();
				
				var baseName = r.Replace(recFolderFile, _new);
				var isAdd = false;
				for (var i = 0; i < 10000; i++) {
					var name = baseName;
					if (i != 0) name += "(" + i + ")"; 
					if (!File.Exists(name + ".flv")) {
						ret.Add(name);
						isAdd = true;
						break;
					}
				}
				if (isAdd) continue;
				
				ret.Add(wr.getRecFilePath()[1]);
			}
			fileNameList = ret;
		}
		public void resetRtmpUrl(string url) {
			rtmpUrl = url;
		}
	}
}
