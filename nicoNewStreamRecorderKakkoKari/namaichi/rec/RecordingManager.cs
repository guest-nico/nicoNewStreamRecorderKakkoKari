/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/11
 * Time: 0:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using namaichi;
using Newtonsoft.Json;
using System.Configuration;
using SunokoLibrary.Application;
using SunokoLibrary.Application.Browsers;

namespace namaichi.rec
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class RecordingManager
	{
		public MainForm form;
		public bool isRecording = false;
		public RecordFromUrl rfu;
		public bool isClickedRecBtn = false;
		public string hlsUrl;
		static readonly Uri TargetUrl = new Uri("http://live.nicovideo.jp/");
		public config.config cfg;
		
		public RecordingManager(MainForm form, config.config cfg)
		{
			this.form = form;
			this.cfg = cfg;
		}

		async public void rec() {
			
            util.debugWriteLine("rm");
            //config.Save();
            
//            int a = 0; a = a / a;
//            try {
//            	a = 0; a = a / a;
//            } catch (Exception e) {util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.StackTrace);}
            
            //Properties.Settings.Default = new namaichi.Properties.Settings(
			if (rfu == null) {
//				var c = await getCookie();
//				var d = c.Result;
				
            	/*
				if (c == null) {
					MessageBox.Show("not login");
					return;
				}
//				var cookie = c;
				//MessageBox.Show(""+c.IsFaulted+c.IsCanceled+c.IsCompleted);
				*/
				var lvid = util.getRegGroup(form.urlText.Text, "(lv\\d+)", 1);
				if (lvid != null) form.urlText.Text = "http://live2.nicovideo.jp/watch/" + lvid;
//				if (lvid != null) form.urlText.Text = "https://cas.nicovideo.jp/user/77252622/lv313508832";
				
				else {
					MessageBox.Show("not found lvid");
					return;
				}
			
				isRecording = true;
				form.recBtn.Text = "中断";
				form.urlText.Enabled = false;
				form.optionMenuItem.Enabled = false;
				form.resetDisplay();
				
				Task.Run(() => {
				    rfu = new RecordFromUrl(this);
				    var _rfu = rfu;
				    util.debugWriteLine("rm rec");
				    
				    //endcode 0-その他の理由 1-stop 2-最初に終了 3-始また後に番組終了
                	var endCode = rfu.rec(form.urlText.Text, lvid);
                	util.debugWriteLine("endcode " + endCode);
                	
                	if (rfu == _rfu) {
	                	isRecording = false;
						rfu = null;
						if (!form.IsDisposed) {
							form.Invoke((MethodInvoker)delegate() {
				        	    form.recBtn.Text = "録画開始";
								form.urlText.Enabled = true;
								form.optionMenuItem.Enabled = true;
							});
						}
						
						util.debugWriteLine("end rec " + rfu);
						if (!isClickedRecBtn && endCode == 3) form.Close();
						hlsUrl = null;
                	}
                	if (bool.Parse(cfg.get("IscloseExit")) && endCode == 3) {
                		rfu = null;
						form.Invoke((MethodInvoker)delegate() {
				       		form.Close();
						});
                	}
				});
				
			} else {
            	form.Invoke((MethodInvoker)delegate() {
	        	    form.recBtn.Text = "録画開始";
					form.urlText.Enabled = true;
					form.optionMenuItem.Enabled = true;
					form.addLogText("録画を中断しました");
				});
				isRecording = false;
				rfu = null;
				hlsUrl = null;
            	
				
			}

		}
		
		
	}
	/*
	[System.Runtime.Serialization.DataContract]
	class chat {
		
		public int id = 3;
		public string name = "aa";
		public chat (int a) {id= a;}
		public void setExit() {
			Application.ApplicationExit += new EventHandler(h);
		}
		public void minus(){
			Application.ApplicationExit -= new EventHandler(h);
		}
		private void h(object se,EventArgs e){
			util.debugWriteLine(id);
			System.Threading.Thread.Sleep(3000);
		}
	}
	*/
}
