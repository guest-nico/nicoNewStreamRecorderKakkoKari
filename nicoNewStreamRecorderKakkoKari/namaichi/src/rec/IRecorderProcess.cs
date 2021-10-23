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
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of IRecorderProcess1.
	/// </summary>
	abstract public class IRecorderProcess
	{
		public DateTime tsHlsRequestTime = DateTime.MinValue;
		public TimeSpan tsStartTime;
		//public bool isTimeShift = false;
		public string msUri;
		public string[] msReq;
		public string msStoreUri;
		public string[] msStoreReq;
		//public long openTime;
		public bool isJikken;
		public string[] gotTsCommentList;
		public double firstSegmentSecond = -1;
		
		public DateTime endTime = DateTime.MinValue;
		public RecordInfo ri;
		
		public IRecorderProcess()
		{
		}
		abstract public void reConnect();
//		abstract public string[] getRecFilePath(long _openTime);
		abstract public string[] getRecFilePath();
		abstract public void sendComment(string s, bool is184);
		abstract public void resetCommentFile();
	}
}
