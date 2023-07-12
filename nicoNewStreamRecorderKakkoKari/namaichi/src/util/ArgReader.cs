/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/29
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using namaichi.config;
using namaichi.info;

namespace namaichi.utility
{
	/// <summary>
	/// Description of ArgReader.
	/// </summary>
	public class ArgReader
	{
		private string[] args;
		private MainForm form;
		public bool isConcatMode = false;
		public string allPathStr;
		public Dictionary<string, string> argConfig = new Dictionary<string, string>();
		public string lvid = null;
		public config.config config;
		public TimeShiftConfig tsConfig;
		public bool isPlayMode = false;
		public AccountInfo ai = null;
		
		public ArgReader(string[] args, config.config config, MainForm form)
		{
			this.args = args;
			this.config = config;
			this.form = form;
		}
		public void read() {
			try {
				if (isAllPath()) {
					isConcatMode = true;
					return;
				}
				setArgConfig();
				util.debugWriteLine("args " + args.Length + " " + string.Join(" ", args));
				foreach(var a in argConfig) util.debugWriteLine(a.Key + " " + a.Value);
				
				foreach (var arg in args) {
					var _a = arg.ToLower();
					if (_a == "-play") isPlayMode = true;
					if (_a == "-chase") form.isChaseChkBtn.Checked = true;
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				form.addLogText("引数の読み込みに失敗しました");
				form.addLogText("引数:" + string.Join(" ", args));
				form.addLogText(e.Message + e.Source + e.StackTrace);
			}
		}
		private bool isAllPath() {
			var isAllPath = true;
			foreach (var a in args) {
				try {
					if (!File.Exists(a) && !Directory.Exists(a)) isAllPath = false;
				} catch (Exception e) {
					isAllPath = false;
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
			}
			if (isAllPath) allPathStr = string.Join("|", args);
			return isAllPath;
		}
		private void setArgConfig() {
			var lowKeys = new List<string>(config.defaultConfig.Keys.Select((x) => x.ToLower()));
			var values = config.defaultConfig.Values.ToList<string>();
			var keys = config.defaultConfig.Keys.ToList();
			lowKeys.AddRange(new string[] {"ts-start", "ts-end",
			                 		//"ts-list", "ts-list-update", "ts-list-command", "ts-list-open", "ts-list-m3u8", 
			                 		"ts-vpos-starttime", "ts-starttime-comment", "ts-endtime-comment", "ts-starttime-open", "ts-endtime-open", "ts-endtime-delete-pos", "accountsetting"});
			foreach (var a in args) {
				if (a.StartsWith("-")) {
					try {
						var name = util.getRegGroup(a, "-(.*)=");
						var val = util.getRegGroup(a, "=(.*)");
						if (name == null) continue;
						
						string setVal = null;
						string setName = null;
						if (!isValidConf(name, val, lowKeys, values, out setVal, out setName, keys)) continue;
						//argConfig.Add(setName, setVal);
						argConfig[setName] = setVal;
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace);
						form.addLogText("引数の読み取りに失敗しました " + a + " " + e.Message + e.Source + e.StackTrace);
					}
				} else {
					if (lvid == null) lvid = util.getRegGroup(a, "(lv\\d+(,\\d+)*)");
				}
			}
		}
		private bool isValidConf(string name, string val, List<string> lowKeys, List<string> defValues, out string setVal, out string setName, List<string> keys) {
			setVal = null;
			setName = null;
			for (var i = 0; i < lowKeys.Count; i++) {
				if (name.ToLower() != lowKeys[i]) continue;
				if (i < defValues.Count && (defValues[i] == "true" || defValues[i] == "false")) {
				    if (val.ToLower() == "true" || val.ToLower() == "false") {
						setVal = val.ToLower();
						setName = keys[i];
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "browsernum") {
					if (val == "1" || val == "2") {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(1 or 2) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "qualityrank") {
					if (isValidQualityRank(val)) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						var rankStr = "";
						for (var j = 0; j < namaichi.config.config.qualityList.Count; j++)
							rankStr += (rankStr != "" ? "," : "") + j.ToString();
						form.addLogText(name + "の値が設定できませんでした(例 「" + rankStr + "」) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "segmentsavetype") {
					if (val == "0" || val == "1") {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0 or 1) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "m3u8updateseconds") {
					double _s = 0;
					if (double.TryParse(val, out _s) && _s > 0) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0以上の数値) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "subfoldernametype") {
					int _s = 0;
					if (int.TryParse(val, out _s) && _s >= 1 && _s <= 8) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(1から8の整数) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "filenametype") {
					int _s = 0;
					if (int.TryParse(val, out _s) && _s >= 1 && _s <= 10) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(1から10の整数) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "filenameformat") {
					if (val.IndexOf("{0}") > -1) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした({0}を含む文字列) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "volume") {
					int _s = 0;
					if (int.TryParse(val, out _s) && _s >= 0 && _s <= 100) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0から100の整数) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "afterConvertMode") {
					int _s = 0;
					if (int.TryParse(val, out _s) && _s >= 0 && _s <= 15) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0から15の整数) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "EngineMode") {
					int _s = 0;
					if (int.TryParse(val, out _s) && _s >= 0 && _s <= 3) {
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0から3の整数) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "soundvolume") {
					int _s = 0;
					if (int.TryParse(val, out _s)) {
						if (_s < 0) val = "0";
						if (_s > 100) val = "100";
						setVal = val;
						setName = keys[i];
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0から100の整数) " + val, false);
						return false;
					}
				}
				
				//ts
				if (lowKeys[i] == "ts-start") {
					var _t = Regex.Match(val.ToLower(), "((-*\\d*)h)*((-*\\d*)m)*((-*\\d*)s)*");
					if (_t.Length != 0) {
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						
						tsConfig.timeType = 0;
						tsConfig.timeSeconds = 0;
						if (_t.Groups[2].Length != 0) tsConfig.timeSeconds += int.Parse(_t.Groups[2].Value) * 3600;
						if (_t.Groups[4].Length != 0) tsConfig.timeSeconds += int.Parse(_t.Groups[4].Value) * 60;
						if (_t.Groups[6].Length != 0) tsConfig.timeSeconds += int.Parse(_t.Groups[6].Value);
						return false;
					} else if (val.ToLower() == "continue" || 
				    		val.ToLower() == "continue-concat") {
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						
						tsConfig.timeType = 1;
						tsConfig.isContinueConcat = val.ToLower() == "continue-concat";  
						return false;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0h0m0s形式の時間 or continue or continue-concat) " + val, false);
						return false;
					}
				}
				if (lowKeys[i] == "ts-end") {
					var _t = Regex.Match(val.ToLower(), "((-*\\d*)h)*((-*\\d*)m)*((-*\\d*)s)*");
					if (_t.Length != 0) {
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						
						tsConfig.endTimeSeconds = 0;
						if (_t.Groups[2].Length != 0) tsConfig.endTimeSeconds += int.Parse(_t.Groups[2].Value) * 3600;
						if (_t.Groups[4].Length != 0) tsConfig.endTimeSeconds += int.Parse(_t.Groups[4].Value) * 60;
						if (_t.Groups[6].Length != 0) tsConfig.endTimeSeconds += int.Parse(_t.Groups[6].Value);
						return false;
					} else {
						form.addLogText(name + "の値が設定できませんでした(0h0m0s形式の時間) " + val, false);
						return false;
					}
				}
				/*
				//ts list
				if (lowKeys[i] == "ts-list") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "IsUrlList";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isOutputUrlList = bool.Parse(val);
						return true;
					} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-list-open") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "IsOpenUrlList";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isOpenUrlList = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-list-m3u8") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "IsM3u8List";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isM3u8List = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-list-update") {
					double _s = 0;
					if (double.TryParse(val, out _s) && _s > 0) {
						setName = "M3u8UpdateSeconds";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.m3u8UpdateSeconds = double.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(0以上の数値) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-list-command") {
//					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "openUrlListCommand";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.openListCommand = val;
						return true;
				   	//} else {
					//	form.addLogText(name + "の値が設定できませんでした(0以上の数値) " + val, false);
					//	return false;;
					//}
				}
				*/
				if (lowKeys[i] == "ts-vpos-starttime") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "IsVposStartTime";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isVposStartTime = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-starttime-comment") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "IsAfterStartTimeComment";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isAfterStartTimeComment = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-endtime-comment") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "IsBeforeEndTimeComment";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isBeforeEndTimeComment = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-starttime-open") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "tsBaseOpenTimeStart";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isOpenTimeBaseStartArg = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-endtime-open") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "tsBaseOpenTimeEnd";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isOpenTimeBaseEndArg = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				if (lowKeys[i] == "ts-endtime-delete-pos") {
					if (val.ToLower() == "true" || val.ToLower() == "false") {
						setName = "tsIsDeletePosTime";
						setVal = val;
						if (tsConfig == null) tsConfig = new TimeShiftConfig();
						tsConfig.isDeletePosTime = bool.Parse(val);
						return true;
				   	} else {
						form.addLogText(name + "の値が設定できませんでした(true or false) " + val, false);
						return false;;
					}
				}
				
				if (lowKeys[i] == "accountsetting") {
					if (val.IndexOf("xml versio") > -1) val = util.getRegGroup(val, "(<AccountSe.+)");
					var ai = AccountInfo.fromJsonArg(val);
					if (ai == null) {
						form.addLogText(name + "の値が設定できませんでした " + val, false);
						return false;
					} else {
						this.ai = ai;
						return false;
					}
				}
				setName = keys[i];
				setVal = val;
				return true;
			}
			return false;
			    
		}
		private bool isValidQualityRank(string val) {
			try {
				var l = val.Split(',').Select((x) => int.Parse(x)).ToList();
				//if (l.Count() != namaichi.config.config.qualityList.Count) return false;
				var a = new List<int>();
				for (var i = 0; i < namaichi.config.config.qualityList.Count; i++)
					a.Add(i);
				foreach (var _l in l) a.Remove(_l);
				//return a.Count == 0;
				return a.Count != namaichi.config.config.qualityList.Count;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return false;
			}
		}

	}
}
