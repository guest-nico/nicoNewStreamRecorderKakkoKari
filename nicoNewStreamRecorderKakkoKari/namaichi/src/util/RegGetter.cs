/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/11/16
 * Time: 0:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace namaichi.utility
{
	/// <summary>
	/// Description of RegGetter.
	/// </summary>
	public class RegGetter
	{
		//private Regex resStreamDuration;
		private Regex extXTargetDuration;
		private Regex _extXTargetDuration;
		private Regex extInf;
		private Regex extXEndlist;
		private Regex extXMap;
		private Regex streamDuration;
		private Regex ts;
		private Regex ts2;
		private Regex fName;
		private Regex extXMediaSequence;
		private Regex maxNo;
		private Regex lastTsNum;
		private Regex renameWithoutTime_time;
		private Regex renameWithoutTime_num;
		
		private Regex wrVisit;
		private Regex wrComment;
		
		public RegGetter()
		{
		}
		/*
		public Regex getStreamDuration() {
			if (resStreamDuration == null)
				resStreamDuration = new Regex("(#STREAM-DURATION)");
			return resStreamDuration;
		}
		*/
		public Regex getExtXTargetDuration() {
			if (extXTargetDuration == null)
				extXTargetDuration = new Regex("#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*(e\\d+)*)");
			return extXTargetDuration;
		}
		public Regex getExtInf() {
			if (extInf == null)
				extInf = new Regex("^#EXTINF:(.+),");
			return extInf;
		}
		public Regex get_ExtXTargetDuration() {
			if (_extXTargetDuration == null)
				_extXTargetDuration = new Regex("^#EXT-X-TARGETDURATION:(\\d+(\\.\\d+)*(e\\d+)*)");
			return _extXTargetDuration;
		}
		public Regex getExtXMap() {
			if (extXMap == null)
				extXMap = new Regex("^#EXT-X-MAP:URI=\"(.+)\"");
			return extXMap;
		}
		public Regex getExtXEndlist() {
			if (extXEndlist == null)
				extXEndlist = new Regex("^(#EXT-X-ENDLIST)$");
			return extXEndlist;
		}
		public Regex getStreamDuration() {
			if (streamDuration == null)
				streamDuration = new Regex("#STREAM-DURATION:(.+)");
			return streamDuration;
		}
		public Regex getTs() {
			if (ts == null)
				ts = new Regex("(\\d+).(ts|mp4)");
			return ts;
		}
		public Regex getTs2() {
			if (ts2 == null)
				ts2 = new Regex("(.+?.(ts|mp4))\\?");
			return ts2;
		}
		public Regex getFName() {
			if (fName == null)
				fName = new Regex(".*(\\\\|/|^)(.+)");
			return fName;
		}
		public Regex getExtXMediaSequence() {
			if (extXMediaSequence == null)
				extXMediaSequence = new Regex("#EXT-X-MEDIA-SEQUENCE\\:(.+)");
			return extXMediaSequence;
		}
		public Regex getMaxNo() {
			if (maxNo == null)
				maxNo = new Regex("(\\d+)\\.(ts|mp4)");
			return maxNo;
		}
		public Regex getLastTsNum() {
			if (lastTsNum == null)
				lastTsNum = new Regex("[\\s\\S]+\n(\\d+).(ts|mp4)");
			return lastTsNum;
		}
		public Regex getRenameWithoutTime_time() {
			if (renameWithoutTime_time == null)
				renameWithoutTime_time = new Regex("(\\d+h\\d+m\\d+s)");
			return renameWithoutTime_time;
		}
		public Regex getRenameWithoutTime_num() {
			if (renameWithoutTime_num == null)
				renameWithoutTime_num = new Regex("\\d+h\\d+m\\d+s_(\\d+)");
			return renameWithoutTime_num;
		}
		
		//websocketRecorder
		public Regex getWrVisit() {
			if (wrVisit == null)
				//wrVisit = new Regex("{\"type\":\"watch\",\"body\":{\"command\":\"statistics\",\"params\":\\[\"(\\d+?)\",\"\\d+?\"");
				wrVisit = new Regex("\"data\":{\"viewers\":(\\d*),\"comments\":\\d*");
				
			return wrVisit;
		}
		public Regex getWrComment() {
			if (wrComment == null)
				wrComment = new Regex("\"data\":{\"viewers\":\\d*,\"comments\":(\\d*)");
			return wrComment;
		}
	}
}
