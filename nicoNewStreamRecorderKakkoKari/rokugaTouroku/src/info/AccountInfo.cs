/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2021/09/30
 * Time: 3:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SunokoLibrary.Application;

namespace rokugaTouroku.info
{
	/// <summary>
	/// Description of AccountInfo.
	/// </summary>
	public class AccountInfo
	{
		public bool isRecSetting = true;
		public CookieSourceInfo si;
		public string accountId;
		public string accountPass;
		public bool isBrowser = false;
		
		public bool useSecondLogin = false;
		public string cookieFile;
	        	
		public AccountInfo(CookieSourceInfo si, string accountId, string accountPass, bool isBrowser, bool isSecondLogin, bool isRecSetting, string cookieFile)
		{
			this.si = si;
			this.accountId = accountId;
			this.accountPass = accountPass;
			this.isBrowser = isBrowser;
			this.useSecondLogin = isSecondLogin;
			this.isRecSetting = isRecSetting;
			this.cookieFile = cookieFile;
		}
		public string getArg() {
			try {
				if (isRecSetting) return null;
				
				XmlSerializer serializer = new XmlSerializer(typeof(AccountSetting));
				using (var ms = new MemoryStream()) {
					var _ai = new AccountSetting();
					_ai.IsCustomized = si != null ? si.IsCustomized : false;
					_ai.BrowserName = si != null ? si.BrowserName : "";
					_ai.ProfileName = si != null ? si.BrowserName : "";
					_ai.CookiePath = si != null && si.CookiePath != null ? si.CookiePath.Replace('\\', '/') : "";
					_ai.EngineId = si != null ? si.EngineId : "";
					_ai.mail = accountId == null ? "" : accountId;
					_ai.pass = accountPass == null ? "" : accountPass;
					_ai.isBrowser = isBrowser;
					
					var ns = new XmlSerializerNamespaces();
					ns.Add(string.Empty, string.Empty);
					serializer.Serialize(ms, _ai, ns);
					var b = Encoding.UTF8.GetString(ms.ToArray());
					
					var a = b.Replace("\n", "").Replace("\r", "");
					a = util.getRegGroup(a, "(<Account.+)");
					var c = "\"" + a + "\"";
					util.debugWriteLine("accountInfo getArg " + a);
					//util.debugWriteLine(a + " " + b + " " + ms.Length);
					
					return c;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return "";
			}
		}
		public class AccountSetting {
			public bool IsCustomized = false;
			public string BrowserName = null;
			public string ProfileName = null;
			public string CookiePath = null;
			public string EngineId = null;
			public string mail = "";
			public string pass = "";
			public bool isBrowser = false;
		}
		
	}
}
