/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2026/08/13
 * Time: 23:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace rokugaTouroku.gui
{
	partial class WebViewLoginForm
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
			this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
			((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
			this.SuspendLayout();
			// 
			// webView21
			// 
			this.webView21.AllowExternalDrop = true;
			this.webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.webView21.CreationProperties = null;
			this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
			this.webView21.Location = new System.Drawing.Point(12, 12);
			this.webView21.Name = "webView21";
			this.webView21.Size = new System.Drawing.Size(484, 566);
			this.webView21.TabIndex = 0;
			this.webView21.ZoomFactor = 1D;
			this.webView21.CoreWebView2InitializationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs>(this.WebView21CoreWebView2InitializationCompleted);
			// 
			// WebViewLoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(508, 590);
			this.Controls.Add(this.webView21);
			this.Name = "WebViewLoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "WebViewLoginForm";
			((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
			this.ResumeLayout(false);
		}
		private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
	}
}
