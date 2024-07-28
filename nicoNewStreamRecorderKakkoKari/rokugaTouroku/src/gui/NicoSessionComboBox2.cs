/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/08/24
 * Time: 3:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SunokoLibrary.Windows.Forms;

namespace rokugaTouroku
{
	using SunokoLibrary.Application;
    using SunokoLibrary.Windows.ViewModels;

    /// <summary>
    /// ニコニコ動画アカウント一覧の表示用コンボボックス。
    /// </summary>
    public class NicoSessionComboBox2 : BrowserComboBox
    {
#pragma warning disable 1591
        protected override void InitLayout()
        {
        	nscb = this;
            base.InitLayout();
            Initialize(new CookieSourceSelector(CookieGetters.Default, importer => new NicoAccountSelectorItem(importer)));
        }
#pragma warning restore 1591
		public static NicoSessionComboBox2 nscb = null;

        class NicoAccountSelectorItem : CookieSourceItem
        {
            public NicoAccountSelectorItem(ICookieImporter importer) : base(importer) { }
            string _accountName, _displayText;
            public string AccountName
            {
                get { return _accountName; }
                private set
                {
                    _accountName = value;
                    OnPropertyChanged();
                }
            }
            public override string DisplayText
            {
                get { return _displayText; }
                protected set
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
            public async override void Initialize()
            {
                var baseText = string.Format("{0}{1}{2}",
                    Importer.SourceInfo.IsCustomized ? "カスタム設定 " : string.Empty,
                    Importer.SourceInfo.BrowserName,
                    Importer.SourceInfo.ProfileName.ToLowerInvariant() == "default" ? string.Empty : string.Format(" {0}", Importer.SourceInfo.ProfileName));
                DisplayText = string.Format("{0} (loading...)", baseText);
                await Task.Factory.StartNew(async () => {
	                AccountName = await GetUserName(Importer);
	                
	                try {
	                	if (nscb != null && !nscb.IsDisposed) {
	                		nscb.BeginInvoke((MethodInvoker)delegate() {
								DisplayText = string.IsNullOrEmpty(AccountName) == false
					                    ? string.Format("{0} ({1})", baseText, AccountName) : baseText;
							});
	                	}
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
					} 
		                
	                
				});
            }
            static async Task<string> GetUserName(ICookieImporter cookieImporter)
            {
                try
                {
                    var myPage = new Uri("https://www.nicovideo.jp/my/channel");
                    
                    var container = new CookieContainer();
                    container.PerDomainCapacity = 200;
                    var client = new HttpClient(new HttpClientHandler() { CookieContainer = container, Proxy = null, UseProxy = false });
                    
                    var result = await cookieImporter.GetCookiesAsync(myPage);
                    
					if (cookieImporter.SourceInfo.BrowserName.StartsWith("IE ") && 
                        result.Status == CookieImportState.AccessError) {
                    	util.debugWriteLine("ﾌﾞﾗｳｻﾞが開いている為Cookie取得できません " + cookieImporter.SourceInfo.BrowserName);
                    	return "ﾌﾞﾗｳｻﾞが開いている為Cookie取得できません";
                    	//return "DLLエラー" + result.Status;
                    } else if (result.Status != CookieImportState.Success) return null;
                    foreach(Cookie c in result.Cookies) {
                    	if (Regex.IsMatch(c.Name, "[^0-9a-zA-Z\\._\\-\\[\\]%#&=\":\\{\\} \\(\\)/\\?\\|]") ||
                    	   		Regex.IsMatch(c.Value, "[^0-9a-zA-Z\\._\\-\\[\\]%#&=\":\\{\\} \\(\\)/\\?\\|]")) {
                    		util.debugWriteLine(c.Name + " " + c.Value);
                    		continue;
                    	}
						if (c.Name != "user_session") continue;
                    	try {
                    		container.Add(new Cookie(c.Name, c.Value, c.Path, c.Domain));
                    	} catch (Exception e) {
                    		util.debugWriteLine(e.Message + e.StackTrace + e.TargetSite + e.Source);
                    	}
	        			
	        		}
                    
//                    if (result.AddTo(container) != CookieImportState.Success)
//                        return null;
					
					var us = container.GetCookies(myPage)["user_session"];
					if (us == null) {
						return null;
					}
					
					var n = util.getMyName(container, us.Value);
					return n;
					
					/*
					//if (n != null) return n;
                    var res = await client.GetStringAsync(myPage);
                    if (string.IsNullOrEmpty(res))
                        return null;
                    var namem = Regex.Match(res, "nickname = \"([^<>]+)\";", RegexOptions.Singleline);
                    if (namem.Success)
                        return namem.Groups[1].Value;
                    else
                        return null;
                    */
                    
                }
                catch (System.Net.Http.HttpRequestException) { return null; }
            }
        }
    }
}
