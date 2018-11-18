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
using namaichi.info;
using SuperSocket.ClientEngine;

namespace namaichi.rec
{
	/// <summary>
	/// Description of RtmpRecorder.
	/// </summary>
	public class RtmpRecorder
	{
		private string getPlayerStatusRes;
		private string lvid;
		private CookieContainer container;
		private config.config cfg;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private string[] recFolderFile;
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
			this.recFolderFile = recFolderFile;
			rm.isTitleBarInfo = bool.Parse(rm.cfg.get("IstitlebarInfo"));
		}
		public void record() {
			//endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
			util.debugWriteLine("rtmp recorder" + util.getMainSubStr(isSub, true));
			var _m = (rm.isPlayOnlyMode) ? "視聴" : "録画";
			if (wr.isTimeShift) {
				rm.form.addLogText("タイムシフトの" + _m + "を開始します");
			} else {
				if (isSub) {
					rm.form.addLogText(_m + "をスタンバイします(サブ)");
				} else
					rm.form.addLogText(_m + "を開始します(メイン)");
			}
			
			while (rm.rfu == rfu && retryMode == 0) {
				if (DateTime.Now < lastConnectTime + TimeSpan.FromSeconds(3)) {
					Thread.Sleep(500);
					continue;
				}
				lastConnectTime = DateTime.Now;
				
				var rtmpdumpArg = getRtmpDumpArgs();
				if (rtmpdumpArg == "end") {
					isEndProgram = true;
					return;
				}
				if (rtmpdumpArg == null) continue;
				
				getProcess(out rtmpdumpP, out ffmpegP, rtmpdumpArg);
				
				if (!isSub) {
					Task.Run(() => errorReadProcess(rtmpdumpP));
					
					while(rm.rfu == rfu && retryMode == 0) {
						if (!rm.isPlayOnlyMode && rtmpdumpP.WaitForExit(300)) break;
						if (rm.isPlayOnlyMode) Thread.Sleep(300);
					}
					if (rm.rfu != rfu || retryMode == 1) {
						util.debugWriteLine("retrymode " + retryMode);
						stopRecording();
					} else {
						//end program
						while (rm.rfu == rfu && !rtmpdumpP.HasExited) {
							Thread.Sleep(500);
						}
					}
					try {
						var f = new FileInfo(util.getOkSJisOut(recFolderFile[1]) + ".flv");
						if (f != null && f.Exists && f.Length == 0) 
							File.Delete(f.FullName + ".flv");
						else {
							if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
							   		int.Parse(rm.cfg.get("afterConvertMode")) != 3) {
								var tf = new ThroughFFMpeg(rm);
								tf.start(recFolderFile[1] + ".flv", true);
							}
							recFolderFile = wr.getRecFilePath(openTime);
						}
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						
					}
					rm.form.addLogText("");
					if (rm.rfu != rfu || retryMode != 0) return;
					
				} else {
					var rtmpdumpTask = 
							Task.Run(() => rtmpdumpReadFFmpegWriteProcess(rtmpdumpP, ffmpegP));
					var ffmpegTask = Task.Run(() => ffmpegReadProcess(rtmpdumpP));
					
					var isContinue = false;
					while (rm.rfu == rfu && retryMode == 0) {
						if (taskEnd(rtmpdumpTask) && taskEnd(ffmpegTask))
							isContinue = true;
						    break;
						if (taskEnd(rtmpdumpTask) || taskEnd(ffmpegTask)) {
//							stopRecording();
//							break;
						}
						Thread.Sleep(500);
					}
					if (isContinue) continue;
					
					util.debugWriteLine("rtmp rec go retryMode " + retryMode);
					if (rm.rfu != rfu || retryMode == 1)
						stopRecording();
					else {
						//end program
						util.debugWriteLine("rtmp endprogram retryMode " + retryMode);
						while (rm.rfu == rfu) {
							if ((rtmpdumpP == null || rtmpdumpP.HasExited) && (ffmpegP == null || ffmpegP.HasExited)) break;
							Thread.Sleep(500);
						}
					}
					util.debugWriteLine("rtmp rec end");
					if (rm.rfu != rfu || retryMode != 0) return;
				}
			}
		}
		private string getRtmpDumpArgs() {
			var url = "http://live.nicovideo.jp/api/getplayerstatus/" + lvid;
			while (rm.rfu == rfu && retryMode == 0
			      ) {
				var res = util.getPageSource(url, container, null, false, 3000);
				util.debugWriteLine(res + util.getMainSubStr(isSub, true));
				if (res == null) {
					Thread.Sleep(3000);
					continue;
				}
				var pageType = getPageType(res);
				if (pageType == 1) {
					Thread.Sleep(65000);
					continue;
				}
				if (!wr.isTimeShift && (pageType == 7 || pageType == 2)) {
					retryMode = 2;
					return "end";
				}
				if (pageType != 0) {
					if (pageType == 2)
					Thread.Sleep(3000);
					continue;
				}
				var xml = new System.Xml.XmlDocument();
				xml.LoadXml(res);
				
				var type = util.getRegGroup(res, "<provider_type>(.+?)</provider_type>");
				if (type == "official") {
					var _ticket = xml.SelectSingleNode("/getplayerstatus/tickets");
					string ticket = null;
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
								continue;
							}
							if (!isSub) rm.hlsUrl = arg;
							if (!isSub && !rm.isPlayOnlyMode) arg += " -o \"" + util.getOkSJisOut(recFolderFile[1]) + ".flv\"";
							util.debugWriteLine(arg + util.getMainSubStr(isSub, true));
							return arg;
						}
					}
					Thread.Sleep(3000);
					continue;
					
				} else {
					var _contentsUrl = xml.SelectSingleNode("/getplayerstatus/stream/contents_list/contents");
					var contentsUrl = (_contentsUrl == null) ? null : _contentsUrl.InnerText.Substring(5);
					var _rtmpUrl = xml.SelectSingleNode("/getplayerstatus/rtmp/url");
					var rtmpUrl = (_rtmpUrl == null) ? null : _rtmpUrl.InnerText;
					var _ticket = xml.SelectSingleNode("/getplayerstatus/rtmp/ticket");
					var ticket = (_ticket == null) ? null : _ticket.InnerText;
					util.debugWriteLine(type + " contentsUrl " + contentsUrl + " rtmpUrl " + rtmpUrl + " ticket " + ticket + util.getMainSubStr(isSub, true));
					var arg = "-vr " + rtmpUrl + "/" + lvid + " -N " + contentsUrl + " -C S:" + ticket;
					if (!isSub) rm.hlsUrl = arg;
					
					if (!isSub && !rm.isPlayOnlyMode) arg += " -o \"" + util.getOkSJisOut(recFolderFile[1]) + ".flv\"";
					util.debugWriteLine(arg + util.getMainSubStr(isSub, true));
					if (contentsUrl == null || rtmpUrl == null || ticket == null) {
						Thread.Sleep(3000);
						continue;
					}
					//rtmpdump エラー
					//derKakkoKari\namaichi\bin\Debug\rec/武田庵路/武田庵路_co2760796(武田食堂)_lv316893954(【世界名作RPG劇場】LIVE A ﾖVI˩ 最終章 中世編【Part8】)_1.flv"(
					//˩˩˩˩˩˩˩˩
					//˩
					return arg;
				}
			}
			Thread.Sleep(3000);
			return null;
		}
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
			else if (res.IndexOf("<code>tsarchive</code>") > -1) return 7;
			else if (res.IndexOf("<code>unknown_error</code>") > -1) return 5;
			else if (res.IndexOf("<code>timeshift_ticket_exhaust</code>") > -1) return 2;
			else if (res.IndexOf("<code>timeshiftfull</code>") > -1) return 1;
			else if (res.IndexOf("<code>maintenance</code>") > -1) return 5;
			else if (res.IndexOf("<code>noauth</code>") > -1) return 5;
			else if (res.IndexOf("<code>full</code>") > -1) return 1;
			else if (res.IndexOf("<code>block_now_count_overflow</code>") > -1) return 5;
			else if (res.IndexOf("<code>premium_only</code>") > -1) return 5;
			else if (res.IndexOf("<code>selected-country</code>") > -1) return 5;
			rm.form.addLogText(res + util.getMainSubStr(isSub, true));
			return 5;
		}
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
		private void h5rTekina() {
			
			/*
			recFolderFileInfo = null;
			string[] recFolderFile = null;
			var type = util.getRegGroup(res, "<provider_type>(.+?)</provider_type>");
			
			long openTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "<start_time>(\\d+)</start_time>"), out openTime))
					return 3;
//				var openTime = long.Parse(util.getRegGroup(data, "\"beginTimeMs\":(\\d+)"));
			openTime = openTime;
			long endTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "<end_time>(\\d+)</end_time>"), out endTime))
					return 3;				
			endTime = endTime;
			var programTime = util.getUnixToDatetime(endTime) - util.getUnixToDatetime(openTime);
			long releaseTime = 0;
			if (data == null || 
			    !long.TryParse(util.getRegGroup(data, "<start_time>(\\d+)</start_time>"), out releaseTime))
					return 3;				
			releaseTime = releaseTime;
			
			recFolderFileInfo = getRecFolderFileInfo(res, type);
			if (!isSub) {
				timeShiftConfig = null;
				if (!isLive) {
					if (rm.ri != null) timeShiftConfig = rm.ri.tsConfig;
					if (rm.argTsConfig != null) {
						timeShiftConfig = getReadyArgTsConfig(rm.argTsConfig, recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], releaseTime);
					} else {
						timeShiftConfig = getTimeShiftConfig(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, releaseTime);
						if (timeShiftConfig == null) return 2;
						
					}
				}
				
				if (!rm.isPlayOnlyMode) {
		//			util.debugWriteLine("rm.rfu " + rm.rfu.GetHashCode() + " rfu " + rfu.GetHashCode() + util.getMainSubStr(isSub, true));
		//			if (recFolderFile == null)
						recFolderFile = getRecFilePath(releaseTime);
					if (recFolderFile == null || recFolderFile[0] == null) {
						//パスが長すぎ
						rm.form.addLogText("パスに問題があります。 " + recFolderFile[1]);
						util.debugWriteLine("too long path? " + recFolderFile[1] + util.getMainSubStr(isSub, true));
						return 2;
					}
				} else recFolderFile = new string[]{"", "", ""};
					
				//display set
				var b = new RecordStateSetter(rm.form, rm, rfu, !isLive, true, recFolderFile, rm.isPlayOnlyMode);
				Task.Run(() => {
					b.set(data, type, recFolderFileInfo);
				});
				
				//hosoInfo
				if (rm.cfg.get("IshosoInfo") == "true" && !rm.isPlayOnlyMode)
					Task.Run(() => {b.writeHosoInfo();});
					
				
				util.debugWriteLine("form disposed" + rm.form.IsDisposed + util.getMainSubStr(isSub, true));
				util.debugWriteLine("recfolderfile test " + recFolderFileInfo + util.getMainSubStr(isSub, true));
				
				var fileName = System.IO.Path.GetFileName(recFolderFile[1]);
				rm.form.setTitle(fileName);
			} else {
				recFolderFile = new string[]{"", "", ""};
			}
			*/
		}
		private void getProcess(out Process rtmpdumpP, out Process ffmpegP, string rtmpdumpArg) {
			rtmpdumpP = ffmpegP = null;
			rtmpdumpP = new Process();
			var si = new ProcessStartInfo();
			si.FileName = "rtmpdump.exe";
			si.Arguments = rtmpdumpArg;
			si.UseShellExecute = false;
			si.CreateNoWindow = true;
			if (isSub) 
				si.RedirectStandardOutput = true;
			else si.RedirectStandardError = true;
			rtmpdumpP.StartInfo = si;
			rtmpdumpP.Start();
			
			if (isSub) {
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
			var o = rtmpdumpP.StandardOutput.BaseStream;
			var _is = ffmpegP.StandardInput.BaseStream;
			var b = new byte[100000000];
			while (!rtmpdumpP.HasExited) {
				try {
					var i = o.Read(b, 0, b.Length);
//					if (isFirst) 
//					Debug.WriteLine("rtmpdump " + i);
					_is.Write(b, 0, i);
					_is.Flush();
				} catch (Exception ee) {
					Debug.WriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
		}
		private void ffmpegReadProcess(Process ffmpegP) {
			var _os = ffmpegP.StandardOutput.BaseStream;
			
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
			Debug.WriteLine("ffmpeg end");
		}
		private void errorReadProcess(Process p) {
			var _os = p.StandardError;
			while (!p.HasExited) {
				try {
					var i = _os.ReadLine();
					if (i == null) continue;
					if (i.Length > 2 && i.Substring(i.Length - 3) == "sec")
						rm.form.setRecordState(i);
					else rm.form.addLogText(i);
				} catch (Exception ee) {
					Debug.WriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			Debug.WriteLine("error end");
		}
		private bool taskEnd(Task t) {
			if (t == null) return true;
			return (t.IsCanceled ||
				t.IsCompleted || t.IsFaulted);
		}
		private void stopRecording() {
			util.debugWriteLine("rtmp rec stop recording" + util.getMainSubStr(isSub, true));
			_stopRecording(rtmpdumpP);
			_stopRecording(ffmpegP);
		}
		private void _stopRecording(Process p) {
			try {
				util.debugWriteLine("stop recording rtmp p " + p + " p.hasexited " + ((p == null) ? "" : p.HasExited.ToString()));
				
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
		
	}
}
