/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2025/02/05
 * Time: 2:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using namaichi.info;
using namaichi.utility;
using Org.BouncyCastle.Utilities;

namespace namaichi.rec
{
	/// <summary>
	/// Description of DliveManager.
	/// </summary>
	public class DliveManager
	{
		RecordingManager rm = null;
		RecordFromUrl rfu = null;
		Record rec = null;
		CookieContainer cc = null;
		string masterUrl = null;
		M3u8Info audioM3u8 = null;
		M3u8Info videoM3u8 = null;
		
		TcpListener listener = null;
		string localUrl = null;
		public DliveManager(RecordingManager rm, RecordFromUrl rfu, Record rec, 
				CookieContainer container)
		{
			this.rm = rm;
			this.rfu = rfu;
			this.rec = rec;
			this.cc = container;
		}
		public void run(string masterUrl) {
			this.masterUrl = masterUrl;
			
			
			var port = int.Parse(rm.cfg.get("localServerPortList"));
			//port = 7993;
			var activeListener = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
			var activePorts = activeListener.Select(x => x.Port).Distinct().ToArray();
			for (var i = 7999; i > 7000; i--) {
				if (Array.IndexOf(activePorts, i) == -1) {
					port = i;
					break;
				}
			}
			
			localUrl = "http://127.0.0.1:" + port.ToString() + "/";
			
			Task.Factory.StartNew(() => getUrls(localUrl));
			runServer(port);
			rec.isRetry = true;
		}
		void getUrls(string localUrl) {
			readMaster(localUrl);
			while (rm.rfu == rfu && rec.isRetry) {
				audioM3u8.addUrl(read(audioM3u8.url));
				videoM3u8.addUrl(read(videoM3u8.url));
				Thread.Sleep(10000);
			}
		}
		void readMaster(string localUrl) {
			var r = read(masterUrl);
			if (r == null) {
				rm.form.addLogText("Master URLを読み込むことができませんでした " + ((masterUrl == null) ? "null" : masterUrl));
				return;
			}
			var m = new Regex("\"*(http[^\"\'\\s]+)").Matches(r);
			foreach (Match _m in m) {
				var url = _m.Groups[1].Value;
				var _r = read(url);
				if (url.IndexOf("main-audio") > -1 && audioM3u8 == null)
					audioM3u8 = new M3u8Info(url, getLocalUrlStr(_r), localUrl);
				if (url.IndexOf("main-video") > -1 && videoM3u8 == null)
					videoM3u8 = new M3u8Info(url, getLocalUrlStr(_r), localUrl);
			}
		}
		string read(string url) {
			var h = getHeader(url);
			var r = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", null, false);
			util.debugWriteLine("m3u8 read " + url + " " + (r == null ? "null" : r.Length.ToString()));
			return r;
		}
		void runServer(int port) {
			while (rm.rfu == rfu && rec.isRetry) {
				try {
					listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
					//listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					listener.Start();
					var m3u8Url = "http://127.0.0.1:" + port.ToString() + "/segment.m3u8";
					rm.form.addLogText(m3u8Url);
					
					var startTime = DateTime.Now;
					//var i = 0;
					/*
					Task.Run(() => {
			         	for (i = 0; i < tsUrlList.Length; i++) {
							Thread.Sleep(i == 0 || i == tsUrlList.Length - 1 ? 25000 : 5000);
							if (rm.rfu != rfu) break;
							util.debugWriteLine("sleep i " + i + " tsUrlList.Len " + tsUrlList.Length);
			         	}
					    util.debugWriteLine("m3u8 loop end");
						stop();
						i = -1;
					});
					*/
					Task.Factory.StartNew(() => launchPlayer(m3u8Url), TaskCreationOptions.LongRunning);
					
					while (rm.rfu == rfu && rec.isRetry) {
						try {
							using (var client = listener.AcceptTcpClient())
							using (var sr = new StreamReader(client.GetStream()))
							using (var sw = new StreamWriter(client.GetStream())) {
								var buf = new List<string>();
								while (true) {
									var l = sr.ReadLine();
									buf.Add(l);
									util.debugWriteLine("ab " + l);
									if (l == null || l.Length == 0) break;
									if (l.StartsWith("GET ")) {
										if (l.IndexOf(".m3u8") > -1) {
											writeM3u8(l, sw);
											//break;
										} else {
											var _url = util.getRegGroup(l, "(http.+) ");
											if (_url == null) break;
											var url = HttpUtility.UrlDecode(_url);
											util.debugWriteLine("url " + url);
											
											//var ver = url.IndexOf("key?") > -1 ? CurlHttpVersion.CURL_HTTP_VERSION_3 : CurlHttpVersion.CURL_HTTP_VERSION_2TLS;
											string d = null; 
											var b = new Curl().getBytes(url, getHeader(url), CurlHttpVersion.CURL_HTTP_VERSION_2TLS, "GET", d, true);
											client.GetStream().Write(b, 0, b.Length);
											client.GetStream().Flush();
										}
									} else {
										util.debugWriteLine("ttt " + l);
									}
								}
							}
						} catch (Exception ee) {
							util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
						}
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					rm.form.addLogText("何らかのエラーが発生しました " + e.Message + e.Source + e.StackTrace + e.TargetSite);
					Thread.Sleep(1000);
				} finally {
					stop();
				}
			}
			rm.form.addLogText("視聴情報の出力を完了しました。");
			Thread.Sleep(20000);
		}
		void writeM3u8(string m3u8Url, StreamWriter sw) {
			string res = null;
			if (m3u8Url.IndexOf("/segment.m3u8") > -1)
				res = getLocalUrlStr(read(masterUrl));
			if (m3u8Url.IndexOf("main-audio") > -1 && audioM3u8 != null)
				res = audioM3u8.getRes(rec.ri.timeShiftConfig, rec.ri.si.isTimeShift);
			if (m3u8Url.IndexOf("main-video") > -1 && videoM3u8 != null)
				res = videoM3u8.getRes(rec.ri.timeShiftConfig, rec.ri.si.isTimeShift);
			if (res == null) {
				rm.form.addLogText("ファイルの準備中です " + m3u8Url);
				return;
			}
			
			var bodyB = Encoding.ASCII.GetBytes(res);
			var buf = "HTTP/1.1 200 OK\r\n";
			buf += "content-type: application/vnd.apple.mpegurl\r\n";
			//buf += "Connection: Keep-Alive\r\n";
			buf += "Connection: close\r\n";
			buf += "Keep-Alive: timeout=15, max=100\r\n";
			buf += "Content-Length: " + bodyB.Length + "\r\n";
			buf += "\r\n";
			buf += res;
			
			var b = Encoding.ASCII.GetBytes(buf);
			sw.BaseStream.Write(b, 0, b.Length);
			sw.BaseStream.Flush();
		}
		string getLocalUrlStr(string originalUrlStr) {
			var a = new Regex("(https:[^\"\'\\s]+)").Replace(originalUrlStr, m => localUrl + HttpUtility.UrlEncode(m.Value));
			return a;
		}
		Dictionary<string, string> getHeader(string url) {
			var h = Curl.getDefaultHeaders("https://live.nicovideo.jp");
			h["Referer"] = "https://live.nicovideo.jp/";
			h["Cookie"] = cc.GetCookieHeader(new Uri(url));
			return h;
		}
		public void stop() {
			try {
				if (listener != null) listener.Stop();
				Thread.Sleep(2000);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
		void launchPlayer(string url) {
			rm.setHlsInfo(url, rec.ri);
			var p = new namaichi.play.Player(rm.form, rm.cfg);
			p.isUseCommentViewer = false;
			p.play();
		}
	}
}
