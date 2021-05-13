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
		public int startTimeMode = 0;
		public int endTimeMode = 0;
		
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
		public bool isDeletePosTime = false;
		public bool isOutputUrlList;
		public string openListCommand;
		public bool isM3u8List;
		public double m3u8UpdateSeconds;
		public bool isOpenUrlList;
		public bool isVposStartTime;
		
		public string startTimeStr;
		public bool isAfterStartTimeComment;
		public bool isBeforeEndTimeComment;
		public bool isOpenTimeBaseStartArg;
		public bool isOpenTimeBaseEndArg;
		
		public TimeShiftConfig(int startType, 
				int h, int m, int s, bool isContinueConcat, 
				bool isVposStartTime, int startTimeMode, int endTimeMode,
				bool isAfterStartTimeComment, bool isOpenTimeBaseStart,
				bool isOpenTimeBaseEnd, bool isBeforeEndTimeComment,
				bool isDeletePosTime)
		{
			this.startType = startType;
			this.h = h;
			this.m = m;
			this.s = s;
			this.isContinueConcat = isContinueConcat;
			this.isVposStartTime = isVposStartTime;
			this.startTimeMode = startTimeMode;
			this.endTimeMode = endTimeMode;
			
			timeSeconds = h * 3600 + m * 60 + s;
			timeType = (startType == 0) ? 0 : 1;
			startTimeStr = (startType == 0) ? (timeSeconds + "s") :
				((isContinueConcat) ? "continue-concat" : "continue");
			this.isAfterStartTimeComment = isAfterStartTimeComment;
			this.isBeforeEndTimeComment = isBeforeEndTimeComment;
			this.isOpenTimeBaseStartArg = isOpenTimeBaseStart;
			this.isOpenTimeBaseEndArg = isOpenTimeBaseEnd;
			this.isDeletePosTime = isDeletePosTime;
		}
		public TimeShiftConfig(int startType, 
				int h, int m, int s, int endH, int endM, int endS,
				bool isContinueConcat, bool isOutputUrlList, 
				string openListCommand, bool isM3u8List, 
				double m3u8UpdateSeconds, bool isOpenUrlList,
				bool isVposStartTime, int startTimeMode, int endTimeMode,
				bool isAfterStartTimeComment, bool isOpenTimeBaseStart,
				bool isOpenTimeBaseEnd, bool isBeforeEndTimeComment,
				bool isDeletePosTime)
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
			this.isVposStartTime = isVposStartTime;
			this.startTimeMode = startTimeMode;
			this.endTimeMode = endTimeMode;
			
			timeSeconds = (startTimeMode == 0) ? 0 : (h * 3600 + m * 60 + s);
			timeType = (startType == 0) ? 0 : 1;
			endTimeSeconds = (endTimeMode == 0) ? 0 : (endH * 3600 + endM * 60 + endS);
			
			startTimeStr = (startType == 0) ? (timeSeconds + "s") :
				((isContinueConcat) ? "continue-concat" : "continue");
			this.isAfterStartTimeComment = isAfterStartTimeComment;
			this.isOpenTimeBaseStartArg = isOpenTimeBaseStart;
			this.isOpenTimeBaseEndArg = isOpenTimeBaseEnd;
			this.isBeforeEndTimeComment = isBeforeEndTimeComment;
			this.isDeletePosTime = isDeletePosTime;
		}
		public TimeShiftConfig() : this(0, 0, 0, 0, 0, 0, 0, 
				false, false, "notepad {i}", false, 5, false, false, 0, 0, false, false, false, false, true) {}
	}
}
