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
		public QualityForm(string qualityRank)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			setInitQualityRankList(qualityRank);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void highRankBtn_Click(object sender, EventArgs e)
		{
			int[] ranks = {1,2,3,4,5,0};
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks, qualityListBox));
		}
		void lowRankBtn_Click(object sender, EventArgs e)
		{
			int[] ranks = {5, 4, 3, 2, 1, 0};
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks, qualityListBox));
		}
		public object[] getRanksToItems(int[] ranks, ListBox owner) {
			var items = new Dictionary<int, string> {
				{0, "自動(abr)"}, {1, "3Mbps(super_high)"},
				{2, "2Mbps(high・高画質)"}, {3, "1Mbps(normal・低画質)"},
				{4, "384kbps(low)"}, {5, "192kbps(super_low)"},
			};
//			var ret = new ListBox.ObjectCollection(owner);
			var ret = new List<object>();
			for (int i = 0; i < ranks.Length; i++) {
				ret.Add((i + 1) + ". " + items[ranks[i]]);
			}
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
			if (selectedIndex > 4) return;
			
			var ranks = getItemsToRanks(qualityListBox.Items);
			var selectedVal = ranks[selectedIndex + 0];
			ranks.RemoveAt(selectedIndex);
			var addIndex = (selectedIndex == 5) ? 5 : (selectedIndex + 1);
			ranks.Insert(addIndex, selectedVal);
			
			qualityListBox.Items.Clear();
			qualityListBox.Items.AddRange(getRanksToItems(ranks.ToArray(), qualityListBox));
			qualityListBox.SetSelected(addIndex, true);
		}
		List<int> getItemsToRanks(ListBox.ObjectCollection items) {
			var itemsDic = new Dictionary<int, string> {
				{0, "自動(abr)"}, {1, "3Mbps(super_high)"},
				{2, "2Mbps(high・高画質)"}, {3, "1Mbps(normal・低画質)"},
				{4, "384kbps(low)"}, {5, "192kbps(super_low)"},
			};
			var ret = new List<int>();
			for (int i = 0; i < items.Count; i++) {
				foreach (KeyValuePair <int, string> p in itemsDic)
					if (p.Value.IndexOf(((string)items[i]).Substring(3)) > -1) ret.Add(p.Key);
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
			return qualityRank.Replace("0", "自")
				.Replace("1", "超高").Replace("2", "高")
				.Replace("3", "中").Replace("4", "低")
				.Replace("5", "超低");
		}
		
		void cancelBtn_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
