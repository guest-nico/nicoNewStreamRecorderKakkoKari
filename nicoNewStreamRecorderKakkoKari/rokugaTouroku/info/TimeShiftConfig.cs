/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/07/26
 * Time: 17:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace rokugaTouroku.info
{
	/// <summary>
	/// Description of TimeShiftConfig.
	/// </summary>
	public class TimeShiftConfig
	{
		//0-start time 1-continue
		private int startType = 0;
		public int h = 0;
		public int m = 0;
		public int s = 0;
		public int endH = 0;
		public int endM = 0;
		public int endS = 0;
		
		public bool isContinueConcat = false;
		public int timeSeconds = 0;
		public int timeType = 0; //0-record from 1-recorded until
		
		public int endTimeSeconds = 0;
		public bool isOutputUrlList;
		public string openListCommand;
		public bool isM3u8List;
		public double m3u8UpdateSeconds;
		public bool isOpenUrlList;
		
		public TimeShiftConfig(int startType, 
				int h, int m, int s, bool isContinueConcat)
		{
			this.startType = startType;
			this.h = h;
			this.m = m;
			this.s = s;
			this.isContinueConcat = isContinueConcat;
			
			timeSeconds = h * 3600 + m * 60 + s;
			timeType = (startType == 0) ? 0 : 1;
		}
		public TimeShiftConfig(int startType, 
				int h, int m, int s, int endH, int endM, int endS,
				bool isContinueConcat, bool isOutputUrlList, 
				string openListCommand, bool isM3u8List, 
				double m3u8UpdateSeconds, bool isOpenUrlList)
		{
			this.startType = startType;
			this.h = h;
			this.m = m;
			this.s = s;
			this.endH = endH;
			this.endM = endM;
			this.endS = endS;
			this.isContinueConcat = isContinueConcat;
			this.isOutputUrlList = isOutputUrlList;
			this.openListCommand = openListCommand;
			this.isM3u8List = isM3u8List;
			this.m3u8UpdateSeconds = m3u8UpdateSeconds;
			this.isOpenUrlList = isOpenUrlList;
			
			timeSeconds = h * 3600 + m * 60 + s;
			timeType = (startType == 0) ? 0 : 1;
			endTimeSeconds = endH * 3600 + endM * 60 + endS;
		}
		public TimeShiftConfig() : this(0, 0, 0, 0, 0, 0, 0, 
				false, false, "notepad {i}", false, 5, false) {}
	}
}
