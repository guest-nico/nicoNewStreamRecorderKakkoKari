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
using namaichi.info;

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
		public string hlsUrl = null;
<<<<<<< HEAD
		public IRecorderProcess wsr = null;
=======
		public WebSocketRecorder wsr = null;
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		static readonly Uri TargetUrl = new Uri("http://live.nicovideo.jp/");
		public config.config cfg;
		public string recordingUrl;
		public string communityNum;
//		public string commentWsUrl;
<<<<<<< HEAD
		public bool isJikken = false;
//		public JikkenRecorder jr = null;
//		public JikkenRecordProcess jrp = null;
		public RedistInfo ri = null;
		
		public bool isTitleBarInfo = false;
		public bool isPlayOnlyMode = false;
=======
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0
		
		public RecordingManager(MainForm form, config.config cfg)
		{
			this.form = form;
			this.cfg = cfg;
		}

		async public void rec() {
            util.debugWriteLine("rm");
            

            
            var lv = util.getRegGroup(form.urlText.Text, "(lv\\d+)");
			util.setLog(cfg, lv);
			util.debugWriteLine(util.versionStr + " " + util.versionDayStr);
			
			if (rfu == null) {
            	var arr = form.urlText.Text.Split('|');
            	
            	try {
            		if (!arr[0].StartsWith("http") && System.IO.File.Exists(arr[0]) ||
	            	   		System.IO.Directory.Exists(arr[0])) {
            			Task.Run(() => new ArgConcat(this, arr).concat());
	            		return;
	            	}
            	} catch (Exception e) {
            		util.debugWriteLine(e.Message + " " + e.Source + " " + e.StackTrace + " " + e.TargetSite);
            	}
				

				var lvid = util.getRegGroup(form.urlText.Text, "(lv\\d+)", 1);
				if (lvid != null) {
					if (isPlayOnlyMode) {
						form.Invoke((MethodInvoker)delegate() {
							form.urlText.Text = "http://live2.nicovideo.jp/watch/" + lvid;
						});
					} else form.urlText.Text = "http://live2.nicovideo.jp/watch/" + lvid;
				}
//				if (lvid != null) form.urlText.Text = "https://cas.nicovideo.jp/user/77252622/lv313508832";
				
				else {
					if (isPlayOnlyMode) {
						form.Invoke((MethodInvoker)delegate() {
							MessageBox.Show("not found lvid");
						});
					} else MessageBox.Show("not found lvid");
					return;
				}
			
				isRecording = true;
<<<<<<< HEAD
				if (isPlayOnlyMode) {
					form.Invoke((MethodInvoker)delegate() {
						form.recBtn.Text = "中断";
						form.urlText.Enabled = false;
						form.optionMenuItem.Enabled = false;
					
						form.resetDisplay();
						recordingUrl = form.urlText.Text;
					});
				} else {
					form.recBtn.Text = "中断";
					form.urlText.Enabled = false;
					form.optionMenuItem.Enabled = false;
				
					form.resetDisplay();
					recordingUrl = form.urlText.Text;
				}
=======
				form.recBtn.Text = "中断";
				form.urlText.Enabled = false;
				form.optionMenuItem.Enabled = false;
				form.resetDisplay();
				recordingUrl = form.urlText.Text;
>>>>>>> 41df14c80172b3ccda9b7c5de41ef417f8572ea0

				rfu = new RecordFromUrl(this);
				Task.Run(() => {
				    
				    var _rfu = rfu;
				    util.debugWriteLine("rm rec 録画開始" + rfu);
				    
				    util.debugWriteLine(form);
				    util.debugWriteLine(form.urlText);
				    util.debugWriteLine(form.urlText.Text);
				    
				    //endcode 0-その他の理由 1-stop 2-最初に終了 3-始また後に番組終了
                	var endCode = rfu.rec(form.urlText.Text, lvid);
                	util.debugWriteLine("endcode " + endCode);
                	
                	if (rfu == _rfu) {
	                	isRecording = false;
						rfu = null;
						if (!form.IsDisposed) {
							try {
								form.Invoke((MethodInvoker)delegate() {
									try {
						        	    form.recBtn.Text = "録画開始";
										form.urlText.Enabled = true;
										form.optionMenuItem.Enabled = true;
									} catch (Exception e) {
			       	       				util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       			}
								});
							} catch (Exception e) {
					       		util.showException(e);
					       	}
						}
						
						util.debugWriteLine("end rec " + rfu);
						if (!isClickedRecBtn && endCode == 3) {
							Environment.ExitCode = 5;
							form.Close();
						}
						hlsUrl = null;
						
						recordingUrl = null;
                	}
                	if (bool.Parse(cfg.get("IscloseExit")) && endCode == 3) {
                		rfu = null;
                		try {
							form.Invoke((MethodInvoker)delegate() {
								try {
                			        Environment.ExitCode = 5;
					       			form.Close();
                			    } catch (Exception e) {
			       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       		}
							});
                		} catch (Exception e) {
				       		util.showException(e);
				       	}
                	}
				});
				
			} else {
            	try {
	            	form.Invoke((MethodInvoker)delegate() {
						try {
			        	    form.recBtn.Text = "録画開始";
							form.urlText.Enabled = true;
							form.optionMenuItem.Enabled = true;
							form.addLogText("録画を中断しました");
							
						} catch (Exception e) {
							util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						}
					});
            	} catch (Exception e) {
		       		util.showException(e);
		       	}
				isRecording = false;
				rfu = null;
				hlsUrl = null;
				
            	recordingUrl = null;
				
			}

		}
		public void setRedistInfo(string[] args) {
			ri = new RedistInfo(args);
		}
		
	}
}
