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
		var jarPath = util.getJarPath();
		var configFile = jarPath[0] + "\\" + jarPath[1] + ".config";
		//System.Diagnostics.Debug.WriteLine(configFile);
        var exeFileMap = new System.Configuration. ExeConfigurationFileMap { ExeConfigFilename = configFile };
        var cfg     = ConfigurationManager.OpenMappedExeConfiguration(exeFileMap, ConfigurationUserLevel.None);
        return cfg;
	}
	public void set(string key, string value) {
		cfg = getConfig();
		
		var keys = cfg.AppSettings.Settings.AllKeys;
		if (System.Array.IndexOf(keys, key) < 0)
			cfg.AppSettings.Settings.Add(key, value);
		else cfg.AppSettings.Settings[key].Value = value;
		try {
			cfg.Save();
		} catch (Exception e) {
			System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
		}
	}
	public string get(string key) {
		System.Diagnostics.Debug.WriteLine(key);
		System.Diagnostics.Debug.WriteLine(key + " " + cfg.AppSettings.Settings[key].Value);
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
			System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
		}
		
		
		// Dictionary<string, string>
	}
	public void saveFromForm(Dictionary<string, string> formData) {
		cfg = getConfig();
		
		foreach (var k in formData.Keys) {
			cfg.AppSettings.Settings[k].Value = formData[k];
			//System.Diagnostics.Debug.WriteLine(k + formData[k]);
		}		
		try {
			cfg.Save();
		} catch (Exception e) {
			System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
		}
	}
	private string[] defaultConfig = {};
}

}