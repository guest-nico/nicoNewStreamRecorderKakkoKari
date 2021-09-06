/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/07
 * Time: 16:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of fileNameOptionForm.
	/// </summary>
	public partial class fileNameOptionForm : Form
	{
		public string ret;
		
		public fileNameOptionForm(string filenameformat, int fontSize)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			fileNameTypeText.Text = filenameformat;
			fileNameTypeLabel.Text = 
				util.getFileNameTypeSample(filenameformat);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			util.setFontSize(fontSize, this, false);
		}
		
		void fileNameTypeOkBtn_Click(object sender, EventArgs e)
		{
			if (fileNameTypeText.Text.IndexOf("{0}") < 0) {
				util.showMessageBoxCenterForm(this, "{0}は必ず入れてください", "注意", MessageBoxButtons.OK, MessageBoxIcon.None);
				return;
			}
			DialogResult = DialogResult.OK;
			ret = fileNameTypeText.Text;
			Close();
		}
		void fileNameTypeCancelBtn_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
		
		void fileNameTypeText_Changed(object sender, EventArgs e)
		{
			fileNameTypeLabel.Text = 
				util.getFileNameTypeSample(fileNameTypeText.Text);
		}
		
		void fileNameTypeDefaultBtn_Click(object sender, EventArgs e)
		{
			fileNameTypeText.Text = 
				"{Y}_{M}_{D}_{h}_{m}_{0}_{1}_{2}_{3}_{4}";
		}
		void fileNameTypeNitijiBtn_Click(object sender, EventArgs e)
		{
			fileNameTypeText.Text = 
				"{Y}年{M}月{D}日({W}){h}時{m}分{1}_{0}_";
		}
		void fileNameTypeSimpleBtn_Click(object sender, EventArgs e)
		{
			fileNameTypeText.Text = 
				"{0}_";
		}
		void fileNameTypeTitleBtn_Click(object sender, EventArgs e)
		{
			fileNameTypeText.Text = 
				"{1}_{0}_";
		}
	}
}
