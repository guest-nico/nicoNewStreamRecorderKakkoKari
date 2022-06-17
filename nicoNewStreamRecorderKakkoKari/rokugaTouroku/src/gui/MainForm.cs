﻿/*
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
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using rokugaTouroku.gui;
using rokugaTouroku.info;
using rokugaTouroku.rec;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SunokoLibrary.Application;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public rokugaTouroku.config.config config = new rokugaTouroku.config.config();
		private SortableBindingList<RecInfo> recListDataSource = new SortableBindingList<RecInfo>();
		private RecListManager rec;
		public TimeShiftConfig setTsConfig = new TimeShiftConfig();
		public string qualityRank;
		public RecInfo displayingRi = null;
		public CookieContainer container = null;
		
		private Thread madeThread;
		
		public MainForm(string[] args)
		{
			madeThread = Thread.CurrentThread;
			//config.set("IsHokan", "false");
			
			System.Diagnostics.Debug.Listeners.Clear();
			System.Diagnostics.Debug.Listeners.Add(new Logger.TraceListener());
		    util.setLog(config, null);
			
		    util.debugWriteLine(util.versionStr + " " + util.versionDayStr);
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			Text = "録画登録ツール（仮 " + util.versionStr;
			afterConvertModeList.SelectedIndex = 0;
			
			var fontSize = config.get("fontSize");  
			if (fontSize != "9")
				util.setFontSize(int.Parse(fontSize), this, true, 523);
			
			try {
				Width = int.Parse(config.get("rokugaTourokuWidth"));
				Height = int.Parse(config.get("rokugaTourokuHeight"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			try {
				if (bool.Parse(config.get("IsRestoreLocation"))) {
					var x = config.get("rokugaTourokuX");
					var y = config.get("rokugaTourokuY");
					if (x != "" && y != "") {
						StartPosition = FormStartPosition.Manual;
						Location = new Point(int.Parse(x), int.Parse(y));
					}
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				StartPosition = FormStartPosition.WindowsDefaultLocation;
			}
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			rec = new RecListManager(this, recListDataSource, config);
			
			
			var qr = config.get("qualityRank").Split(',').ToList();
			var rqr = config.get("rokugaTourokuQualityRank").Split(',').ToList();
			var qrD = config.defaultConfig["qualityRank"].Split(',');
			if (qr.Count != qrD.Length) {
				foreach (var q in qrD) 
					if (qr.IndexOf(q) == -1) qr.Add(q);
			}
			config.set("qualityRank", string.Join(",", qr.ToArray()));
			
			if (rqr.Count != qrD.Length) {
				foreach (var q in qrD) 
					if (rqr.IndexOf(q) == -1) rqr.Add(q);
			}
			config.set("rokugaTourokuQualityRank", string.Join(",", rqr.ToArray()));
			
			//if (config.get("rokugaTourokuQualityRank").Split(',').Length == 5)
			//	config.set("rokugaTourokuQualityRank", config.get("rokugaTourokuQualityRank") + ",5");
			
			changeRecBtnClickEvent(bool.Parse(config.get("IsRecBtnOnlyMouse")));
			
			setQualitySetting();
		}
		private void formInitSetting() {
			recList.DataSource = recListDataSource;
			
			qualityRank = config.get("rokugaTourokuQualityRank");
			qualityBtn.Text = getQualityRankStr(qualityRank);
			setConvertList(int.Parse(config.get("afterConvertMode")));
			recCommmentList.Text = "映像＋コメント";
			
			setBackColor(Color.FromArgb(int.Parse(config.get("tourokuBackColor"))));
			setForeColor(Color.FromArgb(int.Parse(config.get("tourokuForeColor"))));
			
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
			setCookieForm();
			
			setFormState();
			applyMenuSetting();
		}
		void optionItem_Select(object sender, EventArgs e)
        { 
			try {
	        	optionForm o = new optionForm(config);

	        	var size = config.get("fontSize");
	        	if (o.ShowDialog() == DialogResult.OK) {
	        		changeRecBtnClickEvent(bool.Parse(config.get("IsRecBtnOnlyMouse")));
	        		
	        		var newSize = config.get("fontSize");
	        		util.debugWriteLine("size " + size + " new size " + newSize);
	        		if (size != newSize) {
	        			var formSize = Size;
	        			var loc = Location;
	        			loadControlLayout();
	        			util.setFontSize(int.Parse(newSize), this, true, 523);
	        			Size = formSize;
	        			Location = loc;
	        		}
	        	}
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
				DialogResult res = util.showMessageBoxCenterForm(this, "録画中ですが録画を終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
				if (rec.rdg != null) rec.rdg.stopRecording();
			}
			
			try{
				util.debugWriteLine("rokugaTourokuWidth " + Width.ToString() + " rokugaTourokuHeight " + Height.ToString() + " restore rokugaTourokuWidth " + RestoreBounds.Width.ToString() + " restore rokugaTourokuWidth " + RestoreBounds.Height.ToString());
				//var isMiniInfo = tableLayoutPanel1.RowStyles[1].Height != 158;
				var isMiniInfo = !logText.Visible;
				var ritu =  (startTimeLabel.Font.Size / 9);
				if (this.WindowState == FormWindowState.Normal) {
					config.set("rokugaTourokuWidth", Width.ToString());
					config.set("rokugaTourokuHeight", (isMiniInfo ? (int)(Height + 142 * ritu) : Height).ToString());
					config.set("rokugaTourokuX", Location.X.ToString());
					config.set("rokugaTourokuY", Location.Y.ToString());
				} else {
					config.set("rokugaTourokuWidth", RestoreBounds.Width.ToString());
					config.set("rokugaTourokuHeight", (isMiniInfo ? (int)(RestoreBounds.Height + 142 * ritu) : RestoreBounds.Height).ToString());
					config.set("rokugaTourokuX", RestoreBounds.X.ToString());
					config.set("rokugaTourokuY", RestoreBounds.Y.ToString());
				}
				config.set("rokugaTourokuQualityRank", qualityRank);
				config.set("rokugaTourokuMiniInfo", isMiniInfo.ToString());

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			//player.stopPlaying(true, true);
			saveList();
			saveFormState();
			saveMenuSetting();
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
			if (rec.add(urlText.Text)) {
				urlText.Text = "";
				saveList();
			}
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
				if (row >= recListDataSource.Count || row < 0) return;
				
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
			if (bool.Parse(config.get("IsDeleteConfirmMessageRt")) &&
			    	MessageBox.Show("全ての行を削除しますか？", "", MessageBoxButtons.YesNo) == DialogResult.No)
				return;
			var isRec = false;
			foreach(RecInfo ri in recListDataSource) {
				if (ri.state == "録画中") isRec = true;
			}
			if (isRec) {
				DialogResult res = util.showMessageBoxCenterForm(this, "録画中ですが中断しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return;
				if (rec.rdg != null) rec.rdg.stopRecording();
			}
			recListDataSource.Clear();
			clearRiInfo();
		}
		
		void recList_FocusRowEnter(object sender, DataGridViewCellEventArgs e)
		{
			//return;
			util.debugWriteLine("focus row enter " + recListDataSource.Count + " erowi " + e.RowIndex);
			var ri = (RecInfo)recListDataSource[e.RowIndex];
			
			displayRiInfo(ri);
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
			string start = "", end = "";
			
			if (tsConfig.startTimeMode == 0) start = "最初から";
			else if (tsConfig.startTimeMode == 1) {
				start = ((tsConfig.h < 1000) ?
						tsConfig.h.ToString("0") : tsConfig.h.ToString()) +
						"時間" + tsConfig.m.ToString("") + "分" +
						tsConfig.s.ToString("") + "秒";
			} else start = "前回の続きから";
			
			if (tsConfig.endTimeMode == 0) end = "最後まで";
			else if (tsConfig.endTimeMode == 1) {
				end = ((tsConfig.endH < 1000) ?
						tsConfig.endH.ToString("0") : tsConfig.endH.ToString()) +
						"時間" + tsConfig.endM.ToString("") + "分" +
						tsConfig.endS.ToString("") + "秒";
			}
			return start + "-" + end;
		}
		
		void qualityBtn_Click(object sender, EventArgs e)
		{
			var o = new QualityForm(qualityRank, int.Parse(config.get("fontSize")));
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
			
			return qualityRank//.Replace("0", "自")
				.Replace("0", "超高").Replace("1", "高")
				.Replace("2", "中").Replace("3", "低")
				.Replace("4", "超低").Replace("5", "音")
				.Replace("6", "6M").Replace("8", "4M")
				.Replace("7", "8M").Replace("9", "音");
		}
		public void displayRiInfo(RecInfo ri, string ctrl = null, string val = null) {
			var isChange = displayingRi != ri;
			displayingRi = ri;
			//util.debugWriteLine("display c " + recList.RowCount + " " + recListDataSource.Count);
			
			this.Invoke((MethodInvoker)delegate() {
				try {
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
	        	} catch (Exception e) {
	        		util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	        	}
			});
		}
		public void clearRiInfo() {
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
			/*
//			util.debugWriteLine(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Right &&
			   e.ColumnIndex > -1 && e.RowIndex > -1) {
				recList.CurrentCell = recList[e.ColumnIndex, e.RowIndex];
			}
			*/
		}
		void openWatchUrlMenu_Click(object sender, System.EventArgs e)
		{
			var dataSource = recListDataSource;
			var selectedRowIndexList = new List<int>();
			foreach (DataGridViewCell c in recList.SelectedCells) {
				try {
					if (selectedRowIndexList.IndexOf(c.RowIndex) > -1) continue;
					selectedRowIndexList.Add(c.RowIndex);
					var ri = (RecInfo)dataSource[c.RowIndex];
					if (ri.url == null || ri.url == "") continue;
					util.openUrlBrowser(ri.url, config);
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
		}
		void openCommunityUrlMenu_Click(object sender, EventArgs e)
		{
			var dataSource = recListDataSource;
			var selectedRowIndexList = new List<int>();
			foreach (DataGridViewCell c in recList.SelectedCells) {
				try {
					if (selectedRowIndexList.IndexOf(c.RowIndex) > -1) continue;
					selectedRowIndexList.Add(c.RowIndex);
					var ri = (RecInfo)dataSource[c.RowIndex];
					if (ri.communityUrl == null || ri.communityUrl == "") continue;
					util.openUrlBrowser(ri.communityUrl, config);
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
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
//					Clipboard.SetText("https://live.nicovideo.jp/");
				return;
			}
			Clipboard.SetText(ri.communityUrl);
		}
		
		void reAddRowMenu_Click(object sender, EventArgs e)
		{
			var dataSource = recListDataSource;
			var selectedRowIndexList = new List<int>();
			foreach (DataGridViewCell c in recList.SelectedCells) {
				try {
					if (selectedRowIndexList.IndexOf(c.RowIndex) > -1) continue;
					selectedRowIndexList.Add(c.RowIndex);
					var ri = (RecInfo)dataSource[c.RowIndex];
					if (ri.state == "録画中") {
						util.showMessageBoxCenterForm(this, "録画中は再登録できません", "", MessageBoxButtons.OK, MessageBoxIcon.None);
						return;
					}
					var _ri = new RecInfo(ri.id, ri.url, ri.rdg, ri.afterConvertType, ri.tsConfig, ri.timeShift, ri.quality, ri.qualityRank, ri.recComment, ri.isChase, ri.ai);
					Task.Run(() => _ri.setHosoInfo(this));
					
					recListDataSource[c.RowIndex] = _ri;
					resetBindingList(c.RowIndex);
					displayRiInfo(_ri);
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
		}
		void ReAddNewConfigRowMenuClick(object sender, EventArgs e)
		{
			var dataSource = recListDataSource;
			var selectedRowIndexList = new List<int>();
			foreach (DataGridViewCell c in recList.SelectedCells) {
				try {
					if (selectedRowIndexList.IndexOf(c.RowIndex) > -1) continue;
					selectedRowIndexList.Add(c.RowIndex);
					var ri = (RecInfo)dataSource[c.RowIndex];
					if (ri.state == "録画中") {
						util.showMessageBoxCenterForm(this, "録画中は再登録できません", "", MessageBoxButtons.OK, MessageBoxIcon.None);
						return;
					}
					var _ri = new RecInfo(ri.id, ri.url, ri.rdg, afterConvertModeList.Text, setTsConfig, setTimeshiftBtn.Text, qualityBtn.Text, qualityRank, recCommmentList.Text, isChaseChkBox.Checked, (AccountInfo)accountBtn.Tag);
					Task.Run(() => _ri.setHosoInfo(this));
					
					recListDataSource[c.RowIndex] = _ri;
					resetBindingList(c.RowIndex);
					displayRiInfo(_ri);
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
		}
		void deleteRowMenu_Click(object sender, EventArgs e)
		{
			if (recList.SelectedCells.Count == 0) return;
			var selectedRIList = new List<RecInfo>();
			foreach (DataGridViewCell c in recList.SelectedCells) {
				try {
					var ri = (RecInfo)recListDataSource[c.RowIndex];
					if (selectedRIList.IndexOf(ri) > -1) continue;
					else selectedRIList.Add(ri);
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			foreach (var ri in selectedRIList)
				recListDataSource.Remove(ri);
		}
		public bool deleteRow(RecInfo ri) {
			if (ri.state == "録画中") {
				//util.showMessageBoxCenterForm(this, "録画中は登録できません", "", MessageBoxButtons.OK, MessageBoxIcon.None);
				
				DialogResult res = util.showMessageBoxCenterForm(this, "録画中ですが中断しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
				try {
					ri.process.Kill();
				} catch (Exception ee) {
					util.debugWriteLine("reAdd kill exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			recListDataSource.Remove(ri);
			//recList.Rows.RemoveAt(selectedCell.RowIndex);
			//resetBindingList(selectedCell.RowIndex);
			return true;
		}
		void form_Load(object sender, EventArgs e)
		{
			//loadList();
			formInitSetting();
			if (bool.Parse(config.get("rokugaTourokuMiniInfo")))
				FoldBtnLabelClick(null, null);
			loadList();
			
			if (config.brokenCopyFile != null)
				addLogText("設定ファイルを読み込めませんでした。設定ファイルをバックアップしました。" + config.brokenCopyFile);
			
			Task.Run(() =>  {
				try {
					var cg = new CookieGetter(config);
					var cgret = cg.getHtml5RecordCookie("https://live.nicovideo.jp/my", false);
					if (cgret != null && cgret.Result != null
					    	&& cgret.Result[0] != null)
						container = cgret.Result[0];
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
				}
			});
		}
		
		void recList_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
//			util.showException(e, false);
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
				keikaTimeLabel.Text = ri.keikaTime;
			});
			while (ri == displayingRi && (ri.state == "録画中" || ri.state == "待機中")) {
				if (ri.keikaTimeStart == DateTime.MinValue) {
					Thread.Sleep(500);
					continue;
				}
				var keikaDt = DateTime.Now - ri.keikaTimeStart;
				this.Invoke((MethodInvoker) delegate() {
					keikaTimeLabel.Text = keikaDt.ToString("h'時間'mm'分'ss'秒'");
					ri.keikaTime = keikaTimeLabel.Text;
				});
				Thread.Sleep(500);
			}
		}
		void setConvertList(int afterConvertMode) {
			var t = "処理しない";
			if (afterConvertMode == 1) t = "形式を変更せず処理する";
			if (afterConvertMode == 2) t = "ts";
			if (afterConvertMode == 3) t = "avi";			
			if (afterConvertMode == 4) t = "mp4";
			if (afterConvertMode == 5) t = "flv";
			if (afterConvertMode == 6) t = "mov";
			if (afterConvertMode == 7) t = "wmv";
			if (afterConvertMode == 8) t = "vob";
			if (afterConvertMode == 9) t = "mkv";
			if (afterConvertMode == 10) t = "mp3(音声)";
			if (afterConvertMode == 11) t = "wav(音声)";
			if (afterConvertMode == 12) t = "wma(音声)";
			if (afterConvertMode == 13) t = "aac(音声)";
			if (afterConvertMode == 14) t = "ogg(音声)";
			afterConvertModeList.Text = t;
		}
		void versionMenu_Click(object sender, EventArgs e)
		{
			var v = new VersionForm(int.Parse(config.get("fontSize")), this);
			v.ShowDialog();
		}
		
		void urlBulkRegistMenu_Click(object sender, EventArgs e)
		{
			var f = new UrlBulkRegistForm(int.Parse(config.get("fontSize")));
			f.ShowDialog();
			if (f.res == null) return;
			
			foreach(var r in f.res) {
				util.debugWriteLine(r);
				rec.add(r);
			}
			saveList();
		}
		
		void urlListSaveMenu_Click(object sender, EventArgs e)
		{
			var t = "";
			foreach (var d in recListDataSource) {
				var url = ((RecInfo)d).url;
				if (url == null) continue;
				var _url = util.getRegGroup(url, "(lv\\d+)");
				if (_url == null) continue;
				_url = "https://live.nicovideo.jp/watch/" + _url;
				t += _url + "\r\n";
			}
			var f = new UrlListSaveForm(t, int.Parse(config.get("fontSize")));
			f.ShowDialog();
		}
		
		void RecListDragDrop(object sender, DragEventArgs e)
		{
			try {
				var url = e.Data.GetData(DataFormats.Text).ToString();
				rec.add(url);
				saveList();
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		void RecListDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("UniformResourceLocator") ||
			    e.Data.GetDataPresent("UniformResourceLocatorW") ||
			    e.Data.GetDataPresent(DataFormats.Text)) {
				util.debugWriteLine(e.Effect);
				e.Effect = DragDropEffects.Copy;
				
			}
		}
		public void formAction(Action a) {
			if (IsDisposed) return;
			
			if (Thread.CurrentThread == madeThread) {
				try {
					a.Invoke();
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			} else {
				try {
					Invoke((MethodInvoker)delegate() {
						try {    
				       		a.Invoke();
				       	} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						}
					});
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				} 
			}
		}
		public void updateRecListCell(RecInfo ri) {
			formAction(() => {
				var i = recListDataSource.IndexOf(ri);
				if (i == -1) return;
				var cellNum = recList.Columns.Count;
				for (var j = 0; j < cellNum; j++)
					recList.UpdateCellValue(j, i);
			});
		}
		private void loadList() {
			try {
				var f = util.getJarPath()[0] + "/recList.ini";
				var sr = new StreamReader(f);
				var s = sr.ReadToEnd();
				sr.Close();
				var list = JsonConvert.DeserializeObject<List<RecInfo>>(s);
				foreach (var l in list) {
					if (l.state == "録画中") l.state = "録画失敗";
					if (l.samuneUrl != null) l.samune = util.getSamune(l.samuneUrl);
					if (l.ai == null && accountBtn.Tag != null) l.ai = (AccountInfo)accountBtn.Tag;
					
					addList(l);
				}
				//Task.Run(() => {
		        // 	foreach (var l in list) l.setHosoInfo(this);
		        // });
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		public void saveList() {
			try {
				var list = new List<RecInfo>();
				foreach (RecInfo ri in recListDataSource) {
					var _ri = new RecInfo(ri.id, ri.url, ri.rdg, ri.afterConvertType, ri.tsConfig, ri.timeShift, ri.quality, ri.qualityRank, ri.recComment, ri.isChase, ri.ai);
					_ri.samune = null;
					_ri.process = null;
					_ri.rdg = null;
					_ri.title = ri.title;
					_ri.state = ri.state;
					_ri.host = ri.host;
					_ri.communityName = ri.communityName;
					_ri.startTime = ri.startTime;
					//_ri.keikaTime = ri.keikaTime;
					_ri.keikaTimeStart = ri.keikaTimeStart;
					_ri.endTime = ri.endTime;
					_ri.programTime = ri.programTime;
					_ri.communityUrl = ri.communityUrl;
					_ri.description = ri.description;
					_ri.log = ri.log;
					_ri.samuneUrl = ri.samuneUrl;
					list.Add(_ri);
				}
				var json = JToken.FromObject(list).ToString(Formatting.None);
				var f = util.getJarPath()[0] + "/recList.ini_";
				var sw = new StreamWriter(f, false);
				sw.Write(json);
				sw.Close();
				File.Copy(f, f.Substring(0, f.Length - 1), true);
				File.Delete(f);
				
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void UpdateMenuClick(object sender, EventArgs e)
		{
			var v = new UpdateForm(int.Parse(config.get("fontSize")));
			v.ShowDialog();
		}
		void FormColorMenuItemClick(object sender, EventArgs e)
		{
			var d = new ColorDialog();
			d.Color = BackColor;
			var r = d.ShowDialog();
			if (r == DialogResult.OK) {
				setBackColor(d.Color);
				config.set("tourokuBackColor", d.Color.ToArgb().ToString());
			}
		}
		private void setBackColor(Color color) {
			BackColor = //commentList.BackgroundColor = 
				color;
			//if (color != 
		}
		void CharacterColorMenuItemClick(object sender, EventArgs e)
		{
			var d = new ColorDialog();
			d.Color = label1.ForeColor;
			var r = d.ShowDialog();
			if (r == DialogResult.OK) {
				setForeColor(d.Color);
				config.set("tourokuForeColor", d.Color.ToArgb().ToString());
			}
		}
		private void setForeColor(Color color) {
			var c = getChildControls(this);
			foreach (var _c in c)
				if (_c.GetType() == typeof(GroupBox) ||
				    _c.GetType() == typeof(Label) || 
				    _c.GetType() == typeof(CheckBox)) _c.ForeColor = color;
		}
		private List<Control> getChildControls(Control c) {
			util.debugWriteLine("cname " + c.Name);
			var ret = new List<Control>();
			foreach (Control _c in c.Controls) {
				var children = getChildControls(_c);
				ret.Add(_c);
				ret.AddRange(children);
				util.debugWriteLine(c.Name + " " + children.Count);
			}
			util.debugWriteLine(c.Name + " " + ret.Count);
			return ret;
		}
		void RecListRowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			saveList();
		}
		private void loadControlLayout() {
			try {
				saveList();
				config.set("rokugaTourokuQualityRank", qualityRank);
				var convertText = afterConvertModeList.Text;
				var recCommmentText = recCommmentList.Text;
				var timeshiftText = setTimeshiftBtn.Text;
				var _urlText = urlText.Text;
				recList.DataSource = null;
				
				Font = new Font(Font.FontFamily, 9);
				Controls.Clear();
				
				InitializeComponent();
				formInitSetting();
				afterConvertModeList.Text = convertText;
				recCommmentList.Text = recCommmentText;
				setTimeshiftBtn.Text = timeshiftText;
				urlText.Text = _urlText;
				
				Update();

				return;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void setFormState() {
			try {
				var recListWidth = config.get("RecListColumnWidth");
				if (recListWidth != "") {
					var w = recListWidth.Split(',');
					foreach (var _w in w) {
						var nv = _w.Split(':');
						foreach (DataGridViewColumn c in recList.Columns) {
							if (c.HeaderText == nv[0]) c.Width = int.Parse(nv[1]);
						}
					}
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void saveFormState() {
			var recListWidth = new List<string>();
			foreach (DataGridViewColumn c in recList.Columns)
				recListWidth.Add(c.HeaderText + ":" + c.Width.ToString());
			config.set("RecListColumnWidth", string.Join(",", recListWidth.ToArray()));
		}
		void applyMenuSetting() {
			var showRecListColumns = config.get("ShowRecColumns");
			for(var i = 0; i < recList.Columns.Count; i++) {
				if (i >= showRecListColumns.Length) continue;
				recList.Columns[i].Visible = showRecListColumns[i] == '1';
				var menu = (ToolStripMenuItem)displayRecListMenu.DropDownItems[i];
				menu.Checked = recList.Columns[i].Visible;
			}
		}
		void saveMenuSetting() {
			var buf = "";
			for(var i = 0; i < recList.Columns.Count; i++) {
				buf += recList.Columns[i].Visible ? "1" : "0";
			}
			config.set("ShowRecColumns", buf);
		}
		
		void DisplayRecListMenuDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			var i = 0;
			for (var j = 0; j < recList.Columns.Count; j++)
				if (recList.Columns[j].HeaderText == e.ClickedItem.Text) i = j;
			recList.Columns[i].Visible = !recList.Columns[i].Visible;
			((ToolStripMenuItem)e.ClickedItem).Checked = recList.Columns[i].Visible;
		}
		
		void RecListCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try {
				if (recList.Columns[e.ColumnIndex].HeaderText != "状態" ||
				    	e.Value == null || (!e.Value.Equals("録画完了") && !e.Value.Equals("録画失敗"))) return;
				
				var ri = (RecInfo)recListDataSource[e.RowIndex];
				if (ri.state == "録画完了") 
					e.CellStyle.BackColor = Color.FromArgb(207, 255, 117);
				if (ri.state == "録画失敗")
					//e.CellStyle.BackColor = Color.FromArgb(255, 255, 155);
					e.CellStyle.BackColor = Color.FromArgb(255, 225, 226);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		
		void DisplayRecListMenuDropDownOpened(object sender, EventArgs e)
		{
			for (var i = 0; i < recList.Columns.Count; i++) {
				displayRecListMenu.DropDownItems[i].Text = recList.Columns[i].HeaderText;
				((ToolStripMenuItem)displayRecListMenu.DropDownItems[i]).Checked = recList.Columns[i].Visible;
			}
		}
		
		void DeleteFinishedBtnClick(object sender, EventArgs e)
		{
			if (bool.Parse(config.get("IsDeleteConfirmMessageRt")) &&
			    	MessageBox.Show("完了した行を削除しますか？", "", MessageBoxButtons.YesNo) == DialogResult.No)
				return;
			var deleteList = new List<RecInfo>();
			foreach(RecInfo ri in recListDataSource) {
				if (ri.state == "録画完了") deleteList.Add(ri);
			}
			while (true) {
				try {
					foreach (var ri in deleteList) {
						if (recListDataSource.IndexOf(ri) > -1)
							recListDataSource.Remove(ri);
					}
					break;
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
				}
			}
			if (recList.SelectedCells.Count == 0)
				clearRiInfo();
		}
		
		void OpenSettingFolderMenuClick(object sender, EventArgs e)
		{
			string[] jarpath = util.getJarPath();
			string dirPath = jarpath[0];
			try {
				if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
				System.Diagnostics.Process.Start(dirPath);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		
		void OpenReadmeMenuClick(object sender, EventArgs e)
		{
			string[] jarpath = util.getJarPath();
			string path = jarpath[0] + "/readme.html";
			try {
				if (!File.Exists(path)) {
					util.showMessageBoxCenterForm(this, "readme.htmlが見つかりませんでした");
					return;
				}
				System.Diagnostics.Process.Start(path);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		
		void OpenRecExeMenuClick(object sender, EventArgs e)
		{
			string[] jarpath = util.getJarPath();
			string path = jarpath[0] + "/ニコ生新配信録画ツール（仮.exe";
			try {
				if (!File.Exists(path)) {
					util.showMessageBoxCenterForm(this, "ニコ生新配信録画ツール（仮.exeが見つかりませんでした");
					return;
				}
				System.Diagnostics.Process.Start(path);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		void changeRecBtnClickEvent(bool isRecBtnOnlyMouse) {
			recBtn.Click -= new EventHandler(recBtn_Click);
			recBtn.MouseClick -= new MouseEventHandler(recBtn_Click);
			if (isRecBtnOnlyMouse) {
				recBtn.MouseClick += new MouseEventHandler(recBtn_Click);
			} else {
				recBtn.Click += new EventHandler(recBtn_Click);
			}
		}
		private void setQualitySetting() {
			var qr = config.get("qualityRank").Split(',').ToList();
			var qrD = config.defaultConfig["qualityRank"].Split(',');
			if (qr.Count != qrD.Length) {
				foreach (var q in qrD) 
					if (qr.IndexOf(q) == -1) qr.Add(q);
			}
			config.set("qualityRank", string.Join(",", qr.ToArray()));
			
			
			var _qualityCfg = config.get("qualityList");
			var _qualityCfgD = config.defaultConfig["qualityList"];
			if (string.IsNullOrEmpty(_qualityCfg)) 
				_qualityCfg = _qualityCfgD;
			var qualityCfg = JsonConvert.DeserializeObject<Dictionary<int, string>>(_qualityCfg);
			var qualityCfgD = JsonConvert.DeserializeObject<Dictionary<int, string>>(_qualityCfgD);
			//if (qualityCfg.Count != qualityCfgD.Count) {
			foreach (var q in qualityCfgD) {
				if (!qualityCfg.ContainsValue(q.Value)) {
					//qualityCfg.Add(qualityCfg.Count, q.Value);
					config.set("qualityList", _qualityCfgD);
					config.set("qualityRank", config.defaultConfig["qualityRank"]);
					return;
				}
			}
			//}
			rokugaTouroku.config.config.qualityList = 
					qualityCfg;
		}
		void RecListKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete) {
				deleteRowMenu_Click(null, null);
			}
		}
		void setCookieForm() {
			try {
				var isBrowser = config.get("BrowserNum") == "2";
				CookieSourceInfo si = null;
				string id = null, pass = null;
				if (isBrowser) {
					si = SourceInfoSerialize.load(false);
					if (si == null) accountBtn.Text = "デフォルト"; 
					else accountBtn.Text = si.BrowserName + " " + si.ProfileName;
				} else {
					accountBtn.Text = "アカウントログイン";
					id = config.get("accountId");
					pass = config.get("accountPass");
				}
				accountBtn.Tag = new AccountInfo(si, id, pass, isBrowser);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		void AccountBtnClick(object sender, EventArgs e)
		{
			var f = new accountForm(config);
			//util.showMessageBoxCenterForm(f
			f.ShowDialog(this);
			if (f.DialogResult != DialogResult.OK) return;
			accountBtn.Tag = f.ai;
			//f.Tag = f.ai.getArg();
//			/util.debugWriteLine(f.Tag);
			accountBtn.Text = f.si != null ? 
					(f.si.BrowserName + " " + f.si.ProfileName) : "アカウントログイン";
		}
		void RecListDataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			addLogText("list error " + e.ColumnIndex + " " + e.RowIndex + " " + e.Exception.Message + " " + e.Exception.Source + " " + e.Exception.StackTrace + " " + e.Exception.TargetSite);
		}
		void UpBtnClick(object sender, EventArgs e)
		{
			try {
				if (recList.SelectedCells.Count == 0 || recListDataSource.Count == 0) return;
				var selectedIndex = recList.SelectedCells[0].RowIndex;
				if (selectedIndex < 1) return;
				
				var ri = recListDataSource[selectedIndex];
				recListDataSource.RemoveAt(selectedIndex);
				var addIndex = (selectedIndex == 0) ? 0 : (selectedIndex - 1);
				recListDataSource.Insert(addIndex, ri);
				
				recList.ClearSelection();
				recList.Rows[addIndex].Selected = true;
				saveList();
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
			}
		}
		void DownBtnClick(object sender, EventArgs e)
		{
			try {
				var itemCount = recListDataSource.Count;
				if (recList.SelectedCells.Count == 0 || itemCount == 0) return;
				var selectedIndex = recList.SelectedCells[0].RowIndex;
				if (selectedIndex > itemCount - 2) return;
				
				var ri = recListDataSource[selectedIndex];
				recListDataSource.RemoveAt(selectedIndex);
				var addIndex = (selectedIndex == itemCount) ? itemCount : (selectedIndex + 1);
				recListDataSource.Insert(addIndex, ri);
				
				recList.ClearSelection();
				recList.Rows[addIndex].Selected = true;
				saveList();
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace);
			}
		}
		void FoldBtnLabelClick(object sender, EventArgs e)
		{
			//525 698
			//if (sender == null) return;
			var ritu =  (startTimeLabel.Font.Size / 9);
			var a = tableLayoutPanel1.RowStyles;
			//if (tableLayoutPanel1.RowStyles[1].Height == 16 * ritu) {
			if (logText.Visible) {
				tableLayoutPanel1.RowStyles[1].Height = 16 * ritu;
				Height -= (int)(142 * ritu);
				foldBtnLabel.Text = "放送情報";
				logText.Visible = false;
				
			} else {
				tableLayoutPanel1.RowStyles[1].Height = 158 * ritu;
				Height += (int)(142 * ritu);
				foldBtnLabel.Text = "折り畳む";
				logText.Visible = true;
			}
		}
	}
}
