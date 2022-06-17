/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/31
 * Time: 18:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using namaichi.info;
using WebSocket4Net;

namespace namaichi.rec
{
	/// <summary>
	/// Description of IRecorderProcess1.
	/// </summary>
	abstract public class IRecorderProcess
	{
		public CookieContainer container = null;
		internal RecordingManager rm;
		internal RecordFromUrl rfu;
		public DateTime tsHlsRequestTime = DateTime.MinValue;
		public TimeSpan tsStartTime;
		//public bool isTimeShift = false;
		public string msUri;
		public string[] msReq;
		public string msStoreUri;
		public string[] msStoreReq;
		//public long openTime;
		public bool isJikken;
		public bool isHokan = false;
		public string[] gotTsCommentList;
		public double firstSegmentSecond = -1;
		
		public long serverTime;
		public DateTime endTime = DateTime.MinValue;
		public RecordInfo ri;
		public bool isSaveComment = false;
		public long sync = 0;
		public bool IsRetry = true;
		
		public string visitCount = "0";
		public string commentCount = "0";
		public ITimeShiftCommentGetter tscg = null;
		public List<string[]> commentReplaceList = null;
		public List<string> chaseCommentBuf = new List<string>();
		public TimeSpan jisa;
		
		public IRecorderProcess()
		{
		}
		abstract public void reConnect();
		abstract public void reConnect(WebSocket ws);
//		abstract public string[] getRecFilePath(long _openTime);
		abstract public string[] getRecFilePath();
		abstract public void sendComment(string s, bool is184);
		abstract public void resetCommentFile();
		abstract public void setSync(int no, double second, string m3u8Url);
		//abstract public void setRealTimeStatistics();
		public void setRealTimeStatistics() {
			try {
				if (!visitCount.StartsWith("-")) {
			    	Thread.Sleep(10000);
			    	string visit, comment;
			    	var ret = getStatistics(rfu.lvid, container, out visit, out comment);
			    	if (ret) {
			    		if (!visitCount.StartsWith("-")) {
				    		visitCount = "-" + visit;
				    		commentCount = "-" + comment;
			    		}
			    	}
			    }
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		abstract internal bool getStatistics(string lvid, CookieContainer cc, out string visit, out string comment);
		abstract public void stopRecording();
		abstract public void chaseCommentSum();
	}
}
