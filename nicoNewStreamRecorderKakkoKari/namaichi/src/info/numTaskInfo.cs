/*
 * Created by SharpDevelop.
 * User: user
 * Date: 2018/10/12
 * Time: 0:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace namaichi.info
{
	/// <summary>
	/// Description of numTaskInfo.
	/// </summary>
	public class numTaskInfo {
		public int no = -1;
		public string url = null;
		public byte[] res = null;
		public double second = 0;
		public string fileName = null;
		public DateTime dt;
		public int originNo = -1;
		public double startSecond = -1;
		public numTaskInfo(int no, string url, double second, string fileName, double startSecond, int originNo = -1) {
			this.no = no;
			this.url = url;
			this.second = second;
			this.fileName = fileName;
			dt = DateTime.Now;
			this.originNo = (originNo == -1) ? no : originNo;
			this.startSecond = startSecond;
		}
	}
}
