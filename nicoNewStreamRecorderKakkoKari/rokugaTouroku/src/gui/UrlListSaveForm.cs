/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/12/21
 * Time: 5:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of UrlListSaveForm.
	/// </summary>
	public partial class UrlListSaveForm : Form
	{
		public UrlListSaveForm(string t, int fontSize)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			urlListText.Text = t;
			urlListText.SelectionStart = urlListText.Text.Length;
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			util.setFontSize(fontSize, this, false);
		}
		
		void CancelBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void SaveBtnClick(object sender, EventArgs e)
		{
			var f = new SaveFileDialog();
			f.DefaultExt = ".txt";
			f.FileName = "URLlist";
			f.Filter = "TEXT形式(*.txt)|*.txt*";
			var a = f.ShowDialog();
			if (a == DialogResult.OK) {
				var sw = new StreamWriter(f.FileName, false);
				sw.Write(urlListText.Text);
				sw.Close();
			}
		}
	}
}
