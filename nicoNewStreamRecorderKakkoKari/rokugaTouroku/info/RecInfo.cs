/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/20
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using rokugaTouroku.rec;

namespace rokugaTouroku.info
{
	/// <summary>
	/// Description of RecInfo.
	/// </summary>
	public class RecInfo {
		public string id;
		public string state;
		public string title;
		public string host;
		public string communityName;
		public string startTime;
		public string endTime;
		public string programTime;
		public string url;
		public string communityUrl;
		public string description;
		public string qualityRank;
		public string log = "";
		
		public info.TimeShiftConfig tsConfig;
		public Process process;
		public RecDataGetter rdg;
		
		public RecInfo(string id, string url, RecDataGetter rdg) {
			this.id = id;
			this.url = url;
			this.rdg = rdg;
			state = "待機中";
		}
		public RecInfo(string id, string title, 
				string host, string communityName,
				string startTime, string endTime,
				string programTime, string url, 
				string communityUrl, string description,
				string qualityRank, TimeShiftConfig tsConfig, 
				RecDataGetter rdg) {
			this.id = id;
			this.title = title;
			this.host = host;
			this.communityName = communityName;
			this.startTime = startTime;
			this.endTime = endTime;
			this.programTime = programTime;
			this.url = url;
			this.communityUrl = communityUrl;
			this.description = description;
			this.qualityRank = qualityRank;
			this.tsConfig = tsConfig;
			this.rdg = rdg;
		}
		public string Id
        {
            get { return id; }
            set { this.id = value; }
        }
        public string State
        {
            get { return state; }
            set { this.state = value; }
        }
        public string Title
        {
            get { return title; }
            set { this.title = value; }
        }
        public string Host
        {
            get { return host; }
            set { this.host = value; }
        }
        public string CommunityName
        {
            get { return communityName; }
            set { this.communityName = value; }
        }
        public string StartTime
        {
            get { return startTime; }
            set { this.startTime = value; }
        }
        public string EndTime
        {
            get { return endTime; }
            set { this.endTime = value; }
        }
        public string Log
        {
            get { return log; }
            set { this.log = value; }
        }
        public void addLog(string s) {
        	if (log != "") log += "\r\n";
        	log += s;
        	if (log.Length > 20000) 
				log = log.Substring(log.Length - 10000);
        }
	}
}
