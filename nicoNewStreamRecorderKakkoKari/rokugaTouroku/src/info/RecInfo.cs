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
using System.Windows.Forms;
using System.Drawing;
using rokugaTouroku.rec;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace rokugaTouroku.info
{
	/// <summary>
	/// Description of RecInfo.
	/// </summary>
	public class RecInfo {
		public string id = "";
		public string state;
		public string title = "";
		public string host;
		public string communityName = "";
		public string startTime = "";
		public string endTime = "";
		public string programTime = "";
		public string keikaTime = "";
		public string url;
		public string communityUrl;
		public string description = "";
		public string qualityRank = "";
		public string log = "";
		public string afterConvertType;
		public string timeShift;
		public Image samune;
		public string quality;
		public string recComment = "";
		public bool isChase = false;
		
		public info.TimeShiftConfig tsConfig;
		public Process process;
		public RecDataGetter rdg;
		public DateTime keikaTimeStart;
		public string samuneUrl = null;
		public AccountInfo ai = null;
		
		public RecInfo() {
			
		}
		public RecInfo(string id, string url, RecDataGetter rdg, string afterConvertType, info.TimeShiftConfig tsConfig, string tsStr, string qualityRankStr, string qualityRank, string recComment, bool isChase, AccountInfo ai) {
			this.id = id;
			var lvM = Regex.Match(url, "lv\\d+");
			if (lvM != null && !string.IsNullOrEmpty(lvM.Value)) {
				//if (url.IndexOf("live.nicovideo.jp") != -1)
					this.url = "https://live.nicovideo.jp/watch/" + lvM.Value;
				//else this.url = "https://live2.nicovideo.jp/watch/" + lvM.Value;
			} else this.url = url;
			this.rdg = rdg;
			this.state = "待機中";
			//this.afterFFmpegMode = afterFFmpegMode;
			this.afterConvertType = afterConvertType;
			this.timeShift = tsStr;
			this.tsConfig = tsConfig;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			samune = ((System.Drawing.Image)(resources.GetObject("samuneBox.Image")));
			//quality = "sHigh,High,Normal,Llow,sLow,abr";
			quality = qualityRankStr;
			this.qualityRank = qualityRank;
			this.recComment = recComment;
			this.isChase = isChase;
			this.ai = ai;
		}
		public RecInfo(string id, string title, 
				string host, string communityName,
				string startTime, string endTime,
				string programTime, string url, 
				string communityUrl, string description,
				string qualityRank, TimeShiftConfig tsConfig, 
				RecDataGetter rdg, string recComment, bool isChase,
				AccountInfo ai) {
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
			this.recComment = recComment;
			this.isChase = isChase;
			this.ai = ai;
		}
		public void setHosoInfo(MainForm form) {
         	var hig = new namaichi.rec.HosoInfoGetter();
         	var ret = false;
         	for (var i = 0; i < 3; i++) {
	         	ret = hig.get(url);
	         	if (ret) break;
	         	System.Threading.Thread.Sleep(3000);
         	}
         	if (!ret) return;
         	//return;
         	
         	if (string.IsNullOrEmpty(title) && 
         	    	!string.IsNullOrEmpty(hig.title)) 
         		title = hig.title;
			if (string.IsNullOrEmpty(host) && 
         	    	!string.IsNullOrEmpty(hig.userId)) {
         		var _host = hig.userName;
         		if (_host != null) host = _host;
         	}
         	if (string.IsNullOrEmpty(communityName) && 
         	    	!string.IsNullOrEmpty(hig.communityId)) {
         		var isFollow = false;
         		var _comName = util.getCommunityName(hig.communityId, out isFollow, null);
         		if (_comName != null) communityName = _comName;
         		var comUrl = (hig.communityId.StartsWith("ch")) ?
						("https://ch.nicovideo.jp/" + hig.communityId) :
						("https://com.nicovideo.jp/community/" + hig.communityId);
         		communityUrl = comUrl;
         	}
         	form.updateRecListCell(this);
         	form.saveList();
		}
		public string Id
        {
            get { return id; }
            set { this.id = value; }
        }
        /*
        public string ProgramTime
        {
            get { return programTime; }
            set { this.programTime = value; }
        }
        */
        /*
        public string KeikaTime
        {
            get { return keikaTime; }
            set { this.keikaTime = value; }
        }
        */
        
        public string AfterConvertType  {
        	get { return afterConvertType; }
        	set {this.afterConvertType = value; }
        }
        
        public string Quality  {
        	get { return quality; }
        	set {this.quality = value; }
        }
       	public string TimeShift  {
        	get { return timeShift; }
        	set {this.timeShift = value; }
        }
		public string Chase  {
       		get { return isChase ? "追" : ""; }
       		set {this.isChase = value.Equals("追"); }
        }
        public string RecComment  {
        	get { return recComment; }
        	set {this.recComment = value; }
        }
       public string State
        {
            get { return state; }
            set { this.state = value; }
        }
       public string Account  {
       	get {
       		if (ai == null || ai.isRecSetting) return "録画ツールの設定を使用";
       		else if (ai.isBrowser) {
       			return ai.si != null ? (ai.si.BrowserName + " " + ai.si.ProfileName) : "録画ツールの設定を使用";
       		} else return "アカウント";
       	}
       		set { }
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
        public string getAfterConvertTypeNum() {
        	var t = afterConvertType;
			if (t == "処理しない") return "0";
			if (t == "形式を変更せず処理する") return "1";
			if (t == "ts") return "2";
			if (t == "avi") return "3";
			if (t == "mp4") return "4";
			if (t == "flv") return "5";
			if (t == "mov") return "6";
			if (t == "wmv") return "7";
			if (t == "vob") return "8";
			if (t == "mkv") return "9";
			if (t == "mp3(音声)") return "10";
			if (t == "wav(音声)") return "11";
			if (t == "wma(音声)") return "12";
			if (t == "aac(音声)") return "13";
			if (t == "ogg(音声)") return "14";
			return "0";
        }
		public void readHandler(object o, DataReceivedEventArgs e) {
       		try {
       			util.debugWriteLine("read " + e.Data);
       			if (e.Data == null) return;
       			rdg.setInfo(e.Data, this);
       		} catch (Exception ee) {
       			util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
       		}
		}
	}
}
