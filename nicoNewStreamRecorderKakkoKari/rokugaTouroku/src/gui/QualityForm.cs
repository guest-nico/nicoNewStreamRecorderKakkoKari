/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/10/04
 * Time: 4:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of QualityForm.
	/// </summary>
	public partial class QualityForm : Form
	{
		public string ret = null;
		public string qualityStr = null;
		public QualityForm(string qualityRank, int fontSize)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			setInitQualityRankList(qualityRank);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			util.setFontSize(fontSize, this, false);
		}
		void highRankBtn_Click(object sender, EventArgs e)
		{
			List<int> ranks = new List<int>() {7,6,8,0,1,2,3,4,5,9};
			for (var i = ranks.Count; i < config.config.qualityList.Count; i++)
				ranks.Add(i);
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
		}
		void lowRankBtn_Click(object sender, EventArgs e)
		{
			List<int> ranks = new List<int>() {9, 5, 4, 3, 2, 1, 0, 8, 6, 7};
			for (var i = ranks.Count; i < config.config.qualityList.Count; i++)
				ranks.Add(i);
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
		}
		public object[] getRanksToItems(int[] ranks, ListBox owner) {
			var items = config.config.qualityList;
			/*
			var items = new Dictionary<int, string> {
				//{0, "自動(abr)"}, 
				{0, "3Mbps(super_high)"},
				{1, "2Mbps(high)"}, {2, "1Mbps(normal)"},
				{3, "384kbps(low)"}, {4, "192kbps(super_low)"},
				{5, "音声のみ(audio_high)"}, {6, "6Mbps(6Mbps1080p30fps)"} 
			};
			*/
//			var ret = new ListBox.ObjectCollection(owner);
			var ret = new List<object>();
			for (int i = 0; i < ranks.Length; i++) {
				util.debugWriteLine(ranks[i] + " ");
				ret.Add((i + 1) + ". " + items[ranks[i]]);
			}
			foreach (var k in items.Keys)
				if (Array.IndexOf(ranks, k) == -1)
					ret.Add((ret.Count + 1) + ". " + items[k]);
			return ret.ToArray();
		}
		void UpBtn_Click(object sender, EventArgs e)
		{
			var selectedIndex = qualityListBox.SelectedIndex;
			if (selectedIndex < 1) return;
			
			var ranks = getItemsToRanks(qualityListBox.Items);
			var selectedVal = ranks[selectedIndex + 0];
			ranks.RemoveAt(selectedIndex);
			var addIndex = (selectedIndex == 0) ? 0 : (selectedIndex - 1);
			ranks.Insert(addIndex, selectedVal);
			
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
			qualityListBox.SetSelected(addIndex, true);
		}
		void DownBtn_Click(object sender, EventArgs e)
		{
			var selectedIndex = qualityListBox.SelectedIndex;
			var itemCount = qualityListBox.Items.Count;
			if (selectedIndex > itemCount - 2) return;
			
			var ranks = getItemsToRanks(qualityListBox.Items);
			var selectedVal = ranks[selectedIndex + 0];
			ranks.RemoveAt(selectedIndex);
			var addIndex = (selectedIndex == itemCount) ? itemCount : (selectedIndex + 1);
			ranks.Insert(addIndex, selectedVal);
			
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
			qualityListBox.SetSelected(addIndex, true);
		}
		List<int> getItemsToRanks(ListBox.ObjectCollection items) {
			var itemsDic = config.config.qualityList;
			/*
			var itemsDic = new Dictionary<int, string> {
				//{0, "自動(abr)"}, 
				{0, "3Mbps(super_high)"},
				{1, "2Mbps(high)"}, {2, "1Mbps(normal)"},
				{3, "384kbps(low)"}, {4, "192kbps(super_low)"},
				{5, "音声のみ(audio_high)"}, {6, "6Mbps(6Mbps1080p30fps)"}
			};
			*/
			var ret = new List<int>();
			for (int i = 0; i < items.Count; i++) {
				foreach (KeyValuePair <int, string> p in itemsDic) {
					var itemName = util.getRegGroup(items[i].ToString(), " (.+)");
					if (p.Value == itemName) ret.Add(p.Key);
				}
			}
			return ret;
		}
		string getQualityRank() {
			var buf = getItemsToRanks(qualityListBox.Items);
			var ret = "";
			foreach (var r in buf) {
				if (ret != "") ret += ",";
				ret += r;
			}
			return ret;
		}
		void setInitQualityRankList(string qualityRank) {
			var ranks = new List<int>();
			foreach (var r in qualityRank.Split(','))
				ranks.Add(int.Parse(r));
//			ranks.AddRange(qualityRank.Split(','));
			
			qualityListBox.Items.Clear();
			var items = getRanksToItems(ranks.ToArray(), qualityListBox);
			qualityListBox.Items.AddRange(items);
		}
		
		void okBtn_Click(object sender, EventArgs e)
		{
			ret = getQualityRank();
			qualityStr = getQualityRankStr(ret);
			util.debugWriteLine(ret);
			
			Close();
		}
		string getQualityRankStr(string qualityRank) {
			var r = qualityRank.Split(',');
			for (var i = 0; i < r.Length; i++) {
				if (r[i] == "0") r[i] = "超高";
				else if (r[i] == "1") r[i] = "高";
				else if (r[i] == "2") r[i] = "中";
				else if (r[i] == "3") r[i] = "低";
				else if (r[i] == "4") r[i] = "超低";
				else if (r[i] == "5") r[i] = "音";
				else if (r[i] == "6") r[i] = "6M";
				else if (r[i] == "7") r[i] = "8M";
				else if (r[i] == "8") r[i] = "4M";
				else if (r[i] == "9") r[i] = "音";
				else {
					util.debugWriteLine(r[i] + " " + i);
					var ind = int.Parse(r[i]);
					if (config.config.qualityList.ContainsKey(ind))
						r[i] = config.config.qualityList[ind];
				}
			}
			return string.Join(",", r);
			/*
			return qualityRank//.Replace("0", "自")
				.Replace("0", "超高").Replace("1", "高")
				.Replace("2", "中").Replace("3", "低")
				.Replace("4", "超低").Replace("5", "音")
				.Replace("6", "6M");
			*/
		}
		
		void cancelBtn_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
