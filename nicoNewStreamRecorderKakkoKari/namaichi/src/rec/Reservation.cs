/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/24
 * Time: 18:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Reservation.
	/// </summary>
	public class Reservation
	{
		CookieContainer cc;
		string lv;
		
		string useUrl = "https://live.nicovideo.jp/api/timeshift.ticket.use";
		string reservationsUrl = "https://live.nicovideo.jp/api/timeshift.reservations";
		
		public Reservation(CookieContainer cc, string lv)
		{
			this.cc = cc;
			this.lv = lv;
		}
		public string reserve() {
			var id = util.getRegGroup(lv, "(\\d+)");
			//var url = "https://live.nicovideo.jp/api/watchingreservation?mode=confirm_watch_my&vid=" + id + "&next_url&analytic";
			var getMode = "watchNum";
			var url = "";
			if (getMode == "watchNum") {
				url = "https://live.nicovideo.jp/api/watchingreservation?mode=watch_num&vid=" + id + "&next_url=gate%2F" + lv + "&analytic=gate_modal_0_official_" + lv + "_closed";
			} else {
 
			}
 			
			var res = util.getPageSource(url, cc, null, false, 3000);
			if (res == null) return "no";
			var mode = "";
			if (res.IndexOf("watching_reservation_regist") > -1) mode = "watching_reservation_regist";
			if (res.IndexOf("regist_finished") > -1) mode = "regist_finished";
						
			var token = util.getRegGroup(res, "(ulck_\\d+)");
			if (token == null) {
				if (res.IndexOf("申し込み期限切れです。") > -1) return "申し込み期限切れです。";
				if (res.IndexOf("既に、視聴済みです。") > -1) return "既に、視聴済みです。";
				//moveWatch
				return "予約できませんでした。";
			}
			while (true) {
				var wrTask = watchingReservation(token, id, mode);
				wrTask.Wait();
				var wrRet = wrTask.Result;
				util.debugWriteLine(wrRet);
				if (wrRet.IndexOf("を削除して、新しくタイムシフト予約をします。") > -1) {
					return "予約リストが一杯です。";
				}
//				if (wrRet.IndexOf("Nicolive.TimeshiftActions.moveWatch") > -1) {
//					return "ok";
//				}
				if (wrRet.IndexOf("システムエラーが") > -1 ||
				    wrRet.IndexOf("nicolive_video_response status=\"fail\"") > -1) {
					res = util.getPageSource(url, cc, null, false, 3000);
					util.debugWriteLine(res);
					if (res == null) return "no";
					if (res.IndexOf("watching_reservation_regist") > -1) mode = "watching_reservation_regist";
					if (res.IndexOf("regist_finished") > -1) mode = "regist_finished";
					
					token = util.getRegGroup(res, "(ulck_\\d+)");
					if (token == null) {
						if (res.IndexOf("申し込み期限切れです。") > -1) return "申し込み期限切れです。";
						if (res.IndexOf("既に、視聴済みです。") > -1) return "既に、視聴済みです。";
						//moveWatch
						return "予約できませんでした。";
					}
				}
				if (wrRet.IndexOf("nicolive_video_response status=\"ok\"") > -1) return "ok";
				if (wrRet.IndexOf("regist_finished") > -1) mode = "regist_finished";
				Thread.Sleep(1000);
			}
			//return "ok";
		}

		async private Task<string> watchingReservation(string token, string id, string mode) {
			util.debugWriteLine("watching reservation post " + token + " " + mode);
			try {
				var handler = new System.Net.Http.HttpClientHandler();
				handler.UseCookies = true;
				handler.CookieContainer = cc;
				var http = new System.Net.Http.HttpClient(handler);
				handler.UseProxy = true;
				handler.Proxy = util.httpProxy;
				
				
				//var contentStr = "mode=auto_register&vid=" + id + "&token=" + token + "&_=";
				//util.debugWriteLine("reservation " + contentStr);
				
				var _content = new List<KeyValuePair<string, string>>();
				if (mode == "watching_reservation_regist") {
					_content.Add(new KeyValuePair<string, string>("mode", "auto_register"));
					_content.Add(new KeyValuePair<string, string>("vid", id));
					_content.Add(new KeyValuePair<string, string>("token", token));
					_content.Add(new KeyValuePair<string, string>("_", ""));
				} else if (mode == "regist_finished") {
					_content.Add(new KeyValuePair<string, string>("accept", "true"));
					_content.Add(new KeyValuePair<string, string>("mode", "use"));
					_content.Add(new KeyValuePair<string, string>("vid", id));
					_content.Add(new KeyValuePair<string, string>("token", token));
					_content.Add(new KeyValuePair<string, string>("", ""));
				}
				//var content = new System.Net.Http.StringContent(contentStr);
				var content = new System.Net.Http.FormUrlEncodedContent(_content);
				//content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

				http.Timeout = TimeSpan.FromSeconds(3);
				var url = "https://live.nicovideo.jp/api/watchingreservation";
				var _t = http.PostAsync(url, content);
				_t.Wait();
				var _res = _t.Result;
				var res = await _res.Content.ReadAsStringAsync();
	//			var a = _res.Headers;
				
	//			if (res.IndexOf("login_status = 'login'") < 0) return null;
				
//				cc = handler.CookieContainer;
				
				return res;
//				return cc;
			} catch (Exception e) {
				util.debugWriteLine("get watching exception " + e.Message+e.StackTrace);
				return null;
			}
		}
		public string live2Reserve() {
			var id = util.getRegGroup(lv, "(\\d+)");
			var useUrl = "https://live.nicovideo.jp/api/timeshift.ticket.use";
			var useData = Encoding.ASCII.GetBytes("vid=" + id);
			var header = getLive2ReserveHeader(id);
			var res = util.postResStr(useUrl, header, useData);
			if (res == null) return "チケットを予約するための接続に失敗しました";
			var res2 = "";
			
			if (res.IndexOf("TICKET_NOT_FOUND") > -1) {
				
				var reserveData = Encoding.ASCII.GetBytes("vid=" + id + "&overwrite=0");
				res2 = util.postResStr(reservationsUrl, header, reserveData);
				if (res2 == null) return "チケットを取得するための接続に失敗しました" + res;
				if (res2.IndexOf("Timeshift_reservation can overwrite") > -1)
					return "予約リストが一杯です。";
				if (res2.IndexOf("Timeshift_reservation was used") > -1)
					return "すでに視聴済みです。";
				else if (res2.IndexOf("status\":200") > -1) {
					return "ok";
				} else if (res2.IndexOf("Timeshift_reservation was expired") > -1) {
					return "予約の期限切れでした。"; 
				} else {
					return "チケットの予約中に予期せぬ問題が発生しました" + res + "/" + res2;
				}
			}
			return "チケットの予約接続中に予期せぬ問題が発生しました" + res + "/" + res2;
			
		}
		public bool useLive2Reserve() {
			var id = util.getRegGroup(lv, "(\\d+)");
			
			var useData = Encoding.ASCII.GetBytes("vid=" + id);
			var header = getLive2ReserveHeader(id);
			var res = util.postResStr(useUrl, header, useData);
			if (res == null) return false;
			return res.IndexOf("status\":200") > -1;
		}
		private Dictionary<string, string> getLive2ReserveHeader(string id) {
			if (id.StartsWith("lv")) id = util.getRegGroup(lv, "(\\d+)");
			var header = new Dictionary<string, string>();
			header.Add("Cookie", cc.GetCookieHeader(new Uri(useUrl)));
			header.Add("Accept", "application/json, text/plain, */*");
			header.Add("Content-Type", "application/x-www-form-urlencoded");
			header.Add("Referer", "https://live2.nicovideo.jp/watch/lv" + id);
			header.Add("Origin", "https://live2.nicovideo.jp");
			//header.Add("User-Agent", util.userAgent);
			return header;
		}
	}
}
