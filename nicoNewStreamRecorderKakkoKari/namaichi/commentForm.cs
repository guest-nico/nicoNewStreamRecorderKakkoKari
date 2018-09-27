/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/21
 * Time: 4:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

using WebSocket4Net;

using namaichi.play;

namespace namaichi
{
	/// <summary>
	/// Description of commentForm.
	/// </summary>
	public partial class commentForm : Form
	{
		private int commentListclickedRowIndex = -1;
		private config.config config;
		private MainForm form;
		private CommentPlayerWebSocket2 cpws;
		
		public commentForm(config.config config, MainForm form)
		{
			this.config = config;
			this.form = form;
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			try {
				StartPosition = FormStartPosition.Manual;
				
				Location = new Point(int.Parse(config.get("defaultCommentFormX")),
						int.Parse(config.get("defaultCommentFormY")));
				Width = int.Parse(config.get("defaultCommentFormWidth"));
				Height = int.Parse(config.get("defaultCommentFormHeight"));
			} catch (Exception e) {
				util.debugWriteLine("comment foerm init " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			}
			
			commentList.DefaultCellStyle.SelectionBackColor = commentList.DefaultCellStyle.BackColor;
			commentList.DefaultCellStyle.SelectionForeColor = Color.FromArgb(114, 114, 114);
			
			visitLabel.Text = form.visitLabel.Text;
			commentLabel.Text = form.commentLabel.Text;
			if (form.rec.communityNum != null) communityLabel.Text = form.rec.communityNum;
			
			is184ChkBox.Checked = bool.Parse(config.get("Is184"));
			
			Task.Run(() => connectMessageServer());
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		private void commentPanelBorderPaint(object sender, PaintEventArgs e) {
			var color = Color.FromArgb(229, 229, 229);
			ControlPaint.DrawBorder(e.Graphics, commentBorderPanel.ClientRectangle, color, ButtonBorderStyle.Solid);
		}
		private void commentTextPaint(object sender, PaintEventArgs e) {
			System.Diagnostics.Debug.WriteLine("paint");
			//commentText.displayPlaceHolder();
			
			var color = Color.FromArgb(118, 118, 118);
//			ControlPaint.DrawBorder(e.Graphics, commentBorderPanel.ClientRectangle, color, ButtonBorderStyle.Solid);
			e.Graphics.DrawString("aコメント（75文字以内）", commentPlaceHolderText.Font, new SolidBrush(color), 0, 0);
			var g = e.Graphics;
			g.FillRectangle(new SolidBrush(Color.Green), 0,0,100,100);
			color = Color.FromArgb(229, 229, 229);
			g.DrawString("aコメント（75文字以内）", commentPlaceHolderText.Font, new SolidBrush(color), 1, 1);
			ControlPaint.DrawStringDisabled(g, "aaaaa", commentPlaceHolderText.Font, color, commentPlaceHolderText.ClientRectangle, StringFormat.GenericDefault);
			
		}
		void commentText_KeyDown(object sender, KeyEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(commentPlaceHolderText.Text);
		}
		void commentPanel_MouseDown(object sender, MouseEventArgs e)
		{
			commentPlaceHolderText.Focus();
		}
		
		
		void guide_focusget(object sender, EventArgs e)
		{
			commentText.Focus();
		}
		
		void infoButton_Click(object sender, EventArgs e)
		{
			Height = (Height == 300) ? 130 : 300; 
		}
		public void addComment(string time, string contents, string userId, string score, string color) {
			try {
	       		if (!IsDisposed) {
			        	Invoke((MethodInvoker)delegate() {
			       	       	try {
				       	       	var isScroll = (commentList.FirstDisplayedScrollingRowIndex +
					       	       	    commentList.DisplayedRowCount(true) 
					       	       	    >= commentList.Rows.Count);
								var rows = new string[]{time, contents, color, userId, score};
				       	       	commentList.Rows.Add(rows);
				       	       	
				       	       	if (isScroll) commentList.FirstDisplayedScrollingRowIndex = commentList.Rows.Count - 1;
				       	       	
				       	       	while (commentList.Rows.Count > 20 && false) {
				       	       		commentList.Rows.RemoveAt(0);
				       	       	}
			       	       	} catch (Exception e) {
			       	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       	}
			       	       	
						});
	       		}
	       	} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
		}
		void form_Close(object sender, FormClosingEventArgs e)
		{
			if (!kakuninClose()) e.Cancel = true;
			if (cpws != null)
				cpws.isEnd = true;
		}
		bool kakuninClose() {
			try{
				util.debugWriteLine(" comment x " + Location.X + " comment y " + Location.Y + " comment width " + Width.ToString() + " comment height " + Height.ToString() + " restore width " + RestoreBounds.Width.ToString() + " restore height " + RestoreBounds.Height.ToString());
				
				if (WindowState == FormWindowState.Normal) {
					config.set("defaultCommentFormWidth", Width.ToString());
					config.set("defaultCommentFormHeight", Height.ToString());
					config.set("defaultCommentFormX", Location.X.ToString());
					config.set("defaultCommentFormY", Location.Y.ToString());
					
				} else {
					config.set("defaultCommentFormWidth", RestoreBounds.Width.ToString());
					config.set("defaultCommentFormHeight", RestoreBounds.Height.ToString());
					config.set("defaultCommentFormX", RestoreBounds.X.ToString());
					config.set("defaultCommentFormY", RestoreBounds.Y.ToString());
				}
				config.set("Is184", is184ChkBox.Checked.ToString().ToLower());

			} catch(Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
			}
			return true;
		}
		void commentList_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			var row = commentList.Rows[e.RowIndex]; 
			row.Cells[0].Style.
				ForeColor = Color.FromArgb(114, 114, 114);
			var _c = row.Cells[2].Value.ToString();
			Color c;
			if (_c == "red") c = Color.FromArgb(255, 0, 51);
			//else if (_c == "grey") c = Color.FromArgb(114, 114, 114);
			else if (_c == "blue") c = Color.FromArgb(0, 197, 204);
			else c = Color.Black;
			row.Cells[1].Style.ForeColor = c;

		}
		void visitLabel_sizeChanged(object sender, EventArgs e)
		{
//			if (visitLabel.Text.Length == 1) {
				
//			}
			visitLabel.Left = 107 - visitLabel.Width;
			visitImage.Left = 89 - visitLabel.Width;
			updateVisitCommentBarWidth();
		}
		void commentLabel_sizeChanged(object sender, System.EventArgs e)
		{
			updateVisitCommentBarWidth();
		}
		void updateVisitCommentBarWidth() {
			visitCommentBar.Left = visitImage.Left - 2;
			visitCommentBar.Width = commentLabel.Right + 0 - visitCommentBar.Left;
			//visitCommentBar.Width = commentLabel.Left + commentLabel.Width + 4;
		}
		void commentList_Click(object sender, EventArgs e) {
			var row = commentListclickedRowIndex;
			var a = (ContextMenuStrip)sender;
//			var c = a.Tag;
			var b = (ToolStripItemClickedEventArgs)e;
			//toolstrip
			if (b.ClickedItem.Text == "コメントをコピー" && row != -1) {
				try {
					Clipboard.SetText((string)commentList.Rows[row].Cells[1].Value);
				} catch (Exception ee) {
					util.debugWriteLine("clipbord cell click exception " + ee.Message + ee.Source + ee.TargetSite + ee.StackTrace);
				}
			}
			
		}
		void commentList_cellClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			commentListclickedRowIndex = e.RowIndex;
		}
		public void setStatistics(string visit, string comment) {
			try {
	       		if (!IsDisposed) {
			        	Invoke((MethodInvoker)delegate() {
			       	       	try {
				       	       	visitLabel.Text = visit;
				       	       	commentLabel.Text = comment;
			       	       	} catch (Exception e) {
			       	       		util.debugWriteLine("comment form statistics exception " + e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
			       	       	}
			       	       	
						});
	       		}
	       	} catch (Exception e) {
	       		util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.Source + " " + e.TargetSite);
	       	}
		}
		
		async void commentSendButtonPane_Clickl(object sender, EventArgs e)
		{
			commentSend();
		}
		void commentSend() {
			if (commentText.Text == "" || !commentText.Focused) return;
			Task.Run(() => {
			    Invoke((MethodInvoker)delegate() {
		         	commentButtonPanel.BackColor = Color.FromArgb(115, 179, 242);
		         	Thread.Sleep(100);
		         	commentButtonPanel.BackColor = Color.FromArgb(0, 128, 255);
				});
			});
			Task.Run(() => {
				form.rec.wsr.sendComment(commentText.Text, is184ChkBox.Checked);
				
				Invoke((MethodInvoker)delegate() {
					commentText.Text = "";
				});
			});
		}
		private void secretCommentSendBtn_Click(object sender, EventArgs e)
		{
			commentSend();
		}
		private void connectMessageServer() {
			cpws = new CommentPlayerWebSocket2(form.rec.wsr, this);
			cpws.start();
		}
	}
}
