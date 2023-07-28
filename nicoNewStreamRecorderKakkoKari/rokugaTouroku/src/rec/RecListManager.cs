/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/09/21
 * Time: 0:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using namaichi.utility;
using Newtonsoft.Json;
using rokugaTouroku;
using rokugaTouroku.info;

namespace rokugaTouroku.rec
{
	/// <summary>
	/// Description of RecListManager.
	/// </summary>
	public class RecListManager
	{
		public MainForm form;
		public SortableBindingList<RecInfo> recListData;
		public config.config cfg;
		public RecDataGetter rdg;
		 
		public RecListManager(MainForm form, SortableBindingList<RecInfo> recListData, config.config cfg)
		{
			this.form = form;
			this.recListData = recListData;
			this.cfg = cfg;
		}
		public bool add(string t) {
			util.debugWriteLine("rlm add");
            
            var lvid = util.getRegGroup(t, "(lv\\d+(,\\d+)*)");
            var userId = util.getRegGroup(t, "user/(\\d+)");
            var chId = util.getRegGroup(t, "(ch\\d+)");
            var coId = util.getRegGroup(t, "(co\\d+)");
			//util.setLog(cfg, lv);
			
			var lvList = new List<string>();
			var url = "";
			if (lvid != null) {
				lvList.Add(lvid);
			} else if (userId != null) {
				lvList = getUserChannelStreamList(userId, "user");
				if (lvList == null) {
					util.showMessageBoxCenterForm(form, "ユーザーID" + userId + "の放送の取得に失敗しました");
					return false;
				}
				var r = util.showMessageBoxCenterForm(form, "ユーザーID" + userId + "のタイムシフトを登録しますか？ " + lvList.Count + "件", "", MessageBoxButtons.YesNo);
				if (r == DialogResult.No) return false;
			} else if (chId != null) {
				lvList = getUserChannelStreamList(chId, "channel");
				if (lvList == null) {
					util.showMessageBoxCenterForm(form, "チャンネルID" + chId + "の放送の取得に失敗しました");
					return false;
				}
				var r = util.showMessageBoxCenterForm(form, "チャンネルID" + chId + "のタイムシフトを登録しますか？ " + lvList.Count + "件", "", MessageBoxButtons.YesNo);
				if (r == DialogResult.No) return false;
			} else if (coId != null) {
				lvList = getCommunityStreamList(coId);
				if (lvList == null) {
					util.showMessageBoxCenterForm(form, "コミュニティID" + coId + "の放送の取得に失敗しました");
					return false;
				}
				var r = util.showMessageBoxCenterForm(form, "コミュニティID" + coId + "のタイムシフトを登録しますか？ " + lvList.Count + "件", "", MessageBoxButtons.YesNo);
				if (r == DialogResult.No) return false;
			} else {
				util.showMessageBoxCenterForm(form, "not found ID");
				return false;
			}
			//if (lvid != null) form.urlText.Text = "https://cas.nicovideo.jp/user/77252622/lv313508832";
			
			foreach (var _lv in lvList) {
				url = "https://live.nicovideo.jp/watch/" + _lv;
				try {
					if (bool.Parse(cfg.get("IsDuplicateConfirm"))) {
						var delList = new List<RecInfo>();
						foreach (RecInfo d in recListData)
							if (d.id == _lv) delList.Add(d);
						
						foreach (var _ri in delList) 
							if (util.showMessageBoxCenterForm(form, _ri.id + "はリスト内に含まれています。既にある行を削除しますか？\n[" + (String.IsNullOrEmpty(_ri.title) ? "タイトル未取得" : _ri.title) + "][" + _ri.state + "][" + _ri.quality + "] [" + _ri.timeShift + "]", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes) {
								form.deleteRow(_ri);
						}
						
					}
				} catch (Exception e){
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				}
				
				var rdg = new RecDataGetter(this);
				var ri = new RecInfo(_lv, url, rdg, form.afterConvertModeList.Text, form.setTsConfig, form.setTimeshiftBtn.Text, form.qualityBtn.Text, form.qualityRank, form.recCommmentList.Text, form.isChaseChkBox.Checked, (AccountInfo)form.accountBtn.Tag);
				Task.Run(() => ri.setHosoInfo(form));
				
				form.addList(ri);
			}
			
			return true;
				
		}
		public void record() {
			if (util.getRegGroup(form.urlText.Text, "(lv\\d+(,\\d+)*)") == null) {
				form.urlText.Text = "";
			} else {
				if (add(form.urlText.Text))
					form.urlText.Text = "";
			}
			
			if (rdg == null) {
				form.recBtn.Text = "録画停止";
				form.optionMenuItem.Enabled = false;
				Task.Run(() => {
					rdg = new RecDataGetter(this);
					rdg.rec();
					rdg = null;
					form.Invoke((MethodInvoker)delegate() {
		            	form.recBtn.Text = "録画開始";
		            	form.optionMenuItem.Enabled = true;
		            });
				});
			} else {
				rdg.stopRecording();
				rdg = null;
				form.recBtn.Text = "録画開始";
				form.optionMenuItem.Enabled = true;
			}
			
		}
		private List<string> getUserChannelStreamList(string id, string type) {
			try {
				var ret = new List<string>();
				for (var i = 0; i < (type == "user" ? 2 : 10); i++) {
					string url = null;
					Dictionary<string, string> h = new Dictionary<string, string>();
					if (i == 0) {
						url = "https://api.cas.nicovideo.jp/v2/tanzakus/" + type + "/" + id;
						h.Add("Content-Type", "application/json; charset=utf-8");
						h.Add("X-XSS-Protection", "1; mode=block");
						h.Add("Connection", "keep-alive");
					} else {
						url = "https://api.cas.nicovideo.jp/v2/tanzakus/" + type + "/" + id + "/content-groups/live/items?_offset=" + (10 + (i - 1) * 15) + "&_limit=15&sort=liveRecent_asc";
						h.Add("User-Agent", "nicocas-Android/3.24.0");
						var us = form.container.GetCookies(new Uri(url))["user_session"].Value;
						h.Add("Cookie", "user_session=" + us);
						h.Add("X-Frontend-Id", "90");
						h.Add("X-Frontend-Version", "3.24.0");
						h.Add("X-Os-Version", "22");
						h.Add("X-Model-Name", "greatlte");
						h.Add("X-Connection-Environment", "wifi");
					}
					var res = new Curl().getStr(url, h, CurlHttpVersion.CURL_HTTP_VERSION_1_1, "GET", null, false, false);
					var l = getTanzakuStreamLvList(res);
					if (l == null) break;
					foreach (var _l in l) {
						if (_l != null) ret.Add(_l);
					}
					if (l.IndexOf(null) > -1) break;
				}
				return ret;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		private List<string> getTanzakuStreamLvList(string res) {
			try {
				if (res.IndexOf("BAD_REQUEST") > -1) return null;
				var xml = JsonConvert.DeserializeXmlNode(res, "root");
				//xml.LoadXml(res);
				var items = xml.GetElementsByTagName("items");
				if (items == null || items.Count == 0) return null;
				var ret = new List<string>();
				foreach (XmlElement _i in items) {
					var _ts = _i.GetElementsByTagName("timeshift");
					if (_ts.Count == 0) continue;
					
					//foreach (XmlNode i in _i.ChildNodes) {
						var ts = _i.SelectSingleNode("timeshift");
						if (ts == null) continue;
						//var ts = i.Element("timeshift");
						if (ts.FirstChild.InnerText == "true" &&
						   	ts.LastChild.InnerText == "released")
							ret.Add(_i.SelectSingleNode("id").InnerText);
						if (ts.LastChild.InnerText == "expired")
							ret.Add(null);
					//}
				}
				return ret;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		private List<string> getCommunityStreamList(string id) {
			try {
				var _id = id.Substring(2);
				var ret = new List<string>();
				var total = -1;
				for (var i = 0; i < 10; i++) {
					string url = "https://com.nicovideo.jp/api/v1/communities/" + _id + "/lives.json?limit=30&offset=" + (i * 30);
					var res = util.getPageSource(url, form.container);
					if (res == null) {
						util.showMessageBoxCenterForm(form, "コミュニティID" + id + "の放送情報が取得できませんでした");
						return null;
					}
					if (total == -1) {
						var _total = util.getRegGroup(res, "\"total\":(\\d+)");
						if (_total == null) return null;
						total = int.Parse(_total);
					}
					var l = getCommunityJsonStreamLvList(res);
					if (l == null) break;
					foreach (var _l in l) {
						if (_l != null) ret.Add(_l);
					}
					if (l.IndexOf(null) > -1 || i * 30 + 30 > total) break;
				}
				return ret;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		private List<string> getCommunityJsonStreamLvList(string res) {
			try {
				var xml = JsonConvert.DeserializeXmlNode(res, "root");
				var id = xml.GetElementsByTagName("id");
				var status = xml.GetElementsByTagName("status");
				var canView = xml.GetElementsByTagName("can_view");
				var finishedAt = xml.GetElementsByTagName("finished_at");
				if (id.Count != status.Count - 1 || id.Count != canView.Count || id.Count != finishedAt.Count / 2) return null;
				var ret = new List<string>();
				for (var i = 0; i < id.Count; i++) {
					if (status[i + 1].InnerText == "ENDED" && canView[i].InnerText == "true")
						ret.Add(id[i].InnerText);
					var t = finishedAt[i * 2].InnerText;
					var _t = DateTime.Parse(t);
					if (DateTime.Parse(finishedAt[i * 2].InnerText) < DateTime.Now) {
						ret.Add(null);
						break;
					}
				}
				if (ret.Count > 1)
					util.debugWriteLine("ret count " + ret.Count);
				return ret;
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
	}
}
