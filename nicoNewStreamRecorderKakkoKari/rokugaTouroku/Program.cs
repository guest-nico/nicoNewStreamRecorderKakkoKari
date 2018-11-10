/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/10
 * Time: 16:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace rokugaTouroku
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		public static string arg = "";
		
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
			Application.Run(new MainForm(args));
			//args = new string[]{"lv888"};
			//var a = new MainForm(args);
			//while(true) System.Threading.Thread.Sleep(1000);
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
			var frameCount = new System.Diagnostics.StackTrace().FrameCount;
			#if DEBUG
				if (util.isLogFile) {
					if (frameCount > 150) {
						MessageBox.Show("first chance framecount stack " + e.Exception.Message + e.Exception.StackTrace, frameCount.ToString() + " " + DateTime.Now + " " + arg);
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
