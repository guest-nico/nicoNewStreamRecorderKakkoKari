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
		public static string arg = "";
		
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length > 0) arg = util.getRegGroup(args[0], "(lv.+)");
			
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandleExceptionHandler);
			System.Threading.Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(UnhandleExceptionHandler);
			AppDomain.CurrentDomain.UnhandledException += UnhandleExceptionHandler;
			System.Threading.Thread.GetDomain().UnhandledException += UnhandleExceptionHandler;
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(threadException);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			System.Threading.Tasks.TaskScheduler.UnobservedTaskException += taskSchedulerUnobservedTaskException;
			AppDomain.CurrentDomain.FirstChanceException += firstChanceException;
				
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
//			args = new string[]{"-nowindo", "lv316266831", "-stdIO"};
//			args = new String[]{"lv316036760", "-ts-start=5m0s", "-ts-end=5m10s", "-afterConvertMode=4"};
			if (Array.IndexOf(args, "-nowindow") == -1) 
				Application.Run(new MainForm(args));
			else {
				util.isShowWindow = false;
				var a = new MainForm(args);
				while(a.rec.isRecording) System.Threading.Thread.Sleep(1000);
			}
			
		}
		private static void UnhandleExceptionHandler(Object sender, UnhandledExceptionEventArgs e) {
			util.debugWriteLine("unhandled exception");
			var eo = (Exception)e.ExceptionObject;
			util.showException(eo);
			                    
		}
		static void threadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
		    util.debugWriteLine("thread exception");
			var eo = (Exception)e.Exception;
			util.showException(eo);
			
		}
		static private void taskSchedulerUnobservedTaskException(object sender,
				System.Threading.Tasks.UnobservedTaskExceptionEventArgs e) {
			util.debugWriteLine("task_unobserved exception");
			var eo = (Exception)e.Exception;
			util.showException(eo);
			e.SetObserved();
			
		}
		static private void firstChanceException(object sender,
			System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) {
			var frameCount = 0;
			try {
				frameCount = new System.Diagnostics.StackTrace().FrameCount;
			} catch (StackOverflowException ee) {
				return;
			}
			#if DEBUG
				if (util.isLogFile) {
					if (frameCount > 150) {
//						util.debugWriteLine("exception stacktrace framecount " + frameCount);
						MessageBox.Show("first chance framecount stack " + e.Exception.Message + e.Exception.StackTrace, frameCount.ToString() + " " + DateTime.Now + " " + arg);
//						if (e.Exception.GetType() == System.IO.IOException
						return;
					}
				}
			#else
				
			#endif
		
			util.debugWriteLine("exception stacktrace framecount " + frameCount);
		
			util.debugWriteLine("firstchance exception");
			var eo = (Exception)e.Exception;
			util.showException(eo, false);
		
		}
	}
	
}
