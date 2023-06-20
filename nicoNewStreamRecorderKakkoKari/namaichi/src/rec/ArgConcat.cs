/*
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
using System.Threading;

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
		public string concat() {
			Thread.Sleep(500);
			rm.form.addLogText("結合モードに入ります");
			var files = getFiles();
			if (files.Count == 0) {
				rm.form.addLogText(".tsファイルが見つかりませんでした");
				return null;
			}
			if (files.Count == 1) {
				convertFile(files[0]);
				rm.form.addLogText("変換を終了します");
				return null;
			}
			
			foreach (var a in files)
				util.debugWriteLine(a);
			
			util.debugWriteLine("concat get files " + files.Count());
			rm.form.addLogText(files.Count() + "のファイルが見つかりました");
			//var outPath = concatFiles(files);
			
			var outPath = concatFiles(files);
			
//			if (rm.cfg.get("IsAfterRenketuFFmpeg") == "true" ||
//			    int.Parse(rm.cfg.get("afterConvertMode")) > 1) {
			
			rm.form.addLogText("結合を完了しました");
			rm.form.addLogText(outPath);
			return outPath;
		}
		private List<string> getFiles() {
			var ret = new List<string>();
			var keys = new List<int>();
			
			if (arr.Length == 1 && File.Exists(arr[0].Trim()))
				return new List<string>(){arr[0]};
			
			foreach (var f in arr) {
				var _f = f.Trim();
				if (File.Exists(_f) &&
				    util.getRegGroup(_f, "(.+\\.ts$)") != null) {
				    
					util.debugWriteLine(_f);
					var fName = util.getRegGroup(_f, "(.+\\\\)*(.+)", 2);
//					util.debugWriteLine(fName);
					var num = util.getRegGroup(fName, ".+_(\\d+).ts");
					if (num == null) num = util.getRegGroup(fName, "(\\d+)");
//					util.debugWriteLine(num);
					if (num == null) continue;
					keys.Add(int.Parse(num));
					ret.Add(_f);
				}
				if (Directory.Exists(_f)) {
					var files = Directory.GetFiles(_f);
					foreach (var ff in files) {
						if (util.getRegGroup(ff, "(.+\\.ts$)") != null) {
							
							util.debugWriteLine(ff);
							var fName = util.getRegGroup(ff, "(.+\\\\)*(.+)", 2);
//							util.debugWriteLine(fName);
							var num = util.getRegGroup(fName, ".+_(\\d+).ts");
							if (num == null) num = util.getRegGroup(fName, "(\\d+)");
//							util.debugWriteLine(num);
							if (num == null) continue;
							keys.Add(int.Parse(num));
							ret.Add(ff);
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
			
			var outName = getOutFileName(files[0], files.Count == 1);
			outPath = outName;
			var isFFmpegConcat = true;
			if (isFFmpegConcat) {
				new FFMpegConcat(rm, null).concat(outName, files);
			} else {
				using (var outFileStream = new FileStream(outName, FileMode.Append, FileAccess.Write)) {
				//using (var outFileStream = getOutFileStream(files[0])) {
					if (outFileStream == null) {
		//				rm.form.addLogText("出力先パスが取得できませんでした");
						return null;
					}
					util.debugWriteLine("outfname " + outFileStream + outFileStream.Name);
					//outPath = outFileStream.Name;
		
					foreach (var f in files) {
						util.debugWriteLine(f);
						try {
							using (var r = new FileStream(f, FileMode.Open, FileAccess.Read)) {
								var pos = 0;
								var readI = 0;
								var bytes = new byte[1000000];
								while((readI = r.Read(bytes, 0, bytes.Length)) != 0) {
									outFileStream.Write(bytes, 0, readI);
									pos += readI;
								}
							}
							count++;
							
						} catch (Exception e) {
							util.debugWriteLine("arg concat write exception " + f + " " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						}
					}
				}
				if (int.Parse(rm.cfg.get("afterConvertMode")) > 0) {
					var tf = new ThroughFFMpeg(rm);
					tf.start(outPath, true);
				}
			}
			util.debugWriteLine("concated count " + count);
			return outPath;
		}
		private string getOutFileName(string file, bool isSingleFile) {
			var dir = Directory.GetParent(file).ToString();
			var dirName = isSingleFile ? Path.GetFileNameWithoutExtension(file) : Directory.GetParent(file).Name;
			util.debugWriteLine("out parent dir " + dir + " name " + dirName);
			                    
			for (var i = 0; i < 10000; i++) {
				if (isSingleFile) {
					string _f = dir + "/" + dirName + ".ts";
					var _lvid = util.getRegGroup(dirName, "(lv\\d+)");
					if (File.Exists(_f) || Directory.Exists(_f) && 
					    	_f.Length <= 250) return _f;
				}
				var f = dir + "/" + dirName + "_" + i + ".ts";
				var lvid = util.getRegGroup(dirName, "(lv\\d+)");
				if (f.Length > 250) {
					f = dir + "/" + (lvid != null ? lvid : "lv") + ".ts";
					if (f.Length > 250) {
						f = dir + "/out.ts";
						if (f.Length > 250)
							throw new Exception("出力のパスが長すぎます " + f);
					}
				}
				if (File.Exists(f) || Directory.Exists(f)) continue;
				
				try {
					util.debugWriteLine("renketu out fname " + f);			
					//return f;
//					var w = new FileStream(f, FileMode.Append, FileAccess.Write);
					return f;
					//return w;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					rm.form.addLogText(e.Message + e.Source + e.StackTrace);
					try {
						if (lvid != null) f = dir + "/" + lvid + "_" + i + ".ts";
						if (File.Exists(f) || Directory.Exists(f)) continue;
						util.debugWriteLine("renketu out fname " + f);	
						if (f.Length > 250) throw new Exception("出力のパスが長すぎます " + f);
						return f;
						//var w = new FileStream(f, FileMode.Append, FileAccess.Write);
						//return w;
					} catch (Exception ee) {
						util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
						try {
							f = dir + "/out_" + i + ".ts";
							if (File.Exists(f) || Directory.Exists(f)) continue;
							util.debugWriteLine("renketu out fname " + f);	
							if (f.Length > 250) throw new Exception("出力のパスが長すぎます " + f);
							return f;
							//var w = new FileStream(f, FileMode.Append, FileAccess.Write);
							//return w;
						
						} catch (Exception eee) {
							util.debugWriteLine("renketu after exception" + eee.Message + eee.StackTrace + eee.Source + eee.TargetSite);
							rm.form.addLogText("出力先ファイルを作成できませんでした " + eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);
							return null;
						}
					}
				}
			}
			return null;
		}
		private void convertFile(string f) {
			f = f.Trim();
			var ext = util.getRegGroup(f, ".+\\.(.+)");
			if (ext == null) {
				rm.form.addLogText("拡張子が見つかりませんでした");
				return;
			}
			var baseName = util.getRegGroup(f.Substring(0, f.Length - ext.Length - 1), "(.+?)(\\d+)*$");
			util.debugWriteLine(baseName);
			
			for (var i = 0; i < 10000; i++) {
				var n = baseName + i + "." + ext;
				if (!File.Exists(n)) {
					try {
						File.Copy(f, n);
						if (!File.Exists(n)) throw new Exception("ファイルのコピーに失敗しました");
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						rm.form.addLogText("FFmpeg処理のための一時ファイルが作成できませんでした " + n);
						continue;
					}
					new ThroughFFMpeg(rm).start(n, true);
					return;
				}
			}
			return;
		}
	}
}

/*
 * ffmpeg -f concat -safe 0 -i list.txt -c copy concat.avi

where list.txt is

file 'intro_prepped.avi'
file 'intro_prepped.avi'
*/
