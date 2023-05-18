/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2023/05/19
 * Time: 7:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace rokugaTouroku.gui
{
	/// <summary>
	/// Description of MfaInputForm.
	/// </summary>
	public partial class MfaInputForm : Form
	{
		public string code = null;
		public MfaInputForm(string sendTo)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			if (sendTo != null) {
				if (sendTo != "app")
					sendToLabel.Text = sendTo + sendToLabel.Text;
				else {
					sendToLabel.Text = "スマートフォンのアプリを使って確認コードを取得してください";
					label2.Text = "アプリに表示された6桁の数字を入力";
				}
			}
		}
		void OkBtnClick(object sender, EventArgs e)
		{
			if (codeText.Text.Length != 6) {
				MessageBox.Show("入力されたコードが6文字ではありません。");
				return;
			}
			if (util.getRegGroup(codeText.Text, "(\\D)") != null) {
				MessageBox.Show("入力されたコードに数字以外の文字が含まれています。");
				return;
			}
			code = codeText.Text;
			Close();
		}
		void CancelBtnClick(object sender, EventArgs e)
		{
			Close();
		}
	}
}
