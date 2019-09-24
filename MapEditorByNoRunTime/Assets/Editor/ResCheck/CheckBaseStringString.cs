using System;
using System.Collections.Generic;
using System.Linq;

namespace FY.Tools.ResCheck
{
	public class CheckBaseStringString : CheckBase<KeyValuePair<string, string>, Dictionary<string, string>>
	{
		public override void Start()
		{
			base.Start();
			this.resultInfo = new Dictionary<string, string>();
			this.layout.Set(50f, 300f, 300f, 0f);
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
				"Info",
				this.layout.z
			});
		}

		protected override void DrawItem(int index, KeyValuePair<string, string> pair)
		{
			base.DrawHorizontalInfo(new object[]
			{
				index,
				this.layout.x,
				pair.Key,
				this.layout.y,
				pair.Value,
				this.layout.z
			});
		}

		protected override string ItemToString(KeyValuePair<string, string> item)
		{
			return string.Format("{0},,{1}", item.Key, item.Value);
		}

		protected override void SortInfo()
		{
			this.resultInfo = (from i in this.resultInfo
			orderby i.Key
			select i).ToDictionary((KeyValuePair<string, string> i) => i.Key, (KeyValuePair<string, string> j) => j.Value);
		}
	}
}
