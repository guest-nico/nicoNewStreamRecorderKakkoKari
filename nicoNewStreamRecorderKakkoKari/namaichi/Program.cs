/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/06
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;


namespace namaichi
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandleExceptionHandler);
				
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(args));
		}
		private static void UnhandleExceptionHandler(Object sender, UnhandledExceptionEventArgs e) {
			var eo = (Exception)e.ExceptionObject;
			util.debugWriteLine("eo " + eo);
			util.debugWriteLine("0 message " + eo.Message + "\nsource " + 
					eo.Source + "\nstacktrace " + eo.StackTrace + 
					"\n targetsite " + eo.TargetSite + "\n\n");
			
			var _eo = eo.GetBaseException();
			util.debugWriteLine("eo " + _eo);
			util.debugWriteLine("1 message " + _eo.Message + "\nsource " + 
					_eo.Source + "\nstacktrace " + _eo.StackTrace + 
					"\n targetsite " + _eo.TargetSite + "\n\n");
			
			eo = eo.InnerException;
			util.debugWriteLine("eo " + eo);
			util.debugWriteLine("2 message " + eo.Message + "\nsource " + 
					eo.Source + "\nstacktrace " + eo.StackTrace + 
					"\n targetsite " + eo.TargetSite);
			                    
		}
		
	}
	
}
