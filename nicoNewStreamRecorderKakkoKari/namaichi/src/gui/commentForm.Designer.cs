/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/21
 * Time: 4:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace namaichi
{
	partial class commentForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(commentForm));
			this.commentMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.secretCommentSendBtn = new System.Windows.Forms.Button();
			this.is184ChkBox = new System.Windows.Forms.CheckBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.visitImage = new System.Windows.Forms.PictureBox();
			this.visitCommentBar = new System.Windows.Forms.Panel();
			this.visitLabel = new System.Windows.Forms.Label();
			this.commentLabel = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.communityLabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.commentButtonPanel = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.commentList = new System.Windows.Forms.DataGridView();
			this.時間 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.color = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.score = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.commentBorderPanel = new System.Windows.Forms.Panel();
			this.commentPanel = new System.Windows.Forms.Panel();
			this.commentText = new namaichi.placeTextBox5();
			this.commentPlaceHolderText = new namaichi.placeHolderText();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.visitImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.commentList)).BeginInit();
			this.commentBorderPanel.SuspendLayout();
			this.commentPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// commentMenuStrip
			// 
			this.commentMenuStrip.Name = "commentMenuStrip";
			this.commentMenuStrip.Size = new System.Drawing.Size(61, 4);
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label4.Location = new System.Drawing.Point(14, 7);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 19);
			this.label4.TabIndex = 0;
			this.label4.Text = "コメント";
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
			this.panel3.Controls.Add(this.secretCommentSendBtn);
			this.panel3.Controls.Add(this.is184ChkBox);
			this.panel3.Controls.Add(this.label4);
			this.panel3.Location = new System.Drawing.Point(0, 49);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(485, 30);
			this.panel3.TabIndex = 24;
			// 
			// secretCommentSendBtn
			// 
			this.secretCommentSendBtn.Location = new System.Drawing.Point(-100, 5);
			this.secretCommentSendBtn.Name = "secretCommentSendBtn";
			this.secretCommentSendBtn.Size = new System.Drawing.Size(55, 19);
			this.secretCommentSendBtn.TabIndex = 2;
			this.secretCommentSendBtn.Text = "send";
			this.secretCommentSendBtn.UseVisualStyleBackColor = true;
			this.secretCommentSendBtn.Click += new System.EventHandler(this.secretCommentSendBtn_Click);
			// 
			// is184ChkBox
			// 
			this.is184ChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.is184ChkBox.Checked = true;
			this.is184ChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.is184ChkBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.is184ChkBox.Location = new System.Drawing.Point(434, 4);
			this.is184ChkBox.Name = "is184ChkBox";
			this.is184ChkBox.Size = new System.Drawing.Size(48, 21);
			this.is184ChkBox.TabIndex = 1;
			this.is184ChkBox.Text = "184";
			this.is184ChkBox.UseVisualStyleBackColor = true;
			// 
			// panel4
			// 
			this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.panel4.BackColor = System.Drawing.Color.White;
			this.panel4.Controls.Add(this.visitImage);
			this.panel4.Controls.Add(this.visitCommentBar);
			this.panel4.Controls.Add(this.visitLabel);
			this.panel4.Controls.Add(this.commentLabel);
			this.panel4.Controls.Add(this.pictureBox2);
			this.panel4.Location = new System.Drawing.Point(124, 21);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(238, 26);
			this.panel4.TabIndex = 5;
			// 
			// visitImage
			// 
			this.visitImage.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.visitImage.Image = ((System.Drawing.Image)(resources.GetObject("visitImage.Image")));
			this.visitImage.InitialImage = ((System.Drawing.Image)(resources.GetObject("visitImage.InitialImage")));
			this.visitImage.Location = new System.Drawing.Point(72, 7);
			this.visitImage.Name = "visitImage";
			this.visitImage.Size = new System.Drawing.Size(15, 15);
			this.visitImage.TabIndex = 2;
			this.visitImage.TabStop = false;
			// 
			// visitCommentBar
			// 
			this.visitCommentBar.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.visitCommentBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
			this.visitCommentBar.Location = new System.Drawing.Point(69, 3);
			this.visitCommentBar.Name = "visitCommentBar";
			this.visitCommentBar.Size = new System.Drawing.Size(100, 1);
			this.visitCommentBar.TabIndex = 1;
			// 
			// visitLabel
			// 
			this.visitLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.visitLabel.AutoSize = true;
			this.visitLabel.BackColor = System.Drawing.Color.White;
			this.visitLabel.Location = new System.Drawing.Point(105, 8);
			this.visitLabel.Name = "visitLabel";
			this.visitLabel.Size = new System.Drawing.Size(0, 12);
			this.visitLabel.TabIndex = 3;
			this.visitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.visitLabel.SizeChanged += new System.EventHandler(this.visitLabel_sizeChanged);
			// 
			// commentLabel
			// 
			this.commentLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.commentLabel.AutoSize = true;
			this.commentLabel.Location = new System.Drawing.Point(148, 8);
			this.commentLabel.Name = "commentLabel";
			this.commentLabel.Size = new System.Drawing.Size(0, 12);
			this.commentLabel.TabIndex = 4;
			this.commentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.commentLabel.SizeChanged += new System.EventHandler(this.commentLabel_sizeChanged);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.InitialImage")));
			this.pictureBox2.Location = new System.Drawing.Point(130, 7);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(15, 15);
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			// 
			// communityLabel
			// 
			this.communityLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.communityLabel.BackColor = System.Drawing.Color.White;
			this.communityLabel.Location = new System.Drawing.Point(205, 5);
			this.communityLabel.Name = "communityLabel";
			this.communityLabel.Size = new System.Drawing.Size(76, 15);
			this.communityLabel.TabIndex = 0;
			this.communityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.panel4);
			this.panel1.Controls.Add(this.communityLabel);
			this.panel1.Location = new System.Drawing.Point(0, 1);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(485, 47);
			this.panel1.TabIndex = 23;
			// 
			// commentButtonPanel
			// 
			this.commentButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.commentButtonPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.commentButtonPanel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.commentButtonPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.commentButtonPanel.Location = new System.Drawing.Point(386, 453);
			this.commentButtonPanel.Name = "commentButtonPanel";
			this.commentButtonPanel.Size = new System.Drawing.Size(100, 32);
			this.commentButtonPanel.TabIndex = 26;
			this.commentButtonPanel.Text = "コメント";
			this.commentButtonPanel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.commentButtonPanel.Click += new System.EventHandler(this.commentSendButtonPane_Clickl);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.panel2.Controls.Add(this.commentList);
			this.panel2.Location = new System.Drawing.Point(0, 79);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(485, 373);
			this.panel2.TabIndex = 27;
			// 
			// commentList
			// 
			this.commentList.AllowUserToAddRows = false;
			this.commentList.AllowUserToDeleteRows = false;
			this.commentList.AllowUserToResizeRows = false;
			this.commentList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.commentList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.commentList.BackgroundColor = System.Drawing.Color.White;
			this.commentList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.commentList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.commentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.commentList.ColumnHeadersVisible = false;
			this.commentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.時間,
									this.comment,
									this.color,
									this.id,
									this.score});
			this.commentList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.commentList.Location = new System.Drawing.Point(0, 1);
			this.commentList.Name = "commentList";
			this.commentList.RowHeadersVisible = false;
			this.commentList.RowTemplate.Height = 21;
			this.commentList.Size = new System.Drawing.Size(484, 372);
			this.commentList.TabIndex = 0;
			this.commentList.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.commentList_cellClick);
			this.commentList.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.commentList_RowPrePaint);
			// 
			// 時間
			// 
			this.時間.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.時間.FillWeight = 10F;
			this.時間.HeaderText = "時間";
			this.時間.Name = "時間";
			this.時間.Width = 50;
			// 
			// comment
			// 
			this.comment.HeaderText = "comment";
			this.comment.Name = "comment";
			// 
			// color
			// 
			this.color.HeaderText = "color";
			this.color.Name = "color";
			this.color.Visible = false;
			// 
			// id
			// 
			this.id.FillWeight = 15F;
			this.id.HeaderText = "id";
			this.id.Name = "id";
			// 
			// score
			// 
			this.score.FillWeight = 15F;
			this.score.HeaderText = "score";
			this.score.Name = "score";
			// 
			// commentBorderPanel
			// 
			this.commentBorderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.commentBorderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.commentBorderPanel.Controls.Add(this.commentPanel);
			this.commentBorderPanel.Controls.Add(this.textBox1);
			this.commentBorderPanel.Location = new System.Drawing.Point(0, 453);
			this.commentBorderPanel.Name = "commentBorderPanel";
			this.commentBorderPanel.Size = new System.Drawing.Size(385, 32);
			this.commentBorderPanel.TabIndex = 29;
			// 
			// commentPanel
			// 
			this.commentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.commentPanel.BackColor = System.Drawing.Color.White;
			this.commentPanel.Controls.Add(this.commentText);
			this.commentPanel.Controls.Add(this.commentPlaceHolderText);
			this.commentPanel.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.commentPanel.Location = new System.Drawing.Point(1, 1);
			this.commentPanel.Name = "commentPanel";
			this.commentPanel.Size = new System.Drawing.Size(394, 30);
			this.commentPanel.TabIndex = 8;
			this.commentPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.commentPanel_MouseDown);
			// 
			// commentText
			// 
			this.commentText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.commentText.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.commentText.Location = new System.Drawing.Point(17, 8);
			this.commentText.Name = "commentText";
			this.commentText.PlaceHolderText = this.commentPlaceHolderText;
			this.commentText.Size = new System.Drawing.Size(1, 16);
			this.commentText.TabIndex = 15;
			// 
			// commentPlaceHolderText
			// 
			this.commentPlaceHolderText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.commentPlaceHolderText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.commentPlaceHolderText.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.commentPlaceHolderText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(118)))), ((int)(((byte)(118)))), ((int)(((byte)(118)))));
			this.commentPlaceHolderText.Location = new System.Drawing.Point(17, 8);
			this.commentPlaceHolderText.Name = "commentPlaceHolderText";
			this.commentPlaceHolderText.Size = new System.Drawing.Size(371, 16);
			this.commentPlaceHolderText.TabIndex = 16;
			this.commentPlaceHolderText.Text = "コメント（75文字以内）";
			this.commentPlaceHolderText.Enter += new System.EventHandler(this.guide_focusget);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(25, 7);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(320, 19);
			this.textBox1.TabIndex = 0;
			// 
			// commentForm
			// 
			this.AcceptButton = this.secretCommentSendBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
			this.ClientSize = new System.Drawing.Size(486, 485);
			this.Controls.Add(this.commentBorderPanel);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.commentButtonPanel);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel1);
			this.Name = "commentForm";
			this.Text = "commentForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Close);
			this.Click += new System.EventHandler(this.commentSendButtonPane_Clickl);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.visitImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.commentList)).EndInit();
			this.commentBorderPanel.ResumeLayout(false);
			this.commentBorderPanel.PerformLayout();
			this.commentPanel.ResumeLayout(false);
			this.commentPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button secretCommentSendBtn;
		private System.Windows.Forms.DataGridViewTextBoxColumn score;
		private System.Windows.Forms.DataGridViewTextBoxColumn id;
		private System.Windows.Forms.DataGridViewTextBoxColumn color;
		private System.Windows.Forms.DataGridViewTextBoxColumn comment;
		private System.Windows.Forms.DataGridViewTextBoxColumn 時間;
		private System.Windows.Forms.TextBox textBox1;
		private namaichi.placeHolderText commentPlaceHolderText;
		private namaichi.placeTextBox5 commentText;
		private System.Windows.Forms.Panel commentPanel;
		private System.Windows.Forms.Panel commentBorderPanel;
		private System.Windows.Forms.DataGridView commentList;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.CheckBox is184ChkBox;
		private System.Windows.Forms.Label commentButtonPanel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label communityLabel;
		private System.Windows.Forms.PictureBox pictureBox2;
		public System.Windows.Forms.Label commentLabel;
		public System.Windows.Forms.Label visitLabel;
		private System.Windows.Forms.Panel visitCommentBar;
		private System.Windows.Forms.PictureBox visitImage;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ContextMenuStrip commentMenuStrip;
		//private System.Windows.Forms.TextBox commentPlaceHolderText;
		//private System.Windows.Forms.TextBox commentText;
	}
}
