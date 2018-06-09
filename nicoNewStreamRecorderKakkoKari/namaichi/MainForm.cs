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
			
		    
    
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.args = args;
			
			System.Diagnostics.Debug.WriteLine(args.Length);
			System.Diagnostics.Debug.WriteLine(args);
			
			//test
			var name = (args.Length == 0) ? "aaa" : util.getRegGroup(args[0], "(lv\\d+)");
			System.Diagnostics.DefaultTraceListener dtl
		      = (System.Diagnostics.DefaultTraceListener)System.Diagnostics.Debug.Listeners["Default"];
			dtl.LogFileName = util.getJarPath()[0] + "/" + name + ".txt";

		    
			
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
			System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("{W}"));
			var si = nicoSessionComboBox1.Selector.SelectedImporter.SourceInfo;
			System.Diagnostics.Debug.WriteLine(si.EngineId + " " + si.BrowserName + " " + si.ProfileName);
//			var a = new SunokoLibrary.Application.Browsers.FirefoxImporterFactory();
//			foreach (var b in a.GetCookieImporters()) {
//				var c = b.GetCookiesAsync(TargetUrl);
//				c.ConfigureAwait(false);
				
//				System.Diagnostics.Debug.WriteLine(c.Result.Cookies["user_session"]);
//			}
			System.Diagnostics.Debug.WriteLine(nicoSessionComboBox1.Selector.SelectedImporter.SourceInfo.CookiePath);
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
        		System.Diagnostics.Debug.WriteLine(ee.Message + " " + ee.StackTrace);
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
       		if (IsDisposed) return;
       		
       		
        	Invoke((MethodInvoker)delegate() {
        	    string _t = "";
		    	if (logText.Text.Length != 0) _t += "\r\n";
		    	_t += t;
		    	
	    		logText.AppendText(_t);
				if (logText.Text.Length > 20000) 
					logText.Text = logText.Text.Substring(logText.TextLength - 10000);

			});
		}
		public void setRecordState(String t) {
       		if (IsDisposed) return;
        	Invoke((MethodInvoker)delegate() {
        	    recordStateLabel.Text = t;
        	    if (bool.Parse(config.get("IstitlebarInfo"))) {
        	    	Text = t;
        	    }
        	    //recordStateLabel.AutoSize
			});
		}
        private void initRec() {
        	//System.Diagnostics.Debug.WriteLine(int.Parse(config.get("browserName")));
        	//System.Diagnostics.Debug.WriteLine(bool.Parse(config.get("isAllBrowserMode")));
        	
        	//try {
        	//	nicoSessionComboBox1.SelectedIndex = int.Parse(config.get("browserNum"));
        	//} catch (Exception e) {System.Diagnostics.Debug.WriteLine(333); return;};
        	//var t = getCookie();
			//t.ConfigureAwait(false);
			//System.Diagnostics.Debug.WriteLine(t.Result);
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
       		if (IsDisposed) return;
        	Invoke((MethodInvoker)delegate() {
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
			});
		}
		public void setSamune(string url) {
       		if (IsDisposed) return;
       		WebClient cl = new WebClient();
       		cl.Proxy = null;
			byte[] pic = cl.DownloadData(url);
			System.Drawing.Icon icon =  null;
			try {
				var  st = new System.IO.MemoryStream(pic);
				icon = Icon.FromHandle(new System.Drawing.Bitmap(st).GetHicon());
				st.Close();
				
				
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(e.Message);
				}
			

        	Invoke((MethodInvoker)delegate() {
			    samuneBox.Image = icon.ToBitmap();
//       			samuneBox.ImageLocation = url;
				if (bool.Parse(config.get("IstitlebarSamune"))) {
        	    	this.Icon = icon;
				}
					
       			
//       			Icon = new System.Drawing.Icon(url);
			});
			
		}
       public void setKeikaJikan(string keikaJikan) {
	       if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       			keikaTimeLabel.Text = keikaJikan;
				});
       }
       public void setStatistics(string visit, string comment) {
	       	if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       			visitLabel.Text = visit;
	       			commentLabel.Text = comment;
				});
       }
       public void addComment(string time, string comment) {
	       	if (IsDisposed) return;
	        	Invoke((MethodInvoker)delegate() {
	       	       	var rows = new string[]{time, comment};
	       	       	commentList.Rows.Add(rows);
	       	       	commentList.FirstDisplayedScrollingRowIndex = commentList.Rows.Count - 1;
	       	       	while (commentList.Rows.Count > 20) {
	       	       		commentList.Rows.RemoveAt(0);
	       	       	}
	       	       	
				});
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
				System.Diagnostics.Debug.WriteLine(ee.Message + " " + ee.StackTrace);
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
			Close();
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
				System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
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
