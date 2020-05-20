/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/04/21
 * Time: 21:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml.Linq;
using System.Xml;
	
namespace namaichi.info
{
	/// <summary>
	/// Description of ChatInfo.
	/// </summary>
	public class ChatInfo
	{
		private XDocument xml;
		public string json;
		public string contents = "";
		public string premium;
		public string root;
		public long serverTime;
		public int date;
		public long date_usec;
		public long vpos;
		public string userId;
		public string score = "0";
		public string ticket;
		public string lastRes;
		public bool isPingRf;
		public long vposOriginal;
		public int no = -1;
		
		public ChatInfo(XDocument xml, string json)
		{
			this.xml = xml;
			this.json =json;
		}
		public ChatInfo(XDocument xml)
		{
			this.xml = xml;
		}
		public XDocument getFormatXml(long serverTime, bool isOriginalVposBase = false, long vposStartTime = -1) {
			this.serverTime = serverTime;
			//xml.Root
//			util.debugWriteLine(xml.Root);
			var _xml = new XDocument();
			_xml.Add(new XElement(xml.Root.Name));
			root = xml.Root.Name.ToString();
//			util.debugWriteLine(xml.Root.Name);
			
//			var atts = _xml.Root.Attributes();
//			Object[] o = new Object[20];
			
			date = 0;
			
//			foreach(var a in xml.Root.Elements()) {
//				util.debugWriteLine(a.Name + a.Value);
//			}
			
			foreach (XElement e in xml.Root.Elements()) {
//				util.debugWriteLine(xml.Root);
//				o[0] = new XAttribute(e.Name, e.Value);
//				_xml.Root.SetAttributeValue(e.Name, e.Value);
				if ((root == "chat" && e.Name == "content") ||
				    (root == "control" && e.Name == "text") ||
				    (root == "ping" && e.Name == "content")) {
					_xml.Root.Add(e.Value);
					contents = e.Value;
				} else _xml.Root.SetAttributeValue(e.Name, e.Value);
				if (e.Name == "premium") premium = e.Value;
				if (e.Name == "server_time") 
					this.serverTime = int.Parse(e.Value);
				if (e.Name == "date") date = int.Parse(e.Value);
//				_xml.Root.Add(new XAttribute(e.Name, e.Value));
				if (e.Name == "date_usec") date_usec = int.Parse(e.Value);
				if (e.Name == "vpos") vpos = long.Parse(e.Value);
				if (e.Name == "user_id") userId = e.Value;
				if (e.Name == "score") score = e.Value;
				if (e.Name == "ticket") ticket = e.Value;
				if (e.Name == "last_res") lastRes = e.Value;
				if (e.Name == "no") no = int.Parse(e.Value);
			}
			
			if (root == "chat" || root == "control") {
				if (!isOriginalVposBase) {
					vposOriginal = vpos = (date - serverTime) * 100;
				} else {
					vposOriginal = vpos = vpos - vposStartTime * 100;
				}
				if (vpos < 0) vpos = 0;
				_xml.Root.SetAttributeValue("vpos", vpos);
			}
			
			//コメント再生用
			if (root == "control") _xml.Root.Name = "chat";
			var thread = _xml.Root.Attribute("thread");
			if (thread != null) 
				_xml.Root.SetAttributeValue("thread", util.getRegGroup(thread.Value, "(\\d+)"));
				
//			_xml.Add(new XElement("ele", o));
//			http://live2.nicovideo.jp/watch/lv312502201?ref=top&zroute=index&kind=top_onair&row=3
//			util.debugWriteLine(_xml);
			
			return _xml;
		}
		public void getFromXml(long serverTime) {
			this.serverTime = serverTime;
			root = xml.Root.Name.ToString();
			date = 0;
			
			var _xml = new XDocument();
			_xml.Add(new XElement(xml.Root.Name));
			
			var a = xml.Root.Attributes();
			contents = xml.Element(root).Value;
			foreach (var e in xml.Root.Attributes()) {
//				util.debugWriteLine(xml.Root);
//				o[0] = new XAttribute(e.Name, e.Value);
//				_xml.Root.SetAttributeValue(e.Name, e.Value);
				if ((root == "chat" && e.Name == "content") ||
				    (root == "control" && e.Name == "text") ||
				    (root == "ping" && e.Name == "content")) {
					_xml.Root.Add(e.Value);
					contents = e.Value;
				} else _xml.Root.SetAttributeValue(e.Name, e.Value);
				if (e.Name == "premium") premium = e.Value;
				if (e.Name == "server_time") 
					this.serverTime = int.Parse(e.Value);
				if (e.Name == "date") date = int.Parse(e.Value);
//				_xml.Root.Add(new XAttribute(e.Name, e.Value));
				if (e.Name == "date_usec") date_usec = int.Parse(e.Value);
//				if (e.Name == "vpos") vpos = long.Parse(e.Value);
				if (e.Name == "user_id") userId = e.Value;
				if (e.Name == "score") score = e.Value;
				if (e.Name == "ticket") ticket = e.Value;
				if (e.Name == "last_res") lastRes = e.Value;
			}
			
			if (root == "chat" || root == "control") {
				vposOriginal = vpos = (date - serverTime) * 100;
				if (vpos < 0) vpos = 0;
				_xml.Root.SetAttributeValue("vpos", vpos);
			}
			
			//コメント再生用
			if (root == "control") _xml.Root.Name = "chat";
			var thread = _xml.Root.Attribute("thread");
			if (thread != null) 
				_xml.Root.SetAttributeValue("thread", util.getRegGroup(thread.Value, "(\\d+)"));
				
//			_xml.Add(new XElement("ele", o));
//			http://live2.nicovideo.jp/watch/lv312502201?ref=top&zroute=index&kind=top_onair&row=3
//			util.debugWriteLine(_xml);
			
			xml = _xml;
		}
	}
}
