/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/21
 * Time: 4:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of UrlBulkRegistForm.
	/// </summary>
	public partial class UrlBulkRegistForm : Form
	{
		public List<string> res = null;
		
		public UrlBulkRegistForm(int fontSize)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			util.setFontSize(fontSize, this, false);
		}
		
		void CancelBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void RegistBtnClick(object sender, EventArgs e)
		{
			var l = new List<string>();
			foreach (var s in registText.Text.Split('\n')) {
				var r = util.getRegGroup(s, "(lv\\d+(,\\d+)*)");
				if (r != null) l.Add(r);
			}
			res = l;
			Close();
		}
	}
}
