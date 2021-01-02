/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/09/21
 * Time: 0:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using rokugaTouroku;
using rokugaTouroku.info;

namespace rokugaTouroku.rec
{
	/// <summary>
	/// Description of RecListManager.
	/// </summary>
	public class RecListManager
	{
		public MainForm form;
		public BindingSource recListData;
		public config.config cfg;
		public RecDataGetter rdg;
		 
		public RecListManager(MainForm form, BindingSource recListData, config.config cfg)
		{
			this.form = form;
			this.recListData = recListData;
			this.cfg = cfg;
		}
		public bool add(string t) {
			util.debugWriteLine("rlm add");
            
            var lvid = util.getRegGroup(t, "(lv\\d+(,\\d+)*)");
			//util.setLog(cfg, lv);
			
			var url = "";
			if (lvid != null) {
				url = "https://live2.nicovideo.jp/watch/" + lvid;
				
				try {
					if (bool.Parse(cfg.get("IsDuplicateConfirm"))) {
						var delList = new List<RecInfo>();
						foreach (RecInfo d in recListData)
							if (d.id == lvid) delList.Add(d);
						
						foreach (var _ri in delList) 
							if (MessageBox.Show(_ri.id + "はリスト内に含まれています。既にある行を削除しますか？\n[" + _ri.quality + "] [" + _ri.timeShift + "]", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes) {
								form.deleteRow(_ri);
						}
						
					}
				} catch (Exception e){
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			} else {
				MessageBox.Show("not found lvID");
				return false;
			}
			//if (lvid != null) form.urlText.Text = "https://cas.nicovideo.jp/user/77252622/lv313508832";
				
			var rdg = new RecDataGetter(this);
			var ri = new RecInfo(lvid, t, rdg, form.afterConvertModeList.Text, form.setTsConfig, form.setTimeshiftBtn.Text, form.qualityBtn.Text, form.qualityRank, form.recCommmentList.Text, form.isChaseChkBox.Checked);
			Task.Run(() => ri.setHosoInfo(form));
			
			form.addList(ri);
			
			return true;
				
		}
		public void record() {
			if (util.getRegGroup(form.urlText.Text, "(lv\\d+(,\\d+)*)") == null) {
				form.urlText.Text = "";
			} else {
				if (add(form.urlText.Text))
					form.urlText.Text = "";
			}
			
			if (rdg == null) {
				form.recBtn.Text = "録画停止";
				form.optionMenuItem.Enabled = false;
				Task.Run(() => {
					rdg = new RecDataGetter(this);
					rdg.rec();
					rdg = null;
					form.Invoke((MethodInvoker)delegate() {
		            	form.recBtn.Text = "録画開始";
		            	form.optionMenuItem.Enabled = true;
		            });
				});
			} else {
				rdg.stopRecording();
				rdg = null;
				form.recBtn.Text = "録画開始";
				form.optionMenuItem.Enabled = true;
			}
			
		}
		
	}
}
