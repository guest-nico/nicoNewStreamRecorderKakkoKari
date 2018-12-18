/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/10/12
 * Time: 0:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using namaichi.info;

namespace namaichi.rec
{
	/// <summary>
	/// Description of DropSegmentProcess.
	/// </summary>
	public class DropSegmentProcess
	{
		private numTaskInfo nti;
		private DateTime lastWroteSegmentDt;
		private int lastSegmentNo;
		private Record rec;
		private string recFolderFileOrigin;
		private RecordFromUrl rfu;
		private RecordingManager rm;
		public DropSegmentProcess(DateTime _lastWroteSegmentDt, int _lastSegmentNo, Record rec, string recFolderFileOrigin, RecordFromUrl rfu, RecordingManager rm) {
//			this.nti = s;
			this.lastWroteSegmentDt = _lastWroteSegmentDt;
			this.lastSegmentNo = _lastSegmentNo;
			this.rec = rec;
			this.recFolderFileOrigin = recFolderFileOrigin;
			this.rfu = rfu;
			this.rm = rm;
		}
		public void start(numTaskInfo nti) {
			this.nti = nti;
			Thread.Sleep(3000);
			var subL = rfu.subGotNumTaskInfo;
			rec.addDebugBuf("drop segment process s.dt " + nti.dt.ToString() + " lastwrote " + lastWroteSegmentDt.ToString() + " now " + DateTime.Now.ToString() + " nti.no " + nti.no + " nti.originNo " + nti.originNo + " lastsegmentno " + lastSegmentNo);
			if (rfu.subGotNumTaskInfo != null) {
				rec.addDebugBuf("drop segment process start count " + subL.Count);
				if (rfu.subGotNumTaskInfo.Count > 0)
					rec.addDebugBuf("drop segment process subList min " + subL[0].dt + " " + subL[0].no + " max " + subL[subL.Count - 1].dt + " " + subL[subL.Count - 1].no);
			}
			var fName = writeNukeSegment();
			rec.addDebugBuf("drop fname " + fName);
			var dropTime = (nti.no - lastSegmentNo - 1) * nti.second;
			var msg = lastWroteSegmentDt.ToString() + "ぐらいから" + nti.dt.ToString() + "ぐらいまでの動画データが取得できませんでした。(" + dropTime + "秒)";
			string hokanMsg = (fName == null) ? "補完設定がされていませんでした。" : 
				((fName == "") ? "補完用データがありませんでした。" : ("補完を試みました。" + fName));
			if (rm.cfg.get("IsSegmentNukeInfo") == "true")
				writeNukeInfo(msg, fName, nti.no, hokanMsg);
			else 
				rec.addDebugBuf("write nuke info no");
				
			rm.form.addLogText(msg);
			rm.form.addLogText(hokanMsg, true);
		}
		private void writeNukeInfo(string msg, string fName, int reStartSegmentNo, string hokanMsg) {
			var sw = new StreamWriter(recFolderFileOrigin + "n.txt", true);
			sw.WriteLine(msg);
			var msgSeg = "最終取得セグメントNo." + lastSegmentNo;
			msgSeg += (reStartSegmentNo == -1) ? " 録画終了まで" : (" 再開セグメント " + reStartSegmentNo);
			sw.WriteLine(msgSeg);
//			for (var i = lastSegmentNo + 1; i < nti.no; i++) {
//				sw.WriteLine("セグメントNo." + i);
//			}
			sw.WriteLine(hokanMsg);
			sw.WriteLine();
			sw.Close();
			
		}
		private string writeNukeSegment() {
			try {
				if (rfu.subGotNumTaskInfo == null) return null;
				if (rfu.subGotNumTaskInfo.Count == 0) return "";
				 
				var name = getOkFileName();
				
				var c = getCount(rfu.subGotNumTaskInfo);
//				util.debugWriteBuf("write nuke segment start seg dt " + rfu.subGotNumTaskInfo[0].dt + " no " + rfu.subGotNumTaskInfo[0].no + " end dt " + rfu.subGotNumTaskInfo[c].dt + " end no " + rfu.subGotNumTaskInfo[c].no);
				rec.addDebugBuf("drop segment firstData " + rfu.firstFlvData + " len " + rfu.firstFlvData.Length + " rfu.isSubAccountHokan " + rfu.isSubAccountHokan);
				
				var fs = new FileStream(name, FileMode.Create, FileAccess.Write);
				if (rfu.isSubAccountHokan) {
					if (rfu.firstFlvData != null && !rfu.isSubAccountHokan) {
						fs.Write(rfu.firstFlvData, 0, rfu.firstFlvData.Length);
						rec.addDebugBuf("first data write " + name);
					}
					while (rfu.subGotNumTaskInfo.Count > 0) {
						rec.addDebugBuf("drop segment rfu.subGotNumTaskInfo[0] " + rfu.subGotNumTaskInfo[0].dt);
						if (nti != null && rfu.subGotNumTaskInfo[0].dt > nti.dt + TimeSpan.FromSeconds(15)) break;
						fs.Write(rfu.subGotNumTaskInfo[0].res, 0, rfu.subGotNumTaskInfo[0].res.Length);
						rfu.subGotNumTaskInfo.Remove(rfu.subGotNumTaskInfo[0]);
					}
					fs.Close();
				} else {
					rtmpSubNtiWrite(fs);
				}
				
				
//				rfu.subGotNumTaskInfo.RemoveRange(0, i);
				return name;
			} catch (Exception e) {
				rec.addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return "補完中にエラーが発生しました。";
			}
		}
		private void rtmpSubNtiWrite(FileStream fs) {
			//get ts data
			while (true) {
				var ffmpegP = getFFmpegProcess();
				
				var _os = ffmpegP.StandardOutput.BaseStream;
				var ffmpegReadTask = Task.Run(() => ffmpegRead(fs, _os));
				var _is = ffmpegP.StandardInput.BaseStream;
				if (rfu.firstFlvData != null && !rfu.isSubAccountHokan) {
					util.debugWriteLine("firstFlvData write len" + rfu.firstFlvData.Length);
					_is.Write(rfu.firstFlvData, 0, rfu.firstFlvData.Length);
				}
				var no = rfu.subGotNumTaskInfo[0].no;
				var isEnd = false;
				while (true) {
					if ((rfu.subGotNumTaskInfo.Count == 0) ||
					   		(nti != null && rfu.subGotNumTaskInfo[0].dt > 
					     	nti.dt + TimeSpan.FromSeconds(15))) {
						isEnd = true;
						break;
					}
					if (rfu.subGotNumTaskInfo[0].no != no) break;
					rec.addDebugBuf("drop segment rfu.subGotNumTaskInfo[0].dt " + rfu.subGotNumTaskInfo[0].dt);
					_is.Write(rfu.subGotNumTaskInfo[0].res, 0, rfu.subGotNumTaskInfo[0].res.Length);
					rfu.subGotNumTaskInfo.Remove(rfu.subGotNumTaskInfo[0]);
				}
				_is.Close();
				ffmpegReadTask.Wait();
				if (isEnd) break;
			}
			fs.Close();
			var ff = new ThroughFFMpeg(rm);
			ff.start(fs.Name, false);
		}
		private Process getFFmpegProcess() {
			var ffmpegP = new Process();
			var ffmpegSi = new ProcessStartInfo();
			ffmpegSi.FileName = "ffmpeg.exe";
			var ffmpegArg = "-i - -c copy -y -f mpegts pipe:1";
//			var ffmpegArg = "-i - -c copy -y " + name;
			ffmpegSi.Arguments = ffmpegArg;
			ffmpegSi.RedirectStandardInput = true;
			ffmpegSi.RedirectStandardOutput = true;
			ffmpegSi.UseShellExecute = false;
			ffmpegSi.CreateNoWindow = true;
			ffmpegP.StartInfo = ffmpegSi;
			ffmpegP.Start();
			return ffmpegP;
		}
		private void ffmpegRead(FileStream fs, Stream _os) {
			while (true) {
				try {
					var b = new byte[100000];
					var i = _os.Read(b, 0, b.Length);
					if (i == 0) return;
					fs.Write(b, 0, i);
				} catch (Exception e) {
					rec.addDebugBuf(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private string getOkFileName() {
			var ext = (rfu.isSubAccountHokan) ? ".ts" : ".flv";
			ext = ".ts";
			for (var i = 0; i < 1000; i++) {
				var name = recFolderFileOrigin + "n" + i + ext;
				if (!File.Exists(name) && !Directory.Exists(name))
					return name;
			}
			return null;
		}
		private int getCount(List<numTaskInfo> l) {
			while (true) {
				try {
					var ret = l.Count;
					return ret;
				} catch (Exception e) {
					
				}
			}
		}
		public void writeRemaining() {
			rec.addDebugBuf("write remaining dropSegment process lastWrote dt " + lastWroteSegmentDt.ToString() + " lastSegmentNo " + lastSegmentNo + " now " + DateTime.Now.ToString() + " sub got list len " + rfu.subGotNumTaskInfo.Count);
			rec.addDebugBuf("write remaing drop segment process subGotListcount " + rfu.subGotNumTaskInfo.Count);
			var second = (rfu.subGotNumTaskInfo.Count == 0) ? -1 : rfu.subGotNumTaskInfo[0].second;
			if (second == -1) {
				rm.form.addLogText("補完用データがありませんでした。");
				return;
			}
			//var baseSecond = (rfu.subGotNumTaskInfo.Count > 1) ? rfu.subGotNumTaskInfo[0].second : -1;
			var fName = writeNukeSegment();
			//var dropTime = (baseSecond == -1) ? "最終セグメントまで" : (((nti.no - lastSegmentNo - 1) * nti.second).ToString() + "秒");
			//var dropTime = "最終セグメントまで";
			var msg = lastWroteSegmentDt.ToString() + "ぐらいから" + "最終セグメントまでの動画データが取得できませんでした。(" + (rfu.subGotNumTaskInfo.Count * second) + ")";
			string hokanMsg = (fName == null) ? "補完設定がされていませんでした。" : 
				((fName == "") ? "補完用データがありませんでした。" : ("補完を試みました。" + fName));
			writeNukeInfo(msg, fName, -1, hokanMsg);
			rm.form.addLogText(msg);
			rm.form.addLogText(hokanMsg);
		}
	}
}
