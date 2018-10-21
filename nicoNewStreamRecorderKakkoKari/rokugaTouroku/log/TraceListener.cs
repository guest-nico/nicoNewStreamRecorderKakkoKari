<<<<<<< HEAD
﻿/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/06/13
 * Time: 3:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace rokugaTouroku.log
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
			} catch (Exception e) {
				
//				util.debugWriteLine("trace listner exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
	}
}
=======
﻿/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/06/13
 * Time: 3:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace rokugaTouroku.log
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
			} catch (Exception e) {
				
//				util.debugWriteLine("trace listner exception " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			}
		}
	}
}
>>>>>>> 1faa06f1cca31cbe7e39015381b5150050941e1c
