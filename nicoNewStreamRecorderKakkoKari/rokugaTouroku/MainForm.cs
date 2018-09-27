/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/10
 * Time: 16:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using rokugaTouroku.info;
using rokugaTouroku.rec;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private rokugaTouroku.config.config config = new rokugaTouroku.config.config();
		private BindingSource recListDataSource = new BindingSource();
		private RecListManager rec;
		public MainForm(string[] args)
		{
			System.Diagnostics.Debug.Listeners.Clear();
			System.Diagnostics.Debug.Listeners.Add(new log.TraceListener());
		    util.setLog(config, null);
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			try {
				Width = int.Parse(config.get("rokugaTourokuWidth"));
				Height = int.Parse(config.get("rokugaTourokuHeight"));
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			rec = new RecListManager(this, recListDataSource, config);
			recList.DataSource = recListDataSource;
			
		}
		void optionItem_Select(object sender, EventArgs e)
        { 
        	try {
	        	optionForm o = new optionForm(config); o.ShowDialog();
	        } catch (Exception ee) {
        		util.debugWriteLine(ee.Message + " " + ee.StackTrace);
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
			/*
			if (rec.rfu != null) {
				DialogResult res = MessageBox.Show("録画中ですが終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No) return false;
			}
			*/
			try{
				util.debugWriteLine("rokugaTourokuWidth " + Width.ToString() + " rokugaTourokuHeight " + Height.ToString() + " restore rokugaTourokuWidth " + RestoreBounds.Width.ToString() + " restore rokugaTourokuWidth " + RestoreBounds.Height.ToString());
				if (this.WindowState == FormWindowState.Normal) {
					config.set("rokugaTourokuWidth", Width.ToString());
					config.set("rokugaTourokuHeight", Height.ToString());
				} else {
					config.set("rokugaTourokuWidth", RestoreBounds.Width.ToString());
					config.set("rokugaTourokuHeight", RestoreBounds.Height.ToString());
				}

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			//player.stopPlaying(true, true);
			return true;
		}
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
		void addListBtn_Click(object sender, EventArgs e)
		{
			rec.add();
			
			//var b = new info.TimeShiftConfig(0,1,2,3,true);
			//var g = new RecInfo("id", "title", "host", "comname", "start", "end", "proTime", "url", "comurl", "des", "0,1,2", b);
			
		}
		public void addList(RecInfo ri) {
			Invoke((MethodInvoker)delegate() {
				recListDataSource.Add(ri);
			});
		}
		void recBtn_Click(object sender, EventArgs e)
		{
			rec.record();
		}
	}
}
