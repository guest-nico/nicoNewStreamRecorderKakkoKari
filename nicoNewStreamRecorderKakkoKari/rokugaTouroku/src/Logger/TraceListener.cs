/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/06/13
 * Time: 3:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace rokugaTouroku.Logger
{
	/// <summary>
	/// Description of TraceListener.
	/// </summary>
	public class TraceListener:DefaultTraceListener
	{
		public TraceListener()
		{
		}
		public override void WriteLine(string msg) {
			try {
				var dt = DateTime.Now.ToLongTimeString();
				base.WriteLine(dt + " " + msg);
			} catch (Exception) {
				
//				util.debugWriteLine("trace listner exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
	}
}
