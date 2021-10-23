/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/07/26
 * Time: 17:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace namaichi.info
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
		private int h = 0;
		private int m = 0;
		private int s = 0;
		private int endH = 0;
		private int endM = 0;
		private int endS = 0;
		
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
		public bool isAfterStartTimeComment;
		public bool isBeforeEndTimeComment;
		public bool isOpenTimeBaseStartArg;
		public bool isOpenTimeBaseEndArg;
		//public int lastSegmentNo = -1;
		public string[] lastFileTime = null;
		public string lastFileName = null;
		public string[] qualityRank = null;
		//public string startTimeStr;
		
		public TimeShiftConfig(int startType, 
				int h, int m, int s, int endH, int endM, int endS,
				bool isContinueConcat, bool isOutputUrlList, 
				string openListCommand, bool isM3u8List, 
				double m3u8UpdateSeconds, bool isOpenUrlList,
				bool isVposStartTime, int startTimeMode, int endTimeMode,
				bool isAfterStartTimeComment, bool isBeforeEndTimeComment,
				bool isDeletePosTime, string[] qualityRank)
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
			/*
			timeSeconds = h * 3600 + m * 60 + s;
			timeType = (startType == 0) ? 0 : 1;
			endTimeSeconds = endH * 3600 + endM * 60 + endS;
			*/
			timeSeconds = (startTimeMode == 0) ? 0 : (h * 3600 + m * 60 + s);
			timeType = (startType == 0) ? 0 : 1;
			endTimeSeconds = (endTimeMode == 0) ? 0 : (endH * 3600 + endM * 60 + endS);
			
			if (startType == 0) this.isContinueConcat = false;
			this.isAfterStartTimeComment = isAfterStartTimeComment;
			this.isBeforeEndTimeComment = isBeforeEndTimeComment;
			this.isDeletePosTime = isDeletePosTime;
			this.qualityRank = qualityRank;
		}
		public TimeShiftConfig() : this(0, 0, 0, 0, 0, 0, 0, 
				false, false, "notepad {i}", false, 5, false, false, 0, 0, false, false, true, null) {}
		public TimeShiftConfig clone() {
			return new TimeShiftConfig(startType, h, m, s,
					endH, endM, endS, isContinueConcat,
					timeSeconds, timeType, endTimeSeconds,
					isOutputUrlList, openListCommand, isM3u8List,
					m3u8UpdateSeconds, isOpenUrlList, isVposStartTime, 
					startTimeMode, endTimeMode, isAfterStartTimeComment,
					isOpenTimeBaseStartArg, isOpenTimeBaseEndArg,
					isBeforeEndTimeComment, isDeletePosTime, qualityRank
				);
		}
		public TimeShiftConfig(int startType, int h, int m, int s, 
				int endH, int endM, int endS, bool isContinueConcat,
				int timeSeconds, int timeType, int endTimeSeconds,
				bool isOutputUrlList, string openListCommand,
				bool isM3u8List, double m3u8UpdateSeconds, 
				bool isOpenUrlList, bool isVposStartTime, 
				int startTimeMode, int endTimeMode, 
				bool isAfterStartTimeComment, 
				bool isOpenTimeBaseStartArg, bool isOpenTimeBaseEndArg,
				bool isBeforeEndTimeComment, bool isDeletePosTime,
				string[] qualityRank) {
			this.startType = startType;
			this.h = h;
			this.m = m;
			this.s = s;
			this.endH = endH;
			this.endM = endM;
			this.endS = endS;
			this.isContinueConcat = isContinueConcat;
			this.timeSeconds = timeSeconds;
			this.timeType = timeType;
			this.endTimeSeconds = endTimeSeconds;
			this.isOutputUrlList = isOutputUrlList;
			this.openListCommand = openListCommand;
			this.isM3u8List = isM3u8List;
			this.m3u8UpdateSeconds = m3u8UpdateSeconds;
			this.isOutputUrlList = isOpenUrlList;
			this.isVposStartTime = isVposStartTime;
			this.startTimeMode = startTimeMode;
			this.endTimeMode = endTimeMode;
			this.isAfterStartTimeComment = isAfterStartTimeComment;
			this.isOpenTimeBaseStartArg = isOpenTimeBaseStartArg;
			this.isOpenTimeBaseEndArg = isOpenTimeBaseEndArg;
			this.isBeforeEndTimeComment = isBeforeEndTimeComment;
			this.isDeletePosTime = isDeletePosTime;
			this.qualityRank = qualityRank;
		}
	}
}
