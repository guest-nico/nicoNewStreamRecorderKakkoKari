/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2019/01/15
 * Time: 20:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

namespace namaichi.rec
{
	/// <summary>
	/// Description of HosoInfoGetter.
	/// </summary>
	public class HosoInfoGetter
	{
		public string communityId = null;
		public string userId = null;
		public string type = null;
		public string title = null;
		public DateTime dt = DateTime.MinValue;
		public string description = null;
		public string[] tags = null;
		public string thumbnail = null;
		public string group = null;
		public string userName = null; 
		public HosoInfoGetter()
		{
		}
		public bool get(string _url) {
			var lvid = util.getRegGroup(_url, "((lv|c[oh])\\d+)");
			var url =  "https://live.nicovideo.jp/watch/" + lvid;
			//var res = util.getPageSource(url, null);
			var res = util.getPageSourceCurl(url, null, null);
			if (res == null) return false;
			
			title = util.getRegGroup(res, "<meta property=\"og:title\" content=\"(.*?)\"");
			type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
			thumbnail = getThumbnail(res);
			var _dt = util.getRegGroup(res, "(\\d{4}/\\d{1,2}/\\d{1,2}.{0,10}\\d{1,2}:\\d{1,2}(:\\d{1,2})*)");
			if (_dt != null) {
				util.debugWriteLine("dt0 " + _dt);
				_dt = util.getRegGroup(_dt, "(\\d{4}/\\d{1,2}/\\d{1,2})") + " " + util.getRegGroup(_dt, "(\\d{4}:\\d{1,2}(:\\d{1,2})*)");
				util.debugWriteLine("dt1 " + _dt);
				dt = DateTime.Parse(_dt);
			} else {
//				util.debugWriteLine("not dt res " + res);
			}
			var isJikken = res.IndexOf("siteId&quot;:&quot;nicocas") > -1;
			var ret = false;
			if (isJikken) {
				ret = setJikkenInfo(res);
				
			} else {
				ret = setNicoLiveInfo(res);
				
			}
			if (description != null) description = description.Trim(new char[]{'\n', '\r', ' ', '\t'});
			return ret;
		}
		private bool setJikkenInfo(string res) {
			//userId = util.getRegGroup(res, "\"user\"\\:\\{\"user_id\"\\:(.+?),");
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			data = System.Web.HttpUtility.HtmlDecode(data);
			
			
			if (type == "official") {
				communityId = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
//				if (communityId == null) communityId = null;
				userId = null;

			} else {
				
				communityId = util.getRegGroup(data, "\"community\"\\:\\{\"id\"\\:\"(.+?)\"");
				userId = util.getRegGroup(data, "\"broadcaster\"..\"id\".\"(.+?)\"");
				 
			}
			description = util.getRegGroup(data, "\"description\":\"([\\s\\S]*?[^\\\\])\"");
			
			tags = getTag(data);
			return communityId != null && userId != null;
		}
		private bool setNicoLiveInfo(string res) {
			var pageType = util.getPageType(res);
			var data = util.getRegGroup(res, "<script id=\"embedded-data\" data-props=\"([\\d\\D]+?)</script>");
			var isRtmpOnlyPage = res.IndexOf("%3Cgetplayerstatus%20") > -1;
			data = (isRtmpOnlyPage) ? System.Web.HttpUtility.UrlDecode(res) :
						System.Web.HttpUtility.HtmlDecode(data);
			if (data == null) data = System.Web.HttpUtility.HtmlDecode(res);
			type = util.getRegGroup(res, "\"content_type\":\"(.+?)\"");
			if (type == null) type = util.getRegGroup(res, "content_type = '(.+?)'");
			group = util.getRegGroup(data, "\"socialGroup\".+?\"name\".\"(.+?)\"");
			if (group == null) group = util.getRegGroup(res, "COMMUNITY INFO[\\s\\S]+target=\"_blank\"><span itemprop=\"name\">(.+?)</span>");
			util.debugWriteLine(pageType);
			if ((pageType == 0 || pageType == 7 && isRtmpOnlyPage) || true) {
				if (type == "official") {
					communityId = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
//					if (communityId == null) communityId = "official";
					userId = null;
//					communityId = null;
					
				} else {
					var isChannel = util.getRegGroup(data, "visualProviderType\":\"(channel)\",\"title\"") != null;
					communityId = util.getRegGroup(data, "\"socialGroup\".+?\"id\".\"(.+?)\"");
					userId = (isChannel) ? null : (util.getRegGroup(data, "supplier\":{\"name\".+?pageUrl\":\"http[s]*://www.nicovideo.jp/user/(\\d+?)\""));
					userName = util.getRegGroup(data, "supplier\":{\"name\":\"(.+?)\"");
				}
				description = util.getRegGroup(data, "\"description\":\"(.*?[^\\\\])\",");
				
			} else {
				var isChannel = type == "channel";
				if (!isChannel)
					userId = util.getRegGroup(res, "<a href=\"https*://www.nicovideo.jp/user/(\\d+)\" target=\"_blank\">");
				else {
					userName = util.getRegGroup(res, "（提供:<strong><span itemprop=\"name\">(.+?)</span></strong>）");
				}
				communityId = util.getRegGroup(res, "<a href=\"http.+?/channel/(ch\\d+)\" target=\"_blank\">");
				if (communityId == null) communityId = util.getRegGroup(res, "<a href=\"http.+?/community/(co\\d+)\" target=\"_blank\">");
				
				if (type == "official")
					description = util.getRegGroup(res, "description:.*?['\"]([\\s\\S]*?[^\\\\])'");
				else 
					description = util.getRegGroup(res, "<div class=\"gate_description_area\">([\\s\\S]*?)(<div|</div>)");
				
			}
			tags = getTag(data);
			return communityId != null || userId != null || title != null;
		}
		private string[] getTag(string data) {
			var _t = util.getRegGroup(data, "\"tag\":\\{\"list\":\\[(.+?)\\]");
			MatchCollection m;
			if (_t == null) {
				m = new Regex("keyword=(.+?)&amp").Matches(data);
				
				if (m.Count == 0)
					m = Regex.Matches(data, "<a class=\"nicopedia\" rel=\"tag\".+?>([\\s\\S]*?)</a>");
				
			} else if ((m = Regex.Matches(data, "\"text\":\"(.*?)\"")) != null) {
				
			}
			
			var ret = new List<string>();
			var trimChar = new char[]{'\n', '\r', ' ', '\t'};
			foreach (Match _m in m) {
				//if (ret != "") ret += ",";
				var mStr = _m.Groups[1].Value.Trim(trimChar);
				ret.Add(mStr);
			}
			return ret.ToArray();
		}
		private string getThumbnail(string res) {
			return util.getRegGroup(res, "<meta property=\"og:image\" content=\"(.+?)\"");
		}
	}
}
