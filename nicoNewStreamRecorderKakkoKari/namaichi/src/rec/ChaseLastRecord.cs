/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2019/06/15
 * Time: 23:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Threading;
using namaichi.info;
using namaichi.utility;

namespace namaichi.rec
{
	/// <summary>
	/// Description of ChaseLastRecord.
	/// </summary>
	public class ChaseLastRecord
	{
		private string lvid = null;
		private CookieContainer container;
		private RecordingManager rm;
		private string[] recFolderFileInfo;
		private long openTime;
		//private Html5Recorder h5r;
		private RecordInfo ri;
		private TimeShiftConfig tsConfig;
		private RecordFromUrl rfu;
		private int lastSegmentNo = -1;
		
		public ChaseLastRecord(string lvid, 
				CookieContainer container, RecordingManager rm,
				string[] recFolderFileInfo, long openTime, 
				RecordInfo ri, TimeShiftConfig tsConfig, 
				RecordFromUrl rfu, int lastSegmentNo)
		{
			this.lvid = lvid;
			this.container = container;
			this.rm = rm;
			this.recFolderFileInfo = recFolderFileInfo;
			this.openTime = openTime;
			this.ri = ri;
			this.tsConfig = tsConfig;
			this.rfu = rfu;
			this.lastSegmentNo = lastSegmentNo;
		}
		public void rec(string recFolderFile) {
			util.debugWriteLine("chase last record rec start");
			var res = getRes();
			if (res == null) {
				if (int.Parse(rm.cfg.get("afterConvertMode")) > 0) {
					ffmpegThrough(recFolderFile);
				}
				return;
			}
			
			var wr = getWebsocketRecorder(res);
			if (wr == null) {
				if (int.Parse(rm.cfg.get("afterConvertMode")) > 0) {
					ffmpegThrough(recFolderFile);
				}
				return;
			}
			wr.start();
			
		}
		string getRes() {
			for (var i = 0; i < 12 && rm.rfu == rfu; i++) {
				Thread.Sleep(5000);
				//var timeout = i == 0 ? 3000 : 15000;
				//var timeout = 3000;
				//var _res = util.getPageSource("https://live.nicovideo.jp/watch/" + lvid, container, null, false, timeout);
				var url = "https://live.nicovideo.jp/watch/" + lvid;
				var h = util.getHeader(container, null, url);
				var _res = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false);
				if (_res == null) {
					util.debugWriteLine("chaseLastRecord getres getPage null " + i);
					continue;
				}
				
				var pageType = util.getPageType(_res);
				util.debugWriteLine("chase last record pagetype " + pageType);
					
				if (pageType == 7) {
					return _res;
				} else if (pageType == 9) {
					if (bool.Parse(rm.cfg.get("IsChaseReserveRec"))) {
						var ret = new Reservation(container, lvid).reserve();
						if (ret == "ok") {
							rm.form.addLogText("タイムシフトを予約しました");
						} else {
							rm.form.addLogText("タイムシフトの予約に失敗しました");
							rm.form.addLogText("録画を終了します");
							return null;
						}
					} else {
						rm.form.addLogText("この放送のタイムシフトの視聴には予約が必要です");
						rm.form.addLogText("録画を終了します");
						return null;
					}
				} else if (pageType == 2 || pageType == 3) {
					/*
					if (false && _res.IndexOf("<div style=\"font-weight:bold;\">※この放送はタイムシフトに対応しておりません。</div>") > -1) {
						util.debugWriteLine(_res);
						rm.form.addLogText("タイムシフトを取得できませんでした");
						rm.form.addLogText("録画を終了します");
						return null;
					}
					*/
					#if DEBUG
						rm.form.addLogText("pagetype " + pageType);
						url = "http://live.nicovideo.jp/api/getplayerstatus?v=" + lvid;
						util.debugWriteLine(util.getPageSource(url, container));
					#endif
					
					if (i != 11) continue;
					
					util.debugWriteLine(_res);
					rm.form.addLogText("タイムシフトを取得できませんでした");
					rm.form.addLogText("録画を終了します");
					return null;
				}
			}
			rm.form.addLogText("タイムシフトのページの取得に失敗しました");
			rm.form.addLogText("録画を終了します");
			return null;
		}
		WebSocketRecorder getWebsocketRecorder(string res) {
			try {
				var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
				if (data == null) return null;
				data = System.Web.HttpUtility.HtmlDecode(data);
				var type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
				var webSocketRecInfo = RecordInfo.getWebSocketInfo(data, false, false, true, rm.form, this.ri.isFmp4);
				if (webSocketRecInfo == null) return null;
				
				var segmentSaveType = rm.cfg.get("segmentSaveType");
				var lastFile = util.getLastTimeshiftFileName(
					recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, openTime, this.ri.isFmp4);
				util.debugWriteLine("timeshift lastfile " + lastFile);
				string[] lastFileTime = util.getLastTimeShiftFileTime(lastFile, segmentSaveType, this.ri.isFmp4);
				if (lastFileTime == null)
					util.debugWriteLine("timeshift lastfiletime " + 
					                    ((lastFileTime == null) ? "null" : string.Join(" ", lastFileTime)));
				var tsConfig = new TimeShiftConfig(1, int.Parse(lastFileTime[0]), int.Parse(lastFileTime[1]), int.Parse(lastFileTime[2]), 0, 0, 0, true, false, "", false, 0, false, false, 2, 0, false, false, this.tsConfig.isDeletePosTime, this.tsConfig.qualityRank);
				tsConfig.endTimeMode = this.tsConfig.endTimeMode;
				tsConfig.endTimeSeconds = this.tsConfig.endTimeSeconds;
				//tsConfig.lastSegmentNo = lastSegmentNo;
				var	recFolderFile = util.getRecFolderFilePath(recFolderFileInfo[0], recFolderFileInfo[1], recFolderFileInfo[2], recFolderFileInfo[3], recFolderFileInfo[4], recFolderFileInfo[5], rm.cfg, true, tsConfig, openTime, false, this.ri.isFmp4, false, rm.form);
				if (recFolderFile == null || recFolderFile[0] == null) {
					//パスが長すぎ
					rm.form.addLogText("パスに問題があります。 " + recFolderFile[1]);
					util.debugWriteLine("too long path? " + recFolderFile[1]);
					return null;
				}
				
				var userId = util.getRegGroup(res, "\"user\"\\:\\{\"user_id\"\\:(.+?),");
				var isPremium = res.IndexOf("\"member_status\":\"premium\"") > -1;
				
				//test
				var ri = this.ri.clone();
				ri.webSocketRecInfo = webSocketRecInfo;
				ri.userId = null;
				ri.isPremium = isPremium;
				ri.recFolderFile = recFolderFile;
				ri.timeShiftConfig = tsConfig;
				ri.si.isTimeShift = true;
				ri.isRtmp = ri.si.isRtmpOnlyPage = false;
				ri.isChase = false;
				ri.isRealtimeChase = false;
				
				//return new WebSocketRecorder(webSocketRecInfo, container, recFolderFile, rm, rm.rfu, h5r, openTime, true, lvid, tsConfig, null/*userId*/, isPremium, TimeSpan.MaxValue, type, openTime, false, false, false, false, false, null, 0);
				return new WebSocketRecorder(container, rm, rm.rfu, false, null, ri);
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		void ffmpegThrough(string recFolderFile) {
			if (recFolderFile == null) {
				rm.form.addLogText("FFmpeg処理中に録画ファイルが見つかりませんでした ");
				util.debugWriteLine("chaseLastRecord ffmpeg through null");
				return;
			}
			var tf = new ThroughFFMpeg(rm);
			tf.start(recFolderFile + ".ts", true);
		}
	}
}
