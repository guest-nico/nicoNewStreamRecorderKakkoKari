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
//using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
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
		public List<int> recordRunningList = new List<int>();
		public RecordFromUrl rfu;
		public bool isClickedRecBtn = false;
		public string hlsUrl = null;
		public Stream rtmpPipe = null;
		public IRecorderProcess wsr = null;
		static readonly Uri TargetUrl = new Uri("http://live.nicovideo.jp/");
		public config.config cfg;
		public string recordingUrl;
		public string communityNum;
//		public string commentWsUrl;
		public bool isJikken = false;
//		public JikkenRecorder jr = null;
//		public JikkenRecordProcess jrp = null;
		public RedistInfo ri = null;
		
		public bool isTitleBarInfo = false;
		public bool isPlayOnlyMode = false;
		
		public TimeShiftConfig argTsConfig;
		public utility.RegGetter regGetter = new namaichi.utility.RegGetter();
		
		public RecordingManager(MainForm form, config.config cfg)
		{
			this.form = form;
			this.cfg = cfg;
		}
		/*
		async Task<int> test(int a) {
			//util.debugWriteLine(a);
			//c += a;
			return a;
		}
		*/
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

				rfu = new RecordFromUrl(this);
				Task.Run(() => {
					try {
					    var _rfu = rfu;
					    util.debugWriteLine("rm rec 録画開始" + rfu);
					    
					    util.debugWriteLine(form);
					    util.debugWriteLine(form.urlText);
					    util.debugWriteLine(form.urlText.Text);
					    
					    var rfuCode = rfu.GetHashCode();
					    recordRunningList.Add(rfuCode);
					    //endcode 0-その他の理由 1-stop 2-最初に終了 3-始まった後に番組終了
	                	var endCode = rfu.rec(form.urlText.Text, lvid);
	                	util.debugWriteLine("endcode " + endCode);
	                	recordRunningList.Remove(rfuCode);
	                	
	                	if (rfu == _rfu) {
		                	isRecording = false;
							rfu = null;
							if (!form.IsDisposed && util.isShowWindow) {
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
						       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						       	}
							}
							
							util.debugWriteLine("end rec " + rfu);
							if (!isClickedRecBtn && endCode == 3) {
								Environment.ExitCode = 5;
								if (util.isShowWindow) {
			                		try {
										form.Invoke((MethodInvoker)delegate() {
											try {
								       			form.Close();
			                			    } catch (Exception e) {
						       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						       	       		}
										});
			                		} catch (Exception e) {
							       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
							       	}
		                		}
							}
							hlsUrl = null;
							
							recordingUrl = null;
	                	}
	                	if (bool.Parse(cfg.get("IscloseExit")) && endCode == 3) {
	                		rfu = null;
	                		Environment.ExitCode = 5;
	                		if (util.isShowWindow) {
		                		try {
									form.Invoke((MethodInvoker)delegate() {
										try {
							       			form.Close();
		                			    } catch (Exception e) {
					       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
					       	       		}
									});
		                		} catch (Exception e) {
						       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						       	}
	                		}
	                	}
	                	if (util.isStdIO && (endCode == 0 || endCode == 2 || endCode == 3)) {
	                		form.Invoke((MethodInvoker)delegate() {
								try {
					       			form.Close();
	            			    } catch (Exception e) {
			       	       			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       		}
							});
	                		
	                	}
	             	} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
						rfu = null;
						form.Invoke((MethodInvoker)delegate() {
							try {
				        	    form.recBtn.Text = "録画開始";
								form.urlText.Enabled = true;
								form.optionMenuItem.Enabled = true;
							} catch (Exception ee) {
	       	       				util.debugWriteLine(ee.Message + " " + ee.StackTrace + " " + ee.Source + " " + ee.TargetSite);
	       	       			}
						});
					}
				});
		         
			} else {
				stopRecording();
				
				
			}

		}
		public void setRedistInfo(string[] args) {
			ri = new RedistInfo(args);
		}
		public void stopRecording() {
			if (util.isShowWindow) {
            	try {
	            	form.Invoke((MethodInvoker)delegate() {
						try {
			        	    form.recBtn.Text = "録画開始";
							form.urlText.Enabled = true;
							form.optionMenuItem.Enabled = true;
							var _m = (isPlayOnlyMode) ? "視聴" : "録画";
							form.addLogText(_m + "を中断しました");
							
						} catch (Exception e) {
							util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
						}
					});
            	} catch (Exception e) {
		       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
		       	}
			}
			isRecording = false;
			rfu = null;
			hlsUrl = null;
			
        	recordingUrl = null;
		}
	}
}
