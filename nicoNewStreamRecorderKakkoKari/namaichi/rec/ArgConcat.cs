<<<<<<< HEAD
﻿/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/07/23
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace namaichi.rec
{
	/// <summary>
	/// Description of ArgConcat.
	/// </summary>
	public class ArgConcat
	{
		private RecordingManager rm;
		private string[] arr;
		
		public ArgConcat(RecordingManager rm, string[] arr)
		{
			this.rm = rm;
			this.arr = arr;
		}
		public void concat() {
			rm.form.addLogText("結合モードに入ります");
			var files = getFiles();
			
			foreach (var a in files)
				util.debugWriteLine(a);
			
			util.debugWriteLine("concat get files " + files.Count());
			rm.form.addLogText(files.Count() + "のファイルが見つかりました");
			var outPath = concatFiles(files);
			
			if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true") {
				var tf = new ThroughFFMpeg(rm);
				tf.start(outPath, false);
			}
			rm.form.addLogText("結合を完了しました");
			rm.form.addLogText(outPath);
		}
		private List<string> getFiles() {
			var ret = new List<string>();
			var keys = new List<int>();
			foreach (var f in arr) {
				var _f = f.Trim();
				if (File.Exists(_f) && 
				    util.getRegGroup(_f, "(.+\\.ts$)") != null) {
					ret.Add(_f);
					
					util.debugWriteLine(_f);
					var fName = util.getRegGroup(_f, "(.+\\\\)*(.+)", 2);
//					util.debugWriteLine(fName);
					var num = util.getRegGroup(fName, "(\\d+)");
//					util.debugWriteLine(num);
					if (num == null) continue;
					keys.Add(int.Parse(num));
				}
				if (Directory.Exists(_f)) {
					var files = Directory.GetFiles(_f);
					foreach (var ff in files) {
						if (util.getRegGroup(ff, "(.+\\.ts$)") != null) {
							ret.Add(ff);
							
							util.debugWriteLine(ff);
							var fName = util.getRegGroup(ff, "(.+\\\\)*(.+)", 2);
//							util.debugWriteLine(fName);
							var num = util.getRegGroup(fName, "(\\d+)");
//							util.debugWriteLine(num);
							if (num == null) continue;
							keys.Add(int.Parse(num));
					
						}
					}
				}
			}
			string[] retArr = ret.ToArray();
			Array.Sort(keys.ToArray(), retArr);
//			ret.OrderBy(n => int.Parse(util.getRegGroup(n, ".+\\\\[\\D]*(\\d+)")));
			return new List<string>(retArr);
		}
		private string concatFiles(List<string> files) {
			if (files.Count() == 0) return null;
			rm.form.addLogText("結合を開始します");
			
			string outPath = null; 
			var count = 0;
			using (var outFileStream = getOutFileStream(files[0])) {
				if (outFileStream == null) {
	//				rm.form.addLogText("出力先パスが取得できませんでした");
					return null;
				}
				util.debugWriteLine("outfname " + outFileStream + outFileStream.Name);
				outPath = outFileStream.Name;
	
				foreach (var f in files) {
					util.debugWriteLine(f);
					try {
						var r = new FileStream(f, FileMode.Open, FileAccess.Read);
						
						var pos = 0;
						var readI = 0;
						var bytes = new byte[1000000];
						while((readI = r.Read(bytes, 0, bytes.Length)) != 0) {
							outFileStream.Write(bytes, 0, readI);
							pos += readI;
						}
						r.Close();
						count++;
						
					} catch (Exception e) {
						util.debugWriteLine("arg concat write exception " + f + " " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
					}
				}
			}
			util.debugWriteLine("concated count " + count);
			return outPath;
		}
		private FileStream getOutFileStream(string file) {
			var dir = Directory.GetParent(file).ToString();
			var dirName = Directory.GetParent(file).Name;
			util.debugWriteLine("out parent dir " + dir + " name " + dirName);
			                    
			for (var i = 0; i < 10000; i++) {
				var f = dir + "/" + dirName + "_" + i + ".ts";
				var lvid = util.getRegGroup(dirName, "(lv\\d+)");
				if (File.Exists(f) || Directory.Exists(f)) continue;
				
				try {
					util.debugWriteLine("renketu out fname " + f);			
					var w = new FileStream(f, FileMode.Append, FileAccess.Write);
					return w;
				} catch (Exception e) {
					try {
						if (lvid != null) f = dir + "/" + lvid + "_" + i + ".ts";
						if (File.Exists(f) || Directory.Exists(f)) continue;
						util.debugWriteLine("renketu out fname " + f);	
						var w = new FileStream(f, FileMode.Append, FileAccess.Write);
						return w;
					} catch (Exception ee) {
						try {
							f = dir + "/out_" + i + ".ts";
							if (File.Exists(f) || Directory.Exists(f)) continue;
							util.debugWriteLine("renketu out fname " + f);	
							var w = new FileStream(f, FileMode.Append, FileAccess.Write);
							return w;
						
						} catch (Exception eee) {
							util.debugWriteLine("renketu after exception" + eee.Message + eee.StackTrace + eee.Source + eee.TargetSite);
							rm.form.addLogText("出力先ファイルを作成できませんでした");
							return null;
						}
					}
				}
			}
			return null;
		}
	}
}
=======
﻿/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/07/23
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace namaichi.rec
{
	/// <summary>
	/// Description of ArgConcat.
	/// </summary>
	public class ArgConcat
	{
		private RecordingManager rm;
		private string[] arr;
		
		public ArgConcat(RecordingManager rm, string[] arr)
		{
			this.rm = rm;
			this.arr = arr;
		}
		public void concat() {
			rm.form.addLogText("結合モードに入ります");
			var files = getFiles();
			
			foreach (var a in files)
				util.debugWriteLine(a);
			
			util.debugWriteLine("concat get files " + files.Count());
			rm.form.addLogText(files.Count() + "のファイルが見つかりました");
			var outPath = concatFiles(files);
			
			if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true") {
				var tf = new ThroughFFMpeg(rm);
				tf.start(outPath);
			}
			rm.form.addLogText("結合を完了しました");
			rm.form.addLogText(outPath);
		}
		private List<string> getFiles() {
			var ret = new List<string>();
			var keys = new List<int>();
			foreach (var f in arr) {
				var _f = f.Trim();
				if (File.Exists(_f) && 
				    util.getRegGroup(_f, "(.+\\.ts$)") != null) {
					ret.Add(_f);
					
					util.debugWriteLine(_f);
					var fName = util.getRegGroup(_f, "(.+\\\\)*(.+)", 2);
//					util.debugWriteLine(fName);
					var num = util.getRegGroup(fName, "(\\d+)");
//					util.debugWriteLine(num);
					if (num == null) continue;
					keys.Add(int.Parse(num));
				}
				if (Directory.Exists(_f)) {
					var files = Directory.GetFiles(_f);
					foreach (var ff in files) {
						if (util.getRegGroup(ff, "(.+\\.ts$)") != null) {
							ret.Add(ff);
							
							util.debugWriteLine(ff);
							var fName = util.getRegGroup(ff, "(.+\\\\)*(.+)", 2);
//							util.debugWriteLine(fName);
							var num = util.getRegGroup(fName, "(\\d+)");
//							util.debugWriteLine(num);
							if (num == null) continue;
							keys.Add(int.Parse(num));
					
						}
					}
				}
			}
			string[] retArr = ret.ToArray();
			Array.Sort(keys.ToArray(), retArr);
//			ret.OrderBy(n => int.Parse(util.getRegGroup(n, ".+\\\\[\\D]*(\\d+)")));
			return new List<string>(retArr);
		}
		private string concatFiles(List<string> files) {
			if (files.Count() == 0) return null;
			rm.form.addLogText("結合を開始します");
			
			string outPath = null; 
			var count = 0;
			using (var outFileStream = getOutFileStream(files[0])) {
				if (outFileStream == null) {
	//				rm.form.addLogText("出力先パスが取得できませんでした");
					return null;
				}
				util.debugWriteLine("outfname " + outFileStream + outFileStream.Name);
				outPath = outFileStream.Name;
	
				foreach (var f in files) {
					util.debugWriteLine(f);
					try {
						var r = new FileStream(f, FileMode.Open, FileAccess.Read);
						
						var pos = 0;
						var readI = 0;
						var bytes = new byte[1000000];
						while((readI = r.Read(bytes, 0, bytes.Length)) != 0) {
							outFileStream.Write(bytes, 0, readI);
							pos += readI;
						}
						r.Close();
						count++;
						
					} catch (Exception e) {
						util.debugWriteLine("arg concat write exception " + f + " " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
					}
				}
			}
			util.debugWriteLine("concated count " + count);
			return outPath;
		}
		private FileStream getOutFileStream(string file) {
			var dir = Directory.GetParent(file).ToString();
			var dirName = Directory.GetParent(file).Name;
			util.debugWriteLine("out parent dir " + dir + " name " + dirName);
			                    
			for (var i = 0; i < 10000; i++) {
				var f = dir + "/" + dirName + "_" + i + ".ts";
				var lvid = util.getRegGroup(dirName, "(lv\\d+)");
				if (File.Exists(f) || Directory.Exists(f)) continue;
				
				try {
					util.debugWriteLine("renketu out fname " + f);			
					var w = new FileStream(f, FileMode.Append, FileAccess.Write);
					return w;
				} catch (Exception e) {
					try {
						if (lvid != null) f = dir + "/" + lvid + "_" + i + ".ts";
						if (File.Exists(f) || Directory.Exists(f)) continue;
						util.debugWriteLine("renketu out fname " + f);	
						var w = new FileStream(f, FileMode.Append, FileAccess.Write);
						return w;
					} catch (Exception ee) {
						try {
							f = dir + "/out_" + i + ".ts";
							if (File.Exists(f) || Directory.Exists(f)) continue;
							util.debugWriteLine("renketu out fname " + f);	
							var w = new FileStream(f, FileMode.Append, FileAccess.Write);
							return w;
						
						} catch (Exception eee) {
							util.debugWriteLine("renketu after exception" + eee.Message + eee.StackTrace + eee.Source + eee.TargetSite);
							rm.form.addLogText("出力先ファイルを作成できませんでした");
							return null;
						}
					}
				}
			}
			return null;
		}
	}
}
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
