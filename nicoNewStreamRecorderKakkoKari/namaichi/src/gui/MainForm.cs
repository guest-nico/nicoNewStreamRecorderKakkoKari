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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using SunokoLibrary.Application;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Configuration;
using System.IO;
//using System.Text;
using System.Threading;
using System.Security.AccessControl;
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
		//private bool isInitRun = true;
		private namaichi.config.config config = new namaichi.config.config();
		public string[] args;
		private play.Player player;
		private Thread madeThread;
		private Size originalSize = Size.Empty;
		private Icon defIcon;
		public string recEndProcess = null;
		
		public MainForm(string[] args)
		{
			string lv = null;
			foreach (var arg in args) {
				lv = util.getRegGroup(arg, "(lv\\d+(,\\d+)*)");
				if (lv != null) break;
			}
			util.setLog(config, lv);
			
			madeThread = Thread.CurrentThread;
			
			//read std
			if (Array.IndexOf(args, "-std-read") > -1) startStdRead();
			
			System.Diagnostics.Debug.Listeners.Clear();
			System.Diagnostics.Debug.Listeners.Add(new Logger.TraceListener());
		    
			InitializeComponent();
			Text = "ニコ生新配信録画ツール（仮 " + util.versionStr;
			defIcon = Icon;

			this.args = args;
			
			rec = new rec.RecordingManager(this, config);
			player = new Player(this, config);
			
			if (Array.IndexOf(args, "-stdIO") > -1) util.isStdIO = true;
			
			util.debugWriteLine("arg len " + args.Length);
			util.debugWriteLine("arg join " + string.Join(" ", args));
			
			var fontSize = config.get("fontSize");  
			if (fontSize != "9")
				util.setFontSize(int.Parse(fontSize), this, true, 400);
			
			try {
				Width = int.Parse(config.get("Width"));
				Height = int.Parse(config.get("Height"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			
			try {
				if (bool.Parse(config.get("IsRestoreLocation"))) {
					var x = config.get("X");
					var y = config.get("Y");
					if (x != "" && y != "" && int.Parse(x) >= 0 && int.Parse(y) >= 0) {
						StartPosition = FormStartPosition.Manual;
						Location = new Point(int.Parse(x), int.Parse(y));
					}
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				StartPosition = FormStartPosition.WindowsDefaultLocation;
			}
			
			if (bool.Parse(config.get("IsMiniStart")))
				changeSize(true);
			
			if (config.get("qualityRank").Split(',').Length == 5)
				config.set("qualityRank", config.get("qualityRank") + ",5");
			
			changeRecBtnClickEvent(bool.Parse(config.get("IsRecBtnOnlyMouse")));
			
		}
		private void formInitSetting() {
			setBackColor(Color.FromArgb(int.Parse(config.get("recBackColor"))));
			setForeColor(Color.FromArgb(int.Parse(config.get("recForeColor"))));
			setLinkColor(Color.FromArgb(int.Parse(config.get("recLinkColor"))));
		}
		private void init() {
			
			if (args.Length > 0) {
				var ar = new ArgReader(args, config, this);
				ar.read();
				if (ar.isConcatMode) {
					urlText.Text = string.Join("|", args);
	            	rec.rec(false);
				} else {
					if (ar.lvid != null) urlText.Text = ar.lvid;
					config.argConfig = ar.argConfig;
					rec.argTsConfig = ar.tsConfig;
					rec.isRecording = true;
//					rec.setArgConfig(args);
					if (ar.isPlayMode) player.play();
					else rec.rec(false);
				}
				if (bool.Parse(config.get("Isminimized"))) {
					this.WindowState = FormWindowState.Minimized;
				}
            }
		}

		private void recBtnAction(object sender, EventArgs e) {
			rec.isClickedRecBtn = true;
			rec.rec(false);
			
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
	        	optionForm o = new optionForm(config);
	        	var size = config.get("fontSize");
	        	if (o.ShowDialog() == DialogResult.OK) {
	        		changeRecBtnClickEvent(bool.Parse(config.get("IsRecBtnOnlyMouse")));
	        		
	        		var newSize = config.get("fontSize");
	        		if (size != newSize) {
	        			//var formSize = Size;
	        			//var loc = Location;
	        			loadControlLayout();
	        			util.setFontSize(int.Parse(newSize), this, true, 400);
	        			//Size = formSize;
	        			//Location = loc;
	        			//check.popup.setPopupSize();
	        		}
	        	}
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
       		
       		formAction(() => {
				string _t = "";
		    	if (logText.Text.Length != 0) _t += "\r\n";
		    	_t += t;
		    	
	    		logText.AppendText(_t);
				if (logText.Text.Length > 200000) 
					logText.Text = logText.Text.Substring(logText.TextLength - 10000);
								
			});
		}
		public void setRecordState(String t, string titleT = null) {
       		//util.debugWriteLine("setRecordState form");
       		formAction(() => {
		       	try {
	        	    recordStateLabel.Text = t;
	        	    if (rec.isTitleBarInfo) {
	        	    	Text = titleT == null ? t : titleT;
	        	    }
		       	} catch (Exception e) {
	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	}
			});
		}
       public void setRecordStateComplete() {
       		//util.debugWriteLine("setRecordState form");
       		formAction(() => {
		       	try {
       		    	var t = "(complete)";
       		    	//var per = util.getRegGroup(recordStateLabel, "(\\(\\d+%\\))");
       		    	//recordStateLabel.Text = recordStateLabel.Text.Replace(per, "(100)"
	        	    recordStateLabel.Text += t;
	        	    if (rec.isTitleBarInfo) {
	        	    	Text += t;
	        	    }
		       	} catch (Exception e) {
	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	}
			});
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
            	rec.rec(false);

            }
			
			//isInitRun = false;
        }
		public void setInfo(string host, string hostUrl, 
        		string group, string groupUrl, string title, string url, 
        		string gentei, string openTime, string description, bool isJikken, 
        		string endTime) {
       		util.debugWriteLine(hostUrl);
       		formAction(() => {
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
	        	    endTimeLabel.Text = endTime;
		       	} catch (Exception e) {
		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	}
			});
		}
		public void setSamune(string url) {
       		if (!util.isShowWindow) return;
       		if (IsDisposed) return;
       		WebClient cl = new WebClient();
       		cl.Proxy = null;
			
       		System.Drawing.Icon icon =  null;
			try {
       			byte[] pic = cl.DownloadData(url);
				
       			using (var st = new System.IO.MemoryStream(pic)) {
					icon = Icon.FromHandle(new System.Drawing.Bitmap(st).GetHicon());
       			}
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				return;
			}
			
       		formAction(() => {
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
					
       			
//       			Icon = new System.Drawing.Icon(url);
			
		}
       public void setKeikaJikan(string keikaJikan, string timeLabelStr, string stdIoStr, DateTime keikaTimeStart) {
       		formAction(() => {
				try {
					keikaTimeLabel.Text = keikaJikan;
					player.setCtrlFormKeikaJikan(timeLabelStr);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				}
			});
       		//if (!util.isShowWindow) return;
       		if (util.isStdIO && keikaTimeStart != DateTime.MinValue)
       			//Console.WriteLine("info.keikaTime:" + stdIoStr);
       			Console.WriteLine("info.keikaTime:" + keikaTimeStart);
       }
		public void setStatistics(string visit, string comment) {
       		formAction(() => {
				try {
	       			visitLabel.Text = visit;
	       			commentLabel.Text = comment;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				}
			});
			player.setStatistics(visit, comment);
		}
       public void addComment(string time, string comment, string userId, string score, string color) {
	       	if (!bool.Parse(config.get("IsDisplayComment"))) return;
	       	formAction(() => {
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
		public void titleLabel_Click(object _sender, LinkLabelLinkClickedEventArgs e)
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
			try {
				if (titleLabel.Links.Count > 0 && titleLabel.Links[0].Length != 0) {
					string url = (string)titleLabel.Links[0].LinkData;
					Clipboard.SetText(url);
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		void copyCommunityUrlMenu_Clicked(object sender, EventArgs e)
		{
			try {
				if (communityLabel.Links.Count > 0 && communityLabel.Links[0].Length != 0) {
					string url = (string)communityLabel.Links[0].LinkData;
					Clipboard.SetText(url);
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		void copyHost_UrlMenu_Clicked(object sender, EventArgs e)
		{
			try {
				if (hostLabel.Text.Length > 0 &&
						titleLabel.Links.Count > 0 && titleLabel.Links[0].Length != 0) {
					var host = hostLabel.Text;
					var url = (string)titleLabel.Links[0].LinkData;
					Clipboard.SetText(host + " " + url);
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
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
			if (rec.rfu != null && bool.Parse(config.get("IsConfirmCloseMsgBox"))) {
				var _m = (rec.rfu.isPlayOnlyMode) ? "視聴" : "録画";
				DialogResult res = MessageBox.Show(_m + "中ですが終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
			}
			try{
				util.debugWriteLine("width " + Width.ToString() + " height " + Height.ToString() + " restore width " + RestoreBounds.Width.ToString() + " restore height " + RestoreBounds.Height.ToString());
				if (originalSize != Size.Empty) {
					config.set("Width", originalSize.Width.ToString());
					config.set("Height", originalSize.Height.ToString());
					config.set("X", Location.X.ToString());
					config.set("Y", Location.Y.ToString());
				} else {
					if (this.WindowState == FormWindowState.Normal) {
						config.set("Width", Width.ToString());
						config.set("Height", Height.ToString());
						config.set("X", Location.X.ToString());
						config.set("Y", Location.Y.ToString());
					} else {
						config.set("Width", RestoreBounds.Width.ToString());
						config.set("Height", RestoreBounds.Height.ToString());
						config.set("X", RestoreBounds.X.ToString());
						config.set("Y", RestoreBounds.Y.ToString());
					}
				}
				//MessageBox.Show("o " + originalSize + " " + this.WindowState + " l " + Location + " " + Width + " " + Height + " r " + RestoreBounds.X + " " + RestoreBounds.Y + " " + RestoreBounds.Width + " " + RestoreBounds.Height + " c " + config.get("X") + " " + config.get("Y"));

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			player.stopPlaying(true, true);
			try  {
				if (rec != null && rec.rfu != null && rec.rfu.h5r != null && rec.rfu.h5r.wsr != null) {
					var _r = rec.rfu.h5r.wsr;
					rec.stopRecording(rec.rfu.isPlayOnlyMode);
					for (var i = 0; i < 100; i++) {
//						util.debugWriteLine("close rec commentSW " + i);
						if (_r.commentSW == null) break;
						Thread.Sleep(100);
					}
					util.debugWriteLine("end close sw");

					/*
					_r.isRetry = false;
					_r.rec.isRetry = false;
					_r.rec.isEndProgram = true;
					_r.isEndProgram = true;
					
					Task.Run(() => {
						_r.stopRecording();
						if (_r.rec != null) _r.rec.waitForEnd();
						for (var i = 0; i < 1000 && rec.rfu != null; i++) {
							util.debugWriteLine("wait rec close " + i);
							Thread.Sleep(100);
						}
						util.debugWriteLine("wait rec close end");
					}).Wait();
					*/
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
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
			endTimeLabel.Text = "";
			descriptLabel.Text = "";
			typeLabel.Text = "";
			commentList.Rows.Clear();
			Text = "ニコ生新配信録画ツール（仮 " + util.versionStr;
			Icon = defIcon;
			recordStateLabel.Text = "";
		}
		public void setTitle(string s) {
			formAction(() => {
				try {
					Text = s;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
		       	}
			});
		}
		
		void PlayerBtnClick(object sender, EventArgs e)
		{
			player.play();
		}
		public void setPlayerBtnEnable(bool b) {
			formAction(() => {
				try {
					playerBtn.Enabled = b;
				} catch (Exception e) {
		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	}
			});
		}
		
		
		void mainForm_Load(object sender, EventArgs e)
		{
			formInitSetting();
			
			init();
			
			if (!util.isShowWindow) return;
			
			try {
				if (config.brokenCopyFile != null)
					addLogText("設定ファイルを読み込めませんでした。設定ファイルをバックアップしました。" + config.brokenCopyFile);
				
				var a = util.getJarPath();
				var desc = System.Diagnostics.FileVersionInfo.GetVersionInfo(util.getJarPath()[0] + "/websocket4net.dll");
				if (desc.FileDescription != "WebSocket4Net for .NET 4.5 gettable data bytes") {
					formAction(() => {
						System.Windows.Forms.MessageBox.Show("「WebSocket4Net.dll」をver0.86.9以降に同梱されているものと置き換えてください");
					});
				}
				
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
			
			try {
				//.net
				util.dotNetVer = util.Get45PlusFromRegistry();
				util.debugWriteLine(".net ver " + util.dotNetVer);
				if (util.dotNetVer < 4.52) {
					//Task.Run(() => {
					    //Invoke((MethodInvoker)delegate() {
							//var b = new DotNetMessageBox(ver);
							//b.Show(this); 
							System.Windows.Forms.MessageBox.Show("動作には.NET 4.5.2以上が推奨です。");
						//});
					//});
				} else {
					//dll
					var task = Task.Factory.StartNew(() => {
						util.dllCheck(this);            
			        });
				}
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		
		void versionMenu_Click(object sender, EventArgs e)
		{
			var v = new VersionForm(int.Parse(config.get("fontSize")), this);
			v.ShowDialog();
		}
		void startStdRead() {
			Task.Run(() => {
	         	while (true) {
					try {
						var a = Console.ReadLine();
						if (a == null || a.Length == 0) continue;
						if (a == "stop end") {
							if (rec.rfu != null) {
								rec.stopRecording(rec.rfu.isPlayOnlyMode);
							}
							while (rec.recordRunningList.Count > 0) {
								Thread.Sleep(1000);
							}
							Close();
						}
	         		} catch (Exception e) {
	         			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	         		}
				}
			});
		}
		void MainFormDragDrop(object sender, DragEventArgs e)
		{
			try {
				var url = e.Data.GetData(DataFormats.Text).ToString();
				if (urlText.Enabled) urlText.Text = url;
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
			}
		}
		void MainFormDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("UniformResourceLocator") ||
			    e.Data.GetDataPresent("UniformResourceLocatorW") ||
			    e.Data.GetDataPresent(DataFormats.Text)) {
				util.debugWriteLine(e.Effect);
				e.Effect = DragDropEffects.Copy;
				
			}
		}
		bool isFullAccessDirectory() {
			//return true;
			/*
			try {
				var sr = new StreamReader(util.getJarPath()[1] + ".exe");
				sr.Read();
				sr.Close();
				var sw = new StreamWriter(util.getJarPath()[1] + ".exea", false);
				
				return true;
			} catch(Exception e) {
				return false;
			}
			*/
			//Thread.Sleep(1000);
			//Thread.Sleep(10000);
			
			var cfg = config.getConfig();
			for (var i = 0; i < 1; i++) {
				try {
					cfg.AppSettings.Settings["browserNum2"].Value = (1 - int.Parse(cfg.AppSettings.Settings["browserNum2"].Value)).ToString();
					cfg.Save();
					return true;
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					if (e.Message.IndexOf("アクセスが拒否されました") > -1 ||
					   e.Message.IndexOf("オブジェクト参照が") > -1) return false;
				}
			}
			return true;
			
			/*
			var dir = util.getJarPath();
			var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
			var security = Directory.GetAccessControl(dir[0]).GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
			//var security = Directory.GetAccessControl(dir[0]).GetAccessRules(true, true, identity);
		    var principal = new System.Security.Principal.WindowsPrincipal(identity);
		    var isAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
		    util.debugWriteLine("isadmin " + isAdmin);
		    var ids = principal.Identities;
		    var _id = principal.Identity;
		    foreach (var id in ids) util.debugWriteLine("id " + id.Name);
			
				
			foreach(FileSystemAccessRule s in security) {
				util.debugWriteLine("s idenRef " + s.IdentityReference.ToString() + " " + s.FileSystemRights + " ");
				
				//if ((s.IdentityReference.ToString().IndexOf("AUTHORITY") > -1)) {
				if (s.IdentityReference.ToString() == _id.Name) {
					if ((s.FileSystemRights & FileSystemRights.CreateFiles) == 0 ||
					    (s.FileSystemRights & FileSystemRights.Delete) == 0 ||
					    (s.FileSystemRights & FileSystemRights.AppendData) == 0 ||
					    (s.FileSystemRights & FileSystemRights.CreateDirectories) == 0 ||
					    (s.FileSystemRights & FileSystemRights.WriteData) == 0)
							return false;
					//else return false;
					
				}
				
			}
			return true;
			*/
		}
		public void addLogTextDebug(string s) {
			//addLogText(s, true);
		}
		public bool formAction(Action a, bool isAsync = true) {
			if (IsDisposed || !util.isShowWindow) return false;
			
			if (Thread.CurrentThread == madeThread) {
				try {
					a.Invoke();
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					return false;
				}
			} else {
				try {
					var r = BeginInvoke((MethodInvoker)delegate() {
						try {    
				       		a.Invoke();
				       	} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						}
					});
					if (!isAsync) 
						EndInvoke(r);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					return false;
				} 
			}
			return true;
		}
		public void close() {
			formAction(() => {
	           	try {
	       			Close();
			    } catch (Exception e) {
   	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
   	       		}
        	});
		}
		void MiniBtnClick(object sender, EventArgs e)
		{
			if (originalSize == Size.Empty) {
				changeSize(true);
			} else {
				changeSize(false);
			}
		}
		private void changeSize(bool isMini) {
			try {
				urlLabel.Visible = !isMini;
				commentList.Visible = !isMini;
				logText.Visible = !isMini;
				streamStateGroupBox.Visible = !isMini;
				playerBtn.Visible = !isMini;
				label10.Visible = !isMini;
				descriptLabel.Visible = !isMini;
				label5.Visible = !isMini;
				label1.Visible = !isMini;
				label2.Visible = !isMini;
				label3.Visible = !isMini;
				label7.Visible = !isMini;
				label6.Visible = !isMini;
				label8.Visible = !isMini;
				typeLabel.Visible = !isMini;
				genteiLabel.Visible = !isMini;
				keikaTimeLabel.Height = isMini ? 13 : 25;
				titleLabel.Location = isMini ? label5.Location : new Point(78,label5.Location.Y);
				communityLabel.Location = isMini? label1.Location : new Point(78,label1.Location.Y);
				hostLabel.Location = isMini ? label2.Location : new Point(78,label2.Location.Y);
				genteiLabel.Location = isMini ? label3.Location : new Point(78,label3.Location.Y);
				typeLabel.Location = isMini ? label7.Location : new Point(78,label7.Location.Y);
				startTimeLabel.Location = isMini ? label3.Location : new Point(78,label6.Location.Y);
				keikaTimeLabel.Location = isMini ? label7.Location : new Point(78,label8.Location.Y);
				//groupBox5.Location = isMini ? new Point(160, 76) : new Point(179, 76);
				recordGroupBox.Location = isMini ? new Point(6, 143) : new Point(6, 217);
				
				titleLabel.Size = isMini ? new Size(streamInfoGroupBox.Width - 10, 23) : new Size(streamInfoGroupBox.Width - 93, 23);
				communityLabel.Size = isMini ? new Size(streamInfoGroupBox.Width - 10, 23) : new Size(streamInfoGroupBox.Width - 93, 23);
				hostLabel.Size = isMini ? new Size(streamInfoGroupBox.Width - 10, 23) : new Size(streamInfoGroupBox.Width - 93, 23);
				samuneBox.Size = isMini ? new Size(87, 76) : new Size(141, 150);
				
				var urlTextX = isMini ? 19 : 69;
				urlText.Location = new Point(urlTextX, urlText.Location.Y);
				recBtn.Location = new Point(urlText.Location.X + 245, recBtn.Location.Y);
				miniBtn.Location = new Point((isMini) ? (recBtn.Location.X + 83) : 698, miniBtn.Location.Y);
				var _size = Size;
				Size = isMini ? new Size(386, 236) : originalSize;
				streamInfoGroupBox.Size = isMini ? new Size(Width - 196, 121) : new Size(Width - 196, 180);
				originalSize = isMini ? _size : Size.Empty;
				miniBtn.Text = isMini ? "戻" : "小";
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
		public string getKeikaTime() {
			return keikaTimeLabel.Text;
		}
		public string getTitleLabelText() {
			return titleLabel.Text;
		}
		void updateMenu_Click(object sender, EventArgs e)
		{
			var v = new UpdateForm(int.Parse(config.get("fontSize")));
			v.ShowDialog();
		}
		void ColorMenuItemClick(object sender, EventArgs e)
		{
			var d = new ColorDialog();
			d.Color = BackColor;
			var r = d.ShowDialog();
			if (r == DialogResult.OK) {
				setBackColor(d.Color);
				config.set("recBackColor", d.Color.ToArgb().ToString());
			}
		}
		private void setBackColor(Color color) {
			BackColor = //commentList.BackgroundColor = 
				color;
		}
		void CharacterColorMenuItemClick(object sender, EventArgs e)
		{
			var d = new ColorDialog();
			d.Color = label1.ForeColor;
			var r = d.ShowDialog();
			if (r == DialogResult.OK) {
				setForeColor(d.Color);
				config.set("recForeColor", d.Color.ToArgb().ToString());
			}
		}
		private void setForeColor(Color color) {
			var c = getChildControls(this);
			foreach (var _c in c)
				if (_c.GetType() == typeof(GroupBox) ||
				    _c.GetType() == typeof(Label) ||
				   _c.GetType() == typeof(CheckBox)) _c.ForeColor = color;
		}
		void LinkColorMenuItemClick(object sender, EventArgs e)
		{
			var d = new ColorDialog();
			d.Color = titleLabel.LinkColor;
			var r = d.ShowDialog();
			if (r == DialogResult.OK) {
				setLinkColor(d.Color);
				config.set("recLinkColor", d.Color.ToArgb().ToString());
			}
		}
		private void setLinkColor(Color color) {
			var c = getChildControls(this);
			foreach (var _c in c)
				if (_c.GetType() == typeof(LinkLabel)) 
					((LinkLabel)_c).LinkColor = color;
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
		private void loadControlLayout() {
			try {
				
				Font = new Font(Font.FontFamily, 9);
				Controls.Clear();
				
				InitializeComponent();
				formInitSetting();
				
				Update();
				
				return;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
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
					MessageBox.Show("readme.htmlが見つかりませんでした");
					return;
				}
				System.Diagnostics.Process.Start(path);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		
		void OpenTourokuExeMenuClick(object sender, EventArgs e)
		{
			string[] jarpath = util.getJarPath();
			string path = jarpath[0] + "/録画登録ツール（仮.exe";
			try {
				if (!File.Exists(path)) {
					MessageBox.Show("録画登録ツール（仮.exeが見つかりませんでした");
					return;
				}
				System.Diagnostics.Process.Start(path);
			} catch (Exception ee) {
				util.debugWriteLine(ee.Message + " " + ee.StackTrace);
			}
		}
		void changeRecBtnClickEvent(bool isRecBtnOnlyMouse) {
			recBtn.Click -= new EventHandler(recBtnAction);
			recBtn.MouseClick -= new MouseEventHandler(recBtnAction);
			if (isRecBtnOnlyMouse) {
				recBtn.MouseClick += new MouseEventHandler(recBtnAction);
			} else {
				recBtn.Click += new EventHandler(recBtnAction);
			}
		}
		void RecEndMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem i in recEndMenuItem.DropDownItems)
				i.Checked = i == e.ClickedItem;
			var t = e.ClickedItem.Text;
			recEndProcess = t == "何もしない" ? null : t;
		}
	}
}
