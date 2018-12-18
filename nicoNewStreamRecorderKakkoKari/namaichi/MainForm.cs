/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/06
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using SunokoLibrary.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using namaichi.rec;
using namaichi.config;
using namaichi.play;
using namaichi.utility;
using SuperSocket.ClientEngine;

//using System.Diagnostics.Process;

namespace namaichi
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		
		
		public rec.RecordingManager rec;
		private bool isInitRun = true;
		private namaichi.config.config config = new namaichi.config.config();
		private string[] args;
		private play.Player player;
		private string labelUrl;
		
		
		public MainForm(string[] args)
		{
			//args = "-nowindo -stdIO -IsmessageBox=false -IscloseExit=true lv316762771 -ts-start=1785s -ts-end=0s -ts-list=false -ts-list-m3u8=false -ts-list-update=5 -ts-list-open=false -ts-list-command=\"notepad{i}\" -ts-vpos-starttime=true -afterConvertMode=4 -qualityRank=0,1,2,3,4,5 -IsLogFile=true".Split(' ');
			//read std
			if (Array.IndexOf(args, "-std-read") > -1) startStdRead();
			
			#if !DEBUG
				if (config.get("IsLogFile") == "true") 
					config.set("IsLogFile", "false");
			#endif
					
			System.Diagnostics.Debug.Listeners.Clear();
			System.Diagnostics.Debug.Listeners.Add(new Logger.TraceListener());
		    
			InitializeComponent();
			Text = "ニコ生新配信録画ツール（仮 " + util.versionStr;
			
			this.args = args;
			
			rec = new rec.RecordingManager(this, config);
			player = new Player(this, config);
			
//			args = new string[]{"a", "-qualityrank=1,2,3,4,5,0", "lv315967820", "-istitlebarinfo=False", "-ts-start=25h2m", "-openUrlListCommand=notepad"};
//			args = new string[]{"Debug_1.ts"};
			if (Array.IndexOf(args, "-stdIO") > -1) util.isStdIO = true;
			
			var lv = (args.Length == 0) ? null : util.getRegGroup(args[0], "(lv\\d+(,\\d+)*)");
			util.setLog(config, lv);
			
			if (args.Length > 0) {
				var ar = new ArgReader(args, config, this);
				ar.read();
				if (ar.isConcatMode) {
					urlText.Text = string.Join("|", args);
	            	rec.rec();
				} else {
					if (ar.lvid != null) urlText.Text = ar.lvid;
					config.argConfig = ar.argConfig;
					rec.argTsConfig = ar.tsConfig;
					rec.isRecording = true;
//					rec.setArgConfig(args);
					rec.rec();
				}
				if (bool.Parse(config.get("Isminimized"))) {
					this.WindowState = FormWindowState.Minimized;
				}
            }
			
			

			util.debugWriteLine("arg len " + args.Length);
			util.debugWriteLine("arg join " + string.Join(" ", args));
			
			
            //nicoSessionComboBox1.Selector.PropertyChanged += Selector_PropertyChanged;
//            checkBoxShowAll.Checked = bool.Parse(config.get("isAllBrowserMode"));
			//if (isInitRun) initRec();
			try {
				Width = int.Parse(config.get("Width"));
				Height = int.Parse(config.get("Height"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			
		}

		private void recBtnAction(object sender, EventArgs e) {
			rec.isClickedRecBtn = true;
			rec.rec();
			
		}
		/*
		async void Selector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			
			
	
			
            switch(e.PropertyName)
            {
                case "SelectedIndex":
                    var cookieContainer = new CookieContainer();
                    var currentGetter = nicoSessionComboBox1.Selector.SelectedImporter;
                    if (currentGetter != null)
                    {
//                        var result = await currentGetter.GetCookiesAsync(TargetUrl);
                        
//                        var cookie = result.Status == CookieImportState.Success ? result.Cookies["user_session"] : null;
                        
                        //logText.Text += cookie.Name + cookie.Value+ cookie.Expires;
                        
                        //UI更新
//                        txtCookiePath.Text = currentGetter.SourceInfo.CookiePath;
//                        btnOpenCookieFileDialog.Enabled = true;
//                        txtUserSession.Text = cookie != null ? cookie.Value : null;
//                        txtUserSession.Enabled = result.Status == CookieImportState.Success;
                        //Properties.Settings.Default.SelectedSourceInfo = currentGetter.SourceInfo;
                        //Properties.Settings.Default.Save();
//                        config.set("browserNum", nicoSessionComboBox1.Selector.SelectedIndex.ToString());
//                        if (cookie != null) config.set("user_session", cookie.Value);
//                        config.set("isAllBrowserMode", nicoSessionComboBox1.Selector.IsAllBrowserMode.ToString());
                    }
                    else
                    {
//                        txtCookiePath.Text = null;
//                        txtUserSession.Text = null;
//                        txtUserSession.Enabled = false;
//                        btnOpenCookieFileDialog.Enabled = false;
                    }
                    break;
            }
        }

		void btnReload_Click(object sender, EventArgs e)
        { 
			util.debugWriteLine(DateTime.Now.ToString("{W}"));
			var si = nicoSessionComboBox1.Selector.SelectedImporter.SourceInfo;
			util.debugWriteLine(si.EngineId + " " + si.BrowserName + " " + si.ProfileName);
//			var a = new SunokoLibrary.Application.Browsers.FirefoxImporterFactory();
//			foreach (var b in a.GetCookieImporters()) {
//				var c = b.GetCookiesAsync(TargetUrl);
//				c.ConfigureAwait(false);
				
//				util.debugWriteLine(c.Result.Cookies["user_session"]);
//			}
			util.debugWriteLine(nicoSessionComboBox1.Selector.SelectedImporter.SourceInfo.CookiePath);
			//System.IO.Directory.CreateDirectory("aa/ss/u");
			//a.GetCookieImporter(new CookieSourceInfo("
			//var tsk = nicoSessionComboBox1.Selector.UpdateAsync(); 
		}
        void btnOpenCookieFileDialog_Click(object sender, EventArgs e)
        { var tsk = nicoSessionComboBox1.ShowCookieDialogAsync(); }
        void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
        { nicoSessionComboBox1.Selector.IsAllBrowserMode = checkBoxShowAll.Checked;
        	//config.set("isAllBrowserMode", nicoSessionComboBox1.Selector.IsAllBrowserMode.ToString());
        }
        void playBtn_Click(object sender, EventArgs e)
        { player.play();}
        */
        void optionItem_Select(object sender, EventArgs e)
        { 
        	try {
	        	optionForm o = new optionForm(config); o.ShowDialog();
	        } catch (Exception ee) {
        		util.debugWriteLine(ee.Message + " " + ee.StackTrace);
	        }
        }
        
        /*
        public async Task<Cookie> getCookie() {
        	var cookieContainer = new CookieContainer();
            var currentGetter = nicoSessionComboBox1.Selector.SelectedImporter;
            if (currentGetter != null)
            {
            	
            	var result = await currentGetter.GetCookiesAsync(TargetUrl).ConfigureAwait(false);
                var cookie = result.Status == CookieImportState.Success ? result.Cookies["user_session"] : null;
                //logText.Text += cookie.Name + cookie.Value+ cookie.Expires;
                return cookie;
            }
            else return null;
        }
        */
        public void addLogText(string t, bool isInvoke = true) {
       		if (util.isStdIO) Console.WriteLine("info.log:" + t);
       		if (!util.isShowWindow) return;
       		try {
	       		if (this.IsDisposed) return;
	       		if (!this.IsHandleCreated) return;
	       		if (isInvoke) {
		        	this.Invoke((MethodInvoker)delegate() {
		       		       	try {
				        	    string _t = "";
						    	if (logText.Text.Length != 0) _t += "\r\n";
						    	_t += t;
						    	
					    		logText.AppendText(_t);
								if (logText.Text.Length > 200000) 
									logText.Text = logText.Text.Substring(logText.TextLength - 10000);
		       		       	} catch (Exception e) {
		       		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       		       	}
		
					});
	       		} else {
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
	       		}
	       	} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
       		
		}
        public void addLogTextTest(string t) {
       		addLogText(t);
        }
		public void setRecordState(String t) {
       		if (!util.isShowWindow) return;
       		//util.debugWriteLine("setRecordState form");
	       	try {
	       		if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       		    //util.debugWriteLine("setRecordState form after invoke");
	       		    try {
		        	    recordStateLabel.Text = t;
		        	    if (rec.isTitleBarInfo) {
		        	    	Text = t;
		        	    }
	       		    } catch (Exception e) {
	       		       	util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       		    }
		        	   
	        	    //recordStateLabel.AutoSize
				});
	       	} catch (Exception e) {
       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
       		//util.debugWriteLine("setRecordState ok");
		}
        private void initRec() {
        	//util.debugWriteLine(int.Parse(config.get("browserName")));
        	//util.debugWriteLine(bool.Parse(config.get("isAllBrowserMode")));
        	
        	//try {
        	//	nicoSessionComboBox1.SelectedIndex = int.Parse(config.get("browserNum"));
        	//} catch (Exception e) {util.debugWriteLine(333); return;};
        	//var t = getCookie();
			//t.ConfigureAwait(false);
			//util.debugWriteLine(t.Result);
            if (args.Length > 0) {
            	urlText.Text = args[0];
//            	rec = new rec.RecordingManager(this);
            	rec.rec();

            }
			
			isInitRun = false;
        }
		public void setInfo(string host, string hostUrl, 
        		string group, string groupUrl, string title, string url, 
        		string gentei, string openTime, string description, bool isJikken) {
       		if (!util.isShowWindow) return;
       		util.debugWriteLine(hostUrl);
       		
	       	try {
	       		if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       		       	try {
			       		    titleLabel.Links.Clear();
			       		    hostLabel.Links.Clear();
			       		    communityLabel.Links.Clear();
			       		    
			        	    titleLabel.Text = title;
			        	    titleLabel.Links.Add(0, titleLabel.Text.Length, url);
			        	    hostLabel.Text = host;
			        	    if (hostUrl != null) {
				        	    hostLabel.Links.Add(0, hostLabel.Text.Length, hostUrl);
//				        	    hostLabel.LinkArea = new LinkArea(0, hostLabel.Text.Length);
			        	    }
			        	    communityLabel.Text = group;
			        	    if (groupUrl != null) {
			        	    	communityLabel.Links.Add(0, groupLabel.Text.Length, groupUrl);
			        	    }
			        	    genteiLabel.Text = gentei;
			        	    startTimeLabel.Text = openTime;
			        	    descriptLabel.Text = description;
			        	    typeLabel.Text = (isJikken) ? "実験放送" : "ニコニコ生放送";
	       		       	} catch (Exception e) {
	       		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       		       	}
				});
	       	} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
       		//util.debugWriteLine(hostLabel.Text + " " + hostLabel.Links);
		}
		public void setSamune(string url) {
       		if (!util.isShowWindow) return;
       		if (IsDisposed) return;
       		WebClient cl = new WebClient();
       		cl.Proxy = null;
			
       		System.Drawing.Icon icon =  null;
			try {
       			byte[] pic = cl.DownloadData(url);
				
			
				var  st = new System.IO.MemoryStream(pic);
				icon = Icon.FromHandle(new System.Drawing.Bitmap(st).GetHicon());
				st.Close();
				
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				return;
			}
			
			try {
	        	Invoke((MethodInvoker)delegate() {
       			       	try {
						    samuneBox.Image = icon.ToBitmap();
			//       			samuneBox.ImageLocation = url;
							if (bool.Parse(config.get("IstitlebarSamune"))) {
			        	    	this.Icon = icon;
							}
       			       	} catch (Exception e) {
       			       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
       			       	}
       			});
			} catch (Exception e) {
       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
       		}
					
       			
//       			Icon = new System.Drawing.Icon(url);
			
		}
       public void setKeikaJikan(string keikaJikan, string timeLabelStr, string stdIoStr, DateTime keikaTimeStart) {
       		if (!util.isShowWindow) return;
			try {
				if (IsDisposed) return;
				Invoke((MethodInvoker)delegate() {
					try {
						keikaTimeLabel.Text = keikaJikan;
						player.setCtrlFormKeikaJikan(timeLabelStr);
					} catch (Exception e) {
						util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
					}
				});
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
       		if (util.isStdIO && keikaTimeStart != DateTime.MinValue)
       			//Console.WriteLine("info.keikaTime:" + stdIoStr);
       			Console.WriteLine("info.keikaTime:" + keikaTimeStart);
       }
		public void setStatistics(string visit, string comment) {
       		if (!util.isShowWindow) return;
			try {
			   	if (IsDisposed) return;
			    	Invoke((MethodInvoker)delegate() {
			   	       	try {
			       			visitLabel.Text = visit;
			       			commentLabel.Text = comment;
			   	       	} catch (Exception e) {
			   	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			   	       	}
					});
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			player.setStatistics(visit, comment);
		}
       public void addComment(string time, string comment, string userId, string score, bool isTimeShift, string color) {
       	if (!util.isShowWindow) return;
       	try {
       		if (!IsDisposed && !isTimeShift) {
		        	Invoke((MethodInvoker)delegate() {
		       	       	try {
			       	       	var rows = new string[]{time, comment};
			       	       	commentList.Rows.Add(rows);
			       	       	commentList.FirstDisplayedScrollingRowIndex = commentList.Rows.Count - 1;
			       	       	while (commentList.Rows.Count > 20) {
			       	       		commentList.Rows.RemoveAt(0);
			       	       	}
		       	       	} catch (Exception e) {
		       	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	       	}
		       	       	
					});
       		}
       	} catch (Exception e) {
       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
       	}
       	
//       	player.addComment(time, comment, userId, score, color);
       }

		
		void openRecFolderMenu_Click(object sender, EventArgs e)
		{
			openRecFolder();
		}
		void openRecFolder() {
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
		void titleLabel_Click(object _sender, LinkLabelLinkClickedEventArgs e)
		{
			util.debugWriteLine("click");
			LinkLabel sender = (LinkLabel)_sender;
			if (e.Button == MouseButtons.Left) {
				if (sender.Links.Count > 0 && sender.Links[0].Length != 0) {
					string url = (string)sender.Links[0].LinkData;
					if (bool.Parse(config.get("IsdefaultBrowserPath"))) {
						System.Diagnostics.Process.Start(url);
					} else {
						var p = config.get("browserPath");
						System.Diagnostics.Process.Start(p, url);
					}
				}
			} else {
//				if (sender.Links.Count > 0 && sender.Links[0].Length != 0) {
//					labelUrl = (string)sender.Links[0].LinkData;
//					mainWindowRightClickMenu.Show(Cursor.Position);
//				}
			}
		}
		void copyUrlMenu_Clicked(object sender, EventArgs e)
		{
			if (titleLabel.Links.Count > 0 && titleLabel.Links[0].Length != 0) {
				string url = (string)titleLabel.Links[0].LinkData;
				Clipboard.SetText(url);
			}
		}
		void copyCommunityUrlMenu_Clicked(object sender, EventArgs e)
		{
			if (communityLabel.Links.Count > 0 && communityLabel.Links[0].Length != 0) {
				string url = (string)communityLabel.Links[0].LinkData;
				Clipboard.SetText(url);
			}
		}
		void copyHost_UrlMenu_Clicked(object sender, EventArgs e)
		{
			if (hostLabel.Text.Length > 0 &&
					titleLabel.Links.Count > 0 && titleLabel.Links[0].Length != 0) {
				var host = hostLabel.Text;
				var url = (string)titleLabel.Links[0].LinkData;
				Clipboard.SetText(host + " " + url);
			}
		}
		void openRecFolderMenu_Clicked(object sender, EventArgs e)
		{
			openRecFolder();
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
			if (rec.rfu != null) {
				var _m = (rec.isPlayOnlyMode) ? "視聴" : "録画";
				DialogResult res = MessageBox.Show(_m + "中ですが終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
			}
			try{
				util.debugWriteLine("width " + Width.ToString() + " height " + Height.ToString() + " restore width " + RestoreBounds.Width.ToString() + " restore height " + RestoreBounds.Height.ToString());
				if (this.WindowState == FormWindowState.Normal) {
					config.set("Width", Width.ToString());
					config.set("Height", Height.ToString());
				} else {
					config.set("Width", RestoreBounds.Width.ToString());
					config.set("Height", RestoreBounds.Height.ToString());
				}

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			player.stopPlaying(true, true);
			return true;
		}
		public void resetDisplay() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			samuneBox.Image = ((System.Drawing.Image)(resources.GetObject("samuneBox.Image")));
			visitLabel.Text = "";
			commentLabel.Text = "";
			titleLabel.Text = "";
//			titleLabel.Links.Clear();
//			titleLabel.LinkArea = new LinkArea(0,0);
			communityLabel.Text = "";
			hostLabel.Text = "";
			genteiLabel.Text = "";
			startTimeLabel.Text = "";
			keikaTimeLabel.Text = "";
			descriptLabel.Text = "";
			typeLabel.Text = "";
			commentList.Rows.Clear();
			Text = "ニコ生新配信録画ツール（仮";
			Icon = null;
			recordStateLabel.Text = "";
		}
		public void setTitle(string s) {
			if (!util.isShowWindow) return;
			try {
				if (!IsDisposed) {
		        	Invoke((MethodInvoker)delegate() {
						try {
					        Text = s;
						} catch (Exception e) {
		       	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	       		}
					});
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	       	}
		}
		
		void PlayerBtnClick(object sender, EventArgs e)
		{
			player.play();
		}
		public void setPlayerBtnEnable(bool b) {
			if (!util.isShowWindow) return;
			try {
				if (!IsDisposed) {
		        	Invoke((MethodInvoker)delegate() {
						try {
					        playerBtn.Enabled = b;
						} catch (Exception e) {
		       	       		util.debugWriteLine("player btn enabled exception " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	       		}
					});
				}
			} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
		}
		
		
		void mainForm_Load(object sender, EventArgs e)
		{
			if (!util.isShowWindow) return;
			
			var a = util.getJarPath();
			var desc = System.Diagnostics.FileVersionInfo.GetVersionInfo(util.getJarPath()[0] + "/websocket4net.dll");
			if (desc.FileDescription != "WebSocket4Net for .NET 4.5 gettable data bytes") {
				Invoke((MethodInvoker)delegate() {
					System.Windows.Forms.MessageBox.Show("「WebSocket4Net.dll」をver0.86.9以降に同梱されているものと置き換えてください");
				});
			}
			
			
			
			//.net
			var ver = util.Get45PlusFromRegistry();  
			if (ver < 4.52) {
				
				Task.Run(() => {
				    Invoke((MethodInvoker)delegate() {
						var b = new DotNetMessageBox(ver);
						b.Show(this); 
//						System.Windows.Forms.MessageBox.Show("「動作には.NET 4.5.2以上が推奨です。現在は" + ver + "です。");
					});
				});
			}
		}
		
		void versionMenu_Click(object sender, EventArgs e)
		{
			var v = new VersionForm();
			v.ShowDialog();
		}
		void startStdRead() {
			Task.Run(() => {
	         	while (true) {
					var a = Console.ReadLine();
					if (a == null || a.Length == 0) continue;
					if (a == "stop end") {
						if (rec.rfu != null) {
							rec.stopRecording();
						}
						while (rec.recordRunningList.Count > 0) {
							Thread.Sleep(1000);
						}
						Close();
					}
				}
			});
		}
<<<<<<< HEAD
		
		
=======
>>>>>>> e3edae2ec07ab179fa97cf190ec2270511655936
	}
}
