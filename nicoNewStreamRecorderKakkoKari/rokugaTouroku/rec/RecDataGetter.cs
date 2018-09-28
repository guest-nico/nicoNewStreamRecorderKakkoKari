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
using rokugaTouroku;
using rokugaTouroku.info;

namespace rokugaTouroku.rec
{
	/// <summary>
	/// Description of RecDataGetter.
	/// </summary>
	public class RecDataGetter
	{
		private RecListManager rlm;
		
		//public bool isStop = false;
		
		public RecDataGetter(RecListManager rlm)
		{
			this.rlm = rlm;
		}
		public void rec() {
			while (true) {
				try {
					var isAllEnd = true;
					for (var i = 0; i < rlm.recListData.Count; i++) {
						RecInfo ri = (RecInfo)rlm.recListData[i];
						if (ri == null) continue;
						if (ri.state == "待機中" || ri.state == "録画中") isAllEnd = false;
						if (ri.state != "待機中") continue;
						
						Task.Run(() => {recProcess(ri);});
					}
					util.debugWriteLine(isAllEnd);
					if (isAllEnd) break;
				} catch (Exception e) {
					util.debugWriteLine("rdg rec exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
				
				
				Thread.Sleep(1000);
			}
		}
		private void recProcess(RecInfo ri) {
			ri.state = "録画中";
			rlm.form.resetBindingList();
			startRecProcess(ri);
			var r = ri.process.StandardOutput;
			var w = ri.process.StandardInput;
			while (!ri.process.HasExited) {
				var res = r.ReadLine();
				readResProcess(res, w, ri);
			}
			ri.state = (ri.process.ExitCode == 5) ? "録画完了" : "録画失敗";
			rlm.form.resetBindingList();
		}
		private void startRecProcess(RecInfo ri) {
			try {
				ri.process = new Process();
				var si = new ProcessStartInfo();
				si.FileName = "ニコ生新配信録画ツール（仮.exe";
				si.Arguments = "redist " + ri.id + " " + ri.afterFFmpegMode;
				si.CreateNoWindow = true;
				si.UseShellExecute = false;
				si.RedirectStandardInput = true;
				si.RedirectStandardOutput = true;
				si.RedirectStandardError = true;
				ri.process.Start(si);
			} catch (Exception e) {
				rlm.form.addLogText("ニコ生新配信録画ツール（仮.exeを呼び出せませんでした");
				util.debugWriteLine("process start exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		private void readResProcess(string res, StreamWriter w, RecInfo ri) {
			if (res.StartsWith("info:title")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			if (res.StartsWith("info:host")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			if (res.StartsWith("info:title")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			if (res.StartsWith("info:title")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			if (res.StartsWith("info:title")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			if (res.StartsWith("info:title")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			if (res.StartsWith("info:title")) 
				ri.title = util.getRegGroup(res, "/(.*)");
			
		}
	}
}
