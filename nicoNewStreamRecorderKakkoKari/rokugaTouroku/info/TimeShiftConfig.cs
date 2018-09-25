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
		private int h = 0;
		private int m = 0;
		private int s = 0;
		
		public bool isContinueConcat = false;
		public int timeSeconds = 0;
		public int timeType = 0; //0-record from 1-recorded until
		
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
	}
}
