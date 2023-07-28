/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2022/05/17
 * Time: 17:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using namaichi.info;
using namaichi.utility;
using Newtonsoft.Json;

namespace namaichi.rec
{
	/// <summary>
	/// Description of TimeShiftCommentGetter_chPlus.
	/// </summary>
	public class TimeShiftCommentGetter_chPlus : ITimeShiftCommentGetter
	{
		MainForm form;
		RecordingManager rm;
		RecordFromUrl rfu;
		string groupId = null;
		string token = null;
		string id = null;
		Curl getMsgCurl = new Curl();
		
		bool isGetXml = true;
		bool isGetCommentXmlInfo = false;
		private bool isConvertSpace;
		private string commentConvertStr = null;
		private bool isNormalizeComment;
		private string vpRes = null;
		private RedirectLogin rl = null;
		
		bool isSave = true;
		public TimeShiftCommentGetter_chPlus(MainForm form, 
				RecordingManager rm, RecordFromUrl rfu,
				RecordInfo ri,
				string id, string groupId, string vpRes,
				bool isLog, TimeShiftConfig tsConfig, 
				IRecorderProcess rp, RedirectLogin rl)
		{
			/*
			(string uri, string thread,
				string uriStore, string threadStore,        
				string userId, RecordingManager rm, 
				RecordFromUrl rfu, MainForm form,
				RecordInfo ri,
				string lvid, CookieContainer container, 
				IRecorderProcess rp, 
				bool isRtmp,
				RtmpRecorder rr, 
				string roomName, TimeShiftConfig tsConfig,
				string lastRealTimeComment = null, bool isLog = true,
				bool isStore = false, string _uri = null, string _thread = null
				*/
				
			this.form = form;
			this.rm = rm;
			this.rfu = rfu;
			this.groupId = groupId;
			//this.token = token;
			this.id = id;
			this.vpRes = vpRes;
			this.isLog = isLog;
			this.tsConfig = tsConfig;
			this.recFolderFile = ri.recFolderFile[1];
			this.rp = rp;
			this.isVposStartTime = tsConfig.isVposStartTime;
			this.ri = ri;
			this.rl = rl;
			
			this.isGetXml = bool.Parse(rm.cfg.get("IsgetcommentXml"));
			this.isGetCommentXmlInfo = bool.Parse(rm.cfg.get("IsgetcommentXmlInfo"));
			isConvertSpace = bool.Parse(rm.cfg.get("IsCommentConvertSpace"));
			commentConvertStr = rm.cfg.get("commentConvertStr");
			isNormalizeComment = bool.Parse(rm.cfg.get("IsNormalizeComment"));
		}
		override public void save() {
			if (!bool.Parse(rm.cfg.get("IsgetComment"))) {
				isEnd = true;
				return;
			}
			/*
			while (true) {
		        if (rp.firstSegmentSecond != -1 || !rp.IsRetry) break;
				Thread.Sleep(500);
			}
			*/
			
			token = getToken();
			if (token == null) {
				util.debugWriteLine("tscg chPlus token no");
				form.addLogText("コメントのtokenが取得できませんでした");
				return;
			}
			var _baseDt = vpRes.IndexOf("\"live_started_at\":null") > -1 ?
					util.getRegGroup(vpRes, "\"released_at\":\"(.+?)\"") :
					util.getRegGroup(vpRes, "\"live_started_at\":\"(.+?)\"");
			//var baseDt = _baseDt != null ? DateTime.Parse(_baseDt) :
			//2022-04-20 12:00:00
			if (_baseDt != null) _baseDt = _baseDt.Replace(" ", "T") + ".000Z";
			else _baseDt = "2022-02-01T00:00:00.000Z";
			var buf = new List<chPlusMsgInfo>();
			var errorCount = 0;
			var testMostMaxSentAt = DateTime.MinValue;
			while (rm.rfu == rfu && isRetry) {
				try {
					var l = getMessages(_baseDt);
					if (l == null) {
						util.debugWriteLine("tscg chPlus no " + _baseDt);
						Thread.Sleep(5000);
						continue;
					}
					buf.AddRange(l);
					if (l.Count < 110) break;
					_baseDt = l[l.Count - 1].sent_at;
					
					//test
					foreach (var b in l) {
						if (DateTime.Parse(b.sent_at) > testMostMaxSentAt) testMostMaxSentAt = DateTime.Parse(b.sent_at);
						else util.debugWriteLine("aaa");
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					Thread.Sleep(5000);
					errorCount++;
					if (errorCount > 5) return;
				}
			}
			try {
				var chatList = buf.Select(x => x.getChat()).ToList();
				var bufCount = buf.Count;
				foreach (var _chat in chatList) saveChat(_chat);
				endProcess();
				isEnd = true;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			util.debugWriteLine(3);
			
		}
		List<chPlusMsgInfo> getMessages(string dt) {
			try {
				//var b = "[{\"created_at\":\"2022-04-20T10:50:42.013Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"b92028d0-c097-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"ご来場のみなさま、こんばんは。\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":353,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:50:42.013Z\",\"updated_at\":\"2022-04-20T10:50:42.013Z\"},{\"created_at\":\"2022-04-20T10:50:51.398Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"beb83260-c097-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"本日はニコニコチャンネルプラスの生放送にお越しいただきありがとうございます。\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":363,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:50:51.398Z\",\"updated_at\":\"2022-04-20T10:50:51.398Z\"},{\"created_at\":\"2022-04-20T10:51:06.877Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"c7f21ad0-c097-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"このあとは「【生放送｜4月20日配信】なんでもいうことをきくラジオ　きくラジ！（近藤 唯・小澤亜李・嶺内ともみ」をお送り致します。\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":378,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:51:06.877Z\",\"updated_at\":\"2022-04-20T10:51:06.877Z\"},{\"created_at\":\"2022-04-20T10:51:22.561Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"d14b4b10-c097-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"それでは開演まで今しばらくお待ちください。\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":394,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:51:22.561Z\",\"updated_at\":\"2022-04-20T10:51:22.561Z\"},{\"created_at\":\"2022-04-20T10:51:36.039Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"d953df70-c097-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"きくラジ！番組公式Twitter\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":407,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:51:36.039Z\",\"updated_at\":\"2022-04-20T10:51:36.039Z\"},{\"created_at\":\"2022-04-20T10:51:44.429Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"de5415d0-c097-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"ハッシュタグ#なんちけ\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":416,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:51:44.429Z\",\"updated_at\":\"2022-04-20T10:51:44.429Z\"},{\"created_at\":\"2022-04-20T10:51:58.519Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"e6ba0c70-c097-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"チャンネル入会はこちらから!!\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":430,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T10:51:58.519Z\",\"updated_at\":\"2022-04-20T10:51:58.519Z\"},{\"created_at\":\"2022-04-20T10:58:21.490Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cafec920-c098-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"こんばんはー\",\"nickname\":\"おなす\",\"playback_time\":827,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T10:58:21.490Z\",\"updated_at\":\"2022-04-20T10:58:21.490Z\"},{\"created_at\":\"2022-04-20T10:58:27.865Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cecb8890-c098-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"入会しました！\",\"nickname\":\"てぃんご\",\"playback_time\":819,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T10:58:27.865Z\",\"updated_at\":\"2022-04-20T10:58:27.865Z\"},{\"created_at\":\"2022-04-20T10:58:52.711Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"dd9abb70-c098-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"楽しみ\",\"nickname\":\"ゲスト\",\"playback_time\":843,\"priority\":false,\"sender_id\":\"f18f8e30-a956-44e4-9446-1057bfc7e316\",\"sent_at\":\"2022-04-20T10:58:52.711Z\",\"updated_at\":\"2022-04-20T10:58:52.711Z\"},{\"created_at\":\"2022-04-20T10:59:15.772Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"eb598fc0-c098-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"こんばんはー、待機ー\",\"nickname\":\"kisa_ran\",\"playback_time\":867,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T10:59:15.772Z\",\"updated_at\":\"2022-04-20T10:59:15.772Z\"},{\"created_at\":\"2022-04-20T10:59:34.271Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"f66048f0-c098-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"秒で入会しました\",\"nickname\":\"おなす\",\"playback_time\":900,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T10:59:34.271Z\",\"updated_at\":\"2022-04-20T10:59:34.271Z\"},{\"created_at\":\"2022-04-20T11:01:35.781Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"3ecd3d50-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"お待たせいたしました、まもなく開演いたします。\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1007,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:01:35.781Z\",\"updated_at\":\"2022-04-20T11:01:35.781Z\"},{\"created_at\":\"2022-04-20T11:01:42.735Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"42f255f0-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"開演と同時に音声が入りますので音量にお気をつけください。\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1014,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:01:42.735Z\",\"updated_at\":\"2022-04-20T11:01:42.735Z\"},{\"created_at\":\"2022-04-20T11:01:46.415Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"4523dbf0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"88888\",\"nickname\":\"おなす\",\"playback_time\":1032,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:01:46.415Z\",\"updated_at\":\"2022-04-20T11:01:46.415Z\"},{\"created_at\":\"2022-04-20T11:01:50.825Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"47c4c590-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"お\",\"nickname\":\"kisa_ran\",\"playback_time\":1022,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:01:50.825Z\",\"updated_at\":\"2022-04-20T11:01:50.825Z\"},{\"created_at\":\"2022-04-20T11:01:57.428Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"4bb44f40-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"これは入会しないと見れないやつなのかな？\",\"nickname\":\"onion\",\"playback_time\":1030,\"priority\":false,\"sender_id\":\"95c68bfc-9a91-45df-90b5-cb28f5b297a3\",\"sent_at\":\"2022-04-20T11:01:57.428Z\",\"updated_at\":\"2022-04-20T11:01:57.428Z\"},{\"created_at\":\"2022-04-20T11:02:34.933Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"620f1e50-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"きこえた！\",\"nickname\":\"kisa_ran\",\"playback_time\":1066,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:02:34.933Z\",\"updated_at\":\"2022-04-20T11:02:34.933Z\"},{\"created_at\":\"2022-04-20T11:02:40.312Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6543e380-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"嶺内ともみ\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1072,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:02:40.312Z\",\"updated_at\":\"2022-04-20T11:02:40.312Z\"},{\"created_at\":\"2022-04-20T11:02:41.602Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6608ba20-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"はじまったああああ！！！\",\"nickname\":\"おなす\",\"playback_time\":1087,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:02:41.602Z\",\"updated_at\":\"2022-04-20T11:02:41.602Z\"},{\"created_at\":\"2022-04-20T11:02:42.663Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"66aa9f70-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"わー\",\"nickname\":\"ソーノ\",\"playback_time\":1075,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:02:42.663Z\",\"updated_at\":\"2022-04-20T11:02:42.663Z\"},{\"created_at\":\"2022-04-20T11:02:43.746Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"674fe020-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"小澤亜李\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1075,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:02:43.746Z\",\"updated_at\":\"2022-04-20T11:02:43.746Z\"},{\"created_at\":\"2022-04-20T11:02:47.608Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"699d2b80-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"こんばんは\",\"nickname\":\"あるまく\",\"playback_time\":1078,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:02:47.608Z\",\"updated_at\":\"2022-04-20T11:02:47.608Z\"},{\"created_at\":\"2022-04-20T11:02:48.806Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6a53f860-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"嶺内ともみ\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1080,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:02:48.806Z\",\"updated_at\":\"2022-04-20T11:02:48.806Z\"},{\"created_at\":\"2022-04-20T11:02:51.392Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6bde9000-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"こんばんはー\",\"nickname\":\"てぃんご\",\"playback_time\":1083,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:02:51.392Z\",\"updated_at\":\"2022-04-20T11:02:51.392Z\"},{\"created_at\":\"2022-04-20T11:02:52.283Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6c6684b0-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"こんばんはー！！\",\"nickname\":\"kisa_ran\",\"playback_time\":1084,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:02:52.283Z\",\"updated_at\":\"2022-04-20T11:02:52.283Z\"},{\"created_at\":\"2022-04-20T11:02:53.886Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6d5b1de0-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"近藤唯\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1085,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:02:53.886Z\",\"updated_at\":\"2022-04-20T11:02:53.886Z\"},{\"created_at\":\"2022-04-20T11:02:56.476Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6ee651c0-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"こんばんはー！\",\"nickname\":\"おなす\",\"playback_time\":1102,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:02:56.476Z\",\"updated_at\":\"2022-04-20T11:02:56.476Z\"},{\"created_at\":\"2022-04-20T11:03:08.938Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7653dea0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"YJ\",\"nickname\":\"kisa_ran\",\"playback_time\":1100,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:03:08.938Z\",\"updated_at\":\"2022-04-20T11:03:08.938Z\"},{\"created_at\":\"2022-04-20T11:03:10.549Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7749b050-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"となりのヤングジャンプ\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1102,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:03:10.549Z\",\"updated_at\":\"2022-04-20T11:03:10.549Z\"},{\"created_at\":\"2022-04-20T11:03:20.368Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7d23cbf0-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"顔出し助かる\",\"nickname\":\"onion\",\"playback_time\":1111,\"priority\":false,\"sender_id\":\"95c68bfc-9a91-45df-90b5-cb28f5b297a3\",\"sent_at\":\"2022-04-20T11:03:20.368Z\",\"updated_at\":\"2022-04-20T11:03:20.368Z\"},{\"created_at\":\"2022-04-20T11:03:27.596Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"8172dac0-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"きくラジ！番組公式Twitter\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1119,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:03:27.596Z\",\"updated_at\":\"2022-04-20T11:03:27.596Z\"},{\"created_at\":\"2022-04-20T11:03:32.844Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"8493a2c0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ハッシュタグ#なんちけ\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1124,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:03:32.844Z\",\"updated_at\":\"2022-04-20T11:03:32.844Z\"},{\"created_at\":\"2022-04-20T11:03:38.693Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"88101f50-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"生放送ありがとう\",\"nickname\":\"あるまく\",\"playback_time\":1130,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:03:38.693Z\",\"updated_at\":\"2022-04-20T11:03:38.693Z\"},{\"created_at\":\"2022-04-20T11:03:46.811Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"8ce6d4b0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"かわいい\",\"nickname\":\"kisa_ran\",\"playback_time\":1138,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:03:46.811Z\",\"updated_at\":\"2022-04-20T11:03:46.811Z\"},{\"created_at\":\"2022-04-20T11:03:57.418Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"933954a0-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"お顔拝見できて最高です\",\"nickname\":\"おなす\",\"playback_time\":1163,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:03:57.418Z\",\"updated_at\":\"2022-04-20T11:03:57.418Z\"},{\"created_at\":\"2022-04-20T11:04:01.575Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"95b3a370-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"顔出しうれしい\",\"nickname\":\"てぃんご\",\"playback_time\":1153,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:04:01.575Z\",\"updated_at\":\"2022-04-20T11:04:01.575Z\"},{\"created_at\":\"2022-04-20T11:04:01.921Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"95e86f10-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"かわいい\",\"nickname\":\"ソーノ\",\"playback_time\":1154,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:04:01.921Z\",\"updated_at\":\"2022-04-20T11:04:01.921Z\"},{\"created_at\":\"2022-04-20T11:04:06.839Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"98d6dc70-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"いえーーいみてるー？\",\"nickname\":\"おなす\",\"playback_time\":1173,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:04:06.839Z\",\"updated_at\":\"2022-04-20T11:04:06.839Z\"},{\"created_at\":\"2022-04-20T11:04:12.860Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"9c6d97c0-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"1時間！\",\"nickname\":\"kisa_ran\",\"playback_time\":1164,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:04:12.860Z\",\"updated_at\":\"2022-04-20T11:04:12.860Z\"},{\"created_at\":\"2022-04-20T11:04:23.502Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"a2c56ee0-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"知らしめて\",\"nickname\":\"ソーノ\",\"playback_time\":1176,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:04:23.502Z\",\"updated_at\":\"2022-04-20T11:04:23.502Z\"},{\"created_at\":\"2022-04-20T11:04:27.184Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"a4f74300-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"もっと知らしめさせて\",\"nickname\":\"おなす\",\"playback_time\":1193,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:04:27.184Z\",\"updated_at\":\"2022-04-20T11:04:27.184Z\"},{\"created_at\":\"2022-04-20T11:04:28.265Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"a59c3590-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"しらしてこ！\",\"nickname\":\"kisa_ran\",\"playback_time\":1180,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:04:28.265Z\",\"updated_at\":\"2022-04-20T11:04:28.265Z\"},{\"created_at\":\"2022-04-20T11:04:35.612Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"a9fd45c0-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"チャンネル入会はこちらから!!\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":1187,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:04:35.612Z\",\"updated_at\":\"2022-04-20T11:04:35.612Z\"},{\"created_at\":\"2022-04-20T11:04:54.612Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"b5507140-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"入会しました\",\"nickname\":\"あるまく\",\"playback_time\":1205,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:04:54.612Z\",\"updated_at\":\"2022-04-20T11:04:54.612Z\"},{\"created_at\":\"2022-04-20T11:05:00.981Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"b91c4650-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"この放送も？\",\"nickname\":\"名無し\",\"playback_time\":1211,\"priority\":false,\"sender_id\":\"b06b9146-1ef7-4a74-ad33-47c21fafe245\",\"sent_at\":\"2022-04-20T11:05:00.981Z\",\"updated_at\":\"2022-04-20T11:05:00.981Z\"},{\"created_at\":\"2022-04-20T11:05:03.110Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"ba612260-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"番組イベントですと！\",\"nickname\":\"てぃんご\",\"playback_time\":1215,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:05:03.110Z\",\"updated_at\":\"2022-04-20T11:05:03.110Z\"},{\"created_at\":\"2022-04-20T11:05:07.290Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"bcdef3a0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"もちろん入会しました\",\"nickname\":\"おなす\",\"playback_time\":1233,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:05:07.290Z\",\"updated_at\":\"2022-04-20T11:05:07.290Z\"},{\"created_at\":\"2022-04-20T11:05:10.893Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"bf04b9d0-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"入会しました\",\"nickname\":\"kisa_ran\",\"playback_time\":1222,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:05:10.893Z\",\"updated_at\":\"2022-04-20T11:05:10.893Z\"},{\"created_at\":\"2022-04-20T11:05:11.019Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"bf17f3b0-c099-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"毎回顔出しして\",\"nickname\":\"おなす\",\"playback_time\":1237,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:05:11.019Z\",\"updated_at\":\"2022-04-20T11:05:11.019Z\"},{\"created_at\":\"2022-04-20T11:05:15.787Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"c1ef7db0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"入会したー\",\"nickname\":\"ソーノ\",\"playback_time\":1228,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:05:15.787Z\",\"updated_at\":\"2022-04-20T11:05:15.787Z\"},{\"created_at\":\"2022-04-20T11:05:22.080Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"c5afba00-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"イベントとは？\",\"nickname\":\"あるまく\",\"playback_time\":1233,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:05:22.080Z\",\"updated_at\":\"2022-04-20T11:05:22.080Z\"},{\"created_at\":\"2022-04-20T11:05:27.043Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"c8a50530-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"実質無料\",\"nickname\":\"おなす\",\"playback_time\":1253,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:05:27.043Z\",\"updated_at\":\"2022-04-20T11:05:27.043Z\"},{\"created_at\":\"2022-04-20T11:05:30.623Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cac748f0-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"顔出し毎月がいい\",\"nickname\":\"st\",\"playback_time\":1242,\"priority\":false,\"sender_id\":\"cabedd5a-2754-4859-939f-bd7ffa5180c7\",\"sent_at\":\"2022-04-20T11:05:30.623Z\",\"updated_at\":\"2022-04-20T11:05:30.623Z\"},{\"created_at\":\"2022-04-20T11:05:34.190Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cce790e0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"安いよ\",\"nickname\":\"てぃんご\",\"playback_time\":1246,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:05:34.190Z\",\"updated_at\":\"2022-04-20T11:05:34.190Z\"},{\"created_at\":\"2022-04-20T11:05:34.215Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cceb6170-c099-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"やっすい\",\"nickname\":\"あるまく\",\"playback_time\":1245,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:05:34.215Z\",\"updated_at\":\"2022-04-20T11:05:34.215Z\"},{\"created_at\":\"2022-04-20T11:05:37.110Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cea51f60-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"イベントやるまで入るかー\",\"nickname\":\"kisa_ran\",\"playback_time\":1249,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:05:37.110Z\",\"updated_at\":\"2022-04-20T11:05:37.110Z\"},{\"created_at\":\"2022-04-20T11:05:39.584Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"d01ea000-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"イベント？！入会します\",\"nickname\":\"onion\",\"playback_time\":1252,\"priority\":false,\"sender_id\":\"95c68bfc-9a91-45df-90b5-cb28f5b297a3\",\"sent_at\":\"2022-04-20T11:05:39.584Z\",\"updated_at\":\"2022-04-20T11:05:39.584Z\"},{\"created_at\":\"2022-04-20T11:06:02.794Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"ddf430a0-c099-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"入会してよかった\",\"nickname\":\"おなす\",\"playback_time\":1289,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:06:02.794Z\",\"updated_at\":\"2022-04-20T11:06:02.794Z\"},{\"created_at\":\"2022-04-20T11:06:28.112Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"ed0b6900-c099-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"迷わず入会しましたよ\",\"nickname\":\"てぃんご\",\"playback_time\":1300,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:06:28.112Z\",\"updated_at\":\"2022-04-20T11:06:28.112Z\"},{\"created_at\":\"2022-04-20T11:07:08.880Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"05581d00-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"課金していかなきゃ\",\"nickname\":\"ゲスト\",\"playback_time\":1340,\"priority\":false,\"sender_id\":\"2a7b0d83-e1e2-42c3-bb8b-65bbd6a8a518\",\"sent_at\":\"2022-04-20T11:07:08.880Z\",\"updated_at\":\"2022-04-20T11:07:08.880Z\"},{\"created_at\":\"2022-04-20T11:07:09.559Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"05bfb870-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"裏方レポ動画とか\",\"nickname\":\"kisa_ran\",\"playback_time\":1341,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:07:09.559Z\",\"updated_at\":\"2022-04-20T11:07:09.559Z\"},{\"created_at\":\"2022-04-20T11:07:21.223Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"0cb38170-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"きくらじスタンプいいね！\",\"nickname\":\"おなす\",\"playback_time\":1367,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:07:21.223Z\",\"updated_at\":\"2022-04-20T11:07:21.223Z\"},{\"created_at\":\"2022-04-20T11:07:23.657Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"0e26e790-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"ブロマガお願いします\",\"nickname\":\"てぃんご\",\"playback_time\":1355,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:07:23.657Z\",\"updated_at\":\"2022-04-20T11:07:23.657Z\"},{\"created_at\":\"2022-04-20T11:07:28.564Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"1113a740-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"スタンプ欲しあね\",\"nickname\":\"kisa_ran\",\"playback_time\":1360,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:07:28.564Z\",\"updated_at\":\"2022-04-20T11:07:28.564Z\"},{\"created_at\":\"2022-04-20T11:07:29.841Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"11d68210-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"顔出しは今回だけですか？\",\"nickname\":\"LOCK\",\"playback_time\":1362,\"priority\":false,\"sender_id\":\"a71e0604-8d4f-4ae3-874f-fa53d8652a7d\",\"sent_at\":\"2022-04-20T11:07:29.841Z\",\"updated_at\":\"2022-04-20T11:07:29.841Z\"},{\"created_at\":\"2022-04-20T11:07:31.703Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"12f2a070-c09a-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"スタンプいいね\",\"nickname\":\"ソーノ\",\"playback_time\":1364,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:07:31.703Z\",\"updated_at\":\"2022-04-20T11:07:31.703Z\"},{\"created_at\":\"2022-04-20T11:07:38.244Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"16d8b440-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"投げ銭\",\"nickname\":\"名無し\",\"playback_time\":1368,\"priority\":false,\"sender_id\":\"b06b9146-1ef7-4a74-ad33-47c21fafe245\",\"sent_at\":\"2022-04-20T11:07:38.244Z\",\"updated_at\":\"2022-04-20T11:07:38.244Z\"},{\"created_at\":\"2022-04-20T11:07:47.846Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"1c91da60-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ブログ書いてー！\",\"nickname\":\"おなす\",\"playback_time\":1394,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:07:47.846Z\",\"updated_at\":\"2022-04-20T11:07:47.846Z\"},{\"created_at\":\"2022-04-20T11:08:16.450Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"2d9e7a20-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"マジかよ入会するわ\",\"nickname\":\"LOCK\",\"playback_time\":1409,\"priority\":false,\"sender_id\":\"a71e0604-8d4f-4ae3-874f-fa53d8652a7d\",\"sent_at\":\"2022-04-20T11:08:16.450Z\",\"updated_at\":\"2022-04-20T11:08:16.450Z\"},{\"created_at\":\"2022-04-20T11:08:21.949Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"30e58ed0-c09a-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"期待ー\",\"nickname\":\"ソーノ\",\"playback_time\":1414,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:08:21.949Z\",\"updated_at\":\"2022-04-20T11:08:21.949Z\"},{\"created_at\":\"2022-04-20T11:08:24.868Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"32a2f640-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"顔出し期待の入会しました！\",\"nickname\":\"onion\",\"playback_time\":1415,\"priority\":false,\"sender_id\":\"95c68bfc-9a91-45df-90b5-cb28f5b297a3\",\"sent_at\":\"2022-04-20T11:08:24.868Z\",\"updated_at\":\"2022-04-20T11:08:24.868Z\"},{\"created_at\":\"2022-04-20T11:08:28.423Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"34c16970-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"収録風景の写真見たいな\",\"nickname\":\"てぃんご\",\"playback_time\":1420,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:08:28.423Z\",\"updated_at\":\"2022-04-20T11:08:28.423Z\"},{\"created_at\":\"2022-04-20T11:08:40.797Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"3c2188d0-c09a-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"スタート！\",\"nickname\":\"kisa_ran\",\"playback_time\":1432,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:08:40.797Z\",\"updated_at\":\"2022-04-20T11:08:40.797Z\"},{\"created_at\":\"2022-04-20T11:09:13.093Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"4f618350-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"イベントの為に入会してきました\",\"nickname\":\"LOCK\",\"playback_time\":1465,\"priority\":false,\"sender_id\":\"a71e0604-8d4f-4ae3-874f-fa53d8652a7d\",\"sent_at\":\"2022-04-20T11:09:13.093Z\",\"updated_at\":\"2022-04-20T11:09:13.093Z\"},{\"created_at\":\"2022-04-20T11:09:38.191Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"5e5729f0-c09a-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"生放送楽しい\",\"nickname\":\"ソーノ\",\"playback_time\":1490,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:09:38.191Z\",\"updated_at\":\"2022-04-20T11:09:38.191Z\"},{\"created_at\":\"2022-04-20T11:09:48.818Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"64acb720-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"前回は開始早かったので20時助かる\",\"nickname\":\"kisa_ran\",\"playback_time\":1500,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:09:48.818Z\",\"updated_at\":\"2022-04-20T11:09:48.818Z\"},{\"created_at\":\"2022-04-20T11:10:27.116Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7b8086c0-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ねるか\",\"nickname\":\"あるまく\",\"playback_time\":1538,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:10:27.116Z\",\"updated_at\":\"2022-04-20T11:10:27.116Z\"},{\"created_at\":\"2022-04-20T11:10:27.447Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7bb30870-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"うｐ\",\"nickname\":\"おなす\",\"playback_time\":1553,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:10:27.447Z\",\"updated_at\":\"2022-04-20T11:10:27.447Z\"},{\"created_at\":\"2022-04-20T11:10:40.844Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"83af40c0-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"うp\",\"nickname\":\"kisa_ran\",\"playback_time\":1552,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:10:40.844Z\",\"updated_at\":\"2022-04-20T11:10:40.844Z\"},{\"created_at\":\"2022-04-20T11:11:06.512Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"92fbe100-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"動画次回予告とか欲しい\",\"nickname\":\"おなす\",\"playback_time\":1592,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:11:06.512Z\",\"updated_at\":\"2022-04-20T11:11:06.512Z\"},{\"created_at\":\"2022-04-20T11:11:18.209Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"99f4b310-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"企画は動画欲しい\",\"nickname\":\"kisa_ran\",\"playback_time\":1590,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:11:18.209Z\",\"updated_at\":\"2022-04-20T11:11:18.209Z\"},{\"created_at\":\"2022-04-20T11:12:10.755Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"b9469530-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"北海道\",\"nickname\":\"名無し\",\"playback_time\":1640,\"priority\":false,\"sender_id\":\"b06b9146-1ef7-4a74-ad33-47c21fafe245\",\"sent_at\":\"2022-04-20T11:12:10.755Z\",\"updated_at\":\"2022-04-20T11:12:10.755Z\"},{\"created_at\":\"2022-04-20T11:12:13.775Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"bb1365f0-c09a-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"ふつおたってTwitterのDMだったっけ？\",\"nickname\":\"おなす\",\"playback_time\":1659,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:12:13.775Z\",\"updated_at\":\"2022-04-20T11:12:13.775Z\"},{\"created_at\":\"2022-04-20T11:12:16.239Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"bc8b5ff0-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"避暑地いいなあぁ\",\"nickname\":\"kisa_ran\",\"playback_time\":1648,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:12:16.239Z\",\"updated_at\":\"2022-04-20T11:12:16.239Z\"},{\"created_at\":\"2022-04-20T11:12:39.008Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"ca1da600-c09a-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"フォーム引っ越したら出来てましたね\",\"nickname\":\"kisa_ran\",\"playback_time\":1670,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:12:39.008Z\",\"updated_at\":\"2022-04-20T11:12:39.008Z\"},{\"created_at\":\"2022-04-20T11:12:39.783Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"ca93e770-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"北海道\",\"nickname\":\"てぃんご\",\"playback_time\":1671,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:12:39.783Z\",\"updated_at\":\"2022-04-20T11:12:39.783Z\"},{\"created_at\":\"2022-04-20T11:12:43.118Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"cc90c8e0-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"シーズンはたしかに避けたい\",\"nickname\":\"あるまく\",\"playback_time\":1674,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:12:43.118Z\",\"updated_at\":\"2022-04-20T11:12:43.118Z\"},{\"created_at\":\"2022-04-20T11:13:05.821Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"da18fcd0-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"北海道いいなあ\",\"nickname\":\"ソーノ\",\"playback_time\":1698,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:13:05.821Z\",\"updated_at\":\"2022-04-20T11:13:05.821Z\"},{\"created_at\":\"2022-04-20T11:13:11.839Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"ddaf42f0-c09a-11ec-91e0-83ce1bd029ac\",\"mentions\":[],\"message\":\"夏の沖縄の海いってみたい\",\"nickname\":\"おなす\",\"playback_time\":1718,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:13:11.839Z\",\"updated_at\":\"2022-04-20T11:13:11.839Z\"},{\"created_at\":\"2022-04-20T11:13:27.977Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"e74db990-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"森林が呼吸\",\"nickname\":\"kisa_ran\",\"playback_time\":1719,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:13:27.977Z\",\"updated_at\":\"2022-04-20T11:13:27.977Z\"},{\"created_at\":\"2022-04-20T11:13:50.619Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"f4cc9eb0-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"軽井沢推しですね\",\"nickname\":\"ゲスト\",\"playback_time\":1742,\"priority\":false,\"sender_id\":\"2a7b0d83-e1e2-42c3-bb8b-65bbd6a8a518\",\"sent_at\":\"2022-04-20T11:13:50.619Z\",\"updated_at\":\"2022-04-20T11:13:50.619Z\"},{\"created_at\":\"2022-04-20T11:13:52.785Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"f6172010-c09a-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"滝とか涼しいし、夏っぽいしいいかも\",\"nickname\":\"おなす\",\"playback_time\":1759,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:13:52.785Z\",\"updated_at\":\"2022-04-20T11:13:52.785Z\"},{\"created_at\":\"2022-04-20T11:13:55.057Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"f771ce10-c09a-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"優雅\",\"nickname\":\"ソーノ\",\"playback_time\":1747,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:13:55.057Z\",\"updated_at\":\"2022-04-20T11:13:55.057Z\"},{\"created_at\":\"2022-04-20T11:14:34.286Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"0ed3ace0-c09b-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"果物・・・岡山・・・とか？\",\"nickname\":\"おなす\",\"playback_time\":1800,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:14:34.286Z\",\"updated_at\":\"2022-04-20T11:14:34.286Z\"},{\"created_at\":\"2022-04-20T11:14:44.563Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"14f3d230-c09b-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ぶどう狩りいい\",\"nickname\":\"kisa_ran\",\"playback_time\":1796,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:14:44.563Z\",\"updated_at\":\"2022-04-20T11:14:44.563Z\"},{\"created_at\":\"2022-04-20T11:14:58.832Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"1d751900-c09b-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"めっちゃ軽井沢推してて草\",\"nickname\":\"おなす\",\"playback_time\":1825,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:14:58.832Z\",\"updated_at\":\"2022-04-20T11:14:58.832Z\"},{\"created_at\":\"2022-04-20T11:15:29.799Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"2fea4970-c09b-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ドラマパート\",\"nickname\":\"kisa_ran\",\"playback_time\":1841,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:15:29.799Z\",\"updated_at\":\"2022-04-20T11:15:29.799Z\"},{\"created_at\":\"2022-04-20T11:15:33.846Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"3253cf60-c09b-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ラジオドラマの時間\",\"nickname\":\"ソーノ\",\"playback_time\":1846,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:15:33.846Z\",\"updated_at\":\"2022-04-20T11:15:33.846Z\"},{\"created_at\":\"2022-04-20T11:15:50.781Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"3c6be2d0-c09b-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"流れるようにドラマパート\",\"nickname\":\"ゲスト\",\"playback_time\":1862,\"priority\":false,\"sender_id\":\"2a7b0d83-e1e2-42c3-bb8b-65bbd6a8a518\",\"sent_at\":\"2022-04-20T11:15:50.781Z\",\"updated_at\":\"2022-04-20T11:15:50.781Z\"},{\"created_at\":\"2022-04-20T11:16:15.394Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"4b178820-c09b-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"ここは収録なのかな\",\"nickname\":\"onion\",\"playback_time\":1888,\"priority\":false,\"sender_id\":\"95c68bfc-9a91-45df-90b5-cb28f5b297a3\",\"sent_at\":\"2022-04-20T11:16:15.394Z\",\"updated_at\":\"2022-04-20T11:16:15.394Z\"},{\"created_at\":\"2022-04-20T11:18:29.714Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"9b272320-c09b-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"やっぱ亜李ちゃんの先輩呼びが最高すぎて好きすぎる\",\"nickname\":\"おなす\",\"playback_time\":2035,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:18:29.714Z\",\"updated_at\":\"2022-04-20T11:18:29.714Z\"},{\"created_at\":\"2022-04-20T11:18:36.905Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"9f706590-c09b-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"イケボがゆんこん？\",\"nickname\":\"LOCK\",\"playback_time\":2029,\"priority\":false,\"sender_id\":\"a71e0604-8d4f-4ae3-874f-fa53d8652a7d\",\"sent_at\":\"2022-04-20T11:18:36.905Z\",\"updated_at\":\"2022-04-20T11:18:36.905Z\"},{\"created_at\":\"2022-04-20T11:18:39.515Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"a0fea6b0-c09b-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"デレ\",\"nickname\":\"kisa_ran\",\"playback_time\":2031,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:18:39.515Z\",\"updated_at\":\"2022-04-20T11:18:39.515Z\"},{\"created_at\":\"2022-04-20T11:19:15.101Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"b634a4d0-c09b-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"イケボゆんこん\",\"nickname\":\"ソーノ\",\"playback_time\":2067,\"priority\":false,\"sender_id\":\"ef408f13-dd7b-4fbb-bc01-0274f7693490\",\"sent_at\":\"2022-04-20T11:19:15.101Z\",\"updated_at\":\"2022-04-20T11:19:15.101Z\"},{\"created_at\":\"2022-04-20T11:19:33.883Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"c1668cb0-c09b-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"物体内にも移動できるチケット\",\"nickname\":\"ゲスト\",\"playback_time\":2085,\"priority\":false,\"sender_id\":\"2a7b0d83-e1e2-42c3-bb8b-65bbd6a8a518\",\"sent_at\":\"2022-04-20T11:19:33.883Z\",\"updated_at\":\"2022-04-20T11:19:33.883Z\"},{\"created_at\":\"2022-04-20T11:20:54.629Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"f1876950-c09b-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"いい人だ\",\"nickname\":\"ゲスト\",\"playback_time\":2166,\"priority\":false,\"sender_id\":\"2a7b0d83-e1e2-42c3-bb8b-65bbd6a8a518\",\"sent_at\":\"2022-04-20T11:20:54.629Z\",\"updated_at\":\"2022-04-20T11:20:54.629Z\"},{\"created_at\":\"2022-04-20T11:22:03.450Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"1a8ca9a0-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"積極的なかわいい後輩っていいよね\",\"nickname\":\"おなす\",\"playback_time\":2249,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:22:03.450Z\",\"updated_at\":\"2022-04-20T11:22:03.450Z\"},{\"created_at\":\"2022-04-20T11:22:38.384Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"2f5f2b00-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"三角関係スタート\",\"nickname\":\"kisa_ran\",\"playback_time\":2270,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:22:38.384Z\",\"updated_at\":\"2022-04-20T11:22:38.384Z\"},{\"created_at\":\"2022-04-20T11:23:00.167Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"3c5afd70-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"めでたしめでたし\",\"nickname\":\"ゲスト\",\"playback_time\":2292,\"priority\":false,\"sender_id\":\"2a7b0d83-e1e2-42c3-bb8b-65bbd6a8a518\",\"sent_at\":\"2022-04-20T11:23:00.167Z\",\"updated_at\":\"2022-04-20T11:23:00.167Z\"},{\"created_at\":\"2022-04-20T11:23:13.947Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"4491a6b0-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"良いラジオドラマだった\",\"nickname\":\"おなす\",\"playback_time\":2320,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:23:13.947Z\",\"updated_at\":\"2022-04-20T11:23:13.947Z\"},{\"created_at\":\"2022-04-20T11:23:25.135Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"4b3ccdf0-c09c-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"聞き入る\",\"nickname\":\"てぃんご\",\"playback_time\":2317,\"priority\":false,\"sender_id\":\"a6eeedfa-22b6-4e6f-8cc3-82ee4bb800d0\",\"sent_at\":\"2022-04-20T11:23:25.135Z\",\"updated_at\":\"2022-04-20T11:23:25.135Z\"},{\"created_at\":\"2022-04-20T11:23:52.326Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"5b71d260-c09c-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"早い。入会してよかった\",\"nickname\":\"kisa_ran\",\"playback_time\":2344,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:23:52.326Z\",\"updated_at\":\"2022-04-20T11:23:52.326Z\"},{\"created_at\":\"2022-04-20T11:24:06.118Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"63aa5060-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"早くコミック読みたい\",\"nickname\":\"おなす\",\"playback_time\":2372,\"priority\":false,\"sender_id\":\"40fc81e3-306c-48cc-ad9b-7ec62461d222\",\"sent_at\":\"2022-04-20T11:24:06.118Z\",\"updated_at\":\"2022-04-20T11:24:06.118Z\"},{\"created_at\":\"2022-04-20T11:24:09.933Z\",\"end_time_in_seconds\":16,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"65f06fd0-c09c-11ec-9b42-5745cdf8d9b0\",\"mentions\":[],\"message\":\"となりのヤングジャンプ\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":2361,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:24:09.933Z\",\"updated_at\":\"2022-04-20T11:24:09.933Z\"},{\"created_at\":\"2022-04-20T11:24:25.338Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"6f1f0da0-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"あさってか\",\"nickname\":\"あるまく\",\"playback_time\":2376,\"priority\":false,\"sender_id\":\"9452c756-7bd1-4a3c-95cd-41e2cfa0633b\",\"sent_at\":\"2022-04-20T11:24:25.338Z\",\"updated_at\":\"2022-04-20T11:24:25.338Z\"},{\"created_at\":\"2022-04-20T11:24:37.701Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"767d7f50-c09c-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"この券が地味に重要だったのか\",\"nickname\":\"onion\",\"playback_time\":2390,\"priority\":false,\"sender_id\":\"95c68bfc-9a91-45df-90b5-cb28f5b297a3\",\"sent_at\":\"2022-04-20T11:24:37.701Z\",\"updated_at\":\"2022-04-20T11:24:37.701Z\"},{\"created_at\":\"2022-04-20T11:24:43.972Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7a3a6040-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"チャンネル入会はこちらから!!\",\"nickname\":\"なんでも いうことをきくラジオ 『きくラジ！』\",\"playback_time\":2395,\"priority\":true,\"sender_id\":\"-1\",\"sent_at\":\"2022-04-20T11:24:43.972Z\",\"updated_at\":\"2022-04-20T11:24:43.972Z\"},{\"created_at\":\"2022-04-20T11:24:53.607Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"7ff88f70-c09c-11ec-9b79-473e3b670acd\",\"mentions\":[],\"message\":\"たのしみ\",\"nickname\":\"kisa_ran\",\"playback_time\":2405,\"priority\":false,\"sender_id\":\"eda403c3-2157-4c67-9966-ecc4c56cd5bd\",\"sent_at\":\"2022-04-20T11:24:53.607Z\",\"updated_at\":\"2022-04-20T11:24:53.607Z\"},{\"created_at\":\"2022-04-20T11:24:54.057Z\",\"end_time_in_seconds\":null,\"group_id\":\"bc1615b0-bb04-11ec-adeb-551a4edaa771\",\"id\":\"803d3990-c09c-11ec-882c-edb3180ada6d\",\"mentions\":[],\"message\":\"既に入会していた\",\"nickname\":\"LOCK\",\"playback_time\":2406,\"priority\":false,\"sender_id\":\"a71e0604-8d4f-4ae3-874f-fa53d8652a7d\",\"sent_at\":\"2022-04-20T11:24:54.057Z\",\"updated_at\":\"2022-04-20T11:24:54.057Z\"}]";
				//var _l = JsonConvert.DeserializeObject<List<chPlusMsgInfo>>(b);
				
				var url = getUrl(dt);
				//{"token":"eyaaaaa","group_id":"aaaaaaaa-0000-0000-0000-000000000000"}
				var data = "{\"token\":\"" + token + "\",\"group_id\":\"" + groupId + "\"}";
				//data = "aaa";
				var h = util.getHeader();
				h.Add("Content-Type", "application/json");
				var _auth = rl.getAuth();
				//if (_auth != null) h.Add("Authorization", "Bearer " + _auth);
				var r = getMsgCurl.getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "POST", data);
				if (r == null) {
					util.debugWriteLine("tscg_chPlus getMessages null " + url + " " + data);
					return null;
				}
				//if (r.IndexOf("
				var l = JsonConvert.DeserializeObject<List<chPlusMsgInfo>>(r);
				util.debugWriteLine(l);
				return l;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
		}
		string getUrl(string dt) {
			var url = "https://comm-api.sheeta.com/messages.history?limit=120&oldest=";
			//var dt = latest.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
			//"2022-04-20T12%3A03%3A27.000Z";
			
			return url + dt + "&sort_direction=asc";
		}
		string getToken() {
			var url = "https://nfc-api.nicochannel.jp/fc/video_pages/" + id + "/comments_user_token";
			var h = util.getHeader();
			var _auth = rl.getAuth();
			if (_auth != null) h.Add("Authorization", "Bearer " + _auth);
			var token = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS);
			if (token == null) return null; 
			return util.getRegGroup(token, "\"access_token\":\"(.+?)\"");
		}
		void saveChat(ChatInfo chatinfo) {
			try {
				XDocument chatXml;
				var vposStartTime = isVposStartTime ? (long)rp.firstSegmentSecond : 0;
				if (vposStartTime == -1) vposStartTime = tsConfig.timeSeconds;
				
					if (lastRealTimeComment == null) {
						/*
						if (ri.si.type == "official") {
							chatXml = chatinfo.getFormatXml(0, true, vposStartTime);
		//					chatXml = chatinfo.getFormatXml(_openTime + vposStartTime);
						} else {
							chatXml = chatinfo.getFormatXml(ri.si.openTime + vposStartTime);
		
						}
						*/
						//chatXml = chatinfo.getFormatXml(ri.si.openTime, true, );
						chatXml = chatinfo.getFormatXml(0, true, vposStartTime);
					} else {
						chatXml = chatinfo.getFormatXml(0, true, ri.si.openTime);
						//if (ri.si.type == "official") {
							//chatXml = chatinfo.getFormatXml(0, true, rp.serverTime - ri.si._openTime);
						//} else chatXml = chatinfo.getFormatXml(serverTime);
						//} else {
							//chatXml = chatinfo.getFormatXml(0, true, serverTime - _openTime);
						//	chatXml = chatinfo.getFormatXml(rp.serverTime);
						//}
					}
				
				
				if (!isSave) return;
				/*
				if (chatinfo.root == "chat" && chatinfo.date < _gotMinTime) {
					_gotMinTime = chatinfo.date;
					_gotMinXml[1] = _gotMinXml[0];
					_gotMinXml[0] = chatXml.ToString();
				}
				*/
	
				try {
	//				if (commentSW != null) {
						string s;
						if (isGetXml) {
							s = chatXml.ToString();
						} else {
							try {
								chatXml.Root.SetAttributeValue("contents", chatXml.Root.Value);
								chatXml.Root.RemoveNodes();
								s = JsonConvert.SerializeXNode(chatXml);

							} catch (Exception e) {
								util.debugWriteLine(e.Message + e.Source + e.StackTrace);
								s = chatXml.ToString();
							}
							
						}
						
			            
						var isMeetStartTimeSave = !tsConfig.isAfterStartTimeComment ||
              					//chatinfo.date > ri.si._openTime + tsConfig.timeSeconds - 10;
								chatinfo.vpos > tsConfig.timeSeconds * 100 - 1000;
						var isMeetEndTimeSave = !tsConfig.isBeforeEndTimeComment || 
									tsConfig.endTimeSeconds == 0 || 
	  								//chatinfo.date < ri.si._openTime + tsConfig.endTimeSeconds + 10;
									chatinfo.vpos < tsConfig.endTimeSeconds * 100 + 1000;
						
						if (!isMeetStartTimeSave || s == lastRealTimeComment)
							isReachStartTimeSave = true;
							
						var isComSave = isMeetStartTimeSave && isMeetEndTimeSave;
    					if (isComSave) {
							s = util.getReplacedComment(s, rp.commentReplaceList);
							gotCommentList.Add(new GotCommentInfo(s, chatinfo.no, chatinfo.date, chatinfo.vpos));
							gotCount++;
			            	
							if (gotCount % 2000 == 0 && !rfu.isPlayOnlyMode) {
								if (isLog)
									form.addLogText(gotCount + "件のコメントを保存しました");
								gotCommentList = gotCommentList.Distinct().ToList();
							}
						}
			            
	//				}
	           
				} catch (Exception ee) {util.debugWriteLine(ee.Message + " " + ee.StackTrace);}
				
			
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
		private void endProcess() {
			util.debugWriteLine("tscg end process");
			if (isStore) return;
			try {
				var isWrite = (!rfu.isPlayOnlyMode && !rp.ri.isChase && lastRealTimeComment == null);
				if (isWrite && isLog)
					form.addLogText("コメントの後処理を開始します");
				
				
				gotCommentList = gotCommentList.ToList();
	
				util.debugWriteLine("end proccess gotCommentList count " + gotCommentList.Count);
	
				
				util.debugWriteLine("end proccess a");
				
				var keys = new List<long>();
				foreach (var c in gotCommentList) {
					var date = c.vpos;
					keys.Add(date);
				}
				
				util.debugWriteLine("end proccess b");
				
				gotCommentList.Sort(new Comparison<GotCommentInfo>(commentListCompare));
				var chats = gotCommentList.Select(x => x.comment).ToArray();
				
				var maeC = chats.Count();
				chats = chats.Distinct().ToArray();
				var atoC = chats.Count();
				rp.gotTsCommentList = chats;
				
				util.debugWriteLine("end proccess d");
				
				if (rp.ri.isChase && lastRealTimeComment == null && rp.chaseCommentBuf != null) {
					while (rp.chaseCommentBuf.Count == 0 
					       && rm.rfu == rfu) Thread.Sleep(1000);
					rp.chaseCommentSum();
				}
				
				util.debugWriteLine("end proccess c");
				
				if (!isWrite) return;
				
				
				
					
					
					var fileName = util.getOkCommentFileName(rm.cfg, recFolderFile, id, true, false);
					
					if (rm.cfg.get("fileNameType") == "10")
						fileName = fileName.Replace("{w}", rp.visitCount.ToString()).Replace("{c}", rp.commentCount.ToString());
					using (var w = new StreamWriter(fileName + "_", false, System.Text.Encoding.UTF8)) {
						if (isGetXml) {
							w.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
							if (!isGetCommentXmlInfo) 
								w.WriteLine("<packet>");
							else {
								writeXmlStreamInfo(w);
							}
						    w.Flush();
						} else {
							w.WriteLine("[");
						}
						//w.WriteLine(threadLine + ((!isGetXml && chats.Length != 0) ? "," : ""));
						for (var i = 0; i < chats.Length; i++) {
							w.WriteLine(chats[i] + ((!isGetXml && i != chats.Length - 1) ? "," : ""));
						}
						if (isGetXml) {
							w.WriteLine("</packet>");
						} else {
							//w.BaseStream.Position -= 3;
							//w.WriteLine("");
							w.WriteLine("]");
						}
						w.Flush();
						//w.Close();
					}
					File.Delete(fileName);
					File.Move(fileName + "_", fileName);
				
				if (isLog)
					form.addLogText("コメントの保存を完了しました");
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
	}
	
}
