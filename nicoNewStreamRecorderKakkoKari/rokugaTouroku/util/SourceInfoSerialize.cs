/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/05/12
 * Time: 21:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SunokoLibrary.Application;
using System.Xml.Serialization;
using System.IO;


	/// <summary>
	/// Description of namaichiUtil.
	/// </summary>
public class SourceInfoSerialize
{
	public static void save(CookieSourceInfo si, bool isSub) {
//		var sio = new SourceInfoObject(si);
//		XmlSerializer serializer = new XmlSerializer(typeof(SourceInfoObject));
		XmlSerializer serializer = new XmlSerializer(typeof(CookieSourceInfo));
		
		var jarPath = util.getJarPath();
		try {
			var uri = (isSub) ? (jarPath[0] + "\\ニコ生新配信録画ツール（仮0.xml") :
				(jarPath[0] + "\\ニコ生新配信録画ツール（仮.xml");
			var sw = new System.IO.StreamWriter(uri, false, System.Text.Encoding.UTF8);
		
			serializer.Serialize(sw, si);
			sw.Close();
		} catch (Exception e) {
			util.debugWriteLine(e.Message + " " + e.StackTrace + " " + e.TargetSite);
		}
	}
	public static CookieSourceInfo load(bool isSub) {
//		var sio = new SourceInfoObject(si);
//		XmlSerializer serializer = new XmlSerializer(typeof(SourceInfoObject));
		XmlSerializer serializer = new XmlSerializer(typeof(CookieSourceInfo));
		
//		var sr = new System.IO.StreamReader(util.getJarPath()[1] + ".xml", System.Text.Encoding.UTF8);
//		var str = sr.ReadToEnd();
		
		bool IsCustomized = false;
		string BrowserName = "", ProfileName = "", CookiePath = "", EngineId = "";
		var x = new System.Xml.XmlDocument();
		var jarPath = util.getJarPath();
		try {
			var uri = (isSub) ? (jarPath[0] + "\\ニコ生新配信録画ツール（仮0.xml") :
				(jarPath[0] + "\\ニコ生新配信録画ツール（仮.xml");
			x.Load(uri);
		} catch (Exception) {
			return null;
		}
		foreach (System.Xml.XmlNode n in x.LastChild.ChildNodes) {
			util.debugWriteLine(n.Name + " " + n.InnerText);
			if (n.Name == "IsCustomized") IsCustomized = bool.Parse(n.InnerText);
			if (n.Name == "BrowserName") BrowserName = n.InnerText;
			if (n.Name == "ProfileName") ProfileName = n.InnerText;
			if (n.Name == "CookiePath") CookiePath = n.InnerText;
			if (n.Name == "EngineId") EngineId = n.InnerText;
		}
		return new CookieSourceInfo(BrowserName, 
				ProfileName, CookiePath, EngineId, IsCustomized);
		
//		return (CookieSourceInfo)serializer.Deserialize(sr);
//		sr.Close();
//		return null;
		
	}
	/*
	public static void ReadXml(CookieSourceInfo si)
        {
		var reader = System.Xml.XmlReader.Create(util.getJarPath()[1]);
            //?????????
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }
            //??
            reader.ReadStartElement();
            for (var i = 0; i < 5 && reader.NodeType != System.Xml.XmlNodeType.EndElement; )
            {
                var name = reader.Name;
                reader.Read();
                var value = reader.Value;
                reader.Read();
                reader.ReadEndElement();

                //5?for?i??????
                //??????????
                switch (name)
                {
                    case "IsCustomized":
                        si.IsCustomized = value == true.ToString();
                        i++;
                        break;
                    case "BrowserName":
                        si.BrowserName = value;
                        i++;
                        break;
                    case "ProfileName":
                        si.ProfileName = value;
                        i++;
                        break;
                    case "CookiePath":
                        si.CookiePath = value;
                        i++;
                        break;
                    case "EngineId":
                        si.EngineId = value;
                        i++;
                        break;
                }
            }
            reader.ReadEndElement();
        }
        */
}

/*
class SourceInfoObject {
	public string BrowserName;
	public string CookiePath;
	public string EngineId;
	public bool IsCustomized;
	public string ProfileName;
	
	public SourceInfoObject(CookieSourceInfo si) {
		BrowserName = si.BrowserName;
		CookiePath = si.CookiePath;
		EngineId = si.EngineId;
		IsCustomized = si.IsCustomized;
		ProfileName = si.ProfileName;
	}
}
*/