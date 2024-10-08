﻿/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2021/09/30
 * Time: 13:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SunokoLibrary.Application;

namespace namaichi.info
{
	/// <summary>
	/// Description of AccountInfo.
	/// </summary>
	public class AccountInfo
	{
		public CookieSourceInfo si;
		public string accountId;
		public string accountPass;
		public string userSession;
		public bool isBrowser = false;
		public bool isAccount = false;
		public bool isUserSession = false;
		public AccountInfo(CookieSourceInfo si, string accountId, 
				string accountPass, string userSession, bool isBrowser, bool isAccount, 
				bool isUserSession)
		{
			this.si = si;
			this.accountId = accountId;
			this.accountPass = accountPass;
			this.userSession = userSession;
			this.isBrowser = isBrowser;
			this.isAccount = isAccount;
			this.isUserSession = isUserSession;
		}
		public static AccountInfo fromJsonArg(string arg) {
			util.debugWriteLine("AccountInfo fromJsonArg " + arg);
			try {
				var serializer = new XmlSerializer(typeof(AccountSetting));
				using (var ms = new MemoryStream()) {
					var b = Encoding.UTF8.GetBytes(arg);
					ms.Write(b, 0, b.Length);
					ms.Flush();
					ms.Seek(0, SeekOrigin.Begin);
					var _ai = (AccountSetting)serializer.Deserialize(ms);
					if (_ai.isBrowser) {
						var si = new CookieSourceInfo(_ai.BrowserName, _ai.ProfileName, _ai.CookiePath, _ai.EngineId, _ai.IsCustomized);
						return new AccountInfo(si, null, null, null, true, false, false);
					} else if (_ai.isAccount) {
						return new AccountInfo(null, _ai.mail, _ai.pass, null, false, true, false);
					} else {
						return new AccountInfo(null, null, null, _ai.userSession, false, false, true);
					}
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
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
			public string userSession = "";
			public bool isBrowser = false;
			public bool isAccount = false;
			public bool isUserSession = false;
		}
	}
}
