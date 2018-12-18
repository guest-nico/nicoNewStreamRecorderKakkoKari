/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/08/30
 * Time: 20:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace namaichi.info
{
	/// <summary>
	/// Description of WatchingInfo.
	/// </summary>
	public class WatchingInfo
	{
		public string hlsUrl;
		public string msUri;
		public string chatThread;
		public string chatKey;
		public string controlThread;
		public string controlKey;
		public string msVersion;
		public long expireIn = 10000;
		public DateTime roomSince;
		public string visit;
		public string comment;
		
		public WatchingInfo(string res)
		{
			var ver = System.Environment.OSVersion.Version;
			var isSecure = ver.Major >= 6 && ver.Minor >= 1;
			
			hlsUrl = util.getRegGroup(res, "streamServer\".+?\"url\":\"(.+?)\"");
			msUri = util.getRegGroup(res, "\"messageServer\".+?\"" + 
					((isSecure) ? "wss" : "ws") + "\"\\:\"(.+?)\"");
			chatThread = util.getRegGroup(res, "\"threads\".+?\"chat\"\\:\"(.+?)\"");
			chatKey = util.getRegGroup(res, "\"chatThreadKey\"\\:\"(.+?)\"");
			controlThread = util.getRegGroup(res, "\"threads\".+?\"control\"\\:\"(.+?)\"");
			controlKey = util.getRegGroup(res, "\"controlThreadKey\"\\:\"(.+?)\"");
			msVersion = util.getRegGroup(res, "\"version\"\\:(\\d+)");
			var _expireIn = util.getRegGroup(res, "\"expireIn\"\\:(\\d+)");
			if (_expireIn != null) expireIn = long.Parse(_expireIn);
			var _roomSince = util.getRegGroup(res, "\"room\".+?\"since\"\\:\"(.+?)\"");
			roomSince = DateTime.Parse(_roomSince);
			visit = util.getRegGroup(res, "\"statistics\".+?\"viewers\"\\:(\\d+)");
			comment = util.getRegGroup(res, "\"statistics\".+?\"comments\"\\:(\\d+)");
		}
		public string[] getMessageRequest(string userId, int resfrom) {
			var chat = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"" + chatThread + "\",\"version\":\"" + msVersion + "\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + resfrom + ",\"force_184\":\"0\",\"with_global\":1,\"scores\":1,\"nicoru\":0,\"threadkey\":\"" + chatKey + "\",\"service\":\"LIVE\"}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
			var control = "[{\"ping\":{\"content\":\"rs:1\"}},{\"ping\":{\"content\":\"ps:5\"}},{\"thread\":{\"thread\":\"" + controlThread + "\",\"version\":\"" + msVersion + "\",\"fork\":0,\"user_id\":\"" + userId + "\",\"res_from\":" + resfrom + ",\"force_184\":\"0\",\"with_global\":1,\"scores\":1,\"nicoru\":0,\"threadkey\":\"" + controlKey + "\",\"service\":\"LIVE\"}},{\"ping\":{\"content\":\"pf:5\"}},{\"ping\":{\"content\":\"rf:1\"}}]";
			return new string[]{chat, control};
		}
		public void setPutWatching(string res) {
			
			hlsUrl = util.getRegGroup(res, "streamServer\".+?\"url\":\"(.+?)\"");
			var _expireIn = util.getRegGroup(res, "\"expireIn\"\\:(\\d+)");
			if (_expireIn != null) expireIn = long.Parse(_expireIn);
			visit = util.getRegGroup(res, "\"statistics\".+?\"viewers\"\\:(\\d+)");
			comment = util.getRegGroup(res, "\"statistics\".+?\"comments\"\\:(\\d+)");
			util.debugWriteLine("setPutWatching hlsUrl " + hlsUrl);
		}
	}
}
