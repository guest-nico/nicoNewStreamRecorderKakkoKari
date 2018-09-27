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
		public void add() {
			util.debugWriteLine("rlm add");
            
            var lvid = util.getRegGroup(form.urlText.Text, "(lv\\d+)");
			//util.setLog(cfg, lv);
			util.debugWriteLine("ver 0.1.0");
			
			if (lvid != null) {
				form.urlText.Text = "http://live2.nicovideo.jp/watch/" + lvid;
			}
//				if (lvid != null) form.urlText.Text = "https://cas.nicovideo.jp/user/77252622/lv313508832";
				
			else {
				MessageBox.Show("not found lvID");
				return;
			}
			
			//Task.Run(() => {
				var rdg = new RecDataGetter(this);
				var ri = new RecInfo(lvid, form.urlText.Text, rdg);
				form.addList(ri);
				
				form.urlText.Text = "";

		}
		public void record() {
			if (util.getRegGroup(form.urlText.Text, "(lv\\d+)") == null) {
				form.urlText.Text = "";
			} else {
				add();
			}
			
			if (rdg == null) {
				form.recBtn.Text = "録画停止";
				Task.Run(() => {
					rdg = new RecDataGetter(this);
					rdg.rec();
					rdg = null;
					form.Invoke((MethodInvoker)delegate() {
		            	form.recBtn.Text = "録画開始";
		            });
				});
			} else {
				rdg = null;
				form.recBtn.Text = "録画開始";
			}
			
		}
	}
}
