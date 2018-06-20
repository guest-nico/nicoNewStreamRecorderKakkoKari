/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2018/04/29
 * Time: 20:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration;
using System.Collections.Generic;

namespace namaichi.config {
/// <summary>
/// Description of config.
/// </summary>
public class config
{
	private Configuration cfg;
	public config()
	{
		cfg = getConfig();
		defaultMergeFile();
        
 	}
	public Configuration getConfig() {
		try {
			var jarPath = util.getJarPath();
			var configFile = jarPath[0] + "\\" + jarPath[1] + ".config";
			//util.debugWriteLine(configFile);
	        var exeFileMap = new System.Configuration. ExeConfigurationFileMap { ExeConfigFilename = configFile };
	        var cfg     = ConfigurationManager.OpenMappedExeConfiguration(exeFileMap, ConfigurationUserLevel.None);
	        return cfg;
		} catch (Exception e) {
			util.debugWriteLine("getconfig " + e.Message + " " + e.StackTrace + " " + e.TargetSite);
			return null;
		}
	}
	public void set(string key, string value) {
		util.debugWriteLine("config set " + key);
		for (var i = 0; i < 100; i++) {
			cfg = getConfig();
			
			
			var keys = cfg.AppSettings.Settings.AllKeys;
			if (System.Array.IndexOf(keys, key) < 0)
				cfg.AppSettings.Settings.Add(key, value);
			else cfg.AppSettings.Settings[key].Value = value;
			try {
				cfg.Save();
				return;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + " " + e.StackTrace);
				System.Threading.Thread.Sleep(500);
				continue;
			}
		}
	}
	public string get(string key) {
		util.debugWriteLine("config get " + key);
		if (key != "accountId" && key != "accountPass" &&
		   		key != "user_session" && key != "user_session_secure") {
			util.debugWriteLine(key + " " + cfg.AppSettings.Settings[key].Value);
		}
		return cfg.AppSettings.Settings[key].Value;
	}
	private void defaultMergeFile() {
		var defaulBuf = new Dictionary<string, string>(){
			{"accountId",""},
			{"accountPass",""},
			{"user_session",""},
			{"user_session_secure",""},
			{"browserNum","1"},
//			{"isAllBrowserMode","true"},
			{"issecondlogin","false"},
			{"IsdefaultBrowserPath","true"},
			{"browserPath",""},
			{"Isminimized","false"},
			{"IscloseExit","true"},
			{"IsfailExit","false"},
			{"IsgetComment","true"},
			{"IsmessageBox","false"},
			{"IshosoInfo","false"},
			{"Islog","true"},
			{"IstitlebarInfo","true"},
			{"Islimitpopup","true"},
			{"Isretry","true"},
			{"IsdeleteExit","false"},
			{"IsgetcommentXml","true"},
			{"IstitlebarSamune","true"},
			{"IsautoFollowComgen","false"},
			{"qualityRank","0,1,2,3,4,5"},
			{"IsLogFile","false"},
			
			
			{"cookieFile",""},
			{"iscookie","false"},
			{"recordDir",""},
			{"IsdefaultRecordDir","true"},
			{"IscreateSubfolder","true"},
			{"subFolderNameType","1"},
			{"fileNameType","1"},
			{"filenameformat","{Y}年{M}月{D}日{h}時{m}分{0}_{1}_{2}_{3}_{4}"},
			{"ffmpegopt",""},
			{"Height","400"},
			{"Width","648"},
		};
		var buf = new Dictionary<string,string>();
		foreach (var k in cfg.AppSettings.Settings.AllKeys) {
			buf.Add(k, cfg.AppSettings.Settings[k].Value);
		}
		
		cfg.AppSettings.Settings.Clear();
		foreach (var k in defaulBuf.Keys) {
			var v = (buf.ContainsKey(k)) ? buf[k] : defaulBuf[k];
			cfg.AppSettings.Settings.Add(k, v);
		}
		try {
			cfg.Save();
		} catch (Exception e) {
			util.debugWriteLine(e.Message + " " + e.StackTrace);
		}
		
		
		// Dictionary<string, string>
	}
	public void saveFromForm(Dictionary<string, string> formData) {
		cfg = getConfig();
		
		foreach (var k in formData.Keys) {
			cfg.AppSettings.Settings[k].Value = formData[k];
			//util.debugWriteLine(k + formData[k]);
		}		
		try {
			cfg.Save();
		} catch (Exception e) {
			util.debugWriteLine(e.Message + " " + e.StackTrace);
		}
	}
	private string[] defaultConfig = {};
}

}