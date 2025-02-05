/*
 * Created by SharpDevelop.
 * User: pc
 * Date: 2025/02/05
 * Time: 3:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using namaichi.utility;

namespace namaichi.info
{
	/// <summary>
	/// Description of DliveM3u8Info.
	/// </summary>
	public class M3u8Info
	{
		public string url = null;
		string header = null;
		List<SegmentInfo> urlList = new List<SegmentInfo>();
		string localUrl = null;
		public M3u8Info(string url, string r, string localUrl)
		{
			this.url = url;
			this.localUrl = localUrl;
			
			//r = new Regex("\"*(http.+?)\"*").Replace(r, localUrl + HttpUtility.UrlEncode("$1"));
			var extinfI = r.IndexOf("#EXTINF");
			if (extinfI == -1) header = r;
			header = r.Substring(0, extinfI);
			
			addUrl(r);
		}
		public void addUrl(string r) {
			var second = 6.0;
			var secondSum = 0.0;
			foreach (var l in r.Replace("\r", "").Split('\n')) {
				if (l.IndexOf("#EXT-X-ENDLIST") > -1) {
					urlList.Add(new SegmentInfo("#EXT-X-ENDLIST", true, secondSum, second, int.MaxValue));
					break;
				}
				var _second = util.getRegGroup(l, "^#EXTINF:([^,]+)");
				if (_second != null) {
					second = double.Parse(_second);
					
					continue;
				}
				var m = new Regex("(http://.+?(\\d+).cmf[^\"\'\\s]+)").Match(l);
				if (!m.Success) continue;
				var url = m.Groups[1].Value;
				var n = int.Parse(m.Groups[2].Value);
				if (l.IndexOf("%2finit") > -1) continue; //n = 0;
				if (urlList.Count == 0 || n > urlList[urlList.Count - 1].n) {
					urlList.Add(new SegmentInfo(url, false, secondSum, second, n));
					secondSum += second;
				}
				
				//test
				//if (urlList.Count > 10) 
				//	break;
			}
			
		}
		public string getRes(TimeShiftConfig tsConfig, bool isTimeShift) {
			var b = header + "\n";
			var l = urlList.ToList();
			foreach (var u in l) {
				if (u.url == "#EXT-X-ENDLIST") {
					b += u.url;
					break;
				}
				
				if (isTimeShift) {
					if (u.startTime + u.second < tsConfig.timeSeconds)
						continue;
					if (tsConfig.endTimeSeconds != 0 && u.startTime > tsConfig.endTimeSeconds) {
						b += "#EXT-X-ENDLIST\n";
						break;
					}
				}
				b += "#EXTINF:6.00000\n";
				b += u.url + "\n";
				
			}
			return b;
		}
		class SegmentInfo {
			public string url = null;
			public bool isExtXEndlist = false;
			public double startTime = 0;
			public double second;
			public int n = 0;
			public SegmentInfo(string url, 
					bool isExtXEndlist, double startTime, 
					double second, int n) {
				this.url = url;
				this.isExtXEndlist = isExtXEndlist;
				this.startTime = startTime;
				this.second = second;
				this.n = n;
			}
		}
	}
}

//audioとvideoのm3u8の最初にinit00001がセグメントとして記述