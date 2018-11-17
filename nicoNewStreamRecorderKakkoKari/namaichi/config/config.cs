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
using System.Threading;

namespace namaichi.config {
/// <summary>
/// Description of config.
/// </summary>
public class config
{
	private Configuration cfg;
	public Dictionary<string, string> defaultConfig;
	public Dictionary<string, string> argConfig = new Dictionary<string, string>();
	public config()
	{
		cfg = getConfig();
		defaultMergeFile();
        
 	}
	public Configuration getConfig() {
		while (true) {
			try {
				var jarPath = util.getJarPath();
				var configFile = jarPath[0] + "\\" + jarPath[1] + ".config";
				//util.debugWriteLine(configFile);
		        var exeFileMap = new System.Configuration. ExeConfigurationFileMap { ExeConfigFilename = configFile };
		        var cfg     = ConfigurationManager.OpenMappedExeConfiguration(exeFileMap, ConfigurationUserLevel.None);
		        return cfg;
			} catch (Exception e) {
				util.debugWriteLine("getconfig " + e.Message + " " + e.StackTrace + " " + e.TargetSite);
				Thread.Sleep(3000);
				continue;
			}
			
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
	public void set(List<KeyValuePair<string, string>> l) {
		foreach (var _l in l)
			util.debugWriteLine("config set " + _l.Key + " " + _l.Value);
		for (var i = 0; i < 100; i++) {
			cfg = getConfig();
			
			var keys = cfg.AppSettings.Settings.AllKeys;
			
			foreach (var _l in l) {
				if (System.Array.IndexOf(keys, _l.Key) < 0)
					cfg.AppSettings.Settings.Add(_l.Key, _l.Value);
				else cfg.AppSettings.Settings[_l.Key].Value = _l.Value;
			}
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
		try {
			if (key != "accountId" && key != "accountPass" &&
			   		key != "user_session" && key != "user_session_secure") {
				util.debugWriteLine(key + " " + cfg.AppSettings.Settings[key].Value);
			}
		} catch (Exception e) {
			util.debugWriteLine("config get exception " + key + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			return null;
		}
		try {
			if (argConfig.ContainsKey(key)) 
				return argConfig[key];
			return cfg.AppSettings.Settings[key].Value;
		} catch (Exception e) {
			return null;
		}
	}
	private void defaultMergeFile() {
		defaultConfig = new Dictionary<string, string>(){
			{"accountId",""},
			{"accountPass",""},
			{"user_session",""},
			{"user_session_secure",""},
			{"browserNum","1"},
//			{"isAllBrowserMode","true"},
			{"issecondlogin","false"},
			
			{"IsHokan","false"},
			{"accountId2",""},
			{"accountPass2",""},
			{"user_session2",""},
			{"user_session_secure2",""},
			{"browserNum2","1"},
			{"issecondlogin2","false"},
			{"cookieFile2",""},
			{"iscookie2","false"},
			
			{"IsdefaultBrowserPath","true"},
			{"browserPath",""},
			{"Isminimized","false"},
			{"IscloseExit","true"},
			{"IsfailExit","false"},
			{"IsgetComment","true"},
			{"IsmessageBox","false"},
			{"IshosoInfo","false"},
			{"IsDescriptionTag","false"},
			{"Islog","false"},
			{"IstitlebarInfo","true"},
			{"Islimitpopup","true"},
			{"Isretry","true"},
			{"IsdeleteExit","false"},
			{"IsgetcommentXml","true"},
			{"IstitlebarSamune","true"},
			{"IsautoFollowComgen","false"},
			{"qualityRank","0,1,2,3,4,5"},
			{"IsLogFile","false"},
			{"IsSegmentNukeInfo","true"},
			{"segmentSaveType","0"},
			{"IsRenketuAfter","true"},
			{"IsAfterRenketuFFmpeg","false"},
			{"IsDefaultEngine","true"},
			{"anotherEngineCommand",""},
			{"IsUsePlayer","true"},
			{"IsDefaultPlayer","true"},
			{"IsUseCommentViewer","true"},
			{"IsDefaultCommentViewer","true"},
			{"anotherPlayerPath",""},
			{"anotherCommentViewerPath",""},
			{"Is184","true"},
			{"IsUrlList","false"},
			{"IsM3u8List","false"},
			{"M3u8UpdateSeconds","5"},
			{"IsOpenUrlList","false"},
			{"openUrlListCommand","notepad {i}"},
			{"IsVposStartTime","true"},
			{"afterConvertMode","0"},
			
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
			{"Width","715"},
			{"defaultControllerX","100"},
			{"defaultControllerY","100"},
			{"volume","50"},
			{"defaultCommentFormX","100"},
			{"defaultCommentFormY","100"},
			{"defaultCommentFormWidth","500"},
			{"defaultCommentFormHeight","520"},
			
			{"rokugaTourokuWidth","950"},
			{"rokugaTourokuHeight","500"},
			{"rokugaTourokuMaxRecordingNum","10"},
			{"rokugaTourokuQualityRank","0,1,2,3,4,5"},
		};

		var buf = new Dictionary<string,string>();
		foreach (var k in cfg.AppSettings.Settings.AllKeys) {
			buf.Add(k, cfg.AppSettings.Settings[k].Value);
		}
		
		cfg.AppSettings.Settings.Clear();
		foreach (var k in defaultConfig.Keys) {
			var v = (buf.ContainsKey(k)) ? buf[k] : defaultConfig[k];
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
//	private string[] defaultConfig = {};
}

}