/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/08/12
 * Time: 20:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace namaichi
{
	/// <summary>
	/// Description of placeHolderText
	/// </summary>
	public partial class placeHolderText : TextBox
	{
		//string placeHolder = "コメント（75文字以内）";
		//bool isNowDisplay = true;
		//bool isFirst = true;
		//bool isNowIMEComposition = false;
		//bool isExistIMEChar = false;
		public placeTextBox5 placeText;
		IntPtr placeTextBoxHwnd = IntPtr.Zero;
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
    	private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
    	
    	public placeHolderText() {
    		InitializeComponent();
    	}
		public placeHolderText(placeTextBox5 placeText)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			BorderStyle = BorderStyle.None;
			GotFocus += onGetFocus;
			LostFocus += onLeaveFocus;
			TextChanged += onTextChanged;
//			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			//BackColor = Color.Transparent;
//			BackColor = Color.SkyBlue;
			this.placeText = placeText;
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		protected override void WndProc(ref Message m) {
			
	        /*
	        const int WM_PAINT = 0x000F;
	        var WM_IME_COMPOSITION = 0x10F;
	        var WM_IME_STARTCOMPOSITION = 269;
	        var WM_IME_ENDCOMPOSITION = 270;
	        //var WM_ERASEBKGND = 20;
	        var WM_CHAR = 258;
	        var WM_IME_CHAR = 646;
			*/
			var WM_PASTE = 770;
//	        System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + m.Msg);
	        
	        if (placeTextBoxHwnd == IntPtr.Zero && placeText != null &&
	            placeText.IsHandleCreated) placeTextBoxHwnd = placeText.Handle;     
//	        if (placeTextBoxHwnd == IntPtr.Zero) {
//	        	base.WndProc(ref m);
//	        	return;
//	        }
	        
	        if (m.Msg == WM_PASTE && placeTextBoxHwnd != IntPtr.Zero) {
	        	//System.Diagnostics.Debug.WriteLine(m.LParam.ToString());
	        	SendMessage(placeTextBoxHwnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToString());
//	        	placeText._WndProc(ref m);
	        	return;
	        }
//	        System.Diagnostics.Debug.WriteLine(placeText.Text);
	        base.WndProc(ref m);
	        return;
	        
	        /*
	        //if (m.Msg == WM_PAINT)
	        if (m.Msg == WM_IME_STARTCOMPOSITION || 
	           m.Msg == WM_PASTE) {
	        	clearPlaceHolder();
	        }
	        if (m.Msg == WM_IME_STARTCOMPOSITION) {
	        	isNowIMEComposition = true;
	        	isExistIMEChar = false;
	        }
	        if (m.Msg == WM_IME_ENDCOMPOSITION) {
	        	isNowIMEComposition = false;
	        	if (!isExistIMEChar && Text == "") 
	        		displayPlaceHolder();
	        }
	        if (m.Msg == WM_IME_CHAR) isExistIMEChar = false;
	        if (m.Msg == WM_CHAR) {
//	        	if (m.LParam
//	       		if (Text == "") displayPlaceHolder();
//	        	else clearPlaceHolder();
//	        	clearPlaceHolder();
	       		
	        }
	        if (m.Msg == 270) {
//	        	System.Diagnostics.Debug.WriteLine(Text);
	        }
	        /*
	        if ((m.Msg == WM_ERASEBKGND || m.Msg == -8 ||
	            m.Msg == 8465 || m.Msg == 641 || m.Msg == -7 ||
	            m.Msg == -33 || m.Msg == -32 || m.Msg == 133 || 
	            m.Msg == 14 || m.Msg == 13) && isNowDisplay) {
	        	//displayPlaceHolder();
	        	//Focus();
	        	if (m.Msg == -7) {
	        		var _m = Message.Create(m.HWnd, 32, IntPtr.Zero, IntPtr.Zero);
	        		base.WndProc(ref _m);
	        		Focus();
	        		
	        	}
	        	return;
	        }
	        *
	        //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + m.Msg + " " + isNowDisplay);
	        if (m.Msg == WM_PAINT && Text == "" && true)
	        {
//	        	System.Diagnostics.Debug.WriteLine("wmp");
//	        	System.Diagnostics.Debug.WriteLine("wmp focus " + Focused + " isnowdisp " + isNowDisplay);
	        	if (isFirst || isNowDisplay) {
	        		
	        		isFirst = false;
	        		//m.Result = new IntPtr(0x1);
	        		//var _m = Message.Create(m.HWnd, m.Msg, m.WParam, m.LParam);
	        		//base.WndProc(ref _m);
	        		displayPlaceHolder();
	        		//return;
	        	} else {
	        		
	        	}
	        	
//		        	return;
	        	/*
	            using (Graphics g = Graphics.FromHwnd(this.Handle))
	            {   
	                Rectangle rect = this.ClientRectangle;
	                rect.Offset(1, 1);
	                TextRenderer.DrawText(g, placeHolder, this.Font,
	                                      rect, Color.FromArgb(118, 118, 118), TextFormatFlags.Top | TextFormatFlags.Left);
	            }
	            *
	        } else if (m.Msg == WM_IME_COMPOSITION) {
	        	//if (isNowDisplay) 
	        	//	clearPlaceHolder();
	        		//System.Diagnostics.Debug.WriteLine("ime");
	        }
	        base.WndProc(ref m);
	        */
	    }
		string lastText = "";
		void onTextChanged(object sender, EventArgs e) {
			if (Text == "" && lastText != "") displayPlaceHolder();
			else if (Text != "" && lastText == "") {
				clearPlaceHolder();
				
				var now = SelectionStart;
				SelectionStart = 0;
				SelectionStart = now;
			}
			lastText = Text;
			
			if (Text.Length > 75) {
				var now = SelectionStart;
				Text = Text.Substring(0, 75);
				SelectionStart = now;
			}
		}
		void onGetFocus(object sender, EventArgs e) {
//			if (Text == "") clearPlaceHolder();
		}
		void onLeaveFocus(object sender, EventArgs e) {
//			if (Text == "") displayPlaceHolder();
		}
		public void displayPlaceHolder() {
			//isNowDisplay = true;
			Width = 1;
			
		}
		private void clearPlaceHolder() {
			Width = 371;
			//isNowDisplay = false;
		}
	}

}
