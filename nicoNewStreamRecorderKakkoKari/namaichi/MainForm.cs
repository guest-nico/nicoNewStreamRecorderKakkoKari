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
using namaichi.rec;
using namaichi.config;
using namaichi.play;

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
		
		public MainForm(string[] args)
		{
			System.Diagnostics.Debug.Listeners.Clear();
			System.Diagnostics.Debug.Listeners.Add(new log.TraceListener());
		    
			
			this.args = args;
			
			util.debugWriteLine(args.Length);
			util.debugWriteLine(args);
			
			//test
			if (bool.Parse(config.get("IsLogFile"))) {
				var name = (args.Length == 0) ? "lv_" : util.getRegGroup(args[0], "(lv\\d+)");
				var logPath = util.getJarPath()[0] + "/" + name + ".txt";
				
				try {
					#if DEBUG
						System.Diagnostics.DefaultTraceListener dtl
					      = (System.Diagnostics.DefaultTraceListener)System.Diagnostics.Debug.Listeners["Default"];
						dtl.LogFileName = logPath;
					#else
						FileStream fs = new FileStream(logPath, 
					    		FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
						var w = new System.IO.StreamWriter(fs);
						w.AutoFlush = true;
						System.Console.SetOut(w);
					#endif
				} catch (Exception e) {
					util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
				}
				util.isLogFile = true;
			}


		    
			
			//test
//			args = new string[]{};
			
			InitializeComponent();
			
			rec = new rec.RecordingManager(this, config);
			//player = new play.Player(rec);
			
            //nicoSessionComboBox1.Selector.PropertyChanged += Selector_PropertyChanged;
//            checkBoxShowAll.Checked = bool.Parse(config.get("isAllBrowserMode"));
			//if (isInitRun) initRec();
			Width = int.Parse(config.get("Width"));
			Height = int.Parse(config.get("Height"));
			
			if (args.Length > 0) {
				if (bool.Parse(config.get("Isminimized"))) {
					this.WindowState = FormWindowState.Minimized;
				}
            	urlText.Text = args[0];
//            	rec = new rec.RecordingManager(this);
            	rec.rec();

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
		public void setRecordState(String t) {
	       	try {
	       		if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       		    try {
		        	    recordStateLabel.Text = t;
		        	    if (bool.Parse(config.get("IstitlebarInfo"))) {
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
        		string gentei, string openTime, string description) {
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
			        	    hostLabel.Links.Add(0, (hostUrl != null) ? hostLabel.Text.Length : 0, hostUrl);
			        	    hostLabel.LinkArea = new LinkArea(0, (hostUrl == null) ? 0 : hostLabel.Text.Length);
			        	    communityLabel.Text = group;
			        	    communityLabel.Links.Add(0, groupLabel.Text.Length, groupUrl);
			        	    genteiLabel.Text = gentei;
			        	    startTimeLabel.Text = openTime;
			        	    descriptLabel.Text = description;
	       		       	} catch (Exception e) {
	       		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       		       	}
				});
	       	} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
		}
		public void setSamune(string url) {
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
       public void setKeikaJikan(string keikaJikan) {
       	try {
	       if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	              	try {
	       				keikaTimeLabel.Text = keikaJikan;
	              	} catch (Exception e) {
	              		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	              	}
				});
       	} catch (Exception e) {
       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
       	}
       }
       public void setStatistics(string visit, string comment) {
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
			util.showException(e);
		} 
       }
       public void addComment(string time, string comment) {
       	try {
		       	if (IsDisposed) return;
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
       	} catch (Exception e) {
       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
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
		
		void titleLabel_Click(object _sender, LinkLabelLinkClickedEventArgs e)
		{
			LinkLabel sender = (LinkLabel)_sender;
			if (sender.Links.Count > 0 && sender.Links[0].Length != 0) {
				string url = (string)sender.Links[0].LinkData;
				if (bool.Parse(config.get("IsdefaultBrowserPath"))) {
					System.Diagnostics.Process.Start(url);
				} else {
					var p = config.get("browserPath");
					System.Diagnostics.Process.Start(p, url);
				}
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
			if (rec.rfu != null) {
				DialogResult res = MessageBox.Show("録画中ですが終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
			}
			try{
				config.set("Width", Width.ToString());
				config.set("Height", Height.ToString());
			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
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
			descriptLabel.Text = "";
			commentList.Rows.Clear();
			Text = "ニコ生新配信録画ツール（仮";
			Icon = null;
		}
	}
}
