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
using System.Collections.Generic;

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
		public MainForm form;
		private int nowVpos;
		
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
			util.setFontSize(int.Parse(config.get("fontSize")), this, false);
			postingMainFormComment();
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
				if (!isNewComment(time, contents))
					return;
				
	       		if (!IsDisposed) {
			        	Invoke((MethodInvoker)delegate() {
			       	       	try {
				       	       	var isScroll = (commentList.FirstDisplayedScrollingRowIndex +
					       	       	    commentList.DisplayedRowCount(true) 
					       	       	    >= commentList.Rows.Count);
					       		var firstDisplayedRow = commentList.FirstDisplayedScrollingRowIndex != -1 ? commentList.Rows[commentList.FirstDisplayedScrollingRowIndex] : null;
								var rows = new string[]{time, contents, color, userId, score};
								if (!isDuplicateRow(rows))
				       	       		commentList.Rows.Add(rows);
				       	       	
				       	       	while (commentList.Rows.Count > 300 && true) {
				       	       		commentList.Rows.RemoveAt(0);
				       	       	}
								if (isScroll) {
									commentList.FirstDisplayedScrollingRowIndex = commentList.Rows.Count - 1;
								} else {
									var originalFirstI = commentList.Rows.IndexOf(firstDisplayedRow);
									if (originalFirstI != commentList.FirstDisplayedScrollingRowIndex && originalFirstI > -1)
										commentList.FirstDisplayedScrollingRowIndex = originalFirstI;
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
		bool isNewComment(string time, string userId) {
			if (commentList.Rows.Count == 0) return true;
			var lastRowCells = commentList.Rows[commentList.Rows.Count - 1].Cells;
			var _time = TimeSpan.Parse(time);
			var _lastRowTime = TimeSpan.Parse(lastRowCells[0].Value.ToString());
			var ret = lastRowCells[3].Value.ToString() != userId ||
					(TimeSpan.Parse(time) >
			 			TimeSpan.Parse(lastRowCells[0].Value.ToString()));
			if (!ret)
				util.debugWriteLine("aa");
			return ret;
		}
		void form_Close(object sender, FormClosingEventArgs e)
		{
			if (!kakuninClose()) e.Cancel = true;
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
			await commentSend();
		}
		async Task commentSend() {
			if (commentText.Text == "" || !commentText.Focused) return;
			await Task.Run(() => {
			    Invoke((MethodInvoker)delegate() {
		         	commentButtonPanel.BackColor = Color.FromArgb(115, 179, 242);
		         	Thread.Sleep(100);
		         	commentButtonPanel.BackColor = Color.FromArgb(0, 128, 255);
				});
			});
			await Task.Run(() => {
				form.rec.wsr.sendComment(commentText.Text, is184ChkBox.Checked);
				
				Invoke((MethodInvoker)delegate() {
					commentText.Text = "";
				});
			});
		}
		async private void secretCommentSendBtn_Click(object sender, EventArgs e)
		{
			await commentSend();
		}
		public void setTime(int h, int m, int s) {
			nowVpos = (h * 3600 + m * 60 + s) * 100;
		}
		private void connectMessageServer() {
			return;
			/*
			while (form.rec.wsr == null) {
				if (IsDisposed) return;
				Thread.Sleep(300);
			}
			if (form.rec.wsr.ri.si.isTimeShift && !((rec.WebSocketRecorder)form.rec.wsr).ri.isRealtimeChase) {
				displayTsComment();
			} else {
				cpws = new CommentPlayerWebSocket2(form.rec.wsr, this);
				cpws.start();
			}
			*/
		}
		void displayTsComment() {
			while (form.rec.wsr != null && 
			       form.rec.wsr.gotTsCommentList == null &&
			       !this.IsDisposed) {
				Thread.Sleep(200);
			}
			if (form.rec.wsr.gotTsCommentList == null) return;
			
			var chats = getChats(form.rec.wsr.gotTsCommentList);
			
			int lastCommentIndex = -1;
			while (form.rec.wsr != null && !this.IsDisposed) {
				try {
					for (var i = lastCommentIndex + 1; i < chats.Count; i++) {
//						util.debugWriteLine(string.Join(" ", chats[i]));
						var vpos = int.Parse(chats[i][5]); 
						if (vpos > nowVpos + 200) break;
						
						addComment(chats[i][0], chats[i][2], chats[i][1], chats[i][3], chats[i][4]);
						lastCommentIndex = i;
					}
					Thread.Sleep(300);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
		}
		private List<string[]> getChats(string[] gotTsComments) {
			var isXml = bool.Parse(form.rec.cfg.get("IsgetcommentXml"));
			var ret = new List<string[]>();
			foreach (var c in gotTsComments) {
//				if (c.IndexOf(" />") > -1 || c.IndexOf("content
				var vpos = int.Parse(util.getRegGroup(c, "vpos.+?(\\d+)"));
				var id = util.getRegGroup(c, "user_id.+?\"(.+?)\"");
				string content = null;
				string premium = null;
				string root = null;
				string score = "0";
				if (isXml) {
					content = util.getRegGroup(c, ">(.+)</");
					premium = util.getRegGroup(c, "premium=\"(.)\"");
					root = (c.IndexOf("type=\"") > -1) ? "control" : "chat";
					score = util.getRegGroup(c, "score=\"(.+?)\"");
				} else {
					if (c.IndexOf("\"text\":") > -1) content = util.getRegGroup(c, "\"text\":[ ]*\"(.+?)\"");
					else if (c.IndexOf("\"content\":") > -1) content = util.getRegGroup(c, "\"content\":[ ]*\"(.+?)\"");
					premium = util.getRegGroup(c, "\"premium\":[ ]*(\\d)");
					root = (c.IndexOf("\"type\":") > -1) ? "control" : "chat";
					score = util.getRegGroup(c, "\"score\":[ ]*(-\\d+)");
				}
				if (content == null) continue;

				if (score == null) score = "0";
				var __time = (int)(vpos / 100);
				if (vpos < 0) __time = 0;

				var h = (int)(__time / (60 * 60));
				var m = (int)((__time % (60 * 60)) / 60);
				var _m = (m < 10) ? ("0" + m.ToString()) : m.ToString();
				var s = __time % 60;
				var _s = (s < 10) ? ("0" + s.ToString()) : s.ToString();
				var keikaTime = h + ":" + _m + ":" + _s + "";
			
				var color = (premium == "3") ? "red" :
					((premium == "7") ? "blue" : "black");
				if (root == "control") color = "red";
			
				ret.Add(new string[]{keikaTime, id, content, score, color, vpos.ToString()});
			}
			return ret;
		}
		bool isDuplicateRow(string[] rows) {
			try {
				var _rows = commentList.Rows;
				foreach (DataGridViewRow _r in _rows) {
					var r = _r.Cells;
					if (r[0].Value.ToString() == rows[0] && r[1].Value.ToString() == rows[1] &&
					    	r[2].Value.ToString() == rows[2] && r[3].Value.ToString() == rows[3])
						return true;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			return false;
		}
		void postingMainFormComment() {
			try {
				var r = form.getFormComment();
				foreach (var _r in r) {
					commentList.Rows.Add(_r[0], _r[1], _r[4], _r[2], _r[3]);
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
		}
	}
}
