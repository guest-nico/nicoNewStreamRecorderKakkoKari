/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/05/03
 * Time: 20:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing.Text;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using namaichi.rec;
using namaichi;

namespace namaichi.play
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	public class Player
	{
		private MainForm form;
		private config.config config;
		private Process process = null;
		private Process process2 = null;
		private Process commentProcess = null;
		string lastPlayUrl = null;
		private defaultFFplayController ctrl = null;
		private commentForm commentForm = null;
		private bool isDefaultPlayer = false;
		private bool isDefaultCommentPlayer = false;
		
		private bool isRecording = false;
		public bool isReconnect = false;
		
		private bool isUsePlayer = false;
		private bool isUseCommentViewer = false;
		
		public StreamWriter pipeWriter;
		
		public Player(MainForm form, config.config config)
		{
			this.form = form;
			this.config = config;
			
		}
		public void play() {
			util.debugWriteLine("play");
			isUsePlayer = bool.Parse(config.get("IsUsePlayer"));
			isUseCommentViewer = bool.Parse(config.get("IsUseCommentViewer"));
			
			if (!form.isPlayingBtn()) {
				form.setPlayerBtnPlaying(true);
				lastPlayUrl = null;
				
				Task.Run(() => {
		         	if (!getHlsUrl()) {
		         		//end();
		         		form.Invoke((MethodInvoker)delegate() {
		         			form.setPlayerBtnPlaying(false);
		         			form.recBtn.Enabled = true;
						});
		         		//form.rec.rfu.isPlayOnlyMode = false;
		         		return;
		         	}
					
					if (isUsePlayer)
						videoPlay(true);
					if (isUseCommentViewer)
						commentPlay(true);
				});
			} else {
				end();
				
			}
		}
		private void end() {
			util.debugWriteLine("play end");
			Task.Run(() => {
			    form.setPlayerBtnPlaying(false);
				stopPlaying(true, true);
				if (isDefaultPlayer && isUsePlayer) ctrlFormClose();
				if (isDefaultCommentPlayer && isUseCommentViewer) defaultCommentFormClose();
				
				form.Invoke((MethodInvoker)delegate() {
					if (!form.recBtn.Enabled) {
						form.recBtn.Enabled = true;
						form.rec.rec(false);
					}
				    //form.rec.isPlayOnlyMode = false;
				});
			});
		}
		private void videoPlay(bool isStart) {
			isRecording = true;
			isDefaultPlayer = bool.Parse(config.get("IsDefaultPlayer"));
			if (isStart) {
				Task.Run(() => {
					var isStarted = false;
					while (form.isPlayingBtn()) {
		         		if (form.rec.hlsUrl == "end") {
		         			form.rec.hlsUrl = null;
		         			Thread.Sleep(15000);
		         			stopPlaying(true, false);
		         			if (isDefaultPlayer) ctrlFormClose();
		         			break;
				        }
						if (form.rec.hlsUrl == "timeshift") {
							form.addLogText("RTMPのタイムシフト録画中はツールでの視聴ができません。");
		         			form.rec.hlsUrl = null;
		         			stopPlaying(true, true);
		         			if (isDefaultPlayer) ctrlFormClose();
		         			break;
				        }
						
						if (!isPlayable() && !isStarted) {
							Thread.Sleep(500);
							continue;
						} else isStarted = true;
						
						lastPlayUrl = form.rec.hlsUrl;
						
						//isRecording = true;
						sendPlayCommand(isDefaultPlayer);
						while (process == null) Thread.Sleep(500);
						
						try {
							while (true) {
								if ((form.rec.hlsUrl == "end" ||
								     form.rec.hlsUrl == null) && 
							        process.HasExited) break;
								if (!form.isPlayingBtn()) break;
								if (form.rec.rfu == null) break;
							    
								Thread.Sleep(300);
								//var isChangeUrlOk = !DefaultPlayer || form.rec.hlsUrl != lastPlayUrl;
								var isChangeUrlOk = form.rec.hlsUrl != lastPlayUrl;								
								if (isChangeUrlOk && form.rec.hlsUrl != null    
									&& (form.rec.hlsUrl.StartsWith("http") || form.rec.hlsUrl.StartsWith("-vr")) || isReconnect) {
									isReconnect = false;
									
									stopPlaying(true, false);
	
									lastPlayUrl = form.rec.hlsUrl;
									sendPlayCommand(isDefaultPlayer);
	//								if (isDefaultPlayer) {
	//									ctrlFormClose();
	//								}
									var aaa = process.HasExited;
								}
							}
						} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						}
						stopPlaying(true, false);
				    	if (isDefaultPlayer) ctrlFormClose();
				    	//isRecording = false;
						break;
					}
				    isRecording = false;
				    form.setPlayerBtnPlaying(false);
				    
				});
				
			} else {
				
			}
			
		}
		private void sendPlayCommand(bool isDefaultPlayer) {
			Environment.SetEnvironmentVariable("SDL_AUDIODRIVER", "directsound", EnvironmentVariableTarget.Process);
			var exArgs = getExArgs();
			if (isDefaultPlayer) {
				try {
					var volume = (ctrl != null) ? ((ctrl.volume == -10) ? 0 : ctrl.volume) : int.Parse(config.get("volume"));
					util.debugWriteLine("kia 00 " + form.rec.hlsUrl);
					
					if (form.rec.hlsUrl.StartsWith("http"))
						playCommand("ffplay", form.rec.hlsUrl + " -autoexit -volume " + volume + " " + exArgs);
					else 
	//					playCommandStd("MPC-HC.1.7.13.x86/mpc-hc.exe", "-");
						Task.Run(() => playCommandStd("ffplay.exe", form.rec.hlsUrl + " -autoexit -volume " + volume + " " + exArgs));
					
					util.debugWriteLine("kia 0 " + ctrl);
					
					if (ctrl == null) {
						ctrl = new defaultFFplayController(config, process, this);
						form.formAction(() => ctrl.Show(), false);
	            	} else {
	            		ctrl.process = process;
	            		ctrl.reset();
	            	}
					
					util.debugWriteLine("kia 1 " + ctrl);
				} catch (Exception e) {
					util.debugWriteLine("sendPlayCommand exception " + e.Message + e.Source + e.StackTrace + e.StackTrace + e.TargetSite);
					
				}
			} else {
				if (form.rec.hlsUrl.StartsWith("http"))
					playCommand(config.get("anotherPlayerPath"), form.rec.hlsUrl + " " + exArgs);
				else
					playCommandStd(config.get("anotherPlayerPath"), form.rec.hlsUrl + " " + exArgs);
			}
		}
		
		private void commentPlay(bool isStart) {
			isDefaultCommentPlayer = bool.Parse(config.get("IsDefaultCommentViewer"));
			isRecording = true;
			if (isStart) {
				Task.Run(() => {
					if (isDefaultCommentPlayer) {
				        //while (form.rec.wscUrl == null && form.rec.hlsUrl != null) {
						//	Thread.Sleep(300);
						//	continue;
				        //}
				        form.Invoke((MethodInvoker)delegate() {
							commentForm = new commentForm(config, form);
							//commentForm.Show(form);
							commentForm.Show();
				        });
				        while (isRecording) {
							Thread.Sleep(300);
						}
						defaultCommentFormClose();
						
					} else {
				        commentCommand(config.get("anotherCommentViewerPath"), form.rec.recordingUrl);
						
				        /*
 						while (true) {
							Thread.Sleep(300);
							try {
								if (commentProcess.HasExited) break;
							} catch (Exception e) {
								util.debugWriteLine("comment hasexited exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
							}
						}
						*/
					}
				});
			} else {
				
			}
		}
		bool isPlayable() {
			//return form.rec.hlsUrl != null &&
			//		form.rec.hlsUrl != lastPlayUrl;
			return form.rec.hlsUrl != null;
		}
		private void playCommand(string exe, string args) {
			process = new System.Diagnostics.Process();
			process.StartInfo.FileName = exe;
//			process.StartInfo.RedirectStandardOutput = true;
//			process.StartInfo.RedirectStandardError = true;
			if (isDefaultPlayer) {
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
			}

			util.debugWriteLine(args);
			process.StartInfo.Arguments = args;
			
			try {
				process.Start();
				//c
				//Thread.Sleep(5000);
				//moto
				Thread.Sleep(1000);
				if (isDefaultPlayer)
					setPipeName(process);
				
				Task.Run(() => setWindowSize(process));
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.StackTrace + ee.Source + ee.TargetSite);
				
				form.addLogText("プレイヤーの起動中に問題が発生しました");
				form.addLogText("デフォルトプレイヤー: " + isDefaultPlayer.ToString());
				form.addLogText("プレイヤーのパス: " + exe);
				if (string.IsNullOrEmpty(exe)) form.addLogText("プレイヤーのパスが設定されていませんでした");
				else if (!File.Exists(exe)) form.addLogText("プレイヤーのパスの場所にファイルが見つかりませんでした");
				else form.addLogText("その他の理由でプレイヤーを開始できませんでした\n" + exe + "\n引数: " + args);
			}
		}
		private void playCommandStd(string exe, string args) {
			try {
				
				var arg = (args.StartsWith("http")) ? ("-i " + form.rec.hlsUrl + " -f matroska -") : form.rec.hlsUrl;
				process = new Process();
				var si = new ProcessStartInfo();
				var rtmpPath = (bool.Parse(config.get("IsDefaultRtmpPath"))) ? 
						"rtmpdump.exe" : config.get("rtmpPath");
				si.FileName = (args.StartsWith("http")) ? "ffmpeg.exe" : rtmpPath;
				si.Arguments = arg;
				si.RedirectStandardOutput = true;
				si.UseShellExecute = false;
				si.CreateNoWindow = true;
				process.StartInfo = si;
				process.Start();
				
				
				process2 = new Process();
				var ffmpegSi = new ProcessStartInfo();
				ffmpegSi.FileName = exe;
//				var ffmpegArg = "- /new";
				
				var ffmpegArg = "-";
//				ffmpegArg = "-i " + args;
				if (exe.ToLower().IndexOf("mpc-hc") > -1) ffmpegArg += " /new";
				ffmpegSi.Arguments = ffmpegArg;
				ffmpegSi.RedirectStandardInput = true;
				ffmpegSi.RedirectStandardOutput = true;
				ffmpegSi.UseShellExecute = false;
				ffmpegSi.CreateNoWindow = true;
				process2.StartInfo = ffmpegSi;
				process2.Start();
				Thread.Sleep(1000);
				if (isDefaultPlayer)
					setPipeName(process2);
				
				var o = process.StandardOutput.BaseStream;
				var _is = process2.StandardInput.BaseStream;
				
				var b = new byte[100000000];
				while (!process.HasExited && !process2.HasExited) {
					try {
						var i = o.Read(b, 0, b.Length);
						
						_is.Write(b, 0, i);
						_is.Flush();
							
					} catch (Exception ee) {
						Debug.WriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
					}
				}
			} catch (Exception eee) {
				Debug.WriteLine(eee.Message + eee.Source + eee.StackTrace + eee.TargetSite);
			}
			stopPlaying(true, false);
			
		}
		private void commentCommand(string exe, string args) {
			commentProcess = new System.Diagnostics.Process();
			commentProcess.StartInfo.FileName = exe;
//			process.StartInfo.RedirectStandardOutput = true;
//			process.StartInfo.RedirectStandardError = true;
//			process.StartInfo.RedirectStandardInput = true;
//			process.StartInfo.UseShellExecute = false;
//			process.StartInfo.CreateNoWindow = true;
			util.debugWriteLine(args);
			commentProcess.StartInfo.Arguments = args;
			
			try {
				commentProcess.Start();
				
			} catch (Exception ee) {
				util.debugWriteLine("comment exception " + ee.Message + ee.StackTrace);
				form.addLogText("コメントビューワーを開始できませんでした " + exe + " " + args);
			}
		}
		public void stopPlaying(bool isVideoStop, bool isCommentStop) {
			if (isVideoStop) {
				stopProcessCore(process);
				stopProcessCore(process2);
			}
			if (isCommentStop)
				stopProcessCore(commentProcess);
			
				
		}
		private void stopProcessCore(Process p) {
			try {
				if (p != null && !p.HasExited) 
					p.Kill();
				
			} catch (Exception ee) {
				util.debugWriteLine("stop process " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		private void ctrlFormClose() {
			if (ctrl == null || ctrl.IsDisposed) return;
			try {
				ctrl.Invoke((MethodInvoker)delegate() {
					try {
				    	ctrl.Close();
				    } catch (Exception e) {
				    	util.debugWriteLine("ctrl close exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
	           		}
				});
			} catch (Exception ee) {
				util.debugWriteLine("ctrl close2 exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			ctrl = null;
		}
		private void defaultCommentFormClose() {
			if (commentForm == null || commentForm.IsDisposed) return;
			try {
				commentForm.Invoke((MethodInvoker)delegate() {
					try {
				    	commentForm.Close();
				    } catch (Exception e) {
				    	util.debugWriteLine("comment form close exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
	           		}
				});
			} catch (Exception ee) {
				util.debugWriteLine("comment form close2 exception " + ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		public void setCtrlFormKeikaJikan(string s) {
			if (ctrl == null && commentForm == null) return;
			if (ctrl != null) ctrl.setTimeLabel(s);
			
			if (util.getRegGroup(s, "(\\d+:\\d+)/") != null) {
				var __m = util.getRegGroup(s, "(\\d+):\\d+");
				var __s = util.getRegGroup(s, "\\d+:(\\d+)");
				if (__m == null || __s == null) return;
				if (commentForm != null) commentForm.setTime(0, int.Parse(__m), int.Parse(__s));
			} else {
				var __h = util.getRegGroup(s, "(\\d+):");
				var __m = util.getRegGroup(s, "\\d+:(\\d+):");
				var __s = util.getRegGroup(s, "\\d+:\\d+:(\\d+)");
				if (__h == null || __m == null || __s == null) return;
				if (commentForm != null) commentForm.setTime(int.Parse(__h), int.Parse(__m), int.Parse(__s));
			}
		}
		public void addComment(string time, string contents, string userId, string score, string color) {
			if (commentForm != null) commentForm.addComment(time, contents, userId, score, color);
		}
		public void setStatistics(string visit, string comment) {
			if (commentForm != null) commentForm.setStatistics(visit, comment);
		}
		private bool getHlsUrl() {
			if (form.rec.rfu == null) {
				form.rec.hlsUrl = null;
				//form.rec.isPlayOnlyMode = true;
				form.rec.rec(true);
				form.Invoke((MethodInvoker)delegate() {
					form.recBtn.Enabled = false;
				});
				if (form.rec.rfu == null) return false;
				while(form.rec.rfu != null) {
					if (form.rec.hlsUrl == "end") return false;
					if (form.rec.hlsUrl != null) {
						
						return true;
					}
					Thread.Sleep(300);
				}
				return false;
			}
			return true;
		}
		private void setPipeName(Process p) {
			var pn = ((int)(new Random().NextDouble() * 10000)).ToString();
			//form.addLogText(pn);
			p.StandardInput.WriteLine(pn);
			p.StandardInput.Flush();
			
			//a
			/*
			for (var i = 0; i < 10; i++) {
				Thread.Sleep(1000);
				try {
					var server = new NamedPipeClientStream(pn);
					server.Connect(3000);
					pipeWriter = new StreamWriter(server);
				} catch (Exception e) {
					util.debugWriteLine("named pipe sleep i " + i + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
					continue;
				}
			    break;
			}
			*/
			//b
			/*
			Thread.Sleep(7000);
			try {
				var server = new NamedPipeClientStream(pn);
				server.Connect(5000);
				pipeWriter = new StreamWriter(server);
			} catch (Exception e) {
				util.debugWriteLine("named pipe sleep  " + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			*/
			/*
			//c
			Thread.Sleep(1000);
			try {
				var server = new NamedPipeClientStream(pn);
				server.Connect(5000);
				pipeWriter = new StreamWriter(server);
			} catch (Exception e) {
				util.debugWriteLine("named pipe sleep  " + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			*/
			//moto
			Thread.Sleep(1000);
			try {
				var server = new NamedPipeClientStream(pn);
				server.Connect(5000);
				pipeWriter = new StreamWriter(server);
			} catch (Exception e) {
				util.debugWriteLine("named pipe sleep  " + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				form.addLogText("デフォルトのプレイヤーの起動設定中に問題が発生しました" + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			
	//                while (server.IsConnected) {
//        	pipeWriter.WriteLine();
//        	pipeWriter.Flush();
			
		}
		private string getExArgs() {
			var args = config.get("playerArgs");
			try {
				var f = ((WebSocketRecorder)form.rec.wsr).ri.recFolderFile[1];
				if (string.IsNullOrEmpty(f)) args = args.Replace("{f}", "{t}");
				f = util.getRegGroup(f.Replace("\\", "/"), ".+/(.+)");
				args = args.Replace("{f}", "\"" + f + "\"");
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			try {
				var t = form.getTitleLabelText();
				if (t != "") args = args.Replace("{t}", "\"" + t + "\"");
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
			return args;
		}
		bool isBigWindow(util.RECT rect) {
			var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
			var isWidthOver = rect.right - rect.left >= screenBounds.Width - 20;
			var isHeightOver = rect.top - rect.bottom >= screenBounds.Height - 20;
			return isWidthOver || isHeightOver;
		}
		void setWindowSize(Process process) {
			while (!process.HasExited) {
				util.RECT rect;
				if (util.GetWindowRect(process.MainWindowHandle, out rect)) {
					if (isBigWindow(rect)) {
						util.MoveWindow(process.MainWindowHandle, 50, 50, 600, 350, true);
					}
					break;
				}
				Thread.Sleep(500);
			}
		}
	}
}
