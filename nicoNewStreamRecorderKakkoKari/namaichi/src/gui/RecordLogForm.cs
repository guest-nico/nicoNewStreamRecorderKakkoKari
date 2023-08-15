/*
 * Created by SharpDevelop.
 * User: ajkkh
 * Date: 2023/07/30
 * Time: 8:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace namaichi.gui
{
	/// <summary>
	/// Description of RecordLogForm.
	/// </summary>
	public partial class RecordLogForm : Form
	{
		public RecordLogForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			textBox1.Text = RecordLogInfo.getText();
			textBox2.Text = RecordLogInfo.getFileText();
		}
		
	}
}
