/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/10
 * Time: 16:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using rokugaTouroku.info;
using rokugaTouroku.rec;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private rokugaTouroku.config.config config = new rokugaTouroku.config.config();
		private BindingSource recListDataSource = new BindingSource();
		private RecListManager rec;
		public TimeShiftConfig setTsConfig = new TimeShiftConfig();
		public string qualityRank;
		public RecInfo displayingRi = null;
		
		public MainForm(string[] args)
		{
			System.Diagnostics.Debug.Listeners.Clear();
			System.Diagnostics.Debug.Listeners.Add(new log.TraceListener());
		    util.setLog(config, null);
			
		    util.debugWriteLine(util.versionStr + " " + util.versionDayStr);
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			Text = "録画登録ツール（仮 " + util.versionStr;
			afterConvertModeList.SelectedIndex = 0;
			
			try {
				Width = int.Parse(config.get("rokugaTourokuWidth"));
				Height = int.Parse(config.get("rokugaTourokuHeight"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			rec = new RecListManager(this, recListDataSource, config);
			recList.DataSource = recListDataSource;
			
			qualityRank = config.get("rokugaTourokuQualityRank");
			qualityBtn.Text = getQualityRankStr(qualityRank);
			setConvertList(int.Parse(config.get("afterConvertMode")));
		}
		void optionItem_Select(object sender, EventArgs e)
        { 
        	try {
	        	optionForm o = new optionForm(config); o.ShowDialog();
	        } catch (Exception ee) {
        		util.debugWriteLine(ee.Message + " " + ee.StackTrace);
	        }
        }
		void openRecFolderMenu_Click(object sender, EventArgs e)
		{
			string[] jarpath = util.getJarPath();
			string dirPath = (config.get("IsdefaultRecordDir") == "true") ?
					(jarpath[0] + "\\rec") : config.get("recordDir");
			try {
				if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
				System.Diagnostics.Process.Start(dirPath);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		void endMenu_Click(object sender, EventArgs e)
		{
			try {
				Close();
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.TargetSite);
			}
				
//			if (kakuninClose()) Close();;
		}
		
		void form_Close(object sender, FormClosingEventArgs e)
		{
			if (!kakuninClose()) e.Cancel = true;
		}
		bool kakuninClose() {
			
			if (rec.rdg != null) {
				DialogResult res = MessageBox.Show("録画中ですが終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
				if (rec.rdg != null) rec.rdg.stopRecording();
			}
			
			try{
				util.debugWriteLine("rokugaTourokuWidth " + Width.ToString() + " rokugaTourokuHeight " + Height.ToString() + " restore rokugaTourokuWidth " + RestoreBounds.Width.ToString() + " restore rokugaTourokuWidth " + RestoreBounds.Height.ToString());
				if (this.WindowState == FormWindowState.Normal) {
					config.set("rokugaTourokuWidth", Width.ToString());
					config.set("rokugaTourokuHeight", Height.ToString());
				} else {
					config.set("rokugaTourokuWidth", RestoreBounds.Width.ToString());
					config.set("rokugaTourokuHeight", RestoreBounds.Height.ToString());
				}
				config.set("rokugaTourokuQualityRank", qualityRank);

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			//player.stopPlaying(true, true);
			return true;
		}
		public void addLogText(string t) {
       		try {
	       		if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       		       	try {
			        	    string _t = "";
					    	if (logText.Text.Length != 0) _t += "\r\n";
					    	_t += t;
					    	
				    		logText.AppendText(_t);
							if (logText.Text.Length > 20000) 
								logText.Text = logText.Text.Substring(logText.TextLength - 10000);
	       		       	} catch (Exception e) {
	       		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       		       	}
	
				});
	       	} catch (Exception e) {
	       		util.showException(e);
	       	}
       		
		}
		void addListBtn_Click(object sender, EventArgs e)
		{
			rec.add();
			urlText.Focus();
			//var b = new info.TimeShiftConfig(0,1,2,3,true);
			//var g = new RecInfo("id", "title", "host", "comname", "start", "end", "proTime", "url", "comurl", "des", "0,1,2", b);
			
		}
		public void addList(RecInfo ri) {
			Invoke((MethodInvoker)delegate() {
				recListDataSource.Add(ri);
			});
		}
		void recBtn_Click(object sender, EventArgs e)
		{
			rec.record();
		}
		object lo = new object();
		public void resetBindingList(int row, string column = null, string val = null) {
			var a = new object();
			lock (a) {
				if (row >= recListDataSource.Count) return;
				
				var ri = (RecInfo)recListDataSource[row];
				this.Invoke((MethodInvoker)delegate() {
	            	if (column != null) {
	            		recList[column, row].Value = val;
	            		recList.Refresh();
	            		return;
	            	}
				    /*
//					return;
	            	recList["放送ID", row].Value = (ri.id == null) ? "" : string.Copy(ri.id);
            		recList["形式", row].Value = (ri.afterConvertType == null) ? "" : string.Copy(ri.afterConvertType);
            		recList["画質", row].Value = (ri.quality == null) ? "" : string.Copy(ri.quality);
            		recList["タイムシフト設定", row].Value = (ri.timeShift == null) ? "" : string.Copy(ri.timeShift);
	            	recList["状態", row].Value = (ri.state == null) ? "" : string.Copy(ri.state);
	            	recList["タイトル", row].Value = (ri.title == null) ? "" : string.Copy(ri.title);
	            	recList["放送者", row].Value = (ri.host == null) ? "" : string.Copy(ri.host);
	            	recList["コミュニティ名", row].Value = (ri.communityName == null) ? "" : string.Copy(ri.communityName);
	            	recList["開始時刻", row].Value = (ri.startTime == null) ? "" : string.Copy(ri.startTime);
	            	recList["終了時刻", row].Value = (ri.endTime == null) ? "" : string.Copy(ri.endTime);
	            	recList["ログ", row].Value = (ri.log == null) ? "" : string.Copy(ri.log);
	            	*/
	            	recList.Refresh();
				});
			}
		}
		
		void clearBtn_Click(object sender, EventArgs e)
		{
			var isRec = false;
			foreach(RecInfo ri in recListDataSource) {
				if (ri.state == "録画中") isRec = true;
			}
			if (isRec) {
				DialogResult res = MessageBox.Show("録画中ですが中断しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return;
				if (rec.rdg != null) rec.rdg.stopRecording();
			}
			recListDataSource.Clear();
		}
		
		void recList_FocusRowEnter(object sender, DataGridViewCellEventArgs e)
		{
			//return;
			util.debugWriteLine("focus row enter " + recListDataSource.Count + " erowi " + e.RowIndex);
			var ri = (RecInfo)recListDataSource[e.RowIndex];
			
			displayRiInfo(ri);
			
			var i = 0;
		}
		void setTimeshiftBtn_Click(object sender, System.EventArgs e)
		{
			var segmentSaveType = config.get("segmentSaveType");
			var o = new TimeShiftOptionForm(segmentSaveType, config, setTsConfig);
	       	try {
    	    	o.ShowDialog(this);
	       	} catch (Exception ee) {
	       		util.debugWriteLine("timeshift option form invoke " + ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
			}
			if (o.ret != null) {
				setTsConfig = o.ret;
				setTimeshiftBtn.Text =  getSetTsBtnText(setTsConfig);
			}
		}
		private string getSetTsBtnText(TimeShiftConfig tsConfig) {
			if (tsConfig.timeType == 0) {
				var start = ((tsConfig.h < 1000) ? 
						tsConfig.h.ToString("0") : tsConfig.h.ToString()) +
						"時間" + tsConfig.m.ToString("") + "分" +
						tsConfig.s.ToString("") + "秒";
				var end = ((tsConfig.endH < 1000) ? 
						tsConfig.endH.ToString("0") : tsConfig.endH.ToString()) +
						"時間" + tsConfig.endM.ToString("") + "分" +
						tsConfig.endS.ToString("") + "秒";
				return start + "-" + end;
			} else {
				//return (tsConfig.isContinueConcat) ? 
				return "前回の続きから録画";
			}
		}
		
		void qualityBtn_Click(object sender, EventArgs e)
		{
			var o = new QualityForm(qualityRank);
	       	try {
    	    	o.ShowDialog(this);
	       	} catch (Exception ee) {
	       		util.debugWriteLine("timeshift option form invoke " + ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
			}
			if (o.ret != null) {
				qualityRank = o.ret;
				qualityBtn.Text =  o.qualityStr;
			}
		}
		string getQualityRankStr(string qualityRank) {
			return qualityRank.Replace("0", "自")
				.Replace("1", "超高").Replace("2", "高")
				.Replace("3", "中").Replace("4", "低")
				.Replace("5", "超低");
		}
		public void displayRiInfo(RecInfo ri, string ctrl = null, string val = null) {
			var isChange = displayingRi != ri;
			displayingRi = ri;
			//util.debugWriteLine("display c " + recList.RowCount + " " + recListDataSource.Count);
			
			this.Invoke((MethodInvoker)delegate() {
				if (ctrl == "startTime" || ctrl == null) startTimeLabel.Text = ri.startTime;
				if (ctrl == "endTime" || ctrl == null) endTimeLabel.Text = ri.endTime;
				if (ctrl == "programTime" || ctrl == null) programTimeLabel.Text = ri.programTime;
				
				if (ctrl == "keikaTime" || ctrl == null) {
					if (isChange) {
						Task.Run(() => displayKeikaTime(ri));
//						Task.Run(() => util.debugWriteLine("aa"));
					}
				}
				
				if (ctrl == "title" || ctrl == null) titleLabel.Text = ri.title;
				if (ctrl == "host" || ctrl == null) hostLabel.Text = ri.host;
				if (ctrl == "communityName" || ctrl == null) communityLabel.Text = ri.communityName;
				if (ctrl == "url" || ctrl == null) urlLabel.Text = ri.url;
				if (ctrl == "communityUrl" || ctrl == null) communityUrlLabel.Text = ri.communityUrl;
				if (ctrl == "description" || ctrl == null) descriptLabel.Text = ri.description;
//				if (ctrl == "quality" || ctrl == null) qualityLabel.Text = ri.quality;
//				if (ctrl == "timeshift" || ctrl == null) timeshiftLabel.Text = ri.timeShift;
//				if (ctrl == "afterConvertMode" || ctrl == null) afterConvertModeLabel.Text = ri.afterConvertType;
				if ((ctrl == "samuneUrl" || ctrl == null) && ri.samune != null) samuneBox.Image = ri.samune;
				if (ctrl == "log" || ctrl == null) setLogText(ri.log);
			});
		}
		public void clearRiInfo(RecInfo ri) {
			startTimeLabel.Text = "";
			endTimeLabel.Text = "";
			programTimeLabel.Text = "";
			keikaTimeLabel.Text = "";
			titleLabel.Text = "";
			hostLabel.Text = "";
			communityLabel.Text = "";
			urlLabel.Text = "";
			communityUrlLabel.Text = "";
			descriptLabel.Text = "";
//			qualityLabel.Text = "";
//			timeshiftLabel.Text = "";
//			afterConvertModeLabel.Text = "";
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			samuneBox.Image = ((System.Drawing.Image)(resources.GetObject("samuneBox.Image")));
			logText.Text = "";
		}
		private void setLogText(string t) {
//			util.debugWriteLine(logText.Text.Length + " " + t.Length);
			var checkLength = (logText.Text.Length < t.Length) ? 
					logText.Text.Length : t.Length;
			var isAppend = (logText.Text == t.Substring(0, checkLength) && checkLength != 0);
			this.Invoke((MethodInvoker)delegate() {
		       	try {
					if (isAppend)
						logText.AppendText("\r\n" + t.Substring(logText.Text.Length));
					else {
						logText.ResetText();
						logText.AppendText(t);
					}
		       	} catch (Exception e) {
		       		util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		       	}
			});
			
		}
		void recListCell_MouseDown(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
		{
//			util.debugWriteLine(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Right &&
			   e.ColumnIndex > -1 && e.RowIndex > -1) {
				recList.CurrentCell = recList[e.ColumnIndex, e.RowIndex];
			}
		}
		void openWatchUrlMenu_Click(object sender, System.EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedCell = recList.SelectedCells[0];
			var ri = (RecInfo)recListDataSource[selectedCell.RowIndex];
			System.Diagnostics.Process.Start(ri.url);
		}
		void openCommunityUrlMenu_Click(object sender, EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedCell = recList.SelectedCells[0];
			var ri = (RecInfo)recListDataSource[selectedCell.RowIndex];
			if (ri.communityUrl == null) {
				if (ri.communityName == "official") 
					System.Diagnostics.Process.Start("http://live.nicovideo.jp/");
				return;
			}
			System.Diagnostics.Process.Start(ri.communityUrl);
		}
		void copyWatchUrlMenu_Click(object sender, EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedCell = recList.SelectedCells[0];
			var ri = (RecInfo)recListDataSource[selectedCell.RowIndex];
			Clipboard.SetText(ri.url);
		}
		void copyCommunityUrlMenu_Click(object sender, EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedCell = recList.SelectedCells[0];
			var ri = (RecInfo)recListDataSource[selectedCell.RowIndex];
			if (ri.communityUrl == null) {
//				if (ri.communityName == "official") 
//					Clipboard.SetText("http://live.nicovideo.jp/");
				return;
			}
			System.Diagnostics.Process.Start(ri.communityUrl);
		}
		
		void reAddRowMenu_Click(object sender, EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedCell = recList.SelectedCells[0];
			var ri = (RecInfo)recListDataSource[selectedCell.RowIndex];
			if (ri.state == "録画中") {
				MessageBox.Show("録画中は再登録できません", "", MessageBoxButtons.OK, MessageBoxIcon.None);
				/*
				DialogResult res = MessageBox.Show("録画中ですが中断しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return;
				try {
					ri.process.Kill();
				} catch (Exception ee) {
					util.debugWriteLine("reAdd kill exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
				*/
				return;
			}
			var _ri = new RecInfo(ri.id, ri.url, ri.rdg, ri.afterConvertType, ri.tsConfig, ri.timeShift, ri.quality, ri.qualityRank);
			recListDataSource[selectedCell.RowIndex] = _ri;
			resetBindingList(selectedCell.RowIndex);
			displayRiInfo(_ri);
		}
		void deleteRowMenu_Click(object sender, EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedCell = recList.SelectedCells[0];
			var ri = (RecInfo)recListDataSource[selectedCell.RowIndex];
			if (ri.state == "録画中") {
				//MessageBox.Show("録画中は登録できません", "", MessageBoxButtons.OK, MessageBoxIcon.None);
				
				DialogResult res = MessageBox.Show("録画中ですが中断しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return;
				try {
					ri.process.Kill();
				} catch (Exception ee) {
					util.debugWriteLine("reAdd kill exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			recListDataSource.Remove(selectedCell.RowIndex);
			recList.Rows.RemoveAt(selectedCell.RowIndex);
			//resetBindingList(selectedCell.RowIndex);
		}
		
		void form_Load(object sender, EventArgs e)
		{
			System.Type d = typeof(DataGridView);
			System.Type t = typeof(TextBox);
			System.Reflection.PropertyInfo dinfo =
		　　　　　　d.GetProperty(
		　　　　　　"DoubleBuffered", System.Reflection.BindingFlags.Instance |
		　　　　　　System.Reflection.BindingFlags.NonPublic);
			System.Reflection.PropertyInfo tinfo =
		　　　　　　t.GetProperty(
		　　　　　　"DoubleBuffered", System.Reflection.BindingFlags.Instance |
		　　　　　　System.Reflection.BindingFlags.NonPublic);
			dinfo.SetValue(recList, true);
			tinfo.SetValue(logText, true);
		}
		
		void recList_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
//			util.showException(e, false);
			var a = 0;
		}
		public int getRecListCount() {
			var ret = 0;
			lock(lo) {
				Invoke((MethodInvoker)delegate() {
					ret = recListDataSource.Count;
				});
			}
			return ret;
		}
		public int getRecListSelectedCount() {
			var ret = 0;
			lock(lo) {
				Invoke((MethodInvoker)delegate() {
					ret = recList.SelectedCells.Count;
				});
			}
			return ret;
		}
		private void displayKeikaTime(RecInfo ri) {
			this.Invoke((MethodInvoker) delegate() {
				keikaTimeLabel.Text = "";
			});
			while (ri == displayingRi && (ri.state == "録画中" || ri.state == "待機中")) {
				if (ri.keikaTimeStart == DateTime.MinValue) {
					Thread.Sleep(500);
					continue;
				}
				var keikaDt = DateTime.Now - ri.keikaTimeStart;
				this.Invoke((MethodInvoker) delegate() {
					keikaTimeLabel.Text = keikaDt.ToString("h'時間'mm'分'ss'秒'");
				});
				Thread.Sleep(500);
			}
			var i = 0;
		}
		void setConvertList(int afterConvertMode) {
			var t = "ts(変換無し)";
			if (afterConvertMode == 1) t = "avi";  
			if (afterConvertMode == 2) t = "mp4";
			if (afterConvertMode == 3) t = "flv";
			if (afterConvertMode == 4) t = "mov";
			if (afterConvertMode == 5) t = "wmv";
			if (afterConvertMode == 6) t = "vob";
			if (afterConvertMode == 7) t = "mkv";
			if (afterConvertMode == 8) t = "mp3(音声)";
			if (afterConvertMode == 9) t = "wav(音声)";
			if (afterConvertMode == 10) t = "wma(音声)";
			if (afterConvertMode == 11) t = "aac(音声)";
			if (afterConvertMode == 12) t = "ogg(音声)";
			afterConvertModeList.Text = t;
		}
		void versionMenu_Click(object sender, EventArgs e)
		{
			var v = new VersionForm();
			v.ShowDialog();
		}
	}
}
