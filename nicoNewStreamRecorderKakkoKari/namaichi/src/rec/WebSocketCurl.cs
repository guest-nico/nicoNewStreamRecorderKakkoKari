/*
 * Created by SharpDevelop.
 * User: ajkkh
 * Date: 2024/02/14
 * Time: 17:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using namaichi.info;
using namaichi.utility;
using Newtonsoft.Json;
using WebSocket4Net;

namespace namaichi.rec
{
	/// <summary>
	/// Description of WebSocketCurl.
	/// </summary>
	public class WebSocketCurl
	{
		internal string[] webSocketRecInfo;
		internal RecordingManager rm;
		private DateTime lastWebsocketConnectTime = DateTime.MinValue;
		internal WebSocketRecorder wr;
		public bool isRetry = true;
		DateTime lastPingTime = DateTime.Now;
		
		public WebSocketCurl() {}
		public WebSocketCurl(RecordingManager rm, WebSocketRecorder wr, string[] webSocketInfo)
		{
			this.rm = rm;
			this.wr = wr;
			this.webSocketRecInfo = webSocketInfo;
		}
		private IntPtr easy = IntPtr.Zero;
		
		public bool connect() {
			//string mes = "";
			while ((wr == null || wr.IsRetry) && isRetry) {
				try {
					var r = connectCore();
					if (!r) {
						close();
						Thread.Sleep(6000);
					}
				} finally {
					releaseHandle();
				}
			}
			return true;
		}
		private bool connectCore() {
			isRetry = true;
			lock(this) {
				var  isPass = (TimeSpan.FromSeconds(5) > (DateTime.Now - lastWebsocketConnectTime));
				if (isPass) 
					Thread.Sleep(5000);
				lastWebsocketConnectTime = DateTime.Now;
			}
			
			try {
				var url = webSocketRecInfo[0];
				#if DEBUG
					rm.form.addLogText("connect " + webSocketRecInfo[0].Substring(0, 10));
				#endif
				util.debugWriteLine("connect " + webSocketRecInfo[0].Substring(0, 10));
				
				easy = Curl.curl_easy_init();
				
				if (easy == IntPtr.Zero) {
					rm.form.addLogText("ライブラリよりWebSocketへの接続を開始できませんでした " + url.Substring(0, 10));
					return false;
				}
				util.debugWriteLine("curl push connect  ");
				
				
				Curl.curl_easy_setopt(easy, CURLoption.CURLOPT_URL, url);
				Curl.curl_easy_setopt(easy, CURLoption.CURLOPT_SSL_VERIFYPEER, 0);
				Curl.curl_easy_setopt(easy, CURLoption.CURLOPT_CONNECT_ONLY, 2L);
				Curl.curl_easy_setopt(easy, CURLoption.CURLOPT_USERAGENT, util.userAgent);
				
				easy = setHeader(easy);
				
				var code = Curl.curl_easy_perform(easy);
				util.debugWriteLine("curl ws connect code " + code);
				if(code != CURLcode.CURLE_OK) {
					util.debugWriteLine("curl easy error " + code + " " + url);
					rm.form.addLogText("WebSocketの接続に失敗しました " + code + " " + url.Substring(0, 10));
					stop();
					Thread.Sleep(5000);
					return false;
				} else {
					Thread.Sleep(1000);
					
					onOpen();
					
					var buf = new List<byte>();
					var recvS = "";
					while ((wr == null || wr.IsRetry) && isRetry) {
						if (!isActiveWs()) {
							stop();
							break;
						}
						if (isTimeOut()) {
							stop();
							rm.form.addLogText("timeout " + webSocketRecInfo[0].Substring(0, 10));
							break;
						}
							
						IntPtr recvPtr = IntPtr.Zero;
						try {
							uint recvN = 0;
							//Thread.Sleep(1000);
							var wsFramePtr = IntPtr.Zero;
							var recvBytes = new byte[100000];
							
							CURLcode recvCode;
							recvPtr = Curl.curl_ws_recv_wrap(easy, out recvCode, out recvN);
							if (recvCode != CURLcode.CURLE_OK) {
								util.debugWriteLine("curl ws recvCode not ok " + recvCode);
								rm.form.addLogText("recv code error " + recvCode + " " + webSocketRecInfo[0].Substring(0, 10));
								break;
							}
							
							var recvNI = (int)recvN;
							Marshal.Copy(recvPtr, recvBytes, 0, recvNI);
							
							recvS = Encoding.UTF8.GetString(recvBytes, 0, recvNI);
							if (string.IsNullOrEmpty(recvS)) {
								Thread.Sleep(1000);
								continue;
							}
							var bufS = "";
							if (isErrorMes(recvS) || buf.Count != 0) {
								//buf += recvS;
								buf.AddRange(recvBytes.ToList().GetRange(0, recvNI));
								bufS = Encoding.UTF8.GetString(buf.ToArray(), 0, buf.Count);
								if (isErrorMes(bufS)) {
									if (recvS.EndsWith("}}"))
										buf.Clear();
									continue;
								}
								util.debugWriteLine("error recv s " + bufS);
								
							} else bufS = recvS;
							
							onMessageReceive(bufS);
							buf.Clear();
							
							//onMessageReceiveCore(recvS);
						} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace);
							
						} finally {
							if (recvPtr != IntPtr.Zero) Curl.memFree(recvPtr);
							if (wr != null) util.debugWriteLine("curl ws url recv " + url + " " + recvS);
						}
						
					}
			    }
				#if DEBUG
					if (rm != null)	rm.form.addLogText("ws curl recv end " + DateTime.Now + " " + url.Substring(0, 10));
				#endif
				util.debugWriteLine("ws curl recv end");
				close();
				isRetry = false;
				return true;
				
			} catch (Exception ee) {
				util.debugWriteLine("push connect exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				return false;
			} finally {
				releaseHandle();
			}
		}
		public void wsSend(string mes) {
			util.debugWriteLine("ws send curl " + mes);
			if (easy == IntPtr.Zero) {
				util.debugWriteLine("curl ws send no easy " + mes);
				return;
			}
			var bytes = Encoding.UTF8.GetBytes(mes);
			var len = Encoding.UTF8.GetByteCount(mes);
			var aalen = mes.Length;
			int outN = 0;
			var sendCode = Curl.curl_ws_send_wrap(easy, bytes, len, out outN, 0, (int)curlWsFlags.CURLWS_TEXT);
			
			util.debugWriteLine("curl ws send code " + sendCode + " " + mes + " iswr " + (wr != null && this == wr.wsCurl) + " " + (wr != null && this == wr.wscCurl[0]));
		}
		virtual public void stop() {
			//wr.IsRetry = false;
			isRetry = false;
			releaseHandle();
		}
		virtual internal bool isActiveWs() {
			var ret = this == wr.wsCurl;
			return ret;
		}
		void releaseHandle() {
			try {
				lock(this) {
					if (easy != IntPtr.Zero) {
						Curl.curl_easy_cleanup(easy);
						easy = IntPtr.Zero;
					}
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		virtual internal void close() {
			util.debugWriteLine("on close wscurl");
			if (wr == null) return;
			
			if (isActiveWs())
				wr.onCloseProcess();
		}
		virtual internal void onOpen() {
			var mes = webSocketRecInfo[1]; 
			wsSend(mes);
		}
		virtual internal void onMessageReceive(string recvS) {
			util.debugWriteLine("receive curlws " + recvS + " iswr " + (this == wr.wsCurl) + " " + (this == wr.wscCurl[0]));
			wr.onMessageReceiveCore(recvS);
			if (recvS.IndexOf("\"ping\"") > -1) setPingTime();
		}
		virtual internal IntPtr setHeader(IntPtr easy) {
			return easy;
		}
		virtual internal bool isErrorMes(string recvS) {
			return false;
		}
		public void setPingTime() {
			lastPingTime = DateTime.Now;
		}
		virtual internal bool isTimeOut() {
			return DateTime.Now > lastPingTime + TimeSpan.FromSeconds(60);
		}
	}
	public class WebSocketCurlWSC : WebSocketCurl {
		public WebSocketCurlWSC() {}
		public WebSocketCurlWSC(RecordingManager rm, WebSocketRecorder wr, string[] webSocketInfo) : base(rm, wr, webSocketInfo) {
		}
		override internal void onOpen() {
			wr.onWscOpen(null, null);
		}
		override internal void onMessageReceive(string recvS) {
			util.debugWriteLine("receive curlws " + recvS + " iswr " + (this == wr.wsCurl) + " " + (this == wr.wscCurl[0]));
			wr.onWscMessageReceive(null, new MessageReceivedEventArgs(recvS, null));
		}
		override internal void close() {
			return;
		}
		override internal IntPtr setHeader(IntPtr easy) {
			return Curl.curl_add_header(easy, "Sec-WebSocket-Protocol: msg.nicovideo.jp#json");
		}
		override internal bool isActiveWs() {
			return this == wr.wscCurl[0];
		}
		override internal bool isTimeOut() {
			return false;
		}
	}
	public class WebSocketCurlWSCTS : WebSocketCurlWSC {
		private TimeShiftCommentGetter tscg = null;
		private DateTime lastReceiveTime = DateTime.Now;
		public WebSocketCurlWSCTS() {
		}
		public WebSocketCurlWSCTS(RecordingManager rm, TimeShiftCommentGetter tscg, string[] webSocketInfo) {
			this.rm = rm;
			this.tscg = tscg;
			this.webSocketRecInfo = webSocketInfo;
		}
		override internal void onOpen() {
			tscg.onWscOpen(null, null);
			Task.Factory.StartNew(() => sendPongAndConnectionCheck(), TaskCreationOptions.LongRunning);
		}
		override internal void onMessageReceive(string recvS) {
			//util.debugWriteLine("receive curlws " + recvS);
			tscg.onWscMessageReceive(null, new MessageReceivedEventArgs(recvS, null));
			lastReceiveTime = DateTime.Now;
		}
		override internal IntPtr setHeader(IntPtr easy) {
			return Curl.curl_add_header(easy, "Sec-WebSocket-Protocol: msg.nicovideo.jp#json");
		}
		override internal bool isErrorMes(string recvS) {
			try {
				return !recvS.StartsWith("{\"") || !recvS.EndsWith("}}");
			} catch (Exception e) {
				util.debugWriteLine("test convert curlws " + e.Message + e.Source + e.StackTrace + " " + recvS);
				return true;
			}
		}
		override internal bool isActiveWs() {
			return true;
		}
		void sendPongAndConnectionCheck() {
			while (isRetry) {
				wsSend("");
				Thread.Sleep(60000);
				if (DateTime.Now - lastReceiveTime > TimeSpan.FromSeconds(30)) {
					rm.form.addLogText("reconnect " + webSocketRecInfo[0].Substring(0, 10));
					stop();
				}
			}
		}
	}
}
