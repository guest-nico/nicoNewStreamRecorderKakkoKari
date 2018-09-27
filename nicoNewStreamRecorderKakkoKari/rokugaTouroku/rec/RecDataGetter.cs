/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/09/21
 * Time: 0:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

using rokugaTouroku;
using rokugaTouroku.info;

namespace rokugaTouroku.rec
{
	/// <summary>
	/// Description of RecDataGetter.
	/// </summary>
	public class RecDataGetter
	{
		private RecListManager rlm;
		
		//public bool isStop = false;
		
		public RecDataGetter(RecListManager rlm)
		{
			this.rlm = rlm;
		}
		public void rec() {
			while (true) {
				try {
					for (var i = 0; i < rlm.recListData.Count; i++) {
						RecInfo ri = (RecInfo)rlm.recListData[i];
						if (ri == null ||ri.state != "待機中") continue;
						ri.state = "a";
						((RecInfo)rlm.recListData[i]).State = "s";
					}
				} catch (Exception e) {
					util.debugWriteLine("rdg rec exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
				
				rlm.form.Invoke((MethodInvoker)delegate() {
					//rlm.form.recList.ResetBindings();
					rlm.recListData.ResetBindings(false);
				            });
				break;
				Thread.Sleep(3000);
			}
		}
	}
}
