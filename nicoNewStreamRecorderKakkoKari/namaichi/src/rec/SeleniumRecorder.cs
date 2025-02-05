/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2020/07/19
 * Time: 16:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace namaichi.rec
{
	/// <summary>
	/// Description of SeleniumRecorder.
	/// </summary>
	public class SeleniumRecorder
	{
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private ChromeDriver driver = null;
		private Record _rec;
		private string m3u8Url = null;
		private string recfolder;
		private string quality = "not found";
		private StreamWriter commentSW = null;
		private long servertime = util.getUnixTime() - 9 * 60 * 60;
		private string lastRecId = null;
		public SeleniumRecorder(RecordingManager rm, RecordFromUrl rfu)
		{
			this.rm = rm;
			this.rfu = rfu;
			recfolder = util.getRecFolderFilePath("n", "g", "t", rfu.lvid, "c", "u", rm.cfg, false, null, 0, false)[1];
		}
		public int rec() {
			try {
				util.debugWriteLine("selenium rec " + rfu.id);
				
				
				var opt = new ChromeOptions();
				opt.PerformanceLoggingPreferences = new ChromePerformanceLoggingPreferences();
				opt.SetLoggingPreference("performance", LogLevel.All);
				//opt.AddArgument("user-data-dir=" + util.getJarPath()[0] + "/chrome");
				//opt.AddArgument("disk-cache-dir=z:aa");
				
				driver = new ChromeDriver(opt);
				Application.ApplicationExit += (o, ee) => close();
	            
	            driver.Navigate().GoToUrl("https://live2.nicovideo.jp/watch/" + rfu.lvid);
	            
	            while (rm.rfu == rfu) {
		            try {
		            	var log = driver.Manage().Logs.GetLog("performance");
		            	foreach (var l in log) {
		            		/*
		            		if (l.Message.IndexOf("\"wss:") > -1) {
		            			var url = util.getRegGroup(l.Message, "\"(wss://.+?)\"");
		            			if (url != null) util.debugWriteLine(url);
		            		}
		            		*/
		            		if (l.Message.IndexOf("webSocketFrameSent") > -1 &&
		            		    	l.Message.IndexOf("\"quality\":\"") > -1)
		            			quality = util.getRegGroup(l.Message, "\"quality\"\\:\"(.+?)\"");
		            		if (l.Message.IndexOf("master.m3u8") > -1) {
		            			
		            			var url = util.getRegGroup(l.Message, ",\"url\"\\:\"(https://[^\"]+?.m3u8.*?)\"");
		            			if (url != null) {
		            				util.debugWriteLine("m3u8 url " + url);
		            				record(url, l.Message);
		            			}
		            		}
		            		if (l.Message.IndexOf("\\\"thread\\\"") > -1)
		            			onReceiveChat(l.Message);
		            	}
		            	
		            } catch (Exception ee) {
		            	util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
		            	
		            }
	            	Thread.Sleep(500);
	            }
	            
	            //foreach (var c in driver.Manage().Cookies.AllCookies) {
	            //	util.debugWriteLine(c.Domain + " " + c.Name + " " + c.Value);
	            //}
	            //close();
				return 0;
			} catch (Exception e) {
				util.debugWriteLine("selenium exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				return 0;
			} finally {
				close();
			}
		}
		private void close() {
			try {
				if (driver != null) {
					driver.Close();
		            driver.Quit();
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			try {
				if (commentSW != null) {
					commentSW.WriteLine("</packet>");
					commentSW.Close();
				}
					
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void record(string url, string msg) {
			util.debugWriteLine("record " + url + " " + msg);
			var docId = util.getRegGroup(msg, "\"documentURL\"\\:\".+?(lv\\d+)");
			if (docId == null) return;
			
			if (lastRecId != docId)
				_rec = null;
			
			if (_rec == null) {
				m3u8Url = url;
				recfolder = util.getRecFolderFilePath("n", "g", "t", docId, "c", "u", rm.cfg, false, null, 0, false)[1];
		        _rec = new Record(rm, true, rfu, url, recfolder, null, false, null, rfu.lvid, null, 0, null, recfolder, false, null);
				lastRecId = docId;        
		        Task.Run(() => {
			        _rec.record(quality);
					if (_rec.isEndProgram) {
						util.debugWriteLine("stop websocket recd");
						
					}
				});
				
			} else {
				
					_rec.reSetHlsUrl(url, quality, null, false);
				
	        }
		}
		private void onReceiveChat(string msg) {
			try {
				util.debugWriteLine("chat msg receive " + msg);
				if (msg.IndexOf("webSocketFrameSent") > -1) return;
				var payload = util.getRegGroup(msg, "payloadData\":\"(.+?[^\\\\])\"");
				var dec = payload.Replace("\\\"", "\"");
				util.debugWriteLine("chat msg dec " + dec);
				
				//if (servertime == 0) {
					var t = util.getRegGroup(dec, "\"server_time\"\\:(\\d+)");
					if (t != null) servertime = int.Parse(t);
					//else servertime = util.getUnixTime() - 9 * 60 * 60;
				//}
				
				var xml = JsonConvert.DeserializeXNode(dec);
				var chatinfo = new namaichi.info.ChatInfo(xml);
				var chatXml = chatinfo.getFormatXml(servertime);
				
				if (commentSW == null) {
					commentSW = new StreamWriter(recfolder + ".xml");
					commentSW.WriteLine("<packet>");
				}
				commentSW.WriteLine(chatXml);
				util.debugWriteLine("message write " + chatXml);
				
				addDisplayComment(chatinfo);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void addDisplayComment(namaichi.info.ChatInfo chat) {
			
			if (chat.root.Equals("thread")) return;
			if (chat.contents == "再読み込みを行いました<br>読み込み中のままの方はお手数ですがプレイヤー下の更新ボタンをお試し下さい") {
				util.debugWriteLine("chat 再読み込みを行いました");
				return;
			}
			if (chat.contents == null) return;
			var __time = chat.date - servertime;
			var isMinus = __time < 0;
			if (isMinus) __time *= -1;
			var h = (int)(__time / (60 * 60));
			var m = (int)((__time % (60 * 60)) / 60);
			var _m = (m < 10) ? ("0" + m.ToString()) : m.ToString();
			var s = __time % 60;
			var _s = (s < 10) ? ("0" + s.ToString()) : s.ToString();
			var keikaTime = h + ":" + _m + ":" + _s + "";
			if (isMinus) keikaTime = "-" + keikaTime;

			var c = (chat.premium == "3") ? "red" :
				((chat.premium == "7") ? "blue" : "black");
			
			rm.form.addComment(keikaTime, chat.contents, chat.userId, chat.score, c);
			
		}
	}
}
