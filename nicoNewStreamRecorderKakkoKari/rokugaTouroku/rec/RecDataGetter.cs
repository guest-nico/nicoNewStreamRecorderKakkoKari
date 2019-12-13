/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/09/21
 * Time: 0:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Net;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using rokugaTouroku;
using rokugaTouroku.info;

namespace rokugaTouroku.rec
{
	/// <summary>
	/// Description of RecDataGetter.
	/// </summary>
	public class RecDataGetter
	{
		public RecListManager rlm;
		
		//public bool isStop = false;
		
		public RecDataGetter(RecListManager rlm)
		{
			this.rlm = rlm;
		}
		public void rec() {
			var maxRecordingNum = int.Parse(rlm.cfg.get("rokugaTourokuMaxRecordingNum"));
			while (true) {
				try {
					var isAllEnd = true;
					
					var _count = rlm.form.getRecListCount();
					util.debugWriteLine("rlm.reclistdata.count " + _count + " reclist count " + rlm.form.recList.Rows.Count);
					for (var i = 0; i < _count; i++) {
						if (rlm.rdg == null) return;
							
						util.debugWriteLine("i " + i + " count " + _count);
						RecInfo ri = (RecInfo)rlm.recListData[i];
						util.debugWriteLine(i + " " + ri);
						
						if (ri == null) continue;
						if (ri.state == "待機中" || ri.state == "録画中") isAllEnd = false;
						if (ri.state != "待機中") continue;
						
						if (getRecordingNum(_count, rlm.recListData) < maxRecordingNum &&
						    isListTop(i)) {
							ri.state = "録画中";
							Task.Run(() => {recProcess(ri);});
						}
						Thread.Sleep(2000);
					}
					util.debugWriteLine(isAllEnd);
					if (isAllEnd) break;
				} catch (Exception e) {
					util.debugWriteLine("rdg rec exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
				
				
				Thread.Sleep(1000);
			}
			util.debugWriteLine("rec rdg end");
		}
		private void recProcess(RecInfo ri) {
			util.debugWriteLine("recProcess " + ri.id);
//			ri.state = "録画中";
			var row = rlm.recListData.IndexOf(ri);
			if (row == -1) return;
			rlm.form.resetBindingList(row, "状態", "録画中");
			startRecProcess(ri);
			var r = ri.process.StandardOutput;
			var w = ri.process.StandardInput;
			while (!ri.process.HasExited && rlm.rdg == this) {
				var res = r.ReadLine();
				if (res == null) break;
				util.debugWriteLine("res " + res);
				
				readResProcess(res, w, ri);
			}
			util.debugWriteLine("recProcess loop end wait mae " + ri.id + " " + ri.state);
			ri.process.WaitForExit();
			ri.state = (ri.process.ExitCode == 5) ? "録画完了" : "録画失敗";
			util.debugWriteLine("recProcess loop end wait go " + ri.id + " " + ri.state);
			row = rlm.recListData.IndexOf(ri);
			rlm.form.resetBindingList(row);
		}
		private void startRecProcess(RecInfo ri) {
			util.debugWriteLine("startrecprocess " + ri);
			try {
				ri.process = new Process();
				var si = new ProcessStartInfo();
				si.FileName = "ニコ生新配信録画ツール（仮.exe";
				//si.FileName = "nicoNewStreamRecorderKakkoKari.exe";
				var isGetComment = (ri.recComment == "映像＋コメント" || ri.recComment == "コメントのみ") ? " -IsgetComment=true" : " -IsgetComment=false";
				var isGetRec = (ri.recComment == "映像＋コメント" || ri.recComment == "映像のみ") ? 
					((rlm.cfg.get("EngineMode") == "3") ? " -EngineMode=0" : "") :
					" -EngineMode=3";
				si.Arguments = "-nowindo -stdIO -IsmessageBox=false -IscloseExit=true " + ri.id + " -ts-start=" + ri.tsConfig.startTimeStr + " -ts-end=" + ri.tsConfig.endTimeSeconds + "s -ts-list=" + ri.tsConfig.isOutputUrlList.ToString().ToLower() + " -ts-list-m3u8=" + ri.tsConfig.isM3u8List.ToString().ToLower() + " -ts-list-update=" + (int)ri.tsConfig.m3u8UpdateSeconds + " -ts-list-open=" + ri.tsConfig.isOpenUrlList.ToString().ToLower() + " -ts-list-command=\"" + ri.tsConfig.openListCommand + "\" -ts-vpos-starttime=" + ri.tsConfig.isVposStartTime.ToString().ToLower() + " -afterConvertMode=" + ri.getAfterConvertTypeNum() + " -qualityRank=" + ri.qualityRank + " -IsLogFile=false -std-read " + isGetComment + isGetRec;
				util.debugWriteLine(si.Arguments);
				//si.CreateNoWindow = true;
				si.UseShellExecute = false;
				//si.WindowStyle = ProcessWindowStyle.Hidden;
				si.RedirectStandardInput = true;
				si.RedirectStandardOutput = true;
				si.RedirectStandardError = true;
				ri.process.StartInfo = si;
				ri.process.Start();
			} catch (Exception e) {
				rlm.form.addLogText("ニコ生新配信録画ツール（仮.exeを呼び出せませんでした");
				util.debugWriteLine("process start exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void readResProcess(string res, StreamWriter w, RecInfo ri) {
			if (res.StartsWith("info")) {
				setInfo(res, ri);
				return;
			}
			if (res.StartsWith("msgbox:")) {
				//showMsgBox(res);
			}
			
			
		}
		private void setInfo(string res, RecInfo ri) {
			if (res.StartsWith("info.title:")) 
				ri.title = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.host:")) 
				ri.host = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.communityName:")) 
				ri.communityName = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.url:")) 
				ri.url = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.communityUrl:")) 
				ri.communityUrl = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.description:")) 
				ri.description = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.startTime:")) 
				ri.startTime = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.endTime:")) 
				ri.endTime = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.programTime:")) 
				ri.programTime = util.getRegGroup(res, ":(.*)");
			if (res.StartsWith("info.keikaTime:")) {
				ri.keikaTimeStart = DateTime.Parse(util.getRegGroup(res, ":(.*)"));
			}
			if (res.StartsWith("info.samuneUrl:")) 
				ri.samune = getSamune(util.getRegGroup(res, ":(.*)"));
			if (res.StartsWith("info.log:")) {
				if (ri.log != "") ri.log += "\r\n";
				ri.log += util.getRegGroup(res, ":(.*)");
			}
			var ctrl = util.getRegGroup(res, "\\.(.+?):");
			var val = util.getRegGroup(res, ":(.+)");
			
			var row = rlm.recListData.IndexOf(ri);
			if (row == -1) return;
			rlm.form.resetBindingList(row);
			
			var _count = rlm.form.getRecListCount();
			var selectedRow0 = rlm.form.getRecListSelectedCount();
			var selectedRowIndex = (selectedRow0 > 0) ? rlm.form.recList.SelectedCells[0].RowIndex : -1;
			util.debugWriteLine("setinfo c " + _count + " selected rowindex " + selectedRow0);
			try {
				if (selectedRow0 > 0 && 
				    rlm.recListData[rlm.form.recList.SelectedCells[0].RowIndex] == ri) 
					rlm.form.displayRiInfo(ri, ctrl, val);
			} catch (ArgumentOutOfRangeException e) {
				util.debugWriteLine("exception ok インデックスが範囲を超えています。負でない値で、コレクションのサイズよりも小さくなければなりません。");
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			util.debugWriteLine("setinfo ok");
		}
		public void stopRecording() {
			foreach (RecInfo ri in rlm.recListData) {
				try {
					if (ri.state == "録画中") {
						if (ri.process.HasExited) continue;
						//ri.process.Kill();
						ri.process.StandardInput.WriteLine("stop end");
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private Bitmap getSamune(string url) {
       		WebClient cl = new WebClient();
       		cl.Proxy = null;
			
       		System.Drawing.Icon icon =  null;
			try {
       			util.debugWriteLine("samune url " + url);
       			byte[] pic = cl.DownloadData(url);
				
				var  st = new System.IO.MemoryStream(pic);
				icon = Icon.FromHandle(new System.Drawing.Bitmap(st).GetHicon());
				st.Close();
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				return null;
			}
			return icon.ToBitmap();
		}
		private int getRecordingNum(int count, BindingSource list) {
			var c = 0;
			for (var i = 0; i < count; i++) {
				RecInfo ri = (RecInfo)rlm.recListData[i];
				if (ri.state == "録画中") c++;
			}
			return c;
		}
		private bool isListTop(int nowIndex) {
			for (var i = 0; i < nowIndex; i++) {
				RecInfo ri = (RecInfo)rlm.recListData[i];
				if (ri.state == "待機中") 
					return false;
			}
			return true;
		}
	}
}
