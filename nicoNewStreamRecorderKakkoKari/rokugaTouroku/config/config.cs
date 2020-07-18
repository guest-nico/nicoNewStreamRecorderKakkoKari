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
using System.IO;

namespace rokugaTouroku.config {
/// <summary>
/// Description of config.
/// </summary>
public class config
{
	private Configuration cfg;
	public string brokenCopyFile = null;
	
	public config()
	{
		cfg = getConfig();
		defaultMergeFile();
        
 	}
	public Configuration getConfig() {
		for (var i = 0; i < 5; i++) {
			try {
				var jarPath = util.getJarPath();
				var configFile = jarPath[0] + "\\ニコ生新配信録画ツール（仮.config";
				if (i > 3) System.IO.File.Delete(configFile);
				//util.debugWriteLine(configFile);
		        var exeFileMap = new System.Configuration. ExeConfigurationFileMap { ExeConfigFilename = configFile };
		        var cfg     = ConfigurationManager.OpenMappedExeConfiguration(exeFileMap, ConfigurationUserLevel.None);
		        return cfg;
			} catch (Exception e) {
				util.debugWriteLine("getconfig " + e.Message + " " + e.StackTrace + " " + e.TargetSite);
				if (e.Message.IndexOf("レベルのデータ") > -1) break;
				Thread.Sleep(3000);
				continue;
				//ルート レベルのデータが無効です。
			}
		}
		resetConfig();
		return this.cfg;
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
		try {
			if (key != "accountId" && key != "accountPass" &&
			   		key != "user_session" && key != "user_session_secure") {
				util.debugWriteLine(key + " " + cfg.AppSettings.Settings[key].Value);
			}
		} catch (Exception e) {
			util.debugWriteLine("config get exception " + key + " " + e.Message + e.Source + e.StackTrace + e.TargetSite);
			return null;
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
			
			{"IsHokan","true"},
			{"accountId2",""},
			{"accountPass2",""},
			{"user_session2",""},
			{"user_session_secure2",""},
			{"browserNum2","1"},
			{"issecondlogin2","false"},
			{"cookieFile2",""},
			{"iscookie2","false"},
			{"IsBrowserShowAll","false"},
			
			{"useProxy","false"},
			{"proxyAddress",""},
			{"proxyPort",""},
			
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
			{"IsgetcommentXmlInfo","false"},
			{"IsgetcommentStore","false"},
			{"IsCommentConvertSpace","false"},
			{"IsSaveCommentOnlyRetryingRec","false"},
			{"IsDisplayComment","true"},
			{"IstitlebarSamune","true"},
			{"IsautoFollowComgen","false"},
			{"qualityRank","0,1,2,3,4"},
			{"IsMiniStart","false"},
			{"IsConfirmCloseMsgBox","true"},
			{"IsLogFile","false"},
			{"IsSegmentNukeInfo","true"},
			{"IsNotSleep","false"},
			{"IsRestoreLocation","false"},
			
			{"segmentSaveType","0"},
			{"IsRenketuAfter","true"},
//			{"IsAfterRenketuFFmpeg","false"},
			{"EngineMode","0"},
			{"anotherEngineCommand",""},
			{"IsDefaultRtmpPath","true"},
			{"rtmpPath",""},
			{"latency","3.0"},
			{"IsChaseRecord","false"},
			{"IsArgChaseRecFromFirst","false"},
			{"IsOnlyTimeShiftChase","true"},
			{"IsChaseReserveRec","false"},
			{"IsUsePlayer","true"},
			{"IsDefaultPlayer","true"},
			{"IsUseCommentViewer","true"},
			{"IsDefaultCommentViewer","true"},
			{"anotherPlayerPath",""},
			{"anotherCommentViewerPath",""},
			{"Is184","true"},
			{"playerArgs",""},
			
			{"tsStartTimeMode","0"},
			{"tsEndTimeMode","0"},
			{"tsStartSecond","0"},
			{"tsEndSecond","0"},
			{"tsBaseOpenTimeStart","false"},
			{"tsBaseOpenTimeEnd","false"},
			{"tsIsRenketu","false"},
			{"IsVposStartTime","true"},
			{"IsAfterStartTimeComment","false"},
			
			{"IsUrlList","false"},
			{"IsM3u8List","false"},
			{"M3u8UpdateSeconds","5"},
			{"IsOpenUrlList","false"},
			{"openUrlListCommand","notepad {i}"},
			{"afterConvertMode","0"},
			{"IsSoundEnd","false"},
			{"soundPath",""},
			{"IsSoundDefault","true"},
			{"soundVolume","50"},
			
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
			{"X",""},
			{"Y",""},
			{"defaultControllerX","100"},
			{"defaultControllerY","100"},
			{"volume","50"},
			{"defaultCommentFormX","100"},
			{"defaultCommentFormY","100"},
			{"defaultCommentFormWidth","500"},
			{"defaultCommentFormHeight","520"},
			
			{"rokugaTourokuWidth","950"},
			{"rokugaTourokuHeight","500"},
			{"rokugaTourokuX",""},
			{"rokugaTourokuY",""},
			{"rokugaTourokuMaxRecordingNum","10"},
			{"IsDuplicateConfirm","false"},
			{"rokugaTourokuQualityRank","0,1,2,3,4"},
			
			{"recBackColor","-1"},
			{"recForeColor","-16777216"},
			{"recLinkColor","-16776961"},
			{"tourokuBackColor","-986896"},
			{"tourokuForeColor","-16777216"},
		};
		try {
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
		} catch (Exception e) {
			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
			resetConfig();
		}
		
		
		// Dictionary<string, string>
	}
	public void saveFromForm(Dictionary<string, string> formData) {
		cfg = getConfig();
		
		foreach (var k in formData.Keys) {
			util.debugWriteLine(k + formData[k]);
			try {
				cfg.AppSettings.Settings[k].Value = formData[k];
			} catch (Exception e) {
				util.debugWriteLine("config set exception name " + k + " data " + formData[k] + " " + e.Message + e.TargetSite + e.StackTrace + e.Source);
			}
		}		
		try {
			cfg.Save();
		} catch (Exception e) {
			util.debugWriteLine(e.Message + " " + e.StackTrace);
		}
	}
	private bool resetConfig() {
		var jarPath = util.getJarPath();
		var configFile = jarPath[0] + "\\ニコ生新配信録画ツール（仮.config";
		if (File.Exists(configFile)) {
			var n = DateTime.Now;
			var fn = jarPath[0] + "\\" + n.ToString("yyyyMMddhhmmss") + "ニコ生新配信録画ツール（仮.config";
			try {
				File.Copy(configFile, fn);
				File.Delete(configFile);
				brokenCopyFile = fn;
				cfg = getConfig();
				defaultMergeFile();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				try {
					for (var i = 0; i < 10000; i++) {
						fn = configFile + i.ToString();
						if (File.Exists(fn)) continue;
						File.Copy(configFile, fn);
						File.Delete(configFile);
						brokenCopyFile = fn;
						cfg = getConfig();
						defaultMergeFile();
						break;
					}
				} catch (Exception ee) {
					util.debugWriteLine(ee.Message + ee.Source + ee.StackTrace + ee.TargetSite);
					return false;
				}
			}
		}
		return true;
	}
//	private string[] defaultConfig = {};
}

}