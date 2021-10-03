/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2021/09/30
 * Time: 1:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku.gui
{
	partial class accountForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cookieFileText = new System.Windows.Forms.TextBox();
			this.useAccountLoginRadioBtn = new System.Windows.Forms.RadioButton();
			this.useCookieRadioBtn = new System.Windows.Forms.RadioButton();
			this.passText = new System.Windows.Forms.TextBox();
			this.mailText = new System.Windows.Forms.TextBox();
			this.nicoSessionComboBox1 = new rokugaTouroku.NicoSessionComboBox2();
			this.isCookieFileSiteiChkBox = new System.Windows.Forms.CheckBox();
			this.checkBoxShowAll = new System.Windows.Forms.CheckBox();
			this.loginBtn = new System.Windows.Forms.Button();
			this.cookieFileSanshouBtn = new System.Windows.Forms.Button();
			this.btnReload = new System.Windows.Forms.Button();
			this.useSecondLoginChkBox = new System.Windows.Forms.CheckBox();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(310, 308);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(74, 23);
			this.cancelBtn.TabIndex = 8;
			this.cancelBtn.Text = "キャンセル";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(230, 308);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(74, 23);
			this.okBtn.TabIndex = 7;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtnClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.cookieFileText);
			this.groupBox3.Controls.Add(this.useAccountLoginRadioBtn);
			this.groupBox3.Controls.Add(this.useCookieRadioBtn);
			this.groupBox3.Controls.Add(this.passText);
			this.groupBox3.Controls.Add(this.mailText);
			this.groupBox3.Controls.Add(this.nicoSessionComboBox1);
			this.groupBox3.Controls.Add(this.isCookieFileSiteiChkBox);
			this.groupBox3.Controls.Add(this.checkBoxShowAll);
			this.groupBox3.Controls.Add(this.loginBtn);
			this.groupBox3.Controls.Add(this.cookieFileSanshouBtn);
			this.groupBox3.Controls.Add(this.btnReload);
			this.groupBox3.Controls.Add(this.useSecondLoginChkBox);
			this.groupBox3.Location = new System.Drawing.Point(5, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(394, 275);
			this.groupBox3.TabIndex = 19;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "ニコニコ動画アカウントの共有　(普段ニコニコ生放送を見ているブラウザ)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 232);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 19);
			this.label2.TabIndex = 20;
			this.label2.Text = "パスワード：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 204);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 19);
			this.label1.TabIndex = 20;
			this.label1.Text = "メールアドレス：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cookieFileText
			// 
			this.cookieFileText.Location = new System.Drawing.Point(20, 115);
			this.cookieFileText.Name = "cookieFileText";
			this.cookieFileText.Size = new System.Drawing.Size(297, 19);
			this.cookieFileText.TabIndex = 19;
			// 
			// useAccountLoginRadioBtn
			// 
			this.useAccountLoginRadioBtn.Checked = true;
			this.useAccountLoginRadioBtn.Location = new System.Drawing.Point(6, 183);
			this.useAccountLoginRadioBtn.Name = "useAccountLoginRadioBtn";
			this.useAccountLoginRadioBtn.Size = new System.Drawing.Size(311, 18);
			this.useAccountLoginRadioBtn.TabIndex = 18;
			this.useAccountLoginRadioBtn.TabStop = true;
			this.useAccountLoginRadioBtn.Text = "ブラウザとクッキーを共有せず、次のアカウントでログインする";
			this.useAccountLoginRadioBtn.UseVisualStyleBackColor = true;
			// 
			// useCookieRadioBtn
			// 
			this.useCookieRadioBtn.Checked = true;
			this.useCookieRadioBtn.Location = new System.Drawing.Point(6, 18);
			this.useCookieRadioBtn.Name = "useCookieRadioBtn";
			this.useCookieRadioBtn.Size = new System.Drawing.Size(189, 18);
			this.useCookieRadioBtn.TabIndex = 18;
			this.useCookieRadioBtn.TabStop = true;
			this.useCookieRadioBtn.Text = "次のブラウザとクッキーを共有する";
			this.useCookieRadioBtn.UseVisualStyleBackColor = true;
			// 
			// passText
			// 
			this.passText.Location = new System.Drawing.Point(95, 232);
			this.passText.Margin = new System.Windows.Forms.Padding(2);
			this.passText.Name = "passText";
			this.passText.Size = new System.Drawing.Size(193, 19);
			this.passText.TabIndex = 13;
			// 
			// mailText
			// 
			this.mailText.Location = new System.Drawing.Point(95, 203);
			this.mailText.Margin = new System.Windows.Forms.Padding(2);
			this.mailText.Name = "mailText";
			this.mailText.Size = new System.Drawing.Size(193, 19);
			this.mailText.TabIndex = 12;
			// 
			// nicoSessionComboBox1
			// 
			this.nicoSessionComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.nicoSessionComboBox1.Location = new System.Drawing.Point(20, 61);
			this.nicoSessionComboBox1.Margin = new System.Windows.Forms.Padding(2);
			this.nicoSessionComboBox1.Name = "nicoSessionComboBox1";
			this.nicoSessionComboBox1.Size = new System.Drawing.Size(297, 20);
			this.nicoSessionComboBox1.TabIndex = 15;
			// 
			// isCookieFileSiteiChkBox
			// 
			this.isCookieFileSiteiChkBox.AutoSize = true;
			this.isCookieFileSiteiChkBox.Location = new System.Drawing.Point(20, 94);
			this.isCookieFileSiteiChkBox.Margin = new System.Windows.Forms.Padding(2);
			this.isCookieFileSiteiChkBox.Name = "isCookieFileSiteiChkBox";
			this.isCookieFileSiteiChkBox.Size = new System.Drawing.Size(194, 16);
			this.isCookieFileSiteiChkBox.TabIndex = 16;
			this.isCookieFileSiteiChkBox.Text = "さらにクッキーファイルを直接指定する";
			this.isCookieFileSiteiChkBox.UseVisualStyleBackColor = true;
			// 
			// checkBoxShowAll
			// 
			this.checkBoxShowAll.AutoSize = true;
			this.checkBoxShowAll.Location = new System.Drawing.Point(20, 41);
			this.checkBoxShowAll.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxShowAll.Name = "checkBoxShowAll";
			this.checkBoxShowAll.Size = new System.Drawing.Size(151, 16);
			this.checkBoxShowAll.TabIndex = 16;
			this.checkBoxShowAll.Text = "すべてのブラウザを表示する";
			this.checkBoxShowAll.UseVisualStyleBackColor = true;
			// 
			// loginBtn
			// 
			this.loginBtn.Location = new System.Drawing.Point(302, 230);
			this.loginBtn.Margin = new System.Windows.Forms.Padding(2);
			this.loginBtn.Name = "loginBtn";
			this.loginBtn.Size = new System.Drawing.Size(69, 23);
			this.loginBtn.TabIndex = 17;
			this.loginBtn.Text = "ログインする";
			this.loginBtn.UseVisualStyleBackColor = true;
			this.loginBtn.Click += new System.EventHandler(this.LoginBtnClick);
			// 
			// cookieFileSanshouBtn
			// 
			this.cookieFileSanshouBtn.Location = new System.Drawing.Point(322, 113);
			this.cookieFileSanshouBtn.Margin = new System.Windows.Forms.Padding(2);
			this.cookieFileSanshouBtn.Name = "cookieFileSanshouBtn";
			this.cookieFileSanshouBtn.Size = new System.Drawing.Size(40, 23);
			this.cookieFileSanshouBtn.TabIndex = 17;
			this.cookieFileSanshouBtn.Text = "参照";
			this.cookieFileSanshouBtn.UseVisualStyleBackColor = true;
			// 
			// btnReload
			// 
			this.btnReload.Location = new System.Drawing.Point(322, 59);
			this.btnReload.Margin = new System.Windows.Forms.Padding(2);
			this.btnReload.Name = "btnReload";
			this.btnReload.Size = new System.Drawing.Size(40, 23);
			this.btnReload.TabIndex = 17;
			this.btnReload.Text = "更新";
			this.btnReload.UseVisualStyleBackColor = true;
			// 
			// useSecondLoginChkBox
			// 
			this.useSecondLoginChkBox.Location = new System.Drawing.Point(20, 139);
			this.useSecondLoginChkBox.Margin = new System.Windows.Forms.Padding(2);
			this.useSecondLoginChkBox.Name = "useSecondLoginChkBox";
			this.useSecondLoginChkBox.Size = new System.Drawing.Size(368, 36);
			this.useSecondLoginChkBox.TabIndex = 20;
			this.useSecondLoginChkBox.Text = "ブラウザからクッキーが取得できなかった場合、次のアカウントでログインする";
			this.useSecondLoginChkBox.UseVisualStyleBackColor = true;
			// 
			// accountForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(404, 340);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Name = "accountForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "アカウント設定";
			this.Load += new System.EventHandler(this.AccountFormLoad);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox useSecondLoginChkBox;
		private System.Windows.Forms.Button btnReload;
		private System.Windows.Forms.Button cookieFileSanshouBtn;
		private System.Windows.Forms.Button loginBtn;
		private System.Windows.Forms.CheckBox checkBoxShowAll;
		private System.Windows.Forms.CheckBox isCookieFileSiteiChkBox;
		private rokugaTouroku.NicoSessionComboBox2 nicoSessionComboBox1;
		private System.Windows.Forms.TextBox mailText;
		private System.Windows.Forms.TextBox passText;
		private System.Windows.Forms.RadioButton useCookieRadioBtn;
		private System.Windows.Forms.RadioButton useAccountLoginRadioBtn;
		private System.Windows.Forms.TextBox cookieFileText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
	}
}
