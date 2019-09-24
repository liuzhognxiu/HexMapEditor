using System;
using System.Collections.Generic;
using System.Linq;

namespace FY.Tools.ResCheck
{
	public class CheckBaseStringInt : CheckBase<KeyValuePair<string, int>, Dictionary<string, int>>
	{
		public override void Start()
		{
			base.Start();
			this.resultInfo = new Dictionary<string, int>();
			this.layout.Set(50f, 500f, 100f, 0f);
		}

		public override bool IsEmpty()
		{
			return this.resultInfo.Count == 0;
		}

		protected override void RemoveInfo(string key)
		{
			this.resultInfo.Remove(key);
		}

		protected override void DrawTableTitle()
		{
			base.DrawHorizontalInfo(new object[]
			{
				"ID",
				this.layout.x,
				"Asset",
				this.layout.y,
				"Count",
				this.layout.z
			});
		}

		protected override void DrawItem(int index, KeyValuePair<string, int> pair)
		{
			base.DrawHorizontalInfo(new object[]
			{
				index,
				this.layout.x,
				pair.Key,
				this.layout.y,
				pair.Value.ToString(),
				this.layout.z
			});
		}

		protected override string ItemToString(KeyValuePair<string, int> item)
		{
			return string.Format("{0},,{1}", item.Key, item.Value);
		}

		protected override void SortInfo()
		{
			this.resultInfo = (from i in this.resultInfo
			orderby i.Value descending
			select i).ToDictionary((KeyValuePair<string, int> i) => i.Key, (KeyValuePair<string, int> j) => j.Value);
		}
	}
}
