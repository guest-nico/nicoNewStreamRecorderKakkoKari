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
		public bool isChangeList = true;
		
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
					util.debugWriteLine("rlm.reclistdata.count " + _count);// + " reclist count " + rlm.form.recList.Rows.Count);
					for (var i = 0; i < _count; i++) {
						if (rlm.rdg == null) return;
						if (_count != rlm.form.getRecListCount()) break;
						util.debugWriteLine("i " + i + " count " + _count);
						RecInfo ri = (RecInfo)rlm.form.getRecListData(i);
						util.debugWriteLine(i + " " + ri);
						
						if (ri == null) continue;
						if (ri.state == "待機中" || ri.state == "録画中") isAllEnd = false;
						if (ri.state != "待機中") continue;
						
						if (getRecordingNum(_count) < maxRecordingNum &&
						    isListTop(i)) {
							ri.state = "録画中";
							ri.rdg = this;
							Task.Run(() => {recProcess(ri);});
						}
						Thread.Sleep(2000);
					}
					util.debugWriteLine(isAllEnd);
					if (isAllEnd) break;
					
					if (isChangeList) rlm.form.saveList();
					isChangeList = false;
					
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
			try {
				ri.process.BeginOutputReadLine();
				util.debugWriteLine("recProcess loop end wait mae " + ri.id + " " + ri.state);
				ri.process.WaitForExit();
				ri.state = (ri.process.ExitCode == 5) ? "録画完了" : "録画失敗";
			} catch (Exception e) {
				util.debugWriteLine("ri " + ri + " ri.process " + (ri.process == null ? null : ri.process));
				if (ri != null && ri.process != null)
					util.debugWriteLine(ri.process.HasExited + " " + ri.process.ExitCode);
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				ri.state = "録画終了";
			}
			
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
				var isGetComment = (ri.recComment == "映像＋コメント" || ri.recComment == "コメントのみ") ? " -IsgetComment=true" : " -IsgetComment=false";
				var isGetRec = (ri.recComment == "映像＋コメント" || ri.recComment == "映像のみ") ? 
					((rlm.cfg.get("EngineMode") == "3") ? " -EngineMode=0" : "") :
					" -EngineMode=3";
				
				si.Arguments = "-nowindo -stdIO -IsmessageBox=false";
				si.Arguments += " -IscloseExit=true " + ri.id;
				si.Arguments += " -ts-start=" + ri.tsConfig.startTimeStr;
				si.Arguments += " -ts-end=" + ri.tsConfig.endTimeSeconds + "s";
				//si.Arguments += " -ts-list=" + ri.tsConfig.isOutputUrlList.ToString().ToLower();
				//si.Arguments += " -ts-list-m3u8=" + ri.tsConfig.isM3u8List.ToString().ToLower();
				//si.Arguments += " -ts-list-update=" + (int)ri.tsConfig.m3u8UpdateSeconds;
				//si.Arguments += " -ts-list-open=" + ri.tsConfig.isOpenUrlList.ToString().ToLower();
				//si.Arguments += " -ts-list-command=\"" + ri.tsConfig.openListCommand + "\"";
				si.Arguments += " -ts-endtime-delete-pos=" + ri.tsConfig.isDeletePosTime.ToString().ToLower();
				si.Arguments += " -ts-vpos-starttime=" + ri.tsConfig.isVposStartTime.ToString().ToLower();
				si.Arguments += " -ts-starttime-comment=" + ri.tsConfig.isAfterStartTimeComment.ToString().ToLower();
				si.Arguments += " -ts-endtime-comment=" + ri.tsConfig.isBeforeEndTimeComment.ToString().ToLower();
				si.Arguments += " -ts-starttime-open=" + ri.tsConfig.isOpenTimeBaseStartArg.ToString().ToLower();
				si.Arguments += " -ts-endtime-open=" + ri.tsConfig.isOpenTimeBaseEndArg.ToString().ToLower();
				si.Arguments += " -afterConvertMode=" + ri.getAfterConvertTypeNum();
				si.Arguments += " -qualityRank=" + ri.qualityRank;
				si.Arguments += " -std-read ";
				si.Arguments += isGetComment + isGetRec;
				if (ri.ai != null) {
					var arg = ri.ai.getArg();
					if (arg != null) si.Arguments += " -accountSetting=" + arg;
				}
				if (ri.isChase) si.Arguments += " -chase ";
				
				util.debugWriteLine(si.Arguments);
				//si.CreateNoWindow = true;
				si.UseShellExecute = false;
				//si.WindowStyle = ProcessWindowStyle.Hidden;
				si.RedirectStandardInput = true;
				si.RedirectStandardOutput = true;
				si.RedirectStandardError = true;
				ri.process.StartInfo = si;
				ri.process.OutputDataReceived += ri.readHandler;
				ri.process.Start();
			} catch (Exception e) {
				rlm.form.addLogText("ニコ生新配信録画ツール（仮.exeを呼び出せませんでした");
				util.debugWriteLine("process start exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		/*
		private void readResProcess(string res, StreamWriter w, RecInfo ri) {
			if (res.StartsWith("info")) {
				setInfo(res, ri);
				return;
			}
			if (res.StartsWith("msgbox:")) {
				//showMsgBox(res);
			}
			
			
		}
		*/
		public void setInfo(string res, RecInfo ri) {
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
			} else isChangeList = true;
			if (res.StartsWith("info.samuneUrl:")) {
				ri.samuneUrl = util.getRegGroup(res, ":(.*)");
				ri.samune = util.getSamune(ri.samuneUrl);
			}
			if (res.StartsWith("info.log:")) {
				if (ri.log != "") ri.log += "\r\n";
				ri.log += util.getRegGroup(res, ":(.*)");
				if (rlm.form.isDisplayingRi(ri)) 
					rlm.form.setLogText(ri.log);
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
				util.debugWriteLine(e.Message + e.ParamName + e.StackTrace + e.TargetSite);
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
		
		private int getRecordingNum(int count) {
			var c = 0;
			for (var i = 0; i < count; i++) {
				RecInfo ri = (RecInfo)rlm.form.getRecListData(i);
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
