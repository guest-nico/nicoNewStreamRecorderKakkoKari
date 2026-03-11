/*
 * Created by SharpDevelop.
 * User: kogak
 * Date: 2025/06/02
 * Time: 3:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace rokugaTouroku.info
{
	/// <summary>
	/// Description of AppSettingInfo.
	/// </summary>
	public class AppSettingInfo
	{
		public string baseDir = "";
		public string IscreateSubfolder = "true";
		public string subFolderNameType = "1";
		public string fileNameType = "1";
		public string filenameformat = "{Y}年{M}月{D}日{h}時{m}分{0}_{1}_{2}_{3}_{4}";
		public string ext = "";
		
		public bool isMin = false;
		public AppSettingInfo() {
			
		}
		public AppSettingInfo(string baseDir, string IscreateSubfolder, 
				string subFolderNameType, string fileNameType, 
				string filenameformat, string ext)
		{
			this.baseDir = baseDir;
			this.IscreateSubfolder = IscreateSubfolder;
			this.subFolderNameType = subFolderNameType;
			this.fileNameType = fileNameType;
			this.filenameformat = filenameformat;
			this.ext = ext;
		}
	}
}
