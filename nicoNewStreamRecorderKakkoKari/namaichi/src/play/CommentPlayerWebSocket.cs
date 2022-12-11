/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/27
 * Time: 14:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Xml.Linq;
using WebSocket4Net;
using Newtonsoft.Json;

using namaichi.rec;
using namaichi.info;

namespace namaichi.play
{
	/// <summary>
	/// Description of CommentPlayerWebSocket.
	/// </summary>
	public class CommentPlayerWebSocket2
	{
		private IRecorderProcess wsr;
		private commentForm cf;
		
		private WebSocket wsc;
		private DateTime lastWebsocketConnectTime;
		
		public bool isEnd = false;
		private string ticket;
		private DeflateDecoder deflateDecoder = new DeflateDecoder();
		
		public CommentPlayerWebSocket2(IRecorderProcess wsr, commentForm cf)
		{
			this.wsr = wsr;
			this.cf = cf;
		}
		public void start() {
			connect();
		}
		public void connect() {
	    	util.debugWriteLine("connect message server player");
	    	if (wsr == null)
	    		return;
	    	while (wsr.msUri  == null && wsr != null) 
	    		Thread.Sleep(100);
			var msUri = wsr.msUri;
			util.debugWriteLine("msuri player2 " + msUri);
			
			lock (this) {
				var  isPass = (TimeSpan.FromSeconds(3) > (DateTime.Now - lastWebsocketConnectTime));
				lastWebsocketConnectTime = DateTime.Now;
				if (isPass) 
					Thread.Sleep(3000);
			}
			
//			msThread = wsr.msReq;
	//		String msReq = "[truncated][{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":-1000,\"with_global\":1,\"scores\":1,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]\0";
	
			
			//msReq = wsr.msReq;
			
			//msReq = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + msThread + "\",\"version\":\"20061206\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + res_from + ",\"with_global\":1,\"scores\":0,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			
			//util.debugWriteLine(msAddr + " " + msPort);
			util.debugWriteLine("msuri player " + msUri);
			//util.debugWriteLine("msreq " + msReq);
			
			try {
				var header =  new List<KeyValuePair<string, string>>();
				header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Protocol", "msg.nicovideo.jp#json"));
				if (wsr.isJikken) {
					header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Extensions", "permessage-deflate"));
					header.Add(new KeyValuePair<string,string>("Sec-WebSocket-Version", "13"));
				}
				wsc = new WebSocket(msUri,  "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null,  SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
				//wsc = new WebSocket(msUri,  "", null, header, util.userAgent, "", WebSocketVersion.Rfc6455, null, SslProtocols.Tls12);
				wsc.Opened += onWscOpen;
				wsc.Closed += onWscClose;
				wsc.MessageReceived += onWscMessageReceive;
				wsc.Error += onWscError;
				
		        wsc.Open();
		        
		        var _wsc = wsc; 
				Thread.Sleep(5000);
				if (_wsc.State == WebSocketState.Connecting) 
					wsc.Close();
				
			} catch (Exception ee) {
				util.debugWriteLine("wsc player connect exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				return ;
			}
	        
			util.debugWriteLine("ms start");
		
		}
		private void onWscOpen(object sender, EventArgs e) {
			util.debugWriteLine("player ms open a");
			util.debugWriteLine("player wsr " + wsr);
			util.debugWriteLine("player wsr msreq " + wsr.msReq);
			
			if (wsr == null|| wsr.msReq == null) {
				if (wsc != null) wsc.Close();
				return;
			}
			try {
				util.debugWriteLine("player wsr msreq2 " + string.Join(" ", wsr.msReq));
			} catch (Exception) {
				util.debugWriteLine("player comment req exception ");
			}
			
			var res_from = (wsr.ri.si.isTimeShift) ? "1" : "-100";
			var msReq = Regex.Replace(wsr.msReq[0], "res_from\"\\:.+?,", "res_from\":" + res_from + ",");
			wsc.Send(msReq);
			if (wsr.msReq.Length == 2) {
				wsc.Send(wsr.msReq[1]);
				var res_from2 = (wsr.ri.si.isTimeShift) ? "1" : "-10";
				var msReq2 = Regex.Replace(wsr.msReq[1], "res_from\"\\:.+?,", "res_from\":" + res_from2 + ",");
			}

		}
		
		private void onWscClose(object sender, EventArgs e) {
			util.debugWriteLine("ms onclose");
//			closeWscProcess();
			wsc = null;
			try {
				if (!isEnd)
					connect();
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			
		}

		private void onWscError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			util.debugWriteLine("ms onerror");
			//stopRecording();
//			endStreamProcess();
		}
		private void onWscMessageReceive(object sender, MessageReceivedEventArgs e) {
			util.debugWriteLine("on wsc message ");
			
			if (cf.form.recBtn.Text == "録画開始") {
				isEnd = true;
				if (wsc != null) wsc.Close();
				return;
			}
			
			if (e.Message.StartsWith("{\"ping\":{\"content\":\"rf:")) {
//				closeWscProcess();
//				try {commentSW.Close();}
//				catch (Exception eee) {util.debugWriteLine(eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);}
				isEnd = true;
//				rm.form.addLogText("コメントの保存を完了しました");
			}
			
			/*
			if (rm.rfu != rfu || (!wsr.isTimeShift && !isRetry)) {
				try {
					if (wsc != null) wsc.Close();
				} catch (Exception ee) {
					util.debugWriteLine("wsc message receive exception " + ee.Source + " " + ee.StackTrace + " " + ee.TargetSite + " " + ee.Message);
				}
				//stopRecording();
				util.debugWriteLine("tigau rfu comment" + e.Message);
				return;
			}
			*/
			List<ChatInfo> chatInfoList;
			if (wsr.isJikken) {
				//実験放送　なくし
				chatInfoList = null;
				//chatInfoList = ((JikkenRecordProcess)wsr).getChatInfoList(deflateDecoder.decode(e.Data));
			}
			else {
				chatInfoList = new List<ChatInfo>();
				var xml = JsonConvert.DeserializeXNode(e.Message);
				chatInfoList.Add(new ChatInfo(xml));
			}
									
//			string msg = (wsr.isJikken) ? deflateDecoder.decode(e.Data) : e.Message;
			 
			foreach (var chatinfo in chatInfoList) {
				if (cf.form.recBtn.Text == "録画開始") {
					isEnd = true;
					break;
				}
				
				XDocument chatXml;
				chatXml = chatinfo.getFormatXml(wsr.ri.si.openTime);
				
				util.debugWriteLine("player xml " + chatXml.ToString());
				
				if (chatinfo.root == "chat" && (chatinfo.contents.IndexOf("/hb ifseetno") != -1 && 
						chatinfo.premium == "3")) continue;
				if (chatinfo.root != "chat" && chatinfo.root != "thread" && chatinfo.root != "control") continue;
				
				if (chatinfo.root == "thread") {
	//				serverTime = chatinfo.serverTime;
					ticket = chatinfo.ticket;
				}
				
//				util.debugWriteLine("wsc message " + wsc);
				
	//			Newtonsoft.Json
				//if (e.Message.IndexOf("chat") < 0 &&
				//    	e.Message.IndexOf("thread") < 0) return;
				
				
	//            util.debugWriteLine(jsonCommentToXML(text));
				
				//if (!isTimeShift)
				addDisplayComment(chatinfo);
			}
		}
		private void addDisplayComment(namaichi.info.ChatInfo chat) {
			
			if (chat.root.Equals("thread")) return;
			if (chat.contents == "再読み込みを行いました<br>読み込み中のままの方はお手数ですがプレイヤー下の更新ボタンをお試し下さい") {
				util.debugWriteLine("chat 再読み込みを行いました");
				return;
			}
			if (chat.contents == null) return;
//			var time = util.getUnixToDatetime(chat.vpos / 100);
//			var unixKijunDt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			var __time = chat.date - wsr.ri.si.openTime; //- (60 * 60 * 9);
			if (__time < 0) __time = 0;

//			var __timeDt = util.getUnixToDatetime(__time);
//			var openTimeDt = util.getUnixToDatetime(openTime);
//			var __timeDt0 = __timeDt - openTimeDt;
//			var __timeDt1 = __timeDt0. - new timespunixKijunDt;
			var h = (int)(__time / (60 * 60));
			var m = (int)((__time % (60 * 60)) / 60);
			var _m = (m < 10) ? ("0" + m.ToString()) : m.ToString();
			var s = __time % 60;
			var _s = (s < 10) ? ("0" + s.ToString()) : s.ToString();
			var keikaTime = h + ":" + _m + ":" + _s + "";
			/*
//			- unixKijunDt;
			
//			var __time = new TimeSpan(chat.vpos * 10000);
			var h = (int)(__timeSpan.TotalHours);
			var m = __timeSpan.Minutes;
			var s = __timeSpan.Seconds;
			*/
//			- new TimeSpan(9,0,0);
			var c = (chat.premium == "3") ? "red" :
				((chat.premium == "7") ? "blue" : "black");
			if (chat.root == "control") c = "red";
			
			cf.addComment(keikaTime, chat.contents, chat.userId, chat.score, c);
			
		}
		
	}
}
