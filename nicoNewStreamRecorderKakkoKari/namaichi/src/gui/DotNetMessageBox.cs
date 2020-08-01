/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/16
 * Time: 13:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace namaichi
{
	/// <summary>
	/// Description of DotNetMessageBox.
	/// </summary>
	public partial class DotNetMessageBox : Form
	{
		public DotNetMessageBox(double ver, int fontSize)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
//			label2.Text += ver + "です。";
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			util.setFontSize(fontSize, this, false);
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
