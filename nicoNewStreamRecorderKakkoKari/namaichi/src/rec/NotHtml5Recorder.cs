/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/04/15
 * Time: 0:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace namaichi.rec
{
	/// <summary>
	/// Description of NotHtml5Recorder.
	/// </summary>
	public class NotHtml5Recorder
	{
		private string url;
		private CookieContainer container;
		private string lvid;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private bool isOfficial;
		private NotHtmlCommentGetter commentGetter;
		public NotHtml5Recorder(string url, CookieContainer container, 
				string lvid, RecordingManager rm, RecordFromUrl rfu)
		{
			this.url = url;
			this.container = container;
			this.lvid = lvid;
			this.rm = rm;
			this.rfu = rfu;
		}
		public void record(string res) {
			string getPlayerStatusInfo = getGetPlayerStatusInfo();
			//string res = util.getPageSource(url, container);
			res =  System.Web.HttpUtility.HtmlDecode(res);
			if (getPlayerStatusInfo == null) return ;
			
			while (util.getRegGroup(getPlayerStatusInfo, 
					"(<archive>0</archive>)") != null && rm.rfu == rfu) {
				util.debugWriteLine("record try");
				string[] recFolderFile = getNotHtmlRecFolderFile(res, getPlayerStatusInfo);
				if (recFolderFile == null) return;
				util.debugWriteLine("recff " + string.Join("  ",  recFolderFile));
				string[] command = getGetPlayerStatusCommand(getPlayerStatusInfo, recFolderFile);
				if (command == null) return;
				util.debugWriteLine("command " + string.Join("  ",  command));
				string[] messageInfo = getMessageInfo(getPlayerStatusInfo);
				bool isFFmpeg = Array.IndexOf(command, "-i") != -1;
				util.debugWriteLine("isffmpeg " + isFFmpeg);
				
				
				//var rec = new Record(rm, isFFmpeg, rfu, "", "", null, false, null, null, null, 0, null, null, false, null);
				var rec = new Record(rm, rfu, "", null, null, null, null);
				commentGetter = new NotHtmlCommentGetter(messageInfo, rm, rfu, recFolderFile);
				
				/*
				var recTask = Task.Run(() => rec.recordCommand(command));
				var commentTask = Task.Run(() => commentGetter.start());
				recTask.Wait();
				commentGetter.close();
				*/
				System.Threading.Thread.Sleep(3000);
		       
		        getPlayerStatusInfo = getGetPlayerStatusInfo();
		        if (getPlayerStatusInfo == null) break;
		        
			}

			
		}
		public string getGetPlayerStatusInfo() {
			//var a = new WebHeaderCollection();
			var res = util.getPageSource("https://live.nicovideo.jp/api/getplayerstatus?v=" + lvid, container);
//			util.debugWriteLine(res);
			return res;
		}
		private String[] getNotHtmlRecFolderFile(string res, string getPlayerStatusInfo) {
			isOfficial = (getPlayerStatusInfo.IndexOf("<provider_type>official</provider_type>") < 0) ? false : true;
			bool isChannel = (getPlayerStatusInfo.IndexOf("<provider_type>channel</provider_type>") < 0) ? false : true;
			
			string host, group, communityNum, userId;
			userId = "";
			if (!isOfficial && !isChannel) {
				group = util.getRegGroup(res, "class=\"commu_name\" title=\"(.*?)\"");
				group = System.Web.HttpUtility.UrlDecode(group);
						
				communityNum = util.getRegGroup(res, ",\"group_id\":\"(.*?)\"");
		
				host = util.getRegGroup(res, "%3Cowner_name%3E(.*?)%3C");
				host = System.Web.HttpUtility.UrlDecode(host);
				
			} else {
				group = util.getRegGroup(res, "class=\"ch_name\" title=\"(.*?)\"");
				if (group == null && isOfficial) group = "official";
				if (group != null) 
					group =  System.Web.HttpUtility.UrlDecode(group);
				
				communityNum = (isOfficial) ? "official" : 
						util.getRegGroup(res, ",\"group_id\":\"(.*?)\"");
				host = util.getRegGroup(res, "class=\"company\" title=\"(.*?)\"");
				
				if (isOfficial) {
					string cn = util.getRegGroup(res, "<a href=\".*?(ch[0-9]*)\".*class=\"ch_name\"");
	//				System.out.println(cn);
					communityNum = (cn != null) ? cn : "official";
					if (cn != null) communityNum = cn;
					if (host == null) host = "公式生放送";
				}
	//			communityNum = util.uniToOriginal(util.getRegGroup(res, ",\"group_id\":\"(.*?)\""));
				
				
			}
//			string title = util.getRegGroup(res, "Nicolive_JS_Conf.Watch = \\{.*?\"videoTitle\":\"(.*?)\"");
			string title = util.getRegGroup(getPlayerStatusInfo, "<title>(.+)</title>");
			string lvid = util.getRegGroup(url, "(lv[0-9]+)");
			
			util.debugWriteLine("host group title " + host + " " + group + " " + title + " " + lvid + " " + communityNum);
			
	//		System.out.println(String.join(" ", util.getRecFolderFilePath(host, group, title, lvid, communityNum)));
			
			return util.getRecFolderFilePath(host, group, title, lvid, communityNum, userId, rm.cfg, false, null, 0, false, false, false, rm.form);
		}
		private string[] getGetPlayerStatusCommand(string getPlayerStatusInfo, string[] recFolderFile) {
			//0-ticket 1-contentsUrl 2-rtmpUrl 3-lvid
			bool isOfficial = (getPlayerStatusInfo.IndexOf("<provider_type>official</provider_type>") < 0) ? false : true;
			string[] getPlayerStatusCommandVal = getPlayersStatusCommandVal(isOfficial, getPlayerStatusInfo);
	//		System.out.println("getplaycoommandval " + String.join(" ", getPlayerStatusCommandVal));
			
			return getCommand(isOfficial, getPlayerStatusCommandVal, recFolderFile);
			
		}
		private String[] getPlayersStatusCommandVal(bool isOfficial, string getPlayerStatusInfo) {

			if (!isOfficial) {
				util.debugWriteLine("not official");
				string ticket = util.getRegGroup(getPlayerStatusInfo, "<ticket>(.+?)</ticket>");
				string contentsUrl = util.getRegGroup(getPlayerStatusInfo, "<contents_list>.+?(rtmp://.+?)<");
				string rtmpUrl = util.getRegGroup(getPlayerStatusInfo, "<url>(.+?)</url>");
				string lvid = util.getRegGroup(getPlayerStatusInfo, "<id>(.+?)</id>");
				util.debugWriteLine(ticket + " " + contentsUrl + " " + rtmpUrl + " " + lvid); 
				if (ticket == null || contentsUrl == null || rtmpUrl == null || lvid == null) return null;
				
				util.debugWriteLine(ticket);
				util.debugWriteLine(contentsUrl);
				util.debugWriteLine(rtmpUrl);
				util.debugWriteLine(lvid);
				string[] ret = {ticket, contentsUrl, rtmpUrl, lvid};
				return ret;
			} else {
				string contentsCases = util.getRegGroup(getPlayerStatusInfo, "<contents .+?>case:(.+?)</contents>");
	//			System.out.println(contentsCases);
				string decodedContentsCases = null, decodedTickets = null;
				string premiumContentsUrl0, premiumContentsUrl1;
				
				try {
					decodedContentsCases = System.Web.HttpUtility.HtmlDecode(contentsCases);
					premiumContentsUrl0 = util.getRegGroup(decodedContentsCases, "premium.+?(rtmp.+?),.+?,");
					premiumContentsUrl1 = util.getRegGroup(decodedContentsCases, "premium.+?rtmp.+?,(.+?),");
					string tickets = util.getRegGroup(getPlayerStatusInfo, "<tickets>(.+?)</tickets>");
					string ticketUid = util.getRegGroup(tickets, premiumContentsUrl1 + "\">(uid=.+?)</stream>");
					
					if (ticketUid != null) {
						
						decodedTickets = ticketUid.Replace("&amp;", "&");
						util.debugWriteLine(premiumContentsUrl0);
						util.debugWriteLine(premiumContentsUrl1);
						util.debugWriteLine(tickets);
						util.debugWriteLine(ticketUid);
						util.debugWriteLine(decodedTickets);
						string url = "\"" + premiumContentsUrl0 + "/" + premiumContentsUrl1 + "?" + decodedTickets + "\"";
						util.debugWriteLine(url);
						string[] ret = {url, "rtmpdump"};
						return ret;
					} else {
	//					String decodedContentsCases = null, decodedTickets = null;
						string ticket = util.getRegGroup(getPlayerStatusInfo, "<ticket>(.+?)</ticket>");
						string stream = util.getRegGroup(getPlayerStatusInfo, "<stream name=\"" + premiumContentsUrl1 + "\">(.*?)</stream>");
						stream = stream.Replace("&amp;", "&");
						util.debugWriteLine("ticket " + ticket + " precon0 " + premiumContentsUrl0);
						util.debugWriteLine("precon1 " + premiumContentsUrl1 + " stream " + stream);
						string url = "\"" + premiumContentsUrl0 + "/" + premiumContentsUrl1 + "?" + stream + " live=1\"";
						string[] ret = {url, "ffmpeg"};
						return ret;
					}
					
				} catch (Exception e) {util.debugWriteLine(e.Message + e.StackTrace);}
			}
			return null;
		}
		private string[] getCommand(bool isOfficial, string[] getPlayerStatusCommandVal, string[] recFolderFile) {
			string[] val = getPlayerStatusCommandVal;
			if (!isOfficial) {
				//0-ticket 1-contentsUrl 2-rtmpUrl 3-lvid
				string[] command = new string[] {
						"-vr",
						val[2] + "/" + val[3],
						"-N", val[1], "-C",
						"S:" + val[0], "-o", 
						recFolderFile[1] + ".flv"};
				return command;
			} else {
				//0-premiumContentsUrl0 1-premiumContentsUrl1 2-ticketUid
				if (val[1].Equals("rtmpdump")) 
					return new string[] { 
							"-vr", val[0], "-o", recFolderFile[1] + ".flv"};
				else return new string[] { 
						"-i", val[0], "-codec", "copy", recFolderFile[1] + ".flv"};
			}
		}
		private string[] getMessageInfo(string getPlayerStatusInfo) {
			string url = util.getRegGroup(getPlayerStatusInfo, "<addr>(.+?)</addr>");
			string port = util.getRegGroup(getPlayerStatusInfo, "<port>(.+?)</port>");
			string thread = util.getRegGroup(getPlayerStatusInfo, "<thread>(.+?)</thread>");
			return new string[]{url, port, thread};
		}

	}
	class NotHtmlCommentGetter {
		private string[] messageInfo;
		//private bool isAlive = true;
		private RecordingManager rm;
		private RecordFromUrl rfu;
		private string[] recFolderFileInfo;
		
		public NotHtmlCommentGetter(string[] messageInfo, RecordingManager rm, RecordFromUrl rfu, string[] recFolderFileInfo) {
			//0-url 1-port 2-thread
			this.messageInfo = messageInfo;
			this.rm = rm;
			this.rfu = rfu;
			this.recFolderFileInfo = recFolderFileInfo;
		}
		
		public Boolean start() {
			util.debugWriteLine("comment start");
			
			/*
			Socket s = null;
			BufferedReader br = null;
			BufferedWriter bw = null;
			BufferedWriter fileBw = null;
			try {
				s = new Socket(messageInfo[0], Integer.valueOf(messageInfo[1]));
				br = new BufferedReader(new InputStreamReader(s.getInputStream()));
				bw = new BufferedWriter(new OutputStreamWriter(s.getOutputStream()));
				fileBw = new BufferedWriter(new FileWriter(recFolderFileInfo[1] + ".xml", true));
				
				bw.write("<thread thread=\"" + messageInfo[2] + "\" version=\"20061206\" res_from=\"-10\" scores=\"1\" />\0");
				bw.flush();
				
				while(isAlive && gui.recBtn.getText() == "stop") {
					char a = (char)br.read();
					if (a == -1) break;
					if (a == 0) a = '\n';
					fileBw.write(a);
					if (a == '\n') fileBw.flush();
				}
				
				
			} catch (IOException e) {
				e.printStackTrace();
			} finally {
				s.close();
				br.close();
				bw.close();
				fileBw.close();
			}
			*/
			return true;
		}
		public void close() {
	//		gui.addLogText(isAlive + " " + gui.recBtn.getText());
			//isAlive = false;
		}
	}
}
